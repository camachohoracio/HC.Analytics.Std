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

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    Tridiagonal 2-d matrix holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    TODO.

    @author wolfgang.hoschek@cern.ch
    @version 0.9, 04/14/2000
    */

    [Serializable]
    public class TridiagonalDoubleMatrix2D : WrapperDoubleMatrix2D
    {
        /*
         * The non zero elements of the matrix: {lower, diagonal, upper}.
         */

        public static int NONZERO = 4;
        public int[] dims;
        public double[] values;

        //public double diagonal[];
        //public double lower[];
        //public double upper[];

        //public int diagonalNonZeros;
        //public int lowerNonZeros;
        //public int upperNonZeros;
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

        public TridiagonalDoubleMatrix2D(double[,] values)
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

        public TridiagonalDoubleMatrix2D(int m_intRows, int m_intColumns)
            : base(null)
        {
            setUp(m_intRows, m_intColumns);

            int d = Math.Min(m_intRows, m_intColumns);
            int u = d - 1;
            int l = d - 1;
            if (m_intRows > m_intColumns)
            {
                l++;
            }
            if (m_intRows < m_intColumns)
            {
                u++;
            }

            values = new double[l + d + u]; // {lower, diagonal, upper}
            int[] dimensions = {0, l, l + d, l + d + u, 0, 0, 0};
            // {lowerStart, diagonalStart, upperStart, values.Length, lowerNonZeros, diagonalNonZeros, upperNonZeros}
            dims = dimensions;

            //diagonal = new double[d];
            //lower = new double[l];
            //upper = new double[u];

            //diagonalNonZeros = 0;
            //lowerNonZeros = 0;
            //upperNonZeros = 0;
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
                for (int i = values.Length; --i >= 0;)
                {
                    values[i] = 0;
                }
                for (int i = dims.Length; --i >= NONZERO;)
                {
                    dims[i] = 0;
                }

                //for (int i=diagonal.Length; --i >= 0; ) diagonal[i]=0;
                //for (int i=upper.Length; --i >= 0; ) upper[i]=0;
                //for (int i=lower.Length; --i >= 0; ) lower[i]=0;

                //diagonalNonZeros = 0;
                //lowerNonZeros = 0;
                //upperNonZeros = 0;
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

                /*
                double[] vals = values.elements();
                for (int j=values.Size(); --j >= 0; ) {
                    vals[j] *= alpha;
                }
                */

                forEachNonZero(
                    new IntIntDoubleFunction3_(function));
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
            // overriden for performance only
            if (source == this)
            {
                return this; // nothing to do
            }
            checkShape(source);

            if (source is TridiagonalDoubleMatrix2D)
            {
                // quickest
                TridiagonalDoubleMatrix2D other = (TridiagonalDoubleMatrix2D) source;

                Array.Copy(other.values, 0, values, 0, values.Length);
                Array.Copy(other.dims, 0, dims, 0, dims.Length);
                return this;
            }

            if (source is RCDoubleMatrix2D || source is SparseDoubleMatrix2D)
            {
                assign(0);
                source.forEachNonZero(
                    new IntIntDoubleFunction5_(this)
                    );
                return this;
            }

            return base.assign(source);
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
                    new IntIntDoubleFunction6_(this, alpha)
                    );
                return this;
            }

            if (function == Functions.m_mult)
            {
                // x[i] = x[i] * y[i]
                forEachNonZero(
                    new IntIntDoubleFunction7_(this,
                                               y)
                    );
                return this;
            }

            if (function == Functions.m_div)
            {
                // x[i] = x[i] / y[i]
                forEachNonZero(
                    new IntIntDoubleFunction8_(
                        this,
                        y)
                    );
                return this;
            }

            return base.assign(y, function);
        }

        public new DoubleMatrix2D forEachNonZero(IntIntDoubleFunction function)
        {
            for (int kind = 0; kind <= 2; kind++)
            {
                int i = 0, j = 0;
                switch (kind)
                {
                    case 0:
                        {
                            i = 1;
                        } // lower 
                        break;
                        // case 1: {   } // diagonal
                    case 2:
                        {
                            j = 1;
                        } // upper
                        break;
                }
                int low = dims[kind];
                int high = dims[kind + 1];

                for (int k = low; k < high; k++, i++, j++)
                {
                    double value = values[k];
                    if (value != 0)
                    {
                        double r = function.Apply(i, j, value);
                        if (r != value)
                        {
                            if (r == 0)
                            {
                                dims[kind + NONZERO]++; // one non zero more
                            }
                            values[k] = r;
                        }
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
            int i = row;
            int j = column;

            int k = j - i + 1;
            int q = i;
            if (k == 0)
            {
                q = j; // lower diagonal
            }

            if (k >= 0 && k <= 2)
            {
                return values[dims[k] + q];
            }
            return 0;


            //int k = -1;
            //int q = 0;

            //if (i==j) { k=0; q=i; }
            //if (i==j+1) { k=1; q=j; }
            //if (i==j-1) { k=2; q=i; }

            //if (k<0) return 0;
            //return values[dims[k]+q];


            //if (i==j) return diagonal[i];
            //if (i==j+1) return lower[j];
            //if (i==j-1) return upper[i];

            //return 0;
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
            return new TridiagonalDoubleMatrix2D(m_intRows, m_intColumns);
        }

        /**
         * Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix2D</tt> the new matrix must be of type <tt>DenseDoubleMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix2D</tt> the new matrix must be of type <tt>SparseDoubleMatrix1D</tt>, etc.
         *
         * @param size the number of cells the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public new DoubleMatrix1D like1D(int size)
        {
            return new SparseDoubleMatrix1D(size);
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
            int i = row;
            int j = column;

            bool isZero = (value == 0);

            int k = j - i + 1;
            int q = i;
            if (k == 0)
            {
                q = j; // lower diagonal
            }

            if (k >= 0 && k <= 2)
            {
                int index = dims[k] + q;
                if (values[index] != 0)
                {
                    if (isZero)
                    {
                        dims[k + NONZERO]--; // one nonZero less
                    }
                }
                else
                {
                    if (!isZero)
                    {
                        dims[k + NONZERO]++; // one nonZero more
                    }
                }
                values[index] = value;
                return;
            }

            if (!isZero)
            {
                throw new ArgumentException("Can't store non-zero value to non-tridiagonal coordinate: row=" + row +
                                            ", column=" + column + ", value=" + value);
            }

            //int k = -1;
            //int q = 0;

            //if (i==j) { k=0; q=i; } // diagonal
            //if (i==j+1) { k=1; q=j; } // lower diagonal
            //if (i==j-1) { k=2; q=i; } // upper diagonal

            //if (k>0) {
            //int index = dims[k]+q;
            //if (values[index]!=0) {
            //if (isZero) dims[k+NONZERO]--; // one nonZero less
            //}
            //else {
            //if (!isZero) dims[k+NONZERO]++; // one nonZero more
            //}
            //values[index] = value;
            //return;
            //}

            //if (!isZero) throw new ArgumentException("Can't store non-zero value to non-tridiagonal coordinate: row="+row+", column="+column+", value="+value);


            //if (i==j) {
            //if (diagonal[i]!=0) {
            //if (isZero) diagonalNonZeros--;
            //}
            //else {
            //if (!isZero) diagonalNonZeros++;
            //}
            //diagonal[i] = value;
            //return;
            //}

            //if (i==j+1) {
            //if (lower[j]!=0) {
            //if (isZero) lowerNonZeros--;
            //}
            //else {
            //if (!isZero) lowerNonZeros++;
            //}
            //lower[j] = value;
            //return;
            //}

            //if (i==j-1) {
            //if (upper[i]!=0) {
            //if (isZero) upperNonZeros--;
            //}
            //else {
            //if (!isZero) upperNonZeros++;
            //}
            //upper[i] = value;
            //return;
            //}

            //if (!isZero) throw new ArgumentException("Can't store non-zero value to non-tridiagonal coordinate: row="+row+", column="+column+", value="+value);
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

            forEachNonZero(
                new IntIntDoubleFunction9_(
                    transposeA,
                    yElements,
                    zElements,
                    zi,
                    zStride,
                    yi,
                    yStride)
                );

            if (alpha != 1)
            {
                z.assign(Functions.mult(alpha));
            }
            return z;
        }

        public new DoubleMatrix2D zMult(
            DoubleMatrix2D B,
            DoubleMatrix2D C,
            double alpha,
            double beta,
            bool transposeA,
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

            forEachNonZero(
                new IntIntDoubleFunction10_(fun,
                                            Brows,
                                            Crows,
                                            transposeA,
                                            alpha));

            return C;
        }
    }
}
