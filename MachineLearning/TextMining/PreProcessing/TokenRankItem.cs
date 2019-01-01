using HC.Core.Text;

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class TokenRankItem
    {
        public TokenWrapper Token { get; set; }
        public int DocId { get; set; }
        public double Weight { get; set;}
    }
}

