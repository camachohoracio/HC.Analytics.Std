#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Analysis
{
    public class MatrixExtended : MatrixClass
    {
        #region Members

        /* ------------------------
           Class variables
         * ------------------------ */

        /** Arr for internal storage of elements.
        @serial internal array storage.
        */
        //private int m_intColumns;
        //private int m_intRows;
        //protected double[,] m_dblArr;
        private readonly int[] m_permutationIndex; // row permutation index
        private bool m_blnSupressErrorMessage; // true - LU decompostion failure message supressed
        private bool m_eigenDone; // = true when eigen values and vectors calculated
        private int[] m_eigenIndices; // indices of the eigen values before sorting into descending order
        private double[] m_eigenValues; // eigen values of the matrix
        private double[,] m_eigenVector; // eigen vectors of the matrix
        private double[,] m_hessenberg; // 2-D  Hessenberg equivalent
        private bool m_hessenbergDone; // = true when Hessenberg matrix calculated
        private bool m_matrixCheck = true; // check on matrix status
        private int m_maximumJacobiIterations = 100; // maximum number of Jacobi iterations
        private int m_numberOfRotations; // number of rotations in Jacobi transformation
        private double m_rowSwapIndex = 1.0D; // row swap index
        private double[] m_sortedEigenValues; // eigen values of the matrix sorted into descending order

        private double[,] m_sortedEigenVector;
        // eigen vectors of the matrix sorted to matching descending eigen value order

        private double m_tiny = 1.0e-100; // small number replacing zero in LU decomposition

        #endregion

        #region Constructors

        /** Construct an m-by-n m_dblArr of zeros. 
        @param m    Number of rows.
        @param n    Number of colums.
        */

        public MatrixExtended(int intRows, int intColumns) :
            base(intRows, intColumns)
        {
        }


        /** Construct an m-by-n constant m_dblArr.
        @param m    Number of rows.
        @param n    Number of colums.
        @param s    Fill the m_dblArr with this scalar value.
        */

        public MatrixExtended(int m, int n, double s) :
            base(m, n, s)
        {
        }

        /** Construct a m_dblArr from a 2-D array.
        @param A    Two-dimensional array of doubles.
        @exception  HCException All rows must have the same length
        @see        #constructWithCopy
        */

        public MatrixExtended(double[,] A) :
            base(A)
        {
        }

        /** Construct a m_dblArr quickly without checking arguments.
        @param A    Two-dimensional array of doubles.
        @param m    Number of rows.
        @param n    Number of colums.
        */

        public MatrixExtended(double[,] A, int m, int n) :
            base(A, m, n)
        {
        }

        public MatrixExtended(double[] vals)
            : this(vals, vals.Length)
        {
        }

        /** Construct a m_dblArr from a one-dimensional packed array
        @param vals One-dimensional array of doubles, packed by columns (ala Fortran).
        @param m    Number of rows.
        @exception  HCException Arr length must be a multiple of m.
        */

        public MatrixExtended(double[] vals, int m) :
            base(vals, m)
        {
        }

        #endregion

        /* ------------------------
           Public Methods
         * ------------------------ */

        // Set the m_dblArr with a copy of an existing m_intRows x m_intColumns 2-D m_dblArr of variables
        public void setTwoDarray(double[,] aarray)
        {
            if (m_intRows != aarray.GetLength(0))
            {
                throw new ArgumentException(
                    "row length of this Matrix_non_stable differs from that of the 2D array argument");
            }
            if (m_intColumns != aarray.GetLength(1))
            {
                throw new ArgumentException(
                    "column length of this Matrix_non_stable differs from that of the 2D array argument");
            }
            for (int i = 0; i < m_intRows; i++)
            {
                if (aarray.GetLength(1) != m_intColumns)
                {
                    throw new ArgumentException("All rows must have the same length");
                }
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = aarray[i, j];
                }
            }
        }

        // Set an individual array element
        // i = row index
        // j = column index
        // aa = value of the element
        public void setElement(int i, int j, double aa)
        {
            m_dblArr[i, j] = aa;
        }


        // Set a sub-m_dblArr starting with row index i, column index j
        public void setSubMatrix(int i, int j, double[,] subMatrix)
        {
            int k = subMatrix.GetLength(0);
            int l = subMatrix.GetLength(1);
            if (i > k)
            {
                throw new ArgumentException("row indices inverted");
            }
            if (j > l)
            {
                throw new ArgumentException("column indices inverted");
            }
            int m = 0;
            int n = 0;
            for (int p = 0; p < k; p++)
            {
                n = 0;
                for (int q = 0; q < l; q++)
                {
                    m_dblArr[i + p, j + q] = subMatrix[m, n];
                    n++;
                }
                m++;
            }
        }

        // Set a sub-m_dblArr starting with row index i, column index j
        // and ending with row index k, column index l
        // See setSubMatrix above - this method has been retained for compatibility purposes
        public void setSubMatrix(int i, int j, int k, int l, double[,] subMatrix)
        {
            setSubMatrix(i, j, subMatrix);
        }

        // Set a sub-m_dblArr
        // row = array of row indices
        // col = array of column indices
        public void setSubMatrix(int[] row, int[] col, double[,] subMatrix)
        {
            int n = row.Length;
            int m = col.Length;
            for (int p = 0; p < n; p++)
            {
                for (int q = 0; q < m; q++)
                {
                    m_dblArr[row[p], col[q]] = subMatrix[p, q];
                }
            }
        }

        // Get the value of m_matrixCheck
        public bool getMatrixCheck()
        {
            return m_matrixCheck;
        }

        // SPECIAL MATRICES
        // Construct an identity m_dblArr
        public static MatrixExtended identityMatrix(int m_intRows)
        {
            MatrixExtended special = new MatrixExtended(m_intRows, m_intRows);
            for (int i = 0; i < m_intRows; i++)
            {
                special.m_dblArr[i, i] = 1.0;
            }
            return special;
        }

        // Construct a square unit m_dblArr
        public static MatrixExtended unitMatrix(int m_intRows)
        {
            MatrixExtended special = new MatrixExtended(m_intRows, m_intRows);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intRows; j++)
                {
                    special.m_dblArr[i, j] = 1.0;
                }
            }
            return special;
        }

        // Construct a rectangular unit m_dblArr
        public static MatrixExtended unitMatrix(int m_intRows, int m_intColumns)
        {
            MatrixExtended special = new MatrixExtended(m_intRows, m_intColumns);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    special.m_dblArr[i, j] = 1.0;
                }
            }
            return special;
        }

        // Construct a square scalar m_dblArr
        public static MatrixExtended scalarMatrix(int m_intRows, double diagconst)
        {
            MatrixExtended special = new MatrixExtended(m_intRows, m_intRows);
            double[,] specialArray = special.getArrayReference();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = i; j < m_intRows; j++)
                {
                    if (i == j)
                    {
                        specialArray[i, j] = diagconst;
                    }
                }
            }
            return special;
        }

        // Construct a rectangular scalar m_dblArr
        public static MatrixExtended scalarMatrix(int m_intRows, int m_intColumns, double diagconst)
        {
            MatrixExtended special = new MatrixExtended(m_intRows, m_intColumns);
            double[,] specialArray = special.getArrayReference();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = i; j < m_intColumns; j++)
                {
                    if (i == j)
                    {
                        specialArray[i, j] = diagconst;
                    }
                }
            }
            return special;
        }

        // Construct a square diagonal m_dblArr
        public static MatrixExtended diagonalMatrix(int m_intRows, double[] diag)
        {
            if (diag.Length != m_intRows)
            {
                throw new ArgumentException("m_dblArr dimension differs from diagonal array length");
            }
            MatrixExtended special = new MatrixExtended(m_intRows, m_intRows);
            double[,] specialArray = special.getArrayReference();
            for (int i = 0; i < m_intRows; i++)
            {
                specialArray[i, i] = diag[i];
            }
            return special;
        }

        // Construct a rectangular diagonal m_dblArr
        public static MatrixExtended diagonalMatrix(int m_intRows, int m_intColumns, double[] diag)
        {
            if (diag.Length != m_intRows)
            {
                throw new ArgumentException("m_dblArr dimension differs from diagonal array length");
            }
            MatrixExtended special = new MatrixExtended(m_intRows, m_intColumns);
            double[,] specialArray = special.getArrayReference();
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = i; j < m_intColumns; j++)
                {
                    if (i == j)
                    {
                        specialArray[i, j] = diag[i];
                    }
                }
            }
            return special;
        }

        // GET VALUES
        // Return the number of rows
        public int getNumberOfRows()
        {
            return m_intRows;
        }

        // Return the number of rows
        public int getNrow()
        {
            return m_intRows;
        }

        // Return the number of columns
        public int getNumberOfColumns()
        {
            return m_intColumns;
        }

        // Return the number of columns
        public int getNcol()
        {
            return m_intColumns;
        }

        // Return a reference to the internal 2-D array
        public double[,] getArrayReference()
        {
            return m_dblArr;
        }

        // Return a reference to the internal 2-D array
        // included for backward compatibility with incorrect earlier documentation
        public double[,] getArrayPointer()
        {
            return m_dblArr;
        }

        // Return a copy of the internal 2-D array
        public double[,] getArrayCopy()
        {
            double[,] c = new double[m_intRows,m_intColumns];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    c[i, j] = m_dblArr[i, j];
                }
            }
            return c;
        }

        // Return a copy of a row
        public double[] getRowCopy(int i)
        {
            if (i >= m_intRows)
            {
                throw new ArgumentException("Row index, " + i + ", must be less than the number of rows, " + m_intRows);
            }
            if (i < 0)
            {
                throw new ArgumentException("Row index, " + i + ", must be zero or positive");
            }
            double[] dblNewArray = new double[
                m_intColumns];

            for (int index = 0; index < m_intColumns; index++)
            {
                dblNewArray[index] = m_dblArr[i, index];
            }
            return dblNewArray;
        }

        // Return a copy of a column
        public double[] getColumnCopy(int ii)
        {
            if (ii >= m_intColumns)
            {
                throw new ArgumentException("Column index, " + ii + ", must be less than the number of columns, " +
                                            m_intColumns);
            }
            if (ii < 0)
            {
                throw new ArgumentException("column index, " + ii + ", must be zero or positive");
            }
            double[] col = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                col[i] = m_dblArr[i, ii];
            }
            return col;
        }


        // Return  a single element of the internal 2-D array
        public double getElement(int i, int j)
        {
            return m_dblArr[i, j];
        }

        // Return a single element of the internal 2-D array
        // included for backward compatibility with incorrect earlier documentation
        public double getElementCopy(int i, int j)
        {
            return m_dblArr[i, j];
        }

        // Return a single element of the internal 2-D array
        // included for backward compatibility with incorrect earlier documentation
        public double getElementPointer(int i, int j)
        {
            return m_dblArr[i, j];
        }

        // Return a sub-m_dblArr starting with row index i, column index j
        // and ending with row index k, column index l
        public MatrixExtended getSubMatrix(int i, int j, int k, int l)
        {
            if (i > k)
            {
                throw new ArgumentException("row indices inverted");
            }
            if (j > l)
            {
                throw new ArgumentException("column indices inverted");
            }
            int n = k - i + 1, m = l - j + 1;
            MatrixExtended subMatrix = new MatrixExtended(n, m);
            double[,] sarray = subMatrix.getArrayReference();
            for (int p = 0; p < n; p++)
            {
                for (int q = 0; q < m; q++)
                {
                    sarray[p, q] = m_dblArr[i + p, j + q];
                }
            }
            return subMatrix;
        }

        // Return a sub-m_dblArr
        // row = array of row indices
        // col = array of column indices
        public MatrixExtended getSubMatrix(int[] row, int[] col)
        {
            int n = row.Length;
            int m = col.Length;
            MatrixExtended subMatrix = new MatrixExtended(n, m);
            double[,] sarray = subMatrix.getArrayReference();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    sarray[i, j] = m_dblArr[row[i], col[j]];
                }
            }
            return subMatrix;
        }

        // Return a reference to the permutation index array
        public int[] getIndexReference()
        {
            return m_permutationIndex;
        }

        // Return a reference to the permutation index array
        // included for backward compatibility with incorrect earlier documentation
        public int[] getIndexPointer()
        {
            return m_permutationIndex;
        }

        // Return a copy of the permutation index array
        public int[] getIndexCopy()
        {
            int[] indcopy = new int[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                indcopy[i] = m_permutationIndex[i];
            }
            return indcopy;
        }

        // Return the row swap index
        public double getSwap()
        {
            return m_rowSwapIndex;
        }

        // COPY
        // Copy a Matrix_non_stable [static method]
        public static MatrixExtended Copy(MatrixExtended a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                int nr = a.getNumberOfRows();
                int nc = a.getNumberOfColumns();
                double[,] aarray = a.getArrayReference();
                MatrixExtended b = new MatrixExtended(nr, nc);
                b.m_intRows = nr;
                b.m_intColumns = nc;
                double[,] barray = b.getArrayReference();
                for (int i = 0; i < nr; i++)
                {
                    for (int j = 0; j < nc; j++)
                    {
                        barray[i, j] = aarray[i, j];
                    }
                }
                for (int i = 0; i < nr; i++)
                {
                    b.m_permutationIndex[i] = a.m_permutationIndex[i];
                }
                return b;
            }
        }

        // Copy a Matrix_non_stable [instance method]
        public new MatrixExtended Copy()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                int nr = m_intRows;
                int nc = m_intColumns;
                MatrixExtended b = new MatrixExtended(nr, nc);
                double[,] barray = b.getArrayReference();
                b.m_intRows = nr;
                b.m_intColumns = nc;
                for (int i = 0; i < nr; i++)
                {
                    for (int j = 0; j < nc; j++)
                    {
                        barray[i, j] = m_dblArr[i, j];
                    }
                }
                for (int i = 0; i < nr; i++)
                {
                    b.m_permutationIndex[i] = m_permutationIndex[i];
                }
                return b;
            }
        }

        // Clone a Matrix_non_stable
        // COLUMN MATRICES
        // Converts a 1-D array of doubles to a column  m_dblArr
        public static MatrixExtended columnMatrix(double[] darray)
        {
            int nr = darray.Length;
            MatrixExtended pp = new MatrixExtended(nr, 1);
            for (int i = 0; i < nr; i++)
            {
                pp.m_dblArr[i, 0] = darray[i];
            }
            return pp;
        }

        // ROW MATRICES
        // Converts a 1-D array of doubles to a row m_dblArr
        public static MatrixExtended rowMatrix(double[] darray)
        {
            int nc = darray.Length;
            MatrixExtended pp = new MatrixExtended(1, nc);
            for (int i = 0; i < nc; i++)
            {
                pp.m_dblArr[0, i] = darray[i];
            }
            return pp;
        }

        // ADDITION
        // Add this m_dblArr to m_dblArr B.  This m_dblArr remains unaltered [instance method]
        public MatrixExtended plus(MatrixExtended bmat)
        {
            if ((m_intRows != bmat.m_intRows) || (m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = bmat.m_intRows;
            int nc = bmat.m_intColumns;
            MatrixExtended cmat = new MatrixExtended(nr, nc);
            double[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_dblArr[i, j] + bmat.m_dblArr[i, j];
                }
            }
            return cmat;
        }

        // Add this m_dblArr to 2-D array B.  This m_dblArr remains unaltered [instance method]
        public MatrixExtended plus(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_intRows != nr) || (m_intColumns != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            MatrixExtended cmat = new MatrixExtended(nr, nc);
            double[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_dblArr[i, j] + bmat[i, j];
                }
            }
            return cmat;
        }


        // Add matrices A and B [static method]
        public static MatrixExtended plus(MatrixExtended amat, MatrixExtended bmat)
        {
            if ((amat.m_intRows != bmat.m_intRows) || (amat.m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = amat.m_intRows;
            int nc = amat.m_intColumns;
            MatrixExtended cmat = new MatrixExtended(nr, nc);
            double[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = amat.m_dblArr[i, j] + bmat.m_dblArr[i, j];
                }
            }
            return cmat;
        }

        // Add m_dblArr B to this m_dblArr [equivalence of +=]
        // SUBTRACTION
        // Subtract m_dblArr B from this m_dblArr.   This m_dblArr remains unaltered [instance method]
        public MatrixExtended minus(MatrixExtended bmat)
        {
            if ((m_intRows != bmat.m_intRows) || (m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = m_intRows;
            int nc = m_intColumns;
            MatrixExtended cmat = new MatrixExtended(nr, nc);
            double[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_dblArr[i, j] - bmat.m_dblArr[i, j];
                }
            }
            return cmat;
        }

        // Subtract a  2-D array from this m_dblArr.  This m_dblArr remains unaltered [instance method]
        public MatrixExtended minus(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_intRows != nr) || (m_intColumns != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            MatrixExtended cmat = new MatrixExtended(nr, nc);
            double[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_dblArr[i, j] - bmat[i, j];
                }
            }
            return cmat;
        }


        // Subtract m_dblArr B from m_dblArr A [static method]
        public static MatrixExtended minus(MatrixExtended amat, MatrixExtended bmat)
        {
            if ((amat.m_intRows != bmat.m_intRows) || (amat.m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = amat.m_intRows;
            int nc = amat.m_intColumns;
            MatrixExtended cmat = new MatrixExtended(nr, nc);
            double[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = amat.m_dblArr[i, j] - bmat.m_dblArr[i, j];
                }
            }
            return cmat;
        }

        // Subtract m_dblArr B from this m_dblArr [equivlance of -=]
        public void minusEquals(MatrixExtended bmat)
        {
            if ((m_intRows != bmat.m_intRows) || (m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = bmat.m_intRows;
            int nc = bmat.m_intColumns;

            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    m_dblArr[i, j] -= bmat.m_dblArr[i, j];
                }
            }
        }

        // MULTIPLICATION
        // Multiply this  m_dblArr by a m_dblArr.   [instance method]
        // This m_dblArr remains unaltered.
        public MatrixExtended times(MatrixExtended bmat)
        {
            if (m_intColumns != bmat.m_intRows)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            MatrixExtended cmat = new MatrixExtended(m_intRows, bmat.m_intColumns);
            double[,] carray = cmat.getArrayReference();
            double sum = 0.0D;

            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < bmat.m_intColumns; j++)
                {
                    sum = 0.0D;
                    for (int k = 0; k < m_intColumns; k++)
                    {
                        sum += m_dblArr[i, k]*bmat.m_dblArr[k, j];
                    }
                    carray[i, j] = sum;
                }
            }
            return cmat;
        }

        // Multiply this  m_dblArr by a 2-D array.   [instance method]
        // This m_dblArr remains unaltered.
        public MatrixExtended times(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);

            if (m_intColumns != nr)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            MatrixExtended cmat = new MatrixExtended(m_intRows, nc);
            double[,] carray = cmat.getArrayReference();
            double sum = 0.0D;

            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    sum = 0.0D;
                    for (int k = 0; k < m_intColumns; k++)
                    {
                        sum += m_dblArr[i, k]*bmat[k, j];
                    }
                    carray[i, j] = sum;
                }
            }
            return cmat;
        }

        // Multiply this m_dblArr by a constant [instance method]
        // This m_dblArr remains unaltered
        public MatrixExtended times(double constant)
        {
            MatrixExtended cmat = new MatrixExtended(m_intRows, m_intColumns);
            double[,] carray = cmat.getArrayReference();

            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    carray[i, j] = m_dblArr[i, j]*constant;
                }
            }
            return cmat;
        }

        // Multiply two matrices {static method]
        public static MatrixExtended times(MatrixExtended amat, MatrixExtended bmat)
        {
            if (amat.m_intColumns != bmat.m_intRows)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            MatrixExtended cmat = new MatrixExtended(amat.m_intRows, bmat.m_intColumns);
            double[,] carray = cmat.getArrayReference();
            double sum = 0.0D;

            for (int i = 0; i < amat.m_intRows; i++)
            {
                for (int j = 0; j < bmat.m_intColumns; j++)
                {
                    sum = 0.0D;
                    for (int k = 0; k < amat.m_intColumns; k++)
                    {
                        sum += (amat.m_dblArr[i, k]*bmat.m_dblArr[k, j]);
                    }
                    carray[i, j] = sum;
                }
            }
            return cmat;
        }

        // Multiply a Matrix_non_stable by a 2-D array of doubles [static method]
        public static MatrixExtended times(MatrixExtended amat, double[,] bmat)
        {
            if (amat.m_intColumns != bmat.GetLength(0))
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            MatrixExtended cmat = new MatrixExtended(amat.m_intRows, bmat.GetLength(1));
            MatrixExtended dmat = new MatrixExtended(bmat);
            double[,] carray = cmat.getArrayReference();
            double sum = 0.0D;

            for (int i = 0; i < amat.m_intRows; i++)
            {
                for (int j = 0; j < dmat.m_intColumns; j++)
                {
                    sum = 0.0D;
                    for (int k = 0; k < amat.m_intColumns; k++)
                    {
                        sum += (amat.m_dblArr[i, k]*dmat.m_dblArr[k, j]);
                    }
                    carray[i, j] = sum;
                }
            }
            return cmat;
        }

        // Multiply a m_dblArr by a constant [static method]
        public static MatrixExtended times(MatrixExtended amat, double constant)
        {
            MatrixExtended cmat = new MatrixExtended(amat.m_intRows, amat.m_intColumns);
            double[,] carray = cmat.getArrayReference();

            for (int i = 0; i < amat.m_intRows; i++)
            {
                for (int j = 0; j < amat.m_intColumns; j++)
                {
                    carray[i, j] = amat.m_dblArr[i, j]*constant;
                }
            }
            return cmat;
        }

        // Multiply this m_dblArr by a m_dblArr [equivalence of *=]
        public void timesEquals(MatrixExtended bmat)
        {
            if (m_intColumns != bmat.m_intRows)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            MatrixExtended cmat = new MatrixExtended(m_intRows, bmat.m_intColumns);
            double[,] carray = cmat.getArrayReference();
            double sum = 0.0D;

            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < bmat.m_intColumns; j++)
                {
                    sum = 0.0D;
                    for (int k = 0; k < m_intColumns; k++)
                    {
                        sum += m_dblArr[i, k]*bmat.m_dblArr[k, j];
                    }
                    carray[i, j] = sum;
                }
            }

            m_intRows = cmat.m_intRows;
            m_intColumns = cmat.m_intColumns;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    m_dblArr[i, j] = cmat.m_dblArr[i, j];
                }
            }
        }

        // DIVISION
        // Divide this Matrix_non_stable by a Matrix_non_stable  - instance method
        public MatrixExtended over(MatrixExtended bmat)
        {
            if ((m_intRows != bmat.m_intRows) || (m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            return times(bmat.inverse());
        }

        // Divide a Matrix_non_stable by a Matrix_non_stable - static method.
        public MatrixExtended over(MatrixExtended amat, MatrixExtended bmat)
        {
            if ((amat.m_intRows != bmat.m_intRows) || (amat.m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            return amat.times(bmat.inverse());
        }


        // Divide this Matrix_non_stable by a  2-D array of doubles.
        public MatrixExtended over(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_intRows != nr) || (m_intColumns != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            MatrixExtended cmat = new MatrixExtended(bmat);
            return times(cmat.inverse());
        }

        // Divide a Matrix_non_stable by a  2-D array of doubles - static method.
        public MatrixExtended over(MatrixExtended amat, double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((amat.m_intRows != nr) || (amat.m_intColumns != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            MatrixExtended cmat = new MatrixExtended(bmat);
            return amat.times(cmat.inverse());
        }

        // Divide a 2-D array of doubles by a Matrix_non_stable - static method.
        public MatrixExtended over(double[,] amat, MatrixExtended bmat)
        {
            int nr = amat.GetLength(0);
            int nc = amat.GetLength(1);
            if ((bmat.m_intRows != nr) || (bmat.m_intColumns != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            MatrixExtended cmat = new MatrixExtended(amat);
            return cmat.times(bmat.inverse());
        }

        // Divide a 2-D array of doubles by a 2-D array of doubles - static method.
        public MatrixExtended over(double[,] amat, double[,] bmat)
        {
            int nr = amat.GetLength(0);
            int nc = amat.GetLength(1);
            if ((bmat.GetLength(0) != nr) || (bmat.GetLength(1) != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            MatrixExtended cmat = new MatrixExtended(amat);
            MatrixExtended dmat = new MatrixExtended(bmat);
            return cmat.times(dmat.inverse());
        }

        // Divide a this m_dblArr by a m_dblArr[equivalence of /=]
        public void overEquals(MatrixExtended bmat)
        {
            if ((m_intRows != bmat.m_intRows) || (m_intColumns != bmat.m_intColumns))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            MatrixExtended cmat = new MatrixExtended(bmat.m_dblArr);
            timesEquals(cmat.inverse());
        }

        // Divide this Matrix_non_stable by a 2D array of doubles [equivalence of /=]
        public void overEquals(double[,] bmat)
        {
            MatrixExtended pmat = new MatrixExtended(bmat);
            overEquals(pmat);
        }

        // INVERSE
        // Inverse of a square m_dblArr [instance method]
        public MatrixExtended inverse()
        {
            int n = m_intRows;
            if (n != m_intColumns)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            MatrixExtended invmat = new MatrixExtended(n, n);

            if (n == 1)
            {
                double[,] hold = getArrayCopy();
                if (hold[0, 0] == 0.0)
                {
                    throw new ArgumentException("Matrix_non_stable is singular");
                }
                hold[0, 0] = 1.0/hold[0, 0];
                invmat = new MatrixExtended(hold);
            }
            else
            {
                if (n == 2)
                {
                    double[,] hold = getArrayCopy();
                    double det = hold[0, 0]*hold[1, 1] - hold[0, 1]*hold[1, 0];
                    if (det == 0.0)
                    {
                        throw new ArgumentException("Matrix_non_stable is singular");
                    }
                    double[,] hold2 = new double[2,2];
                    hold2[0, 0] = hold[1, 1]/det;
                    hold2[1, 1] = hold[0, 0]/det;
                    hold2[1, 0] = -hold[1, 0]/det;
                    hold2[0, 1] = -hold[0, 1]/det;
                    invmat = new MatrixExtended(hold2);
                }
                else
                {
                    double[] col = new double[n];
                    double[] xvec = new double[n];
                    double[,] invarray = invmat.getArrayReference();
                    MatrixExtended ludmat;

                    ludmat = luDecomp();
                    for (int j = 0; j < n; j++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            col[i] = 0.0D;
                        }
                        col[j] = 1.0;
                        xvec = ludmat.luBackSub(col);
                        for (int i = 0; i < n; i++)
                        {
                            invarray[i, j] = xvec[i];
                        }
                    }
                }
            }
            return invmat;
        }

        // Inverse of a square m_dblArr [static method]
        public static MatrixExtended inverse(MatrixExtended amat)
        {
            int n = amat.m_intRows;
            if (n != amat.m_intColumns)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            MatrixExtended invmat = new MatrixExtended(n, n);

            if (n == 1)
            {
                double[,] hold = amat.getArrayCopy();
                if (hold[0, 0] == 0.0)
                {
                    throw new ArgumentException("Matrix_non_stable is singular");
                }
                hold[0, 0] = 1.0/hold[0, 0];
                invmat = new MatrixExtended(hold);
            }
            else
            {
                if (n == 2)
                {
                    double[,] hold = amat.getArrayCopy();
                    double det = hold[0, 0]*hold[1, 1] - hold[0, 1]*hold[1, 0];
                    if (det == 0.0)
                    {
                        throw new ArgumentException("Matrix_non_stable is singular");
                    }
                    double[,] hold2 = new double[2,2];
                    hold2[0, 0] = hold[1, 1]/det;
                    hold2[1, 1] = hold[0, 0]/det;
                    hold2[1, 0] = -hold[1, 0]/det;
                    hold2[0, 1] = -hold[0, 1]/det;
                    invmat = new MatrixExtended(hold2);
                }
                else
                {
                    double[] col = new double[n];
                    double[] xvec = new double[n];
                    double[,] invarray = invmat.getArrayReference();
                    MatrixExtended ludmat;

                    ludmat = amat.luDecomp();
                    for (int j = 0; j < n; j++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            col[i] = 0.0D;
                        }
                        col[j] = 1.0;
                        xvec = ludmat.luBackSub(col);
                        for (int i = 0; i < n; i++)
                        {
                            invarray[i, j] = xvec[i];
                        }
                    }
                }
            }
            return invmat;
        }

        // TRANSPOSE
        // Transpose of a m_dblArr [instance method]
        public MatrixExtended transpose()
        {
            MatrixExtended tmat = new MatrixExtended(m_intColumns, m_intRows);
            double[,] tarray = tmat.getArrayReference();
            for (int i = 0; i < m_intColumns; i++)
            {
                for (int j = 0; j < m_intRows; j++)
                {
                    tarray[i, j] = m_dblArr[j, i];
                }
            }
            return tmat;
        }

        // Transpose of a m_dblArr [static method]
        public static MatrixExtended transpose(MatrixExtended amat)
        {
            MatrixExtended tmat = new MatrixExtended(amat.m_intColumns, amat.m_intRows);
            double[,] tarray = tmat.getArrayReference();
            for (int i = 0; i < amat.m_intColumns; i++)
            {
                for (int j = 0; j < amat.m_intRows; j++)
                {
                    tarray[i, j] = amat.m_dblArr[j, i];
                }
            }
            return tmat;
        }

        // OPPOSITE
        // Opposite of a m_dblArr [instance method]
        public MatrixExtended opposite()
        {
            MatrixExtended opp = Copy(this);
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    opp.m_dblArr[i, j] = -m_dblArr[i, j];
                }
            }
            return opp;
        }

        // Opposite of a m_dblArr [static method]
        public static MatrixExtended opposite(MatrixExtended amat)
        {
            MatrixExtended opp = Copy(amat);
            for (int i = 0; i < amat.m_intRows; i++)
            {
                for (int j = 0; j < amat.m_intColumns; j++)
                {
                    opp.m_dblArr[i, j] = -amat.m_dblArr[i, j];
                }
            }
            return opp;
        }

        // TRACE
        // Trace of a  m_dblArr [instance method]
        // Trace of a m_dblArr [static method]
        public static double trace(MatrixExtended amat)
        {
            double trac = 0.0D;
            for (int i = 0; i < Math.Min(amat.m_intColumns, amat.m_intColumns); i++)
            {
                trac += amat.m_dblArr[i, i];
            }
            return trac;
        }

        // DETERMINANT
        //  Returns the determinant of a square m_dblArr [instance method]
        public double determinant()
        {
            int n = m_intRows;
            if (n != m_intColumns)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            double det = 0.0D;
            if (n == 2)
            {
                det = m_dblArr[0, 0]*m_dblArr[1, 1] - m_dblArr[0, 1]*m_dblArr[1, 0];
            }
            else
            {
                MatrixExtended ludmat = luDecomp();
                det = ludmat.m_rowSwapIndex;
                for (int j = 0; j < n; j++)
                {
                    det *= ludmat.m_dblArr[j, j];
                }
            }
            return det;
        }

        //  Returns the determinant of a square m_dblArr [static method] - Matrix_non_stable input
        public static double determinant(MatrixExtended amat)
        {
            int n = amat.m_intRows;
            if (n != amat.m_intColumns)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            double det = 0.0D;

            if (n == 2)
            {
                double[,] hold = amat.getArrayCopy();
                det = hold[0, 0]*hold[1, 1] - hold[0, 1]*hold[1, 0];
            }
            else
            {
                MatrixExtended ludmat = amat.luDecomp();
                det = ludmat.m_rowSwapIndex;
                for (int j = 0; j < n; j++)
                {
                    det *= (ludmat.m_dblArr[j, j]);
                }
            }
            return det;
        }

        //  Returns the determinant of a square m_dblArr [static method] - [,] array input
        public static double determinant(double[,] mat)
        {
            int n = mat.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                if (n != mat.GetLength(1))
                {
                    throw new ArgumentException("Matrix_non_stable is not square");
                }
            }
            double det = 0.0D;

            if (n == 2)
            {
                det = mat[0, 0]*mat[1, 1] - mat[0, 1]*mat[1, 0];
            }
            else
            {
                MatrixExtended amat = new MatrixExtended(mat);
                MatrixExtended ludmat = amat.luDecomp();
                det = ludmat.m_rowSwapIndex;
                for (int j = 0; j < n; j++)
                {
                    det *= (ludmat.m_dblArr[j, j]);
                }
            }
            return det;
        }

        // Returns the Log(determinant) of a square m_dblArr [instance method].
        // Useful if determinant() underflows or overflows.
        public double logDeterminant()
        {
            int n = m_intRows;
            if (n != m_intColumns)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            double det = 0.0D;
            MatrixExtended ludmat = luDecomp();

            det = ludmat.m_rowSwapIndex;
            det = Math.Log(det);
            for (int j = 0; j < n; j++)
            {
                det += Math.Log(ludmat.m_dblArr[j, j]);
            }
            return det;
        }

        // Returns the Log(determinant) of a square m_dblArr [static method] - m_dblArr input.
        // Useful if determinant() underflows or overflows.
        public static double logDeterminant(MatrixExtended amat)
        {
            int n = amat.m_intRows;
            if (n != amat.m_intColumns)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            double det = 0.0D;
            MatrixExtended ludmat = amat.luDecomp();

            det = ludmat.m_rowSwapIndex;
            det = Math.Log(det);
            for (int j = 0; j < n; j++)
            {
                det += Math.Log(ludmat.m_dblArr[j, j]);
            }
            return det;
        }

        // Returns the Log(determinant) of a square m_dblArr [static method] double[,] input.
        // Useful if determinant() underflows or overflows.
        public static double logDeterminant(double[,] mat)
        {
            int n = mat.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                if (n != mat.GetLength(1))
                {
                    throw new ArgumentException("Matrix_non_stable is not square");
                }
            }
            MatrixExtended amat = new MatrixExtended(mat);
            return amat.determinant();
        }

        // REDUCED ROW ECHELON FORM
        public MatrixExtended reducedRowEchelonForm()
        {
            double[,] mat = new double[m_intRows,m_intColumns];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    mat[i, j] = m_dblArr[i, j];
                }
            }

            int leadingCoeff = 0;
            int rowPointer = 0;

            bool testOuter = true;
            while (testOuter)
            {
                int counter = rowPointer;
                bool testInner = true;
                while (testInner && mat[counter, leadingCoeff] == 0)
                {
                    counter++;
                    if (counter == m_intRows)
                    {
                        counter = rowPointer;
                        leadingCoeff++;
                        if (leadingCoeff == m_intColumns)
                        {
                            testInner = false;
                        }
                    }
                }
                if (testInner)
                {
                    double[] temp = getRowCopy(rowPointer);
                    for (int k = 0; k < m_intRows; k++)
                    {
                        mat[rowPointer, k] = mat[counter, k];
                        mat[counter, k] = temp[k];
                    }

                    double pivot = mat[rowPointer, leadingCoeff];

                    for (int j = 0; j < m_intColumns; j++)
                    {
                        mat[rowPointer, j] /= pivot;
                    }

                    for (int i = 0; i < m_intRows; i++)
                    {
                        if (i != rowPointer)
                        {
                            pivot = mat[i, leadingCoeff];
                            for (int j = 0; j < m_intColumns; j++)
                            {
                                mat[i, j] -= pivot*mat[rowPointer, j];
                            }
                        }
                    }
                    leadingCoeff++;
                    if (leadingCoeff >= m_intColumns)
                    {
                        testOuter = false;
                    }
                }
                rowPointer++;
                if (rowPointer >= m_intRows || !testInner)
                {
                    testOuter = false;
                }
            }

            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (mat[i, j] == -0.0)
                    {
                        mat[i, j] = 0.0;
                    }
                }
            }

            return new MatrixExtended(mat);
        }

        // FROBENIUS NORM of a m_dblArr
        // Sometimes referred to as the EUCLIDEAN NORM
        public double frobeniusNorm()
        {
            double norm = 0.0D;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    norm = hypot(norm, Math.Abs(m_dblArr[i, j]));
                }
            }
            return norm;
        }


        // ONE NORM of a m_dblArr
        public double oneNorm()
        {
            double norm = 0.0D;
            double sum = 0.0D;
            for (int i = 0; i < m_intRows; i++)
            {
                sum = 0.0D;
                for (int j = 0; j < m_intColumns; j++)
                {
                    sum += Math.Abs(m_dblArr[i, j]);
                }
                norm = Math.Max(norm, sum);
            }
            return norm;
        }

        // INFINITY NORM of a m_dblArr
        public double infinityNorm()
        {
            double norm = 0.0D;
            double sum = 0.0D;
            for (int i = 0; i < m_intRows; i++)
            {
                sum = 0.0D;
                for (int j = 0; j < m_intColumns; j++)
                {
                    sum += Math.Abs(m_dblArr[i, j]);
                }
                norm = Math.Max(norm, sum);
            }
            return norm;
        }

        // SUM OF THE ELEMENTS
        // Returns sum of all elements
        public double sum()
        {
            double sum = 0.0;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    sum += m_dblArr[i, j];
                }
            }
            return sum;
        }

        // Returns sums of the rows
        public double[] rowSums()
        {
            double[] sums = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                sums[i] = 0.0;
                for (int j = 0; j < m_intColumns; j++)
                {
                    sums[i] += m_dblArr[i, j];
                }
            }
            return sums;
        }

        // Returns sums of the columns
        public double[] columnSums()
        {
            double[] sums = new double[m_intColumns];
            for (int i = 0; i < m_intColumns; i++)
            {
                sums[i] = 0.0;
                for (int j = 0; j < m_intRows; j++)
                {
                    sums[i] += m_dblArr[j, i];
                }
            }
            return sums;
        }


        // MEAN OF THE ELEMENTS
        // Returns m_mean of all elements
        public double mean()
        {
            double mean = 0.0;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    mean += m_dblArr[i, j];
                }
            }
            mean /= m_intRows*m_intColumns;
            return mean;
        }

        // Returns means of the rows
        public double[] rowMeans()
        {
            double[] means = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                means[i] = 0.0;
                for (int j = 0; j < m_intColumns; j++)
                {
                    means[i] += m_dblArr[i, j];
                }
                means[i] /= m_intColumns;
            }
            return means;
        }

        // Returns means of the columns
        public double[] columnMeans()
        {
            double[] means = new double[m_intColumns];
            for (int i = 0; i < m_intColumns; i++)
            {
                means[i] = 0.0;
                for (int j = 0; j < m_intRows; j++)
                {
                    means[i] += m_dblArr[j, i];
                }
                means[i] /= m_intRows;
            }
            return means;
        }

        // SUBTRACT THE MEAN OF THE ELEMENTS
        // Returns a m_dblArr whose elements are the original elements minus the m_mean of all elements
        public MatrixExtended subtractMean()
        {
            MatrixExtended mat = new MatrixExtended(m_intRows, m_intColumns);

            double mean = 0.0;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    mean += m_dblArr[i, j];
                }
            }
            mean /= m_intRows*m_intColumns;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    mat.m_dblArr[i, j] = m_dblArr[i, j] - mean;
                }
            }
            return mat;
        }

        // Returns a m_dblArr whose rows are the elements are the original row minus the m_mean of the elements of that row
        public MatrixExtended subtractRowMeans()
        {
            MatrixExtended mat = new MatrixExtended(m_intRows, m_intColumns);

            for (int i = 0; i < m_intRows; i++)
            {
                double mean = 0.0;
                for (int j = 0; j < m_intColumns; j++)
                {
                    mean += m_dblArr[i, j];
                }
                mean /= m_intColumns;
                for (int j = 0; j < m_intColumns; j++)
                {
                    mat.m_dblArr[i, j] = m_dblArr[i, j] - mean;
                }
            }
            return mat;
        }

        // Returns m_dblArr whose columns are the elements are the original column minus the m_mean of the elements of that olumnc
        public MatrixExtended subtractColumnMeans()
        {
            MatrixExtended mat = new MatrixExtended(m_intRows, m_intColumns);

            for (int i = 0; i < m_intColumns; i++)
            {
                double mean = 0.0;
                for (int j = 0; j < m_intRows; j++)
                {
                    mean += m_dblArr[j, i];
                }
                mean /= m_intRows;
                for (int j = 0; j < m_intRows; j++)
                {
                    mat.m_dblArr[j, i] = m_dblArr[j, i] - mean;
                }
            }
            return mat;
        }


        // MEDIAN OF THE ELEMENTS
        // Returns median of all elements
        public double median()
        {
            Stat st = new Stat(
                getRowCopy(0));

            for (int i = 1; i < m_intRows; i++)
            {
                st.concatenate(
                    getRowCopy(i));
            }
            return st.median();
        }

        // Returns medians of the rows
        public double[] rowMedians()
        {
            double[] medians = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(
                    ArrayHelper.GetRowCopy<double>(
                        m_dblArr,
                        i));
                medians[i] = st.median();
            }

            return medians;
        }

        // Returns medians of the columns
        public double[] columnMedians()
        {
            double[] medians = new double[m_intRows];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                medians[i] = st.median();
            }

            return medians;
        }

        // SET THE DENOMINATOR OF THE VARIANCES AND STANDARD DEVIATIONS TO NUMBER OF ELEMENTS, n
        // Default value = n-1
        public void setDenominatorToN()
        {
            Stat.setStaticDenominatorToN();
        }


        // VARIANCE OF THE ELEMENTS
        // Returns variance of all elements
        public double variance()
        {
            Stat st = new Stat(
                getRowCopy(0));

            for (int i = 1; i < m_intRows; i++)
            {
                st.concatenate(
                    getRowCopy(i));
            }

            return st.variance();
        }

        // Returns variances of the rows
        public double[] rowVariances()
        {
            double[] variances = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(
                    getRowCopy(i));
                variances[i] = st.variance();
            }

            return variances;
        }

        // Returns variances of the columns
        public double[] columnVariances()
        {
            double[] variances = new double[m_intColumns];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                variances[i] = st.variance();
            }

            return variances;
        }


        // STANDARD DEVIATION OF THE ELEMENTS
        // Returns standard deviation of all elements
        public double standardDeviation()
        {
            Stat st = new Stat(getRowCopy(0));

            for (int i = 1; i < m_intRows; i++)
            {
                st.concatenate(getRowCopy(i));
            }

            return st.standardDeviation();
        }

        // Returns standard deviations of the rows
        public double[] rowStandardDeviations()
        {
            double[] standardDeviations = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(getRowCopy(i));
                standardDeviations[i] = st.standardDeviation();
            }

            return standardDeviations;
        }

        // Returns standard deviations of the columns
        public double[] columnStandardDeviations()
        {
            double[] standardDeviations = new double[m_intColumns];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                standardDeviations[i] = st.standardDeviation();
            }

            return standardDeviations;
        }


        // STANDARD ERROR OF THE ELEMENTS
        // Returns standard error of all elements
        public double stanadardError()
        {
            Stat st = new Stat(getRowCopy(0));

            for (int i = 1; i < m_intRows; i++)
            {
                st.concatenate(getRowCopy(i));
            }

            return st.standardError();
        }

        // Returns standard errors of the rows
        public double[] rowStandardErrors()
        {
            double[] standardErrors = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(getRowCopy(i));
                standardErrors[i] = st.standardError();
            }

            return standardErrors;
        }

        // Returns standard errors of the columns
        public double[] columnStandardErrors()
        {
            double[] standardErrors = new double[m_intRows];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                standardErrors[i] = st.standardError();
            }

            return standardErrors;
        }


        // MAXIMUM ELEMENT
        // Returns the value, row index and column index of the maximum element
        public double[] maximumElement()
        {
            double[] ret = new double[3];
            double[] holdD = new double[m_intRows];
            ArrayMaths am = null;
            int[] holdI = new int[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                am = new ArrayMaths(getRowCopy(i));
                holdD[i] = am.maximum();
                holdI[i] = am.maximumIndex();
            }
            am = new ArrayMaths(holdD);
            ret[0] = am.maximum();
            int maxI = am.maximumIndex();
            ret[1] = maxI;
            ret[2] = holdI[maxI];

            return ret;
        }

        // Returns maxima of the rows
        public double[] rowMaxima()
        {
            double[] maxima = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(getRowCopy(i));
                maxima[i] = st.maximum();
            }

            return maxima;
        }

        // Returns maxima of the columns
        public double[] columnMaxima()
        {
            double[] maxima = new double[m_intRows];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                maxima[i] = st.maximum();
            }

            return maxima;
        }

        // MINIMUM ELEMENT
        // Returns the value, row index and column index of the minimum element
        public double[] minimumElement()
        {
            double[] ret = new double[3];
            double[] holdD = new double[m_intRows];
            ArrayMaths am = null;
            int[] holdI = new int[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                am = new ArrayMaths(getRowCopy(i));
                holdD[i] = am.minimum();
                holdI[i] = am.minimumIndex();
            }
            am = new ArrayMaths(holdD);
            ret[0] = am.minimum();
            int minI = am.minimumIndex();
            ret[1] = minI;
            ret[2] = holdI[minI];

            return ret;
        }

        // Returns minima of the rows
        public double[] rowMinima()
        {
            double[] minima = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(getRowCopy(i));
                minima[i] = st.minimum();
            }

            return minima;
        }

        // Returns minima of the columns
        public double[] columnMinima()
        {
            double[] minima = new double[m_intRows];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                minima[i] = st.minimum();
            }

            return minima;
        }

        // RANGE
        // Returns the range of all the elements
        public double range()
        {
            return maximumElement()[0] - minimumElement()[0];
        }

        // Returns ranges of the rows
        public double[] rowRanges()
        {
            double[] ranges = new double[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                Stat st = new Stat(getRowCopy(i));
                ranges[i] = st.maximum() - st.minimum();
            }

            return ranges;
        }

        // Returns ranges of the columns
        public double[] columnRanges()
        {
            double[] ranges = new double[m_intRows];
            for (int i = 0; i < m_intColumns; i++)
            {
                double[] hold = new double[m_intRows];
                for (int j = 0; j < m_intRows; j++)
                {
                    hold[i] = m_dblArr[j, i];
                }
                Stat st = new Stat(hold);
                ranges[i] = st.maximum() - st.minimum();
            }

            return ranges;
        }

        // PIVOT
        // Swaps rows and columns to place absolute maximum element in positiom m_dblArr[0,0]
        public int[] pivot()
        {
            double[] max = maximumElement();
            int maxI = (int) max[1];
            int maxJ = (int) max[2];
            double[] min = minimumElement();
            int minI = (int) min[1];
            int minJ = (int) min[2];
            if (Math.Abs(min[0]) > Math.Abs(max[0]))
            {
                maxI = minI;
                maxJ = minJ;
            }
            int[] ret = {maxI, maxJ};

            ArrayHelper.SwapRowFromArray(
                m_dblArr,
                0,
                maxI);

            double hold2 = 0.0;
            for (int i = 0; i < m_intRows; i++)
            {
                hold2 = m_dblArr[i, 0];
                m_dblArr[i, 0] = m_dblArr[i, maxJ];
                m_dblArr[i, maxJ] = hold2;
            }

            return ret;
        }

        // MATRIX TESTS

        // Check if a m_dblArr is square
        public bool isSquare()
        {
            bool test = false;
            if (m_intRows == m_intColumns)
            {
                test = true;
            }
            return test;
        }

        // Check if a m_dblArr is symmetric
        public bool isSymmetric()
        {
            bool test = true;
            if (m_intRows == m_intColumns)
            {
                for (int i = 0; i < m_intRows; i++)
                {
                    for (int j = i + 1; j < m_intColumns; j++)
                    {
                        if (m_dblArr[i, j] != m_dblArr[j, i])
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                test = false;
            }
            return test;
        }

        // Check if a m_dblArr is zero
        public bool isZero()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is unit
        public bool isUnit()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (m_dblArr[i, j] != 1.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is diagonal
        public bool isDiagonal()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i != j && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is upper triagonal
        public bool isUpperTriagonal()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (j < i && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is lower triagonal
        public bool isLowerTriagonal()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i > j && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is tridiagonal
        public bool isTridiagonal()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i < (j + 1) && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                    if (j > (i + 1) && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is upper Hessenberg
        public bool isUpperHessenberg()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (j < (i + 1) && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is lower Hessenberg
        public bool isLowerHessenberg()
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i > (j + 1) && m_dblArr[i, j] != 0.0D)
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is a identity m_dblArr
        public bool isIdentity()
        {
            bool test = true;
            if (m_intRows == m_intColumns)
            {
                for (int i = 0; i < m_intRows; i++)
                {
                    if (m_dblArr[i, i] != 1.0D)
                    {
                        test = false;
                    }
                    for (int j = i + 1; j < m_intColumns; j++)
                    {
                        if (m_dblArr[i, j] != 0.0D)
                        {
                            test = false;
                        }
                        if (m_dblArr[j, i] != 0.0D)
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                test = false;
            }
            return test;
        }

        // Check if a m_dblArr is symmetric within a given tolerance
        public bool isNearlySymmetric(double tolerance)
        {
            bool test = true;
            if (m_intRows == m_intColumns)
            {
                for (int i = 0; i < m_intRows; i++)
                {
                    for (int j = i + 1; j < m_intColumns; j++)
                    {
                        if (Math.Abs(m_dblArr[i, j] - m_dblArr[j, i]) > Math.Abs(tolerance))
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                test = false;
            }
            return test;
        }

        // Check if a m_dblArr is zero within a given tolerance
        public bool isNearlyZero(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (Math.Abs(m_dblArr[i, j]) > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is unit within a given tolerance
        public bool isNearlyUnit(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (Math.Abs(m_dblArr[i, j] - 1.0D) > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }


        // Check if a m_dblArr is upper triagonal within a given tolerance
        public bool isNearlyUpperTriagonal(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (j < i && m_dblArr[i, j] > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is lower triagonal within a given tolerance
        public bool isNearlyLowerTriagonal(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i > j && m_dblArr[i, j] > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }


        // Check if a m_dblArr is an identy m_dblArr within a given tolerance
        public bool isNearlyIdenty(double tolerance)
        {
            bool test = true;
            if (m_intRows == m_intColumns)
            {
                for (int i = 0; i < m_intRows; i++)
                {
                    if (Math.Abs(m_dblArr[i, i] - 1.0D) > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                    for (int j = i + 1; j < m_intColumns; j++)
                    {
                        if (m_dblArr[i, j] > Math.Abs(tolerance))
                        {
                            test = false;
                        }
                        if (m_dblArr[j, i] > Math.Abs(tolerance))
                        {
                            test = false;
                        }
                    }
                }
            }
            else
            {
                test = false;
            }
            return test;
        }

        // Check if a m_dblArr is tridiagonal within a given tolerance
        public bool isTridiagonal(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i < (j + 1) && m_dblArr[i, j] > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                    if (j > (i + 1) && m_dblArr[i, j] > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is upper Hessenberg within a given tolerance
        public bool isNearlyUpperHessenberg(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (j < (i + 1) && m_dblArr[i, j] > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // Check if a m_dblArr is lower Hessenberg within a given tolerance
        public bool isNearlyLowerHessenberg(double tolerance)
        {
            bool test = true;
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intColumns; j++)
                {
                    if (i > (j + 1) && m_dblArr[i, j] > Math.Abs(tolerance))
                    {
                        test = false;
                    }
                }
            }
            return test;
        }

        // LU DECOMPOSITION OF MATRIX A
        // For details of LU decomposition
        // See Numerical Recipes, The Art of Scientific Computing
        // by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
        // Cambridge University Press,   http://www.nr.com/
        // This method has followed their approach but modified to an object oriented language
        // Matrix_non_stable ludmat is the returned LU decompostion
        // int[] index is the vector of row permutations
        // m_rowSwapIndex returns +1.0 for even number of row interchanges
        //       returns -1.0 for odd number of row interchanges
        public MatrixExtended luDecomp()
        {
            if (m_intRows != m_intColumns)
            {
                throw new ArgumentException("A m_dblArr is not square");
            }
            int n = m_intRows;
            int imax = 0;
            double dum = 0.0D, temp = 0.0D, big = 0.0D;
            double[] vv = new double[n];
            double sum = 0.0D;
            double dumm = 0.0D;

            m_matrixCheck = true;

            MatrixExtended ludmat = Copy(this);
            double[,] ludarray = ludmat.getArrayReference();

            ludmat.m_rowSwapIndex = 1.0D;
            for (int i = 0; i < n; i++)
            {
                big = 0.0D;
                for (int j = 0; j < n; j++)
                {
                    if ((temp = Math.Abs(ludarray[i, j])) > big)
                    {
                        big = temp;
                    }
                }
                if (big == 0.0D)
                {
                    if (!m_blnSupressErrorMessage)
                    {
                        PrintToScreen.WriteLine(
                            "Attempted LU Decomposition of a singular m_dblArr in Matrix_non_stable.luDecomp()");
                        PrintToScreen.WriteLine("NaN m_dblArr returned and m_matrixCheck set to false");
                    }
                    m_matrixCheck = false;
                    for (int k = 0; k < n; k++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            ludarray[k, j] = double.NaN;
                        }
                    }
                    return ludmat;
                }
                vv[i] = 1.0/big;
            }
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < j; i++)
                {
                    sum = ludarray[i, j];
                    for (int k = 0; k < i; k++)
                    {
                        sum -= ludarray[i, k]*ludarray[k, j];
                    }
                    ludarray[i, j] = sum;
                }
                big = 0.0D;
                for (int i = j; i < n; i++)
                {
                    sum = ludarray[i, j];
                    for (int k = 0; k < j; k++)
                    {
                        sum -= ludarray[i, k]*ludarray[k, j];
                    }
                    ludarray[i, j] = sum;
                    if ((dum = vv[i]*Math.Abs(sum)) >= big)
                    {
                        big = dum;
                        imax = i;
                    }
                }
                if (j != imax)
                {
                    for (int k = 0; k < n; k++)
                    {
                        dumm = ludarray[imax, k];
                        ludarray[imax, k] = ludarray[j, k];
                        ludarray[j, k] = dumm;
                    }
                    ludmat.m_rowSwapIndex = -ludmat.m_rowSwapIndex;
                    vv[imax] = vv[j];
                }
                ludmat.m_permutationIndex[j] = imax;

                if (ludarray[j, j] == 0.0D)
                {
                    ludarray[j, j] = m_tiny;
                }
                if (j != n - 1)
                {
                    dumm = 1.0/ludarray[j, j];
                    for (int i = j + 1; i < n; i++)
                    {
                        ludarray[i, j] *= dumm;
                    }
                }
            }
            return ludmat;
        }

        // Solves the set of n linear equations A.X=B using not A but its LU decomposition
        // bvec is the vector B (input)
        // xvec is the vector X (output)
        // index is the permutation vector produced by luDecomp()
        public double[] luBackSub(double[] bvec)
        {
            int ii = 0, ip = 0;
            int n = bvec.Length;
            if (n != m_intColumns)
            {
                throw new ArgumentException("vector length is not equal to m_dblArr dimension");
            }
            if (m_intColumns != m_intRows)
            {
                throw new ArgumentException("m_dblArr is not square");
            }
            double sum = 0.0D;
            double[] xvec = new double[n];
            for (int i = 0; i < n; i++)
            {
                xvec[i] = bvec[i];
            }
            for (int i = 0; i < n; i++)
            {
                ip = m_permutationIndex[i];
                sum = xvec[ip];
                xvec[ip] = xvec[i];
                if (ii == 0)
                {
                    for (int j = ii; j <= i - 1; j++)
                    {
                        sum -= m_dblArr[i, j]*xvec[j];
                    }
                }
                else
                {
                    if (sum == 0.0)
                    {
                        ii = i;
                    }
                }
                xvec[i] = sum;
            }
            for (int i = n - 1; i >= 0; i--)
            {
                sum = xvec[i];
                for (int j = i + 1; j < n; j++)
                {
                    sum -= m_dblArr[i, j]*xvec[j];
                }
                xvec[i] = sum/m_dblArr[i, i];
            }
            return xvec;
        }

        // Solves the set of n linear equations A.X=B
        // bvec is the vector B (input)
        // xvec is the vector X (output)
        public double[] solveLinearSet(double[] bvec)
        {
            double[] xvec = null;
            if (m_intRows == m_intColumns)
            {
                // square m_dblArr - LU decomposition used
                MatrixExtended ludmat = luDecomp();
                xvec = ludmat.luBackSub(bvec);
            }
            else
            {
                if (m_intRows > m_intColumns)
                {
                    // overdetermined equations - least squares used - must be used with care
                    int n = bvec.Length;
                    if (m_intRows != n)
                    {
                        throw new ArgumentException(
                            "Overdetermined equation solution - vector length is not equal to m_dblArr column length");
                    }
                    MatrixExtended avecT = transpose();
                    double[,] avec = avecT.getArrayCopy();
                    RegressionAn reg = new RegressionAn(avec, bvec);
                    reg.linearGeneral();
                    xvec = reg.getCoeff();
                }
                else
                {
                    throw new ArgumentException("This class does not handle underdetermined equations");
                }
            }
            return xvec;
        }

        //Supress printing of LU decompostion failure message
        public void supressErrorMessage()
        {
            m_blnSupressErrorMessage = true;
        }


        // HESSENBERG MARTIX

        // Calculates the Hessenberg equivalant of this m_dblArr
        public void hessenbergMatrix()
        {
            m_hessenberg = getArrayCopy();
            double pivot = 0.0D;
            int pivotIndex = 0;
            double hold = 0.0D;

            for (int i = 1; i < m_intRows - 1; i++)
            {
                // identify pivot
                pivot = 0.0D;
                pivotIndex = i;
                for (int j = i; j < m_intRows; j++)
                {
                    if (Math.Abs(m_hessenberg[j, i - 1]) > Math.Abs(pivot))
                    {
                        pivot = m_hessenberg[j, i - 1];
                        pivotIndex = j;
                    }
                }

                // row and column interchange
                if (pivotIndex != i)
                {
                    for (int j = i - 1; j < m_intRows; j++)
                    {
                        hold = m_hessenberg[pivotIndex, j];
                        m_hessenberg[pivotIndex, j] = m_hessenberg[i, j];
                        m_hessenberg[i, j] = hold;
                    }
                    for (int j = 0; j < m_intRows; j++)
                    {
                        hold = m_hessenberg[j, pivotIndex];
                        m_hessenberg[j, pivotIndex] = m_hessenberg[j, i];
                        m_hessenberg[j, i] = hold;
                    }

                    // elimination
                    if (pivot != 0.0)
                    {
                        for (int j = i + 1; j < m_intRows; j++)
                        {
                            hold = m_hessenberg[j, i - 1];
                            if (hold != 0.0)
                            {
                                hold /= pivot;
                                m_hessenberg[j, i - 1] = hold;
                                for (int k = i; k < m_intRows; k++)
                                {
                                    m_hessenberg[j, k] -= hold*m_hessenberg[i, k];
                                }
                                for (int k = 0; k < m_intRows; k++)
                                {
                                    m_hessenberg[k, i] += hold*m_hessenberg[k, j];
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 2; i < m_intRows; i++)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    m_hessenberg[i, j] = 0.0;
                }
            }
            m_hessenbergDone = true;
        }

        // return the Hessenberg equivalent
        public double[,] getHessenbergMatrix()
        {
            if (!m_hessenbergDone)
            {
                hessenbergMatrix();
            }
            return m_hessenberg;
        }


        // EIGEN VALUES AND EIGEN VECTORS
        // For a discussion of eigen systems see
        // Numerical Recipes, The Art of Scientific Computing
        // by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
        // Cambridge University Press,   http://www.nr.com/
        // These methods follow their approach but modified to an object oriented language

        // Return eigen values as calculated
        public double[] getEigenValues()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_eigenValues;
        }

        // Return eigen values in descending order
        public double[] getSortedEigenValues()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_sortedEigenValues;
        }

        // Return eigen vectors as calculated as columns
        // Each vector as a column
        public double[,] getEigenVectorsAsColumns()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_eigenVector;
        }

        // Return eigen vectors as calculated as columns
        // Each vector as a column
        public double[,] getEigenVector()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_eigenVector;
        }

        // Return eigen vectors as calculated as rows
        // Each vector as a row
        public double[,] getEigenVectorsAsRows()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            double[,] ret = new double[m_intRows,m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intRows; j++)
                {
                    ret[i, j] = m_eigenVector[j, i];
                }
            }
            return ret;
        }

        // Return eigen vectors reordered to match a descending order of eigen values
        // Each vector as a column
        public double[,] getSortedEigenVectorsAsColumns()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_sortedEigenVector;
        }

        // Return eigen vectors reordered to match a descending order of eigen values
        // Each vector as a column
        public double[,] getSortedEigenVector()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_sortedEigenVector;
        }

        // Return eigen vectors reordered to match a descending order of eigen values
        // Each vector as a row
        public double[,] getSortedEigenVectorsAsRows()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            double[,] ret = new double[m_intRows,m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                for (int j = 0; j < m_intRows; j++)
                {
                    ret[i, j] = m_sortedEigenVector[j, i];
                }
            }
            return ret;
        }

        // Return the number of rotations used in the Jacobi procedure
        public int getNumberOfJacobiRotations()
        {
            return m_numberOfRotations;
        }

        // Returns the eigen values and eigen vectors of a symmetric m_dblArr
        // Follows the approach of Numerical methods but adapted to object oriented programming (see above)
        private void symmetricEigen()
        {
            if (!isSymmetric())
            {
                throw new ArgumentException("m_dblArr is not symmetric");
            }
            double[,] amat = getArrayCopy();
            m_eigenVector = new double[m_intRows,m_intRows];
            m_eigenValues = new double[m_intRows];
            double threshold = 0.0D;
            double cot2rotationAngle = 0.0D;
            double tanHalfRotationAngle = 0.0D;
            double offDiagonalSum = 0.0D;
            double scaledOffDiagonal = 0.0D;
            double sElement = 0.0D;
            double cElement = 0.0D;
            double sOverC = 0.0D;
            double vectorDifference = 0.0D;
            double[] holdingVector1 = new double[m_intRows];
            double[] holdingVector2 = new double[m_intRows];

            for (int p = 0; p < m_intRows; p++)
            {
                for (int q = 0; q < m_intRows; q++)
                {
                    m_eigenVector[p, q] = 0.0;
                }
                m_eigenVector[p, p] = 1.0;
            }
            for (int p = 0; p < m_intRows; p++)
            {
                holdingVector1[p] = amat[p, p];
                m_eigenValues[p] = amat[p, p];
                holdingVector2[p] = 0.0;
            }
            m_numberOfRotations = 0;
            for (int i = 1; i <= m_maximumJacobiIterations; i++)
            {
                offDiagonalSum = 0.0;
                for (int p = 0; p < m_intRows - 1; p++)
                {
                    for (int q = p + 1; q < m_intRows; q++)
                    {
                        offDiagonalSum += Math.Abs(amat[p, q]);
                    }
                }
                if (offDiagonalSum == 0.0)
                {
                    m_eigenDone = true;
                    eigenSort();
                    return;
                }
                if (i < 4)
                {
                    threshold = 0.2*offDiagonalSum/(m_intRows*m_intRows);
                }
                else
                {
                    threshold = 0.0;
                }
                for (int p = 0; p < m_intRows - 1; p++)
                {
                    for (int q = p + 1; q < m_intRows; q++)
                    {
                        scaledOffDiagonal = 100.0*Math.Abs(amat[p, q]);
                        if (i > 4 && (Math.Abs(m_eigenValues[p]) + scaledOffDiagonal) == Math.Abs(m_eigenValues[p]) &&
                            (Math.Abs(m_eigenValues[q]) + scaledOffDiagonal) == Math.Abs(m_eigenValues[q]))
                        {
                            amat[p, q] = 0.0;
                        }
                        else if (Math.Abs(amat[p, q]) > threshold)
                        {
                            vectorDifference = m_eigenValues[q] - m_eigenValues[p];
                            if ((Math.Abs(vectorDifference) + scaledOffDiagonal) == Math.Abs(vectorDifference))
                            {
                                sOverC = amat[p, q]/vectorDifference;
                            }
                            else
                            {
                                cot2rotationAngle = 0.5*vectorDifference/amat[p, q];
                                sOverC = 1.0/
                                         (Math.Abs(cot2rotationAngle) +
                                          Math.Sqrt(1.0 + cot2rotationAngle*cot2rotationAngle));
                                if (cot2rotationAngle < 0.0)
                                {
                                    sOverC = -sOverC;
                                }
                            }
                            cElement = 1.0/Math.Sqrt(1.0 + sOverC*sOverC);
                            sElement = sOverC*cElement;
                            tanHalfRotationAngle = sElement/(1.0 + cElement);
                            vectorDifference = sOverC*amat[p, q];
                            holdingVector2[p] -= vectorDifference;
                            holdingVector2[q] += vectorDifference;
                            m_eigenValues[p] -= vectorDifference;
                            m_eigenValues[q] += vectorDifference;
                            amat[p, q] = 0.0;
                            for (int j = 0; j <= p - 1; j++)
                            {
                                rotation(amat, tanHalfRotationAngle, sElement, j, p, j, q);
                            }
                            for (int j = p + 1; j <= q - 1; j++)
                            {
                                rotation(amat, tanHalfRotationAngle, sElement, p, j, j, q);
                            }
                            for (int j = q + 1; j < m_intRows; j++)
                            {
                                rotation(amat, tanHalfRotationAngle, sElement, p, j, q, j);
                            }
                            for (int j = 0; j < m_intRows; j++)
                            {
                                rotation(m_eigenVector, tanHalfRotationAngle, sElement, j, p, j, q);
                            }
                            ++m_numberOfRotations;
                        }
                    }
                }
                for (int p = 0; p < m_intRows; p++)
                {
                    holdingVector1[p] += holdingVector2[p];
                    m_eigenValues[p] = holdingVector1[p];
                    holdingVector2[p] = 0.0;
                }
            }
            PrintToScreen.WriteLine("Maximum iterations, " + m_maximumJacobiIterations +
                                    ", reached - values at this point returned");
            m_eigenDone = true;
            eigenSort();
        }

        // m_dblArr rotaion required by symmetricEigen
        private void rotation(double[,] a, double tau, double sElement, int i, int j, int k, int l)
        {
            double aHold1 = a[i, j];
            double aHold2 = a[k, l];
            a[i, j] = aHold1 - sElement*(aHold2 + aHold1*tau);
            a[k, l] = aHold2 + sElement*(aHold1 - aHold2*tau);
        }

        // Sorts eigen values into descending order and rearranges eigen vecors to match
        // follows Numerical Recipes (see above)
        private void eigenSort()
        {
            int k = 0;
            double holdingElement;
            m_sortedEigenValues = (double[]) m_eigenValues.Clone();
            m_sortedEigenVector = (double[,]) m_eigenVector.Clone();
            m_eigenIndices = new int[m_intRows];

            for (int i = 0; i < m_intRows - 1; i++)
            {
                holdingElement = m_sortedEigenValues[i];
                k = i;
                for (int j = i + 1; j < m_intRows; j++)
                {
                    if (m_sortedEigenValues[j] >= holdingElement)
                    {
                        holdingElement = m_sortedEigenValues[j];
                        k = j;
                    }
                }
                if (k != i)
                {
                    m_sortedEigenValues[k] = m_sortedEigenValues[i];
                    m_sortedEigenValues[i] = holdingElement;

                    for (int j = 0; j < m_intRows; j++)
                    {
                        holdingElement = m_sortedEigenVector[j, i];
                        m_sortedEigenVector[j, i] = m_sortedEigenVector[j, k];
                        m_sortedEigenVector[j, k] = holdingElement;
                    }
                }
            }
            m_eigenIndices = new int[m_intRows];
            for (int i = 0; i < m_intRows; i++)
            {
                bool test = true;
                int j = 0;
                while (test)
                {
                    if (m_sortedEigenValues[i] == m_eigenValues[j])
                    {
                        m_eigenIndices[i] = j;
                        test = false;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
        }

        // Return indices of the eigen values before sorting into descending order
        public int[] eigenValueIndices()
        {
            if (!m_eigenDone)
            {
                symmetricEigen();
            }
            return m_eigenIndices;
        }


        // Method not in java.lang.maths required in this Class
        // See Fmath.class for public versions of this method
        private static double hypot(double aa, double bb)
        {
            double cc = 0.0D, ratio = 0.0D;
            double amod = Math.Abs(aa);
            double bmod = Math.Abs(bb);

            if (amod == 0.0D)
            {
                cc = bmod;
            }
            else
            {
                if (bmod == 0.0D)
                {
                    cc = amod;
                }
                else
                {
                    if (amod <= bmod)
                    {
                        ratio = amod/bmod;
                        cc = bmod*Math.Sqrt(1.0D + ratio*ratio);
                    }
                    else
                    {
                        ratio = bmod/amod;
                        cc = amod*Math.Sqrt(1.0D + ratio*ratio);
                    }
                }
            }
            return cc;
        }
    }
}
