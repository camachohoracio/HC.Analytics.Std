#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Mutation
{
    [Serializable]
    public abstract class AbstractMutation : IMutation
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructor

        public AbstractMutation(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region IMutation Members

        public abstract Individual DoMutation(
            Individual individual);

        #endregion
    }
}
