#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Repair;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    [Serializable]
    public class RepairConstraintsInt : AbstractRepairConstraints
    {
        #region Constructor

        public RepairConstraintsInt(
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

        protected override bool ValidateAddVariable(int intIndex, Individual individual)
        {
            //
            // validate values to be added
            //
            var blnAddVariable = false;
            if (GetChromosomeValue(individual, intIndex) <
                (int) GetMaxChromosomeValue(intIndex))
            {
                blnAddVariable = true;
            }

            return blnAddVariable;
        }

        protected override bool ValidateRemoveVariable(int intIndex, Individual individual)
        {
            var blnAddVariable = false;
            if ((int) GetChromosomeValue(individual, intIndex) >
                0)
            {
                blnAddVariable = true;
            }

            return blnAddVariable;
        }

        #endregion
    }
}
