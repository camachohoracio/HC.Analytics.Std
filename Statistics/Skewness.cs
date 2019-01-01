#region

using System;
using HC.Analytics.ConvertClasses;
using HC.Analytics.Mathematics;

#endregion

namespace HC.Analytics.Statistics
{
    public class Skewness
    {
        // STATIC VARIABLES
        private static bool nFactorOptionS; // = true  varaiance, covariance and standard deviation denominator = n
        // = false varaiance and standard deviation denominator = n-1

        // SKEWNESS
        // Static Methods
        // Moment skewness of a 1D array of doubles
        public static double momentSkewness(double[] aa)
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
                sum += Math.Pow((aa[i] - mean), 3);
            }
            sum = sum/denom;
            return sum/Math.Pow(
                           StdDev.GetSampleStdDev(aa), 3);
        }


        // Moment skewness of a 1D array of floats
        public static float momentSkewness(float[] aa)
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
                sum += (float) Math.Pow((aa[i] - mean), 3);
            }
            sum = sum/denom;
            return sum/((float) Math.Pow(
                                    StdDev.GetSampleStdDev(aa), 3));
        }

        // Moment skewness of a 1D array of decimal
        public static double momentSkewness(decimal[] aa)
        {
            int n = aa.Length;
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }
            decimal sum = decimal.Zero;
            decimal mean = (decimal) Mean.GetSampleMean(aa);
            double sd = StdDev.GetSampleStdDev(aa);
            for (int i = 0; i < n; i++)
            {
                decimal hold = aa[i] - (mean);
                sum = sum + (hold*(hold*(hold)));
            }
            sum = sum*((decimal) (1.0/denom));
            return ((double) sum)/Math.Pow(sd, 3);
        }

        // Moment skewness of a 1D array of long
        public static double momentSkewness(long[] aa)
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
                sum += Math.Pow((aa[i] - mean), 3);
            }
            sum = sum/denom;
            return sum/Math.Pow(StdDev.GetSampleStdDev(aa), 3);
        }

        // Moment skewness of a 1D array of int
        public static double momentSkewness(int[] aa)
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
                sum += Math.Pow((aa[i] - mean), 3);
            }
            sum = sum/denom;
            return sum/Math.Pow(StdDev.GetSampleStdDev(aa), 3);
        }

        // Median skewness of a 1D array of doubles
        public static double medianSkewness(double[] aa)
        {
            double mean = Mean.GetSampleMean(aa);
            double median = Median.GetMedian(aa);
            double sd = StdDev.GetSampleStdDev(aa);
            return 3.0*(mean - median)/sd;
        }

        // Median skewness of a 1D array of floats
        public static float medianSkewness(float[] aa)
        {
            float mean = Mean.GetSampleMean(aa);
            float median = Median.GetMedian(aa);
            float sd = (float) StdDev.GetSampleStdDev(aa);
            return 3.0F*(mean - median)/sd;
        }

        // Median skewness of a 1D array of decimal
        public static double medianSkewness(decimal[] aa)
        {
            decimal mean = (decimal) Mean.GetSampleMean(aa);
            decimal median = Median.GetMedian(aa);
            double sd = StdDev.GetSampleStdDev(aa);
            return 3.0*((double) (mean - median))/sd;
        }

        // Median skewness of a 1D array of long
        public static double medianSkewness(long[] aa)
        {
            double mean = Mean.GetSampleMean(aa);
            double median = Median.GetMedian(aa);
            double sd = StdDev.GetSampleStdDev(aa);
            return 3.0*(mean - median)/sd;
        }

        // Median skewness of a 1D array of int
        public static double medianSkewness(int[] aa)
        {
            double mean = Mean.GetSampleMean(aa);
            double median = Median.GetMedian(aa);
            double sd = StdDev.GetSampleStdDev(aa);
            return 3.0*(mean - median)/sd;
        }


        // Quartile skewness of a 1D array of double
        public static double quartileSkewness(double[] aa)
        {
            int n = aa.Length;
            double median50 = Median.GetMedian(aa);
            int start1 = 0;
            int start2 = 0;
            int end1 = n/2 - 1;
            int end2 = n - 1;
            if (Fmath.isOdd(n))
            {
                start2 = end1 + 2;
            }
            else
            {
                start2 = end1 + 1;
            }
            double[] first = new double[aa.Length];
            for (int i = start1; i <= end1; i++)
            {
                first[i - start1] = aa[i];
            }

            double[] last = new double[aa.Length];
            for (int i = start2; i <= end2; i++)
            {
                last[i - start1] = aa[i];
            }

            //ArrayMaths am = new ArrayMaths(aa);
            //double[] first = am.subarray_as_double(start1, end1);
            //double[] last = am.subarray_as_double(start2, end2);
            double median25 = Median.GetMedian(first);
            double median75 = Median.GetMedian(last);

            return (median25 - 2.0*median50 + median75)/(median75 - median25);
        }

        // Quartile skewness of a 1D array of float
        public static float quartileSkewness(float[] aa)
        {
            int n = aa.Length;
            float median50 = Median.GetMedian(aa);
            int start1 = 0;
            int start2 = 0;
            int end1 = n/2 - 1;
            int end2 = n - 1;
            if (Fmath.isOdd(n))
            {
                start2 = end1 + 2;
            }
            else
            {
                start2 = end1 + 1;
            }

            float[] first = ArrConv.subarray_as_float(
                start1,
                end1,
                aa);
            float[] last = ArrConv.subarray_as_float(
                start2,
                end2,
                aa);
            float median25 = Median.GetMedian(first);
            float median75 = Median.GetMedian(last);

            return (median25 - 2.0F*median50 + median75)/(median75 - median25);
        }


        // Quartile skewness of a 1D array of decimal
        public static decimal quartileSkewness(decimal[] aa)
        {
            int n = aa.Length;
            decimal median50 = Median.GetMedian(aa);
            int start1 = 0;
            int start2 = 0;
            int end1 = n/2 - 1;
            int end2 = n - 1;
            if (Fmath.isOdd(n))
            {
                start2 = end1 + 2;
            }
            else
            {
                start2 = end1 + 1;
            }
            decimal[] first = ArrConv.subarray_as_decimal(
                start1,
                end1,
                aa);
            decimal[] last = ArrConv.subarray_as_decimal(
                start2,
                end2,
                aa);
            decimal median25 = Median.GetMedian(first);
            decimal median75 = Median.GetMedian(last);
            decimal ret1 = (median25 - (median50*(decimal) 2.0)) + (median75);
            decimal ret2 = median75 - (median25);
            decimal ret = ret1/(ret2);
            first = null;
            last = null;
            median25 = 0;
            median50 = 0;
            median75 = 0;
            ret1 = 0;
            ret2 = 0;
            return ret;
        }

        // Quartile skewness of a 1D array of long
        public static decimal quartileSkewness(long[] aa)
        {
            decimal[] bd = ArrConv.array_as_decimal(aa);
            return quartileSkewness(bd);
        }


        // Quartile skewness of a 1D array of int
        public static double quartileSkewness(int[] aa)
        {
            int n = aa.Length;
            double median50 = Median.GetMedian(aa);
            int start1 = 0;
            int start2 = 0;
            int end1 = n/2 - 1;
            int end2 = n - 1;
            if (Fmath.isOdd(n))
            {
                start2 = end1 + 2;
            }
            else
            {
                start2 = end1 + 1;
            }
            double[] first = ArrConv.subarray_as_double(start1, end1, aa);
            double[] last = ArrConv.subarray_as_double(start2, end2, aa);
            double median25 = Median.GetMedian(first);
            double median75 = Median.GetMedian(last);

            return (median25 - 2.0*median50 + median75)/(median75 - median25);
        }
    }
}
