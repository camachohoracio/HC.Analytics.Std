#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class PowLawDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly PowLawDist m_ownInstance = new PowLawDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblAlpha;
        private double m_dblCut;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 17;

        #endregion

        #region Constructors

        public PowLawDist(
            double dblAlpha,
            double dblCut,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblAlpha,
                dblCut);
        }

        #endregion

        #region Parameters

        public double Alpha
        {
            get { return m_dblAlpha; }
            set
            {
                m_dblAlpha = value;
                SetState(
                    m_dblAlpha,
                    m_dblCut);
            }
        }

        public double Cut
        {
            get { return m_dblCut; }
            set
            {
                m_dblCut = value;
                SetState(
                    m_dblAlpha,
                    m_dblCut);
            }
        }

        #endregion

        #region Initializaiton

        private void SetState(
            double dblAlpha,
            double dblCut)
        {
            m_dblAlpha = dblAlpha;
            m_dblCut = dblCut;
        }

        #endregion

        #region Public

        /**
        *
        * generate a power-law distribution with exponent <CODE>alpha</CODE>
        * and lower cutoff
        * <CODE>cut</CODE>
        * <CENTER>
        * </CENTER>
        *
        *@param alpha the exponent
        *@param cut the lower cutoff
        *
        */

        public override double NextDouble()
        {
            return Cut*Math.Pow(m_rng.NextDouble(), 1.0/(Alpha + 1.0));
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
