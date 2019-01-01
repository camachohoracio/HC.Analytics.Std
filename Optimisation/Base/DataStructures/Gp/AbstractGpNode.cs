#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public abstract class AbstractGpNode : IDisposable
    {
        #region Members

        public GpOperatorsContainer GpOperatorsContainer { get; protected set; }

        private bool m_blnIsDisposed;

        #endregion

        #region Properties

        [XmlElement("Depth", typeof (int))]
        public int Depth { get; set; }

        [XmlIgnore]
        public GpOperatorNode Parent { get; set; }

        [XmlElement("IsOperatorNode", typeof (bool))]
        public bool IsOperatorNode { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Used for xml serialization
        /// </summary>
        public AbstractGpNode()
        {
        }

        public AbstractGpNode(
            GpOperatorsContainer gpOperatorsContainer)
        {
            try
            {
                //if (gpOperatorsContainer == null)
                //{
                //    throw new HCException("Null operators container");
                //}
                GpOperatorsContainer = gpOperatorsContainer;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Abstract methods

        public abstract object Compute(AbstractGpVariable gpVariable);
        public abstract string ComputeToString();

        public abstract override string ToString();

        public void Dispose()
        {
            try
            {
                if (m_blnIsDisposed)
                {
                    return;
                }

                if (GpOperatorsContainer != null)
                {
                    GpOperatorsContainer.Dispose();
                    GpOperatorsContainer = null;
                }
                if (Parent != null)
                {
                    Parent.Dispose();
                    Parent = null;
                }
                EventHandlerHelper.RemoveAllEventHandlers(this);
                m_blnIsDisposed = true;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        ~AbstractGpNode()
        {
            Dispose();
        }

        public abstract void ToStringB(StringBuilder sb);
        public abstract void GetNodeList(List<AbstractGpNode> nodeList);

        public abstract AbstractGpNode Clone(
            GpOperatorNode parent,
            HeuristicProblem heuristicProblem);

        #endregion
    }
}
