#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators
{
    [Serializable]
    public class GpObjectiveFunction : IHeuristicObjectiveFunction
    {
        #region Members

        private AbstractGpBridge m_gpBridge;

        #endregion

        #region Constructors

        public GpObjectiveFunction(AbstractGpBridge gpBridge)
        {
            m_gpBridge = gpBridge;
        }

        #endregion

        #region Properties

        public string ObjectiveName { get; set; }

        public AbstractGpBridge GpBridge
        {
            get { return m_gpBridge; }
            set { m_gpBridge = value; }
        }

        #endregion

        #region IHeuristicObjectiveFunction Members

        public double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            return m_gpBridge.GetRegressionFit(individual, heuristicProblem);
        }

        public List<VariableContribution> GetRankList()
        {
            throw new NotImplementedException();
        }

        public ResultRow GetResultObject(
            Individual solution,
            bool blnGetFinalResults)
        {
            return null;
        }

        public ObjectiveFunctionType ObjectiveFunctionType
        {
            get { return ObjectiveFunctionType.STD_OBJECTIVE_FUNCT; }
        }

        public double Evaluate()
        {
            throw new NotImplementedException();
        }

        public int VariableCount
        {
            get
            {
                //
                // otherwise the solver will complain that there is only one variable
                //
                return 2;
            }
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        ~GpObjectiveFunction()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                EventHandlerHelper.RemoveAllEventHandlers(this);
                if (m_gpBridge != null)
                {
                    m_gpBridge.Dispose();
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
