#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class TfIdfStats
    {
        #region Properties

        public Dictionary<TokenWrapper, double>[] TfIdfWeights { get; set; }

        #endregion

        public TfIdfStats(
            IdfStats idfStats,
            DataWrapper dataWrapper)
        {
            TfIdfWeights = new Dictionary<TokenWrapper, double>[
                dataWrapper.Data.Length];

            for (int i = 0; i < dataWrapper.Data.Length; i++)
            {
                TfIdfWeights[i] = new Dictionary<TokenWrapper, double>(
                    new TokenWrapperComparer());
                TokenWrapper[][] row = dataWrapper.Data[i].Columns;
                List<TokenWrapper> tokens = new List<TokenWrapper>();
                foreach (TokenWrapper[] col in row)
                {
                    tokens.AddRange(col);
                }
                TokenFrequencies tokenFrequencies =
                    new TokenFrequencies(new DataWrapper(tokens.ToArray()));
                foreach(KeyValuePair<TokenWrapper, int> kvp in 
                    tokenFrequencies.FreqMap)
                {
                    double dblTfIdfWeight = idfStats.IdfMap[kvp.Key] * 
                        kvp.Value;
                    TfIdfWeights[i].Add(kvp.Key,dblTfIdfWeight);
                }
            }
        }

        private List<TokenRankItem> GetRankingList(
            double dblPercentile)
        {
            var q = (from n in TfIdfWeights
                     select n.Values).ToArray();
            double dblTotalWeight = (from n in q select n.Sum()).Sum();
            List<TokenRankItem> normTfIdf =
                new List<TokenRankItem>();
            int intId = 0;
            foreach (Dictionary<TokenWrapper, double> dict in TfIdfWeights)
            {
                foreach (KeyValuePair<TokenWrapper, double> kvp in dict)
                {
                    normTfIdf.Add(
                        new TokenRankItem
                        {
                            DocId = intId,
                            Token = kvp.Key,
                            Weight = kvp.Value / dblTotalWeight,
                        });
                }
                intId++;
            }

            //
            // sort items
            //
            normTfIdf = normTfIdf.OrderBy(
                tokenItem => tokenItem.Weight).ToList();
            double dblCurrPercentile;
            dblTotalWeight = 0;
            List<TokenRankItem> selectedList =
                new List<TokenRankItem>();
            foreach (TokenRankItem tokenItem in normTfIdf)
            {
                dblTotalWeight +=
                    tokenItem.Weight;
                if (dblTotalWeight > dblPercentile)
                {
                    break;
                }
                selectedList.Add(tokenItem);
            }
            return selectedList;
        }

        public void RemoveRankedWeights(
            DataWrapper dataWrapper,
            double dblRankQuantile)
        {
            List<TokenRankItem> tokenRank = GetRankingList(dblRankQuantile);
            foreach (TokenRankItem tokenRankItem in tokenRank)
            {
                TokenWrapper[][] row = dataWrapper.Data[tokenRankItem.DocId].Columns;
                for (int i = 0; i < row.Length; i++)
                {
                    TokenWrapper[] col = row[i];
                    List<TokenWrapper> selectedTokens =
                        new List<TokenWrapper>();
                    bool blnFound = false;
                    foreach (TokenWrapper token in col)
                    {
                        if (!tokenRankItem.Token.Equals(token))
                        {
                            selectedTokens.Add(token);
                        }
                        else
                        {
                            blnFound = true;
                        }
                    }
                    if (!blnFound)
                    {
                        throw new Exception("Token not found");
                    }
                    row[i] = selectedTokens.ToArray();
                }
            }
        }

    }
}

