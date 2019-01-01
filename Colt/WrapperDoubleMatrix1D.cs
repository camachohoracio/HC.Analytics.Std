#region

using System;
using HC.Analytics.Colt.CustomImplementations;
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
    //package Appendimpl;

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    1-d matrix holding <tt>double</tt> elements; either a view wrapping another matrix or a matrix whose views are wrappers.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class WrapperDoubleMatrix1D : DoubleMatrix1D
    {
        /*
         * The elements of the matrix.
         */
        public DoubleMatrix1D content;

        public WrapperDoubleMatrix1D(DoubleMatrix1D newContent)
        {
            if (newContent != null)
            {
                setUp(newContent.Size());
            }
            content = newContent;
        }

        /**
         * Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
         * Override this method in wrappers.
         */

        public new DoubleMatrix1D getContent()
        {
            return content;
        }

        /**
         * Returns the matrix cell value at coordinate <tt>index</tt>.
         *
         * <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>index&lt;0 || index&gt;=Size()</tt>.
         *
         * @param     index   the index of the cell.
         * @return    the value of the specified cell.
         */

        public override double getQuick(int index)
        {
            return content.getQuick(index);
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified m_intSize.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix1D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix1D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intSize the number of cell the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override DoubleMatrix1D like(int m_intSize)
        {
            return content.like(m_intSize);
        }

        /**
         * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix1D</tt> the new matrix must be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix1D</tt> the new matrix must be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         *
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix2D like2D(int rows, int columns)
        {
            return content.like2D(rows, columns);
        }

        /**
         * Sets the matrix cell at coordinate <tt>index</tt> to the specified value.
         *
         * <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>index&lt;0 || index&gt;=Size()</tt>.
         *
         * @param     index   the index of the cell.
         * @param    value the value to be filled into the specified cell.
         */

        public override void setQuick(int index, double value)
        {
            content.setQuick(index, value);
        }

        /**
        Constructs and returns a new <i>flip view</i>.
        What used to be index <tt>0</tt> is now index <tt>Size()-1</tt>, ..., what used to be index <tt>Size()-1</tt> is now index <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @return a new flip view.
        */

        public new DoubleMatrix1D viewFlip()
        {
            DoubleMatrix1D view = new WrapperDoubleMatrix1D2_(
                this,
                m_intSize);
            return view;
        }

        /**
        Constructs and returns a new <i>sub-range view</i> that is a <tt>width</tt> sub matrix starting at <tt>index</tt>.

        Operations on the returned view can only be applied to the restricted range.
        Any attempt to access coordinates not contained in the view will throw an <tt>HCException</tt>.
        <p>
        <b>Note that the view is really just a range restriction:</b> 
        The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa. 
        <p>
        The view contains the cells from <tt>index..index+width-1</tt>.
        and has <tt>view.Size() == width</tt>.
        A view's legal coordinates are again zero based, as usual.
        In other words, legal coordinates of the view are <tt>0 .. view.Size()-1==width-1</tt>.
        As usual, any attempt to access a cell at other coordinates will throw an <tt>HCException</tt>.

        @param     index   The index of the first cell.
        @param     width   The width of the range.
        @throws	HCException if <tt>index<0 || width<0 || index+width>Size()</tt>.
        @return the new view.
		
        */

        public new DoubleMatrix1D viewPart(int index, int width)
        {
            checkRange(index, width);
            DoubleMatrix1D view = new WrapperDoubleMatrix1D2_(this,
                                                              m_intSize);

            view.m_intSize = width;

            return view;
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        There holds <tt>view.Size() == indexes.Length</tt> and <tt>view.get(i) == get(indexes[i])</tt>.
        Indexes can occur multiple times and can be in arbitrary order.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        this     = (0,0,8,0,7)
        indexes  = (0,2,4,2)
        -->
        view     = (0,8,7,8)
        </pre>
        Note that modifying <tt>indexes</tt> after this call has returned has no effect on the view.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param  indexes   The indexes of the cells that shall be visible in the new view. To indicate that <i>all</i> cells shall be visible, simply set this parameter to <tt>null</tt>.
        @return the new view.
        @throws HCException if <tt>!(0 <= indexes[i] < Size())</tt> for any <tt>i=0..indexes.Length-1</tt>.
        */

        public new DoubleMatrix1D viewSelection(int[] indexes)
        {
            // check for "all"
            if (indexes == null)
            {
                indexes = new int[m_intSize];
                for (int i = m_intSize; --i >= 0;)
                {
                    indexes[i] = i;
                }
            }

            checkIndexes(indexes);
            int[] idx = indexes;

            DoubleMatrix1D view =
                new WrapperDoubleMatrix1D3_(this, idx);

            return view;
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param offsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override DoubleMatrix1D viewSelectionLike(int[] offsets)
        {
            throw new HCException(); // should never get called
        }

        /**
        Constructs and returns a new <i>m_intStride view</i> which is a sub matrix consisting of every i-th cell.
        More specifically, the view has m_intSize <tt>Size()/m_intStride</tt> holding cells <tt>get(i*m_intStride)</tt> for all <tt>i = 0..Size()/m_intStride - 1</tt>.

        @param  m_intStride  the step factor.
        @throws	HCException if <tt>m_intStride <= 0</tt>.
        @return the new view.
		
        */

        public new DoubleMatrix1D viewStrides(int _stride)
        {
            if (m_intStride <= 0)
            {
                throw new HCException("illegal m_intStride: " + m_intStride);
            }
            DoubleMatrix1D view = new WrapperDoubleMatrix1D4_(this, _stride);
            view.m_intSize = m_intSize;
            if (m_intSize != 0)
            {
                view.m_intSize = (m_intSize - 1)/_stride + 1;
            }
            return view;
        }
    }
}
