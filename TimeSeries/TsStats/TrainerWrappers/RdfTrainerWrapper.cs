using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HC.Core;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public class RdfTrainerWrapper : ATrainerWrapper
    {
        private List<alglib.decisionforest> m_forest;
        private const int BATCH_SIZE = 10;
        private const int NTREES = 70; // value between 50 and 100
        private const double R_PARAM = 0.3; // percent of a training set, TODO calibrate this parameter
        public const int ENSEMBLE_SIZE_RDF = 3;

        public RdfTrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intNumClasses,
            int intEnsembleSize = ENSEMBLE_SIZE_RDF) :
            base(
            xData,
            yData,
            intNumClasses)
        {
            m_intEnsembleSize = intEnsembleSize;
            DoTraining();
        }

        private void DoTraining()
        {
            m_forest = TrainEnsemble(
                m_xData,
                m_yData,
                m_intEnsembleSize,
                m_intNumClasses);
        }

        public static alglib.decisionforest Train(
            SortedDictionary<DateTime, double[]> xMapTrain,
            SortedDictionary<DateTime, double> yMapTrain,
            int intNumClasses,
            double[] weights = null)
        {
            return Train(
                xMapTrain.Values.ToList(),
                yMapTrain.Values.ToList(),
                intNumClasses,
                weights: weights);
        }

        public static double GetPrediction(
            double[] feature,
            alglib.decisionforest treeResultReturn)
        {
            try
            {
                int intClasses =
                    Math.Max(
                        1,
                        treeResultReturn.innerobj.nclasses);
                var y = new double[
                    intClasses];
                alglib.dfprocess(
                    treeResultReturn, 
                    feature, 
                    ref y);
                if (intClasses > 1)
                {
                    return TimeSeriesHelper.GetClass(y);
                }
                double dblForecast = y[0];
                if (double.IsNaN(dblForecast) ||
                    double.IsInfinity(dblForecast))
                {
                    throw new HCException("Invalid forecast");
                }
                return dblForecast;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static alglib.decisionforest Train(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intNumClasses,
            double dblR = R_PARAM,
            double[] weights = null)
        {
            try
            {
                if (xMapTrain.Count != yMapTrain.Count)
                {
                    throw new HCException("Invalid map size");
                }
                double[,] xy = TimeSeriesHelper.GetXy(
                    yMapTrain,
                    xMapTrain);
                return Train(intNumClasses, xy);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static alglib.decisionforest Train(int intNumClasses, double[,] xy)
        {
            alglib.decisionforest df;
            alglib.dfreport rep;
            int intInfo;
            alglib.dfbuildrandomdecisionforest(
                xy,
                xy.GetLength(0),
                xy.GetLength(1) - 1,
                Math.Max(1, intNumClasses), // make sure this is not given zero for regression
                NTREES,
                intNumClasses > 1 ? 0.1 : 0.55,
                out intInfo,
                out df,
                out rep);

            if (intInfo != 1)
            {
                throw new HCException("Invalid tree train result [" +
                                      intInfo + "]");
            }
            return df;
        }

        public static List<alglib.decisionforest> TrainForecastEnsemble<T>(
            SortedDictionary<T, double[]> xMapTrain,
            SortedDictionary<T, double> yMapTrain,
            int intEnsembleSize)
        {
            return TrainForecastEnsemble(
                xMapTrain,
                yMapTrain,
                intEnsembleSize,
                R_PARAM);
        }

        public static List<alglib.decisionforest> TrainForecastEnsemble<T>(
            SortedDictionary<T, double[]> xMapTrain,
            SortedDictionary<T, double> yMapTrain,
            int intEnsembleSize,
            double dblR)
        {
            try
            {
                if (xMapTrain.Count != yMapTrain.Count)
                {
                    throw new HCException("Invalid map size");
                }
                int intNumBatches = intEnsembleSize / BATCH_SIZE;
                if (intNumBatches == 0)
                {
                    List<alglib.decisionforest> forestEnsemble =
                        GetForestEnsembleForecasting(
                        xMapTrain,
                        yMapTrain,
                        intEnsembleSize,
                        dblR);
                    return forestEnsemble;
                }
                var batchArr = new List<alglib.decisionforest>[intNumBatches];
                var allForestEnsemble = new List<alglib.decisionforest>();
                Parallel.For(0, intNumBatches, delegate(int i)
                {
                    List<alglib.decisionforest> currForestEnsemble =
                        GetForestEnsembleForecasting(xMapTrain,
                                          yMapTrain,
                                          BATCH_SIZE,
                                          dblR);
                    batchArr[i] = currForestEnsemble;
                });
                for (int i = 0; i < intNumBatches; i++)
                {
                    allForestEnsemble.AddRange(batchArr[i]);
                }
                int intLeft = intEnsembleSize - intNumBatches * BATCH_SIZE;
                List<alglib.decisionforest> finalForestEnsemble =
                    GetForestEnsembleForecasting(xMapTrain,
                                      yMapTrain,
                                      intLeft,
                                      dblR);
                allForestEnsemble.AddRange(finalForestEnsemble);
                return allForestEnsemble;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<alglib.decisionforest>();
        }

        private static List<alglib.decisionforest> GetForestEnsembleForecasting<T>(
            SortedDictionary<T, double[]> xMapTrain,
            SortedDictionary<T, double> yMapTrain,
            int intEnsembleSize,
            double dblR,
            double[] weights = null)
        {
            var forestEnsemble = new List<alglib.decisionforest>(intEnsembleSize + 1);
            for (int i = 0; i < intEnsembleSize; i++)
            {
                forestEnsemble.Add(
                    TrainForecast(xMapTrain,
                                  yMapTrain,
                                  dblR,
                                  weights));
            }
            return forestEnsemble;
        }

        private static List<alglib.decisionforest> TrainForestEnsemble(
             double[,] xy,
            int intEnsembleSize,
            int intClasses,
            double dblR,
            double[] weights = null)
        {
            var forestEnsemble = new List<alglib.decisionforest>(intEnsembleSize + 1);
            for (int i = 0; i < intEnsembleSize; i++)
            {
                forestEnsemble.Add(
                    Train(
                        intClasses,
                        xy));
            }
            return forestEnsemble;
        }

        public static alglib.decisionforest TrainForecast<T>(
            SortedDictionary<T, double[]> xMapTrain,
            SortedDictionary<T, double> yMapTrain,
            double dblR,
            double[] weights)
        {
            try
            {
                if (xMapTrain.Count != yMapTrain.Count)
                {
                    throw new HCException("Invalid map size");
                }
                const int nclasses = 1; // one means regression
                double[,] xy = TimeSeriesHelper.GetXy(
                    yMapTrain.Values.ToList(),
                    xMapTrain.Values.ToList());
                alglib.decisionforest df;
                alglib.dfreport rep;
                int info;
                alglib.dfbuildrandomdecisionforest(
                    xy,
                    xy.GetLength(0),
                    xy.GetLength(1) - 1,
                    nclasses,
                    NTREES,
                    dblR,
                    out info,
                    out df,
                    out rep);
                return df;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<alglib.decisionforest> TrainEnsemble(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intEnsembleSize,
            int intClasses,
            double[] weights = null,
            bool blnVerbose = true)
        {
            return TrainEnsemble(
                xMapTrain,
                yMapTrain,
                intEnsembleSize,
                intClasses,
                R_PARAM,
                weights,
                blnVerbose);
        }

        public static List<alglib.decisionforest> TrainEnsemble(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intEnsembleSize,
            int intClasses,
            double dblR,
            double[] weights,
            bool blnVerbose = true)
        {
            try
            {
                if (xMapTrain.Count != yMapTrain.Count)
                {
                    throw new HCException("Invalid map size");
                }
                int intNumBatches = intEnsembleSize / BATCH_SIZE;
                if (blnVerbose)
                {
                    Verboser.WriteLine("Training RDF [" +
                        intClasses + "] classes [" +
                        xMapTrain.Count + "] samples [" +
                        intNumBatches + "] batches...");
                }
                double[,] xy = TimeSeriesHelper.GetXy(
                   yMapTrain,
                   xMapTrain);
                if (intNumBatches == 0)
                {
                    List<alglib.decisionforest> forestEnsemble =
                        TrainForestEnsemble(
                        xy,
                        intEnsembleSize,
                        intClasses,
                        dblR,
                        weights);
                    return forestEnsemble;
                }
                var batchArr = new List<alglib.decisionforest>[intNumBatches];
                var allForestEnsemble = new List<alglib.decisionforest>();
               // Parallel.For(0, intNumBatches, delegate(int i)
                for (int i = 0; i < intNumBatches; i++)
                {
                    List<alglib.decisionforest> currForestEnsemble =
                        TrainForestEnsemble( xy,
                                          BATCH_SIZE,
                                          intClasses,
                                          dblR,
                                          weights);
                    batchArr[i] = currForestEnsemble;
                    if (blnVerbose)
                    {
                        Verboser.Talk("Finish RDF batch [" +
                                      i + "] of [" +
                                      intNumBatches + "][" +
                                      intClasses + "] classes");
                    }
                }
                for (int i = 0; i < intNumBatches; i++)
                {
                    allForestEnsemble.AddRange(batchArr[i]);
                }
                int intLeft = intEnsembleSize - intNumBatches * BATCH_SIZE;
                List<alglib.decisionforest> finalForestEnsemble =
                    TrainForestEnsemble(xy,
                                      intLeft,
                                      intClasses,
                                      dblR,
                                      weights);
                allForestEnsemble.AddRange(finalForestEnsemble);
                if (blnVerbose)
                {
                    Verboser.WriteLine("Finish training RDF [" +
                        intClasses + "] classes");
                }
                return allForestEnsemble;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<alglib.decisionforest>();
        }

        public static List<double> GetPredictions(
            List<double[]> xData,
            List<alglib.decisionforest> ensemble,
            int intClases)
        {
            var predictions = new List<double>();
            for (int i = 0; i < xData.Count; i++)
            {
                predictions.Add(ForecastStatic(
                    xData[i],
                    intClases,
                    ensemble));
            }
            return predictions;
        }

        public static List<double> GetPredictionsEnsemble(
            double[] feature,
            List<alglib.decisionforest> ensemble)
        {
            try
            {
                int intEnsembleSize = ensemble.Count;
                var predictions = new List<double>(intEnsembleSize + 2);
                for (int i = 0; i < intEnsembleSize; i++)
                {
                    double dblPrediction = GetPrediction(
                        feature, 
                        ensemble[i]);
                    predictions.Add(dblPrediction);
                }
                int intClasses = ensemble[0].innerobj.nclasses;
                if (intClasses <= 1)
                {
                    predictions =
                        TimeSeriesHelper.CheckOutlayersInPredictions(
                            predictions);
                }
                return predictions;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static int GetClass(
            double[] feature,
            int intNumClasses,
            alglib.decisionforest treeResultCassification)
        {
            try
            {
                var yClassify = new double[intNumClasses];
                alglib.dfprocess(treeResultCassification, feature, ref yClassify);
                int intClass = TimeSeriesHelper.GetClass(
                    yClassify);

                if (intClass < 0)
                {
                    throw new HCException("negative class");
                }
                return intClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }


        public static List<int> GetClassificationsEnsemble(
            int intNumClasses,
            double[] feature,
            List<alglib.decisionforest> ensemble)
        {
            try
            {
                int intEnsembleSize = ensemble.Count;
                var predictions = new List<int>();
                for (int i = 0; i < intEnsembleSize; i++)
                {
                    int intClass = GetClass(feature, intNumClasses, ensemble[i]);
                    predictions.Add(intClass);
                }
                return predictions;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<int>();
        }

        public static alglib.decisionforest DecodeRdf(
            ASelfDescribingClass selfDescribingClass)
        {
            var baseForest = new alglib.dforest.decisionforest();
            var propertiesMap = baseForest.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(t => t.Name, t => t);

            var forest = new alglib.decisionforest(
                baseForest);
            Dictionary<string, object> objValues = selfDescribingClass.GetObjValues();
            foreach (KeyValuePair<string, object> keyValuePair in objValues)
            {
                if (keyValuePair.Value != null)
                {
                    propertiesMap[keyValuePair.Key].SetValue(baseForest, keyValuePair.Value);
                }
            }
            return forest;
        }

        public static ASelfDescribingClass EncodeRdf(
            alglib.decisionforest forest)
        {
            FieldInfo[] properties = forest.innerobj.GetType().GetFields(
                BindingFlags.Public | BindingFlags.Instance);
            var selfDescribingClass = new SelfDescribingClass();
            selfDescribingClass.SetClassName("forestTest");
            foreach (var propertyInfo in properties)
            {
                selfDescribingClass.SetObjValueToDict(
                    propertyInfo.Name,
                    propertyInfo.GetValue(forest.innerobj));
            }

            return selfDescribingClass;
        }


        public override double Forecast(double[] feature)
        {
            try
            {
                return ForecastStatic(
                    feature,
                    m_intNumClasses,
                    m_forest);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static double ForecastStatic(
            double[] feature,
            int intNumClasses,
            List<alglib.decisionforest> forest)
        {
            try
            {
                if (intNumClasses > 1)
                {
                    var votes = GetClassificationsEnsemble(
                        intNumClasses,
                        feature,
                        forest);
                    return TimeSeriesHelper.GetMostVotedClass(votes);
                }
                return GetPredictionsEnsemble(
                    feature,
                    forest).Average();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static double GetMrseStatic(
            List<double[]> xData,
            List<double> yData,
            List<alglib.decisionforest> treeResult,
            out double[] errors)
        {
            errors = null;
            try
            {
                errors = new double[xData.Count];
                for (int i = 0; i < xData.Count; i++)
                {
                    double dblPrediction =
                        GetPredictionsEnsemble(xData[i], treeResult).Average();
                    errors[i] = dblPrediction - yData[i];
                }
                return TimeSeriesHelper.GetMrse(errors);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}
