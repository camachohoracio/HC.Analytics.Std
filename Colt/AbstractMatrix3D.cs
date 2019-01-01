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
    ////package impl;

    /**
    Abstract base class for 3-d matrices holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Note that this implementation is not .</b>

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class AbstractMatrix3D : AbstractMatrix
    {
        /** the number of m_intSlices this matrix (view) has */

        /** the number of columns this matrix (view) has */
        public int m_columnStride;
        public int m_columnZero;
        public int m_intColumns;
        public int m_intRows;
        public int m_intSlices;


        /** the number of elements between two m_intSlices, i.e. <tt>index(k+1,i,j) - index(k,i,j)</tt>. */

        /** the number of elements between two rows, i.e. <tt>index(k,i+1,j) - index(k,i,j)</tt>. */
        public int m_rowStride;

        /** the number of elements between two columns, i.e. <tt>index(k,i,j+1) - index(k,i,j)</tt>. */
        public int m_rowZero;
        public int m_sliceStride;
        public int m_sliceZero;
        // isNoView implies: offset==0, sliceStride==rows*m_intSlices, rowStride==columns, columnStride==1
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

        public int _columnOffset(int absRank)
        {
            return absRank;
        }

        /**
         * Returns the absolute rank of the given relative rank. 
         *
         * @param  rank   the relative rank of the element.
         * @return the absolute rank of the element.
         */

        public int _columnRank(int rank)
        {
            return m_columnZero + rank*m_columnStride;
        }

        /**
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
         * Default implementation. Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */

        public int _rowOffset(int absRank)
        {
            return absRank;
        }

        /**
         * Returns the absolute rank of the given relative rank. 
         *
         * @param  rank   the relative rank of the element.
         * @return the absolute rank of the element.
         */

        public int _rowRank(int rank)
        {
            return m_rowZero + rank*m_rowStride;
        }

        /**
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
         * Default implementation. Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */

        public int _sliceOffset(int absRank)
        {
            return absRank;
        }

        /**
         * Returns the absolute rank of the given relative rank. 
         *
         * @param  rank   the relative rank of the element.
         * @return the absolute rank of the element.
         */

        public int _sliceRank(int rank)
        {
            return m_sliceZero + rank*m_sliceStride;
        }

        /**
         * Checks whether the receiver contains the given box and throws an exception, if necessary.
         * @throws	HCException if <tt>row<0 || height<0 || row+height>rows || slice<0 || depth<0 || slice+depth>m_intSlices  || column<0 || width<0 || column+width>columns</tt>
         */

        public void checkBox(int slice, int row, int column, int depth, int height, int width)
        {
            if (slice < 0 || depth < 0 || slice + depth > m_intSlices || row < 0 || height < 0 ||
                row + height > m_intRows || column < 0 || width < 0 || column + width > m_intColumns)
            {
                throw new HCException(toStringShort() + ", slice:" + slice + ", row:" + row + " ,column:" + column +
                                    ", depth:" + depth + " ,height:" + height + ", width:" + width);
            }
        }

        /**
         * Sanity check for operations requiring a column index to be within bounds.
         * @throws HCException if <tt>column < 0 || column >= Columns()</tt>.
         */

        public void checkColumn(int column)
        {
            if (column < 0 || column >= m_intColumns)
            {
                throw new HCException("Attempted to access " + toStringShort() + " at column=" + column);
            }
        }

        /**
         * Checks whether indexes are legal and throws an exception, if necessary.
         * @throws HCException if <tt>! (0 <= indexes[i] < Columns())</tt> for any i=0..indexes.Length-1.
         */

        public void checkColumnIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= m_intColumns)
                {
                    checkColumn(index);
                }
            }
        }

        /**
         * Sanity check for operations requiring a row index to be within bounds.
         * @throws HCException if <tt>row < 0 || row >= Rows()</tt>.
         */

        public void checkRow(int row)
        {
            if (row < 0 || row >= m_intRows)
            {
                throw new HCException("Attempted to access " + toStringShort() + " at row=" + row);
            }
        }

        /**
         * Checks whether indexes are legal and throws an exception, if necessary.
         * @throws HCException if <tt>! (0 <= indexes[i] < Rows())</tt> for any i=0..indexes.Length-1.
         */

        public void checkRowIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= m_intRows)
                {
                    checkRow(index);
                }
            }
        }

        /**
         * Sanity check for operations requiring two matrices with the same number of m_intSlices, rows and columns.
         * @throws ArgumentException if <tt>Slices() != B.Slices() || Rows() != B.Rows() || Columns() != B.Columns()</tt>.
         */

        public void checkShape(AbstractMatrix3D B)
        {
            if (m_intSlices != B.m_intSlices || m_intRows != B.m_intRows || m_intColumns != B.m_intColumns)
            {
                throw new ArgumentException("Incompatible dimensions: " + toStringShort() + " and " + B.toStringShort());
            }
        }

        /**
         * Sanity check for operations requiring matrices with the same number of m_intSlices, rows and columns.
         * @throws ArgumentException if <tt>Slices() != B.Slices() || Rows() != B.Rows() || Columns() != B.Columns() || Slices() != C.Slices() || Rows() != C.Rows() || Columns() != C.Columns()</tt>.
         */

        public void checkShape(AbstractMatrix3D B, AbstractMatrix3D C)
        {
            if (m_intSlices != B.m_intSlices || m_intRows != B.m_intRows || m_intColumns != B.m_intColumns ||
                m_intSlices != C.m_intSlices || m_intRows != C.m_intRows || m_intColumns != C.m_intColumns)
            {
                throw new ArgumentException("Incompatible dimensions: " + toStringShort() + ", " + B.toStringShort() +
                                            ", " + C.toStringShort());
            }
        }

        /**
         * Sanity check for operations requiring a slice index to be within bounds.
         * @throws HCException if <tt>slice < 0 || slice >= Slices()</tt>.
         */

        public void checkSlice(int slice)
        {
            if (slice < 0 || slice >= m_intSlices)
            {
                throw new HCException("Attempted to access " + toStringShort() + " at slice=" + slice);
            }
        }

        /**
         * Checks whether indexes are legal and throws an exception, if necessary.
         * @throws HCException if <tt>! (0 <= indexes[i] < Slices())</tt> for any i=0..indexes.Length-1.
         */

        public void checkSliceIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= m_intSlices)
                {
                    checkSlice(index);
                }
            }
        }

        /**
         * Returns the number of columns.
         */

        public int Columns()
        {
            return m_intColumns;
        }

        /**
         * Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the third-coordinate.
         */

        public int index(int slice, int row, int column)
        {
            return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
        }

        /**
         * Returns the number of rows.
         */

        public int Rows()
        {
            return m_intRows;
        }

        /**
         * Sets up a matrix with a given number of m_intSlices and rows.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @throws	ArgumentException if <tt>(double)rows*m_intSlices > int.MaxValue</tt>.
         * @throws	ArgumentException if <tt>m_intSlices<0 || rows<0 || columns<0</tt>.
         */

        public void setUp(int m_intSlices, int rows, int columns)
        {
            setUp(m_intSlices, rows, columns, 0, 0, 0, rows*columns, columns, 1);
        }

        /**
         * Sets up a matrix with a given number of m_intSlices and rows and the given strides.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @param sliceZero the position of the first element.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param sliceStride the number of elements between two m_intSlices, i.e. <tt>index(k+1,i,j)-index(k,i,j)</tt>.
         * @param rowStride the number of elements between two rows, i.e. <tt>index(k,i+1,j)-index(k,i,j)</tt>.
         * @param columnnStride the number of elements between two columns, i.e. <tt>index(k,i,j+1)-index(k,i,j)</tt>.
         * @throws	ArgumentException if <tt>(double)m_intSlices*rows*columnss > int.MaxValue</tt>.
         * @throws	ArgumentException if <tt>m_intSlices<0 || rows<0 || columns<0</tt>.
         */

        public void setUp(int intSlices, int rows, int columns, int sliceZero, int rowZero, int columnZero,
                          int sliceStride, int rowStride, int columnStride)
        {
            if (m_intSlices < 0 || rows < 0 || columns < 0)
            {
                throw new ArgumentException("negative size");
            }
            if ((double) m_intSlices*rows*columns > int.MaxValue)
            {
                throw new ArgumentException("matrix too large");
            }

            m_intSlices = intSlices;
            m_intRows = rows;
            m_intColumns = columns;

            m_sliceZero = sliceZero;
            m_rowZero = rowZero;
            m_columnZero = columnZero;

            m_sliceStride = sliceStride;
            m_rowStride = rowStride;
            m_columnStride = columnStride;

            isNoView = true;
        }

        public int[] shape()
        {
            int[] shape = new int[3];
            shape[0] = m_intSlices;
            shape[1] = m_intRows;
            shape[2] = m_intColumns;
            return shape;
        }

        /**
         * Returns the number of cells which is <tt>Slices()*Rows()*Columns()</tt>.
         */

        public override int Size()
        {
            return m_intSlices*m_intRows*m_intColumns;
        }

        /**
         * Returns the number of m_intSlices.
         */

        public int Slices()
        {
            return m_intSlices;
        }

        /**
         * Returns a string representation of the receiver's shape.
         */

        public string toStringShort()
        {
            return AbstractFormatter.shape(this);
        }

        /**
        Self modifying version of viewColumnFlip().
        */

        public AbstractMatrix3D vColumnFlip()
        {
            if (m_intColumns > 0)
            {
                m_columnZero += (m_intColumns - 1)*m_columnStride;
                m_columnStride = -m_columnStride;
                isNoView = false;
            }
            return this;
        }

        /**
        Self modifying version of viewDice().
        @throws ArgumentException if some of the parameters are equal or not in range 0..2.
        */

        public AbstractMatrix3D vDice(int axis0, int axis1, int axis2)
        {
            int d = 3;
            if (axis0 < 0 || axis0 >= d || axis1 < 0 || axis1 >= d || axis2 < 0 || axis2 >= d ||
                axis0 == axis1 || axis0 == axis2 || axis1 == axis2)
            {
                throw new ArgumentException("Illegal Axes: " + axis0 + ", " + axis1 + ", " + axis2);
            }

            // swap shape
            int[] shape_ = shape();

            m_intSlices = shape_[axis0];
            m_intRows = shape_[axis1];
            m_intColumns = shape_[axis2];

            // swap strides
            int[] strides = new int[3];
            strides[0] = m_sliceStride;
            strides[1] = m_rowStride;
            strides[2] = m_columnStride;

            m_sliceStride = strides[axis0];
            m_rowStride = strides[axis1];
            m_columnStride = strides[axis2];

            isNoView = false;
            return this;
        }

        /**
        Self modifying version of viewPart().
        @throws HCException if <tt>slice<0 || depth<0 || slice+depth>Slices() || row<0 || height<0 || row+height>Rows() || column<0 || width<0 || column+width>Columns()</tt>
        */

        public AbstractMatrix3D vPart(int slice, int row, int column, int depth, int height, int width)
        {
            checkBox(slice, row, column, depth, height, width);

            m_sliceZero += m_sliceStride*slice;
            m_rowZero += m_rowStride*row;
            m_columnZero += m_columnStride*column;

            m_intSlices = depth;
            m_intRows = height;
            m_intColumns = width;

            isNoView = false;
            return this;
        }

        /**
        Self modifying version of viewRowFlip().
        */

        public AbstractMatrix3D vRowFlip()
        {
            if (m_intRows > 0)
            {
                m_rowZero += (m_intRows - 1)*m_rowStride;
                m_rowStride = -m_rowStride;
                isNoView = false;
            }
            return this;
        }

        /**
        Self modifying version of viewSliceFlip().
        */

        public AbstractMatrix3D vSliceFlip()
        {
            if (m_intSlices > 0)
            {
                m_sliceZero += (m_intSlices - 1)*m_sliceStride;
                m_sliceStride = -m_sliceStride;
                isNoView = false;
            }
            return this;
        }

        /**
        Self modifying version of viewStrides().
        @throws	HCException if <tt>sliceStride<=0 || rowStride<=0 || columnStride<=0</tt>.
        */

        public AbstractMatrix3D vStrides(int sliceStride, int rowStride, int columnStride)
        {
            if (sliceStride <= 0 || rowStride <= 0 || columnStride <= 0)
            {
                throw new HCException("illegal strides: " + sliceStride + ", " + rowStride + ", " + columnStride);
            }

            m_sliceStride *= sliceStride;
            m_rowStride *= rowStride;
            m_columnStride *= columnStride;

            if (m_intSlices != 0)
            {
                m_intSlices = (m_intSlices - 1)/sliceStride + 1;
            }
            if (m_intRows != 0)
            {
                m_intRows = (m_intRows - 1)/rowStride + 1;
            }
            if (m_intColumns != 0)
            {
                m_intColumns = (m_intColumns - 1)/columnStride + 1;
            }

            isNoView = false;
            return this;
        }
    }
}
