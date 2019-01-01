#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public class HeuristicMultiObjectiveFunction : IHeuristicObjectiveFunction
    {
        #region Properties

        public string ObjectiveName { get; set; }

        public int ObjectiveCount
        {
            get { return m_objectiveFunctions.Count; }
        }

        public List<IHeuristicObjectiveFunction> ObjectiveFunctions
        {
            get { return m_objectiveFunctions; }
        }

        #endregion

        #region Members

        private readonly List<IHeuristicObjectiveFunction> m_objectiveFunctions;

        #endregion

        public HeuristicMultiObjectiveFunction(
            List<IHeuristicObjectiveFunction> objectiveFunctions)
        {
            m_objectiveFunctions = objectiveFunctions;
        }

        #region IHeuristicObjectiveFunction Members

        public ObjectiveFunctionType ObjectiveFunctionType
        {
            get { return ObjectiveFunctionType.MULTI_OBJECTIVE_FUNCT; }
            set { throw new NotImplementedException(); }
        }

        public double Evaluate()
        {
            throw new NotImplementedException();
        }

        public int VariableCount
        {
            get { return m_objectiveFunctions[0].VariableCount; }
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        public double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            var dblFitnessArr =
                EvaluateMultiObjective(individual, heuristicProblem);
            for (var i = 0; i < dblFitnessArr.Length; i++)
            {
                individual.SetFitnessValue(
                    dblFitnessArr[i],
                    i);
            }

            return 0.0; // throw new NotImplementedException();
        }

        #endregion

        public double[] EvaluateMultiObjective(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            try
            {
                var dblObjectivesArray =
                    new double[ObjectiveCount];

                for (var i = 0; i < ObjectiveCount; i++)
                {
                    IHeuristicObjectiveFunction objectiveFunction = m_objectiveFunctions[i];
                    double dblCurrVal = objectiveFunction.Evaluate(individual,
                        heuristicProblem);
                    dblObjectivesArray[i] = dblCurrVal;
                }
                return dblObjectivesArray;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public ResultRow GetResultObject(
            bool blnGetFinalResults)
        {
            throw new NotImplementedException();
        }

        public void AddConstraint(
            IHeuristicObjectiveFunction objectiveFunction)
        {
            m_objectiveFunctions.Add(objectiveFunction);
        }

        ~HeuristicMultiObjectiveFunction()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (m_objectiveFunctions != null)
            {
                for (int i = 0; i < m_objectiveFunctions.Count; i++)
                {
                    m_objectiveFunctions[i].Dispose();
                }
            }
        }
    }
}
