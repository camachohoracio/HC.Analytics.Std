using System;

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    public class PointResults
    {
        /**
         * Point on curve at which results are for
         */
        protected double point;

        /**
         * Sampled values of result at this point
         */
        protected double[] results;

        /**
         * Create a vector of results for a point
         */
        public PointResults(int numResults)
        {
            results = new double[numResults];
        }

        public double getPoint()
        {
            return point;
        }

        public void setPoint(double point)
        {
            this.point = point;
        }

        /**
         * Set the nth result
         */
        public void addResult(int numResult, double result)
        {
            results[numResult] = result;
        }

        public double[] getResults()
        {
            return results;
        }

        public String toString()
        {
            String result = "<" + point + ": " + results[0];
            for (int i = 1; i < results.Length; i++)
            {
                result = result + "," + results[i];
            }
            return result + ">";
        }
    }
}

