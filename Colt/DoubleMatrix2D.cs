#region

using System;
using HC.Analytics.Colt.doubleAlgo;
using HC.Core.Exceptions;

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
    ////package matrix;

    ////import DoubleArrayList;
    ////import IntArrayList;
    ////import AbstractMatrix2D;
    ////import DenseDoubleMatrix1D;
    ////import DenseDoubleMatrix2D;
    /**
    Abstract base class for 2-d matrices holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    A matrix has a number of m_intRows and m_intColumns, which are assigned upon instance construction - The matrix's size is then <tt>Rows()*Columns()</tt>.
    Elements are accessed via <tt>[row,column]</tt> coordinates.
    Legal coordinates range from <tt>[0,0]</tt> to <tt>[Rows()-1,Columns()-1]</tt>.
    Any attempt to access an element at a coordinate <tt>column&lt;0 || column&gt;=Columns() || row&lt;0 || row&gt;=Rows()</tt> will throw an <tt>HCException</tt>.
    <p>
    <b>Note</b> that this implementation is not .

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class DoubleMatrix2D : AbstractMatrix2D
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
        Applies a function to each cell and aggregates the results.
        Returns a value <tt>v</tt> such that <tt>v==a(Size())</tt> where <tt>a(i) == aggr( a(i-1), f(get(row,column)) )</tt> and terminators are <tt>a(1) == f(get(0,0)), a(0)==double.NaN</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Functions F = Functions.functions;
        2 x 2 matrix
        0 1
        2 3

        // Sum( x[row,col]*x[row,col] ) 
        matrix.aggregate(Functions.plus,Functions.square);
        --> 14
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.
        @param f a function transforming the current cell value.
        @return the aggregated measure.
        @see Functions
        */

        public double aggregate(
            DoubleDoubleFunction aggr,
            DoubleFunction f)
        {
            if (Size() == 0)
            {
                return double.NaN;
            }
            double a = f.Apply(getQuick(m_intRows - 1, m_intColumns - 1));
            int d = 1; // last cell already done
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns - d; --column >= 0;)
                {
                    a = aggr.Apply(a, f.Apply(getQuick(row, column)));
                }
                d = 0;
            }
            return a;
        }

        /**
        Applies a function to each corresponding cell of two matrices and aggregates the results.
        Returns a value <tt>v</tt> such that <tt>v==a(Size())</tt> where <tt>a(i) == aggr( a(i-1), f(get(row,column),other.get(row,column)) )</tt> and terminators are <tt>a(1) == f(get(0,0),other.get(0,0)), a(0)==double.NaN</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Functions F = Functions.functions;
        x == 2 x 2 matrix
        0 1
        2 3

        y == 2 x 2 matrix
        0 1
        2 3

        // Sum( x[row,col] * y[row,col] ) 
        x.aggregate(y, Functions.plus, Functions.mult);
        --> 14

        // Sum( (x[row,col] + y[row,col])^2 )
        x.aggregate(y, Functions.plus, Functions.chain(Functions.square,Functions.plus));
        --> 56
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
        @param f a function transforming the current cell values.
        @return the aggregated measure.
        @throws	ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>
        @see Functions
        */

        public double aggregate(DoubleMatrix2D other,
                                DoubleDoubleFunction aggr,
                                DoubleDoubleFunction f)
        {
            checkShape(other);
            if (Size() == 0)
            {
                return double.NaN;
            }
            double a = f.Apply(getQuick(m_intRows - 1, m_intColumns - 1),
                               other.getQuick(m_intRows - 1, m_intColumns - 1));
            int d = 1; // last cell already done
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns - d; --column >= 0;)
                {
                    a = aggr.Apply(a, f.Apply(getQuick(row, column), other.getQuick(row, column)));
                }
                d = 0;
            }
            return a;
        }

        public double[,] GetArr()
        {
            double[,] arr = new double[m_intRows,m_intColumns];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    arr[i, j] = getQuick(i, j);
                }
            }
            return arr;
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

        public DoubleMatrix2D assign(double[,] values)
        {
            if (values.GetLength(0) != m_intRows)
            {
                throw new ArgumentException("Must have same number of m_intRows: m_intRows=" + values.GetLength(0) +
                                            "Rows()=" + Rows());
            }
            for (int row = m_intRows; --row >= 0;)
            {
                if (values.GetLength(1) != m_intColumns)
                {
                    throw new ArgumentException("Must have same number of m_intColumns in every row: m_intColumns=" +
                                                m_intColumns + "Columns()=" + Columns());
                }
                for (int column = m_intColumns; --column >= 0;)
                {
                    setQuick(row, column, values[row, column]);
                }
            }
            return this;
        }

        /**
         * Sets all cells to the state specified by <tt>value</tt>.
         * @param    value the value to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         */

        public DoubleMatrix2D assign(double value)
        {
            int r = m_intRows;
            int c = m_intColumns;
            //for (int row=m_intRows; --row >= 0;) {
            //	for (int column=m_intColumns; --column >= 0;) {
            for (int row = 0; row < r; row++)
            {
                for (int column = 0; column < c; column++)
                {
                    setQuick(row, column, value);
                }
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

        public DoubleMatrix2D assign(
            DoubleFunction function)
        {
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    setQuick(row, column, function.Apply(getQuick(row, column)));
                }
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of m_intRows and m_intColumns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     other   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Columns() != other.Columns() || Rows() != other.Rows()</tt>
         */

        public DoubleMatrix2D assign(DoubleMatrix2D other)
        {
            if (other == this)
            {
                return this;
            }
            checkShape(other);
            if (haveSharedCells(other))
            {
                other = other.Copy();
            }

            //for (int row=0; row<m_intRows; row++) {
            //for (int column=0; column<m_intColumns; column++) {
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    setQuick(row, column, other.getQuick(row, column));
                }
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

        public DoubleMatrix2D assign(
            DoubleMatrix2D y,
            DoubleDoubleFunction function)
        {
            checkShape(y);
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    setQuick(row, column, function.Apply(getQuick(row, column), y.getQuick(row, column)));
                }
            }
            return this;
        }

        /**
         * Returns the number of cells having non-zero values; ignores tolerance.
         */

        public int cardinality()
        {
            int cardinality = 0;
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    if (getQuick(row, column) != 0)
                    {
                        cardinality++;
                    }
                }
            }
            return cardinality;
        }

        /**
         * Constructs and returns a deep copy of the receiver.
         * <p>
         * <b>Note that the returned matrix is an independent deep copy.</b>
         * The returned matrix is not backed by this matrix, so changes in the returned matrix are not reflected in this matrix, and vice-versa. 
         *
         * @return  a deep copy of the receiver.
         */

        public DoubleMatrix2D Copy()
        {
            return like().assign(this);
        }

        /**
         * Returns whether all cells are equal to the given value.
         *
         * @param     value the value to test against.
         * @return    <tt>true</tt> if all cells are equal to the given value, <tt>false</tt> otherwise.
         */

        public bool Equals(double value)
        {
            return Property.DEFAULT.Equals(this, value);
        }

        /**
         * Compares this object against the specified object.
         * The result is <code>true</code> if and only if the argument is 
         * not <code>null</code> and is at least a <code>DoubleMatrix2D</code> object
         * that has the same number of m_intColumns and m_intRows as the receiver and 
         * has exactly the same values at the same coordinates.
         * @param   obj   the object to Compare with.
         * @return  <code>true</code> if the objects are the same;
         *          <code>false</code> otherwise.
         */

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!(obj is DoubleMatrix2D))
            {
                return false;
            }

            return Property.DEFAULT.Equals(this, (DoubleMatrix2D) obj);
        }

        /**
         * Assigns the result of a function to each <i>non-zero</i> cell; <tt>x[row,col] = function(x[row,col])</tt>.
         * Use this method for fast special-purpose iteration.
         * If you want to modify another matrix instead of <tt>this</tt> (i.e. work in read-only mode), simply return the input value unchanged.
         *
         * Parameters to function are as follows: <tt>first==row</tt>, <tt>second==column</tt>, <tt>third==nonZeroValue</tt>.
         *
         * @param function a function object taking as argument the current non-zero cell's row, column and value.
         * @return <tt>this</tt> (for convenience only).
         */

        public DoubleMatrix2D forEachNonZero(
            IntIntDoubleFunction function)
        {
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    double value = getQuick(row, column);
                    if (value != 0)
                    {
                        double r = function.Apply(row, column, value);
                        if (r != value)
                        {
                            setQuick(row, column, r);
                        }
                    }
                }
            }
            return this;
        }

        /**
         * Returns the matrix cell value at coordinate <tt>[row,column]</tt>.
         *
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @return    the value of the specified cell.
         * @throws	HCException if <tt>column&lt;0 || column&gt;=Columns() || row&lt;0 || row&gt;=Rows()</tt>
         */

        public double get(int row, int column)
        {
            if (column < 0 || column >= m_intColumns || row < 0 || row >= m_intRows)
            {
                throw new HCException("row:" + row + ", column:" + column);
            }
            return getQuick(row, column);
        }

        /**
         * Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
         * Override this method in wrappers.
         */

        public DoubleMatrix2D getContent()
        {
            return this;
        }

        /**
        Fills the coordinates and values of cells having non-zero values into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists all have a new size, the number of non-zero values.
        <p>
        In general, fill order is <i>unspecified</i>.
        This implementation fills like <tt>for (row = 0..m_intRows-1) for (column = 0..m_intColumns-1) do ... </tt>.
        However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
        (Of course, result lists indexes are guaranteed to correspond to the same cell).
        <p>
        <b>Example:</b>
        <br>
        <pre>
        2 x 3 matrix:
        0, 0, 8
        0, 7, 0
        -->
        rowList    = (0,1)
        columnList = (2,1)
        valueList  = (8,7)
        </pre>
        In other words, <tt>get(0,2)==8, get(1,1)==7</tt>.

        @param rowList the list to be filled with row indexes, can have any size.
        @param columnList the list to be filled with column indexes, can have any size.
        @param valueList the list to be filled with values, can have any size.
        */

        public void getNonZeros(IntArrayList rowList, IntArrayList columnList, DoubleArrayList valueList)
        {
            rowList.Clear();
            columnList.Clear();
            valueList.Clear();
            int r = m_intRows;
            int c = m_intColumns;
            for (int row = 0; row < r; row++)
            {
                for (int column = 0; column < c; column++)
                {
                    double value = getQuick(row, column);
                    if (value != 0)
                    {
                        rowList.Add(row);
                        columnList.Add(column);
                        valueList.Add(value);
                    }
                }
            }
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
        public abstract double getQuick(int row, int column);
        /**
         * Returns <tt>true</tt> if both matrices share at least one identical cell.
         */

        public bool haveSharedCells(DoubleMatrix2D other)
        {
            if (other == null)
            {
                return false;
            }
            if (this == other)
            {
                return true;
            }
            return getContent().haveSharedCellsRaw(other.getContent());
        }

        /**
         * Returns <tt>true</tt> if both matrices share at least one identical cell.
         */

        public bool haveSharedCellsRaw(DoubleMatrix2D other)
        {
            return false;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same number of m_intRows and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @return  a new empty matrix of the same dynamic type.
         */

        public DoubleMatrix2D like()
        {
            return like(m_intRows, m_intColumns);
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
        public abstract DoubleMatrix2D like(int m_intRows, int m_intColumns);
        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */
        public abstract DoubleMatrix1D like1D(int size);
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
        public abstract DoubleMatrix1D like1D(int size, int zero, int stride);
        /**
         * Sets the matrix cell at coordinate <tt>[row,column]</tt> to the specified value.
         *
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @param    value the value to be filled into the specified cell.
         * @throws	HCException if <tt>column&lt;0 || column&gt;=Columns() || row&lt;0 || row&gt;=Rows()</tt>
         */

        public void set(int row, int column, double value)
        {
            if (column < 0 || column >= m_intColumns || row < 0 || row >= m_intRows)
            {
                throw new HCException("row:" + row + ", column:" + column);
            }
            setQuick(row, column, value);
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
        public abstract void setQuick(int row, int column, double value);
        /**
         * Constructs and returns a 2-dimensional array containing the cell values.
         * The returned array <tt>values</tt> has the form <tt>values[row,column]</tt>
         * and has the same number of m_intRows and m_intColumns as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @return an array filled with the values of the cells.
         */

        public double[,] ToArray()
        {
            double[,] values = new double[m_intRows,m_intColumns];
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    values[row, column] = getQuick(row, column);
                }
            }
            return values;
        }

        /**
         * Returns a string representation using default formatting.
         * @see doublealgo.Formatter
         */

        public override string ToString()
        {
            return new FormatterDoubleAlgo().ToString(this);
        }

        /**
         * Constructs and returns a new view equal to the receiver.
         * The view is a shallow Clone. Calls <code>Clone()</code> and casts the result.
         * <p>
         * <b>Note that the view is not a deep copy.</b>
         * The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa. 
         * <p>
         * Use {@link #Copy()} to construct an independent deep copy rather than a new view.
         *
         * @return  a new view of the receiver.
         */

        public DoubleMatrix2D view()
        {
            DoubleMatrix2D doubleMatrix2D = (DoubleMatrix2D) Clone();
            return doubleMatrix2D;
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

        public DoubleMatrix1D viewColumn(int column)
        {
            checkColumn(column);
            int viewSize = m_intRows;
            int viewZero = index(0, column);
            int viewStride = m_rowStride;
            return like1D(viewSize, viewZero, viewStride);
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

        public DoubleMatrix2D viewColumnFlip()
        {
            return (DoubleMatrix2D) (view().vColumnFlip());
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

        public DoubleMatrix2D viewDice()
        {
            return (DoubleMatrix2D) (view().vDice());
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

        public DoubleMatrix2D viewPart(int row, int column, int height, int width)
        {
            return (DoubleMatrix2D) (view().vPart(row, column, height, width));
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

        public DoubleMatrix1D viewRow(int row)
        {
            checkRow(row);
            int viewSize = m_intColumns;
            int viewZero = index(row, 0);
            int viewStride = m_columnStride;
            return like1D(viewSize, viewZero, viewStride);
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

        public DoubleMatrix2D viewRowFlip()
        {
            return (DoubleMatrix2D) (view().vRowFlip());
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

        public DoubleMatrix2D viewSelection(int[] rowIndexes, int[] columnIndexes)
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
            int[] rowOffsets = new int[rowIndexes.Length];
            int[] columnOffsets = new int[columnIndexes.Length];
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowOffsets[i] = _rowOffset(_rowRank(rowIndexes[i]));
            }
            for (int i = columnIndexes.Length; --i >= 0;)
            {
                columnOffsets[i] = _columnOffset(_columnRank(columnIndexes[i]));
            }
            return viewSelectionLike(rowOffsets, columnOffsets);
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding all <b>m_intRows</b> matching the given condition.
        Applies the condition to each row and takes only those row where <tt>condition.Apply(viewRow(i))</tt> yields <tt>true</tt>.
        To match m_intColumns, use a dice view.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        // extract and view all m_intRows which have a value < threshold in the first column (representing "age")
         double threshold = 16;
        matrix.viewSelection( 
        &nbsp;&nbsp;&nbsp;new DoubleMatrix1DProcedure() {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public  bool Apply(DoubleMatrix1D m) { return m.get(0) < threshold; }
        &nbsp;&nbsp;&nbsp;}
        );

        // extract and view all m_intRows with RMS < threshold
        // The RMS (Root-Mean-Square) is a measure of the average "size" of the elements of a data sequence.
        matrix = 0 1 2 3
         double threshold = 0.5;
        matrix.viewSelection( 
        &nbsp;&nbsp;&nbsp;new DoubleMatrix1DProcedure() {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public  bool Apply(DoubleMatrix1D m) { return Math.Sqrt(m.aggregate(Functions.plus,Functions.square) / m.Size()) < threshold; }
        &nbsp;&nbsp;&nbsp;}
        );
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param  condition The condition to be matched.
        @return the new view.
        */

        public DoubleMatrix2D viewSelection(DoubleMatrix1DProcedure condition)
        {
            IntArrayList matches = new IntArrayList();

            for (int i = 0; i < m_intRows; i++)
            {
                if (condition.Apply(viewRow(i)))
                {
                    matches.Add(i);
                }
            }

            matches.trimToSize();
            return viewSelection(matches.elements(), null); // take all m_intColumns
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */
        public abstract DoubleMatrix2D viewSelectionLike(int[] rowOffsets, int[] columnOffsets);
        /**
        Sorts the matrix m_intRows into ascending order, according to the <i>natural ordering</i> of the matrix values in the given column.
        This sort is guaranteed to be <i>stable</i>.
        For further information, see {@link doublealgo.Sorting#Sort(DoubleMatrix2D,int)}.
        For more advanced sorting functionality, see {@link doublealgo.Sorting}.
        @return a new sorted vector (matrix) view.
        @throws HCException if <tt>column < 0 || column >= Columns()</tt>.
        */

        public DoubleMatrix2D viewSorted(int column)
        {
            return SortingDoubleAlgo.mergeSort.Sort(this, column);
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

        public DoubleMatrix2D viewStrides(int rowStride, int columnStride)
        {
            return (DoubleMatrix2D) (view().vStrides(rowStride, columnStride));
        }

        /**
         * Applies a procedure to each cell's value.
         * Iterates downwards from <tt>[Rows()-1,Columns()-1]</tt> to <tt>[0,0]</tt>,
         * as demonstrated by this snippet:
         * <pre>
         * for (int row=m_intRows; --row >=0;) {
         *    for (int column=m_intColumns; --column >= 0;) {
         *        if (!procedure.Apply(getQuick(row,column))) return false;
         *    }
         * }
         * return true;
         * </pre>
         * Note that an implementation may use more efficient techniques, but must not use any other order.
         *
         * @param procedure a procedure object taking as argument the current cell's value. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        private bool xforEach(
            DoubleProcedure procedure)
        {
            for (int row = m_intRows; --row >= 0;)
            {
                for (int column = m_intColumns; --column >= 0;)
                {
                    if (!procedure.Apply(getQuick(row, column)))
                    {
                        return false;
                    }
                }
            }
            return true;
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

        public void zAssign8Neighbors(DoubleMatrix2D B,
                                      Double9Function function)
        {
            if (function == null)
            {
                throw new HCException("function must not be null.");
            }
            checkShape(B);
            if (m_intRows < 3 || m_intColumns < 3)
            {
                return; // nothing to do
            }
            int r = m_intRows - 1;
            int c = m_intColumns - 1;
            double a00, a01, a02;
            double a10, a11, a12;
            double a20, a21, a22;
            for (int i = 1; i < r; i++)
            {
                a00 = getQuick(i - 1, 0);
                a01 = getQuick(i - 1, 1);
                a10 = getQuick(i, 0);
                a11 = getQuick(i, 1);
                a20 = getQuick(i + 1, 0);
                a21 = getQuick(i + 1, 1);

                for (int j = 1; j < c; j++)
                {
                    // in each step six cells can be remembered in registers - they don't need to be reread from slow memory
                    // in each step 3 instead of 9 cells need to be read from memory.
                    a02 = getQuick(i - 1, j + 1);
                    a12 = getQuick(i, j + 1);
                    a22 = getQuick(i + 1, j + 1);

                    B.setQuick(i, j, function.Apply(
                                         a00, a01, a02,
                                         a10, a11, a12,
                                         a20, a21, a22));

                    a00 = a01;
                    a10 = a11;
                    a20 = a21;

                    a01 = a02;
                    a11 = a12;
                    a21 = a22;
                }
            }
        }

        /**
         * Linear algebraic matrix-vector multiplication; <tt>z = A * y</tt>; 
         * Equivalent to <tt>return A.zMult(y,z,1,0);</tt>
         */

        public DoubleMatrix1D zMult(DoubleMatrix1D y, DoubleMatrix1D z)
        {
            return zMult(y, z, 1, (z == null ? 1 : 0), false);
        }

        /**
         * Linear algebraic matrix-vector multiplication; <tt>z = alpha * A * y + beta*z</tt>.
         * <tt>z[i] = alpha*Sum(A[i,j] * y[j]) + beta*z[i], i=0..A.Rows()-1, j=0..y.Size()-1</tt>.
         * Where <tt>A == this</tt>.
         * <br>
         * Note: Matrix shape conformance is checked <i>after</i> potential transpositions.
         *
         * @param y the source vector.
         * @param z the vector where results are to be stored. Set this parameter to <tt>null</tt> to indicate that a new result vector shall be constructed.
         * @return z (for convenience only).
         * 
         * @throws ArgumentException if <tt>A.Columns() != y.Size() || A.Rows() > z.Size())</tt>.
         */

        public DoubleMatrix1D zMult(DoubleMatrix1D y, DoubleMatrix1D z, double alpha, double beta, bool transposeA)
        {
            if (transposeA)
            {
                return viewDice().zMult(y, z, alpha, beta, false);
            }
            //bool ignore = (z==null);
            if (z == null)
            {
                z = new DenseDoubleMatrix1D(m_intRows);
            }
            if (m_intColumns != y.Size() || m_intRows > z.Size())
            {
                throw new ArgumentException("Incompatible args: " + toStringShort() + ", " + y.toStringShort() + ", " +
                                            z.toStringShort());
            }

            for (int i = m_intRows; --i >= 0;)
            {
                double s = 0;
                for (int j = m_intColumns; --j >= 0;)
                {
                    s += getQuick(i, j)*y.getQuick(j);
                }
                z.setQuick(i, alpha*s + beta*z.getQuick(i));
            }
            return z;
        }

        /**
         * Linear algebraic matrix-matrix multiplication; <tt>C = A x B</tt>;
         * Equivalent to <tt>A.zMult(B,C,1,0,false,false)</tt>.
         */

        public DoubleMatrix2D zMult(DoubleMatrix2D B, DoubleMatrix2D C)
        {
            return zMult(B, C, 1, (C == null ? 1 : 0), false, false);
        }

        /**
         * Linear algebraic matrix-matrix multiplication; <tt>C = alpha * A x B + beta*C</tt>.
         * <tt>C[i,j] = alpha*Sum(A[i,k] * B[k,j]) + beta*C[i,j], k=0..n-1</tt>.
         * <br>
         * Matrix shapes: <tt>A(m x n), B(n x p), C(m x p)</tt>.
         * <br>
         * Note: Matrix shape conformance is checked <i>after</i> potential transpositions.
         *
         * @param B the second source matrix.
         * @param C the matrix where results are to be stored. Set this parameter to <tt>null</tt> to indicate that a new result matrix shall be constructed.
         * @return C (for convenience only).
         *
         * @throws ArgumentException if <tt>B.Rows() != A.Columns()</tt>.
         * @throws ArgumentException if <tt>C.Rows() != A.Rows() || C.Columns() != B.Columns()</tt>.
         * @throws ArgumentException if <tt>A == C || B == C</tt>.
         */

        public DoubleMatrix2D zMult(DoubleMatrix2D B, DoubleMatrix2D C, double alpha, double beta, bool transposeA,
                                    bool transposeB)
        {
            if (transposeA)
            {
                return viewDice().zMult(B, C, alpha, beta, false, transposeB);
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

            for (int j = p; --j >= 0;)
            {
                for (int i = m; --i >= 0;)
                {
                    double s = 0;
                    for (int k = n; --k >= 0;)
                    {
                        s += getQuick(i, k)*B.getQuick(k, j);
                    }
                    C.setQuick(i, j, alpha*s + beta*C.getQuick(i, j));
                }
            }
            return C;
        }

        /**
         * Returns the sum of all cells; <tt>Sum( x[i,j] )</tt>.
         * @return the sum.
         */

        public double zSum()
        {
            if (Size() == 0)
            {
                return 0;
            }
            return aggregate(Functions.m_plus, Functions.m_identity);
        }
    }
}
