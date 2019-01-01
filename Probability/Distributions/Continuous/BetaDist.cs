#region

using System;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Mathematics.Functions.Beta;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    /**
 * Beta distribution; <A HREF="http://www.taglink.com.dataStructures.mixtureGaussian.cern.ch/RD11/rkb/AN16pp/node15.html#SECTION000150000000000000000"> math definition</A>
 * and <A HREF="http://www.statsoft.com/textbook/glosb.html#Beta Distribution"> animated definition</A>.
 * <p>
 * <tt>p(x) = k * x^(alpha-1) * (1-x)^(beta-1)</tt> with <tt>k = g(alpha+beta)/(g(alpha)*g(beta))</tt> and <tt>g(a)</tt> being the gamma function.
 * <p>
 * Valid parameter ranges: <tt>alpha &gt; 0</tt> and <tt>beta &gt; 0</tt>.
 * <p>
 * Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
 * <dt>
 * Static methods operate on a default uniform random number generator; they are synchronized.
 * <p>
 * <b>Implementation:</b>
 * <dt>Method: Stratified Rejection/Patchwork Rejection.
 * High performance implementation.
 * <dt>This is a port of <tt>bsprc.c</tt> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
 * C-RAND's implementation, in turn, is based upon
 * <p>
 * H. Sakasegawa (1983): Stratified rejection and squeeze method for generating beta random numbers,
 * Ann. Inst. Statist. Math. 35 B, 291-302.
 * <p>
 * and
 * <p>
 * Stadlober E., H. Zechner (1993), <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html"> Generating beta variates via patchwork rejection,</A>,
 * Computing 50, 1-18.
 *
 * @author wolfgang.hoschek@taglink.com.dataStructures.mixtureGaussian.cern.ch
 * @version 1.0, 09/24/99
 */

    /// <summary>
    /// Beta distribution class.
    /// Quick calculation of Beta probabilities.
    /// Beta values are stored in memory in order to speedup computation.
    /// The data is loaded via serialization when the classs is initialized.
    /// Note: This class is not threadsafe
    /// </summary>
    public class BetaDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly BetaDist m_ownInstance = new BetaDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblA;

        private double m_dblA_;
        private double m_dblA_last;

        /// <summary>
        /// Alpha paramter
        /// </summary>
        private double m_dblAlpha;

        private double m_dblB;

        private double m_dblB_;
        private double m_dblB_last;

        /// <summary>
        /// Beta parameter
        /// </summary>
        private double m_dblBeta;

        private double m_dblC;
        private double m_dblD;
        private double m_dblD1;
        private double m_dblDl;
        private double m_dblF2;
        private double m_dblF4;
        private double m_dblF5;

        private double m_dblFa;
        private double m_dblFb;
        private double m_dblLl;
        private double m_dblLr;
        private double m_dblM;

        // chached values for b01
        private double m_dblMl;
        private double m_dblMu;
        private double m_dblP1;
        private double m_dblP2;
        private double m_dblP3;
        private double m_dblP4;

        /// <summary>
        /// cache to speed up pdf()
        /// </summary>
        private double m_dblPdfConst;

        // chached values for b1prs
        private double m_dblPLast;
        private double m_dblQLast;
        private double m_dblS;
        private double m_dblT;
        private double m_dblX1;
        private double m_dblX2;
        private double m_dblX4;
        private double m_dblX5;
        private double m_dblZ2;
        private double m_dblZ4;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 1;

        #endregion

        #region Constructors

        public BetaDist(
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
            m_dblAlpha = dblAlpha;
            m_dblBeta = dblBeta;
            m_dblPdfConst = LogGammaFunct.LogGamma(Alpha + Beta) -
                            LogGammaFunct.LogGamma(Alpha) -
                            LogGammaFunct.LogGamma(Beta);
        }

        #endregion

        #region Public

        /// <summary>
        /// Calculates the beta distribution. This method is expensive.
        /// </summary>
        /// <param name="dblAlpha">
        /// Alpha parameter for beta distribution
        /// </param>
        /// <param name="dblBeta">
        /// Beta parameter for Beta distribution
        /// </param>
        /// <param name="dblCurrentLoss">
        /// Loss threshold
        /// </param>
        /// <returns>
        /// Beta distribution value
        /// </returns>
        /**
         * Returns the area from zero to <tt>x</tt> under the beta density
         * function.
         * <pre>
         *                          x
         *            -             -
         *           | (a+b)       | |  a-1      b-1
         * P(x)  =  ----------     |   t    (1-t)    dt
         *           -     -     | |
         *          | (a) | (b)   -
         *                         0
         * </pre>
         * This function is identical to the incomplete beta
         * integral function <tt>Gamma.incompleteBeta(a, b, x)</tt>.
         *
         * The complemented function is
         *
         * <tt>1 - P(1-x)  =  Gamma.incompleteBeta( b, a, x )</tt>;
         *
         */
        public override double Cdf(
            double dblX)
        {
            return IncompleteBetaFunct.IncompleteBeta(
                Alpha,
                Beta,
                dblX);
        }

        /**
         * Returns the area under the right hand tail (from <tt>x</tt> to
         * infinity) of the beta density function.
         *
         * This function is identical to the incomplete beta
         * integral function <tt>Gamma.incompleteBeta(b, a, x)</tt>.
         */

        public double CdfComplemented(
            double dblX)
        {
            return IncompleteBetaFunct.IncompleteBeta(
                Beta,
                Alpha,
                dblX);
        }

        public override double CdfInv(
            double dblProb)
        {
            return CdfInvStatic(
                Alpha,
                Beta,
                dblProb);
        }

        /**
         * Returns the PDF function.
         */

        public override double Pdf(double dblX)
        {
            if (dblX < 0 || dblX > 1)
            {
                return 0.0;
            }
            return Math.Exp(m_dblPdfConst)*
                   Math.Pow(dblX, Alpha - 1)*
                   Math.Pow(1 - dblX, Beta - 1);
        }


        /**
         * Returns a beta distributed random number; bypasses the internal state.
         */

        public override double NextDouble()
        {
            /******************************************************************
             *                                                                *
             * Beta Distribution - Stratified Rejection/Patchwork Rejection   *
             *                                                                *
             ******************************************************************
             * For parameters a < 1 , b < 1  and  a < 1 < b   or  b < 1 < a   *
             * the stratified rejection methods b00 and b01 of Sakasegawa are *
             * used. Both procedures employ suitable two-part power functions *
             * from which samples can be obtained by inversion.               *
             * If a > 1 , b > 1 (unimodal case) the patchwork rejection       *
             * method b1prs of Zechner/Stadlober is utilized:                 *
             * The area below the density function f(x) in its body is        *
             * rearranged by certain point reflections. Within a large center *
             * interval variates are sampled efficiently by rejection from    *
             * uniform hats. Rectangular immediate acceptance regions speed   *
             * up the generation. The remaining tails are covered by          *
             * exponential functions.                                         *
             * If (a-1)(b-1) = 0  sampling is done by inversion if either a   *
             * or b are not equal to one. If  a = b = 1  a uniform random     *
             * variate is delivered.                                          *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - bsprc samples a random variate from the beta    *
             *                distribution with parameters  a > 0, b > 0.     *
             * REFERENCES : - H. Sakasegawa (1983): Stratified rejection and  *
             *                squeeze method for generating beta random       *
             *                numbers, Ann. Inst. Statist. Math. 35 B,        *
             *                291-302.                                        *
             *              - H. Zechner, E. Stadlober (1993): Generating     *
             *                beta variates via patchwork rejection,          *
             *                Computing 50, 1-18.                             *
             *                                                                *
             * SUBPROGRAMS: - drand(seed) ... (0,1)-Uniform generator with    *
             *                unsigned long integer *seed.                    *
             *              - b00(seed,a,b) ... Beta generator for a<1, b<1   *
             *              - b01(seed,a,b) ... Beta generator for a<1<b or   *
             *                                  b<1<a                         *
             *              - b1prs(seed,a,b) ... Beta generator for a>1, b>1 *
             *                with unsigned long integer *seed, double a, b.  *
             *                                                                *
             ******************************************************************/
            double a = Alpha;
            double b = Beta;
            if (a > 1.0)
            {
                if (b > 1.0)
                {
                    return (b1prs(a, b));
                }
                if (b < 1.0)
                {
                    return (1.0 - b01(b, a));
                }
                if (b == 1.0)
                {
                    return (Math.Exp(Math.Log(m_rng.NextDouble())/a));
                }
            }

            if (a < 1.0)
            {
                if (b > 1.0)
                {
                    return (b01(a, b));
                }
                if (b < 1.0)
                {
                    return (b00(a, b));
                }
                if (b == 1.0)
                {
                    return (Math.Exp(Math.Log(m_rng.NextDouble())/a));
                }
            }

            if (a == 1.0)
            {
                if (b != 1.0)
                {
                    return (1.0 - Math.Exp(Math.Log(m_rng.NextDouble())/b));
                }
                if (b == 1.0)
                {
                    return (m_rng.NextDouble());
                }
            }

            return 0.0;
        }

        /**
         *  Be(alpha1, alpha2)
         *
         *@param  alpha1  alpha1
         *@param  alpha2  alpha2
         *@return         Be(alpha1, alpha2)
         *@author:        <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public double NextDouble2()
        {
            // to do : compare performance for this method

            GammaDist univariateGammaDistribution1 =
                new GammaDist(Alpha, 1, m_rng);
            GammaDist univariateGammaDistribution2 =
                new GammaDist(Alpha, 1, m_rng);

            if (Alpha == 1 && Beta == 1)
            {
                return m_rng.NextDouble();
            }
            double y1 = univariateGammaDistribution1.NextDouble();
            return y1/(y1 + univariateGammaDistribution2.NextDouble());
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "BetaDist(" + Alpha + "," + Beta + ")";
        }

        #endregion

        #region StaticMethods

        // beta distribution mean
        public static double betaMean(double alpha, double beta)
        {
            return betaMean(0.0D, 1.0D, alpha, beta);
        }

        // beta distribution mean
        public static double betaMean(double min, double max, double alpha, double beta)
        {
            if (alpha <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, alpha, " + alpha + "must be greater than zero");
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, beta, " + beta + "must be greater than zero");
            }
            return min + alpha*(max - min)/(alpha + beta);
        }

        // beta distribution mode
        public static double betaMode(double alpha, double beta)
        {
            return betaMode(0.0D, 1.0D, alpha, beta);
        }

        // beta distribution mode
        public static double betaMode(double min, double max, double alpha, double beta)
        {
            if (alpha <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, alpha, " + alpha + "must be greater than zero");
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, beta, " + beta + "must be greater than zero");
            }

            double mode = double.NaN;
            if (alpha > 1)
            {
                if (beta > 1)
                {
                    mode = min + (alpha + beta)*(max - min)/(alpha + beta - 2);
                }
                else
                {
                    mode = max;
                }
            }
            else
            {
                if (alpha == 1)
                {
                    if (beta > 1)
                    {
                        mode = min;
                    }
                    else
                    {
                        if (beta == 1)
                        {
                            mode = double.NaN;
                        }
                        else
                        {
                            mode = max;
                        }
                    }
                }
                else
                {
                    if (beta >= 1)
                    {
                        mode = min;
                    }
                    else
                    {
                        PrintToScreen.WriteLine("Class Stat; method betaMode; distribution is bimodal wirh modes at " +
                                                min + " and " + max);
                        PrintToScreen.WriteLine("NaN returned");
                    }
                }
            }
            return mode;
        }

        // beta distribution standard deviation
        public static double betaStandardDeviation(double alpha, double beta)
        {
            return betaStandDev(alpha, beta);
        }

        // beta distribution standard deviation
        public static double betaStandDev(double alpha, double beta)
        {
            return betaStandDev(0.0D, 1.0D, alpha, beta);
        }

        // beta distribution standard deviation
        public static double betaStandardDeviation(double min, double max, double alpha, double beta)
        {
            return betaStandDev(min, max, alpha, beta);
        }

        // beta distribution standard deviation
        public static double betaStandDev(double min, double max, double alpha, double beta)
        {
            if (alpha <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, alpha, " + alpha + "must be greater than zero");
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, beta, " + beta + "must be greater than zero");
            }
            return ((max - min)/(alpha + beta))*Math.Sqrt(alpha*beta/(alpha + beta + 1));
        }

        // beta distribution pdf
        public static double betaPDF(double min, double max, double alpha, double beta, double x)
        {
            if (alpha <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, alpha, " + alpha + "must be greater than zero");
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, beta, " + beta + "must be greater than zero");
            }
            if (x < min)
            {
                throw new ArgumentException("x, " + x + ", must be greater than or equal to the minimum value, " + min);
            }
            if (x > max)
            {
                throw new ArgumentException("x, " + x + ", must be less than or equal to the maximum value, " + max);
            }
            double pdf = Math.Pow(x - min, alpha - 1)*Math.Pow(max - x, beta - 1)/Math.Pow(max - min, alpha + beta - 1);
            return pdf/BetaFunct.betaFunction(alpha, beta);
        }

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
            double dblCdf =
                IncompleteBetaFunct.IncompleteBeta(
                    dblAlpha,
                    dblBeta,
                    dblX);
            //
            // validate small negative value
            //
            if (dblCdf < 0.0 && dblCdf > -1E-25)
            {
                dblCdf = 0.0;
            }
            return dblCdf;
        }

        // beta distribution pdf
        public static double betaCDF(double min, double max, double alpha, double beta, double limit)
        {
            if (alpha <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, alpha, " + alpha + "must be greater than zero");
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, beta, " + beta + "must be greater than zero");
            }
            if (limit < min)
            {
                throw new ArgumentException("limit, " + limit + ", must be greater than or equal to the minimum value, " +
                                            min);
            }
            if (limit > max)
            {
                throw new ArgumentException("limit, " + limit + ", must be less than or equal to the maximum value, " +
                                            max);
            }
            return RegularizedBetaFunction.regularisedBetaFunction(alpha, beta, (limit - min)/(max - min));
        }


        public static double CdfInvStatic(
            double dblAlpha,
            double dblBeta,
            double dblProbability)
        {
            // If there is a CV less than 10^-10 then we have an
            // effective delta, so just return the mean.
            if ((dblBeta/Math.Pow(dblAlpha + dblBeta, 2)) < Math.Pow(10, -10))
            {
                return (dblAlpha/(dblAlpha + dblBeta));
            }

            double inverseValue =
                IncompleteBetaFunct.InvIncompleteBeta(
                    dblAlpha,
                    dblBeta,
                    dblProbability);
            if (inverseValue < 10E-100)
            {
                inverseValue = 0.0;
            }
            return inverseValue;
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

        #region Private

        private double b00(
            double a,
            double b)
        {
            double U, V, X, Z;

            if (a != m_dblA_last || b != m_dblB_last)
            {
                m_dblA_last = a;
                m_dblB_last = b;

                m_dblA_ = a - 1.0;
                m_dblB_ = b - 1.0;
                m_dblC = (b*m_dblB_)/(a*m_dblA_); // b(1-b) / a(1-a)
                m_dblT = (m_dblC == 1.0) ? 0.5 : (1.0 - Math.Sqrt(m_dblC))/(1.0 - m_dblC); // t = t_opt
                m_dblFa = Math.Exp(m_dblA_*Math.Log(m_dblT));
                m_dblFb = Math.Exp(m_dblB_*Math.Log(1.0 - m_dblT)); // f(t) = fa * fb

                m_dblP1 = m_dblT/a; // 0 < X < t
                m_dblP2 = (1.0 - m_dblT)/b + m_dblP1; // t < X < 1
            }

            for (;;)
            {
                if ((U = m_rng.NextDouble()*m_dblP2) <= m_dblP1)
                {
                    //  X < t
                    Z = Math.Exp(Math.Log(U/m_dblP1)/a);
                    X = m_dblT*Z;
                    // squeeze accept:   L(x) = 1 + (1 - b)x
                    if ((V = m_rng.NextDouble()*m_dblFb) <= 1.0 - m_dblB_*X)
                    {
                        break;
                    }
                    // squeeze reject:   U(x) = 1 + ((1 - t)^(b-1) - 1)/t * x
                    if (V <= 1.0 + (m_dblFb - 1.0)*Z)
                    {
                        // quotient accept:  q(x) = (1 - x)^(b-1) / fb
                        if (Math.Log(V) <= m_dblB_*Math.Log(1.0 - X))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //  X > t
                    Z = Math.Exp(Math.Log((U - m_dblP1)/(m_dblP2 - m_dblP1))/b);
                    X = 1.0 - (1.0 - m_dblT)*Z;
                    // squeeze accept:   L(x) = 1 + (1 - a)(1 - x)
                    if ((V = m_rng.NextDouble()*m_dblFa) <= 1.0 - m_dblA_*(1.0 - X))
                    {
                        break;
                    }
                    // squeeze reject:   U(x) = 1 + (t^(a-1) - 1)/(1 - t) * (1 - x)
                    if (V <= 1.0 + (m_dblFa - 1.0)*Z)
                    {
                        // quotient accept:  q(x) = x^(a-1) / fa
                        if (Math.Log(V) <= m_dblA_*Math.Log(X))
                        {
                            break;
                        }
                    }
                }
            }
            return (X);
        }

        /**
         *
         */

        protected double b01(
            double a,
            double b)
        {
            double U, V, X, Z;

            if (a != m_dblA_last || b != m_dblB_last)
            {
                m_dblA_last = a;
                m_dblB_last = b;

                m_dblA_ = a - 1.0;
                m_dblB_ = b - 1.0;
                m_dblT = m_dblA_/(a - b); // one step Newton * start value t
                m_dblFb = Math.Exp((m_dblB_ - 1.0)*Math.Log(1.0 - m_dblT));
                m_dblFa = a - (a + m_dblB_)*m_dblT;
                m_dblT -= (m_dblT - (1.0 - m_dblFa)*(1.0 - m_dblT)*m_dblFb/b)/(1.0 - m_dblFa*m_dblFb);
                m_dblFa = Math.Exp(m_dblA_*Math.Log(m_dblT));
                m_dblFb = Math.Exp(m_dblB_*Math.Log(1.0 - m_dblT)); // f(t) = fa * fb
                if (m_dblB_ <= 1.0)
                {
                    m_dblMl = (1.0 - m_dblFb)/m_dblT; //   ml = -m1
                    m_dblMu = m_dblB_*m_dblT; //   mu = -m2 * t
                }
                else
                {
                    m_dblMl = m_dblB_;
                    m_dblMu = 1.0 - m_dblFb;
                }
                m_dblP1 = m_dblT/a; //  0 < X < t
                m_dblP2 = m_dblFb*(1.0 - m_dblT)/b + m_dblP1; //  t < X < 1
            }

            for (;;)
            {
                if ((U = m_rng.NextDouble()*m_dblP2) <= m_dblP1)
                {
                    //  X < t
                    Z = Math.Exp(Math.Log(U/m_dblP1)/a);
                    X = m_dblT*Z;
                    // squeeze accept:   L(x) = 1 + m1*x,  ml = -m1
                    if ((V = m_rng.NextDouble()) <= 1.0 - m_dblMl*X)
                    {
                        break;
                    }
                    // squeeze reject:   U(x) = 1 + m2*x,  mu = -m2 * t
                    if (V <= 1.0 - m_dblMu*Z)
                    {
                        // quotient accept:  q(x) = (1 - x)^(b-1)
                        if (Math.Log(V) <= m_dblB_*Math.Log(1.0 - X))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //  X > t
                    Z = Math.Exp(Math.Log((U - m_dblP1)/(m_dblP2 - m_dblP1))/b);
                    X = 1.0 - (1.0 - m_dblT)*Z;
                    // squeeze accept:   L(x) = 1 + (1 - a)(1 - x)
                    if ((V = m_rng.NextDouble()*m_dblFa) <= 1.0 - m_dblA_*(1.0 - X))
                    {
                        break;
                    }
                    // squeeze reject:   U(x) = 1 + (t^(a-1) - 1)/(1 - t) * (1 - x)
                    if (V <= 1.0 + (m_dblFa - 1.0)*Z)
                    {
                        // quotient accept:  q(x) = (x)^(a-1) / fa
                        if (Math.Log(V) <= m_dblA_*Math.Log(X))
                        {
                            break;
                        }
                    }
                }
            }
            return (X);
        }

        /**
         *
         */

        protected double b1prs(
            double p,
            double q)
        {
            double U, V, W, X, Y;

            if (p != m_dblPLast || q != m_dblQLast)
            {
                m_dblPLast = p;
                m_dblQLast = q;

                m_dblA = p - 1.0;
                m_dblB = q - 1.0;
                m_dblS = m_dblA + m_dblB;
                m_dblM = m_dblA/m_dblS;
                if (m_dblA > 1.0 || m_dblB > 1.0)
                {
                    m_dblD = Math.Sqrt(m_dblM*(1.0 - m_dblM)/(m_dblS - 1.0));
                }

                if (m_dblA <= 1.0)
                {
                    m_dblX2 = (m_dblDl = m_dblM*0.5);
                    m_dblX1 = m_dblZ2 = 0.0;
                    m_dblD1 = m_dblLl = 0.0;
                }
                else
                {
                    m_dblX2 = m_dblM - m_dblD;
                    m_dblX1 = m_dblX2 - m_dblD;
                    m_dblZ2 = m_dblX2*(1.0 - (1.0 - m_dblX2)/(m_dblS*m_dblD));
                    if (m_dblX1 <= 0.0 || (m_dblS - 6.0)*m_dblX2 - m_dblA + 3.0 > 0.0)
                    {
                        m_dblX1 = m_dblZ2;
                        m_dblX2 = (m_dblX1 + m_dblM)*0.5;
                        m_dblDl = m_dblM - m_dblX2;
                    }
                    else
                    {
                        m_dblDl = m_dblD;
                    }
                    m_dblD1 = f(m_dblX1, m_dblA, m_dblB, m_dblM);
                    m_dblLl = m_dblX1*(1.0 - m_dblX1)/(m_dblS*(m_dblM - m_dblX1)); // z1 = x1 - ll
                }
                m_dblF2 = f(m_dblX2, m_dblA, m_dblB, m_dblM);

                if (m_dblB <= 1.0)
                {
                    m_dblX4 = 1.0 - (m_dblD = (1.0 - m_dblM)*0.5);
                    m_dblX5 = m_dblZ4 = 1.0;
                    m_dblF5 = m_dblLr = 0.0;
                }
                else
                {
                    m_dblX4 = m_dblM + m_dblD;
                    m_dblX5 = m_dblX4 + m_dblD;
                    m_dblZ4 = m_dblX4*(1.0 + (1.0 - m_dblX4)/(m_dblS*m_dblD));
                    if (m_dblX5 >= 1.0 || (m_dblS - 6.0)*m_dblX4 - m_dblA + 3.0 < 0.0)
                    {
                        m_dblX5 = m_dblZ4;
                        m_dblX4 = (m_dblM + m_dblX5)*0.5;
                        m_dblD = m_dblX4 - m_dblM;
                    }
                    m_dblF5 = f(m_dblX5, m_dblA, m_dblB, m_dblM);
                    m_dblLr = m_dblX5*(1.0 - m_dblX5)/(m_dblS*(m_dblX5 - m_dblM)); // z5 = x5 + lr
                }
                m_dblF4 = f(m_dblX4, m_dblA, m_dblB, m_dblM);

                m_dblP1 = m_dblF2*(m_dblDl + m_dblDl); //  x1 < X < m
                m_dblP2 = m_dblF4*(m_dblD + m_dblD) + m_dblP1; //  m  < X < x5
                m_dblP3 = m_dblD1*m_dblLl + m_dblP2; //       X < x1
                m_dblP4 = m_dblF5*m_dblLr + m_dblP3; //  x5 < X
            }

            for (;;)
            {
                if ((U = m_rng.NextDouble()*m_dblP4) <= m_dblP1)
                {
                    // immediate accept:  x2 < X < m, - f(x2) < W < 0
                    if ((W = U/m_dblDl - m_dblF2) <= 0.0)
                    {
                        return (m_dblM - U/m_dblF2);
                    }
                    // immediate accept:  x1 < X < x2, 0 < W < f(x1)
                    if (W <= m_dblD1)
                    {
                        return (m_dblX2 - W/m_dblD1*m_dblDl);
                    }
                    // candidates for acceptance-rejection-test
                    V = m_dblDl*(U = m_rng.NextDouble());
                    X = m_dblX2 - V;
                    Y = m_dblX2 + V;
                    // squeeze accept:    L(x) = f(x2) (x - z2) / (x2 - z2)
                    if (W*(m_dblX2 - m_dblZ2) <= m_dblF2*(X - m_dblZ2))
                    {
                        return (X);
                    }
                    if ((V = m_dblF2 + m_dblF2 - W) < 1.0)
                    {
                        // squeeze accept:    L(x) = f(x2) + (1 - f(x2))(x - x2)/(m - x2)
                        if (V <= m_dblF2 + (1.0 - m_dblF2)*U)
                        {
                            return (Y);
                        }
                        // quotient accept:   x2 < Y < m,   W >= 2f2 - f(Y)
                        if (V <= f(Y, m_dblA, m_dblB, m_dblM))
                        {
                            return (Y);
                        }
                    }
                }
                else if (U <= m_dblP2)
                {
                    U -= m_dblP1;
                    // immediate accept:  m < X < x4, - f(x4) < W < 0
                    if ((W = U/m_dblD - m_dblF4) <= 0.0)
                    {
                        return (m_dblM + U/m_dblF4);
                    }
                    // immediate accept:  x4 < X < x5, 0 < W < f(x5)
                    if (W <= m_dblF5)
                    {
                        return (m_dblX4 + W/m_dblF5*m_dblD);
                    }
                    // candidates for acceptance-rejection-test
                    V = m_dblD*(U = m_rng.NextDouble());
                    X = m_dblX4 + V;
                    Y = m_dblX4 - V;
                    // squeeze accept:    L(x) = f(x4) (z4 - x) / (z4 - x4)
                    if (W*(m_dblZ4 - m_dblX4) <= m_dblF4*(m_dblZ4 - X))
                    {
                        return (X);
                    }
                    if ((V = m_dblF4 + m_dblF4 - W) < 1.0)
                    {
                        // squeeze accept:    L(x) = f(x4) + (1 - f(x4))(x4 - x)/(x4 - m)
                        if (V <= m_dblF4 + (1.0 - m_dblF4)*U)
                        {
                            return (Y);
                        }
                        // quotient accept:   m < Y < x4,   W >= 2f4 - f(Y)
                        if (V <= f(Y, m_dblA, m_dblB, m_dblM))
                        {
                            return (Y);
                        }
                    }
                }
                else if (U <= m_dblP3)
                {
                    // X < x1
                    Y = Math.Log(U = (U - m_dblP2)/(m_dblP3 - m_dblP2));
                    if ((X = m_dblX1 + m_dblLl*Y) <= 0.0)
                    {
                        continue; // X > 0!!
                    }
                    W = m_rng.NextDouble()*U;
                    // squeeze accept:    L(x) = f(x1) (x - z1) / (x1 - z1)
                    //                    z1 = x1 - ll,   W <= 1 + (X - x1)/ll
                    if (W <= 1.0 + Y)
                    {
                        return (X);
                    }
                    W *= m_dblD1;
                }
                else
                {
                    // x5 < X
                    Y = Math.Log(U = (U - m_dblP3)/(m_dblP4 - m_dblP3));
                    if ((X = m_dblX5 - m_dblLr*Y) >= 1.0)
                    {
                        continue; // X < 1!!
                    }
                    W = m_rng.NextDouble()*U;
                    // squeeze accept:    L(x) = f(x5) (z5 - x) / (z5 - x5)
                    //                    z5 = x5 + lr,   W <= 1 + (x5 - X)/lr
                    if (W <= 1.0 + Y)
                    {
                        return (X);
                    }
                    W *= m_dblF5;
                }
                // density accept:  f(x) = (x/m)^a ((1 - x)/(1 - m))^b
                if (Math.Log(W) <= m_dblA*Math.Log(X/m_dblM) + m_dblB*Math.Log((1.0 - X)/(1.0 - m_dblM)))
                {
                    return (X);
                }
            }
        }

        private double f(double x, double a, double b, double m)
        {
            return Math.Exp(a*Math.Log(x/m) + b*Math.Log((1.0 - x)/(1.0 - m)));
        }

        #endregion
    }
}
