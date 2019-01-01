#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Statistics
{
    public class Mean
    {
        public static double GetSampleMean(long[] dblDataArray)
        {
            return dblDataArray.Average();
        }

        public static float GetSampleMean(float[] dblDataArray)
        {
            return dblDataArray.Average();
        }

        public static double GetSampleMean(double[] dblDataArray)
        {
            return dblDataArray.Average();
        }

        public static double GetSampleMean(decimal[] dblDataArray)
        {
            return (double) dblDataArray.Average();
        }

        public static double GetSampleMean(int[] dblDataArray)
        {
            return dblDataArray.Average();
        }

        public static double GetSampleMean(IEnumerable<double> dblDataArray)
        {
            return dblDataArray.Average();
        }

        public static double GetSampleMean(List<double> dblDataArray)
        {
            return dblDataArray.Average();
        }

        // UNIFORM ORDER STATISTIC MEDIANS
        public static double[] uniformOrderStatisticMedians(int n)
        {
            double nn = n;
            double[] uosm = new double[n];
            uosm[n - 1] = Math.Pow(0.5, 1.0/nn);
            uosm[0] = 1.0 - uosm[n - 1];
            for (int i = 1; i < n - 1; i++)
            {
                uosm[i] = (i + 1 - 0.3175)/(nn + 0.365);
            }
            return uosm;
        }

        public static double[] GetMeanVector(List<double[]> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                {
                    return new double[] { };
                }

                int intFeatSize = items[0].Length;
                var meanArr = new double[intFeatSize];
                int intN = items.Count;
                for (int i = 0; i < intN; i++)
                {
                    var currFeat = items[i];
                    for (int j = 0; j < intFeatSize; j++)
                    {
                        meanArr[j] += currFeat[j];
                    }
                }
                for (int j = 0; j < intFeatSize; j++)
                {
                    meanArr[j] /= intN;
                }

                return meanArr;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new double[] {};
        }
    }
}
