#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public abstract class AbstractGpOperatorNodeFactory
    {
        #region Members

        protected readonly double m_dblFullTreeProb;
        protected readonly GpOperatorsContainer m_gpOperatorsContainer;

        #endregion

        #region Constructors

        public AbstractGpOperatorNodeFactory()
        {
        }

        public AbstractGpOperatorNodeFactory(
            double dblFullTreeProb,
            GpOperatorsContainer gpOperatorsContainer)
        {
            m_dblFullTreeProb = dblFullTreeProb;
            m_gpOperatorsContainer = gpOperatorsContainer;
        }

        #endregion

        #region Public

        public AbstractGpNode CreateNewOperator(
            GpOperatorNode parent,
            int intMaxDepth,
            int intDepth,
            RngWrapper rngWrapper)
        {
            return CreateNewOperator(
                parent,
                intMaxDepth,
                intDepth,
                rngWrapper.NextDouble() < m_dblFullTreeProb,
                rngWrapper);
        }

        #endregion

        #region Abstract Methods

        protected abstract AbstractGpNode CreateNewOperator(
            GpOperatorNode parent,
            int intMaxDepth,
            int intDepth,
            bool blnFullTree,
            RngWrapper rngWrapper);

        #endregion
    }
}
