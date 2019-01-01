#region

using System;
using System.Xml.Serialization;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpVarNodeFactory : AbstractGpVarNodeFactory
    {
        #region Members

        [XmlElement("GpOperatorsContainer_", typeof (GpOperatorsContainer))] public GpOperatorsContainer
            GpOperatorsContainer_;

        #endregion

        #region Constructor

        /// <summary>
        ///   Constructor used for serialization
        /// </summary>
        public GpVarNodeFactory()
        {
        }

        public GpVarNodeFactory(
            GpOperatorsContainer gpOperatorsContainer)
        {
            GpOperatorsContainer_ = gpOperatorsContainer;
        }

        #endregion

        public override AbstractGpVariableNode BuildVariable(
            int intDepth,
            GpOperatorNode parent)
        {
            return new GpVariableNode(
                intDepth,
                parent,
                GpOperatorsContainer_);
        }
    }
}
