#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Constraints
{
    public class LinearConstraintInt : AbstractLinearConstraint
    {
        #region Constructors

        public LinearConstraintInt(
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

        public LinearConstraintInt(
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

        #region Protected methods

        protected override double GetChromosomeValue(
            Individual individual,
            int intIndex)
        {
            return individual.GetChromosomeValueInt(intIndex);
        }

        protected override void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            individual.AddChromosomeValueInt(
                intIndex,
                (int) Math.Round(dblWeight, 0),
                m_heuristicProblem);
        }

        protected override void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            individual.RemoveChromosomeValueInt(
                intIndex,
                (int) Math.Round(dblWeight, 0),
                m_heuristicProblem);
        }

        protected override double[] GetChromosomeCopy(
            Individual individual)
        {
            var intChromosomeArr = individual.GetChromosomeCopyInt();
            var dblChromosomeArr = new double[intChromosomeArr.Length];

            for (var i = 0; i < intChromosomeArr.Length; i++)
            {
                dblChromosomeArr[i] = intChromosomeArr[i];
            }

            return dblChromosomeArr;
        }

        protected override double GetMaxChromosomeValue(int intIndex)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
