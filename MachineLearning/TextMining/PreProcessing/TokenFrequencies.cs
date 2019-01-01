#region

using System.Collections.Generic;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class TokenFrequencies
    {
        #region Properties

        public Dictionary<TokenWrapper, int> FreqMap { get; set; }

        #endregion

        #region Public

        public TokenFrequencies(DataWrapper dataWrapper)
        {
            FreqMap = new Dictionary<TokenWrapper, int>(
                new TokenWrapperComparer());

            foreach(RowWrapper row in dataWrapper.Data)
            {
                foreach (TokenWrapper[] col in row.Columns)
                {
                    foreach (TokenWrapper token in col)
                    {
                        int intFreq;
                        if (!FreqMap.TryGetValue(token, out intFreq))
                        {
                            FreqMap[token] = 1;
                        }
                        else
                        {
                            FreqMap[token] = intFreq + 1;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
