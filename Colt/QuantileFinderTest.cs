#region

using System;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package quantile;

    ////import Timer;
    ////import DoubleArrayList;
    ////import IntArrayList;
    /**
     * A class holding test cases for exact and approximate quantile finders.
     */

    internal class QuantileFinderTest
    {
        /**
         * Finds the first and last indexes of a specific element within a sorted list.
         * @return int[]
         * @param list DoubleArrayList
         * @param element the element to search for
         */

        public static IntArrayList binaryMultiSearch(DoubleArrayList list, double element)
        {
            int index = list.binarySearch(element);
            if (index < 0)
            {
                return null; //not found
            }

            int from = index - 1;
            while (from >= 0 && list.get(from) == element)
            {
                from--;
            }
            from++;

            int to = index + 1;
            while (to < list.Size() && list.get(to) == element)
            {
                to++;
            }
            to--;

            return new IntArrayList(new[] {from, to});
        }

        /**
         * Observed epsilon
         */

        public static double epsilon(int size, double phi, double rank)
        {
            double s = size;
            //PrintToScreen.WriteLine(Environment.NewLine);
            //PrintToScreen.WriteLine("s="+size+", rank="+rank+", phi="+phi+", eps="+Math.Abs((rank)/s - phi));
            //PrintToScreen.WriteLine(Environment.NewLine);
            return Math.Abs(rank/s - phi);
        }

        /**
         * Observed epsilon
         */

        public static double epsilon(DoubleArrayList sortedList, double phi, double element)
        {
            double rank = Descriptive.rankInterpolated(sortedList, element);
            return epsilon(sortedList.Size(), phi, rank);
        }

        /**
         * Observed epsilon
         */

        public static double epsilon(DoubleArrayList sortedList, DoubleQuantileFinder finder, double phi)
        {
            double element = finder.quantileElements(new DoubleArrayList(new[] {phi})).get(0);
            return epsilon(sortedList, phi, element);
        }

        public static void main(string[] args)
        {
            testBestBandKCalculation(args);
            //testQuantileCalculation(args);
            //testCollapse();
        }

        /**
         * This method was created in VisualAge.
         * @return double[]
         * @param values cern.it.hepodbms.primitivearray.DoubleArrayList
         * @param phis double[]
         */

        public static double observedEpsilonAtPhi(double phi, ExactDoubleQuantileFinder exactFinder,
                                                  DoubleQuantileFinder approxFinder)
        {
            int N = (int) exactFinder.Size();

            int exactRank = (int) Utils.epsilonCeiling(phi*N) - 1;
            //PrintToScreen.WriteLine("exactRank="+exactRank);
            exactFinder.quantileElements(new DoubleArrayList(new[] {phi})).get(0);
            // just to ensure exactFinder is sorted
            double approxElement = approxFinder.quantileElements(new DoubleArrayList(new[] {phi})).get(0);
            //PrintToScreen.WriteLine("approxElem="+approxElement);
            IntArrayList approxRanks = binaryMultiSearch(exactFinder.m_buffer, approxElement);
            int from = approxRanks.get(0);
            int to = approxRanks.get(1);

            int distance;
            if (from <= exactRank && exactRank <= to)
            {
                distance = 0;
            }
            else
            {
                if (from > exactRank)
                {
                    distance = Math.Abs(from - exactRank);
                }
                else
                {
                    distance = Math.Abs(exactRank - to);
                }
            }

            double epsilon = distance/(double) N;
            return epsilon;
        }

        /**
         * This method was created in VisualAge.
         * @return double[]
         * @param values cern.it.hepodbms.primitivearray.DoubleArrayList
         * @param phis double[]
         */

        public static DoubleArrayList observedEpsilonsAtPhis(DoubleArrayList phis, ExactDoubleQuantileFinder exactFinder,
                                                             DoubleQuantileFinder approxFinder, double desiredEpsilon)
        {
            DoubleArrayList epsilons = new DoubleArrayList(phis.Size());

            for (int i = phis.Size(); --i >= 0;)
            {
                double epsilon = observedEpsilonAtPhi(phis.get(i), exactFinder, approxFinder);
                epsilons.Add(epsilon);
                if (epsilon > desiredEpsilon)
                {
                    PrintToScreen.WriteLine("Real epsilon = " + epsilon + " is larger than desired by " +
                                      (epsilon - desiredEpsilon));
                }
            }
            return epsilons;
        }

        /**
         * Not yet commented.
         */

        public static void test()
        {
            string[] args = new string[20];

            string size = "10000";
            args[0] = size;

            //string b="5";
            string b = "12";
            args[1] = b;

            string k = "2290";
            args[2] = k;

            string enableLogging = "log";
            args[3] = enableLogging;

            string chunks = "10";
            args[4] = chunks;

            string computeExactQuantilesAlso = "exact";
            args[5] = computeExactQuantilesAlso;

            string doShuffle = "shuffle";
            args[6] = doShuffle;

            string epsilon = "0.001";
            args[7] = epsilon;

            string delta = "0.0001";
            //string delta = "0.0001";
            args[8] = delta;

            string quantiles = "1";
            args[9] = quantiles;

            string max_N = "-1";
            args[10] = max_N;


            testQuantileCalculation(args);
        }

        /**
         * This method was created in VisualAge.
         */

        public static void testBestBandKCalculation(string[] args)
        {
            //bool known_N;
            //if (args==null) known_N = false;
            //else known_N = (args[0]);

            int[] quantiles = {100, 10000};
            //int[] quantiles = {1,100,10000};

            long[] sizes = {long.MaxValue, 1000000, 10000000, 100000000};

            double[] deltas = {0.0, 0.1, 0.00001};
            //double[] deltas = {0.0, 0.001, 0.00001, 0.000001};

            //double[] epsilons = {0.0, 0.01, 0.001, 0.0001, 0.00001};
            double[] epsilons = {0.0, 0.1, 0.01, 0.001, 0.0001, 0.00001, 0.000001};


            //if (! known_N) sizes = [] {0};
            PrintToScreen.WriteLine("\n\n");
            //if (known_N) 
            //	PrintToScreen.WriteLine("Computing b's and k's for KNOWN N");
            //else 
            //	PrintToScreen.WriteLine("Computing b's and k's for UNKNOWN N");
            PrintToScreen.WriteLine("mem [Math.Round(elements/1000.0)]");
            PrintToScreen.WriteLine("***********************************");
            Timer timer = new Timer().start();

            for (int q = 0; q < quantiles.Length; q++)
            {
                int p = quantiles[q];
                PrintToScreen.WriteLine("------------------------------");
                PrintToScreen.WriteLine("computing for p = " + p);
                for (int s = 0; s < sizes.Length; s++)
                {
                    long N = sizes[s];
                    PrintToScreen.WriteLine("   ------------------------------");
                    PrintToScreen.WriteLine("   computing for N = " + N);
                    for (int e = 0; e < epsilons.Length; e++)
                    {
                        double epsilon = epsilons[e];
                        PrintToScreen.WriteLine("      ------------------------------");
                        PrintToScreen.WriteLine("      computing for e = " + epsilon);
                        for (int d = 0; d < deltas.Length; d++)
                        {
                            double delta = deltas[d];
                            for (int knownCounter = 0; knownCounter < 2; knownCounter++)
                            {
                                bool known_N;
                                if (knownCounter == 0)
                                {
                                    known_N = true;
                                }
                                else
                                {
                                    known_N = false;
                                }

                                DoubleQuantileFinder finder = QuantileFinderFactory.newDoubleQuantileFinder(known_N, N,
                                                                                                            epsilon,
                                                                                                            delta, p,
                                                                                                            null);
                                //PrintToScreen.WriteLine(finder.ToString());
                                /*
                                double[] returnSamplingRate = new double[1];
                                long[] result;
                                if (known_N) {
                                    result = QuantileFinderFactory.known_N_compute_B_and_K(N, epsilon, delta, p, returnSamplingRate);
                                }
                                else {
                                    result = QuantileFinderFactory.unknown_N_compute_B_and_K(epsilon, delta, p);
                                    long b1 = result[0];
                                    long k1 = result[1];

                                    if (N>=0) {
                                        long[] resultKnown = QuantileFinderFactory.known_N_compute_B_and_K(N, epsilon, delta, p, returnSamplingRate);
                                        long b2 = resultKnown[0];
                                        long k2 = resultKnown[1];
				
                                        if (b2 * k2 < b1 * k1) { // the KnownFinder is smaller
                                            result = resultKnown;
                                        }
                                    }
                                }
						

                                long b = result[0];
                                long k = result[1];
                                */
                                string knownStr = known_N ? "  known" : "unknown";
                                long mem = finder.totalMemory();
                                if (mem == 0)
                                {
                                    mem = N;
                                }
                                //else if (mem==0 && !known_N && N<0) mem = long.MaxValue; // actually infinity
                                //else if (mem==0 && !known_N && N>=0) mem = N;
                                //Console.Write("         (e,d,N,p)=("+epsilon+","+delta+","+N+","+p+") --> ");
                                Console.Write("         (known, d)=(" + knownStr + ", " + delta + ") --> ");
                                //Console.Write("(mem,b,k,memF");
                                Console.Write("(MB,mem");
                                //if (known_N) Console.Write(",sampling");
                                //Console.Write(")=("+(Math.Round(b*k/1000.0))+","+b+","+k+", "+Math.Round(b*k*8/1024.0/1024.0));
                                //Console.Write(")=("+b*k/1000.0+","+b+","+k+", "+b*k*8/1024.0/1024.0+", "+Math.Round(b*k*8/1024.0/1024.0));
                                Console.Write(")=(" + mem*8.0/1024.0/1024.0 + ",  " + mem/1000.0 + ",  " +
                                              Math.Round(mem*8.0/1024.0/1024.0));
                                //if (known_N) Console.Write(","+returnSamplingRate[0]);
                                PrintToScreen.WriteLine(")");
                            }
                        }
                    }
                }
            }

            timer.stop().display();
        }

        /**
         * This method was created in VisualAge.
         */

        public static void testLocalVarDeclarationSpeed(int size)
        {
            PrintToScreen.WriteLine("free=" +
                              MemoryHelper.GetAvailableMemory());
            PrintToScreen.WriteLine("total=" +
                              MemoryHelper.GetAvailableMemory());

            /*Timer timer = new Timer().start();
            for (int i=0; i<size; i++) {
                for (int j=0; j<size; j++) {
                    DoubleBuffer buffer=null;
                    int val=10;
                    double f=1.0f;
                }
            }
            PrintToScreen.WriteLine(timer.stop());
            */

            Timer timer = new Timer().start();
            //DoubleBuffer buffer;
            //int val;
            //double f;
            int j;

            for (int i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    //buffer=null;
                    //val = 10;
                    //f = 1.0f;
                }
            }
            PrintToScreen.WriteLine(timer.stop());

            PrintToScreen.WriteLine("free=" + MemoryHelper.GetAvailableMemory());
            PrintToScreen.WriteLine("total=" + MemoryHelper.GetAvailableMemory());
        }

        /**
         */

        public static void testQuantileCalculation(string[] args)
        {
            int size = int.Parse(args[0]);
            int b = int.Parse(args[1]);
            int k = int.Parse(args[2]);
            //cern.it.util.Log.enableLogging(args[3].Equals("log"));
            int chunks = int.Parse(args[4]);
            bool computeExactQuantilesAlso = args[5].Equals("exact");
            bool doShuffle = args[6].Equals("shuffle");
            double epsilon = System.Convert.ToDouble(args[7]);
            double delta = System.Convert.ToDouble(args[8]);
            int quantiles = int.Parse(args[9]);
            long max_N = long.Parse(args[10]);


            PrintToScreen.WriteLine("free=" + MemoryHelper.GetAvailableMemory());
            PrintToScreen.WriteLine("total=" + MemoryHelper.GetAvailableMemory());

            double[] phis = {0.001, 0.01, 0.1, 0.5, 0.9, 0.99, 0.999, 1.0};
            //int quantiles = phis.Length;

            Timer timer = new Timer();
            Timer timer2 = new Timer();
            DoubleQuantileFinder approxFinder;

            approxFinder = QuantileFinderFactory.newDoubleQuantileFinder(false, max_N, epsilon, delta, quantiles, null);
            PrintToScreen.WriteLine(approxFinder);
            //new UnknownApproximateDoubleQuantileFinder(b,k);
            //approxFinder = new ApproximateDoubleQuantileFinder(b,k);
            /*
            double[] returnSamplingRate = new double[1];
            long[] result = ApproximateQuantileFinder.computeBestBandK(size*chunks, epsilon, delta, quantiles, returnSamplingRate);
            approxFinder = new ApproximateQuantileFinder((int) result[0], (int) result[1]);
            PrintToScreen.WriteLine("epsilon="+epsilon);
            PrintToScreen.WriteLine("delta="+delta);
            PrintToScreen.WriteLine("samplingRate="+returnSamplingRate[0]);
            */


            DoubleQuantileFinder exactFinder = QuantileFinderFactory.newDoubleQuantileFinder(false, -1, 0.0, delta,
                                                                                             quantiles, null);
            PrintToScreen.WriteLine(exactFinder);

            DoubleArrayList list = new DoubleArrayList(size);

            for (int chunk = 0; chunk < chunks; chunk++)
            {
                list.setSize(0);
                int d = chunk*size;
                timer2.start();
                for (int i = 0; i < size; i++)
                {
                    list.Add((i + d));
                }
                timer2.stop();


                //PrintToScreen.WriteLine("unshuffled="+list);
                if (doShuffle)
                {
                    Timer timer3 = new Timer().start();
                    list.shuffle();
                    PrintToScreen.WriteLine("shuffling took ");
                    timer3.stop().display();
                }
                //PrintToScreen.WriteLine("shuffled="+list);
                //list.Sort();
                //PrintToScreen.WriteLine("sorted="+list);

                timer.start();
                approxFinder.addAllOf(list);
                timer.stop();

                if (computeExactQuantilesAlso)
                {
                    exactFinder.addAllOf(list);
                }
            }
            PrintToScreen.WriteLine("list.Add() took" + timer2);
            PrintToScreen.WriteLine("approxFinder.Add() took" + timer);

            //PrintToScreen.WriteLine("free="+MemoryHelper.GetAvailableMemory());
            //PrintToScreen.WriteLine("total="+MemoryHelper.GetAvailableMemory());

            timer.reset().start();

            //approxFinder.close();
            DoubleArrayList approxQuantiles = approxFinder.quantileElements(new DoubleArrayList(phis));

            timer.stop().display();

            PrintToScreen.WriteLine("Phis=" + new DoubleArrayList(phis));
            PrintToScreen.WriteLine("ApproxQuantiles=" + approxQuantiles);

            //PrintToScreen.WriteLine("MaxLevel of full buffers="+maxLevelOfFullBuffers(approxFinder.bufferSet));

            //PrintToScreen.WriteLine("total buffers filled="+ approxFinder.totalBuffersFilled);
            //PrintToScreen.WriteLine("free="+MemoryHelper.GetAvailableMemory());
            //PrintToScreen.WriteLine("total="+MemoryHelper.GetAvailableMemory());


            if (computeExactQuantilesAlso)
            {
                PrintToScreen.WriteLine("Comparing with exact quantile computation...");

                timer.reset().start();

                //exactFinder.close();
                DoubleArrayList exactQuantiles = exactFinder.quantileElements(new DoubleArrayList(phis));
                timer.stop().display();

                PrintToScreen.WriteLine("ExactQuantiles=" + exactQuantiles);


                //double[] errors1 = errors1(exactQuantiles.elements(), approxQuantiles.elements());
                //PrintToScreen.WriteLine("Error1="+new DoubleArrayList(errors1));

                /*
                 DoubleArrayList buffer = new DoubleArrayList((int)exactFinder.Size());
                exactFinder.forEach(
                    new DoubleFunction() {
                        public void Apply(double element) {
                            buffer.Add(element);
                        }
                    }
                );
                */


                DoubleArrayList observedEpsilons = observedEpsilonsAtPhis(new DoubleArrayList(phis),
                                                                          (ExactDoubleQuantileFinder) exactFinder,
                                                                          approxFinder, epsilon);
                PrintToScreen.WriteLine("observedEpsilons=" + observedEpsilons);

                double element = 1000.0f;


                PrintToScreen.WriteLine("exact phi(" + element + ")=" + exactFinder.phi(element));
                PrintToScreen.WriteLine("apprx phi(" + element + ")=" + approxFinder.phi(element));

                PrintToScreen.WriteLine("exact elem(phi(" + element + "))=" +
                                  exactFinder.quantileElements(new DoubleArrayList(new[] {exactFinder.phi(element)})));
                PrintToScreen.WriteLine("apprx elem(phi(" + element + "))=" +
                                  approxFinder.quantileElements(new DoubleArrayList(new[] {approxFinder.phi(element)})));
            }
        }

        /**
         * Not yet commented.
         */

        public static void testRank()
        {
            DoubleArrayList list = new DoubleArrayList(new double[] {1.0f, 5.0f, 5.0f, 5.0f, 7.0f, 10.0f});
            //PrintToScreen.WriteLine(rankOfWithin(5.0f, list));
        }
    }
}
