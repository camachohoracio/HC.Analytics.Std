#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Selection
{
    [Serializable]
    public abstract class AbstractSelection : ISelection
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructor

        protected AbstractSelection(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region ISelection Members

        public abstract Individual DoSelection();

        #endregion
    }
}
