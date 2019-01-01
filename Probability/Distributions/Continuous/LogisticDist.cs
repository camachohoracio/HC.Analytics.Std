#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class LogisticDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static LogisticDist m_ownInstance = new LogisticDist(
            new RngWrapper(INT_RND_SEED));

        #endregion

        #region Constants

        private const int INT_RND_SEED = 15;

        #endregion

        #region Constructors

        public LogisticDist(RngWrapper rng) : base(rng)
        {
        }

        #endregion

        #region Public

        /**
         * Returns a random number from the standard Logistic distribution Log(0,1).
         * <p>
         * <b>Implementation:</b> Inversion method.
         * This is a port of <tt>login.c</tt> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         */

        public override double NextDouble()
        {
            double u = m_rng.NextDouble();
            return (-Math.Log(1.0/u - 1.0));
        }

        public override double Cdf(double dblX)
        {
            throw new NotImplementedException();
        }

        public override double CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        public override double Pdf(double dblX)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
