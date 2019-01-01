#region

using System;
using System.Text;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package Appendlinalg;

    //////import DoubleFactory2D;
    //////import DoubleMatrix1D;
    //////import DoubleMatrix2D;
    /**
     * Linear algebraic matrix operations operating on {@link DoubleMatrix2D}; concentrates most functionality of this package.
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     */

    [Serializable]
    public class Algebra : PersistentObject
    {
        /**
         * A default Algebra object; has {@link Property#DEFAULT} attached for tolerance. 
         * Allows ommiting to construct an Algebra object time and again.
         * 
         * Note that this Algebra object is immutable.
         * Any attempt to assign a new Property object to it (via method <tt>setProperty</tt>), or to alter the tolerance of its property object (via <tt>property().setTolerance(...)</tt>) will throw an exception.
         */
        public static Algebra DEFAULT;

        /**
         * A default Algebra object; has {@link Property#ZERO} attached for tolerance. 
         * Allows ommiting to construct an Algebra object time and again.
         * 
         * Note that this Algebra object is immutable.
         * Any attempt to assign a new Property object to it (via method <tt>setProperty</tt>), or to alter the tolerance of its property object (via <tt>property().setTolerance(...)</tt>) will throw an exception.
         */
        public static Algebra ZERO;

        /**
         * The property object attached to this instance.
         */
        public Property m_property;

        static Algebra()
        {
            // don't use new Algebra(Property.DEFAULT.tolerance()), because then property object would be mutable.
            DEFAULT = new Algebra();
            DEFAULT.m_property = Property.DEFAULT; // immutable property object

            ZERO = new Algebra();
            ZERO.m_property = Property.ZERO; // immutable property object
        }

        /**
         * Constructs a new instance with an equality tolerance given by <tt>Property.DEFAULT.tolerance()</tt>.
         */

        public Algebra()
            : this(Property.DEFAULT.tolerance())
        {
        }

        /**
         * Constructs a new instance with the given equality tolerance.
         * @param tolerance the tolerance to be used for equality operations.
         */

        public Algebra(double tolerance)
        {
            setProperty(new Property(tolerance));
        }

        /**
         * Constructs and returns the cholesky-decomposition of the given matrix.
         */

        private CholeskyDecomposition chol(DoubleMatrix2D matrix)
        {
            return new CholeskyDecomposition(
                new MatrixClass(
                    matrix.GetArr()));
        }

        /**
         * Returns a copy of the receiver.
         * The attached property object is also copied. Hence, the property object of the copy is mutable.
         *
         * @return a copy of the receiver.
         */

        public new Object Clone()
        {
            return new Algebra(m_property.tolerance());
        }

        /**
         * Returns the condition of matrix <tt>A</tt>, which is the ratio of largest to smallest singular value.
         */

        public double cond(DoubleMatrix2D A)
        {
            return svd(A).cond();
        }

        /**
         * Returns the determinant of matrix <tt>A</tt>.
         * @return the determinant.
         */

        public double det(DoubleMatrix2D A)
        {
            return lu(A).det();
        }

        /**
         * Constructs and returns the Eigenvalue-decomposition of the given matrix.
         */

        private EigenvalueDecomposition eig(DoubleMatrix2D matrix)
        {
            return new EigenvalueDecomposition(matrix);
        }

        /**
         * Returns sqrt(a^2 + b^2) without under/overflow.
         */

        public static double hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b/a;
                r = Math.Abs(a)*Math.Sqrt(1 + r*r);
            }
            else if (b != 0)
            {
                r = a/b;
                r = Math.Abs(b)*Math.Sqrt(1 + r*r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }

        /**
         * Returns sqrt(a^2 + b^2) without under/overflow.
         */

        public static DoubleDoubleFunction hypotFunction()
        {
            return new DoubleDoubleFunctionHypot();
        }

        /**
         * Returns the inverse or pseudo-inverse of matrix <tt>A</tt>.
         * @return a new independent matrix; inverse(matrix) if the matrix is square, pseudoinverse otherwise.
         */

        public DoubleMatrix2D inverse(DoubleMatrix2D A)
        {
            if (m_property.isSquare(A) && m_property.isDiagonal(A))
            {
                DoubleMatrix2D inv = A.Copy();
                bool isNonSingular = Diagonal.inverse(inv);
                if (!isNonSingular)
                {
                    throw new ArgumentException("A is singular.");
                }
                return inv;
            }
            return solve(A, DoubleFactory2D.dense.identity(A.Rows()));
        }

        /**
         * Constructs and returns the LU-decomposition of the given matrix.
         */

        private LUDecomposition lu(DoubleMatrix2D matrix)
        {
            return new LUDecomposition(
                new MatrixClass(
                    matrix.GetArr()));
        }

        /**
         * Inner product of two vectors; <tt>Sum(x[i] * y[i])</tt>.
         * Also known as dot product.
         * <br>
         * Equivalent to <tt>x.zDotProduct(y)</tt>.
         *
         * @param x the first source vector.
         * @param y the second source matrix.
         * @return the inner product.
         *
         * @throws ArgumentException if <tt>x.Size() != y.Size()</tt>.
         */

        public double mult(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            return x.zDotProduct(y);
        }

        /**
         * Linear algebraic matrix-vector multiplication; <tt>z = A * y</tt>.
         * <tt>z[i] = Sum(A[i,j] * y[j]), i=0..A.Rows()-1, j=0..y.Size()-1</tt>.
         * @param A the source matrix.
         * @param y the source vector.
         * @return <tt>z</tt>; a new vector with <tt>z.Size()==A.Rows()</tt>.
         * 
         * @throws ArgumentException if <tt>A.Columns() != y.Size()</tt>.
         */

        public DoubleMatrix1D mult(DoubleMatrix2D A, DoubleMatrix1D y)
        {
            return A.zMult(y, null);
        }

        /**
         * Linear algebraic matrix-matrix multiplication; <tt>C = A x B</tt>.
         * <tt>C[i,j] = Sum(A[i,k] * B[k,j]), k=0..n-1</tt>.
         * <br>
         * Matrix shapes: <tt>A(m x n), B(n x p), C(m x p)</tt>.
         *
         * @param A the first source matrix.
         * @param B the second source matrix.
         * @return <tt>C</tt>; a new matrix holding the results, with <tt>C.Rows()=A.Rows(), C.Columns()==B.Columns()</tt>.
         *
         * @throws ArgumentException if <tt>B.Rows() != A.Columns()</tt>.
         */

        public DoubleMatrix2D mult(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.zMult(B, null);
        }

        /**
         * Outer product of two vectors; Sets <tt>A[i,j] = x[i] * y[j]</tt>.
         *
         * @param x the first source vector.
         * @param y the second source vector.
         * @param A the matrix to hold the results. Set this parameter to <tt>null</tt> to indicate that a new result matrix shall be constructed.
         * @return A (for convenience only).
         * @throws ArgumentException	if <tt>A.Rows() != x.Size() || A.Columns() != y.Size()</tt>.
         */

        public DoubleMatrix2D multOuter(DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix2D A)
        {
            int rows = x.Size();
            int columns = y.Size();
            if (A == null)
            {
                A = x.like2D(rows, columns);
            }
            if (A.Rows() != rows || A.Columns() != columns)
            {
                throw new ArgumentException();
            }

            for (int row = rows; --row >= 0;)
            {
                A.viewRow(row).assign(y);
            }

            for (int column = columns; --column >= 0;)
            {
                A.viewColumn(column).assign(x,
                                            Functions.m_mult);
            }
            return A;
        }

        /**
         * Returns the one-norm of vector <tt>x</tt>, which is <tt>Sum(abs(x[i]))</tt>.
         */

        public double norm1(DoubleMatrix1D x)
        {
            if (x.Size() == 0)
            {
                return 0;
            }
            return x.aggregate(Functions.m_plus, Functions.m_absm);
        }

        /**
         * Returns the one-norm of matrix <tt>A</tt>, which is the maximum absolute column sum.
         */

        public double norm1(DoubleMatrix2D A)
        {
            double max = 0;
            for (int column = A.Columns(); --column >= 0;)
            {
                max = Math.Max(max, norm1(A.viewColumn(column)));
            }
            return max;
        }

        /**
         * Returns the two-norm (aka <i>euclidean norm</i>) of vector <tt>x</tt>; equivalent to <tt>mult(x,x)</tt>.
         */

        public double norm2(DoubleMatrix1D x)
        {
            return mult(x, x);
        }

        /**
         * Returns the two-norm of matrix <tt>A</tt>, which is the maximum singular value; obtained from SVD.
         */

        public double norm2(DoubleMatrix2D A)
        {
            return svd(A).norm2();
        }

        /**
         * Returns the Frobenius norm of matrix <tt>A</tt>, which is <tt>Sqrt(Sum(A[i,j]<sup>2</sup>))</tt>.
         */

        public double normF(DoubleMatrix2D A)
        {
            if (A.Size() == 0)
            {
                return 0;
            }
            return A.aggregate(hypotFunction(), Functions.m_identity);
        }

        /**
         * Returns the infinity norm of vector <tt>x</tt>, which is <tt>Max(abs(x[i]))</tt>.
         */

        public double normInfinity(DoubleMatrix1D x)
        {
            // fix for bug reported by T.J.Hunt@open.ac.uk
            if (x.Size() == 0)
            {
                return 0;
            }

            return x.aggregate(Functions.m_max, Functions.m_absm);

            //	if (x.Size()==0) return 0;
            //	return x.aggregate(Functions.plus,Functions.abs);
            //	double Max = 0;
            //	for (int i = x.Size(); --i >= 0; ) {
            //		Max = Math.Max(Max, x.getQuick(i));
            //	}
            //	return Max;
        }

        /**
         * Returns the infinity norm of matrix <tt>A</tt>, which is the maximum absolute row sum.
         */

        public double normInfinity(DoubleMatrix2D A)
        {
            double max = 0;
            for (int row = A.Rows(); --row >= 0;)
            {
                //Max = Math.Max(Max, normInfinity(A.viewRow(row)));
                max = Math.Max(max, norm1(A.viewRow(row)));
            }
            return max;
        }

        /**
        Modifies the given vector <tt>A</tt> such that it is permuted as specified; Useful for pivoting.
        Cell <tt>A[i]</tt> will go into cell <tt>A[indexes[i]]</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Reordering
        [A,B,C,D,E] with indexes [0,4,2,3,1] yields 
        [A,E,C,D,B]
        In other words A[0]<--A[0], A[1]<--A[4], A[2]<--A[2], A[3]<--A[3], A[4]<--A[1].

        Reordering
        [A,B,C,D,E] with indexes [0,4,1,2,3] yields 
        [A,E,B,C,D]
        In other words A[0]<--A[0], A[1]<--A[4], A[2]<--A[1], A[3]<--A[2], A[4]<--A[3].
        </pre>

        @param   A   the vector to permute.
        @param   indexes the permutation indexes, must satisfy <tt>indexes.Length==A.Size() && indexes[i] >= 0 && indexes[i] < A.Size()</tt>;
        @param   work the working storage, must satisfy <tt>work.Length >= A.Size()</tt>; set <tt>work==null</tt> if you don't care about performance.
        @return the modified <tt>A</tt> (for convenience only).
        @throws	HCException if <tt>indexes.Length != A.Size()</tt>.
        */

        public DoubleMatrix1D permute(DoubleMatrix1D A, int[] indexes, double[] work)
        {
            // check validity
            int size = A.Size();
            if (indexes.Length != size)
            {
                throw new HCException("invalid permutation");
            }

            /*
            int i=size;
            int a;
            while (--i >= 0 && (a=indexes[i])==i) if (a < 0 || a >= size) throw new HCException("invalid permutation");
            if (i<0) return; // nothing to permute
            */

            if (work == null || size > work.Length)
            {
                work = A.ToArray();
            }
            else
            {
                A.ToArray(work);
            }
            for (int i = size; --i >= 0;)
            {
                A.setQuick(i, work[indexes[i]]);
            }
            return A;
        }

        /**
        Constructs and returns a new row and column permuted <i>selection view</i> of matrix <tt>A</tt>; equivalent to {@link DoubleMatrix2D#viewSelection(int[],int[])}.
        The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa.
        Use idioms like <tt>result = permute(...).Copy()</tt> to generate an independent sub matrix.
        @return the new permuted selection view.
        */

        public DoubleMatrix2D permute(DoubleMatrix2D A, int[] rowIndexes, int[] columnIndexes)
        {
            return A.viewSelection(rowIndexes, columnIndexes);
        }

        /**
        Modifies the given matrix <tt>A</tt> such that it's columns are permuted as specified; Useful for pivoting.
        Column <tt>A[i]</tt> will go into column <tt>A[indexes[i]]</tt>.
        Equivalent to <tt>permuteRows(transpose(A), indexes, work)</tt>.
        @param   A   the matrix to permute.
        @param   indexes the permutation indexes, must satisfy <tt>indexes.Length==A.Columns() && indexes[i] >= 0 && indexes[i] < A.Columns()</tt>;
        @param   work the working storage, must satisfy <tt>work.Length >= A.Columns()</tt>; set <tt>work==null</tt> if you don't care about performance.
        @return the modified <tt>A</tt> (for convenience only).
        @throws	HCException if <tt>indexes.Length != A.Columns()</tt>.
        */

        public DoubleMatrix2D permuteColumns(DoubleMatrix2D A, int[] indexes, int[] work)
        {
            return permuteRows(A.viewDice(), indexes, work);
        }

        /**
        Modifies the given matrix <tt>A</tt> such that it's rows are permuted as specified; Useful for pivoting.
        Row <tt>A[i]</tt> will go into row <tt>A[indexes[i]]</tt>.
        <p>
        <b>Example:</b>
        <pre>
        Reordering
        [A,B,C,D,E] with indexes [0,4,2,3,1] yields 
        [A,E,C,D,B]
        In other words A[0]<--A[0], A[1]<--A[4], A[2]<--A[2], A[3]<--A[3], A[4]<--A[1].

        Reordering
        [A,B,C,D,E] with indexes [0,4,1,2,3] yields 
        [A,E,B,C,D]
        In other words A[0]<--A[0], A[1]<--A[4], A[2]<--A[1], A[3]<--A[2], A[4]<--A[3].
        </pre>

        @param   A   the matrix to permute.
        @param   indexes the permutation indexes, must satisfy <tt>indexes.Length==A.Rows() && indexes[i] >= 0 && indexes[i] < A.Rows()</tt>;
        @param   work the working storage, must satisfy <tt>work.Length >= A.Rows()</tt>; set <tt>work==null</tt> if you don't care about performance.
        @return the modified <tt>A</tt> (for convenience only).
        @throws	HCException if <tt>indexes.Length != A.Rows()</tt>.
        */

        public DoubleMatrix2D permuteRows(DoubleMatrix2D A, int[] indexes, int[] work)
        {
            // check validity
            int size = A.Rows();
            if (indexes.Length != size)
            {
                throw new HCException("invalid permutation");
            }

            /*
            int i=size;
            int a;
            while (--i >= 0 && (a=indexes[i])==i) if (a < 0 || a >= size) throw new HCException("invalid permutation");
            if (i<0) return; // nothing to permute
            */

            int columns = A.Columns();
            if (columns < size/10)
            {
                // quicker
                double[] doubleWork = new double[size];
                for (int j = A.Columns(); --j >= 0;)
                {
                    permute(A.viewColumn(j), indexes, doubleWork);
                }
                return A;
            }

            Swapper swapper = new Swapper3_(A);

            GenericPermuting.permute(indexes, swapper, work, null);

            return A;
        }

        /**
         * Linear algebraic matrix power; <tt>B = A<sup>k</sup> <==> B = A*A*...*A</tt>.
         * <ul>
         * <li><tt>p &gt;= 1: B = A*A*...*A</tt>.</li>
         * <li><tt>p == 0: B = identity matrix</tt>.</li>
         * <li><tt>p &lt;  0: B = pow(inverse(A),-p)</tt>.</li>
         * </ul>
         * Implementation: Based on logarithms of 2, memory usage minimized.
         * @param A the source matrix; must be square; stays unaffected by this operation.
         * @param p the exponent, can be any number.
         * @return <tt>B</tt>, a newly constructed result matrix; storage-independent of <tt>A</tt>.
         * 
         * @throws ArgumentException if <tt>!property().isSquare(A)</tt>.
         */

        public DoubleMatrix2D pow(DoubleMatrix2D A, int p)
        {
            // matrix multiplication based on log2 method: A*A*....*A is slow, ((A * A)^2)^2 * ... is faster
            // allocates two auxiliary matrices as work space

            //Blas blas = SmpBlas.smpBlas; // for parallel matrix mult; if not initialized defaults to sequential blas
            Property.DEFAULT.checkSquare(A);
            if (p < 0)
            {
                A = inverse(A);
                p = -p;
            }
            if (p == 0)
            {
                return DoubleFactory2D.dense.identity(A.Rows());
            }
            DoubleMatrix2D T = A.like(); // temporary
            if (p == 1)
            {
                return T.assign(A); // safes one auxiliary matrix allocation
            }
            if (p == 2)
            {
                //blas.dgemm(false, false, 1, A, A, 0, T); // mult(A,A); // safes one auxiliary matrix allocation
                return T;
            }

            int k =
                QuickBitVector.mostSignificantBit(p); // index of highest bit in state "true"

            /*
            this is the naive version:
            DoubleMatrix2D B = A.Copy();
            for (int i=0; i<p-1; i++) {
                B = mult(B,A);
            }
            return B;
            */

            // here comes the optimized version:
            //Timer timer = new Timer().start();

            int i = 0;
            while (i <= k && (p & (1 << i)) == 0)
            {
                // while (bit i of p == false)
                // A = mult(A,A); would allocate a lot of temporary memory
                //blas.dgemm(false, false, 1, A, A, 0, T); // A.zMult(A,T);
                DoubleMatrix2D swap = A;
                A = T;
                T = swap; // swap A with T
                i++;
            }

            DoubleMatrix2D B = A.Copy();
            i++;
            for (; i <= k; i++)
            {
                // A = mult(A,A); would allocate a lot of temporary memory
                //blas.dgemm(false, false, 1, A, A, 0, T); // A.zMult(A,T);	
                DoubleMatrix2D swap = A;
                A = T;
                T = swap; // swap A with T

                if ((p & (1 << i)) != 0)
                {
                    // if (bit i of p == true)
                    // B = mult(B,A); would allocate a lot of temporary memory
                    //blas.dgemm(false, false, 1, B, A, 0, T); // B.zMult(A,T);		
                    swap = B;
                    B = T;
                    T = swap; // swap B with T
                }
            }
            //timer.stop().display();
            return B;
        }

        /**
         * Returns the property object attached to this Algebra, defining tolerance.
         * @return the Property object.
         * @see #setProperty(Property)
         */

        public Property property()
        {
            return m_property;
        }

        /**
         * Constructs and returns the QR-decomposition of the given matrix.
         */

        private QRDecomposition qr(DoubleMatrix2D matrix)
        {
            return new QRDecomposition(matrix);
        }

        /**
         * Returns the effective numerical rank of matrix <tt>A</tt>, obtained from Singular Value Decomposition.
         */

        public int rank(DoubleMatrix2D A)
        {
            return svd(A).rank();
        }

        /**
         * Attaches the given property object to this Algebra, defining tolerance.
         * @param the Property object to be attached.
         * @throws	UnsupportedOperationException if <tt>this==DEFAULT && property!=property()</tt> - The DEFAULT Algebra object is immutable.
         * @throws	UnsupportedOperationException if <tt>this==ZERO && property!=property()</tt> - The ZERO Algebra object is immutable.
         * @see #property
         */

        public void setProperty(Property property)
        {
            if (this == DEFAULT && m_property != property)
            {
                throw new ArgumentException("Attempted to modify immutable object.");
            }
            if (this == ZERO && m_property != property)
            {
                throw new ArgumentException("Attempted to modify immutable object.");
            }
            m_property = property;
        }

        /**
         * Solves A*X = B.
         * @return X; a new independent matrix; solution if A is square, least squares solution otherwise.
         */

        public DoubleMatrix2D solve(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return (A.Rows() == A.Columns()
                        ?
                            (new DenseDoubleMatrix2D(lu(A).solve(
                                                         new MatrixClass(
                                                             B.GetArr())).GetArr()))
                        :
                            (qr(A).solve(B)));
        }

        /**
         * Solves X*A = B, which is also A'*X' = B'.
         * @return X; a new independent matrix; solution if A is square, least squares solution otherwise.
         */

        public DoubleMatrix2D solveTranspose(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return solve(transpose(A), transpose(B));
        }

        /**
         * Copies the columns of the indicated rows into a new sub matrix.
         * <tt>sub[0..rowIndexes.Length-1,0..columnTo-columnFrom] = A[rowIndexes(:),columnFrom..columnTo]</tt>;
         * The returned matrix is <i>not backed</i> by this matrix, so changes in the returned matrix are <i>not reflected</i> in this matrix, and vice-versa.
         *
         * @param   A   the source matrix to copy from.
         * @param   rowIndexes the indexes of the rows to copy. May be unsorted.
         * @param   columnFrom the index of the first column to copy (inclusive).
         * @param   columnTo the index of the last column to copy (inclusive).
         * @return  a new sub matrix; with <tt>sub.Rows()==rowIndexes.Length; sub.Columns()==columnTo-columnFrom+1</tt>.
         * @throws	HCException if <tt>columnFrom<0 || columnTo-columnFrom+1<0 || columnTo+1>matrix.Columns() || for any row=rowIndexes[i]: row < 0 || row >= matrix.Rows()</tt>.
         */

        private DoubleMatrix2D subMatrix(DoubleMatrix2D A, int[] rowIndexes, int columnFrom, int columnTo)
        {
            int width = columnTo - columnFrom + 1;
            int rows = A.Rows();
            A = A.viewPart(0, columnFrom, rows, width);
            DoubleMatrix2D sub = A.like(rowIndexes.Length, width);

            for (int r = rowIndexes.Length; --r >= 0;)
            {
                int row = rowIndexes[r];
                if (row < 0 || row >= rows)
                {
                    throw new HCException("Illegal Index");
                }
                sub.viewRow(r).assign(A.viewRow(row));
            }
            return sub;
        }

        /**
         * Copies the rows of the indicated columns into a new sub matrix.
         * <tt>sub[0..rowTo-rowFrom,0..columnIndexes.Length-1] = A[rowFrom..rowTo,columnIndexes(:)]</tt>;
         * The returned matrix is <i>not backed</i> by this matrix, so changes in the returned matrix are <i>not reflected</i> in this matrix, and vice-versa.
         *
         * @param   A   the source matrix to copy from.
         * @param   rowFrom the index of the first row to copy (inclusive).
         * @param   rowTo the index of the last row to copy (inclusive).
         * @param   columnIndexes the indexes of the columns to copy. May be unsorted.
         * @return  a new sub matrix; with <tt>sub.Rows()==rowTo-rowFrom+1; sub.Columns()==columnIndexes.Length</tt>.
         * @throws	HCException if <tt>rowFrom<0 || rowTo-rowFrom+1<0 || rowTo+1>matrix.Rows() || for any col=columnIndexes[i]: col < 0 || col >= matrix.Columns()</tt>.
         */

        private DoubleMatrix2D subMatrix(DoubleMatrix2D A, int rowFrom, int rowTo, int[] columnIndexes)
        {
            if (rowTo - rowFrom >= A.Rows())
            {
                throw new HCException("Too many rows");
            }
            int height = rowTo - rowFrom + 1;
            int columns = A.Columns();
            A = A.viewPart(rowFrom, 0, height, columns);
            DoubleMatrix2D sub = A.like(height, columnIndexes.Length);

            for (int c = columnIndexes.Length; --c >= 0;)
            {
                int column = columnIndexes[c];
                if (column < 0 || column >= columns)
                {
                    throw new HCException("Illegal Index");
                }
                sub.viewColumn(c).assign(A.viewColumn(column));
            }
            return sub;
        }

        /**
        Constructs and returns a new <i>sub-range view</i> which is the sub matrix <tt>A[fromRow..toRow,fromColumn..toColumn]</tt>.
        The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa.
        Use idioms like <tt>result = subMatrix(...).Copy()</tt> to generate an independent sub matrix.

        @param A the source matrix.
        @param fromRow   The index of the first row (inclusive).
        @param toRow   The index of the last row (inclusive).
        @param fromColumn   The index of the first column (inclusive).
        @param toColumn   The index of the last column (inclusive).
        @return a new sub-range view.
        @throws	HCException if <tt>fromColumn<0 || toColumn-fromColumn+1<0 || toColumn>=A.Columns() || fromRow<0 || toRow-fromRow+1<0 || toRow>=A.Rows()</tt>
        */

        public DoubleMatrix2D subMatrix(DoubleMatrix2D A, int fromRow, int toRow, int fromColumn, int toColumn)
        {
            return A.viewPart(fromRow, fromColumn, toRow - fromRow + 1, toColumn - fromColumn + 1);
        }

        /**
         * Constructs and returns the SingularValue-decomposition of the given matrix.
         */

        private SingularValueDecomposition svd(DoubleMatrix2D matrix)
        {
            return new SingularValueDecomposition(matrix);
        }

        /**
        Returns a string with (propertyName, propertyValue) pairs.
        Useful for debugging or to quickly get the rough picture.
        For example,
        <pre>
        cond          : 14.073264490042144
        det           : Illegal operation or error: Matrix must be square.
        norm1         : 0.9620244354009628
        norm2         : 3.0
        normF         : 1.304841791648992
        normInfinity  : 1.5406551198102534
        rank          : 3
        trace         : 0
        </pre>
        */

        public string ToString(DoubleMatrix2D matrix)
        {
            ObjectArrayList names = new ObjectArrayList();
            ObjectArrayList values = new ObjectArrayList();
            string unknown = "Illegal operation or error: ";

            // determine properties
            names.Add("cond");
            try
            {
                values.Add((cond(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("det");
            try
            {
                values.Add((det(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("norm1");
            try
            {
                values.Add((norm1(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("norm2");
            try
            {
                values.Add((norm2(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("normF");
            try
            {
                values.Add((normF(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("normInfinity");
            try
            {
                values.Add((normInfinity(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("rank");
            try
            {
                values.Add((rank(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }

            names.Add("trace");
            try
            {
                values.Add((trace(matrix)));
            }
            catch (ArgumentException exc)
            {
                values.Add(unknown + exc.Message);
            }


            // sort ascending by property name
            IntComparator comp = new IntComparator3_(names);

            Swapper swapper = new Swapper4_(names, values);

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
                buf.Append(Property.blanks(maxLength - name.Length));
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
        Returns the results of <tt>ToString(A)</tt> and additionally the results of all sorts of decompositions applied to the given matrix.
        Useful for debugging or to quickly get the rough picture.
        For example,
        <pre>
        A = 3 x 3 matrix
        249  66  68
        104 214 108
        144 146 293

        cond         : 3.931600417472078
        det          : 9638870.0
        norm1        : 497.0
        norm2        : 473.34508217011404
        normF        : 516.873292016525
        normInfinity : 583.0
        rank         : 3
        trace        : 756.0

        density                      : 1.0
        isDiagonal                   : false
        isDiagonallyDominantByColumn : true
        isDiagonallyDominantByRow    : true
        isIdentity                   : false
        isLowerBidiagonal            : false
        isLowerTriangular            : false
        isNonNegative                : true
        isOrthogonal                 : false
        isPositive                   : true
        isSingular                   : false
        isSkewSymmetric              : false
        isSquare                     : true
        isStrictlyLowerTriangular    : false
        isStrictlyTriangular         : false
        isStrictlyUpperTriangular    : false
        isSymmetric                  : false
        isTriangular                 : false
        isTridiagonal                : false
        isUnitTriangular             : false
        isUpperBidiagonal            : false
        isUpperTriangular            : false
        isZero                       : false
        lowerBandwidth               : 2
        semiBandwidth                : 3
        upperBandwidth               : 2

        -----------------------------------------------------------------------------
        LUDecompositionQuick(A) --> isNonSingular(A), det(A), pivot, L, U, inverse(A)
        -----------------------------------------------------------------------------
        isNonSingular = true
        det = 9638870.0
        pivot = [0, 1, 2]

        L = 3 x 3 matrix
        1        0       0
        0.417671 1       0
        0.578313 0.57839 1

        U = 3 x 3 matrix
        249  66         68       
          0 186.433735  79.598394
          0   0        207.635819

        inverse(A) = 3 x 3 matrix
         0.004869 -0.000976 -0.00077 
        -0.001548  0.006553 -0.002056
        -0.001622 -0.002786  0.004816

        -----------------------------------------------------------------
        QRDecomposition(A) --> hasFullRank(A), H, Q, R, pseudo inverse(A)
        -----------------------------------------------------------------
        hasFullRank = true

        H = 3 x 3 matrix
        1.814086 0        0
        0.34002  1.903675 0
        0.470797 0.428218 2

        Q = 3 x 3 matrix
        -0.814086  0.508871  0.279845
        -0.34002  -0.808296  0.48067 
        -0.470797 -0.296154 -0.831049

        R = 3 x 3 matrix
        -305.864349 -195.230337 -230.023539
           0        -182.628353  467.703164
           0           0        -309.13388 

        pseudo inverse(A) = 3 x 3 matrix
         0.006601  0.001998 -0.005912
        -0.005105  0.000444  0.008506
        -0.000905 -0.001555  0.002688

        --------------------------------------------------------------------------
        CholeskyDecomposition(A) --> isSymmetricPositiveDefinite(A), L, inverse(A)
        --------------------------------------------------------------------------
        isSymmetricPositiveDefinite = false

        L = 3 x 3 matrix
        15.779734  0         0       
         6.590732 13.059948  0       
         9.125629  6.573948 12.903724

        inverse(A) = Illegal operation or error: Matrix is not symmetric positive definite.

        ---------------------------------------------------------------------
        EigenvalueDecomposition(A) --> D, V, realEigenvalues, imagEigenvalues
        ---------------------------------------------------------------------
        realEigenvalues = 1 x 3 matrix
        462.796507 172.382058 120.821435
        imagEigenvalues = 1 x 3 matrix
        0 0 0

        D = 3 x 3 matrix
        462.796507   0          0       
          0        172.382058   0       
          0          0        120.821435

        V = 3 x 3 matrix
        -0.398877 -0.778282  0.094294
        -0.500327  0.217793 -0.806319
        -0.768485  0.66553   0.604862

        ---------------------------------------------------------------------
        SingularValueDecomposition(A) --> cond(A), rank(A), norm2(A), U, S, V
        ---------------------------------------------------------------------
        cond = 3.931600417472078
        rank = 3
        norm2 = 473.34508217011404

        U = 3 x 3 matrix
        0.46657  -0.877519  0.110777
        0.50486   0.161382 -0.847982
        0.726243  0.45157   0.51832 

        S = 3 x 3 matrix
        473.345082   0          0       
          0        169.137441   0       
          0          0        120.395013

        V = 3 x 3 matrix
        0.577296 -0.808174  0.116546
        0.517308  0.251562 -0.817991
        0.631761  0.532513  0.563301
        </pre>
        */

        public string toVerboseString(DoubleMatrix2D matrix)
        {
            /*
                StringBuilder buf = new StringBuilder();
                string unknown = "Illegal operation or error: ";
                string constructionException = "Illegal operation or error upon construction: ";

                buf.Append("------------------------------------------------------------------\n");
                buf.Append("LUDecomposition(A) --> isNonSingular, det, pivot, L, U, inverse(A)\n");
                buf.Append("------------------------------------------------------------------\n");
            */

            string constructionException = "Illegal operation or error upon construction of ";
            StringBuilder buf = new StringBuilder();

            buf.Append("A = ");
            buf.Append(matrix);

            buf.Append("\n\n" + ToString(matrix));
            buf.Append("\n\n" + Property.DEFAULT.ToString(matrix));

            LUDecomposition lu = null;
            try
            {
                lu = new LUDecomposition(new MatrixClass(
                                             matrix.GetArr()));
            }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " LUDecomposition: " + exc.Message);
            }
            if (lu != null)
            {
                buf.Append("\n\n" + lu);
            }

            QRDecomposition qr = null;
            try
            {
                qr = new QRDecomposition(matrix);
            }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " QRDecomposition: " + exc.Message);
            }
            if (qr != null)
            {
                buf.Append("\n\n" + qr);
            }

            CholeskyDecomposition chol = null;
            try
            {
                chol = new CholeskyDecomposition(
                    new MatrixClass(
                        matrix.GetArr()));
            }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " CholeskyDecomposition: " + exc.Message);
            }
            if (chol != null)
            {
                buf.Append("\n\n" + chol);
            }

            EigenvalueDecomposition eig = null;
            try
            {
                eig = new EigenvalueDecomposition(matrix);
            }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " EigenvalueDecomposition: " + exc.Message);
            }
            if (eig != null)
            {
                buf.Append("\n\n" + eig);
            }

            SingularValueDecomposition svd = null;
            try
            {
                svd = new SingularValueDecomposition(matrix);
            }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " SingularValueDecomposition: " + exc.Message);
            }
            if (svd != null)
            {
                buf.Append("\n\n" + svd);
            }

            return buf.ToString();
        }

        /**
         * Returns the sum of the diagonal elements of matrix <tt>A</tt>; <tt>Sum(A[i,i])</tt>.
         */

        public double trace(DoubleMatrix2D A)
        {
            double sum = 0;
            for (int i = Math.Min(A.Rows(), A.Columns()); --i >= 0;)
            {
                sum += A.getQuick(i, i);
            }
            return sum;
        }

        /**
        Constructs and returns a new view which is the transposition of the given matrix <tt>A</tt>.
        Equivalent to {@link DoubleMatrix2D#viewDice A.viewDice()}.
        This is a zero-copy transposition, taking O(1), i.e. constant time.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa. 
        Use idioms like <tt>result = transpose(A).Copy()</tt> to generate an independent matrix.
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
        @return a new transposed view. 
        */

        public DoubleMatrix2D transpose(DoubleMatrix2D A)
        {
            return A.viewDice();
        }

        /**
        Modifies the matrix to be a lower trapezoidal matrix.
        @return <tt>A</tt> (for convenience only).
        @see #triangulateLower(DoubleMatrix2D)
        */

        public DoubleMatrix2D trapezoidalLower(DoubleMatrix2D A)
        {
            int rows = A.Rows();
            int columns = A.Columns();
            for (int r = rows; --r >= 0;)
            {
                for (int c = columns; --c >= 0;)
                {
                    if (r < c)
                    {
                        A.setQuick(r, c, 0);
                    }
                }
            }
            return A;
        }

        /**
         * Outer product of two vectors; Returns a matrix with <tt>A[i,j] = x[i] * y[j]</tt>.
         *
         * @param x the first source vector.
         * @param y the second source vector.
         * @return the outer product </tt>A</tt>.
         */

        private DoubleMatrix2D xmultOuter(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            DoubleMatrix2D A = x.like2D(x.Size(), y.Size());
            multOuter(x, y, A);
            return A;
        }

        /**
         * Linear algebraic matrix power; <tt>B = A<sup>k</sup> <==> B = A*A*...*A</tt>.
         * @param A the source matrix; must be square.
         * @param k the exponent, can be any number.
         * @return a new result matrix.
         * 
         * @throws ArgumentException if <tt>!Testing.isSquare(A)</tt>.
         */

        private DoubleMatrix2D xpowSlow(DoubleMatrix2D A, int k)
        {
            //Timer timer = new Timer().start();
            DoubleMatrix2D result = A.Copy();
            for (int i = 0; i < k - 1; i++)
            {
                result = mult(result, A);
            }
            //timer.stop().display();
            return result;
        }
    }
}
