#region

using System;
using HC.Analytics.Mathematics.Roots;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.Functions.Erlang
{
    internal class ErlangCfunct : RealRootFunction
    {
        #region Members

        public double nonZeroDelayProbability;
        public double totalResources;

        #endregion

        #region RealRootFunction Members

        public double function(double x)
        {
            return nonZeroDelayProbability - erlangCprobability(x, totalResources);
        }

        #endregion

        // Erlang C equation
        // returns the probablility that a customer will receive a non-zero delay in obtaining obtaining a resource
        // totalTraffic:    total traffic in Erlangs
        // totalResouces:   total number of resources in the system
        public static double erlangCprobability(double totalTraffic, double totalResources)
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
            if (totalTraffic > 0.0D)
            {
                double probB = ErlangBfunct.erlangBprobability(totalTraffic, totalResources);
                prob = 1.0 + (1.0/probB - 1.0)*(totalResources - totalTraffic)/totalResources;
                prob = 1.0/prob;
            }
            return prob;
        }

        public static double erlangCprobability(double totalTraffic, long totalResources)
        {
            return erlangCprobability(totalTraffic, (double) totalResources);
        }

        public static double erlangCprobability(double totalTraffic, int totalResources)
        {
            return erlangCprobability(totalTraffic, (double) totalResources);
        }

        // Erlang C equation
        // returns the maximum total traffic in Erlangs
        // nonZeroDelayProbability:    probablility that a customer will receive a non-zero delay in obtaining obtaining a resource
        // totalResouces:   total number of resources in the system
        public static double erlangCload(double nonZeroDelayProbability, double totalResources)
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

            // Create instance of the class holding the Erlang C equation
            ErlangCfunct eCfunc = new ErlangCfunct();

            // Set instance variables
            eCfunc.nonZeroDelayProbability = nonZeroDelayProbability;
            eCfunc.totalResources = totalResources;

            // lower bound
            double lowerBound = 0.0D;
            // upper bound
            double upperBound = 10.0D;
            // required tolerance
            double tolerance = 1e-6;

            // Create instance of RealRoot
            RealRoot realR = new RealRoot();

            // Set tolerance
            realR.setTolerance(tolerance);

            // Supress error message if iteration limit reached
            realR.supressLimitReachedMessage();

            // Set bounds limits
            realR.noLowerBoundExtension();

            // call root searching method
            double root = realR.bisect(eCfunc, lowerBound, upperBound);

            return root;
        }

        public static double erlangCload(double nonZeroDelayProbability, long totalResources)
        {
            return erlangCload(nonZeroDelayProbability, (double) totalResources);
        }

        public static double erlangCload(double nonZeroDelayProbability, int totalResources)
        {
            return erlangCload(nonZeroDelayProbability, (double) totalResources);
        }

        // Erlang C equation
        // returns the resources bracketing a non-zer delay probability for a given total traffic
        // nonZeroDelayProbability:    probablility that a customer will receive a non-zero delay in obtaining obtaining a resource
        // totalResouces:   total number of resources in the system
        public static double[] erlangCresources(double nonZeroDelayProbability, double totalTraffic)
        {
            double[] ret = new double[8];
            long counter = 1;
            double lastProb = double.NaN;
            double prob = double.NaN;
            bool test = true;
            while (test)
            {
                prob = erlangCprobability(totalTraffic, counter);
                if (prob <= nonZeroDelayProbability)
                {
                    ret[0] = counter;
                    ret[1] = prob;
                    ret[2] = erlangCload(nonZeroDelayProbability, counter);
                    ret[3] = (counter - 1);
                    ret[4] = lastProb;
                    ret[5] = erlangCload(nonZeroDelayProbability, counter - 1);
                    ret[6] = nonZeroDelayProbability;
                    ret[7] = totalTraffic;
                    test = false;
                }
                else
                {
                    lastProb = prob;
                    counter++;
                    if (counter == int.MaxValue)
                    {
                        PrintToScreen.WriteLine("Method erlangCresources: no solution found below " + long.MaxValue +
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
