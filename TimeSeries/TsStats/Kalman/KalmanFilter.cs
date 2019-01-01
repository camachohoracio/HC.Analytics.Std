using System;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.Kalman
{
    public class KalmanFilter : IDisposable
    {
        #region Members

        private const int MIN_DATA_SIZE = 10;
        private const double SMOOTH_FACTOR = 0.05;
        private Kalman1D m_kalman1D;
        private int m_intSize;

        #endregion

        #region Constructors

        public KalmanFilter() : this(SMOOTH_FACTOR) { }

        public KalmanFilter(double dblSmothFactor)
        {
            m_kalman1D = new Kalman1D();
            m_kalman1D.Reset(
                0.1,
                0.1,
                dblSmothFactor,
                400,
                0);
        }

        #endregion

        #region Public

        public bool IsReady()
        {
            return m_intSize > MIN_DATA_SIZE;
        }

        public double Predict()
        {
            return m_kalman1D.Predicition(1);
        }

        public bool Filter(
            double  dblValue,
            out double dblFilteredValue,
            out double dblNoise)
        {
            dblFilteredValue = 0;
            dblNoise = 0;
            try
            {
                double dblPrediction = m_kalman1D.Update(dblValue, 1);
                dblFilteredValue = dblValue;
                dblNoise = 0;
                m_intSize++;

                if (m_intSize < MIN_DATA_SIZE)
                {
                    return false;
                }

                if (!double.IsNaN(dblValue))
                {
                    dblFilteredValue = dblPrediction;
                    dblNoise = dblValue - dblPrediction;
                }
                return true;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        #endregion

        public void Dispose()
        {
            m_kalman1D.Dispose();
            m_kalman1D = null;
        }
    }
}

