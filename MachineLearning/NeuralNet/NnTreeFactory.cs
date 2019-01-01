#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Gp.GpOperators.StdOperators;

#endregion

namespace HC.Analytics.MachineLearning.NeuralNet
{
    public static class NnTreeFactory
    {
        public static AbstractGpNode BuildNn(
            int intInputNodes,
            int intHiddenNodes,
            GpOperatorsContainer gpOperatorsContainer)
        {
            return BuildNn(
                intInputNodes,
                intHiddenNodes,
                gpOperatorsContainer,
                new List<NnOperatorNode>());
        }

        public static AbstractGpNode BuildNn(
            int intInputNodes,
            int intHiddenNodes,
            GpOperatorsContainer gpOperatorsContainer,
            List<NnOperatorNode> extraInputNodes)
        {
            //
            // build a two layers neural network
            //
            NnOperatorNode root =
                new NnOperatorNode();
            root.ChildrenArr =
                new AbstractGpNode[intHiddenNodes];

            for (int j = 0; j < intHiddenNodes; j++)
            {
                // generate hidden node
                NnOperatorNode nnHiddenOperatorNode =
                    new NnOperatorNode();
                root.ChildrenArr[j] = nnHiddenOperatorNode;
                nnHiddenOperatorNode.ChildrenArr =
                    new AbstractGpNode[intInputNodes + 1];
                
                //
                // add operator to hidden layer
                //
                nnHiddenOperatorNode.GpOperator =
                    new Tanh();

                //
                // add terminal nodes
                //
                for (int i = 0; i < intInputNodes; i++)
                {
                    NnOperatorNode nnInputOperatorNode =
                        new NnOperatorNode(gpOperatorsContainer);
                    nnHiddenOperatorNode.ChildrenArr[i] =
                        nnInputOperatorNode;
                }

                //
                // add extra nodes
                //

                for (int i = intInputNodes, inputIndex = 0; 
                    i < intInputNodes + extraInputNodes.Count; 
                    i++, inputIndex++)
                {
                    nnHiddenOperatorNode.ChildrenArr[i] =
                        extraInputNodes[inputIndex];
                }

                //
                // add vias node
                //
                NnOperatorNode nnViasNode =
                    new NnOperatorNode();
                nnViasNode.IsViasNode = true;
                nnHiddenOperatorNode.ChildrenArr[intInputNodes] =
                    nnViasNode;
            }
            return root;
        }
    }
}

