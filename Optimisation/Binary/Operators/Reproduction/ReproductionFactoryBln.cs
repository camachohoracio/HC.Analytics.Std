#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.Reproduction
{
    public static class ReproductionFactoryBln
    {
        public static IReproduction BuildReproductionBln(
            HeuristicProblem heuristicProblem)
        {
            var reproductionList = new List<IReproduction>();
            reproductionList.Add(
                new ReproductionBlnStd(heuristicProblem));
            reproductionList.Add(
                new ReproductionBlnGm(heuristicProblem));

            var reproduction =
                new ReproductionClass(
                    heuristicProblem,
                    reproductionList);

            return reproduction;
        }
    }
}
