#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    public static class ReproductionFactoryInt
    {
        public static IReproduction BuildReproductionInt(
            HeuristicProblem heuristicProblem)
        {
            var reproductionList = new List<IReproduction>();
            reproductionList.Add(
                new ReproductionIntStd(heuristicProblem));
            reproductionList.Add(
                new ReproductionIntGm(heuristicProblem));

            var reproduction =
                new ReproductionClass(
                    heuristicProblem,
                    reproductionList);

            return reproduction;
        }
    }
}
