#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch.NmLocalSearch
{
    public class LocalSearchNmDbl : AbstractLocalSearchNm
    {
        #region Constructors

        public LocalSearchNmDbl(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_nmPopulationGenerator = new NmPopulationGeneratorDbl(heuristicProblem);
        }

        #endregion

        #region Protected Methods

        protected override NmSolver LoadNmSolver()
        {
            return new NmSolver(
                m_heuristicProblem,
                m_nmPopulationGenerator);
        }

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

        protected override Individual BuildIndividual(
            double[] dblChromosomeArr,
            double dblFitness)
        {
            return new Individual(
                dblChromosomeArr,
                dblFitness,
                m_heuristicProblem);
        }
    }
}
