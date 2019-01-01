#region

using System;
using HC.Analytics.Probability.Random;
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
    //package sampling;

    ////import BooleanArrayList;
    ////import RngWrapper;
    /**
     * Conveniently computes a stable <i>Simple RngWrapper Sample Without Replacement (SRSWOR)</i> subsequence of <tt>n</tt> elements from a given input sequence of <tt>N</tt> elements;
     * Example: Computing a sublist of <tt>n=3</tt> random elements from a list <tt>(1,...,50)</tt> may yield the sublist <tt>(7,13,47)</tt>.
     * The subsequence is guaranteed to be <i>stable</i>, i.e. elements never change position relative to each other.
     * Each element from the <tt>N</tt> elements has the same probability to be included in the <tt>n</tt> chosen elements.
     * This class is a convenience adapter for <tt>RandomSampler</tt> using blocks.
     *
     * @see RandomSampler
     * @author  wolfgang.hoschek@cern.ch
     * @version 1.0, 02/05/99
     */

    [Serializable]
    public class RandomSamplingAssistant : PersistentObject
    {
        //[Serializable]public class RandomSamplingAssistant : Object : java.io.Serializable {
        private static int MAX_BUFFER_SIZE = 200;
        public long[] m_buffer;
        public int m_bufferPosition;

        public long m_n;
        public RandomSampler m_sampler;
        public long m_skip;

        /**
         * Constructs a random sampler that samples <tt>n</tt> random elements from an input sequence of <tt>N</tt> elements.
         *
         * @param n the total number of elements to choose (must be &gt;= 0).
         * @param N number of elements to choose from (must be &gt;= n).
         * @param randomGenerator a random number generator. Set this parameter to <tt>null</tt> to use the default random number generator.
         */

        public RandomSamplingAssistant(long n, long N, RngWrapper randomGenerator)
        {
            m_n = n;
            m_sampler = new RandomSampler(n, N, 0, randomGenerator);
            m_buffer = new long[(int) Math.Min(n, MAX_BUFFER_SIZE)];
            if (n > 0)
            {
                m_buffer[0] = -1; // start with the right offset
            }

            fetchNextBlock();
        }

        /**
         * Returns a deep copy of the receiver.
         */

        public new Object Clone()
        {
            RandomSamplingAssistant copy = (RandomSamplingAssistant) base.Clone();
            copy.m_sampler = (RandomSampler) m_sampler.Clone();
            return copy;
        }

        /**
         * Not yet commented.
         */

        public void fetchNextBlock()
        {
            if (m_n > 0)
            {
                long last = m_buffer[m_bufferPosition];
                m_sampler.nextBlock((int) Math.Min(m_n, MAX_BUFFER_SIZE), m_buffer, 0);
                m_skip = m_buffer[0] - last - 1;
                m_bufferPosition = 0;
            }
        }

        /**
         * Returns the used random generator.
         */

        public RngWrapper getRandomGenerator()
        {
            return m_sampler.my_RandomGenerator;
        }

        /**
         * Tests random sampling.
         */

        public static void main(string[] args)
        {
            long n = long.Parse(args[0]);
            long N = long.Parse(args[1]);
            //test(n,N);
            testArraySampling((int) n, (int) N);
        }

        /**
         * Just shows how this class can be used; samples n elements from and int[] array.
         */

        public static int[] sampleArray(int n, int[] elements)
        {
            RandomSamplingAssistant assistant = new RandomSamplingAssistant(n, elements.Length, null);
            int[] sample = new int[n];
            int j = 0;
            int Length = elements.Length;
            for (int i = 0; i < Length; i++)
            {
                if (assistant.sampleNextElement())
                {
                    sample[j++] = elements[i];
                }
            }
            return sample;
        }

        /**
         * Returns whether the next element of the input sequence shall be sampled (picked) or not.
         * @return <tt>true</tt> if the next element shall be sampled (picked), <tt>false</tt> otherwise.
         */

        public bool sampleNextElement()
        {
            if (m_n == 0)
            {
                return false; //reject
            }
            if (m_skip-- > 0)
            {
                return false; //reject
            }

            //accept
            m_n--;
            if (m_bufferPosition < m_buffer.Length - 1)
            {
                m_skip = m_buffer[m_bufferPosition + 1] - m_buffer[m_bufferPosition++];
                --m_skip;
            }
            else
            {
                fetchNextBlock();
            }

            return true;
        }

        /**
         * Tests the methods of this class.
         * To do benchmarking, comment the lines printing stuff to the console.
         */

        public static void test(long n, long N)
        {
            RandomSamplingAssistant assistant = new RandomSamplingAssistant(n, N, null);

            LongArrayList sample = new LongArrayList((int) n);
            Timer timer = new Timer().start();

            for (long i = 0; i < N; i++)
            {
                if (assistant.sampleNextElement())
                {
                    sample.Add(i);
                }
            }

            timer.stop().display();
            PrintToScreen.WriteLine("sample=" + sample);
            PrintToScreen.WriteLine("Good bye.\n");
        }

        /**
         * Tests the methods of this class.
         * To do benchmarking, comment the lines printing stuff to the console.
         */

        public static void testArraySampling(int n, int N)
        {
            int[] elements = new int[N];
            for (int i = 0; i < N; i++)
            {
                elements[i] = i;
            }

            Timer timer = new Timer().start();

            int[] sample = sampleArray(n, elements);

            timer.stop().display();

            /*
            Console.Write("\nElements = [");
            for (int i=0; i<N-1; i++) Console.Write(elements[i]+", ");
            Console.Write(elements[N-1]);
            PrintToScreen.WriteLine("]");


            Console.Write("\nSample = [");
            for (int i=0; i<n-1; i++) Console.Write(sample[i]+", ");
            Console.Write(sample[n-1]);
            PrintToScreen.WriteLine("]");
            */

            PrintToScreen.WriteLine("Good bye.\n");
        }

        /**
         * Returns whether the next elements of the input sequence shall be sampled (picked) or not.
         * one is chosen from the first block, one from the second, ..., one from the last block.
         * @param acceptList a bitvector which will be filled with <tt>true</tt> where sampling shall occur and <tt>false</tt> where it shall not occur.
         */

        private void xsampleNextElements(BooleanArrayList acceptList)
        {
            // manually inlined
            int Length = acceptList.Size();
            bool[] accept = acceptList.elements();
            for (int i = 0; i < Length; i++)
            {
                if (m_n == 0)
                {
                    accept[i] = false;
                    continue;
                } //reject
                if (m_skip-- > 0)
                {
                    accept[i] = false;
                    continue;
                } //reject

                //accept
                m_n--;
                if (m_bufferPosition < m_buffer.Length - 1)
                {
                    m_skip = m_buffer[m_bufferPosition + 1] - m_buffer[m_bufferPosition++];
                    --m_skip;
                }
                else
                {
                    fetchNextBlock();
                }

                accept[i] = true;
            }
        }
    }
}
