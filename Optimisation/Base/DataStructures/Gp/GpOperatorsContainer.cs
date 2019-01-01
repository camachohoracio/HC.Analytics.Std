#region

using System;
using System.Xml.Serialization;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpOperatorsContainer : IDisposable
    {
        #region Properties

        [XmlElement("MaxTreeDepth", typeof (int))]
        public int MaxTreeDepth { get; set; }

        [XmlArray("GpOperatorArr")]
        [XmlArrayItem("GpOperator", typeof (AbstractGpOperator))]
        public AbstractGpOperator[] GpOperatorArr { get; set; }

        [XmlArray("GpConstantArr")]
        [XmlArrayItem("GpConstant", typeof (GpConstants))]
        public GpConstants[] GpConstantArr { get; set; }

        [XmlElement("GpVariable", typeof (AbstractGpVariable))]
        public AbstractGpVariable GpVariable { get; set; }

        [XmlElement("GpVariableNodeFactory", typeof (AbstractGpVarNodeFactory))]
        public AbstractGpVarNodeFactory GpVarNodeFactory { get; set; }

        [XmlElement("GpOperatorNodeFactory", typeof (AbstractGpOperatorNodeFactory))]
        public AbstractGpOperatorNodeFactory GpOperatorNodeFactory { get; set; }

        [XmlElement("CrossoverProbability", typeof (double))]
        public double CrossoverProbability { get; set; }

        [XmlElement("MaxTreeDepthMutation", typeof (int))]
        public int MaxTreeDepthMutation { get; set; }

        [XmlElement("TournamentSize", typeof (int))]
        public int TournamentSize { get; set; }

        [XmlElement("MaxTreeSize", typeof (int))]
        public int MaxTreeSize { get; set; }

        [XmlElement("TimeHorizon", typeof (int))]
        public int TimeHorizon { get; set; }

        [XmlElement("NodeEvaluator", typeof (AbstractNodeEvaluator))]
        public AbstractNodeEvaluator NodeEvaluator { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Used for serialization
        /// </summary>
        public GpOperatorsContainer()
        {
        }

        public GpOperatorsContainer(
            AbstractGpOperator[] abstractGpOperators,
            GpConstants[] constants,
            AbstractGpVariable gpVariable,
            int intMaxTreeDepth,
            double dblCrossoverProbability,
            int intMaxTreeDepthMutation,
            int intMaxTreeSize,
            int intTournamentSize,
            int intTimeHorizon)
        {
            GpOperatorArr = abstractGpOperators;
            GpConstantArr = constants;
            GpVariable = gpVariable;
            MaxTreeDepth = intMaxTreeDepth;
            CrossoverProbability = dblCrossoverProbability;
            MaxTreeDepthMutation = intMaxTreeDepthMutation;
            MaxTreeSize = intMaxTreeSize;
            TournamentSize = intTournamentSize;
            TimeHorizon = intTimeHorizon;
            MinNumParams = int.MaxValue;
            foreach (AbstractGpOperator abstractGpOperator in abstractGpOperators)
            {
                MinNumParams = Math.Min(abstractGpOperator.NumbParameters, MinNumParams);
            }
        }

        #endregion

        public void Dispose()
        {
            
        }

        public int MinNumParams { get; set; }
    }
}
