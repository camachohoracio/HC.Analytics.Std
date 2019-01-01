using System;
using System.Collections.Generic;
using HC.Core.Helpers;

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    //package ir.classifiers;

    //import java.io.*;
    //import java.util.*;

    //import ir.utilities.*;

    /**
     * Gives learning curves with K-fold cross validation for a classifier.
     *
     * @author Sugato Basu and Ray Mooney
     */
    public class CVLearningCurve
    {
        /**
         * Stores all the examples for each class
         */
        protected List<Example>[] totalExamples;

        /**
         * foldBins[i][j] stores the examples for class i in fold j. This stores the training-test splits for all the folds
         */
        protected List<Example>[,] foldBins;

        /**
         * The classifier for which K-fold CV learning curve has to be generated
         */
        protected Classifier classifier;

        /**
         * Seed for random number generator
         */
        protected long randomSeed;

        /**
         * Number of classes in the data
         */
        protected int numClasses;

        /**
         * Total number of training examples per fold
         */
        protected int totalNumTrain;

        /**
         * Number of folds of cross validation to run
         */
        protected int numFolds;

        /**
         * Points on the X axis (percentage of train data) to plot
         */
        protected double[] points;

        /**
         * Default points
         */
        protected static double[] DEFAULT_POINTS = { 0.0, 0.01, 0.05, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };

        /**
         * Flag for debug display
         */
        protected bool debug = false;

        /**
         * Total Training time
         */
        protected double trainTime;

        /**
         * Total Testing time
         */
        protected double testTime;

        /**
         * Total number of examples tested in test time
         */
        protected int testTimeNum;

        /**
         * Accuracy results for test data, one PointResults for each point on the curve
         */
        protected PointResults[] testResults;

        /**
         * Accuracy results for training data, one PointResults for each point on the curve
         */
        protected PointResults[] trainResults;

        /**
         * Creates a CVLearning curve object
         *
         * @param nfolds   Number of folds of CV to perform
         * @param c        Classifier on which to perform K-fold CV
         * @param examples List of examples.
         * @param points   Points (in percentage of full train set) to plot on learning curve
         * @param debug    Debugging flag to set verbose trace printing
         */
        public CVLearningCurve(int nfolds, Classifier c, List<Example> examples, double[] points,
                               long randomSeed, bool debug)
        {
            if (nfolds < 2)
            {
                throw new ArgumentException("Cannot have less than 2 folds");
            }
            numFolds = nfolds;
            classifier = c;
            numClasses = c.getCategories().Length;
            totalExamples = new List<Example>[numClasses];
            foldBins = new List<Example>[numClasses, numFolds];
            setTotalExamples(examples);
            this.points = points;
            // Initialize results for each point to be plotted on the curve
            testResults = new PointResults[points.Length];
            trainResults = new PointResults[points.Length];
            this.randomSeed = randomSeed;
            this.debug = debug;
            trainTime = testTime = 0;
        }

        /**
         * Creates a CVLearning curve object with 10 folds and default points
         *
         * @param c        Classifier on which to perform K-fold CV
         * @param examples List of examples.
         */
        public CVLearningCurve(Classifier c, List<Example> examples) :
            this(10, c, examples, DEFAULT_POINTS, 1, false)
        {

        }

        /**
         * Return classifier
         */
        public Classifier getClassifier()
        {
            return classifier;
        }

        /**
         * Set the classifier
         */
        public void setClassifier(Classifier c)
        {
            classifier = c;
        }

        /**
         * Return all the examples
         */
        public List<Example>[] getTotalExamples()
        {
            return totalExamples;
        }

        /**
         * Set all the examples
         */
        public void setTotalExamples(List<Example>[] data)
        {
            totalExamples = data;
        }

        /**
         * Return the fold Bins
         */
        public List<Example>[,] getFoldBins()
        {
            return foldBins;
        }

        /**
         * Set the fold Bins
         */
        public void setFoldBins(List<Example>[,] bins)
        {
            foldBins = bins;
        }

        /**
         * Sets the totalExamples by partitioning examples into categories to
         * get a stratified sample
         */
        public void setTotalExamples(List<Example> examples)
        {
            totalNumTrain = (int)Math.Round((1.0 - 1.0 / numFolds) * examples.Count);
            foreach (Example example in examples)
            {
                int category = example.getCategory();
                if (totalExamples[category] == null)
                    totalExamples[category] = new List<Example>();
                totalExamples[category].Add(example);
            }
        }

        /**
         * GetTask a CV learning curve test and print total training and test time
         * and generate an averge learning curve plot output files suitable
         * for gunuplot
         */
        public void run()
        {
            PrintToScreen.WriteLine("Generating 10 fold CV learning curves...");
            trainAndTest();
            PrintToScreen.WriteLine();
            PrintToScreen.WriteLine("Total Training time in seconds: " + trainTime / 1000.0);
            PrintToScreen.WriteLine("Testing time per example in milliseconds: " +
                Math.Round(testTime / testTimeNum, 2));
            // Create Gnuplot of learning curve
            //makeGnuplotFile(trainResults, classifier.getName() + "Train");
            PrintToScreen.WriteLine("GNUPLOT train accuracy file is " + classifier.getName() + "Train.gplot");
            //makeGnuplotFile(testResults, classifier.getName());
            PrintToScreen.WriteLine("GNUPLOT test accuracy file is " + classifier.getName() + ".gplot");
        }

        /**
         * GetTask training and test for each point to be plotted, gathering a result for
         * each fold.
         */
        public void trainAndTest()
        {
            // randomly mix the training examples in each category
            randomizeOrder();
            // create foldBins from totalExamples -- effectively creates the
            // training-test splits for each fold
            binExamples();
            // Gather results for each point (number of examples) to be plotted 
            // on the learning curve
            for (int i = 0; i < points.Length; i++)
            {
                double percent = points[i];
                PrintToScreen.WriteLine("Train Percentage: " + 100 * percent + "%");
                // Initialize PointResults for training and test accuracy for
                // this point
                testResults[i] = new PointResults(numFolds);
                trainResults[i] = new PointResults(numFolds);
                // Train and test for each fold for this point
                for (int fold = 0; fold < numFolds; fold++)
                {
                    PrintToScreen.WriteLine("  Calculating results for fold " + fold);
                    // Creates training data for this fold, from the first
                    // percent data in each of the training folds
                    List<Example> train = getTrainCV(fold, percent);
                    // Creates testing data for this fold
                    List<Example> test = getTestCV(fold);
                    // Get testing results for this fold and percent setting
                    trainAndTestFold(train, test, fold, testResults[i], trainResults[i]);
                    if (debug)
                    {
                        PrintToScreen.WriteLine("Training on:\n" + train);
                        PrintToScreen.WriteLine("Testing on:\n" + test);
                    }
                }
            }
        }

        /**
         * Train and test on given example sets for the given fold:
         *
         * @param train             The training dataset vector
         * @param test              The testing dataset vector
         * @param fold              The current fold number
         * @param testPointResults  train accuracy PointResults for this point
         * @param trainPointResults test accuracy PointResults for this point
         */
        public void trainAndTestFold(List<Example> train, List<Example> test, int fold,
                                     PointResults testPointResults, PointResults trainPointResults)
        {
            long startTime = DateTime.Now.Millisecond;
            // train the classifier on train data
            classifier.train(train);
            double timeTaken = DateTime.Now.Millisecond - startTime;
            trainTime += timeTaken;

            // UnitTest on test data and measure time and accuracy
            int testCorrect = 0;
            startTime = DateTime.Now.Millisecond;
            foreach (Example example in test)
            {
                // classify the test example
                if (classifier.test(example))
                    testCorrect++;
            }
            timeTaken = DateTime.Now.Millisecond - startTime;
            testTime += timeTaken;
            testTimeNum += test.Count;

            testPointResults.setPoint(train.Count);
            double testAccuracy = 1.0 * testCorrect / test.Count;
            testPointResults.addResult(fold, testAccuracy);

            // UnitTest on training data and measure accuracy
            int trainCorrect = 0;
            foreach (Example example in train)
            {
                // classify the test example
                if (classifier.test(example))
                    trainCorrect++;
            }
            trainPointResults.setPoint(train.Count);
            double trainAccuracy = 1.0 * trainCorrect / train.Count;
            if (train.Count == 0) trainAccuracy = 1.0;
            trainPointResults.addResult(fold, trainAccuracy);

            PrintToScreen.WriteLine("    Train Accuracy = " + Math.Round(100 * trainAccuracy, 3) +
                "%; UnitTest Accuracy = " + Math.Round(100 * testAccuracy, 3) + "%");
        }

        /**
         * Set the fold Bins from the total Examples -- this effectively
         * stores the training-test split
         */
        public void binExamples()
        {
            for (int classNum = 0; classNum < numClasses; classNum++)
            {
                for (int j = 0; j < numFolds; j++)
                {
                    foldBins[classNum, j] = new List<Example>();
                }
                for (int j = 0; j < totalExamples[classNum].Count; j++)
                {
                    int foldNum = j % numFolds;
                    foldBins[classNum, foldNum].Add(totalExamples[classNum][j]);
                }
            }
        }

        /**
         * Creates the training set for one fold of a cross-validation
         * on the dataset.
         *
         * @param foldnum The fold for which training set is to be constructed
         * @param percent Percentage of examples to use for training in this fold
         * @return The training data
         */
        public List<Example> getTrainCV(int foldnum, double percent)
        {
            List<Example> train = new List<Example>();
            // Compute number of train examples to use
            int numTrain = (int)Math.Round(percent * totalNumTrain);
            // Collect enough from other fold bins to get this many training
            for (int j = 0; j < numFolds; j++)
            {
                // Avoid test fold for disjoint training
                if (j != foldnum)
                {
                    int foldSize = sizeOfFold(j);
                    // If adding this whole fold will not go over the number of
                    // training examples still needed...
                    if ((train.Count + foldSize) <= numTrain)
                    {
                        // Add all the examples in the fold to training data
                        for (int i = 0; i < numClasses; i++)
                        {
                            train.AddRange(foldBins[i, j]);
                        }
                    }
                    // Otherwise need to add just a fraction of this fold to complete
                    // train data
                    else
                    {
                        double fractionNeeded = ((double)(numTrain - train.Count)) / foldSize;
                        // Add needed fraction of data in each class in this fold
                        for (int i = 0; i < numClasses; i++)
                        {
                            // Number of examples needed from this fold and class
                            int len = (int)Math.Round(fractionNeeded * foldBins[i, j].Count);
                            for (int k = 0; k < len; k++)
                            {
                                train.Add(foldBins[i, j][k]);
                            }
                        }
                        break;
                    }
                }
            }
            PrintToScreen.WriteLine("    Number of training examples: " + train.Count);
            return train;
        }

        /**
         * Computes the total number of examples in given fold
         */
        protected int sizeOfFold(int foldNum)
        {
            int size = 0;
            for (int i = 0; i < numClasses; i++)
            {
                size += foldBins[i, foldNum].Count;
            }
            return size;
        }

        /**
         * Creates the testing set for one fold of a cross-validation
         * on the dataset.
         *
         * @param foldnum The fold which is to be used as testing data
         * @return The test data
         */
        public List<Example> getTestCV(int foldnum)
        {
            List<Example> test = new List<Example>();
            for (int i = 0; i < numClasses; i++)
                test.AddRange(foldBins[i, foldnum]);

            return test;
        }

        /**
         * Shuffles the examples in totalExamples so that they are ordered randomly.
         */
        private void randomizeOrder()
        {
            Random random = new Random((int)randomSeed);
            for (int i = 0; i < numClasses; i++)
            {
                int maxSize = totalExamples[i].Count;
                for (int j = maxSize - 1; j > 0; j--)
                {
                    int next = random.Next(maxSize);
                    Example temp = totalExamples[i][j];
                    totalExamples[i][j] = totalExamples[i][next];
                    totalExamples[i][next] = temp;
                }
            }
        }

        /**
         * Write out the final learning curve data.
         * One line for each value: [training set size, accuracy]
         * This is the format needed for GNUPLOT.
         *
         * @param allResults Array of results from which GNUPLOT data is generated
         * @param name       Name of classifier
         */
        void writeCurve(PointResults[] allResults, String name)
        {
            foreach (PointResults pointResults in allResults)
            {
                double accuracy = 0;
                double point = pointResults.getPoint();
                double[] results = pointResults.getResults();
                foreach (double result in results)
                {
                    accuracy += result;
                }
                // find average accuracy across the K folds
                accuracy /= results.Length;
                PrintToScreen.WriteLine(Math.Round(point) + "\t" + accuracy);
            }
        }

        /**
         * Write out an appropriate input file for GNUPLOT for the final
         * learning curve  to the output file with a ".gplot" extension.
         * See GNUPLOT documentation.
         *
         * @param allResults Array of results from which GNUPLOT data is generated
         * @param name Name of classifier
         */
        void makeGnuplotFile(PointResults[] allResults, String name)
        {
            writeCurve(allResults, name);
            PrintToScreen.WriteLine("set xlabel \"Size of training set\"\nset ylabel \"Accuracy\"\n\nset terminal postscript color\nset size 0.75,0.75\n\nset data style linespoints\n\nplot \'" + name + ".data\' title \"" + name + "\"");
        }
    }

}

