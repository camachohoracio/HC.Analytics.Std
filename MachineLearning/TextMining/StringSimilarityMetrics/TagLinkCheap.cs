#region

using System;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLinkCheap : IStringMetric, IDisposable
    {
        #region Members

        private readonly TagLinkCheapToken m_tagLinkCheapToken;
        private TokenStatistics m_tokenStatistics;
        private readonly double m_dblThreshold;
        private readonly int m_intColumnCount;
        private readonly int m_intLimit;
        private readonly int m_intW;
        private DataWrapper m_strDataArray;

        #endregion

        #region Constructors

        public TagLinkCheap(
            int limit)
            : this(null,
                limit,
                null) { }

        public TagLinkCheap(
            DataWrapper rowObjectArray,
            int limit,
            TokenStatistics tokenStatistics)
            : this(rowObjectArray,
                limit,
                TextMiningConstants.CHEAP_LINK_WINDOW,
                tokenStatistics) { }

        public TagLinkCheap(
            DataWrapper rowObjectArray,
            int limit,
            TokenStatistics tokenStatistics,
            double dblTokenThreshold) : 
                this(rowObjectArray,
                    limit,
                    TextMiningConstants.CHEAP_LINK_WINDOW,
                    tokenStatistics,
                    dblTokenThreshold)
        {

        }

        public TagLinkCheap(
            DataWrapper rowObjectArray,
            int limit,
            int w,
            TokenStatistics tokenStatistics) :
            this(rowObjectArray,
                    limit,
                    TextMiningConstants.CHEAP_LINK_WINDOW,
                    tokenStatistics,
                    TextMiningConstants.DBL_CHEAP_LINK_THRESHOLD)
        {
        }

        public TagLinkCheap(
            DataWrapper rowObjectArray,
            int limit,
            int w,
            TokenStatistics tokenStatistics,
            double dblTokenThreshold)
        {
            m_dblThreshold = dblTokenThreshold;
            m_tokenStatistics = tokenStatistics;
            m_intW = w;
            m_strDataArray = rowObjectArray;
            m_intColumnCount = rowObjectArray.Data[0].Columns.Length;
            m_tagLinkCheapToken = new TagLinkCheapToken();
            m_intLimit = limit;
        }

        #endregion

        #region Public

        public double GetStringMetric(int rowT, int rowU)
        {
            return GetStringMetric(rowT, rowU, 0, m_intColumnCount);
        }

        public double GetStringMetric(
            int rowT, 
            int rowU, 
            int columnIndex, 
            int totalColumns)
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
                columnIndex,
                totalColumns,
                tTokens,
                uTokens,
                tIdfArray,
                uIdfArray);
        }

        public double GetStringMetric(
            TokenWrapper[][] tTokens,
            TokenWrapper[][] uTokens)
        {
            double[][] tIdfArray = m_tokenStatistics.GetIdfArray(tTokens);
            double[][] uIdfArray = m_tokenStatistics.GetIdfArray(uTokens);
            return GetStringMetric(0,
                m_intColumnCount,
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
            return GetStringMetric(0, 
                m_intColumnCount, 
                tTokens, 
                uTokens, 
                tIdfArray, 
                uIdfArray);
        }

        public double GetStringMetric(
            int columnIndex,
            int totalColumns,
            TokenWrapper[][] tTokens,
            TokenWrapper[][] uTokens,
            double[][] tIdfArray,
            double[][] uIdfArray)
        {
            double stringMetric = 0.0;
			double dblNormalizer = 0.0;
            for (int column = columnIndex; column < totalColumns; column++)
            {
                int highLimit = Math.Min(tTokens[column].Length, m_intLimit);
                for (int i = 0; i < highLimit; i++)
                {
                    TokenWrapper actualTToken = tTokens[column][i];
                    double max = 0.0;
                    int maxIndex = -1;
                    int lowLimit2 = Math.Max(0, i - m_intW);
                    int highLimit2 = Math.Min(uTokens[column].Length, m_intLimit);
                    for (int j = lowLimit2; j < highLimit2; j++)
                    {
                        double actualStringMetric;
                        TokenWrapper actualUToken = uTokens[column][j];
                        if (actualTToken.Equals(actualUToken))
                        {
                            actualStringMetric = 1.0;
                            max = actualStringMetric;
                            maxIndex = j;
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
                                maxIndex = j;
                                if (actualStringMetric == 1.0)
                                {
                                    break;
                                }
                            }
                        }
                    }
	                dblNormalizer += tIdfArray[column][i] * tIdfArray[column][i];
					
                    if (maxIndex >= 0)
                    {
                        double tfidfWeight =
                            Math.Max(tIdfArray[column][i],
                                     uIdfArray[column][maxIndex]);
                        stringMetric +=
                            max*tfidfWeight*tfidfWeight;
                    }
                }
            }
	        stringMetric = Math.Min(1.0, stringMetric / dblNormalizer); 
			
            return stringMetric;
        }

        #endregion

        public void Dispose()
        {
            m_tagLinkCheapToken.Dispose();
            m_tokenStatistics = null;
            m_strDataArray = null;
        }
    }
}
