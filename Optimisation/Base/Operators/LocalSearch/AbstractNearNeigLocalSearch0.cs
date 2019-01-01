#region

using System;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    [Serializable]
    public abstract class AbstractNearNeigLocalSearch0 : AbstractLocalSearch
    {
        #region Members

        protected int m_intSearchIterations;
        protected SearchDirectionOperator m_searchDirectionOperator;

        #endregion

        #region Constructors

        public AbstractNearNeigLocalSearch0(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_searchDirectionOperator = new SearchDirectionOperator(
                heuristicProblem);
        }

        #endregion
    }
}
