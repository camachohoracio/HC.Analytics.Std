using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowAroon : ITechnicalIndicator
    {
        public double Indicator { get { return Aroon; }}

        public bool IsReady
        {
            get { return m_low.Count >= m_intPeriods &&
                !double.IsNaN(Aroon); }
        }

        public double Aroon { get; private set; }

        private readonly int m_intPeriods;
        private List<double> m_high;
        private List<double> m_low;
        private DateTime m_prevTime;

        public RollingWindowAroon(
            int intPeriods)
        {
            Aroon = double.NaN;
            m_intPeriods = intPeriods;
            m_high = new List<double>();
            m_low = new List<double>();
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
                m_low.Add(dblLow);
                m_high.Add(dblHigh);
                if (m_low.Count > m_intPeriods)
                {
                    m_low.RemoveAt(0);
                    m_high.RemoveAt(0);
                    double dblLowest = m_low.Min();
                    double dblHighest = m_high.Max();
                    int intPeriodsSinceHigh = m_intPeriods - m_high.IndexOf(dblHighest);
                    int intPeriodsSinceLow = m_intPeriods - m_low.IndexOf(dblLowest);

                    //Aroon Up = ((25 - Days since 25-day high) / 25) x 100
                    //Aroon Down = ((25 - Days since 25-day low) / 25) x 100
                    double dblAaronUp = ((m_intPeriods - intPeriodsSinceHigh)*1.0/m_intPeriods)*100.0;
                    double dblAaronDown = ((m_intPeriods - intPeriodsSinceLow) * 1.0 / m_intPeriods) * 100.0;

                    double dblCurrRange = (dblAaronUp - dblAaronDown);
                    double dblMidPointDelta = dblCurrRange / 2.0;

                    if (double.IsNaN(dblMidPointDelta))
                    {
                        throw new HCException("NaN mid point delta");
                    }
                    Aroon = dblMidPointDelta;
                    if (double.IsNaN(Aroon))
                    {
                        throw new HCException("NaN Aroon");
                    }
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
            if(m_high != null)
            {
                m_high.Clear();
                m_high = null;
            }
            if (m_low != null)
            {
                m_low.Clear();
                m_low = null;
            }
        }
    }
}