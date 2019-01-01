using System;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowStochasticCross : ITechnicalIndicator, ICrossingIndicator
    {
        private object m_lockObject = new object();
        public RollingWindowTsEvent<DailyTsEvent> RwEv { get; private set; }
        public RollingWindowMean RwFast { get; private set; }
        public RollingWindowMean RwSlow { get; private set; }
        private double m_dblRwSlow;
        private double m_dblRwFast;

        public double Indicator => RwSlow.Mean;

        bool ITechnicalIndicator.IsReady => IsReady();

        public bool IsReady()
        {
            return
                RwSlow.IsReady();
        }

        private bool m_blnLastCrossIsGoingUp;

        public DateTime TimeOfCrossing { get; set; }
        public bool GetLastCrossIsGoingUp()
        {
            return m_blnLastCrossIsGoingUp;
        }

        public DateTime LastUpdateTime { get; set; }
        public double StochasticSlow => m_dblRwSlow;
        public double StochasticFast => m_dblRwFast;

        public bool IsGoingUp =>
            GetIsGoingUp(
                StochasticSlow,
                StochasticFast);

        private bool m_blnPrevIsGoingUp;

        public bool GetIsGoingUp(
            double dblStochasticSlow,
            double dblStochasticFast)
        {
            return dblStochasticFast > dblStochasticSlow;
        }

        public RollingWindowStochasticCross(
            int intSmaFast,
            int intSmaSlow,
            int intDays)
        {
            RwEv =
                new RollingWindowTsEvent<DailyTsEvent>(intDays);
            RwSlow = new RollingWindowMean(intSmaSlow);
            RwFast = new RollingWindowMean(intSmaFast);
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
                    if (LastUpdateTime >= dateTime &&
                        dateTime != DateTime.MinValue)
                    {
                        throw new Exception("Invalid time last [" +
                                            LastUpdateTime + "] >= curr["  +
                                            dateTime +
                                            "]");
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

                    DailyTsEvent ev =
                        new DailyTsEvent
                        {
                            Close = dblClose,
                            High = dblHigh,
                            Low = dblLow,
                            AdjClose = dblClose,
                            Time = dateTime
                        };

                    RwEv.Update(
                        ev);

                    if (!RwEv.IsReady)
                    {
                        return;
                    }

                    double dblMin = 
                        (from n in RwEv.TsList
                         select n.Low).Min();
                    double dblMax = 
                        (from n in RwEv.TsList
                         select n.High).Max();
                    double dblDenominator =
                        dblMax - dblMin;
                    double dblPrcX =
                        Math.Max(
                            0,
                        Math.Min(100,
                        dblDenominator == 0 ? 0 :
                        100.0 *(ev.AdjClose - dblMin)/
                        dblDenominator));

                    RwFast.Update(
                        dateTime, 
                        dblPrcX);

                    if (!RwFast.IsReady())
                    {
                        return;
                    }
                    RwSlow.Update(
                        dateTime,
                        RwFast.Mean);

                    m_dblRwSlow = RwSlow.Mean;
                    m_dblRwFast = RwFast.Mean;

                    if (IsGoingUp != m_blnPrevIsGoingUp)
                    {
                         TimeOfCrossing = dateTime;
                         m_blnLastCrossIsGoingUp = IsGoingUp;
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

        public RollingWindowStochasticCross Clone()
        {
            var clone = 
                (RollingWindowStochasticCross)MemberwiseClone();
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
