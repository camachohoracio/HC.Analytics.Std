#region

using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Constraints
{
    public class LinearConstraintDbl : AbstractLinearConstraint
    {
        #region Constructors

        public LinearConstraintDbl(
            double[] dblCoefficients,
            int[] intIndexes,
            InequalityType inequality,
            double dblBoundary) :
                base(
                dblCoefficients,
                intIndexes,
                inequality,
                dblBoundary)
        {
        }

        public LinearConstraintDbl(
            double[] dblCoefficients,
            double[] dblScaleArr,
            int[] intIndexes,
            InequalityType inequality,
            double dblBoundary,
            HeuristicProblem heuristicProblem) :
                base(
                dblCoefficients,
                dblScaleArr,
                intIndexes,
                inequality,
                dblBoundary,
                heuristicProblem)
        {
        }

        #endregion

        protected override double GetChromosomeValue(
            Individual individual,
            int intIndex)
        {
            return individual.GetChromosomeValueDbl(intIndex);
        }

        protected override void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            individual.AddChromosomeValueDbl(
                intIndex,
                dblWeight);
        }

        protected override void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            individual.RemoveChromosomeValueDbl(
                intIndex,
                dblWeight);
        }

        protected override double[] GetChromosomeCopy(
            Individual individual)
        {
            return individual.GetChromosomeCopyDbl();
        }

        protected override double GetMaxChromosomeValue(int intIndex)
        {
            return 1.0;
        }
    }
}
