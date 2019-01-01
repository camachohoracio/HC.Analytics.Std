#region

using System;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Integer.Operators.LocalSearch.NmLocalSearch;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchExpensiveInt : AbstractLocalSearchExpensive
    {
        #region Constructors

        public LocalSearchExpensiveInt(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override ILocalSearch CreateLocalSearchNearN()
        {
            return new LocalSearchNerestNeighbourInt(m_heuristicProblem,
                                                     EXPENSIVE_NEIGHBOURHOOD);
        }

        public override AbstractLocalSearchNm CreateLocalSearchNM()
        {
            return new LocalSearchNmInt(m_heuristicProblem);
        }
    }
}
