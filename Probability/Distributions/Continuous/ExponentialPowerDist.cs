#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class ExponentialPowerDist : AbstractUnivContDist
    {
        #region Memebers

        // cached vars for method NextDouble(tau) (for performance only)
        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly ExponentialPowerDist m_ownInstance = new ExponentialPowerDist(
            1, new RngWrapper(INT_RND_SEED));

        private double m_dblS;
        private double m_dblSm1;
        private double m_dblTau;
        private double m_dblTau_set = -1.0;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 9;

        #endregion

        #region Constructors

        public ExponentialPowerDist(
            double dblTau,
            RngWrapper rng)
            : base(rng)
        {
            SetState(dblTau);
        }

        #endregion

        #region Parameters

        public double Tau
        {
            get { return m_dblTau; }

            set
            {
                m_dblTau = value;
                SetState(m_dblTau);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblTau)
        {
            if (dblTau < 1.0)
            {
                throw new ArgumentException();
            }
            m_dblTau = dblTau;
        }

        #endregion

        #region Public

        /**
         * Returns a random number from the distribution; bypasses the internal state.
         * @throws ArgumentException if <tt>tau &lt; 1.0</tt>.
         */

        public override double NextDouble()
        {
            double u, u1, v, x, y;

            if (Tau != m_dblTau_set)
            {
                // SET-UP
                m_dblS = 1.0/Tau;
                m_dblSm1 = 1.0 - m_dblS;

                m_dblTau_set = Tau;
            }

            // GENERATOR
            do
            {
                u = m_rng.NextDouble(); // U(0/1)
                u = (2.0*u) - 1.0; // U(-1.0/1.0)
                u1 = Math.Abs(u); // u1=|u|
                v = m_rng.NextDouble(); // U(0/1)

                if (u1 <= m_dblSm1)
                {
                    // Uniform hat-function for x <= (1-1/tau)
                    x = u1;
                }
                else
                {
                    // Exponential hat-function for x > (1-1/tau)
                    y = Tau*(1.0 - u1); // U(0/1)
                    x = m_dblSm1 - m_dblS*Math.Log(y);
                    v = v*y;
                }
            } // Acceptance/Rejection
            while (Math.Log(v) > -Math.Exp(Math.Log(x)*Tau));

            // Random sign
            if (u < 0.0)
            {
                return x;
            }
            else
            {
                return -x;
            }
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "ExponentialPowerDist(" + Tau + ")";
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
            double dblTau,
            double dblX)
        {
            m_ownInstance.SetState(
                dblTau);
            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblTau,
            double dblX)
        {
            m_ownInstance.SetState(
                dblTau);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblTau,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblTau);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblTau)
        {
            m_ownInstance.SetState(
                dblTau);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblTau,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblTau);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblTau,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblTau);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
