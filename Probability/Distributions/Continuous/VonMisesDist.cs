#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class VonMises : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly VonMises m_ownInstance = new VonMises(
            1, new RngWrapper(INT_RND_SEED));

        private double m_dblK;
        // cached vars for method NextDouble(a) (for performance only)
        private double m_dblKSet = -1.0;
        private double m_dblR;
        private double m_dblRho;
        private double m_dblTau;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 21;

        #endregion

        #region Constructors

        public VonMises(
            double dblK,
            RngWrapper rng) : base(rng)
        {
            if (dblK <= 0.0)
            {
                throw new ArgumentException();
            }
            SetState(
                dblK);
        }

        #endregion

        #region Parameters

        public double K
        {
            get { return m_dblK; }
            set
            {
                m_dblK = value;
                SetState(m_dblK);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblK)
        {
            if (dblK <= 0.0)
            {
                throw new ArgumentException();
            }

            m_dblK = dblK;
        }

        #endregion

        #region Public

        /**
        * Returns a random number from the distribution; bypasses the internal state.
        * @throws ArgumentException if <tt>k &lt;= 0.0</tt>.
        */

        public override double NextDouble()
        {
            /******************************************************************
             *                                                                *
             *         Von Mises Distribution - Acceptance Rejection          *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :  - mwc samples a random number from the von Mises   *
             *               distribution ( -Pi <= x <= Pi) with parameter    *
             *               k > 0  via  rejection from the wrapped Cauchy    *
             *               distibution.                                     *
             * REFERENCE:  - D.J. Best, N.I. Fisher (1979): Efficient         *
             *               simulation of the von Mises distribution,        *
             *               Appl. Statist. 28, 152-157.                      *
             * SUBPROGRAM: - drand(seed) ... (0,1)-Uniform generator with     *
             *               unsigned long integer *seed.                     *
             *                                                                *
             * Implemented by F. Niederl, August 1992                         *
             ******************************************************************/
            double u, v, w, c, z;
            if (K <= 0.0)
            {
                throw new ArgumentException();
            }

            if (m_dblKSet != K)
            {
                // SET-UP
                m_dblTau = 1.0 + Math.Sqrt(1.0 + 4.0*K*K);
                m_dblRho = (m_dblTau - Math.Sqrt(2.0*m_dblTau))/(2.0*K);
                m_dblR = (1.0 + m_dblRho*m_dblRho)/(2.0*m_dblRho);
                m_dblKSet = K;
            }

            // GENERATOR
            do
            {
                u = m_rng.NextDouble(); // U(0/1)
                v = m_rng.NextDouble(); // U(0/1)
                z = Math.Cos(Math.PI*u);
                w = (1.0 + m_dblR*z)/(m_dblR + z);
                c = K*(m_dblR - w);
            }
            while ((c*(2.0 - c) < v) && (Math.Log(c/v) + 1.0 < c)); // Acceptance/Rejection

            return (m_rng.NextDouble() > 0.5) ? Math.Acos(w) : -Math.Acos(w); // Random sign //
            // 0 <= x <= Pi : -Pi <= x <= 0 //
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "VonMisesDist(" +
                   K + ")";
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
            double dblK,
            double dblX)
        {
            m_ownInstance.SetState(
                dblK);

            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblK,
            double dblX)
        {
            m_ownInstance.SetState(
                dblK);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblK,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblK);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblK)
        {
            m_ownInstance.SetState(
                dblK);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblK,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblK);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblK,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblK);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
