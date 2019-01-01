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
    ////import RngWrapper;
    /**
     * Conveniently computes a stable subsequence of elements from a given input sequence;
     * Picks (samples) exactly one random element from successive blocks of <tt>weight</tt> input elements each.
     * For example, if weight==2 (a block is 2 elements), and the input is 5*2=10 elements long, then picks 5 random elements from the 10 elements such that
     * one element is randomly picked from the first block, one element from the second block, ..., one element from the last block.
     * weight == 1.0 --> all elements are picked (sampled). weight == 10.0 --> Picks one random element from successive blocks of 10 elements each. Etc.
     * The subsequence is guaranteed to be <i>stable</i>, i.e. elements never change position relative to each other.
     *
     * @author  wolfgang.hoschek@cern.ch
     * @version 1.0, 02/05/99
     */

    [Serializable]
    public class WeightedRandomSampler : PersistentObject
    {
        //[Serializable]public class BlockedRandomSampler : Object : java.io.Serializable {
        private static int UNDEFINED = -1;
        public RngWrapper m_generator;
        public int m_nextSkip;
        public int m_nextTriggerPos;
        public int m_skip;
        public int m_weight;
        /**
         * Calls <tt>BlockedRandomSampler(1,null)</tt>.
         */

        public WeightedRandomSampler()
            : this(1, null)
        {
        }

        /**
         * Chooses exactly one random element from successive blocks of <tt>weight</tt> input elements each.
         * For example, if weight==2, and the input is 5*2=10 elements long, then chooses 5 random elements from the 10 elements such that
         * one is chosen from the first block, one from the second, ..., one from the last block.
         * weight == 1.0 --> all elements are consumed (sampled). 10.0 --> Consumes one random element from successive blocks of 10 elements each. Etc.
         * @param weight the weight.
         * @param randomGenerator a random number generator. Set this parameter to <tt>null</tt> to use the default random number generator.
         */

        public WeightedRandomSampler(
            int weight,
            RngWrapper randomGenerator)
        {
            if (randomGenerator == null)
            {
                randomGenerator = new RngWrapper();
            }
            m_generator = randomGenerator;
            setWeight(weight);
        }

        /**
         * Returns a deep copy of the receiver.
         */

        public new Object Clone()
        {
            WeightedRandomSampler copy = (WeightedRandomSampler) base.Clone();
            copy.m_generator = (RngWrapper) m_generator.Clone();
            return copy;
        }

        /**
         * Not yet commented.
         * @param weight int
         */

        public int getWeight()
        {
            return m_weight;
        }

        /**
         * Chooses exactly one random element from successive blocks of <tt>weight</tt> input elements each.
         * For example, if weight==2, and the input is 5*2=10 elements long, then chooses 5 random elements from the 10 elements such that
         * one is chosen from the first block, one from the second, ..., one from the last block.
         * @return <tt>true</tt> if the next element shall be sampled (picked), <tt>false</tt> otherwise.
         */

        public bool sampleNextElement()
        {
            if (m_skip > 0)
            {
                //reject
                m_skip--;
                return false;
            }

            if (m_nextTriggerPos == UNDEFINED)
            {
                if (m_weight == 1)
                {
                    m_nextTriggerPos = 0; // tuned for speed
                }
                else
                {
                    m_nextTriggerPos = m_generator.NextInt(0, m_weight - 1);
                }

                m_nextSkip = m_weight - 1 - m_nextTriggerPos;
            }

            if (m_nextTriggerPos > 0)
            {
                //reject
                m_nextTriggerPos--;
                return false;
            }

            //accept
            m_nextTriggerPos = UNDEFINED;
            m_skip = m_nextSkip;

            return true;
        }

        /**
         * Not yet commented.
         * @param weight int
         */

        public void setWeight(int weight)
        {
            if (weight < 1)
            {
                throw new ArgumentException("bad weight");
            }
            m_weight = weight;
            m_skip = 0;
            m_nextTriggerPos = UNDEFINED;
            m_nextSkip = 0;
        }

        /**
         * Not yet commented.
         */

        public static void test(int weight, int size)
        {
            WeightedRandomSampler sampler = new WeightedRandomSampler();
            sampler.setWeight(weight);

            IntArrayList sample = new IntArrayList();
            for (int i = 0; i < size; i++)
            {
                if (sampler.sampleNextElement())
                {
                    sample.Add(i);
                }
            }

            PrintToScreen.WriteLine("Sample = " + sample);
        }

        /**
         * Chooses exactly one random element from successive blocks of <tt>weight</tt> input elements each.
         * For example, if weight==2, and the input is 5*2=10 elements long, then chooses 5 random elements from the 10 elements such that
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
                if (m_skip > 0)
                {
                    //reject
                    m_skip--;
                    accept[i] = false;
                    continue;
                }

                if (m_nextTriggerPos == UNDEFINED)
                {
                    if (m_weight == 1)
                    {
                        m_nextTriggerPos = 0; // tuned for speed
                    }
                    else
                    {
                        m_nextTriggerPos = m_generator.NextInt(0, m_weight - 1);
                    }

                    m_nextSkip = m_weight - 1 - m_nextTriggerPos;
                }

                if (m_nextTriggerPos > 0)
                {
                    //reject
                    m_nextTriggerPos--;
                    accept[i] = false;
                    continue;
                }

                //accept
                m_nextTriggerPos = UNDEFINED;
                m_skip = m_nextSkip;
                accept[i] = true;
            }
        }
    }
}
