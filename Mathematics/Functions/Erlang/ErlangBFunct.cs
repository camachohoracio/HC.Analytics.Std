#region

using System;
using HC.Analytics.Mathematics.Roots;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.Functions.Erlang
{
    internal class ErlangBfunct : RealRootFunction
    {
        #region Members

        public double blockingProbability;
        public double totalResources;

        #endregion

        #region RealRootFunction Members

        public double function(double x)
        {
            return blockingProbability - erlangBprobability(x, totalResources);
        }

        #endregion

        // Erlang B equation
        // returns the probablility that a customer will be rejected due to lack of resources
        // totalTraffic:    total traffic in Erlangs
        // totalResouces:   total number of resources in the system
        // recursive calculation
        public static double erlangBprobability(double totalTraffic, double totalResources)
        {
            if (totalResources < 1)
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(totalResources))
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be, arithmetically, an integer");
            }
            double prob = 0.0D;
            double iCount = 1.0D;
            if (totalTraffic > 0.0D)
            {
                prob = 1.0D;
                double hold = 0.0;
                while (iCount <= totalResources)
                {
                    hold = prob*totalTraffic;
                    prob = hold/(iCount + hold);
                    iCount += 1.0;
                }
            }
            return prob;
        }


        public static double erlangBprobability(double totalTraffic, long totalResources)
        {
            return erlangBprobability(totalTraffic, (double) totalResources);
        }

        public static double erlangBprobability(double totalTraffic, int totalResources)
        {
            return erlangBprobability(totalTraffic, (double) totalResources);
        }

        // Erlang B equation
        // returns the maximum total traffic in Erlangs
        // blockingProbability:    probablility that a customer will be rejected due to lack of resources
        // totalResouces:   total number of resources in the system
        public static double erlangBload(double blockingProbability, double totalResources)
        {
            if (totalResources < 1)
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(totalResources))
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be, arithmetically, an integer");
            }

            // Create instance of the class holding the Erlang B equation
            ErlangBfunct eBfunc = new ErlangBfunct();

            // Set instance variables
            eBfunc.blockingProbability = blockingProbability;
            eBfunc.totalResources = totalResources;

            // lower bound
            double lowerBound = 0.0D;
            // upper bound   // arbitrary - may be extended by bisects automatic extension
            double upperBound = 20.0;
            // required tolerance
            double tolerance = 1e-6;

            // Create instance of RealRoot
            RealRoot realR = new RealRoot();

            // Set tolerance
            realR.setTolerance(tolerance);

            // Set bounds limits
            realR.noLowerBoundExtension();

            // Supress error message if iteration limit reached
            realR.supressLimitReachedMessage();

            // call root searching method
            double root = realR.bisect(eBfunc, lowerBound, upperBound);

            return root;
        }

        public static double erlangBload(double blockingProbability, long totalResources)
        {
            return erlangBload(blockingProbability, (double) totalResources);
        }

        public static double erlangBload(double blockingProbability, int totalResources)
        {
            return erlangBload(blockingProbability, (double) totalResources);
        }

        // Erlang B equation
        // returns the resources bracketing a blocking probability for a given total traffic
        // blockingProbability:    probablility that a customer will be rejected due to lack of resources
        // totalResouces:   total number of resources in the system
        public static double[] erlangBresources(double blockingProbability, double totalTraffic)
        {
            double[] ret = new double[8];
            long counter = 1;
            double lastProb = double.NaN;
            double prob = double.NaN;
            bool test = true;
            while (test)
            {
                prob = erlangBprobability(totalTraffic, counter);
                if (prob <= blockingProbability)
                {
                    ret[0] = counter;
                    ret[1] = prob;
                    ret[2] = erlangBload(blockingProbability, counter);
                    ret[3] = (counter - 1);
                    ret[4] = lastProb;
                    ret[5] = erlangBload(blockingProbability, counter - 1);
                    ret[6] = blockingProbability;
                    ret[7] = totalTraffic;
                    test = false;
                }
                else
                {
                    lastProb = prob;
                    counter++;
                    if (counter == int.MaxValue)
                    {
                        PrintToScreen.WriteLine("Method erlangBresources: no solution found below " + long.MaxValue +
                                                "resources");
                        for (int i = 0; i < 8; i++)
                        {
                            ret[i] = double.NaN;
                        }
                        test = false;
                    }
                }
            }
            return ret;
        }
    }
}
