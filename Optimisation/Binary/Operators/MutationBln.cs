#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Mutation;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators
{
    [Serializable]
    public class MutationBln : AbstractMutation
    {
        #region Members

        private readonly double m_dblMutationProbability;
        private readonly double m_dblMutationRate;

        #endregion

        #region Constructors

        /// <summary>
        ///   Default constructor
        /// </summary>
        /// <param name = "heuristicProblem"></param>
        public MutationBln(HeuristicProblem heuristicProblem) :
            this(heuristicProblem,
                 OptimisationConstants.MUTATION_RATE,
                 OptimisationConstants.MUTATION_PROBABILITY)
        {
        }

        public MutationBln(
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

            bool[] blnChromosomeArr = null;
            if (m_heuristicProblem.VariableCount > 0)
            {
                //copy chromosome from parent
                blnChromosomeArr = individual.GetChromosomeCopyBln();
                //number of points to swap
                var ps = (int) (m_heuristicProblem.VariableCount*
                                m_dblMutationRate + 1.0);
                int index;

                // swap ps randomly selected points
                for (var i = 0; i < ps; i++)
                {
                    //point to be swapped
                    index = (int) ((m_heuristicProblem.VariableCount)*
                                   rng.NextDouble());

                    if (rng.NextDouble() > m_dblMutationProbability)
                    {
                        // do normal mutation
                        if (blnChromosomeArr[index] == false)
                        {
                            blnChromosomeArr[index] = true;
                        }
                        else
                        {
                            blnChromosomeArr[index] = false;
                        }
                    }
                    else
                    {
                        // do guided mutation
                        if (rng.NextDouble() <=
                            m_heuristicProblem.GuidedConvergence.GetGcProb(index))
                        {
                            blnChromosomeArr[index] = true;
                        }
                        else
                        {
                            blnChromosomeArr[index] = false;
                        }
                    }
                }
            }

            //create new individual and return
            var newIndividual = new Individual(
                blnChromosomeArr,
                m_heuristicProblem);
            return newIndividual;
        }
    }
}
