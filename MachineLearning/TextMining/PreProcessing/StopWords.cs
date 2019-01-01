#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using HC.Core.Helpers;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class StopWords
    {
        #region Constants

        private const string STOP_WORDS_FILE_NAME = @"C:\HC\Config\TextMining\StopWords.txt";

        #endregion

        #region Members

        public HashSet<string> StopWordsSet { get; private set; }
        public static StopWords OwnInstance { get; private set; }

        #endregion

        #region Constructors

        static StopWords()
        {
            OwnInstance = new StopWords();
        }

        public StopWords() : this(STOP_WORDS_FILE_NAME){}

        public StopWords(string strStopWordsFile)
        {
            LoadStopWords(strStopWordsFile);
        }

        #endregion

        #region Public

        public bool Contains(string strToken)
        {
            return StopWordsSet.Contains(strToken);
        }

        public static List<string> CleanStopWords(
            List<string> list,
            double dblPercentile)
        {
            if(list == null || list.Count == 0)
            {
                return new List<string>();
            }

            var document = new SimpleDocument(false, null, list);

            CleanStopWords(
                document,
                dblPercentile,
                new int[1]);
            return (from n in document.Data.Data
                    select n.ToString()).ToList();
        }

        public static void CleanStopWords(
            ADocument document,
            double dblPercentile,
            int[] columns)
        {
            foreach (int intColId in columns)
            {
                //
                // get token frequencies
                //
                var tokenFreq =
                    new Dictionary<TokenWrapper, double>(
                        new TokenWrapperComparer());
                foreach (KeyValuePair<TokenWrapper, int> keyValuePair in
                    document.TokenStatistics.TokenFrequencies[intColId])
                {
                    double dblTotalValue;
                    if(!tokenFreq.ContainsKey(keyValuePair.Key))
                    {
                        dblTotalValue = keyValuePair.Value;
                    }
                    else
                    {
                        dblTotalValue = tokenFreq[keyValuePair.Key] + keyValuePair.Value;
                    }
                    tokenFreq[keyValuePair.Key] = dblTotalValue;
                }

                //
                // get top percentile words
                //
                List<KeyValuePair<TokenWrapper, double>> valuePairs = 
                    (from n in tokenFreq select n).ToList();
                valuePairs = valuePairs.OrderBy(x => -x.Value).ToList();
                double dblSumValues = (from n in valuePairs select n.Value).Sum();
                double dblAcumValue = 0;
                var freqWords = new HashSet<TokenWrapper>(new TokenWrapperComparer());
                foreach (var keyValuePair in valuePairs)
                {
                    dblAcumValue += keyValuePair.Value / dblSumValues;
                    if (dblAcumValue <= dblPercentile)
                    {
                        freqWords.Add(keyValuePair.Key);
                        PrintToScreen.WriteLine("Removing common word " + 
                            keyValuePair.Key + " = " +
                            keyValuePair.Value);
                    }
                    else
                    {
                        break;
                    }
                }

                //
                // remote frequent words
                //
                for (int i = 0; i < document.Data.Length; i++)
                {
                    var selectedTokens = new List<TokenWrapper>();
                    foreach (TokenWrapper strToken in document.Data.Data[i].Columns[intColId])
                    {
                        if(!freqWords.Contains(strToken))
                        {
                            selectedTokens.Add(strToken);
                        }
                    }
                    document.Data.Data[i].Columns[intColId] = selectedTokens.ToArray();
                }
            }
        }

        #endregion

        #region Private & protected

        /// <summary>
        /// Load the stopwords from file to the hashtable where they are indexed.
        /// </summary>
        /// <param name="strStopWordsFile"></param>
        private void LoadStopWords(string strStopWordsFile)
        {
            // Initialize hashtable to proper size given known number of
            // stopwords in the file and a default 75% load factor with
            // 10 extra slots for spare room.
            StopWordsSet = new HashSet<string>();
            try
            {
                // Open stopword file for reading
                var sr = new StreamReader(strStopWordsFile);
                // Read in stopwords, one per line, until file is empty
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // Index word into the hashtable with
                    // the default empty string as a "dummy" value.
                    StopWordsSet.Add(line);
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                PrintToScreen.WriteLine("\nCould not load stopwords file: " + strStopWordsFile + ". " +
                    ex.StackTrace);
            }
        }

        #endregion
    }
}

