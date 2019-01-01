using System;

namespace HC.Analytics.MachineLearning.TextMining
{
    public class StringExtractorResult
    {
        #region Properties

        public double Similarity { get; set; }
        public string T { get; set; }
        public string U { get; set; }
        public int Position { get; set; }

        #endregion

        #region Public

        public override string ToString()
        {
            return "Similarity [" + Math.Round(Similarity, 3) +
                   "], T [" + T +
                   "], U [" + U +
                   "], Position [" + Position + "]";
        }

        #endregion
    }
}

