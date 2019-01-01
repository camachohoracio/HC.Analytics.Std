#region

using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Distributions.Mixtures;
using HC.Analytics.Probability.Random;

#endregion

//using HC.Analytics.Probability.Distributions;

namespace HC.Analytics.MachineLearning.Em
{
    public static class EmHelper
    {
        public static double[] GetOneDimensionArray(double[,] dblArray, int intIndex)
        {
            double[] dblOutArray = new double[dblArray.GetLength(1)];
            for (int i = 0; i < dblArray.GetLength(1); i++)
            {
                dblOutArray[i] = dblArray[intIndex, i];
            }
            return dblOutArray;
        }

        public static double[,] GetTwoDimensionArray(double[,,] dblArray, int intIndex)
        {
            double[,] dblOutArray = new double[
                dblArray.GetLength(1),
                dblArray.GetLength(2)];
            for (int i = 0; i < dblArray.GetLength(1); i++)
            {
                for (int j = 0; j < dblArray.GetLength(2); j++)
                {
                    dblOutArray[i, j] = dblArray[intIndex, i, j];
                }
            }
            return dblOutArray;
        }

        public static void GetMeanCovariance(
            double[,] mean_,
            double[,,] covariance_,
            MultNormalDist[] multivariateGaussianArray)
        {
            int numGaussians_ = multivariateGaussianArray.Length;
            int numVariables_ = mean_.GetLength(1);
            for (int gaussian = 0; gaussian < numGaussians_; gaussian++)
            {
                double[] dblCurrentMeanArray =
                    multivariateGaussianArray[gaussian].MeanVector.GetArr();
                double[,] dblCurrentCovarianceArray =
                    multivariateGaussianArray[gaussian].CovMatrix.GetArr();
                for (int i = 0; i < numVariables_; i++)
                {
                    mean_[gaussian, i] = dblCurrentMeanArray[i];
                    for (int j = 0; j < numVariables_; j++)
                    {
                        covariance_[gaussian, i, j] = dblCurrentCovarianceArray[i, j];
                    }
                }
            }
        }

        public static MixtureGaussian CreateRandomMixtureGaussian(
            int intNumGaussians,
            double[,] dblDataArr)
        {
            double[,] dblMinMaxArr = GetMaxMinArr(
                dblDataArr);
            int intNumVariables = dblDataArr.GetLength(1);

            return CreateRandomMixtureGaussian(
                intNumGaussians,
                intNumVariables,
                dblMinMaxArr,
                GetSigmaArr(
                    dblMinMaxArr,
                    intNumVariables));
        }

        public static double[,] GetMaxMinArr(
            double[,] dblDataArr)
        {
            int intNumVariables = dblDataArr.GetLength(1);
            double[,] dblMaxMinArray = new double[intNumVariables, 2];
            for (int j = 0; j < intNumVariables; j++)
            {
                dblMaxMinArray[j, 0] = double.MaxValue;
                dblMaxMinArray[j, 1] = -double.MaxValue;
            }

            for (int i = 0; i < dblDataArr.GetLength(0); i++)
            {
                for (int j = 0; j < intNumVariables; j++)
                {
                    double dblValue = dblDataArr[i,j];

                    if (dblValue < dblMaxMinArray[j, 0])
                    {
                        dblMaxMinArray[j, 0] = dblValue;
                    }
                    if (dblValue > dblMaxMinArray[j, 1])
                    {
                        dblMaxMinArray[j, 1] = dblValue;
                    }
                }
            }
            return dblMaxMinArray;
        }

        /// <summary>
        /// Generate a random mixture given the number of 
        /// gaussians to be included in the mix
        /// </summary>
        /// <param name="intNumGaussians"></param>
        /// <returns></returns>
        public static MixtureGaussian CreateRandomMixtureGaussian(
            int intNumGaussians,
            int intNumVariables,
            double[,] dblMaxMinArray,
            double[] dblSigmaArr)
        {
            RngWrapper rng = new RngWrapper();
            UnivNormalDistStd univNormalDistStd =
                new UnivNormalDistStd(rng);

            double[] p = new double[intNumGaussians];
            // randomly initialize p, mean vectors and covariance matrix
            MultNormalDist[] multivariateGaussianArray =
                new MultNormalDist[intNumGaussians];
            for (int gaussian = 0; gaussian < intNumGaussians; gaussian++)
            {
                p[gaussian] = 1.0 / intNumGaussians;
                double[] mean = new double[intNumVariables];
                double[,] covariance = new double[intNumVariables,
                    intNumVariables];

                for (int intCurrentVariable = 0; 
                    intCurrentVariable < intNumVariables; 
                    intCurrentVariable++)
                {
                    mean[intCurrentVariable] = dblMaxMinArray[intCurrentVariable, 1] * rng.NextDouble();
                    //PrintToScreen.WriteLine(mean[variable]);
                    covariance[intCurrentVariable, intCurrentVariable] = -1;
                    while (covariance[intCurrentVariable, intCurrentVariable] < 0)
                    {
                        covariance[intCurrentVariable, intCurrentVariable] =
                            (1.0 / ((intNumGaussians * 2))) +
                            (univNormalDistStd.NextDouble() * dblSigmaArr[intCurrentVariable]);
                    }
                }
                multivariateGaussianArray[gaussian] =
                    new MultNormalDist(
                        mean,
                        covariance,
                        rng);
            }
            return new MixtureGaussian(multivariateGaussianArray, dblMaxMinArray, p);
        }


        public static double[] GetSigmaArr(
            double[,] dblMaxMinArray,
            int intNumVariables)
        {
            double[] dblSigmaArr = new double[intNumVariables];

            for (int intVariableIndex = 0; intVariableIndex < intNumVariables; intVariableIndex++)
            {
                double dblDelta = dblMaxMinArray[intVariableIndex, 1] -
                    dblMaxMinArray[intVariableIndex, 0];
                double dblMaxJump = dblDelta / 2.0;
                double dblActualSigma = EmConstants.EM_SIGMA * (dblMaxJump / 1.5);
                dblSigmaArr[intVariableIndex] = dblActualSigma;
            }
            return dblSigmaArr;
        }
    }
}
