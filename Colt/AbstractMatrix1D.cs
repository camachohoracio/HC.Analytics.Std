#region

using System;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright © 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package impl;

    /**
    Abstract base class for 1-d matrices (aka <i>vectors</i>) holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Note that this implementation is not .</b>

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class AbstractMatrix1D : AbstractMatrix
    {
        /** the number of cells this matrix (view) has */
        public int m_intSize;


        /** the index of the first element */

        /** the number of indexes between any two elements, i.e. <tt>index(i+1) - index(i)</tt>. */
        public int m_intStride;
        public int m_intZero;

        /** 
         * Indicates non-flipped state (flip==1) or flipped state (flip==-1).
         * see _setFlip() for further info.
         */
        //public int flip;

        /** 
         * Indicates non-flipped state or flipped state.
         * see _setFlip() for further info.
         */
        //public int flipMask;

        // isNoView implies: offset==0, stride==1
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
         * Default implementation. Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */

        public int _offset(int absRank)
        {
            return absRank;
        }

        /**
         * Returns the absolute rank of the given relative rank. 
         *
         * @param  rank   the relative rank of the element.
         * @return the absolute rank of the element.
         */

        public int _rank(int rank)
        {
            return m_intZero + rank*m_intStride;
            //return zero + ((rank+flipMask)^flipMask);
            //return zero + rank*flip; // slower
        }

        /**
         * Sanity check for operations requiring an index to be within bounds.
         * @throws HCException if <tt>index < 0 || index >= Size()</tt>.
         */

        public void checkIndex(int index)
        {
            if (index < 0 || index >= m_intSize)
            {
                throw new HCException("Attempted to access " + toStringShort() + " at index=" + index);
            }
        }

        /**
         * Checks whether indexes are legal and throws an exception, if necessary.
         * @throws HCException if <tt>! (0 <= indexes[i] < Size())</tt> for any i=0..indexes.Length-1.
         */

        public void checkIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= m_intSize)
                {
                    checkIndex(index);
                }
            }
        }

        /**
         * Checks whether the receiver contains the given range and throws an exception, if necessary.
         * @throws	HCException if <tt>index<0 || index+width>Size()</tt>.
         */

        public void checkRange(int index, int width)
        {
            if (index < 0 || index + width > m_intSize)
            {
                throw new HCException("index: " + index + ", width: " + width + ", size=" + m_intSize);
            }
        }

        /**
         * Sanity check for operations requiring two matrices with the same size.
         * @throws ArgumentException if <tt>Size() != B.Size()</tt>.
         */

        public void checkSize(double[] B)
        {
            if (m_intSize != B.Length)
            {
                throw new ArgumentException("Incompatible sizes: " + toStringShort() + " and " + B.Length);
            }
        }

        /**
         * Sanity check for operations requiring two matrices with the same size.
         * @throws ArgumentException if <tt>Size() != B.Size()</tt>.
         */

        public void checkSize(AbstractMatrix1D B)
        {
            if (m_intSize != B.m_intSize)
            {
                throw new ArgumentException("Incompatible sizes: " + toStringShort() + " and " + B.toStringShort());
            }
        }

        /**
         * Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
         * You may want to override this method for performance.
         *
         * @param     rank   the rank of the element.
         */

        public int index(int rank)
        {
            return _offset(_rank(rank));
        }

        /**
         * Sets up a matrix with a given number of cells.
         * @param size the number of cells the matrix shall have.
         * @throws ArgumentException if <tt>size<0</tt>.
         */

        public void setUp(int size)
        {
            setUp(size, 0, 1);
        }

        /**
         * Sets up a matrix with the given parameters.
         * @param size the number of elements the matrix shall have.
         * @param zero the index of the first element.
         * @param stride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @throws ArgumentException if <tt>size<0</tt>.
         */

        public void setUp(int size, int zero, int stride)
        {
            if (size < 0)
            {
                throw new ArgumentException("negative size");
            }

            m_intSize = size;
            m_intZero = zero;
            m_intStride = stride;
            isNoView = true;
        }

        /**
         * Returns the number of cells.
         */

        public override int Size()
        {
            return m_intSize;
        }

        /**
         * Returns the stride of the given dimension (axis, rank). 
         * 
         * @dimension the index of the dimension.
         * @return the stride in the given dimension.
         * @throws ArgumentException if <tt>dimension != 0</tt>.
         */

        public int stride(int dimension)
        {
            if (dimension != 0)
            {
                throw new ArgumentException("invalid dimension: " + dimension + "used to access" + toStringShort());
            }
            return m_intStride;
        }

        /**
         * Returns a string representation of the receiver's shape.
         */

        public string toStringShort()
        {
            return AbstractFormatter.shape(this);
        }

        /**
        Self modifying version of viewFlip().
        What used to be index <tt>0</tt> is now index <tt>Size()-1</tt>, ..., what used to be index <tt>Size()-1</tt> is now index <tt>0</tt>.
        */

        public AbstractMatrix1D vFlip()
        {
            if (m_intSize > 0)
            {
                m_intZero += (m_intSize - 1)*m_intStride;
                m_intStride = -m_intStride;
                isNoView = false;
            }
            return this;
        }

        /**
        Self modifying version of viewPart().
        @throws	HCException if <tt>index<0 || index+width>Size()</tt>.
        */

        public AbstractMatrix1D vPart(int index, int width)
        {
            checkRange(index, width);
            m_intZero += m_intStride*index;
            m_intSize = width;
            isNoView = false;
            return this;
        }

        /**
        Self modifying version of viewStrides().
        @throws	HCException if <tt>stride <= 0</tt>.
        */

        public AbstractMatrix1D vStrides(int stride)
        {
            if (stride <= 0)
            {
                throw new HCException("illegal stride: " + stride);
            }
            m_intStride *= stride;
            if (m_intSize != 0)
            {
                m_intSize = (m_intSize - 1)/stride + 1;
            }
            isNoView = false;
            return this;
        }
    }
}
