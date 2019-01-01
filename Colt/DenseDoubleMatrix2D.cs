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

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    Dense 2-d matrix holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Internally holds one single contigous one-dimensional array, addressed in row major. 
    Note that this implementation is not .
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 8*Rows()*Columns()</tt>.
    Thus, a 1000*1000 matrix uses 8 MB.
    <p>
    <b>Time complexity:</b>
    <p>
    <tt>O(1)</tt> (i.e. constant time) for the basic operations
    <tt>get</tt>, <tt>getQuick</tt>, <tt>set</tt>, <tt>setQuick</tt> and <tt>m_intSize</tt>,
    <p>
    Cells are internally addressed in row-major.
    Applications demanding utmost speed can exploit this fact.
    Setting/getting values in a loop row-by-row is quicker than column-by-column.
    Thus
    <pre>
       for (int row=0; row < m_intRows; row++) {
          for (int column=0; column < m_intColumns; column++) {
             matrix.setQuick(row,column,someValue);
          }
       }
    </pre>
    is quicker than
    <pre>
       for (int column=0; column < m_intColumns; column++) {
          for (int row=0; row < m_intRows; row++) {
             matrix.setQuick(row,column,someValue);
          }
       }
    </pre>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DenseDoubleMatrix2D : DoubleMatrix2D
    {
        //static long m_serialVersionUID = 1020177651L;
        /**
          * The elements of this matrix.
          * elements are stored in row major, i.e.
          * index==row*m_intColumns + column
          * columnOf(index)==index%m_intColumns
          * rowOf(index)==index/m_intColumns
          * i.e. {row0 column0..m}, {row1 column0..m}, ..., {rown column0..m}
          */
        public double[] m_elements;
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

        public DenseDoubleMatrix2D(double[,] values)
            : this(values.GetLength(0), values.GetLength(0) == 0 ? 0 : values.GetLength(1))
        {
            assign(values);
        }

        /**
         * Constructs a matrix with a given number of m_intRows and m_intColumns.
         * All entries are initially <tt>0</tt>.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @throws	ArgumentException if <tt>m_intRows<0 || m_intColumns<0 || (double)m_intColumns*m_intRows > int.MaxValue</tt>.
         */

        public DenseDoubleMatrix2D(int m_intRows, int m_intColumns)
        {
            setUp(m_intRows, m_intColumns);
            m_elements = new double[m_intRows*m_intColumns];
        }

        /**
         * Constructs a view with the given parameters.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param elements the cells.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two m_intColumns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @throws	ArgumentException if <tt>m_intRows<0 || m_intColumns<0 || (double)m_intColumns*m_intRows > int.MaxValue</tt> or flip's are illegal.
         */

        public DenseDoubleMatrix2D(int m_intRows, int m_intColumns, double[] elements, int rowZero, int columnZero,
                                   int rowStride, int columnStride)
        {
            setUp(m_intRows, m_intColumns, rowZero, columnZero, rowStride, columnStride);
            m_elements = elements;
            isNoView = false;
        }

        /**
         * Sets all cells to the state specified by <tt>values</tt>.
         * <tt>values</tt> is required to have the form <tt>values[row,column]</tt>
         * and have exactly the same number of m_intRows and m_intColumns as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param    values the values to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         * @throws ArgumentException if <tt>values.Length != Rows() || for any 0 &lt;= row &lt; Rows(): values.GetLength(1) != Columns()</tt>.
         */

        public new DoubleMatrix2D assign(double[,] values)
        {
            if (isNoView)
            {
                if (values.GetLength(0) != m_intRows)
                {
                    throw new ArgumentException("Must have same number of m_intRows: m_intRows=" + values.GetLength(0) +
                                                "Rows()=" + Rows());
                }
                int i = m_intColumns*(m_intRows - 1);
                for (int row = m_intRows; --row >= 0;)
                {
                    double[] currentRow =
                        ArrayHelper.GetRowCopy<double>(
                            values, row);
                    if (values.GetLength(1) != m_intColumns)
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
         * Sets all cells to the state specified by <tt>value</tt>.
         * @param    value the value to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         */

        public new DoubleMatrix2D assign(double value)
        {
            double[] elems = m_elements;
            int index_ = index(0, 0);
            int cs = m_columnStride;
            int rs = m_rowStride;
            for (int row = m_intRows; --row >= 0;)
            {
                for (int i = index_, column = m_intColumns; --column >= 0;)
                {
                    elems[i] = value;
                    i += cs;
                }
                index_ += rs;
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

        public new DoubleMatrix2D assign(
            DoubleFunction function)
        {
            double[] elems = m_elements;
            if (elems == null)
            {
                throw new HCException();
            }
            int index_ = index(0, 0);
            int cs = m_columnStride;
            int rs = m_rowStride;

            // specialization for speed
            if (function is Mult)
            {
                // x[i] = mult*x[i]
                double multiplicator = ((Mult) function).m_multiplicator;
                if (multiplicator == 1)
                {
                    return this;
                }
                if (multiplicator == 0)
                {
                    return assign(0);
                }
                for (int row = m_intRows; --row >= 0;)
                {
                    // the general case
                    for (int i = index_, column = m_intColumns; --column >= 0;)
                    {
                        elems[i] *= multiplicator;
                        i += cs;
                    }
                    index_ += rs;
                }
            }
            else
            {
                // the general case x[i] = f(x[i])
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int i = index_, column = m_intColumns; --column >= 0;)
                    {
                        elems[i] = function.Apply(elems[i]);
                        i += cs;
                    }
                    index_ += rs;
                }
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of m_intRows and m_intColumns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     source   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Columns() != source.Columns() || Rows() != source.Rows()</tt>
         */

        public new DoubleMatrix2D assign(DoubleMatrix2D source)
        {
            // overriden for performance only
            if (!(source is DenseDoubleMatrix2D))
            {
                return base.assign(source);
            }
            DenseDoubleMatrix2D other = (DenseDoubleMatrix2D) source;
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
                DoubleMatrix2D c = other.Copy();
                if (!(c is DenseDoubleMatrix2D))
                {
                    // should not happen
                    return base.assign(other);
                }
                other = (DenseDoubleMatrix2D) c;
            }

            double[] elems = m_elements;
            double[] otherElems = other.m_elements;
            if (elems == null || otherElems == null)
            {
                throw new HCException();
            }
            int cs = m_columnStride;
            int ocs = other.m_columnStride;
            int rs = m_rowStride;
            int ors = other.m_rowStride;

            int otherIndex = other.index(0, 0);
            int index_ = index(0, 0);
            for (int row = m_intRows; --row >= 0;)
            {
                for (int i = index_, j = otherIndex, column = m_intColumns; --column >= 0;)
                {
                    elems[i] = otherElems[j];
                    i += cs;
                    j += ocs;
                }
                index_ += rs;
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

        public new DoubleMatrix2D assign(
            DoubleMatrix2D y,
            DoubleDoubleFunction function)
        {
            // overriden for performance only
            if (!(y is DenseDoubleMatrix2D))
            {
                return base.assign(y, function);
            }
            DenseDoubleMatrix2D other = (DenseDoubleMatrix2D) y;
            checkShape(y);

            double[] elems = m_elements;
            double[] otherElems = other.m_elements;
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

            // specialized for speed
            if (function == Functions.m_mult)
            {
                // x[i] = x[i] * y[i]
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                    {
                        elems[i] *= otherElems[j];
                        i += cs;
                        j += ocs;
                    }
                    index2 += rs;
                    otherIndex += ors;
                }
            }
            else if (function == Functions.m_div)
            {
                // x[i] = x[i] / y[i]
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                    {
                        elems[i] /= otherElems[j];
                        i += cs;
                        j += ocs;
                    }
                    index2 += rs;
                    otherIndex += ors;
                }
            }
            else if (function is PlusMult)
            {
                double multiplicator = ((PlusMult) function).m_multiplicator;
                if (multiplicator == 0)
                {
                    // x[i] = x[i] + 0*y[i]
                    return this;
                }
                else if (multiplicator == 1)
                {
                    // x[i] = x[i] + y[i]
                    for (int row = m_intRows; --row >= 0;)
                    {
                        for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                        {
                            elems[i] += otherElems[j];
                            i += cs;
                            j += ocs;
                        }
                        index2 += rs;
                        otherIndex += ors;
                    }
                }
                else if (multiplicator == -1)
                {
                    // x[i] = x[i] - y[i]
                    for (int row = m_intRows; --row >= 0;)
                    {
                        for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                        {
                            elems[i] -= otherElems[j];
                            i += cs;
                            j += ocs;
                        }
                        index2 += rs;
                        otherIndex += ors;
                    }
                }
                else
                {
                    // the general case
                    for (int row = m_intRows; --row >= 0;)
                    {
                        // x[i] = x[i] + mult*y[i]
                        for (int i = index2, j = otherIndex, column = m_intColumns; --column >= 0;)
                        {
                            elems[i] += multiplicator*otherElems[j];
                            i += cs;
                            j += ocs;
                        }
                        index2 += rs;
                        otherIndex += ors;
                    }
                }
            }
            else
            {
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

        public override double getQuick(int row, int column)
        {
            //if (debug) if (column<0 || column>=m_intColumns || row<0 || row>=m_intRows) throw new HCException("row:"+row+", column:"+column);
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

        public new bool haveSharedCellsRaw(DoubleMatrix2D other)
        {
            if (other is SelectedDenseDoubleMatrix2D)
            {
                SelectedDenseDoubleMatrix2D otherMatrix = (SelectedDenseDoubleMatrix2D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is DenseDoubleMatrix2D)
            {
                DenseDoubleMatrix2D otherMatrix = (DenseDoubleMatrix2D) other;
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
            return new DenseDoubleMatrix2D(m_intRows, m_intColumns);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param m_intSize the number of cells the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix1D like1D(int intSize)
        {
            return new DenseDoubleMatrix1D(intSize);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param m_intSize the number of cells the matrix shall have.
         * @param zero the index of the first element.
         * @param m_intStride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix1D like1D(int intSize, int zero, int intStride)
        {
            return new DenseDoubleMatrix1D(intSize, m_elements, zero, intStride);
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

        public override DoubleMatrix2D viewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseDoubleMatrix2D(m_elements, rowOffsets, columnOffsets, 0);
        }

        /**
        8 neighbor stencil transformation. For efficient finite difference operations.
        Applies a function to a moving <tt>3 x 3</tt> window.
        Does nothing if <tt>Rows() < 3 || Columns() < 3</tt>.
        <pre>
        B[i,j] = function.Apply(
        &nbsp;&nbsp;&nbsp;A[i-1,j-1], A[i-1,j], A[i-1,j+1],
        &nbsp;&nbsp;&nbsp;A[i,  j-1], A[i,  j], A[i,  j+1],
        &nbsp;&nbsp;&nbsp;A[i+1,j-1], A[i+1,j], A[i+1,j+1]
        &nbsp;&nbsp;&nbsp;)

        x x x - &nbsp;&nbsp;&nbsp; - x x x &nbsp;&nbsp;&nbsp; - - - - 
        x o x - &nbsp;&nbsp;&nbsp; - x o x &nbsp;&nbsp;&nbsp; - - - - 
        x x x - &nbsp;&nbsp;&nbsp; - x x x ... - x x x 
        - - - - &nbsp;&nbsp;&nbsp; - - - - &nbsp;&nbsp;&nbsp; - x o x 
        - - - - &nbsp;&nbsp;&nbsp; - - - - &nbsp;&nbsp;&nbsp; - x x x 
        </pre>
        Make sure that cells of <tt>this</tt> and <tt>B</tt> do not overlap.
        In case of overlapping views, behaviour is unspecified.
        </pre>
        <p>
        <b>Example:</b>
        <pre>
         double alpha = 0.25;
         double beta = 0.75;

        // 8 neighbors
        Double9Function f = new Double9Function() {
        &nbsp;&nbsp;&nbsp;public  double Apply(
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a00, double a01, double a02,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a10, double a11, double a12,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a20, double a21, double a22) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return beta*a11 + alpha*(a00+a01+a02 + a10+a12 + a20+a21+a22);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
        };
        A.zAssign8Neighbors(B,f);

        // 4 neighbors
        Double9Function g = new Double9Function() {
        &nbsp;&nbsp;&nbsp;public  double Apply(
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a00, double a01, double a02,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a10, double a11, double a12,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a20, double a21, double a22) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return beta*a11 + alpha*(a01+a10+a12+a21);
        &nbsp;&nbsp;&nbsp;}
        C.zAssign8Neighbors(B,g); // fast, even though it doesn't look like it
        };
        </pre>
	
        @param B the matrix to hold the results.
        @param function the function to be applied to the 9 cells.
        @throws HCException if <tt>function==null</tt>.
        @throws ArgumentException if <tt>Rows() != B.Rows() || Columns() != B.Columns()</tt>.
        */

        public new void zAssign8Neighbors(
            DoubleMatrix2D B,
            Double9Function function)
        {
            // 1. using only 4-5 out of the 9 cells in "function" is *not* the limiting factor for performance.

            // 2. if the "function" would be hardwired into the innermost loop, a speedup of 1.5-2.0 would be seen
            // but then the multi-purpose interface is gone...

            if (!(B is DenseDoubleMatrix2D))
            {
                base.zAssign8Neighbors(B, function);
                return;
            }
            if (function == null)
            {
                throw new HCException("function must not be null.");
            }
            checkShape(B);
            int r = m_intRows - 1;
            int c = m_intColumns - 1;
            if (m_intRows < 3 || m_intColumns < 3)
            {
                return; // nothing to do
            }

            DenseDoubleMatrix2D BB = (DenseDoubleMatrix2D) B;
            int A_rs = m_rowStride;
            int B_rs = BB.m_rowStride;
            int A_cs = m_columnStride;
            int B_cs = BB.m_columnStride;
            double[] elems = m_elements;
            double[] B_elems = BB.m_elements;
            if (elems == null || B_elems == null)
            {
                throw new HCException();
            }

            int A_index = index(1, 1);
            int B_index = BB.index(1, 1);
            for (int i = 1; i < r; i++)
            {
                double a00, a01, a02;
                double a10, a11, a12;
                double a20, a21, a22;

                int B11 = B_index;

                int A02 = A_index - A_rs - A_cs;
                int A12 = A02 + A_rs;
                int A22 = A12 + A_rs;

                // in each step six cells can be remembered in registers - they don't need to be reread from slow memory
                a00 = elems[A02];
                A02 += A_cs;
                a01 = elems[A02]; //A02+=A_cs;
                a10 = elems[A12];
                A12 += A_cs;
                a11 = elems[A12]; //A12+=A_cs;
                a20 = elems[A22];
                A22 += A_cs;
                a21 = elems[A22]; //A22+=A_cs; 

                for (int j = 1; j < c; j++)
                {
                    //in each step 3 instead of 9 cells need to be read from memory.
                    a02 = elems[A02 += A_cs];
                    a12 = elems[A12 += A_cs];
                    a22 = elems[A22 += A_cs];

                    B_elems[B11] = function.Apply(
                        a00, a01, a02,
                        a10, a11, a12,
                        a20, a21, a22);
                    B11 += B_cs;

                    // move remembered cells
                    a00 = a01;
                    a01 = a02;
                    a10 = a11;
                    a11 = a12;
                    a20 = a21;
                    a21 = a22;
                }
                A_index += A_rs;
                B_index += B_rs;
            }
        }

        public new DoubleMatrix1D zMult(DoubleMatrix1D y, DoubleMatrix1D z, double alpha, double beta, bool transposeA)
        {
            if (transposeA)
            {
                return viewDice().zMult(y, z, alpha, beta, false);
            }
            if (z == null)
            {
                z = new DenseDoubleMatrix1D(m_intRows);
            }
            if (!(y is DenseDoubleMatrix1D && z is DenseDoubleMatrix1D))
            {
                return base.zMult(y, z, alpha, beta, transposeA);
            }

            if (m_intColumns != y.m_intSize || m_intRows > z.m_intSize)
            {
                throw new ArgumentException("Incompatible args: " + toStringShort() + ", " + y.toStringShort() + ", " +
                                            z.toStringShort());
            }

            DenseDoubleMatrix1D yy = (DenseDoubleMatrix1D) y;
            DenseDoubleMatrix1D zz = (DenseDoubleMatrix1D) z;
            double[] AElems = m_elements;
            double[] yElems = yy.m_elements;
            double[] zElems = zz.m_elements;
            if (AElems == null || yElems == null || zElems == null)
            {
                throw new HCException();
            }
            int As = m_columnStride;

            int ys = yy.m_intStride;
            int zs = zz.m_intStride;

            int indexA = index(0, 0);
            int indexY = yy.index(0);
            int indexZ = zz.index(0);

            int cols = m_intColumns;
            for (int row = m_intRows; --row >= 0;)
            {
                double sum = 0;

                /*
                // not loop unrolled
                for (int i=indexA, j=indexY, column=m_intColumns; --column >= 0; ) {
                    sum += AElems[i] * yElems[j];
                    i += As;
                    j += ys;
                }
                */

                // loop unrolled
                int i = indexA - As;
                int j = indexY - ys;
                for (int k = cols%4; --k >= 0;)
                {
                    sum += AElems[i += As]*yElems[j += ys];
                }
                for (int k = cols/4; --k >= 0;)
                {
                    sum += AElems[i += As]*yElems[j += ys] +
                           AElems[i += As]*yElems[j += ys] +
                           AElems[i += As]*yElems[j += ys] +
                           AElems[i += As]*yElems[j += ys];
                }

                zElems[indexZ] = alpha*sum + beta*zElems[indexZ];
                indexA += m_rowStride;
                indexZ += zs;
            }

            return z;
        }

        public new DoubleMatrix2D zMult(DoubleMatrix2D B, DoubleMatrix2D C, double alpha, double beta, bool transposeA,
                                        bool transposeB)
        {
            // overriden for performance only
            if (transposeA)
            {
                return viewDice().zMult(B, C, alpha, beta, false, transposeB);
            }
            if (B is SparseDoubleMatrix2D || B is RCDoubleMatrix2D)
            {
                // exploit quick sparse mult
                // A*B = (B' * A')'
                if (C == null)
                {
                    return B.zMult(this, null, alpha, beta, !transposeB, true).viewDice();
                }
                else
                {
                    B.zMult(this, C.viewDice(), alpha, beta, !transposeB, true);
                    return C;
                }
                /*
                 RCDoubleMatrix2D transB = new RCDoubleMatrix2D(B.m_intColumns,B.m_intRows);
                B.forEachNonZero(
                    new IntIntDoubleFunction() {
                        public double Apply(int i, int j, double value) {
                            transB.setQuick(j,i,value);
                            return value;
                        }
                    }
                );

                return transB.zMult(viewDice(),C.viewDice()).viewDice();
                */
            }
            if (transposeB)
            {
                return zMult(B.viewDice(), C, alpha, beta, transposeA, false);
            }

            int m = m_intRows;
            int n = m_intColumns;
            int p = B.m_intColumns;
            if (C == null)
            {
                C = new DenseDoubleMatrix2D(m, p);
            }
            if (!(C is DenseDoubleMatrix2D))
            {
                return base.zMult(B, C, alpha, beta, transposeA, transposeB);
            }
            if (B.m_intRows != n)
            {
                throw new ArgumentException("Matrix2D inner dimensions must agree:" + toStringShort() + ", " +
                                            B.toStringShort());
            }
            if (C.m_intRows != m || C.m_intColumns != p)
            {
                throw new ArgumentException("Incompatibel result matrix: " + toStringShort() + ", " + B.toStringShort() +
                                            ", " + C.toStringShort());
            }
            if (this == C || B == C)
            {
                throw new ArgumentException("Matrices must not be identical");
            }

            DenseDoubleMatrix2D BB = (DenseDoubleMatrix2D) B;
            DenseDoubleMatrix2D CC = (DenseDoubleMatrix2D) C;
            double[] AElems = m_elements;
            double[] BElems = BB.m_elements;
            double[] CElems = CC.m_elements;
            if (AElems == null || BElems == null || CElems == null)
            {
                throw new HCException();
            }

            int cA = m_columnStride;
            int cB = BB.m_columnStride;
            int cC = CC.m_columnStride;

            int rA = m_rowStride;
            int rB = BB.m_rowStride;
            int rC = CC.m_rowStride;

            /*
            A is blocked to hide memory latency
                    xxxxxxx B
                    xxxxxxx
                    xxxxxxx
            A
            xxx     xxxxxxx C
            xxx     xxxxxxx
            ---     -------
            xxx     xxxxxxx
            xxx     xxxxxxx
            ---     -------
            xxx     xxxxxxx
            */
            int BLOCK_SIZE = 30000; // * 8 == Level 2 cache in bytes
            //if (n+p == 0) return C;
            //int m_optimal = (BLOCK_SIZE - n*p) / (n+p);
            int m_optimal = (BLOCK_SIZE - n)/(n + 1);
            if (m_optimal <= 0)
            {
                m_optimal = 1;
            }
            int blocks = m/m_optimal;
            int rr = 0;
            if (m%m_optimal != 0)
            {
                blocks++;
            }
            for (; --blocks >= 0;)
            {
                int jB = BB.index(0, 0);
                int indexA = index(rr, 0);
                int jC = CC.index(rr, 0);
                rr += m_optimal;
                if (blocks == 0)
                {
                    m_optimal += m - rr;
                }

                for (int j = p; --j >= 0;)
                {
                    int iA = indexA;
                    int iC = jC;
                    for (int i = m_optimal; --i >= 0;)
                    {
                        int kA = iA;
                        int kB = jB;
                        double s = 0;

                        /*
                        // not unrolled:
                        for (int k = n; --k >= 0; ) {
                            //s += getQuick(i,k) * B.getQuick(k,j);
                            s += AElems[kA] * BElems[kB];
                            kB += rB;
                            kA += cA;
                        }
                        */

                        // loop unrolled				
                        kA -= cA;
                        kB -= rB;

                        for (int k = n%4; --k >= 0;)
                        {
                            s += AElems[kA += cA]*BElems[kB += rB];
                        }
                        for (int k = n/4; --k >= 0;)
                        {
                            s += AElems[kA += cA]*BElems[kB += rB] +
                                 AElems[kA += cA]*BElems[kB += rB] +
                                 AElems[kA += cA]*BElems[kB += rB] +
                                 AElems[kA += cA]*BElems[kB += rB];
                        }

                        CElems[iC] = alpha*s + beta*CElems[iC];
                        iA += rA;
                        iC += rC;
                    }
                    jB += cB;
                    jC += cC;
                }
            }
            return C;
        }

        /**
         * Returns the sum of all cells; <tt>Sum( x[i,j] )</tt>.
         * @return the sum.
         */

        public new double zSum()
        {
            double sum = 0;
            double[] elems = m_elements;
            if (elems == null)
            {
                throw new HCException();
            }
            int index2 = index(0, 0);
            int cs = m_columnStride;
            int rs = m_rowStride;
            for (int row = m_intRows; --row >= 0;)
            {
                for (int i = index2, column = m_intColumns; --column >= 0;)
                {
                    sum += elems[i];
                    i += cs;
                }
                index2 += rs;
            }
            return sum;
        }
    }
}
