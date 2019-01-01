using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowAtr : ITechnicalIndicator
    {
        public double Indicator { get { return Atr; }}

        public bool IsReady
        {
            get { return m_tr.Count >= m_intPeriods &&
                    !double.IsNaN(Atr); }
        }


        public double Atr { get; private set; }

        private readonly int m_intPeriods;
        private List<double> m_tr;
        private double m_dblPrevAtr;
        private DateTime m_prevTime;
        private double m_dblPrevClose;

        public RollingWindowAtr(
            int intPeriods)
        {
            m_dblPrevClose = double.NaN;
            m_dblPrevAtr = double.NaN;
            Atr = double.NaN;
            m_intPeriods = intPeriods;
            m_tr = new List<double>();
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

                if(double.IsNaN(
                    dblHigh) ||
                    double.IsNaN(
                    dblLow) ||
                    double.IsNaN(
                    dblClose))
                {
                    throw new HCException("NaN value");
                }

                if(!double.IsNaN(m_dblPrevClose))
                {
                    double dblTr =
                        Math.Max(
                            Math.Max(
                                dblHigh - dblLow,
                                Math.Abs(dblHigh - m_dblPrevClose)),
                            Math.Abs(dblLow - m_dblPrevClose));
                    m_tr.Add(dblTr);
                    if(m_tr.Count > m_intPeriods)
                    {
                        m_tr.RemoveAt(0);
                        if(double.IsNaN(m_dblPrevAtr))
                        {
                            Atr = m_tr.Sum()/m_intPeriods;
                        }
                        else
                        {
                            Atr = (m_dblPrevAtr*(m_intPeriods - 1) + dblTr)/m_intPeriods;
                        }
                        if(double.IsNaN(Atr))
                        {
                            throw new HCException("NaN ATR");
                        }
                        m_dblPrevAtr = Atr;
                    }
                }

                m_dblPrevClose = dblClose;
                m_prevTime = time;

            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Dispose()
        {
            if(m_tr != null)
            {
                m_tr.Clear();
                m_tr = null;
            }
        }
    }
}
