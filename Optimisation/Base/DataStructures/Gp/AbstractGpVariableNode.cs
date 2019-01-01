#region

using System;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public abstract class AbstractGpVariableNode : AbstractGpNode
    {
        #region Constructors

        /// <summary>
        ///   Constructor used for serialization
        /// </summary>
        public AbstractGpVariableNode()
        {
        }

        public AbstractGpVariableNode(
            int depth,
            GpOperatorNode parent,
            GpOperatorsContainer gpOperatorsContainer) :
                base(gpOperatorsContainer)
        {
            IsOperatorNode = false;
            Parent = parent;
            Depth = depth;
        }

        #endregion
    }
}
