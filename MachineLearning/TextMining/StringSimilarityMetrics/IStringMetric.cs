namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public interface IStringMetric
    {
        // default constructor
        double GetStringMetric(int x, int y);
    }
}
