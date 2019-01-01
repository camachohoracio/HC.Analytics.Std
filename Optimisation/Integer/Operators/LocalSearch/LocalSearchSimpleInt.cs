#region

using System;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchSimpleInt : AbstractLocalSearchSimple
    {
        #region Constructors

        public LocalSearchSimpleInt(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        public override ILocalSearch CreateLocalSearchNearN()
        {
            return new LocalSearchNerestNeighbourInt(
                m_heuristicProblem,
                SIMPLE_NEIGHBOURHOOD);
        }
    }
}
