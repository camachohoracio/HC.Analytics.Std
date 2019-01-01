#region

using System;
using System.Text;
using System.Xml.Serialization;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses
{
    [Serializable]
    public partial class Individual
    {
        #region Members

        private GpOperatorsContainer m_gpOperatorsContainer;

        #endregion

        #region Properties

        [XmlElement("GpTreeDepth", typeof (int))]
        public int GpTreeDepth { get; set; }

        [XmlElement("GpTreeSize", typeof (int))]
        public int GpTreeSize { get; set; }

        [XmlElement("GpTreeRoot", typeof (AbstractGpNode))]
        public AbstractGpNode GpTreeRoot { get; set; }

        #endregion

        #region Constructors

        public Individual(
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem heuristicProblem)
            : this(
                null,
                null,
                null,
                0,
                heuristicProblem)
        {
            m_gpOperatorsContainer = gpOperatorsContainer;
            CreateRandomTree(heuristicProblem);
        }


        public Individual(
            HeuristicProblem heuristicProblem,
            GpOperatorsContainer gpOperatorsContainer,
            AbstractGpNode root)
            : this(
                null,
                null,
                null,
                0,
                heuristicProblem)
        {
            Root = root;
            m_gpOperatorsContainer = gpOperatorsContainer;
            //GpTreeSize = GpIndividualHelper.CountNodes(root);
        }

        private void CreateRandomTree(
            HeuristicProblem heuristicProblem)
        {
            try
            {
                RngWrapper rng =
                    HeuristicProblem.CreateRandomGenerator();
                int intSize = 0;
                AbstractGpNode gpTreeRoot = null;
                while (intSize == 0 || intSize > m_gpOperatorsContainer.MaxTreeSize)
                {
                    gpTreeRoot = m_gpOperatorsContainer.
                        GpOperatorNodeFactory.CreateNewOperator(
                            null,
                            m_gpOperatorsContainer.MaxTreeDepth,
                            GpTreeDepth,
                            rng);

                    intSize = GpIndividualHelper.CountNodes(gpTreeRoot);
                }
                //GpTreeSize = intSize;
                Root = gpTreeRoot;

            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Properties

        public AbstractGpNode Root
        {
            get { return GpTreeRoot; }
            set
            {
                Size = GpIndividualHelper.CountNodes(value);
                GpTreeRoot = value;
            }
        }

        public int Size
        {
            get { return GpTreeSize; }
            set { GpTreeSize = value; }
        }

        #endregion

        public Individual CloneIndividualTree(
            Individual newIndividual,
            HeuristicProblem heuristicProblem)
        {
            newIndividual.Root = Root.Clone(
                null,
                heuristicProblem);
            newIndividual.GpTreeDepth = GpTreeDepth;
            //newIndividual.Size = Size;
            newIndividual.m_gpOperatorsContainer =
                m_gpOperatorsContainer;
            return newIndividual;
        }

        public double EvaluateTree(AbstractGpVariable gpVariable)
        {
            // evaluate a gp operator node
            return Convert.ToDouble(Root.Compute(gpVariable));
        }

        public int CompareToTree(Individual other)
        {
            return Size - other.Size;
        }

        public void ToStringTree(StringBuilder sb)
        {
            if (Root != null)
            {
                Root.ToStringB(sb);
            }
            //return "";
        }

        public bool ContainsChromosomeTree()
        {
            return Root != null;
        }
    }
}
