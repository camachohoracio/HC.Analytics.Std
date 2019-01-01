using System;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowMacd : ITechnicalIndicator
    {
        public double Indicator { get { return Macd; }}

        public bool IsReady
        {
            get { return !double.IsNaN(Macd); }
        }

        public double Macd { get; private set; }

        private readonly int m_intPeriods;
        private DateTime m_prevTime;
        private ExMAvgFunction m_emaSignal;
        private ExMAvgFunction m_fastEma;
        private ExMAvgFunction m_slowEma;

        public RollingWindowMacd(
            int intPeriods) : this(
                Math.Max(intPeriods / 2, 4),
                intPeriods,
                intPeriods * 2)
        {
        }

        public RollingWindowMacd(
            int intPeriodsSignal,
            int intPeriodsFast,
            int intPeriodsSlow)
        {
            Macd = double.NaN;
            m_intPeriods = intPeriodsFast;
            m_emaSignal = new ExMAvgFunction(intPeriodsSignal, false);
            m_fastEma = new ExMAvgFunction(intPeriodsFast, false);
            m_slowEma = new ExMAvgFunction(intPeriodsSlow, false);
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
                dblClose);
        }

        public void Update(
            DateTime time,
            double dblClose)
        {
            try
            {
                if (time < m_prevTime)
                {
                    throw new HCException("Invalid prev time");
                }

                if (double.IsNaN(
                        dblClose))
                {
                    throw new HCException("NaN value");
                }

                m_slowEma.Update(
                    new TsRow2D(
                        time, 
                        dblClose));

                m_fastEma.Update(
                    new TsRow2D(
                        time,
                        dblClose));

                double dblMacdLine = 
                    m_fastEma.LastMovingAverage - 
                    m_slowEma.LastMovingAverage;

                m_emaSignal.Update(
                    new TsRow2D(
                        time,
                        dblMacdLine));

                Macd =
                    m_emaSignal.LastMovingAverage;
                if (double.IsNaN(Macd))
                {
                    throw new HCException("NaN Macd");
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
            if(m_emaSignal != null)
            {
                m_emaSignal.Dispose();
                m_emaSignal = null;
            }
            if (m_fastEma != null)
            {
                m_fastEma.Dispose();
                m_fastEma = null;
            }
            if (m_slowEma != null)
            {
                m_slowEma.Dispose();
                m_slowEma = null;
            }
        }
    }
}
