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
    2-d matrix holding <tt>double</tt> elements; either a view wrapping another matrix or a matrix whose views are wrappers.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 04/14/2000
    */

    [Serializable]
    public class WrapperDoubleMatrix2D : DoubleMatrix2D
    {
        /*
         * The elements of the matrix.
         */
        public DoubleMatrix2D content;
        /**
         * Constructs a matrix with a copy of the given values.
         * <tt>values</tt> is required to have the form <tt>values[row,column]</tt>
         * and have exactly the same number of m_intColumns in every row.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param values The values to be filled into the new matrix.
         * @throws ArgumentException if <tt>for any 1 &lt;= row &lt; values.Length: values.GetLength(1) != values[row-1].Length</tt>.
         */

        public WrapperDoubleMatrix2D(DoubleMatrix2D newContent)
        {
            if (newContent != null)
            {
                setUp(newContent.Rows(), newContent.Columns());
            }
            content = newContent;
        }

        /**
         * Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
         * Override this method in wrappers.
         */

        public new DoubleMatrix2D getContent()
        {
            return content;
        }

        /**
         * Returns the matrix cell value at coordinate <tt>[row,column]</tt>.
         *
         * <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>0 &lt;= column &lt; Columns() && 0 &lt;= row &lt; Rows()</tt>.
         *
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @return    the value at the specified coordinate.
         */

        public override double getQuick(int row, int column)
        {
            return content.getQuick(row, column);
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of m_intRows and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override DoubleMatrix2D like(int m_intRows, int m_intColumns)
        {
            return content.like(m_intRows, m_intColumns);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix1D like1D(int size)
        {
            return content.like1D(size);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @param offset the index of the first element.
         * @param stride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix1D like1D(int size, int offset, int stride)
        {
            throw new HCException(); // should never get called
        }

        /**
         * Sets the matrix cell at coordinate <tt>[row,column]</tt> to the specified value.
         *
         * <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>0 &lt;= column &lt; Columns() && 0 &lt;= row &lt; Rows()</tt>.
         *
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @param    value the value to be filled into the specified cell.
         */

        public override void setQuick(int row, int column, double value)
        {
            content.setQuick(row, column, value);
        }

        /**
        Constructs and returns a new <i>slice view</i> representing the m_intRows of the given column.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To obtain a slice view on subranges, construct a sub-ranging view (<tt>viewPart(...)</tt>), then apply this method to the sub-range view.
        <p> 
        <b>Example:</b> 
        <table border="0">
          <tr nowrap> 
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
            <td>viewColumn(0) ==></td>
            <td valign="top">Matrix1D of size 2:<br>
              1, 4</td>
           </tr>
        </table>

        @param column the column to fix.
        @return a new slice view.
        @throws HCException if <tt>column < 0 || column >= Columns()</tt>.
        @see #viewRow(int)
        */

        public new DoubleMatrix1D viewColumn(int column)
        {
            return viewDice().viewRow(column);
        }

        /**
        Constructs and returns a new <i>flip view</i> along the column axis.
        What used to be column <tt>0</tt> is now column <tt>Columns()-1</tt>, ..., what used to be column <tt>Columns()-1</tt> is now column <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        <p> 
        <b>Example:</b> 
        <table border="0">
          <tr nowrap> 
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
            <td>columnFlip ==></td>
            <td valign="top">2 x 3 matrix:<br>
              3, 2, 1 <br>
              6, 5, 4</td>
            <td>columnFlip ==></td>
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
          </tr>
        </table>

        @return a new flip view.
        @see #viewRowFlip()
        */

        public new DoubleMatrix2D viewColumnFlip()
        {
            if (m_intColumns == 0)
            {
                return this;
            }

            DoubleMatrix2D view = new WrapperDoubleMatrix2D_(this,
                                                             m_intColumns);

            return view;
        }

        /**
        Constructs and returns a new <i>dice (transposition) view</i>; Swaps axes; example: 3 x 4 matrix --> 4 x 3 matrix.
        The view has both dimensions exchanged; what used to be m_intColumns become m_intRows, what used to be m_intRows become m_intColumns.
        In other words: <tt>view.get(row,column)==get(column,row)</tt>.
        This is a zero-copy transposition, taking O(1), i.e. constant time.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 
        Use idioms like <tt>result = viewDice(A).Copy()</tt> to generate an independent transposed matrix.
        <p> 
        <b>Example:</b> 
        <table border="0">
          <tr nowrap> 
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
            <td>transpose ==></td>
            <td valign="top">3 x 2 matrix:<br>
              1, 4 <br>
              2, 5 <br>
              3, 6</td>
            <td>transpose ==></td>
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
          </tr>
        </table>

        @return a new dice view.
        */

        public new DoubleMatrix2D viewDice()
        {
            DoubleMatrix2D view = new WrapperDoubleMatrix2D2_(this, m_intColumns);
            view.m_intRows = m_intColumns;
            view.m_intColumns = m_intRows;
            return view;
        }

        /**
        Constructs and returns a new <i>sub-range view</i> that is a <tt>height x width</tt> sub matrix starting at <tt>[row,column]</tt>.

        Operations on the returned view can only be applied to the restricted range.
        Any attempt to access coordinates not contained in the view will throw an <tt>HCException</tt>.
        <p>
        <b>Note that the view is really just a range restriction:</b> 
        The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa. 
        <p>
        The view contains the cells from <tt>[row,column]</tt> to <tt>[row+height-1,column+width-1]</tt>, all inclusive.
        and has <tt>view.Rows() == height; view.Columns() == width;</tt>.
        A view's legal coordinates are again zero based, as usual.
        In other words, legal coordinates of the view range from <tt>[0,0]</tt> to <tt>[view.Rows()-1==height-1,view.Columns()-1==width-1]</tt>.
        As usual, any attempt to access a cell at a coordinate <tt>column&lt;0 || column&gt;=view.Columns() || row&lt;0 || row&gt;=view.Rows()</tt> will throw an <tt>HCException</tt>.

        @param     row   The index of the row-coordinate.
        @param     column   The index of the column-coordinate.
        @param     height   The height of the box.
        @param     width   The width of the box.
        @throws	HCException if <tt>column<0 || width<0 || column+width>Columns() || row<0 || height<0 || row+height>Rows()</tt>
        @return the new view.
		
        */

        public new DoubleMatrix2D viewPart(int row, int column, int height, int width)
        {
            checkBox(row, column, height, width);

            DoubleMatrix2D view = new WrapperDoubleMatrix2D3_(this,
                                                              row,
                                                              column);

            view.m_intRows = height;
            view.m_intColumns = width;

            return view;
        }

        /**
        Constructs and returns a new <i>slice view</i> representing the m_intColumns of the given row.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To obtain a slice view on subranges, construct a sub-ranging view (<tt>viewPart(...)</tt>), then apply this method to the sub-range view.
        <p> 
        <b>Example:</b> 
        <table border="0">
          <tr nowrap> 
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
            <td>viewRow(0) ==></td>
            <td valign="top">Matrix1D of size 3:<br>
              1, 2, 3</td>
           </tr>
        </table>

        @param row the row to fix.
        @return a new slice view.
        @throws HCException if <tt>row < 0 || row >= Rows()</tt>.
        @see #viewColumn(int)
        */

        public new DoubleMatrix1D viewRow(int row)
        {
            checkRow(row);
            return new DelegateDoubleMatrix1D(this, row);
        }

        /**
        Constructs and returns a new <i>flip view</i> along the row axis.
        What used to be row <tt>0</tt> is now row <tt>Rows()-1</tt>, ..., what used to be row <tt>Rows()-1</tt> is now row <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        <p> 
        <b>Example:</b> 
        <table border="0">
          <tr nowrap> 
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
            <td>rowFlip ==></td>
            <td valign="top">2 x 3 matrix:<br>
              4, 5, 6 <br>
              1, 2, 3</td>
            <td>rowFlip ==></td>
            <td valign="top">2 x 3 matrix: <br>
              1, 2, 3<br>
              4, 5, 6 </td>
          </tr>
        </table>

        @return a new flip view.
        @see #viewColumnFlip()
        */

        public new DoubleMatrix2D viewRowFlip()
        {
            if (m_intRows == 0)
            {
                return this;
            }

            DoubleMatrix2D view = new WrapperDoubleMatrix2D4_(this,
                                                              m_intRows);

            return view;
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        There holds <tt>view.Rows() == rowIndexes.Length, view.Columns() == columnIndexes.Length</tt> and <tt>view.get(i,j) == get(rowIndexes[i],columnIndexes[j])</tt>.
        Indexes can occur multiple times and can be in arbitrary order.
        <p>
        <b>Example:</b>
        <pre>
        this = 2 x 3 matrix:
        1, 2, 3
        4, 5, 6
        rowIndexes     = (0,1)
        columnIndexes  = (1,0,1,0)
        -->
        view = 2 x 4 matrix:
        2, 1, 2, 1
        5, 4, 5, 4
        </pre>
        Note that modifying the index arguments after this call has returned has no effect on the view.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 
        <p>
        To indicate "all" m_intRows or "all m_intColumns", simply set the respective parameter
        @param  rowIndexes   The m_intRows of the cells that shall be visible in the new view. To indicate that <i>all</i> m_intRows shall be visible, simply set this parameter to <tt>null</tt>.
        @param  columnIndexes   The m_intColumns of the cells that shall be visible in the new view. To indicate that <i>all</i> m_intColumns shall be visible, simply set this parameter to <tt>null</tt>.
        @return the new view.
        @throws HCException if <tt>!(0 <= rowIndexes[i] < Rows())</tt> for any <tt>i=0..rowIndexes.Length-1</tt>.
        @throws HCException if <tt>!(0 <= columnIndexes[i] < Columns())</tt> for any <tt>i=0..columnIndexes.Length-1</tt>.
        */

        public new DoubleMatrix2D viewSelection(int[] rowIndexes, int[] columnIndexes)
        {
            // check for "all"
            if (rowIndexes == null)
            {
                rowIndexes = new int[m_intRows];
                for (int i = m_intRows; --i >= 0;)
                {
                    rowIndexes[i] = i;
                }
            }
            if (columnIndexes == null)
            {
                columnIndexes = new int[m_intColumns];
                for (int i = m_intColumns; --i >= 0;)
                {
                    columnIndexes[i] = i;
                }
            }

            checkRowIndexes(rowIndexes);
            checkColumnIndexes(columnIndexes);
            int[] rix = rowIndexes;
            int[] cix = columnIndexes;

            DoubleMatrix2D view = new WrapperDoubleMatrix2D6_(this,
                                                              rix,
                                                              cix);
            view.m_intRows = rowIndexes.Length;
            view.m_intColumns = columnIndexes.Length;

            return view;
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override DoubleMatrix2D viewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            throw new HCException(); // should never be called
        }

        /**
        Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        More specifically, the view has <tt>Rows()/rowStride</tt> m_intRows and <tt>Columns()/columnStride</tt> m_intColumns holding cells <tt>get(i*rowStride,j*columnStride)</tt> for all <tt>i = 0..Rows()/rowStride - 1, j = 0..Columns()/columnStride - 1</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @param rowStride the row step factor.
        @param columnStride the column step factor.
        @return a new view.
        @throws	HCException if <tt>rowStride<=0 || columnStride<=0</tt>.
        */

        public new DoubleMatrix2D viewStrides(int _rowStride, int _columnStride)
        {
            if (_rowStride <= 0 || _columnStride <= 0)
            {
                throw new HCException("illegal stride");
            }
            DoubleMatrix2D view = new WrapperDoubleMatrix2D7_(
                this,
                _rowStride,
                _columnStride);
            view.m_intRows = m_intRows;
            view.m_intColumns = m_intColumns;
            if (m_intRows != 0)
            {
                view.m_intRows = (m_intRows - 1)/_rowStride + 1;
            }
            if (m_intColumns != 0)
            {
                view.m_intColumns = (m_intColumns - 1)/_columnStride + 1;
            }
            return view;
        }
    }
}
