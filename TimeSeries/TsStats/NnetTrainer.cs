#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HC.Analytics.Statistics;
using HC.Core;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public static class NnetTrainer
    {
        private const int MAX_ITERATIONS = 100;
        private const int RESTARTS = 3;
        private const int NUM_OUTPUT_NEURONS = 1;
        public const int NUM_HIDDEN_NEURONS = 6; // TODO, where is this magic number comming from??
        private const double DEFAULT_DECAY = 0.001;
        private const double WSTEP = 0.01;

        public static List<ASelfDescribingClass> TrainNnetSetClientForecast(
            SortedDictionary<DateTime, double[]> xMapTrain,
            SortedDictionary<DateTime, double> yMapTrain,
            int intEnsembleSize,
            int intNumClasses,
            double dblSampleSizeFactor,
            bool blnUseEarlyStop,
            bool blnBalancePositiveNegatives,
            int intThreads = 40)
        {
            try
            {
                List<alglib.multilayerperceptron> nnets = TrainNnetSet(
                    xMapTrain,
                    yMapTrain,
                    intEnsembleSize,
                    intNumClasses,
                    dblSampleSizeFactor,
                    blnUseEarlyStop,
                    blnBalancePositiveNegatives,
                    intThreads);

                List<ASelfDescribingClass> encodedResults = (from n in nnets select EncodeNnet(n)).ToList();

                return encodedResults;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static alglib.multilayerperceptron DecodeNnet(ASelfDescribingClass selfDescribingClass)
        {
            var basePerceptron = new alglib.mlpbase.multilayerperceptron();
            var propertiesMap = basePerceptron.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                                                                                              .ToDictionary(t => t.Name, t => t);

            var multilayerperceptron = new alglib.multilayerperceptron(
                basePerceptron);
            Dictionary<string, object> objValues = selfDescribingClass.GetObjValues();
            foreach (KeyValuePair<string, object> keyValuePair in objValues)
            {
                if (keyValuePair.Value != null)
                {
                    propertiesMap[keyValuePair.Key].SetValue(basePerceptron, keyValuePair.Value);
                }
            }
            return multilayerperceptron;
        }

        public static ASelfDescribingClass EncodeNnet(
            alglib.multilayerperceptron multilayerperceptron)
        {
            FieldInfo[] properties = multilayerperceptron
                .innerobj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var selfDescribingClass = new SelfDescribingClass();
            selfDescribingClass.SetClassName("multilayerperceptronTest");
            foreach (var propertyInfo in properties)
            {
                selfDescribingClass.SetObjValueToDict(
                    propertyInfo.Name,
                    propertyInfo.GetValue(multilayerperceptron.innerobj));
            }

            return selfDescribingClass;
        }

        public static List<alglib.multilayerperceptron> TrainNnetSet(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intEnsembleSize,
            int intNumClasses,
            double dblSampleSizeFactor,
            bool blnUseEarlyStop,
            int intThreads = 40)
        {
            return TrainNnetSet(
                xMapTrain,
                yMapTrain,
                intEnsembleSize,
                intNumClasses,
                dblSampleSizeFactor,
                blnUseEarlyStop,
                true,
                intThreads);
        }

        public static List<alglib.multilayerperceptron> TrainNnetSet(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intEnsembleSize,
            int intNumClasses,
            double dblSampleSizeFactor,
            bool blnUseEarlyStop,
            bool blnVerbose,
            int intThreads = 40,
            int intHiddenNeurons = NUM_HIDDEN_NEURONS)
        {
            if (xMapTrain.Count != yMapTrain.Count)
            {
                throw new HCException("Invalid map size");
            }

            return TrainNnetSet(
                xMapTrain,
                yMapTrain,
                intEnsembleSize,
                DEFAULT_DECAY,
                intNumClasses,
                dblSampleSizeFactor,
                blnUseEarlyStop,
                blnVerbose,
                false,
                intThreads,
                intHiddenNeurons);
        }

        public static List<alglib.multilayerperceptron> TrainNnetSet(
            SortedDictionary<DateTime, double[]> xMapTrain,
            SortedDictionary<DateTime, double> yMapTrain,
            int intEnsembleSize,
            int intNumClasses,
            double dblSampleSizeFactor,
            bool blnUseEarlyStop,
            bool blnBalancePositiveNegatives,
            int intThreads)
        {
            TimeSeriesHelper.CheckKeys(ref xMapTrain, ref yMapTrain);
            return TrainNnetSet(
                xMapTrain.Values.ToList(),
                yMapTrain.Values.ToList(),
                intEnsembleSize,
                DEFAULT_DECAY,
                intNumClasses,
                dblSampleSizeFactor,
                blnUseEarlyStop,
                blnBalancePositiveNegatives,
                intThreads);
        }

        public static List<alglib.multilayerperceptron> TrainNnetSet(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intEnsembleSize,
            double dblDecay,
            int intNumClasses,
            double dblSampleSizeFactor,
            bool blnUseEarlyStop,
            bool blnBalancePositiveNegatives,
            int intThreads)
        {
            return TrainNnetSet(
                xMapTrain,
                yMapTrain,
                intEnsembleSize,
                dblDecay,
                intNumClasses,
                dblSampleSizeFactor,
                blnUseEarlyStop,
                true,
                blnBalancePositiveNegatives,
                intThreads);
        }

        public static List<alglib.multilayerperceptron> TrainNnetSet(
            List<double[]> xMapTrain,
            List<double> yMapTrain,
            int intEnsembleSize,
            double dblDecay,
            int intNumClasses,
            double dblSampleSizeFactor,
            bool blnUseEarlyStop,
            bool blnVerbse,
            bool blnBalancePositiveNegatives,
            int intThreads,
            int intHiddenNeurons = NUM_HIDDEN_NEURONS)
        {
            try
            {
                if (xMapTrain.Count != yMapTrain.Count)
                {
                    throw new HCException("Invalid map size");
                }
                if(xMapTrain.Count == 0)
                {
                    return new List<alglib.multilayerperceptron>();
                }

                var classes = new List<double>();
                if(blnBalancePositiveNegatives && intNumClasses != 2)
                {
                    blnBalancePositiveNegatives = false;
                     Console.WriteLine("Balance negative/positives is only compatible with 2 classes");
                }
                if(blnBalancePositiveNegatives)
                {
                    classes = yMapTrain.Distinct().ToList();
                    if(classes.Count != 2)
                    {
                        throw new HCException("Invalid number of clsses");
                    }
                }

                
                int intSmallVars = xMapTrain.First().Length;
                var ensemble = new List<alglib.multilayerperceptron>();
                var lockObj = new object();
                int intPrc;
                int intCounter = 0;
                Parallel.For(0, intEnsembleSize,
                    new ParallelOptions { MaxDegreeOfParallelism = intThreads },
                    delegate(int j)
                //for (int j = 0; j < intEnsembleSize; j++)
                {
                    try
                    {
                        var logTime = DateTime.Now;

                        alglib.multilayerperceptron multilayerperceptron;
                        List<double[]> xSample;
                        List<double> ySample;
                        if (dblSampleSizeFactor > 0)
                        {
                            //
                            // sample with replacement. Even if sample size is 1, it is good to have different sample sets for each training instance
                            //
                            TimeSeriesHelper.SelectRandomSampleData(
                                xMapTrain,
                                yMapTrain,
                                dblSampleSizeFactor,
                                out xSample,
                                out ySample);
                        }
                        else
                        {
                            xSample = xMapTrain;
                            ySample = yMapTrain;
                        }

                        if(blnBalancePositiveNegatives)
                        {
                            TrainerHelper.BalancePostiveNegatives(
                                ySample ,
                                xSample ,
                                classes,
                                out ySample ,
                                out xSample );
                        }

                        String strMessage;
                        double[,] xy = TimeSeriesHelper.GetXy(ySample, xSample);
                        if (blnUseEarlyStop)
                        {
                            List<double[]> xVal;
                            List<double> yVal;
                            TimeSeriesHelper.SelectRandomSampleData(
                                xMapTrain,
                                yMapTrain,
                                1 - dblSampleSizeFactor,
                                out xVal,
                                out yVal);
                            double[,] xyVal = TimeSeriesHelper.GetXy(yVal, xVal);

                            if (intNumClasses <= 1)
                            {
                                if (blnVerbse)
                                {
                                    strMessage = "Training [" + (j + 1) + "]/[" +
                                                 intEnsembleSize + "] early stop regresssion nnet with sample factor [" +
                                                 dblSampleSizeFactor + "][" + xMapTrain.Count + "] samples [" +
                                                 intHiddenNeurons + "] hneurons [" +
                                                 intThreads + "] threads...";
                                    Verboser.WriteLine(strMessage);
                                    Logger.Log(strMessage);
                                }
                                multilayerperceptron =
                                    TrainNnForecastEarlyStop(intSmallVars,
                                                             xy,
                                                             xyVal,
                                                             dblDecay,
                                                             intHiddenNeurons);
                            }
                            else
                            {
                                if (blnVerbse)
                                {
                                    strMessage = "Classification training [" + (j+1) + "]/[" +
                                                 intEnsembleSize +
                                                 "] early stop nnet for classification with sample factor [" +
                                                 dblSampleSizeFactor + "][" + xMapTrain.Count + "] samples ["+
                                                 intNumClasses + "]classes...";
                                    Verboser.WriteLine(strMessage);
                                    Logger.Log(strMessage);
                                }
                                multilayerperceptron =
                                    TrainNnClassificationEarlyStop(
                                        intSmallVars,
                                        intNumClasses,
                                        xy,
                                        xyVal,
                                        dblDecay);
                            }
                        }
                        else
                        {
                            if (blnVerbse)
                            {
                                strMessage = "Training [" + (j + 1) + "]/[" +
                                             intEnsembleSize + "] nnet with sample factor [" + 
                                             dblSampleSizeFactor +
                                             "][" + xMapTrain.Count + "] samples [" +
                                             intNumClasses + "] class [" +
                                                 intHiddenNeurons + "] neurons [" +
                                                 intThreads + "] threads...";
                                Verboser.WriteLine(strMessage);
                                Logger.Log(strMessage);
                            }
                            if (intNumClasses <= 1)
                            {
                                multilayerperceptron =
                                    TrainNnForecast(intSmallVars,
                                                    xy,
                                                    dblDecay);
                            }
                            else
                            {
                                if (blnVerbse)
                                {
                                    strMessage = "Classification training [" + (j + 1) + "]/[" +
                                                 intEnsembleSize + "] nnet for classification with sample factor [" +
                                                 dblSampleSizeFactor + "][" + xMapTrain.Count + "] samples [" +
                                                 intNumClasses + "] classes...";
                                    Verboser.WriteLine(strMessage);
                                    Logger.Log(strMessage);
                                }
                                multilayerperceptron =
                                    TrainNnClassification(
                                        intSmallVars,
                                        intNumClasses,
                                        xy,
                                        dblDecay);
                            }
                        }
                        lock (lockObj)
                        {
                            ensemble.Add(multilayerperceptron);
                            intPrc = (intCounter++*100)/intEnsembleSize;
                        }
                        if (blnVerbse)
                        {
                            Verboser.WriteLine("Finish training nnet. Time (secs) [" +
                                              (DateTime.Now - logTime).TotalSeconds + "][" +
                                              intPrc + "%][" + xMapTrain.Count + "] samples");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                });
                return ensemble;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<alglib.multilayerperceptron>();
        }


        private static alglib.multilayerperceptron TrainNnForecastEarlyStop(
            int intVarCount, 
            double[,] xyTrain, 
            double[,] xyVal, 
            double dblDecay,
            int intHiddenNeurons = NUM_HIDDEN_NEURONS)
        {
            try
            {
                alglib.multilayerperceptron multilayerperceptron;
                alglib.mlpcreate1(
                    intVarCount,
                    intHiddenNeurons,
                    NUM_OUTPUT_NEURONS,
                    out multilayerperceptron);
                int intInfo;
                alglib.mlpreport report;

                alglib.mlptraines(
                    multilayerperceptron,
                    xyTrain,
                    xyTrain.GetLength(0),
                    xyVal,
                    xyVal.GetLength(0),
                    dblDecay,
                    RESTARTS,
                    out intInfo,
                    out report);

                if (intInfo != 6)
                {
                    throw new Exception("Invalid nnet training");
                }
                return multilayerperceptron;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<alglib.mlpensemble> TrainNnetEarlyStopEnsembleSet(
            SortedDictionary<DateTime, double[]> xMapTrain,
            SortedDictionary<DateTime, double> yMapTrain,
            int intEnsembleSize)
        {
            try
            {
                double[,] xy = GetXy(yMapTrain, xMapTrain);
                int intSmallVars = xMapTrain.Values.First().Length;
                List<alglib.mlpensemble> ensemble = TrainNnetEarlyStopEnsembleSet(intSmallVars, intEnsembleSize, xy);
                return ensemble;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private static List<alglib.mlpensemble> TrainNnetEarlyStopEnsembleSet(
            int intSmallVars,
            int intEnsembleSize,
            double[,] xy)
        {
            var ensemble = new List<alglib.mlpensemble>();
            var logTime = DateTime.Now;
            Verboser.WriteLine("Training early stop ensemble nnet...");
            alglib.mlpensemble multilayerperceptron =
                TrainNnEarlyStopEnsemble(intSmallVars, xy, intEnsembleSize);
            ensemble.Add(multilayerperceptron);
            Verboser.WriteLine(
                "Finish training early stop ensemble nnet. Time (secs) [" +
                (DateTime.Now - logTime).TotalSeconds + "][" + xy.GetLength(0) + "] samples");
            return ensemble;
        }

        private static alglib.mlpensemble TrainNnEarlyStopEnsemble(
            int intVarCount,
            double[,] xy,
            int intEnsembleSize)
        {
            try
            {
                alglib.mlpensemble multilayerperceptron;
                alglib.mlpecreate1(
                    intVarCount,
                    NUM_HIDDEN_NEURONS, 
                    1,
                    intEnsembleSize,
                    out multilayerperceptron);
                int intInfo;
                alglib.mlpreport report;

                alglib.mlpetraines(
                    multilayerperceptron,
                    xy,
                    xy.GetLength(0),
                    DEFAULT_DECAY,
                    RESTARTS,
                    out intInfo,
                    out report);

                if (intInfo != 6)
                {
                    throw new Exception("Invalid training");
                }
                return multilayerperceptron;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private static alglib.multilayerperceptron TrainNnClassificationEarlyStop(
            int intVarCount,
            int intNumClasses,
            double[,] xyTrain,
            double[,] xyVal,
            double dblDecay)
        {
            try
            {
                alglib.multilayerperceptron multilayerperceptron;
                alglib.mlpcreatec1(
                    intVarCount,
                    NUM_HIDDEN_NEURONS,
                    intNumClasses,
                    out multilayerperceptron);

                int intInfo;
                alglib.mlpreport report;

                alglib.mlptraines(
                    multilayerperceptron,
                    xyTrain,
                    xyTrain.GetLength(0),
                    xyVal,
                    xyVal.GetLength(0),
                    dblDecay,
                    RESTARTS,
                    out intInfo,
                    out report);

                if (intInfo != 6)
                {
                    throw new Exception("Invalid nnet training");
                }
                return multilayerperceptron;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new alglib.multilayerperceptron();
        }

        private static alglib.multilayerperceptron TrainNnClassification(
            int intVarCount,
            int intNumClasses,
            double[,] xy,
            double dblDecay)
        {
            try
            {
                alglib.multilayerperceptron multilayerperceptron;
                alglib.mlpcreatec1(
                    intVarCount,
                    NUM_HIDDEN_NEURONS,
                    intNumClasses,
                    out multilayerperceptron);

                int intInfo;
                alglib.mlpreport report;

                alglib.mlptrainlbfgs(
                    multilayerperceptron,
                    xy,
                    xy.GetLength(0),
                    dblDecay,
                    RESTARTS,
                    WSTEP,
                    MAX_ITERATIONS,
                    out intInfo,
                    out report);

                if (intInfo != 2)
                {
                    throw new Exception("Invalid training");
                }
                return multilayerperceptron;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new alglib.multilayerperceptron();
        }

        private static alglib.multilayerperceptron TrainNnForecast(
            int intVarCount,
            double[,] xy,
            double dblDecay)
        {
            try
            {
                alglib.multilayerperceptron multilayerperceptron;
                alglib.mlpcreate1(
                    intVarCount,
                    NUM_HIDDEN_NEURONS,
                    NUM_OUTPUT_NEURONS,
                    out multilayerperceptron);
                int intInfo;
                alglib.mlpreport report;

                alglib.mlptrainlbfgs(
                    multilayerperceptron,
                    xy,
                    xy.GetLength(0),
                    dblDecay,
                    RESTARTS,
                    WSTEP,
                    MAX_ITERATIONS,
                    out intInfo,
                    out report);

                if (intInfo != 2)
                {
                    throw new Exception("Invalid nnet training");
                }
                return multilayerperceptron;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private static double[,] GetXy(
            SortedDictionary<DateTime, double> yMap,
            SortedDictionary<DateTime, double[]> xMap)
        {
            try
            {
                if (xMap.Count != yMap.Count)
                {
                    throw new HCException("Invalid map size");
                }
                if (xMap.Keys.First() != yMap.Keys.First())
                {
                    throw new HCException("Invalid map dates");
                }

                int intVars = xMap.First().Value.Length;
                int intN = xMap.Count;
                var outArr = new double[intN,intVars + 1];
                var xMapArr = xMap.ToArray();
                var yMapArr = yMap.ToArray();
                for (int i = 0; i < intN; i++)
                {
                    for (int j = 0; j < intVars; j++)
                    {
                        outArr[i, j] = xMapArr[i].Value[j];
                    }
                    outArr[i, intVars] = yMapArr[i].Value;
                }
                return outArr;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<double> GetPredictionsEarlyStopEnsemble(
            double[] featureToForecast,
            List<alglib.mlpensemble> ensemble)
        {
            try
            {
                int intEnsembleSize = ensemble.Count;
                var predictions = new List<double>();
                for (int i = 0; i < intEnsembleSize; i++)
                {
                    var yPrediction = new double[1];
                    alglib.mlpeprocess(ensemble[i], featureToForecast, ref yPrediction);
                    double dblPrediction = yPrediction[0];
                    predictions.Add(dblPrediction);
                }

                double dblStdDev = StdDev.GetSampleStdDev(predictions);
                double dblMean = predictions.Average();
                var predictionCheck = new List<double>();
                foreach (double dblPrediction0 in predictions)
                {
                    double dblPrediction = dblPrediction0;
                    double dblUpperLimit = dblMean + 7.0*dblStdDev;
                    double dblLowerLimit = Math.Max(0, dblMean - 7.0*dblStdDev);
                    if (dblPrediction > dblUpperLimit)
                    {
                        dblPrediction = dblUpperLimit;
                    }
                    else if (dblPrediction < dblLowerLimit)
                    {
                        dblPrediction = dblLowerLimit;
                    }
                    predictionCheck.Add(dblPrediction);
                }
                return predictionCheck;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<int> GetNnetClassificationsEnsemble(
            int intNumClasses,
            double[] featureToForecast,
            List<alglib.multilayerperceptron> ensemble)
        {
            int intEnsembleSize = ensemble.Count;
            var predictions = new List<int>();
            for (int i = 0; i < intEnsembleSize; i++)
            {
                var yPrediction = new double[intNumClasses];
                alglib.mlpprocess(ensemble[i], featureToForecast, ref yPrediction);
                int intClass = TimeSeriesHelper.GetClass(yPrediction);
                predictions.Add(intClass);
            }
            return predictions;
        }


        public static List<double> GetNnetPredictionsEnsemble(
            double[] featureToForecast,
            List<alglib.multilayerperceptron> ensemble)
        {
            try
            {
                int intEnsembleSize = ensemble.Count;
                var predictions = new List<double>();
                for (int i = 0; i < intEnsembleSize; i++)
                {
                    var yPrediction = new double[1];
                    alglib.mlpprocess(ensemble[i], featureToForecast, ref yPrediction);
                    double dblPrediction = yPrediction[0];
                    predictions.Add(dblPrediction);
                }

                List<double> predictionCheck = TimeSeriesHelper.CheckOutlayersInPredictions(predictions);
                return predictionCheck;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<alglib.mlpensemble> TrainNnetEarlyStopEnsembleSet(
            List<double[]> xMapTrain, 
            List<double> yMapTrain, 
            int intEnsembleSize)
        {
            double[,] xy = TimeSeriesHelper.GetXy(yMapTrain, xMapTrain);
            int intSmallVars = xMapTrain.First().Length;
            List<alglib.mlpensemble> ensemble = TrainNnetEarlyStopEnsembleSet(
                intSmallVars, 
                intEnsembleSize, 
                xy);
            return ensemble;
        }
    }
}