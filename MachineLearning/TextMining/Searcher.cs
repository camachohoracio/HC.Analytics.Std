#region

using System;
using System.Collections.Generic;
using HC.Analytics.MachineLearning.MstCluster;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics;
using HC.Core;
using HC.Core.Events;
using HC.Core.Logging;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining
{
    public class Searcher : IDisposable
    {
        #region Members

        //
        // declare progress bar event handler
        //
        public event EventHandler ProgressBarEventHandler;

        //
        // parameters
        //
        private readonly TagLinkCheap m_tagLinkCheapStringMetric1;
        private readonly TagLinkCheap m_tagLinkCheapStringMetric2;
        private readonly TagLinkCheap m_tagLinkCheapStringMetric3;
        private readonly TokenStatistics m_tokenStatistics;
        private readonly double m_dblCheapThreshold1;
        private readonly double m_dblCheapThreshold2;
        private readonly double m_dblCheapThreshold3;
        private readonly DataWrapper m_strDataArray;
        private readonly TagLink m_stringMetric;

        #endregion

        public Searcher(DataWrapper dataArray)
            : this(dataArray, 0)
        {
        }

        public Searcher(DataWrapper dataArray, int intSearchIntensity)
        {
            try
            {
                if (dataArray.Length == 0)
                {
                    return;
                }
                m_dblCheapThreshold1 = TextMiningConstants.DBL_SEARCHER_CHEAP_THRESHOLD_1;
                m_dblCheapThreshold2 = TextMiningConstants.DBL_SEARCHER_CHEAP_THRESHOLD_2;
                m_dblCheapThreshold3 = TextMiningConstants.DBL_SEARCHER_CHEAP_THRESHOLD_3;

                m_strDataArray = dataArray;
                m_tokenStatistics = new TokenStatistics(dataArray);
                if (intSearchIntensity <= 0)
                {
                    m_stringMetric = new TagLink(dataArray, m_tokenStatistics, 0.2);
                }
                else
                {
                    m_stringMetric = new TagLink(dataArray, m_tokenStatistics, intSearchIntensity, 0.2);
                }
                m_tagLinkCheapStringMetric1 = new TagLinkCheap(dataArray, intSearchIntensity + 2, m_tokenStatistics);
                m_tagLinkCheapStringMetric2 = new TagLinkCheap(dataArray, intSearchIntensity + 4, m_tokenStatistics);
                m_tagLinkCheapStringMetric3 = new TagLinkCheap(dataArray, intSearchIntensity + 8, m_tokenStatistics);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public List<MstDistanceObj> Search(TokenWrapper[][] row)
        {
            int intN = m_strDataArray.Length;
            int lastPercentage = 0;
            double iteration = 0;
            double sumMinutes = 0;
            double goal = intN;

            DateTime start = DateTime.Now;
            const string strMessage = "Please wait. Comparing records...";
            SendMessageEvent.OnSendMessage(strMessage, 0);
            InvokeProgressBarEventHandler(strMessage, 0);

            List<MstDistanceObj> candidateList = new List<MstDistanceObj>();

            for (int j = 0; j < intN; j++, iteration++)
            {
                if (m_tagLinkCheapStringMetric1.GetStringMetric(row, j) >= m_dblCheapThreshold1)
                {
                    if (m_tagLinkCheapStringMetric2.GetStringMetric(row, j) >= m_dblCheapThreshold2)
                    {
                        if (m_tagLinkCheapStringMetric3.GetStringMetric(row, j) >= m_dblCheapThreshold3)
                        {
                            // compute TagLink
                            double tagLinkScore = m_stringMetric.GetStringMetric(row, j);
                            if (tagLinkScore > 0.0)
                            {
                                MstDistanceObj actualScoreObject =
                                    new MstDistanceObj(-1, j, tagLinkScore);
                                candidateList.Add(actualScoreObject);
                            }
                        }
                    }
                }
                //
                // update progress
                //
                int percentage = (int) ((iteration/goal)*100.00);
                if (percentage != lastPercentage)
                {
                    DateTime end = DateTime.Now;
                    TimeSpan ts = end - start;
                    sumMinutes += ((ts.TotalSeconds)/60.0);
                    double avgMinutes = sumMinutes/(percentage);
                    int estimatedTime = (int) ((100.00 - percentage)*avgMinutes) + 1;
                    string message = "Comparing records... Completed: " +
                                     percentage + "%. Estimated time for completion: " +
                                     estimatedTime + " Min.";

                    SendMessageEvent.OnSendMessage(message, percentage);
                    InvokeProgressBarEventHandler(message, percentage);
                    start = DateTime.Now;
                }
                lastPercentage = percentage;
            }
            // process the colected scores

            candidateList.Sort();
            candidateList.Reverse();
            return candidateList;
        }

        private void InvokeProgressBarEventHandler(
            string strMessage, int p)
        {
            if (ProgressBarEventHandler != null &&
                ProgressBarEventHandler.GetInvocationList().Length > 0)
            {
                SendMessageEventArgs sendMessageEventArgs =
                    new SendMessageEventArgs();
                sendMessageEventArgs.Progress = p;
                sendMessageEventArgs.StrMessage = strMessage;
                ProgressBarEventHandler.Invoke(this,
                                               sendMessageEventArgs);
            }
        }

        public List<MstDistanceObj> Search(string str)
        {
            try
            {
                TokenWrapper[] toks = Tokeniser.TokeniseAndWrap(str);
                return Search(new[] {toks});
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<MstDistanceObj>();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            m_tagLinkCheapStringMetric1.Dispose();
            m_tagLinkCheapStringMetric2.Dispose();
            m_tagLinkCheapStringMetric3.Dispose();
            m_tokenStatistics.Dispose();
            m_strDataArray.Dispose();
            m_stringMetric.Dispose();
        }
    }
}
