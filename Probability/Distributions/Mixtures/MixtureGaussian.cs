#region

using System;
using System.Text;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Optimisation.Continuous;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Probability.Distributions.Mixtures
{
    public class MixtureGaussian
    {
        #region Members

        private readonly double[,] m_dblMaxMinArray;
        private readonly int m_numGaussians;
        private readonly int m_numVariables;
        private readonly RngWrapper m_rng;
        private double[] m_dblHighArr;
        private double[] m_dblLowArr;
        private double m_dblMixtureHigh = -1;
        private double m_dblNormalizerInv = -1;
        private double[] m_dblPArr;
        private MultNormalDist[] m_multivariateGaussianArray;

        #endregion

        public MixtureGaussian(
            int numGaussians,
            double[,] maxMin,
            RngWrapper rng)
        {
            m_numGaussians = numGaussians;
            m_numVariables = maxMin.GetLength(0);
            m_dblMaxMinArray = maxMin;
            m_rng = rng;
            GetLowHighBound();
            GuessParameters();
        }

        public MixtureGaussian(
            MultNormalDist[] multivariateGaussianArray,
            double[,] maxMin,
            double[] p)
        {
            m_multivariateGaussianArray = multivariateGaussianArray;
            m_numGaussians = multivariateGaussianArray.Length;
            m_numVariables = multivariateGaussianArray[0].VariableCount;
            m_dblMaxMinArray = maxMin;
            m_dblPArr = p;
            GetLowHighBound();
        }

        private void GuessParameters()
        {
            m_multivariateGaussianArray = new MultNormalDist[m_numGaussians];
            // distribute parameters along the search space
            double[] delta = new double[m_numVariables];
            for (int variable = 0; variable < m_numVariables; variable++)
            {
                delta[variable] = (m_dblMaxMinArray[variable, 1] - m_dblMaxMinArray[variable, 0])/
                                  ((m_numGaussians + 1));
            }

            // initialize p, mean vectors and covariance matrix
            double p_ = 1.0/(m_numGaussians);
            m_dblPArr = new double[m_numGaussians];
            for (int gaussian = 0; gaussian < m_numGaussians; gaussian++)
            {
                m_dblPArr[gaussian] = p_;
                double[,] covariance_ = new double[m_numVariables,m_numVariables];
                double[] mean_ = new double[m_numVariables];
                for (int variable = 0; variable < m_numVariables; variable++)
                {
                    mean_[variable] = delta[variable];
                    delta[variable] += (m_dblMaxMinArray[variable, 1] - m_dblMaxMinArray[variable, 0])/
                                       ((m_numGaussians + 1));
                    covariance_[variable, variable] = 1.0/
                                                      ((m_numGaussians*2));
                }
                m_multivariateGaussianArray[gaussian] = new MultNormalDist(
                    mean_,
                    covariance_,
                    m_rng);
            }
        }

        public int GetNumGaussians()
        {
            return m_numGaussians;
        }

        public int GetNumVariables()
        {
            return m_numVariables;
        }

        private void GetLowHighBound()
        {
            m_dblLowArr = new double[m_numVariables];
            m_dblHighArr = new double[m_numVariables];
            for (int variable = 0; variable < m_numVariables; variable++)
            {
                m_dblLowArr[variable] = m_dblMaxMinArray[variable, 0];
                m_dblHighArr[variable] = m_dblMaxMinArray[variable, 1];
            }
        }

        public void SetLow(double[] low)
        {
            m_dblLowArr = low;
        }

        public double Cdf(double[] x)
        {
            double[] x_ = new double[m_numVariables], low_ = new double[m_numVariables];
            double prob = 0.0;
            for (int gaussian = 0; gaussian < m_numGaussians; gaussian++)
            {
                double[] mean_ = m_multivariateGaussianArray[gaussian].MeanVector.GetArr();
                for (int variable = 0; variable < m_numVariables; variable++)
                {
                    x_[variable] = x[variable] - mean_[variable];
                    low_[variable] = m_dblLowArr[variable] - mean_[variable];
                }
                prob += m_dblPArr[gaussian]*m_multivariateGaussianArray[gaussian].Cdf(low_, x_);
            }
            return prob;
        }

        public double CdfNormalized(double[] x)
        {
            return Cdf(x)/Cdf(m_dblHighArr);
        }

        public double CdfInv(double[] x)
        {
            if (m_dblMixtureHigh == -1)
            {
                GetMixtureHigh();
                m_dblNormalizerInv = CdfInv(m_dblHighArr);
            }
            double gaussianVolume = Cdf(x);
            double squareVolume = m_dblMixtureHigh;
            for (int i = 0; i < m_numVariables; i++)
            {
                squareVolume *= (x[i] - m_dblLowArr[i]);
            }
            return squareVolume - gaussianVolume;
        }

        public double CdfInvNormalized(double[] x)
        {
            return CdfInv(x)/m_dblNormalizerInv;
        }

        public void SetMultivariateGaussianArray(MultNormalDist[]
                                                     multivariateGaussianArray)
        {
            m_multivariateGaussianArray = multivariateGaussianArray;
        }

        public MultNormalDist[] GetMultivariateGaussianArray()
        {
            return m_multivariateGaussianArray;
        }

        public override string ToString()
        {
            StringBuilder strOut = new StringBuilder();
            strOut.Append(
                "\n*********************** Mixture paramaters ***********************\n");
            strOut.Append("  Gaussians: " + m_numGaussians + Environment.NewLine);
            for (int gaussian = 0; gaussian < m_numGaussians; gaussian++)
            {
                double[] mean_ = m_multivariateGaussianArray[gaussian].MeanVector.GetArr();
                double[,] covariance_ = m_multivariateGaussianArray[gaussian].CovMatrix.GetArr();
                strOut.Append("  Gaussian_" + (gaussian + 1) + Environment.NewLine);
                strOut.Append("\tmean=\n\t\t[");
                for (int variable = 0; variable < m_numVariables; variable++)
                {
                    strOut.Append(mean_[variable] + ", ");
                }
                strOut.Append("]\n");

                strOut.Append("\tcovariance=\n");
                for (int variable1 = 0; variable1 < m_numVariables; variable1++)
                {
                    for (int variable2 = 0; variable2 < m_numVariables; variable2++)
                    {
                        strOut.Append("\t\t|" + covariance_[variable1, variable2] +
                                      ", ");
                    }
                    strOut.Append("|\n");
                }
                strOut.Append("\tp=\t" + m_dblPArr[gaussian] + Environment.NewLine);
            }
            return strOut.ToString();
        }

        public double[] GetP()
        {
            return m_dblPArr;
        }

        public double GetMixtureHigh(double[] x)
        {
            int iteration = 0, numIterations = 200;
            double f = -Pdf(x), eps = Double.MinValue, xtol = Double.MinValue;
            bool diagco = false;
            double[] g = GetGradient(x), diag = new double[m_numVariables];
            int[] iprint = {
                               0, 0
                           },
                  iflag = {
                              0
                          };
            do
            {
                try
                {
                    Lbfgs.lbfgs(
                        m_numVariables,
                        7,
                        x,
                        f,
                        g,
                        diagco,
                        diag,
                        iprint,
                        eps,
                        xtol,
                        iflag);
                    PrintToScreen.WriteLine("solution at it: " + iteration + "\t(" + x[0] + "\t" +
                                            x[1] + ")");
                }
                catch
                {
                }
                // function
                f = -Pdf(x);
                // gradient
                g = GetGradient(x);
                iteration++;
            } while (iflag[0] != 0 && iteration < numIterations && iflag[0] > 0);

            return -f;
        }

        public double GetMixtureHigh()
        {
            double maxZ = -Double.MaxValue;
            for (int gaussian = 0; gaussian < m_numGaussians; gaussian++)
            {
                double[] x = (double[]) m_multivariateGaussianArray[gaussian].MeanVector.GetArr().
                                            Clone();
                double z = GetMixtureHigh(x);
                if (z > maxZ)
                {
                    maxZ = z;
                }
            }
            m_dblMixtureHigh = maxZ;
            return m_dblMixtureHigh;
        }

        public double Pdf(double[] dblXArray)
        {
            double dblSum = 0;
            for (int gaussian = 0; gaussian < m_numGaussians; gaussian++)
            {
                dblSum += m_dblPArr[gaussian]*
                          m_multivariateGaussianArray[gaussian].Pdf(dblXArray);
            }
            return dblSum;
        }

        public double[] GetGradient(double[] x_)
        {
            double[] z0 = new double[m_numVariables];
            for (int gaussian = 0; gaussian < m_numGaussians; gaussian++)
            {
                double p_ = m_dblPArr[gaussian];
                double[] g = GetGradient(
                    new Vector(x_),
                    m_multivariateGaussianArray[gaussian]);
                for (int variable = 0; variable < m_numVariables; variable++)
                {
                    z0[variable] += p_*g[variable];
                }
            }
            return z0;
        }

        public double[] GetGradient(
            Vector x,
            MultNormalDist normalDist)
        {
            Vector res = x.Minus(normalDist.MeanVector);
            Vector g = res.Times(normalDist.InvCovMatrix);
            double dblPdf = normalDist.Pdf(x);
            return g.Times(dblPdf).GetArr();
        }

        public void SetP(double[] p)
        {
            m_dblPArr = p;
        }
    }
}
