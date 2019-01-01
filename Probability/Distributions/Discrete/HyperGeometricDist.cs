#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Discrete
{
    public class HyperGeometricDist : AbstractUnivDiscrDist
    {
        #region Constants

        private const int INT_RND_SEED = 31;

        #endregion

        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly HyperGeometricDist m_ownInstance = new HyperGeometricDist(
            1, 1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblCPm;

        private double m_dblDl;
        private double m_dblDr;
        private double m_dblF1;
        private double m_dblF2;
        private double m_dblF4;
        private double m_dblF5;
        private double m_dblFm;
        private double m_dblLl;
        private double m_dblLr;
        private double m_dblMp;
        private double m_dblNp;
        private double m_dblP1;
        private double m_dblP2;
        private double m_dblP3;
        private double m_dblP4;
        private double m_dblP5;
        private double m_dblP6;
        private double m_dblR1;
        private double m_dblR2;
        private double m_dblR4;
        private double m_dblR5;
        private int m_intB;
        private int m_intK1;
        private int m_intK2;
        private int m_intK4;
        private int m_intK5;
        private int m_intM;
        private int m_intMLast = -1;
        private int m_intMp;
        private int m_intN;
        private int m_intNLast = -1;
        private int m_intNLast2 = -1;
        private int m_intS;
        private int m_intT;
        private int m_NMn;

        #endregion

        #region Constructors

        public HyperGeometricDist(
            int intN,
            int intS,
            int intT,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                intN,
                intS,
                intT);
        }

        #endregion

        #region Parameters

        public int N
        {
            get { return m_intN; }
            set
            {
                m_intN = value;
                SetState(
                    m_intN,
                    m_intS,
                    m_intT);
            }
        }

        public int S
        {
            get { return m_intS; }
            set
            {
                m_intS = value;
                SetState(
                    m_intN,
                    m_intS,
                    m_intT);
            }
        }

        public int T
        {
            get { return m_intT; }
            set
            {
                m_intT = value;
                SetState(
                    m_intN,
                    m_intS,
                    m_intT);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            int intN,
            int intS,
            int intT)
        {
            m_intN = intN;
            m_intS = intS;
            m_intT = intT;
        }

        #endregion

        #region Private

        private double fc_lnpk(int k, int N_Mn, int M, int n)
        {
            return (Arithmetic.LogFactorial(k) + Arithmetic.LogFactorial(M - k) + Arithmetic.LogFactorial(n - k) +
                    Arithmetic.LogFactorial(N_Mn + k));
        }

        /**
         * Returns a random number from the distribution.
         */

        private int hmdu(
            int N,
            int M,
            int n)
        {
            int I, K;
            double p, nu, c, d, U = 0;

            if (N != m_intNLast || M != m_intMLast || n != m_intNLast2)
            {
                // set-up           */
                m_intNLast = N;
                m_intMLast = M;
                m_intNLast2 = n;

                m_dblMp = (M + 1);
                m_dblNp = (n + 1);
                m_NMn = N - M - n;

                p = m_dblMp/(N + 2.0);
                nu = m_dblNp*p; /* mode, real       */
                if ((m_intM = (int) nu) == nu && p == 0.5)
                {
                    /* mode, integer    */
                    m_intMp = m_intM--;
                }
                else
                {
                    m_intMp = m_intM + 1; /* mp = m + 1       */
                }

                /* mode probability, using the external function flogfak(k) = ln(k!)    */
                m_dblFm =
                    Math.Exp(Arithmetic.LogFactorial(N - M) - Arithmetic.LogFactorial(m_NMn + m_intM) -
                             Arithmetic.LogFactorial(n - m_intM)
                             + Arithmetic.LogFactorial(M) - Arithmetic.LogFactorial(M - m_intM) -
                             Arithmetic.LogFactorial(m_intM)
                             - Arithmetic.LogFactorial(N) + Arithmetic.LogFactorial(N - n) + Arithmetic.LogFactorial(n));

                /* safety bound  -  guarantees at least 17 significant decimal digits   */
                /*                  b = Min(n, (long int)(nu + k*c')) */
                m_intB = (int) (nu + 11.0*Math.Sqrt(nu*(1.0 - p)*(1.0 - n/(double) N) + 1.0));
                if (m_intB > n)
                {
                    m_intB = n;
                }
            }

            for (;;)
            {
                if ((U = m_rng.NextDouble() - m_dblFm) <= 0.0)
                {
                    return (m_intM);
                }
                c = d = m_dblFm;

                /* down- and upward search from the mode                                */
                for (I = 1; I <= m_intM; I++)
                {
                    K = m_intMp - I; /* downward search  */
                    c *= K/(m_dblNp - K)*((m_NMn + K)/(m_dblMp - K));
                    if ((U -= c) <= 0.0)
                    {
                        return (K - 1);
                    }

                    K = m_intM + I; /* upward search    */
                    d *= (m_dblNp - K)/K*((m_dblMp - K)/(m_NMn + K));
                    if ((U -= d) <= 0.0)
                    {
                        return (K);
                    }
                }

                /* upward search from K = 2m + 1 to K = b                               */
                for (K = m_intMp + m_intM; K <= m_intB; K++)
                {
                    d *= (m_dblNp - K)/K*((m_dblMp - K)/(m_NMn + K));
                    if ((U -= d) <= 0.0)
                    {
                        return (K);
                    }
                }
            }
        }

        /**
         * Returns a random number from the distribution.
         */

        private int hprs(
            int N,
            int M,
            int n)
        {
            int Dk = 0, X, V;
            double Mp, np, p, nu, U = 0, Y = 0, W; /* (X, Y) <-> (V, W) */

            if (N != m_intNLast || M != m_intMLast || n != m_intNLast2)
            {
                /* set-up            */
                m_intNLast = N;
                m_intMLast = M;
                m_intNLast2 = n;

                Mp = (M + 1);
                np = (n + 1);
                m_NMn = N - M - n;

                p = Mp/(N + 2.0);
                nu = np*p; // main parameters

                // approximate deviation of reflection points k2, k4 from nu - 1/2
                U = Math.Sqrt(nu*(1.0 - p)*(1.0 - (n + 2.0)/(N + 3.0)) + 0.25);

                // mode m, reflection points k2 and k4, and points k1 and k5, which
                // delimit the centre region of h(x)
                // k2 = Ceiling (nu - 1/2 - U),    k1 = 2*k2 - (m - 1 + delta_ml)
                // k4 = Floor(nu - 1/2 + U),    k5 = 2*k4 - (m + 1 - delta_mr)

                m_intM = (int) nu;
                m_intK2 = (int) Math.Ceiling(nu - 0.5 - U);
                if (m_intK2 >= m_intM)
                {
                    m_intK2 = m_intM - 1;
                }
                m_intK4 = (int) (nu - 0.5 + U);
                m_intK1 = m_intK2 + m_intK2 - m_intM + 1; // delta_ml = 0
                m_intK5 = m_intK4 + m_intK4 - m_intM; // delta_mr = 1

                // range width of the critical left and right centre region
                m_dblDl = (m_intK2 - m_intK1);
                m_dblDr = (m_intK5 - m_intK4);

                // recurrence constants r(k) = p(k)/p(k-1) at k = k1, k2, k4+1, k5+1
                m_dblR1 = (np/m_intK1 - 1.0)*(Mp - m_intK1)/(m_NMn + m_intK1);
                m_dblR2 = (np/m_intK2 - 1.0)*(Mp - m_intK2)/(m_NMn + m_intK2);
                m_dblR4 = (np/(m_intK4 + 1) - 1.0)*(M - m_intK4)/(m_NMn + m_intK4 + 1);
                m_dblR5 = (np/(m_intK5 + 1) - 1.0)*(M - m_intK5)/(m_NMn + m_intK5 + 1);

                // reciprocal values of the scale parameters of expon. tail envelopes
                m_dblLl = Math.Log(m_dblR1); // expon. tail left  //
                m_dblLr = -Math.Log(m_dblR5); // expon. tail right //

                // hypergeom. constant, necessary for computing function values f(k)
                m_dblCPm = fc_lnpk(m_intM, m_NMn, M, n);

                // function values f(k) = p(k)/p(m)  at  k = k2, k4, k1, k5
                m_dblF2 = Math.Exp(m_dblCPm - fc_lnpk(m_intK2, m_NMn, M, n));
                m_dblF4 = Math.Exp(m_dblCPm - fc_lnpk(m_intK4, m_NMn, M, n));
                m_dblF1 = Math.Exp(m_dblCPm - fc_lnpk(m_intK1, m_NMn, M, n));
                m_dblF5 = Math.Exp(m_dblCPm - fc_lnpk(m_intK5, m_NMn, M, n));

                // area of the two centre and the two exponential tail regions
                // area of the two immediate acceptance regions between k2, k4
                m_dblP1 = m_dblF2*(m_dblDl + 1.0); // immed. left
                m_dblP2 = m_dblF2*m_dblDl + m_dblP1; // centre left
                m_dblP3 = m_dblF4*(m_dblDr + 1.0) + m_dblP2; // immed. right
                m_dblP4 = m_dblF4*m_dblDr + m_dblP3; // centre right
                m_dblP5 = m_dblF1/m_dblLl + m_dblP4; // expon. tail left
                m_dblP6 = m_dblF5/m_dblLr + m_dblP5; // expon. tail right
            }

            for (;;)
            {
                // generate uniform number U -- U(0, p6)
                // case distinction corresponding to U
                if ((U = m_rng.NextDouble()*m_dblP6) < m_dblP2)
                {
                    // centre left

                    // immediate acceptance region R2 = [k2, m) *[0, f2),  X = k2, ... m -1
                    if ((W = U - m_dblP1) < 0.0)
                    {
                        return (m_intK2 + (int) (U/m_dblF2));
                    }
                    // immediate acceptance region R1 = [k1, k2)*[0, f1),  X = k1, ... k2-1
                    if ((Y = W/m_dblDl) < m_dblF1)
                    {
                        return (m_intK1 + (int) (W/m_dblF1));
                    }

                    // computation of candidate X < k2, and its counterpart V > k2
                    // either squeeze-acceptance of X or acceptance-rejection of V
                    Dk = (int) (m_dblDl*m_rng.NextDouble()) + 1;
                    if (Y <= m_dblF2 - Dk*(m_dblF2 - m_dblF2/m_dblR2))
                    {
                        // quick accept of
                        return (m_intK2 - Dk); // X = k2 - Dk
                    }
                    if ((W = m_dblF2 + m_dblF2 - Y) < 1.0)
                    {
                        // quick reject of V
                        V = m_intK2 + Dk;
                        if (W <= m_dblF2 + Dk*(1.0 - m_dblF2)/(m_dblDl + 1.0))
                        {
                            // quick accept of
                            return (V); // V = k2 + Dk
                        }
                        if (Math.Log(W) <= m_dblCPm - fc_lnpk(V, m_NMn, M, n))
                        {
                            return (V); // final accept of V
                        }
                    }
                    X = m_intK2 - Dk;
                }
                else if (U < m_dblP4)
                {
                    // centre right

                    // immediate acceptance region R3 = [m, k4+1)*[0, f4), X = m, ... k4
                    if ((W = U - m_dblP3) < 0.0)
                    {
                        return (m_intK4 - (int) ((U - m_dblP2)/m_dblF4));
                    }
                    // immediate acceptance region R4 = [k4+1, k5+1)*[0, f5)
                    if ((Y = W/m_dblDr) < m_dblF5)
                    {
                        return (m_intK5 - (int) (W/m_dblF5));
                    }

                    // computation of candidate X > k4, and its counterpart V < k4
                    // either squeeze-acceptance of X or acceptance-rejection of V
                    Dk = (int) (m_dblDr*m_rng.NextDouble()) + 1;
                    if (Y <= m_dblF4 - Dk*(m_dblF4 - m_dblF4*m_dblR4))
                    {
                        // quick accept of
                        return (m_intK4 + Dk); // X = k4 + Dk
                    }
                    if ((W = m_dblF4 + m_dblF4 - Y) < 1.0)
                    {
                        // quick reject of V
                        V = m_intK4 - Dk;
                        if (W <= m_dblF4 + Dk*(1.0 - m_dblF4)/m_dblDr)
                        {
                            // quick accept of
                            return (V); // V = k4 - Dk
                        }
                        if (Math.Log(W) <= m_dblCPm - fc_lnpk(V, m_NMn, M, n))
                        {
                            return (V); // final accept of V
                        }
                    }
                    X = m_intK4 + Dk;
                }
                else
                {
                    Y = m_rng.NextDouble();
                    if (U < m_dblP5)
                    {
                        // expon. tail left
                        Dk = (int) (1.0 - Math.Log(Y)/m_dblLl);
                        if ((X = m_intK1 - Dk) < 0)
                        {
                            continue; // 0 <= X <= k1 - 1
                        }
                        Y *= (U - m_dblP4)*m_dblLl; // Y -- U(0, h(x))
                        if (Y <= m_dblF1 - Dk*(m_dblF1 - m_dblF1/m_dblR1))
                        {
                            return (X); // quick accept of X
                        }
                    }
                    else
                    {
                        // expon. tail right
                        Dk = (int) (1.0 - Math.Log(Y)/m_dblLr);
                        if ((X = m_intK5 + Dk) > n)
                        {
                            continue; // k5 + 1 <= X <= n
                        }
                        Y *= (U - m_dblP5)*m_dblLr; // Y -- U(0, h(x))   /
                        if (Y <= m_dblF5 - Dk*(m_dblF5 - m_dblF5*m_dblR5))
                        {
                            return (X); // quick accept of X
                        }
                    }
                }

                // acceptance-rejection test of candidate X from the original area
                // test, whether  Y <= f(X),    with  Y = U*h(x)  and  U -- U(0, 1)
                // log f(X) = Log( m! (M - m)! (n - m)! (N - M - n + m)! )
                //          - Log( X! (M - X)! (n - X)! (N - M - n + X)! )
                // by using an external function for log k!
                if (Math.Log(Y) <= m_dblCPm - fc_lnpk(X, m_NMn, M, n))
                {
                    return (X);
                }
            }
        }

        #endregion

        #region Public

        /**
         * Returns a random number from the distribution; bypasses the internal state.
         */

        public override int NextInt()
        {
            /******************************************************************
             *                                                                *
             * Hypergeometric Distribution - Patchwork Rejection/Inversion    *
             *                                                                *
             ******************************************************************
             *                                                                *
             * The basic algorithms work for parameters 1 <= n <= M <= N/2.   *
             * Otherwise parameters are re-defined in the set-up step and the *
             * random number K is adapted before delivering.                  *
             * For l = m-Max(0,n-N+M) < 10  Inversion method hmdu is applied: *
             * The random numbers are generated via modal down-up search,     *
             * starting at the mode m. The cumulative probabilities           *
             * are avoided by using the technique of chop-down.               *
             * For l >= 10  the Patchwork Rejection method  hprs is employed: *
             * The area below the histogram function f(x) in its              *
             * body is rearranged by certain point reflections. Within a      *
             * large center interval variates are sampled efficiently by      *
             * rejection from uniform hats. Rectangular immediate acceptance  *
             * regions speed up the generation. The remaining tails are       *
             * covered by exponential functions.                              *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - hprsc samples a random number from the          *
             *                Hypergeometric distribution with parameters     *
             *                N (number of red and black balls), M (number    *
             *                of red balls) and n (number of trials)          *
             *                valid for N >= 2, M,n <= N.                     *
             * REFERENCE :  - H. Zechner (1994): Efficient sampling from      *
             *                continuous and discrete unimodal distributions, *
             *                Doctoral Dissertation, 156 pp., Technical       *
             *                University Graz, Austria.                       *
             * SUBPROGRAMS: - flogfak(k)  ... Log(k!) with long integer k     *
             *              - drand(seed) ... (0,1)-Uniform generator with    *
             *                unsigned long integer *seed.                    *
             *              - hmdu(seed,N,M,n) ... Hypergeometric generator   *
             *                for l<10                                        *
             *              - hprs(seed,N,M,n) ... Hypergeometric generator   *
             *                for l>=10 with unsigned long integer *seed,     *
             *                long integer  N , M , n.                        *
             *                                                                *
             ******************************************************************/
            int Nhalf, n_le_Nhalf, M_le_Nhalf, K = 0;

            Nhalf = N/2;
            n_le_Nhalf = (T <= Nhalf) ? T : N - T;
            M_le_Nhalf = (S <= Nhalf) ? S : N - S;

            if ((T*S/N) < 10)
            {
                K = (n_le_Nhalf <= M_le_Nhalf)
                        ? hmdu(N, M_le_Nhalf, n_le_Nhalf)
                        : hmdu(N, n_le_Nhalf, M_le_Nhalf);
            }
            else
            {
                K = (n_le_Nhalf <= M_le_Nhalf)
                        ? hprs(N, M_le_Nhalf, n_le_Nhalf)
                        : hprs(N, n_le_Nhalf, M_le_Nhalf);
            }

            if (T <= Nhalf)
            {
                return (S <= Nhalf) ? K : T - K;
            }
            else
            {
                return (S <= Nhalf) ? S - K : T - N + S + K;
            }
        }

        /**
         * Returns the probability distribution function.
         */

        public override double Pdf(int k)
        {
            return Arithmetic.binomial(S, k)*Arithmetic.binomial(N - S, T - k)
                   /Arithmetic.binomial(N, T);
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "HyperGeometric(" + N + "," + S + "," + T + ")";
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
            int intN,
            int intS,
            int intT,
            int intX)
        {
            m_ownInstance.SetState(
                intN,
                intS,
                intT);

            return m_ownInstance.Pdf(intX);
        }

        public static double CdfStatic(
            int intN,
            int intS,
            int intT,
            int intX)
        {
            m_ownInstance.SetState(
                intN,
                intS,
                intT);

            return m_ownInstance.Cdf(intX);
        }

        public static double CdfInvStatic(
            int intN,
            int intS,
            int intT,
            double dblProbability)
        {
            m_ownInstance.SetState(
                intN,
                intS,
                intT);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextIntStatic(
            int intN,
            int intS,
            int intT)
        {
            m_ownInstance.SetState(
                intN,
                intS,
                intT);

            return m_ownInstance.NextInt();
        }

        public static int[] NextIntArrStatic(
            int intN,
            int intS,
            int intT,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                intN,
                intS,
                intT);

            return m_ownInstance.NextIntArr(intSampleSize);
        }

        #endregion
    }
}
