using System;
using System.Collections.Generic;
using HC.Analytics.TimeSeries;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.Statistics
{
    public static class StatsHelper
    {
        public static double GetMeanFromList(List<Double> list) 
	    {
		    if(list == null || list.Count == 0)
		    {
			    return 0;
		    }
		    double dblMean = 0;
		    foreach(double dbl in list)
		    {
			    dblMean += dbl;
		    }
		    return dblMean / list.Count;
	    }

        public static double GetCorrelationFromTs(
                List<TsRow2D> list1,
                List<TsRow2D> list2)
        {
            try
            {
                double dblSx = 0;
                double dblSx2 = 0;
                double dblSxy = 0;
                double dblSy = 0;
                double dblSy2 = 0;
                int intN = 0;

                for (int i = 0; i < Math.Min(list1.Count, list2.Count); i++)
                {
                    if (list1[i].Time.Ticks != list2[i].Time.Ticks)
                    {
                        throw new HCException("Invalid date");
                    }

                    double dblVal1 = list1[i].Fx;
                    double dblVal2 = list2[i].Fx;
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

                if (Double.IsNaN(dblCorrelation) || Double.IsInfinity(dblCorrelation))
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
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}
