#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Discrete
{
    public class ZipfDist : AbstractUnivDiscrDist
    {
        #region Constants

        private const int INT_RND_SEED = 35;

        #endregion

        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly ZipfDist m_ownInstance = new ZipfDist(
            1, new RngWrapper(INT_RND_SEED));

        private double m_dblZ;

        #endregion

        #region Constructors

        public ZipfDist(
            double dblZ,
            RngWrapper rng) : base(rng)
        {
            SetState(dblZ);
        }

        #endregion

        #region Parameters

        public double Z
        {
            get { return m_dblZ; }
            set
            {
                m_dblZ = value;
                SetState(m_dblZ);
                ;
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblZ)
        {
            m_dblZ = dblZ;
        }

        #endregion

        #region Public

        /**
         * Returns a zipfian distributed random number with the given skew.
         * <p>
         * Algorithm from page 551 of:
         * Devroye, Luc (1986) `Non-uniform random variate generation',
         * Springer-Verlag: Berlin.   ISBN 3-540-96305-7 (also 0-387-96305-7)
         *
         * @param z the skew of the distribution (must be &gt;1.0).
         * @returns a zipfian distributed number in the closed interval <tt>[1,Integer.MAX_VALUE]</tt>.
         */

        public override int NextInt()
        {
            /* Algorithm from page 551 of:
             * Devroye, Luc (1986) `Non-uniform random variate generation',
             * Springer-Verlag: Berlin.   ISBN 3-540-96305-7 (also 0-387-96305-7)
             */
            double b = Math.Pow(2.0, Z - 1.0);
            double constant = -1.0/(Z - 1.0);

            int result = 0;
            for (;;)
            {
                double u = m_rng.NextDouble();
                double v = m_rng.NextDouble();
                result = (int) (Math.Floor(Math.Pow(u, constant)));
                double t = Math.Pow(1.0 + 1.0/result, Z - 1.0);
                if (v*result*(t - 1.0)/(b - 1.0) <= t/b)
                {
                    break;
                }
            }
            return result;
        }

        public override double Cdf(int intX)
        {
            throw new NotImplementedException();
        }

        public override int CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        public override double Pdf(int intX)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblZ,
            int intX)
        {
            m_ownInstance.SetState(
                dblZ);

            return m_ownInstance.Pdf(intX);
        }

        public static double CdfStatic(
            double dblZ,
            int intX)
        {
            m_ownInstance.SetState(
                dblZ);

            return m_ownInstance.Cdf(intX);
        }

        public static double CdfInvStatic(
            double dblZ,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblZ);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextIntStatic(
            double dblZ)
        {
            m_ownInstance.SetState(
                dblZ);

            return m_ownInstance.NextInt();
        }

        public static int[] NextIntArrStatic(
            double dblZ,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblZ);

            return m_ownInstance.NextIntArr(intSampleSize);
        }

        #endregion
    }
}
