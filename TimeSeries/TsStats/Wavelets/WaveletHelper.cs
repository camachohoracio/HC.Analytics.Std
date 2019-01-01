using System.Collections.Generic;
using HC.Core.Exceptions;

namespace HC.Analytics.TimeSeries.TsStats.Wavelets
{
    public static class WaveletHelper
    {
        public static List<double> GetLastNValues(
            List<double> data,
            int intN)
        {
            var outData = new List<double>();
            int intIndex = data.Count - intN;
            for (int i = intIndex; i < data.Count; i++)
            {
                outData.Add(data[i]);
            }
            if(outData.Count != intN)
            {
                throw new HCException("Invalid vector size");
            }
            return outData;
        }
    }
}

