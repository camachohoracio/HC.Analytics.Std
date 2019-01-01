#region

using System;
using System.Text;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Distributions.Mixtures;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.MachineLearning.Em
{
    public class EmMixtureGaussian : IComparable<EmMixtureGaussian>
    {
        #region Properties

        public double Likelihood
        {
            get { return m_dblLikelihood; }
        }

        #endregion

        #region Memebers

        private readonly double[,] m_dblDataArr;
        private readonly int m_intProofConvergence;
        private readonly MixtureGaussian m_mixtureGaussian;
        private readonly RngWrapper m_rng;
        private double[,,] m_dblBestCovarianceArr;
        private double m_dblBestLikelihood = -Double.MaxValue;
        private double[,] m_dblBestMeanArr;
        private double[] m_dblBestP;
        private double m_dblConvergence;
        private double m_dblImprovement;
        private double m_dblLikelihood;
        private double m_dblNewLikelihood;
        private int m_intAcutalIteration;
        private int m_intNumIterations;

        #endregion

        #region Constructors

        public EmMixtureGaussian(
            int intNumGuassians,
            double[,] dblDataArray) :
                this(dblDataArray,
                     EmHelper.CreateRandomMixtureGaussian(
                         intNumGuassians,
                         dblDataArray))
        {
        }

        public EmMixtureGaussian(
            double[,] dblDataArray,
            MixtureGaussian mixtureGaussian) : this(
                dblDataArray,
                mixtureGaussian,
                EmConstants.EM_CONVERGENCE,
                EmConstants.EM_ITERATIONS,
                EmConstants.INT_EM_CONVERGENCE,
                new RngWrapper())
        {
        }

        public EmMixtureGaussian(
            double[,] dblDataArray,
            MixtureGaussian mixtureGaussian,
            double dblConvergence,
            int intNumIterations,
            int intProofConvergence,
            RngWrapper rng)
        {
            m_rng = rng;
            m_dblDataArr = dblDataArray;
            m_mixtureGaussian = mixtureGaussian;
            m_dblConvergence = dblConvergence;
            m_intNumIterations = intNumIterations;
            m_intProofConvergence = intProofConvergence;
            InitBestParameters();
        }

        #endregion

        #region IComparable<EmMixtureGaussian> Members

        public int CompareTo(EmMixtureGaussian compare)
        {
            double difference =
                GetLikelihood() - compare.GetLikelihood();
            if (difference < 0)
            {
                return 1;
            }
            if (difference > 0)
            {
                return -1;
            }
            return 0;
        }

        #endregion

        private void InitBestParameters()
        {
            int numGaussians_ = m_mixtureGaussian.GetNumGaussians();
            int numVariables_ = m_dblDataArr.GetLength(1);
            m_dblBestMeanArr = new double[numGaussians_,numVariables_];
            m_dblBestP = new double[numVariables_];
            m_dblBestCovarianceArr = new double[
                numGaussians_,
                numVariables_,
                numVariables_];
        }

        /// <summary>
        /// Expectation - Maximization algorithm
        /// </summary>
        public void RunEm()
        {
            m_intAcutalIteration = 0;
            int numSamples_ = m_dblDataArr.GetLength(0);
            int numGaussians_ = m_mixtureGaussian.GetNumGaussians();
            int numVariables_ = m_dblDataArr.GetLength(1);
            bool converge = false;

            // old parameters
            double[] p_ = m_mixtureGaussian.GetP();
            double[,] mean_ = new double[numGaussians_,numVariables_];
            double[,,] covariance_ = new double[numGaussians_,numVariables_,
                numVariables_];

            MultNormalDist[] multivariateGaussianArray = m_mixtureGaussian.
                GetMultivariateGaussianArray();
            EmHelper.GetMeanCovariance(
                mean_,
                covariance_,
                multivariateGaussianArray);

            // new parameters
            double[,] e = new double[numSamples_,numGaussians_];
            double[] e2 = new double[numGaussians_];
            double[,] newMean = new double[numGaussians_,numVariables_];
            double[,,] newCovariance = new double[numGaussians_,numVariables_,
                numVariables_];
            double[] newP = new double[numGaussians_];

            double actualLikelihood = Expectation(
                numGaussians_,
                numVariables_,
                numSamples_,
                e,
                e2,
                newMean,
                newCovariance,
                p_,
                mean_,
                multivariateGaussianArray);

            Evaluation(actualLikelihood, p_, mean_, covariance_);
            //PrintToScreen.WriteLine(ToString());
            int actialProofConvergence = 0;
            while (!converge)
            {
                m_intAcutalIteration++;
                Maximization(
                    numGaussians_,
                    numVariables_,
                    numSamples_,
                    newCovariance,
                    e2,
                    newMean,
                    newP);
                VerifyEmParams(numGaussians_, numVariables_, newCovariance,
                               newP);
                SetNewParameters(numGaussians_, newCovariance, newMean, newP,
                                 multivariateGaussianArray);

                // old parameters
                p_ = m_mixtureGaussian.GetP();
                EmHelper.GetMeanCovariance(
                    mean_,
                    covariance_,
                    multivariateGaussianArray);

                // new parameters
                e = new double[numSamples_,numGaussians_];
                e2 = new double[numGaussians_];
                newMean = new double[numGaussians_,numVariables_];
                newCovariance = new double[numGaussians_,numVariables_,
                    numVariables_];
                newP = new double[numGaussians_];
                actualLikelihood = Expectation(
                    numGaussians_,
                    numVariables_,
                    numSamples_,
                    e,
                    e2,
                    newMean,
                    newCovariance,
                    p_,
                    mean_,
                    multivariateGaussianArray);

                Evaluation(actualLikelihood, p_, mean_, covariance_);
                if (m_intAcutalIteration == m_intNumIterations)
                {
                    converge = true;
                }
                if (m_dblImprovement <= m_dblConvergence)
                {
                    actialProofConvergence++;
                    if (actialProofConvergence >= m_intProofConvergence ||
                        m_intAcutalIteration >= m_intNumIterations)
                    {
                        converge = true;
                    }
                }
                else
                {
                    actialProofConvergence = 0;
                }
                //PrintToScreen.WriteLine(ToString());
            }
            SetBestParameters(numGaussians_);

            //PrintToScreen.WriteLine(ToString());
        }

        private double Expectation(
            int numGaussians_,
            int numVariables_,
            int numSamples_,
            double[,] e,
            double[] e2,
            double[,] newMean,
            double[,,] newCovariance,
            double[] p_,
            double[,] mean_,
            MultNormalDist[] multivariateGaussianArray)
        {
            double actualLikelihood = 0.0;
            for (int sample = 0; sample < numSamples_; sample++)
            {
                double sum = 0;

                double[] actualData = EmHelper.GetOneDimensionArray(m_dblDataArr, sample);
                double[] actualE = EmHelper.GetOneDimensionArray(e, sample);
                for (int gaussian = 0; gaussian < numGaussians_; gaussian++)
                {
                    double z0 = multivariateGaussianArray[gaussian].Pdf(
                        actualData);
                    if (z0 < 0.0 || double.IsInfinity(z0))
                    {
                        //Debugger.Break();
                    }
                    /*if (Double.isInfinite(z0)) {
                      PrintToScreen.WriteLine("is infinity!!");
                      PrintToScreen.WriteLine(multivariateGaussianArray[gaussian].ToString());
                    }*/
                    if (z0 < 0 || Double.IsNaN(z0))
                    {
                        z0 = 0;
                    }
                    double z = p_[gaussian]*z0;
                    sum += z;

                    if (z < 0 || double.IsInfinity(z))
                    {
                        //Debugger.Break();
                    }
                    actualE[gaussian] = z;
                }
                if (sum == 0)
                {
                    sum = 1E-20;
                }
                actualLikelihood += Math.Log(sum);

                if (double.IsInfinity(actualLikelihood))
                {
                    //Debugger.Break();
                }

                for (int gaussian = 0; gaussian < numGaussians_; gaussian++)
                {
                    if (sum <= 0 || Double.IsNaN(sum))
                    {
                        actualE[gaussian] = 1.0/(numGaussians_);
                    }
                    else
                    {
                        actualE[gaussian] = actualE[gaussian]/sum;
                    }
                    for (int variable = 0; variable < numVariables_; variable++)
                    {
                        newMean[gaussian, variable] += actualData[variable]*
                                                       actualE[gaussian];
                        double dif = actualData[variable] - mean_[gaussian, variable];
                        for (int variable2 = variable; variable2 < numVariables_; variable2++)
                        {
                            newCovariance[gaussian, variable, variable2] += actualE[gaussian]*
                                                                            dif*
                                                                            (actualData[variable2] -
                                                                             mean_[gaussian, variable2]);
                        }
                    }
                    e2[gaussian] += actualE[gaussian];
                    if (e2[gaussian] < 0)
                    {
                        //Debugger.Break();
                    }
                }
            }

            return actualLikelihood;
        }

        private void Maximization(
            int numGaussians_,
            int numVariables_,
            int numSamples_,
            double[,,] newCovariance,
            double[] e2,
            double[,] newMean,
            double[] newP)
        {
            for (int gaussian = 0; gaussian < numGaussians_; gaussian++)
            {
                double e2Value = e2[gaussian];
                if (e2Value <= 0)
                {
                    e2Value = 0.00000001;
                }
                for (int variable = 0; variable < numVariables_; variable++)
                {
                    for (int variable2 = variable; variable2 < numVariables_; variable2++)
                    {
                        double newValue = newCovariance[gaussian, variable, variable2]/
                                          e2Value;
                        newCovariance[gaussian, variable, variable2] = newValue;
                        newCovariance[gaussian, variable2, variable] = newValue;
                    }
                    newMean[gaussian, variable] = newMean[gaussian, variable]/e2Value;
                }
                newP[gaussian] = e2Value/(numSamples_);
                if (newP[gaussian] > 1.0 || newP[gaussian] < 0.0)
                {
                    //Debugger.Break();
                }
            }
        }

        private void Evaluation(double actualLikelihood,
                                double[] p_,
                                double[,] mean_,
                                double[,,] covariance_)
        {
            m_dblNewLikelihood = actualLikelihood;
            m_dblImprovement = m_dblNewLikelihood - m_dblLikelihood;
            m_dblLikelihood = m_dblNewLikelihood;
            if (m_dblBestLikelihood < m_dblNewLikelihood)
            {
                m_dblBestLikelihood = m_dblNewLikelihood;
                m_dblBestP = p_;
                m_dblBestMeanArr = mean_;
                m_dblBestCovarianceArr = covariance_;
            }
        }

        private void SetNewParameters(int numGaussians_, double[,,] newCovariance,
                                      double[,] newMean, double[] newP,
                                      MultNormalDist[]
                                          multivariateGaussianArray)
        {
            for (int gaussian = 0; gaussian < numGaussians_; gaussian++)
            {
                multivariateGaussianArray[gaussian].MeanVector = new Vector(
                    EmHelper.GetOneDimensionArray(
                        newMean,
                        gaussian));
                multivariateGaussianArray[gaussian].CovMatrix = new MatrixClass(
                    EmHelper.GetTwoDimensionArray(
                        newCovariance,
                        gaussian));
            }
            m_mixtureGaussian.SetP(newP);
        }

        private void SetBestParameters(int numGaussians)
        {
            MultNormalDist[] multivariateGaussianArray = new MultNormalDist[
                numGaussians];
            for (int gaussian = 0; gaussian < numGaussians; gaussian++)
            {
                multivariateGaussianArray[gaussian] = new MultNormalDist(
                    EmHelper.GetOneDimensionArray(m_dblBestMeanArr, gaussian),
                    EmHelper.GetTwoDimensionArray(m_dblBestCovarianceArr, gaussian),
                    m_rng);
            }
            m_mixtureGaussian.SetMultivariateGaussianArray(multivariateGaussianArray);
            m_mixtureGaussian.SetP(m_dblBestP);
            m_dblNewLikelihood = m_dblBestLikelihood;
            m_dblLikelihood = m_dblBestLikelihood;
            m_dblImprovement = 0.0;
        }

        private void VerifyEmParams(int numGaussians, int numVariables,
                                    double[,,] newCovariance, double[] newP)
        {
            double sum = 0.0;
            for (int gaussian = 0; gaussian < numGaussians; gaussian++)
            {
                double pLowBound = (1.0/(numGaussians))*0.25;
                double pHighBound = (1.0/(numGaussians))*2.5;
                if (newP[gaussian] <= pLowBound)
                {
                    newP[gaussian] = pLowBound;
                }
                if (newP[gaussian] >= pHighBound)
                {
                    newP[gaussian] = newP[gaussian] - (pHighBound*0.2);
                }
                sum += newP[gaussian];
            }
            for (int gaussian = 0; gaussian < numGaussians; gaussian++)
            {
                newP[gaussian] = newP[gaussian]/sum;
            }
            // check covariances
            for (int gaussian = 0; gaussian < numGaussians; gaussian++)
            {
                for (int variable = 0; variable < numVariables; variable++)
                {
                    if (newCovariance[gaussian, variable, variable] < 0.01)
                    {
                        newCovariance[gaussian, variable, variable] = 0.01;
                        for (int i = 0; i < numVariables; i++)
                        {
                            if (i != variable)
                            {
                                newCovariance[gaussian, variable, i] = 0.0;
                                newCovariance[gaussian, i, variable] = 0.0;
                            }
                        }
                    }
                    for (int variable2 = 0; variable2 < numVariables; variable2++)
                    {
                        if (variable != variable2)
                        {
                            double diff = newCovariance[gaussian, variable, variable] -
                                          Math.Abs(newCovariance[gaussian, variable, variable2]);
                            if (diff <= 0.0001)
                            {
                                double sign = newCovariance[gaussian, variable, variable2]/
                                              Math.Abs(newCovariance[gaussian, variable, variable2]);
                                double newValue = sign*
                                                  newCovariance[gaussian, variable, variable]*
                                                  0.9;
                                newCovariance[gaussian, variable, variable2] = newValue;
                                newCovariance[gaussian, variable2, variable] = newValue;
                            }
                        }
                    }
                }
                //validateCholdc(newCovariance[gaussian]);
            }
        }

        private void ValidateCholdc(double[,] a)
        {
            double[] p = new double[a.GetLength(0)];
            int n = a.GetLength(0);
            if (a.GetLength(1) != n)
            {
                throw new HCException("a is not square matrix");
            }
            if (n != p.Length)
            {
                throw new HCException(" Matrix dimentions must agree");
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    double sum = a[i, j];
                    for (int k = i - 1; k >= 0; k--)
                    {
                        sum -= a[i, k]*a[j, k];
                    }
                    if (i == j)
                    {
                        if (sum <= 0)
                        {
                            double value = Math.Abs(sum)*2 + 0.0001;
                            a[i, j] += value;
                            sum += value;
                        }
                        p[i] = Math.Sqrt(sum);
                    }
                    else
                    {
                        a[j, i] = sum/p[i];
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            buff.Append(m_mixtureGaussian.ToString());
            buff.Append("  Likelihood " + m_dblNewLikelihood + " (It: " +
                        m_intAcutalIteration + "/" + m_intNumIterations + ")\n");
            buff.Append("  Improvement: " + m_dblImprovement + " (It: " +
                        m_intAcutalIteration + "/" + m_intNumIterations + ")\n");
            return buff.ToString();
        }

        public MixtureGaussian GetMixtureGaussian()
        {
            return m_mixtureGaussian;
        }

        public double GetLikelihood()
        {
            if (m_intAcutalIteration == 0)
            {
                int numSamples_ = m_dblDataArr.GetLength(0);
                int numGaussians_ = m_mixtureGaussian.GetNumGaussians();
                int numVariables_ = m_dblDataArr.GetLength(1);

                // old parameters
                double[] p_ = m_mixtureGaussian.GetP();
                double[,] mean_ = new double[numGaussians_,numVariables_];
                double[,,] covariance_ = new double[numGaussians_,numVariables_,
                    numVariables_];
                MultNormalDist[] multivariateGaussianArray = m_mixtureGaussian.
                    GetMultivariateGaussianArray();
                EmHelper.GetMeanCovariance(
                    mean_,
                    covariance_,
                    multivariateGaussianArray);

                // new parameters
                double[,] e = new double[numSamples_,numGaussians_];
                double[] e2 = new double[numGaussians_];
                double[,] newMean = new double[numGaussians_,numVariables_];
                double[,,] newCovariance = new double[numGaussians_,numVariables_,
                    numVariables_];
                return Expectation(numGaussians_, numVariables_,
                                   numSamples_, e, e2, newMean, newCovariance, p_, mean_,
                                   multivariateGaussianArray);
            }
            return m_dblLikelihood;
        }

        public void SetIterations(int numIterations)
        {
            m_intNumIterations = numIterations;
        }

        public void SetConvergence(double convergence)
        {
            m_dblConvergence = convergence;
        }
    }
}
