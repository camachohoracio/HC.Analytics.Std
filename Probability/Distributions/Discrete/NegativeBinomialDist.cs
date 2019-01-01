#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Discrete
{
    public class NegativeBinomialDist : AbstractUnivDiscrDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly NegativeBinomialDist m_ownInstance = new NegativeBinomialDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblP;
        private GeometricDist m_geometricDist;
        private int m_intS;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 32;

        #endregion

        #region Constructors

        public NegativeBinomialDist(
            double dblP,
            int dblS,
            RngWrapper rng)
            : base(rng)
        {
            SetState(dblP, dblS);
        }

        #endregion

        #region Parameters

        public double P
        {
            get { return m_dblP; }
            set
            {
                m_dblP = value;
                SetState(
                    m_dblP,
                    m_intS);
            }
        }

        public int S
        {
            get { return m_intS; }
            set
            {
                m_intS = value;
                SetState(
                    m_dblP,
                    m_intS);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblP,
            int intS)
        {
            m_dblP = dblP;
            m_intS = intS;
            m_geometricDist =
                new GeometricDist(P,
                                  m_rng);
        }

        #endregion

        #region Public

        /**
         * Returns the sum of the terms <tt>0</tt> through <tt>k</tt> of the Negative Binomial Distribution.
         * <pre>
         *   k
         *   --  ( n+j-1 )   n      j
         *   >   (       )  p  (1-p)
         *   --  (   j   )
         *  j=0
         * </pre>
         * In a sequence of Bernoulli trials, this is the probability
         * that <tt>k</tt> or fewer failures precede the <tt>n</tt>-th success.
         * <p>
         * The terms are not computed individually; instead the incomplete
         * beta integral is employed, according to the formula
         * <p>
         * <tt>y = negativeBinomial( k, n, p ) = Gamma.incompleteBeta( n, k+1, p )</tt>.
         *
         * All arguments must be positive,
         * @param k end term.
         * @param n the number of trials.
         * @param p the probability of success (must be in <tt>(0.0,1.0)</tt>).
         */

        public override double Cdf(int k)
        {
            if ((P < 0.0) || (P > 1.0))
            {
                throw new ArgumentException();
            }
            if (k < 0)
            {
                return 0.0;
            }

            return IncompleteBetaFunct.IncompleteBeta(S, k + 1, P);
        }

        /**
         * Returns the sum of the terms <tt>k+1</tt> to infinity of the Negative
         * Binomial distribution.
         * <pre>
         *   inf
         *   --  ( n+j-1 )   n      j
         *   >   (       )  p  (1-p)
         *   --  (   j   )
         *  j=k+1
         * </pre>
         * The terms are not computed individually; instead the incomplete
         * beta integral is employed, according to the formula
         * <p>
         * y = negativeBinomialComplemented( k, n, p ) = Gamma.incompleteBeta( k+1, n, 1-p ).
         *
         * All arguments must be positive,
         * @param k end term.
         * @param n the number of trials.
         * @param p the probability of success (must be in <tt>(0.0,1.0)</tt>).
         */

        public double CdfComplemented(int k)
        {
            if ((P < 0.0) || (P > 1.0))
            {
                throw new ArgumentException();
            }
            if (k < 0)
            {
                return 0.0;
            }

            return IncompleteBetaFunct.IncompleteBeta(k + 1, S, 1.0 - P);
        }

        /**
         *  Insert the method's description here. Creation date: (1/26/00 12:42:53 PM)
         *
         *@param  s   int
         *@param  p   double
         *@return     int
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public override int NextInt()
        {
            int x = 0;
            for (int i = 0; i < S; i++)
            {
                x += m_geometricDist.NextInt();
            }
            return x;
        }

        /**
         * Returns the probability distribution function.
         */

        public override double Pdf(int k)
        {
            if (k > S)
            {
                throw new ArgumentException();
            }
            return Arithmetic.Binomial(S, k)*Math.Pow(S, k)*Math.Pow(1.0 - P, S - k);
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "NegiveBinomial(" + S + "," + P + ")";
        }

        public override int CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblP,
            int intN,
            int intX)
        {
            m_ownInstance.SetState(
                dblP,
                intN);

            return m_ownInstance.Pdf(intX);
        }

        public static double CdfStatic(
            double dblP,
            int intN,
            int intX)
        {
            m_ownInstance.SetState(
                dblP,
                intN);

            return m_ownInstance.Cdf(intX);
        }

        public static double CdfInvStatic(
            double dblP,
            int intN,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblP,
                intN);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextIntStatic(
            double dblP,
            int intN)
        {
            m_ownInstance.SetState(
                dblP,
                intN);

            return m_ownInstance.NextInt();
        }

        public static int[] NextIntArrStatic(
            double dblP,
            int intN,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblP,
                intN);

            return m_ownInstance.NextIntArr(intSampleSize);
        }

        #endregion
    }
}
