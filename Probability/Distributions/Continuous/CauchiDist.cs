#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class CauchiDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly CauchiDist m_ownInstance = new CauchiDist(
            1, new RngWrapper(INT_RND_SEED));

        private double m_dblMean;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 5;

        #endregion

        #region Constructors

        public CauchiDist(
            double dblMean,
            RngWrapper rng)
            : base(rng)
        {
            SetState(dblMean);
        }

        #endregion

        #region Parameters

        public double Mean
        {
            get { return m_dblMean; }
            set
            {
                m_dblMean = value;
                SetState(m_dblMean);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblMean)
        {
            m_dblMean = dblMean;
        }

        #endregion

        #region Public

        public override double Pdf(double dblX)
        {
            return 1.0/(Mean*Math.PI*(1.0 + Math.Pow(dblX/Mean, 2.0)));
        }

        /**
         * Returns a cauchy distributed random number from the standard Cauchy distribution C(0,1).
         * <A HREF="http://www.taglink.com.dataStructures.mixtureGaussian.cern.ch/RD11/rkb/AN16pp/node25.html#SECTION000250000000000000000"> math definition</A>
         * and <A HREF="http://www.statsoft.com/textbook/glosc.html#Cauchy Distribution"> animated definition</A>.
         * <p>
         * <tt>p(x) = 1/ (mean*pi * (1+(x/mean)^2))</tt>.
         * <p>
         * <b>Implementation:</b>
         * This is a port of <tt>cin.c</tt> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         * <p>
         * @returns a number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).
         */

        public override double NextDouble()
        {
            return Mean + Math.Tan(Math.PI*m_rng.NextDouble());
        }

        public override double Cdf(double dblX)
        {
            throw new NotImplementedException();
        }

        public override double CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblMean,
            double dblX)
        {
            m_ownInstance.SetState(
                dblMean);

            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblMean,
            double dblX)
        {
            m_ownInstance.SetState(
                dblMean);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblMean,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblMean);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblMean)
        {
            m_ownInstance.SetState(
                dblMean);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblMean,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblMean);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblMean,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblMean);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
