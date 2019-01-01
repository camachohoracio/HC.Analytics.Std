using System.Collections.Generic;

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class SimpleDocument : ADocument
    {
        public SimpleDocument(
            bool blnDoStem,
            StopWords stopWords,
            string strDocument) : this(blnDoStem,
                                        stopWords,
                                        new List<string>(new[] { strDocument }))
        {
        }

        public SimpleDocument(
            bool blnDoStem,
            StopWords stopWords,
            List<string> strDocument) : base(
                blnDoStem,
                stopWords,
                strDocument,
                ','){ }

    }
}