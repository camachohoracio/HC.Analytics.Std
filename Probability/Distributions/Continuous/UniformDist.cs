#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class UniformDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly UniformDist m_ownInstance = new UniformDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblA;
        private double m_dblB;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 20;

        #endregion

        #region Constructors

        public UniformDist(
            double dblA,
            double dblB,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblA,
                dblB);
        }

        #endregion

        #region Paramaters

        public double A
        {
            get { return m_dblA; }
            set
            {
                m_dblA = value;
                SetState(
                    m_dblA,
                    m_dblB);
            }
        }

        public double B
        {
            get { return m_dblB; }
            set
            {
                m_dblB = value;
                SetState(
                    m_dblA,
                    m_dblB);
            }
        }

        #endregion

        #region Initializaiton

        private void SetState(
            double dblA,
            double dblB)
        {
            if (dblB < dblA)
            {
                SetState(dblB, dblA);
                return;
            }
            m_dblA = dblA;
            m_dblB = dblB;
        }

        #endregion

        #region Public

        /**
        * Returns the cumulative distribution function (assuming a continous uniform distribution).
        */

        public override double Cdf(double x)
        {
            if (x <= A)
            {
                return 0.0;
            }
            if (x >= B)
            {
                return 1.0;
            }
            return (x - A)/(B - A);
        }

        /**
         * Returns a uniformly distributed random number in the open interval <tt>(Min,Max)</tt> (excluding <tt>Min</tt> and <tt>Max</tt>).
         */

        public override double NextDouble()
        {
            return A + (B - A)*m_rng.NextDouble();
        }

        /**
         * Returns the probability distribution function (assuming a continous uniform distribution).
         */

        public override double Pdf(double dblX)
        {
            if (dblX <= A || dblX >= B)
            {
                return 0.0;
            }
            return 1.0/(B - A);
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "UniformDist(" + A + "," + B + ")";
        }

        public override double CdfInv(double dblProbability)
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
