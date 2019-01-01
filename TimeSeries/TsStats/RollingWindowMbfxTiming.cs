using System;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowMbfxTiming : ITechnicalIndicator, ICrossingIndicator
    {
        private object m_lockObject = new object();
        private readonly RollingWindowMean m_rwMiddle;
        private readonly RollingWindowMean m_rwScale;
        private double m_dblRwSlow;
        private double m_dblRwFast;

        public double Mbfx { get; private set; }
        public double Indicator => Mbfx;

        bool ITechnicalIndicator.IsReady => IsReady();

        public bool IsReady()
        {
            return
                m_rwSlow.IsReady();
        }

        public DateTime TimeOfCrossing { get; set; }
        public bool GetLastCrossIsGoingUp()
        {
            return m_blnLastCrossingGoingUp;
        }

        public DateTime LastUpdateTime { get; set; }
        public double StochasticSlow => m_dblRwSlow;
        public double StochasticFast => m_dblRwFast;

        
        public bool IsGoingUp =>
            GetIsGoingUp(
                StochasticSlow,
                StochasticFast);

        private bool m_blnPrevIsGoingUp;
        private readonly RollingWindowMean m_rwSlow;
        private readonly double m_dblScaleFactor;
        private bool m_blnLastCrossingGoingUp;

        public bool GetIsGoingUp(
            double dblStochasticSlow,
            double dblStochasticFast)
        {
            return dblStochasticFast > dblStochasticSlow;
        }

        public RollingWindowMbfxTiming(
            int intMaFast,
            int intMaSlow)
        {
            m_rwMiddle = new RollingWindowMean(intMaFast);
            m_rwScale = new RollingWindowMean(intMaFast);
            m_rwSlow = new RollingWindowMean(intMaSlow);
            m_dblScaleFactor = 1.0/intMaFast;
        }

        #region Public

        public void Update(
            DateTime dateTime,
            double dblClose,
            double dblLow,
            double dblHigh,
            double dblVolume)
        {
            try
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
                        throw new HCException("invalid close price");
                    }

                    if (dblHigh < dblLow)
                    {
                        throw new HCException("invalid high/low");
                    }

                    double dblDiff = dblHigh + dblLow;
                    m_rwMiddle.Update(
                        dateTime,
                         dblDiff / 2.0);
                    m_rwScale.Update(
                        dateTime,
                        dblDiff);

                    if (!m_rwMiddle.IsReady())
                    {
                        return;
                    }
                    double dblScale =
                        m_rwScale.Mean*m_dblScaleFactor;
                    double dblMiddle =
                        m_rwMiddle.Mean;
                    double dblH =
                        (dblHigh - dblMiddle) / dblScale;
                    double dblL =
                        (dblLow - dblMiddle) / dblScale;
                    double dblC =
                        (dblClose - dblMiddle) / dblScale;

                    Mbfx = (dblH + dblL + dblC)/3.0;
                    m_rwSlow.Update(Mbfx);
                    m_dblRwFast = Mbfx;
                    m_dblRwSlow = m_rwSlow.Mean;

                    if (IsGoingUp != m_blnPrevIsGoingUp)
                    {
                        TimeOfCrossing = dateTime;
                        m_blnLastCrossingGoingUp = IsGoingUp;
                    }
                    m_blnPrevIsGoingUp = IsGoingUp;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                LastUpdateTime = dateTime;
            }
        }


        #endregion

        public RollingWindowMbfxTiming Clone()
        {
            var clone =
                (RollingWindowMbfxTiming)MemberwiseClone();
            return clone;
        }


        public string Name()
        {
            return GetType().Name;
        }


        public void Dispose()
        {
            m_lockObject = null;
        }

        public void Update(DailyTsEvent ev)
        {
            Update(
                ev.Time,
                ev.AdjClose,
                ev.Low,
                ev.High,
                ev.Volume);
        }
    }
}
