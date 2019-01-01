#region

using System;
using HC.Analytics.Mathematics;

#endregion

namespace HC.Analytics.Statistics
{
    public class Kurtosis
    {
        // STATIC VARIABLES
        private static bool nFactorOptionS; // = true  varaiance, covariance and standard deviation denominator = n
        // = false varaiance and standard deviation denominator = n-1


        // KURTOSIS
        // Static Methods
        // Kutosis of a 1D array of doubles
        public static double kurtosis(double[] aa)
        {
            int n = aa.Length;
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }
            double sum = 0.0D;
            double mean = Mean.GetSampleMean(aa);
            for (int i = 0; i < n; i++)
            {
                sum += Math.Pow((aa[i] - mean), 4);
            }
            sum = sum/denom;
            return sum/Fmath.square(StdDev.GetSampleVariance(aa));
        }

        public static double curtosis(double[] aa)
        {
            return kurtosis(aa);
        }

        // Kutosis excess of a 1D array of doubles
        public static double kurtosisExcess(double[] aa)
        {
            return kurtosis(aa) - 3.0D;
        }

        public static double curtosisExcess(double[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static double excessKurtosis(double[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static double excessCurtosis(double[] aa)
        {
            return kurtosisExcess(aa);
        }

        // Kutosis of a 1D array of floats
        public static float kurtosis(float[] aa)
        {
            int n = aa.Length;
            float denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }
            float sum = 0.0F;
            float mean = Mean.GetSampleMean(aa);
            for (int i = 0; i < n; i++)
            {
                sum += (float) Math.Pow((aa[i] - mean), 4);
            }
            sum = sum/denom;
            return sum/(Fmath.square((float) StdDev.GetSampleVariance(aa)));
        }

        public static float curtosis(float[] aa)
        {
            return kurtosis(aa);
        }

        // Kutosis excess of a 1D array of floats
        public static float kurtosisExcess(float[] aa)
        {
            return kurtosis(aa) - 3.0F;
        }

        public static float curtosisExcess(float[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static float excessKurtosis(float[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static float excessCurtosis(float[] aa)
        {
            return kurtosisExcess(aa);
        }

        // Kutosis of a 1D array of long
        public static decimal kurtosis(long[] aa)
        {
            decimal[] bd = new decimal[aa.Length];
            for (int i = 0; i < bd.Length; i++)
            {
                bd[i] = aa[i];
            }
            return kurtosis(bd);
        }

        public static decimal curtosis(long[] aa)
        {
            return kurtosis(aa);
        }

        // Kutosis excess of a 1D array of long
        public static decimal kurtosisExcess(long[] aa)
        {
            return kurtosis(aa) - (decimal) 3.0;
        }

        public static decimal curtosisExcess(long[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static decimal excessKurtosis(long[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static decimal excessCurtosis(long[] aa)
        {
            return kurtosisExcess(aa);
        }


        // Kutosis of a 1D array of decimal
        public static decimal kurtosis(decimal[] aa)
        {
            int n = aa.Length;
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }
            decimal sum = decimal.Zero;
            decimal mean = (decimal) Mean.GetSampleMean(aa);
            for (int i = 0; i < n; i++)
            {
                decimal hold = aa[i] - (mean);
                sum = sum + (hold*(hold*(hold*(hold))));
            }
            sum = sum/((decimal) (denom));
            mean = (decimal) StdDev.GetSampleVariance(aa);
            sum = sum/(mean*(mean));
            mean = 0;
            return sum;
        }

        public static decimal curtosis(decimal[] aa)
        {
            return kurtosis(aa);
        }

        // Kutosis excess of a 1D array of decimal
        public static decimal kurtosisExcess(decimal[] aa)
        {
            return kurtosis(aa) - (decimal) 3.0;
        }

        public static decimal curtosisExcess(decimal[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static decimal excessCurtosis(decimal[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static decimal excessKurtosis(decimal[] aa)
        {
            return kurtosisExcess(aa);
        }


        // Kutosis of a 1D array of int
        public static double kurtosis(int[] aa)
        {
            int n = aa.Length;
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }
            double sum = 0.0D;
            double mean = Mean.GetSampleMean(aa);
            for (int i = 0; i < n; i++)
            {
                sum += Math.Pow((aa[i] - mean), 4);
            }
            sum = sum/denom;
            return sum/Fmath.square(StdDev.GetSampleVariance(aa));
        }

        public static double curtosis(int[] aa)
        {
            return kurtosis(aa);
        }

        // Kutosis excess of a 1D array of int
        public static double kurtosisExcess(int[] aa)
        {
            return kurtosis(aa) - 3.0D;
        }

        public static double curtosisExcess(int[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static double excessCurtosis(int[] aa)
        {
            return kurtosisExcess(aa);
        }

        public static double excessKurtosis(int[] aa)
        {
            return kurtosisExcess(aa);
        }
    }
}
