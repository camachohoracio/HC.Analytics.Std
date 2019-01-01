#region

using System;

#endregion

namespace HC.Analytics.MachineLearning
{
    public class EuclideanSimilarity : ISimilarityMetric
    {
        #region ISimilarityMetric Members

        public double GetSimilarity(object object1, object object2)
        {
            return GetSimilarity(((double[])object1), ((double[])object2));
        }

        #endregion

        public static double GetSimilarity(double[] dblVector1, double[] dblVector2)
        {
            double dblNumerator = 0;
            for (int i = 0; i < dblVector1.Length; i++)
            {
                dblNumerator += Math.Pow(dblVector1[i] -
                                dblVector2[i], 2);
            }

            double dblDistance = Math.Sqrt(dblNumerator) / 
                Math.Sqrt(dblVector1.Length);

            if (double.IsNaN(dblDistance))
            {
                //Debugger.Break();
            }

            return 1.0 - dblDistance;
        }
    }
}
