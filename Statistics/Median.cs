#region

using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Mathematics;

#endregion

namespace HC.Analytics.Statistics
{
    public class Median
    {
        // MEDIANS
        // Median of a 1D array of decimal, aa
        public static decimal GetMedian(decimal[] aa)
        {
            int n = aa.Length;
            int nOverTwo = n/2;
            decimal med;
            decimal[] bb = (decimal[]) aa.Clone();
            if (Fmath.isOdd(n))
            {
                med = bb[nOverTwo];
            }
            else
            {
                med = (bb[nOverTwo - 1] + (bb[nOverTwo]))/((decimal) (2.0));
            }
            return med;
        }

        // Median of a 1D array of long, aa
        public static long GetMedian(long[] aa)
        {
            int n = aa.Length;
            int nOverTwo = n/2;
            long med;
            long[] bb = (long[]) aa.Clone();
            if (Fmath.isOdd(n))
            {
                med = bb[nOverTwo];
            }
            else
            {
                med = (bb[nOverTwo - 1] + bb[nOverTwo])/2;
            }
            return med;
        }

        // Median of a 1D array of doubles, aa
        public static double GetMedian(double[] aa)
        {
            int n = aa.Length;
            int nOverTwo = n/2;
            double dblMed;
            double[] bb = Fmath.selectionSort(aa);
            if (Fmath.isOdd(n))
            {
                dblMed = bb[nOverTwo];
            }
            else
            {
                dblMed = (bb[nOverTwo - 1] + bb[nOverTwo])/2.0D;
            }

            return dblMed;
        }

        public static double GetMedian(List<double> aa)
        {
            int nOverTwo = aa.Count/2;
            aa.Sort();
            if (Fmath.isOdd(aa.Count))
            {
                return aa[nOverTwo];
            }
            return (aa[nOverTwo - 1] + aa[nOverTwo])/2.0D;
        }


        public static T GetMedian<T,K>(SortedDictionary<T,K> aa)
        {
            int n = aa.Count;
            int nOverTwo = n / 2;
            var bb = aa.Keys.ToList();
            return bb[nOverTwo];
        }


        // Median of a 1D array of floats, aa
        public static float GetMedian(float[] aa)
        {
            int n = aa.Length;
            int nOverTwo = n/2;
            float med = 0.0F;
            float[] bb = Fmath.selectionSort(aa);
            if (Fmath.isOdd(n))
            {
                med = bb[nOverTwo];
            }
            else
            {
                med = (bb[nOverTwo - 1] + bb[nOverTwo])/2.0F;
            }

            return med;
        }

        // Median of a 1D array of int, aa
        public static double GetMedian(int[] aa)
        {
            int n = aa.Length;
            int nOverTwo = n/2;
            double med = 0.0D;
            int[] bb = Fmath.selectionSort(aa);
            if (Fmath.isOdd(n))
            {
                med = bb[nOverTwo];
            }
            else
            {
                med = (bb[nOverTwo - 1] + bb[nOverTwo])/2.0D;
            }

            return med;
        }
    }
}
