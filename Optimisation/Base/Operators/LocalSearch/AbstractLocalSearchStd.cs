#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    /// <summary>
    ///   Local search operator for continuous optimisation problems.
    ///   Implements the Nelder-Mead solver for "Expensive" local search
    /// </summary>
    [Serializable]
    public abstract class AbstractLocalSearchStd : AbstractLocalSearch
    {
        #region Members

        private readonly ILocalSearch m_localSearchExpensiveDbl;
        private readonly ILocalSearch m_localSearchSimpleDbl;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructors
        /// </summary>
        public AbstractLocalSearchStd(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_localSearchExpensiveDbl = CreateExpensiveLocalSearch();

            m_localSearchSimpleDbl = CreateCheapLocalSearch();
        }

        #endregion

        #region Public

        /// <summary>
        ///   Do local search
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
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
                m_localSearchExpensiveDbl.DoLocalSearch(
                    individual);
            }
            else
            {
                m_localSearchSimpleDbl.DoLocalSearch(
                    individual);
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

        #region Abstract Methods

        public abstract ILocalSearch CreateCheapLocalSearch();
        public abstract ILocalSearch CreateExpensiveLocalSearch();

        #endregion
    }
}
