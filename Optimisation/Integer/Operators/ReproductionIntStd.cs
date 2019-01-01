#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Mutation;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Operators.Selection;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    [Serializable]
    public class ReproductionIntStd : AbstractReproduction
    {
        #region Members

        /// <summary>
        ///   Crossover operator
        /// </summary>
        private readonly ICrossover m_crossover;

        /// <summary>
        ///   Crossover rate
        /// </summary>
        private readonly double m_dblCrossoverProb;

        private readonly IMutation m_mutation;

        /// <summary>
        ///   Selection operator
        /// </summary>
        private readonly ISelection m_selection;

        #endregion

        #region Constructors

        public ReproductionIntStd(
            HeuristicProblem heuristicProblem)
            : this(
                heuristicProblem,
                OptimisationConstants.CROSSOVER_PROB)
        {
            m_selection = new MixedSelection(heuristicProblem);
            m_crossover = new TwoPointsCrossoverInt(heuristicProblem);
            m_mutation = new MutationInt(heuristicProblem);
        }

        public ReproductionIntStd(
            HeuristicProblem heuristicProblem,
            double dblCrossoverProb)
            : base(
                heuristicProblem)
        {
            m_dblCrossoverProb = dblCrossoverProb;
            ReproductionProb = OptimisationConstants.STD_REPRODUCTION_PROB;
        }

        #endregion

        #region Public

        /// <summary>
        ///   Cluster instance
        /// </summary>
        /// <param name = "individual"></param>
        public override void ClusterInstance(
            Individual individual)
        {
            // do not cluster instance
        }


        /// <summary>
        ///   Reproduce individual via Genetic algorithm
        /// </summary>
        /// <param name = "repairIndividual">
        ///   Repair operator
        /// </param>
        /// <param name = "localSearch">
        ///   Local search
        /// </param>
        /// <returns>
        ///   New individual
        /// </returns>
        public override Individual DoReproduction()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();

            var parent1 = m_selection.DoSelection();
            Individual parent2;
            Individual newIndividual;
            if (rng.NextDouble() < m_dblCrossoverProb)
            {
                // do crossover
                parent2 = m_selection.DoSelection();
                var intTrials = 0;
                while (parent2.Equals(parent1))
                {
                    //
                    // avoid infinite loop
                    // occurs when the population is the same.
                    //
                    if (intTrials > m_heuristicProblem.PopulationSize)
                    {
                        break;
                    }
                    parent2 = m_selection.DoSelection();
                    intTrials++;
                }
                newIndividual = m_crossover.DoCrossover(
                    rng,
                    new[]
                        {
                            parent1,
                            parent2
                        });
            }
            else
            {
                // do mutation
                newIndividual = m_mutation.DoMutation(parent1);
            }
            return newIndividual;
        }

        #endregion
    }
}
