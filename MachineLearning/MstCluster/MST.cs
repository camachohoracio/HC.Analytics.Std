#region

using System;
using System.Collections.Generic;
using HC.Core.Exceptions;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.MstCluster
{
    [Serializable]
    public class Mst
    {
        #region Events

        public delegate double GetDistance(int intI, int intJ);
        public event GetDistance OnGetDistance;

        #endregion

        #region Properties

        public Dictionary<int, MstEdge> HtEdge { get; private set; }

        public Dictionary<int, List<int>> HtParent { get; private set; }

        public Dictionary<string, double> HtScores { get; private set; }

        /// <summary>
        /// delta1 verifies nodes which degree >1
        /// </summary>
        public double NodeDegreesThreshold { get; private set; }

        /// <summary>
        /// parameter delta2 reduces the branch length in the trees
        /// It also controls the weights in the root edges
        /// </summary>
        public double BranchLenghtThreshold { get; private set; }

        /// <summary>
        /// parameter delta3 controls the number of adjacent nodes in the root nodes
        /// The value set to this parameter is the same for all string
        /// metrics wich return score is in [0,1]
        /// </summary>
        public double AdjacentNodeThreshold { get; private set; }

        /// <summary>
        /// controls how much the threshold will be relaxed according branch size
        /// </summary>
        public double BranchSizeThreshold { get; private set; }

        /// <summary>
        /// Main similairty threshold
        /// </summary>
        public double EdgeThreshold { get; set; }

        #endregion

        #region Members

        private readonly Dictionary<int, double> m_htMaxAdjacentEdge;
        private readonly Dictionary<int, object> m_htParentObject;

        #endregion

        #region Constructors

        public Mst() :
            this(
            MstClusterConstants.EDGE_THRESHOLD,
            MstClusterConstants.NODE_DEGREES_THRESHOLD,
            MstClusterConstants.BRANCH_LENGHT_THRESHOLD,
            MstClusterConstants.ADJACENT_NODE,
            MstClusterConstants.BRANCH_SIZE_THRESHOLD)
        {
            
        }

        public Mst(
            double edgeThreshold,
            double dblNodeDegreesThreshold,
            double dblBranchLenghtThreshold,
            double dblAdjacentNodeThreshold,
            double branchSizeThreshold)
        {
            EdgeThreshold = edgeThreshold;
            NodeDegreesThreshold = dblNodeDegreesThreshold;
            BranchLenghtThreshold = dblBranchLenghtThreshold;
            AdjacentNodeThreshold = dblAdjacentNodeThreshold;
            BranchSizeThreshold = branchSizeThreshold;
            HtEdge = new Dictionary<int, MstEdge>();
            HtParent = new Dictionary<int, List<int>>();
            m_htParentObject = new Dictionary<int, object>();
            HtScores = new Dictionary<string, double>();
            m_htMaxAdjacentEdge = new Dictionary<int, double>();
        }

        #endregion

        public bool CheckLoop(int x, int y)
        {
            MstEdge mstEdgeX = null;
            MstEdge mstEdgeY = null;
            if (HtEdge.ContainsKey(x))
            {
                mstEdgeX = HtEdge[x];
            }
            if (HtEdge.ContainsKey(y))
            {
                mstEdgeY = HtEdge[y];
            }
            // check for cycles
            if (mstEdgeX != null && mstEdgeY != null)
            {
                if (mstEdgeX.GetParent() == mstEdgeY.GetParent())
                {
                    return false;
                }
            }
            return true;
        }

        public bool Link(
            MstDistanceObj actualScoreObject,
            Object rootObject)
        {
            return Link(
                actualScoreObject,
                rootObject,
                true);
        }

        public double GetScore(int i, int j)
        {
            //
            // get confidence score
            //
            string strScoreKey;
            if (i > j)
            {
                strScoreKey = j + "-" + i;
            }
            else
            {
                strScoreKey = i + "-" + j;
            }
            return HtScores[strScoreKey];
        }

        public bool Link(
            MstDistanceObj actualScoreObject, 
            Object rootObject,
            bool blnValidateConstraints)
        {
            // initialization data
            double actualScore = actualScoreObject.GetScore();
            int x = actualScoreObject.GetX();
            int y = actualScoreObject.GetY();
            MstEdge mstEdgeX = null;
            MstEdge mstEdgeY = null;

            if (x > y)
            {
                //Debugger.Break();
                throw new HCException("Error. MST index order not correct");
            }

            string position = x + "-" + y;


            if (HtEdge.ContainsKey(x))
            {
                mstEdgeX = HtEdge[x];
            }
            if (HtEdge.ContainsKey(y))
            {
                mstEdgeY = HtEdge[y];
            }
            // check for cycles
            if (mstEdgeX != null && mstEdgeY != null)
            {
                if (mstEdgeX.GetParent() == mstEdgeY.GetParent())
                {
                    return false;
                }
            }

            if (blnValidateConstraints)
            {
                if (!ValidateConstraints(
                         x,
                         y,
                         mstEdgeX,
                         mstEdgeY,
                         actualScore,
                         position))
                {
                    return false;
                }
            }
            else
            {
                //
                // add score to map
                //
                if (!HtScores.ContainsKey(position))
                {
                    HtScores.Add(position, actualScoreObject.GetScore());
                }
            }

            // add the edge for the current position
            if (AddEdge(x, y, rootObject))
            {
                // keep track of the maximum adjacent edges
                if (!m_htMaxAdjacentEdge.ContainsKey(x))
                {
                    m_htMaxAdjacentEdge.Add(x, actualScore);
                }
                if (!m_htMaxAdjacentEdge.ContainsKey(y))
                {
                    m_htMaxAdjacentEdge.Add(y, actualScore);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool ValidateConstraints(
            int x, 
            int y, 
            MstEdge mstEdgeX, 
            MstEdge mstEdgeY, 
            double actualScore, 
            string position)
        {
            // Constraint 1: Set a loose threshold k_star
            if (actualScore < EdgeThreshold)
            {
                return false;
            }

            if (!HtScores.ContainsKey(position))
            {
                HtScores.Add(position, actualScore);
            }

            // Constrain 2: Verify nodes which degree >1
            if (m_htMaxAdjacentEdge.ContainsKey(x))
            {
                double maxScore = m_htMaxAdjacentEdge[x];
                if (maxScore - actualScore > NodeDegreesThreshold)
                {
                    return false;
                }
            }

            if (m_htMaxAdjacentEdge.ContainsKey(y))
            {
                double maxScore = m_htMaxAdjacentEdge[y];
                if (maxScore - actualScore > NodeDegreesThreshold)
                {
                    return false;
                }
            }

            // Constraint 4: keep a low weight to incident edges to the root node
            if (mstEdgeX != null)
            {
                if (mstEdgeX.GetParent() == x)
                {
                    double maxScore = m_htMaxAdjacentEdge[x];
                    if (maxScore - actualScore > AdjacentNodeThreshold)
                    {
                        return false;
                    }
                }
            }
            if (mstEdgeY != null)
            {
                if (mstEdgeY.GetParent() == y)
                {
                    double maxScore = m_htMaxAdjacentEdge[y];
                    if (maxScore - actualScore > AdjacentNodeThreshold)
                    {
                        return false;
                    }
                }
            }

            // Constraint 3: Verify branches with more than two nodes
            // verify in node x
            if (mstEdgeX != null)
            {
                List<int> beforeList = GetBeforeNodes(x);
                int listSize = beforeList.Count;
                if (listSize > 0)
                {
                    double sumScores = 0.0;
                    foreach (int actualNode in beforeList)
                    {
                        int xNew = Math.Min(y, actualNode);
                        int yNew = Math.Max(y, actualNode);
                        string positionNew = xNew + "-" + yNew;
                        if (HtScores.ContainsKey(positionNew))
                        {
                            sumScores += HtScores[positionNew];
                        }
                        else
                        {
                            double tmpScore = InvokeOnGetDistance(xNew, yNew);
                            HtScores.Add(positionNew, tmpScore);
                            sumScores += tmpScore;
                        }
                    }
                    double averageScore = sumScores / listSize;
                    double actualThreshold;

                    if (listSize == 1)
                    {
                        actualThreshold = BranchLenghtThreshold;
                    }
                    else
                    {
                        actualThreshold = BranchLenghtThreshold - (listSize - 1.0)*BranchSizeThreshold;
                    }
                    if (averageScore < actualThreshold)
                    {
                        return false;
                    }
                }
            }

            // verify in node y
            if (mstEdgeY != null)
            {
                List<int> beforeList = GetBeforeNodes(y);
                int listSize = beforeList.Count;
                if (listSize > 0)
                {
                    double sumScores = 0.0;
                    foreach (int actualNode in beforeList)
                    {
                        int intXNew = Math.Min(x, actualNode);
                        int intYNew = Math.Max(x, actualNode);
                        string strPositionNew = intXNew + "-" + intYNew;
                        if (HtScores.ContainsKey(strPositionNew))
                        {
                            sumScores += HtScores[strPositionNew];
                        }
                        else
                        {
                            double tmpScore = InvokeOnGetDistance(intXNew, intYNew);
                            sumScores += tmpScore;
                        }
                    }
                    double dblAverageScore = sumScores/listSize;
                    double dblActualThreshold;
                    if (listSize == 1)
                    {
                        dblActualThreshold = BranchLenghtThreshold;
                    }
                    else
                    {
                        dblActualThreshold = BranchLenghtThreshold - ((listSize) - 1.0)*BranchSizeThreshold;
                    }
                    if (dblAverageScore < dblActualThreshold)
                    {
                        //Console::WriteLine("averageScore2 "+averageScore);
                        return false;
                    }
                }
            }
            return true;
        }

        private List<int> GetBeforeNodes(int node)
        {
            //Console::WriteLine("node "+node);
            List<int> beforeList = new List<int>();
            MstEdge mStEdge = HtEdge[node];
            int before = mStEdge.GetBefore();
            //Console::WriteLine("before "+before);
            if (before > -1)
            {
                beforeList.Add(before);
            }
            while (before > -1)
            {
                mStEdge = HtEdge[before];
                before = mStEdge.GetBefore();

                if (before > -1)
                {
                    beforeList.Add(before);
                }
            }
            return beforeList;
        }


        private bool AddEdge(int x, int y, Object rootObject)
        {
            if (!HtEdge.ContainsKey(x) && !HtEdge.ContainsKey(y))
            {
                int parent, beforeX = -1, beforeY = -1;
                if (x < y)
                {
                    parent = x;
                    beforeY = x;
                }
                else
                {
                    parent = y;
                    beforeX = y;
                }
                MSTParentObject mStParentObject = new MSTParentObject(parent);
                HtEdge.Add(x, new MstEdge(beforeX, mStParentObject));
                HtEdge.Add(y, new MstEdge(beforeY, mStParentObject));
                List<int> parentMembersList = new List<int>();
                parentMembersList.Add(x);
                parentMembersList.Add(y);
                HtParent.Add(parent, parentMembersList);
                m_htParentObject.Add(parent, rootObject);
                return true;
            }
            if (HtEdge.ContainsKey(x) && HtEdge.ContainsKey(y))
            {
                MstEdge mstEdgeX = HtEdge[x];
                MstEdge mstEdgeY = HtEdge[y];
                MSTParentObject parentObjectX = mstEdgeX.getParentObject();
                MSTParentObject parentObjectY = mstEdgeY.getParentObject();
                if (parentObjectX.GetParent() == parentObjectY.GetParent())
                {
                    // there is a cycle
                    return false;
                }
                //MSTParentObject parentObject;
                if (parentObjectX.GetParent() < parentObjectY.GetParent())
                {
                    // change the parents of all members of y
                    ChangeParent(y, x, parentObjectX);
                }
                else
                {
                    // change the parents of all members of x
                    ChangeParent(x, y, parentObjectY);
                }
                return true;
            }
            if (HtEdge.ContainsKey(x))
            {
                MstEdge actualMstEdge = HtEdge[x];
                HtEdge.Add(y, new MstEdge(x, actualMstEdge.getParentObject()));
                // add y to parent list
                int currentParent = actualMstEdge.GetParent();
                List<int> parentMembersList = HtParent[currentParent];
                parentMembersList.Add(y);
            }
            else
            {
                MstEdge actualMstEdge = HtEdge[y];
                HtEdge.Add(x, new MstEdge(y, actualMstEdge.getParentObject()));
                // add x to parent list
                int currentParent = actualMstEdge.GetParent();
                List<int> parentMembersList = HtParent[currentParent];
                parentMembersList.Add(x);
            }
            return true;
        }

        private void ChangeParent(
            int position,
            int beforeFinal,
            MSTParentObject newMstParentObject)
        {
            MstEdge mStEdge = HtEdge[position];
            // set before. Change the direction of the tree
            int before = mStEdge.GetBefore(),
                oldParent = mStEdge.GetParent();
            mStEdge.SetBefore(beforeFinal);
            beforeFinal = position;
            while (before > -1)
            {
                mStEdge = HtEdge[before];
                int beforeOld = before;
                before = mStEdge.GetBefore();
                mStEdge.SetBefore(beforeFinal);
                beforeFinal = beforeOld;
            }
            // change all the parents from the old tree
            int newParent = newMstParentObject.GetParent();
            List<int> oldMemberList = HtParent[oldParent];
            List<int> newMemberList = HtParent[newParent];
            foreach (int currentNode in oldMemberList)
            {
                mStEdge = HtEdge[currentNode];
                mStEdge.SetParentObject(newMstParentObject);
                newMemberList.Add(currentNode);
            }
            HtParent.Remove(oldParent);
            m_htParentObject.Remove(oldParent);
        }

        public override string ToString()
        {
            foreach (KeyValuePair<int, MstEdge> de in HtEdge)
            {
                MstEdge currentMstEdge = de.Value;
                int current = de.Key;
                int before = currentMstEdge.GetBefore();
                int parent = currentMstEdge.GetParent();
                PrintToScreen.WriteLine(current + "-" + before + ", parent " + parent);
            }
            string out1;
            foreach (KeyValuePair<int, List<int>> de in HtParent)
            {
                List<int> memberList = de.Value;
                int current = de.Key;
                out1 = "parent " + current + " : ";

                foreach (int currentMember in memberList)
                {
                    out1 += currentMember + ", ";
                }
                PrintToScreen.WriteLine(out1);
            }
            return "";
        }



        private double InvokeOnGetDistance(int intI, int intJ)
        {
            if(OnGetDistance != null)
            {
                if(OnGetDistance.GetInvocationList().Length > 0)
                {
                    return OnGetDistance.Invoke(intI, intJ);
                }
            }
            throw  new HCException("Error. Distance not defined.");
        }
    }
}
