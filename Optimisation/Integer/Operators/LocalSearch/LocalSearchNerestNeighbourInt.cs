#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch
{
    public class LocalSearchNerestNeighbourInt : AbstractNearNeigLocalSearch
    {
        #region Constructor

        public LocalSearchNerestNeighbourInt(
            HeuristicProblem heuristicProblem,
            int intSearchIterations) :
                base(heuristicProblem,
                     intSearchIterations)
        {
        }

        #endregion

        #region Protected methods

        protected override double GetChromosomeValue(
            Individual individual,
            int intIndex)
        {
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }
            return individual.GetChromosomeValueInt(intIndex);
        }

        protected override void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }
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
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }
            individual.RemoveChromosomeValueInt(
                intIndex,
                (int) Math.Round(dblWeight, 0),
                m_heuristicProblem);
        }

        protected override double[] GetChromosomeCopy(
            Individual individual)
        {
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }

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

        protected override double GetNearestNeighWeight(
            double dblChromosomeValue,
            int intIndex,
            bool blnGoForward,
            int intScaleIndex)
        {
            var dblWeight = Math.Max(
                Math.Round(MUTATION_FACTOR*dblChromosomeValue),
                1);

            if (blnGoForward)
            {
                dblWeight = Math.Min(
                    dblWeight,
                    m_heuristicProblem.VariableRangesIntegerProbl[intScaleIndex] - dblChromosomeValue);

                //
                // check that value is in the specified range
                //
                if (dblWeight > m_heuristicProblem.VariableRangesIntegerProbl[intScaleIndex] ||
                    dblWeight < 0)
                {
                    //Debugger.Break();
                    throw new HCException("Error. Value not valid: " + dblWeight);
                }
            }


            return dblWeight;
        }

        #endregion
    }
}
