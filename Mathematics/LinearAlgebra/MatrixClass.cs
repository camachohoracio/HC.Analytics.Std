#region

using System;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Helpers;

//using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.LinearAlgebra
{
    /**
       Jama = Java Matrix class.
    <P>
       The Java Matrix Class provides the fundamental operations of numerical
       linear algebra.  Various constructors create Matrices from two dimensional
       arrays of double precision floating point numbers.  Various "gets" and
       "sets" provide access to submatrices and matrix elements.  Several methods 
       implement basic matrix arithmetic, including matrix addition and
       multiplication, matrix norms, and element-by-element array operations.
       Methods for reading and printing matrices are also included.  All the
       operations in this version of the Matrix Class involve real matrices.
       Complex matrices may be handled in a future version.
    <P>
       Five fundamental matrix decompositions, which consist of pairs or triples
       of matrices, permutation vectors, and the like, produce results in five
       decomposition classes.  These decompositions are accessed by the Matrix
       class to compute solutions of simultaneous linear equations, determinants,
       inverses and other matrix functions.  The five decompositions are:
    <P><UL>
       <LI>Cholesky Decomposition of symmetric, positive definite matrices.
       <LI>LU Decomposition of rectangular matrices.
       <LI>QR Decomposition of rectangular matrices.
       <LI>Singular Value Decomposition of rectangular matrices.
       <LI>Eigenvalue Decomposition of both symmetric and nonsymmetric square matrices.
    </UL>
    <DL>
    <DT><B>Example of use:</B></DT>
    <P>
    <DD>Solve a linear system A x = b and compute the residual norm, ||b - A x||.
    <P><PRE>
          double[][] vals = {{1.,2.,3},{4.,5.,6.},{7.,8.,10.}};
          Matrix A = new Matrix(vals);
          Matrix b = Matrix.random(3,1);
          Matrix x = A.solve(b);
          Matrix r = A.times(x).minus(b);
          double rnorm = r.normInf();
    </PRE></DD>
    </DL>

    @author The MathWorks, Inc. and the National Institute of Standards and Technology.
    @version 5 August 1998
    */

    [Serializable]
    public class MatrixClass
    {
        /* ------------------------
           Class variables
         * ------------------------ */

        /** Arr for internal storage of elements.
        @serial internal array storage.
        */
        protected double[,] m_dblArr;
        protected int m_intColumns;
        protected int m_intRows;

        /* ------------------------
           Constructors
         * ------------------------ */

        /** Construct an m-by-n matrix of zeros. 
        @param m    Number of rows.
        @param n    Number of colums.
        */

        public MatrixClass(int intRows, int intColumns)
        {
            m_intRows = intRows;
            m_intColumns = intColumns;
            m_dblArr = new double[intRows,intColumns];
        }


        /** Construct an m-by-n constant matrix.
        @param m    Number of rows.
        @param n    Number of colums.
        @param s    Fill the matrix with this scalar value.
        */

        public MatrixClass(int m, int n, double s)
        {
            m_intRows = m;
            m_intColumns = n;
            m_dblArr = new double[m,n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    m_dblArr[i, j] = s;
                }
            }
        }

        /** Construct a matrix from a 2-D array.
        @param A    Two-dimensional array of doubles.
        @exception  HCException All rows must have the same length
        @see        #constructWithCopy
        */

        public MatrixClass(double[,] A)
        {
            //m = A.Length;
            //n = A.GetLength(1);
            //for (int i = 0; i < m; i++) {
            //   if (A.GetLength(1) != n) {
            //      throw new ArgumentException("All rows must have the same length.");
            //   }
            //}

            m_intRows = A.GetLength(0);
            m_intColumns = A.GetLength(1);

            m_dblArr = A;
        }

        /** Construct a matrix quickly without checking arguments.
        @param A    Two-dimensional array of doubles.
        @param m    Number of rows.
        @param n    Number of colums.
        */

        public MatrixClass(double[,] A, int m, int n)
        {
            m_dblArr = A;
            m_intRows = m;
            m_intColumns = n;
        }

        public MatrixClass(double[] vals)
            : this(vals, vals.Length)
        {
        }

        /** Construct a matrix from a one-dimensional packed array
        @param vals One-dimensional array of doubles, packed by columns (ala Fortran).
        @param m    Number of rows.
        @exception  HCException Arr length must be a multiple of m.
        */

        public MatrixClass(double[] vals, int m)
        {
            m_intRows = m;
            m_intColumns = (m != 0 ? vals.Length/m : 0);
            if (m*m_intColumns != vals.Length)
            {
                throw new ArgumentException("Arr length must be a multiple of m.");
            }
            m_dblArr = new double[m,m_intColumns];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = vals[i + j*m];
                }
            }
        }


        /* ------------------------
           Public Methods
         * ------------------------ */

        /** Construct a matrix from a copy of a 2-D array.
        @param A    Two-dimensional array of doubles.
        @exception  HCException All rows must have the same length
        */

        public static MatrixClass constructWithCopy(double[,] A)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            MatrixClass X = new MatrixClass(m, n);
            double[,] C = X.GetArr();
            for (int i = 0; i < m; i++)
            {
                //if (A.GetLength(1) != n) {
                //   throw new ArgumentException
                //      ("All rows must have the same length.");
                //}
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j];
                }
            }
            return X;
        }

        public void SetArr(double[,] dblArr)
        {
            m_dblArr = dblArr;
            m_intRows = dblArr.GetLength(0);
            m_intColumns = dblArr.GetLength(1);
        }

        /** Make a deep copy of a matrix
        */

        public MatrixClass Copy()
        {
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j];
                }
            }
            return X;
        }

        /** Clone the Matrix object.
        */

        public Object clone()
        {
            return Copy();
        }

        /** Access the internal two-dimensional array.
        @return     Pointer to the two-dimensional array of matrix elements.
        */

        public double[,] GetArr()
        {
            return m_dblArr;
        }

        /** Copy the internal two-dimensional array.
        @return     Two-dimensional array copy of matrix elements.
        */

        public double[,] GetArrCopy()
        {
            double[,] C = new double[m_intRows,m_intColumns];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j];
                }
            }
            return C;
        }

        /** Make a one-dimensional column packed copy of the internal array.
        @return     Matrix elements packed in a one-dimensional array by columns.
        */

        public double[] getColumnPackedCopy()
        {
            double[] vals = new double[m_intRows*m_intColumns];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    vals[i + j*m_intRows] = m_dblArr[i, j];
                }
            }
            return vals;
        }

        /** Make a one-dimensional row packed copy of the internal array.
        @return     Matrix elements packed in a one-dimensional array by rows.
        */

        public double[] getRowPackedCopy()
        {
            double[] vals = new double[m_intRows*m_intColumns];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    vals[i*m_intColumns + j] = m_dblArr[i, j];
                }
            }
            return vals;
        }

        /** Get row dimension.
        @return     m, the number of rows.
        */

        public int GetRowDimension()
        {
            return m_intRows;
        }

        /** Get column dimension.
        @return     n, the number of columns.
        */

        public int GetColumnDimension()
        {
            return m_intColumns;
        }

        /** Get a single element.
        @param i    Row index.
        @param j    Column index.
        @return     A(i,j)
        @exception  HCException
        */

        public double Get(int i, int j)
        {
            return m_dblArr[i, j];
        }

        /** Get a submatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param j0   Initial column index
        @param j1   Final column index
        @return     A(i0:i1,j0:j1)
        @exception  HCException Submatrix indices
        */

        public MatrixClass getMatrix(int i0, int i1, int j0, int j1)
        {
            MatrixClass X = new MatrixClass(i1 - i0 + 1, j1 - j0 + 1);
            double[,] B = X.GetArr();
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        B[i - i0, j - j0] = m_dblArr[i, j];
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
            return X;
        }

        /** Get a submatrix.
        @param r    Arr of row indices.
        @param c    Arr of column indices.
        @return     A(r(:),c(:))
        @exception  HCException Submatrix indices
        */

        public MatrixClass getMatrix(int[] r, int[] c)
        {
            MatrixClass X = new MatrixClass(r.Length, c.Length);
            double[,] B = X.GetArr();
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        B[i, j] = m_dblArr[r[i], c[j]];
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
            return X;
        }

        /** Get a submatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param c    Arr of column indices.
        @return     A(i0:i1,c(:))
        @exception  HCException Submatrix indices
        */

        public MatrixClass getMatrix(int i0, int i1, int[] c)
        {
            MatrixClass X = new MatrixClass(i1 - i0 + 1, c.Length);
            double[,] B = X.GetArr();
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        B[i - i0, j] = m_dblArr[i, c[j]];
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
            return X;
        }

        /** Get a submatrix.
        @param r    Arr of row indices.
        @param i0   Initial column index
        @param i1   Final column index
        @return     A(r(:),j0:j1)
        @exception  HCException Submatrix indices
        */

        public MatrixClass getMatrix(int[] r, int j0, int j1)
        {
            MatrixClass X = new MatrixClass(r.Length, j1 - j0 + 1);
            double[,] B = X.GetArr();
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        B[i, j - j0] = m_dblArr[r[i], j];
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
            return X;
        }

        /** Set a single element.
        @param i    Row index.
        @param j    Column index.
        @param s    A(i,j).
        @exception  HCException
        */

        public void Set(int i, int j, double s)
        {
            m_dblArr[i, j] = s;
        }

        /** Set a submatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param j0   Initial column index
        @param j1   Final column index
        @param X    A(i0:i1,j0:j1)
        @exception  HCException Submatrix indices
        */

        public void setMatrix(int i0, int i1, int j0, int j1, MatrixClass X)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        m_dblArr[i, j] = X.Get(i - i0, j - j0);
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
        }

        /** Set a submatrix.
        @param r    Arr of row indices.
        @param c    Arr of column indices.
        @param X    A(r(:),c(:))
        @exception  HCException Submatrix indices
        */

        public void setMatrix(int[] r, int[] c, MatrixClass X)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        m_dblArr[r[i], c[j]] = X.Get(i, j);
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
        }

        /** Set a submatrix.
        @param r    Arr of row indices.
        @param j0   Initial column index
        @param j1   Final column index
        @param X    A(r(:),j0:j1)
        @exception  HCException Submatrix indices
        */

        public void setMatrix(int[] r, int j0, int j1, MatrixClass X)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        m_dblArr[r[i], j] = X.Get(i, j - j0);
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
        }

        /** Set a submatrix.
        @param i0   Initial row index
        @param i1   Final row index
        @param c    Arr of column indices.
        @param X    A(i0:i1,c(:))
        @exception  HCException Submatrix indices
        */

        public void setMatrix(int i0, int i1, int[] c, MatrixClass X)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        m_dblArr[i, c[j]] = X.Get(i - i0, j);
                    }
                }
            }
            catch
            {
                throw new HCException("Submatrix indices");
            }
        }

        /** Matrix transpose.
        @return    A'
        */

        public MatrixClass Transpose()
        {
            MatrixClass X = new MatrixClass(m_intColumns, m_intRows);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[j, i] = m_dblArr[i, j];
                }
            }
            return X;
        }

        /** One norm
        @return    maximum column sum.
        */

        public double norm1()
        {
            double f = 0;
            for (int j = 0; j < m_intColumns; j++)
            {
                double s = 0;
                for (int i = 0; i < m_intRows; i++)
                {
                    s += Math.Abs(m_dblArr[i, j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }

        /** Two norm
        @return    maximum singular value.
        */

        public double norm2()
        {
            return (new SingularValueDecomposition(this).norm2());
        }

        /** Infinity norm
        @return    maximum row sum.
        */

        public double normInf()
        {
            double f = 0;
            for (int i = 0; i < m_intRows; i++)
            {
                double s = 0;
                for (int j = 0; j < m_intColumns; j++)
                {
                    s += Math.Abs(m_dblArr[i, j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }

        /** Frobenius norm
        @return    sqrt of sum of squares of all elements.
        */

        public double normF()
        {
            double f = 0;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    f = MathHelper.Hypot(f, m_dblArr[i, j]);
                }
            }
            return f;
        }

        /**  Unary minus
        @return    -A
        */

        public MatrixClass uminus()
        {
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = -m_dblArr[i, j];
                }
            }
            return X;
        }

        /** C = A + B
        @param B    another matrix
        @return     A + B
        */

        public MatrixClass Plus(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j] + B.m_dblArr[i, j];
                }
            }
            return X;
        }

        public MatrixClass Plus(double dblK)
        {
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j] + dblK;
                }
            }
            return X;
        }

        /** A = A + B
        @param B    another matrix
        @return     A + B
        */

        public MatrixClass plusEquals(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = m_dblArr[i, j] + B.m_dblArr[i, j];
                }
            }
            return this;
        }

        /** C = A - B
        @param B    another matrix
        @return     A - B
        */

        public MatrixClass Minus(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j] - B.m_dblArr[i, j];
                }
            }
            return X;
        }

        /** A = A - B
        @param B    another matrix
        @return     A - B
        */

        public MatrixClass MinusEquals(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = m_dblArr[i, j] - B.m_dblArr[i, j];
                }
            }
            return this;
        }

        /** Element-by-element multiplication, C = A.*B
        @param B    another matrix
        @return     A.*B
        */

        public MatrixClass arrayTimes(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j]*B.m_dblArr[i, j];
                }
            }
            return X;
        }

        /** Element-by-element multiplication in place, A = A.*B
        @param B    another matrix
        @return     A.*B
        */

        public MatrixClass arrayTimesEquals(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = m_dblArr[i, j]*B.m_dblArr[i, j];
                }
            }
            return this;
        }

        /** Element-by-element right division, C = A./B
        @param B    another matrix
        @return     A./B
        */

        public MatrixClass arrayRightDivide(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = m_dblArr[i, j]/B.m_dblArr[i, j];
                }
            }
            return X;
        }

        /** Element-by-element right division in place, A = A./B
        @param B    another matrix
        @return     A./B
        */

        public MatrixClass arrayRightDivideEquals(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = m_dblArr[i, j]/B.m_dblArr[i, j];
                }
            }
            return this;
        }

        /** Element-by-element left division, C = A.\B
        @param B    another matrix
        @return     A.\B
        */

        public MatrixClass arrayLeftDivide(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = B.m_dblArr[i, j]/m_dblArr[i, j];
                }
            }
            return X;
        }

        /** Element-by-element left division in place, A = A.\B
        @param B    another matrix
        @return     A.\B
        */

        public MatrixClass arrayLeftDivideEquals(MatrixClass B)
        {
            CheckMatrixDimensions(B);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = B.m_dblArr[i, j]/m_dblArr[i, j];
                }
            }
            return this;
        }

        /** Multiply a matrix by a scalar, C = s*A
        @param s    scalar
        @return     s*A
        */

        public MatrixClass Times(double s)
        {
            MatrixClass X = new MatrixClass(m_intRows, m_intColumns);
            double[,] C = X.GetArr();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    C[i, j] = s*m_dblArr[i, j];
                }
            }
            return X;
        }

        /** Multiply a matrix by a scalar in place, A = s*A
        @param s    scalar
        @return     replace A by s*A
        */

        public MatrixClass timesEquals(double s)
        {
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = s*m_dblArr[i, j];
                }
            }
            return this;
        }

        /** Linear algebraic matrix multiplication, A * B
        @param B    another matrix
        @return     Matrix product, A * B
        @exception  HCException Matrix inner dimensions must agree.
        */

        public MatrixClass Times(MatrixClass B)
        {
            if (B.m_intRows != m_intColumns)
            {
                throw new ArgumentException("Matrix inner dimensions must agree.");
            }
            MatrixClass X = new MatrixClass(m_intRows, B.m_intColumns);
            double[,] C = X.GetArr();
            double[] Bcolj = new double[m_intColumns];
            for (int j = 0; j < B.m_intColumns; j++)
            {
                for (int k = 0; k < m_intColumns; k++)
                {
                    Bcolj[k] = B.m_dblArr[k, j];
                }
                for (int i = 0; i < m_intRows; i++)
                {
                    double[] Arowi =
                        ArrayHelper.GetRowCopy(m_dblArr, i);
                    double s = 0;
                    for (int k = 0; k < m_intColumns; k++)
                    {
                        s += Arowi[k]*Bcolj[k];
                    }
                    C[i, j] = s;
                }
            }
            return X;
        }

        /** LU Decomposition
        @return     LUDecomposition
        @see LUDecomposition
        */

        public LUDecomposition lu()
        {
            return new LUDecomposition(this);
        }

        /** QR Decomposition
        @return     QRDecomposition
        @see QRDecomposition
        */

        public QRDecomposition qr()
        {
            return new QRDecomposition(this);
        }

        /** Cholesky Decomposition
        @return     CholeskyDecomposition
        @see CholeskyDecomposition
        */

        //private CholeskyDecomposition CholeskyDecomposition()
        //{
        //    return new CholeskyDecomposition(this);
        //}

        public MatrixClass CholeskyDecomposition()
        {
            return new CholeskyDecomposition(this).GetLMatrix();
        }

        /** Singular Value Decomposition
        @return     SingularValueDecomposition
        @see SingularValueDecomposition
        */

        public SingularValueDecomposition svd()
        {
            return new SingularValueDecomposition(this);
        }

        /** Eigenvalue Decomposition
        @return     EigenvalueDecomposition
        @see EigenvalueDecomposition
        */

        public EigenvalueDecomposition eig()
        {
            return new EigenvalueDecomposition(this);
        }

        /** Solve A*X = B
        @param B    right hand side
        @return     solution if A is square, least squares solution otherwise
        */

        public MatrixClass solve(MatrixClass B)
        {
            return (m_intRows == m_intColumns
                        ? (new LUDecomposition(this)).solve(B)
                        :
                            (new QRDecomposition(this)).solve(B));
        }

        /** Solve X*A = B, which is also A'*X' = B'
        @param B    right hand side
        @return     solution if A is square, least squares solution otherwise.
        */

        public MatrixClass solveTranspose(MatrixClass B)
        {
            return Transpose().solve(B.Transpose());
        }

        /** Matrix inverse or pseudoinverse
        @return     inverse(A) if A is square, pseudoinverse otherwise.
        */

        public MatrixClass Inverse()
        {
            return solve(identity(m_intRows, m_intRows));
        }

        /** Matrix determinant
        @return     determinant
        */

        public double Determinant()
        {
            return new LUDecomposition(this).det();
        }

        /** Matrix rank
        @return     effective numerical rank, obtained from SVD.
        */

        public int rank()
        {
            return new SingularValueDecomposition(this).rank();
        }

        /** Matrix condition (2 norm)
        @return     ratio of largest to smallest singular value.
        */

        public double cond()
        {
            return new SingularValueDecomposition(this).cond();
        }

        /** Matrix trace.
        @return     sum of the diagonal elements.
        */

        public double trace()
        {
            double t = 0;
            for (int i = 0; i < Math.Min(m_intRows, m_intColumns); i++)
            {
                t += m_dblArr[i, i];
            }
            return t;
        }

        /** Generate matrix with random elements
        @param m    Number of rows.
        @param n    Number of colums.
        @return     An m-by-n matrix with uniformly distributed random elements.
        */

        public static MatrixClass random(int m, int n)
        {
            MatrixClass A = new MatrixClass(m, n);
            double[,] X = A.GetArr();
            RngWrapper uniformRandomGenerator =
                new RngWrapper();

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i, j] = uniformRandomGenerator.NextDouble();
                }
            }
            return A;
        }

        /** Generate identity matrix
        @param m    Number of rows.
        @param n    Number of colums.
        @return     An m-by-n matrix with ones on the diagonal and zeros elsewhere.
        */

        public static MatrixClass identity(int m, int n)
        {
            MatrixClass A = new MatrixClass(m, n);
            double[,] X = A.GetArr();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i, j] = (i == j ? 1.0 : 0.0);
                }
            }
            return A;
        }


        /** Print the matrix to stdout.   Line the elements up in columns
          * with a Fortran-like 'Fw.d' style format.
        @param w    Column width.
        @param d    Number of digits after the decimal.
        */

        //public void print (int w, int d) {
        //   print(new StreamWriter(Console,true),w,d); }

        /** Print the matrix to the output stream.   Line the elements up in
          * columns with a Fortran-like 'Fw.d' style format.
        @param output Output stream.
        @param w      Column width.
        @param d      Number of digits after the decimal.
        */

        //public void print (StreamWriter output, int w, int d) {
        //   DecimalFormat format = new DecimalFormat();
        //   format.setDecimalFormatSymbols(new DecimalFormatSymbols(Locale.US));
        //   format.setMinimumIntegerDigits(1);
        //   format.setMaximumFractionDigits(d);
        //   format.setMinimumFractionDigits(d);
        //   format.setGroupingUsed(false);
        //   print(output,format,w+2);
        //}

        /** Print the matrix to stdout.  Line the elements up in columns.
          * Use the format object, and right justify within columns of width
          * characters.
          * Note that is the matrix is to be read back in, you probably will want
          * to use a NumberFormat that is set to US Locale.
        @param format A  Formatting object for individual elements.
        @param width     Field width for each column.
        @see java.text.DecimalFormat#setDecimalFormatSymbols
        */

        //public void print (NumberFormat format, int width) {
        //   print(new StreamWriter(Console,true),format,width); }

        // DecimalFormat is a little disappointing coming from Fortran or C's printf.
        // Since it doesn't pad on the left, the elements will come out different
        // widths.  Consequently, we'll pass the desired column width in as an
        // argument and do the extra padding ourselves.

        /** Print the matrix to the output stream.  Line the elements up in columns.
          * Use the format object, and right justify within columns of width
          * characters.
          * Note that is the matrix is to be read back in, you probably will want
          * to use a NumberFormat that is set to US Locale.
        @param output the output stream.
        @param format A formatting object to format the matrix elements 
        @param width  Column width.
        @see java.text.DecimalFormat#setDecimalFormatSymbols
        */

        //public void print (StreamWriter output, NumberFormat format, int width) {
        //   output.println();  // start on new line.
        //   for (int i = 0; i < m; i++) {
        //      for (int j = 0; j < n; j++) {
        //         String s = format.format(A[i,j]); // format the number
        //         int padding = Math.Max(1,width-s.Length()); // At _least_ 1 space
        //         for (int k = 0; k < padding; k++)
        //            output.print(' ');
        //         output.print(s);
        //      }
        //      output.println();
        //   }
        //   output.println();   // end with blank line.
        //}

        /** Read a matrix from a stream.  The format is the same the print method,
          * so printed matrices can be read back in (provided they were printed using
          * US Locale).  Elements are separated by
          * whitespace, all the elements for each row appear on a single line,
          * the last row is followed by a blank line.
        @param input the input stream.
        */

        //public static Matrix read (BufferedReader input) 
        //{
        //   StreamTokenizer tokenizer= new StreamTokenizer(input);

        //   // Although StreamTokenizer will parse numbers, it doesn't recognize
        //   // scientific notation (E or D); however, Double.valueOf does.
        //   // The strategy here is to disable StreamTokenizer's number parsing.
        //   // We'll only get whitespace delimited words, EOL's and EOF's.
        //   // These words should all be numbers, for Double.valueOf to parse.

        //   tokenizer.resetSyntax();
        //   tokenizer.wordChars(0,255);
        //   tokenizer.whitespaceChars(0, ' ');
        //   tokenizer.eolIsSignificant(true);
        //   java.util.Vector v = new java.util.Vector();

        //   // Ignore initial empty lines
        //   while (tokenizer.nextToken() == StreamTokenizer.TT_EOL);
        //   if (tokenizer.ttype == StreamTokenizer.TT_EOF)
        // throw new java.io.IOException("Unexpected EOF on matrix read.");
        //   do {
        //      v.Add(Double.valueOf(tokenizer.sval)); // Read & store 1st row.
        //   } while (tokenizer.nextToken() == StreamTokenizer.TT_WORD);

        //   int n = v.size();  // Now we've got the number of columns!
        //   double row[] = new double[n];
        //   for (int j=0; j<n; j++)  // extract the elements of the 1st row.
        //      row[j]=((Double)v.elementAt(j));
        //   v.removeAllElements();
        //   v.Add(row);  // Start storing rows instead of columns.
        //   while (tokenizer.nextToken() == StreamTokenizer.TT_WORD) {
        //      // While non-empty lines
        //      v.Add(row = new double[n]);
        //      int j = 0;
        //      do {
        //         if (j >= n) throw new java.io.IOException
        //            ("Row " + v.size() + " is too long.");
        //         row[j++] = Double.valueOf(tokenizer.sval);
        //      } while (tokenizer.nextToken() == StreamTokenizer.TT_WORD);
        //      if (j < n) throw new java.io.IOException
        //         ("Row " + v.size() + " is too short.");
        //   }
        //   int m = v.size();  // Now we've got the number of rows.
        //   double[,] A = new double[m,];
        //   v.copyInto(A);  // copy the rows out of the vector
        //   return new Matrix(A);
        //}


        /* ------------------------
           Private Methods
         * ------------------------ */

        /** Check if size(A) == size(B) **/

        private void CheckMatrixDimensions(MatrixClass B)
        {
            if (B.m_intRows != m_intRows || B.m_intColumns != m_intColumns)
            {
                throw new ArgumentException("Matrix dimensions must agree.");
            }
        }
    }
}
