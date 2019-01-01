#region

using System;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLinkCheapNew : IStringMetric
    {
        #region Members

        private readonly TagLinkCheapToken m_tagLinkCheapToken;
        private readonly TokenStatistics m_tokenStatistics;
        private readonly double m_dblThreshold;
        private readonly int m_intColumnCount;
        private readonly int m_intMaxComparisons;
        private readonly DataWrapper m_strDataArray;

        #endregion

        #region Constructors

        public TagLinkCheapNew(
            int intMaxComparisons)
            : this(null,
                intMaxComparisons,
                null) { }

        public TagLinkCheapNew(
            DataWrapper rowObjectArray,
            int intMaxComparisons,
            TokenStatistics tokenStatistics)
        {
            m_dblThreshold = TextMiningConstants.DBL_CHEAP_LINK_THRESHOLD;
            m_tokenStatistics = tokenStatistics;
            m_strDataArray = rowObjectArray;
            m_intColumnCount = rowObjectArray.Data[0].Columns.Length;
            m_tagLinkCheapToken = new TagLinkCheapToken();
            m_intMaxComparisons = intMaxComparisons;
        }

        #endregion

        #region Public

        public double GetStringMetric(int rowT, int rowU)
        {
            if (m_strDataArray == null)
            {
                return 0.0;
            }
            TokenWrapper[][] tTokens = m_strDataArray.Data[rowT].Columns;
            TokenWrapper[][] uTokens = m_strDataArray.Data[rowU].Columns;
            double[][] tIdfArray = null;
            double[][] uIdfArray = null;
            if (m_tokenStatistics != null)
            {
                tIdfArray = m_tokenStatistics.GetIdfArray(rowT);
                uIdfArray = m_tokenStatistics.GetIdfArray(rowU);
            }
            return GetStringMetric(
                tTokens,
                uTokens,
                tIdfArray,
                uIdfArray);
        }

        public double GetStringMetric(TokenWrapper[][] tTokens, int rowU)
        {
            TokenWrapper[][] uTokens = m_strDataArray.Data[rowU].Columns;
            double[][] tIdfArray = m_tokenStatistics.GetIdfArray(tTokens);
            double[][] uIdfArray = m_tokenStatistics.GetIdfArray(rowU);
            return GetStringMetric(
                tTokens, 
                uTokens, 
                tIdfArray, 
                uIdfArray);
        }

        public double GetStringMetric(
            TokenWrapper[][] tTokens,
            TokenWrapper[][] uTokens,
            double[][] tIdfArray,
            double[][] uIdfArray)
        {
            double dblStringMetric = 0.0;
            for (int intColumn = 0; intColumn < m_intColumnCount; intColumn++)
            {
                //
                // calculate the number of comparisons to perform
                //
                int intWindow;
                if(m_intMaxComparisons > tTokens[intColumn].Length)
                {
                    intWindow = 1;
                }
                else
                {
                    intWindow = m_intMaxComparisons / tTokens[intColumn].Length;
                }

                int intHighLimit = Math.Min(tTokens[intColumn].Length, m_intMaxComparisons);
                for (int i = 0; i < intHighLimit; i++)
                {
                    TokenWrapper actualTToken = tTokens[intColumn][i];
                    double max = 0.0;
                    int intMaxIndex = -1;
                    int intLowLimit2 = Math.Max(0, i - intWindow);
                    int intHighLimit2 = Math.Min(uTokens[intColumn].Length, intWindow);
                    for (int j = intLowLimit2; j < intHighLimit2; j++)
                    {
                        double actualStringMetric;
                        TokenWrapper actualUToken = uTokens[intColumn][j];
                        if (actualTToken.Equals(actualUToken))
                        {
                            actualStringMetric = 1.0;
                            max = actualStringMetric;
                            intMaxIndex = j;
                            break;
                        }
                        if (TagLinkPrefix.GetStringMetric(
                            actualTToken,
                            actualUToken) > 0)
                        {
                            actualStringMetric =
                                m_tagLinkCheapToken.GetStringMetric(
                                    actualTToken,
                                    actualUToken);
                            if (actualStringMetric > m_dblThreshold &&
                                max < actualStringMetric)
                            {
                                max = actualStringMetric;
                                intMaxIndex = j;
                                if (actualStringMetric == 1.0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (intMaxIndex >= 0)
                    {
                        double dblTfidfWeight =
                            Math.Max(tIdfArray[intColumn][i],
                                     uIdfArray[intColumn][intMaxIndex]);
                        dblStringMetric +=
                            max*dblTfidfWeight*dblTfidfWeight;
                    }
                }
            }
            return dblStringMetric;
        }

        #endregion
    }
}
