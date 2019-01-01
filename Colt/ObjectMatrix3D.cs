#region

using System;
using HC.Analytics.Colt.objectAlgo;
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
    //package matrix;

    ////import IntArrayList;
    ////import ObjectArrayList;
    ////import AbstractMatrix3D;
    /**
    Abstract base class for 3-d matrices holding <tt>Object</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    A matrix has a number of m_intSlices, m_intRows and m_intColumns, which are assigned upon instance construction - The matrix's size is then <tt>Slices()*Rows()*Columns()</tt>.
    Elements are accessed via <tt>[slice,row,column]</tt> coordinates.
    Legal coordinates range from <tt>[0,0,0]</tt> to <tt>[Slices()-1,Rows()-1,Columns()-1]</tt>.
    Any attempt to access an element at a coordinate <tt>slice&lt;0 || slice&gt;=Slices() || row&lt;0 || row&gt;=Rows() || column&lt;0 || column&gt;=column()</tt> will throw an <tt>HCException</tt>.
    <p>
    <b>Note</b> that this implementation is not lock.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class ObjectMatrix3D : AbstractMatrix3D
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
        Applies a function to each cell and aggregates the results.
        Returns a value <tt>v</tt> such that <tt>v==a(Size())</tt> where <tt>a(i) == aggr( a(i-1), f(get(slice,row,column)) )</tt> and terminators are <tt>a(1) == f(get(0,0,0)), a(0)==null</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Functions F = Functions.functions;
        2 x 2 x 2 matrix
        0 1
        2 3

        4 5
        6 7

        // Sum( x[slice,row,col]*x[slice,row,col] ) 
        matrix.aggregate(Functions.plus,Functions.square);
        --> 140
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.
        @param f a function transforming the current cell value.
        @return the aggregated measure.
        @see Functions
        */

        public Object aggregate(ObjectObjectFunction aggr, ObjectFunction f)
        {
            if (Size() == 0)
            {
                return null;
            }
            Object a = f.Apply(getQuick(
                                   m_intSlices - 1, m_intRows - 1, m_intColumns - 1));
            int d = 1; // last cell already done
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns - d; --column >= 0;)
                    {
                        a = aggr.Apply(a, f.Apply(getQuick(slice, row, column)));
                    }
                    d = 0;
                }
            }
            return a;
        }

        /**
        Applies a function to each corresponding cell of two matrices and aggregates the results.
        Returns a value <tt>v</tt> such that <tt>v==a(Size())</tt> where <tt>a(i) == aggr( a(i-1), f(get(slice,row,column),other.get(slice,row,column)) )</tt> and terminators are <tt>a(1) == f(get(0,0,0),other.get(0,0,0)), a(0)==null</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Functions F = Functions.functions;
        x = 2 x 2 x 2 matrix
        0 1
        2 3

        4 5
        6 7

        y = 2 x 2 x 2 matrix
        0 1
        2 3

        4 5
        6 7

        // Sum( x[slice,row,col] * y[slice,row,col] ) 
        x.aggregate(y, Functions.plus, Functions.mult);
        --> 140

        // Sum( (x[slice,row,col] + y[slice,row,col])^2 )
        x.aggregate(y, Functions.plus, Functions.chain(Functions.square,Functions.plus));
        --> 560
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
        @param f a function transforming the current cell values.
        @return the aggregated measure.
        @throws	ArgumentException if <tt>Slices() != other.Slices() || Rows() != other.Rows() || Columns() != other.Columns()</tt>
        @see Functions
        */

        public Object aggregate(ObjectMatrix3D other, ObjectObjectFunction aggr, ObjectObjectFunction f)
        {
            checkShape(other);
            if (Size() == 0)
            {
                return null;
            }
            Object a = f.Apply(getQuick(m_intSlices - 1, m_intRows - 1, m_intColumns - 1),
                               other.getQuick(m_intSlices - 1, m_intRows - 1, m_intColumns - 1));
            int d = 1; // last cell already done
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns - d; --column >= 0;)
                    {
                        a = aggr.Apply(a, f.Apply(getQuick(slice, row, column), other.getQuick(slice, row, column)));
                    }
                    d = 0;
                }
            }
            return a;
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

        public ObjectMatrix3D assign(Object[,,] values)
        {
            if (values.GetLength(0) != m_intSlices)
            {
                throw new ArgumentException("Must have same number of m_intSlices: m_intSlices=" + values.GetLength(0) +
                                            "Slices()=" +
                                            Slices());
            }
            for (int slice = m_intSlices; --slice >= 0;)
            {
                //Object[,] currentSlice = 
                //    values[slice];
                if (values.GetLength(1) != m_intRows)
                {
                    throw new ArgumentException("Must have same number of m_intRows in every slice: m_intRows=" +
                                                values.GetLength(1) + "Rows()=" + Rows());
                }
                for (int row = m_intRows; --row >= 0;)
                {
                    //Object[] currentRow = currentSlice[row];
                    if (values.GetLength(2) != m_intColumns)
                    {
                        throw new ArgumentException(
                            "Must have same number of m_intColumns in every row: m_intColumns=" +
                            m_intColumns + "Columns()=" + Columns());
                    }
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        setQuick(slice, row, column,
                                 values[slice, row, column]);
                    }
                }
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[slice,row,col] = function(x[slice,row,col])</tt>.
        <p>
        <b>Example:</b>
        <pre>
        matrix = 1 x 2 x 2 matrix
        0.5 1.5      
        2.5 3.5

        // change each cell to its sine
        matrix.assign(Functions.sin);
        -->
        1 x 2 x 2 matrix
        0.479426  0.997495 
        0.598472 -0.350783
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param function a function object taking as argument the current cell's value.
        @return <tt>this</tt> (for convenience only).
        @see Functions
        */

        public ObjectMatrix3D assign(ObjectFunction function)
        {
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        setQuick(slice, row, column, function.Apply(getQuick(slice, row, column)));
                    }
                }
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of m_intSlices, m_intRows and m_intColumns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     other   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Slices() != other.Slices() || Rows() != other.Rows() || Columns() != other.Columns()</tt>
         */

        public ObjectMatrix3D assign(ObjectMatrix3D other)
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

            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        setQuick(slice, row, column, other.getQuick(slice, row, column));
                    }
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
        m1 = 1 x 2 x 2 matrix 
        0 1 
        2 3

        m2 = 1 x 2 x 2 matrix 
        0 2 
        4 6

        m1.assign(m2, Functions.pow);
        -->
        m1 == 1 x 2 x 2 matrix
         1   1 
        16 729
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param y the secondary matrix to operate on.
        @param function a function object taking as first argument the current cell's value of <tt>this</tt>,
        and as second argument the current cell's value of <tt>y</tt>,
        @return <tt>this</tt> (for convenience only).
        @throws	ArgumentException if <tt>Slices() != other.Slices() || Rows() != other.Rows() || Columns() != other.Columns()</tt>
        @see Functions
        */

        public ObjectMatrix3D assign(ObjectMatrix3D y, ObjectObjectFunction function)
        {
            checkShape(y);
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        setQuick(slice, row, column,
                                 function.Apply(getQuick(slice, row, column), y.getQuick(slice, row, column)));
                    }
                }
            }
            return this;
        }

        /**
         * Sets all cells to the state specified by <tt>value</tt>.
         * @param    value the value to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         */

        public ObjectMatrix3D assign(Object value)
        {
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        setQuick(slice, row, column, value);
                    }
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
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        if (getQuick(slice, row, column) != null)
                        {
                            cardinality++;
                        }
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

        public ObjectMatrix3D Copy()
        {
            return like().assign(this);
        }

        /**
        * Compares the specified Object with the receiver for equality.
        * Equivalent to <tt>Equals(otherObj,true)</tt>.  
        *
        * @param otherObj the Object to be compared for equality with the receiver.
        * @return true if the specified Object is equal to the receiver.
        */

        public new bool Equals(Object otherObj)
        {
            //delta
            return Equals(otherObj, true);
        }

        /**
        * Compares the specified Object with the receiver for equality.
        * Returns true if and only if the specified Object is also at least an ObjectMatrix3D, both matrices have the
        * same size, and all corresponding pairs of cells in the two matrices are the same.
        * In other words, two matrices are defined to be equal if they contain the
        * same cell values in the same order.
        * Tests elements for equality or identity as specified by <tt>testForEquality</tt>.
        * When testing for equality, two elements <tt>e1</tt> and
        * <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        * e1.0equals(e2))</tt>.)  
        *
        * @param otherObj the Object to be compared for equality with the receiver.
        * @param testForEquality if true -> tests for equality, otherwise for identity.
        * @return true if the specified Object is equal to the receiver.
        */

        public bool Equals(Object otherObj, bool testForEquality)
        {
            //delta
            if (!(otherObj is ObjectMatrix3D))
            {
                return false;
            }
            if (this == otherObj)
            {
                return true;
            }
            if (otherObj == null)
            {
                return false;
            }
            ObjectMatrix3D other = (ObjectMatrix3D) otherObj;
            if (m_intRows != other.Rows())
            {
                return false;
            }
            if (m_intColumns != other.Columns())
            {
                return false;
            }

            if (!testForEquality)
            {
                for (int slice = m_intSlices; --slice >= 0;)
                {
                    for (int row = m_intRows; --row >= 0;)
                    {
                        for (int column = m_intColumns; --column >= 0;)
                        {
                            if (getQuick(slice, row, column) != other.getQuick(slice, row, column))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int slice = m_intSlices; --slice >= 0;)
                {
                    for (int row = m_intRows; --row >= 0;)
                    {
                        for (int column = m_intColumns; --column >= 0;)
                        {
                            if (
                                !(getQuick(slice, row, column) == null
                                      ? other.getQuick(slice, row, column) == null
                                      : getQuick(slice, row, column).Equals(other.getQuick(slice, row, column))))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /**
         * Returns the matrix cell value at coordinate <tt>[slice,row,column]</tt>.
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @return    the value of the specified cell.
         * @throws	HCException if <tt>slice&lt;0 || slice&gt;=Slices() || row&lt;0 || row&gt;=Rows() || column&lt;0 || column&gt;=column()</tt>.
         */

        public Object get(int slice, int row, int column)
        {
            if (slice < 0 || slice >= m_intSlices || row < 0 || row >= m_intRows || column < 0 || column >= m_intColumns)
            {
                throw new HCException("slice:" + slice + ", row:" + row + ", column:" + column);
            }
            return getQuick(slice, row, column);
        }

        /**
         * Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
         * Override this method in wrappers.
         */

        public ObjectMatrix3D getContent()
        {
            return this;
        }

        /**
        Fills the coordinates and values of cells having non-zero values into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists all have a new size, the number of non-zero values.
        <p>
        In general, fill order is <i>unspecified</i>.
        This implementation fill like: <tt>for (slice = 0..m_intSlices-1) for (row = 0..m_intRows-1) for (column = 0..colums-1) do ... </tt>.
        However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
        (Of course, result lists indexes are guaranteed to correspond to the same cell).
        For an example, see {@link ObjectMatrix2D#getNonZeros(IntArrayList,IntArrayList,ObjectArrayList)}.

        @param sliceList the list to be filled with slice indexes, can have any size.
        @param rowList the list to be filled with row indexes, can have any size.
        @param columnList the list to be filled with column indexes, can have any size.
        @param valueList the list to be filled with values, can have any size.
        */

        public void getNonZeros(IntArrayList sliceList, IntArrayList rowList, IntArrayList columnList,
                                ObjectArrayList valueList)
        {
            sliceList.Clear();
            rowList.Clear();
            columnList.Clear();
            valueList.Clear();
            int s = m_intSlices;
            int r = m_intRows;
            int c = m_intColumns;
            for (int slice = 0; slice < s; slice++)
            {
                for (int row = 0; row < r; row++)
                {
                    for (int column = 0; column < c; column++)
                    {
                        Object value = getQuick(slice, row, column);
                        if (value != null)
                        {
                            sliceList.Add(slice);
                            rowList.Add(row);
                            columnList.Add(column);
                            valueList.Add(value);
                        }
                    }
                }
            }
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
        public abstract Object getQuick(int slice, int row, int column);
        /**
         * Returns <tt>true</tt> if both matrices share at least one identical cell.
         */

        public bool haveSharedCells(ObjectMatrix3D other)
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

        public bool haveSharedCellsRaw(ObjectMatrix3D other)
        {
            return false;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same number of m_intSlices, m_intRows and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix3D</tt> the new matrix must also be of type <tt>DenseObjectMatrix3D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix3D</tt> the new matrix must also be of type <tt>SparseObjectMatrix3D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @return  a new empty matrix of the same dynamic type.
         */

        public ObjectMatrix3D like()
        {
            return like(m_intSlices, m_intRows, m_intColumns);
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
        public abstract ObjectMatrix3D like(int m_intSlices, int m_intRows, int m_intColumns);
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

        public abstract ObjectMatrix2D like2D(int m_intRows, int m_intColumns, int rowZero, int columnZero,
                                              int rowStride, int columnStride);

        /**
         * Sets the matrix cell at coordinate <tt>[slice,row,column]</tt> to the specified value.
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the column-coordinate.
         * @param    value the value to be filled into the specified cell.
         * @throws	HCException if <tt>row&lt;0 || row&gt;=Rows() || slice&lt;0 || slice&gt;=Slices() || column&lt;0 || column&gt;=column()</tt>.
         */

        public void set(int slice, int row, int column, Object value)
        {
            if (slice < 0 || slice >= m_intSlices || row < 0 || row >= m_intRows || column < 0 || column >= m_intColumns)
            {
                throw new HCException("slice:" + slice + ", row:" + row + ", column:" + column);
            }
            setQuick(slice, row, column, value);
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
        public abstract void setQuick(int slice, int row, int column, Object value);
        /**
         * Constructs and returns a 2-dimensional array containing the cell values.
         * The returned array <tt>values</tt> has the form <tt>values[slice,row,column]</tt>
         * and has the same number of m_intSlices, m_intRows and m_intColumns as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @return an array filled with the values of the cells.
         */

        public Object[,,] ToArray()
        {
            Object[,,] values = new Object[m_intSlices,m_intRows,m_intColumns];
            for (int slice = m_intSlices; --slice >= 0;)
            {
                //Object[,] currentSlice = values[slice];
                for (int row = m_intRows; --row >= 0;)
                {
                    //Object[] currentRow = values[slice, row];
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        values[slice, row, column] = getQuick(slice, row, column);
                    }
                }
            }
            return values;
        }

        /**
         * Returns a string representation using default formatting.
         * @see Formatter
         */

        public override string ToString()
        {
            return new Formatter().ToString(this);
        }

        /**
         * Constructs and returns a new view equal to the receiver.
         * The view is a shallow Clone. Calls <code>Clone()</code> and casts the result.
         * <p>
         * <b>Note that the view is not a deep copy.</b>
         * The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa. 
         * <p>
         * Use {@link #Copy()} if you want to construct an independent deep copy rather than a new view.
         *
         * @return  a new view of the receiver.
         */

        public ObjectMatrix3D view()
        {
            return (ObjectMatrix3D) Clone();
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

        public ObjectMatrix2D viewColumn(int column)
        {
            checkColumn(column);
            int sliceRows = m_intSlices;
            int sliceColumns = m_intRows;

            //int sliceOffset = index(0,0,column);
            int sliceRowZero = m_sliceZero;
            int sliceColumnZero = m_rowZero + _columnOffset(_columnRank(column));

            int sliceRowStride = m_sliceStride;
            int sliceColumnStride = m_rowStride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowStride, sliceColumnStride);
        }

        /**
        Constructs and returns a new <i>flip view</i> along the column axis.
        What used to be column <tt>0</tt> is now column <tt>Columns()-1</tt>, ..., what used to be column <tt>Columns()-1</tt> is now column <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @return a new flip view.
        @see #viewSliceFlip()
        @see #viewRowFlip()
        */

        public ObjectMatrix3D viewColumnFlip()
        {
            return (ObjectMatrix3D) (view().vColumnFlip());
        }

        /**
        Constructs and returns a new <i>dice view</i>; Swaps dimensions (axes); Example: 3 x 4 x 5 matrix --> 4 x 3 x 5 matrix.
        The view has dimensions exchanged; what used to be one axis is now another, in all desired permutations.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @param axis0 the axis that shall become axis 0 (legal values 0..2).
        @param axis1 the axis that shall become axis 1 (legal values 0..2).
        @param axis2 the axis that shall become axis 2 (legal values 0..2).
        @return a new dice view.
        @throws ArgumentException if some of the parameters are equal or not in range 0..2.
        */

        public ObjectMatrix3D viewDice(int axis0, int axis1, int axis2)
        {
            return (ObjectMatrix3D) (view().vDice(axis0, axis1, axis2));
        }

        /**
        Constructs and returns a new <i>sub-range view</i> that is a <tt>depth x height x width</tt> sub matrix starting at <tt>[slice,row,column]</tt>;
        Equivalent to <tt>view().part(slice,row,column,depth,height,width)</tt>; Provided for convenience only.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param     slice   The index of the slice-coordinate.
        @param     row   The index of the row-coordinate.
        @param     column   The index of the column-coordinate.
        @param     depth   The depth of the box.
        @param     height   The height of the box.
        @param     width   The width of the box.
        @throws	HCException if <tt>slice<0 || depth<0 || slice+depth>Slices() || row<0 || height<0 || row+height>Rows() || column<0 || width<0 || column+width>Columns()</tt>
        @return the new view.
		
        */

        public ObjectMatrix3D viewPart(int slice, int row, int column, int depth, int height, int width)
        {
            return (ObjectMatrix3D) (view().vPart(slice, row, column, depth, height, width));
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

        public ObjectMatrix2D viewRow(int row)
        {
            checkRow(row);
            int sliceRows = m_intSlices;
            int sliceColumns = m_intColumns;

            //int sliceOffset = index(0,row,0);
            int sliceRowZero = m_sliceZero;
            int sliceColumnZero = m_columnZero + _rowOffset(_rowRank(row));

            int sliceRowStride = m_sliceStride;
            int sliceColumnStride = m_columnStride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowStride, sliceColumnStride);
        }

        /**
        Constructs and returns a new <i>flip view</i> along the row axis.
        What used to be row <tt>0</tt> is now row <tt>Rows()-1</tt>, ..., what used to be row <tt>Rows()-1</tt> is now row <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @return a new flip view.
        @see #viewSliceFlip()
        @see #viewColumnFlip()
        */

        public ObjectMatrix3D viewRowFlip()
        {
            return (ObjectMatrix3D) (view().vRowFlip());
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        There holds <tt>view.Slices() == sliceIndexes.Length, view.Rows() == rowIndexes.Length, view.Columns() == columnIndexes.Length</tt> and 
        <tt>view.get(k,i,j) == get(sliceIndexes[k],rowIndexes[i],columnIndexes[j])</tt>.
        Indexes can occur multiple times and can be in arbitrary order.
        For an example see {@link ObjectMatrix2D#viewSelection(int[],int[])}.
        <p>
        Note that modifying the index arguments after this call has returned has no effect on the view.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param  sliceIndexes   The m_intSlices of the cells that shall be visible in the new view. To indicate that <i>all</i> m_intSlices shall be visible, simply set this parameter to <tt>null</tt>.
        @param  rowIndexes   The m_intRows of the cells that shall be visible in the new view. To indicate that <i>all</i> m_intRows shall be visible, simply set this parameter to <tt>null</tt>.
        @param  columnIndexes   The m_intColumns of the cells that shall be visible in the new view. To indicate that <i>all</i> m_intColumns shall be visible, simply set this parameter to <tt>null</tt>.
        @return the new view.
        @throws HCException if <tt>!(0 <= sliceIndexes[i] < Slices())</tt> for any <tt>i=0..sliceIndexes.Length-1</tt>.
        @throws HCException if <tt>!(0 <= rowIndexes[i] < Rows())</tt> for any <tt>i=0..rowIndexes.Length-1</tt>.
        @throws HCException if <tt>!(0 <= columnIndexes[i] < Columns())</tt> for any <tt>i=0..columnIndexes.Length-1</tt>.
        */

        public ObjectMatrix3D viewSelection(int[] sliceIndexes, int[] rowIndexes, int[] columnIndexes)
        {
            // check for "all"
            if (sliceIndexes == null)
            {
                sliceIndexes = new int[m_intSlices];
                for (int i = m_intSlices; --i >= 0;)
                {
                    sliceIndexes[i] = i;
                }
            }
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

            checkSliceIndexes(sliceIndexes);
            checkRowIndexes(rowIndexes);
            checkColumnIndexes(columnIndexes);

            int[] sliceOffsets = new int[sliceIndexes.Length];
            int[] rowOffsets = new int[rowIndexes.Length];
            int[] columnOffsets = new int[columnIndexes.Length];

            for (int i = sliceIndexes.Length; --i >= 0;)
            {
                sliceOffsets[i] = _sliceOffset(_sliceRank(sliceIndexes[i]));
            }
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowOffsets[i] = _rowOffset(_rowRank(rowIndexes[i]));
            }
            for (int i = columnIndexes.Length; --i >= 0;)
            {
                columnOffsets[i] = _columnOffset(_columnRank(columnIndexes[i]));
            }

            return viewSelectionLike(sliceOffsets, rowOffsets, columnOffsets);
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding all <b>m_intSlices</b> matching the given condition.
        Applies the condition to each slice and takes only those where <tt>condition.Apply(viewSlice(i))</tt> yields <tt>true</tt>.
        To match m_intRows or m_intColumns, use a dice view.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        // extract and view all m_intSlices which have an aggregate sum > 1000
        matrix.viewSelection( 
        &nbsp;&nbsp;&nbsp;new ObjectMatrix2DProcedure() {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public  bool Apply(ObjectMatrix2D m) { return m.zSum > 1000; }
        &nbsp;&nbsp;&nbsp;}
        );
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param  condition The condition to be matched.
        @return the new view.
        */

        public ObjectMatrix3D viewSelection(ObjectMatrix2DProcedure condition)
        {
            IntArrayList matches = new IntArrayList();
            for (int i = 0; i < m_intSlices; i++)
            {
                if (condition.Apply(viewSlice(i)))
                {
                    matches.Add(i);
                }
            }

            matches.trimToSize();
            return viewSelection(matches.elements(), null, null); // take all m_intRows and m_intColumns
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param sliceOffsets the offsets of the visible elements.
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */
        public abstract ObjectMatrix3D viewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets);
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

        public ObjectMatrix2D viewSlice(int slice)
        {
            checkSlice(slice);
            int sliceRows = m_intRows;
            int sliceColumns = m_intColumns;

            //int sliceOffset = index(slice,0,0);
            int sliceRowZero = m_rowZero;
            int sliceColumnZero = m_columnZero + _sliceOffset(_sliceRank(slice));

            int sliceRowStride = m_rowStride;
            int sliceColumnStride = m_columnStride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowStride, sliceColumnStride);
        }

        /**
        Constructs and returns a new <i>flip view</i> along the slice axis.
        What used to be slice <tt>0</tt> is now slice <tt>Slices()-1</tt>, ..., what used to be slice <tt>Slices()-1</tt> is now slice <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @return a new flip view.
        @see #viewRowFlip()
        @see #viewColumnFlip()
        */

        public ObjectMatrix3D viewSliceFlip()
        {
            return (ObjectMatrix3D) (view().vSliceFlip());
        }

        /**
        Sorts the matrix m_intSlices into ascending order, according to the <i>natural ordering</i> of the matrix values in the given <tt>[row,column]</tt> position.
        This sort is guaranteed to be <i>stable</i>.
        For further information, see {@link Appendobjectalgo.Sorting#Sort(ObjectMatrix3D,int,int)}.
        For more advanced sorting functionality, see {@link Appendobjectalgo.Sorting}.
        @return a new sorted vector (matrix) view.
        @throws HCException if <tt>row < 0 || row >= Rows() || column < 0 || column >= Columns()</tt>.
        */

        public ObjectMatrix3D viewSorted(int row, int column)
        {
            return SortingObjectAlgo.mergeSort.Sort(this, row, column);
        }

        /**
        Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        More specifically, the view has <tt>Slices()/sliceStride</tt> m_intSlices and <tt>Rows()/rowStride</tt> m_intRows and <tt>Columns()/columnStride</tt> m_intColumns 
        holding cells <tt>get(k*sliceStride,i*rowStride,j*columnStride)</tt> for all <tt>k = 0..Slices()/sliceStride - 1, i = 0..Rows()/rowStride - 1, j = 0..Columns()/columnStride - 1</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @param sliceStride the slice step factor.
        @param rowStride the row step factor.
        @param columnStride the column step factor.
        @return a new view.
        @throws	HCException if <tt>sliceStride<=0 || rowStride<=0 || columnStride<=0</tt>.
        */

        public ObjectMatrix3D viewStrides(int sliceStride, int rowStride, int columnStride)
        {
            return (ObjectMatrix3D) (view().vStrides(sliceStride, rowStride, columnStride));
        }

        /**
         * Applies a procedure to each cell's value.
         * Iterates downwards from <tt>[Slices()-1,Rows()-1,Columns()-1]</tt> to <tt>[0,0,0]</tt>,
         * as demonstrated by this snippet:
         * <pre>
         * for (int slice=m_intSlices; --slice >=0;) {
         *    for (int row=m_intRows; --row >= 0;) {
         *       for (int column=m_intColumns; --column >= 0;) {
         *           if (!procedure.Apply(get(slice,row,column))) return false;
         *       }
         *    }
         * }
         * return true;
         * </pre>
         * Note that an implementation may use more efficient techniques, but must not use any other order.
         *
         * @param procedure a procedure object taking as argument the current cell's value. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        private bool xforEach(ObjectProcedure procedure)
        {
            for (int slice = m_intSlices; --slice >= 0;)
            {
                for (int row = m_intRows; --row >= 0;)
                {
                    for (int column = m_intColumns; --column >= 0;)
                    {
                        if (!procedure.Apply(getQuick(slice, row, column)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /**
         * Applies a procedure to each cell's coordinate.
         * Iterates downwards from <tt>[Slices()-1,Rows()-1,Columns()-1]</tt> to <tt>[0,0,0]</tt>,
         * as demonstrated by this snippet:
         * <pre>
         * for (int slice=m_intSlices; --slice >=0;) {
         *    for (int row=m_intRows; --row >= 0;) {
         *       for (int column=m_intColumns; --column >= 0;) {
         *           if (!procedure.Apply(slice,row,column)) return false;
         *       }
         *    }
         * }
         * return true;
         * </pre>
         * Note that an implementation may use more efficient techniques, but must not use any other order.
         *
         * @param procedure a procedure object taking as first argument the current slice, as second argument the current row, and as third argument the current column. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        private bool xforEachCoordinate(IntIntIntProcedure procedure)
        {
            for (int column = m_intColumns; --column >= 0;)
            {
                for (int slice = m_intSlices; --slice >= 0;)
                {
                    for (int row = m_intRows; --row >= 0;)
                    {
                        if (!procedure.Apply(slice, row, column))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
