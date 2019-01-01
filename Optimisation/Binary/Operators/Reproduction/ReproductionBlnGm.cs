#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.Reproduction
{
    [Serializable]
    public class ReproductionBlnGm : AbstractReproduction
    {
        public ReproductionBlnGm(HeuristicProblem heuristicProblem)
            : base(heuristicProblem)
        {
            ReproductionProb = OptimisationConstants.GUIDED_MUTATION_REPRODUCTION_PROB;
        }

        public override Individual DoReproduction()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();

            var blnNewChrosmosome = new bool[
                m_heuristicProblem.VariableCount];
            var blnBestChromosomeArr = m_heuristicProblem.Population.
                GetIndividualFromPopulation(
                    m_heuristicProblem,
                    0).
                GetChromosomeCopyBln();
            for (var j = 0; j < m_heuristicProblem.VariableCount; j++)
            {
                if (rng.NextDouble() <= OptimisationConstants.DBL_GM_BETA)
                {
                    if (rng.NextDouble() <=
                        m_heuristicProblem.GuidedConvergence.GetGcProb(j))
                    {
                        blnNewChrosmosome[j] = true;
                    }
                    else
                    {
                        blnNewChrosmosome[j] = false;
                    }
                }
                else
                {
                    blnNewChrosmosome[j] =
                        blnBestChromosomeArr[j];
                }
            }
            return new Individual(
                blnNewChrosmosome,
                m_heuristicProblem);
        }

        public override void ClusterInstance(
            Individual individual)
        {
            throw new NotImplementedException();
        }
    }
}
