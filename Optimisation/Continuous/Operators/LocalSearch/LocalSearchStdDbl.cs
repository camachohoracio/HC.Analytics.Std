#region

using System;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchStdDbl : AbstractLocalSearchStd
    {
        #region Constructors

        /// <summary>
        ///   Constructors
        /// </summary>
        public LocalSearchStdDbl(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override ILocalSearch CreateCheapLocalSearch()
        {
            return new LocalSearchSimpleDbl(m_heuristicProblem);
        }

        public override ILocalSearch CreateExpensiveLocalSearch()
        {
            return new LocalSearchExpensiveDbl(m_heuristicProblem);
        }
    }
}
