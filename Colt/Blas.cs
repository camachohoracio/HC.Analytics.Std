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
    //package Appendlinalg;

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /**
    <p>
    Subset of the <A HREF="http://netlib2.cs.utk.edu/blas/faq.html">BLAS</A> (Basic Linear Algebra System); 
    High quality "building block" routines for performing basic vector and matrix operations. 
    Because the BLAS are efficient, portable, and widely available, they're commonly used in the development
    of high quality linear algebra software.
    <p>
    Mostly for compatibility with legacy notations. Most operations actually just delegate to the appropriate 
    methods directly defined on matrices and vectors. </p>
    <p>
    This class : the BLAS functions for operations on matrices from the 
      matrix package. It follows the spirit of the <A HREF="http://math.nist.gov/javanumerics/blas.html">Draft Proposal for Java BLAS Interface</A>, 
      by Roldan Pozo of the National Institute of Standards and Technology. Interface 
      definitions are also identical to the Ninja interface. Because the matrix //package 
      supports sections, the interface is actually simpler. </p>
    <p>Currently, the following operations are supported: </p>
    <ol>
      <li>BLAS Level 1: Vector-Vector operations </li>
      <ul>
        <li>ddot  : dot product of two vectors </li>
        <li>daxpy : scalar times a vector plus a vector </li>
        <li>drotg : construct a Givens plane rotation </li>
        <li>drot  : apply a plane rotation </li>
        <li>dcopy : copy vector X into vector Y </li>
        <li>dswap : interchange vectors X and Y </li>
        <li>dnrm2 : Euclidean norm of a vector </li>
        <li>dasum : sum of absolute values of vector components </li>
        <li>dscal : scale a vector by a scalar </li>
        <li>idamax: index of element with maximum absolute value </li>
      </ul>
      <li>2.BLAS Level 2: Matrix-Vector operations </li>
      <ul>
        <li>dgemv : matrix-vector multiply with general matrix </li>
        <li>dger  : rank-1 update on general matrix </li>
        <li>dsymv : matrix-vector multiply with symmetric matrix </li>
        <li>dtrmv : matrix-vector multiply with triangular matrix </li>
      </ul>
      <li>3.BLAS Level 3: Matrix-Matrix operations 
        <ul>
          <li>dgemm : matrix-matrix multiply with general matrices </li>
        </ul>
      </li>
    </ol>

    @author wolfgang.hoschek@cern.ch
    @version 0.9, 16/04/2000 
    */

    public interface Blas
    {
        /**
        Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col])</tt>.

        @param A the matrix to modify.
        @param function a function object taking as argument the current cell's value.
        @see Functions
        */
        void assign(DoubleMatrix2D A, DoubleFunction function);
        /**
        Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col],y[row,col])</tt>.

        @param x the matrix to modify.
        @param y the secondary matrix to operate on.
        @param function a function object taking as first argument the current cell's value of <tt>this</tt>,
        and as second argument the current cell's value of <tt>y</tt>,
        @return <tt>this</tt> (for convenience only).
        @throws	ArgumentException if <tt>x.Columns() != y.Columns() || x.Rows() != y.Rows()</tt>
        @see Functions
        */
        void assign(DoubleMatrix2D x, DoubleMatrix2D y, DoubleDoubleFunction function);
        /**
        Returns the sum of absolute values; <tt>|x[0]| + |x[1]| + ... </tt>.
        In fact equivalent to <tt>x.aggregate(Functions.plus, Functions.abs)</tt>.
        @param x the first vector.
        */
        double dasum(DoubleMatrix1D x);
        /**
        Combined vector scaling; <tt>y = y + alpha*x</tt>.
        In fact equivalent to <tt>y.assign(x,Functions.plusMult(alpha))</tt>.

        @param alpha a scale factor.
        @param x the first source vector.
        @param y the second source vector, this is also the vector where results are stored.

        @throws ArgumentException <tt>x.Size() != y.Size()</tt>..
        */
        void daxpy(double alpha, DoubleMatrix1D x, DoubleMatrix1D y);
        /**
        Combined matrix scaling; <tt>B = B + alpha*A</tt>.
        In fact equivalent to <tt>B.assign(A,Functions.plusMult(alpha))</tt>.

        @param alpha a scale factor.
        @param A the first source matrix.
        @param B the second source matrix, this is also the matrix where results are stored.

        @throws ArgumentException if <tt>A.Columns() != B.Columns() || A.Rows() != B.Rows()</tt>.
        */
        void daxpy(double alpha, DoubleMatrix2D A, DoubleMatrix2D B);
        /**
        Vector assignment (copying); <tt>y = x</tt>.
        In fact equivalent to <tt>y.assign(x)</tt>.

        @param x the source vector.
        @param y the destination vector.
 
        @throws ArgumentException <tt>x.Size() != y.Size()</tt>.
        */
        void dcopy(DoubleMatrix1D x, DoubleMatrix1D y);
        /**
        Matrix assignment (copying); <tt>B = A</tt>.
        In fact equivalent to <tt>B.assign(A)</tt>.

        @param A the source matrix.
        @param B the destination matrix.

        @throws ArgumentException if <tt>A.Columns() != B.Columns() || A.Rows() != B.Rows()</tt>.
        */
        void dcopy(DoubleMatrix2D A, DoubleMatrix2D B);
        /**
        Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>.
        In fact equivalent to <tt>x.zDotProduct(y)</tt>.
        @param x the first vector.
        @param y the second vector.
        @return the sum of products.

        @throws ArgumentException if <tt>x.Size() != y.Size()</tt>.
        */
        double ddot(DoubleMatrix1D x, DoubleMatrix1D y);
        /**
        Generalized linear algebraic matrix-matrix multiply; <tt>C = alpha*A*B + beta*C</tt>.
        In fact equivalent to <tt>A.zMult(B,C,alpha,beta,transposeA,transposeB)</tt>.
        Note: Matrix shape conformance is checked <i>after</i> potential transpositions.

        @param transposeA set this flag to indicate that the multiplication shall be performed on A'.
        @param transposeB set this flag to indicate that the multiplication shall be performed on B'.
        @param alpha a scale factor.
        @param A the first source matrix.
        @param B the second source matrix.
        @param beta a scale factor.
        @param C the third source matrix, this is also the matrix where results are stored.
 
        @throws ArgumentException if <tt>B.Rows() != A.Columns()</tt>.
        @throws ArgumentException if <tt>C.Rows() != A.Rows() || C.Columns() != B.Columns()</tt>.
        @throws ArgumentException if <tt>A == C || B == C</tt>.
        */

        void dgemm(bool transposeA, bool transposeB, double alpha, DoubleMatrix2D A, DoubleMatrix2D B, double beta,
                   DoubleMatrix2D C);

        /**
        Generalized linear algebraic matrix-vector multiply; <tt>y = alpha*A*x + beta*y</tt>.
        In fact equivalent to <tt>A.zMult(x,y,alpha,beta,transposeA)</tt>.
        Note: Matrix shape conformance is checked <i>after</i> potential transpositions.

        @param transposeA set this flag to indicate that the multiplication shall be performed on A'.
        @param alpha a scale factor.
        @param A the source matrix.
        @param x the first source vector.
        @param beta a scale factor.
        @param y the second source vector, this is also the vector where results are stored.

        @throws ArgumentException <tt>A.Columns() != x.Size() || A.Rows() != y.Size())</tt>..
        */
        void dgemv(bool transposeA, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta, DoubleMatrix1D y);
        /**
        Performs a rank 1 update; <tt>A = A + alpha*x*y'</tt>.
        Example:
        <pre>
        A = { {6,5}, {7,6} }, x = {1,2}, y = {3,4}, alpha = 1 -->
        A = { {9,9}, {13,14} }
        </pre>

        @param alpha a scalar.
        @param x an m element vector.
        @param y an n element vector.
        @param A an m by n matrix.
        */
        void dger(double alpha, DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix2D A);
        /**
        Return the 2-norm; <tt>sqrt(x[0]^2 + x[1]^2 + ...)</tt>.
        In fact equivalent to <tt>Math.Sqrt(Algebra.DEFAULT.norm2(x))</tt>.

        @param x the vector.
        */
        double dnrm2(DoubleMatrix1D x);
        /**
        Applies a givens plane rotation to (x,y); <tt>x = c*x + s*y; y = c*y - s*x</tt>.
        @param x the first vector.
        @param y the second vector.
        @param c the cosine of the angle of rotation.
        @param s the sine of the angle of rotation.
        */
        void drot(DoubleMatrix1D x, DoubleMatrix1D y, double c, double s);
        /**
        Constructs a Givens plane rotation for <tt>(a,b)</tt>.
        Taken from the LINPACK translation from FORTRAN to Java, interface slightly modified.
        In the LINPACK listing DROTG is attributed to Jack Dongarra

        @param  a  rotational elimination parameter a.
        @param  b  rotational elimination parameter b.
        @param  rotvec[]  Must be at least of Length 4. On output contains the values <tt>{a,b,c,s}</tt>.
        */
        void drotg(double a, double b, double[] rotvec);
        /**
        Vector scaling; <tt>x = alpha*x</tt>.
        In fact equivalent to <tt>x.assign(Functions.mult(alpha))</tt>.

        @param alpha a scale factor.
        @param x the first vector.
        */
        void dscal(double alpha, DoubleMatrix1D x);
        /**
        Matrix scaling; <tt>A = alpha*A</tt>.
        In fact equivalent to <tt>A.assign(Functions.mult(alpha))</tt>.

        @param alpha a scale factor.
        @param A the matrix.
        */
        void dscal(double alpha, DoubleMatrix2D A);
        /**
        Swaps the elements of two vectors; <tt>y <==> x</tt>.
        In fact equivalent to <tt>y.swap(x)</tt>.

        @param x the first vector.
        @param y the second vector.

        @throws ArgumentException <tt>x.Size() != y.Size()</tt>.
        */
        void dswap(DoubleMatrix1D x, DoubleMatrix1D y);
        /**
        Swaps the elements of two matrices; <tt>B <==> A</tt>.

        @param A the first matrix.
        @param B the second matrix.

        @throws ArgumentException if <tt>A.Columns() != B.Columns() || A.Rows() != B.Rows()</tt>.
        */
        void dswap(DoubleMatrix2D x, DoubleMatrix2D y);
        /**
        Symmetric matrix-vector multiplication; <tt>y = alpha*A*x + beta*y</tt>.
        Where alpha and beta are scalars, x and y are n element vectors and
        A is an n by n symmetric matrix.
        A can be in upper or lower triangular format.
        @param isUpperTriangular is A upper triangular or lower triangular part to be used?
        @param alpha scaling factor.
        @param A the source matrix.
        @param x the first source vector.
        @param beta scaling factor.
        @param y the second vector holding source and destination.
        */

        void dsymv(bool isUpperTriangular, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta,
                   DoubleMatrix1D y);

        /**
        Triangular matrix-vector multiplication; <tt>x = A*x</tt> or <tt>x = A'*x</tt>.
        Where x is an n element vector and A is an n by n unit, or non-unit,
        upper or lower triangular matrix.
        @param isUpperTriangular is A upper triangular or lower triangular?
        @param transposeA set this flag to indicate that the multiplication shall be performed on A'.
        @param isUnitTriangular true --> A is assumed to be unit triangular; false --> A is not assumed to be unit triangular
        @param A the source matrix.
        @param x the vector holding source and destination.
        */
        void dtrmv(bool isUpperTriangular, bool transposeA, bool isUnitTriangular, DoubleMatrix2D A, DoubleMatrix1D x);
        /**
        Returns the index of largest absolute value; <tt>i such that |x[i]| == max(|x[0]|,|x[1]|,...).</tt>.

        @param x the vector to search through.
        @return the index of largest absolute value (-1 if x is empty).
        */
        int idamax(DoubleMatrix1D x);
    }
}
