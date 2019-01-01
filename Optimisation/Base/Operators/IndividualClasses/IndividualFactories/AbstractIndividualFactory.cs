#region

using System;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories
{
    [Serializable]
    public abstract class AbstractIndividualFactory : IIndividualFactory
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public AbstractIndividualFactory(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region Public Methods

        public abstract Individual BuildRandomIndividual();

        #endregion
    }
}
