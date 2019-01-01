#region

using System;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public abstract class AbstractGpVarNodeFactory
    {
        #region Abstract Methods

        public abstract AbstractGpVariableNode BuildVariable(
            int intDepth,
            GpOperatorNode parent);

        #endregion
    }
}
