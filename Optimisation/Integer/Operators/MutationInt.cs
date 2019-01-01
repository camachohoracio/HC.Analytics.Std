#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Mutation;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    [Serializable]
    public class MutationInt : AbstractMutation
    {
        #region Members

        private const double MUTATION_PROBABILITY = 0.5;
        private const double MUTATION_RATE = 0.2;

        private readonly double m_dblMutationProbability;
        private readonly double m_dblMutationRate;

        #endregion

        #region Constructors

        /// <summary>
        ///   Default constructor
        /// </summary>
        /// <param name = "heuristicProblem"></param>
        public MutationInt(HeuristicProblem heuristicProblem) :
            this(heuristicProblem,
                 MUTATION_RATE,
                 MUTATION_PROBABILITY)
        {
        }

        public MutationInt(
            HeuristicProblem heuristicProblem,
            double dblMutationRate,
            double dblMutationProbability) :
                base(heuristicProblem)
        {
            m_dblMutationRate = dblMutationRate;
            m_dblMutationProbability = dblMutationProbability;
        }

        #endregion

        public override Individual DoMutation(
            Individual individual)
        {
            var rng = HeuristicProblem.CreateRandomGenerator();

            int[] intNewChromosome = null;
            if (m_heuristicProblem.VariableCount > 0)
            {
                //copy chromosome from parent
                intNewChromosome = individual.GetChromosomeCopyInt();
                //number of points to swap
                var ps = (int) (m_heuristicProblem.VariableCount*
                                m_dblMutationRate + 1.0);
                int index;

                // swap ps randomly selected points
                for (var i = 0; i < ps; i++)
                {
                    //point to be swapped
                    index = rng.NextInt(0, m_heuristicProblem.VariableCount - 1);

                    //PrintToScreen.WriteLine(rng.NextDouble());

                    if (rng.NextDouble() > m_dblMutationProbability)
                    {
                        // do simple mutation
                        intNewChromosome[index] = rng.NextInt(
                            0, (int) m_heuristicProblem.VariableRangesIntegerProbl[index]);
                    }
                    else
                    {
                        // do guided-mutation
                        intNewChromosome[index] =
                            (int) m_heuristicProblem.GuidedConvergence.DrawGuidedConvergenceValue(
                                index,
                                rng);
                    }
                }
            }

            //create new individual and return
            var newIndividual =
                new Individual(
                    intNewChromosome,
                    m_heuristicProblem);
            return newIndividual;
        }
    }
}
