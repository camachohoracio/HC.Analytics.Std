#region

using System;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class FDist : AbstractUnivContDist
    {
        #region Members

        private double m_dblDegreesOfFreedom1;
        private double m_dblDegreesOfFreedom2;

        #endregion

        #region Parameters

        public double DegreesOfFreedom1
        {
            get { return m_dblDegreesOfFreedom1; }

            set
            {
                m_dblDegreesOfFreedom1 = value;
                SetState(
                    m_dblDegreesOfFreedom1,
                    m_dblDegreesOfFreedom2);
            }
        }

        public double DegreesOfFreedom2
        {
            get { return m_dblDegreesOfFreedom2; }

            set
            {
                m_dblDegreesOfFreedom2 = value;
                SetState(
                    m_dblDegreesOfFreedom1,
                    m_dblDegreesOfFreedom2);
            }
        }

        #endregion

        #region Constructors

        public FDist(
            double dblDegreesOfFreedom1,
            double dblDegreesOfFreedom2,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblDegreesOfFreedom1,
                dblDegreesOfFreedom2);
        }

        #endregion

        private void SetState(
            double dblDegreesOfFreedom1,
            double dblDegreesOfFreedom2)
        {
            m_dblDegreesOfFreedom1 = dblDegreesOfFreedom1;
            m_dblDegreesOfFreedom2 = dblDegreesOfFreedom2;
        }

        public override double Cdf(double dblX)
        {
            return CdfStatic(
                m_dblDegreesOfFreedom1,
                m_dblDegreesOfFreedom2,
                dblX);
        }

        public static double CdfStatic(
            double dblDegreesOfFreedom1,
            double dblDegreesOfFreedom2,
            double dblX)
        {
            double dblXParameter =
                (dblDegreesOfFreedom1*dblX)/
                ((dblDegreesOfFreedom1*dblX) + dblDegreesOfFreedom2);
            double dblAlpha =
                dblDegreesOfFreedom1/2.0;
            double dblBeta =
                dblDegreesOfFreedom2/2.0;
            return IncompleteBetaFunct.IncompleteBeta(
                dblAlpha,
                dblBeta,
                dblXParameter);
        }

        public override double Pdf(double dblX)
        {
            return PdfStatic(
                m_dblDegreesOfFreedom1,
                m_dblDegreesOfFreedom2,
                dblX);
        }

        public double PdfStatic(
            double dblDegreesOfFreedom1,
            double dblDegreesOfFreedom2,
            double dblX)
        {
            if (dblX < 0.0)
            {
                throw new ArgumentException("Invalid variate-value.");
            }
            if (dblX == 0.0)
            {
                return 0.0;
            }
            else
            {
                double lnB = LogGammaFunct.LnB(0.5*dblDegreesOfFreedom2, 0.5*dblDegreesOfFreedom1);
                return Math.Exp(0.5*((dblDegreesOfFreedom1*
                                      (Math.Log(dblDegreesOfFreedom1) - Math.Log(dblDegreesOfFreedom2)) +
                                      (dblDegreesOfFreedom1 - 2D)*
                                      Math.Log(dblX)) - (dblDegreesOfFreedom1 + dblDegreesOfFreedom2)*
                                                        Math.Log(1.0 + (dblDegreesOfFreedom1/dblDegreesOfFreedom2)*dblX)) -
                                lnB);
            }
        }

        public override double CdfInv(double dblProbability)
        {
            return CdfInvStatic(
                m_dblDegreesOfFreedom1,
                m_dblDegreesOfFreedom2,
                dblProbability);
        }

        public static double CdfInvStatic(
            double dblDegreesOfFreedom1,
            double dblDegreesOfFreedom2,
            double dblProbability)
        {
            double result = 0.0D;

            if ((dblDegreesOfFreedom1 > 0.0D) && (dblDegreesOfFreedom2 > 0.0D))
            {
                if ((dblProbability >= 0.0D) && (dblProbability <= 1.0D))
                {
                    result =
                        IncompleteBetaFunct.InvIncompleteBeta(
                            dblDegreesOfFreedom1/2.0D,
                            dblDegreesOfFreedom2/2.0D,
                            1.0D - dblProbability);

                    if ((result >= 0.0D) && (result < 1.0D))
                    {
                        result =
                            result*dblDegreesOfFreedom2/
                            (dblDegreesOfFreedom1*(1.0D - result));
                    }
                    else
                    {
                        throw new ArithmeticException(
                            "inverse incomplete beta evaluation failed");
                    }
                }
                else
                {
                    throw new ArgumentException("p < 0 or p > 1");
                }
            }
            else
            {
                throw new ArgumentException("dfn or dfd <= 0");
            }

            return result;
        }

        public override double NextDouble()
        {
            ChiSquareDist chiSquared1 = new ChiSquareDist(
                m_dblDegreesOfFreedom1,
                m_rng);

            ChiSquareDist chiSquared2 = new ChiSquareDist(
                m_dblDegreesOfFreedom2,
                m_rng);

            return (m_dblDegreesOfFreedom2*chiSquared1.NextDouble())/
                   (m_dblDegreesOfFreedom1*chiSquared2.NextDouble());
        }
    }
}
