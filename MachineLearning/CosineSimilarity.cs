#region

using System;

#endregion

namespace HC.Analytics.MachineLearning
{
    public class CosineSimilarity : ISimilarityMetric
    {
        #region ISimilarityMetric Members

        public double GetSimilarity(object object1, object object2)
        {
            return GetSimilarity(((double[])object1), ((double[])object2));
        }

        #endregion

        public static double GetSimilarity(double[] dblVector1, double[] dblVector2)
        {
            double dblDenominator = 0;
            double dblNumerator = 0;
            double dblSumSquare1 = 0;
            double dblSumSquare2 = 0;
            for (int i = 0; i < dblVector1.Length; i++)
            {
                dblNumerator += dblVector1[i] *
                                dblVector2[i];
                dblSumSquare1 += Math.Pow(dblVector1[i], 2);
                dblSumSquare2 += Math.Pow(dblVector2[i], 2);
            }
            dblDenominator = Math.Sqrt(dblSumSquare1 * dblSumSquare2);
            return dblNumerator / dblDenominator;
        }
    }
}
