#region

using System;
using System.Collections.Generic;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Statistics
{
    public static class Correlation
    {
        public static double GetCorrelation(
            List<double> list1,
            List<double> list2)
        {
            double dblSx = 0;
            double dblSx2 = 0;
            double dblSxy = 0;
            double dblSy = 0;
            double dblSy2 = 0;
            int intN = 0;

            if (list1.Count != list2.Count)
            {
                throw new HCException("Invalid list size");
            }

            for (int i = 0; i < list1.Count; i++)
            {
                double dblVal1 = list1[i];
                double dblVal2 = list2[i];
                dblSx += dblVal1;
                dblSx2 += dblVal1 * dblVal1;
                dblSxy += dblVal1 * dblVal2;
                dblSy += dblVal2;
                dblSy2 += dblVal2 * dblVal2;
                intN++;
            }

            double dblCorrelation = 0;

            if (!(dblSxy == 0 && (dblSx == 0 || dblSy == 0)))
            {
                dblCorrelation = (intN * dblSxy - dblSx * dblSy) /
                    Math.Sqrt((intN * dblSx2 - dblSx * dblSx) * (intN * dblSy2 - dblSy * dblSy));
            }

            if (double.IsNaN(dblCorrelation) || double.IsInfinity(dblCorrelation))
            {
                return 0;
            }

            if (dblCorrelation < -1.0)
            {
                return -1;
            }
            if (dblCorrelation > 1.0)
            {
                return 1;
            }
            return dblCorrelation;
        }
    }
}

