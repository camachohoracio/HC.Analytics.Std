using System;
using System.Linq;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Obv = On balance volume
    /// </summary>
    public class RollingWindowPolarizedFractalEfficiency : ITechnicalIndicator
    {
        private object m_lockObject = new object();
        public RollingWindowTsEvent<DailyTsEvent> RwEv { get; private set; }
        public ExMAvgFunction Ema { get; private set; }

        #region Properties

        public double Indicator => Ema.LastMovingAverage;

        bool ITechnicalIndicator.IsReady => IsReady();

        public bool IsReady()
        {
            return
                RwEv.IsReady;
        }

        public DateTime LastUpdateTime { get; set; }
        private readonly double m_dblNSquare;
        private double m_dblPrevClose;
        private double m_dblSumDenom;
        private readonly RollingWindowTsEvent<TsRow2D> m_denomsRw;

        #endregion
        public RollingWindowPolarizedFractalEfficiency(
            int intEma,
            int intDaysSamples)
        {
            RwEv =
                new RollingWindowTsEvent<DailyTsEvent>(
                    intDaysSamples);
            m_denomsRw =
                new RollingWindowTsEvent<TsRow2D>(
                    intDaysSamples);

            Ema = new ExMAvgFunction(intEma);
            m_dblNSquare =
                Math.Pow(intDaysSamples, 2);
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


                    RwEv.Update(
                        ev);

                    if (!RwEv.IsReady)
                    {
                        return;
                    }

                    if (m_dblPrevClose > 0)
                    {
                        double dblFirstClose =
                            RwEv.TsList.First().AdjClose;

                        double dblDenom =
                            Math.Sqrt(
                                Math.Pow(dblClose - m_dblPrevClose, 2)
                                + 1);
                        m_denomsRw.Update(new TsRow2D(
                            dateTime,
                            dblDenom));
                        m_dblSumDenom += dblDenom;

                        if (m_denomsRw.IsReady)
                        {
                            double dblFirstVal =
                                m_denomsRw.TsList.First().Fx;
                            m_dblSumDenom -= dblFirstVal;

                            int intSing = dblFirstClose >= dblClose
                                ? 1
                                : -1;
                            double dblNum =
                                Math.Sqrt(
                                    Math.Pow(dblClose - dblFirstClose, 2)
                                    + m_dblNSquare);
                            double dblVal =
                                dblNum/m_dblSumDenom;

                            Ema.Update(
                                dateTime,
                                intSing*dblVal);
                        }
                    }
                    m_dblPrevClose = dblClose;
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
