#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class LambdaDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly LambdaDist m_ownInstance = new LambdaDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblL3;
        private double m_dblL4;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 12;

        #endregion

        #region Constructors

        public LambdaDist(
            double dblL3,
            double dblL4,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblL3,
                dblL4);
        }

        #endregion

        #region Parameters

        public double L3
        {
            get { return m_dblL3; }
            set
            {
                m_dblL3 = value;
                SetState(
                    m_dblL3,
                    m_dblL4);
            }
        }

        public double L4
        {
            get { return m_dblL4; }
            set
            {
                m_dblL4 = value;
                SetState(
                    m_dblL3,
                    m_dblL4);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblL3,
            double dblL4)
        {
            m_dblL3 = dblL3;
            m_dblL4 = dblL4;
        }

        #endregion

        #region Public

        /**
         * Returns a lambda distributed random number with parameters l3 and l4.
         * <p>
         * <b>Implementation:</b> Inversion method.
         * This is a port of <tt>lamin.c</tt> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         * C-RAND's implementation, in turn, is based upon
         * <p>
         * J.S. Ramberg, B:W. Schmeiser (1974): An approximate method for generating asymmetric variables, Communications ACM 17, 78-82.
         * <p>
         */

        public override double NextDouble()
        {
            double l_sign;
            if ((L3 < 0) || (L4 < 0))
            {
                l_sign = -1.0; // sign(l)
            }
            else
            {
                l_sign = 1.0;
            }

            double u = m_rng.NextDouble(); // U(0/1)
            double x = l_sign*(Math.Exp(Math.Log(u)*L3) -
                               Math.Exp(Math.Log(1.0 - u)*L4));
            return x;
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
            double dblAlpha,
            double dblBeta,
            double dblX)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblAlpha,
            double dblBeta,
            double dblX)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblAlpha,
            double dblBeta,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblAlpha,
            double dblBeta)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblAlpha,
            double dblBeta,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblAlpha,
            double dblBeta,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
