#region

using System;

#endregion

namespace HC.Analytics.Colt.stat
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package cern.jet.stat.quantile;

    //import cern.colt.list.DoubleArrayList;
    /**
     * A buffer holding <tt>double</tt> elements; internally used for computing approximate quantiles.
     */

    [Serializable]
    public class DoubleBufferStat : Buffer
    {
        public bool isSorted;
        public DoubleArrayList values;
        /**
         * This method was created in VisualAge.
         * @param k int
         */

        public DoubleBufferStat(int k)
            : base(k)
        {
            values = new DoubleArrayList(0);
            isSorted = false;
        }

        /**
         * Adds a value to the receiver.
         */

        public void add(double value)
        {
            if (!m_isAllocated)
            {
                allocate(); // lazy buffer allocation can safe memory.
            }
            values.Add(value);
            isSorted = false;
        }

        /**
         * Adds a value to the receiver.
         */

        public void addAllOfFromTo(DoubleArrayList elements, int from, int to)
        {
            if (!m_isAllocated)
            {
                allocate(); // lazy buffer allocation can safe memory.
            }
            values.addAllOfFromTo(elements, from, to);
            isSorted = false;
        }

        /**
         * Allocates the receiver.
         */

        public void allocate()
        {
            m_isAllocated = true;
            values.ensureCapacity(m_k);
        }

        /**
         * Clears the receiver.
         */

        public override void Clear()
        {
            values.Clear();
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return a deep copy of the receiver.
         */

        public Object clone()
        {
            DoubleBufferStat copy = (DoubleBufferStat) base.Clone();
            if (values != null)
            {
                copy.values = copy.values.Copy();
            }
            return copy;
        }

        /**
         * Returns whether the specified element is contained in the receiver.
         */

        public bool contains(double element)
        {
            Sort();
            return values.contains(element);
        }

        /**
         * Returns whether the receiver is empty.
         */

        public override bool isEmpty()
        {
            return values.Size() == 0;
        }

        /**
         * Returns whether the receiver is empty.
         */

        public override bool isFull()
        {
            return values.Size() == m_k;
        }

        /**
         * Returns the number of elements currently needed to store all contained elements.
         * This number usually differs from the results of method <tt>size()</tt>, according to the underlying algorithm.
         */

        public int memory()
        {
            return values.elements().Length;
        }

        /**
         * Returns the rank of a given element within the sorted sequence of the receiver.
         * A rank is the number of elements <= element.
         * Ranks are of the form {1,2,...size()}.
         * If no element is <= element, then the rank is zero.
         * If the element lies in between two contained elements, then uses linear interpolation.
         * @return the rank of the element.
         * @param list cern.colt.list.DoubleArrayList
         * @param element the element to search for
         */

        public double rank(double element)
        {
            Sort();
            return Descriptive.rankInterpolated(values, element);
        }

        /**
         * Returns the number of elements contained in the receiver.
         */

        public override int Size()
        {
            return values.Size();
        }

        /**
         * Sorts the receiver.
         */

        public override void Sort()
        {
            if (!isSorted)
            {
                // IMPORTANT: TO DO : replace mergeSort with quickSort!
                // currently it is mergeSort only for debugging purposes (JDK 1.2 can't be imported into VisualAge).
                values.Sort();
                //values.mergeSort();
                isSorted = true;
            }
        }

        /**
         * Returns a String representation of the receiver.
         */

        public String toString()
        {
            return "k=" + m_k +
                   ", w=" + (weight()) +
                   ", l=" + (level()) +
                   ", size=" + values.Size();
            //", v=" + values.toString();
        }
    }
}
