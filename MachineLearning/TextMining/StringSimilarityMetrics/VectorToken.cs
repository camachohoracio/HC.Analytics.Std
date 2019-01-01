#region

using System;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class VectorToken
    {
        public double GetStringMetric(string T, string U)
        {
            return GetRawMetric(T, U);
        }

        public static double GetRawMetric(string T, string U)
        {
            if (T == "")
            {
                return 0.0;
            }
            if (T.Equals(U))
            {
                return 1.0;
            }
            int tSize = T.Length;
            double score = 0;
            for (int t = 0; t < tSize; t++)
            {
                double currentTValue = double.Parse(T[t].ToString());
                double currentUValue = double.Parse(U[t].ToString());
                score += 1.0 - Math.Abs(currentTValue - currentUValue);
            }

            score = score/(tSize);
            return score;
        }
    }
}
