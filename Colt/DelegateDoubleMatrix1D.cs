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
    //package Appendimpl;

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    1-d matrix holding <tt>double</tt> elements; either a view wrapping another 2-d matrix and therefore delegating calls to it.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DelegateDoubleMatrix1D : WrapperDoubleMatrix1D
    {
        /*
         * The elements of the matrix.
         */
        public DoubleMatrix2D m_content;
        /*
         * The row this view is bound to.
         */
        public int m_intRow;


        public DelegateDoubleMatrix1D(DoubleMatrix2D newContent, int row)
            : base(null)
        {
            if (row < 0 || row >= newContent.Rows())
            {
                throw new ArgumentException();
            }
            setUp(newContent.Columns());
            m_intRow = row;
            m_content = newContent;
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
            return m_content.getQuick(m_intRow, index);
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix1D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix1D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param size the number of cell the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public new DoubleMatrix1D like(int size)
        {
            return m_content.like1D(size);
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
            return m_content.like(rows, columns);
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
            m_content.setQuick(m_intRow, index, value);
        }
    }
}
