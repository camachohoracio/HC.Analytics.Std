#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    /// <summary>
    ///   Template which serves to define a regression model.
    ///   It is used as the objetive function of the GP problem
    /// </summary>
    public abstract class AbstractGpBridge : IDisposable
    {
        #region Members

        protected GpOperatorsContainer m_gpOperatorsContainer;
        protected int m_intNumbTestCases;

        #endregion

        #region Properties

        public GpOperatorsContainer GpOperatorsContainer
        {
            get { return m_gpOperatorsContainer; }
            private set { m_gpOperatorsContainer = value; }
        }

        #endregion

        #region Constructors

        public AbstractGpBridge(
            GpOperatorsContainer gpOperatorsContainer)
        {
            m_gpOperatorsContainer = gpOperatorsContainer;
        }

        #endregion

        protected AbstractGpVariable GetVariable(
            object value)
        {
            //
            // clone parameter and assign value
            //
            AbstractGpVariable gpVariable =
                m_gpOperatorsContainer.GpVariable.Clone();
            gpVariable.SetValue(value);
            return gpVariable;
        }

        public abstract double GetRegressionFit(
            Individual gpIndividual,
            HeuristicProblem heuristicProblem);

        public virtual void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (m_gpOperatorsContainer != null)
            {
                m_gpOperatorsContainer.Dispose();
            }
        }

        ~AbstractGpBridge()
        {
            Dispose();
        }

    }
}
