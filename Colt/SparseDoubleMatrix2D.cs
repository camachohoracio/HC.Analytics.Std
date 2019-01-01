#region

using System;
using HC.Analytics.Colt.CustomImplementations;
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
    ////import map.OpenIntDoubleHashMap;
    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    Sparse hashed 2-d matrix holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Note that this implementation is not lock.
    Uses a {@link map.OpenIntDoubleHashMap}, which is a compact and performant hashing technique.
    <p>
    <b>Memory requirements:</b>
    <p>
    Cells that
    <ul>
    <li>are never set to non-zero values do not use any memory.
    <li>switch from zero to non-zero state do use memory.
    <li>switch back from non-zero to zero state also do use memory. However, their memory is automatically reclaimed from time to time. It can also manually be reclaimed by calling {@link #trimToSize()}.
    </ul>
    <p>
    worst case: <tt>memory [bytes] = (1/minLoadFactor) * nonZeros * 13</tt>.
    <br>best  case: <tt>memory [bytes] = (1/maxLoadFactor) * nonZeros * 13</tt>.
    <br>Where <tt>nonZeros = cardinality()</tt> is the number of non-zero cells.
    Thus, a 1000 x 1000 matrix with minLoadFactor=0.25 and maxLoadFactor=0.5 and 1000000 non-zero cells consumes between 25 MB and 50 MB.
    The same 1000 x 1000 matrix with 1000 non-zero cells consumes between 25 and 50 KB.
    <p>
    <b>Time complexity:</b>
    <p>
    This class offers <i>expected</i> time complexity <tt>O(1)</tt> (i.e. constant time) for the basic operations
    <tt>get</tt>, <tt>getQuick</tt>, <tt>set</tt>, <tt>setQuick</tt> and <tt>size</tt>
    assuming the hash function disperses the elements properly among the buckets.
    Otherwise, pathological cases, although highly improbable, can occur, degrading performance to <tt>O(N)</tt> in the worst case.
    As such this sparse class is expected to have no worse time complexity than its dense counterpart {@link DenseDoubleMatrix2D}.
    However, constant factors are considerably larger.
    <p>
    Cells are internally addressed in row-major.
    Performance sensitive applications can exploit this fact.
    Setting values in a loop row-by-row is quicker than column-by-column, because fewer hash collisions occur.
    Thus
    <pre>
        for (int row=0; row < rows; row++) {
            for (int column=0; column < columns; column++) {
                matrix.setQuick(row,column,someValue);
            }
        }
    </pre>
    is quicker than
    <pre>
        for (int column=0; column < columns; column++) {
            for (int row=0; row < rows; row++) {
                matrix.setQuick(row,column,someValue);
            }
        }
    </pre>

    @see map
    @see map.OpenIntDoubleHashMap
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class SparseDoubleMatrix2D : DoubleMatrix2D
    {
        /*
         * The elements of the matrix.
         */
        public int m_dummy;
        public AbstractIntDoubleMap m_elements;

        /**
         * Constructs a matrix with a copy of the given values.
         * <tt>values</tt> is required to have the form <tt>values[row,column]</tt>
         * and have exactly the same number of columns in every row.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param values The values to be filled into the new matrix.
         * @throws ArgumentException if <tt>for any 1 &lt;= row &lt; values.Length: values.GetLength(1) != values[row-1].Length</tt>.
         */

        public SparseDoubleMatrix2D(double[,] values)
            : this(values.GetLength(0), values.GetLength(0) == 0 ? 0 : values.GetLength(1))
        {
            assign(values);
        }

        /**
         * Constructs a matrix with a given number of rows and columns and default memory usage.
         * All entries are initially <tt>0</tt>.
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @throws	ArgumentException if <tt>rows<0 || columns<0 || (double)columns*rows > int.MaxValue</tt>.
         */

        public SparseDoubleMatrix2D(int rows, int columns)
            : this(rows, columns, rows*(columns/1000), 0.2, 0.5)
        {
        }

        /**
         * Constructs a matrix with a given number of rows and columns using memory as specified.
         * All entries are initially <tt>0</tt>.
         * For details related to memory usage see {@link map.OpenIntDoubleHashMap}.
         * 
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @param initialCapacity   the initial capacity of the hash map.
         *                          If not known, set <tt>initialCapacity=0</tt> or small.     
         * @param minLoadFactor        the minimum load factor of the hash map.
         * @param maxLoadFactor        the maximum load factor of the hash map.
         * @throws	ArgumentException if <tt>initialCapacity < 0 || (minLoadFactor < 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor <= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</tt>.
         * @throws	ArgumentException if <tt>rows<0 || columns<0 || (double)columns*rows > int.MaxValue</tt>.
         */

        public SparseDoubleMatrix2D(int rows, int columns, int initialCapacity, double minLoadFactor,
                                    double maxLoadFactor)
        {
            setUp(rows, columns);
            m_elements = new OpenIntDoubleHashMap(initialCapacity, minLoadFactor, maxLoadFactor);
        }

        /**
         * Constructs a view with the given parameters.
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @param elements the cells.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two rows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two columns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @throws	ArgumentException if <tt>rows<0 || columns<0 || (double)columns*rows > int.MaxValue</tt> or flip's are illegal.
         */

        public SparseDoubleMatrix2D(int rows, int columns, AbstractIntDoubleMap elements, int rowZero, int columnZero,
                                    int rowStride, int columnStride)
        {
            setUp(rows, columns, rowZero, columnZero, rowStride, columnStride);
            m_elements = elements;
            isNoView = false;
        }

        /**
         * Sets all cells to the state specified by <tt>value</tt>.
         * @param    value the value to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         */

        public new DoubleMatrix2D assign(double value)
        {
            // overriden for performance only
            if (isNoView && value == 0)
            {
                m_elements.Clear();
            }
            else
            {
                base.assign(value);
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

        public new DoubleMatrix2D assign(DoubleFunction function)
        {
            if (isNoView && function is Mult)
            {
                // x[i] = mult*x[i]
                m_elements.assign(function);
            }
            else
            {
                base.assign(function);
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of rows and columns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     source   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Columns() != source.Columns() || Rows() != source.Rows()</tt>
         */

        public new DoubleMatrix2D assign(DoubleMatrix2D source)
        {
            // overriden for performance only
            if (!(source is SparseDoubleMatrix2D))
            {
                return base.assign(source);
            }
            SparseDoubleMatrix2D other = (SparseDoubleMatrix2D) source;
            if (other == this)
            {
                return this; // nothing to do
            }
            checkShape(other);

            if (isNoView && other.isNoView)
            {
                // quickest
                m_elements.assign(other.m_elements);
                return this;
            }
            return base.assign(source);
        }

        public new DoubleMatrix2D assign(DoubleMatrix2D y, DoubleDoubleFunction function)
        {
            if (!isNoView)
            {
                return base.assign(y, function);
            }

            checkShape(y);

            if (function is PlusMult)
            {
                // x[i] = x[i] + alpha*y[i]
                double alpha = ((PlusMult) function).m_multiplicator;
                if (alpha == 0)
                {
                    return this; // nothing to do
                }
                y.forEachNonZero(
                    new IntIntDoubleFunction2_(this, alpha));
                return this;
            }

            if (function == Functions.m_mult)
            {
                // x[i] = x[i] * y[i]
                m_elements.forEachPair(
                    new IntDoubleProcedure8_(
                        m_elements,
                        y,
                        m_intColumns));
            }

            if (function == Functions.m_div)
            {
                // x[i] = x[i] / y[i]
                m_elements.forEachPair(
                    new IntDoubleProcedure9_(
                        m_elements,
                        y,
                        m_intColumns)
                    );
            }

            return base.assign(y, function);
        }

        /**
         * Returns the number of cells having non-zero values.
         */

        public new int cardinality()
        {
            if (isNoView)
            {
                return m_elements.Size();
            }
            else
            {
                return base.cardinality();
            }
        }

        /**
         * Ensures that the receiver can hold at least the specified number of non-zero cells without needing to allocate new internal memory.
         * If necessary, allocates new internal memory and increases the capacity of the receiver.
         * <p>
         * This method never need be called; it is for performance tuning only.
         * Calling this method before tt>set()</tt>ing a large number of non-zero values boosts performance,
         * because the receiver will grow only once instead of potentially many times and hash collisions get less probable.
         *
         * @param   minNonZeros   the desired minimum number of non-zero cells.
         */

        public new void ensureCapacity(int minCapacity)
        {
            m_elements.ensureCapacity(minCapacity);
        }

        public new DoubleMatrix2D forEachNonZero(IntIntDoubleFunction function)
        {
            if (isNoView)
            {
                m_elements.forEachPair(
                    new IntDoubleProcedure10_(
                        m_elements,
                        function,
                        m_intColumns)
                    );
            }
            else
            {
                base.forEachNonZero(function);
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
            //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new HCException("row:"+row+", column:"+column);
            //return elements.get(index(row,column));
            //manually inlined:
            return m_elements.get(m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride);
        }

        /**
         * Returns <tt>true</tt> if both matrices share common cells.
         * More formally, returns <tt>true</tt> if at least one of the following conditions is met
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
            // return base.index(row,column);
            // manually inlined for speed:
            return m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override DoubleMatrix2D like(int rows, int columns)
        {
            return new SparseDoubleMatrix2D(rows, columns);
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
         * @param offset the index of the first element.
         * @param stride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix1D like1D(int size, int offset, int stride)
        {
            return new SparseDoubleMatrix1D(size, m_elements, offset, stride);
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
            //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new HCException("row:"+row+", column:"+column);
            //int index =	index(row,column);
            //manually inlined:
            int index = m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride;

            //if (value == 0 || Math.Abs(value) < TOLERANCE)
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
         * Releases any superfluous memory created by explicitly putting zero values into cells formerly having non-zero values; 
         * An application can use this operation to minimize the
         * storage of the receiver.
         * <p>
         * <b>Background:</b>
         * <p>
         * Cells that <ul>
         * <li>are never set to non-zero values do not use any memory.
         * <li>switch from zero to non-zero state do use memory.
         * <li>switch back from non-zero to zero state also do use memory. However, their memory can be reclaimed by calling <tt>trimToSize()</tt>.
         * </ul>
         * A sequence like <tt>set(r,c,5); set(r,c,0);</tt>
         * sets a cell to non-zero state and later back to zero state.
         * Such as sequence generates obsolete memory that is automatically reclaimed from time to time or can manually be reclaimed by calling <tt>trimToSize()</tt>.
         * Putting zeros into cells already containing zeros does not generate obsolete memory since no memory was allocated to them in the first place.
         */

        public new void trimToSize()
        {
            m_elements.trimToSize();
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
            return new SelectedSparseDoubleMatrix2D(m_elements, rowOffsets, columnOffsets, 0);
        }

        public new DoubleMatrix1D zMult(DoubleMatrix1D y, DoubleMatrix1D z, double alpha, double beta, bool transposeA)
        {
            int m = m_intRows;
            int n = m_intColumns;
            if (transposeA)
            {
                m = m_intColumns;
                n = m_intRows;
            }

            bool ignore = (z == null);
            if (z == null)
            {
                z = new DenseDoubleMatrix1D(m);
            }

            if (!(isNoView && y is DenseDoubleMatrix1D && z is DenseDoubleMatrix1D))
            {
                return base.zMult(y, z, alpha, beta, transposeA);
            }

            if (n != y.Size() || m > z.Size())
            {
                throw new ArgumentException("Incompatible args: " + ((transposeA ? viewDice() : this).toStringShort()) +
                                            ", " + y.toStringShort() + ", " + z.toStringShort());
            }

            if (!ignore)
            {
                z.assign(Functions.mult(beta/alpha));
            }

            DenseDoubleMatrix1D zz = (DenseDoubleMatrix1D) z;
            double[] zElements = zz.m_elements;
            int zStride = zz.m_intStride;
            int zi = z.index(0);

            DenseDoubleMatrix1D yy = (DenseDoubleMatrix1D) y;
            double[] yElements = yy.m_elements;
            int yStride = yy.m_intStride;
            int yi = y.index(0);

            if (yElements == null || zElements == null)
            {
                throw new HCException();
            }

            m_elements.forEachPair(
                new IntDoubleProcedure11_(
                    transposeA,
                    yElements,
                    zElements,
                    zi,
                    zStride,
                    yi,
                    yStride,
                    m_intColumns)
                );

            /*
            forEachNonZero(
                new IntIntDoubleFunction() {
                    public double Apply(int i, int j, double value) {
                        if (transposeA) { int tmp=i; i=j; j=tmp; }
                        zElements[zi + zStride*i] += value * yElements[yi + yStride*j];
                        //z.setQuick(row,z.getQuick(row) + value * y.getQuick(column));
                        //PrintToScreen.WriteLine("["+i+","+j+"]-->"+value);
                        return value;
                    }
                }
            );
            */

            if (alpha != 1)
            {
                z.assign(Functions.mult(alpha));
            }
            return z;
        }

        public new DoubleMatrix2D zMult(DoubleMatrix2D B, DoubleMatrix2D C, double alpha, double beta, bool transposeA,
                                        bool transposeB)
        {
            if (!(isNoView))
            {
                return base.zMult(B, C, alpha, beta, transposeA, transposeB);
            }
            if (transposeB)
            {
                B = B.viewDice();
            }
            int m = m_intRows;
            int n = m_intColumns;
            if (transposeA)
            {
                m = m_intColumns;
                n = m_intRows;
            }
            int p = B.m_intColumns;
            bool ignore = (C == null);
            if (C == null)
            {
                C = new DenseDoubleMatrix2D(m, p);
            }

            if (B.m_intRows != n)
            {
                throw new ArgumentException("Matrix2D inner dimensions must agree:" + toStringShort() + ", " +
                                            (transposeB ? B.viewDice() : B).toStringShort());
            }
            if (C.m_intRows != m || C.m_intColumns != p)
            {
                throw new ArgumentException("Incompatibel result matrix: " + toStringShort() + ", " +
                                            (transposeB ? B.viewDice() : B).toStringShort() + ", " + C.toStringShort());
            }
            if (this == C || B == C)
            {
                throw new ArgumentException("Matrices must not be identical");
            }

            if (!ignore)
            {
                C.assign(Functions.mult(beta));
            }

            // cache views	
            DoubleMatrix1D[] Brows = new DoubleMatrix1D[n];
            for (int i = n; --i >= 0;)
            {
                Brows[i] = B.viewRow(i);
            }
            DoubleMatrix1D[] Crows = new DoubleMatrix1D[m];
            for (int i = m; --i >= 0;)
            {
                Crows[i] = C.viewRow(i);
            }

            PlusMult fun = PlusMult.plusMult(0);

            m_elements.forEachPair(
                new IntDoubleProcedure12_(
                    fun,
                    Brows,
                    Crows,
                    transposeA,
                    alpha,
                    m_intColumns)
                );

            return C;
        }
    }
}
