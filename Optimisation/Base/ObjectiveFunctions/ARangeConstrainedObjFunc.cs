#region

using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public abstract class ARangeConstrainedObjFunc
    {
        public ConstraintClass Constraints { get; set; }

        public ARangeConstrainedObjFunc()
        {
            Constraints = new ConstraintClass();
        }

        public virtual bool CheckConstraint(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            return Constraints.CheckConstraints(individual);
        }

        public void AddConstraint(IConstraint constraint)
        {
            Constraints.AddConstraint(constraint);
        }
    }
}
