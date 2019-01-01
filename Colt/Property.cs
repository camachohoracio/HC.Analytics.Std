#region

using System;
using System.Text;
using HC.Analytics.Colt.CustomImplementations;
using HC.Analytics.Colt.CustomImplementations.tmp;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright © 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package linalg;

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    ////import DoubleMatrix3D;
    /**
    Tests matrices for linear algebraic properties (equality, tridiagonality, symmetry, singularity, etc).
    <p>
    Except where explicitly indicated, all methods involving equality tests (<tt>==</tt>) allow for numerical instability, to a degree specified upon instance construction and returned by method {@link #tolerance()}.
    The public static  variable <tt>DEFAULT</tt> represents a default Property object with a tolerance of <tt>1.0E-9</tt>.
    The public static  variable <tt>ZERO</tt> represents a Property object with a tolerance of <tt>0.0</tt>.
    The public static  variable <tt>TWELVE</tt> represents a Property object with a tolerance of <tt>1.0E-12</tt>.
    As long as you are happy with these tolerances, there is no need to construct Property objects.
    Simply use idioms like <tt>Property.DEFAULT.Equals(A,B)</tt>, <tt>Property.ZERO.Equals(A,B)</tt>, <tt>Property.TWELVE.Equals(A,B)</tt>.
    <p>
    To work with a different tolerance (e.g. <tt>1.0E-15</tt> or <tt>1.0E-5</tt>) use the constructor and/or method {@link #setTolerance(double)}.
    Note that the public static  Property objects are immutable: Is is not possible to alter their tolerance. 
    Any attempt to do so will throw an HCException.
    <p>
    Note that this implementation is not .
    <p>
    Example: <tt>Equals(DoubleMatrix2D A, DoubleMatrix2D B)</tt> is defined as follows
    <table>
    <td class="PRE"> 
    <pre>
    { some other tests not related to tolerance go here }
    double epsilon = tolerance();
    for (int row=rows; --row >= 0;) {
    &nbsp;&nbsp;&nbsp;for (int column=columns; --column >= 0;) {
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;//if (!(A.getQuick(row,column) == B.getQuick(row,column))) return false;
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (Math.Abs(A.getQuick(row,column) - B.getQuick(row,column)) > epsilon) return false;
    &nbsp;&nbsp;&nbsp;}
    }
    return true;
    </pre>
    </td>
    </table>
    Here are some example properties
    <table border="1" cellspacing="0">
      <tr align="left" valign="top"> 
        <td valign="middle" align="left"><tt>matrix</tt></td>
        <td> <tt>4&nbsp;x&nbsp;4&nbsp;<br>
          0&nbsp;0&nbsp;0&nbsp;0<br>
          0&nbsp;0&nbsp;0&nbsp;0<br>
          0&nbsp;0&nbsp;0&nbsp;0<br>
          0&nbsp;0&nbsp;0&nbsp;0 </tt></td>
        <td><tt>4&nbsp;x&nbsp;4<br>
          1&nbsp;0&nbsp;0&nbsp;0<br>
          0&nbsp;0&nbsp;0&nbsp;0<br>
          0&nbsp;0&nbsp;0&nbsp;0<br>
          0&nbsp;0&nbsp;0&nbsp;1 </tt></td>
        <td><tt>4&nbsp;x&nbsp;4<br>
          1&nbsp;1&nbsp;0&nbsp;0<br>
          1&nbsp;1&nbsp;1&nbsp;0<br>
          0&nbsp;1&nbsp;1&nbsp;1<br>
          0&nbsp;0&nbsp;1&nbsp;1 </tt></td>
        <td><tt> 4&nbsp;x&nbsp;4<br>
          0&nbsp;1&nbsp;1&nbsp;1<br>
          0&nbsp;1&nbsp;1&nbsp;1<br>
          0&nbsp;0&nbsp;0&nbsp;1<br>
          0&nbsp;0&nbsp;0&nbsp;1 </tt></td>
        <td><tt> 4&nbsp;x&nbsp;4<br>
          0&nbsp;0&nbsp;0&nbsp;0<br>
          1&nbsp;1&nbsp;0&nbsp;0<br>
          1&nbsp;1&nbsp;0&nbsp;0<br>
          1&nbsp;1&nbsp;1&nbsp;1 </tt></td>
        <td><tt>4&nbsp;x&nbsp;4<br>
          1&nbsp;1&nbsp;0&nbsp;0<br>
          0&nbsp;1&nbsp;1&nbsp;0<br>
          0&nbsp;1&nbsp;0&nbsp;1<br>
          1&nbsp;0&nbsp;1&nbsp;1 </tt><tt> </tt> </td>
        <td><tt>4&nbsp;x&nbsp;4<br>
          1&nbsp;1&nbsp;1&nbsp;0<br>
          0&nbsp;1&nbsp;0&nbsp;0<br>
          1&nbsp;1&nbsp;0&nbsp;1<br>
          0&nbsp;0&nbsp;1&nbsp;1 </tt> </td>
      </tr>
      <tr align="center" valign="middle"> 
        <td><tt>upperBandwidth</tt></td>
        <td> 
          <div align="center"><tt>0</tt></div>
        </td>
        <td> 
          <div align="center"><tt>0</tt></div>
        </td>
        <td> 
          <div align="center"><tt>1</tt></div>
        </td>
        <td><tt>3</tt></td>
        <td align="center" valign="middle"><tt>0</tt></td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>1</tt></div>
        </td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>2</tt></div>
        </td>
      </tr>
      <tr align="center" valign="middle"> 
        <td><tt>lowerBandwidth</tt></td>
        <td> 
          <div align="center"><tt>0</tt></div>
        </td>
        <td> 
          <div align="center"><tt>0</tt></div>
        </td>
        <td> 
          <div align="center"><tt>1</tt></div>
        </td>
        <td><tt>0</tt></td>
        <td align="center" valign="middle"><tt>3</tt></td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>3</tt></div>
        </td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>2</tt></div>
        </td>
      </tr>
      <tr align="center" valign="middle"> 
        <td><tt>semiBandwidth</tt></td>
        <td> 
          <div align="center"><tt>1</tt></div>
        </td>
        <td> 
          <div align="center"><tt>1</tt></div>
        </td>
        <td> 
          <div align="center"><tt>2</tt></div>
        </td>
        <td><tt>4</tt></td>
        <td align="center" valign="middle"><tt>4</tt></td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>4</tt></div>
        </td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>3</tt></div>
        </td>
      </tr>
      <tr align="center" valign="middle"> 
        <td><tt>description</tt></td>
        <td> 
          <div align="center"><tt>zero</tt></div>
        </td>
        <td> 
          <div align="center"><tt>diagonal</tt></div>
        </td>
        <td> 
          <div align="center"><tt>tridiagonal</tt></div>
        </td>
        <td><tt>upper triangular</tt></td>
        <td align="center" valign="middle"><tt>lower triangular</tt></td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>unstructured</tt></div>
        </td>
        <td align="center" valign="middle"> 
          <div align="center"><tt>unstructured</tt></div>
        </td>
      </tr>
    </table>

    @author wolfgang.hoschek@cern.ch
    @version 1.1, 28/May/2000 (fixed strange bugs involving NaN, -inf, inf)
    */

    [Serializable]
    public class Property : PersistentObject
    {
        /**
         * The default Property object; currently has <tt>tolerance()==1.0E-9</tt>.
         */
        public static Property DEFAULT = new Property(1.0E-9);

        /**
         * A Property object with <tt>tolerance()==0.0</tt>.
         */

        /**
         * A Property object with <tt>tolerance()==1.0E-12</tt>.
         */
        public static Property TWELVE = new Property(1.0E-12);
        public static Property ZERO = new Property(0.0);

        public double tolerance_;
        /**
         * Not instantiable by no-arg constructor.
         */

        private Property()
            : this(1.0E-9)
        {
        }

        /**
         * Constructs an instance with a tolerance of <tt>Math.Abs(newTolerance)</tt>.
         */

        public Property(double newTolerance)
        {
            tolerance_ = Math.Abs(newTolerance);
        }

        /**
         * Returns a string with <tt>Length</tt> blanks.
         */

        public static string blanks(int Length)
        {
            if (Length < 0)
            {
                Length = 0;
            }
            StringBuilder buf = new StringBuilder(Length);
            for (int k = 0; k < Length; k++)
            {
                buf.Append(' ');
            }
            return buf.ToString();
        }

        /**
         * Checks whether the given matrix <tt>A</tt> is <i>rectangular</i>.
         * @throws ArgumentException if <tt>A.Rows() < A.Columns()</tt>.
         */

        public void checkRectangular(DoubleMatrix2D A)
        {
            if (A.Rows() < A.Columns())
            {
                throw new ArgumentException("Matrix must be rectangular: " + AbstractFormatter.shape(A));
            }
        }

        /**
         * Checks whether the given matrix <tt>A</tt> is <i>square</i>.
         * @throws ArgumentException if <tt>A.Rows() != A.Columns()</tt>.
         */

        public void checkSquare(DoubleMatrix2D A)
        {
            if (A.Rows() != A.Columns())
            {
                throw new ArgumentException("Matrix must be square: " + AbstractFormatter.shape(A));
            }
        }

        /**
         * Returns the matrix's fraction of non-zero cells; <tt>A.cardinality() / A.Size()</tt>.
         */

        public double density(DoubleMatrix2D A)
        {
            return A.cardinality()/(double) A.Size();
        }

        /**
         * Returns whether all cells of the given matrix <tt>A</tt> are equal to the given value.
         * The result is <tt>true</tt> if and only if <tt>A != null</tt> and
         * <tt>! (Math.Abs(value - A[i]) > tolerance())</tt> holds for all coordinates.
         * @param   A   the first matrix to Compare.
         * @param   value   the value to Compare against.
         * @return  <tt>true</tt> if the matrix is equal to the value;
         *          <tt>false</tt> otherwise.
         */

        public bool Equals(DoubleMatrix1D A, double value)
        {
            if (A == null)
            {
                return false;
            }
            double epsilon = tolerance();
            for (int i = A.Size(); --i >= 0;)
            {
                //if (!(A.getQuick(i) == value)) return false;
                //if (Math.Abs(value - A.getQuick(i)) > epsilon) return false;
                double x = A.getQuick(i);
                double diff = Math.Abs(value - x);
                if ((double.IsNaN(diff)) && ((double.IsNaN(value) && double.IsNaN(x)) || value == x))
                {
                    diff = 0;
                }
                if (!(diff <= epsilon))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns whether both given matrices <tt>A</tt> and <tt>B</tt> are equal.
         * The result is <tt>true</tt> if <tt>A==B</tt>. 
         * Otherwise, the result is <tt>true</tt> if and only if both arguments are <tt>!= null</tt>, 
         * have the same size and 
         * <tt>! (Math.Abs(A[i] - B[i]) > tolerance())</tt> holds for all indexes.
         * @param   A   the first matrix to Compare.
         * @param   B   the second matrix to Compare.
         * @return  <tt>true</tt> if both matrices are equal;
         *          <tt>false</tt> otherwise.
         */

        public bool Equals(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            if (A == B)
            {
                return true;
            }
            if (!(A != null && B != null))
            {
                return false;
            }
            int size = A.Size();
            if (size != B.Size())
            {
                return false;
            }

            double epsilon = tolerance();
            for (int i = size; --i >= 0;)
            {
                //if (!(getQuick(i) == B.getQuick(i))) return false;
                //if (Math.Abs(A.getQuick(i) - B.getQuick(i)) > epsilon) return false;
                double x = A.getQuick(i);
                double value = B.getQuick(i);
                double diff = Math.Abs(value - x);
                if ((double.IsNaN(diff)) && ((double.IsNaN(value) && double.IsNaN(x)) || value == x))
                {
                    diff = 0;
                }
                if (!(diff <= epsilon))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns whether all cells of the given matrix <tt>A</tt> are equal to the given value.
         * The result is <tt>true</tt> if and only if <tt>A != null</tt> and
         * <tt>! (Math.Abs(value - A[row,col]) > tolerance())</tt> holds for all coordinates.
         * @param   A   the first matrix to Compare.
         * @param   value   the value to Compare against.
         * @return  <tt>true</tt> if the matrix is equal to the value;
         *          <tt>false</tt> otherwise.
         */

        public bool Equals(DoubleMatrix2D A, double value)
        {
            if (A == null)
            {
                return false;
            }
            int rows = A.Rows();
            int columns = A.Columns();

            double epsilon = tolerance();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    //if (!(A.getQuick(row,column) == value)) return false;
                    //if (Math.Abs(value - A.getQuick(row,column)) > epsilon) return false;
                    double x = A.getQuick(row, column);
                    double diff = Math.Abs(value - x);
                    if ((double.IsNaN(diff)) && ((double.IsNaN(value) && double.IsNaN(x)) || value == x))
                    {
                        diff = 0;
                    }
                    if (!(diff <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Returns whether both given matrices <tt>A</tt> and <tt>B</tt> are equal.
         * The result is <tt>true</tt> if <tt>A==B</tt>. 
         * Otherwise, the result is <tt>true</tt> if and only if both arguments are <tt>!= null</tt>, 
         * have the same number of columns and rows and 
         * <tt>! (Math.Abs(A[row,col] - B[row,col]) > tolerance())</tt> holds for all coordinates.
         * @param   A   the first matrix to Compare.
         * @param   B   the second matrix to Compare.
         * @return  <tt>true</tt> if both matrices are equal;
         *          <tt>false</tt> otherwise.
         */

        public bool Equals(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            if (A == B)
            {
                return true;
            }
            if (!(A != null && B != null))
            {
                return false;
            }
            int rows = A.Rows();
            int columns = A.Columns();
            if (columns != B.Columns() || rows != B.Rows())
            {
                return false;
            }

            double epsilon = tolerance();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    //if (!(A.getQuick(row,column) == B.getQuick(row,column))) return false;
                    //if (Math.Abs((A.getQuick(row,column) - B.getQuick(row,column)) > epsilon) return false;
                    double x = A.getQuick(row, column);
                    double value = B.getQuick(row, column);
                    double diff = Math.Abs(value - x);
                    if ((double.IsNaN(diff)) && ((double.IsNaN(value) && double.IsNaN(x)) || value == x))
                    {
                        diff = 0;
                    }
                    if (!(diff <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Returns whether all cells of the given matrix <tt>A</tt> are equal to the given value.
         * The result is <tt>true</tt> if and only if <tt>A != null</tt> and
         * <tt>! (Math.Abs(value - A[slice,row,col]) > tolerance())</tt> holds for all coordinates.
         * @param   A   the first matrix to Compare.
         * @param   value   the value to Compare against.
         * @return  <tt>true</tt> if the matrix is equal to the value;
         *          <tt>false</tt> otherwise.
         */

        public bool Equals(DoubleMatrix3D A, double value)
        {
            if (A == null)
            {
                return false;
            }
            int rows = A.Rows();
            int columns = A.Columns();

            double epsilon = tolerance();
            for (int slice = A.Slices(); --slice >= 0;)
            {
                for (int row = rows; --row >= 0;)
                {
                    for (int column = columns; --column >= 0;)
                    {
                        //if (!(A.getQuick(slice,row,column) == value)) return false;
                        //if (Math.Abs(value - A.getQuick(slice,row,column)) > epsilon) return false;
                        double x = A.getQuick(slice, row, column);
                        double diff = Math.Abs(value - x);
                        if ((double.IsNaN(diff)) && ((double.IsNaN(value) && double.IsNaN(x)) || value == x))
                        {
                            diff = 0;
                        }
                        if (!(diff <= epsilon))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /**
         * Returns whether both given matrices <tt>A</tt> and <tt>B</tt> are equal.
         * The result is <tt>true</tt> if <tt>A==B</tt>. 
         * Otherwise, the result is <tt>true</tt> if and only if both arguments are <tt>!= null</tt>, 
         * have the same number of columns, rows and slices, and
         * <tt>! (Math.Abs(A[slice,row,col] - B[slice,row,col]) > tolerance())</tt> holds for all coordinates.
         * @param   A   the first matrix to Compare.
         * @param   B   the second matrix to Compare.
         * @return  <tt>true</tt> if both matrices are equal;
         *          <tt>false</tt> otherwise.
         */

        public bool Equals(DoubleMatrix3D A, DoubleMatrix3D B)
        {
            if (A == B)
            {
                return true;
            }
            if (!(A != null && B != null))
            {
                return false;
            }
            int slices = A.Slices();
            int rows = A.Rows();
            int columns = A.Columns();
            if (columns != B.Columns() || rows != B.Rows() || slices != B.Slices())
            {
                return false;
            }

            double epsilon = tolerance();
            for (int slice = slices; --slice >= 0;)
            {
                for (int row = rows; --row >= 0;)
                {
                    for (int column = columns; --column >= 0;)
                    {
                        //if (!(A.getQuick(slice,row,column) == B.getQuick(slice,row,column))) return false;
                        //if (Math.Abs(A.getQuick(slice,row,column) - B.getQuick(slice,row,column)) > epsilon) return false;
                        double x = A.getQuick(slice, row, column);
                        double value = B.getQuick(slice, row, column);
                        double diff = Math.Abs(value - x);
                        if ((double.IsNaN(diff)) && ((double.IsNaN(value) && double.IsNaN(x)) || value == x))
                        {
                            diff = 0;
                        }
                        if (!(diff <= epsilon))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /**
        Modifies the given matrix square matrix <tt>A</tt> such that it is diagonally dominant by row and column, hence non-singular, hence invertible.
        For testing purposes only.
        @param A the square matrix to modify.
        @throws ArgumentException if <tt>!isSquare(A)</tt>.
        */

        public void generateNonSingular(DoubleMatrix2D A)
        {
            checkSquare(A);
            Functions F = Functions.m_functions;
            int min = Math.Min(A.Rows(), A.Columns());
            for (int i = min; --i >= 0;)
            {
                A.setQuick(i, i, 0);
            }
            for (int i = min; --i >= 0;)
            {
                double rowSum = A.viewRow(i).aggregate(Functions.m_plus, Functions.m_absm);
                double colSum = A.viewColumn(i).aggregate(Functions.m_plus, Functions.m_absm);
                A.setQuick(i, i, Math.Max(rowSum, colSum) + i + 1);
            }
        }

        /**
         */

        public static string get(
            ObjectArrayList list,
            int index)
        {
            return ((string) list.get(index));
        }

        /**
         * A matrix <tt>A</tt> is <i>diagonal</i> if <tt>A[i,j] == 0</tt> whenever <tt>i != j</tt>.
         * Matrix may but need not be square.
         */

        public bool isDiagonal(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    //if (row!=column && A.getQuick(row,column) != 0) return false;
                    if (row != column && !(Math.Abs(A.getQuick(row, column)) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>diagonally dominant by column</i> if the absolute value of each diagonal element is larger than the sum of the absolute values of the off-diagonal elements in the corresponding column.
         * <tt>returns true if for all i: abs(A[i,i]) &gt; Sum(abs(A[j,i])); j != i.</tt>
         * Matrix may but need not be square.
         * <p>
         * Note: Ignores tolerance.
         */

        public bool isDiagonallyDominantByColumn(DoubleMatrix2D A)
        {
            Functions F = Functions.m_functions;
            double epsilon = tolerance();
            int min = Math.Min(A.Rows(), A.Columns());
            for (int i = min; --i >= 0;)
            {
                double diag = Math.Abs(A.getQuick(i, i));
                diag += diag;
                if (diag <= A.viewColumn(i).aggregate(Functions.m_plus, Functions.m_absm))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>diagonally dominant by row</i> if the absolute value of each diagonal element is larger than the sum of the absolute values of the off-diagonal elements in the corresponding row.
         * <tt>returns true if for all i: abs(A[i,i]) &gt; Sum(abs(A[i,j])); j != i.</tt> 
         * Matrix may but need not be square.
         * <p>
         * Note: Ignores tolerance.
         */

        public bool isDiagonallyDominantByRow(DoubleMatrix2D A)
        {
            Functions F = Functions.m_functions;
            double epsilon = tolerance();
            int min = Math.Min(A.Rows(), A.Columns());
            for (int i = min; --i >= 0;)
            {
                double diag = Math.Abs(A.getQuick(i, i));
                diag += diag;
                if (diag <= A.viewRow(i).aggregate(
                                Functions.m_plus,
                                Functions.m_absm))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is an <i>identity</i> matrix if <tt>A[i,i] == 1</tt> and all other cells are zero.
         * Matrix may but need not be square.
         */

        public bool isIdentity(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    double v = A.getQuick(row, column);
                    if (row == column)
                    {
                        if (!(Math.Abs(1 - v) < epsilon))
                        {
                            return false;
                        }
                    }
                    else if (!(Math.Abs(v) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>lower bidiagonal</i> if <tt>A[i,j]==0</tt> unless <tt>i==j || i==j+1</tt>.
         * Matrix may but need not be square.
         */

        public bool isLowerBidiagonal(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(row == column || row == column + 1))
                    {
                        //if (A.getQuick(row,column) != 0) return false;
                        if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>lower triangular</i> if <tt>A[i,j]==0</tt> whenever <tt>i &lt; j</tt>.
         * Matrix may but need not be square.
         */

        public bool isLowerTriangular(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int column = columns; --column >= 0;)
            {
                for (int row = Math.Min(column, rows); --row >= 0;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>non-negative</i> if <tt>A[i,j] &gt;= 0</tt> holds for all cells.
         * <p>
         * Note: Ignores tolerance.
         */

        public bool isNonNegative(DoubleMatrix2D A)
        {
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(A.getQuick(row, column) >= 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A square matrix <tt>A</tt> is <i>orthogonal</i> if <tt>A*transpose(A) = I</tt>.
         * @throws ArgumentException if <tt>!isSquare(A)</tt>.
         */

        public bool isOrthogonal(DoubleMatrix2D A)
        {
            checkSquare(A);
            return Equals(A.zMult(A, null, 1, 0, false, true),
                          DoubleFactory2D.dense.identity(A.Rows()));
        }

        /**
         * A matrix <tt>A</tt> is <i>positive</i> if <tt>A[i,j] &gt; 0</tt> holds for all cells.
         * <p>
         * Note: Ignores tolerance.
         */

        public bool isPositive(DoubleMatrix2D A)
        {
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(A.getQuick(row, column) > 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>singular</i> if it has no inverse, that is, iff <tt>det(A)==0</tt>.
         */

        public bool isSingular(DoubleMatrix2D A)
        {
            return !(Math.Abs(Algebra.DEFAULT.det(A)) >= tolerance());
        }

        /**
         * A square matrix <tt>A</tt> is <i>skew-symmetric</i> if <tt>A = -transpose(A)</tt>, that is <tt>A[i,j] == -A[j,i]</tt>.
         * @throws ArgumentException if <tt>!isSquare(A)</tt>.
         */

        public bool isSkewSymmetric(DoubleMatrix2D A)
        {
            checkSquare(A);
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = rows; --column >= 0;)
                {
                    //if (A.getQuick(row,column) != -A.getQuick(column,row)) return false;
                    if (!(Math.Abs(A.getQuick(row, column) + A.getQuick(column, row)) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>square</i> if it has the same number of rows and columns.
         */

        public bool isSquare(DoubleMatrix2D A)
        {
            return A.Rows() == A.Columns();
        }

        /**
         * A matrix <tt>A</tt> is <i>strictly lower triangular</i> if <tt>A[i,j]==0</tt> whenever <tt>i &lt;= j</tt>.
         * Matrix may but need not be square.
         */

        public bool isStrictlyLowerTriangular(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int column = columns; --column >= 0;)
            {
                for (int row = Math.Min(rows, column + 1); --row >= 0;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>strictly triangular</i> if it is triangular and its diagonal elements all equal 0.
         * Matrix may but need not be square.
         */

        public bool isStrictlyTriangular(DoubleMatrix2D A)
        {
            if (!isTriangular(A))
            {
                return false;
            }

            double epsilon = tolerance();
            for (int i = Math.Min(A.Rows(), A.Columns()); --i >= 0;)
            {
                //if (A.getQuick(i,i) != 0) return false;
                if (!(Math.Abs(A.getQuick(i, i)) <= epsilon))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>strictly upper triangular</i> if <tt>A[i,j]==0</tt> whenever <tt>i &gt;= j</tt>.
         * Matrix may but need not be square.
         */

        public bool isStrictlyUpperTriangular(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int column = columns; --column >= 0;)
            {
                for (int row = rows; --row >= column;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>symmetric</i> if <tt>A = tranpose(A)</tt>, that is <tt>A[i,j] == A[j,i]</tt>.
         * @throws ArgumentException if <tt>!isSquare(A)</tt>.
         */

        public bool isSymmetric(DoubleMatrix2D A)
        {
            checkSquare(A);
            return Equals(A, A.viewDice());
        }

        /**
         * A matrix <tt>A</tt> is <i>triangular</i> iff it is either upper or lower triangular.
         * Matrix may but need not be square.
         */

        public bool isTriangular(DoubleMatrix2D A)
        {
            return isLowerTriangular(A) || isUpperTriangular(A);
        }

        /**
         * A matrix <tt>A</tt> is <i>tridiagonal</i> if <tt>A[i,j]==0</tt> whenever <tt>Math.Abs(i-j) > 1</tt>.
         * Matrix may but need not be square.
         */

        public bool isTridiagonal(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (Math.Abs(row - column) > 1)
                    {
                        //if (A.getQuick(row,column) != 0) return false;
                        if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>unit triangular</i> if it is triangular and its diagonal elements all equal 1.
         * Matrix may but need not be square.
         */

        public bool isUnitTriangular(DoubleMatrix2D A)
        {
            if (!isTriangular(A))
            {
                return false;
            }

            double epsilon = tolerance();
            for (int i = Math.Min(A.Rows(), A.Columns()); --i >= 0;)
            {
                //if (A.getQuick(i,i) != 1) return false;
                if (!(Math.Abs(1 - A.getQuick(i, i)) <= epsilon))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>upper bidiagonal</i> if <tt>A[i,j]==0</tt> unless <tt>i==j || i==j-1</tt>.
         * Matrix may but need not be square.
         */

        public bool isUpperBidiagonal(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(row == column || row == column - 1))
                    {
                        //if (A.getQuick(row,column) != 0) return false;
                        if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>upper triangular</i> if <tt>A[i,j]==0</tt> whenever <tt>i &gt; j</tt>.
         * Matrix may but need not be square.
         */

        public bool isUpperTriangular(DoubleMatrix2D A)
        {
            double epsilon = tolerance();
            int rows = A.Rows();
            int columns = A.Columns();
            for (int column = columns; --column >= 0;)
            {
                for (int row = rows; --row > column;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(Math.Abs(A.getQuick(row, column)) <= epsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * A matrix <tt>A</tt> is <i>zero</i> if all its cells are zero.
         */

        public bool isZero(DoubleMatrix2D A)
        {
            return Equals(A, 0);
        }

        /**
        The <i>lower bandwidth</i> of a square matrix <tt>A</tt> is the maximum <tt>i-j</tt> for which <tt>A[i,j]</tt> is nonzero and <tt>i &gt; j</tt>.
        A <i>banded</i> matrix has a "band" about the diagonal.
        Diagonal, tridiagonal and triangular matrices are special cases.

        @param A the square matrix to analyze.
        @return the lower bandwith.
        @throws ArgumentException if <tt>!isSquare(A)</tt>.
        @see #semiBandwidth(DoubleMatrix2D)
        @see #upperBandwidth(DoubleMatrix2D)
        */

        public int lowerBandwidth(DoubleMatrix2D A)
        {
            checkSquare(A);
            double epsilon = tolerance();
            int rows = A.Rows();

            for (int k = rows; --k >= 0;)
            {
                for (int i = rows - k; --i >= 0;)
                {
                    int j = i + k;
                    //if (A.getQuick(j,i) != 0) return k;
                    if (!(Math.Abs(A.getQuick(j, i)) <= epsilon))
                    {
                        return k;
                    }
                }
            }
            return 0;
        }

        /**
        Returns the <i>semi-bandwidth</i> of the given square matrix <tt>A</tt>.
        A <i>banded</i> matrix has a "band" about the diagonal.
        It is a matrix with all cells equal to zero, 
        with the possible exception of the cells along the diagonal line,
        the <tt>k</tt> diagonal lines above the diagonal, and the <tt>k</tt> diagonal lines below the diagonal.
        The <i>semi-bandwith l</i> is the number <tt>k+1</tt>.
        The <i>bandwidth p</i> is the number <tt>2*k + 1</tt>.
        For example, a tridiagonal matrix corresponds to <tt>k=1, l=2, p=3</tt>, 
        a diagonal or zero matrix corresponds to <tt>k=0, l=1, p=1</tt>, 
        <p>
        The <i>upper bandwidth</i> is the maximum <tt>j-i</tt> for which <tt>A[i,j]</tt> is nonzero and <tt>j &gt; i</tt>.
        The <i>lower bandwidth</i> is the maximum <tt>i-j</tt> for which <tt>A[i,j]</tt> is nonzero and <tt>i &gt; j</tt>. 
        Diagonal, tridiagonal and triangular matrices are special cases.
        <p>
        Examples:
        <table border="1" cellspacing="0">
          <tr align="left" valign="top"> 
            <td valign="middle" align="left"><tt>matrix</tt></td>
            <td> <tt>4&nbsp;x&nbsp;4&nbsp;<br>
              0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0 </tt></td>
            <td><tt>4&nbsp;x&nbsp;4<br>
              1&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;1 </tt></td>
            <td><tt>4&nbsp;x&nbsp;4<br>
              1&nbsp;1&nbsp;0&nbsp;0<br>
              1&nbsp;1&nbsp;1&nbsp;0<br>
              0&nbsp;1&nbsp;1&nbsp;1<br>
              0&nbsp;0&nbsp;1&nbsp;1 </tt></td>
            <td><tt> 4&nbsp;x&nbsp;4<br>
              0&nbsp;1&nbsp;1&nbsp;1<br>
              0&nbsp;1&nbsp;1&nbsp;1<br>
              0&nbsp;0&nbsp;0&nbsp;1<br>
              0&nbsp;0&nbsp;0&nbsp;1 </tt></td>
            <td><tt> 4&nbsp;x&nbsp;4<br>
              0&nbsp;0&nbsp;0&nbsp;0<br>
              1&nbsp;1&nbsp;0&nbsp;0<br>
              1&nbsp;1&nbsp;0&nbsp;0<br>
              1&nbsp;1&nbsp;1&nbsp;1 </tt></td>
            <td><tt>4&nbsp;x&nbsp;4<br>
              1&nbsp;1&nbsp;0&nbsp;0<br>
              0&nbsp;1&nbsp;1&nbsp;0<br>
              0&nbsp;1&nbsp;0&nbsp;1<br>
              1&nbsp;0&nbsp;1&nbsp;1 </tt><tt> </tt> </td>
            <td><tt>4&nbsp;x&nbsp;4<br>
              1&nbsp;1&nbsp;1&nbsp;0<br>
              0&nbsp;1&nbsp;0&nbsp;0<br>
              1&nbsp;1&nbsp;0&nbsp;1<br>
              0&nbsp;0&nbsp;1&nbsp;1 </tt> </td>
          </tr>
          <tr align="center" valign="middle"> 
            <td><tt>upperBandwidth</tt></td>
            <td> 
              <div align="center"><tt>0</tt></div>
            </td>
            <td> 
              <div align="center"><tt>0</tt></div>
            </td>
            <td> 
              <div align="center"><tt>1</tt></div>
            </td>
            <td><tt>3</tt></td>
            <td align="center" valign="middle"><tt>0</tt></td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>1</tt></div>
            </td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>2</tt></div>
            </td>
          </tr>
          <tr align="center" valign="middle"> 
            <td><tt>lowerBandwidth</tt></td>
            <td> 
              <div align="center"><tt>0</tt></div>
            </td>
            <td> 
              <div align="center"><tt>0</tt></div>
            </td>
            <td> 
              <div align="center"><tt>1</tt></div>
            </td>
            <td><tt>0</tt></td>
            <td align="center" valign="middle"><tt>3</tt></td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>3</tt></div>
            </td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>2</tt></div>
            </td>
          </tr>
          <tr align="center" valign="middle"> 
            <td><tt>semiBandwidth</tt></td>
            <td> 
              <div align="center"><tt>1</tt></div>
            </td>
            <td> 
              <div align="center"><tt>1</tt></div>
            </td>
            <td> 
              <div align="center"><tt>2</tt></div>
            </td>
            <td><tt>4</tt></td>
            <td align="center" valign="middle"><tt>4</tt></td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>4</tt></div>
            </td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>3</tt></div>
            </td>
          </tr>
          <tr align="center" valign="middle"> 
            <td><tt>description</tt></td>
            <td> 
              <div align="center"><tt>zero</tt></div>
            </td>
            <td> 
              <div align="center"><tt>diagonal</tt></div>
            </td>
            <td> 
              <div align="center"><tt>tridiagonal</tt></div>
            </td>
            <td><tt>upper triangular</tt></td>
            <td align="center" valign="middle"><tt>lower triangular</tt></td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>unstructured</tt></div>
            </td>
            <td align="center" valign="middle"> 
              <div align="center"><tt>unstructured</tt></div>
            </td>
          </tr>
        </table>

        @param A the square matrix to analyze.
        @return the semi-bandwith <tt>l</tt>.
        @throws ArgumentException if <tt>!isSquare(A)</tt>.
        @see #lowerBandwidth(DoubleMatrix2D)
        @see #upperBandwidth(DoubleMatrix2D)
        */

        public int semiBandwidth(DoubleMatrix2D A)
        {
            checkSquare(A);
            double epsilon = tolerance();
            int rows = A.Rows();

            for (int k = rows; --k >= 0;)
            {
                for (int i = rows - k; --i >= 0;)
                {
                    int j = i + k;
                    //if (A.getQuick(j,i) != 0) return k+1;
                    //if (A.getQuick(i,j) != 0) return k+1;
                    if (!(Math.Abs(A.getQuick(j, i)) <= epsilon))
                    {
                        return k + 1;
                    }
                    if (!(Math.Abs(A.getQuick(i, j)) <= epsilon))
                    {
                        return k + 1;
                    }
                }
            }
            return 1;
        }

        /**
         * Sets the tolerance to <tt>Math.Abs(newTolerance)</tt>.
         * @throws UnsupportedOperationException if <tt>this==DEFAULT || this==ZERO || this==TWELVE</tt>.
         */

        public void setTolerance(double newTolerance)
        {
            if (this == DEFAULT || this == ZERO || this == TWELVE)
            {
                throw new ArgumentException("Attempted to modify immutable object.");
                //throw new UnsupportedOperationException("Attempted to modify object."); // since JDK1.2
            }
            tolerance_ = Math.Abs(newTolerance);
        }

        /**
         * Returns the current tolerance.
         */

        public double tolerance()
        {
            return tolerance_;
        }

        /**
        Returns summary information about the given matrix <tt>A</tt>.
        That is a string with (propertyName, propertyValue) pairs.
        Useful for debugging or to quickly get the rough picture of a matrix.
        For example,
        <pre>
        density                      : 0.9
        isDiagonal                   : false
        isDiagonallyDominantByRow    : false
        isDiagonallyDominantByColumn : false
        isIdentity                   : false
        isLowerBidiagonal            : false
        isLowerTriangular            : false
        isNonNegative                : true
        isOrthogonal                 : Illegal operation or error: Matrix must be square.
        isPositive                   : true
        isSingular                   : Illegal operation or error: Matrix must be square.
        isSkewSymmetric              : Illegal operation or error: Matrix must be square.
        isSquare                     : false
        isStrictlyLowerTriangular    : false
        isStrictlyTriangular         : false
        isStrictlyUpperTriangular    : false
        isSymmetric                  : Illegal operation or error: Matrix must be square.
        isTriangular                 : false
        isTridiagonal                : false
        isUnitTriangular             : false
        isUpperBidiagonal            : false
        isUpperTriangular            : false
        isZero                       : false
        lowerBandwidth               : Illegal operation or error: Matrix must be square.
        semiBandwidth                : Illegal operation or error: Matrix must be square.
        upperBandwidth               : Illegal operation or error: Matrix must be square.
        </pre>
        */

        public string ToString(DoubleMatrix2D A)
        {
            ObjectArrayList names = new ObjectArrayList();
            ObjectArrayList values = new ObjectArrayList();
            string unknown = "Illegal operation or error: ";

            // determine properties
            names.Add("density");
            try
            {
                values.Add(density(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            // determine properties
            names.Add("isDiagonal");
            try
            {
                values.Add(isDiagonal(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            // determine properties
            names.Add("isDiagonallyDominantByRow");
            try
            {
                values.Add(isDiagonallyDominantByRow(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            // determine properties
            names.Add("isDiagonallyDominantByColumn");
            try
            {
                values.Add(isDiagonallyDominantByColumn(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isIdentity");
            try
            {
                values.Add(isIdentity(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isLowerBidiagonal");
            try
            {
                values.Add(isLowerBidiagonal(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isLowerTriangular");
            try
            {
                values.Add(isLowerTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isNonNegative");
            try
            {
                values.Add(isNonNegative(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isOrthogonal");
            try
            {
                values.Add(isOrthogonal(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isPositive");
            try
            {
                values.Add(isPositive(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isSingular");
            try
            {
                values.Add(isSingular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isSkewSymmetric");
            try
            {
                values.Add(isSkewSymmetric(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isSquare");
            try
            {
                values.Add(isSquare(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isStrictlyLowerTriangular");
            try
            {
                values.Add(isStrictlyLowerTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isStrictlyTriangular");
            try
            {
                values.Add(isStrictlyTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isStrictlyUpperTriangular");
            try
            {
                values.Add(isStrictlyUpperTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isSymmetric");
            try
            {
                values.Add(isSymmetric(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isTriangular");
            try
            {
                values.Add(isTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isTridiagonal");
            try
            {
                values.Add(isTridiagonal(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isUnitTriangular");
            try
            {
                values.Add(isUnitTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isUpperBidiagonal");
            try
            {
                values.Add(isUpperBidiagonal(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isUpperTriangular");
            try
            {
                values.Add(isUpperTriangular(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("isZero");
            try
            {
                values.Add(isZero(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("lowerBandwidth");
            try
            {
                values.Add(lowerBandwidth(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("semiBandwidth");
            try
            {
                values.Add(semiBandwidth(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("upperBandwidth");
            try
            {
                values.Add(upperBandwidth(A));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            // sort ascending by property name
            IntComparator comp = new IntComparator2_(
                names,
                this);

            Swapper swapper = new Swapper2_(names, this, values);
            GenericSorting.quickSort(0, names.Size(), comp, swapper);

            // determine padding for nice formatting
            int maxLength = 0;
            for (int i = 0; i < names.Size(); i++)
            {
                int Length = ((string) names.get(i)).Length;
                maxLength = Math.Max(Length, maxLength);
            }

            // finally, format properties
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < names.Size(); i++)
            {
                string name = ((string) names.get(i));
                buf.Append(name);
                buf.Append(blanks(maxLength - name.Length));
                buf.Append(" : ");
                buf.Append(values.get(i));
                if (i < names.Size() - 1)
                {
                    buf.Append('\n');
                }
            }

            return buf.ToString();
        }

        /**
        The <i>upper bandwidth</i> of a square matrix <tt>A</tt> is the 
        maximum <tt>j-i</tt> for which <tt>A[i,j]</tt> is nonzero and <tt>j &gt; i</tt>.
        A <i>banded</i> matrix has a "band" about the diagonal. 
        Diagonal, tridiagonal and triangular matrices are special cases.

        @param A the square matrix to analyze.
        @return the upper bandwith.
        @throws ArgumentException if <tt>!isSquare(A)</tt>.
        @see #semiBandwidth(DoubleMatrix2D)
        @see #lowerBandwidth(DoubleMatrix2D)
        */

        public int upperBandwidth(DoubleMatrix2D A)
        {
            checkSquare(A);
            double epsilon = tolerance();
            int rows = A.Rows();

            for (int k = rows; --k >= 0;)
            {
                for (int i = rows - k; --i >= 0;)
                {
                    int j = i + k;
                    //if (A.getQuick(i,j) != 0) return k;
                    if (!(Math.Abs(A.getQuick(i, j)) <= epsilon))
                    {
                        return k;
                    }
                }
            }
            return 0;
        }
    }
}
