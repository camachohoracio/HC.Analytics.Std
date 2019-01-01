#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Discrete
{
    public class BinomialDist : AbstractUnivDiscrDist
    {
        #region Constants

        private const int INT_RND_SEED = 29;

        #endregion

        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly BinomialDist m_ownInstance = new BinomialDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblC;
        private double m_dblCh;
        private double m_dblLl;
        private double m_dblLogN;
        private double m_dblLogP;
        private double m_dblLogQ;
        private double m_dblLr;

        private double m_dblNp;
        private double m_dblP;
        private double m_dblP0;
        private double m_dblP1;
        private double m_dblP2;
        private double m_dblP3;
        private double m_dblP4;
        private double m_dblPar;
        private double m_dblPLast = -1.0;
        private double m_dblPPrev = -1.0;
        private double m_dblPq;
        private double m_dblQ;
        private double m_dblRc;
        private double m_dblSs;
        private double m_dblXl;
        private double m_dblXm;
        private double m_dblXr;
        private int m_intB;
        private int m_intM;
        private int m_intN;
        private int m_intNLast = -1;
        private int m_intNm;
        private int m_intNPrev = -1;

        #endregion

        #region Constructors

        /**
 * Constructs a binomial distribution.
 * Example: n=1, p=0.5.
 * @param n the number of trials (also known as <i>sample size</i>).
 * @param p the probability of success.
 * @param randomGenerator a uniform random number generator.
 * @throws ArgumentException if <tt>n*Math.Min(p,1-p) &lt;= 0.0</tt>
 */

        public BinomialDist(
            int intN,
            double dblP,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblP,
                intN);
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
                    m_intN);
            }
        }

        public int N
        {
            get { return m_intN; }
            set
            {
                m_intN = value;
                SetState(
                    m_dblP,
                    m_intN);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblP,
            int intN)
        {
            m_dblP = dblP;
            m_intN = intN;

            /**
             * Sets the parameters number of trials and the probability of success.
             * @param n the number of trials
             * @param p the probability of success.
             * @throws ArgumentException if <tt>n*Math.Min(p,1-p) &lt;= 0.0</tt>
             */

            if (N*Math.Min(P, 1 - P) <= 0.0)
            {
                throw new ArgumentException();
            }
            N = N;
            P = P;

            m_dblLogP = Math.Log(P);
            m_dblLogQ = Math.Log(1.0 - P);
            m_dblLogN = Arithmetic.LogFactorial(N);
        }

        #endregion

        #region Public

        /**
         * Returns the sum of the terms <tt>0</tt> through <tt>k</tt> of the Binomial
         * probability density.
         * <pre>
         *   k
         *   --  ( n )   j      n-j
         *   >   (   )  p  (1-p)
         *   --  ( j )
         *  j=0
         * </pre>
         * The terms are not summed directly; instead the incomplete
         * beta integral is employed, according to the formula
         * <p>
         * <tt>y = binomial( k, n, p ) = Gamma.incompleteBeta( n-k, k+1, 1-p )</tt>.
         * <p>
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
            if ((k < 0) || (N < k))
            {
                throw new ArgumentException();
            }

            if (k == N)
            {
                return (1.0);
            }
            if (k == 0)
            {
                return Math.Pow(1.0 - P, N - k);
            }

            return IncompleteBetaFunct.IncompleteBeta(N - k, k + 1, 1.0 - P);
        }

        /**
         * Returns the probability distribution function.
         */

        public override double Pdf(int k)
        {
            if (k < 0)
            {
                throw new ArgumentException();
            }
            int r = N - k;
            return Math.Exp(
                m_dblLogN -
                Arithmetic.LogFactorial(k) -
                Arithmetic.LogFactorial(r) +
                m_dblLogP*k + m_dblLogQ*r);
        }

        /**
         * Returns the sum of the terms <tt>k+1</tt> through <tt>n</tt> of the Binomial
         * probability density.
         * <pre>
         *   n
         *   --  ( n )   j      n-j
         *   >   (   )  p  (1-p)
         *   --  ( j )
         *  j=k+1
         * </pre>
         * The terms are not summed directly; instead the incomplete
         * beta integral is employed, according to the formula
         * <p>
         * <tt>y = binomialComplemented( k, n, p ) = Gamma.incompleteBeta( k+1, n-k, p )</tt>.
         * <p>
         * All arguments must be positive,
         * @param k end term.
         * @param n the number of trials.
         * @param p the probability of success (must be in <tt>(0.0,1.0)</tt>).
         */

        public double CdfCompl(int k)
        {
            if ((P < 0.0) || (P > 1.0))
            {
                throw new ArgumentException();
            }
            if ((k < 0) || (N < k))
            {
                throw new ArgumentException();
            }

            if (k == N)
            {
                return (0.0);
            }
            if (k == 0)
            {
                return 1.0 - Math.Pow(1.0 - P, N - k);
            }

            return IncompleteBetaFunct.IncompleteBeta(k + 1, N - k, P);
        }

        /**
         *  Binomial(p)
         *
         *@param  p   p
         *@return     Binomial(p)
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public int NextInt2()
        {
            return m_rng.NextDouble() < P ? 0 : 1;
        }


        /******************************************************************
         *                                                                *
         *     Binomial-Distribution - Acceptance Rejection/Inversion     *
         *                                                                *
         ******************************************************************
         *                                                                *
         * Acceptance Rejection method combined with Inversion for        *
         * generating Binomial random numbers with parameters             *
         * n (number of trials) and p (probability of success).           *
         * For  Min(n*p,n*(1-p)) < 10  the Inversion method is applied:   *
         * The random numbers are generated via sequential search,        *
         * starting at the lowest index k=0. The cumulative probabilities *
         * are avoided by using the technique of chop-down.               *
         * For  Min(n*p,n*(1-p)) >= 10  Acceptance Rejection is used:     *
         * The algorithm is based on a hat-function which is uniform in   *
         * the centre region and exponential in the tails.                *
         * A triangular immediate acceptance region in the centre speeds  *
         * up the generation of binomial variates.                        *
         * If candidate k is near the mode, f(k) is computed recursively  *
         * starting at the mode m.                                        *
         * The acceptance test by Stirling's formula is modified          *
         * according to W. Hoermann (1992): The generation of binomial    *
         * random variates, to appear in J. Statist. Comput. Simul.       *
         * If  p < .5  the algorithm is applied to parameters n, p.       *
         * Otherwise p is replaced by 1-p, and k is replaced by n - k.    *
         *                                                                *
         ******************************************************************
         *                                                                *
         * FUNCTION:    - samples a random number from the binomial       *
         *                distribution with parameters n and p  and is    *
         *                valid for  n*Min(p,1-p)  >  0.                  *
         * REFERENCE:   - V. Kachitvichyanukul, B.W. Schmeiser (1988):    *
         *                Binomial random variate generation,             *
         *                Communications of the ACM 31, 216-222.          *
         * SUBPROGRAMS: - StirlingCorrection()                            *
         *                            ... Correction term of the Stirling *
         *                                approximation for Log(k!)       *
         *                                (series in 1/k or table values  *
         *                                for small k) with long int k    *
         *              - randomGenerator    ... (0,1)-Uniform engine     *
         *                                                                *
         ******************************************************************/

        public override int NextInt()
        {
            double C1_3 = 0.33333333333333333;
            double C5_8 = 0.62500000000000000;
            double C1_6 = 0.16666666666666667;
            int DMAX_KM = 20;

            int bh, i, K, Km, nK;
            double f, rm, U, V, X, T, E;

            if (N != m_intNLast || P != m_dblPLast)
            {
                // set-up
                m_intNLast = N;
                m_dblPLast = P;
                m_dblPar = Math.Min(P, 1.0 - P);
                m_dblQ = 1.0 - m_dblPar;
                m_dblNp = N*m_dblPar;

                // Check for invalid input values

                if (m_dblNp <= 0.0)
                {
                    return -1;
                }

                rm = m_dblNp + m_dblPar;
                m_intM = (int) rm; // mode, integer
                if (m_dblNp < 10)
                {
                    m_dblP0 = Math.Exp(N*Math.Log(m_dblQ)); // Chop-down
                    bh = (int) (m_dblNp + 10.0*Math.Sqrt(m_dblNp*m_dblQ));
                    m_intB = Math.Min(N, bh);
                }
                else
                {
                    m_dblRc = (N + 1.0)*(m_dblPq = m_dblPar/m_dblQ); // recurr. relat.
                    m_dblSs = m_dblNp*m_dblQ; // variance
                    i = (int) (2.195*Math.Sqrt(m_dblSs) - 4.6*m_dblQ); // i = p1 - 0.5
                    m_dblXm = m_intM + 0.5;
                    m_dblXl = (m_intM - i); // limit left
                    m_dblXr = (m_intM + i + 1L); // limit right
                    f = (rm - m_dblXl)/(rm - m_dblXl*m_dblPar);
                    m_dblLl = f*(1.0 + 0.5*f);
                    f = (m_dblXr - rm)/(m_dblXr*m_dblQ);
                    m_dblLr = f*(1.0 + 0.5*f);
                    m_dblC = 0.134 + 20.5/(15.3 + m_intM); // parallelogram
                    // height
                    m_dblP1 = i + 0.5;
                    m_dblP2 = m_dblP1*(1.0 + m_dblC + m_dblC); // probabilities
                    m_dblP3 = m_dblP2 + m_dblC/m_dblLl; // of regions 1-4
                    m_dblP4 = m_dblP3 + m_dblC/m_dblLr;
                }
            }

            if (m_dblNp < 10)
            {
                //Inversion Chop-down
                double pk;

                K = 0;
                pk = m_dblP0;
                U = m_rng.NextDouble();
                while (U > pk)
                {
                    ++K;
                    if (K > m_intB)
                    {
                        U = m_rng.NextDouble();
                        K = 0;
                        pk = m_dblP0;
                    }
                    else
                    {
                        U -= pk;
                        pk = (((N - K + 1)*m_dblPar*pk)/(K*m_dblQ));
                    }
                }
                return ((P > 0.5) ? (N - K) : K);
            }

            for (;;)
            {
                V = m_rng.NextDouble();
                if ((U = m_rng.NextDouble()*m_dblP4) <= m_dblP1)
                {
                    // triangular region
                    K = (int) (m_dblXm - U + m_dblP1*V);
                    return (P > 0.5) ? (N - K) : K; // immediate accept
                }
                if (U <= m_dblP2)
                {
                    // parallelogram
                    X = m_dblXl + (U - m_dblP1)/m_dblC;
                    if ((V = V*m_dblC + 1.0 - Math.Abs(m_dblXm - X)/m_dblP1) >= 1.0)
                    {
                        continue;
                    }
                    K = (int) X;
                }
                else if (U <= m_dblP3)
                {
                    // left tail
                    if ((X = m_dblXl + Math.Log(V)/m_dblLl) < 0.0)
                    {
                        continue;
                    }
                    K = (int) X;
                    V *= (U - m_dblP2)*m_dblLl;
                }
                else
                {
                    // right tail
                    if ((K = (int) (m_dblXr - Math.Log(V)/m_dblLr)) > N)
                    {
                        continue;
                    }
                    V *= (U - m_dblP3)*m_dblLr;
                }

                // acceptance test :  two cases, depending on |K - m|
                if ((Km = Math.Abs(K - m_intM)) <= DMAX_KM || Km + Km + 2L >= m_dblSs)
                {
                    // computation of p(K) via recurrence relationship from the mode
                    f = 1.0; // f(m)
                    if (m_intM < K)
                    {
                        for (i = m_intM; i < K;)
                        {
                            if ((f *= (m_dblRc/++i - m_dblPq)) < V)
                            {
                                break; // multiply  f
                            }
                        }
                    }
                    else
                    {
                        for (i = K; i < m_intM;)
                        {
                            if ((V *= (m_dblRc/++i - m_dblPq)) > f)
                            {
                                break; // multiply  V
                            }
                        }
                    }
                    if (V <= f)
                    {
                        break; // acceptance test
                    }
                }
                else
                {
                    // lower and upper squeeze tests, based on lower bounds for log p(K)
                    V = Math.Log(V);
                    T = -Km*Km/(m_dblSs + m_dblSs);
                    E = (Km/m_dblSs)*((Km*(Km*C1_3 + C5_8) + C1_6)/m_dblSs + 0.5);
                    if (V <= T - E)
                    {
                        break;
                    }
                    if (V <= T + E)
                    {
                        if (N != m_intNPrev || m_dblPar != m_dblPPrev)
                        {
                            m_intNPrev = N;
                            m_dblPPrev = m_dblPar;

                            m_intNm = N - m_intM + 1;
                            m_dblCh = m_dblXm*Math.Log((m_intM + 1.0)/(m_dblPq*m_intNm)) +
                                      Arithmetic.stirlingCorrection(m_intM + 1) + Arithmetic.stirlingCorrection(m_intNm);
                        }
                        nK = N - K + 1;

                        // computation of log f(K) via Stirling's formula
                        // acceptance-rejection test
                        if (V <= m_dblCh + (N + 1.0)*Math.Log(m_intNm/(double) nK) +
                                 (K + 0.5)*Math.Log(nK*m_dblPq/(K + 1.0)) -
                                 Arithmetic.stirlingCorrection(K + 1) - Arithmetic.stirlingCorrection(nK))
                        {
                            break;
                        }
                    }
                }
            }
            return (P > 0.5) ? (N - K) : K;
        }


        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "BinomialDist(" + N + "," + P + ")";
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
