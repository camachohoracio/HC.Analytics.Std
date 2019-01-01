#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class LaplaceDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly LaplaceDist m_ownInstance = new LaplaceDist(
            new RngWrapper(INT_RND_SEED));

        #endregion

        #region Constants

        private const int INT_RND_SEED = 13;

        #endregion

        #region Constructors

        public LaplaceDist(RngWrapper rng) : base(rng)
        {
        }

        #endregion

        #region Public

        /**
         * Returns a Laplace (Double Exponential) distributed random number from the standard Laplace distribution L(0,1).
         * <p>
         * <b>Implementation:</b> Inversion method.
         * This is a port of <tt>lapin.c</tt> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         * <p>
         * @returns a number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).
         */

        public override double NextDouble()
        {
            double u = m_rng.NextDouble();
            u = u + u - 1.0;
            if (u > 0)
            {
                return -Math.Log(1.0 - u);
            }
            else
            {
                return Math.Log(1.0 + u);
            }
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

        #region StaticMethods

        public static double PdfStatic(
            double dblX)
        {
            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblX)
        {
            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblProbability)
        {
            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic()
        {
            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            int intSampleSize)
        {
            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            int intSampleSize)
        {
            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
