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
    //package buffer;

    ////import IntArrayList;
    /**
     * Fixed sized (non resizable) streaming buffer connected to a target <tt>IntBufferConsumer</tt> to which data is automatically flushed upon buffer overflow.
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     */

    [Serializable]
    public class IntBuffer : PersistentObject, IntBufferConsumer
    {
        public int m_capacity;
        public int[] m_elements;

        // vars cached for speed
        public IntArrayList m_list;
        public int m_size;
        public IntBufferConsumer m_target;
        /**
         * Constructs and returns a new buffer with the given target.
         * @param target the target to flush to.
         * @param capacity the number of points the buffer shall be capable of holding before overflowing and flushing to the target.
         */

        public IntBuffer(IntBufferConsumer target, int capacity)
        {
            m_target = target;
            m_capacity = capacity;
            m_elements = new int[capacity];
            m_list = new IntArrayList(m_elements);
            m_size = 0;
        }

        /**
         * Adds the specified element to the receiver.
         *
         * @param element the element to add.
         */

        /**
         * Adds all elements of the specified list to the receiver.
         * @param list the list of which all elements shall be added.
         */

        #region IntBufferConsumer Members

        public void addAllOf(IntArrayList list)
        {
            int listSize = list.Size();
            if (m_size + listSize >= m_capacity)
            {
                flush();
            }
            m_target.addAllOf(list);
        }

        #endregion

        public void Add(int element)
        {
            if (m_size == m_capacity)
            {
                flush();
            }
            m_elements[m_size++] = element;
        }

        /**
         * Sets the receiver's size to zero.
         * In other words, forgets about any internally buffered elements.
         */

        public void Clear()
        {
            m_size = 0;
        }

        /**
         * Adds all internally buffered elements to the receiver's target, then resets the current buffer size to zero.
         */

        public void flush()
        {
            if (m_size > 0)
            {
                m_list.setSize(m_size);
                m_target.addAllOf(m_list);
                m_size = 0;
            }
        }
    }
}
