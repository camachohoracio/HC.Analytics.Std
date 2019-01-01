#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class UnivNormalDistTrncStd : UnivNormalDistStd
    {
        #region Constants

        private const int INT_RND_SEED = 28;

        #endregion

        #region Memebers

        private double m_dblHighLimit;
        private double m_dblLowLimit;
        private UnivNormalDistStd m_univNormalDistStd;

        #endregion

        #region Constructors

        public UnivNormalDistTrncStd(
            double dblLowLimit,
            double dblHighLimit,
            RngWrapper rng) : base(rng)
        {
            SetState(dblLowLimit,
                     dblHighLimit);
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblLowLimit,
            double dblHighLimit)
        {
            m_dblLowLimit = dblLowLimit;
            m_dblHighLimit = dblHighLimit;
            m_univNormalDistStd = new UnivNormalDistStd(m_rng);
        }

        #endregion

        #region Public

        /**
             * Pdf of truncated normal
             * @return pdf
             * @param x x
             * @param low lower limit of truncation
             * @param high upper limit of truncation
             */

        public override double Pdf(
            double dblX)
        {
            return Pdf(dblX)/
                   (Cdf(m_dblHighLimit) - Cdf(m_dblLowLimit));
        }

        /**
         *  Insert the method's description here. Creation date: (2/10/00 2:31:07 PM)
         *
         *@param  low   double
         *@param  high  double
         *@return       double
         */

        public override double NextDouble()
        {
            if (Double.IsNaN(m_dblLowLimit) || Double.IsNaN(m_dblHighLimit))
            {
                throw new ArgumentException("low or high is NaN");
            }
            if (m_dblLowLimit >= m_dblHighLimit)
            {
                throw new ArgumentException(" low limit has be smaller than higher");
            }
            if (Double.NegativeInfinity == m_dblLowLimit)
            {
                return -NextDouble(-m_dblHighLimit);
            }
            if (Double.PositiveInfinity == m_dblHighLimit)
            {
                return NextDouble(m_dblLowLimit);
            }
            if (m_dblHighLimit - m_dblLowLimit > 0.2 &&
                (!((m_dblHighLimit > 1.5 && m_dblLowLimit > 1.5) || (m_dblHighLimit < 1.5 && m_dblLowLimit < 1.5))))
            {
                double aa = 0;
                if (m_dblHighLimit > 0 && m_dblLowLimit < 0)
                {
                    aa = Pdf(0);
                }
                else
                {
                    aa = Pdf(m_dblLowLimit);
                    aa = Math.Max(aa, Pdf(m_dblHighLimit));
                }
                do
                {
                    double y = m_rng.NextDouble(m_dblLowLimit, m_dblHighLimit);
                    double u = m_rng.NextDouble();
                    if (u*aa <= Pdf(y))
                    {
                        return y;
                    }
                }
                while (true);
            }
            return m_univNormalDistStd.CdfInv(
                m_rng.NextDouble(
                    Cdf(m_dblLowLimit),
                    Cdf(m_dblHighLimit)));
        }

        #endregion

        #region Private

        /**
         *  Insert the method's description here. Creation date: (2/10/00 4:30:30 PM)
         *
         *@param  low  double
         *@return      double
         */

        private double NextDouble(double dblLow)
        {
            if (Double.IsNaN(dblLow))
            {
                throw new ArgumentException("low is NaN");
            }
            double cut = .45;
            if (dblLow > cut)
            {
                double z = -Math.Log(
                                m_rng.NextDouble())/dblLow;
                while (m_rng.NextDouble() > Math.Exp(-.5*z*z))
                {
                    z = -Math.Log(
                             m_rng.NextDouble())/dblLow;
                }
                if (z < 0)
                {
                    throw new ArgumentException("z<0");
                }
                return z + dblLow;
            }
            double x = m_univNormalDistStd.NextDouble();
            while (x < dblLow)
            {
                x = m_univNormalDistStd.NextDouble();
            }
            return x;
        }

        #endregion
    }
}
