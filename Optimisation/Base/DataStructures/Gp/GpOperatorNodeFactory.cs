#region

using System;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpOperatorNodeFactory : AbstractGpOperatorNodeFactory
    {
        #region Constructor

        public GpOperatorNodeFactory()
        {
        }

        public GpOperatorNodeFactory(
            double dblFullTreeProb,
            GpOperatorsContainer gpOperatorsContainer)
            : base(dblFullTreeProb,
                   gpOperatorsContainer)
        {
        }

        #endregion

        #region Protected

        protected override AbstractGpNode CreateNewOperator(
            GpOperatorNode parent,
            int intMaxDepth,
            int intDepth,
            bool blnFullTree,
            RngWrapper rngWrapper)
        {
            try
            {
                int intSize = 0;
                GpOperatorNode newOperator = null;
                int intParentSize = parent == null ? 0 : GpIndividualHelper.CountNodes(parent);
                int intNodesAvailable = m_gpOperatorsContainer.MaxTreeSize -
                    intSize - m_gpOperatorsContainer.MinNumParams;
                if (intNodesAvailable <= 0)
                {
                    throw new HCException("invalid nodes available [" + intNodesAvailable + "]");
                }

                while (intSize == 0 || intSize > m_gpOperatorsContainer.MaxTreeSize)
                {
                    AbstractGpOperator newRandomOperator = GpOperatorNode.SelectRandomOpeator(
                        rngWrapper,
                        m_gpOperatorsContainer,
                        intNodesAvailable);

                    //int intSizeTest = intSize + newRandomOperator.NumbParameters;
                    //bool blnIsTreeFull = intSizeTest > m_gpOperatorsContainer.MaxTreeSize;

                    //if (blnIsTreeFull)
                    //{
                    //    throw new HCException("Tree is full");
                    //}

                    newOperator = new GpOperatorNode(
                        parent,
                        intDepth,
                        intMaxDepth,
                        rngWrapper,
                        blnFullTree,
                        ref intParentSize,
                        m_gpOperatorsContainer.MaxTreeSize,
                        m_gpOperatorsContainer,
                        newRandomOperator);
                    intSize = GpIndividualHelper.CountNodes(newOperator);

                    //if (intSize > m_gpOperatorsContainer.MaxTreeSize)
                    //{
                    //    throw new HCException("invalid nodes available [" + intSize + "] > [" + m_gpOperatorsContainer.MaxTreeSize + "]");
                    //}
                }

                intSize = GpIndividualHelper.CountNodes(newOperator);

                if (intSize > m_gpOperatorsContainer.MaxTreeSize)
                {
                    throw new HCException("invalid nodes available [" + intSize + "] > [" + m_gpOperatorsContainer.MaxTreeSize + "]");
                }

                return newOperator;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        #endregion
    }
}
