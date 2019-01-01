using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowDssBr : ITechnicalIndicator, ICrossingIndicator
    {
        private object m_lockObject = new object();
        public RollingWindowTsEvent<DailyTsEvent> RwEv1 { get; private set; }
        public RollingWindowTsEvent<TsRow2D> RwEv2 { get; private set; }
        public ExMAvgFunction RwFast { get; private set; }
        public RollingWindowMean RwSlow { get; private set; }
        private double m_dblRwSlow;
        private double m_dblRwFast;

        #region Properties

        public double Indicator => RwSlow.Mean;

        bool ITechnicalIndicator.IsReady => IsReady();

        public bool IsReady()
        {
            return
                RwSlow.IsReady();
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
        private readonly ExMAvgFunction m_firstEma;
        private bool m_blnLastCrossingGoingUp;

        public bool GetIsGoingUp(
            double dblStochasticSlow,
            double dblStochasticFast)
        {
            return dblStochasticFast > dblStochasticSlow;
        }

        #endregion
        public RollingWindowDssBr(
            int intEmaStep1,
            int intEmaFast,
            int intMaSlow,
            int intDaysSamples)
        {
            RwEv1 =
                new RollingWindowTsEvent<DailyTsEvent>(
                    intDaysSamples);
            RwEv2 =
                new RollingWindowTsEvent<TsRow2D>(
                    intDaysSamples);
            RwSlow = new RollingWindowMean(intMaSlow);
            RwFast = new ExMAvgFunction(intEmaFast);
            m_firstEma = new ExMAvgFunction(intEmaStep1);
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

                    DailyTsEvent ev =
                        new DailyTsEvent
                        {
                            Close = dblClose,
                            High = dblHigh,
                            Low = dblLow,
                            AdjClose = dblClose,
                            Time = dateTime
                        };

                    RwEv1.Update(
                        ev);

                    if (!RwEv1.IsReady)
                    {
                        return;
                    }

                    double dblMin =
                        (from n in RwEv1.TsList
                         select n.Low).Min();
                    double dblMax =
                        (from n in RwEv1.TsList
                         select n.High).Max();
                    double dblDenominator =
                        dblMax - dblMin;
                    double dblPrcX =
                        Math.Max(
                            0,
                        Math.Min(100,
                        dblDenominator == 0 ? 0 :
                        100.0 * (ev.AdjClose - dblMin) /
                        dblDenominator));

                    m_firstEma.Update(
                        dateTime,
                        dblPrcX);

                    //if (!RwFast.IsReady)
                    //{
                    //    return;
                    //}

                    RwEv2.Update(
                        new TsRow2D
                        {
                            Time = dateTime,
                            Fx = m_firstEma.LastMovingAverage
                        });

                    if (!RwEv2.IsReady)
                    {
                        return;
                    }

                    List<double> windowData = 
                        (from n in RwEv2.TsList
                        select n.Fx).ToList();
                    dblMin =
                        windowData.Min();
                    dblMax =
                        windowData.Max();
                    dblDenominator =
                        dblMax - dblMin;
                    dblPrcX =
                        Math.Max(
                            0,
                        Math.Min(100,
                        dblDenominator == 0 ? 0 :
                        100.0 * (m_firstEma.LastMovingAverage - dblMin) /
                        dblDenominator));

                    RwFast.Update(
                        dateTime,
                        dblPrcX);

                    RwSlow.Update(
                        dateTime,
                        dblPrcX);

                    if (!RwSlow.IsReady())
                    {
                        return;
                    }
                    m_dblRwFast = RwFast.LastMovingAverage;
                    m_dblRwSlow = RwSlow.Mean;

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
