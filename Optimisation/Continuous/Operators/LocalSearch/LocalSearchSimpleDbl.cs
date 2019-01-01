#region

using System;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchSimpleDbl : AbstractLocalSearchSimple
    {
        #region Constructors

        public LocalSearchSimpleDbl(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        public override ILocalSearch CreateLocalSearchNearN()
        {
            return new LocalSearchNerestNeighbourDbl(
                m_heuristicProblem,
                SIMPLE_NEIGHBOURHOOD);
        }
    }
}
