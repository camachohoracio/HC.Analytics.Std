#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public abstract class AEqualityConstrainObjFunc : IHeuristicObjectiveFunction
    {
        #region Properties

        public double LowValue { get; set; }
        public double HighValue { get; set; }
        public double TargetValue { get; set; }
        public string ObjectiveName { get; set; }

        #endregion

        #region Constructors

        public AEqualityConstrainObjFunc()
        {
            LowValue = -double.MaxValue;
            HighValue = double.MaxValue;
        }

        #endregion

        #region Abstract methods

        public abstract bool CheckConstraint(Individual individual);

        #endregion


        public double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            throw new System.NotImplementedException();
        }

        public ObjectiveFunctionType ObjectiveFunctionType
        {
            get { throw new System.NotImplementedException(); }
        }

        public int VariableCount
        {
            get { throw new System.NotImplementedException(); }
        }

        public double Evaluate()
        {
            throw new System.NotImplementedException();
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}

