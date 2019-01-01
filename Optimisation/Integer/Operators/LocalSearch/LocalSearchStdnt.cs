#region

using System;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchStdInt : AbstractLocalSearchStd
    {
        #region Constructors

        /// <summary>
        ///   Constructors
        /// </summary>
        public LocalSearchStdInt(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override ILocalSearch CreateCheapLocalSearch()
        {
            return new LocalSearchSimpleInt(m_heuristicProblem);
        }

        public override ILocalSearch CreateExpensiveLocalSearch()
        {
            return new LocalSearchExpensiveInt(m_heuristicProblem);
        }
    }
}
