#region

using System;
using System.Collections.Generic;
using HC.Analytics.MachineLearning.TextMining.DataStructures;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Core.Logging;
using HC.Core.Text;
using NUnit.Framework;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLink : IStringMetric, IDisposable
    {
        #region Constants

        private const int WINDOW_SIZE = 3;
        private const double TOTAL_SCORE_THRESHOLD = 0.2;

        #endregion

        #region Properties

        public double TokenThreshold { get; set; } 

        #endregion

        #region Members

        private TokenStatistics m_tokenStatistics;
        private readonly double m_dblLossThreshold;
        private int m_intColumnCount;
        private readonly int m_intW;
        private TagLinkTokenCheap m_tagLinkToken;
        private DataWrapper m_strDataArray;

        #endregion

        #region Constructors

        public TagLink(
                DataWrapper rowObjectArray,
                TokenStatistics tokenStatistics) : this(
                    rowObjectArray,
                    tokenStatistics,
                    WINDOW_SIZE,
                    TOTAL_SCORE_THRESHOLD)
        {
        }

        public TagLink(
            double threshold)
            : this(null,
                   null,
                   WINDOW_SIZE,
                   threshold) { }

        public TagLink()
            : this(null,
                   null,
                   WINDOW_SIZE,
                   TOTAL_SCORE_THRESHOLD)
        { }

        public TagLink(
            DataWrapper rowObjectArray,
            TokenStatistics tokenStatistics,
            double threshold)
            : this(rowObjectArray,
                   tokenStatistics,
                   WINDOW_SIZE,
                   threshold) { }
        
        public TagLink(
            DataWrapper strDataArray,
            TokenStatistics tokenStatistics,
            int intW,
            double dblThreshold)
        {
            m_tokenStatistics = tokenStatistics;
            m_intW = intW;
            m_strDataArray = strDataArray;
            if (strDataArray != null &&
                strDataArray.Data.Length > 0)
            {
                m_intColumnCount = strDataArray.Data[0].Columns.Length;
            }
            m_tagLinkToken = new TagLinkTokenCheap();
            m_dblLossThreshold = 1.0 - dblThreshold;
        }

        #endregion

        #region Public

        public double GetStringMetric(
            string strT,
            string strU)
        {
            if (m_tokenStatistics == null)
            {
                m_tokenStatistics = 
                    new TokenStatistics(
                        strT + " " +
                        strU);
                m_intColumnCount = 1;
            }
            var tokT =
                new TokenWrapper[1][];
            tokT[0] = Tokeniser.TokeniseAndWrap(strT);
            var tokU =
                new TokenWrapper[1][];
            tokU[0] = Tokeniser.TokeniseAndWrap(strU);
            return GetStringMetric(
                tokT,
                tokU);
        }

        public double GetStringMetric(
            TokenWrapper[][] tTokens,
            TokenWrapper[][] uTokens)
        {
            double[][] tIdfArray = m_tokenStatistics.GetIdfArray(tTokens);
            double[][] uIdfArray = m_tokenStatistics.GetIdfArray(uTokens);
            return GetStringMetric(
                m_intColumnCount,
                tTokens,
                uTokens,
                tIdfArray,
                uIdfArray,
                m_intW,
                m_tagLinkToken,
                m_dblLossThreshold);
        }

        public double GetStringMetric(
            TokenWrapper[][] tTokens, 
            int rowU)
        {
            TokenWrapper[][] uTokens = m_strDataArray.Data[rowU].Columns;
            double[][] tIdfArray = m_tokenStatistics.GetIdfArray(tTokens);
            double[][] uIdfArray = m_tokenStatistics.GetIdfArray(rowU);
            return GetStringMetric(
                m_intColumnCount, 
                tTokens, 
                uTokens, 
                tIdfArray, 
                uIdfArray,
                m_intW,
                m_tagLinkToken,
                m_dblLossThreshold);
        }

        public double GetStringMetric(
            int rowT, 
            int rowU)
        {
            TokenWrapper[][] tTokens = m_strDataArray.Data[rowT].Columns;
            TokenWrapper[][] uTokens = m_strDataArray.Data[rowU].Columns;
            double[][] tIdfArray = m_tokenStatistics.GetIdfArray(rowT);
            double[][] uIdfArray = m_tokenStatistics.GetIdfArray(rowU);
            return GetStringMetric(
                m_intColumnCount,
                tTokens, 
                uTokens, 
                tIdfArray, 
                uIdfArray,
                m_intW,
                m_tagLinkToken,
                m_dblLossThreshold);
        }

        public double GetStringMetric(
            int rowT,
            int rowU,
            DataWrapper dataWrapper1,
            DataWrapper dataWrapper2,
            TokenStatistics tokenStatistics1,
            TokenStatistics tokenStatistics2)
        {
            TokenWrapper[][] tTokens = dataWrapper1.Data[rowT].Columns;
            TokenWrapper[][] uTokens = dataWrapper2.Data[rowU].Columns;
            double[][] tIdfArray = tokenStatistics1.GetIdfArray(rowT);
            double[][] uIdfArray = tokenStatistics2.GetIdfArray(rowU);
            return GetStringMetric(
                m_intColumnCount,
                tTokens,
                uTokens,
                tIdfArray,
                uIdfArray,
                m_intW,
                m_tagLinkToken,
                m_dblLossThreshold);
        }

        #endregion

        #region Private

        private double GetStringMetric(
            int totalColumns, 
            TokenWrapper[][] tTokens,
            TokenWrapper[][] uTokens, 
            double[][] tIdfArray, 
            double[][] uIdfArray,
            int intW,
            TagLinkTokenCheap tagLinkToken,
            double dblLossThreshold)
        {
            try
            {
            // let T be the smallest string size
            int tSize = GetMinStringSize(tTokens);
            int uSize = GetMinStringSize(uTokens);
            double minStringSize = Math.Min(tSize, uSize);

            if (uSize < tSize)
            {
                TokenWrapper[][] tmpTokens = tTokens;
                tTokens = uTokens;
                uTokens = tmpTokens;
                double[][] tmpIdfArray = tIdfArray;
                tIdfArray = uIdfArray;
                uIdfArray = tmpIdfArray;
            }
            bool blnComputeScore;
            bool blnPerfectMatch;
            List<Candidates> candidateList = GetCandidateList(
                tTokens,
                uTokens,
                tIdfArray,
                uIdfArray,
                totalColumns,
                minStringSize,
                out blnComputeScore,
                out blnPerfectMatch,
                intW,
                tagLinkToken,
                dblLossThreshold);

            if (!blnComputeScore)
            {
                return 0.0;
            }
            if (blnPerfectMatch)
            {
                return 1.0;
            }
            candidateList.Sort();
            candidateList.Reverse();
            double score = GetScore(candidateList);
            if (score >= 1.0)
            {
                return 0.999999999;
            }
            return score;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0.0;
        }


        private static double GetScore(List<Candidates> candidateList)
        {
            double scoreValue = 0;
            HashSet<string> tMap = new HashSet<string>();
            HashSet<string> uMap = new HashSet<string>();
            foreach (Candidates actualCandidates in candidateList)
            {
                int intActualTPos = actualCandidates.GetTPos();
                int intActualUPos = actualCandidates.GetUPos();
                int intActualTCol = actualCandidates.GetTCol();
                int intActualUCol = actualCandidates.GetUCol();
                string strTKey = intActualTPos + " " + intActualTCol;
                string strUKey = intActualUPos + " " + intActualUCol;
                if ((!tMap.Contains(strTKey)) &&
                    (!uMap.Contains(strUKey)))
                {
                    double actualScore = actualCandidates.GetScore();
                    tMap.Add(strTKey);
                    uMap.Add(strUKey);
                    scoreValue += actualScore;
                }
            }
            return scoreValue;
        }

        private List<Candidates> GetCandidateList(
            TokenWrapper[][] tTokens,
            TokenWrapper[][] uTokens,
            double[][] tIdfArray, 
            double[][] uIdfArray,
            int totalColumns,
            double minStringSize,
            out bool blnComputeScore,
            out bool blnPerfectMatch,
            int intW,
            TagLinkTokenCheap tagLinkToken,
            double dblLossThreshold)
        {
            blnComputeScore = true;
            blnPerfectMatch = true;
            var candidateList = new List<Candidates>();
            double dblTotalScoreLoss = 0.0;
            int intTRealPosition = 0;
            for (int columnT = 0; columnT < totalColumns && 
                blnComputeScore; columnT++)
            {
                int tLength = tTokens[columnT].Length;
                for (int t = 0; t < tLength && blnComputeScore; t++)
                {
                    double dblMaxScore = 0.0;
                    TokenWrapper tTok = tTokens[columnT][t];
                    int intLastTr = -1;
                    int intURealPosition = 0;
                    int intColumnU = columnT;
                    int intUTokensLength = uTokens[intColumnU].Length;
                    if (intUTokensLength == 0)
                    {
                        break;
                    }
                    int intLowLimit = Math.Min(Math.Max(0, t - intW), intUTokensLength - 1);
                    int intUpLimit = Math.Min(intUTokensLength, t + intW);
                    for (int u = intLowLimit, intFlag = 0; u < intUpLimit && intFlag == 0; u++)
                    {
                        int intTr = Math.Abs(intTRealPosition - intURealPosition);
                        if (intLastTr >= 0 && intLastTr < intTr)
                        {
                            intFlag = 1;
                        }
                        else
                        {
                            TokenWrapper uTok = uTokens[intColumnU][u];
                            double dblCheapInnerScore = TagLinkPrefix.GetStringMetric(tTok, uTok);
                            double dblInnerScore = 0.0;
                            if (dblCheapInnerScore >= 0.1)
                            {
                                dblInnerScore = 
                                    tagLinkToken.GetStringMetric(
                                        tTok, 
                                        uTok);
                            }
                            if (dblInnerScore >= 0.0)
                            {
                                double dblMatched;
                                if (dblInnerScore == 1.0)
                                {
                                    dblMatched = tTokens[columnT][t].Length;
                                }
                                else
                                {
                                    dblMatched = tagLinkToken.GetMatched();
                                    if (t.Equals(u) && blnPerfectMatch)
                                    {
                                        blnPerfectMatch = false;
                                    }
                                }
                                double weightMatched = 0.0;
                                if (minStringSize > 0.0)
                                {
                                    weightMatched = dblMatched/minStringSize;
                                }
                                double dblWeightTfidf =
                                    tIdfArray[columnT][t]*uIdfArray[intColumnU][u];
                                double weight = (weightMatched + dblWeightTfidf)/2.0;
                                if (dblInnerScore == 1)
                                {
                                    intLastTr = intTr;
                                }
                                if (dblInnerScore > 0)
                                {
                                    double tokenScore = dblInnerScore*weight;
                                    if (tokenScore > dblMaxScore)
                                    {
                                        dblMaxScore = tokenScore;
                                    }
                                    if (TokenThreshold == 0 || 
                                        tokenScore >= TokenThreshold)
                                    {
                                        candidateList.Add(
                                            new Candidates(t, u, columnT, intColumnU,
                                                           tokenScore));
                                    }
                                }
                            }
                        }
                        intURealPosition++;
                    }
                    //
                    // get the expected score
                    //
                    double dblExpectedScore =
                        ((tIdfArray[columnT][t]*
                          tIdfArray[columnT][t]) +
                         ((tTok.Length)/minStringSize))
                        /2.0;
                    dblTotalScoreLoss += (dblExpectedScore - dblMaxScore);
                    if (dblTotalScoreLoss > dblLossThreshold)
                    {
                        blnComputeScore = false;
                    }
                    intTRealPosition++;
                }
            }
            return candidateList;
        }

        private static int GetMinStringSize(
            TokenWrapper[][] tokens)
        {
            int tSize = 0;
            for (int column = 0; column < tokens.Length; column++)
            {
                for (int i = 0; i < tokens[column].Length; i++)
                {
                    tSize += tokens[column][i].Length;
                }
            }
            return tSize;
        }

        #endregion

        public void Dispose()
        {
            if (m_tokenStatistics != null)
            {


                m_tokenStatistics.Dispose();
                m_tokenStatistics = null;
            }
            m_tagLinkToken.Dispose();
            m_tagLinkToken = null;
            if (m_strDataArray != null)
            {
                m_strDataArray.Dispose();
                m_strDataArray = null;
            }
        }

        [Test]
        public static void DoTest()
        {
            string s1 = "hello world blaaasd";
            string s2 = "bla heello bl2a";
            double dblSim = new TagLink().GetStringMetric(
                s1,
                s2);
            Console.WriteLine("similarity = " + dblSim);
        }
    }
}
