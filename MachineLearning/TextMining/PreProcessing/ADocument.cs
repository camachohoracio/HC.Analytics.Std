#region

using System.Collections.Generic;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    /// <summary>
    /// Docment is an abstract class that provides for tokenization
    /// of a document with stop-word removal and an iterator-like interface
    /// similar to  StringTokenizer.
    /// Also provides a method for converting a document into a
    /// vector-space bag-of-words in the form of a Dictionary of
    /// tokens and their occurrence counts.
    /// 
    /// </summary>
    public abstract class ADocument
    {
        #region Properties

        public List<string> DataList { get; private set; }

        /// <summary>
        /// Data stored as row-column-token
        /// </summary>
        public DataWrapper Data { get; private set; }

        public TokenStatistics TokenStatistics
        {
            get
            {
                if (m_tokenStatistics == null)
                {
                    m_tokenStatistics = new TokenStatistics(this);
                }
                return m_tokenStatistics;
            }
        }

        #endregion

        #region Members

        private static readonly PorterStemer m_stemmer = new PorterStemer();
        private readonly StopWords m_stopWords;
        private TokenStatistics m_tokenStatistics;

        /// <summary>
        /// Whether to stem tokens with the Porter stemmer
        /// </summary>
        private readonly bool m_blnDoStem;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Document making sure that the stopwords
        ///  are loaded, indexed, and ready for use.  Subclasses
        /// that create concrete instances MUST call prepareNextToken
        /// before finishing to ensure that the first token is precomputed
        /// and available.
        /// </summary>
        /// <param name="blnDoStem"></param>
        /// <param name="stopWords"></param>
        /// <param name="document"></param>
        /// <param name="charColumnDelimiter"></param>
        protected ADocument(
            bool blnDoStem,
            StopWords stopWords,
            List<string> document,
            char charColumnDelimiter)
        {
            m_stopWords = stopWords;
            DataList = document;
            m_blnDoStem = blnDoStem;
            LoadTokens(charColumnDelimiter);
        }


        #endregion

        private void LoadTokens(char charColumnDelimiter)
        {
            if (DataList == null || DataList.Count == 0)
            {
                return;
            }

            int intCols = DataList[0].Split(charColumnDelimiter).Length;
            if (Data == null)
            {
                Data = new DataWrapper();
            }

            Data.Data = new RowWrapper[DataList.Count];
            for (int intRow = 0; intRow < DataList.Count; intRow++)
            {
                Data.Data[intRow] = new RowWrapper
                                        {
                                            Columns = new TokenWrapper[intCols][]
                                        };
                string strLine = DataList[intRow];
                string[] cols = strLine.Split(',');
                for (int intCol = 0; intCol < intCols; intCol++)
                {
                    string strCol = cols[intCol];
                    var validTokens = new List<TokenWrapper>();
                    string[] tokens = Tokeniser.Tokenise(strCol, false);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string strToken = tokens[i];
                        if (m_stopWords == null ||
                            !m_stopWords.Contains(strToken))
                        {
                            if (m_blnDoStem)
                            {
                                strToken = m_stemmer.DoStem(strToken);
                            }
                            validTokens.Add(new TokenWrapper(strToken));
                        }
                    }
                    Data.Data[intRow].Columns[intCol] = validTokens.ToArray();
                }
            }
        }
    }
}
