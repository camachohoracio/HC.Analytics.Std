#region

using System;
using System.Linq;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Wavelets
{
    public class HaarFilter : IDisposable
    {
        #region Members

        private const int MIN_DATA_SIZE = 260;
        private const int MAX_DATA_SIZE = 500;
        private readonly RollingWindowTsFunction m_rollingWindowTsFunction;

        #endregion

        #region Constructors

        public HaarFilter()
        {
            m_rollingWindowTsFunction = new RollingWindowTsFunction(MAX_DATA_SIZE);
        }

        #endregion

        #region Public

        public bool IsReady()
        {
            return m_rollingWindowTsFunction.Data.Count > MIN_DATA_SIZE;
        }

        public void Filter(
            DateTime dateTime,
            double dblValue,
            out double dblFilteredValue,
            out double dblNoise)
        {
            m_rollingWindowTsFunction.Update(
                dateTime,
                dblValue);
            dblFilteredValue = double.NaN;
            dblNoise = double.NaN;
            if (m_rollingWindowTsFunction.Data.Count < MIN_DATA_SIZE)
            {
                return;
            }
            var data = (from n in m_rollingWindowTsFunction.Data select n.Fx).ToList();
            int intNearesPower2 = binary.nearestPower2(data.Count);
            var getLastValues = WaveletHelper.GetLastNValues(data, intNearesPower2);
            var noiseFilter = new noise_filter();
            double[] results;
            double[] noise;
            noiseFilter.filter_time_series(
                getLastValues.ToArray(),
                out results,
                out noise);
            dblFilteredValue = results.Last();
            dblNoise = noise.Last();
        }

        #endregion

        public void Dispose()
        {
            m_rollingWindowTsFunction.Dispose();
        }
    }
}

