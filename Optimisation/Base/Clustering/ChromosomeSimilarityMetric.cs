#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Clustering
{
    /// <summary>
    ///   Measures the similarity between two chromosomes are returns a value
    ///   between 0 and 1, where 1 means the maximum similarity and 0 otherwise
    /// </summary>
    [Serializable]
    public class ChromosomeSimilarityMetric
    {
        #region Constructors

        #endregion

        #region Public

        /// <summary>
        ///   Get similarity metric from two vectors
        /// </summary>
        /// <param name = "vector1">
        ///   IIndividual vector 1
        /// </param>
        /// <param name = "vector2">
        ///   IIndividual vector 2
        /// </param>
        /// <returns>
        ///   Similarity metric. 1 is the maximum similarity
        ///   0 means no similarity at all.
        /// </returns>
        public double GetStringMetric(
            Individual individual1,
            Individual individual2)
        {
            return GetStringMetric(individual1.GetChromosomeCopy(),
                                   individual2.GetChromosomeCopy());
        }

        /// <summary>
        ///   Compare two chromosomes and return a similarity metric
        ///   between zero and one
        /// </summary>
        /// <param name = "dblChromosomeArray_1">
        ///   Chromosome array 1
        /// </param>
        /// <param name = "dblChromosomeArray_2">
        ///   Chromosome array 2
        /// </param>
        /// <returns>
        ///   Similarity metric
        /// </returns>
        public double GetStringMetric(double[] dblChromosomeArray_1,
                                      double[] dblChromosomeArray_2)
        {
            var tSize = dblChromosomeArray_1.Length;
            double currentTValue;
            double currentUValue;
            var score = 0.0;
            for (var t = 0; t < tSize; t++)
            {
                currentTValue = dblChromosomeArray_1[t];
                currentUValue = dblChromosomeArray_2[t];
                score += 1.0 - Math.Abs(currentTValue - currentUValue);
            }

            score = score/tSize;
            return score;
        }

        #endregion
    }
}
