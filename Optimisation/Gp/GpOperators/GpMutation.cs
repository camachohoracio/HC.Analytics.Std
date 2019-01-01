#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Mutation;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators
{
    [Serializable]
    public class GpMutation : IMutation
    {
        #region Members

        private readonly GpOperatorsContainer m_gpOperatorsContainer;
        //private readonly HeuristicProblem m_heuristicProblem;
        private readonly int m_intDeph;

        #endregion

        #region Constructors

        public GpMutation(
            int intDeph,
            GpOperatorsContainer gpOperatorsContainer)
        {
            m_intDeph = intDeph;
            m_gpOperatorsContainer = gpOperatorsContainer;
            //m_heuristicProblem = heuristicProblem;
        }

        #endregion

        #region IMutation Members

        public Individual DoMutation(
            Individual individual)
        {
            var rng = HeuristicProblem.CreateRandomGenerator();

            var gpIndividual = individual;
            Mutate(
                rng,
                m_intDeph,
                gpIndividual);
            return gpIndividual;
        }

        #endregion

        #region Private

        private void Mutate(
            RngWrapper random,
            int intMaxDepth,
            Individual gpIndividual)
        {
            try
            {
                int intNewSize = 0;
                AbstractGpNode tree1 = null;
                AbstractGpNode tree2 = null;
                GpOperatorNode parent = null;
                int intTrials = 0;
                while (intNewSize == 0 || intNewSize > m_gpOperatorsContainer.MaxTreeSize)
                {
                    int intParentCount = -1;
                    //
                    // avoid selecting something that cannot be mutated
                    //
                    int intTrials3 = 0;
                    while (intParentCount == -1 ||
                        intParentCount >= m_gpOperatorsContainer.MaxTreeSize)
                    {
                        int intTrials2 = 0;
                        while (tree1 == null)
                        {
                            int intNodeNumber = random.NextInt(0,
                                                               gpIndividual.Size - 1) + 1;
                            tree1 = GpIndividualHelper.ReturnNodeNumber(
                                intNodeNumber,
                                gpIndividual.Root);
                            if(intTrials2++ > 5)
                            {
                                return;
                            }
                        }
                        parent = tree1.Parent;
                        intParentCount = GpIndividualHelper.CountNodes(parent);
                        
                        if(intTrials3++ > 5)
                        {
                            return;
                        }
                    }

                    //
                    // create random tree
                    //
                    tree2 = m_gpOperatorsContainer.GpOperatorNodeFactory.CreateNewOperator(
                            parent,
                            intMaxDepth,
                            gpIndividual.GpTreeDepth,
                            random);
                    intNewSize = gpIndividual.Size - GpIndividualHelper.CountNodes(tree1) +
                                     GpIndividualHelper.CountNodes(tree2);

                    if(intTrials++ > 5)
                    {
                        // give up
                        return;
                    }
                }

                if (parent != null)
                {
                    AbstractGpNode[] children = parent.ChildrenArr;
                    for (var i = 0; i < children.Length; i++)
                    {
                        if (children[i].Equals(tree1))
                        {
                            children[i] = tree2;
                            break;
                        }
                    }
                }
                else
                {
                    gpIndividual.Root = tree2;
                    gpIndividual.GpTreeDepth = tree2.Depth;
                }


                if (intNewSize > m_gpOperatorsContainer.MaxTreeSize)
                {
                    throw new HCException("Invalid size");
                }

                //gpIndividual.Size = intNewSize;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion
    }
}
