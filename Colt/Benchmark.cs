#region

using System;
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
    //package engine;

    /**
     * Benchmarks the performance of the currently provided uniform pseudo-random number generation engines.
     * <p>
     * All distributions are obtained by using a <b>uniform</b> pseudo-random number generation engine.
     * followed by a transformation to the desired distribution.
     * Therefore, the performance of the uniform engines is crucial.
     * <p>
     * <h2 align=center>Comparison of uniform generation engines</h2>
     * <center>
     *   <table border>
     *     <tr> 
     *       <td align="center" width="40%">Name</td>
     *       <td align="center" width="20%">Period</td>
     *       <td align="center" width="40%">
     *         <p>Speed<br>
     *           [# million uniform random numbers generated/sec]<br>
     *           Pentium Pro 200 Mhz, JDK 1.2, NT</p>
     *         </td>
     *     </tr>
     *     <tr> 
     *       <td align="center" width="40%"> <tt>MersenneTwister</tt></td>
     *       <td align="center" width="20%">2<sup>19937</sup>-1 (=10<sup>6001</sup>)</td>
     *       <td align="center" width="40">2.5</td>
     *     </tr>
     *     <tr> 
     *       <td align="center" width="40%"> <tt>Ranlux</tt> (default luxury level 3) </td>
     *       <td align="center" width="20%">10<sup>171</sup></td>
     *       <td align="center" width="40">0.4</td>
     *     </tr>
     *     <tr> 
     *       <td align="center" width="40"> <tt>Ranmar</tt></td>
     *       <td align="center" width="20">10<sup>43</sup></td>
     *       <td align="center" width="40%">1.6</td>
     *     </tr>
     *     <tr> 
     *       <td align="center" width="40%"> <tt>Ranecu</tt> </td>
     *       <td align="center" width="20">10<sup>18</sup></td>
     *       <td align="center" width="40%">1.5</td>
     *     </tr>
     *     <tr> 
     *       <td align="center"> <tt>RngWrapper.nextFloat() </tt><tt> 
     *         </tt></td>
     *       <td align="center"><font size=+3>?</font></td>
     *       <td align="center">2.4</td>
     *     </tr>
     *   </table>
     * </center>
     * <p>
     * <b>Note:</b> Methods working on the default uniform random generator are <b>lock</b> and therefore in current VM's <b>slow</b> (as of June '99).
     * Methods taking as argument a uniform random generator are <b>not lock</b> and therefore much <b>quicker</b>.
     * Thus, if you need a lot of random numbers, you should use the unsynchronized approach:
     * <p>
     * <b>Example usage:</b><pre>
     * edu.cornell.lassp.houle.RngPack.RandomElement generator;
     * generator = new MersenneTwister(new DateTime());
     * //generator = new edu.cornell.lassp.houle.RngPack.Ranecu(new DateTime());
     * //generator = new edu.cornell.lassp.houle.RngPack.Ranmar(new DateTime());
     * //generator = new edu.cornell.lassp.houle.RngPack.Ranlux(new DateTime());
     * //generator = makeDefaultGenerator();
     * for (int i=1000000; --i >=0; ) {
     *    double uniform = generator.NextDouble();
     *    ...
     * }
     * </pre>
     *
     *
     * @see cern.jet.random
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     */

    [Serializable]
    public class Benchmark
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */

        public Benchmark()
        {
            throw new HCException("Non instantiable");
        }

        /**
         * Benchmarks <tt>NextDouble()</tt> for various uniform generation engines.
         */

        public static void benchmark(int times)
        {
            Timer timer = new Timer();
            RandomEngine gen;

            timer.reset().start();
            for (int i = times; --i >= 0;)
            {
                ; // no operation
            }
            timer.stop().display();
            float emptyLoop = timer.elapsedTime();
            PrintToScreen.WriteLine("empty loop timing done.");

            gen = new MersenneTwister();
            PrintToScreen.WriteLine("\n MersenneTwister:");
            timer.reset().start();
            for (int i = times; --i >= 0;)
            {
                gen.NextDouble();
            }
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime() - emptyLoop) + " numbers per second.");


            gen = new MersenneTwister64();
            PrintToScreen.WriteLine("\n MersenneTwister64:");
            timer.reset().start();
            for (int i = times; --i >= 0;)
            {
                gen.NextDouble();
            }
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime() - emptyLoop) + " numbers per second.");

            /*
            gen = new edu.stanford.mt.MersenneTwister();
            PrintToScreen.WriteLine("\n edu.stanford.mt.MersenneTwister:");
            timer.reset().start();
            for (int i=times; --i>=0; ) gen.NextDouble();
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime()-emptyLoop)+ " numbers per second.");
            */


            gen = new DRand();
            PrintToScreen.WriteLine("\nDRand:");
            timer.reset().start();
            for (int i = times; --i >= 0;)
            {
                gen.NextDouble();
            }
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime() - emptyLoop) + " numbers per second.");


            RngWrapper javaGen = new RngWrapper();
            PrintToScreen.WriteLine("\njava.util.RngWrapper.nextFloat():");
            timer.reset().start();
            for (int i = times; --i >= 0;)
            {
                javaGen.NextDouble(); // NextDouble() is slower
            }
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime() - emptyLoop) + " numbers per second.");

            /*
            gen = new edu.cornell.lassp.houle.RngPack.Ranecu();
            PrintToScreen.WriteLine("\nRanecu:");
            timer.reset().start();
            for (int i=times; --i>=0; ) gen.NextDouble();
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime()-emptyLoop)+ " numbers per second.");	
	
            gen = new edu.cornell.lassp.houle.RngPack.Ranmar();
            PrintToScreen.WriteLine("\nRanmar:");
            timer.reset().start();
            for (int i=times; --i>=0; ) gen.NextDouble();
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime()-emptyLoop)+ " numbers per second.");

            gen = new edu.cornell.lassp.houle.RngPack.Ranlux();
            PrintToScreen.WriteLine("\nRanlux:");
            timer.reset().start();
            for (int i=times; --i>=0; ) gen.NextDouble();
            timer.stop().display();
            PrintToScreen.WriteLine(times/(timer.elapsedTime()-emptyLoop)+ " numbers per second.");
            */

            PrintToScreen.WriteLine("\nGood bye.\n");
        }

        /**
         * Tests various methods of this class.
         */

        public static void main(string[] args)
        {
            long from = long.Parse(args[0]);
            long to = long.Parse(args[1]);
            int times = int.Parse(args[2]);
            int runs = int.Parse(args[3]);
            //testRandomFromTo(from,to,times);
            //benchmark(1000000);
            //benchmark(1000000);
            for (int i = 0; i < runs; i++)
            {
                benchmark(times);
                //benchmarkSync(times);
            }
        }

        /**
         * Prints the first <tt>size</tt> random numbers generated by the given engine.
         */

        public static void test(int size, RngWrapper randomEngine)
        {
            RngWrapper random;

            /*
            PrintToScreen.WriteLine("NextDouble():");
            random = (RngWrapper) randomEngine.Clone();
            //Timer timer = new Timer().start();
            for (int j=0, i=size; --i>=0; j++) {
                Console.Write(" "+random.NextDouble());
                if (j%8==7) PrintToScreen.WriteLine();
            }

            PrintToScreen.WriteLine("\n\nfloat():");
            random = (RngWrapper) randomEngine.Clone();
            for (int j=0, i=size; --i>=0; j++) {
                Console.Write(" "+random.nextFloat());
                if (j%8==7) PrintToScreen.WriteLine();
            }

            PrintToScreen.WriteLine("\n\ndouble():");
            random = (RngWrapper) randomEngine.Clone();
            for (int j=0, i=size; --i>=0; j++) {
                Console.Write(" "+random.NextDouble());
                if (j%8==7) PrintToScreen.WriteLine();
            }
            */
            PrintToScreen.WriteLine("\n\nint():");
            random = (RngWrapper) randomEngine.Clone();
            for (int j = 0, i = size; --i >= 0; j++)
            {
                Console.Write(" " + random.NextInt(int.MaxValue));
                if (j%8 == 7)
                {
                    PrintToScreen.WriteLine();
                }
            }

            //timer.stop().display();
            PrintToScreen.WriteLine("\n\nGood bye.\n");
        }

        /**
         * Tests various methods of this class.
         */

        private static void xtestRandomFromTo(long from, long to, int times)
        {
            PrintToScreen.WriteLine("from=" + from + ", to=" + to);

            //set.OpenMultiFloatHashSet multiset = new set.OpenMultiFloatHashSet();

            RngWrapper randomJava = new RngWrapper();
            //edu.cornell.lassp.houle.RngPack.RandomElement random = new edu.cornell.lassp.houle.RngPack.Ranecu();
            //edu.cornell.lassp.houle.RngPack.RandomElement random = new edu.cornell.lassp.houle.RngPack.MT19937B();
            //edu.cornell.lassp.houle.RngPack.RandomElement random = new edu.stanford.mt.MersenneTwister();
            RandomEngine random = new MersenneTwister();
            int _from = (int) from, _to = (int) to;
            Timer timer = new Timer().start();
            for (int j = 0, i = times; --i >= 0; j++)
            {
                //randomJava.NextInt(10000);
                //Integers.randomFromTo(_from,_to);
                Console.Write(" " + random.NextDouble());
                if (j%8 == 7)
                {
                    PrintToScreen.WriteLine();
                }
                //multiset.Add(nextIntFromTo(_from,_to));
            }

            timer.stop().display();
            //PrintToScreen.WriteLine(multiset); //check the distribution
            PrintToScreen.WriteLine("Good bye.\n");
        }
    }
}
