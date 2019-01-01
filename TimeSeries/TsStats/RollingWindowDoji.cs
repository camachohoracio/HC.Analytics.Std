using System;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowDoji : ITechnicalIndicator, ICrossingIndicator
    {
        private object m_lockObject = new object();
        public RollingWindowStdDev RwOpenClose { get; private set; }
        public RollingWindowStdDev RwMaxMin { get; private set; }

        public double Indicator => 0;

        bool ITechnicalIndicator.IsReady => IsReady();

        public bool IsReady()
        {
            return
                RwOpenClose.IsReady();
        }

        public DateTime LastUpdateTime { get; set; }
        public bool IsDoji { get; private set; }

        public DateTime TimeOfCrossing
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private double m_dblLastOpen;

        public bool GetIsGoingUp(
            double dblStochasticSlow,
            double dblStochasticFast)
        {
            return dblStochasticFast > dblStochasticSlow;
        }

        public RollingWindowDoji(
            int intWindow)
        {
            RwOpenClose = new RollingWindowStdDev(intWindow);
            RwMaxMin = new RollingWindowStdDev(intWindow);
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
                    IsDoji = false;
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

                    if (m_dblLastOpen > 0)
                    {
                        double dblOpenCloseRatio =
                            Math.Abs(
                            (m_dblLastOpen - dblClose) / dblClose);
                        double dblHighLowRatio =
                            (dblHigh - dblLow) / dblClose;

                        if (RwMaxMin.IsReady())
                        {
                            if(dblOpenCloseRatio < (RwMaxMin.Mean -
                                2.5 * RwMaxMin.StdDev) &&
                                    dblHighLowRatio >=
                                    (RwMaxMin.Mean -
                                2.0 * RwMaxMin.StdDev))
                            {
                                IsDoji = true;
                            }
                        }

                        RwOpenClose.Update(
                            dateTime,
                            dblOpenCloseRatio);

                        RwMaxMin.Update(
                            dateTime,
                            dblHighLowRatio);

                    }
                    m_dblLastOpen = dblClose;
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

        public bool GetLastCrossIsGoingUp()
        {
            throw new NotImplementedException();
        }
    }
}
