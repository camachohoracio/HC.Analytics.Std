#region

using System;
using System.Collections.Generic;
using System.IO;
using HC.Analytics.MachineLearning.MstCluster;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics;
using HC.Core.Events;
using HC.Core.Helpers;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining
{
    public class Merger
    {
        #region Events

        public event EventHandler ProgressBarEventHandler;

        #endregion

        #region Members

        // parameters
        private readonly TokenStatistics tokenStatistics;
        private readonly double m_dblCheapThreshold1;
        private readonly double m_dblCheapThreshold2;
        private readonly double m_dblCheapThreshold3;
        private readonly double m_dblThreshold;
        // define the number each record will search for a match
        private readonly int m_intDbSize1;
        private readonly int m_intDbSize2;
        private readonly int m_intSearchLength;
        private readonly string m_strTableName;
        private int m_intColumnCount;
        private DataWrapper m_strDataArray;

        #endregion

        #region Constructos

        public Merger(
            DataWrapper dataArray, 
            int dbSize1, 
            int dbSize2, 
            string tableName)
        {
            m_dblCheapThreshold1 = TextMiningConstants.DBL_MERGER_CHEAP_THRESHOLD_1;
            m_dblCheapThreshold2 = TextMiningConstants.DBL_MERGER_CHEAP_THRESHOLD_2;
            m_dblCheapThreshold3 = TextMiningConstants.DBL_MERGER_CHEAP_THRESHOLD_3;
            m_dblThreshold = TextMiningConstants.DBL_MERGER_THRESHOLD;
            m_intSearchLength = TextMiningConstants.INT_MERGER_SEARCH_LENGTH;
            m_strDataArray = dataArray;
            m_strTableName = tableName;
            m_intDbSize1 = dbSize1;
            m_intDbSize2 = dbSize2;
            tokenStatistics = new TokenStatistics(dataArray);
            m_intColumnCount = dataArray.Data[0].Columns.Length;
        }

        #endregion


        public void merge()
        {
            int N = m_strDataArray.Length, lastPercentage = 0, estimatedTime = Int32.MaxValue;
            double iteration = 0,
                   sumMinutes = 0.0,
                   goal = m_intDbSize1*m_intDbSize2;
            // declare the merge result
            List<MstDistanceObj> mergeResultList = new List<MstDistanceObj>();
            // declare string metrics
            TagLink tagLink = new TagLink(m_strDataArray, tokenStatistics, m_dblThreshold);
            TagLinkCheap ceapLink1 = new TagLinkCheap(m_strDataArray, 2, tokenStatistics);
            TagLinkCheap ceapLink2 = new TagLinkCheap(m_strDataArray, 4, tokenStatistics);
            TagLinkCheap ceapLink3 = new TagLinkCheap(m_strDataArray, 8, tokenStatistics);
            DateTime start = DateTime.Now, end = DateTime.Now;

            SendMessageEvent.OnSendMessage("Please wait. Comparing records...", 0);

            for (int i = 0; i < m_intDbSize1; i++)
            {
                // to do :
                // Application.DoEvents();

                List<MstDistanceObj> candidateList = new List<MstDistanceObj>();
                for (int j = m_intDbSize1; j < N; j++, iteration++)
                {
                    if (ceapLink1.GetStringMetric(i, j) >= m_dblCheapThreshold1)
                    {
                        if (ceapLink2.GetStringMetric(i, j) >= m_dblCheapThreshold2)
                        {
                            double cheapStringMetric = ceapLink3.GetStringMetric(i, j);
                            if (cheapStringMetric >= m_dblCheapThreshold3)
                            {
                                MstDistanceObj actualScoreObject =
                                    new MstDistanceObj(i, j, cheapStringMetric);
                                candidateList.Add(actualScoreObject);
                            }
                        }
                    }
                    int percentage = (int) ((iteration/goal)*100.00);
                    if (percentage != lastPercentage)
                    {
                        end = DateTime.Now;
                        TimeSpan ts = end - start;
                        sumMinutes += ((ts.TotalSeconds)/60.0);
                        double avgMinutes = sumMinutes/(percentage);
                        estimatedTime = (int) ((100.00 - percentage)*avgMinutes) + 1;
                        string message = "Comparing records... Completed: " +
                                         percentage + "%. Estimated time for completion: " +
                                         estimatedTime + " Min.";
                        PrintToScreen.WriteLine(message);

                        SendMessageEvent.OnSendMessage(message, percentage);

                        start = DateTime.Now;
                    }
                    lastPercentage = percentage;
                }

                // to do :
                //Application.DoEvents();

                // sort the candidate list
                candidateList.Sort();
                candidateList.Reverse();
                int searchCount = 0, maxX = -1, maxY = -1;
                double maxStringMetric = -1.0;
                // search for the most likely match
                foreach (MstDistanceObj currentScoreObject2 in candidateList)
                {
                    int x = currentScoreObject2.GetX(),
                        y = currentScoreObject2.GetY();
                    // compute taglink
                    double currentStringMetric = tagLink.GetStringMetric(x, y);
                    if (currentStringMetric > maxStringMetric)
                    {
                        maxStringMetric = currentStringMetric;
                        maxX = x;
                        maxY = y;
                    }
                    if (currentStringMetric >= 0.999999)
                    {
                        searchCount = m_intSearchLength;
                    }
                    searchCount++;
                    if (searchCount >= m_intSearchLength)
                    {
                        break;
                    }
                }
                // add the most likely match to the result list
                if (maxStringMetric > -1 && maxStringMetric >= m_dblThreshold)
                {
                    if (maxStringMetric > 1.0)
                    {
                        maxStringMetric = 1.0;
                    }
                    MstDistanceObj actualScoreObject =
                        new MstDistanceObj(maxX, maxY, maxStringMetric);
                    mergeResultList.Add(actualScoreObject);
                }
            }
            saveResults(mergeResultList, N);
        }

        private void saveResults(List<MstDistanceObj> mergeResultList, int N)
        {
            StreamWriter sw = new StreamWriter(m_strTableName + "_matches.txt");
            // sort the result list
            mergeResultList.Sort();
            mergeResultList.Reverse();
            foreach (MstDistanceObj currentScoreObject2 in mergeResultList)
            {
                int intX = currentScoreObject2.GetX();
                int intY = currentScoreObject2.GetY();
                // compute taglink
                double currentScore = currentScoreObject2.GetScore();
                sw.WriteLine(intX + "\t" + intY + "\t" + currentScore);
            }
            sw.Close();
        }
    }
}
