using System;
using System.Collections.Generic;
using HC.Core.Helpers;

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    public class TestNaiveBayes
    {
        /**
         * A driver method for testing the NaiveBayes classifier using
         * 10-fold cross validation.
         *
         * @param args a list of command-line arguments.  Specifying "-debug"
         *             will provide detailed output
         */
        public static void main(String[] args)
        {
            String[] categories = { "bio", "chem", "phys" };
            List<Example> examples = null;
            PrintToScreen.WriteLine("Initializing Naive Bayes classifier...");
            NaiveBayes BC;
            bool debug;
            // setting debug flag gives very detailed output, suitable for debugging
            if (args.Length == 1 && args[0].Equals("-debug"))
                debug = true;
            else
                debug = false;
            BC = new NaiveBayes(categories, debug);

            // Perform 10-fold cross validation to generate learning curve
            CVLearningCurve cvCurve = new CVLearningCurve(BC, examples);
            cvCurve.run();
        }
    }
}

