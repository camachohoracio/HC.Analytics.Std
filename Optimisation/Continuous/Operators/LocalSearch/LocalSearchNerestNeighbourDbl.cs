#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch
{
    public class LocalSearchNerestNeighbourDbl : AbstractNearNeigLocalSearch
    {
        #region Constructor

        public LocalSearchNerestNeighbourDbl(
            HeuristicProblem heuristicProblem,
            int intSearchIterations) :
                base(heuristicProblem,
                     intSearchIterations)
        {
        }

        #endregion

        #region Protected Methods

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

            return individual.GetChromosomeValueDbl(intIndex);
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
            individual.AddChromosomeValueDbl(
                intIndex,
                dblWeight);
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
            individual.RemoveChromosomeValueDbl(
                intIndex,
                dblWeight);
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

            return individual.GetChromosomeCopyDbl();
        }

        protected override double GetMaxChromosomeValue(int intIndex)
        {
            return 1.0;
        }

        protected override double GetNearestNeighWeight(
            double dblChromosomeValue,
            int intIndex,
            bool blnGoForward,
            int intScaleIndex)
        {
            var dblWeight = MUTATION_FACTOR*dblChromosomeValue;

            if (blnGoForward)
            {
                dblWeight = Math.Min(
                    dblWeight,
                    1.0 - dblChromosomeValue);
            }

            return dblWeight;
        }

        #endregion
    }
}
