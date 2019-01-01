#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;
using NUnit.Framework;
using HC.Analytics.Statistics;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public static class NormalDistrHypTest
    {
        [Test]
        public static void UnitTest()
        {
            var rng = new Random.RngWrapper();
            var randoms = new List<double>();
            var normal = new List<double>();
            for (int i = 0; i < 1000; i++)
            {
                double dblRandom = rng.NextDouble();
                randoms.Add(dblRandom);
                normal.Add(alglib.normaldistr.invnormaldistribution(dblRandom));
            }
            bool blnRandom = !IsNormallyDistributed(randoms);
            bool blnNormal = IsNormallyDistributed(normal);

            if(!blnNormal || !blnRandom)
            {
                throw new HCException("Invalid result");
            }
        }

        public static bool IsNormallyDistributed(List<double> data)
        {
            return IsNormallyDistributed(data, 0.05);
        }

        public static bool IsNormallyDistributed(List<double> data,
            double dblAlpha)
        {
            try
            {
                if (data == null || data.Count == 0)
                {
                    return false;
                }

                data = data.ToList();
                double dblSkewness = Skewness.momentSkewness(data.ToArray());
                double dblKurtosis = Kurtosis.kurtosis(data.ToArray());
                double dblJarkeBera = (data.Count/6.0)*(Math.Pow(dblSkewness, 2) +
                                                        (Math.Pow(dblKurtosis - 3.0, 2)/4.0));
                double dblCriticalValue = alglib.chisquaredistr.invchisquaredistribution(
                    2, dblAlpha);

                bool blnRejectNullHyp = dblJarkeBera < dblCriticalValue;
                return blnRejectNullHyp;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }
    }
}

