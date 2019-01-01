#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public abstract class AbstractHeuristicObjectiveFunction : IHeuristicObjectiveFunction
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;

        #endregion

        #region Properties

        public ObjectiveFunctionType ObjectiveFunctionType { get; set; }
        public string ObjectiveName { get; set; }

        #endregion

        #region Constructors

        public AbstractHeuristicObjectiveFunction(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region Abstract Methods

        public abstract double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem);

        public abstract double Evaluate();

        public abstract int VariableCount { get; }

        public abstract string GetVariableDescription(int intIndex);

        #endregion

        public virtual void Dispose()
        {
        }
    }
}
