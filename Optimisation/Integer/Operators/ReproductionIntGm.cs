#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    public class ReproductionIntGm : AbstractReproduction
    {
        #region Constructor

        public ReproductionIntGm(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            ReproductionProb = OptimisationConstants.GUIDED_MUTATION_REPRODUCTION_PROB;
        }

        #endregion

        /// <summary>
        ///   Reproduce individual via guided mutation.
        ///   This operator allows quicker convergence.
        /// </summary>
        /// <param name = "repairIndividual">
        ///   Repair operator
        /// </param>
        /// <param name = "intBestChromosome">
        ///   Best chromosome found so far
        /// </param>
        /// <param name = "localSearch">
        ///   Local search operator
        /// </param>
        /// <returns>
        ///   New individual
        /// </returns>
        public override Individual DoReproduction()
        {
            var rngWrapper = HeuristicProblem.CreateRandomGenerator();

            var intBestChromosome =
                m_heuristicProblem.Population.GetIndividualFromPopulation(
                    m_heuristicProblem,
                    0).GetChromosomeCopyInt();

            var intNewChrosmosomeArr = new int[m_heuristicProblem.VariableCount];

            for (var j = 0; j < m_heuristicProblem.VariableCount; j++)
            {
                if (rngWrapper.NextDouble() <= OptimisationConstants.DBL_GM_BETA)
                {
                    intNewChrosmosomeArr[j] = (int) m_heuristicProblem.GuidedConvergence.DrawGuidedConvergenceValue(
                        j,
                        rngWrapper);
                }
                else
                {
                    intNewChrosmosomeArr[j] = intBestChromosome[j];
                }
            }
            return new Individual(
                intNewChrosmosomeArr,
                m_heuristicProblem);
        }

        public override void ClusterInstance(Individual individual)
        {
            throw new NotImplementedException();
        }
    }
}
