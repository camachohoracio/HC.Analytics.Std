using System;
using System.Collections.Generic;
using System.Text;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics;
using HC.Core.Text;

namespace HC.Analytics.MachineLearning.TextMining
{
    public class StringExtractor
    {
        #region Members

        private Dictionary<string, List<int>> m_mapTokenToDocumentLocation;
        private TokenWrapper[] m_documentTokens;
        private TagLinkCheap m_cheapLink;
        private TagLink m_tagLink;
        private TokenStatistics m_tokenStatistics;
        private DataWrapper m_dataWrapper;

        #endregion

        #region Constructors

        public StringExtractor(
            string strDocument)
        {
            GetTokenPosition(strDocument);
            LoadData();
            LoadStringMetrics();
        }

        #endregion

        #region Public

        public List<StringExtractorResult> Extract(
            string strLine)
        {
            TokenWrapper[] currTokens = Tokeniser.TokeniseAndWrap(strLine);
            var strTTokens = new TokenWrapper[1][];
            strTTokens[0] = currTokens;
            List<StringExtractorResult> matchResults = Extract(strTTokens);
            return matchResults;
        }

        public List<StringExtractorResult> Extract(
            TokenWrapper[][] strTTokens)
        {
            TokenWrapper[] currTokens = strTTokens[0];
            var matchResults = new List<StringExtractorResult>();
            for (int i = 0; i < currTokens.Length; i++)
            {
                TokenWrapper tokenWrapper = currTokens[i];
                List<int> positionList;
                if (m_mapTokenToDocumentLocation.TryGetValue(
                    tokenWrapper.Token,
                    out positionList))
                {
                    for (int j = 0; j < positionList.Count; j++)
                    {
                        int intPosition = positionList[j];
                        StringExtractorResult currMatchResults = SearchAtPosition(
                            strTTokens,
                            intPosition);
                        if (currMatchResults != null)
                        {
                            matchResults.Add(currMatchResults);
                        }
                    }
                }
            }
            return matchResults;
        }

        #endregion

        #region Private

        private StringExtractorResult SearchAtPosition(
            TokenWrapper[][] strTTokens, 
            int intPosition)
        {
            int intTokenLength = strTTokens[0].Length;
            int intMinIndex = Math.Max(0, intPosition - intTokenLength);
            //var matches = new List<StringExtractorResult>();
            for (int i = intMinIndex; i <= intPosition; i++)
            {
                int intCurrEndPosition = Math.Min(m_documentTokens.Length - 1,
                                                  i + intTokenLength - 1);
                var strUTokens = new TokenWrapper[1][];
                strUTokens[0] = GetTokenSubset(i, intCurrEndPosition);
                double dblCheapMetric = m_cheapLink.GetStringMetric(
                    strTTokens,
                    strUTokens);
                if(dblCheapMetric > 0.1)
                {
                    double dblExpensiveMetric = m_tagLink.GetStringMetric(
                        strTTokens,
                        strUTokens);
                    if(dblExpensiveMetric > 0.85 &&
                        Math.Abs(strTTokens[0].Length - strUTokens[0].Length) <= 2)
                    {
                        return new StringExtractorResult
                                   {
                                       Position = i,
                                       Similarity = dblExpensiveMetric,
                                       T = GetStr(strTTokens),
                                       U = GetStr(strUTokens),
                                   };
                    }
                }

            }
            return null;
        }

        private static string GetStr(TokenWrapper[][] tokens)
        {
            var sb = new StringBuilder();
            bool blnLoadTitle = true;
            for (int i = 0; i < tokens[0].Length; i++)
            {
                if(blnLoadTitle)
                {
                    blnLoadTitle = false;
                }
                else
                {
                    sb.Append(" ");
                }
                sb.Append(tokens[0][i]);
            }
            return sb.ToString();
        }

        private void LoadStringMetrics()
        {
            m_cheapLink = new TagLinkCheap(
                m_dataWrapper,
                100000000,
                100000000,
                m_tokenStatistics);
            m_tagLink = new TagLink(
                m_dataWrapper,
                m_tokenStatistics,
                100000000,
                0.2);
        }

        private void LoadData()
        {
            m_dataWrapper = new DataWrapper(m_documentTokens);
            m_tokenStatistics = new TokenStatistics(m_dataWrapper);
        }

        private TokenWrapper[] GetTokenSubset(
            int intStartPosition,
            int intEndPosition)
        {
            var tokens = new TokenWrapper[intEndPosition - intStartPosition + 1];
            for (int i = intStartPosition, intPosition = 0; 
                i <= intEndPosition; 
                i++, intPosition++)
            {
                tokens[intPosition] = m_documentTokens[i];
            }
            return tokens;
        }

        private void GetTokenPosition(string strDocument)
        {
            m_mapTokenToDocumentLocation = new Dictionary<string, List<int>>();
            m_documentTokens = Tokeniser.TokeniseAndWrap(strDocument);
            for (int i = 0; i < m_documentTokens.Length; i++)
            {
                string strToken = m_documentTokens[i].Token;
                List<int> locationList;
                if(!m_mapTokenToDocumentLocation.TryGetValue(
                    strToken, 
                    out locationList))
                {
                    locationList = new List<int>();
                    m_mapTokenToDocumentLocation[strToken] = locationList;
                }
                locationList.Add(i);
            }
        }

        #endregion
    }
}

