#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Repair
{
    [Serializable]
    public abstract class AbstractRepair : IRepair
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public AbstractRepair(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region Abstract Methods

        public abstract bool DoRepair(Individual individual);

        public abstract void AddRepairOperator(IRepair repair);

        #endregion

        public virtual void Dispose()
        {
            
        }
    }
}
