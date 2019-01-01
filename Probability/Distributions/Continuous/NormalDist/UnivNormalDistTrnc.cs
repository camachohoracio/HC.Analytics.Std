#region

using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class UnivNormalDistTrnc : UnivNormalDistTrncStd
    {
        #region Constants

        private const int INT_RND_SEED = 27;

        #endregion

        #region Memebers

        private double m_dblHighLimit;
        private double m_dblLowLimit;
        private double m_dblMean;
        private double m_dblStdDev;
        private double m_dblVar;

        #endregion

        #region Properties

        public double Var
        {
            get { return m_dblVar; }
        }

        #endregion

        #region Constructors

        public UnivNormalDistTrnc(
            double dblMean,
            double dblStdDev,
            double dblLowLimit,
            double dblHighLimit,
            RngWrapper rng) : base(
                dblLowLimit,
                dblHighLimit,
                rng)
        {
            SetState(
                dblMean,
                dblStdDev,
                dblLowLimit,
                dblHighLimit);
        }

        #endregion

        #region Parameters

        public double Mean
        {
            get { return m_dblMean; }
            set
            {
                m_dblMean = value;
                SetState(
                    m_dblMean,
                    m_dblStdDev,
                    m_dblLowLimit,
                    m_dblHighLimit);
            }
        }

        public double StdDev
        {
            get { return m_dblStdDev; }
            set
            {
                m_dblStdDev = value;
                SetState(
                    m_dblMean,
                    m_dblStdDev,
                    m_dblLowLimit,
                    m_dblHighLimit);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblMean,
            double dblStdDev,
            double dblLowLimit,
            double dblHighLimit)
        {
            m_dblMean = dblMean;
            m_dblStdDev = dblStdDev;
            m_dblVar = dblStdDev*dblStdDev;
            m_dblLowLimit = dblLowLimit;
            m_dblHighLimit = dblHighLimit;
        }

        #endregion

        #region Public

        /**
             * Pdf of truncated normal
             * @return pdf
             * @param x x
             * @param low lower limit of truncation
             * @param high upper limit of truncation
             * @param mu mean
             * @param sd standart deviation
             */

        public override double Pdf(
            double dblX)
        {
            m_dblLowLimit -= Mean;
            m_dblHighLimit -= Mean;
            m_dblLowLimit /= StdDev;
            m_dblHighLimit /= StdDev;
            return Pdf(dblX)/
                   (Cdf(m_dblHighLimit) - Cdf(m_dblLowLimit));
        }

        #endregion
    }
}
