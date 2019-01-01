using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Probability.Random;

namespace HC.Analytics.MachineLearning.NeuralNet
{
    [Serializable]
    public class NnOperatorNodeFactory : AbstractGpOperatorNodeFactory
    {
        #region Constructor

        public NnOperatorNodeFactory()
        { }

        public NnOperatorNodeFactory(
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
            return new NnOperatorNode(
                parent,
                intDepth,
                intMaxDepth,
                rngWrapper,
                blnFullTree,
                m_gpOperatorsContainer);
        }

        #endregion
    }
}
