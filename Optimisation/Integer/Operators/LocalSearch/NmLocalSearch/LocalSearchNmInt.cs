#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch.NmLocalSearch
{
    public class LocalSearchNmInt : AbstractLocalSearchNm
    {
        #region Constructors

        public LocalSearchNmInt(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_nmPopulationGenerator = new NmPopulationGeneratorInt(
                heuristicProblem);
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

        protected override NmSolver LoadNmSolver()
        {
            return new NmSolver(
                m_heuristicProblem,
                m_nmPopulationGenerator);
        }

        #endregion

        protected override Individual BuildIndividual(double[] dblChromosomeArr, double dblFitness)
        {
            //
            // create int chromosome
            //
            var intChromosomeArr = new int[dblChromosomeArr.Length];
            for (var i = 0; i < dblChromosomeArr.Length; i++)
            {
                intChromosomeArr[i] = (int) Math.Round(dblChromosomeArr[i], 0);
            }

            return new Individual(
                intChromosomeArr,
                dblFitness,
                m_heuristicProblem);
        }
    }
}
