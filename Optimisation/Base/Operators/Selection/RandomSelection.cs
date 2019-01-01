#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Selection
{
    [Serializable]
    public class RandomSelection : AbstractSelection
    {
        #region Constructor

        public RandomSelection(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override Individual DoSelection()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            var intIndividualIndex =
                rng.NextInt(0, m_heuristicProblem.PopulationSize - 1);
            return m_heuristicProblem.Population.GetIndividualFromPopulation(
                m_heuristicProblem,
                intIndividualIndex);
        }
    }
}
