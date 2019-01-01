#region

using System;
using System.Collections.Generic;
using System.IO;
using HC.Core.Logging;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class TokenStatistics : IDisposable
    {
        #region Constants

        private const string FREQ_WORDS_FILE_NAME = @"C:\HC\Data\TextMining\CommonWordsFreq.txt";

        #endregion

        #region Properties

        public Dictionary<TokenWrapper, double> IdfMap { get; private set; }
        public int Columns { get; private set; }
        public double[] ColumnWeights { get; private set; }
        public double[][][] IdfWeights { get; private set; }
        public Dictionary<TokenWrapper, int>[] TokenFrequencies { get; private set; }

        #endregion

        #region Members

        private int m_intN;
        private DataWrapper m_strDataArray;
        private static Dictionary<string, double> m_freqWordMap;

        #endregion

        #region Constructors

        public TokenStatistics(
            string str) : this(
                new DataWrapper(
                    new List<string> { str })) { }

        public TokenStatistics(
            ADocument document) : this(document.Data) { }

        public TokenStatistics(
            DataWrapper strDataArray)
        {
            LoadStats(strDataArray);
        }

        #endregion

        #region Public

        public static Dictionary<string, double> LoadFrequencyWords()
        {
            if(m_freqWordMap== null)
            {
                m_freqWordMap = new Dictionary<string, double>();
                using (StreamReader sr = new StreamReader(FREQ_WORDS_FILE_NAME))
                {
                    string strLine;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        string[] tokens = strLine.Split(',');
                        m_freqWordMap[tokens[0]] = double.Parse(tokens[1]);
                    }
                }
            }
            return m_freqWordMap;
        }

        #endregion


        private void LoadStats(DataWrapper strDataArray)
        {
            if (strDataArray == null || strDataArray.Data.Length == 0)
            {
                return;
            }
            m_strDataArray = strDataArray;
            m_intN = strDataArray.Length;
            Columns = strDataArray.Data[0].Columns.Length;
            GetWordStatistics();
            GetIdfStats();
            GetTokenFrequencies();
        }

        /// <summary>
        /// Returns a hashmap version of the term-vector (bag of words) for this
        /// document, where each token is a key whose value is the number of times
        /// it occurs in the document as stored in a Weight.
        /// </summary>
        /// <returns></returns>
        private void GetTokenFrequencies()
        {
            int intCols = m_strDataArray.Data[0].Columns.Length;
            TokenFrequencies = new Dictionary<TokenWrapper, int>[intCols];
            for (int intColId = 0; intColId < intCols; intColId++)
            {
                TokenFrequencies[intColId] = new Dictionary<TokenWrapper, int>(
                    new TokenWrapperComparer());
            }
            for (int intRows = 0; intRows < m_strDataArray.Length; intRows++)
            {
                for (int intColId = 0; intColId < intCols; intColId++)
                {
                    if (m_strDataArray.Data[intRows].Columns[intColId] != null)
                    {
                        for (int intTokenId = 0;
                             intTokenId < m_strDataArray.Data[intRows].Columns[intColId].Length;
                             intTokenId++)
                        {
                            int intValue;
                            TokenWrapper tokenWrapper =
                                m_strDataArray.Data[intRows].Columns[intColId][intTokenId];
                            if (!TokenFrequencies[intColId].TryGetValue(
                                tokenWrapper,
                                out intValue))
                            {
                                TokenFrequencies[intColId][tokenWrapper] = 1;
                            }
                            else
                            {
                                TokenFrequencies[intColId][tokenWrapper] = intValue + 1;
                            }
                        }
                    }
                }
            }
        }

        private void GetWordStatistics()
        {
            var tokenCountArray = new int[Columns];
            var differentTokens = new int[Columns];
            var tokensColumnWeight = new double[Columns];
            var mapArray = new HashSet<TokenWrapper>[Columns];
            for (int column = 0; column < Columns; column++)
            {
                mapArray[column] = new HashSet<TokenWrapper>(new TokenWrapperComparer());
            }

            //
            // get idf weights ignoring columns
            //
            double dblN = m_strDataArray.Data.GetLength(0);
            var idfMap1 = new Dictionary<TokenWrapper, int>(
                new TokenWrapperComparer());
            for (int row = 0; row < dblN; row++)
            {
                TokenWrapper[][] currentRowArray = m_strDataArray.Data[row].Columns;
                var rowMap = new HashSet<TokenWrapper>(
                    new TokenWrapperComparer());
                for (int column = 0; column < Columns; column++)
                {
                    int tokenCount = currentRowArray[column].Length;
                    tokenCountArray[column] += tokenCount;
                    for (int token = 0; token < tokenCount; token++)
                    {
                        TokenWrapper actualToken = currentRowArray[column][token];
                        //if (!string.IsNullOrEmpty(actualToken))
                        {
                            if (!rowMap.Contains(actualToken))
                            {
                                rowMap.Add(actualToken);
                            }
                            if (!mapArray[column].Contains(actualToken))
                            {
                                mapArray[column].Add(actualToken);
                                differentTokens[column]++;
                            }
                        }
                    }
                }
                foreach (TokenWrapper actualToken2 in rowMap)
                {
                    if (!idfMap1.ContainsKey(actualToken2))
                    {
                        idfMap1.Add(actualToken2, 1);
                    }
                    else
                    {
                        int freq0 = idfMap1[actualToken2];
                        idfMap1[actualToken2] = freq0 + 1;
                    }
                }
            }

            IdfMap = new Dictionary<TokenWrapper, double>(new TokenWrapperComparer());
            foreach (KeyValuePair<TokenWrapper, int> de in idfMap1)
            {
                TokenWrapper actualToken2 = de.Key;
                int freq = de.Value;
                double idfWeight = Math.Log(((dblN)/(freq)) + 1.0);
                IdfMap.Add(actualToken2, idfWeight);
            }
            for (int column = 0; column < Columns; column++)
            {
                double numerator = Math.Log(tokenCountArray[column]);
                tokensColumnWeight[column] = (differentTokens[column])/numerator;
            }
            ColumnWeights = tokensColumnWeight;
            // normalize weights
            double sum = 0.0;
            for (int i = 0; i < Columns; i++)
            {
                sum += ColumnWeights[i];
            }
            for (int i = 0; i < Columns; i++)
            {
                ColumnWeights[i] = ColumnWeights[i]/sum;
            }
        }

        public double[] GetColumnWeights()
        {
            return ColumnWeights;
        }

        public double[][] GetIdfArray(int intRow)
        {
            return IdfWeights[intRow];
        }

        private void GetIdfStats()
        {
            IdfWeights = new double[m_intN][][];

            for (int row = 0; row < m_intN; row++)
            {
                TokenWrapper[][] currentRowArray = m_strDataArray.Data[row].Columns;
                IdfWeights[row] = GetIdfArray(currentRowArray);
            }
        }

        public double[][] GetIdfArray(TokenWrapper[][] strRowArray)
        {
            double[][] idfArray = new double[Columns][];
            double dblSq = 0.0;
            for (int columnIndex = 0; columnIndex < Columns; columnIndex++)
            {
                int currentTokenArrayLength = strRowArray[columnIndex].Length;
                idfArray[columnIndex] = new double[currentTokenArrayLength];
                for (int j = 0; j < currentTokenArrayLength; j++)
                {
                    double idfWeight;
                    TokenWrapper token = strRowArray[columnIndex][j];
                    if (string.IsNullOrEmpty(token.Token))
                    {
                        idfWeight = 0.0;
                    }
                    else if (IdfMap.ContainsKey(token))
                    {
                        idfWeight = (IdfMap[strRowArray[columnIndex][j]])
                                    *ColumnWeights[columnIndex];
                    }
                    else
                    {
                        idfWeight = Math.Log(((m_intN)/1.0) + 1.0);
                    }
                    idfArray[columnIndex][j] = idfWeight;
                    dblSq += idfWeight*idfWeight;
                }
            }
            dblSq = Math.Sqrt(dblSq);
            for (int columnIndex = 0; columnIndex < Columns; columnIndex++)
            {
                int currentTokenArrayLength = strRowArray[columnIndex].Length;
                for (int j = 0; j < currentTokenArrayLength; j++)
                {
                    idfArray[columnIndex][j] = idfArray[columnIndex][j]/dblSq;
                }
            }
            return idfArray;
        }

        public void Dispose()
        {
            try
            {
                if (IdfMap != null)
                {
                    IdfMap.Clear();
                    IdfMap = null;
                }
                ColumnWeights = null;
                IdfWeights = null;
                TokenFrequencies = null;
                if (m_strDataArray != null)
                {
                    m_strDataArray.Dispose();
                    m_strDataArray = null;
                }
                if (m_freqWordMap != null)
                {
                    m_freqWordMap.Clear();
                    m_freqWordMap = null;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
