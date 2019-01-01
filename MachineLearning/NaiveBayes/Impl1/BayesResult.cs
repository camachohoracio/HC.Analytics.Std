#region

using System.Collections.Generic;

#endregion

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    /**
     * An object to hold the result of training a NaiveBayes classifier.
     * Stores the class priors and the counts of features in each class.
     *
     * @author Sugato Basu, Prem Melville and Ray Mooney
     */

    public class BayesResult
    {
        /**
         * Stores the prior probabilities of each class
         */
        protected double[] classPriors;

        /**
         * Stores the counts for each feature: an entry in the Dictionary stores
         * the array of class counts for a feature
         */
        protected Dictionary<string, double[]> featureTable;

        /**
         * Sets the class priors
         */

        public void setClassPriors(double[] priors)
        {
            classPriors = priors;
        }

        /**
         * Returns the class priors
         */

        public double[] getClassPriors()
        {
            return (classPriors);
        }

        /**
         * Sets the feature hash
         */

        public void setFeatureTable(Dictionary<string, double[]> table)
        {
            featureTable = table;
        }

        /**
         * Returns the feature hash
         */

        public Dictionary<string, double[]> getFeatureTable()
        {
            return (featureTable);
        }
    }
}
