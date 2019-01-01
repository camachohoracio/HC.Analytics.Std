#region

using System;
using System.Collections.Generic;
using HC.Analytics.MachineLearning.NeuralNet.LocalSearchSolver;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Gp.GpOperators;
using HC.Analytics.Optimisation.ProblemFactories;

#endregion

namespace HC.Analytics.MachineLearning.NeuralNet
{
    public class NnProblemFactory : 
        AHeuristicProblemFactory
    {
        #region Members

        private readonly GpRegressionProblemFactory m_gpRegressionProblemFactory;
        private List<NnOperatorNode> m_operatorNodes;
        private int m_intInputNodes;

        #endregion

        #region Constructors

        public NnProblemFactory(
            EnumOptimimisationPoblemType problemType, 
            IHeuristicObjectiveFunction objectiveFunction) : base(
                problemType, 
                objectiveFunction)
        {
            throw new NotImplementedException();
        }

        public NnProblemFactory(
            EnumOptimimisationPoblemType problemType, 
            IHeuristicObjectiveFunction objectiveFunction, 
            GpOperatorsContainer gpOperatorsContainer, 
            AbstractGpBridge gpBridge) : base(
                problemType, 
                objectiveFunction, 
                gpOperatorsContainer, 
                gpBridge,
                false)
        {
            throw new NotImplementedException();
        }

        public NnProblemFactory(
            int intIterations,
            int intPopulation,
            AbstractGpBridge gpBridge,
            double dblCrossoverProb,
            int intMaxTreeDepthMutation,
            int intMaxTreeSize,
            int intTournamentSize,
            GpOperatorsContainer gpOperatorsContainer,
            List<NnOperatorNode> operatorNodes,
            int intInputNodes)
            : base(
                EnumOptimimisationPoblemType.GENETIC_PROGRAMMING,
                new GpObjectiveFunction(gpBridge),
                gpOperatorsContainer,
                gpBridge,
                false)
        {
            m_intInputNodes = intInputNodes;
            m_operatorNodes = operatorNodes;
            m_gpRegressionProblemFactory =
                new GpRegressionProblemFactory(
                    intIterations,
                    intPopulation,
                    gpBridge,
                    dblCrossoverProb,
                    intMaxTreeDepthMutation,
                    intMaxTreeSize,
                    intTournamentSize,
                    gpOperatorsContainer);
        }

        #endregion

        #region Public

        public override HeuristicProblem BuildProblem()
        {
            HeuristicProblem gpHeuristicProblem =
                m_gpRegressionProblemFactory.BuildProblem();

            //
            // build neural network
            //
            AbstractGpNode gpVariableNode =
                NnTreeFactory.BuildNn(
                    m_intInputNodes,
                    NnConstants.HIDDEN_NODES,
                    gpHeuristicProblem.GpOperatorsContainer,
                    m_operatorNodes);

            //
            // evaluate lower bound individual
            //
            Individual individual =
                new Individual(
                    gpHeuristicProblem,
                    gpHeuristicProblem.GpOperatorsContainer,
                    gpVariableNode);

            //individual.Evaluate(
            //    false,
            //    false,
            //    true,
            //    gpHeuristicProblem);

            NnStratVarLocalSrch nnStratVarLocalSrch =
                new NnStratVarLocalSrch(
                    gpHeuristicProblem,
                    gpHeuristicProblem.GpOperatorsContainer);

            HeuristicProblem localSearchHeuristicProblem =
                nnStratVarLocalSrch.BuildLocalSearchHeuristicProblem(
                    individual);
            return localSearchHeuristicProblem;
        }

        #endregion
    }
}

