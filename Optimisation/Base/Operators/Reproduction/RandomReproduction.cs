#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Reproduction
{
    public class RandomReproduction : AbstractReproduction
    {
        public RandomReproduction(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        public override Individual DoReproduction()
        {
            return m_heuristicProblem.IndividualFactory.BuildRandomIndividual();
        }

        public override void ClusterInstance(Individual individual)
        {
        }
    }
}
