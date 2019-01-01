#region

using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators
{
    public static class OperatorHelper
    {
        public static void UpgradeSolverOperators(
            HeuristicProblem heuristicProblem)
        {
            if (heuristicProblem.IsMultiObjective())
            {
                heuristicProblem.MultiObjectiveRanking.Rank();
            }
            else
            {
                //
                // update gc probabilities
                //
                if (heuristicProblem.GuidedConvergence != null)
                {
                    heuristicProblem.GuidedConvergence.UpdateGcProbabilities(
                        heuristicProblem);
                }
                //
                // Upgrade population
                //
                heuristicProblem.Population.LoadPopulation();
            }
        }
    }
}
