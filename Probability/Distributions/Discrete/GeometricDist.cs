#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Discrete
{
    public class GeometricDist : AbstractUnivDiscrDist
    {
        #region Constants

        private const int INT_RND_SEED = 30;

        #endregion

        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly GeometricDist m_ownInstance = new GeometricDist(
            1, new RngWrapper(INT_RND_SEED));

        private double m_dblP;

        #endregion

        #region Constructors

        public GeometricDist(
            double dblP,
            RngWrapper rng) : base(rng)
        {
            SetState(dblP);
        }

        #endregion

        #region Parameters

        public double P
        {
            get { return m_dblP; }
            set
            {
                m_dblP = value;
                SetState(m_dblP);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblP)
        {
            m_dblP = dblP;
        }

        #endregion

        #region Public

        /**
         *  Geometric(p)
         *
         *@param  p   p
         *@return     Geometric(p)
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public override int NextInt()
        {
            return (int) Math.Floor(
                             Math.Log(
                                 m_rng.NextDouble())/Math.Log(1.0 - P));
        }


        /**
         * Returns the probability distribution function of the discrete geometric distribution.
         * <p>
         * <tt>p(k) = p * (1-p)^k</tt> for <tt> k &gt;= 0</tt>.
         * <p>
         * @param k the argument to the probability distribution function.
         * @param p the parameter of the probability distribution function.
         */

        public override double Pdf(int k)
        {
            if (k < 0)
            {
                throw new ArgumentException();
            }
            return P*Math.Pow(1 - P, k);
        }

        public override double Cdf(int intX)
        {
            throw new NotImplementedException();
        }

        public override int CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblP,
            int intX)
        {
            m_ownInstance.SetState(
                dblP);

            return m_ownInstance.Pdf(intX);
        }

        public static double CdfStatic(
            double dblP,
            int intX)
        {
            m_ownInstance.SetState(
                dblP);

            return m_ownInstance.Cdf(intX);
        }

        public static double CdfInvStatic(
            double dblP,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblP);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextIntStatic(
            double dblP)
        {
            m_ownInstance.SetState(
                dblP);

            return m_ownInstance.NextInt();
        }

        public static int[] NextIntArrStatic(
            double dblP,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblP);

            return m_ownInstance.NextIntArr(intSampleSize);
        }

        #endregion
    }
}
