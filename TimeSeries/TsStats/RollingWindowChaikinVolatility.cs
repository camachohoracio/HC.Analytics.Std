using System;
using System.Collections.Generic;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowChaikinVolatility : ITechnicalIndicator
    {
        public double Indicator { get { return ChaikinVolatility; }}

        public bool IsReady
        {
            get
            {
                return 
                    m_listLow.Count >= m_intPeriods &&
                    !double.IsNaN(ChaikinVolatility); 
            }
        }

        public double ChaikinVolatility { get; private set; }

        private readonly int m_intPeriods;
        private ExMAvgFunction m_rollingWindowEmaRange;
        private ExMAvgFunction m_rollingWindowVsLow;
        private List<double> m_listLow;

        private DateTime m_prevTime;

        public RollingWindowChaikinVolatility(
            int intPeriods)
        {
            ChaikinVolatility = double.NaN;
            m_intPeriods = intPeriods;
            m_rollingWindowEmaRange = new ExMAvgFunction(intPeriods);
            m_rollingWindowVsLow = new ExMAvgFunction(intPeriods);
            m_listLow = new List<double>();
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
                m_rollingWindowEmaRange.Update(
                    new TsRow2D(time,
                    dblHigh - dblLow));
                m_listLow.Add(dblLow);
                if (m_listLow.Count > m_intPeriods)
                {
                    m_listLow.RemoveAt(0);
                    m_rollingWindowVsLow.Update(new TsRow2D(
                        time,
                        dblHigh - m_listLow[0]));

                    double dblEmaRange = m_rollingWindowEmaRange.LastMovingAverage;
                    double dblEmaVsLow = m_rollingWindowVsLow.LastMovingAverage;

                    if(dblEmaVsLow == 0)
                    {
                        dblEmaVsLow = 1e-6;
                    }

                    ChaikinVolatility = 100.0 * (dblEmaRange - dblEmaVsLow) / dblEmaVsLow;

                    if(double.IsNaN(ChaikinVolatility))
                    {
                        throw new HCException("invalid chaikinVolatility");
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
            if(m_rollingWindowEmaRange != null)
            {
                m_rollingWindowEmaRange.Dispose();
                m_rollingWindowEmaRange = null;
            }
            if (m_rollingWindowVsLow != null)
            {
                m_rollingWindowVsLow.Dispose();
                m_rollingWindowVsLow = null;
            }
            if (m_listLow != null)
            {
                m_listLow.Clear();
                m_listLow = null;
            }
        }
    }
}