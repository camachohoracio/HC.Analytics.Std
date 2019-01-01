#region

using System;
using System.Collections.Generic;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class IdfStats
    {
        #region Properties

        public Dictionary<TokenWrapper, double> IdfMap { get; set; }

        #endregion

        #region Constructors

        public IdfStats(
            DataWrapper dataWrapper,
            TokenFrequencies tokenFreq)
        { 
            IdfMap = new Dictionary<TokenWrapper,double>(
                new TokenWrapper());
            double dblN = dataWrapper.Data.Length;
            foreach (KeyValuePair<TokenWrapper, int> kvp in tokenFreq.FreqMap)
            {
                double dblIdfWeight = Math.Log((dblN / kvp.Value) + 1.0);
                IdfMap[kvp.Key] = dblIdfWeight;
            }
        }

        #endregion
    }
}

