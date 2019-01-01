#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch.NmLocalSearch
{
    public class NmPopulationGeneratorInt : AbstractNmPopulationGenerator
    {
        #region Constructors

        public NmPopulationGeneratorInt(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        #region Public

        public override string ToStringIndividual(Individual individual)
        {
            return individual.ToStringInt();
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

        #endregion

        public override AbstractNmVertex CreateNmVertex(
            int intDimensions,
            Individual individual)
        {
            return new NmVertexInt(
                individual.Clone(m_heuristicProblem),
                m_heuristicProblem);
        }
    }
}
