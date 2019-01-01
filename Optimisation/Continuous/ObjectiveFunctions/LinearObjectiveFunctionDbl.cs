#region

using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Continuous.ObjectiveFunctions
{
    public class LinearObjectiveFunctionDbl : AbstractLinearObjectiveFunction
    {
        #region Constructors

        public LinearObjectiveFunctionDbl(
            double[] dblReturnArray,
            double[] dblScaleArr,
            HeuristicProblem heuristicProblem) :
                this(dblReturnArray,
                     dblScaleArr,
                     null,
                     heuristicProblem)
        {
        }

        public LinearObjectiveFunctionDbl(
            double[] dblReturnArray,
            double[] dblScaleArr,
            int[] intIndexes,
            HeuristicProblem heuristicProblem) :
                base(
                dblReturnArray,
                dblScaleArr,
                intIndexes,
                heuristicProblem)

        {
        }

        #endregion

        #region Protected methods

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

        #endregion
    }
}
