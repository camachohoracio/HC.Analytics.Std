#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Gp.GpOperators;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpConstantNode : AbstractGpNode
    {
        #region Properties

        [XmlElement("GpConstant_", typeof (GpConstants))]
        public GpConstants GpConstant_
        {
            get { return m_gpConstant; }
            set { m_gpConstant = value; }
        }

        #endregion

        #region Members

        private GpConstants m_gpConstant;

        #endregion

        #region Constructors

        public GpConstantNode()
        {
        }

        public GpConstantNode(
            GpConstants constant,
            int depth,
            GpOperatorNode parent,
            GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            Parent = parent;
            m_gpConstant = constant;
            Depth = depth;
            IsOperatorNode = false;
        }

        public GpConstantNode(
            GpOperatorsContainer gpOperatorsContainer) :
                base(gpOperatorsContainer)
        {
            // TODO Auto-generated constructor stub
        }

        #endregion

        public override object Compute(AbstractGpVariable gpVariable)
        {
            //
            // return the value of the constant
            //
            return m_gpConstant.GetValue();
        }

        public override string ToString()
        {
            return m_gpConstant.ToString();
        }

        public override void ToStringB(StringBuilder sb)
        {
            m_gpConstant.ToStringB(sb);
        }

        public override AbstractGpNode Clone(
            GpOperatorNode parent,
            HeuristicProblem heuristicProblem)
        {
            try
            {
                if (GpOperatorsContainer == null)
                {
                    GpOperatorsContainer =
                        ((GpObjectiveFunction) heuristicProblem.ObjectiveFunction).GpBridge.GpOperatorsContainer;
                }
                var newNode = new GpConstantNode(GpOperatorsContainer);
                newNode.Depth = Depth;
                newNode.Parent = parent;
                newNode.m_gpConstant = m_gpConstant;
                newNode.IsOperatorNode = false;
                return newNode;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
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
    }
}
