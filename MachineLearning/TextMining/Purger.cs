#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HC.Analytics.MachineLearning.MstCluster;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics;
using HC.Core.Events;
using HC.Core.Helpers;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining
{
    public class Purger
    {
        #region Events & delegates

        public event EventHandler ProgressBarEventHandler;

        #endregion

        #region Properties

        public Mst Mst { get; private set; }
        public TokenStatistics TokenStatistics { get; private set; }
        public double NodeDegreesThreshold { get; set; }
        public double BranchLenghtThreshold { get; set; }
        public double AdjacentNodeThreshold { get; set; }
        public double BranchSizeThreshold { get; set; }
        public double EdgeThreshold { get; set; }
        
        #endregion

        #region Members

        // declare progress bar event handler
        private static readonly object m_lockObject = new object();
        private readonly double m_dblCheapThreshold1;
        private readonly double m_dblCheapThreshold2;
        private readonly double m_dblCheapThreshold3;
        private readonly int m_intSearchLength;
        private readonly DataWrapper m_data;
        private readonly IStringMetric m_cheapLink;
        private readonly IStringMetric m_tagLink;

        #endregion

        #region Constructors

        public Purger(
            double kStar,
            double nodeDegrees,
            double branchLenghtThreshold,
            double adjacentNodeThreshold,
            double branchSizeThreshold,
            DataWrapper data)
        {
            EdgeThreshold = kStar;
            NodeDegreesThreshold = nodeDegrees;
            BranchLenghtThreshold = branchLenghtThreshold;
            AdjacentNodeThreshold = adjacentNodeThreshold;
            AdjacentNodeThreshold = branchSizeThreshold;
            m_dblCheapThreshold1 = TextMiningConstants.PURGER_CHEAP_THRESHOLD_1;
            m_dblCheapThreshold2 = TextMiningConstants.PURGER_CHEAP_THRESHOLD_2;
            m_dblCheapThreshold3 = TextMiningConstants.PURGER_CHEAP_THRESHOLD_3;
            m_intSearchLength = MstClusterConstants.PURGER_SEARCH_LENGTH;
            m_data = data;
            TokenStatistics = new TokenStatistics(data);
            m_cheapLink = new TagLinkCheap(data, 4, TokenStatistics);
            m_tagLink = new TagLink(
                data, 
                TokenStatistics, 
                EdgeThreshold);
        }

        public Purger(
            DataWrapper dataArray) : this(
            MstClusterConstants.EDGE_THRESHOLD,
            MstClusterConstants.NODE_DEGREES_THRESHOLD,
            MstClusterConstants.BRANCH_LENGHT_THRESHOLD,
            MstClusterConstants.ADJACENT_NODE,
            MstClusterConstants.BRANCH_SIZE_THRESHOLD,
            dataArray) { }

        #endregion

        public void Purge()
        {
            Mst = DoPurge();
            // evaluate the results
            //Mst.EvaluateTree();
        }

        private Mst DoPurge()
        {
            //
            // get the blocker class
            //
            var blocker = new Blocker(m_data);
            int intN = m_data.Length;
            int lastPercentage = 0;
            //double dblIteration = 0;
            double dblSumMinutes = 0;
            double dblGoal = (((intN) * ((intN) - 1.0)) / 2.0);
            var mst = new Mst(EdgeThreshold,
                              NodeDegreesThreshold,
                              BranchLenghtThreshold,
                              AdjacentNodeThreshold,
                              AdjacentNodeThreshold);

            mst.OnGetDistance += OnGetDistance;
            //
            // declare string metrics
            //
            TagLinkCheap cheapLink1 = new TagLinkCheap(m_data, 2, TokenStatistics);
            TagLinkCheap cheapLink2 = new TagLinkCheap(m_data, 4, TokenStatistics);
            TagLinkCheap cheapLink3 = new TagLinkCheap(m_data, 8, TokenStatistics);

            DateTime start = DateTime.Now;
            DateTime start2 = DateTime.Now;
            int intTotalComparisons = 0;
            const string strMessage = "Please wait. Comparing records...";
            SendMessageEvent.OnSendMessage(strMessage, 0);
            InvokeProgressBarEventHandler(strMessage, 0);
            Parallel.For(0, intN, delegate(int i)
                                      //for (int i = 0; i < intN; i++)
                                      {
                                          List<MstDistanceObj> candidateList = new List<MstDistanceObj>();
                                          List<RowWrapper> candidateRowList = new List<RowWrapper>();
                                          List<int> indexList = new List<int>();

                                          GetCandidatesList(
                                              i, 
                                              intN, 
                                              blocker, 
                                              mst, 
                                              cheapLink1, 
                                              cheapLink2, 
                                              cheapLink3, 
                                              candidateList, 
                                              candidateRowList, 
                                              indexList);

                                          //
                                          // define column and token weights for each cluster
                                          //
                                          // compare the candidates
                                          if (candidateRowList.Count > 0)
                                          {
                                              candidateRowList.Add(m_data.Data[i]);
                                              DataWrapper candidatesDataWrapper = 
                                                  new DataWrapper(
                                                    candidateRowList.ToArray());
                                              int[] indexArray = indexList.ToArray();
                                              TokenStatistics tokenStatistics2 =
                                                  new TokenStatistics(candidatesDataWrapper);
                                              TagLink tagLink2 = new TagLink(candidatesDataWrapper, 
                                                                              tokenStatistics2, 
                                                                              EdgeThreshold);

                                              // sort the candidate list
                                              List<MstDistanceObj> candidateList2 = new List<MstDistanceObj>();
                                              candidateList.Sort();
                                              candidateList.Reverse();
                                              int searchCount = 0;
                                              // search for the most likely match
                                              foreach (MstDistanceObj currentScoreObject2 in candidateList)
                                              {
                                                  int y = currentScoreObject2.GetY();
                                                  // compute taglink
                                                  double currentStringMetric =
                                                      tagLink2.GetStringMetric(y, candidateRowList.Count - 1);
                                                  if (currentStringMetric > 0.0)
                                                  {
                                                      MstDistanceObj actualScoreObject =
                                                          new MstDistanceObj(i, indexArray[y], currentStringMetric);
                                                      candidateList2.Add(actualScoreObject);
                                                  }

                                                  searchCount++;

                                                  if (searchCount >= m_intSearchLength)
                                                  {
                                                      break;
                                                  }
                                              }

                                              // process the colected scores

                                              candidateList2.Sort();
                                              candidateList2.Reverse();
                                              foreach (MstDistanceObj currentScoreObject2 in candidateList2)
                                              {
                                                  if (currentScoreObject2.GetScore() > 1.0)
                                                  {
                                                      currentScoreObject2.SetScore(1.0);
                                                  }
                                                  lock (m_lockObject)
                                                  {
                                                      mst.Link(currentScoreObject2, null);
                                                  }
                                              }
                                          }

                                          //
                                          // display progress
                                          //
                                          DisplayProgress(
                                              i, 
                                              intN, 
                                              ref intTotalComparisons, 
                                              dblGoal, 
                                              ref lastPercentage, 
                                              ref start, 
                                              ref dblSumMinutes, 
                                              strMessage);
                                      }
                );
            TimeSpan ts2 = DateTime.Now - start2;
            PrintToScreen.WriteLine("time: " + ts2.TotalSeconds);
            return mst;
        }

        private void GetCandidatesList(
            int i, 
            int intN, 
            Blocker blocker, 
            Mst mst, 
            TagLinkCheap cheapLink1, 
            TagLinkCheap cheapLink2, 
            TagLinkCheap cheapLink3, 
            List<MstDistanceObj> candidateList,
            List<RowWrapper> rowList, 
            List<int> indexList)
        {
            for (int j = i + 1; j < intN; j++)
            {
                if (blocker.CheckCodeMatch(i, j))
                {
                    if (mst.CheckLoop(i + 1, j + 1))
                    {
                        if (cheapLink1.GetStringMetric(i, j) >= m_dblCheapThreshold1)
                        {
                            if (cheapLink2.GetStringMetric(i, j) >= m_dblCheapThreshold2)
                            {
                                double cheapStringMetric = cheapLink3.GetStringMetric(i, j);
                                if (cheapStringMetric >= m_dblCheapThreshold3)
                                {
                                    MstDistanceObj actualScoreObject =
                                        new MstDistanceObj(i, candidateList.Count,
                                                           cheapStringMetric);
                                    candidateList.Add(actualScoreObject);

                                    // add row to the list
                                    rowList.Add(m_data.Data[j]);
                                    indexList.Add(j);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DisplayProgress(
            int i, 
            int intN, 
            ref int intTotalComparisons, 
            double dblGoal, 
            ref int lastPercentage, 
            ref DateTime start, 
            ref double dblSumMinutes, 
            string strMessage)
        {
            lock (m_lockObject)
            {
                intTotalComparisons += intN - i;
                int intPercentage = (int)((intTotalComparisons / dblGoal) * 100.00);
                if (intPercentage != lastPercentage)
                {
                    TimeSpan ts = DateTime.Now - start;
                    dblSumMinutes += ((ts.TotalSeconds) / 60.0);
                    double avgMinutes = dblSumMinutes / (intPercentage);
                    int estimatedTime = (int)((100.00 - intPercentage) * avgMinutes) + 1;
                    string message = "Comparing records... Total comparisons: " +
                                     intTotalComparisons + ", Completed: " +
                                     intPercentage + "%. Estimated time for completion: " +
                                     estimatedTime + " Min.";
                    SendMessageEvent.OnSendMessage(message, intPercentage);
                    InvokeProgressBarEventHandler(message, intPercentage);
                    PrintToScreen.WriteLine(strMessage + "," + intPercentage);
                    start = DateTime.Now;
                }
                lastPercentage = intPercentage;
            }
        }

        private double OnGetDistance(int intI, int intJ)
        {
            double tmpScore = 0;
            if (m_cheapLink.GetStringMetric(intI, intJ) >= 0.1)
            {
                tmpScore = m_tagLink.GetStringMetric(intI, intJ);
            }
            return tmpScore;
        }

        private void InvokeProgressBarEventHandler(
            string strMessage, int p)
        {
            if(ProgressBarEventHandler != null &&
                ProgressBarEventHandler.GetInvocationList().Length > 0)
            {
                var sendMessageEventArgs = 
                    new SendMessageEventArgs
                        {
                            Progress = p, 
                            StrMessage = strMessage
                        };
                ProgressBarEventHandler.Invoke(this,
                    sendMessageEventArgs);
            }
        }
    }
}
