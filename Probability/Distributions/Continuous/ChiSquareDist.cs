#region

using System;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class ChiSquareDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        // cached vars for method NextDouble(a) (for performance only)
        private double m_dblB;
        private double m_dblFreedomIn = -1.0;
        private double m_dblV;
        private double m_dblVd;
        private double m_dblVm;
        private double m_dblVp;

        #endregion

        #region Constructors

        public ChiSquareDist(
            double dblV,
            RngWrapper rng)
            : base(rng)
        {
            SetState(dblV);
        }

        #endregion

        #region Parameters

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

        #region Constants

        private const int INT_RND_SEED = 6;

        #endregion

        #region Initialization

        private void SetState(
            double dblV)
        {
            m_dblV = dblV;
            if (V < 1.0)
            {
                throw new ArgumentException();
            }
        }

        #endregion

        #region Public

        /**
         * Returns the area under the left hand tail (from 0 to <tt>x</tt>)
         * of the Chi square probability density function with
         * <tt>v</tt> degrees of freedom.
         * <pre>
         *                                  inf.
         *                                    -
         *                        1          | |  v/2-1  -t/2
         *  P( x | v )   =   -----------     |   t      e     dt
         *                    v/2  -       | |
         *                   2    | (v/2)   -
         *                                   x
         * </pre>
         * where <tt>x</tt> is the Chi-square variable.
         * <p>
         * The incomplete gamma integral is used, according to the
         * formula
         * <p>
         * <tt>y = chiSquare( v, x ) = incompleteGamma( v/2.0, x/2.0 )</tt>.
         * <p>
         * The arguments must both be positive.
         *
         * @param v degrees of freedom.
         * @param x integration end point.
         */

        public override double Cdf(double dblX)
        {
            if (dblX < 0.0 || V < 1.0)
            {
                return 0.0;
            }

            return IncompleteGamaFunct.IncompleteGamma(V/2.0, dblX/2.0);
        }

        /**
         * Returns the area under the right hand tail (from <tt>x</tt> to
         * infinity) of the Chi square probability density function
         * with <tt>v</tt> degrees of freedom.
         * <pre>
         *                                  inf.
         *                                    -
         *                        1          | |  v/2-1  -t/2
         *  P( x | v )   =   -----------     |   t      e     dt
         *                    v/2  -       | |
         *                   2    | (v/2)   -
         *                                   x
         * </pre>
         * where <tt>x</tt> is the Chi-square variable.
         *
         * The incomplete gamma integral is used, according to the
         * formula
         *
         * <tt>y = chiSquareComplemented( v, x ) = incompleteGammaComplement( v/2.0, x/2.0 )</tt>.
         *
         *
         * The arguments must both be positive.
         *
         * @param v degrees of freedom.
         */

        public double CdfComp(double dblX)
        {
            if (dblX < 0.0 || V < 1.0)
            {
                return 0.0;
            }
            return IncompleteGamaFunct.IncompleteGammaComplement(V/2.0, dblX/2.0);
        }

        /**
        * Returns the probability distribution function.
        */

        public override double Pdf(double dblX)
        {
            if (dblX <= 0.0)
            {
                throw new ArgumentException();
            }

            double logGamma = LogGammaFunct.LogGamma(V/2.0);
            return Math.Exp((V/2.0 - 1.0)*Math.Log(dblX/2.0) - dblX/2.0 - logGamma)/2.0;
        }

        public override double NextDouble()
        {
            /******************************************************************
             *                                                                *
             *        Chi Distribution - Ratio of Uniforms  with shift        *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - chru samples a random number from the Chi       *
             *                distribution with parameter  a > 1.             *
             * REFERENCE :  - J.F. Monahan (1987): An algorithm for           *
             *                generating chi random variables, ACM Trans.     *
             *                Math. Software 13, 168-172.                     *
             * SUBPROGRAM : - anEngine  ... pointer to a (0,1)-Uniform        *
             *                engine                                          *
             *                                                                *
             * Implemented by R. Kremer, 1990                                 *
             ******************************************************************/

            double u, v, z, zz, r;

            //if( a < 1 )  return (-1.0); // Check for invalid input value

            if (V == 1.0)
            {
                for (;;)
                {
                    u = m_rng.NextDouble();
                    v = m_rng.NextDouble()*0.857763884960707;
                    z = v/u;
                    if (z < 0)
                    {
                        continue;
                    }
                    zz = z*z;
                    r = 2.5 - zz;
                    if (z < 0.0)
                    {
                        r = r + zz*z/(3.0*z);
                    }
                    if (u < r*0.3894003915)
                    {
                        return (z*z);
                    }
                    if (zz > (1.036961043/u + 1.4))
                    {
                        continue;
                    }
                    if (2.0*Math.Log(u) < (-zz*0.5))
                    {
                        return (z*z);
                    }
                }
            }
            else
            {
                if (V != m_dblFreedomIn)
                {
                    m_dblB = Math.Sqrt(V - 1.0);
                    m_dblVm = -0.6065306597*(1.0 - 0.25/(m_dblB*m_dblB + 1.0));
                    m_dblVm = (-m_dblB > m_dblVm) ? -m_dblB : m_dblVm;
                    m_dblVp = 0.6065306597*(0.7071067812 + m_dblB)/(0.5 + m_dblB);
                    m_dblVd = m_dblVp - m_dblVm;
                    m_dblFreedomIn = V;
                }
                for (;;)
                {
                    u = m_rng.NextDouble();
                    v = m_rng.NextDouble()*m_dblVd + m_dblVm;
                    z = v/u;
                    if (z < -m_dblB)
                    {
                        continue;
                    }
                    zz = z*z;
                    r = 2.5 - zz;
                    if (z < 0.0)
                    {
                        r = r + zz*z/(3.0*(z + m_dblB));
                    }
                    if (u < r*0.3894003915)
                    {
                        return ((z + m_dblB)*(z + m_dblB));
                    }
                    if (zz > (1.036961043/u + 1.4))
                    {
                        continue;
                    }
                    if (2.0*Math.Log(u) < (Math.Log(1.0 + z/m_dblB)*m_dblB*m_dblB - zz*0.5 - z*m_dblB))
                    {
                        return ((z + m_dblB)*(z + m_dblB));
                    }
                }
            }
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "ChiSquare(" + V + ")";
        }

        public override double CdfInv(double dblProbability)
        {
            double result = 0;
            double y = dblProbability;
            double v = V;
            HCException.AssertMustBeTrue(
                ((double)(y) >= (double)(0) && (double)(y) <= (double)(1)) && (double)(v) >= (double)(1), "Domain error in InvChiSquareDistribution");
            result = 2 * IncompleteGamaFunct.IncompleteGammaComplement(0.5 * v, y);
            return result;
        }

        #endregion
    }
}
