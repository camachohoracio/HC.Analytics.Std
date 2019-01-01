using System;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowObv : ITechnicalIndicator
    {
        private object m_lockObject = new object();
        private double m_dblPrevClose;

        #region Properties

        public double Indicator { get { return Obv; } }
        bool ITechnicalIndicator.IsReady { get { return true; }}

        public double Obv
        {
            get;
            set;
        }

        public double LastValue { get; set; }

        public DateTime LastUpdateTime { get; set; }


        #endregion

        #region Public

        public void Update(
            DateTime dateTime,
            double dblClose,
            double dblVolume)
        {
            lock (m_lockObject)
            {
                if (LastUpdateTime > dateTime &&
                    dateTime != DateTime.MinValue)
                {
                    throw new Exception("Invalid time");
                }

                if (double.IsNaN(dblClose) ||
                    double.IsInfinity(dblClose))
                {
                    throw new Exception("Invalid value");
                }

                LastValue = dblClose;
                double dblMultiplier = 1;
                if(dblClose < m_dblPrevClose)
                {
                    dblMultiplier = -1;
                }
                LastUpdateTime = dateTime;

                double dblPrevObv = Obv;
                double dblObv = dblPrevObv + dblMultiplier * dblVolume;
                Obv = dblObv;
                m_dblPrevClose = dblClose;
            }
        }


        #endregion

        public RollingWindowObv Clone()
        {
            var clone = (RollingWindowObv)MemberwiseClone();
            return clone;
        }


        public string Name()
        {
            return GetType().Name;
        }

        public void Update(
            DateTime dateTime, 
            double dblClose, 
            double dblLow, 
            double dblHigh, 
            double dblVolume)
        {
            Update(
                dateTime,
                dblClose,
                dblVolume);
        }

        public void Dispose()
        {
            m_lockObject = null;
        }
    }
}
