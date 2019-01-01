#region

using System;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLinkCheapToken : IDisposable
    {
        #region Members

        private readonly double m_dblAlfa;
        private readonly int m_intWindowSize;

        #endregion

        #region Constructors

        public TagLinkCheapToken()
        {
            m_intWindowSize = TextMiningConstants.CHEAP_LINK_TOKEN_WINDOW;
            m_dblAlfa = TextMiningConstants.DBL_CHEAP_LINK_TOKEN_ALPHA;
        }

        #endregion

        #region Public

        public double GetStringMetric(TokenWrapper t, TokenWrapper u)
        {
            if (t.Token.Length == 0 || u.Token.Length == 0)
            {
                return 0.0;
            }
            // get Min string length
            int uLength = u.Token.Length;
            int minLength = Math.Min(t.Token.Length, uLength) - 1;
            int sampleSize = (int) ((minLength)*m_dblAlfa);
            int partition = (int)((minLength) / ((double)(sampleSize - 1)));
            int actualPosition = 0;
            double matched = 0;
            if (sampleSize == 0)
            {
                sampleSize++;
            }
            if (partition == 0)
            {
                partition++;
            }
            for (int i = 0; i < sampleSize; i++)
            {
                char tChar = t[actualPosition];
                for (int j = Math.Max(actualPosition - m_intWindowSize, 0);
                     j < Math.Min(actualPosition + m_intWindowSize + 1, uLength);
                     j++)
                {
                    char uChar = u[j];
                    if (tChar == uChar)
                    {
                        matched++;
                        break;
                    }
                }
                actualPosition += partition;
            }
            return (matched)/sampleSize;
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
