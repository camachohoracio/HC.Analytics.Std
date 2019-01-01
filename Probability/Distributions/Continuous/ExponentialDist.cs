#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class ExponentialDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly ExponentialDist m_ownInstance = new ExponentialDist(
            1, new RngWrapper(INT_RND_SEED));

        private double m_dblLambda;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 8;

        #endregion

        #region Constructors

        public ExponentialDist(
            double dblLambda,
            RngWrapper rng)
            : base(rng)
        {
            SetState(dblLambda);
        }

        #endregion

        #region Parameters

        public double Lambda
        {
            get { return m_dblLambda; }
            set
            {
                m_dblLambda = value;
                SetState(m_dblLambda);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblLambda)
        {
            m_dblLambda = dblLambda;
        }

        #endregion

        #region Public

        /**
        * Returns the cumulative distribution function.
        */

        public override double Cdf(double x)
        {
            if (x <= 0.0)
            {
                return 0.0;
            }
            return 1.0 - Math.Exp(-x*Lambda);
        }

        /**
         * Returns a random number from the distribution; bypasses the internal state.
         */

        public override double NextDouble()
        {
            return -Math.Log(m_rng.NextDouble())/Lambda;
        }

        /**
         * Returns the probability distribution function.
         */

        public override double Pdf(double dblX)
        {
            if (dblX < 0.0)
            {
                return 0.0;
            }
            return Lambda*Math.Exp(-dblX*Lambda);
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "ExponentialDist(" + Lambda + ")";
        }

        public override double CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblLambda,
            double dblX)
        {
            m_ownInstance.SetState(
                dblLambda);

            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblLambda,
            double dblX)
        {
            m_ownInstance.SetState(
                dblLambda);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblLambda,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblLambda);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblLambda)
        {
            m_ownInstance.SetState(
                dblLambda);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblLambda,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblLambda);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblLambda,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblLambda);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
