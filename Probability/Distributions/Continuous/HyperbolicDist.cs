#region

using System;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class HyperbolicDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly HyperbolicDist m_ownInstance = new HyperbolicDist(
            1, 1, 1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblAlpha;

        // cached values shared for generateHyperbolic(...)
        private double m_dblASetup;
        private double m_dblBeta;
        private double m_dblBSetup = -1.0;
        private double m_dblDelta;
        private double m_dblE;
        private double m_dblHl;
        private double m_dblHr;
        private double m_dblMiu;
        private double m_dblMmb1;
        private double m_dblMpa1;
        private double m_dblPdfConstant;
        private double m_dblPm;
        private double m_dblPmr;
        private double m_dblPr;
        private double m_dblS;
        private double m_dblSamb;
        private double m_dblU;
        private double m_dblV;
        private double m_dblX;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 11;

        #endregion

        #region Constructors

        public HyperbolicDist(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);
        }

        #endregion

        #region Paramters

        public double Alpha
        {
            get { return m_dblAlpha; }
            set
            {
                m_dblAlpha = value;
                SetState(
                    m_dblAlpha,
                    m_dblBeta,
                    m_dblDelta,
                    m_dblMiu);
            }
        }

        /// <summary>
        /// Asymmetry parameter
        /// </summary>
        public double Beta
        {
            get { return m_dblBeta; }
            set
            {
                m_dblBeta = value;
                SetState(
                    m_dblAlpha,
                    m_dblBeta,
                    m_dblDelta,
                    m_dblMiu);
            }
        }

        /// <summary>
        /// Schale parameter
        /// </summary>
        public double Delta
        {
            get { return m_dblDelta; }
            set
            {
                m_dblDelta = value;
                SetState(
                    m_dblAlpha,
                    m_dblBeta,
                    m_dblDelta,
                    m_dblMiu);
            }
        }

        /// <summary>
        /// location parameter
        /// </summary>
        public double Miu
        {
            get { return m_dblMiu; }
            set
            {
                m_dblMiu = value;
                SetState(
                    m_dblAlpha,
                    m_dblBeta,
                    m_dblDelta,
                    m_dblMiu);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu)
        {
            m_dblAlpha = dblAlpha;
            m_dblBeta = dblBeta;
            m_dblDelta = dblDelta;
            m_dblMiu = dblMiu;
            double dblGammaValue = Math.Sqrt(Alpha*Alpha - Beta*Beta);
            m_dblPdfConstant = dblGammaValue/(2.0*Alpha*Delta*BesselFunct.K1(Delta*dblGammaValue));
        }

        #endregion

        #region Public

        public override double Pdf(double dblX)
        {
            return m_dblPdfConstant*
                   Math.Exp(-Alpha*
                            (Math.Sqrt(Math.Pow(Delta, 2) +
                                       Math.Pow(dblX - Miu, 2)) +
                             Beta*(dblX - Miu)));
        }


        /**
        * Returns a hyperbolic distributed random number; bypasses the internal state.
        */

        public override double NextDouble()
        {
            /******************************************************************
             *                                                                *
             *        Hyperbolic Distribution - Non-Universal Rejection       *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION   : - hyplc.c samples a random number from the        *
             *                hyperbolic distribution with shape parameter a  *
             *                and b valid for a>0 and |b|<a using the         *
             *                non-universal rejection method for log-concave  *
             *                densities.                                      *
             * REFERENCE :  - L. Devroye (1986): Non-Uniform Random Variate   *
             *                Generation, Springer Verlag, New York.          *
             * SUBPROGRAM : - drand(seed) ... (0,1)-Uniform generator with    *
             *                unsigned long integer *seed.                    *
             *                                                                *
             ******************************************************************/
            double a = Alpha;
            double b = Beta;

            if ((m_dblASetup != a) || (m_dblBSetup != b))
            {
                // SET-UP
                double mpa, mmb, mode;
                double amb;
                double a_, b_, a_1, b_1; //, pl;
                double help_1, help_2;
                amb = a*a - b*b; // a^2 - b^2
                m_dblSamb = Math.Sqrt(amb); // -Log(f(mode))
                mode = b/m_dblSamb; // mode
                help_1 = a*Math.Sqrt(2.0*m_dblSamb + 1.0);
                help_2 = b*(m_dblSamb + 1.0);
                mpa = (help_2 + help_1)/amb; // fr^-1(Exp(-sqrt(a^2 - b^2) - 1.0))
                mmb = (help_2 - help_1)/amb; // fl^-1(Exp(-sqrt(a^2 - b^2) - 1.0))
                a_ = mpa - mode;
                b_ = -mmb + mode;
                m_dblHr = -1.0/(-a*mpa/Math.Sqrt(1.0 + mpa*mpa) + b);
                m_dblHl = 1.0/(-a*mmb/Math.Sqrt(1.0 + mmb*mmb) + b);
                a_1 = a_ - m_dblHr;
                b_1 = b_ - m_dblHl;
                m_dblMmb1 = mode - b_1; // lower border
                m_dblMpa1 = mode + a_1; // upper border

                m_dblS = (a_ + b_);
                m_dblPm = (a_1 + b_1)/m_dblS;
                m_dblPr = m_dblHr/m_dblS;
                m_dblPmr = m_dblPm + m_dblPr;

                m_dblASetup = a;
                m_dblBSetup = b;
            }

            // GENERATOR
            for (;;)
            {
                m_dblU = m_rng.NextDouble();
                m_dblV = m_rng.NextDouble();
                if (m_dblU <= m_dblPm)
                {
                    // Rejection with a uniform majorizing function
                    // over the body of the distribution
                    m_dblX = m_dblMmb1 + m_dblU*m_dblS;
                    if (Math.Log(m_dblV) <= (-a*Math.Sqrt(1.0 + m_dblX*m_dblX) + b*m_dblX + m_dblSamb))
                    {
                        break;
                    }
                }
                else
                {
                    if (m_dblU <= m_dblPmr)
                    {
                        // Rejection with an exponential envelope on the
                        // right side of the mode
                        m_dblE = -Math.Log((m_dblU - m_dblPm)/m_dblPr);
                        m_dblX = m_dblMpa1 + m_dblHr*m_dblE;
                        if ((Math.Log(m_dblV) - m_dblE) <= (-a*Math.Sqrt(1.0 + m_dblX*m_dblX) + b*m_dblX + m_dblSamb))
                        {
                            break;
                        }
                    }
                    else
                    {
                        // Rejection with an exponential envelope on the
                        // left side of the mode
                        m_dblE = Math.Log((m_dblU - m_dblPmr)/(1.0 - m_dblPmr));
                        m_dblX = m_dblMmb1 + m_dblHl*m_dblE;
                        if ((Math.Log(m_dblV) + m_dblE) <= (-a*Math.Sqrt(1.0 + m_dblX*m_dblX) + b*m_dblX + m_dblSamb))
                        {
                            break;
                        }
                    }
                }
            }
            return (m_dblX);
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "HyperbolicDist(" + Alpha + "," + Beta + ")";
        }

        public override double Cdf(double dblX)
        {
            throw new NotImplementedException();
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
            double dblDelta,
            double dblMiu,
            double dblX)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);
            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu,
            double dblX)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);
            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);
            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);
            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblAlpha,
            double dblBeta,
            double dblDelta,
            double dblMiu,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta,
                dblDelta,
                dblMiu);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
