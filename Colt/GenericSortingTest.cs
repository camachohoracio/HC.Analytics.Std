#region

using System;
using HC.Analytics.Colt.CustomImplementations;
using HC.Analytics.Colt.doubleAlgo;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
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
    //package cern.colt;

    ////import IntComparator;
    /**
    Demonstrates how to use {@link Sort}.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 03-Jul-99
    */

    internal class GenericSortingTest : Object
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Just a demo.
         */

        public static void demo1()
        {
            int[] x;
            double[] y;
            double[] z;

            x = new[] {3, 2, 1};
            y = new[] {3.0, 2.0, 1.0};
            z = new[] {6.0, 7.0, 8.0};

            Swapper swapper = new Swapper6_(x, y, z);

            IntComparator comp = new IntComparator8_(x);

            PrintToScreen.WriteLine("before:");
            PrintToScreen.WriteLine("X=" + Arrays.ToString(x));
            PrintToScreen.WriteLine("Y=" + Arrays.ToString(y));
            PrintToScreen.WriteLine("Z=" + Arrays.ToString(z));


            int from = 0;
            int to = x.Length;
            GenericSorting.quickSort(from, to, comp, swapper);

            PrintToScreen.WriteLine("after:");
            PrintToScreen.WriteLine("X=" + Arrays.ToString(x));
            PrintToScreen.WriteLine("Y=" + Arrays.ToString(y));
            PrintToScreen.WriteLine("Z=" + Arrays.ToString(z));
            PrintToScreen.WriteLine("\n\n");
        }

        /**
         * Just a demo.
         */

        public static void demo2()
        {
            int[] x;
            double[] y;
            double[] z;

            x = new[] {6, 7, 8, 9};
            y = new[] {3.0, 2.0, 1.0, 3.0};
            z = new[] {5.0, 4.0, 4.0, 1.0};

            Swapper swapper = new Swapper7_(x, y, z);

            IntComparator comp = new IntComparator9_(y, z);


            PrintToScreen.WriteLine("before:");
            PrintToScreen.WriteLine("X=" + Arrays.ToString(x));
            PrintToScreen.WriteLine("Y=" + Arrays.ToString(y));
            PrintToScreen.WriteLine("Z=" + Arrays.ToString(z));


            int from = 0;
            int to = x.Length;
            GenericSorting.quickSort(from, to, comp, swapper);

            PrintToScreen.WriteLine("after:");
            PrintToScreen.WriteLine("X=" + Arrays.ToString(x));
            PrintToScreen.WriteLine("Y=" + Arrays.ToString(y));
            PrintToScreen.WriteLine("Z=" + Arrays.ToString(z));
            PrintToScreen.WriteLine("\n\n");
        }

        /**
         * Checks the correctness of the partition method by generating random input parameters and checking whether results are correct.
         */

        public static void testRandomly(int runs)
        {
            RngWrapper engine = new RngWrapper();
            RngWrapper gen = new RngWrapper();

            for (int run = 0; run < runs; run++)
            {
                int maxSize = 50;
                int maxSplittersSize = 2*maxSize;


                int size = gen.NextInt(1, maxSize);
                int from, to;
                if (size == 0)
                {
                    from = 0;
                    to = -1;
                }
                else
                {
                    from = gen.NextInt(0, size - 1);
                    to = gen.NextInt(Math.Min(from, size - 1), size - 1);
                }

                DoubleMatrix2D A1 = new DenseDoubleMatrix2D(size, size);
                DoubleMatrix2D P1 = A1.viewPart(from, from, size - to, size - to);

                int intervalFrom = gen.NextInt(size/2, 2*size);
                int intervalTo = gen.NextInt(intervalFrom, 2*size);

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        A1.set(i, j, gen.NextInt(intervalFrom, intervalTo));
                    }
                }

                DoubleMatrix2D A2 = A1.Copy();
                DoubleMatrix2D P2 = A2.viewPart(from, from, size - to, size - to);

                int c = 0;
                DoubleMatrix2D S1 = SortingDoubleAlgo.quickSort.Sort(P1, c);
                DoubleMatrix2D S2 = SortingDoubleAlgo.mergeSort.Sort(P2, c);

                if (!(S1.viewColumn(c).Equals(S2.viewColumn(c))))
                {
                    throw new HCException();
                }
            }

            PrintToScreen.WriteLine("All tests passed. No bug detected.");
        }
    }
}
