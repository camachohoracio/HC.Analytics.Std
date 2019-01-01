#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Statistics
{
    public static class StdDev
    {
        public static double[] GetStdDevVector(
            List<double[]> items,
            double[] meanArr = null)
        {
            try
            {
                if (items == null ||
                    items.Count  == 0)
                {
                    return new double[0];
                }
                if(items.Count < 2)
                {
                    return new double[items[0].Length];
                }

                if (meanArr == null)
                {
                    meanArr = Mean.GetMeanVector(items);
                }
                int intN = items.Count;
                int intFeatSize = items[0].Length;
                double[] stdDevArr = new double[intFeatSize];

                for (int j = 0; j < intFeatSize; j++)
                {
                    double dblMean = meanArr[j];
                    for (int i = 0; i < intN; i++)
                    {
                        stdDevArr[j] += Math.Pow(items[i][j] - dblMean, 2);
                    }
                }
                for (int j = 0; j < intFeatSize; j++)
                {
                    stdDevArr[j] = Math.Sqrt(stdDevArr[j] / (intN - 1.0));
                }

                return stdDevArr;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new double[] {};
        }

        public static double GetSampleVariance(int[] dblDataArray)
        {
            double dblMean = dblDataArray.Average();
            double dblSumSq = 0;
            for (int i = 0; i < dblDataArray.Length; i++)
            {
                dblSumSq += Math.Pow(dblDataArray[i] - dblMean, 2);
            }
            return dblSumSq/((dblDataArray.Length - 1.0));
        }

        public static double GetSampleVariance(long[] dblDataArray)
        {
            double dblMean = dblDataArray.Average();
            double dblSumSq = 0;
            for (int i = 0; i < dblDataArray.Length; i++)
            {
                dblSumSq += Math.Pow(dblDataArray[i] - dblMean, 2);
            }
            return dblSumSq/((dblDataArray.Length - 1.0));
        }

        public static double GetSampleVariance(float[] dblDataArray)
        {
            double dblMean = dblDataArray.Average();
            double dblSumSq = 0;
            for (int i = 0; i < dblDataArray.Length; i++)
            {
                dblSumSq += Math.Pow(dblDataArray[i] - dblMean, 2);
            }
            return dblSumSq/((dblDataArray.Length - 1.0));
        }


        public static double GetSampleVariance(decimal[] dblDataArray)
        {
            double dblMean = (double) dblDataArray.Average();
            double dblSumSq = 0;
            for (int i = 0; i < dblDataArray.Length; i++)
            {
                dblSumSq += Math.Pow((double) dblDataArray[i] - dblMean, 2);
            }
            return dblSumSq/((dblDataArray.Length - 1.0));
        }


        public static double GetSampleVariance(double[] dblDataArray)
        {
            double dblMean;
            return GetSampleVariance(dblDataArray, out dblMean);
        }

        public static double GetSampleVariance(double[] dblDataArray,
            out double dblMean)
        {
            dblMean = dblDataArray.Average();
            double dblSumSq = 0;
            for (int i = 0; i < dblDataArray.Length; i++)
            {
                dblSumSq += Math.Pow(dblDataArray[i] - dblMean, 2);
            }
            return dblSumSq/((dblDataArray.Length - 1.0));
        }

        public static double GetSampleVariance(List<double> dblDataArray)
        {
            try
            {
                if(dblDataArray == null || dblDataArray.Count == 0)
                {
                    return 0;
                }

                double dblMean = dblDataArray.Average();
                double dblSumSq = 0;
                for (int i = 0; i < dblDataArray.Count; i++)
                {
                    dblSumSq += Math.Pow(dblDataArray[i] - dblMean, 2);
                }
                return dblSumSq/((dblDataArray.Count - 1.0));
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static double GetSampleVariance(IEnumerable<double> dblDataArray)
        {
            try
            {
                double dblMean = dblDataArray.Average();
                double dblSumSq = 0;
                foreach (var d in dblDataArray)
                {
                    dblSumSq += Math.Pow(d - dblMean, 2);
                }
                return dblSumSq/((dblDataArray.Count() - 1.0));
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }


        public static double GetSampleStdDev(List<double> dblDataArray)
        {
            if (dblDataArray.Count <= 2)
            {
                return 0;
            }
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }

        public static double GetSampleStdDev(double[] dblDataArray)
        {
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }

        public static double GetSampleStdDev(long[] dblDataArray)
        {
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }

        public static double GetSampleStdDev(int[] dblDataArray)
        {
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }

        public static double GetSampleStdDev(decimal[] dblDataArray)
        {
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }

        public static double GetSampleStdDev(float[] dblDataArray)
        {
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }

        public static double GetSampleStdDev(IEnumerable<double> dblDataArray)
        {
            return Math.Sqrt(GetSampleVariance(dblDataArray));
        }
    }
}
