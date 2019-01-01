#region

using System;

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

    /**
     * A buffer holding elements; internally used for computing approximate quantiles.
     */

    [Serializable]
    public abstract class Buffer : PersistentObject
    {
        public bool m_isAllocated;
        public int m_k;
        public int m_level;
        public int m_weight;
        /**
         * This method was created in VisualAge.
         * @param k int
         */

        public Buffer(int k)
        {
            m_k = k;
            m_weight = 1;
            m_level = 0;
            m_isAllocated = false;
        }

        /**
         * Clears the receiver.
         */
        public abstract void Clear();
        /**
         * Returns whether the receiver is already allocated.
         */

        public bool isAllocated()
        {
            return m_isAllocated;
        }

        /**
         * Returns whether the receiver is empty.
         */
        public abstract bool isEmpty();
        /**
         * Returns whether the receiver is empty.
         */
        public abstract bool isFull();
        /**
         * Returns whether the receiver is partial.
         */

        public bool isPartial()
        {
            return !(isEmpty() || isFull());
        }

        /**
         * Returns whether the receiver's level.
         */

        public int level()
        {
            return m_level;
        }

        /**
         * Sets the receiver's level.
         */

        public void level(int level)
        {
            m_level = level;
        }

        /**
         * Returns the number of elements contained in the receiver.
         */
        public abstract int Size();
        /**
         * Sorts the receiver.
         */
        public abstract void Sort();
        /**
         * Returns whether the receiver's weight.
         */

        public int weight()
        {
            return m_weight;
        }

        /**
         * Sets the receiver's weight.
         */

        public void weight(int weight)
        {
            m_weight = weight;
        }
    }
}
