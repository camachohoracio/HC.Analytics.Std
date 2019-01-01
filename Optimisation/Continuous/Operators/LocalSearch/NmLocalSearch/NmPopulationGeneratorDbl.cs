#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.LocalSearch.NmLocalSearch
{
    public class NmPopulationGeneratorDbl : AbstractNmPopulationGenerator
    {
        #region Constructors

        public NmPopulationGeneratorDbl(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        #region Public

        public override string ToStringIndividual(Individual individual)
        {
            return individual.ToStringDbl();
        }

        protected override double[] GetChromosomeCopy(
            Individual individual)
        {
            return individual.GetChromosomeCopyDbl();
        }

        #endregion

        public override AbstractNmVertex CreateNmVertex(
            int intDimensions,
            Individual individual)
        {
            return new NmVertexDbl(
                individual.Clone(m_heuristicProblem),
                m_heuristicProblem);
        }
    }
}
