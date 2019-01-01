namespace HC.Analytics.MachineLearning
{
    public interface ISimilarityMetric
    {
        double GetSimilarity(object object1, object object2);
    }
}
