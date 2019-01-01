#region

using System.Collections.Generic;
using HC.Analytics.MachineLearning.MstCluster;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining
{
    public class MatchTwoDatasets
    {
        #region Members

        private readonly DataWrapper m_dataWrapper1;
        private readonly DataWrapper m_dataWrapper2;

        #endregion

        #region Constructors

        public MatchTwoDatasets(
            DataWrapper dataWrapper1,
            DataWrapper dataWrapper2)
        {
            m_dataWrapper1 = dataWrapper1;
            m_dataWrapper2 = dataWrapper2;
        }

        #endregion

        #region Public

        public void DoMatch()
        {
            List<MstDistanceObj> resultList = new List<MstDistanceObj>();
            TokenStatistics tokenStatistics1 = new TokenStatistics(m_dataWrapper1);
            TokenStatistics tokenStatistics2 = new TokenStatistics(m_dataWrapper2);
            TagLinkCheapNew tagLinkCheap1 = new TagLinkCheapNew(200);
            TagLinkCheapNew tagLinkCheap2 = new TagLinkCheapNew(400);
            TagLinkCheapNew tagLinkCheap3 = new TagLinkCheapNew(800);

            for (int i = 0; i < m_dataWrapper1.Length; i++)
            {
                List<RowWrapper> candidateRowList = new List<RowWrapper>();
                List<MstDistanceObj> candidateScoreList = new List<MstDistanceObj>();
                List<int> indexList = new List<int>();
                for (int j = 0; j < m_dataWrapper2.Length; j++)
                {
                    double dblCheapScore;
                    bool blnMatch = GetCheapSimMatch(
                        i,
                        j,
                        tagLinkCheap1,
                        tagLinkCheap2,
                        tagLinkCheap3,
                        tokenStatistics1,
                        tokenStatistics2,
                        out dblCheapScore);
                    if (blnMatch)
                    {
                        candidateRowList.Add(m_dataWrapper2.Data[j]);
                        MstDistanceObj actualScoreObject =
                            new MstDistanceObj(
                                i,
                                candidateRowList.Count,
                                dblCheapScore);
                        candidateScoreList.Add(actualScoreObject);
                        indexList.Add(j);
                    }
                }

                if (candidateRowList.Count > 0)
                {
                    //
                    // get data
                    //
                    ProcessCandidates(
                        i, 
                        candidateRowList, 
                        candidateScoreList, 
                        indexList, 
                        resultList);
                }
            }
        }

        private void ProcessCandidates(
            int i, 
            List<RowWrapper> candidateRowList, 
            List<MstDistanceObj> candidateList, 
            List<int> indexList, 
            List<MstDistanceObj> resultList)
        {
            candidateRowList.Add(m_dataWrapper1.Data[i]);
            DataWrapper candidatesDataWrapper =
                new DataWrapper(
                    candidateRowList.ToArray());
            //
            // get statistics
            //
            TokenStatistics tokenStatistics =
                new TokenStatistics(candidatesDataWrapper);
            TagLink tagLink2 = new TagLink(candidatesDataWrapper,
                                           tokenStatistics,
                                           MstClusterConstants.EDGE_THRESHOLD);

            // sort the candidate list
            candidateList.Sort();
            candidateList.Reverse();
            //
            // search for the most likely match
            //
            foreach (MstDistanceObj mstDistanceObj in candidateList)
            {
                int y = mstDistanceObj.GetY();
                //
                // compute taglink
                //
                double currentStringMetric =
                    tagLink2.GetStringMetric(y, candidateRowList.Count - 1);
                if (currentStringMetric > 0.7)
                {
                    MstDistanceObj actualScoreObject =
                        new MstDistanceObj(-1, indexList[y], currentStringMetric);
                    resultList.Add(actualScoreObject);
                }
            }
        }

        private bool GetCheapSimMatch(
            int i,
            int j,
            TagLinkCheapNew tagLinkCheap1,
            TagLinkCheapNew tagLinkCheap2,
            TagLinkCheapNew tagLinkCheap3,
            TokenStatistics tokenStatistics1,
            TokenStatistics tokenStatistics2,
            out double dblCheapScore)
        {
            dblCheapScore = -1;
            double dblScore1 = tagLinkCheap1.GetStringMetric(
                m_dataWrapper1.Data[i].Columns,
                m_dataWrapper2.Data[j].Columns,
                tokenStatistics1.IdfWeights[i],
                tokenStatistics2.IdfWeights[j]);

            bool blnMatch = false;
            if (dblScore1 >= TextMiningConstants.PURGER_CHEAP_THRESHOLD_1)
            {
                if (dblScore1 >= TextMiningConstants.PURGER_CHEAP_THRESHOLD_2)
                {
                    if (dblScore1 >= TextMiningConstants.PURGER_CHEAP_THRESHOLD_3)
                    {
                        dblCheapScore = dblScore1;
                        blnMatch = true;
                    }
                    else
                    {
                        //
                        // get score3
                        //
                        blnMatch = MatchScore3(
                            i,
                            j,
                            tokenStatistics1,
                            tokenStatistics2,
                            tagLinkCheap3,
                            out dblCheapScore);
                    }
                }
                else
                {
                    //
                    // get score2
                    //
                    double dblScore2 = tagLinkCheap2.GetStringMetric(
                        m_dataWrapper1.Data[i].Columns,
                        m_dataWrapper2.Data[j].Columns,
                        tokenStatistics1.IdfWeights[i],
                        tokenStatistics2.IdfWeights[j]);
                    if (dblScore2 >= TextMiningConstants.PURGER_CHEAP_THRESHOLD_2)
                    {
                        if (dblScore2 >= TextMiningConstants.PURGER_CHEAP_THRESHOLD_3)
                        {
                            blnMatch = true;
                        }
                        else
                        {
                            //
                            // get score3
                            //
                            blnMatch = MatchScore3(
                                i,
                                j,
                                tokenStatistics1,
                                tokenStatistics2,
                                tagLinkCheap3,
                                out dblCheapScore);
                        }
                    }
                }
            }
            return blnMatch;
        }

        private bool MatchScore3(
            int i,
            int j,
            TokenStatistics tokenStatistics1,
            TokenStatistics tokenStatistics2,
            TagLinkCheapNew tagLinkCheap3,
            out double dblCheapScore)
        {
            bool blnMatch = false;
            double dblScore3 = tagLinkCheap3.GetStringMetric(
                m_dataWrapper1.Data[i].Columns,
                m_dataWrapper2.Data[j].Columns,
                tokenStatistics1.IdfWeights[i],
                tokenStatistics2.IdfWeights[j]);
            dblCheapScore = dblScore3;
            if (dblScore3 > TextMiningConstants.PURGER_CHEAP_THRESHOLD_3)
            {
                blnMatch = true;
            }
            return blnMatch;
        }

        #endregion

    }
}

