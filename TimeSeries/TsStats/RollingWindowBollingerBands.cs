using System;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowBollingerBands : ITechnicalIndicator
    {
        public double Indicator { get { return BollingerBand; }}

        public bool IsReady
        {
            get
            {
                return m_rollingWindowStdDev.IsReady() &&
                !double.IsNaN(BollingerBand); }
        }

        public double BollingerBand { get; private set; }

        private readonly int m_intPeriods;
        private RollingWindowStdDev m_rollingWindowStdDev;
        private DateTime m_prevTime;

        public RollingWindowBollingerBands(
            int intPeriods)
        {
            BollingerBand = double.NaN;
            m_intPeriods = intPeriods;
            m_rollingWindowStdDev = new RollingWindowStdDev(intPeriods);
        }

        public string Name()
        {
            return GetType().Name + "[" +
                m_intPeriods + "]";
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
                dblHigh,
                dblLow,
                dblClose);
        }

        public void Update(
            DateTime time,
            double dblHigh,
            double dblLow,
            double dblClose)
        {
            try
            {
                if (time < m_prevTime)
                {
                    throw new HCException("Invalid prev time");
                }

                if (double.IsNaN(
                    dblHigh) ||
                    double.IsNaN(
                        dblLow) ||
                    double.IsNaN(
                        dblClose))
                {
                    throw new HCException("NaN value");
                }
                m_rollingWindowStdDev.Update(
                    time,
                    dblClose);
                if (m_rollingWindowStdDev.IsReady())
                {
                    BollingerBand = dblClose - (m_rollingWindowStdDev.Mean +
                    2.0*m_rollingWindowStdDev.StdDev);
                }

                m_prevTime = time;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Dispose()
        {
            if(m_rollingWindowStdDev != null)
            {
                m_rollingWindowStdDev.Dispose();
                m_rollingWindowStdDev = null;
            }
        }
    }
}