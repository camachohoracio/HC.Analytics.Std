#region

using System;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.ObjectiveFunctions
{
    public class LinearObjectiveFunctionInt : AbstractLinearObjectiveFunction
    {
        #region Members

        private HeuristicProblem m_heuristicProblem;

        #endregion

        #region Constructors

        public LinearObjectiveFunctionInt(
            double[] dblReturnArray,
            double[] dblScaleArr,
            HeuristicProblem heuristicProblem) :
                this(dblReturnArray,
                     dblScaleArr,
                     null,
                     heuristicProblem)
        {
        }

        public LinearObjectiveFunctionInt(
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
