#region

using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch;
using HC.Analytics.Optimisation.ProblemFactories;

#endregion

namespace HC.Analytics.MachineLearning.NeuralNet.LocalSearchSolver
{
    public class NnRegressionSolver : GpRegressionProblemFactory
    {
        #region Properties

        public HeuristicProblem GpHeuristicProblem { get; private set; }
        public Individual BestIndividual { get; private set; }

        #endregion

        #region Events

        public delegate void CompletedGeneration(
            HeuristicProblem heuristicProblem);

        public event CompletedGeneration OnCompletedGeneration;

        #endregion

        #region Constructor

        public NnRegressionSolver(
            int intIterations,
            int intPopulation,
            AbstractGpBridge gpBridge,
            double dblCrossoverProbability,
            int intMaxTreeDepthMutation,
            int intMaxTreeSize,
            int intTournamentSize,
            GpOperatorsContainer gpOperatorsContainer)
            : base(
                intIterations,
                intPopulation,
                gpBridge,
                dblCrossoverProbability,
                intMaxTreeDepthMutation,
                intMaxTreeSize,
                intTournamentSize,
                gpOperatorsContainer)
        {
            GpHeuristicProblem =
                BuildProblem();
        }
        
        #endregion

        #region Public

        public void Solve()
        {
            AbstractGpNode gpVariableNode =
                NnTreeFactory.BuildNn(
                    NnConstants.INPUT_NODES,
                    NnConstants.HIDDEN_NODES,
                    GpHeuristicProblem.GpOperatorsContainer);

            Individual individual =
                new Individual(
                    GpHeuristicProblem,
                    GpHeuristicProblem.GpOperatorsContainer,
                    gpVariableNode);
            individual.Evaluate(
                false, 
                false, 
                true,
                GpHeuristicProblem);
            
            NnStratVarLocalSrch nnStratVarLocalSrch =
                new NnStratVarLocalSrch(
                    GpHeuristicProblem,
                    GpHeuristicProblem.GpOperatorsContainer);

            GpHeuristicProblem.LocalSearch =
                nnStratVarLocalSrch;

            nnStratVarLocalSrch.OnCompletedGeneration +=
                InvokeOnCompletedGeneration;
                
            nnStratVarLocalSrch.DoLocalSearch1(
                individual.Clone(
                GpHeuristicProblem));
        }

        #endregion

        #region Private

        private void InvokeOnCompletedGeneration(
            HeuristicProblem localSearchHeuristicProblem)
        {
            BestIndividual =
                ((AbstractGpVarObjFunction) 
                    localSearchHeuristicProblem.ObjectiveFunction).
                        BestIndividual;

            if(OnCompletedGeneration != null && 
                OnCompletedGeneration.GetInvocationList().Length > 0)
            {
                OnCompletedGeneration.Invoke(localSearchHeuristicProblem);
            }
        }

        #endregion
    }
}
