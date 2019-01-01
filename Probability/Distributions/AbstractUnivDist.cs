#region

using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions
{
    public abstract class AbstractUnivDist : IDist
    {
        #region Members

        /// <summary>
        /// Random number generator
        /// </summary>
        protected RngWrapper m_rng;

        #endregion

        #region Constructors

        public AbstractUnivDist(RngWrapper rng)
        {
            m_rng = rng;
        }

        #endregion
    }
}
