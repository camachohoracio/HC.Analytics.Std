#region

using System;
using HC.Analytics.TimeSeries.TsStats.Kalman;
using HC.Analytics.TimeSeries.TsStats.Wavelets;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class FilterWrapper : IDisposable
    {
        private HaarFilter m_haarFilter;
        private KalmanFilter m_kalmanFilter;

        #region Constructors

        public FilterWrapper()
        {
            m_haarFilter= new HaarFilter();
            m_kalmanFilter = new KalmanFilter();
        }

        #endregion

        public bool IsReady()
        {
            return m_haarFilter.IsReady() ||
                   m_kalmanFilter.IsReady();
        }

        public double Filter(
            DateTime dateTime,
            double dblValue)
        {
            try
            {
                if (double.IsNaN(dblValue))
                {
                    return dblValue;
                }
                double dblFilteredHaarValue;
                double dblNoise;
                m_haarFilter.Filter(
                    dateTime,
                    dblValue,
                    out dblFilteredHaarValue,
                    out dblNoise);
                double dblFilteredKalmanValue;
                m_kalmanFilter.Filter(
                    dblValue,
                    out dblFilteredKalmanValue,
                    out dblNoise);
                if (!m_haarFilter.IsReady() ||
                    !m_kalmanFilter.IsReady())
                {
                    if (m_kalmanFilter.IsReady())
                    {
                        return dblFilteredKalmanValue;
                    }
                    if (m_haarFilter.IsReady())
                    {
                        return dblFilteredHaarValue;
                    }
                    return dblValue;
                }

                double dblFiltered = (dblFilteredHaarValue + dblFilteredKalmanValue)/2.0;
                if(double.IsNaN(dblFiltered))
                {
                    return dblValue;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return dblValue;
        }

        public void Dispose()
        {
            m_haarFilter.Dispose();
            m_haarFilter = null;
            m_kalmanFilter.Dispose();
            m_kalmanFilter = null;
        }
    }
}

