#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators.LocalSearch.NmLocalSearch
{
    public class NmVertexInt : AbstractNmVertex
    {
        #region Constructors

        public NmVertexInt(
            Individual individual,
            HeuristicProblem heuristicProblem) :
                base(
                individual,
                heuristicProblem)
        {
        }

        #endregion

        protected override AbstractNmVertex CreateNmVertex()
        {
            return new NmVertexInt(
                m_individual.Clone(m_heuristicProblem),
                m_heuristicProblem);
        }

        public override void SetVertexValue(
            int intIndex,
            double dblValue)
        {
            m_dblCoordinatesArr[intIndex] = dblValue;
            SetChromosomeValue(intIndex, dblValue);
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

            var intValue =
                Math.Max(0,
                         Math.Min(
                             (int) m_heuristicProblem.VariableRangesIntegerProbl[intIndex],
                             (int) Math.Round(dblValue, 0)));

            individual.SetChromosomeValueInt(intIndex, intValue);
        }

        protected override double[] GetChromosomeCopy(Individual individual)
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
    }
}
