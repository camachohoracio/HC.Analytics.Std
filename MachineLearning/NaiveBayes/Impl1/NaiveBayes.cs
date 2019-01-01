#region

using System;
using System.Collections.Generic;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    /**
     * Implements the NaiveBayes Classifier with Laplace smoothing. Stores probabilities
     * internally as logs to prevent underflow problems.
     *
     * @author Sugato Basu, Prem Melville, and Ray Mooney
     */

    public class NaiveBayes : Classifier
    {
        /**
         * Flag to set Laplace smoothing when estimating probabilities
         */

        /**
         * Name of classifier
         */
        public static string name = "NaiveBayes";
        private bool debug = false;
        private double EPSILON = 1e-6;
        private bool isLaplace = true;

        /**
         * Number of categories
         */
        private int numCategories;

        /**
         * Number of features
         */

        /**
         * Number of training examples, set by train function
         */
        private int numExamples;
        private int numFeatures;
        private BayesResult trainResult;

        /**
         * Flag for debug prints
         */

        /**
         * Create a naive Bayes classifier with these attributes
         *
         * @param categories The array of Strings containing the category names
         * @param debug      Flag to turn on detailed output
         */

        public NaiveBayes(string[] categories, bool debug)
        {
            this.categories = categories;
            this.debug = debug;
            numCategories = categories.Length;
        }

        /**
         * Sets the debug flag
         */

        public void setDebug(bool bool_)
        {
            debug = bool_;
        }

        /**
         * Sets the Laplace smoothing flag
         */

        public void setLaplace(bool bool_)
        {
            isLaplace = bool_;
        }

        /**
         * Sets the value of EPSILON (default 1e-6)
         */

        public void setEpsilon(double ep)
        {
            EPSILON = ep;
        }

        /**
         * Returns the name
         */

        public override string getName()
        {
            return name;
        }

        /**
         * Returns value of EPSILON
         */

        public double getEpsilon()
        {
            return EPSILON;
        }

        /**
         * Returns training result
         */

        public BayesResult getTrainResult()
        {
            return trainResult;
        }

        /**
         * Returns value of isLaplace
         */

        public bool getIsLaplace()
        {
            return (isLaplace);
        }

        /**
         * Trains the Naive Bayes classifier - estimates the prior probs and calculates the
         * counts for each feature in different categories
         *
         * @param trainExamples The vector of training examples
         */

        public override void train(List<Example> trainExamples)
        {
            trainResult = new BayesResult();
            numExamples = trainExamples.Count;
            //calculate class priors
            trainResult.setClassPriors(calculatePriors(trainExamples));
            //calculate counts of feature for each class
            trainResult.setFeatureTable(conditionalProbs(trainExamples));
            if (debug)
            {
                displayProbs(trainResult.getClassPriors(), trainResult.getFeatureTable());
            }
        }

        /**
         * Categorizes the test example using the trained Naive Bayes classifier, returning true if
         * the predicted category is same as the actual category
         *
         * @param testExample The test example to be categorized
         */

        public override bool test(Example testExample)
        {
            // calculate posterior probs
            double[] posteriorProbs = calculateProbs(testExample);
            // predicted class
            int predictedClass = argMax(posteriorProbs);
            if (debug)
            {
                Console.Write("Document: " + testExample.name + "\nResults: ");
                for (int j = 0; j < numCategories; j++)
                {
                    Console.Write(categories[j] + "(" + posteriorProbs[j] + ")\t");
                }
                PrintToScreen.WriteLine("\nCorrect class: " + testExample.getCategory() + ", Predicted class: " +
                                  predictedClass + Environment.NewLine);
            }
            return (predictedClass == testExample.getCategory());
        }

        /**
         * Calculates the class priors
         *
         * @param trainExamples The training examples from which class priors will be estimated
         */

        protected double[] calculatePriors(List<Example> trainExamples)
        {
            double[] classCounts = new double[numCategories];

            //init class counts
            for (int i = 0; i < numCategories; i++)
                classCounts[i] = 0;

            //increment the count of the class that each example belongs to
            foreach (Example ex in trainExamples)
            {
                classCounts[ex.getCategory()]++;
            }

            // Get log probs from counts, with Laplace smoothing if specified
            for (int i = 0; i < numCategories; i++)
            {
                if (isLaplace)
                    classCounts[i] = Math.Log((classCounts[i] + 1)/(numExamples + numCategories));
                else
                    classCounts[i] = Math.Log(classCounts[i]/numExamples);
            }
            if (debug)
            {
                PrintToScreen.WriteLine("\nLog Class Priors:");
                for (int i = 0; i < numCategories; i++)
                    Console.Write(classCounts[i] + " ");
                PrintToScreen.WriteLine();
            }
            return classCounts;
        }

        /**
         * Calculates the conditional probs of each feature in the different categories
         *
         * @param trainExamples The training examples from which counts will be estimated
         */

        protected Dictionary<string, double[]> conditionalProbs(List<Example> trainExamples)
        {
            // Initialize Dictionary giving conditional prob of each class given a feature
            Dictionary<string, double[]> featureHash = new Dictionary<string, double[]>();
            double[] totalCounts = new double[numCategories]; // stores total count of all features in each category

            for (int i = 0; i < numCategories; i++)
                totalCounts[i] = 0;

            foreach (Example currentExample in trainExamples)
            {
                if (debug)
                {
                    PrintToScreen.WriteLine("\nExample: " + currentExample);
                    PrintToScreen.WriteLine("Number of tokens: " + currentExample.getHashMapVector().Map.Count);
                }
                foreach (KeyValuePair<string, Weight> entry in currentExample.getHashMapVector().Map)
                {
                    // An entry in the Dictionary maps a token to a Weight
                    string token = entry.Key;
                    // The count for the token is in the value of the Weight value
                    int count = (int) entry.Value.GetValue();
                    double[] countArray; // stores counts for current feature
                    if (debug)
                        PrintToScreen.WriteLine("Counts of token: " + token);
                    if (!featureHash.ContainsKey(token))
                    {
                        countArray = new double[numCategories]; //create a new array
                        for (int m = 0; m < numCategories; m++)
                            countArray[m] = 0.0; //init to 0
                        featureHash.Add(token, countArray); //Add to Dictionary
                    }
                    else
                    {
                        // retrieve existing array from Dictionary
                        countArray = featureHash[token];
                    }
                    countArray[currentExample.getCategory()] += count;
                    totalCounts[currentExample.getCategory()] += count;
                    if (debug)
                    {
                        for (int k = 0; k < countArray.Length; k++)
                            Console.Write(countArray[k] + " ");
                        PrintToScreen.WriteLine();
                    }
                }
            }
            numFeatures = featureHash.Count;
            //We can now compute the log probabilities
            if (debug)
            {
                PrintToScreen.WriteLine("\nLog Probs before multiplying priors...\n");
            }
            foreach (KeyValuePair<string, double[]> entry in featureHash)
            {
                string token = entry.Key;
                double[] countArray = entry.Value;
                for (int j = 0; j < numCategories; j++)
                {
                    if (isLaplace) //Laplace smoothing
                        countArray[j] = (countArray[j] + 1)/(totalCounts[j] + numFeatures);
                    else
                    {
                        if (countArray[j] == 0)
                            countArray[j] = EPSILON; // to avoid 0 counts when no Laplace smoothing
                        else
                            countArray[j] = countArray[j]/totalCounts[j];
                    }
                    countArray[j] = Math.Log(countArray[j]); //take log of probability
                }
                if (debug)
                {
                    PrintToScreen.WriteLine("Log probs of " + token);
                    for (int k = 0; k < countArray.Length; k++)
                        Console.Write(countArray[k] + " ");
                    PrintToScreen.WriteLine();
                }
            }
            return (featureHash);
        }

        /**
         * Calculates the prob of the testExample being generated by each category
         *
         * @param testExample The test example to be categorized
         */

        protected double[] calculateProbs(Example testExample)
        {
            //set initial probabilities to the prior probs
            double[] probs = (double[]) trainResult.getClassPriors().Clone();
            Dictionary<string, double[]> Dictionary = trainResult.getFeatureTable();
            foreach (KeyValuePair<string, Weight> entry in testExample.getHashMapVector().Map)
            {
                // An entry in the Dictionary maps a token to a Weight
                string token = entry.Key;
                // The count for the token is in the value of the Weight
                int count = (int) entry.Value.GetValue();
                if (Dictionary.ContainsKey(token))
                {
//ignore unknowns
                    double[] countArray = Dictionary[token]; // stores the category array for one token
                    for (int k = 0; k < numCategories; k++)
                        probs[k] += count*countArray[k]; //multiplying the probs == adding the logs
                }
            }
            return probs;
        }

        /**
         * Displays the probs for each feature in the different categories
         *
         * @param classPriors Prior probs
         * @param featureHash Feature Dictionary after training
         */

        protected void displayProbs(double[] classPriors, Dictionary<string, double[]> featureHash)
        {
            PrintToScreen.WriteLine("\nAfter multiplying priors...");
            foreach (KeyValuePair<string, double[]> entry in featureHash)
            {
                string token = entry.Key;
                double[] probs = entry.Value;
                Console.Write("\nFeature: " + token + ", Probs: ");
                for (int num = 0; num < probs.Length; num++)
                {
                    //double posterior = classPriors[num]+probs[num];
                    double posterior = Math.Pow(Math.E, classPriors[num] + probs[num]);
                    Console.Write(" " + posterior);
                }
            }
            PrintToScreen.WriteLine();
        }
    }
}
