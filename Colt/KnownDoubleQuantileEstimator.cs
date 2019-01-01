#region

using System;
using HC.Analytics.Colt.stat;
using HC.Analytics.Mathematics;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

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

    ////import DoubleArrayList;
    ////import Arithmetic;
    ////import RngWrapper;
    ////import RandomSamplingAssistant;
    /**
     * Approximate quantile finding algorithm for known <tt>N</tt> requiring only one pass and little main memory; computes quantiles over a sequence of <tt>double</tt> elements.
     *
     * <p>Needs as input the following parameters:<p>
     * <dt>1. <tt>N</tt> - the number of values of the data sequence over which quantiles are to be determined.
     * <dt>2. <tt>quantiles</tt> - the number of quantiles to be computed.
     * <dt>3. <tt>epsilon</tt> - the allowed approximation error on quantiles. The approximation guarantee of this algorithm is explicit.
     *
     * <p>It is also possible to couple the approximation algorithm with random sampling to further reduce memory requirements. 
     * With sampling, the approximation guarantees are explicit but probabilistic, i.e. they apply with respect to a (user controlled) confidence parameter "delta".
     *
     * <dt>4. <tt>delta</tt> - the probability allowed that the approximation error fails to be smaller than epsilon. Set <tt>delta</tt> to zero for explicit non probabilistic guarantees.
     *
     * You usually don't instantiate quantile finders by using the constructor. Instead use the factory <tt>QuantileFinderFactor</tt> to do so. It will set up the right parametrization for you.
     * 
     * <p>After Gurmeet Singh Manku, Sridhar Rajagopalan and Bruce G. Lindsay, 
     * Approximate Medians and other Quantiles in One Pass and with Limited Memory,
     * Proc. of the 1998 ACM SIGMOD Int. Conf. on Management of Data,
     * Paper available <A HREF="http://www-cad.eecs.berkeley.edu/~manku"> here</A>.
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     * @see QuantileFinderFactory
     * @see UnknownApproximateDoubleQuantileFinder
     */

    [Serializable]
    public class KnownDoubleQuantileEstimator : DoubleQuantileEstimator
    {
        public double m_beta; //correction factor for phis
        public long m_N; // see method sampleNextElement()

        public RandomSamplingAssistant m_samplingAssistant;
        public double m_samplingRate; // see method sampleNextElement()
        public bool m_weHadMoreThanOneEmptyBuffer;
        /**
         * Constructs an approximate quantile finder with b buffers, each having k elements.
         * @param b the number of buffers
         * @param k the number of elements per buffer
         * @param N the total number of elements over which quantiles are to be computed.
         * @param samplingRate 1.0 --> all elements are consumed. 10.0 --> Consumes one random element from successive blocks of 10 elements each. Etc.
         * @param generator a uniform random number generator.
         */

        public KnownDoubleQuantileEstimator(int b, int k, long N, double samplingRate, RngWrapper generator)
        {
            m_samplingRate = samplingRate;
            m_N = N;

            if (samplingRate <= 1.0)
            {
                m_samplingAssistant = null;
            }
            else
            {
                m_samplingAssistant = new RandomSamplingAssistant(
                    Arithmetic.Floor(N/samplingRate), N, generator);
            }

            setUp(b, k);
            Clear();
        }

        /**
         * @param infinities the number of infinities to fill.
         * @param buffer the buffer into which the infinities shall be filled.
         */

        public void addInfinities(int missingInfinities, DoubleBufferStat buffer)
        {
            RandomSamplingAssistant oldAssistant = m_samplingAssistant;
            m_samplingAssistant = null; // switch off sampler
            //double[] infinities = new double[missingInfinities];

            bool even = true;
            for (int i = 0; i < missingInfinities; i++)
            {
                if (even)
                {
                    buffer.values.Add(Double.MaxValue);
                }
                else
                {
                    buffer.values.Add(-Double.MaxValue);
                }

                //if (even) {infinities[i]=Double.MaxValue;}
                //else	  {infinities[i]=-Double.MaxValue;}

                //if (even) {Add(Double.MaxValue);}
                //else	  {Add(-Double.MaxValue);}
                even = !even;
            }

            //buffer.values.addAllOfFromTo(new DoubleArrayList(infinities),0,missingInfinities-1);

            //totalElementsFilled -= infinities;

            m_samplingAssistant = oldAssistant; // switch on sampler again
        }

        /**
         * Not yet commented.
         */

        public new DoubleBufferStat[] buffersToCollapse()
        {
            int minLevel = bufferSet._getMinLevelOfFullOrPartialBuffers();
            return bufferSet._getFullOrPartialBuffersWithLevel(minLevel);
        }

        /**
         * Removes all elements from the receiver.  The receiver will
         * be empty after this call returns, and its memory requirements will be close to zero.
         */

        public new void Clear()
        {
            base.Clear();
            m_beta = 1.0;
            m_weHadMoreThanOneEmptyBuffer = false;
            //setSamplingRate(samplingRate,N);

            RandomSamplingAssistant assist = m_samplingAssistant;
            if (assist != null)
            {
                m_samplingAssistant = new RandomSamplingAssistant(Arithmetic.Floor(m_N/m_samplingRate), m_N,
                                                                  assist.getRandomGenerator());
            }
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return a deep copy of the receiver.
         */

        public new Object Clone()
        {
            KnownDoubleQuantileEstimator copy = (KnownDoubleQuantileEstimator) base.Clone();
            if (m_samplingAssistant != null)
            {
                copy.m_samplingAssistant = (RandomSamplingAssistant) copy.m_samplingAssistant.Clone();
            }
            return copy;
        }

        /**
         * Not yet commented.
         */

        public override void newBuffer()
        {
            int numberOfEmptyBuffers = bufferSet._getNumberOfEmptyBuffers();
            //DoubleBuffer[] emptyBuffers = bufferSet._getEmptyBuffers();
            if (numberOfEmptyBuffers == 0)
            {
                throw new HCException("Oops, no empty buffer.");
            }

            currentBufferToFill = bufferSet._getFirstEmptyBuffer();
            if (numberOfEmptyBuffers == 1 && !m_weHadMoreThanOneEmptyBuffer)
            {
                currentBufferToFill.level(bufferSet._getMinLevelOfFullOrPartialBuffers());
            }
            else
            {
                m_weHadMoreThanOneEmptyBuffer = true;
                currentBufferToFill.level(0);
                /*
                for (int i=0; i<emptyBuffers.Length; i++) {
                    emptyBuffers[i].level = 0;			
                }
                */
            }
            //currentBufferToFill.state = DoubleBuffer.PARTIAL;
            currentBufferToFill.weight(1);
        }

        /**
         * Not yet commented.
         */

        public override void postCollapse(DoubleBufferStat[] toCollapse)
        {
            m_weHadMoreThanOneEmptyBuffer = false;
        }

        /**
         */

        public new DoubleArrayList preProcessPhis(DoubleArrayList phis)
        {
            if (m_beta > 1.0)
            {
                phis = phis.Copy();
                for (int i = phis.Size(); --i >= 0;)
                {
                    phis.set(i, (2*phis.get(i) + m_beta - 1)/(2*m_beta));
                }
            }
            return phis;
        }

        /**
         * Computes the specified quantile elements over the values previously added.
         * @param phis the quantiles for which elements are to be computed. Each phi must be in the interval [0.0,1.0]. <tt>phis</tt> must be sorted ascending.
         * @return the approximate quantile elements.
         */

        public new DoubleArrayList quantileElements(DoubleArrayList phis)
        {
            /*
            * The KNOWN quantile finder reads off quantiles from FULL buffers only.
            * Since there might be a partially full buffer, this method first satisfies this constraint by temporarily filling a few +infinity, -infinity elements to make up a full block.
            * This is in full conformance with the explicit approximation guarantees.
            *
            * For those of you working on online apps:
            * The approximation guarantees are given for computing quantiles AFTER N elements have been filled, not for intermediate displays.
            * If you have one thread filling and another thread displaying concurrently, you will note that in the very beginning the infinities will dominate the display.
            * This could confuse users, because, of course, they don't expect any infinities, even if they "disappear" after a short while.
            * To prevent panic exclude phi's close to zero or one in the early phases of processing.
            */
            DoubleBufferStat partial = bufferSet._getPartialBuffer();
            int missingValues = 0;
            if (partial != null)
            {
                // any auxiliary infinities needed?
                missingValues = bufferSet.k() - partial.Size();
                if (missingValues <= 0)
                {
                    throw new HCException("Oops! illegal missing values.");
                }

                //PrintToScreen.WriteLine("adding "+missingValues+" infinity elements...");
                addInfinities(missingValues, partial);

                //determine beta (N + Infinity values = beta * N)
                m_beta = (totalElementsFilled + missingValues)/(double) totalElementsFilled;
            }
            else
            {
                m_beta = 1.0;
            }

            DoubleArrayList quantileElements = base.quantileElements(phis);

            // restore state we were in before.
            // remove the temporarily added infinities.
            if (partial != null)
            {
                removeInfinitiesFrom(missingValues, partial);
            }

            // now you can continue filling the remaining values, if any.
            return quantileElements;
        }

        /**
         * Reading off quantiles requires to fill some +infinity, -infinity values to make a partial buffer become full.
         *
         * This method removes the infinities which were previously temporarily added to a partial buffer.
         * Removing them is necessary if we want to continue filling more elements.
         * Precondition: the buffer is sorted ascending.
         * @param infinities the number of infinities previously filled.
         * @param buffer the buffer into which the infinities were filled.
         */

        public void removeInfinitiesFrom(int infinities, DoubleBufferStat buffer)
        {
            int plusInf = 0;
            int minusInf = 0;
            // count them (this is not very clever but it's safe)
            bool even = true;
            for (int i = 0; i < infinities; i++)
            {
                if (even)
                {
                    plusInf++;
                }
                else
                {
                    minusInf++;
                }
                even = !even;
            }

            buffer.values.removeFromTo(buffer.Size() - plusInf, buffer.Size() - 1);
            buffer.values.removeFromTo(0, minusInf - 1);
            //totalElementsFilled -= infinities;
        }

        /**
         * Not yet commented.
         */

        public override bool sampleNextElement()
        {
            if (m_samplingAssistant == null)
            {
                return true;
            }

            /*
             * This is a KNOWN N quantile finder!
             * One should not try to fill more than N elements,
             * because otherwise we can't give explicit approximation guarantees anymore.
             * Use an UNKNOWN quantile finder instead if your app may fill more than N elements.
             *
             * However, to make this class meaningful even under wired use cases, we actually do allow to fill more than N elements (without explicit approx. guarantees, of course).
             * Normally, elements beyond N will not get sampled because the sampler is exhausted. 
             * Therefore the histogram will no more change no matter how much you fill.
             * This might not be what the user expects.
             * Therefore we use a new (unexhausted) sampler with the same parametrization.
             *
             * If you want this class to ignore any elements beyong N, then comment the following line.
             */
            //if ((totalElementsFilled-1) % N == 0) setSamplingRate(samplingRate, N); // delete if appropriate

            return m_samplingAssistant.sampleNextElement();
        }
    }
}
