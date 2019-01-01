#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.MachineLearning.NeuralNet
{
    /// <summary>
    /// Neural network operator node
    /// </summary>
    [Serializable]
    public class NnOperatorNode : GpOperatorNode
    {
        #region Properties

        public double Weight { get; set; }

        public bool IsViasNode { get; set; }

        public int OperatorId { get; set; }

        #endregion

        #region Constructors

        public NnOperatorNode(
            GpOperatorsContainer gpOperatorsContainer) :
            this(
            0,
            gpOperatorsContainer){}

        public NnOperatorNode() :
            this(
            0,
            null){}

        public NnOperatorNode(
            int intOperatorName,
            GpOperatorsContainer gpOperatorsContainer)
        {
            GpOperatorsContainer = gpOperatorsContainer;
            OperatorId = intOperatorName;
            GetRandomWeight();
        }

        public NnOperatorNode(
            AbstractGpOperator gpOperator,
            int depth,
            GpOperatorNode parent,
            double dblWeight,
            GpOperatorsContainer gpOperatorsContainer) :
            base(
                gpOperator,
                depth,
                parent,
                gpOperatorsContainer)
        {
            Weight = dblWeight;
        }

        public NnOperatorNode(
            GpOperatorNode parent, 
            int intDepth, 
            int intMaxDepth,
            RngWrapper random, 
            bool blnFullTree,
            GpOperatorsContainer gpOperatorsContainer) :
            base(
                //
                // randomly pick up an operator (sum, product, etc)
                //
                    gpOperatorsContainer.GpOperatorArr[
                        random.NextInt(
                            0,
                            gpOperatorsContainer.GpOperatorArr.Length - 1)],
                     intDepth,
                     parent,
                     gpOperatorsContainer)
        {
            //
            // set random weight
            //
            if(Parent != null)
            {
                GetRandomWeight();
            }

            int intChildren = random.NextInt(1, 4);
            m_childrenArr = new AbstractGpNode[intChildren];
            GpOperatorsContainer = gpOperatorsContainer;

            if (intDepth == intMaxDepth - 1)
            {
                //
                // end of the tree
                // m_childrenArr have to be constants or
                // variables only
                //
                for (int i = 0; i < m_childrenArr.Length; i++)
                {
                    m_childrenArr[i] = CreateTerminalChild(
                        intDepth + 1);
                }
            }
            else
            {
                // m_childrenArr can be constants, terminals or operators
                for (int i = 0; i < m_childrenArr.Length; i++)
                {
                    //
                    // check if it is a full size tree
                    //
                    if (blnFullTree)
                    {
                        m_childrenArr[i] = new NnOperatorNode(
                            this,
                            intDepth + 1,
                            intMaxDepth,
                            random,
                            true,
                            gpOperatorsContainer);
                    }
                    else
                    {
                        //
                        // decide if the node is an operator or a terminal one
                        //
                        if (random.NextDouble() < 0.5)
                        {
                            m_childrenArr[i] = new NnOperatorNode(
                                this,
                                intDepth + 1,
                                intMaxDepth,
                                random,
                                false,
                                gpOperatorsContainer);
                        }
                        else
                        {
                            m_childrenArr[i] = CreateTerminalChild(
                                intDepth + 1);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public

        public override object Compute(
            AbstractGpVariable gpVariable)
        {
            if (ChildrenArr != null)
            {
                ParameterValuesArr =
                    new object[ChildrenArr.Length];

                //
                // compute a hidden layer
                //
                for (int i = 0; i < ParameterValuesArr.Length; i++)
                {
                    ParameterValuesArr[i] = ChildrenArr[i].Compute(gpVariable);
                }

                double dblSum = (from n in ParameterValuesArr select (double)n).Sum();
                double dblResult = dblSum * Weight;
                
                //
                // compute hidden operator
                //
                object resultWrap = dblResult;
                if(GpOperator != null)
                {
                    resultWrap = GpOperator.Compute(
                        new []
                            {
                                resultWrap
                            });
                }
                return resultWrap;
            }

            //
            // compute an input node
            //
            if (IsViasNode)
            {
                //
                // compute vias node
                //
                return NnConstants.VIAS_WEIGHT*Weight;
            }
            //
            // compute terminal node
            //
            //return gpVariable.Value * Weight;
            return GpOperatorsContainer.NodeEvaluator.EvaluateVarNode(
                gpVariable,
                this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(
                "nn(" + Weight + ")");
            
            if (ChildrenArr != null)
            {
                sb.AppendLine("{");
                for (int i = 0; i < ChildrenArr.Length; i++)
                {
                    sb.AppendLine(
                        ChildrenArr[i].ToString());
                }
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        public override AbstractGpNode Clone(
            GpOperatorNode parent,
            HeuristicProblem heuristicProblem)
        {
            NnOperatorNode newNode =
                new NnOperatorNode
                    {
                        GpOperator = GpOperator,
                        Depth = Depth,
                        Parent = parent,
                        Weight = Weight,
                        GpOperatorsContainer = GpOperatorsContainer,
                        IsOperatorNode = false,
                        IsViasNode = IsViasNode
                    };

            //
            // clone children
            //
            if (ChildrenArr != null)
            {
                AbstractGpNode[] children = new AbstractGpNode[
                    ChildrenArr.Length];

                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = ChildrenArr[i].Clone(
                        null,
                        heuristicProblem);
                }
                newNode.ChildrenArr = children;
            }

            return newNode;
        }

        public override void GetNodeList(List<AbstractGpNode> nodeList)
        {
            nodeList.Add(this);
            if (ChildrenArr != null)
            {
                for (int i = 0; i < ChildrenArr.Length; i++)
                {
                    ChildrenArr[i].GetNodeList(nodeList);
                }
            }
        }

        public override void ToStringB(StringBuilder sb)
        {
            sb.Append(ToString());
        }

        public override string ComputeToString()
        {
            return ToString();
        }

        #endregion

        #region Private

        private void GetRandomWeight()
        {
            RngWrapper rng =
                new RngWrapper();
            Weight = NnConstants.NN_MIN_VALUE +
                     rng.NextDouble() *
                     (NnConstants.NN_MAX_VALUE - NnConstants.NN_MIN_VALUE);
        }

        private AbstractGpNode CreateTerminalChild(
            int depth)
        {
           NnOperatorNode nnInputOperatorNode =
                        new NnOperatorNode();
            nnInputOperatorNode.Depth = depth;
            return nnInputOperatorNode;
        }

        #endregion
    }
}
