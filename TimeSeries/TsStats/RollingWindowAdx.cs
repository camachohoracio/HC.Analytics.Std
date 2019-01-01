using System;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowAdx : ITechnicalIndicator
    {
        public bool IsReady
        {
            get { return m_atr.IsReady; }
        }

        public double Adx { get; private set; }

        private DateTime m_prevTime;
        private double m_dblPrevLow;
        private double m_dblPrevHigh;
        private ExMAvgFunction m_smaPlus;
        private ExMAvgFunction m_smaMinus;
        private RollingWindowAtr m_atr;
        private ExMAvgFunction m_smaAdx;
        private readonly int m_intPeriods;

        public RollingWindowAdx(
            int intPeriods)
        {
            m_intPeriods = intPeriods;
            m_dblPrevLow = double.NaN;
            m_dblPrevHigh = double.NaN;
            m_smaPlus = new ExMAvgFunction(
                1.0 / intPeriods,
                false);
            m_smaMinus = new ExMAvgFunction(
                1.0 / intPeriods,
                false);
            m_atr = new RollingWindowAtr(
                intPeriods);
            m_smaAdx = new ExMAvgFunction(
                1.0 / intPeriods,
                false);
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

                m_atr.Update(
                    time,
                    dblHigh,
                    dblLow,
                    dblClose);

                if(!double.IsNaN(m_dblPrevHigh))
                {

                    //UpMove = today's high − yesterday's high
                    //DownMove = yesterday's low − today's low
                    //if UpMove > DownMove and UpMove > 0, then +DM = UpMove, else +DM = 0
                    //if DownMove > UpMove and DownMove > 0, then −DM = DownMove, else −DM = 0

                    double dblUpMove = dblHigh - m_dblPrevHigh;
                    double dblDownMove = m_dblPrevLow - dblLow;
                    double dblPlusDm = 0;
                    double dblMinusDm = 0;
                    if (dblUpMove > dblDownMove &&
                        dblUpMove > 0)
                    {
                        dblPlusDm = dblUpMove;
                    }
                    if (dblDownMove > dblUpMove &&
                        dblDownMove > 0)
                    {
                        dblMinusDm = dblDownMove;
                    }

                    m_smaPlus.Update(
                        new TsRow2D(
                            time,
                            dblPlusDm));
                    m_smaMinus.Update(
                        new TsRow2D(
                            time,
                            dblMinusDm));

                    if (m_atr.IsReady)
                    {
                        //+DI = 100 times the smoothed moving average of (+DM) divided by average true range
                        //−DI = 100 times the smoothed moving average of (−DM) divided by average true range
                        double dblAtr = m_atr.Atr;
                        dblAtr = dblAtr == 0 ? 1e-6 : dblAtr;
                        double dblDiPlus =
                            100.0*m_smaPlus.LastMovingAverage/dblAtr;
                        double dblDiMinus =
                            100.0*m_smaMinus.LastMovingAverage/dblAtr;
                        
                        //A.D.X. = 100 times the smoothed moving average of the absolute value of (+DI − −DI) divided by (+DI + −DI)
                        double dblNominator = Math.Abs(dblDiPlus - dblDiMinus);
                        double dblDenominator = (dblDiPlus + dblDiMinus);

                        dblDenominator = dblDenominator == 0 ? 1e-6 : dblDenominator;
                        double dblAdx =
                            dblNominator /
                            dblDenominator;
                        
                        if(double.IsNaN(dblAdx))
                        {
                            throw new HCException("NaN ADX ["+
                                dblNominator + "]/[" +
                                dblDenominator + "]");
                        }

                        m_smaAdx.Update(
                            new TsRow2D(
                                time,
                                dblAdx));

                        Adx = m_smaAdx.LastMovingAverage;
                    }
                }

                m_dblPrevHigh = dblHigh;
                m_dblPrevLow = dblLow;
                m_prevTime = time;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public double Indicator
        {
            get { return Adx; }
            set
            {
                throw new HCException("Not implemented");
            }
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

        public void Dispose()
        {
            if(m_smaPlus != null)
            {
                m_smaPlus.Dispose();
                m_smaPlus = null;
            }
            if(m_smaMinus != null)
            {
                m_smaMinus.Dispose();
                m_smaMinus = null;
            }
            if(m_atr != null)
            {
                m_atr.Dispose();
                m_atr = null;
            }
            if(m_smaAdx != null)
            {
                m_smaAdx.Dispose();
                m_smaAdx = null;
            }
        }
    }
}
