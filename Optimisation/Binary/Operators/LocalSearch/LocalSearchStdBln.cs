#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.LocalSearch
{
    /// <summary>
    ///   Local search operator for binary solvers.
    ///   Find a solution in the neighbourhood of a given individual.
    ///   The neighbour solution is sometimes better than the prvided individual.
    /// </summary>
    [Serializable]
    public class LocalSearchStdBln : AbstractLocalSearch
    {
        #region Members

        private readonly ILocalSearch m_localSearchExpensiveBln;
        private readonly ILocalSearch m_localSearchSimpleBln;

        #endregion

        #region Constructor

        /// <summary>
        ///   Constructor
        /// </summary>
        public LocalSearchStdBln(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            // initialize inner local search operators
            m_localSearchExpensiveBln = new LocalSearchExpensiveBln(heuristicProblem);
            m_localSearchSimpleBln = new LocalSearchSimpleBln(heuristicProblem);
        }

        #endregion

        #region Public

        public override void DoLocalSearch(
            Individual individual)
        {
            var rng = new RngWrapper();

            CheckIterations();

            if (LocalSearchHelper.CalculateExtensiveLocalSearch(
                rng,
                m_heuristicProblem,
                m_intLocaSearchIterations))
            {
                m_localSearchExpensiveBln.DoLocalSearch(individual);
            }
            else
            {
                m_localSearchSimpleBln.DoLocalSearch(individual);
            }
        }


        private void CheckIterations()
        {
            m_intLocaSearchIterations++;
            var bestIndividual =
                m_heuristicProblem.Population.GetIndividualFromPopulation(
                    m_heuristicProblem,
                    0);

            if (bestIndividual != null)
            {
                if (bestIndividual.Fitness > m_dblBestFitness)
                {
                    m_dblBestFitness =
                        bestIndividual.Fitness;
                    m_intLocaSearchIterations = 0;
                }
            }
        }

        #endregion
    }
}
