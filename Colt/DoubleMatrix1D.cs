#region

using System;
using HC.Analytics.Colt.doubleAlgo;

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
    ////import AbstractMatrix1D;
    /**
    Abstract base class for 1-d matrices (aka <i>vectors</i>) holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    A matrix has a number of cells (its <i>m_intSize</i>), which are assigned upon instance construction.
    Elements are accessed via zero based indexes.
    Legal indexes are of the form <tt>[0..Size()-1]</tt>.
    Any attempt to access an element at a coordinate <tt>index&lt;0 || index&gt;=Size()</tt> will throw an <tt>HCException</tt>.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class DoubleMatrix1D : AbstractMatrix1D
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
        Applies a function to each cell and aggregates the results.
        Returns a value <tt>v</tt> such that <tt>v==a(Size())</tt> where <tt>a(i) == aggr( a(i-1), f(get(i)) )</tt> and terminators are <tt>a(1) == f(get(0)), a(0)==double.NaN</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Functions F = Functions.functions;
        matrix = 0 1 2 3 

        // Sum( x[i]*x[i] ) 
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
            if (m_intSize == 0)
            {
                return double.NaN;
            }
            double a = f.Apply(getQuick(m_intSize - 1));
            for (int i = m_intSize - 1; --i >= 0;)
            {
                a = aggr.Apply(a, f.Apply(getQuick(i)));
            }
            return a;
        }

        /**
        Applies a function to each corresponding cell of two matrices and aggregates the results.
        Returns a value <tt>v</tt> such that <tt>v==a(Size())</tt> where <tt>a(i) == aggr( a(i-1), f(get(i),other.get(i)) )</tt> and terminators are <tt>a(1) == f(get(0),other.get(0)), a(0)==double.NaN</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Functions F = Functions.functions;
        x = 0 1 2 3 
        y = 0 1 2 3 

        // Sum( x[i]*y[i] )
        x.aggregate(y, Functions.plus, Functions.mult);
        --> 14

        // Sum( (x[i]+y[i])^2 )
        x.aggregate(y, Functions.plus, Functions.chain(Functions.square,Functions.plus));
        --> 56
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
        @param f a function transforming the current cell values.
        @return the aggregated measure.
        @throws	ArgumentException if <tt>Size() != other.Size()</tt>.
        @see Functions
        */

        public double aggregate(
            DoubleMatrix1D other,
            DoubleDoubleFunction aggr,
            DoubleDoubleFunction f)
        {
            checkSize(other);
            if (m_intSize == 0)
            {
                return double.NaN;
            }
            double a = f.Apply(getQuick(m_intSize - 1), other.getQuick(m_intSize - 1));
            for (int i = m_intSize - 1; --i >= 0;)
            {
                a = aggr.Apply(a, f.Apply(getQuick(i), other.getQuick(i)));
            }
            return a;
        }

        /**
         * Sets all cells to the state specified by <tt>values</tt>.
         * <tt>values</tt> is required to have the same number of cells as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param    values the values to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         * @throws ArgumentException if <tt>values.Length != Size()</tt>.
         */

        public DoubleMatrix1D assign(double[] values)
        {
            if (values.Length != m_intSize)
            {
                throw new ArgumentException("Must have same number of cells: Length=" + values.Length + "Size()=" +
                                            Size());
            }
            for (int i = m_intSize; --i >= 0;)
            {
                setQuick(i, values[i]);
            }
            return this;
        }

        /**
         * Sets all cells to the state specified by <tt>value</tt>.
         * @param    value the value to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         */

        public DoubleMatrix1D assign(double value)
        {
            for (int i = m_intSize; --i >= 0;)
            {
                setQuick(i, value);
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[i] = function(x[i])</tt>.
        (Iterates downwards from <tt>[Size()-1]</tt> to <tt>[0]</tt>).
        <p>
        <b>Example:</b>
        <pre>
        // change each cell to its sine
        matrix =   0.5      1.5      2.5       3.5 
        matrix.assign(Functions.sin);
        -->
        matrix ==  0.479426 0.997495 0.598472 -0.350783
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param function a function object taking as argument the current cell's value.
        @return <tt>this</tt> (for convenience only).
        @see Functions
        */

        public DoubleMatrix1D assign(
            DoubleFunction function)
        {
            for (int i = m_intSize; --i >= 0;)
            {
                setQuick(i, function.Apply(getQuick(i)));
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same m_intSize.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     other   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Size() != other.Size()</tt>.
         */

        public DoubleMatrix1D assign(DoubleMatrix1D other)
        {
            if (other == this)
            {
                return this;
            }
            checkSize(other);
            if (haveSharedCells(other))
            {
                other = other.Copy();
            }

            for (int i = m_intSize; --i >= 0;)
            {
                setQuick(i, other.getQuick(i));
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[i] = function(x[i],y[i])</tt>.
        <p>
        <b>Example:</b>
        <pre>
        // assign x[i] = x[i]<sup>y[i]</sup>
        m1 = 0 1 2 3;
        m2 = 0 2 4 6;
        m1.assign(m2, Functions.pow);
        -->
        m1 == 1 1 16 729
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param y the secondary matrix to operate on.
        @param function a function object taking as first argument the current cell's value of <tt>this</tt>,
        and as second argument the current cell's value of <tt>y</tt>,
        @return <tt>this</tt> (for convenience only).
        @throws	ArgumentException if <tt>Size() != y.Size()</tt>.
        @see Functions
        */

        public DoubleMatrix1D assign(DoubleMatrix1D y,
                                     DoubleDoubleFunction function)
        {
            checkSize(y);
            for (int i = m_intSize; --i >= 0;)
            {
                setQuick(i, function.Apply(getQuick(i), y.getQuick(i)));
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[i] = function(x[i],y[i])</tt>.
        (Iterates downwards from <tt>[Size()-1]</tt> to <tt>[0]</tt>).
        <p>
        <b>Example:</b>
        <pre>
        // assign x[i] = x[i]<sup>y[i]</sup>
        m1 = 0 1 2 3;
        m2 = 0 2 4 6;
        m1.assign(m2, Functions.pow);
        -->
        m1 == 1 1 16 729

        // for non-standard functions there is no shortcut: 
        m1.assign(m2,
        &nbsp;&nbsp;&nbsp;new DoubleDoubleFunction() {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public double Apply(double x, double y) { return Math.Pow(x,y); }
        &nbsp;&nbsp;&nbsp;}
        );
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param y the secondary matrix to operate on.
        @param function a function object taking as first argument the current cell's value of <tt>this</tt>,
        and as second argument the current cell's value of <tt>y</tt>,
        @return <tt>this</tt> (for convenience only).
        @throws	ArgumentException if <tt>Size() != y.Size()</tt>.
        @see Functions
        */

        public DoubleMatrix1D assign(DoubleMatrix1D y,
                                     DoubleDoubleFunction function,
                                     IntArrayList nonZeroIndexes)
        {
            checkSize(y);
            int[] nonZeroElements = nonZeroIndexes.elements();

            // specialized for speed
            if (function == Functions.m_mult)
            {
                // x[i] = x[i] * y[i]
                int j = 0;
                for (int index = nonZeroIndexes.Size(); --index >= 0;)
                {
                    int i = nonZeroElements[index];
                    for (; j < i; j++)
                    {
                        setQuick(j, 0); // x[i] = 0 for all zeros
                    }
                    setQuick(i, getQuick(i)*y.getQuick(i)); // x[i] * y[i] for all nonZeros
                    j++;
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
                    for (int index = nonZeroIndexes.Size(); --index >= 0;)
                    {
                        int i = nonZeroElements[index];
                        setQuick(i, getQuick(i) + y.getQuick(i));
                    }
                }
                else if (multiplicator == -1)
                {
                    // x[i] = x[i] - y[i]
                    for (int index = nonZeroIndexes.Size(); --index >= 0;)
                    {
                        int i = nonZeroElements[index];
                        setQuick(i, getQuick(i) - y.getQuick(i));
                    }
                }
                else
                {
                    // the general case x[i] = x[i] + mult*y[i]
                    for (int index = nonZeroIndexes.Size(); --index >= 0;)
                    {
                        int i = nonZeroElements[index];
                        setQuick(i, getQuick(i) + multiplicator*y.getQuick(i));
                    }
                }
            }
            else
            {
                // the general case x[i] = f(x[i],y[i])
                return assign(y, function);
            }
            return this;
        }

        /**
         * Returns the number of cells having non-zero values; ignores tolerance.
         */

        public int cardinality()
        {
            int cardinality = 0;

            for (int i = m_intSize; --i >= 0;)
            {
                if (getQuick(i) != 0)
                {
                    cardinality++;
                }
            }
            return cardinality;
        }

        /**
         * Returns the number of cells having non-zero values, but at most maxCardinality; ignores tolerance.
         */

        public int cardinality(int maxCardinality)
        {
            int cardinality = 0;
            int i = m_intSize;
            while (--i >= 0 && cardinality < maxCardinality)
            {
                if (getQuick(i) != 0)
                {
                    cardinality++;
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

        public DoubleMatrix1D Copy()
        {
            DoubleMatrix1D copy = like();
            copy.assign(this);
            return copy;
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
         * not <code>null</code> and is at least a <code>DoubleMatrix1D</code> object
         * that has the same sizes as the receiver and 
         * has exactly the same values at the same indexes.
         * @param   obj   the object to Compare with.
         * @return  <code>true</code> if the objects are the same;
         *          <code>false</code> otherwise.
         */

        public new bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!(obj is DoubleMatrix1D))
            {
                return false;
            }

            return Property.DEFAULT.Equals(this, (DoubleMatrix1D) obj);
        }

        /**
         * Returns the matrix cell value at coordinate <tt>index</tt>.
         *
         * @param     index   the index of the cell.
         * @return    the value of the specified cell.
         * @throws	HCException if <tt>index&lt;0 || index&gt;=Size()</tt>.
         */

        public double get(int index)
        {
            if (index < 0 || index >= m_intSize)
            {
                checkIndex(index);
            }
            return getQuick(index);
        }

        /**
         * Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
         * Override this method in wrappers.
         */

        public DoubleMatrix1D getContent()
        {
            return this;
        }

        /**
        Fills the coordinates and values of cells having non-zero values into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists all have a new m_intSize, the number of non-zero values.
        <p>
        In general, fill order is <i>unspecified</i>.
        This implementation fills like: <tt>for (index = 0..Size()-1)  do ... </tt>.
        However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
        (Of course, result lists indexes are guaranteed to correspond to the same cell).
        <p>
        <b>Example:</b>
        <br>
        <pre>
        0, 0, 8, 0, 7
        -->
        indexList  = (2,4)
        valueList  = (8,7)
        </pre>
        In other words, <tt>get(2)==8, get(4)==7</tt>.

        @param indexList the list to be filled with indexes, can have any m_intSize.
        @param valueList the list to be filled with values, can have any m_intSize.
        */

        public void getNonZeros(IntArrayList indexList, DoubleArrayList valueList)
        {
            bool fillIndexList = indexList != null;
            bool fillValueList = valueList != null;
            if (fillIndexList)
            {
                indexList.Clear();
            }
            if (fillValueList)
            {
                valueList.Clear();
            }
            int s = m_intSize;
            for (int i = 0; i < s; i++)
            {
                double value = getQuick(i);
                if (value != 0)
                {
                    if (fillIndexList)
                    {
                        indexList.Add(i);
                    }
                    if (fillValueList)
                    {
                        valueList.Add(value);
                    }
                }
            }
        }

        /**
        Fills the coordinates and values of cells having non-zero values into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists all have a new m_intSize, the number of non-zero values.
        <p>
        In general, fill order is <i>unspecified</i>.
        This implementation fills like: <tt>for (index = 0..Size()-1)  do ... </tt>.
        However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
        (Of course, result lists indexes are guaranteed to correspond to the same cell).
        <p>
        <b>Example:</b>
        <br>
        <pre>
        0, 0, 8, 0, 7
        -->
        indexList  = (2,4)
        valueList  = (8,7)
        </pre>
        In other words, <tt>get(2)==8, get(4)==7</tt>.

        @param indexList the list to be filled with indexes, can have any m_intSize.
        @param valueList the list to be filled with values, can have any m_intSize.
        */

        public void getNonZeros(IntArrayList indexList, DoubleArrayList valueList, int maxCardinality)
        {
            bool fillIndexList = indexList != null;
            bool fillValueList = valueList != null;
            int card = cardinality(maxCardinality);
            if (fillIndexList)
            {
                indexList.setSize(card);
            }
            if (fillValueList)
            {
                valueList.setSize(card);
            }
            if (!(card < maxCardinality))
            {
                return;
            }

            if (fillIndexList)
            {
                indexList.setSize(0);
            }
            if (fillValueList)
            {
                valueList.setSize(0);
            }
            int s = m_intSize;
            for (int i = 0; i < s; i++)
            {
                double value = getQuick(i);
                if (value != 0)
                {
                    if (fillIndexList)
                    {
                        indexList.Add(i);
                    }
                    if (fillValueList)
                    {
                        valueList.Add(value);
                    }
                }
            }
        }

        /**
         * Returns the matrix cell value at coordinate <tt>index</tt>.
         *
         * <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>index&lt;0 || index&gt;=Size()</tt>.
         *
         * @param     index   the index of the cell.
         * @return    the value of the specified cell.
         */
        public abstract double getQuick(int index);
        /**
         * Returns <tt>true</tt> if both matrices share at least one identical cell.
         */

        public bool haveSharedCells(DoubleMatrix1D other)
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

        public bool haveSharedCellsRaw(DoubleMatrix1D other)
        {
            return false;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same m_intSize.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix1D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix1D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @return  a new empty matrix of the same dynamic type.
         */

        public DoubleMatrix1D like()
        {
            return like(m_intSize);
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified m_intSize.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix1D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix1D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intSize the number of cell the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */
        public abstract DoubleMatrix1D like(int m_intSize);
        /**
         * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix1D</tt> the new matrix must be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix1D</tt> the new matrix must be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         *
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */
        public abstract DoubleMatrix2D like2D(int rows, int columns);
        /**
         * Sets the matrix cell at coordinate <tt>index</tt> to the specified value.
         *
         * @param     index   the index of the cell.
         * @param    value the value to be filled into the specified cell.
         * @throws	HCException if <tt>index&lt;0 || index&gt;=Size()</tt>.
         */

        public void set(int index, double value)
        {
            if (index < 0 || index >= m_intSize)
            {
                checkIndex(index);
            }
            setQuick(index, value);
        }

        /**
         * Sets the matrix cell at coordinate <tt>index</tt> to the specified value.
         *
         * <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>index&lt;0 || index&gt;=Size()</tt>.
         *
         * @param     index   the index of the cell.
         * @param    value the value to be filled into the specified cell.
         */
        public abstract void setQuick(int index, double value);
        /**
        Swaps each element <tt>this[i]</tt> with <tt>other[i]</tt>.
        @throws ArgumentException if <tt>Size() != other.Size()</tt>.
        */

        public void swap(DoubleMatrix1D other)
        {
            checkSize(other);
            for (int i = m_intSize; --i >= 0;)
            {
                double tmp = getQuick(i);
                setQuick(i, other.getQuick(i));
                other.setQuick(i, tmp);
            }
            return;
        }

        /**
        Constructs and returns a 1-dimensional array containing the cell values.
        The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        The returned array <tt>values</tt> has the form 
        <br>
        <tt>for (int i=0; i < Size(); i++) values[i] = get(i);</tt>

        @return an array filled with the values of the cells.
        */

        public double[] ToArray()
        {
            double[] values = new double[m_intSize];
            ToArray(values);
            return values;
        }

        /**
        Fills the cell values into the specified 1-dimensional array.
        The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        After this call returns the array <tt>values</tt> has the form 
        <br>
        <tt>for (int i=0; i < Size(); i++) values[i] = get(i);</tt>

        @throws ArgumentException if <tt>values.Length < Size()</tt>.
        */

        public void ToArray(double[] values)
        {
            if (values.Length < m_intSize)
            {
                throw new ArgumentException("values too small");
            }
            for (int i = m_intSize; --i >= 0;)
            {
                values[i] = getQuick(i);
            }
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

        public DoubleMatrix1D view()
        {
            return (DoubleMatrix1D) Clone();
        }

        /**
        Constructs and returns a new <i>flip view</i>.
        What used to be index <tt>0</tt> is now index <tt>Size()-1</tt>, ..., what used to be index <tt>Size()-1</tt> is now index <tt>0</tt>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        @return a new flip view.
        */

        public DoubleMatrix1D viewFlip()
        {
            return (DoubleMatrix1D) (view().vFlip());
        }

        /**
        Constructs and returns a new <i>sub-range view</i> that is a <tt>width</tt> sub matrix starting at <tt>index</tt>.

        Operations on the returned view can only be applied to the restricted range.
        Any attempt to access coordinates not contained in the view will throw an <tt>HCException</tt>.
        <p>
        <b>Note that the view is really just a range restriction:</b> 
        The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa. 
        <p>
        The view contains the cells from <tt>index..index+width-1</tt>.
        and has <tt>view.Size() == width</tt>.
        A view's legal coordinates are again zero based, as usual.
        In other words, legal coordinates of the view are <tt>0 .. view.Size()-1==width-1</tt>.
        As usual, any attempt to access a cell at other coordinates will throw an <tt>HCException</tt>.

        @param     index   The index of the first cell.
        @param     width   The width of the range.
        @throws	HCException if <tt>index<0 || width<0 || index+width>Size()</tt>.
        @return the new view.
		
        */

        public DoubleMatrix1D viewPart(int index, int width)
        {
            return (DoubleMatrix1D) (view().vPart(index, width));
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        There holds <tt>view.Size() == indexes.Length</tt> and <tt>view.get(i) == get(indexes[i])</tt>.
        Indexes can occur multiple times and can be in arbitrary order.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        this     = (0,0,8,0,7)
        indexes  = (0,2,4,2)
        -->
        view     = (0,8,7,8)
        </pre>
        Note that modifying <tt>indexes</tt> after this call has returned has no effect on the view.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param  indexes   The indexes of the cells that shall be visible in the new view. To indicate that <i>all</i> cells shall be visible, simply set this parameter to <tt>null</tt>.
        @return the new view.
        @throws HCException if <tt>!(0 <= indexes[i] < Size())</tt> for any <tt>i=0..indexes.Length-1</tt>.
        */

        public DoubleMatrix1D viewSelection(int[] indexes)
        {
            // check for "all"
            if (indexes == null)
            {
                indexes = new int[m_intSize];
                for (int i = m_intSize; --i >= 0;)
                {
                    indexes[i] = i;
                }
            }

            checkIndexes(indexes);
            int[] offsets = new int[indexes.Length];
            for (int i = indexes.Length; --i >= 0;)
            {
                offsets[i] = index(indexes[i]);
            }
            return viewSelectionLike(offsets);
        }

        /**
        Constructs and returns a new <i>selection view</i> that is a matrix holding the cells matching the given condition.
        Applies the condition to each cell and takes only those cells where <tt>condition.Apply(get(i))</tt> yields <tt>true</tt>.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        // extract and view all cells with even value
        matrix = 0 1 2 3 
        matrix.viewSelection( 
        &nbsp;&nbsp;&nbsp;new DoubleProcedure() {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public  bool Apply(double a) { return a % 2 == 0; }
        &nbsp;&nbsp;&nbsp;}
        );
        -->
        matrix ==  0 2
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 

        @param  condition The condition to be matched.
        @return the new view.
        */

        public DoubleMatrix1D viewSelection(
            DoubleProcedure condition)
        {
            IntArrayList matches = new IntArrayList();
            for (int i = 0; i < m_intSize; i++)
            {
                if (condition.Apply(getQuick(i)))
                {
                    matches.Add(i);
                }
            }
            matches.trimToSize();
            return viewSelection(matches.elements());
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param offsets the offsets of the visible elements.
         * @return  a new view.
         */
        public abstract DoubleMatrix1D viewSelectionLike(int[] offsets);
        /**
        Sorts the vector into ascending order, according to the <i>natural ordering</i>.
        This sort is guaranteed to be <i>stable</i>.
        For further information, see {@link doublealgo.Sorting#Sort(DoubleMatrix1D)}.
        For more advanced sorting functionality, see {@link doublealgo.Sorting}.
        @return a new sorted vector (matrix) view.
        */

        public DoubleMatrix1D viewSorted()
        {
            return SortingDoubleAlgo.mergeSort.Sort(this);
        }

        /**
        Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        More specifically, the view has m_intSize <tt>Size()/stride</tt> holding cells <tt>get(i*stride)</tt> for all <tt>i = 0..Size()/stride - 1</tt>.

        @param  stride  the step factor.
        @throws	HCException if <tt>stride <= 0</tt>.
        @return the new view.
		
        */

        public DoubleMatrix1D viewStrides(int stride)
        {
            return (DoubleMatrix1D) (view().vStrides(stride));
        }

        /**
         * Applies a procedure to each cell's value.
         * Iterates downwards from <tt>[Size()-1]</tt> to <tt>[0]</tt>,
         * as demonstrated by this snippet:
         * <pre>
         * for (int i=Size(); --i >=0;) {
         *    if (!procedure.Apply(getQuick(i))) return false;
         * }
         * return true;
         * </pre>
         * Note that an implementation may use more efficient techniques, but must not use any other order.
         *
         * @param procedure a procedure object taking as argument the current cell's value. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        private bool xforEach(DoubleProcedure procedure)
        {
            for (int i = m_intSize; --i >= 0;)
            {
                if (!procedure.Apply(getQuick(i)))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>.
         * Where <tt>x == this</tt>.
         * Operates on cells at indexes <tt>0 .. Math.Min(Size(),y.Size())</tt>.
         * @param y the second vector.
         * @return the sum of products.
         */

        public double zDotProduct(DoubleMatrix1D y)
        {
            return zDotProduct(y, 0, m_intSize);
        }

        /**
         * Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>.
         * Where <tt>x == this</tt>.
         * Operates on cells at indexes <tt>from .. Min(Size(),y.Size(),from+Length)-1</tt>. 
         * @param y the second vector.
         * @param from the first index to be considered.
         * @param Length the number of cells to be considered.
         * @return the sum of products; zero if <tt>from<0 || Length<0</tt>.
         */

        public double zDotProduct(DoubleMatrix1D y, int from, int Length)
        {
            if (from < 0 || Length <= 0)
            {
                return 0;
            }

            int tail = from + Length;
            if (m_intSize < tail)
            {
                tail = m_intSize;
            }
            if (y.m_intSize < tail)
            {
                tail = y.m_intSize;
            }
            Length = tail - from;

            double sum = 0;
            int i = tail - 1;
            for (int k = Length; --k >= 0; i--)
            {
                sum += getQuick(i)*y.getQuick(i);
            }
            return sum;
        }

        /**
         * Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>.
         * Where <tt>x == this</tt>.
         * @param y the second vector.
         * @param nonZeroIndexes the indexes of cells in <tt>y</tt>having a non-zero value.
         * @return the sum of products.
         */

        public double zDotProduct(DoubleMatrix1D y, int from, int Length, IntArrayList nonZeroIndexes)
        {
            // determine minimum Length
            if (from < 0 || Length <= 0)
            {
                return 0;
            }

            int tail = from + Length;
            if (m_intSize < tail)
            {
                tail = m_intSize;
            }
            if (y.m_intSize < tail)
            {
                tail = y.m_intSize;
            }
            Length = tail - from;
            if (Length <= 0)
            {
                return 0;
            }

            // setup
            int[] nonZeroIndexElements = nonZeroIndexes.elements();
            int index = 0;
            int s = nonZeroIndexes.Size();

            // skip to start	
            while ((index < s) && nonZeroIndexElements[index] < from)
            {
                index++;
            }

            // now the sparse dot product
            int i;
            double sum = 0;
            while ((--Length >= 0) && (index < s) && ((i = nonZeroIndexElements[index]) < tail))
            {
                sum += getQuick(i)*y.getQuick(i);
                index++;
            }

            return sum;
        }

        /**
         * Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>.
         * Where <tt>x == this</tt>.
         * @param y the second vector.
         * @param nonZeroIndexes the indexes of cells in <tt>y</tt>having a non-zero value.
         * @return the sum of products.
         */

        public double zDotProduct(DoubleMatrix1D y, IntArrayList nonZeroIndexes)
        {
            return zDotProduct(y, 0, m_intSize, nonZeroIndexes);
            /*
            double sum = 0;
            int[] nonZeroIndexElements = nonZeroIndexes.elements();
            for (int index=nonZeroIndexes.Size(); --index >= 0; ) {
                int i = nonZeroIndexElements[index];
                sum += getQuick(i) * y.getQuick(i);
            }
            return sum;
            */
        }

        /**
         * Returns the sum of all cells; <tt>Sum( x[i] )</tt>.
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
