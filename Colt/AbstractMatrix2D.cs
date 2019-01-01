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
    Abstract base class for 2-d matrices holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Note that this implementation is not .</b>

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class AbstractMatrix2D : AbstractMatrix
    {
        /** the number of colums and rows this matrix (view) has */
        public int m_columnStride;
        public int m_columnZero;
        public int m_intColumns;
        public int m_intRows;

        /** the number of elements between two rows, i.e. <tt>index(i+1,j,k) - index(i,j,k)</tt>. */
        public int m_rowStride;

        /** the number of elements between two columns, i.e. <tt>index(i,j+1,k) - index(i,j,k)</tt>. */


        /** the index of the first element */
        public int m_rowZero;

        /** 
         * Indicates non-flipped state (flip==1) or flipped state (flip==-1).
         * see _setFlip() for further info.
         */
        //public int rowFlip, columnFlip;

        /** 
         * Indicates non-flipped state or flipped state.
         * see _setFlip() for further info.
         */
        //public int rowFlipMask, columnFlipMask;

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
            //return columnZero + ((rank+columnFlipMask)^columnFlipMask);
            //return columnZero + rank*columnFlip; // slower
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
            //return rowZero + ((rank+rowFlipMask)^rowFlipMask);
            //return rowZero + rank*rowFlip; // slower
        }

        /**
         * Checks whether the receiver contains the given box and throws an exception, if necessary.
         * @throws	HCException if <tt>column<0 || width<0 || column+width>Columns() || row<0 || height<0 || row+height>Rows()</tt>
         */

        public void checkBox(int row, int column, int height, int width)
        {
            if (column < 0 || width < 0 || column + width > m_intColumns || row < 0 || height < 0 ||
                row + height > m_intRows)
            {
                throw new HCException(toStringShort() + ", column:" + column + ", row:" + row + " ,width:" + width +
                                    ", height:" + height);
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
         * Sanity check for operations requiring two matrices with the same number of columns and rows.
         * @throws ArgumentException if <tt>Columns() != B.Columns() || Rows() != B.Rows()</tt>.
         */

        public void checkShape(AbstractMatrix2D B)
        {
            if (m_intColumns != B.m_intColumns || m_intRows != B.m_intRows)
            {
                throw new ArgumentException("Incompatible dimensions: " + toStringShort() + " and " + B.toStringShort());
            }
        }

        /**
         * Sanity check for operations requiring matrices with the same number of columns and rows.
         * @throws ArgumentException if <tt>Columns() != B.Columns() || Rows() != B.Rows() || Columns() != C.Columns() || Rows() != C.Rows()</tt>.
         */

        public void checkShape(AbstractMatrix2D B, AbstractMatrix2D C)
        {
            if (m_intColumns != B.m_intColumns || m_intRows != B.m_intRows || m_intColumns != C.m_intColumns ||
                m_intRows != C.m_intRows)
            {
                throw new ArgumentException("Incompatible dimensions: " + toStringShort() + ", " + B.toStringShort() +
                                            ", " + C.toStringShort());
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
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         */

        public int index(int row, int column)
        {
            return _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
        }

        /**
         * Returns the number of rows.
         */

        public int Rows()
        {
            return m_intRows;
        }

        /**
         * Sets up a matrix with a given number of rows and columns.
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @throws	ArgumentException if <tt>rows<0 || columns<0 || (double)columns*rows > int.MaxValue</tt>.
         */

        public void setUp(int rows, int columns)
        {
            setUp(rows, columns, 0, 0, columns, 1);
        }

        /**
         * Sets up a matrix with a given number of rows and columns and the given strides.
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two rows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two columns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @throws	ArgumentException if <tt>rows<0 || columns<0 || (double)columns*rows > int.MaxValue</tt> or flip's are illegal.
         */

        public void setUp(int rows, int columns, int rowZero, int columnZero, int rowStride, int columnStride)
        {
            if (rows < 0 || columns < 0)
            {
                throw new ArgumentException("negative size");
            }
            m_intRows = rows;
            m_intColumns = columns;

            m_rowZero = rowZero;
            m_columnZero = columnZero;

            m_rowStride = rowStride;
            m_columnStride = columnStride;

            isNoView = true;
            if ((double) columns*rows > int.MaxValue)
            {
                throw new ArgumentException("matrix too large");
            }
        }

        /**
         * Returns the number of cells which is <tt>Rows()*Columns()</tt>.
         */

        public override int Size()
        {
            return m_intRows*m_intColumns;
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

        public AbstractMatrix2D vColumnFlip()
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
        */

        public AbstractMatrix2D vDice()
        {
            int tmp;
            // swap;
            tmp = m_intRows;
            m_intRows = m_intColumns;
            m_intColumns = tmp;
            tmp = m_rowStride;
            m_rowStride = m_columnStride;
            m_columnStride = tmp;
            tmp = m_rowZero;
            m_rowZero = m_columnZero;
            m_columnZero = tmp;

            // flips stay unaffected

            isNoView = false;
            return this;
        }

        /**
        Self modifying version of viewPart().
        @throws	HCException if <tt>column<0 || width<0 || column+width>Columns() || row<0 || height<0 || row+height>Rows()</tt>
        */

        public AbstractMatrix2D vPart(int row, int column, int height, int width)
        {
            checkBox(row, column, height, width);
            m_rowZero += m_rowStride*row;
            m_columnZero += m_columnStride*column;
            m_intRows = height;
            m_intColumns = width;
            isNoView = false;
            return this;
        }

        /**
        Self modifying version of viewRowFlip().
        */

        public AbstractMatrix2D vRowFlip()
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
        Self modifying version of viewStrides().
        @throws	HCException if <tt>rowStride<=0 || columnStride<=0</tt>.
        */

        public AbstractMatrix2D vStrides(int rowStride, int columnStride)
        {
            if (rowStride <= 0 || columnStride <= 0)
            {
                throw new HCException("illegal strides: " + rowStride + ", " + columnStride);
            }
            rowStride *= rowStride;
            columnStride *= columnStride;
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
