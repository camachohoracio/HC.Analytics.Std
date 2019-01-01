#region

using System;
using HC.Core.Helpers;

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

    ////import ObjectMatrix2D;
    ////import AppendObjectMatrix3D;
    /**
    Dense 3-d matrix holding <tt>Object</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Internally holds one single contigous one-dimensional array, addressed in (in decreasing order of significance): slice major, row major, column major.
    Note that this implementation is not lock.
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 8*Slices()*Rows()*Columns()</tt>.
    Thus, a 100*100*100 matrix uses 8 MB.
    <p>
    <b>Time complexity:</b>
    <p>
    <tt>O(1)</tt> (i.e. constant time) for the basic operations
    <tt>get</tt>, <tt>getQuick</tt>, <tt>set</tt>, <tt>setQuick</tt> and <tt>size</tt>,
    <p>
    Applications demanding utmost speed can exploit knowledge about the internal addressing.
    Setting/getting values in a loop slice-by-slice, row-by-row, column-by-column is quicker than, for example, column-by-column, row-by-row, slice-by-slice.
    Thus
    <pre>
       for (int slice=0; slice < m_intSlices; slice++) {
          for (int row=0; row < m_intRows; row++) {
             for (int column=0; column < m_intColumns; column++) {
                matrix.setQuick(slice,row,column,someValue);
             }		    
          }
       }
    </pre>
    is quicker than
    <pre>
       for (int column=0; column < m_intColumns; column++) {
          for (int row=0; row < m_intRows; row++) {
             for (int slice=0; slice < m_intSlices; slice++) {
                matrix.setQuick(slice,row,column,someValue);
             }
          }
       }
    </pre>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DenseObjectMatrix3D : ObjectMatrix3D
    {
        /**
          * The elements of this matrix.
          * elements are stored in slice major, then row major, then column major, in order of significance, i.e.
          * index==slice*sliceStride+ row*rowStride + column*columnStride
          * i.e. {slice0 row0..m}, {slice1 row0..m}, ..., {sliceN row0..m}
          * with each row storead as 
          * {row0 column0..m}, {row1 column0..m}, ..., {rown column0..m}
          */
        public Object[] m_elements;
        /**
         * Constructs a matrix with a copy of the given values.
         * <tt>values</tt> is required to have the form <tt>values[slice,row,column]</tt>
         * and have exactly the same number of m_intRows in in every slice and exactly the same number of m_intColumns in in every row.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param values The values to be filled into the new matrix.
         * @throws ArgumentException if <tt>for any 1 &lt;= slice &lt; values.Length: values[slice].Length != values[slice-1].Length</tt>.
         * @throws ArgumentException if <tt>for any 1 &lt;= row &lt; values.GetLength(1): values[slice,row].Length != values[slice,row-1].Length</tt>.
         */

        public DenseObjectMatrix3D(Object[,,] values)
            : this(
                values.GetLength(0), (values.GetLength(0) == 0 ? 0 : values.GetLength(1)),
                (values.GetLength(0) == 0 ? 0 : values.GetLength(1) == 0 ? 0 : values.GetLength(2)))
        {
            assign(values);
        }

        /**
         * Constructs a matrix with a given number of m_intSlices, m_intRows and m_intColumns.
         * All entries are initially <tt>0</tt>.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @throws	ArgumentException if <tt>(Object)m_intSlices*m_intColumns*m_intRows > int.MaxValue</tt>.
         * @throws	ArgumentException if <tt>m_intSlices<0 || m_intRows<0 || m_intColumns<0</tt>.
         */

        public DenseObjectMatrix3D(int m_intSlices, int m_intRows, int m_intColumns)
        {
            setUp(m_intSlices, m_intRows, m_intColumns);
            m_elements = new Object[m_intSlices*m_intRows*m_intColumns];
        }

        /**
         * Constructs a view with the given parameters.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param elements the cells.
         * @param sliceZero the position of the first element.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param sliceStride the number of elements between two m_intSlices, i.e. <tt>index(k+1,i,j)-index(k,i,j)</tt>.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(k,i+1,j)-index(k,i,j)</tt>.
         * @param columnnStride the number of elements between two m_intColumns, i.e. <tt>index(k,i,j+1)-index(k,i,j)</tt>.
         * @throws	ArgumentException if <tt>(Object)m_intSlices*m_intColumns*m_intRows > int.MaxValue</tt>.
         * @throws	ArgumentException if <tt>m_intSlices<0 || m_intRows<0 || m_intColumns<0</tt>.
         */

        public DenseObjectMatrix3D(int m_intSlices, int m_intRows, int m_intColumns, Object[] elements, int sliceZero,
                                   int rowZero, int columnZero, int sliceStride, int rowStride, int columnStride)
        {
            setUp(m_intSlices, m_intRows, m_intColumns, sliceZero, rowZero, columnZero, sliceStride, rowStride,
                  columnStride);
            m_elements = elements;
            isNoView = false;
        }

        /**
         * Sets all cells to the state specified by <tt>values</tt>.
         * <tt>values</tt> is required to have the form <tt>values[slice,row,column]</tt>
         * and have exactly the same number of m_intSlices, m_intRows and m_intColumns as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param    values the values to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         * @throws ArgumentException if <tt>values.Length != Slices() || for any 0 &lt;= slice &lt; Slices(): values[slice].Length != Rows()</tt>.
         * @throws ArgumentException if <tt>for any 0 &lt;= column &lt; Columns(): values[slice,row].Length != Columns()</tt>.
         */

        public new ObjectMatrix3D assign(Object[,,] values)
        {
            if (isNoView)
            {
                if (values.GetLength(0) != m_intSlices)
                {
                    throw new ArgumentException("Must have same number of m_intSlices: m_intSlices=" +
                                                values.GetLength(0) + "Slices()=" + Slices());
                }
                int i = m_intSlices*m_intRows*m_intColumns - m_intColumns;
                for (int slice = m_intSlices; --slice >= 0;)
                {
                    Object[,] currentSlice =
                        ArrayHelper.GetSliceCopy(
                            values, slice);
                    if (currentSlice.GetLength(0) != m_intRows)
                    {
                        throw new ArgumentException("Must have same number of m_intRows in every slice: m_intRows=" +
                                                    currentSlice.GetLength(0) + "Rows()=" + Rows());
                    }
                    for (int row = m_intRows; --row >= 0;)
                    {
                        Object[] currentRow = ArrayHelper.GetRowCopy(
                            currentSlice, row);
                        if (currentRow.Length != m_intColumns)
                        {
                            throw new ArgumentException(
                                "Must have same number of m_intColumns in every row: m_intColumns=" + currentRow.Length +
                                "Columns()=" + Columns());
                        }
                        Array.Copy(currentRow, 0, m_elements, i, m_intColumns);
                        i -= m_intColumns;
                    }
                }
            }
            else
            {
                base.assign(values);
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of m_intSlices, m_intRows and m_intColumns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     source   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Slices() != source.Slices() || Rows() != source.Rows() || Columns() != source.Columns()</tt>
         */

        public new ObjectMatrix3D assign(ObjectMatrix3D source)
        {
            // overriden for performance only
            if (!(source is DenseObjectMatrix3D))
            {
                return base.assign(source);
            }
            DenseObjectMatrix3D other = (DenseObjectMatrix3D) source;
            if (other == this)
            {
                return this;
            }
            checkShape(other);
            if (haveSharedCells(other))
            {
                ObjectMatrix3D c = other.Copy();
                if (!(c is DenseObjectMatrix3D))
                {
                    // should not happen
                    return base.assign(source);
                }
                other = (DenseObjectMatrix3D) c;
            }

            if (isNoView && other.isNoView)
            {
                // quickest
                Array.Copy(other.m_elements, 0, m_elements, 0, m_elements.Length);
                return this;
            }
            return base.assign(other);
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

        public override Object getQuick(int slice, int row, int column)
        {
            //if (debug) if (slice<0 || slice>=m_intSlices || row<0 || row>=m_intRows || column<0 || column>=m_intColumns) throw new HCException("slice:"+slice+", row:"+row+", column:"+column);
            //return elements[index(slice,row,column)];
            //manually inlined:
            return
                m_elements[
                    m_sliceZero + slice*m_sliceStride + m_rowZero + row*m_rowStride + m_columnZero +
                    column*m_columnStride];
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

        public new bool haveSharedCellsRaw(ObjectMatrix3D other)
        {
            if (other is SelectedDenseObjectMatrix3D)
            {
                SelectedDenseObjectMatrix3D otherMatrix = (SelectedDenseObjectMatrix3D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is DenseObjectMatrix3D)
            {
                DenseObjectMatrix3D otherMatrix = (DenseObjectMatrix3D) other;
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
            //return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
            //manually inlined:
            return m_sliceZero + slice*m_sliceStride + m_rowZero + row*m_rowStride + m_columnZero +
                   column*m_columnStride;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of m_intSlices, m_intRows and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix3D</tt> the new matrix must also be of type <tt>DenseObjectMatrix3D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix3D</tt> the new matrix must also be of type <tt>SparseObjectMatrix3D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override ObjectMatrix3D like(int m_intSlices, int m_intRows, int m_intColumns)
        {
            return new DenseObjectMatrix3D(m_intSlices, m_intRows, m_intColumns);
        }

        /**
         * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix3D</tt> the new matrix must also be of type <tt>DenseObjectMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix3D</tt> the new matrix must also be of type <tt>SparseObjectMatrix2D</tt>, etc.
         *
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two m_intColumns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override ObjectMatrix2D like2D(int m_intRows, int m_intColumns, int rowZero, int columnZero,
                                              int rowStride, int columnStride)
        {
            return new DenseObjectMatrix2D(m_intRows, m_intColumns, m_elements, rowZero, columnZero, rowStride,
                                           columnStride);
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

        public override void setQuick(int slice, int row, int column, Object value)
        {
            //if (debug) if (slice<0 || slice>=m_intSlices || row<0 || row>=m_intRows || column<0 || column>=m_intColumns) throw new HCException("slice:"+slice+", row:"+row+", column:"+column);
            //elements[index(slice,row,column)] = value;
            //manually inlined:
            m_elements[
                m_sliceZero + slice*m_sliceStride + m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride]
                = value;
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param sliceOffsets the offsets of the visible elements.
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override ObjectMatrix3D viewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseObjectMatrix3D(m_elements, sliceOffsets, rowOffsets, columnOffsets, 0);
        }
    }
}
