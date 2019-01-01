#region

using System;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    /// <summary>
    ///   Local search helper
    /// </summary>
    [Serializable]
    public class LocalSearchHelper
    {
        #region Public

        /// <summary>
        ///   Return true if extensive local search is to be calculated
        /// </summary>
        /// <param name = "rng"></param>
        /// <returns></returns>
        public static bool CalculateExtensiveLocalSearch(
            RngWrapper rng,
            HeuristicProblem heuristicProblem,
            int intLocaSearchIterations)
        {
            if (intLocaSearchIterations < OptimisationConstants.EXPENSIVE_LOCAL_SERCH_ITERATIONS)
            {
                return false;
            }

            //
            // Ramdomly set an extensive/simple local search
            //
            if (rng.NextDouble() <= OptimisationConstants.DBL_EXTENSIVE_LOCAL_SEARCH ||
                heuristicProblem.Population.GetIndividualFromPopulation(
                    heuristicProblem,
                    0) == null)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
