#region

using System;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Random;
using HC.Core.SearchUtils;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class TStudentDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        //private static readonly TStudentDist m_ownInstance = new TStudentDist(
        //    1, new RngWrapper(INT_RND_SEED));

        // Note: this is not threadsafe
        private static readonly TStudentDist m_ownInstance = null;
        protected double m_dblTerm; // performance cache for pdf()
        /// <summary>
        /// Degrees of freedom
        /// </summary>
        private double m_dblV;
        private UnivNormalDistStd m_univNormalDistStd;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 19;

        #endregion

        #region Constructors

        public TStudentDist(
            double dblT,
            RngWrapper rng) : base(rng)
        {
            SetState(dblT);
        }

        #endregion

        #region Parameters

        /// <summary>
        /// Degrees of freedom
        /// </summary>
        public double V
        {
            get { return m_dblV; }
            set
            {
                m_dblV = value;
                SetState(m_dblV);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblV)
        {
            m_dblV = dblV;
            double dblVal = LogGammaFunct.LogGamma((V + 1)/2) - LogGammaFunct.LogGamma(V/2);
            m_dblTerm = Math.Exp(dblVal)/Math.Sqrt(Math.PI*V);
            m_univNormalDistStd =
                new UnivNormalDistStd(m_rng);
        }

        #endregion

        #region Public

        // The uniform random number generated shared by all <b>static</b> methods.
        /**
         * Returns a random number from the distribution; bypasses the internal state.
         * @param a degrees of freedom.
         * @throws ArgumentException if <tt>a &lt;= 0.0</tt>.
         */

        public override double NextDouble()
        {
            /*
             * The polar method of Box/Muller for generating Normal variates
             * is adapted to the Student-t distribution. The two generated
             * variates are not independent and the expected no. of uniforms
             * per variate is 2.5464.
             *
             * REFERENCE :  - R.W. Bailey (1994): Polar generation of random
             *                variates with the t-distribution, Mathematics
             *                of Computation 62, 779-781.
             */
            if (V <= 0.0)
            {
                throw new ArgumentException();
            }
            double u, v, w;

            do
            {
                u = 2.0*m_rng.NextDouble() - 1.0;
                v = 2.0*m_rng.NextDouble() - 1.0;
            }
            while ((w = u*u + v*v) > 1.0);

            return (u*Math.Sqrt(V*(Math.Exp(-2.0/V*Math.Log(w)) - 1.0)/w));
        }

        /**
         * Returns the probability distribution function.
         */

        public override double Pdf(double dblX)
        {
            return m_dblTerm*Math.Pow((1 + dblX*dblX/V), -(V + 1)*0.5);
        }

        public static double CdfStatic(
            double t,
            double dblV)
        {
            if (dblV <= 0)
            {
                throw new ArgumentException();
            }
            if (t == 0)
            {
                return (0.5);
            }

            double cdf = 0.5*IncompleteBetaFunct.IncompleteBeta(
                                 0.5*dblV, 0.5, dblV/(dblV + t*t));

            if (t >= 0)
            {
                cdf = 1.0 - cdf; // fixes bug reported by stefan.bentink@molgen.mpg.de
            }

            return cdf;
        }

        public override double Cdf(double dblX)
        {
            return CdfInvStatic(
                V,
                dblX);
        }

        /**
         * Returns the value, <tt>t</tt>, for which the area under the
         * Student-t probability density function (integrated from
         * minus infinity to <tt>t</tt>) is equal to <tt>1-alpha/2</tt>.
         * The value returned corresponds to usual Student t-distribution lookup
         * table for <tt>t<sub>alpha[size]</sub></tt>.
         * <p>
         * The function uses the studentT function to determine the return
         * value iteratively.
         *
         * @param alpha probability
         * @param size size of data set
         */

        public override double CdfInv(
            double dblCumProb)
        {
            return CdfInvStatic(
                V,
                dblCumProb);
        }


        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "TStudentDist(" + V + ")";
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblV,
            double dblX)
        {
            m_ownInstance.SetState(
                dblV);
            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfInvStatic(
            double dblV,
            double dblCumProb)
        {
            double f1, f2, f3;
            double x1, x2, x3;
            double g, s12;

            x1 = UnivNormalDistStd.CdfInvStatic(dblCumProb);

            // Return inverse of normal for large size
            if (dblV > 200)
            {
                return x1;
            }
            const double dblScale = 100;
            double dblMid = dblScale/2.0;
            return SearchUtilsClass.BinarySearch_(
                dblScale,
                0,
                -dblCumProb,
                dblVal =>
                    {
                        double dblCurr = -CdfStatic(
                            dblVal - dblMid,
                            dblV);
                        return dblCurr;
                    },
                1E-20,
                (int)1e5) - dblMid;
            

            // Find a pair of x1,x2 that braket zero
            f1 = CdfStatic(
                     x1,
                     dblV) - dblCumProb;
            x2 = x1;
            f2 = f1;
            do
            {
                if (f2 > 0)
                {
                    x2 = x2/2;
                }
                else
                {
                    x2 = x2 + x1;
                }
                f2 = CdfStatic(x2,
                               dblV) - dblCumProb;
            }
            while (f1*f2 > 0);

            // Find better approximation
            // Pegasus-method
            do
            {
                // Calculate slope of secant and t value for which it is 0.
                s12 = (f2 - f1)/(x2 - x1);
                x3 = x2 - f2/s12;

                // Calculate function value at x3
                f3 = CdfStatic(x3,
                               dblV) - dblCumProb;
                if (Math.Abs(f3) < 1e-8)
                {
                    // This criteria needs to be very tight!
                    // We found a perfect value -> return
                    return x3;
                }

                if (f3*f2 < 0)
                {
                    x1 = x2;
                    f1 = f2;
                    x2 = x3;
                    f2 = f3;
                }
                else
                {
                    g = f2/(f2 + f3);
                    f1 = g*f1;
                    x2 = x3;
                    f2 = f3;
                }
            }
            while (Math.Abs(x2 - x1) > 0.001);

            if (Math.Abs(f2) <= Math.Abs(f1))
            {
                return x2;
            }
            else
            {
                return x1;
            }
        }

        public static double NextDoubleStatic(
            double dblV)
        {
            m_ownInstance.SetState(
                dblV);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblV,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblV);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblV,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblV);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
