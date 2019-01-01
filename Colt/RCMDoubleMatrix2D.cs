#region

using System;

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
    Sparse row-compressed-modified 2-d matrix holding <tt>double</tt> elements.
    @author wolfgang.hoschek@cern.ch
    @version 0.9, 04/14/2000
    */

    [Serializable]
    public class RCMDoubleMatrix2D : WrapperDoubleMatrix2D
    {
        /*
         * The elements of the matrix.
         */
        private readonly IntArrayList[] indexes;
        private readonly DoubleArrayList[] values;
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

        public RCMDoubleMatrix2D(double[,] values)
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

        public RCMDoubleMatrix2D(int m_intRows, int m_intColumns)
            : base(null)
        {
            setUp(m_intRows, m_intColumns);
            indexes = new IntArrayList[m_intRows];
            values = new DoubleArrayList[m_intRows];
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
                for (int row = m_intRows; --row >= 0;)
                {
                    indexes[row] = null;
                    values[row] = null;
                }
            }
            else
            {
                base.assign(value);
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

        public override double getQuick(int row, int column)
        {
            int k = -1;
            if (indexes[row] != null)
            {
                k = indexes[row].binarySearch(column);
            }
            if (k < 0)
            {
                return 0;
            }
            return values[row].getQuick(k);
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
            return new RCMDoubleMatrix2D(m_intRows, m_intColumns);
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

            int k = -1;
            IntArrayList indexList = indexes[i];
            if (indexList != null)
            {
                k = indexList.binarySearch(j);
            }

            if (k >= 0)
            {
                // found
                if (value == 0)
                {
                    DoubleArrayList valueList = values[i];
                    indexList.remove(k);
                    valueList.remove(k);
                    int s = indexList.Size();
                    if (s > 2 && s*3 < indexList.elements().Length)
                    {
                        indexList.setSize(s*3/2);
                        indexList.trimToSize();
                        indexList.setSize(s);

                        valueList.setSize(s*3/2);
                        valueList.trimToSize();
                        valueList.setSize(s);
                    }
                }
                else
                {
                    values[i].setQuick(k, value);
                }
            }
            else
            {
                // not found
                if (value == 0)
                {
                    return;
                }

                k = -k - 1;

                if (indexList == null)
                {
                    indexes[i] = new IntArrayList(3);
                    values[i] = new DoubleArrayList(3);
                }
                indexes[i].beforeInsert(k, j);
                values[i].beforeInsert(k, value);
            }
        }

        /**
         * Linear algebraic matrix-vector multiplication; <tt>z = A * y</tt>.
         * <tt>z[i] = alpha*Sum(A[i,j] * y[j]) + beta*z[i], i=0..A.Rows()-1, j=0..y.Size()-1</tt>.
         * Where <tt>A == this</tt>.
         * @param y the source vector.
         * @param z the vector where results are to be stored.
         * 
         * @throws ArgumentException if <tt>A.Columns() != y.Size() || A.Rows() > z.Size())</tt>.
         */

        public void zMult(DoubleMatrix1D y, DoubleMatrix1D z, IntArrayList nonZeroIndexes, DoubleMatrix1D[] allRows,
                          double alpha, double beta)
        {
            if (m_intColumns != y.Size() || m_intRows > z.Size())
            {
                throw new ArgumentException("Incompatible args: " + toStringShort() + ", " + y.toStringShort() + ", " +
                                            z.toStringShort());
            }

            z.assign(Functions.mult(beta/alpha));
            for (int i = indexes.Length; --i >= 0;)
            {
                if (indexes[i] != null)
                {
                    for (int k = indexes[i].Size(); --k >= 0;)
                    {
                        int j = indexes[i].getQuick(k);
                        double value = values[i].getQuick(k);
                        z.setQuick(i, z.getQuick(i) + value*y.getQuick(j));
                    }
                }
            }

            z.assign(Functions.mult(alpha));
        }
    }
}
