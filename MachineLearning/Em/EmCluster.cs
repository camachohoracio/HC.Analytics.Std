#region

using System;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Distributions.Mixtures;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.MachineLearning.Em
{
    public class EmCluster
    {
        #region Properties

        public double Likelihood
        {
            get { return m_emMixtureGaussian.Likelihood; }
        }

        #endregion

        #region Members

        private const double NORMALISER_FACTOR = 1000000000.0;
        private double[] m_dblWeightArr;
        private MixtureGaussian m_trainedMixtureGaussian;
        private EmMixtureGaussian m_emMixtureGaussian;

        #endregion

        #region Constructor

        public EmCluster()
        { 

        }

        #endregion

        #region Public Methods

        public void DoCluster(
            int numGaussians,
            double[,] data)
        {
            data = (double[,])data.Clone();

            //NormaliseData(data);

            int columnCount = data.GetLength(1);

            double[,] maxMin = EmHelper.GetMaxMinArr(data);

            //double[,] maxMin = new double[columnCount,2];
            //for (int column = 0; column < columnCount; column++)
            //{
            //    maxMin[column, 0] = 0.0;
            //    maxMin[column, 1] = 1.0;
            //}

            MixtureGaussian mixtureGaussian =
                InitializeMixtureGaussian(
                    numGaussians, 
                    columnCount, 
                    maxMin);

            m_emMixtureGaussian =
                new EmMixtureGaussian(
                    data,
                    mixtureGaussian);

            m_emMixtureGaussian.RunEm();

            m_trainedMixtureGaussian =
                m_emMixtureGaussian.GetMixtureGaussian();
        }

        public int ClusterInstance(double[] dblXArr)
        {
            //
            // normalise vector
            //
            //for(int j = 0; j < dblXArr.Length; j++)
            //{
            //        dblXArr[j] = dblXArr[j] / m_dblWeightArr[j];
            //        dblXArr[j] = dblXArr[j] * NORMALISER_FACTOR;
            //}
            

            MultNormalDist[] multNormalDistArr = 
                m_trainedMixtureGaussian.GetMultivariateGaussianArray();

            double dblMaxPdf = -double.MaxValue;
            int intMaxIndex = -1;
            double[] dblPArr = m_trainedMixtureGaussian.GetP();
            for (int i = 0; i < multNormalDistArr.Length; i++)
            {
                double dblCurrentPdf =
                    100 * dblPArr[i] * 
                    multNormalDistArr[i].Pdf(dblXArr);

                if (dblCurrentPdf > dblMaxPdf)
                {
                    dblMaxPdf = dblCurrentPdf;
                    intMaxIndex = i;
                }
            }
            return intMaxIndex;
        }

        #endregion

        #region Private Methods

        private static double[] GetSigma(
            int numVariables, 
            double[,] maxMin)
        {
            double[] sigmaArray = new double[numVariables];
            for (int variable = 0; variable < numVariables; variable++)
            {
                double delta = maxMin[variable, 1] - maxMin[variable, 0];
                double maxJump = delta/2;
                double actualSigma = 0.2*(maxJump/1.5);
                sigmaArray[variable] = actualSigma;
            }
            return sigmaArray;
        }

        private MixtureGaussian InitializeMixtureGaussian(
            int numGaussians,
            int numVariables,
            double[,] maxMin)
        {
            RngWrapper rng = new RngWrapper();
            UnivNormalDistStd univNormalDistStd = new UnivNormalDistStd(rng);
            double[] p = new double[numGaussians];
            // randomly initialize p, mean vectors and covariance matrix
            MultNormalDist[] multivariateGaussianArray = new MultNormalDist[
                numGaussians];
            double totalP = 0;
            for (int gaussian = 0; gaussian < numGaussians; gaussian++)
            {
                p[gaussian] = Math.Abs(univNormalDistStd.NextDouble());
                totalP += p[gaussian];
                double[] mean = new double[numVariables];
                double[,] covariance = new double[numVariables,numVariables];
                double[] sigmaArray = GetSigma(numVariables, maxMin);

                for (int variable = 0; variable < numVariables; variable++)
                {
                    mean[variable] = maxMin[variable, 1]*univNormalDistStd.NextDouble();
                    //System.out.println(mean[variable]);
                    covariance[variable, variable] = -1;
                    while (covariance[variable, variable] < 0)
                    {
                        covariance[variable, variable] =
                            (1.0/((numGaussians*2))) +
                            (univNormalDistStd.NextDouble()*sigmaArray[variable]);
                    }
                }
                //System.out.println("numVariables " + numVariables);
                //System.out.println(covariance.length + ", " +  covariance[0].length);
                multivariateGaussianArray[gaussian] =
                    new MultNormalDist(
                        mean,
                        covariance,
                        rng);
            }

            // normalize p
            for (int gaussian = 0; gaussian < numGaussians; gaussian++)
            {
                p[gaussian] = p[gaussian]/totalP;
            }

            //return new MixtureGaussian(numGaussians, maxMin, new RngWrapper());
            return new MixtureGaussian(multivariateGaussianArray, maxMin, p);
        }


        private void NormaliseData(double[,] data)
        {
            int samples = data.GetLength(0);
            int fields = data.GetLength(1);

            m_dblWeightArr = new double[fields];

            for (int i = 0; i < samples; i++)
            {
                for (int j = 0; j < fields; j++)
                {
                    m_dblWeightArr[j] += data[i, j];
                }
            }
            for (int i = 0; i < samples; i++)
            {
                for (int j = 0; j < fields; j++)
                {
                    data[i, j] = data[i, j] / m_dblWeightArr[j];
                    data[i, j] = data[i, j] * NORMALISER_FACTOR;
                }
            }
        }

        #endregion

        public override string ToString()
        {
            return m_trainedMixtureGaussian.ToString();
        }
    }
}
