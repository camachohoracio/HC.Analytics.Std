#region

using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    /// <summary>
    /// Multivariate continuous distribution.
    /// </summary>
    public abstract class AbstractMultContDist : IDist
    {
        #region Members

        protected int m_intVariableCount;
        protected RngWrapper m_rng;

        #endregion

        #region AbstractMethods

        public abstract double Cdf(Vector xVector);
        public abstract double Cdf(double[] dblXArr);

        public abstract double Cdf(
            Vector lowLimitVector,
            Vector highLimitVector);

        public abstract double Cdf(
            double[] dblLowLimitArr,
            double[] dblHighLimitArr);

        public abstract double Pdf(Vector xVector);
        public abstract double Pdf(double[] dblXArr);

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public abstract double[] NextDouble();

        public abstract Vector NextDoubleVector();

        #endregion

        #region Constructors

        public AbstractMultContDist(RngWrapper rng)
        {
            m_rng = rng;
        }

        #endregion
    }
}
