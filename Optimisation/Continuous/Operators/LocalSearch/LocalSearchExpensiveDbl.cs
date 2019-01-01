#region

using System;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Continuous.Operators.LocalSearch.NmLocalSearch;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchExpensiveDbl : AbstractLocalSearchExpensive
    {
        #region Constructors

        public LocalSearchExpensiveDbl(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override ILocalSearch CreateLocalSearchNearN()
        {
            return new LocalSearchNerestNeighbourDbl(m_heuristicProblem,
                                                     EXPENSIVE_NEIGHBOURHOOD);
        }

        public override AbstractLocalSearchNm CreateLocalSearchNM()
        {
            return new LocalSearchNmDbl(m_heuristicProblem);
        }
    }
}
