#region

using System;
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

    ////import map.AbstractIntDoubleMap;
    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    Selection view on sparse 2-d matrices holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Objects of this class are typically constructed via <tt>viewIndexes</tt> methods on some source matrix.
    The interface introduced in abstract base classes defines everything a user can do.
    From a user point of view there is nothing special about this class; it presents the same functionality with the same signatures and semantics as its abstract superclass(es) while introducing no additional functionality.
    Thus, this class need not be visible to users.
    By the way, the same principle applies to concrete DenseXXX and SparseXXX classes: they presents the same functionality with the same signatures and semantics as abstract superclass(es) while introducing no additional functionality.
    Thus, they need not be visible to users, either. 
    Factory methods could hide all these concrete types.
    <p>
    This class uses no delegation. 
    Its instances point directly to the data. 
    Cell addressing overhead is 1 additional int addition and 2 additional array index accesses per get/set.
    <p>
    Note that this implementation is not lock.
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 4*(rowIndexes.Length+columnIndexes.Length)</tt>.
    Thus, an index view with 1000 x 1000 indexes additionally uses 8 KB.
    <p>
    <b>Time complexity:</b>
    <p>
    Depends on the parent view holding cells.
    <p>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class SelectedSparseDoubleMatrix2D : DoubleMatrix2D
    {
        /*
         * The elements of the matrix.
         */
        public int[] m_columnOffsets;
        public AbstractIntDoubleMap m_elements;

        /**
          * The offset.
          */
        public int m_offset;
        public int[] m_rowOffsets;
        /**
         * Constructs a matrix view with the given parameters.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param elements the cells.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two m_intColumns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @param  rowOffsets   The row offsets of the cells that shall be visible.
         * @param  columnOffsets   The column offsets of the cells that shall be visible.
         * @param  offset   
         */

        public SelectedSparseDoubleMatrix2D(int m_intRows, int m_intColumns, AbstractIntDoubleMap elements, int rowZero,
                                            int columnZero, int rowStride, int columnStride, int[] rowOffsets,
                                            int[] columnOffsets, int offset)
        {
            // be sure parameters are valid, we do not check...
            setUp(m_intRows, m_intColumns, rowZero, columnZero, rowStride, columnStride);

            m_elements = elements;
            m_rowOffsets = rowOffsets;
            m_columnOffsets = columnOffsets;
            m_offset = offset;

            isNoView = false;
        }

        /**
         * Constructs a matrix view with the given parameters.
         * @param elements the cells.
         * @param  rowOffsets   The row offsets of the cells that shall be visible.
         * @param  columnOffsets   The column offsets of the cells that shall be visible.
         * @param  offset   
         */

        public SelectedSparseDoubleMatrix2D(AbstractIntDoubleMap elements, int[] rowOffsets, int[] columnOffsets,
                                            int offset)
            : this(rowOffsets.Length, columnOffsets.Length, elements, 0, 0, 1, 1, rowOffsets, columnOffsets, offset)
        {
        }

        /**
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
         * Default implementation. Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */

        public new int _columnOffset(int absRank)
        {
            return m_columnOffsets[absRank];
        }

        /**
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
         * Default implementation. Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */

        public new int _rowOffset(int absRank)
        {
            return m_rowOffsets[absRank];
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
            //if (debug) if (column<0 || column>=m_intColumns || row<0 || row>=m_intRows) throw new HCException("row:"+row+", column:"+column);
            //return elements.get(index(row,column));
            //manually inlined:
            return
                m_elements.get(m_offset + m_rowOffsets[m_rowZero + row*m_rowStride] +
                               m_columnOffsets[m_columnZero + column*m_columnStride]);
        }

        /**
         * Returns <tt>true</tt> if both matrices share common cells.
         * More formally, returns <tt>true</tt> if <tt>other != null</tt> and at least one of the following conditions is met
         * <ul>
         * <li>the receiver is a view of the other matrix
         * <li>the other matrix is a view of the receiver
         * <li><tt>this == other</tt>
         * </ul>
         */

        public new bool haveSharedCellsRaw(DoubleMatrix2D other)
        {
            if (other is SelectedSparseDoubleMatrix2D)
            {
                SelectedSparseDoubleMatrix2D otherMatrix = (SelectedSparseDoubleMatrix2D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is SparseDoubleMatrix2D)
            {
                SparseDoubleMatrix2D otherMatrix = (SparseDoubleMatrix2D) other;

                return m_elements == otherMatrix.m_elements;
            }
            return false;
        }

        /**
         * Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
         *
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         */

        public new int index(int row, int column)
        {
            //return offset + base.index(row,column);
            //manually inlined:
            return m_offset + m_rowOffsets[m_rowZero + row*m_rowStride] +
                   m_columnOffsets[m_columnZero + column*m_columnStride];
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
            return new SparseDoubleMatrix2D(m_intRows, m_intColumns);
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
            return new SparseDoubleMatrix1D(size);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @param zero the index of the first element.
         * @param stride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix1D like1D(int size, int zero, int stride)
        {
            throw new HCException();
            // this method is never called since viewRow() and viewColumn are overridden properly.
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
            //if (debug) if (column<0 || column>=m_intColumns || row<0 || row>=m_intRows) throw new HCException("row:"+row+", column:"+column);
            //int index =	index(row,column);
            //manually inlined:
            int index = m_offset + m_rowOffsets[m_rowZero + row*m_rowStride] +
                        m_columnOffsets[m_columnZero + column*m_columnStride];

            if (value == 0)
            {
                m_elements.removeKey(index);
            }
            else
            {
                m_elements.put(index, value);
            }
        }

        /**
         * Sets up a matrix with a given number of m_intRows and m_intColumns.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @throws	ArgumentException if <tt>(double)m_intColumns*m_intRows > int.MaxValue</tt>.
         */

        public new void setUp(int m_intRows, int m_intColumns)
        {
            base.setUp(m_intRows, m_intColumns);
            m_rowStride = 1;
            m_columnStride = 1;
            m_offset = 0;
        }

        /**
        Self modifying version of viewDice().
        */

        public new AbstractMatrix2D vDice()
        {
            base.vDice();
            // swap
            int[] tmp = m_rowOffsets;
            m_rowOffsets = m_columnOffsets;
            m_columnOffsets = tmp;

            // flips stay unaffected

            isNoView = false;
            return this;
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

        @param the column to fix.
        @return a new slice view.
        @throws ArgumentException if <tt>column < 0 || column >= Columns()</tt>.
        @see #viewRow(int)
        */

        public new DoubleMatrix1D viewColumn(int column)
        {
            checkColumn(column);
            int viewSize = m_intRows;
            int viewZero = m_rowZero;
            int viewStride = m_rowStride;
            int[] viewOffsets = m_rowOffsets;
            int viewOffset = m_offset + _columnOffset(_columnRank(column));
            return new SelectedSparseDoubleMatrix1D(
                viewSize,
                m_elements,
                viewZero,
                viewStride,
                viewOffsets,
                viewOffset);
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

        @param the row to fix.
        @return a new slice view.
        @throws HCException if <tt>row < 0 || row >= Rows()</tt>.
        @see #viewColumn(int)
        */

        public new DoubleMatrix1D viewRow(int row)
        {
            checkRow(row);
            int viewSize = m_intColumns;
            int viewZero = m_columnZero;
            int viewStride = m_columnStride;
            int[] viewOffsets = m_columnOffsets;
            int viewOffset = m_offset + _rowOffset(_rowRank(row));
            return new SelectedSparseDoubleMatrix1D(viewSize, m_elements, viewZero, viewStride, viewOffsets, viewOffset);
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
            return new SelectedSparseDoubleMatrix2D(m_elements, rowOffsets, columnOffsets, m_offset);
        }
    }
}
