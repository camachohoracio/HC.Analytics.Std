#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    /**
     * Abstract class specifying the functionality of a classifier. Provides methods for
     * training and testing a classifier
     *
     * @author Sugato Basu and Yuk Wah Wong
     */

    public abstract class Classifier
    {
        /**
         * Used for breaking ties in argMax()
         */
        protected static Random random = new Random();

        /**
         * Array of categories (classes) in the data
         */
        protected string[] categories;


        /**
         * The name of a classifier
         *
         * @return the name of a particular classifier
         */
        public abstract string getName();

        /**
         * Returns the categories (classes) in the data
         *
         * @return an array containing strings describing the categories
         */

        public string[] getCategories()
        {
            return categories;
        }

        /**
         * Trains the classifier on the training examples
         *
         * @param trainingExamples a list of Example objects that will be used
         *                         for training the classifier
         */
        public abstract void train(List<Example> trainingExamples);

        /**
         * Returns true if the predicted category of the test example matches the correct category,
         * false otherwise
         */
        public abstract bool test(Example testExample);

        /**
         * Returns the array index with the maximum value
         *
         * @param results Array of doubles whose index with Max value is to be found.
         *                Ties are broken randomly.
         */

        protected int argMax(double[] results)
        {
            List<int> maxIndices = new List<int>();
            maxIndices.Add(0);
            double max = results[0];

            for (int i = 1; i < results.Length; i++)
            {
                if (results[i] > max)
                {
                    max = results[i];
                    maxIndices.Clear();
                    maxIndices.Add(i);
                }
                else if (results[i] == max)
                {
                    maxIndices.Add(i);
                }
            }
            int returnIndex;
            if (maxIndices.Count > 1)
            {
                // break ties randomly
                int winnerIdx = random.Next(maxIndices.Count);
                returnIndex = maxIndices[winnerIdx];
            }
            else
            {
                returnIndex = maxIndices[0];
            }
            return (returnIndex);
        }
    }
}
