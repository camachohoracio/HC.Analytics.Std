#region

using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Gp.GpOperators;

#endregion

namespace HC.Analytics.Optimisation.ProblemFactories
{
    public class GpRegressionProblemFactory : AHeuristicProblemFactory
    {
        #region Members

        private readonly double m_dblCrossoverProbability;
        private readonly int m_intIterations;
        private readonly int m_intMaxTreeDepthMutation;
        private readonly int m_intMaxTreeSize;
        private readonly int m_intPopulation;
        private readonly int m_intTournamentSize;

        #endregion

        #region Constructor

        public GpRegressionProblemFactory(
            int intIterations,
            int intPopulation,
            AbstractGpBridge gpBridge,
            double dblCrossoverProbability,
            int intMaxTreeDepthMutation,
            int intMaxTreeSize,
            int intTournamentSize,
            GpOperatorsContainer gpOperatorsContainer)
            : base(
                EnumOptimimisationPoblemType.GENETIC_PROGRAMMING,
                new GpObjectiveFunction(gpBridge))
        {
            m_gpBridge = gpBridge;
            m_gpOperatorsContainer = gpOperatorsContainer;
            m_intIterations = intIterations;
            m_intPopulation = intPopulation;
            m_intMaxTreeDepthMutation = intMaxTreeDepthMutation;
            m_dblCrossoverProbability = dblCrossoverProbability;
            m_intMaxTreeSize = intMaxTreeSize;
            m_intTournamentSize = intTournamentSize;
        }

        #endregion

        public new HeuristicProblem BuildProblem()
        {
            var heuristicProblem =
                base.BuildProblem();

            heuristicProblem.Iterations = m_intIterations;
            heuristicProblem.PopulationSize = m_intPopulation;

            heuristicProblem.GpOperatorsContainer = m_gpOperatorsContainer;
            heuristicProblem.GpBridge = m_gpBridge;
            heuristicProblem.LocalSearch = null;
            heuristicProblem.GuidedConvergence = null;

            //m_gpOperatorsContainer.NumParameters = m_intNumParameters;

            heuristicProblem.IndividualFactory =
                new GpIndividualFatory(
                    heuristicProblem,
                    m_gpOperatorsContainer);

            heuristicProblem.Reproduction = new GpReproduction(
                heuristicProblem,
                m_dblCrossoverProbability,
                m_intMaxTreeDepthMutation,
                m_intMaxTreeSize,
                m_intTournamentSize,
                m_gpOperatorsContainer);

            heuristicProblem.RepairIndividual = null;

            heuristicProblem.Threads = 0;
            return heuristicProblem;
        }
    }
}
