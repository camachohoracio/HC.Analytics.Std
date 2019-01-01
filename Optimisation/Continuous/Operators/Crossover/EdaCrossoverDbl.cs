#region

using System;
using HC.Analytics.Optimisation.Base.Helpers;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.Crossover
{
    /// <summary>
    ///   Allows generate new individuals based on the the
    ///   distribution of solutions.
    ///   It requires a larger population size in order to
    ///   estimate the parameters of the crossover.
    /// </summary>
    [Serializable]
    public class EdaCrossoverDbl : AbstractCrossover
    {
        #region Members

        #endregion

        #region Constructors

        public EdaCrossoverDbl(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        public override Individual DoCrossover(
            RngWrapper rng,
            Individual[] individuals)
        {
            try
            {
                var dblNewChromosomeArray = new double[
                    m_heuristicProblem.VariableCount];

                //
                // reproduction via EDA (global search)
                //
                // get mean & stdDev vectors
                var dblMeanVector = GetMeanVector();

                var dblStdDevVector =
                    GetStdDevVector(
                        dblMeanVector,
                        rng);

                for (var j = 0;
                     j <
                     m_heuristicProblem.VariableCount;
                     j++)
                {
                    // noise is a random number normally distributed
                    var dblNoise = OptimizationHelper.Noise(rng);

                    dblNewChromosomeArray[j] = dblMeanVector[j] + dblStdDevVector[j]*dblNoise;
                    //
                    // validate chromosome value
                    //
                    if (dblNewChromosomeArray[j] < 0)
                    {
                        dblNewChromosomeArray[j] = 0;
                    }
                    else if (dblNewChromosomeArray[j] > 1.0)
                    {
                        dblNewChromosomeArray[j] = 1.0;
                    }
                }

                return new Individual(
                    dblNewChromosomeArray,
                    m_heuristicProblem);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        private double[] GetStdDevVector(
            double[] dblMeanVector,
            RngWrapper rng)
        {
            try
            {
                var intEdaPartentSize = GetEdaParentSize();
                var intEdaCurrentPartenSize = 0;
                var dblStdDevVector = new double[
                    m_heuristicProblem.VariableCount];
                intEdaCurrentPartenSize = 0;
                for (var i = 0; i < intEdaPartentSize; i++)
                    //foreach (IIndividual solution in
                    //    HeuristicProblem.Population_.LargePopulationSizeLargePopulationArr)
                {
                    var solution =
                        m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                            m_heuristicProblem,
                            i);
                    // get stdDev vector
                    for (var j = 0;
                         j <
                         m_heuristicProblem.VariableCount;
                         j++)
                    {
                        var dblCrValue = solution.GetChromosomeValueDbl(j);
                        dblStdDevVector[j] += Math.Pow(dblCrValue - dblMeanVector[j], 2);
                    }

                    intEdaCurrentPartenSize++;
                    if (intEdaCurrentPartenSize == intEdaPartentSize)
                    {
                        break;
                    }
                }
                for (var j = 0;
                     j <
                     m_heuristicProblem.VariableCount;
                     j++)
                {
                    dblStdDevVector[j] = Math.Sqrt(dblStdDevVector[j]/
                                                   (intEdaCurrentPartenSize - 1.0));

                    //
                    // validate std deviation
                    //
                    if (dblStdDevVector[j] < 1E-4)
                    {
                        if (rng.NextDouble() >= 0.6)
                        {
                            dblStdDevVector[j] = 0.1;
                        }
                        else
                        {
                            dblStdDevVector[j] = 1E-4;
                        }
                    }
                }

                return dblStdDevVector;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new double[] {};
        }

        private double[] GetMeanVector()
        {
            var intEdaCurrentPartenSize = 0;
            var intEdaParentSize = GetEdaParentSize();

            var dblMeanVector = new double[
                m_heuristicProblem.VariableCount];

            //foreach (IIndividual solution in
            //    HeuristicProblem.Population_.LargePopulationArr)
            for (var i = 0; i < intEdaParentSize; i++)
            {
                var solution =
                    m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                        m_heuristicProblem,
                        i);

                for (var j = 0;
                     j <
                     m_heuristicProblem.VariableCount;
                     j++)
                {
                    dblMeanVector[j] +=
                        solution.GetChromosomeValueDbl(j);
                }
                intEdaCurrentPartenSize++;

                if (intEdaCurrentPartenSize == intEdaParentSize)
                {
                    break;
                }
            }

            for (var j = 0;
                 j <
                 m_heuristicProblem.VariableCount;
                 j++)
            {
                dblMeanVector[j] /= intEdaCurrentPartenSize;
            }
            return dblMeanVector;
        }

        private int GetEdaParentSize()
        {
            if (m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                m_heuristicProblem,
                m_heuristicProblem.Population.LargePopulationSize - 1) == null)
            {
                //
                // large population is not ready
                // count the number of individuals
                //
                bool blnThereWasIndividual = false;
                for (var i = 0; i < m_heuristicProblem.Population.LargePopulationSize; i++)
                {
                    if (m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                        m_heuristicProblem,
                        i) == null)
                    {
                        return i;
                    }
                    blnThereWasIndividual = true;
                }
                if (blnThereWasIndividual)
                {
                    return m_heuristicProblem.Population.LargePopulationSize;
                }
            }
            else
            {
                return m_heuristicProblem.Population.LargePopulationSize;
            }
            //Debugger.Break();
            throw new HCException("Error. Individual is null.");
        }
    }
}
