#region

using System;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    [Serializable]
    public class TwoPointsCrossoverInt : AbstractTwoPointsCrossover
    {
        #region Constructor

        public TwoPointsCrossoverInt(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
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
            return m_heuristicProblem.VariableRangesIntegerProbl[intIndex];
        }

        protected override Individual CreateIndividual(double[] dblChromosomeArr)
        {
            var intChromosomeArr = new int[dblChromosomeArr.Length];
            for (var i = 0; i < dblChromosomeArr.Length; i++)
            {
                intChromosomeArr[i] = (int) dblChromosomeArr[i];
            }

            return new Individual(intChromosomeArr,
                                  m_heuristicProblem);
        }

        #endregion
    }
}
