#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class WeibullDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly WeibullDist m_ownInstance = new WeibullDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblK;
        private double m_dblLambda;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 22;

        #endregion

        #region Constructors

        public WeibullDist(
            double dblLambda,
            double dblK,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblLambda,
                dblK);
        }

        #endregion

        #region Parameters

        public double Lambda
        {
            get { return m_dblLambda; }
            set
            {
                m_dblLambda = value;
                SetState(
                    m_dblLambda,
                    m_dblK);
            }
        }

        public double K
        {
            get { return m_dblK; }
            set
            {
                m_dblK = value;
                SetState(
                    m_dblLambda,
                    m_dblK);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblLambda,
            double dblK)
        {
            m_dblLambda = dblLambda;
            m_dblK = dblK;
        }

        #endregion

        #region Public

        /**
         *  generate single weibull(eta,betta) random variable; Creation date: (1/26/00
         *  1:39:51 PM)
         *
         *@param  lambda   double - scale parameter
         *@param  k  double - shape parameter
         *@return       double - random variables
         *@author:      <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public override double NextDouble()
        {
            return Lambda*Math.Pow(-Math.Log(1 - m_rng.NextDouble()), 1/K);
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
