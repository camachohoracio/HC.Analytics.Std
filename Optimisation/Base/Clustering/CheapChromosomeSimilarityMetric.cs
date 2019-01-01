#region

using System;

#endregion

namespace HC.Analytics.Optimisation.Base.Clustering
{
    /// <summary>
    ///   Meassure the simularity between two chromosomes. 
    ///   This methods performas a quick and shallow computation.
    /// </summary>
    [Serializable]
    public class CheapChromosomeSimilarityMetric
    {
        #region Members

        /// <summary>
        ///   Alpha parameter. Number of values to be evaluated by the similarity method
        /// </summary>
        public static double m_dblAlfa = OptimisationConstants.DBL_CHEAP_METRIC_ALPHA;

        /// <summary>
        ///   Window size. Number of chromosomes to check in the vecinity in order to 
        ///   meassure similarity
        /// </summary>
        public static int m_intWindowsSize = OptimisationConstants.INT_CHEAP_METRIC_WINDOW_SIZE;

        #endregion

        #region Constructors

        #endregion

        #region Public

        //public double GetStringMetric(IIndividual vector1, IIndividual vector2)
        //{
        //    return CompareVectors(vector1.GetChromosomeCopy(),
        //                          vector2.GetChromosomeCopy());
        //}

        #endregion

        #region Private

        /// <summary>
        ///   Compare two vectors
        /// </summary>
        /// <param name = "dblVectorArray_1">
        ///   Vector 1
        /// </param>
        /// <param name = "dblVectorArray_2">
        ///   Vector 2
        /// </param>
        /// <returns></returns>
        private static double CompareVectors(
            double[] dblVectorArray_1,
            double[] dblVectorArray_2)
        {
            if (dblVectorArray_1.Length == 0 || dblVectorArray_2.Length == 0)
            {
                return 0.0;
            }
            // get Min string length
            var uLength = dblVectorArray_2.Length;
            int minLength = Math.Min(dblVectorArray_1.Length, uLength) - 1,
                sampleSize = (int) (minLength*m_dblAlfa),
                partition = (int) (minLength/((double) (sampleSize - 1))),
                actualPosition = 0;
            double matched = 0;
            if (sampleSize == 0)
            {
                sampleSize++;
            }
            if (partition == 0)
            {
                partition++;
            }
            for (var i = 0; i < sampleSize; i++)
            {
                for (var j = Math.Max(actualPosition - m_intWindowsSize, 0);
                     j < Math.Min(actualPosition + m_intWindowsSize + 1, uLength);
                     j++)
                {
                    matched = 1.0 - Math.Abs(dblVectorArray_1[actualPosition] - dblVectorArray_2[j]);
                    if (matched >= 0.7)
                    {
                        break;
                    }
                }
                actualPosition += partition;
            }
            return matched/sampleSize;
        }

        #endregion
    }
}
