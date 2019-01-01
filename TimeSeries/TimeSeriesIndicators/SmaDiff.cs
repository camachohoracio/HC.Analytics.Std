#region

using System;
using HC.Analytics.TimeSeries.TsStats;

#endregion

namespace HC.Analytics.TimeSeries.TimeSeriesIndicators
{
    public class SmaDiff
    {
        #region Members

        private readonly IncrBasicRollingWindow m_fastEma;
        private readonly int m_intFastWindow;
        private readonly int m_intSlowWindow;
        private readonly IncrBasicRollingWindow m_priceToCrossFromLastEma;
        private readonly IncrBasicRollingWindow m_slowEma;
        private double m_dblLastDiff;
        private double m_dblLastLastDiff;
        private double m_dblLastPriceToCrossFromLast;
        private double m_dblLastValue;
        private double m_dblPriceToCross;
        private double m_dblPriceToCrossFromLast;

        #endregion

        #region Properties

        public bool IsPopulated
        {
            get { return m_fastEma.IsFull && m_slowEma.IsFull; }
        }

        #endregion

        #region Constructors

        public SmaDiff(int intFastWindow,
                       int intSlowWindow) :
                           this(intFastWindow,
                                intSlowWindow,
                                intFastWindow + intSlowWindow)
        {
        }

        public SmaDiff(int intFastWindow,
                       int intSlowWindow,
                       int intPriceToCrossWindow)
        {
            m_intFastWindow = intFastWindow;
            m_intSlowWindow = intSlowWindow;
            m_fastEma = new IncrBasicRollingWindow(intFastWindow);
            m_slowEma = new IncrBasicRollingWindow(intSlowWindow);
            m_priceToCrossFromLastEma = new IncrBasicRollingWindow(intPriceToCrossWindow);
        }

        #endregion

        #region Public

        public void Update(double dblValue)
        {
            m_dblLastValue = dblValue;
            m_fastEma.Update(dblValue);
            m_slowEma.Update(dblValue);
            m_dblLastLastDiff = m_dblLastDiff;
            m_dblLastDiff = GetDiff();
            FindPriceToCross();
        }

        public void UpdateEmpty()
        {
            m_dblLastLastDiff = m_dblLastDiff;
            m_dblLastPriceToCrossFromLast = m_dblPriceToCrossFromLast;
        }

        public double GetDiff()
        {
            return m_fastEma.MeanValue - m_slowEma.MeanValue;
        }

        public double GetDiffAsPcnt()
        {
            if (m_fastEma.NumOfValues == 0)
            {
                return 0;
            }

            return 100.0*(m_fastEma.MeanValue - m_slowEma.MeanValue)/m_fastEma.LastValue;
        }

        public int GetCrossCheckTradeSign(double dblThreshold)
        {
            var intCurrentSign = Sign(m_dblLastDiff, dblThreshold);
            if (intCurrentSign != 0 &&
                Sign(m_dblLastLastDiff, dblThreshold) != intCurrentSign)
            {
                return intCurrentSign;
            }
            return 0;
        }

        #endregion

        #region Private

        private void FindPriceToCross()
        {
            var dblFastEmaFirstPrice = m_fastEma.GetFirst();
            var dblSlowEmaFirstPrice = m_slowEma.GetFirst();
            m_dblLastPriceToCrossFromLast = m_dblPriceToCrossFromLast;

            if (Math.Abs(m_dblLastDiff) > 0)
            {
                var dblCrossPrice = ((m_intFastWindow*m_intSlowWindow)/
                                     (double) (m_intSlowWindow - m_intFastWindow))*
                                    ((-1.0*m_dblLastDiff) + ((dblFastEmaFirstPrice/m_intFastWindow) -
                                                             (dblSlowEmaFirstPrice/m_intSlowWindow)));
                m_dblPriceToCross = dblCrossPrice;
                m_dblPriceToCrossFromLast = m_dblPriceToCross - m_dblLastValue;
            }
            else
            {
                m_dblPriceToCross = -1;
                m_dblPriceToCrossFromLast = 0;
            }
            m_priceToCrossFromLastEma.Update(m_dblPriceToCrossFromLast);
        }

        private static int Sign(double dblProb, double dblThreshold)
        {
            if (dblProb > dblThreshold)
            {
                return 1;
            }
            if (dblProb < -dblThreshold)
            {
                return -1;
            }
            return 0;
        }

        #endregion

        public double GetBps()
        {
            var dblStdDev = GetLogicallySlower().StdDevValue;
            if (!double.IsInfinity(dblStdDev) && !double.IsNaN(dblStdDev) && dblStdDev > 0)
            {
                return GetDiff()/dblStdDev;
            }
            return 0;
        }

        private IncrBasicRollingWindow GetLogicallySlower()
        {
            if (m_fastEma.WindowSize > m_slowEma.WindowSize)
            {
                return m_fastEma;
            }
            return m_slowEma;
        }
    }
}
