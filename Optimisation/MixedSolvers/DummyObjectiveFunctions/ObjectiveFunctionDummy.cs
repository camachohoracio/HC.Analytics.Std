#region

using System;
using System.Linq;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers.DummyObjectiveFunctions
{
    [Serializable]
    public class ObjectiveFunctionDummy : IHeuristicObjectiveFunction
    {
        #region Events

        #region Delegates

        public delegate double EvaluateObjective(Individual individual,
            HeuristicProblem heuristicProblem);

        #endregion

        public event EvaluateObjective OnEvaluateObjective;

        #endregion

        #region Members

        private readonly int m_intVariableCount;

        #endregion

        #region Properties

        public string ObjectiveName { get; set; }

        #endregion

        #region Constructors

        public ObjectiveFunctionDummy(
            int intVariableCount)
        {
            m_intVariableCount = intVariableCount;
        }

        #endregion

        #region IHeuristicObjectiveFunction Members

        public double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            //
            // objective function evaluated by
            // multi-objective problem
            //
            return InvokeOnEvaluateObjective(individual, heuristicProblem);
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
            get { return m_intVariableCount; }
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        private double InvokeOnEvaluateObjective(
            Individual individual,
            HeuristicProblem heuristicProblem)
        {
            if (OnEvaluateObjective != null)
            {
                if (OnEvaluateObjective.GetInvocationList().Count() > 0)
                {
                    return OnEvaluateObjective.Invoke(individual, heuristicProblem);
                }
            }

            //Debugger.Break();
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
        }
    }
}
