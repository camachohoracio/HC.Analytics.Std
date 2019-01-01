using System;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowMeanCross : ITechnicalIndicator, ICrossingIndicator
    {
        private object m_lockObject = new object();
        public RollingWindowMean RwFast { get; private set; }
        public RollingWindowMean RwSlow { get; private set; }
        private double m_dblRwSlow;
        private double m_dblRwFast;

        public double Indicator => 0;

        bool ITechnicalIndicator.IsReady => IsReady();

        public bool IsReady()
        {
            return
                RwSlow.IsReady();
        }

        public DateTime TimeOfCrossing { get; set; }

        public DateTime LastUpdateTime { get; set; }
        public double StochasticSlow => m_dblRwSlow;
        public double StochasticFast => m_dblRwFast;

        public bool IsGoingUp =>
            GetIsGoingUp(
                StochasticSlow,
                StochasticFast);

        private bool m_blnPrevIsGoingUp;
        private bool m_blnLastCrossingGoingUp;

        public bool GetIsGoingUp(
            double dblStochasticSlow,
            double dblStochasticFast)
        {
            return dblStochasticFast < dblStochasticSlow;
        }

        public bool GetLastCrossIsGoingUp()
        {
            return m_blnLastCrossingGoingUp;
        }

        public RollingWindowMeanCross(
            int intPeriodsSignal1Slow,
            int intPeriodsSignal1Fast)
        {
            RwSlow = new RollingWindowMean(
                intPeriodsSignal1Slow);
            RwFast = new RollingWindowMean(
                intPeriodsSignal1Fast);
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

                    RwFast.Update(
                        dateTime,
                        dblClose);

                    if (!RwFast.IsReady())
                    {
                        return;
                    }
                    RwSlow.Update(
                        dateTime,
                        dblClose);

                    m_dblRwSlow = RwSlow.Mean;
                    m_dblRwFast = RwFast.Mean;

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

        public RollingWindowMacdCross Clone()
        {
            var clone = 
                (RollingWindowMacdCross)MemberwiseClone();
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
