#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class TriangularDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly TriangularDist m_ownInstance = new TriangularDist(
            1, 1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblA;
        private double m_dblB;
        private double m_dblC;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 18;

        #endregion

        #region Constructors

        public TriangularDist(
            double dblA,
            double dblB,
            double dblC,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblA,
                dblB,
                dblC);
        }

        #endregion

        #region Parameters

        public double A
        {
            get { return m_dblA; }
            set
            {
                m_dblA = value;
                SetState(
                    m_dblA,
                    m_dblB,
                    m_dblC);
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
                    m_dblB,
                    m_dblC);
            }
        }

        public double C
        {
            get { return m_dblC; }
            set
            {
                m_dblC = value;
                SetState(
                    m_dblA,
                    m_dblB,
                    m_dblC);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblA,
            double dblB,
            double dblC)
        {
            m_dblA = dblA;
            m_dblB = dblB;
            m_dblC = dblC;
        }

        #endregion

        #region Public

        /**
         *  Insert the method's description here. Creation date: (2/10/00 2:15:05 PM)
         *
         *@param  c  double
         *@return    double
         */

        public double NextDoubleStd(double c)
        {
            double u = m_rng.NextDouble();
            return (u <= c) ? Math.Sqrt(c*u) : 1.0 - Math.Sqrt((1.0 - c)*(1.0 - u));
        }


        /**
         *  Insert the method's description here. Creation date: (2/10/00 2:17:32 PM)
         *
         *@param  a  double
         *@param  b  double
         *@param  c  double
         *@return    double
         */

        public override double NextDouble()
        {
            return (NextDoubleStd(C) - A)/(B - A);
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
            double dblA,
            double dblB,
            double dblC,
            double dblX)
        {
            m_ownInstance.SetState(
                dblA,
                dblB,
                dblC);

            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblA,
            double dblB,
            double dblC,
            double dblX)
        {
            m_ownInstance.SetState(
                dblA,
                dblB,
                dblC);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblA,
            double dblB,
            double dblC,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblA,
                dblB,
                dblC);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblA,
            double dblB,
            double dblC)
        {
            m_ownInstance.SetState(
                dblA,
                dblB,
                dblC);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblA,
            double dblB,
            double dblC,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblA,
                dblB,
                dblC);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblA,
            double dblB,
            double dblC,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblA,
                dblB,
                dblC);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
