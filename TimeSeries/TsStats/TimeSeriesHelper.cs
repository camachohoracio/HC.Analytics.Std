using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.Analytics.Analysis;
using HC.Analytics.Probability.Random;
using HC.Analytics.Statistics;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Time;
using HC.Core.Logging;


namespace HC.Analytics.TimeSeries.TsStats
{
    public static class TimeSeriesHelper
    {
        private const double STD_DEV_OUTLAYER_FACTOR = 7.0;

        private const int MAX_FORECAST_DAYS = 10;

        private static readonly RngWrapper m_rng = new RngWrapper();

        public static readonly double[] FIBO_LEVELS = new []
        {
            0.236,
            0.382,
            0.50,
            0.618,
            1.0
        };

        public static readonly double[] FIBO_LEVELS_TAKE_PROFIT = new[]
        {
            -1.618,
            -2.0,
            -2.618
        };

        public static readonly double[] FIBO_LEVELS_RETRACEMENTS = new[]
        {
            -1.382,
            -1.0,
            -0.764,
            -0.618,
            -0.50,
            -0.382,
            -0.236,
            0.0,
            0.236,
            0.382,
            0.50,
            0.618,
            0.764,
            1.0,
            1.382
        };

        public static readonly double[] FIBO_LEVELS_EXTENSIONS = new[]
        {
            0.236,
            0.382,
            0.50,
            0.618,
            1.0,
            1.382,
            1.618,
            2.0,
            2.618
        };

        public static List<HashSet<string>> SplitSets(
            HashSet<string> set,
            int intSize)
        {
            var result =
                new List<HashSet<string>>();
            try
            {
                if (set.Count <= intSize)
                {
                    result.Add(set);
                    return result;
                }
                var currSet = new HashSet<string>();
                foreach (string strItem in set)
                {
                    currSet.Add(strItem);
                    if (currSet.Count >= intSize)
                    {
                        result.Add(currSet);
                        currSet = new HashSet<string>();
                    }
                }
                if (currSet.Count > 0)
                {
                    result.Add(currSet);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return result;
        }

        public static void FibonacciRetracement(
            double dblMin,
            double dblMax,
            out double[] inLevels,
            out double[] outLevelsUp,
            out double[] outLevelsDown)
        {
            inLevels = new double[FIBO_LEVELS.Length];
            outLevelsUp = new double[FIBO_LEVELS.Length];
            outLevelsDown = new double[FIBO_LEVELS.Length];
            double dblDiff = dblMax - dblMin;
            for (int i = 0; i < FIBO_LEVELS.Length; i++)
            {
                double dblLevel = dblDiff * FIBO_LEVELS[i];
                inLevels[i] = dblMin + dblLevel;
                outLevelsUp[i] = dblMax + dblLevel;
                outLevelsDown[i] = dblMin - dblLevel;
            }
        }

        public static void GetInOutOfSampleMapsClassi<T>(
                    SortedDictionary<T, double> yMapOri0,
                    SortedDictionary<T, double[]> xMapOri0,
                    SortedDictionary<T, double> yMapClassify0,
                    double dblOutOfSampleFactor,
                    out SortedDictionary<T, double[]> xMapOriOutOfSample,
                    out SortedDictionary<T, double[]> xMapOriInSample,
                    out SortedDictionary<T, double> yMapOriOutOfSample,
                    out SortedDictionary<T, double> yMapOriInSample,
                    out SortedDictionary<T, double> yMapClassifyOutOfSample,
                    out SortedDictionary<T, double> yMapClassifyInSample)
        {
            xMapOriOutOfSample = null;
            xMapOriInSample = null;
            yMapOriOutOfSample = null;
            yMapOriInSample = null;
            yMapClassifyOutOfSample = null;
            yMapClassifyInSample = null;
            try
            {
                List<T> keysInSample = yMapOri0.Keys.ToList();
                new RngWrapper().Shuffle(keysInSample);
                keysInSample = TakeFirst(keysInSample,
                                         (int)(keysInSample.Count * dblOutOfSampleFactor));
                GetInOutOfSample(
                    keysInSample,
                    xMapOri0,
                    out xMapOriInSample,
                    out xMapOriOutOfSample);
                GetInOutOfSample(
                    keysInSample,
                    yMapOri0,
                    out yMapOriInSample,
                    out yMapOriOutOfSample);
                GetInOutOfSample(
                    keysInSample,
                    yMapClassify0,
                    out yMapClassifyInSample,
                    out yMapClassifyOutOfSample);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        /// <summary>
        /// 0=Short,1=nothing,2=Long
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="yMap"></param>
        /// <param name="classiCuts"></param>
        /// <returns></returns>
        public static SortedDictionary<T, double> GetThreeClassesClassiMap<T>(
            SortedDictionary<T, double[]> yMap,
            out double[] classiCuts)
        {
            classiCuts = new double[2];
            try
            {
                // 0 = open, 1 = close, 2 = low, 3 = high
                var yMapReturns = new SortedDictionary<T, double>();
                foreach (var kvp in yMap)
                {
                    double dblHighDiff = 10000.0*(kvp.Value[3] - kvp.Value[0])/ kvp.Value[0];
                    double dblLowDiff = 10000.0 * (kvp.Value[0] - kvp.Value[2]) / kvp.Value[0];
                    if (dblHighDiff > dblLowDiff)
                    {
                        yMapReturns[kvp.Key] = (dblHighDiff);
                    }
                    else
                    {
                        yMapReturns[kvp.Key] = (-dblLowDiff);
                    }
                }
                var neg = (from n in yMapReturns
                           where n.Value <= 0
                           select n).ToList();
                var pos = (from n in yMapReturns
                           where n.Value > 0
                           select n).ToList();
                neg.Sort((a, b) => a.Value.CompareTo(b.Value));
                pos.Sort((a, b) => a.Value.CompareTo(b.Value));

                //
                // doing negative returns
                //
                int intIndexNeg = neg.Count * 1 / 2;
                var classMap = new SortedDictionary<T, double>();
                for (int i = 0; i < neg.Count; i++)
                {
                    if (i < intIndexNeg)
                    {
                        classMap[neg[i].Key] = 0;
                    }
                    else
                    {
                        classMap[neg[i].Key] = 1;
                    }
                }

                //
                // doing possitive returns
                //
                int intIndexPos = pos.Count * 1 / 2;
                for (int i = 0; i < pos.Count; i++)
                {
                    if (i < intIndexPos)
                    {
                        classMap[pos[i].Key] = 1;
                    }
                    else
                    {
                        classMap[pos[i].Key] = 2;
                    }
                }
                classiCuts[0] = neg[intIndexNeg].Value;
                classiCuts[1] = pos[intIndexNeg].Value;
                return classMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static SortedDictionary<T, double> GetThreeClassesClassiMap0<T>(
            SortedDictionary<T, double> yMap,
            out double[] classiCuts)
        {
            classiCuts = new double[2];
            try
            {
                var yMapReturns = GetReturns(
                    yMap);
                var classMap = GetThreeClassesClassiMap00(
                    out classiCuts, yMapReturns);
                return classMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static SortedDictionary<T, double> GetThreeClassesClassiMap00<T>(
            out double[] classiCuts, 
            SortedDictionary<T, double> yMap,
            int intReturnPeriods = 0,
            double dblRatioClass =2.0/3.0)
        {
            classiCuts = new double[2];
            try
            {
                SortedDictionary<T, double> yMapReturns;
                if (intReturnPeriods > 0)
                {
                    yMapReturns = GetReturns(
                        yMap,
                        intReturnPeriods,
                        true);
                }
                else
                {
                    yMapReturns =
                        new SortedDictionary<T, double>(
                            CloneMap(yMap));
                }

                var neg = (from n in yMapReturns
                    where n.Value <= 0
                    select n).ToList();
                var pos = (from n in yMapReturns
                    where n.Value > 0
                    select n).ToList();
                neg.Sort((a, b) => 
                    a.Value.CompareTo(b.Value));
                pos.Sort((a, b) => 
                    a.Value.CompareTo(b.Value));

                //
                // doing negative returns
                //
                int intIndexNeg = (int)(neg.Count*dblRatioClass);
                var classMap = new SortedDictionary<T, double>();
                for (int i = 0; i < neg.Count; i++)
                {
                    if (i < intIndexNeg)
                    {
                        classMap[neg[i].Key] = 0;
                    }
                    else
                    {
                        classMap[neg[i].Key] = 1;
                    }
                }

                //
                // doing possitive returns
                //
                int intIndexPos =(int) (pos.Count*(1.0 - dblRatioClass));
                for (int i = 0; i < pos.Count; i++)
                {
                    if (i < intIndexPos)
                    {
                        classMap[pos[i].Key] = 1;
                    }
                    else
                    {
                        classMap[pos[i].Key] = 2;
                    }
                }

                classiCuts[0] = neg[intIndexNeg].Value;
                classiCuts[1] = pos[intIndexPos].Value;
                return classMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static void GetInOutOfSample<T, U>(
            SortedDictionary<T, U> xMap,
            double dblInSamplePrportion,
            out List<T> keysInSample,
            out SortedDictionary<T, U> xMapInSample,
            out SortedDictionary<T, U> xMapOutOfSample)
        {

            RngWrapper rng = new RngWrapper();
            var rngKeys = xMap.Keys.ToList();
            rng.ShuffleList(
                rngKeys);
            int intSampleSize =
                (int)Math.Min(
                    rngKeys.Count - 1,
                    (rngKeys.Count - 1)*dblInSamplePrportion);
            keysInSample = rngKeys.Take(intSampleSize).ToList();
            GetInOutOfSample(
                keysInSample,
                xMap,
                out xMapInSample,
                out xMapOutOfSample);
        }

        public static void GetInOutOfSample<T, U>(
            List<T> keysInSample,
            SortedDictionary<T, U> xMap,
            out SortedDictionary<T, U> xMapInSample,
            out SortedDictionary<T, U> xMapOutOfSample)
        {
            xMapInSample = null;
            xMapOutOfSample = null;
            try
            {
                xMapInSample = new SortedDictionary<T, U>(
                    (from n in xMap
                     where keysInSample.Contains(n.Key)
                     select n).ToDictionary(t => t.Key, t => t.Value));
                xMapOutOfSample = new SortedDictionary<T, U>(
                    (from n in xMap
                     where !keysInSample.Contains(n.Key)
                     select n).ToDictionary(t => t.Key, t => t.Value));

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static SortedDictionary<DateTime, TsRow2D> ToSortedMap(
                List<TsRow2D> tsEventsList)
        {
            try
            {
                var sortedMap =
                    new SortedDictionary<DateTime, TsRow2D>();
                foreach (TsRow2D tsRow2D in tsEventsList)
                {
                    sortedMap[tsRow2D.Time] = tsRow2D;
                }
                return sortedMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, TsRow2D>();
        }


        public static double GetRegressionReturnForecast(
            int intForecast,
            double dblInitialPrice,
            double[] featureToForecast,
            SortedDictionary<DateTime, double> yMap,
            SortedDictionary<DateTime, double[]> xMap,
            int intEnsembleSize,
            bool blnNormalizeX,
            bool blnNormalizeY)
        {
            try
            {
                if (xMap == null ||
                    xMap.Count == 0 ||
                    yMap == null ||
                    yMap.Count == 0)
                {
                    return double.NaN;
                }

                if (blnNormalizeX)
                {
                    double[] lastFeature = xMap.Values.Last();
                    featureToForecast = GetLogReturn(lastFeature, featureToForecast);
                }

                double dblDeltaY = double.NaN;
                double dblMinValY = double.NaN;
                if (blnNormalizeY)
                {
                    yMap = GetLogReturns(yMap, intForecast);
                    yMap = NormalizeFeatures(yMap, out dblDeltaY, out dblMinValY);
                }

                if (blnNormalizeX)
                {
                    xMap = GetLogReturns(xMap, intForecast);
                    double[] dblDeltaX;
                    double[] dblMinValX;
                    xMap = NormalizeFeatures(xMap, out dblDeltaX, out dblMinValX);
                    featureToForecast = NormalizeFeature(dblDeltaX, dblMinValX, featureToForecast);
                }

                double dblForecast = 0;
                if (intEnsembleSize > 0)
                {
                    List<RegressionTrainerWrapper> regressionEnsemble = GetRegressionEnsemble(
                        yMap,
                        xMap,
                        intEnsembleSize,
                        0.95);

                    List<double> regressionEnsemblePrediction = GetRegressionEnsemblePrediction(
                        featureToForecast,
                        regressionEnsemble);
                    //
                    // decode forecast
                    //
                    if (blnNormalizeY)
                    {
                        for (int i = 0; i < regressionEnsemblePrediction.Count; i++)
                        {
                            double dblCurrForecast = regressionEnsemblePrediction[i];
                            dblCurrForecast = Math.Exp(dblMinValY + dblCurrForecast * dblDeltaY) *
                                              dblInitialPrice;
                            regressionEnsemblePrediction[i] = dblCurrForecast;
                        }
                    }
                    dblForecast = GetHarmonicMean(
                        regressionEnsemblePrediction);
                }
                else
                {
                    RegressionTrainerWrapper regression = GetRegression(yMap, xMap);
                    if (regression != null &&
                        regression.Weights != null &&
                        regression.Weights.Length > 0)
                    {
                        dblForecast = regression.Forecast(featureToForecast);
                        if (blnNormalizeY)
                        {
                            dblForecast = Math.Exp(dblMinValY + dblForecast * dblDeltaY) *
                                          dblInitialPrice;
                        }
                    }
                }
                return dblForecast;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static SortedDictionary<DateTime, double[]> GetAutoRegressive(
            SortedDictionary<DateTime, double[]> xMap,
            SortedDictionary<DateTime, double> yMap,
            int intAutoRegressiveWindow)
        {
            try
            {
                if (xMap == null ||
                    xMap.Count == 0)
                {
                    return new SortedDictionary<DateTime, double[]>();
                }
                int intVars = xMap.Values.First().Length;
                var xMapOut = new SortedDictionary<DateTime, double[]>();
                var lockObj = new object();
                Parallel.For(0, intVars, delegate(int i)
                                             //for (int i = 0; i < intVars; i++)
                                             {
                                                 var currXMap = SelectVariable(xMap, i);
                                                 SortedDictionary<DateTime, double[]> currXMapAutoregress = GetAutoRegressiveMapIncluded
                                                     (
                                                         intAutoRegressiveWindow,
                                                         currXMap);
                                                 RegressionTrainerWrapper regression = GetRegression(
                                                     yMap,
                                                     currXMapAutoregress);

                                                 foreach (
                                                     KeyValuePair<DateTime, double[]> keyValuePair in
                                                         currXMapAutoregress)
                                                 {
                                                     double[] currFeat;
                                                     lock (lockObj)
                                                     {
                                                         if (!xMapOut.TryGetValue(keyValuePair.Key, out currFeat))
                                                         {
                                                             currFeat = new double[intVars];
                                                             xMapOut[keyValuePair.Key] = currFeat;
                                                         }
                                                     }
                                                     currFeat[i] = regression.Forecast(keyValuePair.Value);
                                                 }
                                             });
                return xMapOut;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static void LoadSamplesGivenForecastPeriods<T>(
            SortedDictionary<T, double> yData,
            SortedDictionary<T, double[]> xData,
            int intForecastPeriods,
            out List<SortedDictionary<T, double[]>> allSamplesX,
            out List<SortedDictionary<T, double>> allSamplesY,
            out List<List<T>> indexMappings,
            int intMaxForecastDays = MAX_FORECAST_DAYS)
        {
            allSamplesX = null;
            allSamplesY = null;
            indexMappings = null;
            try
            {
                intForecastPeriods = Math.Min(intForecastPeriods, intMaxForecastDays);

                if (intForecastPeriods == 0)
                {
                    allSamplesX = new List<SortedDictionary<T, double[]>>
                                      {
                                          xData
                                      };
                    allSamplesY = new List<SortedDictionary<T, double>>
                                      {
                                          yData
                                      };
                    indexMappings = new List<List<T>>
                                        {
                                            xData.Keys.ToList()
                                        };
                    return;
                }

                allSamplesX = new List<SortedDictionary<T, double[]>>();
                allSamplesY = new List<SortedDictionary<T, double>>();
                indexMappings = new List<List<T>>();
                var xDataArr = xData.ToArray();
                var yDataArr = yData.ToArray();
                for (int i = 0; i < intForecastPeriods; i++)
                {
                    var samplesX = new SortedDictionary<T, double[]>();
                    var samplesY = new SortedDictionary<T, double>();
                    var currIndexMappings = new List<T>();
                    for (int j = xData.Count - i - 1; j >= 0; j--)
                    {
                        samplesX[xDataArr[j].Key] = xDataArr[j].Value;
                        samplesY[yDataArr[j].Key] = yDataArr[j].Value;
                        currIndexMappings.Add(yDataArr[j].Key);
                        j -= intForecastPeriods;
                    }
                    allSamplesX.Add(samplesX);
                    allSamplesY.Add(samplesY);
                    indexMappings.Add(currIndexMappings);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public static void LoadSamplesGivenForecastPeriods(
            List<double> yData,
            List<double[]> xData,
            int intForecastDays,
            out List<List<double[]>> allSamplesX,
            out List<List<double>> allSamplesY,
            out List<List<int>> indexMappings,
            int intMaxForecastDays = MAX_FORECAST_DAYS)
        {
            allSamplesX = null;
            allSamplesY = null;
            indexMappings = null;
            try
            {
                if(xData.Count != yData.Count)
                {
                    throw new HCException("Invalid size");
                }

                intForecastDays = Math.Min(intForecastDays, intMaxForecastDays);

                if (intForecastDays <= 0)
                {
                    allSamplesX = new List<List<double[]>>
                                      {
                                          xData
                                      };
                    allSamplesY = new List<List<double>>
                                      {
                                          yData
                                      };
                    indexMappings = new List<List<int>>();
                    var list = new List<int>();
                    for (int a = 0; a < xData.Count; a++)
                    {
                        list.Add(a);
                    }
                    indexMappings.Add(list);
                    return;
                }

                allSamplesX = new List<List<double[]>>();
                allSamplesY = new List<List<double>>();
                indexMappings = new List<List<int>>();
                for (int i = 0; i < intForecastDays; i++)
                {
                    var samplesX = new List<double[]>();
                    var samplesY = new List<double>();
                    var currIndexMappings = new List<int>();
                    for (int j = i; j < xData.Count;)
                    {
                        samplesX.Add(xData[j]);
                        samplesY.Add(yData[j]);
                        currIndexMappings.Add(j);
                        j += intForecastDays;
                    }
                    allSamplesX.Add(samplesX);
                    allSamplesY.Add(samplesY);
                    indexMappings.Add(currIndexMappings);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public static int GetMostCorrelatedSymbol(
            HashSet<int> varSet,
            List<double[]> xData,
            List<double> yData,
            TrainerWrapperFactory trainerWrapperFactory,
            bool blnUseAbsCorrelation)
        {
            Dictionary<int, double> map;
            return GetMostCorrelatedSymbol(
                varSet,
                xData,
                yData,
                trainerWrapperFactory,
                out map,
                blnUseAbsCorrelation);
        }

        public static int GetMostCorrelatedSymbol(
            HashSet<int> varSet,
            List<double[]> xData,
            List<double> yData,
            TrainerWrapperFactory trainerWrapperFactory,
            out Dictionary<int, double> correlations,
            bool blnUseAbsCorrelation)
        {
            correlations = new Dictionary<int, double>();
            try
            {
                if (varSet == null ||
                    varSet.Count == 0)
                {
                    return -1;
                }
                double dblMaxCorrelation = -Double.MaxValue;
                int intSelectVar = -1;
                foreach (int intVarId in varSet)
                {
                    List<double> currVector = GetVector(xData, intVarId);

                    if(trainerWrapperFactory.EnumTrainerWrapper != EnumTrainerWrapper.LinearRegression)
                    {
                        currVector = trainerWrapperFactory.Build(
                            currVector,
                            yData).GetForecasts();
                    }

                    double dblAbsCorr = Correlation.GetCorrelation(
                            yData,
                            currVector);
                    if (blnUseAbsCorrelation)
                    {
                        dblAbsCorr = Math.Abs(dblAbsCorr);
                    }
                    correlations[intVarId] = dblAbsCorr;
                    if (dblMaxCorrelation < dblAbsCorr)
                    {
                        dblMaxCorrelation = dblAbsCorr;
                        intSelectVar = intVarId;
                    }
                }
                return intSelectVar;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }

        public static int GetMostCorrelatedIndex(
            List<double> yVars,
            List<double[]> xVars,
            List<int> exclustionVariables)
        {
            try
            {
                if (xVars == null ||
                    xVars.Count == 0)
                {
                    return -1;
                }
                int intVars = xVars.First().Length;
                var varSet = new HashSet<int>();
                for (int i = 0; i < intVars; i++)
                {
                    varSet.Add(i);
                }

                double dblMaxCorrelation = -Double.MaxValue;
                int intSelectVar = -1;
                foreach (int intVarId in varSet)
                {
                    if (exclustionVariables.Contains(intVarId))
                    {
                        continue;
                    }

                    List<double> currVector = GetVector(xVars, intVarId);
                    double dblAbsCorr = Math.Abs(
                        Stat.CorrCoeff(
                        yVars.ToArray(), 
                        currVector.ToArray()));

                    if (dblMaxCorrelation < dblAbsCorr)
                    {
                        dblMaxCorrelation = dblAbsCorr;
                        intSelectVar = intVarId;
                    }
                }
                return intSelectVar;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }

        public static bool IsAValidSymbol(
            string strSymbol)
        {
            try
            {
                if (String.IsNullOrEmpty(strSymbol))
                {
                    return false;
                }

                strSymbol = strSymbol
                    .Replace("M", string.Empty)
                    .Replace("B", string.Empty);

                double dblVal;
                if (Double.TryParse(strSymbol, out dblVal))
                {
                    return false;
                }
                if (strSymbol.Contains("http://uk.advfn.com/p.php?pid=financials&symbol="))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static List<double[]> MergeTsLists(
            List<double[]> list1,
            List<double[]> list2)
        {
            var outList = new List<double[]>();
            try
            {
                if (list1 == null ||
                    list2 == null)
                {
                    return outList;
                }
                for (int i = 0; i < list1.Count; i++)
                {
                    var currList = new List<double>(list1[i]);
                    currList.AddRange(list2[i]);
                    outList.Add(currList.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return outList;
        }

        public static int GetMostVotedClass(
            int[] votes)
        {
            try
            {
                int intMaxCounter = -int.MaxValue;
                var votingMap = new Dictionary<int, int>();
                int intNnetVotedClass = -1;
                for (int i = 0; i < votes.Length; i++)
                {
                    int intClass = votes[i];
                    int intCounter;
                    votingMap.TryGetValue(intClass, out intCounter);
                    intCounter++;
                    votingMap[intClass] = intCounter;
                    if (intCounter > intMaxCounter)
                    {
                        intNnetVotedClass = intClass;
                        intMaxCounter = intCounter;
                    }
                }
                return intNnetVotedClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }


        public static int GetMostVotedClass(
            List<int> votes)
        {
            bool blnIsUndecided;
            
            return GetMostVotedClass(
                votes,
                out blnIsUndecided);
        }

        public static double GetMostVotedClass(
            List<double> votes)
        {
            bool blnIsUndecided;

            return GetMostVotedClass(
                votes,
                out blnIsUndecided);
        }

        public static int GetMostVotedClass(
            List<int> votes,
            out bool blnIsUndecided)
        {
            blnIsUndecided = false;
            try
            {
                if (votes == null)
                {
                    return 1;
                }
                if (votes.Count == 1)
                {
                    return votes[0];
                }


                int intMaxCounter = -int.MaxValue;
                var votingMap = new Dictionary<int, int>();
                int intNnetVotedClass = -1;
                for (int i = 0; i < votes.Count; i++)
                {
                    int intClass = votes[i];
                    int intCounter;
                    votingMap.TryGetValue(intClass, out intCounter);
                    intCounter++;
                    votingMap[intClass] = intCounter;
                    if (intCounter > intMaxCounter)
                    {
                        intNnetVotedClass = intClass;
                        intMaxCounter = intCounter;
                    }
                }
                blnIsUndecided = votingMap.Count > 1 &&
                    intMaxCounter * 1.0 / votes.Count <=
                                    1.0 / votingMap.Count;
                return intNnetVotedClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }

        public static double GetMostVotedClass(
            List<double> votes,
            out bool blnIsUndecided)
        {
            blnIsUndecided = false;
            try
            {
                if (votes == null)
                {
                    return 1;
                }
                if (votes.Count == 1)
                {
                    return votes[0];
                }


                int intMaxCounter = -int.MaxValue;
                var votingMap = new Dictionary<double, int>();
                double intNnetVotedClass = -1;
                for (int i = 0; i < votes.Count; i++)
                {
                    double intClass = votes[i];
                    int intCounter;
                    votingMap.TryGetValue(intClass, out intCounter);
                    intCounter++;
                    votingMap[intClass] = intCounter;
                    if (intCounter > intMaxCounter)
                    {
                        intNnetVotedClass = intClass;
                        intMaxCounter = intCounter;
                    }
                }
                blnIsUndecided = votingMap.Count > 1 &&
                    intMaxCounter * 1.0 / votes.Count <=
                                    1.0 / votingMap.Count;
                return intNnetVotedClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }

        public static void GetAutoRegressiveVariables(
            int intSize,
            SortedDictionary<DateTime, double> data,
            out SortedDictionary<DateTime, double> yData,
            out SortedDictionary<DateTime, double[]> xData)
        {
            yData = new SortedDictionary<DateTime, double>();
            xData = new SortedDictionary<DateTime, double[]>();
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return;
                }
                var dataArr = data.ToArray();
                for (int i = intSize; i < data.Count; i++)
                {
                    var currVars = new List<double>();
                    for (int j = 0; j < intSize; j++)
                    {
                        currVars.Add(dataArr[i - j - 1].Value);
                    }
                    xData.Add(
                        dataArr[i].Key,
                        currVars.ToArray());
                    yData.Add(
                        dataArr[i].Key,
                        dataArr[i].Value);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void GetAutoRegressiveVariables(
            int intSize,
            List<double> data,
            out List<double> yData,
            out List<double[]> xData)
        {
            yData = new List<double>();
            xData = new List<double[]>();
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return;
                }
                for (int i = intSize; i < data.Count; i++)
                {
                    var currVars = new List<double>();
                    for (int j = 0; j < intSize; j++)
                    {
                        currVars.Add(data[i - j - 1]);
                    }
                    xData.Add(currVars.ToArray());
                    yData.Add(data[i]);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<TsRow2D> GetLogReturns(
            int intSize,
            List<TsRow2D> data)
        {
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return new List<TsRow2D>();
                }
                var outData = new List<TsRow2D>();
                for (int i = intSize; i < data.Count; i++)
                {
                    outData.Add(
                        new TsRow2D(
                            data[i].Time,
                            (data[i].Fx / data[i - intSize].Fx) - 1.0));
                }
                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static List<double> GetLogReturns(
            List<double> list,
            int intSize)
        {
            try
            {
                var returns = new List<double>();
                for (int i = intSize; i < list.Count; i++)
                {
                    double dblPrevVal = list[i - intSize];
                    double dblCurrVal = list[i];

                    if (Double.IsNaN(dblPrevVal) ||
                        Double.IsNaN(dblCurrVal))
                    {
                        returns.Add(double.NaN);
                        continue;
                    }

                    double dblReturn;
                    if (dblCurrVal == 0 && dblPrevVal > 0)
                    {
                        dblReturn = -6;
                    }
                    else
                    {
                        if (dblPrevVal == 0)
                        {
                            dblPrevVal = 1e-6;
                        }
                        if (dblCurrVal == 0)
                        {
                            dblCurrVal = 1e-6;
                        }


                        dblReturn = Math.Log(dblCurrVal / dblPrevVal);
                    }
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<TsRow2D> GetDerivative(
            int intSize,
            List<TsRow2D> data)
        {
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return new List<TsRow2D>();
                }
                var outData = new List<TsRow2D>();
                for (int i = intSize; i < data.Count; i++)
                {
                    outData.Add(
                        new TsRow2D(
                            data[i].Time,
                            data[i].Fx - data[i - intSize].Fx));
                }
                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static void ValidateDatesFromPair(
            List<TsRow2D> data1,
            List<TsRow2D> data2,
            out List<TsRow2D> outData1,
            out List<TsRow2D> outData2)
        {
            outData1 = null;
            outData2 = null;
            try
            {
                if (data1 == null ||
                    data1.Count == 0 ||
                    data2 == null ||
                    data2.Count == 0)
                {
                    return;
                }
                var dateSet1 = new HashSet<DateTime>(from n in data1 select n.Time);
                var dateSet2 = new HashSet<DateTime>(from n in data2 select n.Time);
                outData1 = (from n in data1
                            where dateSet1.Contains(n.Time) &&
                                  dateSet2.Contains(n.Time)
                            select n).ToList();
                outData2 = (from n in data2
                            where dateSet1.Contains(n.Time) &&
                                  dateSet2.Contains(n.Time)
                            select n).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<double> GetDerivative(int intSize, List<double> data)
        {
            var outData = new List<double>();
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return new List<double>();
                }
                for (int i = intSize; i < data.Count; i++)
                {
                    outData.Add(data[i] - data[i - intSize]);
                }
                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return outData;
        }

        public static List<double[]> GetDerivative(int intSize, List<double[]> data)
        {
            var outData = new List<double[]>();
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return new List<double[]>();
                }
                int intJ = data[0].Length;
                for (int i = intSize; i < data.Count; i++)
                {
                    var returns = new List<double>();
                    for (int j = 0; j < intJ; j++)
                    {
                        returns.Add(data[i][j] - data[i - intSize][j]);
                    }
                    outData.Add(returns.ToArray());
                }
                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return outData;
        }

        public static double GetCurrentPrice(
            double dblX,
            TsFunction currentTs,
            out int intIndex)
        {
            intIndex = -1;
            try
            {
                intIndex = (int)dblX;
                if (intIndex >= currentTs.Count)
                {
                    return double.NaN;
                }
                return currentTs[intIndex].Fx;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static List<T> GetIntersectionSet<T>(
            List<T> dates1,
            List<T> dates2)
        {
            try
            {
                if (dates1 == null ||
                    dates1.Count == 0 ||
                    dates2 == null ||
                    dates2.Count == 0)
                {
                    return new List<T>();
                }
                return (from n in dates1 where dates2.Contains(n) select n).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<T>();
        }

        public static SortedDictionary<DateTime, T> FilterMapByKeys<T>(
            SortedDictionary<DateTime, T> map,
            DateTime minDate,
            DateTime maxDate)
        {
            try
            {
                if (map == null ||
                    map.Count == 0)
                {
                    return new SortedDictionary<DateTime, T>();
                }
                var mapOut = new SortedDictionary<DateTime, T>(
                    (from n in map
                     where n.Key >= minDate &&
                           n.Key <= maxDate
                     select n).ToDictionary(
                         t => t.Key, t => t.Value));
                return mapOut;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, T>();
        }

        public static SortedDictionary<K, T> FilterMapByKeys<K, T>(
            SortedDictionary<K, T> map,
            List<K> keys)
        {
            try
            {
                if (map == null ||
                    map.Count == 0)
                {
                    return new SortedDictionary<K, T>();
                }

                var mapOut = new SortedDictionary<K, T>();
                for (int i = 0; i < keys.Count; i++)
                {
                    K key = keys[i];
                    T val;
                    if (map.TryGetValue(key, out val))
                    {
                        mapOut[key] = val;
                    }
                }
                return mapOut;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<K, T>();
        }

        public static SortedDictionary<DateTime, double[]> MergeFeatures(
            SortedDictionary<DateTime, double> map1,
            SortedDictionary<DateTime, double> map2)
        {
            try
            {
                if (map1 == null ||
                    map1.Count == 0 ||
                    map2 == null ||
                    map2.Count == 0)
                {
                    return new SortedDictionary<DateTime, double[]>();
                }
                var dateSet = GetIntersectionSet(map1.Keys.ToList(), map2.Keys.ToList());
                dateSet.Sort();
                var outMap = new SortedDictionary<DateTime, double[]>();
                foreach (DateTime dateTime in dateSet)
                {
                    double dblFeat1;
                    double dblFeat2;
                    if (map1.TryGetValue(dateTime, out dblFeat1) &&
                        map2.TryGetValue(dateTime, out dblFeat2))
                    {
                        var newFeat = new[]
                                          {
                                              dblFeat1,
                                              dblFeat2
                                          };
                        outMap[dateTime] = newFeat;
                    }
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static SortedDictionary<T, double[]> MergeFeatures<T>(
            SortedDictionary<T, double[]> map1,
            SortedDictionary<T, double[]> map2,
            List<string> varNames1,
            List<string> varNames2,
            out List<string> varNames)
        {
            varNames = new List<string>();
            try
            {
                if (map1 == null ||
                    map1.Count == 0)
                {
                    varNames = varNames2.ToList();
                    return new SortedDictionary<T, double[]>(
                        map2);
                }

                if (map2 == null ||
                    map2.Count == 0)
                {
                    varNames = varNames1.ToList();
                    return new SortedDictionary<T, double[]>(
                        map1);
                }

                if (varNames1.Count != varNames1.Distinct().Count())
                {
                    throw new HCException("Var names are not unique");
                }
                if (varNames2.Count != varNames2.Distinct().Count())
                {
                    throw new HCException("Var names are not unique");
                }

                var varIndexes2 = new List<int>();
                varNames = varNames1.ToList();
                for(int i = 0; i <varNames2.Count; i++)
                {
                    string strVarName2 = varNames2[i];
                    if (!varNames1.Contains(strVarName2))
                    {
                        varIndexes2.Add(i);
                        varNames.Add(strVarName2);
                    }
                }

                List<T> dateSet = GetIntersectionSet(
                    map1.Keys.ToList(), 
                    map2.Keys.ToList());
                dateSet.Sort();
                var outMap = new SortedDictionary<T, double[]>();
                foreach (T dateTime in dateSet)
                {
                    double[] feat1;
                    double[] feat2;
                    if (map1.TryGetValue(dateTime, out feat1) &&
                        map2.TryGetValue(dateTime, out feat2))
                    {
                        List<double> newFeat;
                        if (feat1 != null)
                        {
                            newFeat = feat1.ToList();
                        }
                        else
                        {
                            newFeat = new List<double>();
                        }
                        newFeat.AddRange(
                            from n in varIndexes2
                            select
                            feat2[n]);
                        outMap[dateTime] = newFeat.ToArray();
                    }
                }

                if (varNames.Count != outMap.First().Value.Length)
                {
                    throw new HCException("invalid feature names");
                }

                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static SortedDictionary<T, double[]> MergeFeatures<T>(
            SortedDictionary<T, double[]> map1,
            SortedDictionary<T, double[]> map2)
        {
            try
            {
                if (map1 == null ||
                    map1.Count == 0 ||
                    map2 == null ||
                    map2.Count == 0)
                {
                    if ((map2 == null ||
                        map2.Count == 0) &&
                        (map1 != null &&
                         map1.Count > 0))
                    {
                        return new SortedDictionary<T, double[]>(map1);
                    }

                    if ((map1 == null ||
                        map1.Count == 0) &&
                        (map2 != null &&
                         map2.Count > 0))
                    {
                        return new SortedDictionary<T, double[]>(map2);
                    }

                    return new SortedDictionary<T, double[]>();
                }

                var dateSet = GetIntersectionSet(map1.Keys.ToList(), map2.Keys.ToList());
                dateSet.Sort();
                var outMap = new SortedDictionary<T, double[]>();
                foreach (T dateTime in dateSet)
                {
                    double[] feat1;
                    double[] feat2;
                    if (map1.TryGetValue(dateTime, out feat1) &&
                        map2.TryGetValue(dateTime, out feat2))
                    {
                        List<double> newFeat;
                        if (feat1 != null)
                        {
                            newFeat = feat1.ToList();
                        }
                        else
                        {
                            newFeat = new List<double>();
                        }
                        newFeat.AddRange(feat2);
                        outMap[dateTime] = newFeat.ToArray();
                    }
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static SortedDictionary<DateTime, double[]> MergeFeatures(
            SortedDictionary<DateTime, double> map1,
            SortedDictionary<DateTime, double[]> map2)
        {
            try
            {
                if (map1 == null ||
                    map1.Count == 0 ||
                    map2 == null ||
                    map2.Count == 0)
                {
                    return new SortedDictionary<DateTime, double[]>();
                }
                var dateSet = GetIntersectionSet(map1.Keys.ToList(), map2.Keys.ToList());
                dateSet.Sort();
                var outMap = new SortedDictionary<DateTime, double[]>();
                foreach (DateTime dateTime in dateSet)
                {
                    double feat1;
                    double[] feat2;
                    if (map1.TryGetValue(dateTime, out feat1) &&
                        map2.TryGetValue(dateTime, out feat2))
                    {
                        var newFeat = new List<double>(new[] { feat1 });
                        newFeat.AddRange(feat2);
                        outMap[dateTime] = newFeat.ToArray();
                    }
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static SortedDictionary<DateTime, double[]> MergeFeatures(
            SortedDictionary<DateTime, double[]> map1,
            SortedDictionary<DateTime, double> map2)
        {
            try
            {
                if (map1 == null ||
                    map1.Count == 0 ||
                    map2 == null ||
                    map2.Count == 0)
                {
                    return new SortedDictionary<DateTime, double[]>();
                }
                var dateSet = GetIntersectionSet(map1.Keys.ToList(), map2.Keys.ToList());
                dateSet.Sort();
                var outMap = new SortedDictionary<DateTime, double[]>();
                foreach (DateTime dateTime in dateSet)
                {
                    double[] feat1;
                    double feat2;
                    if (map1.TryGetValue(dateTime, out feat1) &&
                        map2.TryGetValue(dateTime, out feat2))
                    {
                        var newFeat = feat1.ToList();
                        newFeat.Add(feat2);
                        outMap[dateTime] = newFeat.ToArray();
                    }
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static SortedDictionary<K, T> ShiftDataForwards<K, T>(
            int intWindow,
            SortedDictionary<K, T> dataMap)
        {
            try
            {
                if (dataMap == null ||
                    dataMap.Count == 0)
                {
                    return new SortedDictionary<K, T>();
                }
                var outData = new SortedDictionary<K, T>();
                KeyValuePair<K, T>[] dataArr = dataMap.ToArray();
                for (int i = 0; i < dataArr.Length - intWindow; i++)
                {
                    outData[dataArr[i].Key] = dataArr[i + intWindow].Value;
                }
                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<K, T>();
        }

        public static SortedDictionary<DateTime, T> ShiftDataBackwards<T>(
            int intWindow,
            SortedDictionary<DateTime, T> dataMap,
            out SortedDictionary<DateTime, DateTime> currToPrevDates,
            long lngBarSize = 0)
        {
            currToPrevDates = new SortedDictionary<DateTime, DateTime>();
            try
            {
                if (dataMap == null ||
                    dataMap.Count == 0)
                {
                    return new SortedDictionary<DateTime, T>();
                }
                if (intWindow <= 0)
                {
                    return new SortedDictionary<DateTime, T>(dataMap);

                }
                var outData = new SortedDictionary<DateTime, T>();
                KeyValuePair<DateTime, T>[] dataArr = dataMap.ToArray();
                var lastDate = dataMap.Keys.Last();
                for (int i = intWindow; i < dataArr.Length + intWindow; i++)
                {
                    DateTime currPrevDate = dataArr[i - intWindow].Key;
                    DateTime currDate;
                    if (i < dataArr.Length)
                    {
                        outData[dataArr[i].Key] = dataArr[i - intWindow].Value;
                        currDate = dataArr[i].Key;
                    }
                    else if (lngBarSize > 0)
                    {
                        lastDate = lastDate.AddTicks(lngBarSize);
                        outData[lastDate] = dataArr[i - intWindow].Value;
                        currDate = lastDate;
                    }
                    else
                    {
                        lastDate = DateHelper.GetNextWorkingDate(lastDate, false);
                        outData[lastDate] = dataArr[i - intWindow].Value;
                        currDate = lastDate;
                    }
                    currToPrevDates[currDate] = currPrevDate;
                }

                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, T>();
        }

        public static SortedDictionary<K, T> ShiftDataBackwardsGeneric<K,T>(
            int intWindow,
            SortedDictionary<K, T> dataMap)
        {
            try
            {
                if (dataMap == null ||
                    dataMap.Count == 0)
                {
                    return new SortedDictionary<K, T>();
                }
                var outData = new SortedDictionary<K, T>();
                KeyValuePair<K, T>[] dataArr = dataMap.ToArray();
                var lastDate = dataMap.Keys.Last();
                for (int i = intWindow; i < dataArr.Length + intWindow; i++)
                {
                    K currPrevDate = dataArr[i - intWindow].Key;
                    K currDate;
                    if (i < dataArr.Length)
                    {
                        outData[dataArr[i].Key] = dataArr[i - intWindow].Value;
                        currDate = dataArr[i].Key;
                    }
                }

                return outData;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<K, T>();
        }

        public static SortedDictionary<DateTime, T> ShiftDataBackwards<T>(
            int intWindow,
            SortedDictionary<DateTime, T> dataMap,
            long lngBarSize = 0)
        {
            try
            {
                if (dataMap == null)
                {
                    return new SortedDictionary<DateTime, T>();
                }

                if (intWindow == 0)
                {
                    return new SortedDictionary<DateTime, T>(dataMap);
                }
                SortedDictionary<DateTime, DateTime> tmp;
                return ShiftDataBackwards(
                    intWindow,
                    dataMap,
                    out tmp,
                    lngBarSize);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, T>();
        }

        public static List<double> GetVector(
            SortedDictionary<DateTime, double[]> xVars,
            int intVarId)
        {
            try
            {
                if (xVars == null ||
                    xVars.Count == 0)
                {
                    return new List<double>();
                }
                return (from n in xVars select n.Value[intVarId]).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<double> GetVector(List<double[]> xVars, int intVarId)
        {
            try
            {
                if (xVars == null || xVars.Count == 0)
                {
                    return new List<double>();
                }
                if (xVars[0].Length - 1 < intVarId)
                {
                    return new List<double>();
                }
                return (from n in xVars select n[intVarId]).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }


        public static SortedDictionary<T, double> LoadYMapReturns<T>(
            SortedDictionary<T, double> map,
            int intWindow,
            bool blnAsPrc)
        {
            try
            {
                if(intWindow<= 0)
                {
                    return new SortedDictionary<T, double>(map);
                }

                KeyValuePair<T, double>[] yArr = map.ToArray();
                var yMapReturns = new SortedDictionary<T, double>();
                for (int i = intWindow; i < yArr.Length; i++)
                {
                    double dblReturn;
                    if (intWindow <= 0)
                    {
                        dblReturn = yArr[i].Value;
                    }
                    else
                    {
                        dblReturn = yArr[i].Value - yArr[i - intWindow].Value;
                        if (blnAsPrc)
                        {
                            dblReturn /= yArr[i - intWindow].Value;
                        }
                    }
                    yMapReturns[yArr[i].Key] = dblReturn;
                }
                return yMapReturns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }


        public static SortedDictionary<T, double[]> SelectVariables<T>(
            SortedDictionary<T, double[]> map,
            List<int> selectedVariables)
        {
            try
            {
                if (map == null ||
                    map.Count == 0)
                {
                    return new SortedDictionary<T, double[]>();
                }

                int intVars = map.Values.First().Length;
                if (intVars < selectedVariables.Count)
                {
                    throw new HCException("Invalid number of vars");
                }

                var mapOut = new SortedDictionary<T, double[]>();
                foreach (var kvp in map)
                {
                    double[] doublese = kvp.Value;
                    var currFeat = new double[selectedVariables.Count];
                    int i = 0;
                    foreach (int intSelectedVAr in selectedVariables)
                    {
                        currFeat[i] = doublese[intSelectedVAr];
                        i++;
                    }
                    mapOut.Add(kvp.Key, currFeat);
                }
                return mapOut;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static List<double[]> SelectVariables(
            List<double[]> xVars,
            List<int> selectedVariables)
        {
            try
            {
                if (xVars == null ||
                    xVars.Count == 0 ||
                    selectedVariables == null ||
                    selectedVariables.Count == 0)
                {
                    return new List<double[]>();
                }

                int intVars = xVars.First().Length;
                if (intVars < selectedVariables.Count)
                {
                    throw new HCException("Invalid number of vars");
                }

                var listOut = new List<double[]>(xVars.Count + 2);
                for (int k = 0; k < xVars.Count; k++)
                {
                    double[] doublese = xVars[k];
                    var currFeat = new double[selectedVariables.Count];
                    for (int i = 0; i < selectedVariables.Count; i++)
                    {
                        currFeat[i] = doublese[selectedVariables[i]];
                    }
                    listOut.Add(currFeat);
                }
                return listOut;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[]>();
        }

        public static List<double> GetRegressionEnsemblePrediction(
            double[] feature,
            List<RegressionTrainerWrapper> regressions)
        {
            try
            {
                if (feature == null ||
                    feature.Length == 0 ||
                    regressions == null ||
                    regressions.Count == 0)
                {
                    return new List<double>();
                }
                var predictions = new List<double>();
                foreach (var regression in regressions)
                {
                    predictions.Add(
                        regression.Forecast(feature));
                }
                return predictions;
                //return GetHarmonicMean(predictions);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<RegressionTrainerWrapper> GetRegressionEnsemble<T>(
            SortedDictionary<T, double> yMap,
            SortedDictionary<T, double[]> xMap,
            int intEnsembleSize,
            double dblSampleSizeFactor)
        {
            try
            {
                if (xMap == null ||
                    xMap.Count == 0 ||
                    yMap == null ||
                    yMap.Count == 0)
                {
                    return new List<RegressionTrainerWrapper>();
                }
                List<T> dateSet = GetIntersectionSet(
                    yMap.Keys.ToList(),
                    xMap.Keys.ToList());

                List<double[]> x = (from n in xMap
                                    where dateSet.Contains(n.Key)
                                    select n.Value).ToList();
                List<double> y = (from n in yMap
                                  where dateSet.Contains(n.Key)
                                  select n.Value).ToList();
                var regressions = new List<RegressionTrainerWrapper>();
                var lockObject = new object();
                Parallel.For(0, intEnsembleSize, delegate(int i)
                                                     {
                                                         try
                                                         {
                                                             List<double[]> xSample;
                                                             List<double> ySample;
                                                             SelectRandomSampleData(
                                                                 x,
                                                                 y,
                                                                 dblSampleSizeFactor,
                                                                 out xSample,
                                                                 out ySample);
                                                             var regression = new RegressionTrainerWrapper(xSample, ySample);
                                                             lock (lockObject)
                                                             {
                                                                 regressions.Add(regression);
                                                             }
                                                         }
                                                         catch (Exception ex)
                                                         {
                                                             Logger.Log(ex);
                                                         }
                                                     });

                return regressions;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<RegressionTrainerWrapper>();
        }

        public static SortedDictionary<DateTime, double> GetLogReturns(
            SortedDictionary<DateTime, double> yMap0,
            SortedDictionary<DateTime, double> baseMap,
            int intSize)
        {
            try
            {
                if (yMap0 == null ||
                    yMap0.Count == 0 ||
                    baseMap == null ||
                    baseMap.Count == 0)
                {
                    return new SortedDictionary<DateTime, double>();
                }
                var baseMapArr = baseMap.ToArray();
                var yMap = CloneMap(yMap0);
                int intHighIndex = baseMap.Keys.ToList().IndexOf(yMap.Keys.First());
                if (intHighIndex < 0)
                {
                    intHighIndex = Math.Abs(TimeSeriesUtils.GetIndexOfDate(
                        baseMap.Keys.ToList(),
                        yMap.Keys.First()));

                }
                int intLowIndex = intHighIndex -
                                  intSize;
                for (int i = intLowIndex; i < intHighIndex; i++)
                {
                    yMap[baseMapArr[i].Key] = baseMapArr[i].Value;
                }
                SortedDictionary<DateTime, double> logYMap = GetLogReturns(yMap, intSize);
                if (logYMap.Count != yMap0.Count ||
                    logYMap.First().Key != yMap0.First().Key)
                {
                    throw new HCException("Invalid map");
                }
                return logYMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double>();
        }

        public static void SelectRandomSampleData<T, U>(
            List<T> x,
            List<U> y,
            double dblSampleSizeFactor,
            out List<T> trainX,
            out List<U> trainY)
        {
            trainX = new List<T>();
            trainY = new List<U>();
            try
            {
                int intInstances = x.Count;
                if (x.Count != y.Count)
                {
                    throw new HCException("Invalid sample size");
                }
                var rngWrapper = new RngWrapper();
                var intSampleSize = (int)(intInstances * dblSampleSizeFactor);
                for (int i = 0; i < intSampleSize; i++)
                {
                    int intIndex = rngWrapper.NextInt(0, intInstances - 1);
                    trainX.Add(x[intIndex]);
                    trainY.Add(y[intIndex]);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SelectRandomSampleData<T>(
            List<T> xy,
            double dblSampleSizeFactor,
            out List<T> trainXy)
        {
            trainXy = new List<T>();
            try
            {
                int intInstances = xy.Count;
                var rngWrapper = new RngWrapper();
                var intSampleSize = (int)(intInstances * dblSampleSizeFactor);
                for (int i = 0; i < intSampleSize; i++)
                {
                    int intIndex = rngWrapper.NextInt(0, intInstances - 1);
                    trainXy.Add(xy[intIndex]);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SelectRandomSampleData<T, U>(
            SortedDictionary<T, U> xy,
            double dblSampleSizeFactor,
            out SortedDictionary<T, U> trainXy,
            out SortedDictionary<T, U> validationXy)
        {
            List<KeyValuePair<T, U>> trainXyList;
            List<KeyValuePair<T, U>> validationXyList;
            SelectRandomSampleData(
                xy.ToList(),
                dblSampleSizeFactor,
                out trainXyList,
                out validationXyList);
            trainXy = new SortedDictionary<T, U>(trainXyList.ToDictionary(t => t.Key, t => t.Value));
            validationXy = new SortedDictionary<T, U>(validationXyList.ToDictionary(t => t.Key, t => t.Value));
        }

        public static void SelectRandomSampleData<T, U>(
            List<T> x,
            List<U> y,
            double dblSampleSizeFactor,
            out List<T> x1,
            out List<T> x2,
            out List<U> y1,
            out List<U> y2,
            out List<int> shuffledList1,
            out List<int> shuffledList2,
            RngWrapper rngWrapper = null)
        {
            x1 = new List<T>();
            x2 = new List<T>();
            y1 = new List<U>();
            y2 = new List<U>();
            shuffledList1 = new List<int>();
            shuffledList2 = new List<int>();

            try
            {
                int intInstances = x.Count;
                if (rngWrapper == null)
                {
                    rngWrapper = new RngWrapper();
                }

                List<int> shuffled = rngWrapper.GetShuffledList(intInstances);

                //
                // get validation set
                //
                var intSampleSize = (int)(Math.Round(intInstances * dblSampleSizeFactor, 0));
                int i = 0;
                for (; i < intSampleSize; i++)
                {
                    int intRandomIndex = shuffled[i];
                    x2.Add(x[intRandomIndex]);
                    y2.Add(y[intRandomIndex]);
                    shuffledList2.Add(intRandomIndex);
                }

                //
                // get training set
                //
                for (; i < intInstances; i++)
                {
                    int intRandomIndex = shuffled[i];
                    x1.Add(x[intRandomIndex]);
                    y1.Add(y[intRandomIndex]);
                    shuffledList1.Add(intRandomIndex);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SelectRandomSampleData<T>(
            List<T> xy,
            double dblSampleSizeFactor,
            out List<T> trainXy,
            out List<T> validationXy)
        {
            trainXy = new List<T>();
            validationXy = new List<T>();
            try
            {
                int intInstances = xy.Count;
                var rngWrapper = new RngWrapper();
                List<int> shuffledList = rngWrapper.GetShuffledList(intInstances);

                //
                // get validation set
                //
                var intSampleSize = (int)(Math.Round(intInstances * dblSampleSizeFactor, 0));
                int i = 0;
                for (; i < intSampleSize; i++)
                {
                    int intRandomIndex = shuffledList[i];
                    validationXy.Add(xy[intRandomIndex]);
                }

                //
                // get training set
                //
                for (; i < intInstances; i++)
                {
                    int intRandomIndex = shuffledList[i];
                    trainXy.Add(xy[intRandomIndex]);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static RegressionTrainerWrapper GetRegression(
            SortedDictionary<DateTime, double> yMap,
            SortedDictionary<DateTime, double[]> xMap)
        {
            try
            {
                List<DateTime> dateSet = GetIntersectionSet(
                    yMap.Keys.ToList(),
                    xMap.Keys.ToList());

                List<double[]> x = (from n in xMap
                                    where dateSet.Contains(n.Key)
                                    select n.Value).ToList();
                List<double> y = (from n in yMap
                                  where dateSet.Contains(n.Key)
                                  select n.Value).ToList();
                var regression = new RegressionTrainerWrapper(x, y);
                return regression;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static SortedDictionary<DateTime, double> GetLogRatios(
            SortedDictionary<DateTime, double> map1,
            SortedDictionary<DateTime, double> map2)
        {
            try
            {
                //var map1Arr = map1.ToArray();
                //var map2Arr = map2.ToArray();
                var ratios = new SortedDictionary<DateTime, double>();
                //if (map1Arr.Length != map2Arr.Length)
                //{
                //    throw new HCException("Invalid map size");
                //}
                foreach (var kvp in map1)
                {
                    double dblVal;
                    if (!map2.TryGetValue(
                        kvp.Key,
                        out dblVal))
                    {
                        
                    }
                    double dblRatio = GetLogRatio(
                        kvp.Value,
                        dblVal);
                    ratios[kvp.Key] = dblRatio;
                }
                return ratios;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double>();
        }

        public static double GetLogRatio(double dblVal1, double dblVal2)
        {
            try
            {
                if (dblVal1 == 0 || dblVal2 == 0)
                {
                    return 0;
                }
                return Math.Log(
                    Math.Abs(dblVal1 / dblVal2));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static SortedDictionary<DateTime, double> GetLogReturns(
            SortedDictionary<DateTime, double> map)
        {
            try
            {
                return GetLogReturns(map, 1);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double>();
        }

        public static double[] GetReturns(
            double[] mapArr)
        {
            return GetReturns(mapArr, 1);
        }

        public static double[] GetReturns(
            double[] mapArr,
            int intSize,
            bool blnUsePrc = false)
        {
            try
            {
                var returns = new List<double>(mapArr.Length);
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize];
                    double dblCurrVal = mapArr[i];
                    double dblReturn = dblCurrVal - dblPrevVal;

                    if (blnUsePrc)
                    {
                        if (dblPrevVal == 0)
                        {
                            dblReturn = 0;
                        }
                        else
                        {
                            dblReturn /= dblPrevVal;
                        }
                    }
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double[] GetAbsReturns(
            double[] mapArr)
        {
            return GetAbsReturns(mapArr, 1);
        }

        public static double[] GetAbsReturns(
            double[] mapArr,
            int intSize)
        {
            try
            {
                var returns = new List<double>(mapArr.Length);
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize];
                    double dblCurrVal = mapArr[i];
                    double dblReturn = Math.Abs(dblCurrVal - dblPrevVal);
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double[] GetAbsReturnsRatio(
            double[] mapArr)
        {
            return GetAbsReturnsRatio(mapArr, 1);
        }

        public static double[] GetAbsReturnsRatio(
            double[] mapArr,
            int intSize)
        {
            try
            {
                var returns = new List<double>(mapArr.Length);
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize];
                    double dblCurrVal = mapArr[i];
                    double dblReturn = Math.Abs(dblCurrVal - dblPrevVal)/dblPrevVal;
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static SortedDictionary<T, double> GetReturns<T>(
            SortedDictionary<T, double> map,
            int intSize = 1,
            bool blnUsePrc = false,
            int intSign = 1)
        {
            try
            {
                if(intSize == 0)
                {
                    return new SortedDictionary<T, double>(map);
                }

                KeyValuePair<T, double>[] mapArr = map.ToArray();
                var returns = new SortedDictionary<T, double>();
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize].Value;
                    double dblCurrVal = mapArr[i].Value;
                    if(double.IsNaN(dblCurrVal) ||
                        double.IsNaN(dblPrevVal))
                    {
                        returns[mapArr[i].Key] = double.NaN;
                        continue;
                    }

                    double dblReturn = intSign * (dblCurrVal - dblPrevVal);
                    if (blnUsePrc)
                    {
                        dblReturn /= dblPrevVal;
                    }

                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns[mapArr[i].Key] = dblReturn;
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static List<double> GetReturns(
            List<double> map,
            int intSize = 1,
            bool blnUsePrc = false,
            int intSign = 1)
        {
            try
            {
                if (intSize == 0)
                {
                    return new List<double>(map);
                }

                var mapArr = map.ToArray();
                var returns = new List<double>(
                    map.Count + 1);
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize];
                    double dblCurrVal = mapArr[i];
                    if (double.IsNaN(dblCurrVal) ||
                        double.IsNaN(dblPrevVal))
                    {
                        returns.Add(double.NaN);
                        continue;
                    }

                    double dblReturn = intSign * (dblCurrVal - dblPrevVal);
                    if (blnUsePrc)
                    {
                        dblReturn /= dblPrevVal;
                    }

                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List< double>();
        }

        public static SortedDictionary<T, double[]> GetReturns<T>(
            SortedDictionary<T, double[]> map,
            int intSize,
            bool blnUsePrc)
        {
            try
            {
                KeyValuePair<T, double[]>[] mapArr = map.ToArray();
                var returns = new SortedDictionary<T, double[]>();
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double[] dblPrevVal = mapArr[i - intSize].Value;
                    double[] dblCurrVal = mapArr[i].Value;
                    var dblReturn = GetReturn(
                        dblPrevVal,
                        dblCurrVal,
                        blnUsePrc);

                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns[mapArr[i].Key] = dblReturn;
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static List<double[]> GetReturns(
            List<double[]> map,
            int intSize,
            bool blnUsePrc)
        {
            try
            {
                var mapArr = map.ToArray();
                var returns = new List<double[]>(
                    map.Count+1);
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double[] dblPrevVal = mapArr[i - intSize];
                    double[] dblCurrVal = mapArr[i];
                    var dblReturn = GetReturn(
                        dblPrevVal,
                        dblCurrVal,
                        blnUsePrc);

                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[]>();
        }

        public static List<TsRow2D> GetReturns(
            List<TsRow2D> mapArr,
            int intSize)
        {
            try
            {
                var returns = new List<TsRow2D>();
                for (int i = intSize; i < mapArr.Count; i++)
                {
                    double dblPrevVal = mapArr[i - intSize].Fx;
                    double dblCurrVal = mapArr[i].Fx;
                    double dblReturn = dblCurrVal - dblPrevVal;
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(new TsRow2D(mapArr[i].Time, dblReturn));
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static double[] GetReturnRatios(
            double[] mapArr)
        {
            return GetReturnRatios(mapArr,
                             1);
        }

        public static double[] GetReturnRatios(
            double[] mapArr,
            int intSize)
        {
            try
            {
                var returns = new List<double>();
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize];
                    double dblCurrVal = mapArr[i];
                    double dblReturn = (dblCurrVal - dblPrevVal) / dblPrevVal;
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new double[0];
        }

        public static List<double> GetReturnRatios(
            List<double> mapArr,
            int intSize)
        {
            try
            {
                var returns = new List<double>();
                for (int i = intSize; i < mapArr.Count; i++)
                {
                    double dblPrevVal = mapArr[i - intSize];
                    double dblCurrVal = mapArr[i];
                    double dblReturn = (dblCurrVal - dblPrevVal) / dblPrevVal;
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns.Add(dblReturn);
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static SortedDictionary<T, double> GetLogReturns<T>(
            SortedDictionary<T, double> map,
            int intSize)
        {
            try
            {
                KeyValuePair<T, double>[] mapArr = map.ToArray();
                var returns = new SortedDictionary<T, double>();
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double dblPrevVal = mapArr[i - intSize].Value;
                    double dblCurrVal = mapArr[i].Value;

                    if (Double.IsNaN(dblPrevVal) ||
                        Double.IsNaN(dblCurrVal))
                    {
                        returns[mapArr[i].Key] = double.NaN;
                        continue;
                    }

                    double dblReturn;
                    if (dblCurrVal == 0 && dblPrevVal > 0)
                    {
                        dblReturn = -6;
                    }
                    else
                    {
                        if (dblPrevVal == 0)
                        {
                            dblPrevVal = 1e-6;
                        }
                        if (dblCurrVal == 0)
                        {
                            dblCurrVal = 1e-6;
                        }


                        dblReturn = Math.Log(dblCurrVal / dblPrevVal);
                    }
                    if (!IsValid(dblReturn))
                    {
                        throw new HCException("Invalid value");
                    }
                    returns[mapArr[i].Key] = dblReturn;
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static double[] GetLogReturn(
            double[] dblPrevVal,
            double[] dblCurrVal)
        {
            try
            {
                var dblReturn = new double[dblCurrVal.Length];
                for (int j = 0; j < dblCurrVal.Length; j++)
                {
                    dblReturn[j] = Math.Log(dblCurrVal[j] / dblPrevVal[j]);
                    if (!IsValid(dblReturn[j]))
                    {
                        throw new HCException("Invalid value");
                    }
                }
                return dblReturn;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double[] GetReturn(
            double[] dblPrevVal,
            double[] dblCurrVal,
            bool blnUsePrc)
        {
            try
            {
                var dblReturn = new double[dblCurrVal.Length];
                for (int j = 0; j < dblCurrVal.Length; j++)
                {
                    dblReturn[j] = dblCurrVal[j] - dblPrevVal[j];
                    if(blnUsePrc)
                    {
                        dblReturn[j] /= dblPrevVal[j];
                    }
                    if (!IsValid(dblReturn[j]))
                    {
                        throw new HCException("Invalid value");
                    }
                }
                return dblReturn;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static SortedDictionary<DateTime, double[]> GetLogReturns(
            SortedDictionary<DateTime, double[]> map,
            int intSize)
        {
            try
            {
                var mapArr = map.ToArray();
                var returns = new SortedDictionary<DateTime, double[]>();
                for (int i = intSize; i < mapArr.Length; i++)
                {
                    double[] dblPrevVal = mapArr[i - intSize].Value;
                    double[] dblCurrVal = mapArr[i].Value;
                    var dblReturn = new double[dblCurrVal.Length];
                    for (int j = 0; j < dblCurrVal.Length; j++)
                    {
                        dblReturn[j] = Math.Log(dblCurrVal[j] / dblPrevVal[j]);
                        if (!IsValid(dblReturn[j]))
                        {
                            throw new HCException("Invalid value");
                        }
                    }
                    returns[mapArr[i].Key] = dblReturn;
                }
                return returns;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static bool IsValid(double[] dblVal)
        {
            try
            {
                for (int i = 0; i < dblVal.Length; i++)
                {
                    if (!IsValid(dblVal[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static SortedDictionary<T, double> GetClassifierMap<T>(
            SortedDictionary<T, double> yMap,
            int intNumClasses,
            int intBaseClassificationIndex = 0,
            int intReturnPeriods = 1)
        {
            try
            {
                if (intBaseClassificationIndex < 0)
                {
                    throw new HCException("Negative base index");
                }

                SortedDictionary<T, double> returns;
                if (intReturnPeriods > 0)
                {
                    returns = GetReturns(
                        yMap,
                        intReturnPeriods,
                        true);
                }
                else
                {
                    returns =
                        new SortedDictionary<T, double>(
                            CloneMap(yMap));
                }

                if (intNumClasses == 2)
                {
                    // 0 = positive val, -1 negative val
                    var yMapClassification = new SortedDictionary<T, double>(
                        (from n in returns
                         select
                             new KeyValuePair<T, double>(
                             n.Key,
                             (intBaseClassificationIndex + GetBinaryClass(
                                 n.Value,
                                 0)))).ToDictionary(t => t.Key, t => t.Value));
                    return yMapClassification;
                }


                List<double> list = returns.Values.ToList();
                list.Sort();
                int intSize = list.Count/intNumClasses;
                int intCurrSize = intSize;

                var histogram = new double[intNumClasses];
                for (int i = 0; i < intNumClasses; i++)
                {
                    if(i == intNumClasses - 1)
                    {
                        histogram[i] = list.Last();
                    }
                    else  
                    {
                        histogram[i] = list[intCurrSize];
                    }
                    intCurrSize += intSize;
                }

                var yMapClassi = new SortedDictionary<T, double>();
                foreach (KeyValuePair<T, double> keyValuePair in returns)
                {
                    for (int i = 0; i < intNumClasses; i++)
                    {
                        if(keyValuePair.Value <= histogram[i])
                        {
                            yMapClassi[keyValuePair.Key] = i + intBaseClassificationIndex;
                            break;
                        }
                    }
                }

                return yMapClassi;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static bool IsValid(double dblVal)
        {
            try
            {
                return !Double.IsNaN(dblVal) && !Double.IsInfinity(dblVal);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static int GetBinaryClass(
            double dblValue,
            double dblThreshold)
        {
            return dblValue > dblThreshold ? 0 : 1; // this is a new change and may bring  problems
        }

        public static SortedDictionary<T, double> NormalizeFeatures<T>(
            SortedDictionary<T, double> features,
            out double dblDelta,
            out double dblMinVal)
        {
            dblMinVal = double.NaN;
            dblDelta = double.NaN;
            try
            {
                if(features == null ||
                    features.Count == 0)
                {
                    return new SortedDictionary<T, double>();
                }

                var validVals = (from n in features
                                 where !Double.IsNaN(n.Value)
                                 select n.Value).ToList();

                if(validVals.Count == 0)
                {
                    return new SortedDictionary<T, double>();
                }

                dblMinVal = validVals.Min();
                var dblMaxVal = validVals.Max();
                dblDelta = dblMaxVal - dblMinVal;
                SortedDictionary<T, double> newFeatMap = NormalizeFeatures(
                    features,
                    dblDelta,
                    dblMinVal);
                return newFeatMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static List<double> NormalizeFeatures(
            List<double> features,
            out double dblDelta,
            out double dblMinVal)
        {
            dblMinVal = double.NaN;
            dblDelta = double.NaN;
            try
            {
                dblMinVal = Double.MaxValue;
                var dblMaxVal = -Double.MaxValue;
                foreach (var keyValuePair in features)
                {
                    dblMinVal = Math.Min(dblMinVal, keyValuePair);
                    dblMaxVal = Math.Max(dblMaxVal, keyValuePair);
                }
                dblDelta = dblMaxVal - dblMinVal;
                List<double> newFeatMap = NormalizeFeatures(
                    features,
                    dblDelta,
                    dblMinVal);
                return newFeatMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static SortedDictionary<T, double> NormalizeFeatures<T>(
            SortedDictionary<T, double> features,
            double dblDelta,
            double dblMinVal)
        {
            try
            {
                var newFeatMap = new SortedDictionary<T, double>();
                foreach (KeyValuePair<T, double> keyValuePair in features)
                {
                    var dblCurrVal = keyValuePair.Value;
                    if (Double.IsNaN(dblCurrVal))
                    {
                        newFeatMap[keyValuePair.Key] = double.NaN;
                        continue;
                    }
                    double newFeat;
                    if (dblDelta == 0)
                    {
                        newFeat = 0;
                    }
                    else
                    {
                        newFeat = (dblCurrVal - dblMinVal) / dblDelta;
                    }
                    if (!IsValid(newFeat))
                    {
                        throw new HCException("Invalid value");
                    }
                    newFeatMap[keyValuePair.Key] = newFeat;
                }
                return newFeatMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static List<double> NormalizeFeatures(
            List<double> features,
            double dblDelta,
            double dblMinVal)
        {
            var newFeatMap = new List<double>();
            foreach (var keyValuePair in features)
            {
                var currFeat = keyValuePair;
                double newFeat;
                if (dblDelta == 0)
                {
                    newFeat = 0;
                }
                else
                {
                    newFeat = (currFeat - dblMinVal) / dblDelta;
                }
                if (!IsValid(newFeat))
                {
                    throw new HCException("Invalid value");
                }
                newFeatMap.Add(newFeat);
            }
            return newFeatMap;
        }


        public static SortedDictionary<T, double[]> NormalizeFeatures<T>(
            SortedDictionary<T, double[]> features,
            out double[] dblDelta,
            out double[] dblMinVal)
        {
            dblMinVal = null;
            dblDelta = null;
            try
            {
                if (features == null || features.Count == 0)
                {
                    return new SortedDictionary<T, double[]>();
                }

                int intVars = features.Values.First().Length;
                dblMinVal = new double[intVars];
                var dblMaxVal = new double[intVars];
                dblDelta = new double[intVars];
                for (int i = 0; i < intVars; i++)
                {
                    dblMinVal[i] = double.MaxValue;
                    dblMaxVal[i] = -double.MaxValue;
                }
                var featList = features.ToList();
                int intN = featList.Count;
                for (int j = 0; j < intN; j++)
                {
                    double[] featCurr = featList[j].Value;
                    for (int i = 0; i < intVars; i++)
                    {
                        double dblCurrVal = featCurr[i];
                        dblMinVal[i] = Math.Min(dblMinVal[i], dblCurrVal);
                        dblMaxVal[i] = Math.Max(dblMaxVal[i], dblCurrVal);
                    }
                }
                for (int i = 0; i < intVars; i++)
                {
                    dblDelta[i] = dblMaxVal[i] - dblMinVal[i];
                }

                var newFeatMap =
                    new SortedDictionary<T, double[]>();
                for (int j = 0; j < intN; j++)
                {
                    var newFeat = new double[intVars];
                    var currKvp = featList[j];
                    var currFeat = currKvp.Value;
                    for (int i = 0; i < intVars; i++)
                    {
                        double dblNewVal = 0;
                        if (dblDelta[i] > 0)
                        {
                            dblNewVal = (currFeat[i] - dblMinVal[i])/dblDelta[i];
                        }
                        if (!IsValid(dblNewVal))
                        {
                            throw new HCException("Invalid value");
                        }
                        newFeat[i] = dblNewVal;
                    }
                    newFeatMap[currKvp.Key] = newFeat;
                }

                return newFeatMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static SortedDictionary<T, double[]> NormalizeFeatures<T>(
            SortedDictionary<T, double[]> features,
            double[] dblDelta,
            double[] dblMinVal,
            bool blnIsZeroOne = false)
        {
            try
            {
                if (features == null || features.Count == 0)
                {
                    return new SortedDictionary<T, double[]>();
                }

                int intVars = features.Values.First().Length;
                var featList = features.ToList();
                int intN = featList.Count;

                if (intVars != dblDelta.Length ||
                    intVars != dblMinVal.Length)
                {
                    throw new HCException("Invalid dimensions");
                }

                var newFeatMap =
                    new SortedDictionary<T, double[]>();
                for (int j = 0; j < intN; j++)
                {
                    var newFeat = new double[intVars];
                    var currKvp = featList[j];
                    var currFeat = currKvp.Value;
                    for (int i = 0; i < intVars; i++)
                    {
                        double dblNewVal = 0;
                        if (dblDelta[i] > 0)
                        {
                            dblNewVal = (currFeat[i] - dblMinVal[i]) / dblDelta[i];
                        }
                        if (!IsValid(dblNewVal))
                        {
                            throw new HCException("Invalid value");
                        }
                        if (blnIsZeroOne)
                        {
                            dblNewVal = Math.Max(dblNewVal, 0);
                            dblNewVal = Math.Min(dblNewVal, 1);
                        }
                        newFeat[i] = dblNewVal;
                    }
                    newFeatMap[currKvp.Key] = newFeat;
                }

                return newFeatMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static double[] NormalizeFeature(
            double[] dblDelta,
            double[] dblMinVal,
            double[] currFeat)
        {
            try
            {
                int intVars = dblDelta.Length;
                var newFeat = new double[intVars];
                for (int i = 0; i < intVars; i++)
                {
                    if (dblDelta[i] == 0)
                    {
                        newFeat[i] = 0;
                    }
                    else
                    {
                        newFeat[i] = (currFeat[i] - dblMinVal[i]) / dblDelta[i];
                    }
                    if (!IsValid(newFeat[i]))
                    {
                        throw new HCException("Invalid value");
                    }
                }
                return newFeat;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double[] NormalizeFeature(
            double[] dblDelta,
            double[] dblMinVal,
            double[] currFeat,
            double[] meanFeat,
            double[] stdDevFeat)
        {
            try
            {
                int intVars = dblDelta.Length;
                var newFeat = new double[intVars];
                for (int i = 0; i < intVars; i++)
                {
                    newFeat[i] = (currFeat[i] - meanFeat[i]) / stdDevFeat[i];
                }
                for (int i = 0; i < intVars; i++)
                {
                    if (dblDelta[i] == 0)
                    {
                        newFeat[i] = 0;
                    }
                    else
                    {
                        newFeat[i] = (currFeat[i] - dblMinVal[i]) / dblDelta[i];
                    }
                    //
                    // make sure the feature is in [0,1] range
                    //
                    newFeat[i] = Math.Min(newFeat[i], 1);
                    newFeat[i] = Math.Max(newFeat[i], 0);
                    if (!IsValid(newFeat[i]))
                    {
                        throw new HCException("Invalid value");
                    }
                }
                return newFeat;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static SortedDictionary<T, double[]> NormalizeFeatures<T>(
            SortedDictionary<T, double[]> features)
        {
            try
            {
                if (features == null ||
                    features.Count == 0)
                {
                    return new SortedDictionary<T, double[]>();
                }

                int intVars = features.First().Value.Length;
                var minArr = new double[intVars];
                var maxArr = new double[intVars];
                for (int i = 0; i < intVars; i++)
                {
                    minArr[i] = Double.MaxValue;
                    maxArr[i] = -Double.MaxValue;
                }
                foreach (KeyValuePair<T, double[]> keyValuePair in features)
                {
                    for (int i = 0; i < intVars; i++)
                    {
                        minArr[i] = Math.Min(minArr[i], keyValuePair.Value[i]);
                        maxArr[i] = Math.Max(maxArr[i], keyValuePair.Value[i]);
                    }
                }
                var newFeatMap = new SortedDictionary<T, double[]>();
                foreach (KeyValuePair<T, double[]> keyValuePair in features)
                {
                    var currFeat = keyValuePair.Value;
                    var newFeat = new double[intVars];
                    for (int i = 0; i < intVars; i++)
                    {
                        double dblDelta = maxArr[i] - minArr[i];
                        if (dblDelta == 0)
                        {
                            newFeat[i] = 0;
                        }
                        else
                        {
                            newFeat[i] = (currFeat[i] - minArr[i]) / dblDelta;
                        }
                        if (!IsValid(newFeat[i]))
                        {
                            throw new HCException("Invalid value");
                        }
                    }
                    newFeatMap[keyValuePair.Key] = newFeat;
                }
                return newFeatMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static SortedDictionary<T, double> GetLogFeatures<T>(
            SortedDictionary<T, double> features)
        {
            try
            {
                var logFeatures = new SortedDictionary<T, double>();
                foreach (KeyValuePair<T, double> keyValuePair in features)
                {
                    double dblCurrVal = keyValuePair.Value;
                    double dblLogVal = GetLogValue(dblCurrVal);
                    logFeatures[keyValuePair.Key] = dblLogVal;
                }
                return logFeatures;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static List<double> GetLogFeatures(
            List<double> features)
        {
            try
            {
                var logFeatures = new List<double>();
                foreach (double keyValuePair in features)
                {
                    double currFeat = keyValuePair;
                    double logFeat = GetLogValue(currFeat);
                    logFeatures.Add(logFeat);
                }
                return logFeatures;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static SortedDictionary<T, double[]> GetLogFeatures<T>(
            SortedDictionary<T, double[]> features)
        {
            try
            {
                int intVars = features.First().Value.Length;
                var logFeatures = new SortedDictionary<T, double[]>();
                foreach (KeyValuePair<T, double[]> keyValuePair in features)
                {
                    double[] logFeat = GetLogFeature(
                        keyValuePair.Value,
                        intVars);
                    logFeatures[keyValuePair.Key] = logFeat;
                }
                return logFeatures;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static double[] GetLogFeature(
            double[] currFeat,
            int intVars)
        {
            try
            {
                //int intVars = currFeat.Length;
                var logFeatures = new double[intVars];
                for (int i = 0; i < intVars; i++)
                {
                    double dblVal = GetLogValue(
                        currFeat[i]);
                    logFeatures[i] = dblVal;
                }
                return logFeatures;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private static double GetLogValue(double dblValue)
        {
            try
            {
                if (Double.IsNaN(dblValue))
                {
                    return double.NaN;
                }
                dblValue += 1.0;
                if (dblValue <= 0)
                {
                    return -1e-10;
                }
                double dblVal = Math.Log(dblValue);
                if (!IsValid(dblVal))
                {
                    throw new HCException("Invalid value");
                }
                return dblVal;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static SortedDictionary<T, double[]> GetPcaMap<T>(
            SortedDictionary<T, double[]> xVars,
            int intPcaSize)
        {
            double[,] basis;
            return GetPcaMap(xVars, intPcaSize, out basis);
        }

        public static SortedDictionary<T, double[]> ApplyPcaMap<T>(
            SortedDictionary<T, double[]> xVars,
            int intPcaSize,
            double[,] basis)
        {
            try
            {
                if (xVars == null ||
                    xVars.Count == 0 ||
                    xVars.First().Value.Length <= intPcaSize)
                {
                    return new SortedDictionary<T, double[]>();
                }

                KeyValuePair<T, double[]>[] xVarsArr;
                double[,] pcaVector = GetPcaVector(xVars, out xVarsArr);
                //var pcaHc = new PcaHc(pcaVector);

                double[,] pcaValues = PcaHc.ApplyPca(
                    pcaVector,
                    basis);

                SortedDictionary<T, double[]> pcaMap = ApplyPcaValuesMap(
                    intPcaSize,
                    xVarsArr,
                    pcaValues);
                return pcaMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static SortedDictionary<T, double[]> GetPcaMap<T>(
            SortedDictionary<T, double[]> xVars,
            int intPcaSize,
            out double[,] basis)
        {
            basis = null;
            try
            {
                if (xVars == null ||
                    xVars.Count == 0 ||
                    xVars.First().Value.Length <= intPcaSize)
                {
                    if (xVars != null)
                    {
                        return new SortedDictionary<T, double[]>(xVars);
                    }
                    return new SortedDictionary<T, double[]>();
                }

                KeyValuePair<T, double[]>[] xVarsArr;
                double[,] pcaVector = GetPcaVector(xVars, out xVarsArr);
                var pcaHc = new PcaHc(pcaVector);

                double[,] pcaValues = pcaHc.GetPcaData(
                    pcaVector,
                    intPcaSize,
                    out basis);

                SortedDictionary<T, double[]> pcaMap = ApplyPcaValuesMap(
                    intPcaSize,
                    xVarsArr,
                    pcaValues);
                return pcaMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static List<double[]> GetPcaList(
            List<double[]> xVars,
            int intPcaSize,
            out double[,] basis)
        {
            basis = null;
            try
            {
                if (xVars == null ||
                    xVars.Count == 0 ||
                    xVars.First().Length <= intPcaSize)
                {
                    return new List<double[]>();
                }

                double[,] pcaVector = GetPcaVector(xVars);
                var pcaHc = new PcaHc(pcaVector);
                double[,] pcaValues = pcaHc.GetPcaData(
                    pcaVector,
                    intPcaSize,
                    out basis);

                List<double[]> pcaList = ApplyPcaValuesList(
                    intPcaSize,
                    xVars,
                    pcaValues);
                return pcaList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[]>();
        }

        public static List<double[]> ApplyPcaList(
            List<double[]> xVars,
            int intPcaSize,
            double[,] basis)
        {
            try
            {
                if (xVars == null ||
                    xVars.Count == 0 ||
                    xVars.First().Length <= intPcaSize)
                {
                    return new List<double[]>();
                }

                double[,] pcaVector = GetPcaVector(xVars);
                double[,] pcaValues = PcaHc.ApplyPca(
                    pcaVector,
                    basis);

                List<double[]> pcaList = ApplyPcaValuesList(
                    intPcaSize,
                    xVars,
                    pcaValues);
                return pcaList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[]>();
        }

        public static double[,] GetPcaBasis(
            List<double[]> xVars,
            int intPcaSize)
        {
            try
            {
                if (xVars == null ||
                    xVars.Count == 0 ||
                    xVars.First().Length <= intPcaSize)
                {
                    return null;
                }

                double[,] pcaVector = GetPcaVector(xVars);
                var pcaHc = new PcaHc(pcaVector);
                double[,] pcaBasis = pcaHc.GetPcaBasis(
                    intPcaSize);
                return pcaBasis;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }


        private static List<double[]> ApplyPcaValuesList(
            int intPcaSize,
            List<double[]> xVarsArr,
            double[,] pcaValues)
        {
            try
            {
                var pcaMap = new List<double[]>();
                for (int i = 0; i < xVarsArr.Count; i++)
                {
                    var currFeat = new double[intPcaSize];
                    for (int j = 0; j < intPcaSize; j++)
                    {
                        currFeat[j] = pcaValues[i, j];
                    }
                    pcaMap.Add(currFeat);
                }
                return pcaMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[]>();
        }

        private static SortedDictionary<T, double[]> ApplyPcaValuesMap<T>(
            int intPcaSize,
            KeyValuePair<T, double[]>[] xVarsArr,
            double[,] pcaValues)
        {
            try
            {
                var pcaMap = new SortedDictionary<T, double[]>();
                for (int i = 0; i < xVarsArr.Length; i++)
                {
                    var currFeat = new double[intPcaSize];
                    for (int j = 0; j < intPcaSize; j++)
                    {
                        currFeat[j] = pcaValues[i, j];
                    }
                    pcaMap[xVarsArr[i].Key] = currFeat;
                }
                return pcaMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        private static double[,] GetPcaVector<T>(
            SortedDictionary<T, double[]> xVars,
            out KeyValuePair<T, double[]>[] xVarsArr)
        {
            int intVars = xVars.First().Value.Length;
            var pcaVector = new double[xVars.Count, intVars];
            xVarsArr = xVars.ToArray();
            for (int i = 0; i < xVarsArr.Length; i++)
            {
                for (int j = 0; j < intVars; j++)
                {
                    pcaVector[i, j] = xVarsArr[i].Value[j];
                }
            }
            return pcaVector;
        }

        private static double[,] GetPcaVector(
            List<double[]> xVars)
        {
            int intVars = xVars.First().Length;
            var pcaVector = new double[xVars.Count, intVars];
            for (int i = 0; i < xVars.Count; i++)
            {
                for (int j = 0; j < intVars; j++)
                {
                    pcaVector[i, j] = xVars[i][j];
                }
            }
            return pcaVector;
        }

        public static double[,] GetXy(
            List<double> yMap,
            List<double[]> xMap)
        {
            try
            {
                if (xMap.Count != yMap.Count)
                {
                    throw new HCException("Invalid map size");
                }

                if(xMap.Count == 0)
                {
                    return new double[0,0];
                }

                int intVars = xMap.First().Length;
                int intN = xMap.Count;
                var outArr = new double[intN, intVars + 1];
                //var xMapArr = xMap.ToArray();
                //var yMapArr = yMap.ToArray();
                for (int i = 0; i < intN; i++)
                {
                    for (int j = 0; j < intVars; j++)
                    {
                        outArr[i, j] = xMap[i][j];
                    }
                    outArr[i, intVars] = yMap[i];
                }
                return outArr;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }


        public static void GetXyVariables(
            SortedDictionary<DateTime, double[]> featuresMap,
            SortedDictionary<DateTime, double> inputYMap,
            int intForecast,
            out SortedDictionary<DateTime, double> yMap,
            out SortedDictionary<DateTime, double[]> xMap)
        {
            yMap = null;
            xMap = null;
            try
            {
                if (featuresMap == null ||
                    featuresMap.Count == 0 ||
                    inputYMap == null ||
                    inputYMap.Count == 0)
                {
                    return;
                }
                List<DateTime> dateSet = GetIntersectionSet(featuresMap.Keys.ToList(),
                                                    inputYMap.Keys.ToList());
                featuresMap = FilterMapByKeys(featuresMap, dateSet);
                inputYMap = FilterMapByKeys(inputYMap, dateSet);
                if (featuresMap.Count != inputYMap.Count)
                {
                    throw new HCException("Invalid map size");
                }

                LoadYMap(inputYMap, intForecast, out yMap);
                xMap = ShiftDataBackwards(intForecast, featuresMap);

                dateSet = GetIntersectionSet(xMap.Keys.ToList(),
                                     yMap.Keys.ToList());
                xMap = FilterMapByKeys(xMap, dateSet);
                yMap = FilterMapByKeys(yMap, dateSet);

                if (xMap.Count != yMap.Count)
                {
                    throw new HCException("Invalid map size");
                }
                var testMap = yMap;
                if ((from n in xMap
                     where !testMap.ContainsKey(n.Key)
                     select n).Any())
                {
                    throw new HCException("Invalid date set");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void LoadYMap(
            SortedDictionary<DateTime, double> inputYMap,
            int intForecast,
            out SortedDictionary<DateTime, double> yMap)
        {
            yMap = null;
            try
            {
                yMap = CloneMap(inputYMap);
                DateTime lastDate = yMap.Keys.Last();
                double dblLastValue = yMap.Values.Last();
                for (int i = 0; i < intForecast; i++)
                {
                    lastDate = DateHelper.GetNextWorkingDate(lastDate, false);
                    yMap[lastDate] = dblLastValue;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static SortedDictionary<DateTime, double[]> ExtractPca(
            List<string> featureNames,
            SortedDictionary<DateTime, double[]> xMapInSample,
            int intPcaSize,
            out List<int> selectedIndexes)
        {
            selectedIndexes = null;
            try
            {
                int intInitVarCount = xMapInSample.First().Value.Length;
                selectedIndexes = new List<int>();
                for (int i = 0; i < intPcaSize; i++)
                {
                    int intPcaIndex = featureNames.IndexOf("pca_" + i);
                    if (intPcaSize < 0)
                    {
                        throw new HCException("Pca index not found");
                    }
                    selectedIndexes.Add(intPcaIndex);
                }

                int intPcaVars = Math.Min(intPcaSize, 10);
                var xMapInSampleNew = new SortedDictionary<DateTime, double[]>();
                foreach (KeyValuePair<DateTime, double[]> keyValuePair in xMapInSample)
                {
                    //
                    // get pca feat
                    //
                    var pcaFeat = new double[intPcaVars];
                    int k = 0;
                    for (int i = intInitVarCount - intPcaSize; i < intInitVarCount; i++)
                    {
                        if (k < intPcaVars)
                        {
                            pcaFeat[k] = keyValuePair.Value[i];
                        }
                        k++;
                    }
                    xMapInSampleNew[keyValuePair.Key] = pcaFeat.ToArray();
                }
                return xMapInSampleNew;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static void SplitMapHead<T>(
            SortedDictionary<DateTime, T> currXMap,
            DateTime dateTime,
            int intInSampleSize,
            out SortedDictionary<DateTime, T> xMap0,
            out SortedDictionary<DateTime, T> xMap1)
        {
            xMap0 = new SortedDictionary<DateTime, T>();
            xMap1 = new SortedDictionary<DateTime, T>();
            try
            {
                if (currXMap == null || currXMap.Count == 0)
                {
                    return;
                }

                int intDateIndex = currXMap.Keys.ToList().IndexOf(dateTime);
                if (intDateIndex < 0)
                {
                    intDateIndex = Math.Abs(TimeSeriesUtils.GetIndexOfDate(
                        currXMap.Keys.ToList(),
                        dateTime));
                }

                KeyValuePair<DateTime, T>[] currXMapArr = currXMap.ToArray();
                int intStartIndex = Math.Min(currXMapArr.Length - 1,
                                             intDateIndex - intInSampleSize + 1);
                if (intStartIndex < 0)
                {
                    intStartIndex = 0;
                    //
                    // we should get the whole data set, since we do not have enough stats
                    //
                    intDateIndex = intInSampleSize + 1; // plus one mans that we ignore the date item
                }
                intStartIndex =
                    Math.Max(
                        0,
                        intStartIndex);
                xMap0 = new SortedDictionary<DateTime, T>();
                xMap1 = new SortedDictionary<DateTime, T>();
                for (int i = intStartIndex; i < currXMapArr.Length; i++)
                {
                    KeyValuePair<DateTime, T> kvp = currXMapArr[i];
                    if (kvp.Key != dateTime && i <= intDateIndex)
                    {
                        xMap0[kvp.Key] = kvp.Value;
                    }
                    else if (kvp.Key == dateTime)
                    {
                        xMap1[kvp.Key] = kvp.Value;
                    }
                }
                if (xMap0.Count != intInSampleSize ||
                    (xMap0.Count > 0 && xMap0.Last().Key != dateTime))
                {
                    Logger.Log("Invalid map size");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void MergeFeatures(
            ref SortedDictionary<DateTime, double> yMap,
            ref SortedDictionary<DateTime, double[]> dummyForecastItemsMap,
            ref SortedDictionary<DateTime, double[]> xMapAllFeatures)
        {
            try
            {
                CheckDates(ref yMap, ref dummyForecastItemsMap, ref xMapAllFeatures);
                xMapAllFeatures = MergeFeatures(xMapAllFeatures, dummyForecastItemsMap);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void CheckDates(
            ref SortedDictionary<DateTime, double> yMap,
            ref SortedDictionary<DateTime, double[]> dummyForecastItemsMap,
            ref SortedDictionary<DateTime, double[]> xMapAllFeatures)
        {
            try
            {
                var allDateSet = GetIntersectionSet(
                    xMapAllFeatures.Keys.ToList(),
                    yMap.Keys.ToList());
                allDateSet = GetIntersectionSet(allDateSet, dummyForecastItemsMap.Keys.ToList());
                yMap = FilterMapByKeys(yMap, allDateSet);
                xMapAllFeatures = FilterMapByKeys(xMapAllFeatures, allDateSet);
                dummyForecastItemsMap = FilterMapByKeys(dummyForecastItemsMap, allDateSet);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static SortedDictionary<DateTime, double[]> GetAutoRegressiveMap(
            int intWindow,
            SortedDictionary<DateTime, double> data)
        {
            try
            {
                var features = new SortedDictionary<DateTime, double[]>();
                var dataArr = data.ToArray();
                for (int i = intWindow; i < dataArr.Length; i++)
                {
                    var currFeat = new List<double>();
                    for (int j = 0; j < intWindow; j++)
                    {
                        //
                        // include current value
                        //
                        currFeat.Add(dataArr[i - j].Value);
                    }
                    features[dataArr[i].Key] = currFeat.ToArray();
                }
                return features;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static SortedDictionary<DateTime, double[]> GetAutoRegressiveMapIncluded(
            int intWindow,
            SortedDictionary<DateTime, double> data)
        {
            try
            {
                var features = new SortedDictionary<DateTime, double[]>();
                var dataArr = data.ToArray();
                for (int i = intWindow - 1; i < dataArr.Length; i++)
                {
                    var currFeat = new List<double>();
                    for (int j = 0; j < intWindow; j++)
                    {
                        //
                        // include current value
                        //
                        currFeat.Add(dataArr[i - intWindow + 1 + j].Value);
                    }
                    features[dataArr[i].Key] = currFeat.ToArray();
                }
                return features;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static void CheckDates(
            ref List<TsRow2D> list1,
            ref List<TsRow2D> list2)
        {
            try
            {
                var map1 = new SortedDictionary<DateTime, TsRow2D>(list1.ToDictionary(
                    t => t.Time,
                    t => t));
                var map2 = new SortedDictionary<DateTime, TsRow2D>(list2.ToDictionary(
                    t => t.Time,
                    t => t));
                CheckDates(
                    ref map1, ref map2);
                list1 = map1.Values.ToList();
                list2 = map2.Values.ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        public static void CheckDates<K, T, U>(
            ref SortedDictionary<K, T> xMap,
            ref SortedDictionary<K, U> yMap)
        {
            try
            {
                var allDateSet = GetIntersectionSet(
                    xMap.Keys.ToList(),
                    yMap.Keys.ToList());
                yMap = FilterMapByKeys(yMap, allDateSet);
                xMap = FilterMapByKeys(xMap, allDateSet);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void CheckKeys<T, U, V>(
            ref SortedDictionary<T, U> xMap,
            ref SortedDictionary<T, V> yMap)
        {
            try
            {
                var allDateSet = GetIntersectionSet(
                    xMap.Keys.ToList(),
                    yMap.Keys.ToList());
                yMap = FilterMapByKeys(yMap, allDateSet);
                xMap = FilterMapByKeys(xMap, allDateSet);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SplitListHead(
            List<DateTime> list,
            DateTime splitDate,
            out List<DateTime> list1,
            out List<DateTime> list2)
        {
            list1 = null;
            list2 = null;
            try
            {
                if (list == null || list.Count == 0)
                {
                    return;
                }

                int intDateIndex = list.IndexOf(splitDate);
                if (intDateIndex < 0)
                {
                    //
                    // date not found
                    //
                    intDateIndex = Math.Abs(TimeSeriesUtils.GetIndexOfDate(
                        list,
                        splitDate));
                }
                SplitListHead(
                    list,
                    intDateIndex,
                    out list1,
                    out list2);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SplitListAtDate(
            List<DateTime> list,
            DateTime splitDate,
            out List<DateTime> list1,
            out List<DateTime> list2)
        {
            list1 = new List<DateTime>();
            list2 = new List<DateTime>();
            try
            {
                if (list == null || list.Count == 0)
                {
                    return;
                }
                int intDateIndex = list.IndexOf(splitDate);
                if (intDateIndex < 0)
                {
                    //
                    // Date not found
                    //
                    intDateIndex = Math.Abs(TimeSeriesUtils.GetIndexOfDate(
                        list,
                        splitDate));
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (i < intDateIndex)
                    {
                        list1.Add(list[i]);
                    }
                    else
                    {
                        list2.Add(list[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SplitListTail<T>(
            List<T> list,
            int intSize,
            out List<T> list1,
            out List<T> list2)
        {
            list1 = new List<T>();
            list2 = new List<T>();
            try
            {
                if (list == null || list.Count == 0)
                {
                    return;
                }

                int intN = list.Count;
                int intCutIndex = intN - intSize;
                if (intCutIndex < 0)
                {
                    intCutIndex = 0;
                }
                for (int i = 0; i < intN; i++)
                {
                    if (i < intCutIndex)
                    {
                        list1.Add(list[i]);
                    }
                    else
                    {
                        list2.Add(list[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void SplitListHead<T>(
            List<T> list,
            int intSize,
            out List<T> list1,
            out List<T> list2)
        {
            list1 = new List<T>();
            list2 = new List<T>();
            try
            {
                if (list == null ||
                    list.Count == 0)
                {
                    return;
                }

                int intN = list.Count;
                int intCutIndex = intSize;
                for (int i = 0; i < intN; i++)
                {
                    if (i < intCutIndex)
                    {
                        list1.Add(list[i]);
                    }
                    else
                    {
                        list2.Add(list[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<T> SelectLastNFromList<T>(
            int intN,
            List<T> list)
        {
            try
            {
                var outList = new List<T>();
                for (int i = Math.Max(0, list.Count - intN); i < list.Count; i++)
                {
                    outList.Add(list[i]);
                }
                return outList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<T>();
        }

        public static List<T> SelectTopNFromList<T>(
            int intN,
            List<T> list)
        {
            try
            {
                var outList = new List<T>();
                for (int i = 0; i < Math.Min(list.Count, intN); i++)
                {
                    outList.Add(list[i]);
                }
                return outList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<T>();
        }


        public static double GetHarmonicMean(
            double dblVal1,
            double dblVal2)
        {
            try
            {
                return (2.0 * dblVal1 * dblVal2) / (dblVal1 + dblVal2);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static double GetHarmonicMean(List<double> list)
        {
            try
            {
                if(list.Count == 1)
                {
                    return list[0];
                }

                double dblInvSum = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    dblInvSum += 1.0 / list[i];
                }
                return list.Count / dblInvSum;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static double GetHarmonicMean(double[] list)
        {
            try
            {
                int intN = list.Length;
                if (intN == 1)
                {
                    return list[0];
                }
                double dblInvSum = 0;
                for (int i = 0; i < intN; i++)
                {
                    dblInvSum += 1.0 / list[i];
                }
                return intN / dblInvSum;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static SortedDictionary<DateTime, double> SelectVariable(
            SortedDictionary<DateTime, double[]> xMap, 
            int i)
        {
            try
            {
                var outMap = new SortedDictionary<DateTime, double>();
                foreach (KeyValuePair<DateTime, double[]> keyValuePair in xMap)
                {
                    outMap[keyValuePair.Key] = keyValuePair.Value[i];
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double>();
        }

        public static List<double> SelectVariableToList(
            SortedDictionary<DateTime, double[]> xMap,
            int i)
        {
            try
            {
                var outMap = new List<double>();
                foreach (KeyValuePair<DateTime, double[]> keyValuePair in xMap)
                {
                    outMap.Add(keyValuePair.Value[i]);
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static double[] GetFeature(double[] doubles, List<int> currSelectedVars)
        {
            try
            {
                var newFeat = new double[currSelectedVars.Count];
                int i = 0;
                foreach (int intSelectedVar in currSelectedVars)
                {
                    newFeat[i] = doubles[intSelectedVar];
                    i++;
                }
                return newFeat;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static SortedDictionary<string, T> CloneMap<T>(
            SortedDictionary<string, T> map)
        {
            try
            {
                return new SortedDictionary<string, T>(map.ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<string, T>();
        }

        public static double GetRSquared(
            List<double> y,
            List<double> yForecast)
        {
            try
            {
                double dblYAvg = y.Average();
                double dblSumTot = (from n in y select Math.Pow(n - dblYAvg, 2)).Sum();
                double dblSumRes = 0;
                for (int i = 0; i < y.Count; i++)
                {
                    dblSumRes += Math.Pow(yForecast[i] - y[i], 2);
                }
                return 1.0 - dblSumRes / dblSumTot;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static SortedDictionary<K, T> CloneMap<T, K>(
            SortedDictionary<K, T> map)
        {
            try
            {
                return new SortedDictionary<K, T>(map.ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<K, T>();
        }

        public static double GetMrse(
            List<double> errors)
        {
            try
            {
                return Math.Sqrt((from n in errors select n * n).Sum() / (errors.Count - 1));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static double GetMrse(
            double[] errors)
        {
            try
            {
                return Math.Sqrt((from n in errors select n * n).Sum() / (errors.Length - 1));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static bool IsValid(List<double> currFeat, out int intIndex)
        {
            intIndex = -1;
            try
            {
                for (int i = 0; i < currFeat.Count; i++)
                {
                    if (!IsValid(currFeat[i]))
                    {
                        intIndex = i;
                        return false;
                    }
                }
                intIndex = -1;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static SortedDictionary<DateTime, double[]> RemoveIndexesFromData(
            SortedDictionary<DateTime, double[]> dataMap,
            List<int> varsToRemove)
        {
            try
            {
                var outDataMap = new SortedDictionary<DateTime, double[]>();
                foreach (KeyValuePair<DateTime, double[]> keyValuePair in dataMap)
                {
                    var currVector = new List<double>();
                    for (int i = 0; i < keyValuePair.Value.Length; i++)
                    {
                        if (!varsToRemove.Contains(i))
                        {
                            currVector.Add(keyValuePair.Value[i]);
                        }
                    }
                    outDataMap[keyValuePair.Key] = currVector.ToArray();
                }
                return outDataMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, double[]>();
        }

        public static int GetMostCorrelatedIndex(
            SortedDictionary<DateTime, double> yMap,
            SortedDictionary<DateTime, double[]> xMap)
        {
            try
            {
                if (yMap == null ||
                    yMap.Count == 0 ||
                    xMap == null ||
                    xMap.Count == 0)
                {
                    return -1;
                }
                return GetMostCorrelatedIndex(yMap.Values.ToList(),
                                              xMap.Values.ToList(),
                                              new List<int>());
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }

        public static double GetAdjustedCoeffDeterm(
            List<double> yVars,
            List<double> residuals,
            int intVars)
        {
            try
            {
                double dblCoeffDeterm = GetCoeffDeterm(yVars, residuals);
                return dblCoeffDeterm - ((1.0 - dblCoeffDeterm) * (intVars / (yVars.Count - intVars - 1.0)));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static double GetCoeffDeterm(
            List<double> yData,
            List<double> errors)
        {
            try
            {
                double dblYAvg = yData.Average();
                double dblSsTot = (from n in yData select Math.Pow(n - dblYAvg, 2)).Sum();
                double dblSsErr = (from n in errors select Math.Pow(n, 2)).Sum();
                return 1.0 - (dblSsErr / dblSsTot);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public static string GetMostCorrelatedVaraible(
            List<double> yData,
            Dictionary<string, List<double>> features)
        {
            try
            {
                double dblMaxCorrelation = -Double.MaxValue;
                string strSelectVar = string.Empty;

                foreach (KeyValuePair<string, List<double>> kvp in features)
                {
                    List<double> currVector = kvp.Value;
                    double dblAbsCorr = Math.Abs(Correlation.GetCorrelation(
                        yData,
                        currVector));

                    if (dblMaxCorrelation < dblAbsCorr)
                    {
                        dblMaxCorrelation = dblAbsCorr;
                        strSelectVar = kvp.Key;
                    }
                }
                return strSelectVar;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return string.Empty;
        }

        public static void GetRandomSampleFeatures<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double> yMap,
            double dblProportion,
            out SortedDictionary<T, double[]> xMapTrain,
            out SortedDictionary<T, double> yMapTrain,
            out SortedDictionary<T, double[]> xMapTest,
            out SortedDictionary<T, double> yMapTest)
        {
            xMapTrain = null;
            yMapTrain = null;
            xMapTest = null;
            yMapTest = null;
            try
            {
                var dates = xMap.Keys.ToList();
                var rng = new RngWrapper();
                rng.Shuffle(dates);
                List<T> dates1;
                List<T> dates2;
                SplitListHead(
                    dates,
                    (int)(dblProportion * dates.Count),
                    out dates1,
                    out dates2);
                xMapTrain = FilterMapByKeys(xMap, dates1);
                xMapTest = FilterMapByKeys(xMap, dates2);
                yMapTrain = FilterMapByKeys(yMap, dates1);
                yMapTest = FilterMapByKeys(yMap, dates2);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void GetRandomSampleFeatures<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double[]> xMapAll,
            SortedDictionary<T, double> yMap,
            double dblProportion,
            out SortedDictionary<T, double[]> xMapTrain,
            out SortedDictionary<T, double[]> xMapTrainAll,
            out SortedDictionary<T, double> yMapTrain,
            out SortedDictionary<T, double[]> xMapTest,
            out SortedDictionary<T, double[]> xMapTestAll,
            out SortedDictionary<T, double> yMapTest)
        {
            xMapTrain = null;
            xMapTrainAll = null;
            yMapTrain = null;
            xMapTest = null;
            xMapTestAll = null;
            yMapTest = null;
            try
            {
                var dates = xMap.Keys.ToList();
                var rng = new RngWrapper();
                rng.Shuffle(dates);
                GetSampleFeatures(
                    xMap,
                    xMapAll,
                    yMap,
                    dblProportion,
                    out xMapTrain,
                    out xMapTrainAll,
                    out yMapTrain,
                    out xMapTest,
                    out xMapTestAll,
                    out yMapTest,
                    dates);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void GetSampleFeatures<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double[]> xMapAll,
            SortedDictionary<T, double> yMap,
            double dblProportion,
            out SortedDictionary<T, double[]> xMapTrain,
            out SortedDictionary<T, double[]> xMapTrainAll,
            out SortedDictionary<T, double> yMapTrain,
            out SortedDictionary<T, double[]> xMapTest,
            out SortedDictionary<T, double[]> xMapTestAll,
            out SortedDictionary<T, double> yMapTest)
        {
            xMapTrain = null;
            xMapTrainAll = null;
            yMapTrain = null;
            xMapTest = null;
            xMapTestAll = null;
            yMapTest = null;
            try
            {
                var dates = xMap.Keys.ToList();
                GetSampleFeatures(
                    xMap,
                    xMapAll,
                    yMap,
                    dblProportion,
                    out xMapTrain,
                    out xMapTrainAll,
                    out yMapTrain,
                    out xMapTest,
                    out xMapTestAll,
                    out yMapTest,
                    dates);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void GetSampleFeatures<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double[]> xMapAll,
            SortedDictionary<T, double> yMap,
            double dblProportion,
            out SortedDictionary<T, double[]> xMapTrain,
            out SortedDictionary<T, double[]> xMapTrainAll,
            out SortedDictionary<T, double> yMapTrain,
            out SortedDictionary<T, double[]> xMapTest,
            out SortedDictionary<T, double[]> xMapTestAll,
            out SortedDictionary<T, double> yMapTest,
            List<T> dates)
        {
            xMapTrain = null;
            xMapTrainAll = null;
            yMapTrain = null;
            xMapTest = null;
            xMapTestAll = null;
            yMapTest = null;
            try
            {
                List<T> dates1;
                List<T> dates2;
                SplitListHead(
                    dates,
                    (int)(dblProportion * dates.Count),
                    out dates1,
                    out dates2);
                xMapTrain = FilterMapByKeys(xMap, dates1);
                xMapTrainAll = FilterMapByKeys(xMapAll, dates1);
                xMapTest = FilterMapByKeys(xMap, dates2);
                xMapTestAll = FilterMapByKeys(xMapAll, dates2);
                yMapTrain = FilterMapByKeys(yMap, dates1);
                yMapTest = FilterMapByKeys(yMap, dates2);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public static void GetSampleFeatures<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double> yMap,
            double dblProportion,
            out SortedDictionary<T, double[]> xMapTrain,
            out SortedDictionary<T, double> yMapTrain,
            out SortedDictionary<T, double[]> xMapTest,
            out SortedDictionary<T, double> yMapTest)
        {
            GetSampleFeatures(
                xMap,
                yMap,
                dblProportion,
                out xMapTrain,
                out yMapTrain,
                out xMapTest,
                out yMapTest,
                false);

        }

        public static void GetSampleFeatures<T>(
            SortedDictionary<T, double[]> xMap,
            SortedDictionary<T, double> yMap,
            double dblProportion,
            out SortedDictionary<T, double[]> xMapTrain,
            out SortedDictionary<T, double> yMapTrain,
            out SortedDictionary<T, double[]> xMapTest,
            out SortedDictionary<T, double> yMapTest,
            bool blnDoShuffle)
        {
            xMapTrain = null;
            yMapTrain = null;
            xMapTest = null;
            yMapTest = null;
            try
            {
                List<T> dates = xMap.Keys.ToList();
                if (blnDoShuffle)
                {
                    new RngWrapper().Shuffle(dates);
                }

                List<T> dates1;
                List<T> dates2;
                SplitListHead(
                    dates,
                    (int)(dblProportion * dates.Count),
                    out dates1,
                    out dates2);
                xMapTrain = FilterMapByKeys(xMap, dates1);
                xMapTest = FilterMapByKeys(xMap, dates2);
                yMapTrain = FilterMapByKeys(yMap, dates1);
                yMapTest = FilterMapByKeys(yMap, dates2);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<double> CheckOutlayersInPredictions(
            List<double> predictions)
        {
            try
            {
                if (predictions == null || predictions.Count == 0)
                {
                    return new List<double>();
                }

                if (predictions.Count == 1)
                {
                    return predictions;
                }

                double dblStdDev = StdDev.GetSampleStdDev(predictions);
                double dblMean = predictions.Average();
                var predictionCheck = new List<double>();
                foreach (double dblPrediction0 in predictions)
                {
                    double dblPrediction = dblPrediction0;
                    double dblUpperLimit = dblMean + STD_DEV_OUTLAYER_FACTOR * dblStdDev;
                    double dblLowerLimit = Math.Max(0, dblMean - STD_DEV_OUTLAYER_FACTOR * dblStdDev);
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
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static SortedDictionary<T, U> TakeFirst<T, U>(
            SortedDictionary<T, U> data,
            int intNumOfFullSamples)
        {
            try
            {
                List<KeyValuePair<T, U>> last = TakeFirst(data.ToList(), intNumOfFullSamples);
                return new SortedDictionary<T, U>(
                    last.ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, U>();
        }

        public static SortedDictionary<T, U> TakeLast<T, U>(
            SortedDictionary<T, U> data,
            int intNumOfFullSamples)
        {
            try
            {
                List<KeyValuePair<T, U>> last = TakeLast(data.ToList(), intNumOfFullSamples);
                return new SortedDictionary<T, U>(
                    last.ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, U>();
        }

        public static List<T> TakeLast<T>(
            List<T> data,
            int intNumOfFullSamples)
        {
            try
            {
                if (data == null ||
                    data.Count == 0 ||
                    intNumOfFullSamples <= 0)
                {
                    return new List<T>();
                }
                data = data.ToList();
                data.Reverse();
                data = data.Take(intNumOfFullSamples).ToList();
                data.Reverse();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<T>();
        }

        public static List<T> TakeFirst<T>(
            List<T> data,
            int intNumOfFullSamples)
        {
            try
            {
                if (data == null ||
                    data.Count == 0)
                {
                    return new List<T>();
                }
                data = data.ToList();
                data = data.Take(intNumOfFullSamples).ToList();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<T>();
        }

        public static SortedDictionary<K, T> GetMapFromDates<T, K>(
            List<K> dateList,
            SortedDictionary<K, T> map)
        {
            try
            {
                if (map == null ||
                    map.Count == 0)
                {
                    return new SortedDictionary<K, T>();
                }
                if (dateList == null ||
                    dateList.Count == 0)
                {
                    return new SortedDictionary<K, T>();
                }
                return new SortedDictionary<K, T>((from n in map
                                                   where dateList.Contains(n.Key)
                                                   select n).ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<K, T>();
        }

        public static SortedDictionary<K, T> RemoveDatesFromMap<T, K>(
            List<K> dateList,
            SortedDictionary<K, T> map)
        {
            try
            {
                if (map == null ||
                    map.Count == 0)
                {
                    return new SortedDictionary<K, T>();
                }
                if (dateList == null ||
                    dateList.Count == 0)
                {
                    return CloneMap(map);
                }
                return new SortedDictionary<K, T>((from n in map
                                                   where !dateList.Contains(n.Key)
                                                   select n).ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<K, T>();
        }

        public static List<TsRow2D> GetTsRowList(
            SortedDictionary<DateTime, double[]> xMap,
            int i)
        {
            try
            {
                if (xMap == null ||
                    xMap.Count == 0 ||
                    xMap.Values.First().Length >= i)
                {
                    return new List<TsRow2D>();
                }
                var rowList = new List<TsRow2D>();
                foreach (KeyValuePair<DateTime, double[]> keyValuePair in xMap)
                {
                    rowList.Add(new TsRow2D(keyValuePair.Key, keyValuePair.Value[i]));
                }
                return rowList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static void SetTsRowList(
            SortedDictionary<DateTime, double[]> xMap,
            int i,
            List<TsRow2D> rows)
        {
            try
            {
                if (xMap == null ||
                    xMap.Count == 0 ||
                    rows == null ||
                    rows.Count == 0 ||
                    i >= xMap.Count)
                {
                    return;
                }
                foreach (TsRow2D tsRow2D in rows)
                {
                    double[] currFeature;
                    if (xMap.TryGetValue(tsRow2D.Time, out currFeature))
                    {
                        currFeature[i] = tsRow2D.Fx;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void CorrectOutliers<T>(
            SortedDictionary<T, double[]> xMap,
            double dblOutliersThreshold)
        {
            try
            {
                SortedDictionary<DateTime, T> lookupMap;
                SortedDictionary<DateTime, double[]> xDateMap = GetDateMap(xMap, out lookupMap);
                CorrectOutliers(xDateMap, dblOutliersThreshold);
                foreach (KeyValuePair<DateTime, T> keyValuePair in lookupMap)
                {
                    xMap[keyValuePair.Value] = xDateMap[keyValuePair.Key];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static SortedDictionary<DateTime, T> GetDateMap<T, K>(
            SortedDictionary<K, T> xMap,
            out SortedDictionary<DateTime, K> lookupMap)
        {
            lookupMap = new SortedDictionary<DateTime, K>();
            try
            {
                var xDateMap = new SortedDictionary<DateTime, T>();
                DateTime currDate = DateTime.MinValue;
                foreach (KeyValuePair<K, T> keyValuePair in xMap)
                {
                    xDateMap[currDate] = keyValuePair.Value;
                    lookupMap[currDate] = keyValuePair.Key;
                    currDate = currDate.AddDays(1);
                }
                return xDateMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, T>();
        }

        public static SortedDictionary<DateTime, T> GetDateMap<T, K>(
            SortedDictionary<K, T> xMap)
        {
            try
            {
                if (xMap == null)
                {
                    return new SortedDictionary<DateTime, T>();
                }

                var xDateMap = new SortedDictionary<DateTime, T>();
                DateTime currDate = DateTime.MinValue;
                foreach (KeyValuePair<K, T> keyValuePair in xMap)
                {
                    xDateMap[currDate] = keyValuePair.Value;
                    currDate = currDate.AddDays(1);
                }
                return xDateMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, T>();
        }

        public static void CorrectOutliers(
            SortedDictionary<DateTime, double[]> xMap,
            double dblOutliersThreshold)
        {
            try
            {
                if (xMap == null || xMap.Count == 0)
                {
                    return;
                }
                int n = xMap.Values.First().Length;
                for (int i = 0; i < n; i++)
                {
                    List<TsRow2D> rows = GetTsRowList(xMap, i);
                    rows = Outliers.CorrectOutliers(rows, dblOutliersThreshold);
                    SetTsRowList(xMap, i, rows);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<double> NormalizeData(
            List<double> data,
            out double dblMin,
            out double dblDelta)
        {
            dblMin = 0;
            dblDelta = 0;
            try
            {
                if (data == null || data.Count == 0)
                {
                    return new List<double>();
                }
                double dblMax = data.Max();
                dblMin = data.Min();
                dblDelta = dblMax - dblMin;
                if (dblDelta == 0)
                {
                    return new List<double>();
                }
                double dblCurrMin = dblMin;
                var results = (from n in data select (n - dblCurrMin)).ToList();
                return results;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static SortedDictionary<T, double> FixExtremeValuesFromNormalyDistrData<T>(
            SortedDictionary<T, double> map,
            double dblThreshold)
        {
            try
            {
                if (map == null || map.Count == 0)
                {
                    return new SortedDictionary<T, double>();
                }
                List<double> dblValidVals = (from n in map
                                             where !Double.IsNaN(n.Value)
                                             select n.Value).ToList();
                double dblStdDev = StdDev.GetSampleStdDev(dblValidVals);
                double dblMean = dblValidVals.Average();
                double dblDefaultValue = dblThreshold * dblStdDev + dblMean;
                var outMap = new SortedDictionary<T, double>();
                foreach (KeyValuePair<T, double> keyValuePair in map)
                {
                    if (Double.IsNaN(keyValuePair.Value))
                    {
                        outMap[keyValuePair.Key] = keyValuePair.Value;
                        continue;
                    }
                    double dblX = Math.Abs((keyValuePair.Value - dblMean) / dblStdDev);
                    if (dblX <= dblThreshold)
                    {
                        outMap[keyValuePair.Key] = keyValuePair.Value;
                    }
                    else
                    {
                        double dblSign = Math.Sign(keyValuePair.Value);
                        Verboser.WriteLine("replaced extreme value [" +
                            keyValuePair.Value + "] to [" +
                            dblSign * dblDefaultValue + "]");
                        outMap[keyValuePair.Key] = dblSign * dblDefaultValue;
                    }
                }
                return outMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static double[] MergeFeature(double[] feature, double[] pcaFeature)
        {
            try
            {
                var feat = feature.ToList();
                feat.AddRange(pcaFeature);
                return feat.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double[] SelectVariables(
            double[] xVars,
            List<string> varNames,
            List<string> selectedVarNames)
        {
            try
            {
                if (xVars.Length != varNames.Count)
                {
                    throw new HCException("Invalid number of vars");
                }

                var selectFeature = new List<double>();
                foreach (string strSelectVar in selectedVarNames)
                {
                    int intIndex = varNames.IndexOf(strSelectVar);
                    if (intIndex < 0)
                    {
                        throw new HCException("Var not found");
                    }
                    selectFeature.Add(xVars[intIndex]);
                }
                return selectFeature.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double GetVolatility(
            List<double> prices)
        {
            List<double> returns = GetLogReturns(prices, 1);
            double dblStdDev = StdDev.GetSampleStdDev(returns);
            return dblStdDev;
        }

        public static int GetClass(
            double[] yPrediction)
        {
            try
            {
                double dblMaxPrediction = -Double.MaxValue;
                int intClass = -1;

                for (int j = 0; j < yPrediction.Length; j++)
                {
                    double dblCurrPrediction = yPrediction[j];
                    if (dblCurrPrediction > dblMaxPrediction)
                    {
                        dblMaxPrediction = dblCurrPrediction;
                        intClass = j;
                    }
                }
                if (intClass < 0)
                {
                    throw new HCException("Class not found");
                }
                return intClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static double GetCorrelation(
            List<double> list1,
            List<double> list2)
        {
            return Correlation.GetCorrelation(list1, list2);
        }

        public static void ExtractFeatures<T, K>(
            List<T> list,
            SortedDictionary<T, K> map1,
            SortedDictionary<T, K> map2)
        {
            try
            {
                if (map1 == null ||
                    map2 == null)
                {
                    return;
                }

                if (list == null)
                {
                    list = new List<T>();
                }

                var q = (from n in map1
                         where list.Contains(n.Key)
                         select n);
                foreach (KeyValuePair<T, K> keyValuePair in q)
                {
                    map2[keyValuePair.Key] = keyValuePair.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static SortedDictionary<DateTime, T> GetMapFromDates<T>(
            SortedDictionary<DateTime, T> dateMap,
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                if (dateMap == null || dateMap.Count == 0)
                {
                    return new SortedDictionary<DateTime, T>();
                }

                return new SortedDictionary<DateTime, T>((from n in dateMap
                                                          where n.Key >= startDate && n.Key <= endDate
                                                          select n).ToDictionary(t => t.Key, t => t.Value));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<DateTime, T>();
        }
        public static SortedDictionary<T, double[]> ShuffleCol<T>(
            SortedDictionary<T, double[]> xVars0,
            int intIndex)
        {
            return ShuffleCol(
                xVars0,
                intIndex,
                m_rng);
        }


        public static SortedDictionary<T, double[]> ShuffleCol<T>(
            SortedDictionary<T, double[]> xVars0,
            int intIndex,
            RngWrapper rngWrapper)
        {
            try
            {
                var res = new SortedDictionary<T, double[]>();
                var col = (from n in xVars0.Values select n[intIndex]).ToList();
                rngWrapper.ShuffleList(col);
                var arrMap = xVars0.ToArray();
                for (int i = 0; i < xVars0.Count; i++)
                {
                    KeyValuePair<T, double[]> keyValuePair = arrMap[i];
                    var arr = (double[])keyValuePair.Value.Clone();
                    arr[intIndex] = col[i];
                    res[keyValuePair.Key] = arr;
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }
        public static List<double[]> ShuffleCol(
            List<double[]> xVars0,
            int intIndex)
        {
            return ShuffleCol(xVars0,
                       intIndex,
                       m_rng);
        }

        public static List<double[][]> DeepCloneList(
            List<double[][]> allSamplesXArr)
        {
            try
            {
                var preAllocatedXVars = new List<double[][]>(
                    allSamplesXArr.Count + 2);
                for (int k = 0; k < allSamplesXArr.Count; k++)
                {
                    double[][] arrBase = allSamplesXArr[k];
                    var currArr = new double[arrBase.Length][];
                    for (int i = 0; i < arrBase.Length; i++)
                    {
                        currArr[i] = (double[]) arrBase[i].Clone();
                    }
                    preAllocatedXVars.Add(currArr);
                }
                return preAllocatedXVars;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[][]>();
        }

        public static List<List<double[]>> DeepCloneList(
            List<List<double[]>> allSamplesXArr)
        {
            try
            {
                var preAllocatedXVars = new List<List<double[]>>(
                    allSamplesXArr.Count + 2);
                for (int k = 0; k < allSamplesXArr.Count; k++)
                {
                    var arrBase = allSamplesXArr[k];
                    var currArr = new List<double[]>(arrBase.Count + 2);
                    for (int i = 0; i < arrBase.Count; i++)
                    {
                        currArr.Add((double[])arrBase[i].Clone());
                    }
                    preAllocatedXVars.Add(currArr);
                }
                return preAllocatedXVars;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<List<double[]>>();
        }


        public static List<double[]> ShuffleCol(
            List<double[]> xVars0,
            int intIndex,
            RngWrapper rngWrapper)
        {
            try
            {
                var res = new List<double[]>(xVars0.Count + 2);
                var col = xVars0.ToList();
                rngWrapper.ShuffleList(col);
                var arrMap = xVars0.ToArray();
                int intSize = xVars0[0].Length;
                for (int i = 0; i < xVars0.Count; i++)
                {
                    double[] keyValuePair = arrMap[i];
                    var arr = new double[intSize];
                    Array.Copy(keyValuePair, 0, arr, 0, intSize);
                    arr[intIndex] = col[i][intIndex];
                    res.Add(arr);
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double[]>();
        }

        public static void ShuffleColFast(
            double[][] xVars0,
            double[][] xVarsPreAllocated1,
            int intIndex,
            int[] suffledArr)
        {
            try
            {
                for (int i = 0; i < xVars0.Length; i++)
                {
                    xVarsPreAllocated1[i][intIndex] = xVars0[
                        suffledArr[i]][intIndex];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void ShuffleColFast(
            List<double[]> xVars0,
            List<double[]> xVarsPreAllocated1,
            int intIndex,
            int[] suffledArr)
        {
            try
            {
                for (int i = 0; i < xVars0.Count; i++)
                {
                    xVarsPreAllocated1[i][intIndex] = xVars0[
                        suffledArr[i]][intIndex];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static SortedDictionary<T, double[]> MergeFeatures<T>(
            SortedDictionary<T, double[]> xLowerBoundMap,
            List<double> list)
        {
            try
            {
                var result = new SortedDictionary<T, double[]>();
                int k = 0;
                foreach (KeyValuePair<T, double[]> keyValuePair in xLowerBoundMap)
                {
                    double[] feat = keyValuePair.Value;
                    var newFeat = new double[keyValuePair.Value.Length + 1];
                    for (int i = 0; i < feat.Length; i++)
                    {
                        newFeat[i] = feat[i];
                    }
                    newFeat[feat.Length] = list[k];
                    result[keyValuePair.Key] = newFeat;
                    k++;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static List<T> SelectVariables<T>(List<T> list, List<int> toSelect)
        {
            var result = new List<T>();
            foreach (int intVar in toSelect)
            {
                result.Add(list[intVar]);
            }
            return result;
        }

        public static double[] GetAvg(List<double[]> xData)
        {
            int intVars = xData[0].Length;
            var result = new double[intVars];
            foreach (double[] row in xData)
            {
                for (int i = 0; i < intVars; i++)
                {
                    result[i] += row[i];
                }
            }
            for (int i = 0; i < intVars; i++)
            {
                result[i] /= xData.Count;
            }
            return result;
        }

        public static void CorrectOutliers(
            List<double> xMap,
            double dblOutliersThreshold)
        {
            try
            {
                var rows = new List<TsRow2D>();
                var currDate = new DateTime();
                foreach (double d in xMap)
                {
                    var tsRow2D = new TsRow2D(currDate, d);
                    rows.Add(tsRow2D);
                    currDate = currDate.AddDays(1);
                }
                rows = Outliers.CorrectOutliers(rows, dblOutliersThreshold);
                for (int i = 0; i < rows.Count; i++)
                {
                    TsRow2D tsRow2D = rows[i];
                    xMap[i] = tsRow2D.Fx;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<double> SelectVariable(
            List<double[]> xVarsTmp,
            int intCurrVar)
        {
            try
            {
                var result = new List<double>();
                foreach (double[] arr in xVarsTmp)
                {
                    result.Add(arr[intCurrVar]);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static int GetMostVotedClass(
            double[] votes,
            int intNumClasses)
        {
            try
            {
                var countPerClass = new int[intNumClasses];
                for (int i = 0; i < votes.Length; i++)
                {
                    countPerClass[(int) votes[i]]++;
                }
                int intMaxClassCount = 0;
                int intMaxClass = 0;
                for (int i = 0; i < intNumClasses; i++)
                {
                    if (countPerClass[i] > intMaxClassCount)
                    {
                        intMaxClassCount = countPerClass[i];
                        intMaxClass = i;
                    }
                }
                return intMaxClass;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static double[] SelectVariables(
            double[] feature,
            List<int> vars)
        {
            try
            {
                var resultFeat = new double[vars.Count];
                for (int i = 0; i < vars.Count; i++)
                {
                    resultFeat[i] = feature[vars[i]];
                }
                return resultFeat;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static void SplitListAtKey<T, U>(
            SortedDictionary<T, U> allForecastsMap,
            T currDate,
            out SortedDictionary<T, U> map1,
            out SortedDictionary<T, U> map2)
        {
            map1 = new SortedDictionary<T, U>();
            map2 = new SortedDictionary<T, U>();
            try
            {
                bool blnFound = false;
                foreach (var kvp in allForecastsMap)
                {
                    if (!blnFound &&
                        kvp.Key.Equals(currDate))
                    {
                        blnFound = true;
                    }
                    if (!blnFound)
                    {
                        map1[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        map2[kvp.Key] = kvp.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static List<TsRow2D> DecodeReturns(
            List<TsRow2D> returnsList,
            double dblLastValue,
            DateTime firstDate)
        {
            try
            {
                // current - prev = return
                // current - return = prev
                var result = new List<TsRow2D>();
                for (int i = returnsList.Count - 1; i >= 0; i--)
                {
                    var currItem = returnsList[i];

                    result.Add(
                        new TsRow2D(currItem.Time,
                                    dblLastValue));
                    dblLastValue = dblLastValue - currItem.Fx;
                }

                result.Add(
                    new TsRow2D(firstDate,
                                dblLastValue));
                result.Reverse();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static List<double> DecodeReturns(
            List<double> returnsList,
            double dblLastValue)
        {
            try
            {
                // current - prev = return
                // current - return = prev
                var result = new List<double>(returnsList.Count + 2);
                for (int i = returnsList.Count - 1; i >= 0; i--)
                {
                    double dblCurrValue = returnsList[i];

                    result.Add(dblLastValue);
                    dblLastValue = dblLastValue - dblCurrValue;
                }

                result.Add(dblLastValue);
                result.Reverse();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<TsRow2D> NormalizeTs(List<TsRow2D> value)
        {
            try
            {
                if (value.Count == 0)
                {
                    return new List<TsRow2D>();
                }
                var vals = (from n in value select n.Fx).ToList();
                var dblStdDev = StdDev.GetSampleStdDev(vals);
                double dblMean = vals.Average();
                var result = new List<TsRow2D>();
                foreach (TsRow2D tsRow2D in value)
                {
                    var newTsRow2D = new TsRow2D(tsRow2D.Time,
                        dblStdDev == 0 ?
                        0 :
                        (tsRow2D.Fx - dblMean) / dblStdDev);
                    result.Add(newTsRow2D);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static double[] SelectVariables<T>(
            double[] arr,
            List<string> varNames,
            List<string> selectedVarNames)
        {
            try
            {
                List<int> indexList = (from n in selectedVarNames select varNames.IndexOf(n)).ToList();
                if ((from n in indexList where n < 0 select n).Any())
                {
                    throw new HCException("Var not found");
                }
                return SelectVariables(arr, indexList);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static SortedDictionary<T, double[]> SelectVariables<T>(
            SortedDictionary<T, double[]> map,
            List<string> varNames,
            List<string> selectedVarNames)
        {
            try
            {

                List<int> indexList = (from n in selectedVarNames select varNames.IndexOf(n)).ToList();
                if ((from n in indexList where n < 0 select n).Any())
                {
                    throw new HCException("Var not found");
                }
                var result = new SortedDictionary<T, double[]>();
                foreach (var kvp in map)
                {
                    result[kvp.Key] = SelectVariables(kvp.Value, indexList);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static List<List<T>> GetListGroups<T>(
            List<T> universeList,
            int intGroupSize)
        {
            try
            {
                var resultList =
                    new List<List<T>>();
                var currentList =
                    new List<T>();
                foreach (T strInstrument in universeList)
                {
                    currentList.Add(strInstrument);
                    if (currentList.Count >= intGroupSize)
                    {
                        resultList.Add(currentList);
                        currentList = new List<T>();
                    }
                }
                if (currentList.Count > 0)
                {
                    resultList.Add(currentList);
                }
                return resultList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<List<T>>();
        }
    }
}