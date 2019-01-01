#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpOperatorNode : AbstractGpNode
    {
        #region Members

        protected AbstractGpNode[] m_childrenArr;

        #endregion

        #region Properties

        [XmlArray("ChildrenArr")]
        [XmlArrayItem("ChildGpNode", typeof (AbstractGpNode))]
        public AbstractGpNode[] ChildrenArr
        {
            get { return m_childrenArr; }
            set { m_childrenArr = value; }
        }

        [XmlArray("ParameterValuesArr")]
        [XmlArrayItem("ParameterValue", typeof (object))]
        public object[] ParameterValuesArr { get; set; }

        [XmlElement("GpOperator", typeof (AbstractGpOperator))]
        public AbstractGpOperator GpOperator { get; set; }

        [XmlArray("ParameterDescrArr")]
        [XmlArrayItem("ParameterDescr", typeof (string))]
        public string[] ParameterDescrArr { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor used for xml serialization
        /// </summary>
        public GpOperatorNode()
        {
        }

        public GpOperatorNode(
            GpOperatorNode parent,
            int intDepth,
            int intMaxDepth,
            RngWrapper random,
            bool blnFullTree,
            ref int intSize,
            int intMaxTreeSize,
            GpOperatorsContainer gpOperatorsContainer,
            AbstractGpOperator randomOperator) :
                this(
                randomOperator,
                intDepth,
                parent,
                gpOperatorsContainer)
        {
            try
            {
                m_childrenArr = new AbstractGpNode[GpOperator.NumbParameters];
                GpOperatorsContainer = gpOperatorsContainer;
                if (intDepth >= intMaxDepth - 1 || 
                    intSize >= intMaxTreeSize - gpOperatorsContainer.MinNumParams)
                {
                    //
                    // end of the tree
                    // m_childrenArr have to be constants or
                    // variables only
                    //
                    for (var i = 0; i < m_childrenArr.Length; i++)
                    {
                        m_childrenArr[i] = CreateEndNodeChild(random, intDepth + 1);
                    }
                }
                else
                {
                    //
                    // m_childrenArr can be constants, terminals or operators
                    //
                    for (var i = 0; i < m_childrenArr.Length; i++)
                    {
                        int intNodesAvailable = intMaxTreeSize - intSize - 2 * gpOperatorsContainer.MinNumParams;
                        bool blnIsTreeFull = false;
                        AbstractGpOperator newRandomOperator = null;
                        if (intNodesAvailable <= 0)
                        {
                            blnIsTreeFull = true;
                        }
                        else
                        {
                            //
                            // randomly pick up an operator (sum, product, etc)
                            //
                            newRandomOperator = SelectRandomOpeator(
                                random,
                                gpOperatorsContainer,
                                Math.Max(intNodesAvailable, gpOperatorsContainer.MinNumParams));
                        }
                        //
                        // check if it is a full size tree
                        //
                        if (blnFullTree && !blnIsTreeFull)
                        {
                            intSize += newRandomOperator.NumbParameters*2;
                            m_childrenArr[i] = new GpOperatorNode(
                                this,
                                intDepth + 1,
                                intMaxDepth,
                                random,
                                true,
                                ref intSize,
                                intMaxTreeSize,
                                gpOperatorsContainer,
                                newRandomOperator);
                        }
                        else
                        {
                            //
                            // decide if the node is an operator or a terminal one
                            //
                            if (random.NextDouble() < GpConstants.OPERATOR_PROB && !blnIsTreeFull)
                            {
                                intSize += newRandomOperator.NumbParameters*2;
                                m_childrenArr[i] = new GpOperatorNode(
                                    this,
                                    intDepth + 1,
                                    intMaxDepth,
                                    random,
                                    false,
                                    ref intSize,
                                    intMaxTreeSize,
                                    gpOperatorsContainer,
                                    newRandomOperator);
                            }
                            else
                            {
                                m_childrenArr[i] = CreateEndNodeChild(
                                    random,
                                    intDepth + 1);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static AbstractGpOperator SelectRandomOpeator(
            RngWrapper random, 
            GpOperatorsContainer gpOperatorsContainer,
            int intNodesAvailable)
        {
            try
            {
                AbstractGpOperator rngOperator = gpOperatorsContainer.GpOperatorArr[
                    random.NextInt(
                        0,
                        gpOperatorsContainer.GpOperatorArr.Length - 1)];

                while (rngOperator.NumbParameters > intNodesAvailable)
                {
                    rngOperator = gpOperatorsContainer.GpOperatorArr[
                        random.NextInt(
                            0,
                            gpOperatorsContainer.GpOperatorArr.Length - 1)];
                }
                return rngOperator;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        protected GpOperatorNode(
            AbstractGpOperator gpOperator,
            int intDepth,
            GpOperatorNode parent,
            GpOperatorsContainer gpOperatorsContainer) :
                base(gpOperatorsContainer)
        {
            IsOperatorNode = true;
            GpOperator = gpOperator;
            Parent = parent;
            Depth = intDepth;

            if (gpOperator != null)
            {
                ParameterValuesArr = new object[gpOperator.NumbParameters];
                ParameterDescrArr = new string[gpOperator.NumbParameters];
            }
        }

        public GpOperatorNode(GpOperatorsContainer gpOperatorsContainer) :
            base(gpOperatorsContainer)
        {
        }

        #endregion

        #region Public

        /// <summary>
        ///   Compute each of the nodes
        /// </summary>
        /// <returns></returns>
        public override object Compute(AbstractGpVariable gpVariable)
        {
            for (var i = 0; i < ParameterValuesArr.Length; i++)
            {
                ParameterValuesArr[i] = m_childrenArr[i].Compute(gpVariable);
            }
            //
            // compute current operator node
            //
            return GpOperator.Compute(ParameterValuesArr);
        }

        public override string ComputeToString()
        {
            if (ParameterDescrArr == null)
            {
                ParameterDescrArr = new string[ParameterValuesArr.Length];
            }
            for (var i = 0; i < ParameterValuesArr.Length; i++)
            {
                ParameterDescrArr[i] = m_childrenArr[i].ComputeToString();
            }
            //
            // compute current operator node
            //
            return GpOperator.ComputeToString(ParameterDescrArr);
        }

        public override string ToString()
        {
            return ComputeToString();
        }

        public override void ToStringB(StringBuilder sb)
        {
            sb.Append(GpOperator + " ");
            for (var i = 0; i < m_childrenArr.Length; i++)
            {
                m_childrenArr[i].ToStringB(sb);
            }
        }

        public override void GetNodeList(
            List<AbstractGpNode> nodeList)
        {
            nodeList.Add(this);
            for (var i = 0; i < m_childrenArr.Length; i++)
            {
                m_childrenArr[i].GetNodeList(nodeList);
            }
        }

        /// <summary>
        ///   Creates a deep clone of the current node
        /// </summary>
        /// <returns></returns>
        //public AbstractGpNode Clone()
        //{
        //    GpOperatorNode newNode = new GpOperatorNode(m_gpOperatorsContainer);
        //    newNode.ParameterValuesArr = (double[]) ParameterValuesArr.Clone();
        //    newNode.ParameterDescrArr = (string[]) ParameterDescrArr.Clone();
        //    newNode.Depth = Depth;
        //    newNode.GpOperator = GpOperator;
        //    newNode.Parent = null;
        //    newNode.IsOperatorNode = true;
        //    AbstractGpNode[] children = new AbstractGpNode[m_childrenArr.Length];
        //    for (int i = 0; i < children.Length; i++)
        //    {
        //        children[i] = m_childrenArr[i].Clone(
        //            newNode);
        //    }
        //    newNode.m_childrenArr = children;
        //    return newNode;
        //}
        public override AbstractGpNode Clone(
            GpOperatorNode parent,
            HeuristicProblem heuristicProblem)
        {
            if (GpOperatorsContainer == null)
            {
                //var objective = (GpObjectiveFunction)heuristicProblem.ObjectiveFunction;
                GpOperatorsContainer = heuristicProblem.GpOperatorsContainer;
            }
            var newNode = new GpOperatorNode(GpOperatorsContainer)
                              {
                                  ParameterValuesArr = (object[]) ParameterValuesArr.Clone(),
                                  ParameterDescrArr = ParameterDescrArr == null  ? null : (string[]) ParameterDescrArr.Clone(),
                                  Depth = Depth,
                                  GpOperator = GpOperator,
                                  Parent = parent,
                                  IsOperatorNode = true
                              };
            var children = new AbstractGpNode[m_childrenArr.Length];
            for (var i = 0; i < children.Length; i++)
            {
                children[i] = m_childrenArr[i].Clone(
                    newNode,
                    heuristicProblem);
            }
            newNode.m_childrenArr = children;
            return newNode;
        }

        #endregion

        #region Private

        private AbstractGpNode CreateEndNodeChild(
            RngWrapper rng,
            int depth)
        {
            //
            // decide if creating a time series variable or a constant node
            //
            if (rng.NextDouble() < 0.5)
            {
                //
                // generate a variable node from the provided factory
                // a variable node can be also used as a sub-function.
                // the node factory provides this flexibility.
                //
                return GpOperatorsContainer.GpVarNodeFactory.BuildVariable(
                    depth,
                    this);
            }
            int intChosenConstant = rng.NextInt(0,
                                               GpOperatorsContainer.GpConstantArr.Length - 1);
            return new GpConstantNode(
                GpOperatorsContainer.GpConstantArr[intChosenConstant],
                depth,
                this,
                GpOperatorsContainer);
        }

        #endregion

    }
}
