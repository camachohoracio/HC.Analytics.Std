#region

using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories
{
    public class GpIndividualFatory : AbstractIndividualFactory
    {
        #region Members

        private readonly GpOperatorsContainer m_gpOperatorsContainer;

        #endregion

        public GpIndividualFatory(
            HeuristicProblem heuristicProblem,
            GpOperatorsContainer gpOperatorsContainer)
            : base(
                heuristicProblem)
        {
            m_gpOperatorsContainer = gpOperatorsContainer;
        }

        public override Individual BuildRandomIndividual()
        {
            return new Individual(
                m_gpOperatorsContainer,
                m_heuristicProblem);
        }
    }
}
