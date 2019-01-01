#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators
{
    [Serializable]
    public class GpCrossover : ICrossover
    {
        #region Members

        private readonly HeuristicProblem m_heuristicProblem;
        private readonly int m_intMaxTreeSize;

        #endregion

        #region Constructor

        public GpCrossover(
            HeuristicProblem heuristicProblem,
            int intMaxTreeSize)
        {
            m_heuristicProblem = heuristicProblem;
            m_intMaxTreeSize = intMaxTreeSize;
        }

        #endregion

        #region ICrossover Members

        public Individual DoCrossover(
            RngWrapper rng,
            Individual[] individuals)
        {
            try
            {
                Individual ind1 = individuals[0].Clone(m_heuristicProblem);
                Individual ind2 = individuals[1].Clone(m_heuristicProblem);

                AbstractGpNode tree1 = null;
                AbstractGpNode tree2 = null;
                int intTrials0 = 0;
                while (tree1 == null ||
                    tree2 == null)
                {
                    int intIndex1 = rng.NextInt(0, ind1.Size - 1) + 1;
                    int intIndex2 = rng.NextInt(0, ind2.Size - 1) + 1;

                    tree1 = GpIndividualHelper.ReturnNodeNumber(intIndex1, ind1.Root);
                    tree2 = GpIndividualHelper.ReturnNodeNumber(intIndex2, ind2.Root);
                    if(intTrials0++ > 5)
                    {
                        return individuals[0];
                    }
                }
                tree1 = tree1.Clone(tree1.Parent, m_heuristicProblem);
                tree2 = tree2.Clone(tree2.Parent, m_heuristicProblem);
                int intSizeTree1 = GpIndividualHelper.CountNodes(tree1);
                int intSizeTree2 = GpIndividualHelper.CountNodes(tree2);

                GpOperatorNode p1 = tree1.Parent;
                GpOperatorNode p2 = tree2.Parent;
                //
                // do individual 1
                //
                if (ind1.Size + intSizeTree2 - intSizeTree1 <= m_intMaxTreeSize)
                {
                    tree2.Parent = p1;
                    if (p1 != null)
                    {
                        AbstractGpNode[] children1 = p1.ChildrenArr;
                        for (int i = 0; i < children1.Length; i++)
                        {
                            if (children1[i].Equals(tree1))
                            {
                                children1[i] = tree2;
                            }
                        }
                    }
                    else
                    {
                        ind1.Root = tree2;
                        ind1.GpTreeDepth = tree2.Depth;
                    }
                    // check size
                    int intSizeCheck = GpIndividualHelper.CountNodes(ind1.Root);
                    if(intSizeCheck > m_intMaxTreeSize)
                    {
                        throw new HCException("Invalid tree size");
                    }
                    //ind1.Size = ind1.Size + intSizeTree2 - intSizeTree1;
                    return ind1;
                }

                //
                // ind 2 should be in bounds then!
                //
                int intNewSize = ind2.Size + intSizeTree1 - intSizeTree2;
                if (intNewSize > m_intMaxTreeSize)
                {
                    throw new HCException("Invalid tree size");
                }

                tree1.Parent = p2;
                if (p2 != null)
                {
                    AbstractGpNode[] children2 = p2.ChildrenArr;
                    for (var i = 0; i < children2.Length; i++)
                    {
                        if (children2[i].Equals(tree2))
                        {
                            children2[i] = tree1;
                        }
                    }
                }
                else
                {
                    ind2.Root = tree1;
                    ind2.GpTreeDepth = tree1.Depth;
                }
                
                int intSizeCheck2 = GpIndividualHelper.CountNodes(ind2.Root);
                if (intSizeCheck2 > m_intMaxTreeSize)
                {
                    throw new HCException("Invalid tree size");
                }

                //ind2.Size = intNewSize;
                return ind2;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return individuals[0];
        }

        #endregion
    }
}
