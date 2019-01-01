#region

using System;
using System.Collections.Generic;
using HC.Core.Events;
using System.Threading.Tasks;
using HC.Core.Exceptions;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.MstCluster
{
    [Serializable]
    public class MstClusterManager
    {
        #region Events & delegates

        public delegate double GetDistanceDel(int intI, int intJ);
        public event GetDistanceDel OnGetDistance;

        #endregion

        #region Members

        private readonly object m_lockObject = new object();
        private readonly object m_lockObjectScore = new object();
        private readonly double m_dblNodeDegreesThreshold;
        private readonly double m_dblBranchLenghtThreshold;
        private readonly double m_dblAdjacentNodeThreshold;
        private readonly double m_dblBranchSizeThreshold;
        private readonly double m_dblEdgeThreshold;
        private readonly int m_intSearchLength;
        private Mst m_mst;
        private readonly int m_intN;
        private readonly double m_dblSimilarityThreshold;

        #endregion

        #region Constructors

        public MstClusterManager(
            int intN,
            double dblSimilarityThreshold)
        {
            m_dblSimilarityThreshold = dblSimilarityThreshold;
            m_dblEdgeThreshold = MstClusterConstants.EDGE_THRESHOLD;
            m_dblNodeDegreesThreshold = MstClusterConstants.NODE_DEGREES_THRESHOLD;
            m_dblBranchLenghtThreshold = MstClusterConstants.BRANCH_LENGHT_THRESHOLD;
            m_dblAdjacentNodeThreshold = MstClusterConstants.ADJACENT_NODE;
            m_dblBranchSizeThreshold = MstClusterConstants.BRANCH_SIZE_THRESHOLD;
            m_intSearchLength = MstClusterConstants.PURGER_SEARCH_LENGTH;
            m_intN = intN;
        }

        #endregion

        public Mst DoCluster()
        {
            int intN = m_intN;
            int lastPercentage = 0;
            double dblSumMinutes = 0;
            double dblGoal = (((intN) * ((intN) - 1.0)) / 2.0);
            m_mst = new Mst(m_dblEdgeThreshold,
                              m_dblNodeDegreesThreshold,
                              m_dblBranchLenghtThreshold,
                              m_dblAdjacentNodeThreshold,
                              m_dblBranchSizeThreshold);

            m_mst.OnGetDistance += InvokeOnGetDistance;
            DateTime start = DateTime.Now;
            DateTime start2 = DateTime.Now;
            int intTotalComparisons = 0;
            SendMessageEvent.OnSendMessage("Please wait. Comparing records...", 0);

            Parallel.For(0, intN, delegate(int i)
            //for (int i = 0; i < intN; i++)
            {
                var candidateList = new List<MstDistanceObj>();

                for (int j = i + 1; j < intN; j++)
                {
                    bool blnCheckLoop = 
                        m_mst.CheckLoop(i + 1, j + 1);

                    if (blnCheckLoop)
                    {
                        double dblSimilarity = InvokeOnGetDistance(i, j);
                        if (dblSimilarity > m_dblSimilarityThreshold)
                        {
                            var actualScoreObject =
                                new MstDistanceObj(i,
                                    j,
                                    dblSimilarity);

                            lock (m_lockObjectScore)
                            {
                                candidateList.Add(actualScoreObject);
                            }
                        }
                    }
                }
                //
                // process the colected scores
                //
                candidateList.Sort();
                candidateList.Reverse();

                if (candidateList.Count > 0)
                {
                    int searchCount = 0;
                    foreach (MstDistanceObj currentScoreObject2 in candidateList)
                    {
                        lock (m_lockObject)
                        {
                            m_mst.Link(currentScoreObject2, null);
                        }
                        searchCount++;
                        if (searchCount >= m_intSearchLength)
                        {
                            break;
                        }
                    }
                }

                lock (m_lockObject)
                {
                    intTotalComparisons += intN - i;
                    int percentage = (int)((intTotalComparisons / dblGoal) * 100.00);
                    if (percentage != lastPercentage)
                    {
                        TimeSpan ts = DateTime.Now - start;
                        dblSumMinutes += ((ts.TotalSeconds) / 60.0);
                        double avgMinutes = dblSumMinutes / (percentage);
                        int estimatedTime = (int)((100.00 - percentage) * avgMinutes) + 1;
                        string message = "Comparing records... Total comparisons: " +
                                         intTotalComparisons + ", Completed: " +
                                         percentage + "%. Estimated time for completion: " +
                                         estimatedTime + " Min.";
                        SendMessageEvent.OnSendMessage(message, percentage);
                        start = DateTime.Now;
                    }
                    lastPercentage = percentage;
                }
            }
            );
            TimeSpan ts2 = DateTime.Now - start2;
            PrintToScreen.WriteLine("time: " + ts2.TotalSeconds);
            return m_mst;
        }

        private double InvokeOnGetDistance(int intI, int intJ)
        {
            if (OnGetDistance != null)
            {
                if (OnGetDistance.GetInvocationList().Length > 0)
                {
                    return OnGetDistance.Invoke(intI, intJ);
                }
            }
            throw new HCException("Score event subscription not found");
        }
    }
}
