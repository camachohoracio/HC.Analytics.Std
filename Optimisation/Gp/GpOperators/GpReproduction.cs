#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Mutation;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Operators.Selection;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators
{
    [Serializable]
    public class GpReproduction : AbstractReproduction
    {
        #region Members

        private readonly ICrossover m_crossover;
        private readonly double m_dblCrossoverProbability;
        private readonly int m_intMaxTreeDepthMutation;
        private readonly IMutation m_mutation;
        private readonly ISelection m_selection;

        #endregion

        #region Constructor

        public GpReproduction(
            HeuristicProblem heuristicProblem,
            double dblCrossoverProbability,
            int intMaxTreeDepthMutation,
            int intMaxTreeSize,
            int intTournamentSize,
            GpOperatorsContainer gpOperatorsContainer) :
                base(heuristicProblem)
        {
            m_dblCrossoverProbability = dblCrossoverProbability;
            m_intMaxTreeDepthMutation = intMaxTreeDepthMutation;
            m_heuristicProblem = heuristicProblem;

            m_selection = new RankSelection(
                heuristicProblem,
                intTournamentSize);

            m_crossover = new GpCrossover(
                heuristicProblem,
                intMaxTreeSize);

            m_mutation = new GpMutation(
                m_intMaxTreeDepthMutation,
                gpOperatorsContainer);
        }

        #endregion

        public override Individual DoReproduction()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            Individual ind1 = m_selection.DoSelection();
            Individual newIndividual;

            if (rng.NextDouble() <= GpConstants.RANDOM_INDIVIDUAL)
            {
                return m_heuristicProblem.IndividualFactory.BuildRandomIndividual();
            }

            if (rng.NextDouble() <= m_dblCrossoverProbability)
            {
                Individual ind2 = m_selection.DoSelection();
                int intTrials = 0;

                bool blnFailed = false;
                while (ind1.Equals(ind2))
                {
                    ind2 = m_selection.DoSelection();

                    //
                    // avoid infinite loop
                    // occurs when the population is the same.
                    //
                    if (intTrials > 5)
                    {
                        blnFailed = true;
                        break;
                    }
                    intTrials++;
                }

                if (blnFailed)
                {
                    newIndividual = ind1.Clone(m_heuristicProblem);
                    m_mutation.DoMutation(newIndividual);
                }
                else
                {
                    newIndividual =
                        m_crossover.DoCrossover(
                            rng,
                            new[] {ind1, ind2});
                }
            }
            else
            {
                newIndividual = ind1.Clone(m_heuristicProblem);
                m_mutation.DoMutation(newIndividual);
            }
            return newIndividual;
        }

        public override void ClusterInstance(
            Individual individual)
        {
        }
    }
}
