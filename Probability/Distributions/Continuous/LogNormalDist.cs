#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class LogNormalDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly LogNormalDist m_ownInstance = new LogNormalDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblMean;
        private double m_dblStdDev;
        private UnivNormalDistStd m_univariateNormalDistributionStd;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 16;

        #endregion

        #region Constructors

        public LogNormalDist(
            double dblMean,
            double dblStdDev,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblMean,
                dblStdDev);
        }

        #endregion

        #region Parameters

        public double Mean
        {
            get { return m_dblMean; }
            set
            {
                m_dblMean = value;
                SetState(m_dblMean,
                         m_dblStdDev);
            }
        }

        public double StdDev
        {
            get { return m_dblStdDev; }
            set
            {
                m_dblStdDev = value;
                SetState(m_dblMean,
                         m_dblStdDev);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblMean,
            double dblStdDev)
        {
            m_dblMean = dblMean;
            m_dblStdDev = dblStdDev;
            m_univariateNormalDistributionStd =
                new UnivNormalDistStd(m_rng);
        }

        #endregion

        #region Public

        /**
         *  Insert the method's description here. Creation date: (1/26/00 11:45:20 AM)
         *
         *@param  mu     double
         *@param  sigma  double
         *@return        double
         *@author:       <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public override double NextDouble()
        {
            return Math.Exp((
                                m_univariateNormalDistributionStd.NextDouble() +
                                Mean)*StdDev);
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
