#region

using System;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Distributions.Continuous;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Copulas
{
    public class TCopula
    {
        #region Memebers

        private readonly double[,] m_dblData;
        private readonly int m_intVariables;
        private readonly RngWrapper m_rng;
        private readonly TStudentDist m_tStudentDist;
        private MatrixClass m_covarianceMatrix;
        private int m_intDegreesOfFreedom;

        #endregion

        public TCopula(double[,] dblData)
        {
            m_dblData = dblData;
            m_intVariables = m_dblData.GetLength(1);
            m_rng = new RngWrapper();
            m_tStudentDist = new TStudentDist(0, m_rng);
            EstimateParameters();
        }

        private void InitializeCopulaParameters()
        {
            // set degrees of freedom
            m_intDegreesOfFreedom = 4;
            // initialize covariance matrix
            m_covarianceMatrix = new MatrixClass(m_intVariables, m_intVariables);
            for (int i = 0; i < m_intVariables; i++)
            {
                for (int j = i; j < m_intVariables; j++)
                {
                    if (i == j)
                    {
                        m_covarianceMatrix.Set(i, j, 1);
                    }
                    else
                    {
                        m_covarianceMatrix.Set(i, j, m_rng.NextDouble());
                        m_covarianceMatrix.Set(j, i, m_covarianceMatrix.Get(i, j));
                    }
                }
            }
        }

        private void EstimateParameters()
        {
            InitializeCopulaParameters();
        }


        public double EvaluateCopula(double[] dblUArray)
        {
            double[] dblYArray = new double[m_intVariables];

            for (int i = 0; i < m_intVariables; i++)
            {
                m_tStudentDist.V = m_intDegreesOfFreedom;
                dblYArray[i] = m_tStudentDist.Pdf(
                    dblUArray[i]);
            }

            MatrixClass yMatrix = new MatrixClass(dblYArray, m_intVariables);
            return DensityFunction(yMatrix);
        }

        public double DensityFunction(MatrixClass yMatrix)
        {
            double dblFirstPart =
                GammaFunct.Gamma(((m_intDegreesOfFreedom + m_intVariables))/2.0)*
                Math.Pow(GammaFunct.Gamma((m_intDegreesOfFreedom)/2.0), m_intVariables - 1)/
                (Math.Pow(GammaFunct.Gamma(((m_intDegreesOfFreedom + 1))/2.0), m_intVariables)*
                 Math.Sqrt(m_covarianceMatrix.Determinant()));

            double dblSecondPart = 1.0;
            for (int i = 0; i < m_intVariables; i++)
            {
                dblSecondPart *= Math.Pow(1.0 + Math.Pow(yMatrix.Get(i, 0), 2)/m_intDegreesOfFreedom,
                                          ((m_intDegreesOfFreedom + 1))/2.0);
            }

            double dblMatrixValue = yMatrix.Transpose().Times(
                m_covarianceMatrix.Inverse()).Times(yMatrix).Get(0, 0);
            double dblThirdPart = Math.Pow(1.0 + dblMatrixValue/m_intDegreesOfFreedom,
                                           -((double) (m_intDegreesOfFreedom + m_intVariables))/2.0);
            double dblCoupulaValue = dblFirstPart*dblSecondPart*dblThirdPart;
            return dblCoupulaValue;
        }
    }
}
