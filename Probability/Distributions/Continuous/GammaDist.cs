#region

using System;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class GammaDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly GammaDist m_ownInstance = new GammaDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblAlpha;
        private double m_dblBeta;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 10;

        #endregion

        #region Constructors

        public GammaDist(
            double dblAlpha,
            double dblBeta,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblAlpha,
                dblBeta);
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
                    m_dblBeta);
            }
        }

        public double Beta
        {
            get { return m_dblBeta; }
            set
            {
                m_dblBeta = value;
                SetState(
                    m_dblAlpha,
                    m_dblBeta);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblAlpha,
            double dblBeta)
        {
            if (dblAlpha <= 0.0)
            {
                throw new ArgumentException();
            }
            if (dblBeta <= 0.0)
            {
                throw new ArgumentException();
            }

            m_dblAlpha = dblAlpha;
            m_dblBeta = dblBeta;
        }

        #endregion

        #region Public

        /**
         * Returns the integral from zero to <tt>x</tt> of the gamma probability
         * density function.
         * <pre>
         *                x
         *        b       -
         *       a       | |   b-1  -at
         * y =  -----    |    t    e    dt
         *       -     | |
         *      | (b)   -
         *               0
         * </pre>
         * The incomplete gamma integral is used, according to the
         * relation
         *
         * <tt>y = Gamma.incompleteGamma( b, a*x )</tt>.
         *
         * @param a the paramater a (alpha) of the gamma distribution.
         * @param b the paramater b (beta, lambda) of the gamma distribution.
         * @param x integration end point.
         */

        public override double Cdf(
            double x)
        {
            if (x < 0.0)
            {
                return 0.0;
            }
            return IncompleteGamaFunct.IncompleteGamma(Beta, Alpha*x);
        }

        /**
         * Returns the integral from <tt>x</tt> to infinity of the gamma
         * probability density function:
         * <pre>
         *               inf.
         *        b       -
         *       a       | |   b-1  -at
         * y =  -----    |    t    e    dt
         *       -     | |
         *      | (b)   -
         *               x
         * </pre>
         * The incomplete gamma integral is used, according to the
         * relation
         * <p>
         * y = Gamma.incompleteGammaComplement( b, a*x ).
         *
         * @param a the paramater a (alpha) of the gamma distribution.
         * @param b the paramater b (beta, lambda) of the gamma distribution.
         * @param x integration end point.
         */

        public double CdfCompl(double x)
        {
            if (x < 0.0)
            {
                return 0.0;
            }
            return IncompleteGamaFunct.IncompleteGammaComplement(Beta, Alpha*x);
        }

        /**
         * Returns the probability distribution function.
         */

        public override double NextDouble()
        {
            /******************************************************************
             *                                                                *
             *    Gamma Distribution - Acceptance Rejection combined with     *
             *                         Acceptance Complement                  *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION:    - gds samples a random number from the standard   *
             *                gamma distribution with parameter  a > 0.       *
             *                Acceptance Rejection  gs  for  a < 1 ,          *
             *                Acceptance Complement gd  for  a >= 1 .         *
             * REFERENCES:  - J.H. Ahrens, U. Dieter (1974): Computer methods *
             *                for sampling from gamma, beta, Poisson and      *
             *                binomial distributions, Computing 12, 223-246.  *
             *              - J.H. Ahrens, U. Dieter (1982): Generating gamma *
             *                variates by a modified rejection technique,     *
             *                Communications of the ACM 25, 47-54.            *
             * SUBPROGRAMS: - drand(seed) ... (0,1)-Uniform generator with    *
             *                unsigned long integer *seed                     *
             *              - NORMAL(seed) ... Normal generator N(0,1).       *
             *                                                                *
             ******************************************************************/
            double a = Alpha;
            double aa = -1.0,
                   aaa = -1.0,
                   b = 0.0,
                   c = 0.0,
                   d = 0.0,
                   e,
                   r,
                   s = 0.0,
                   si = 0.0,
                   ss = 0.0,
                   q0 = 0.0,
                   q1 = 0.0416666664,
                   q2 = 0.0208333723,
                   q3 = 0.0079849875,
                   q4 = 0.0015746717,
                   q5 = -0.0003349403,
                   q6 = 0.0003340332,
                   q7 = 0.0006053049,
                   q8 = -0.0004701849,
                   q9 = 0.0001710320,
                   a1 = 0.333333333,
                   a2 = -0.249999949,
                   a3 = 0.199999867,
                   a4 = -0.166677482,
                   a5 = 0.142873973,
                   a6 = -0.124385581,
                   a7 = 0.110368310,
                   a8 = -0.112750886,
                   a9 = 0.104089866,
                   e1 = 1.000000000,
                   e2 = 0.499999994,
                   e3 = 0.166666848,
                   e4 = 0.041664508,
                   e5 = 0.008345522,
                   e6 = 0.001353826,
                   e7 = 0.000247453;

            double gds, p, q, t, sign_u, u, v, w, x;
            double v1, v2, v12;

            // Check for invalid input values

            if (a <= 0.0)
            {
                throw new ArgumentException();
            }
            if (Beta <= 0.0)
            {
                new ArgumentException();
            }

            if (a < 1.0)
            {
                // CASE A: Acceptance rejection algorithm gs
                b = 1.0 + 0.36788794412*a; // Step 1
                for (;;)
                {
                    p = b*m_rng.NextDouble();
                    if (p <= 1.0)
                    {
                        // Step 2. Case gds <= 1
                        gds = Math.Exp(Math.Log(p)/a);
                        if (Math.Log(m_rng.NextDouble()) <= -gds)
                        {
                            return (gds/Beta);
                        }
                    }
                    else
                    {
                        // Step 3. Case gds > 1
                        gds = -Math.Log((b - p)/a);
                        if (Math.Log(m_rng.NextDouble()) <= ((a - 1.0)*Math.Log(gds)))
                        {
                            return (gds/Beta);
                        }
                    }
                }
            }

            else
            {
                // CASE B: Acceptance complement algorithm gd (gaussian distribution, box muller transformation)
                if (a != aa)
                {
                    // Step 1. Preparations
                    aa = a;
                    ss = a - 0.5;
                    s = Math.Sqrt(ss);
                    d = 5.656854249 - 12.0*s;
                }
                // Step 2. Normal deviate
                do
                {
                    v1 = 2.0*m_rng.NextDouble() - 1.0;
                    v2 = 2.0*m_rng.NextDouble() - 1.0;
                    v12 = v1*v1 + v2*v2;
                }
                while (v12 > 1.0);
                t = v1*Math.Sqrt(-2.0*Math.Log(v12)/v12);
                x = s + 0.5*t;
                gds = x*x;
                if (t >= 0.0)
                {
                    return (gds/Beta); // Immediate acceptance
                }

                u = m_rng.NextDouble(); // Step 3. Uniform random number
                if (d*u <= t*t*t)
                {
                    return (gds/Beta); // Squeeze acceptance
                }

                if (a != aaa)
                {
                    // Step 4. Set-up for hat case
                    aaa = a;
                    r = 1.0/a;
                    q0 = ((((((((q9*r + q8)*r + q7)*r + q6)*r + q5)*r + q4)*
                            r + q3)*r + q2)*r + q1)*r;
                    if (a > 3.686)
                    {
                        if (a > 13.022)
                        {
                            b = 1.77;
                            si = 0.75;
                            c = 0.1515/s;
                        }
                        else
                        {
                            b = 1.654 + 0.0076*ss;
                            si = 1.68/s + 0.275;
                            c = 0.062/s + 0.024;
                        }
                    }
                    else
                    {
                        b = 0.463 + s - 0.178*ss;
                        si = 1.235;
                        c = 0.195/s - 0.079 + 0.016*s;
                    }
                }
                if (x > 0.0)
                {
                    // Step 5. Calculation of q
                    v = t/(s + s); // Step 6.
                    if (Math.Abs(v) > 0.25)
                    {
                        q = q0 - s*t + 0.25*t*t + (ss + ss)*Math.Log(1.0 + v);
                    }
                    else
                    {
                        q = q0 + 0.5*t*t*((((((((a9*v + a8)*v + a7)*v + a6)*
                                              v + a5)*v + a4)*v + a3)*v + a2)*v + a1)*v;
                    } // Step 7. Quotient acceptance
                    if (Math.Log(1.0 - u) <= q)
                    {
                        return (gds/Beta);
                    }
                }

                for (;;)
                {
                    // Step 8. Double exponential deviate t
                    do
                    {
                        e = -Math.Log(m_rng.NextDouble());
                        u = m_rng.NextDouble();
                        u = u + u - 1.0;
                        sign_u = (u > 0) ? 1.0 : -1.0;
                        t = b + (e*si)*sign_u;
                    }
                    while (t <= -0.71874483771719); // Step 9. Rejection of t
                    v = t/(s + s); // Step 10. New q(t)
                    if (Math.Abs(v) > 0.25)
                    {
                        q = q0 - s*t + 0.25*t*t + (ss + ss)*Math.Log(1.0 + v);
                    }
                    else
                    {
                        q = q0 + 0.5*t*t*((((((((a9*v + a8)*v + a7)*v + a6)*
                                              v + a5)*v + a4)*v + a3)*v + a2)*v + a1)*v;
                    }
                    if (q <= 0.0)
                    {
                        continue; // Step 11.
                    }
                    if (q > 0.5)
                    {
                        w = Math.Exp(q) - 1.0;
                    }
                    else
                    {
                        w = ((((((e7*q + e6)*q + e5)*q + e4)*q + e3)*q + e2)*
                             q + e1)*q;
                    } // Step 12. Hat acceptance
                    if (c*u*sign_u <= w*Math.Exp(e - 0.5*t*t))
                    {
                        x = s + 0.5*t;
                        return (x*x/Beta);
                    }
                }
            }
        }

        public override double Pdf(double dblX)
        {
            if (dblX < 0)
            {
                throw new ArgumentException();
            }
            if (dblX == 0)
            {
                if (Alpha == 1.0)
                {
                    return 1.0/Beta;
                }
                else
                {
                    return 0.0;
                }
            }
            if (Alpha == 1.0)
            {
                return Math.Exp(-dblX/Beta)/Beta;
            }

            return Math.Exp((Alpha - 1.0)*Math.Log(dblX/Beta) - dblX/Beta -
                            LogGammaFunct.LogGamma(Alpha))/Beta;
        }

        /**
         *  Gamma(alpha,beta)
         *
         *@param  alpha  alpha
         *@param  beta   beta
         *@return        Gamma(alpha,beta)
         *@author:       <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public double NextRandom2()
        {
            if (Alpha < 1)
            {
                double b = (Math.E + Alpha)/Math.E;
                double y;
                double p;
                do
                {
                    do
                    {
                        double u1 = m_rng.NextDouble();
                        p = b*u1;
                        if (p > 1)
                        {
                            break;
                        }
                        y = Math.Pow(p, 1/Alpha);
                        double u2 = m_rng.NextDouble();
                        if (u2 <= Math.Exp(-y))
                        {
                            return Beta*y;
                        }
                    }
                    while (true);
                    y = -Math.Log((Beta - p)/Alpha);
                    double u2_ = m_rng.NextDouble();
                    if (u2_ <= Math.Pow(y, Alpha - 1))
                    {
                        return Beta*y;
                    }
                }
                while (true);
            }
            else
            {
                double a = 1/Math.Sqrt(2*Alpha - 1);
                double b = Alpha - Math.Log(4);
                double q = Alpha + 1/a;
                double theta = 4.5;
                double d = 1 + Math.Log(theta);
                do
                {
                    double u1 = m_rng.NextDouble();
                    double u2 = m_rng.NextDouble();
                    double v = a*Math.Log(u1/(1 - u2));
                    double y = Alpha*Math.Exp(v);
                    double z = u1*u1*u2;
                    double w = b + q*v + y;
                    if (w + d - theta*z >= 0 && w >= Math.Log(z))
                    {
                        return Beta*y;
                    }
                }
                while (true);
            }
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "GammaDist(" + Alpha + "," + Beta + ")";
        }

        public override double CdfInv(double dblProbability)
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
