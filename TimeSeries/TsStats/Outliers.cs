#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public static class Outliers
    {
        public const int DEFAULT_SAMPLE_SIZE = 30;
        public const int DEFAULT_OUTLIER_THRESHOLD = 6;
        private const int MIN_SAMPLE_SIZE = 4;

        public static List<double> CorrectOutliers(
            List<double> data)
        {
            return CorrectOutliers(data, DEFAULT_OUTLIER_THRESHOLD);
        }

        public static List<double> CorrectOutliers(
            List<double> data,
            double dblThreshold)
        {
            try
            {
                if(data == null ||
                    data.Count == 0)
                {
                    return new List<double>();
                }
                var currDate = new DateTime();
                var tsRows = new List<TsRow2D>();
                foreach (double dblVal in data)
                {
                    var currRow = new TsRow2D(currDate, dblVal);
                    tsRows.Add(currRow);
                    currDate = currDate.AddDays(1);
                }
                int intSampleSize = Math.Min(DEFAULT_SAMPLE_SIZE, tsRows.Count/2);
                List<TsRow2D> result = CorrectOutliers(tsRows,
                    intSampleSize,
                    dblThreshold);
                var values = (from n in result select n.Fx).ToList();
                return values;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<TsRow2D> CorrectOutliers(
            List<TsRow2D> data)
        {
            return CorrectOutliers(data, DEFAULT_SAMPLE_SIZE);
        }

        public static List<TsRow2D> CorrectOutliers(
            List<TsRow2D> data,
            double dblOutlierThreshold)
        {
            return CorrectOutliers(data, DEFAULT_SAMPLE_SIZE, dblOutlierThreshold);
        }

        public static List<TsRow2D> CorrectOutliers(
            List<TsRow2D> data,
            int intSampleSize)
        {
            return CorrectOutliers(data, intSampleSize, DEFAULT_OUTLIER_THRESHOLD);
        }

        public static List<TsRow2D> CorrectOutliers(
            List<TsRow2D> data,
            int intSampleSize,
            double dblOutliersThreshold)
        {
            try
            {
                List<TsRow2D> outliers;
                if (intSampleSize >= data.Count - 2)
                {
                    outliers = GetOutliersSmallSample(data, dblOutliersThreshold);
                }
                else
                {
                    outliers = GetOutliers(data,
                                           dblOutliersThreshold,
                                           intSampleSize);
                }

                if(outliers == null || outliers.Count == 0)
                {
                    return data.ToList();
                }

                Dictionary<DateTime, TsRow2D> outliersMap =
                    outliers.ToDictionary(t => t.Time, t => t);
                var outData = new List<TsRow2D>();
                if (outliersMap.Count > 0)
                {
                    var rollingWindoRegression = new RollingWindowRegression(intSampleSize);
                    var rollingWindowTsFunction = new RollingWindowStdDev(intSampleSize);

                    foreach (TsRow2D tsRow2D in data)
                    {
                        double dblValue;
                        if (outliersMap.ContainsKey(tsRow2D.Time) &&
                            rollingWindoRegression.IsReady())
                        {
                            double dblPrediction = rollingWindoRegression.Predict(
                                rollingWindoRegression.XList.Last() + 1);
                            dblPrediction = Math.Max(dblPrediction, rollingWindowTsFunction.Min);
                            dblPrediction = Math.Min(dblPrediction, rollingWindowTsFunction.Max);

                            Verboser.Talk(typeof(Outliers).Name + 
                                " corrected value [" + dblPrediction + "]. Old [" +
                                tsRow2D.Fx + "]");

                            outData.Add(
                                new TsRow2D
                                    {
                                        Time = tsRow2D.Time,
                                        Fx = dblPrediction
                                    });
                            dblValue = dblPrediction;
                        }
                        else
                        {
                            outData.Add(tsRow2D);
                            dblValue = tsRow2D.Fx;
                        }
                        rollingWindoRegression.Update(tsRow2D.Time, tsRow2D.Fx);
                        rollingWindowTsFunction.Update(tsRow2D.Time, dblValue);
                    }
                }
                else
                {
                    outData.AddRange(data);
                }
                return outData;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        public static void CorrectOutliers<T>(
            SortedDictionary<T, double[]> map,
            double[] feature)
        {
            try
            {
                if (feature.Length != map.First().Value.Length)
                {
                    throw new HCException("Invalid feature size");
                }
                for (int i = 0; i < feature.Length; i++)
                {
                    var list = (from n in map
                                select n.Value[i]).ToList();
                    if (IsOutlier(list.ToList(), feature[i],
                                   12))
                    {
                        double dblMin = list.Min();
                        double dblMax = list.Max();
                        feature[i] = Math.Max(dblMin, feature[i]);
                        feature[i] = Math.Min(dblMax, feature[i]);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static bool IsOutlier(
            List<double> dblList,
            double dblVal)
        {
            return IsOutlier(dblList,
                             dblVal,
                             DEFAULT_OUTLIER_THRESHOLD);
        }

        public static bool IsOutlier(
            List<double> dblList,
            double dblVal,
            double dblOutLayerThreshold)
        {
            var data = new List<TsRow2D>();
            var baseDate = new DateTime();
            for (int i = 0; i < dblList.Count; i++)
            {
                data.Add(
                    new TsRow2D(baseDate, dblList[i]));
                baseDate = baseDate.AddSeconds(1);
            }

            baseDate = baseDate.AddSeconds(1);
            var tsRow = new TsRow2D(baseDate, dblVal);
            return IsOutlier(data, tsRow, dblOutLayerThreshold);
        }

        public static bool ContainsOutlier(
            List<TsRow2D> data)
        {
            return ContainsOutlier(data, DEFAULT_OUTLIER_THRESHOLD);
        }

        public static bool ContainsOutlier(
            List<TsRow2D> data,
            double dblOutlierThreshold)
        {
            try
            {
                List<TsRow2D> outliers = GetOutliers(data, dblOutlierThreshold, DEFAULT_SAMPLE_SIZE);
                return outliers != null && outliers.Count > 0;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static bool IsOutlier(
            List<TsRow2D> data,
            TsRow2D tsRow2D)
        {
            return IsOutlier(data, tsRow2D, DEFAULT_OUTLIER_THRESHOLD);
        }

        public static bool IsOutlier(
            List<TsRow2D> data,
            TsRow2D tsRow2D,
            double dblOutLayerThreshold,
            int intDefaultSampleSize = DEFAULT_SAMPLE_SIZE)
        {
            try
            {
                data = data.ToList();
                data.Add(tsRow2D);
                List<TsRow2D> outliers = GetOutliers(
                    data, 
                    dblOutLayerThreshold, 
                    intDefaultSampleSize);
                return (from n in outliers
                        where n.Time == tsRow2D.Time &&
                              n.Fx == tsRow2D.Fx
                        select n).Any();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static bool IsOutlierInSample(
            List<TsRow2D> data,
            List<TsRow2D> tsRow2D,
            double dblOutLayerThreshold,
            int intDefaultSampleSize = DEFAULT_SAMPLE_SIZE)
        {
            try
            {
                data = data.ToList();
                data.AddRange(tsRow2D);
                List<TsRow2D> outliers = GetOutliers(
                    data,
                    dblOutLayerThreshold,
                    intDefaultSampleSize);
                var dateSet = new HashSet<DateTime>(from n in tsRow2D select n.Time);

                return (from n in outliers
                        where dateSet.Contains(n.Time)
                        select n).Any();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static List<TsRow2D> GetOutliers(List<TsRow2D> data)
        {
            return GetOutliers(data, DEFAULT_OUTLIER_THRESHOLD, DEFAULT_SAMPLE_SIZE);
        }

        public static List<TsRow2D> GetOutliers(
            List<TsRow2D> data,
            double dblOutlierThreshold,
            int intSampleSize)
        {
            try
            {
                if (intSampleSize >= data.Count - 2)
                {
                    return GetOutliersSmallSample(data, dblOutlierThreshold);
                }

                List<TsRow2D> derivatives = TimeSeriesHelper.GetDerivative(1, data);
                Dictionary<DateTime, double> ouliersDerivatives = GetOutliers0(
                    derivatives,
                    dblOutlierThreshold,
                    intSampleSize)
                    .ToDictionary(t => t.Time, t => t.Fx);

                Dictionary<DateTime, TsRow2D> outliersMap = GetOutliers0(data, dblOutlierThreshold,
                                                                         intSampleSize)
                    .ToDictionary(t => t.Time, t => t);

                var foundOutliers = new List<TsRow2D>();
                bool blnOutlierMode = false;
                for (int i = 0; i < data.Count; i++)
                {
                    TsRow2D tsRow2D = data[i];
                    DateTime time = tsRow2D.Time;


                    TsRow2D outlier;
                    if (outliersMap.TryGetValue(time, out outlier))
                    {
                        if (ouliersDerivatives.ContainsKey(time))
                        {
                            foundOutliers.Add(outlier);
                            blnOutlierMode = true;
                        }
                        else if (blnOutlierMode)
                        {
                            //
                            // outlayers are together, but the derivative did not find it
                            //
                            foundOutliers.Add(outlier);
                        }
                    }
                    else
                    {
                        blnOutlierMode = false;
                    }
                }
                return foundOutliers;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        private static List<TsRow2D> GetOutliersSmallSample(
            List<TsRow2D> data, 
            double dblOutlierThreshold)
        {
            try
            {
                if (data.Count < MIN_SAMPLE_SIZE)
                {
                    return new List<TsRow2D>();
                }
                var rollingWindowTsFunction =
                    new RollingWindowTsFunction(MIN_SAMPLE_SIZE);
                var dateValidator = new HashSet<DateTime>();
                var outLiers = new List<TsRow2D>();
                var testData = new List<double>();
                for (int i = 0; i < data.Count; i++)
                {
                    rollingWindowTsFunction.Update(
                        data[i].Time, data[i].Fx);
                    testData.Add(data[i].Fx);
                }
                double dblMedian = Median.GetMedian(testData.ToArray());
                List<double> r = (from n in testData select Math.Abs(n - dblMedian)).ToList();
                double dblMedianR = Median.GetMedian(r.ToArray());

                for (int i = 0; i < data.Count; i++)
                {
                    TsRow2D currRow = data[i];
                    double dblR = Math.Abs(currRow.Fx - dblMedian);
                    if (dblR >= dblOutlierThreshold*dblMedianR &&
                        !dateValidator.Contains(currRow.Time))
                    {
                        outLiers.Add(currRow);
                        dateValidator.Add(currRow.Time);
                    }
                }

                outLiers.Sort((a, b) => a.Time.CompareTo(b.Time));
                return outLiers;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }

        private static List<TsRow2D> GetOutliers0(
            List<TsRow2D> data,
            double dblOutlierThreshold,
            int intSampleSize)
        {
            try
            {
                if (data.Count <= intSampleSize)
                {
                    return new List<TsRow2D>();
                }

                var rollingWindowTsFunction =
                    new RollingWindowTsFunction(intSampleSize);
                var dateValidator = new HashSet<DateTime>();
                var outLiers = new List<TsRow2D>();
                for (int i = 0; i < data.Count; i++)
                {
                    DateTime time = data[i].Time;


                    rollingWindowTsFunction.Update(
                        time, 
                        data[i].Fx);

                    if (rollingWindowTsFunction.IsReady())
                    {
                        List<double> testData = (from n in
                                                     rollingWindowTsFunction.Data
                                                 select n.Fx).ToList();
                        double dblMedian = Median.GetMedian(testData.ToArray());
                        List<double> r = (from n in testData select Math.Abs(n - dblMedian)).ToList();
                        double dblMedianR = Median.GetMedian(r.ToArray());
                        for (int j = 0; j < rollingWindowTsFunction.Data.Count; j++)
                        {
                            TsRow2D currRow = rollingWindowTsFunction.Data[j];
                            double dblR = Math.Abs(currRow.Fx - dblMedian);
                            if (dblR >= dblOutlierThreshold*dblMedianR &&
                                !dateValidator.Contains(currRow.Time))
                            {
                                //Console.WriteLine("Outlayer found. median [" + dblMedian + "]. val[" +
                                //                  currRow.Fx + "]" +
                                //                  dblR + ">=" + (dblOutlierThreshold*dblMedianR));
                                outLiers.Add(currRow);
                                dateValidator.Add(currRow.Time);
                            }
                        }
                    }
                }
                outLiers.Sort((a, b) => a.Time.CompareTo(b.Time));
                return outLiers;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<TsRow2D>();
        }
    }
}

