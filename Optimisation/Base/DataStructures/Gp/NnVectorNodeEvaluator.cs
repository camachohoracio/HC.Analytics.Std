#region

using System;
using HC.Analytics.MachineLearning.NeuralNet;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class NnVectorNodeEvaluator : AbstractNodeEvaluator
    {
        #region Members

        public static double[,] VarNodeValues { get; set; }

        #endregion

        #region Public

        public override object EvaluateVarNode(
            AbstractGpVariable gpVariable,
            AbstractGpNode gpOperatorNode)
        {
            var nnOperatorNode = (NnOperatorNode) gpOperatorNode;

            if (nnOperatorNode.Weight > 2 ||
                nnOperatorNode.Weight < -2)
            {
                throw new HCException("variable out of bounds");
            }
            var dblVariableValue = VarNodeValues[(int)(double) gpVariable.Value,
                                                 nnOperatorNode.OperatorId];

            if (dblVariableValue > 2 ||
                dblVariableValue < -2)
            {
                throw new HCException("Variable out of bounds");
            }

            return nnOperatorNode.Weight*dblVariableValue;
        }

        #endregion
    }
}
