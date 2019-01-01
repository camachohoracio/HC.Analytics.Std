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

    ////import DoubleMatrix2D;
    ////import DoubleMatrix3D;
    /**
    Dense 3-d matrix holding <tt>double</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Internally holds one single contigous one-dimensional array, addressed in (in decreasing order of significance): slice major, row major, column major.
    Note that this implementation is not .
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 8*Slices()*Rows()*Columns()</tt>.
    Thus, a 100*100*100 matrix uses 8 MB.
    <p>
    <b>Time complexity:</b>
    <p>
    <tt>O(1)</tt> (i.e. constant time) for the basic operations
    <tt>get</tt>, <tt>getQuick</tt>, <tt>set</tt>, <tt>setQuick</tt> and <tt>size</tt>,
    <p>
    Applications demanding utmost speed can exploit knowledge about the internal addressing.
    Setting/getting values in a loop slice-by-slice, row-by-row, column-by-column is quicker than, for example, column-by-column, row-by-row, slice-by-slice.
    Thus
    <pre>
       for (int slice=0; slice < m_intSlices; slice++) {
          for (int row=0; row < m_intRows; row++) {
             for (int column=0; column < m_intColumns; column++) {
                matrix.setQuick(slice,row,column,someValue);
             }		    
          }
       }
    </pre>
    is quicker than
    <pre>
       for (int column=0; column < m_intColumns; column++) {
          for (int row=0; row < m_intRows; row++) {
             for (int slice=0; slice < m_intSlices; slice++) {
                matrix.setQuick(slice,row,column,someValue);
             }
          }
       }
    </pre>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DenseDoubleMatrix3D : DoubleMatrix3D
    {
        /**
          * The elements of this matrix.
          * elements are stored in slice major, then row major, then column major, in order of significance, i.e.
          * index==slice*sliceStride+ row*rowStride + column*columnStride
          * i.e. {slice0 row0..m}, {slice1 row0..m}, ..., {sliceN row0..m}
          * with each row storead as 
          * {row0 column0..m}, {row1 column0..m}, ..., {rown column0..m}
          */
        public double[] m_elements;
        /**
         * Constructs a matrix with a copy of the given values.
         * <tt>values</tt> is required to have the form <tt>values[slice,row,column]</tt>
         * and have exactly the same number of m_intRows in in every slice and exactly the same number of m_intColumns in in every row.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param values The values to be filled into the new matrix.
         * @throws ArgumentException if <tt>for any 1 &lt;= slice &lt; values.Length: values[slice].Length != values[slice-1].Length</tt>.
         * @throws ArgumentException if <tt>for any 1 &lt;= row &lt; values.GetLength(1): values[slice,row].Length != values[slice,row-1].Length</tt>.
         */

        public DenseDoubleMatrix3D(double[,,] values)
            : this(
                values.GetLength(0), (values.GetLength(0) == 0 ? 0 : values.GetLength(1)),
                (values.GetLength(0) == 0
                     ? 0
                     : values.GetLength(1) == 0
                           ? 0
                           :
                               values.GetLength(2)))
        {
            assign(values);
        }

        /**
         * Constructs a matrix with a given number of m_intSlices, m_intRows and m_intColumns.
         * All entries are initially <tt>0</tt>.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @throws	ArgumentException if <tt>(double)m_intSlices*m_intColumns*m_intRows > int.MaxValue</tt>.
         * @throws	ArgumentException if <tt>m_intSlices<0 || m_intRows<0 || m_intColumns<0</tt>.
         */

        public DenseDoubleMatrix3D(int m_intSlices, int m_intRows, int m_intColumns)
        {
            setUp(m_intSlices, m_intRows, m_intColumns);
            m_elements = new double[m_intSlices*m_intRows*m_intColumns];
        }

        /**
         * Constructs a view with the given parameters.
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param elements the cells.
         * @param sliceZero the position of the first element.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param sliceStride the number of elements between two m_intSlices, i.e. <tt>index(k+1,i,j)-index(k,i,j)</tt>.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(k,i+1,j)-index(k,i,j)</tt>.
         * @param columnnStride the number of elements between two m_intColumns, i.e. <tt>index(k,i,j+1)-index(k,i,j)</tt>.
         * @throws	ArgumentException if <tt>(double)m_intSlices*m_intColumns*m_intRows > int.MaxValue</tt>.
         * @throws	ArgumentException if <tt>m_intSlices<0 || m_intRows<0 || m_intColumns<0</tt>.
         */

        public DenseDoubleMatrix3D(int m_intSlices, int m_intRows, int m_intColumns, double[] elements, int sliceZero,
                                   int rowZero, int columnZero, int sliceStride, int rowStride, int columnStride)
        {
            setUp(m_intSlices, m_intRows, m_intColumns, sliceZero, rowZero, columnZero, sliceStride, rowStride,
                  columnStride);
            m_elements = elements;
            isNoView = false;
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

        public new DoubleMatrix3D assign(double[,,] values)
        {
            if (isNoView)
            {
                if (values.GetLength(0) != m_intSlices)
                {
                    throw new ArgumentException("Must have same number of m_intSlices: m_intSlices=" +
                                                values.GetLength(0) + "Slices()=" + Slices());
                }

                int i = m_intSlices*m_intRows*m_intColumns - m_intColumns;
                for (int slice = m_intSlices; --slice >= 0;)
                {
                    double[,] currentSlice =
                        ArrayHelper.GetSliceCopy(values, slice);
                    if (currentSlice.GetLength(0) != m_intRows)
                    {
                        throw new ArgumentException("Must have same number of m_intRows in every slice: m_intRows=" +
                                                    currentSlice.GetLength(0) + "Rows()=" + Rows());
                    }
                    for (int row = m_intRows; --row >= 0;)
                    {
                        double[] currentRow =
                            ArrayHelper.GetRowCopy(currentSlice, row);
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
            }
            else
            {
                base.assign(values);
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same number of m_intSlices, m_intRows and m_intColumns.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     source   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Slices() != source.Slices() || Rows() != source.Rows() || Columns() != source.Columns()</tt>
         */

        public new DoubleMatrix3D assign(DoubleMatrix3D source)
        {
            // overriden for performance only
            if (!(source is DenseDoubleMatrix3D))
            {
                return base.assign(source);
            }
            DenseDoubleMatrix3D other = (DenseDoubleMatrix3D) source;
            if (other == this)
            {
                return this;
            }
            checkShape(other);
            if (haveSharedCells(other))
            {
                DoubleMatrix3D c = other.Copy();
                if (!(c is DenseDoubleMatrix3D))
                {
                    // should not happen
                    return base.assign(source);
                }
                other = (DenseDoubleMatrix3D) c;
            }

            if (isNoView && other.isNoView)
            {
                // quickest
                Array.Copy(other.m_elements, 0, m_elements, 0, m_elements.Length);
                return this;
            }
            return base.assign(other);
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

        public override double getQuick(int slice, int row, int column)
        {
            //if (debug) if (slice<0 || slice>=m_intSlices || row<0 || row>=m_intRows || column<0 || column>=m_intColumns) throw new HCException("slice:"+slice+", row:"+row+", column:"+column);
            //return elements[index(slice,row,column)];
            //manually inlined:
            return
                m_elements[
                    m_sliceZero + slice*m_sliceStride + m_rowZero + row*m_rowStride + m_columnZero +
                    column*m_columnStride];
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

        public new bool haveSharedCellsRaw(DoubleMatrix3D other)
        {
            if (other is SelectedDenseDoubleMatrix3D)
            {
                SelectedDenseDoubleMatrix3D otherMatrix = (SelectedDenseDoubleMatrix3D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is DenseDoubleMatrix3D)
            {
                DenseDoubleMatrix3D otherMatrix = (DenseDoubleMatrix3D) other;
                return m_elements == otherMatrix.m_elements;
            }
            return false;
        }

        /**
         * Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
         *
         * @param     slice   the index of the slice-coordinate.
         * @param     row   the index of the row-coordinate.
         * @param     column   the index of the third-coordinate.
         */

        public new int index(int slice, int row, int column)
        {
            //return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
            //manually inlined:
            return m_sliceZero + slice*m_sliceStride + m_rowZero + row*m_rowStride + m_columnZero +
                   column*m_columnStride;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of m_intSlices, m_intRows and m_intColumns.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix3D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix3D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix3D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix3D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intSlices the number of m_intSlices the matrix shall have.
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override DoubleMatrix3D like(int m_intSlices, int m_intRows, int m_intColumns)
        {
            return new DenseDoubleMatrix3D(m_intSlices, m_intRows, m_intColumns);
        }

        /**
         * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         * For example, if the receiver is an instance of type <tt>DenseDoubleMatrix3D</tt> the new matrix must also be of type <tt>DenseDoubleMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseDoubleMatrix3D</tt> the new matrix must also be of type <tt>SparseDoubleMatrix2D</tt>, etc.
         *
         * @param m_intRows the number of m_intRows the matrix shall have.
         * @param m_intColumns the number of m_intColumns the matrix shall have.
         * @param rowZero the position of the first element.
         * @param columnZero the position of the first element.
         * @param rowStride the number of elements between two m_intRows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
         * @param columnStride the number of elements between two m_intColumns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override DoubleMatrix2D like2D(int m_intRows, int m_intColumns, int rowZero, int columnZero,
                                              int rowStride, int columnStride)
        {
            return new DenseDoubleMatrix2D(m_intRows, m_intColumns, m_elements, rowZero, columnZero, rowStride,
                                           columnStride);
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

        public override void setQuick(int slice, int row, int column, double value)
        {
            //if (debug) if (slice<0 || slice>=m_intSlices || row<0 || row>=m_intRows || column<0 || column>=m_intColumns) throw new HCException("slice:"+slice+", row:"+row+", column:"+column);
            //elements[index(slice,row,column)] = value;
            //manually inlined:
            m_elements[
                m_sliceZero + slice*m_sliceStride + m_rowZero + row*m_rowStride + m_columnZero + column*m_columnStride]
                = value;
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param sliceOffsets the offsets of the visible elements.
         * @param rowOffsets the offsets of the visible elements.
         * @param columnOffsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override DoubleMatrix3D viewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseDoubleMatrix3D(m_elements, sliceOffsets, rowOffsets, columnOffsets, 0);
        }

        /**
        27 neighbor stencil transformation. For efficient finite difference operations.
        Applies a function to a moving <tt>3 x 3 x 3</tt> window.
        Does nothing if <tt>Rows() < 3 || Columns() < 3 || Slices() < 3</tt>.
        <pre>
        B[k,i,j] = function.Apply(
        &nbsp;&nbsp;&nbsp;A[k-1,i-1,j-1], A[k-1,i-1,j], A[k-1,i-1,j+1],
        &nbsp;&nbsp;&nbsp;A[k-1,i,  j-1], A[k-1,i,  j], A[k-1,i,  j+1],
        &nbsp;&nbsp;&nbsp;A[k-1,i+1,j-1], A[k-1,i+1,j], A[k-1,i+1,j+1],

        &nbsp;&nbsp;&nbsp;A[k  ,i-1,j-1], A[k  ,i-1,j], A[k  ,i-1,j+1],
        &nbsp;&nbsp;&nbsp;A[k  ,i,  j-1], A[k  ,i,  j], A[k  ,i,  j+1],
        &nbsp;&nbsp;&nbsp;A[k  ,i+1,j-1], A[k  ,i+1,j], A[k  ,i+1,j+1],

        &nbsp;&nbsp;&nbsp;A[k+1,i-1,j-1], A[k+1,i-1,j], A[k+1,i-1,j+1],
        &nbsp;&nbsp;&nbsp;A[k+1,i,  j-1], A[k+1,i,  j], A[k+1,i,  j+1],
        &nbsp;&nbsp;&nbsp;A[k+1,i+1,j-1], A[k+1,i+1,j], A[k+1,i+1,j+1]
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

        Double27Function f = new Double27Function() {
        &nbsp;&nbsp;&nbsp;public  double Apply(
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a000, double a001, double a002,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a010, double a011, double a012,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a020, double a021, double a022,

        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a100, double a101, double a102,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a110, double a111, double a112,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a120, double a121, double a122,

        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a200, double a201, double a202,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a210, double a211, double a212,
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a220, double a221, double a222) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return beta*a111 + alpha*(a000 + ... + a222);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
        };
        A.zAssign27Neighbors(B,f);
        </pre>
	
        @param B the matrix to hold the results.
        @param function the function to be applied to the 27 cells.
        @throws HCException if <tt>function==null</tt>.
        @throws ArgumentException if <tt>Rows() != B.Rows() || Columns() != B.Columns() || Slices() != B.Slices() </tt>.
        */

        public new void zAssign27Neighbors(
            DoubleMatrix3D B,
            Double27Function function)
        {
            // overridden for performance only
            if (!(B is DenseDoubleMatrix3D))
            {
                base.zAssign27Neighbors(B, function);
                return;
            }
            if (function == null)
            {
                throw new HCException("function must not be null.");
            }
            checkShape(B);
            int r = m_intRows - 1;
            int c = m_intColumns - 1;
            if (m_intRows < 3 || m_intColumns < 3 || m_intSlices < 3)
            {
                return; // nothing to do
            }

            DenseDoubleMatrix3D BB = (DenseDoubleMatrix3D) B;
            int A_ss = m_sliceStride;
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

            for (int k = 1; k < m_intSlices - 1; k++)
            {
                int A_index = index(k, 1, 1);
                int B_index = BB.index(k, 1, 1);

                for (int i = 1; i < r; i++)
                {
                    int A002 = A_index - A_ss - A_rs - A_cs;
                    int A012 = A002 + A_rs;
                    int A022 = A012 + A_rs;

                    int A102 = A002 + A_ss;
                    int A112 = A102 + A_rs;
                    int A122 = A112 + A_rs;

                    int A202 = A102 + A_ss;
                    int A212 = A202 + A_rs;
                    int A222 = A212 + A_rs;

                    double a000, a001, a002;
                    double a010, a011, a012;
                    double a020, a021, a022;

                    double a100, a101, a102;
                    double a110, a111, a112;
                    double a120, a121, a122;

                    double a200, a201, a202;
                    double a210, a211, a212;
                    double a220, a221, a222;

                    a000 = elems[A002];
                    A002 += A_cs;
                    a001 = elems[A002];
                    a010 = elems[A012];
                    A012 += A_cs;
                    a011 = elems[A012];
                    a020 = elems[A022];
                    A022 += A_cs;
                    a021 = elems[A022];

                    a100 = elems[A102];
                    A102 += A_cs;
                    a101 = elems[A102];
                    a110 = elems[A112];
                    A112 += A_cs;
                    a111 = elems[A112];
                    a120 = elems[A122];
                    A122 += A_cs;
                    a121 = elems[A122];

                    a200 = elems[A202];
                    A202 += A_cs;
                    a201 = elems[A202];
                    a210 = elems[A212];
                    A212 += A_cs;
                    a211 = elems[A212];
                    a220 = elems[A222];
                    A222 += A_cs;
                    a221 = elems[A222];

                    int B11 = B_index;
                    for (int j = 1; j < c; j++)
                    {
                        // in each step 18 cells can be remembered in registers - they don't need to be reread from slow memory
                        // in each step 9 instead of 27 cells need to be read from memory.
                        a002 = elems[A002 += A_cs];
                        a012 = elems[A012 += A_cs];
                        a022 = elems[A022 += A_cs];

                        a102 = elems[A102 += A_cs];
                        a112 = elems[A112 += A_cs];
                        a122 = elems[A122 += A_cs];

                        a202 = elems[A202 += A_cs];
                        a212 = elems[A212 += A_cs];
                        a222 = elems[A222 += A_cs];

                        B_elems[B11] = function.Apply(
                            a000, a001, a002,
                            a010, a011, a012,
                            a020, a021, a022,
                            a100, a101, a102,
                            a110, a111, a112,
                            a120, a121, a122,
                            a200, a201, a202,
                            a210, a211, a212,
                            a220, a221, a222);
                        B11 += B_cs;

                        // move remembered cells
                        a000 = a001;
                        a001 = a002;
                        a010 = a011;
                        a011 = a012;
                        a020 = a021;
                        a021 = a022;

                        a100 = a101;
                        a101 = a102;
                        a110 = a111;
                        a111 = a112;
                        a120 = a121;
                        a121 = a122;

                        a200 = a201;
                        a201 = a202;
                        a210 = a211;
                        a211 = a212;
                        a220 = a221;
                        a221 = a222;
                    }
                    A_index += A_rs;
                    B_index += B_rs;
                }
            }
        }
    }
}
