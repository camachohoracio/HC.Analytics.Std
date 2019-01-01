#region

using System;
using HC.Core.Exceptions;
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

    ////import AppendObjectMatrix1D;
    ////import ObjectMatrix2D;
    /**
    Dense 2-d matrix holding <tt>Object</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Internally holds one single contigous one-dimensional array, addressed in row major. 
    Note that this implementation is not lock.
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 8*Rows()*Columns()</tt>.
    Thus, a 1000*1000 matrix uses 8 MB.
    <p>
    <b>Time complexity:</b>
    <p>
    <tt>O(1)</tt> (i.e. constant time) for the basic operations
    <tt>get</tt>, <tt>getQuick</tt>, <tt>set</tt>, <tt>setQuick</tt> and <tt>size</tt>,
    <p>
    Cells are internally addressed in row-major.
    Applications demanding utmost speed can exploit this fact.
    Setting/getting values in a loop row-by-row is quicker than column-by-column.
    Thus
    <pre>
       for (int row=0; row < rows_; row++) {
          for (int column=0; column < m_intColumns; column++) {
             matrix.setQuick(row,column,someValue);
          }
       }
    </pre>
    is quicker than
    <pre>
       for (int column=0; column < m_intColumns; column++) {
          for (int row=0; row < rows_; row++) {
             matrix.setQuick(row,column,someValue);
          }
       }
    </pre>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DenseObjectMatrix2D : ObjectMatrix2D
    {
        /**
          * The elements of this matrix.
          * elements are stored in row major, i.e.
          * index==row*m_intColumns + column
          * columnOf(index)==index%m_intColumns
          * rowOf(index)==index/m_intColumns
          * i.e. {row0 column0..m}, {row1 column0..m}, ..., {rown column0..m}
          */
        public Object[] m_elements;
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

        public DenseObjectMatrix2D(Object[,] values)
            : this(values.GetLength(0), values.GetLength(0) == 0 ? 0 : values.GetLength(1))
        {
            assign(values);
        }

        /**
         * Constructs a matrix with a given number of rows_ and m_intColumns.
         * All entries are initially <tt>0</tt>.
         * @param rows_ the number of rows_ the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @throws	ArgumentException if <tt>rows_<0 || m_intColumns<0 || (Object)m_intColumns*rows_ > int.MaxValue</tt>.
         */

        public DenseObjectMatrix2D(int rows_, int m_intColumns)
        {
            setUp(rows_, m_intColumns);
            m_elements = new Object[rows_*m_intColumns];
        }

        /**
         * Constructs a view with the given parameters.
         * @param rows_ the number of rows_ the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param elements the cells.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two rows_, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two m_intColumns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @throws	ArgumentException if <tt>rows_<0 || m_intColumns<0 || (Object)m_intColumns*rows_ > int.MaxValue</tt> or flip's are illegal.
         */

        public DenseObjectMatrix2D(int rows_, int m_intColumns, Object[] elements, int rowZero, int columnZero,
                                   int rowStride, int columnStride)
        {
            setUp(rows_, m_intColumns, rowZero, columnZero, rowStride, columnStride);
            m_elements = elements;
            isNoView = false;
        }

        /**
         * Sets all cells to the state specified by <tt>values</tt>.
         * <tt>values</tt> is required to have the form <tt>values[row,column]</tt>
         * and have exactly the same number of rows_ and m_intColumns as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param    values the values to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         * @throws ArgumentException if <tt>values.Length != Rows() || for any 0 &lt;= row &lt; Rows(): values.GetLength(1) != Columns()</tt>.
         */

        public new ObjectMatrix2D assign(Object[,] values)
        {
            if (isNoView)
            {
                if (values.GetLength(0) != m_intRows)
                {
                    throw new ArgumentException("Must have same number of rows_: rows_=" + values.GetLength(0) +
                                                "Rows()=" + Rows());
                }

                int i = m_intColumns*(m_intRows - 1);
                for (int row = m_intRows; --row >= 0;)
                {
                    Object[] currentRow =
                        ArrayHelper.GetRowCopy(values, row);

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
            else
            {
                base.assign(values);
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col])</tt>.
        <p>
        <b>Example:</b>
        <pre>
        matrix = 2 x 2 matrix
        0.5 1.5      
        2.5 3.5

        // change each cell to its sine
        matrix.assign(Functions.sin);
        -->
        2 x 2 matrix
        0.479426  0.997495 
        0.598472 -0.350783
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param function a function object taking as argument the current cell's value.
        @return <tt>this</tt> (for convenience only).
        @see Functions
        */

        public new ObjectMatrix2D assign(ObjectFunction function)
        {
            Object[] elems = m_elements;
            if (elems == null)
            {
                throw new HCException();
            }
            int index2 = index(0, 0);
            int cs = m_columnStride;
            int rs = m_rowStride;

            // the general case x[i] = f(x[i])
            for (int row = m_intRows; --row >= 0;)
            {
                for (int i = index2, column = m_intColumns; --column >= 0;)
                {
                    elems[i] = function.Apply(elems[i]);
                    i += cs;
                }
                index2 += rs;
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of rows_ and m_intColumns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     source   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Columns() != source.Columns() || Rows() != source.Rows()</tt>
         */

        public new ObjectMatrix2D assign(ObjectMatrix2D source)
        {
            // overriden for performance only
            if (!(source is DenseObjectMatrix2D))
            {
                return base.assign(source);
            }
            DenseObjectMatrix2D other = (DenseObjectMatrix2D) source;
            if (other == this)
            {
                return this; // nothing to do
            }
            checkShape(other);

            if (isNoView && other.isNoView)
            {
                // quickest
                Array.Copy(other.m_elements, 0, m_elements, 0, m_elements.Length);
                return this;
            }

            if (haveSharedCells(other))
            {
                ObjectMatrix2D c = other.Copy();
                if (!(c is DenseObjectMatrix2D))
                {
                    // should not happen
                    return base.assign(other);
                }
                other = (DenseObjectMatrix2D) c;
            }

            Object[] elems = m_elements;
            Object[] otherElems = other.m_elements;
            if (m_elements == null || otherElems == null)
            {
                throw new HCException();
            }
            int cs = m_columnStride;
            int ocs = other.m_columnStride;
            int rs = m_rowStride;
            int ors = other.m_rowStride;

            int otherIndex = other.index(0, 0);
            int index2 = index(0, 0);
            for (int row = m_intRows; --row >= 0;)
            {
                for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                {
                    elems[i] = otherElems[j];
                    i += cs;
                    j += ocs;
                }
                index2 += rs;
                otherIndex += ors;
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col],y[row,col])</tt>.
        <p>
        <b>Example:</b>
        <pre>
        // assign x[row,col] = x[row,col]<sup>y[row,col]</sup>
        m1 = 2 x 2 matrix 
        0 1 
        2 3

        m2 = 2 x 2 matrix 
        0 2 
        4 6

        m1.assign(m2, Functions.pow);
        -->
        m1 == 2 x 2 matrix
         1   1 
        16 729
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param y the secondary matrix to operate on.
        @param function a function object taking as first argument the current cell's value of <tt>this</tt>,
        and as second argument the current cell's value of <tt>y</tt>,
        @return <tt>this</tt> (for convenience only).
        @throws	ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>
        @see Functions
        */

        public new ObjectMatrix2D assign(ObjectMatrix2D y, ObjectObjectFunction function)
        {
            // overriden for performance only
            if (!(y is DenseObjectMatrix2D))
            {
                return base.assign(y, function);
            }
            DenseObjectMatrix2D other = (DenseObjectMatrix2D) y;
            checkShape(y);

            Object[] elems = m_elements;
            Object[] otherElems = other.m_elements;
            if (elems == null || otherElems == null)
            {
                throw new HCException();
            }
            int cs = m_columnStride;
            int ocs = other.m_columnStride;
            int rs = m_rowStride;
            int ors = other.m_rowStride;

            int otherIndex = other.index(0, 0);
            int index2 = index(0, 0);

            // the general case x[i] = f(x[i],y[i])
            for (int row = m_intRows; --row >= 0;)
            {
                for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                {
                    elems[i] = function.Apply(elems[i], otherElems[j]);
                    i += cs;
                    j += ocs;
                }
                index2 += rs;
                otherIndex += ors;
            }
            return this;
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

        public override Object getQuick(int row, int column)
        {
            //if (debug) if (column<0 || column>=m_intColumns || row<0 || row>=rows_) throw new HCException("row:"+row+", column:"+column);
            //return elements[index(row,column)];
            //manually inlined:
            return m_elements[m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride];
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

        public new bool haveSharedCellsRaw(ObjectMatrix2D other)
        {
            if (other is SelectedDenseObjectMatrix2D)
            {
                SelectedDenseObjectMatrix2D otherMatrix = (SelectedDenseObjectMatrix2D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is DenseObjectMatrix2D)
            {
                DenseObjectMatrix2D otherMatrix = (DenseObjectMatrix2D) other;
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
            // return base.index(row,column);
            // manually inlined for speed:
            return m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows_ and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix2D</tt> the new matrix must also be of type <tt>DenseObjectMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix2D</tt> the new matrix must also be of type <tt>SparseObjectMatrix2D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param rows_ the number of rows_ the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override ObjectMatrix2D like(int rows_, int m_intColumns)
        {
            return new DenseObjectMatrix2D(rows_, m_intColumns);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix2D</tt> the new matrix must be of type <tt>DenseObjectMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix2D</tt> the new matrix must be of type <tt>SparseObjectMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override ObjectMatrix1D like1D(int size)
        {
            return new DenseObjectMatrix1D(size);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix2D</tt> the new matrix must be of type <tt>DenseObjectMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix2D</tt> the new matrix must be of type <tt>SparseObjectMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @param zero the index of the first element.
         * @param stride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override ObjectMatrix1D like1D(int size, int zero, int stride)
        {
            return new DenseObjectMatrix1D(size, m_elements, zero, stride);
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

        public override void setQuick(int row, int column, Object value)
        {
            //if (debug) if (column<0 || column>=m_intColumns || row<0 || row>=rows_) throw new HCException("row:"+row+", column:"+column);
            //elements[index(row,column)] = value;
            //manually inlined:
            m_elements[m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride] = value;
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override ObjectMatrix2D viewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseObjectMatrix2D(m_elements, rowOffsets, columnOffsets, 0);
        }
    }
}
