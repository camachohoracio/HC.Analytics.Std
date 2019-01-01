#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch;

#endregion

namespace HC.Analytics.MachineLearning.NeuralNet.LocalSearchSolver
{
    public class NnVarObjFunct : AbstractGpVarObjFunction
    {
        #region Constructors

        public NnVarObjFunct(
            AbstractGpNode root,
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem baseHeuristicProblem,
            int intVariableCount,
            List<AbstractGpNode> varNodeList) :
                base(root,
                    gpOperatorsContainer,
                    baseHeuristicProblem,
                    intVariableCount,
                    varNodeList)
        {
        }

        #endregion

        #region Public

        public override Individual CreateIndividualForParentProblem(
            AbstractGpNode root,
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem baseHeuristicProblem,
            int[] intChromosomeArr,
            double[] dblChromosomeArr)
        {
            AbstractGpNode newRoot = root.Clone(
                null,
                baseHeuristicProblem);

            List<AbstractGpNode> varNodeList =
                ((AbstractGpLocalSearch)baseHeuristicProblem.LocalSearch).GetVariableNodeList(
                    newRoot);

            //
            // set new paramaters to the time series
            //
            for (int i = 0; i < varNodeList.Count; i++)
            {
                NnOperatorNode nnOperatorNode =
                    (NnOperatorNode)varNodeList[i];

                nnOperatorNode.Weight = NnConstants.NN_MIN_VALUE +
                    dblChromosomeArr[i] *
                    (NnConstants.NN_MAX_VALUE - NnConstants.NN_MIN_VALUE);
            }

            //
            // Generate a new individual from the base heuristic problem
            //
            Individual newIndividual = new Individual(
                baseHeuristicProblem,
                gpOperatorsContainer,
                newRoot);
            return newIndividual;
        }

        #endregion
    }
}
