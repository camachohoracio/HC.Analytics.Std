#region

using System;
using HC.Core.Exceptions;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package impl;

    ////import DoubleMatrix2D;
    ////import DoubleMatrix3D;
    /**
    Selection view on dense 3-d matrices holding <tt>double</tt> elements.
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
    Cell addressing overhead is is 1 additional int addition and 3 additional array index accesses per get/set.
    <p>
    Note that this implementation is not .
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 4*(sliceIndexes.Length+rowIndexes.Length+columnIndexes.Length)</tt>.
    Thus, an index view with 100 x 100 x 100 indexes additionally uses 8 KB.
    <p>
    <b>Time complexity:</b>
    <p>
    Depends on the parent view holding cells.
    <p>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class SelectedDenseDoubleMatrix3D : DoubleMatrix3D
    {
        /**
          * The elements of this matrix.
          */
        public int[] m_columnOffsets;
        public double[] m_elements;

        /**
          * The offset.
          */
        public int m_offset;
        public int[] m_rowOffsets;
        public int[] m_sliceOffsets;

        /**
         * Constructs a matrix view with the given parameters.
         * @param elements the cells.
         * @param  sliceOffsets   The slice offsets of the cells that shall be visible.
         * @param  rowOffsets   The row offsets of the cells that shall be visible.
         * @param  columnOffsets   The column offsets of the cells that shall be visible.
         */

        public SelectedDenseDoubleMatrix3D(double[] elements, int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets,
                                           int offset)
        {
            // be sure parameters are valid, we do not check...
            int m_intSlices = sliceOffsets.Length;
            int m_intRows = rowOffsets.Length;
            int m_intColumns = columnOffsets.Length;
            setUp(m_intSlices, m_intRows, m_intColumns);

            m_elements = elements;

            m_sliceOffsets = sliceOffsets;
            m_rowOffsets = rowOffsets;
            m_columnOffsets = columnOffsets;

            m_offset = offset;

            isNoView = false;
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
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
         * Default implementation. Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */

        public new int _sliceOffset(int absRank)
        {
            return m_sliceOffsets[absRank];
        }

        /**
         * Returns the matrix cell value at coordinate <tt>[slice,row,column]</tt>.
         *
         * <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>slice&lt;0 || slice&gt;=Slices() || row&lt;0 || row&gt;=Rows() || column&lt;0 || column&gt;=column()</tt>.
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @return    the value at the specified coordinate.
         */

        public override double getQuick(int slice, int row, int column)
        {
            //if (debug) if (slice<0 || slice>=m_intSlices || row<0 || row>=m_intRows || column<0 || column>=m_intColumns) throw new HCException("slice:"+slice+", row:"+row+", column:"+column);
            //return elements[index(slice,row,column)];
            //manually inlined:
            return
                m_elements[
                    m_offset + m_sliceOffsets[m_sliceZero + slice*m_sliceStride] +
                    m_rowOffsets[m_rowZero + row*m_rowStride] + m_columnOffsets[m_columnZero + column*m_columnStride]];
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

        public new bool haveSharedCellsRaw(DoubleMatrix3D other)
        {
            if (other is SelectedDenseDoubleMatrix3D)
            {
                SelectedDenseDoubleMatrix3D otherMatrix = (SelectedDenseDoubleMatrix3D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is DenseDoubleMatrix3D)
            {
                DenseDoubleMatrix3D otherMatrix = (DenseDoubleMatrix3D) other;
                return m_elements == otherMatrix.m_elements;
            }
            return false;
        }

        /**
         * Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the third-coordinate.
         */

        public new int index(int slice, int row, int column)
        {
            //return offset + base.index(slice,row,column);
            //manually inlined:
            return m_offset + m_sliceOffsets[m_sliceZero + slice*m_sliceStride] +
                   m_rowOffsets[m_rowZero + row*m_rowStride] + m_columnOffsets[m_columnZero + column*m_columnStride];
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of m_intSlices, m_intRows and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix3D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix3D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix3D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix3D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override DoubleMatrix3D like(int m_intSlices, int m_intRows, int m_intColumns)
        {
            return new DenseDoubleMatrix3D(m_intSlices, m_intRows, m_intColumns);
        }

        /**
         * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix3D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix3D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         *
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two m_intColumns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix2D like2D(int m_intRows, int m_intColumns, int rowZero, int columnZero,
                                              int rowStride, int columnStride)
        {
            throw new HCException();
            // this method is never called since viewRow() and viewColumn are overridden properly.
        }

        /**
         * Sets the matrix cell at coordinate <tt>[slice,row,column]</tt> to the specified value.
         *
         * <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>slice&lt;0 || slice&gt;=Slices() || row&lt;0 || row&gt;=Rows() || column&lt;0 || column&gt;=column()</tt>.
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @param    value the value to be filled into the specified cell.
         */

        public override void setQuick(int slice, int row, int column, double value)
        {
            //if (debug) if (slice<0 || slice>=m_intSlices || row<0 || row>=m_intRows || column<0 || column>=m_intColumns) throw new HCException("slice:"+slice+", row:"+row+", column:"+column);
            //elements[index(slice,row,column)] = value;
            //manually inlined:
            m_elements[
                m_offset + m_sliceOffsets[m_sliceZero + slice*m_sliceStride] + m_rowOffsets[m_rowZero + row*m_rowStride] +
                m_columnOffsets[m_columnZero + column*m_columnStride]] = value;
        }

        /**
         * Sets up a matrix with a given number of m_intSlices and m_intRows.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @throws	ArgumentException if <tt>(double)m_intRows*m_intSlices > int.MaxValue</tt>.
         */

        public new void setUp(int m_intSlices, int m_intRows, int m_intColumns)
        {
            base.setUp(m_intSlices, m_intRows, m_intColumns);
            m_sliceStride = 1;
            m_rowStride = 1;
            m_columnStride = 1;
            m_offset = 0;
        }

        /**
        Self modifying version of viewDice().
        @throws ArgumentException if some of the parameters are equal or not in range 0..2.
        */

        public new AbstractMatrix3D vDice(int axis0, int axis1, int axis2)
        {
            base.vDice(axis0, axis1, axis2);
            // swap offsets
            int[,] offsets = new int[3,m_sliceOffsets.Length];
            ArrayHelper.SetRow(
                offsets,
                m_sliceOffsets,
                0);

            ArrayHelper.SetRow(
                offsets,
                m_rowOffsets,
                1);

            ArrayHelper.SetRow(
                offsets,
                m_columnOffsets,
                2);


            m_sliceOffsets =
                ArrayHelper.GetRowCopy(offsets, axis0);
            m_rowOffsets =
                ArrayHelper.GetRowCopy(offsets, axis1);
            m_columnOffsets =
                ArrayHelper.GetRowCopy(offsets, axis2);

            return this;
        }

        /**
        Constructs and returns a new 2-dimensional <i>slice view</i> representing the m_intSlices and m_intRows of the given column.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        <p>
        To obtain a slice view on subranges, construct a sub-ranging view (<tt>view().part(...)</tt>), then apply this method to the sub-range view.
        To obtain 1-dimensional views, apply this method, then apply another slice view (methods <tt>viewColumn</tt>, <tt>viewRow</tt>) on the intermediate 2-dimensional view.
        To obtain 1-dimensional views on subranges, apply both steps.

        @param column the index of the column to fix.
        @return a new 2-dimensional slice view.
        @throws HCException if <tt>column < 0 || column >= Columns()</tt>.
        @see #viewSlice(int)
        @see #viewRow(int)
        */

        public new DoubleMatrix2D viewColumn(int column)
        {
            checkColumn(column);

            int viewRows = m_intSlices;
            int viewColumns = m_intRows;

            int viewRowZero = m_sliceZero;
            int viewColumnZero = m_rowZero;
            int viewOffset = m_offset + _columnOffset(_columnRank(column));

            int viewRowStride = m_sliceStride;
            int viewColumnStride = m_rowStride;

            int[] viewRowOffsets = m_sliceOffsets;
            int[] viewColumnOffsets = m_rowOffsets;

            return new SelectedDenseDoubleMatrix2D(viewRows, viewColumns, m_elements, viewRowZero, viewColumnZero,
                                                   viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets,
                                                   viewOffset);
        }

        /**
        Constructs and returns a new 2-dimensional <i>slice view</i> representing the m_intSlices and m_intColumns of the given row.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        <p>
        To obtain a slice view on subranges, construct a sub-ranging view (<tt>view().part(...)</tt>), then apply this method to the sub-range view.
        To obtain 1-dimensional views, apply this method, then apply another slice view (methods <tt>viewColumn</tt>, <tt>viewRow</tt>) on the intermediate 2-dimensional view.
        To obtain 1-dimensional views on subranges, apply both steps.

        @param row the index of the row to fix.
        @return a new 2-dimensional slice view.
        @throws HCException if <tt>row < 0 || row >= row()</tt>.
        @see #viewSlice(int)
        @see #viewColumn(int)
        */

        public new DoubleMatrix2D viewRow(int row)
        {
            checkRow(row);

            int viewRows = m_intSlices;
            int viewColumns = m_intColumns;

            int viewRowZero = m_sliceZero;
            int viewColumnZero = m_columnZero;
            int viewOffset = m_offset + _rowOffset(_rowRank(row));

            int viewRowStride = m_sliceStride;
            int viewColumnStride = m_columnStride;

            int[] viewRowOffsets = m_sliceOffsets;
            int[] viewColumnOffsets = m_columnOffsets;

            return new SelectedDenseDoubleMatrix2D(viewRows, viewColumns, m_elements, viewRowZero, viewColumnZero,
                                                   viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets,
                                                   viewOffset);
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param sliceOffsets the offsets of the visible elements.
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override DoubleMatrix3D viewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseDoubleMatrix3D(m_elements, sliceOffsets, rowOffsets, columnOffsets, m_offset);
        }

        /**
        Constructs and returns a new 2-dimensional <i>slice view</i> representing the m_intRows and m_intColumns of the given slice.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        <p>
        To obtain a slice view on subranges, construct a sub-ranging view (<tt>view().part(...)</tt>), then apply this method to the sub-range view.
        To obtain 1-dimensional views, apply this method, then apply another slice view (methods <tt>viewColumn</tt>, <tt>viewRow</tt>) on the intermediate 2-dimensional view.
        To obtain 1-dimensional views on subranges, apply both steps.

        @param slice the index of the slice to fix.
        @return a new 2-dimensional slice view.
        @throws HCException if <tt>slice < 0 || slice >= Slices()</tt>.
        @see #viewRow(int)
        @see #viewColumn(int)
        */

        public new DoubleMatrix2D viewSlice(int slice)
        {
            checkSlice(slice);

            int viewRows = m_intRows;
            int viewColumns = m_intColumns;

            int viewRowZero = m_rowZero;
            int viewColumnZero = m_columnZero;
            int viewOffset = m_offset + _sliceOffset(_sliceRank(slice));

            int viewRowStride = m_rowStride;
            int viewColumnStride = m_columnStride;

            int[] viewRowOffsets = m_rowOffsets;
            int[] viewColumnOffsets = m_columnOffsets;

            return new SelectedDenseDoubleMatrix2D(viewRows, viewColumns, m_elements, viewRowZero, viewColumnZero,
                                                   viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets,
                                                   viewOffset);
        }
    }
}
