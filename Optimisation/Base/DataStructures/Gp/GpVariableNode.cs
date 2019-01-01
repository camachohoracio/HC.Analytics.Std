#region

using System;
using System.Collections.Generic;
using System.Text;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    /// <summary>
    ///   Defines a variable node. The variable belongs to a given function
    /// </summary>
    [Serializable]
    public class GpVariableNode : AbstractGpVariableNode
    {
        #region Constructors

        /// <summary>
        ///   Constructor used for serialization
        /// </summary>
        public GpVariableNode()
        {
        }

        public GpVariableNode(
            int depth,
            GpOperatorNode parent,
            GpOperatorsContainer gpOperatorsContainer) :
                base(depth,
                     parent,
                     gpOperatorsContainer)
        {
            IsOperatorNode = false;
            Parent = parent;
            Depth = depth;
        }

        #endregion

        #region Public

        public override object Compute(AbstractGpVariable gpVariable)
        {
            return gpVariable.GetValue();
        }

        public override AbstractGpNode Clone(
            GpOperatorNode parent,
            HeuristicProblem heuristicProblem)
        {
            var newNode = new GpVariableNode(
                Depth,
                parent,
                GpOperatorsContainer);
            newNode.Depth = Depth;
            newNode.Parent = parent;
            newNode.IsOperatorNode = false;
            return newNode;
        }

        public override string ToString()
        {
            return " x ";
        }

        public override void ToStringB(StringBuilder sb)
        {
            sb.Append(ToString());
        }

        public override void GetNodeList(List<AbstractGpNode> nodeList)
        {
            if (nodeList.Contains(this))
            {
                //Debugger.Break();
            }

            nodeList.Add(this);
        }

        public override string ComputeToString()
        {
            return ToString();
        }

        #endregion
    }
}
