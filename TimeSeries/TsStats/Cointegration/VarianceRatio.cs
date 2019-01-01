#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public class VarianceRatio
    {
        public static List<double> GetVarianceRatios(List<double> data, int intSamples)
        {
            var varianceRatios = new List<double>();
            for (int i = 0; i < intSamples; i++)
            {
                varianceRatios.Add(GetVarianceRatio(data, i));
            }
            return varianceRatios;
        }

        public static double GetVarianceRatio(List<double> data, int intTau)
        {
            List<double> derivative1 = TimeSeriesHelper.GetDerivative(1, data);
            List<double> derivativeTau = TimeSeriesHelper.GetDerivative(intTau, data);
            double dblMean1 = derivative1.Average();
            double dblMeanTau = derivativeTau.Average();

            return (from n in derivativeTau select Math.Pow(n - dblMeanTau, 2)).Sum()/
            (intTau*(from n in derivative1 select Math.Pow(n - dblMean1, 2)).Sum());
        }
    }
}

