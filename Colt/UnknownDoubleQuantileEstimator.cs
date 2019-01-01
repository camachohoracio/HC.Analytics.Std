#region

using System;
using System.Text;
using HC.Analytics.Colt.CustomImplementations;
using HC.Analytics.Colt.stat;
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
    ////import ObjectArrayList;
    ////import RngWrapper;
    ////import sampling.WeightedRandomSampler;
    /**
     * Approximate quantile finding algorithm for unknown <tt>N</tt> requiring only one pass and little main memory; computes quantiles over a sequence of <tt>double</tt> elements.
     * This algorithm requires at most two times the memory of a corresponding approx. quantile finder knowing <tt>N</tt>.
     *
     * <p>Needs as input the following parameters:<p>
     * <dt>1. <tt>quantiles</tt> - the number of quantiles to be computed.
     * <dt>2. <tt>epsilon</tt> - the allowed approximation error on quantiles. The approximation guarantee of this algorithm is explicit.
     *
     * <p>It is also possible to couple the approximation algorithm with random sampling to further reduce memory requirements. 
     * With sampling, the approximation guarantees are explicit but probabilistic, i.e. they apply with respect to a (user controlled) confidence parameter "delta".
     *
     * <dt>3. <tt>delta</tt> - the probability allowed that the approximation error fails to be smaller than epsilon. Set <tt>delta</tt> to zero for explicit non probabilistic guarantees.
     *
     * You usually don't instantiate quantile finders by using the constructor. Instead use the factory <tt>QuantileFinderFactor</tt> to do so. It will set up the right parametrization for you.
     * 
     * <p>After Gurmeet Singh Manku, Sridhar Rajagopalan and Bruce G. Lindsay,
     * RngWrapper Sampling Techniques for Space Efficient Online Computation of Order Statistics of Large Datasets.
     * Accepted for Proc. of the 1999 ACM SIGMOD Int. Conf. on Management of Data,
     * Paper (soon) available <A HREF="http://www-cad.eecs.berkeley.edu/~manku"> here</A>.
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     * @see QuantileFinderFactory
     * @see KnownApproximateDoubleQuantileFinder
     */

    [Serializable]
    public class UnknownDoubleQuantileEstimator : DoubleQuantileEstimator
    {
        public int m_currentTreeHeight;
        public double m_precomputeEpsilon;
        public WeightedRandomSampler m_sampler;
        public int m_treeHeightStartingSampling;
        /**
         * Constructs an approximate quantile finder with b buffers, each having k elements.
         * @param b the number of buffers
         * @param k the number of elements per buffer
         * @param h the tree height at which sampling shall start.
         * @param precomputeEpsilon the epsilon for which quantiles shall be precomputed; set this value <=0.0 if nothing shall be precomputed.
         * @param generator a uniform random number generator.
         */

        public UnknownDoubleQuantileEstimator(int b, int k, int h, double precomputeEpsilon,
                                              RngWrapper generator)
        {
            m_sampler = new WeightedRandomSampler(1, generator);
            setUp(b, k);
            m_treeHeightStartingSampling = h;
            m_precomputeEpsilon = precomputeEpsilon;
            Clear();
        }

        /**
         * Not yet commented.
         */

        public new DoubleBufferStat[] buffersToCollapse()
        {
            DoubleBufferStat[] fullBuffers = bufferSet._getFullOrPartialBuffers();

            sortAscendingByLevel(fullBuffers);

            // if there is only one buffer at the lowest level, then increase its level so that there are at least two at the lowest level.
            int minLevel = fullBuffers[1].level();
            if (fullBuffers[0].level() < minLevel)
            {
                fullBuffers[0].level(minLevel);
            }

            return bufferSet._getFullOrPartialBuffersWithLevel(minLevel);
        }

        /**
         * Removes all elements from the receiver.  The receiver will
         * be empty after this call returns, and its memory requirements will be close to zero.
         */

        public new void Clear()
        {
            base.Clear();
            m_currentTreeHeight = 1;
            m_sampler.setWeight(1);
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return a deep copy of the receiver.
         */

        public new Object Clone()
        {
            UnknownDoubleQuantileEstimator copy = (UnknownDoubleQuantileEstimator) base.Clone();
            if (m_sampler != null)
            {
                copy.m_sampler = (WeightedRandomSampler) copy.m_sampler.Clone();
            }
            return copy;
        }

        /**
         * Not yet commented.
         */

        public override void newBuffer()
        {
            currentBufferToFill = bufferSet._getFirstEmptyBuffer();
            if (currentBufferToFill == null)
            {
                throw new HCException("Oops, no empty buffer.");
            }

            currentBufferToFill.level(m_currentTreeHeight - 1);
            currentBufferToFill.weight(m_sampler.getWeight());
        }

        /**
         * Not yet commented.
         */

        public override void postCollapse(DoubleBufferStat[] toCollapse)
        {
            if (toCollapse.Length == bufferSet.b())
            {
                //delta for unknown finder
                m_currentTreeHeight++;
                if (m_currentTreeHeight >= m_treeHeightStartingSampling)
                {
                    m_sampler.setWeight(m_sampler.getWeight()*2);
                }
            }
        }

        /**
         * Computes the specified quantile elements over the values previously added.
         * @param phis the quantiles for which elements are to be computed. Each phi must be in the interval (0.0,1.0]. <tt>phis</tt> must be sorted ascending.
         * @return the approximate quantile elements.
         */

        public new DoubleArrayList quantileElements(DoubleArrayList phis)
        {
            if (m_precomputeEpsilon <= 0.0)
            {
                return base.quantileElements(phis);
            }

            int quantilesToPrecompute = (int) Utils.epsilonCeiling(1.0/m_precomputeEpsilon);
            /*
            if (phis.Size() > quantilesToPrecompute) {
                // illegal use case!
                // we compute results, but loose explicit approximation guarantees.
                return base.quantileElements(phis);
            }
            */

            //select that quantile from the precomputed set that corresponds to a position closest to phi.
            phis = phis.Copy();
            double e = m_precomputeEpsilon;
            for (int index = phis.Size(); --index >= 0;)
            {
                double phi = phis.get(index);
                int i = (int) Math.Round(((2.0*phi/e) - 1.0)/2.0); // finds closest
                i = Math.Min(quantilesToPrecompute - 1, Math.Max(0, i));
                double augmentedPhi = (e/2.0)*(1 + 2*i);
                phis.set(index, augmentedPhi);
            }

            return base.quantileElements(phis);
        }

        /**
         * Not yet commented.
         */

        public override bool sampleNextElement()
        {
            return m_sampler.sampleNextElement();
        }

        /**
         * To do. This could faster be done without sorting (min and second min).
         */

        public static void sortAscendingByLevel(DoubleBufferStat[] fullBuffers)
        {
            new ObjectArrayList(fullBuffers).quickSortFromTo(
                0,
                fullBuffers.Length - 1,
                new Comparer_());
        }

        /**
         * Returns a string representation of the receiver.
         */

        public new string ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());

            //buf.setLength(buf.Length - 1);

            return buf + ", h=" + m_currentTreeHeight + ", hStartSampling=" + m_treeHeightStartingSampling +
                   ", precomputeEpsilon=" + m_precomputeEpsilon + ")";
        }
    }
}
