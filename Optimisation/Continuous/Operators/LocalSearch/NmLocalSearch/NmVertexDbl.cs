#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch.NmLocalSearch
{
    public class NmVertexDbl : AbstractNmVertex
    {
        #region Constructors

        public NmVertexDbl(
            Individual individual,
            HeuristicProblem heuristicProblem) :
                base(
                individual,
                heuristicProblem)
        {
        }

        #endregion

        #region Private & protected methods

        protected override AbstractNmVertex CreateNmVertex()
        {
            return new NmVertexDbl(
                m_individual.Clone(m_heuristicProblem),
                m_heuristicProblem);
        }

        protected override void SetChromosomeValue(int intIndex, double dblValue)
        {
            var individual = m_individual;
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }
            individual.SetChromosomeValueDbl(intIndex, dblValue);
        }

        protected override double[] GetChromosomeCopy(Individual individual)
        {
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }
            return individual.GetChromosomeCopyDbl();
        }

        #endregion
    }
}
