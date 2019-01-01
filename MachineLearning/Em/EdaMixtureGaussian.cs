#region

using System;
using System.Collections;
using System.Text;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Distributions.Mixtures;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.Em
{
    public class EdaMixtureGaussian
    {
        #region Constants

        private const int EDA_ITERATIONS = 10;
        private const int EDA_POPULATION_SIZE = 10;
        private const int EDA_SELECTION = 5;
        private const double EDA_CONVERGENCE = 1e-10;
        private const int EDA_PROOF_CONVERGENCE = 5;

        #endregion

        #region Members

        private readonly double m_dblConvergence;
        private readonly double[,] m_dblDataArray;
        private readonly double[,] m_dblMaxMinArray;
        private readonly int m_intIterations;
        private readonly int m_intNumVariables;
        private readonly int m_intPopulationSize;
        private readonly int m_intProofConvergence;
        private readonly int m_intSelection;
        private readonly StringBuilder m_logBuff;
        private readonly RngWrapper m_rng;
        private readonly UnivNormalDistStd m_univNormalDistStd;
        private int m_intActualIteration;
        private MixtureGaussian m_bestMixtureGaussian;
        private int bestNumGaussians;

        //
        // em parameters
        //
        private double m_dblBestLocalLikelihood;
        private int m_intNumGaussians;
        private double m_dblNewLikelihood;
        private double m_dblOldLikelihood;
        private double[] m_dblSigmaArr;
        private double m_dblSigmaP;

        #endregion

        #region Constructors

        public EdaMixtureGaussian(
            double[,] dblMaxMinArr,
            int intNumGaussians,
            double[,] dblDataArray)
        {
            m_rng = new RngWrapper();
            m_univNormalDistStd = new UnivNormalDistStd(m_rng);
            m_logBuff = new StringBuilder();
            m_intNumVariables = dblDataArray.GetLength(1);
            m_intNumGaussians = intNumGaussians + EmConstants.INITIAL_NUM_GAUSSIANS;
            m_dblDataArray = dblDataArray;
            m_dblMaxMinArray = dblMaxMinArr;

            // set EDA params
            m_intPopulationSize = EDA_POPULATION_SIZE;
            m_intIterations = EDA_ITERATIONS;
            m_intSelection = EDA_SELECTION;
            m_dblConvergence = EDA_CONVERGENCE;
            m_intProofConvergence = EDA_PROOF_CONVERGENCE;

            GetSigmaParams();
        }

        #endregion

        public override string ToString()
        {
            return "EDAGM_it_" + m_intIterations + "_pop_" + m_intPopulationSize + "_sel_" + m_intSelection + "_gauss_" +
                   m_intNumGaussians + "_conv_" + m_dblConvergence + "_prCon_" + m_intProofConvergence;
        }

        /// <summary>
        /// Load Sigma parameter.
        /// Required by the EM algorithm
        /// </summary>
        private void GetSigmaParams()
        {
            m_dblSigmaArr = EmHelper.GetSigmaArr(
                m_dblMaxMinArray,
                m_intNumVariables);

            m_dblSigmaP = EmConstants.EM_SIGMA * 0.5;
        }

        public void RunEda()
        {
            EdaMixtureObject edaMixtureObject = null;
            ArrayList populationList;

            if (edaMixtureObject == null)
            {
                populationList = new ArrayList();
                LoadInitialPopulation(populationList);
                PrintToScreen.WriteLine("Finish generating initial population");
            }
            else
            {
                populationList = edaMixtureObject.getPopulationList();
                m_intActualIteration = edaMixtureObject.getIteration();
                PrintToScreen.WriteLine("file: " + ToString() + " found at iteration: " +
                                        m_intActualIteration);
            }

            int actualProofConvergence = 0;
            for (m_intActualIteration = 0; 
                m_intActualIteration < m_intIterations; 
                m_intActualIteration++)
            {
                EvaluatePopulation(populationList);
                /*while (bestNumGaussians >= numGaussians - 1) {
                  string out = "The best model contains: " + bestNumGaussians +
                      " gaussians, logLikelihood: " + bestLocalLikelihood +
                      ", resample will be performed";
                  PrintToScreen.WriteLine(out);
                  logBuff.Append(out + Environment.NewLine);
                  resample(populationList);
                       }*/
                SelectPopulation(populationList);
                double improvement = m_dblNewLikelihood - m_dblOldLikelihood;
                m_dblOldLikelihood = m_dblNewLikelihood;
                string strOut = m_bestMixtureGaussian.ToString();
                PrintToScreen.WriteLine(strOut);
                m_logBuff.Append(strOut + Environment.NewLine);
                strOut = "Improvement " + improvement + ", Likelihood: " +
                       m_dblNewLikelihood + " (it: " + (m_intActualIteration + 1) + "/" + m_intIterations +
                       ")...";
                PrintToScreen.WriteLine(strOut);
                m_logBuff.Append(strOut + Environment.NewLine);
                if (improvement <= m_dblConvergence)
                {
                    actualProofConvergence++;
                    if (actualProofConvergence >= m_intProofConvergence)
                    {
                        break;
                    }
                }
                else
                {
                    actualProofConvergence = 0;
                }
                Mutation(populationList);
            }
            //TextWriter.write(path, ToString() + ".log", logBuff);
        }

        private void Mutation(ArrayList populationList)
        {
            int actualPopulation = 0;
            ArrayList newPopulationList = new ArrayList();
            while (actualPopulation != m_intPopulationSize)
            {
                foreach (EmMixtureGaussian oldEMMixtureGaussian in populationList)
                {
                    actualPopulation++;
                    EmMixtureGaussian newEMMixtureGaussian = CreateNewChild(
                        oldEMMixtureGaussian);
                    newPopulationList.Add(newEMMixtureGaussian);
                    if (actualPopulation == m_intPopulationSize)
                    {
                        break;
                    }
                }
            }
            // add the new population to populationList
            foreach (EmMixtureGaussian newEMMixtureGaussian in newPopulationList)
            {
                populationList.Add(newEMMixtureGaussian);
            }
        }

        private EmMixtureGaussian CreateNewChild(
            EmMixtureGaussian emMixtureGaussianOld)
        {
            MixtureGaussian mixtureGaussianOld = emMixtureGaussianOld.
                GetMixtureGaussian();

            int intNumGaussians = mixtureGaussianOld.GetNumGaussians();

            MultNormalDist[] multivariateGaussianArrayOld =
                mixtureGaussianOld.GetMultivariateGaussianArray(),
                             multivariateGaussianArrayNew = new MultNormalDist[intNumGaussians];

            double[] oldP = mixtureGaussianOld.GetP();
            double[] newP = new double[intNumGaussians];

            // set p, mean vectors and covariance matrix
            double totalP = 0.0;
            for (int gaussian = 0; gaussian < intNumGaussians; gaussian++)
            {
                double[] oldMean =
                    multivariateGaussianArrayOld[gaussian].MeanVector.GetArr(),
                         newMean = new double[m_intNumVariables];
                double[,] oldCovariance = multivariateGaussianArrayOld[gaussian].CovMatrix.GetArr(),
                          newCovariance = new double[m_intNumVariables,m_intNumVariables];
                newP[gaussian] = -1;

                while (newP[gaussian] < 0)
                {
                    newP[gaussian] = oldP[gaussian] +
                                     (m_univNormalDistStd.NextDouble()*m_dblSigmaP);
                }
                totalP += newP[gaussian];
                for (int variable = 0; variable < m_intNumVariables; variable++)
                {
                    newMean[variable] = oldMean[variable] +
                                        (m_univNormalDistStd.NextDouble()*m_dblSigmaArr[variable]);
                    newCovariance[variable, variable] = -1;
                    while (newCovariance[variable, variable] < 0)
                    {
                        newCovariance[variable, variable] =
                            oldCovariance[variable, variable] +
                            (m_univNormalDistStd.NextDouble()*m_dblSigmaArr[variable]);
                    }
                }
                multivariateGaussianArrayNew[gaussian] = new MultNormalDist(
                    newMean,
                    newCovariance,
                    m_rng);
            }

            // normalize p
            for (int gaussian = 0; gaussian < intNumGaussians; gaussian++)
            {
                newP[gaussian] /= totalP;
            }

            MixtureGaussian newMixtureGaussian = new MixtureGaussian(
                multivariateGaussianArrayNew, 
                m_dblMaxMinArray, 
                newP);

            EmMixtureGaussian newEmMixtureGaussian = new EmMixtureGaussian(
                m_dblDataArray,
                newMixtureGaussian,
                EmConstants.EM_CONVERGENCE,
                EmConstants.EM_ITERATIONS,
                EmConstants.INT_EM_CONVERGENCE,
                m_rng);

            return newEmMixtureGaussian;
        }

        private void SelectPopulation(ArrayList populationList)
        {
            populationList.Sort();
            int counter = 1;
            EmMixtureGaussian actualEmMixtureGaussian = ((EmMixtureGaussian) populationList[0]);
            setEmParameters_Convergence(actualEmMixtureGaussian);
            m_dblNewLikelihood = actualEmMixtureGaussian.GetLikelihood();
            m_bestMixtureGaussian = actualEmMixtureGaussian.GetMixtureGaussian();
            int intCount = populationList.Count;
            for (int i = 0; i < intCount; i++)
            {
                counter++;
                if (counter > m_intSelection)
                {
                    populationList.RemoveAt(
                        populationList.Count - 1);
                }
                else
                {
                    actualEmMixtureGaussian = ((EmMixtureGaussian) populationList[i]);
                    setEmParameters_Convergence(actualEmMixtureGaussian);
                }
            }
        }

        private void setEmParameters_Convergence(EmMixtureGaussian emMixtureGaussian)
        {
            emMixtureGaussian.SetIterations(200);
            emMixtureGaussian.SetConvergence(0.000001);
        }

        private void EvaluatePopulation(ArrayList populationList)
        {
            m_dblBestLocalLikelihood = -Double.MaxValue;
            bestNumGaussians = 0;
            int size = populationList.Count;
            int individual = 0;
            foreach (EmMixtureGaussian currentEmMixtureGaussian in populationList)
            {
                individual++;
                int actualNumGaussians = currentEmMixtureGaussian.GetMixtureGaussian().
                    GetNumGaussians();
                string out_ = "EM individual: " + individual + "/"
                              + size + " (gauss:" + actualNumGaussians
                              + ") (it: " + (m_intActualIteration + 1) + "/" + m_intIterations + ")...";

                PrintToScreen.Write(out_);
                m_logBuff.Append(out_);
                currentEmMixtureGaussian.RunEm();
                double actualLikelihood = currentEmMixtureGaussian.GetLikelihood();
                out_ = " Likelihood: " + actualLikelihood;
                PrintToScreen.WriteLine(out_);
                m_logBuff.Append(out_ + Environment.NewLine);

                if (actualLikelihood > m_dblBestLocalLikelihood)
                {
                    m_dblBestLocalLikelihood = actualLikelihood;
                    bestNumGaussians = actualNumGaussians;
                }
            }
        }

        private void Resample(ArrayList populationList)
        {
            bestNumGaussians = 0;
            int intOldNumGaussians = m_intNumGaussians;
            int intNumGaussians = intOldNumGaussians;
            int intIndividual = 0;
            int intReSampleSize = 9;
            
            m_intNumGaussians += 3;
            for (int actualPopulation = 0;
                 actualPopulation < intReSampleSize;
                 actualPopulation++)
            {
                intIndividual++;
                intNumGaussians++;

                MixtureGaussian mixtureGaussian =
                    EmHelper.CreateRandomMixtureGaussian(
                        intNumGaussians,
                        m_intNumVariables,
                        m_dblMaxMinArray,
                        m_dblSigmaArr);

                if (intNumGaussians == m_intNumGaussians)
                {
                    intNumGaussians = intOldNumGaussians;
                }
                EmMixtureGaussian emMixtureGaussian = new EmMixtureGaussian(
                    m_dblDataArray,
                    mixtureGaussian,
                    EmConstants.EM_CONVERGENCE,
                    EmConstants.EM_ITERATIONS,
                    EmConstants.INT_EM_CONVERGENCE,
                    m_rng);
                int actualNumGaussians = mixtureGaussian.GetNumGaussians();
                string out_ = "EM extra individual: " + intIndividual + "/"
                              + intReSampleSize + " (gauss:" + actualNumGaussians
                              + ") (it: " + (m_intActualIteration + 1) + "/" + m_intIterations +
                              ")...";

                PrintToScreen.Write(out_);
                m_logBuff.Append(out_ + Environment.NewLine);
                emMixtureGaussian.RunEm();
                double actualLikelihood = emMixtureGaussian.GetLikelihood();
                if (actualLikelihood > m_dblBestLocalLikelihood)
                {
                    m_dblBestLocalLikelihood = actualLikelihood;
                    bestNumGaussians = actualNumGaussians;
                }
                out_ = " Likelihood: " + emMixtureGaussian.GetLikelihood();
                PrintToScreen.WriteLine(out_);
                m_logBuff.Append(out_ + Environment.NewLine);
                populationList.Add(emMixtureGaussian);
            }
        }

        private void LoadInitialPopulation(ArrayList populationList)
        {
            int intLocalNumGaussians = EmConstants.INITIAL_NUM_GAUSSIANS - 1;

            for (int actualPopulation = 0;
                 actualPopulation < m_intPopulationSize;
                 actualPopulation++)
            {
                intLocalNumGaussians++;

                MixtureGaussian mixtureGaussian = EmHelper.CreateRandomMixtureGaussian(
                    intLocalNumGaussians,
                    m_intNumVariables,
                    m_dblMaxMinArray,
                    m_dblSigmaArr);

                if (intLocalNumGaussians == m_intNumGaussians)
                {
                    intLocalNumGaussians = EmConstants.INITIAL_NUM_GAUSSIANS - 1;
                }

                EmMixtureGaussian emMixtureGaussian = new EmMixtureGaussian(
                    m_dblDataArray,
                    mixtureGaussian,
                    EmConstants.EM_CONVERGENCE,
                    EmConstants.EM_ITERATIONS,
                    EmConstants.INT_EM_CONVERGENCE,
                    m_rng);

                populationList.Add(emMixtureGaussian);
            }
        }

        public MixtureGaussian GetBestMixtureGaussian()
        {
            return m_bestMixtureGaussian;
        }
    }
}
