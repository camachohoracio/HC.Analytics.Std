#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch;
using HC.Analytics.Optimisation.MixedSolvers;

#endregion

namespace HC.Analytics.MachineLearning.NeuralNet.LocalSearchSolver
{
    [Serializable]
    public class NnStratVarLocalSrch : AbstractGpLocalSearch
    {
        #region Constructors

        public NnStratVarLocalSrch(
            HeuristicProblem heuristicProblem,
            GpOperatorsContainer gpOperatorsContainer) :
                base(heuristicProblem,
                gpOperatorsContainer)
        {
        }

        #endregion

        protected override MixedHeurPrblmFctGeneric GetProblemFactory(
            List<AbstractGpNode> varNodeList, 
            AbstractGpNode newRootNode, 
            int intChromosomeLengthInteger, 
            int intChromosomeLengthContinuous)
        {
            int intVarCount =
                varNodeList.Count;

            AbstractGpVarObjFunction gpVarObjFunction = 
                BuildObjectiveFunction(
                    intVarCount,
                    newRootNode,
                    m_gpOperatorsContainer,
                    m_heuristicProblem,
                    varNodeList);
            
            var gpStratVarProblemFactory =
                new GpStratVarProblemFactory(
                    SOLVER_THREADS,
                    SOLVER_POPULATION_SIZE,
                    SOLVER_ITERATIONS,
                    intChromosomeLengthInteger,
                    intChromosomeLengthContinuous,
                    gpVarObjFunction);

            return gpStratVarProblemFactory;
        }

        protected override AbstractGpVarObjFunction BuildObjectiveFunction(
            int intVariableCount,
            AbstractGpNode root,
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem baseHeuristicProblem,
            List<AbstractGpNode> varNodeList)
        {
            return new NnVarObjFunct(
                root,
                gpOperatorsContainer,
                baseHeuristicProblem,
                intVariableCount,
                varNodeList);
        }

        public override List<AbstractGpNode> GetVariableNodeList(
            AbstractGpNode rootNode)
        {
            List<AbstractGpNode> nodeList = new List<AbstractGpNode>();
            rootNode.GetNodeList(nodeList);

            List<AbstractGpNode> getVariableNodeList =
                new List<AbstractGpNode>();

            foreach (AbstractGpNode currentNode in nodeList)
            {
                if (currentNode is NnOperatorNode)
                {
                    NnOperatorNode nnOperatorNode =
                        (NnOperatorNode)currentNode;

                    getVariableNodeList.Add(nnOperatorNode);
                }
            }
            return getVariableNodeList;
        }

        protected override void GetLowerBoundChromosomes(
            List<AbstractGpNode> timeSeriesNodeList,
            out int[] intChromosomeArr,
            out double[] dblChromosomeArr)
        {
            //
            // initialize problem with the individual provided
            //
            List<int> intChromosomeList = new List<int>();
            List<double> dblChromosomeList = new List<double>();

             for (int i = 0; i < timeSeriesNodeList.Count; i++)
            {
                NnOperatorNode timeSeriesVariableNode =
                    (NnOperatorNode)timeSeriesNodeList[i];
                    
                // factor weights
                double dblChromosomeWeight =
                    (timeSeriesVariableNode.Weight - NnConstants.NN_MIN_VALUE) / 
                    (NnConstants.NN_MAX_VALUE - NnConstants.NN_MIN_VALUE);

                dblChromosomeList.Add(
                    dblChromosomeWeight);
            }

            intChromosomeArr = null;
            if (intChromosomeList.Count > 1)
            {
                intChromosomeArr = intChromosomeList.ToArray();
            }

            dblChromosomeArr = null;
            if (dblChromosomeList.Count > 1)
            {
                dblChromosomeArr = dblChromosomeList.ToArray();
            }
        }

    }
}
