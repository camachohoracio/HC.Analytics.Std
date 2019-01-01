using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowSmi : ITechnicalIndicator
    {
        public double Indicator { get { return Smi; }}

        public bool IsReady
        {
            get { return m_low.Count >= m_intPeriods &&
                !double.IsNaN(Smi); }
        }

        public double Smi { get; private set; }

        private readonly int m_intPeriods;
        private readonly List<double> m_high;
        private readonly List<double> m_low;
        private DateTime m_prevTime;
        private ExMAvgFunction m_emaMidPoint1;
        private ExMAvgFunction m_emaMidPoint2;
        private ExMAvgFunction m_emaRange1;
        private ExMAvgFunction m_emaRange2;

        public RollingWindowSmi(
            int intPeriods)
        {
            Smi = double.NaN;
            m_intPeriods = intPeriods;
            m_high = new List<double>();
            m_low = new List<double>();
            m_emaMidPoint1 = new ExMAvgFunction(20, false);
            m_emaMidPoint2 = new ExMAvgFunction(30, false);
            m_emaRange1 = new ExMAvgFunction(20, false);
            m_emaRange2 = new ExMAvgFunction(30, false);
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
                    double dblCurrRange = (dblHighest - dblLowest);
                    double dblMidPointDelta =
                        dblClose - dblCurrRange/2.0;

                    if (double.IsNaN(dblMidPointDelta))
                    {
                        throw new HCException("NaN mid point delta");
                    }
                    m_emaMidPoint1.Update(
                        new TsRow2D(time, dblMidPointDelta));
                    m_emaMidPoint2.Update(
                        new TsRow2D(
                            time,
                            m_emaMidPoint1.LastMovingAverage));

                    m_emaRange1.Update(
                        new TsRow2D(
                            time,
                            dblCurrRange));
                    m_emaRange2.Update(
                        new TsRow2D(
                            time,
                            m_emaRange1.LastMovingAverage));

                    double dblMovAverage = m_emaRange2.LastMovingAverage;
                    Smi =
                        dblMovAverage == 0
                            ? 0
                            : (100.0*m_emaMidPoint2.LastMovingAverage/
                               dblMovAverage);
                    
                    if (double.IsNaN(Smi))
                    {
                        throw new HCException("NaN SMI");
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
            if(m_emaMidPoint1 != null)
            {
                m_emaMidPoint1.Dispose();
                m_emaMidPoint1 = null;
            }
            if (m_emaMidPoint2 != null)
            {
                m_emaMidPoint2.Dispose();
                m_emaMidPoint2 = null;
            }
            if (m_emaRange1 != null)
            {
                m_emaRange1.Dispose();
                m_emaRange1 = null;
            }
            if (m_emaRange2 != null)
            {
                m_emaRange2.Dispose();
                m_emaRange2 = null;
            }
        }
    }
}
