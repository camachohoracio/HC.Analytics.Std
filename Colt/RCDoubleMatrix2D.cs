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

    ////import DoubleArrayList;
    ////import IntArrayList;
    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    Sparse row-compressed 2-d matrix holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Internally uses the standard sparse row-compressed format, with two important differences that broaden the applicability of this storage format:
    <ul>
    <li>We use a {@link IntArrayList} and {@link DoubleArrayList} to hold the column indexes and nonzero values, respectively. 
    This improves set(...) performance, because the standard way of using non-resizable primitive arrays causes excessive memory allocation, garbage collection and array copying.
    The small downside of this is that set(...,0) does not free memory (The capacity of an arraylist does not shrink upon element removal).
    <li>Column indexes are kept sorted within a row. This both improves get and set performance on m_intRows with many non-zeros, because we can use a binary search. 
    (Experiments show that this hurts < 10% on m_intRows with < 4 nonZeros.)
    </ul>
    <br>
    Note that this implementation is not lock.
    <p>
    <b>Memory requirements:</b>
    <p>
    Cells that
    <ul>
    <li>are never set to non-zero values do not use any memory.
    <li>switch from zero to non-zero state do use memory.
    <li>switch back from non-zero to zero state also do use memory. Their memory is <i>not</i> automatically reclaimed (because of the lists vs. arrays). Reclamation can be triggered via {@link #trimToSize()}.
    </ul>
    <p>
    <tt>memory [bytes] = 4*m_intRows + 12 * nonZeros</tt>.
    <br>Where <tt>nonZeros = cardinality()</tt> is the number of non-zero cells.
    Thus, a 1000 x 1000 matrix with 1000000 non-zero cells consumes 11.5 MB.
    The same 1000 x 1000 matrix with 1000 non-zero cells consumes 15 KB.
    <p>
    <b>Time complexity:</b>
    <p>
    Getting a cell value takes time<tt> O(log nzr)</tt> where <tt>nzr</tt> 
      is the number of non-zeros of the touched row. This is usually quick, because 
      typically there are only few nonzeros per row. So, in practice, get has <i>expected</i> 
      constant time. Setting a cell value takes <i> </i>worst-case time <tt>O(nz)</tt> 
      where <tt>nzr</tt> is the total number of non-zeros in the matrix. This can 
      be extremely slow, but if you traverse coordinates properly (i.e. upwards), each write is done much quicker:
    <table>
    <td class="PRE"> 
    <pre>
    // rather quick
    matrix.assign(0);
    for (int row=0; row < m_intRows; row++) {
    &nbsp;&nbsp;&nbsp;for (int column=0; column < m_intColumns; column++) {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (someCondition) matrix.setQuick(row,column,someValue);
    &nbsp;&nbsp;&nbsp;}
    }

    // poor
    matrix.assign(0);
    for (int row=m_intRows; --row >= 0; ) {
    &nbsp;&nbsp;&nbsp;for (int column=m_intColumns; --column >= 0; ) {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (someCondition) matrix.setQuick(row,column,someValue);
    &nbsp;&nbsp;&nbsp;}
    }
    </pre>
    </td>
    </table>
    If for whatever reasons you can't iterate properly, consider to create an empty dense matrix, store your non-zeros in it, then call <tt>sparse.assign(dense)</tt>. Under the circumstances, this is still rather quick.
    <p>
    Fast iteration over non-zeros can be done via {@link #forEachNonZero}, which supplies your function with row, column and value of each nonzero.
    Although the internally implemented version is a bit more sophisticated,
    here is how a quite efficient user-level matrix-vector multiplication could look like:
    <table>
    <td class="PRE"> 
    <pre>
    // Linear algebraic y = A * x
    A.forEachNonZero(
    &nbsp;&nbsp;&nbsp;new IntIntDoubleFunction() {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public double Apply(int row, int column, double value) {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;y.setQuick(row,y.getQuick(row) + value * x.getQuick(column));
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return value;
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
    &nbsp;&nbsp;&nbsp;}
    );
    </pre>
    </td>
    </table>
    <p>
    Here is how a a quite efficient user-level combined scaling operation could look like:
    <table>
    <td class="PRE"> 
    <pre>
    // Elementwise A = A + alpha*B
    B.forEachNonZero(
    &nbsp;&nbsp;&nbsp;new IntIntDoubleFunction() {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public double Apply(int row, int column, double value) {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;A.setQuick(row,column,A.getQuick(row,column) + alpha*value);
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return value;
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
    &nbsp;&nbsp;&nbsp;}
    );
    </pre>
    </td>
    </table>
    Method {@link #assign(DoubleMatrix2D,DoubleDoubleFunction)} does just that if you supply {@link Functions#plusMult} as argument.


    @author wolfgang.hoschek@cern.ch
    @version 0.9, 04/14/2000
    */

    [Serializable]
    public class RCDoubleMatrix2D : WrapperDoubleMatrix2D
    {
        /*
         * The elements of the matrix.
         */
        public IntArrayList m_indexes;
        public int[] m_starts;
        public DoubleArrayList m_values;
        //public int N;
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

        public RCDoubleMatrix2D(double[,] values)
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

        public RCDoubleMatrix2D(int m_intRows, int m_intColumns)
            : base(null)
        {
            try
            {
                setUp(m_intRows, m_intColumns);
            }
            catch (ArgumentException exc)
            {
                // we can hold m_intRows*m_intColumns>int.MaxValue cells !
                if (!"matrix too large".Equals(exc.Message))
                {
                    throw;
                }
            }
            m_indexes = new IntArrayList();
            m_values = new DoubleArrayList();
            m_starts = new int[m_intRows + 1];
        }

        /**
         * Sets all cells to the state specified by <tt>value</tt>.
         * @param    value the value to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         */

        public new DoubleMatrix2D assign(double value)
        {
            // overriden for performance only
            if (value == 0)
            {
                m_indexes.Clear();
                m_values.Clear();
                for (int i = m_starts.Length; --i >= 0;)
                {
                    m_starts[i] = 0;
                }
            }
            else
            {
                base.assign(value);
            }
            return this;
        }

        public new DoubleMatrix2D assign(DoubleFunction function)
        {
            if (function is Mult)
            {
                // x[i] = mult*x[i]
                double alpha = ((Mult) function).m_multiplicator;
                if (alpha == 1)
                {
                    return this;
                }
                if (alpha == 0)
                {
                    return assign(0);
                }
                if (double.IsNaN(alpha))
                {
                    return assign(alpha); // the funny definition of IsNaN(). This should better not happen.
                }

                double[] vals = m_values.elements();
                for (int j = m_values.Size(); --j >= 0;)
                {
                    vals[j] *= alpha;
                }

                /*
                forEachNonZero(
                    new IntIntDoubleFunction() {
                        public double Apply(int i, int j, double value) {
                            return function.Apply(value);
                        }
                    }
                );
                */
            }
            else
            {
                base.assign(function);
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
            if (source == this)
            {
                return this; // nothing to do
            }
            checkShape(source);
            // overriden for performance only
            if (!(source is RCDoubleMatrix2D))
            {
                //return base.assign(source);

                assign(0);
                source.forEachNonZero(
                    new IntIntDoubleFunction4_(this));
                /*
                indexes.Clear();
                values.Clear();
                int nonZeros=0;
                for (int row=0; row<m_intRows; row++) {
                    starts[row]=nonZeros;
                    for (int column=0; column<m_intColumns; column++) {
                        double v = source.getQuick(row,column);
                        if (v!=0) {
                            values.Add(v);
                            indexes.Add(column);
                            nonZeros++;
                        }
                    }
                }
                starts[m_intRows]=nonZeros;
                */
                return this;
            }

            // even quicker
            RCDoubleMatrix2D other = (RCDoubleMatrix2D) source;

            Array.Copy(other.m_starts, 0, m_starts, 0, m_starts.Length);
            int s = other.m_indexes.Size();
            m_indexes.setSize(s);
            m_values.setSize(s);
            m_indexes.replaceFromToWithFrom(0, s - 1, other.m_indexes, 0);
            m_values.replaceFromToWithFrom(0, s - 1, other.m_values, 0);

            return this;
        }

        public new DoubleMatrix2D assign(DoubleMatrix2D y, DoubleDoubleFunction function)
        {
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
                    new IntIntDoubleFunction11_(
                        this,
                        alpha)
                    );
                return this;
            }

            if (function == Functions.m_mult)
            {
                // x[i] = x[i] * y[i]
                int[] idx = m_indexes.elements();
                double[] vals = m_values.elements();

                for (int i = m_starts.Length - 1; --i >= 0;)
                {
                    int low = m_starts[i];
                    for (int k = m_starts[i + 1]; --k >= low;)
                    {
                        int j = idx[k];
                        vals[k] *= y.getQuick(i, j);
                        if (vals[k] == 0)
                        {
                            remove(i, j);
                        }
                    }
                }
                return this;
            }

            if (function == Functions.m_div)
            {
                // x[i] = x[i] / y[i]
                int[] idx = m_indexes.elements();
                double[] vals = m_values.elements();

                for (int i = m_starts.Length - 1; --i >= 0;)
                {
                    int low = m_starts[i];
                    for (int k = m_starts[i + 1]; --k >= low;)
                    {
                        int j = idx[k];
                        vals[k] /= y.getQuick(i, j);
                        if (vals[k] == 0)
                        {
                            remove(i, j);
                        }
                    }
                }
                return this;
            }

            return base.assign(y, function);
        }

        public new DoubleMatrix2D forEachNonZero(IntIntDoubleFunction function)
        {
            int[] idx = m_indexes.elements();
            double[] vals = m_values.elements();

            for (int i = m_starts.Length - 1; --i >= 0;)
            {
                int low = m_starts[i];
                for (int k = m_starts[i + 1]; --k >= low;)
                {
                    int j = idx[k];
                    double value = vals[k];
                    double r = function.Apply(i, j, value);
                    if (r != value)
                    {
                        vals[k] = r;
                    }
                }
            }
            return this;
        }

        /**
         * Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
         * Override this method in wrappers.
         */

        public new DoubleMatrix2D getContent()
        {
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

        public new double getQuick(int row, int column)
        {
            int k = m_indexes.binarySearchFromTo(column, m_starts[row], m_starts[row + 1] - 1);
            double v = 0;
            if (k >= 0)
            {
                v = m_values.getQuick(k);
            }
            return v;
        }

        public void Insert(int row, int column, int index, double value)
        {
            m_indexes.beforeInsert(index, column);
            m_values.beforeInsert(index, value);
            for (int i = m_starts.Length; --i > row;)
            {
                m_starts[i]++;
            }
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

        public new DoubleMatrix2D like(int m_intRows, int m_intColumns)
        {
            return new RCDoubleMatrix2D(m_intRows, m_intColumns);
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

        public void remove(int row, int index)
        {
            m_indexes.remove(index);
            m_values.remove(index);
            for (int i = m_starts.Length; --i > row;)
            {
                m_starts[i]--;
            }
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

        public new void setQuick(int row, int column, double value)
        {
            int k = m_indexes.binarySearchFromTo(column, m_starts[row], m_starts[row + 1] - 1);
            if (k >= 0)
            {
                // found
                if (value == 0)
                {
                    remove(row, k);
                }
                else
                {
                    m_values.setQuick(k, value);
                }
                return;
            }

            if (value != 0)
            {
                k = -k - 1;
                Insert(row, column, k, value);
            }
        }

        public new void trimToSize()
        {
            m_indexes.trimToSize();
            m_values.trimToSize();
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

            bool ignore = (z == null || !transposeA);
            if (z == null)
            {
                z = new DenseDoubleMatrix1D(m);
            }

            if (!(y is DenseDoubleMatrix1D && z is DenseDoubleMatrix1D))
            {
                return base.zMult(y, z, alpha, beta, transposeA);
            }

            if (n != y.Size() || m > z.Size())
            {
                throw new ArgumentException("Incompatible args: " + ((transposeA ? viewDice() : this).toStringShort()) +
                                            ", " + y.toStringShort() + ", " + z.toStringShort());
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

            /*
            forEachNonZero(
                new IntIntDoubleFunction() {
                    public double Apply(int i, int j, double value) {
                        zElements[zi + zStride*i] += value * yElements[yi + yStride*j];
                        //z.setQuick(row,z.getQuick(row) + value * y.getQuick(column));
                        //PrintToScreen.WriteLine("["+i+","+j+"]-->"+value);
                        return value;
                    }
                }
            );
            */


            int[] idx = m_indexes.elements();
            double[] vals = m_values.elements();
            int s = m_starts.Length - 1;
            if (!transposeA)
            {
                for (int i = 0; i < s; i++)
                {
                    int high = m_starts[i + 1];
                    double sum = 0;
                    for (int k = m_starts[i]; k < high; k++)
                    {
                        int j = idx[k];
                        sum += vals[k]*yElements[yi + yStride*j];
                    }
                    zElements[zi] = alpha*sum + beta*zElements[zi];
                    zi += zStride;
                }
            }
            else
            {
                if (!ignore)
                {
                    z.assign(Functions.mult(beta));
                }
                for (int i = 0; i < s; i++)
                {
                    int high = m_starts[i + 1];
                    double yElem = alpha*yElements[yi + yStride*i];
                    for (int k = m_starts[i]; k < high; k++)
                    {
                        int j = idx[k];
                        zElements[zi + zStride*j] += vals[k]*yElem;
                    }
                }
            }

            return z;
        }

        public new DoubleMatrix2D zMult(DoubleMatrix2D B, DoubleMatrix2D C, double alpha, double beta, bool transposeA,
                                        bool transposeB)
        {
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

            int[] idx = m_indexes.elements();
            double[] vals = m_values.elements();
            for (int i = m_starts.Length - 1; --i >= 0;)
            {
                int low = m_starts[i];
                for (int k = m_starts[i + 1]; --k >= low;)
                {
                    int j = idx[k];
                    fun.m_multiplicator = vals[k]*alpha;
                    if (!transposeA)
                    {
                        Crows[i].assign(Brows[j], fun);
                    }
                    else
                    {
                        Crows[j].assign(Brows[i], fun);
                    }
                }
            }

            return C;
        }
    }
}
