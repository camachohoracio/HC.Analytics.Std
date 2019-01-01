#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public static class SyntheticTsFactory
    {
        public static List<double> GetRandom(int intSize)
        {
            var rng = new Random();
            var outList = new List<double>();
            for (int i = 0; i < intSize; i++)
            {
                outList.Add(rng.NextDouble());
            }
            return outList;
        }

        public static List<double> GetRandomWalk(
            int intSize,
            double dblTrend)
        {
            var rng = new Random();
            var outList = new List<double>();
            double dblCurrVal = rng.NextDouble();
            for (int i = 0; i < intSize; i++)
            {
                int intSign = rng.NextDouble() > 0.5 ? 1 : 0;
                dblCurrVal = dblCurrVal + intSign * rng.NextDouble() + 
                    dblTrend * rng.NextDouble();
                outList.Add(dblCurrVal);
            }
            return outList;
        }
    }
}

