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
////package buffer;

////import DoubleArrayList;
/**
 * Fixed sized (non resizable) streaming buffer connected to a target <tt>DoubleBuffer2DConsumer</tt> to which data is automatically flushed upon buffer overflow.
 *
 * @author wolfgang.hoschek@cern.ch
 * @version 1.0, 09/24/99
 */

    [Serializable]
    public class DoubleBuffer2D : PersistentObject, DoubleBuffer2DConsumer
    {
        public int m_capacity;
        public int m_size;
        public DoubleBuffer2DConsumer m_target;
        public double[] m_xElements;

        // vars cached for speed
        public DoubleArrayList m_xList;
        public double[] m_yElements;
        public DoubleArrayList m_yList;
/**
 * Constructs and returns a new buffer with the given target.
 * @param target the target to flush to.
 * @param capacity the number of points the buffer shall be capable of holding before overflowing and flushing to the target.
 */

        public DoubleBuffer2D(DoubleBuffer2DConsumer target, int capacity)
        {
            m_target = target;
            m_capacity = capacity;
            m_xElements = new double[capacity];
            m_yElements = new double[capacity];
            m_xList = new DoubleArrayList(m_xElements);
            m_yList = new DoubleArrayList(m_yElements);
            m_size = 0;
        }

/**
 * Adds the specified point (x,y) to the receiver.
 *
 * @param x the x-coordinate of the point to add.
 * @param y the y-coordinate of the point to add.
 */

/**
 * Adds all specified points (x,y) to the receiver.
 * @param x the x-coordinates of the points to add.
 * @param y the y-coordinates of the points to add.
 */

        #region DoubleBuffer2DConsumer Members

        public void addAllOf(DoubleArrayList x, DoubleArrayList y)
        {
            int listSize = x.Size();
            if (m_size + listSize >= m_capacity)
            {
                flush();
            }
            m_target.addAllOf(x, y);
        }

        #endregion

        public void Add(double x, double y)
        {
            if (m_size == m_capacity)
            {
                flush();
            }
            m_xElements[m_size] = x;
            m_yElements[m_size++] = y;
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
 * Adds all internally buffered points to the receiver's target, then resets the current buffer size to zero.
 */

        public void flush()
        {
            if (m_size > 0)
            {
                m_xList.setSize(m_size);
                m_yList.setSize(m_size);
                m_target.addAllOf(m_xList, m_yList);
                m_size = 0;
            }
        }
    }
}
