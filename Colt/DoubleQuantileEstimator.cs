#region

using System;
using HC.Analytics.Colt.stat;

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
    /**
      * The abstract base class for approximate quantile finders computing quantiles over a sequence of <tt>double</tt> elements.
      */
    //abstract class ApproximateDoubleQuantileFinder : Object : DoubleQuantileFinder {
    [Serializable]
    public abstract class DoubleQuantileEstimator : PersistentObject, DoubleQuantileFinder
    {
        public DoubleBufferSet bufferSet;
        public DoubleBufferStat currentBufferToFill;
        public int totalElementsFilled;
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Adds a value to the receiver.
         * @param value the value to add.
         */

        #region DoubleQuantileFinder Members

        public void Add(double value)
        {
            totalElementsFilled++;
            if (!sampleNextElement())
            {
                return;
            }

            //PrintToScreen.WriteLine("adding "+value);

            if (currentBufferToFill == null)
            {
                if (bufferSet._getFirstEmptyBuffer() == null)
                {
                    collapse();
                }
                newBuffer();
            }

            currentBufferToFill.add(value);
            if (currentBufferToFill.isFull())
            {
                currentBufferToFill = null;
            }
        }

        /**
         * Adds all values of the specified list to the receiver.
         * @param values the list of which all values shall be added.
         */

        public void addAllOf(DoubleArrayList values)
        {
            addAllOfFromTo(values, 0, values.Size() - 1);
        }

        /**
         * Adds the part of the specified list between indexes <tt>from</tt> (inclusive) and <tt>to</tt> (inclusive) to the receiver.
         *
         * @param values the list of which elements shall be added.
         * @param from the index of the first element to be added (inclusive).
         * @param to the index of the last element to be added (inclusive).
         */

        public void addAllOfFromTo(DoubleArrayList values, int from, int to)
        {
            /*
            // the obvious version, but we can do quicker...
            double[] theValues = values.elements();
            int theSize=values.Size();
            for (int i=0; i<theSize; ) Add(theValues[i++]);
            */

            double[] valuesToAdd = values.elements();
            int k = bufferSet.k();
            int bufferSize = k;
            double[] bufferValues = null;
            if (currentBufferToFill != null)
            {
                bufferValues = currentBufferToFill.values.elements();
                bufferSize = currentBufferToFill.Size();
            }

            for (int i = from - 1; ++i <= to;)
            {
                if (sampleNextElement())
                {
                    if (bufferSize == k)
                    {
                        // full
                        if (bufferSet._getFirstEmptyBuffer() == null)
                        {
                            collapse();
                        }
                        newBuffer();
                        if (!currentBufferToFill.m_isAllocated)
                        {
                            currentBufferToFill.allocate();
                        }
                        currentBufferToFill.isSorted = false;
                        bufferValues = currentBufferToFill.values.elements();
                        bufferSize = 0;
                    }

                    bufferValues[bufferSize++] = valuesToAdd[i];
                    if (bufferSize == k)
                    {
                        // full
                        currentBufferToFill.values.setSize(bufferSize);
                        currentBufferToFill = null;
                    }
                }
            }
            if (currentBufferToFill != null)
            {
                currentBufferToFill.values.setSize(bufferSize);
            }

            totalElementsFilled += to - from + 1;
        }

        /**
         * Not yet commented.
         */

        /**
         * Removes all elements from the receiver.  The receiver will
         * be empty after this call returns, and its memory requirements will be close to zero.
         */

        public void Clear()
        {
            totalElementsFilled = 0;
            currentBufferToFill = null;
            bufferSet.Clear();
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return a deep copy of the receiver.
         */

        public new Object Clone()
        {
            DoubleQuantileEstimator copy = (DoubleQuantileEstimator) base.Clone();
            if (bufferSet != null)
            {
                copy.bufferSet = (DoubleBufferSet) copy.bufferSet.Clone();
                if (currentBufferToFill != null)
                {
                    int index = new ObjectArrayList(bufferSet.m_buffers).IndexOf(currentBufferToFill, true);
                    copy.currentBufferToFill = copy.bufferSet.m_buffers[index];
                }
            }
            return copy;
        }

        /**
         * Not yet commented.
         */

        /**
         * Applies a procedure to each element of the receiver, if any.
         * Iterates over the receiver in no particular order.
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        public bool forEach(DoubleProcedure procedure)
        {
            return bufferSet.forEach(procedure);
        }

        /**
         * Returns the number of elements currently needed to store all contained elements.
         * This number usually differs from the results of method <tt>Size()</tt>, according to the underlying datastructure.
         */

        public long memory()
        {
            return bufferSet.memory();
        }

        /**
         * Not yet commented.
         */
        /**
         * Returns how many percent of the elements contained in the receiver are <tt>&lt;= element</tt>.
         * Does linear interpolation if the element is not contained but lies in between two contained elements.
         *
         * @param the element to search for.
         * @return the percentage <tt>p</tt> of elements <tt>&lt;= element</tt> (<tt>0.0 &lt;= p &lt;=1.0)</tt>.
         */

        public double phi(double element)
        {
            return bufferSet.phi(element);
        }

        public DoubleArrayList quantileElements(DoubleArrayList phis)
        {
            /*
            //check parameter
            DoubleArrayList sortedPhiList = phis.Copy();
            sortedPhiList.Sort();
            if (! phis.Equals(sortedPhiList)) {
                throw new ArgumentException("Phis must be sorted ascending.");
            }
            */

            //PrintToScreen.WriteLine("starting to augment missing values, if necessary...");

            phis = preProcessPhis(phis);

            long[] triggerPositions = new long[phis.Size()];
            long totalSize = bufferSet.totalSize();
            for (int i = phis.Size(); --i >= 0;)
            {
                triggerPositions[i] = Utils.epsilonCeiling(phis.get(i)*totalSize) - 1;
            }

            //PrintToScreen.WriteLine("triggerPositions="+Arrays.ToString(triggerPositions));
            //PrintToScreen.WriteLine("starting to determine quantiles...");
            //PrintToScreen.WriteLine(bufferSet);

            DoubleBufferStat[] fullBuffers = bufferSet._getFullOrPartialBuffers();
            double[] quantileElements = new double[phis.Size()];

            //do the main work: determine values at given positions in sorted sequence
            return new DoubleArrayList(bufferSet.getValuesAtPositions(fullBuffers, triggerPositions));
        }

        public long Size()
        {
            return totalElementsFilled;
        }

        public long totalMemory()
        {
            return bufferSet.b()*bufferSet.k();
        }

        #endregion

        public DoubleBufferStat[] buffersToCollapse()
        {
            int minLevel = bufferSet._getMinLevelOfFullOrPartialBuffers();
            return bufferSet._getFullOrPartialBuffersWithLevel(minLevel);
        }

        public void collapse()
        {
            DoubleBufferStat[] toCollapse = buffersToCollapse();
            DoubleBufferStat outputBuffer = bufferSet.collapse(toCollapse);

            int minLevel = toCollapse[0].level();
            outputBuffer.level(minLevel + 1);

            postCollapse(toCollapse);
        }

        /**
         * Returns whether the specified element is contained in the receiver.
         */

        public bool contains(double element)
        {
            return bufferSet.contains(element);
        }

        public abstract void newBuffer();

        /**
         * Not yet commented.
         */
        public abstract void postCollapse(DoubleBufferStat[] toCollapse);
        /**
         * Default implementation does nothing.
         */

        public DoubleArrayList preProcessPhis(DoubleArrayList phis)
        {
            return phis;
        }

        /**
         * Computes the specified quantile elements over the values previously added.
         * @param phis the quantiles for which elements are to be computed. Each phi must be in the interval [0.0,1.0]. <tt>phis</tt> must be sorted ascending.
         * @return the approximate quantile elements.
         */

        /**
         * Not yet commented.
         */
        public abstract bool sampleNextElement();
        /**
         * Initializes the receiver
         */

        public void setUp(int b, int k)
        {
            if (!(b >= 2 && k >= 1))
            {
                throw new ArgumentException("Assertion: b>=2 && k>=1");
            }
            bufferSet = new DoubleBufferSet(b, k);
            Clear();
        }

        /**
         * Returns the number of elements currently contained in the receiver (identical to the number of values added so far).
         */

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            string s = ToString();
            s = s.Substring(s.LastIndexOf('.') + 1);
            int b = bufferSet.b();
            int k = bufferSet.k();
            return s + "(mem=" + memory() + ", b=" + b + ", k=" + k + ", size=" + Size() + ", totalSize=" +
                   bufferSet.totalSize() + ")";
        }

        /**
         * Returns the number of elements currently needed to store all contained elements.
         * This number usually differs from the results of method <tt>Size()</tt>, according to the underlying datastructure.
         */
    }
}
