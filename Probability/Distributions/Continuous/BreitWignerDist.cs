#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class BreitWignerDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly BreitWignerDist m_ownInstance = new BreitWignerDist(
            1, 1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblCut;
        private double m_dblGamma;
        private double m_dblMean;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 2;

        #endregion

        #region Constructors

        public BreitWignerDist(
            double dblMean,
            double dblGamma,
            double dblCut,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblMean,
                dblGamma,
                dblCut);
        }

        #endregion

        #region Parameters

        public double Mean
        {
            get { return m_dblMean; }
            set
            {
                m_dblMean = value;
                SetState(
                    m_dblMean,
                    m_dblGamma,
                    m_dblCut);
            }
        }


        public double Gamma
        {
            get { return m_dblGamma; }
            set
            {
                m_dblGamma = value;
                SetState(
                    m_dblMean,
                    m_dblGamma,
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
                    m_dblMean,
                    m_dblGamma,
                    m_dblCut);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblMean,
            double dblGamma,
            double dblCut)
        {
            m_dblMean = dblMean;
            m_dblGamma = dblGamma;
            m_dblCut = dblCut;
        }

        #endregion

        #region Public

        /**
         * Returns a random number from the distribution; bypasses the internal state.
         * @param cut </tt>cut==Double.NEGATIVE_INFINITY</tt> indicates "don't cut".
         */

        public override double NextDouble()
        {
            double val, rval, displ;
            if (Gamma == 0.0)
            {
                return Mean;
            }
            if (Cut == Double.NegativeInfinity)
            {
                // don't cut
                rval = 2.0*m_rng.NextDouble() - 1.0;
                displ = 0.5*Gamma*Math.Tan(rval*(Math.PI/2.0));
                return Mean + displ;
            }
            else
            {
                val = Math.Atan(2.0*Cut/Gamma);
                rval = 2.0*m_rng.NextDouble() - 1.0;
                displ = 0.5*Gamma*Math.Tan(rval*val);

                return Mean + displ;
            }
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "BreitWignerDist(" +
                   Mean + "," +
                   Gamma + "," +
                   Cut + ")";
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
            double dblMean,
            double dblGamma,
            double dblCut,
            double dblX)
        {
            m_ownInstance.SetState(
                dblMean,
                dblGamma,
                dblCut);

            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblMean,
            double dblGamma,
            double dblCut,
            double dblX)
        {
            m_ownInstance.SetState(
                dblMean,
                dblGamma,
                dblCut);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblMean,
            double dblGamma,
            double dblCut,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblMean,
                dblGamma,
                dblCut);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblMean,
            double dblGamma,
            double dblCut)
        {
            m_ownInstance.SetState(
                dblMean,
                dblGamma,
                dblCut);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblMean,
            double dblGamma,
            double dblCut,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblMean,
                dblGamma,
                dblCut);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblMean,
            double dblGamma,
            double dblCut,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblMean,
                dblGamma,
                dblCut);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
