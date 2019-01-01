#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Repair;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators
{
    [Serializable]
    public class RepairConstraintsDbl : AbstractRepairConstraints
    {
        #region Constructor

        public RepairConstraintsDbl(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        #region Protected Methdos

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

        protected override bool ValidateAddVariable(int intIndex, Individual individual)
        {
            //
            // validate values to be added
            //
            var blnAddVariable = false;
            if (GetChromosomeValue(individual, intIndex) <
                GetMaxChromosomeValue(intIndex) -
                MathConstants.DBL_ROUNDING_FACTOR)
            {
                blnAddVariable = true;
            }

            return blnAddVariable;
        }

        protected override bool ValidateRemoveVariable(int intIndex, Individual individual)
        {
            var blnAddVariable = false;
            if (GetChromosomeValue(individual, intIndex) >
                MathConstants.DBL_ROUNDING_FACTOR)
            {
                blnAddVariable = true;
            }

            return blnAddVariable;
        }

        #endregion
    }
}
