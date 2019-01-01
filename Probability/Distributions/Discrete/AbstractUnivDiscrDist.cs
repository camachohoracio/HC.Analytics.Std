#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Discrete
{
    public abstract class AbstractUnivDiscrDist : AbstractUnivDist
    {
        #region Constructors

        public AbstractUnivDiscrDist(RngWrapper rng) : base(rng)
        {
        }

        #endregion

        #region Public

        public int[] NextIntArr(int n)
        {
            int[] x = new int[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = NextInt();
            }
            return x;
        }

        #endregion

        #region AbstractMethods

        public abstract int NextInt();

        /// <summary>
        /// Probability density function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public abstract double Pdf(int intX);

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        /// <param name="dblX"></param>
        /// <returns></returns>
        public abstract double Cdf(int intX);

        /// <summary>
        /// Inverse Cdf
        /// </summary>
        /// <param name="dblProbability"></param>
        /// <returns></returns>
        public abstract int CdfInv(double dblProbability);

        #endregion

        protected EmpiricalWalkerDist Clone()
        {
            throw new NotImplementedException();
        }
    }
}
