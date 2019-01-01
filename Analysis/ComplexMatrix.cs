#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Complex;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Analysis
{
/*
*   Class   ComplexMatrix
*
*   Defines a complex matrix and includes the methods
*   needed for standard matrix manipulations, e.g. multiplation,
*   and related procedures, e.g. solution of complex linear
*   simultaneous equations
*
*   See class PhasorMatrix for phasor matrix manipulation
*   See class ComplexClass for standard complex arithmetic
*
* 	WRITTEN BY: Dr Michael Thomas Flanagan
*
*   DATE:	    June 2002
*   UPDATES:    16 February 2006 Set methods corrected thanks to Myriam Servi?res, Equipe IVC , Ecole Polytechnique de l'universit? de Nantes, Laboratoire IRCCyN/UMR CNRS
*               7 March 2006
*               31 March 2006 Norm methods corrected thanks to Jinshan Wu, University of British Columbia
*               22 April 2006 getSubMatrix corrected thanks to Joachim Wesner
*               1 July 2007 dividsion and extra conversion methods added
*               19 April 2008 rowMatrix and columnMatrix added
*               8 October 2008 inverse methods updated
*               16 June 2009 timesEquals corrected thanks to Bjorn Nordstom, Ericsonn
*               5 November 2009 Reduced Row Echelon Form added
*               12 January 2010 SetSubMatrix method corrected thanks to Dr. Kay Nehrke, Philips Technologie, Hamburg
*
*
*   DOCUMENTATION:
*   See Michael Thomas Flanagan's Java library on-line web pages:
*   http://www.ee.ucl.ac.uk/~mflanaga/java/ComplexMatrix.html
*   http://www.ee.ucl.ac.uk/~mflanaga/java/
*
*   Copyright (c) 2002 - 2010 Michael Thomas Flanagan

*
*   PERMISSION TO COPY:
*   Permission to use, copy and modify this software and its documentation for
*   NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
*   to the author, Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all copies.
*
*   Dr Michael Thomas Flanagan makes no representations about the suitability
*   or fitness of the software for any or for a particular purpose.
*   Michael Thomas Flanagan shall not be liable for any damages suffered
*   as a result of using, modifying or distributing this software or its derivatives.
*
***************************************************************************************/

    public class ComplexMatrix
    {
        private static double TINY = 1.0e-30;
        private readonly int[] m_index; // row permutation index
        private readonly ComplexClass[,] m_matrix; // 2-D ComplexClass Matrix_non_stable
        private double m_dswap = 1.0D; // row swap index
        private int m_ncol; // number of columns
        private int m_nrow; // number of rows

/*********************************************************/

        // CONSTRUCTORS
        // Construct a nrow x ncol matrix of complex variables all equal to zero
        public ComplexMatrix(int nrow, int ncol)
        {
            m_nrow = nrow;
            m_ncol = ncol;
            m_matrix = ComplexClass.twoDarray(nrow, ncol);
            m_index = new int[nrow];
            for (int i = 0; i < nrow; i++)
            {
                m_index[i] = i;
            }
            m_dswap = 1.0;
        }

        // Construct a nrow x ncol matrix of complex variables all equal to the complex number const
        public ComplexMatrix(int nrow, int ncol, ComplexClass constant)
        {
            m_nrow = nrow;
            m_ncol = ncol;
            m_matrix = ComplexClass.twoDarray(nrow, ncol, constant);
            m_index = new int[nrow];
            for (int i = 0; i < nrow; i++)
            {
                m_index[i] = i;
            }
            m_dswap = 1.0;
        }

        // Construct matrix with a copy of an existing nrow x ncol 2-D array of complex variables
        public ComplexMatrix(ComplexClass[,] twoD)
        {
            m_nrow = twoD.GetLength(0);
            m_ncol = twoD.GetLength(1);
            m_matrix = ComplexClass.twoDarray(m_nrow, m_ncol);
            for (int i = 0; i < m_nrow; i++)
            {
                if (twoD.GetLength(1) != m_ncol)
                {
                    throw new ArgumentException("All rows must have the same length");
                }
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j] = ComplexClass.Copy(twoD[i, j]);
                }
            }
            m_index = new int[m_nrow];
            for (int i = 0; i < m_nrow; i++)
            {
                m_index[i] = i;
            }
            m_dswap = 1.0;
        }

        // Construct matrix with a copy of an existing nrow x ncol 2-D array of double variables
        public ComplexMatrix(double[,] twoD)
        {
            m_nrow = twoD.GetLength(0);
            m_ncol = twoD.GetLength(1);
            for (int i = 0; i < m_nrow; i++)
            {
                if (twoD.GetLength(1) != m_ncol)
                {
                    throw new ArgumentException("All rows must have the same length");
                }
            }
            m_matrix = ComplexClass.twoDarray(m_nrow, m_ncol);
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j] = new ComplexClass(twoD[i, j], 0.0);
                }
            }
            m_index = new int[m_nrow];
            for (int i = 0; i < m_nrow; i++)
            {
                m_index[i] = i;
            }
            m_dswap = 1.0;
        }

        // Construct matrix with a copy of the complex matrix and permutation index of an existing ComplexMatrix bb.
        public ComplexMatrix(ComplexMatrix bb)
        {
            m_nrow = bb.m_nrow;
            m_ncol = bb.m_ncol;
            m_matrix = (bb.Copy()).m_matrix;
            m_index = bb.m_index;
            m_dswap = bb.m_dswap;
        }

        // Construct matrix with a copy of the 2D matrix and permutation index of an existing Matrix_non_stable bb.
        public ComplexMatrix(MatrixExtended bb)
        {
            m_nrow = bb.getNrow();
            m_ncol = bb.getNcol();
            double[,] array = bb.getArrayCopy();
            m_matrix = ComplexClass.twoDarray(m_nrow, m_ncol);
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j] = new ComplexClass(array[i, j], 0.0);
                }
            }
            m_index = bb.getIndexCopy();
            m_dswap = bb.getSwap();
        }


        // SET VALUES
        // Set the matrix with a copy of an existing nrow x ncol 2-D matrix of complex variables
        public void setTwoDarray(ComplexClass[,] aarray)
        {
            if (m_nrow != aarray.GetLength(0))
            {
                throw new ArgumentException(
                    "row length of this ComplexMatrix differs from that of the 2D array argument");
            }
            if (m_ncol != aarray.GetLength(1))
            {
                throw new ArgumentException(
                    "column length of this ComplexMatrix differs from that of the 2D array argument");
            }
            for (int i = 0; i < m_nrow; i++)
            {
                if (aarray.GetLength(1) != m_ncol)
                {
                    throw new ArgumentException("All rows must have the same length");
                }
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j] = ComplexClass.Copy(aarray[i, j]);
                }
            }
        }

        // Set the matrix with a copy of an existing nrow x ncol 2-D matrix of double variables
        public void setTwoDarray(double[,] aarray)
        {
            if (m_nrow != aarray.GetLength(0))
            {
                throw new ArgumentException(
                    "row length of this ComplexMatrix differs from that of the 2D array argument");
            }
            if (m_ncol != aarray.GetLength(1))
            {
                throw new ArgumentException(
                    "column length of this ComplexMatrix differs from that of the 2D array argument");
            }
            for (int i = 0; i < m_nrow; i++)
            {
                if (aarray.GetLength(1) != m_ncol)
                {
                    throw new ArgumentException("All rows must have the same length");
                }
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j] = new ComplexClass(aarray[i, j]);
                }
            }
        }

        // Set an individual array element
        // i = row index
        // j = column index
        // aa = value of the element
        public void setElement(int i, int j, ComplexClass aa)
        {
            m_matrix[i, j] = ComplexClass.Copy(aa);
        }

        // Set an individual array element
        // i = row index
        // j = column index
        // aa = real part of the element
        // bb = imag part of the element
        public void setElement(int i, int j, double aa, double bb)
        {
            m_matrix[i, j].reset(aa, bb);
        }

        // Set a sub-matrix starting with row index i, column index j
        // and ending with row index k, column index l
        // See SetSubMatrix below - this method has ben retained for compatibilityb purposes
        public void setSubMatrix(int i, int j, int k, int l, ComplexClass[,] subMatrix)
        {
            setSubMatrix(i, j, subMatrix);
        }

        // Set a sub-matrix starting with row index i, column index j
        public void setSubMatrix(int i, int j, ComplexClass[,] subMatrix)
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
                    m_matrix[i + p, j + q] = ComplexClass.Copy(subMatrix[m, n]);
                    n++;
                }
                m++;
            }
        }


        // Set a sub-matrix
        // row = array of row indices
        // col = array of column indices
        public void setSubMatrix(int[] row, int[] col, ComplexClass[,] subMatrix)
        {
            int n = row.Length;
            int m = col.Length;
            for (int p = 0; p < n; p++)
            {
                for (int q = 0; q < m; q++)
                {
                    m_matrix[row[p], col[q]] = ComplexClass.Copy(subMatrix[p, q]);
                }
            }
        }


        // SPECIAL MATRICES
        // Construct a complex identity matrix
        public static ComplexMatrix identityMatrix(int nrow)
        {
            ComplexMatrix u = new ComplexMatrix(nrow, nrow);
            for (int i = 0; i < nrow; i++)
            {
                u.m_matrix[i, i] = ComplexClass.plusOne();
            }
            return u;
        }

        // Construct a complex scalar matrix
        public static ComplexMatrix scalarMatrix(int nrow, ComplexClass diagconst)
        {
            ComplexMatrix u = new ComplexMatrix(nrow, nrow);
            ComplexClass[,] uarray = u.getArrayReference();
            for (int i = 0; i < nrow; i++)
            {
                for (int j = i; j < nrow; j++)
                {
                    if (i == j)
                    {
                        uarray[i, j] = ComplexClass.Copy(diagconst);
                    }
                }
            }
            return u;
        }

        // Construct a complex diagonal matrix
        public static ComplexMatrix diagonalMatrix(int nrow, ComplexClass[] diag)
        {
            if (diag.Length != nrow)
            {
                throw new ArgumentException("matrix dimension differs from diagonal array length");
            }
            ComplexMatrix u = new ComplexMatrix(nrow, nrow);
            ComplexClass[,] uarray = u.getArrayReference();
            for (int i = 0; i < nrow; i++)
            {
                for (int j = i; j < nrow; j++)
                {
                    if (i == j)
                    {
                        uarray[i, j] = ComplexClass.Copy(diag[i]);
                    }
                }
            }
            return u;
        }

        // COLUMN MATRICES
        // Converts a 1-D array of ComplexClass to a column  matrix
        public static ComplexMatrix columnMatrix(ComplexClass[] darray)
        {
            int nr = darray.Length;
            ComplexMatrix pp = new ComplexMatrix(nr, 1);
            for (int i = 0; i < nr; i++)
            {
                pp.m_matrix[i, 0] = darray[i];
            }
            return pp;
        }

        // ROW MATRICES
        // Converts a 1-D array of ComplexClass to a row matrix
        public static ComplexMatrix rowMatrix(ComplexClass[] darray)
        {
            int nc = darray.Length;
            ComplexMatrix pp = new ComplexMatrix(1, nc);
            for (int i = 0; i < nc; i++)
            {
                pp.m_matrix[0, i] = darray[i];
            }
            return pp;
        }

        // CONVERSIONS
        // Converts a 1-D array of ComplexClass to a complex column matrix
        public static ComplexMatrix toComplexColumnMatrix(ComplexClass[] carray)
        {
            int nr = carray.Length;
            ComplexMatrix cc = new ComplexMatrix(nr, 1);
            for (int i = 0; i < nr; i++)
            {
                cc.m_matrix[i, 0] = carray[i].Copy();
            }
            return cc;
        }

        // Converts a 1-D array of doubles to a complex coumn matrix
        public static ComplexMatrix toComplexColumnMatrix(double[] darray)
        {
            int nr = darray.Length;
            ComplexMatrix cc = new ComplexMatrix(nr, 1);
            for (int i = 0; i < nr; i++)
            {
                cc.m_matrix[i, 0].reset(darray[i], 0.0D);
            }
            return cc;
        }

        // Converts a 1-D array of ComplexClass to a complex row matrix
        public static ComplexMatrix toComplexRowMatrix(ComplexClass[] carray)
        {
            int nc = carray.Length;
            ComplexMatrix cc = new ComplexMatrix(1, nc);
            for (int i = 0; i < nc; i++)
            {
                cc.m_matrix[0, i] = carray[i].Copy();
            }
            return cc;
        }

        // Converts a 1-D array of doubles to a complex row matrix
        public static ComplexMatrix toComplexRowMatrix(double[] darray)
        {
            int nc = darray.Length;
            ComplexMatrix cc = new ComplexMatrix(1, nc);
            for (int i = 0; i < nc; i++)
            {
                cc.m_matrix[0, i].reset(darray[i], 0.0D);
            }
            return cc;
        }

        // Converts a matrix of doubles (Matrix_non_stable) to a complex matrix (ComplexMatix)
        public static ComplexMatrix toComplexMatrix(MatrixExtended marray)
        {
            int nr = marray.getNrow();
            int nc = marray.getNcol();

            ComplexMatrix pp = new ComplexMatrix(nr, nc);
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    pp.m_matrix[i, j].reset(marray.getElementCopy(i, j), 0.0D);
                }
            }
            return pp;
        }

        // Converts a 2D array of doubles to a complex matrix (ComplexMatix)
        public static ComplexMatrix toComplexMatrix(double[,] darray)
        {
            int nr = darray.GetLength(0);
            int nc = darray.GetLength(1);
            for (int i = 1; i < nr; i++)
            {
                if (darray.GetLength(1) != nc)
                {
                    throw new ArgumentException("All rows must have the same length");
                }
            }
            ComplexMatrix pp = new ComplexMatrix(nr, nc);
            for (int i = 0; i < pp.m_nrow; i++)
            {
                for (int j = 0; j < pp.m_ncol; j++)
                {
                    pp.m_matrix[i, j].reset(darray[i, j], 0.0D);
                }
            }
            return pp;
        }

        // GET VALUES
        // Return the number of rows
        public int getNrow()
        {
            return m_nrow;
        }

        // Return the number of columns
        public int getNcol()
        {
            return m_ncol;
        }

        // Return a reference to the internal 2-D array
        public ComplexClass[,] getArrayReference()
        {
            return m_matrix;
        }

        // Return a reference to the internal 2-D array
        public ComplexClass[,] getArray()
        {
            return m_matrix;
        }

        // Return a reference to the internal 2-D array
        // included for backward compatibility with earlier incorrect documentation
        public ComplexClass[,] getArrayPointer()
        {
            return m_matrix;
        }

        // Return a copy of the internal 2-D array
        public ComplexClass[,] getArrayCopy()
        {
            ComplexClass[,] c = new ComplexClass[m_nrow,m_ncol];
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    c[i, j] = ComplexClass.Copy(m_matrix[i, j]);
                }
            }
            return c;
        }

        // Return a single element of the internal 2-D array
        public ComplexClass getElementReference(int i, int j)
        {
            return m_matrix[i, j];
        }

        // Return a reference to a single element of the internal 2-D array
        // included for backward compatibility with earlier incorrect documentation
        public ComplexClass getElementPointer(int i, int j)
        {
            return m_matrix[i, j];
        }

        // Return a copy of a single element of the internal 2-D array
        public ComplexClass getElementCopy(int i, int j)
        {
            return ComplexClass.Copy(m_matrix[i, j]);
        }

        // Return a sub-matrix starting with row index i, column index j
        // and ending with row index k, column index l
        public ComplexMatrix getSubMatrix(int i, int j, int k, int l)
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
            ComplexMatrix subMatrix = new ComplexMatrix(n, m);
            ComplexClass[,] sarray = subMatrix.getArrayReference();
            for (int p = 0; p < n; p++)
            {
                for (int q = 0; q < m; q++)
                {
                    sarray[p, q] = ComplexClass.Copy(m_matrix[i + p, j + q]);
                }
            }
            return subMatrix;
        }

        // Return a sub-matrix
        // row = array of row indices
        // col = array of column indices
        public ComplexMatrix getSubMatrix(int[] row, int[] col)
        {
            int n = row.Length;
            int m = col.Length;
            ComplexMatrix subMatrix = new ComplexMatrix(n, m);
            ComplexClass[,] sarray = subMatrix.getArrayReference();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    sarray[i, j] = ComplexClass.Copy(m_matrix[row[i], col[j]]);
                }
            }
            return subMatrix;
        }

        // Return a reference to the permutation index array
        public int[] getIndexReference()
        {
            return m_index;
        }

        // Return a reference to the permutation index array
        public int[] getIndexPointer()
        {
            return m_index;
        }

        // Return a copy of the permutation index array
        public int[] getIndexCopy()
        {
            int[] indcopy = new int[m_nrow];
            for (int i = 0; i < m_nrow; i++)
            {
                indcopy[i] = m_index[i];
            }
            return indcopy;
        }

        // Return the row swap index
        public double getSwap()
        {
            return m_dswap;
        }

        // COPY
        // Copy a ComplexMatrix [static method]
        public static ComplexMatrix Copy(ComplexMatrix a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                int nr = a.getNrow();
                int nc = a.getNcol();
                ComplexClass[,] aarray = a.getArrayReference();
                ComplexMatrix b = new ComplexMatrix(nr, nc);
                b.m_nrow = nr;
                b.m_ncol = nc;
                ComplexClass[,] barray = b.getArrayReference();
                for (int i = 0; i < nr; i++)
                {
                    for (int j = 0; j < nc; j++)
                    {
                        barray[i, j] = ComplexClass.Copy(aarray[i, j]);
                    }
                }
                for (int i = 0; i < nr; i++)
                {
                    b.m_index[i] = a.m_index[i];
                }
                return b;
            }
        }

        // Copy a ComplexMatrix [instance method]
        public ComplexMatrix Copy()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                int nr = m_nrow;
                int nc = m_ncol;
                ComplexMatrix b = new ComplexMatrix(nr, nc);
                ComplexClass[,] barray = b.getArrayReference();
                b.m_nrow = nr;
                b.m_ncol = nc;
                for (int i = 0; i < nr; i++)
                {
                    for (int j = 0; j < nc; j++)
                    {
                        barray[i, j] = ComplexClass.Copy(m_matrix[i, j]);
                    }
                }
                for (int i = 0; i < nr; i++)
                {
                    b.m_index[i] = m_index[i];
                }
                return b;
            }
        }

        // Clone a ComplexMatrix
        public Object clone()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                int nr = m_nrow;
                int nc = m_ncol;
                ComplexMatrix b = new ComplexMatrix(nr, nc);
                ComplexClass[,] barray = b.getArrayReference();
                b.m_nrow = nr;
                b.m_ncol = nc;
                for (int i = 0; i < nr; i++)
                {
                    for (int j = 0; j < nc; j++)
                    {
                        barray[i, j] = ComplexClass.Copy(m_matrix[i, j]);
                    }
                }
                for (int i = 0; i < nr; i++)
                {
                    b.m_index[i] = m_index[i];
                }
                return b;
            }
        }

        // ADDITION
        // Add this matrix to matrix B.  This matrix remains unaltered [instance method]
        public ComplexMatrix plus(ComplexMatrix bmat)
        {
            if ((m_nrow != bmat.m_nrow) || (m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = bmat.m_nrow;
            int nc = bmat.m_ncol;
            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].plus(bmat.m_matrix[i, j]);
                }
            }
            return cmat;
        }

        // Add this matrix to a Comlex 2-D array.  [instance method]
        public ComplexMatrix plus(ComplexClass[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].plus(bmat[i, j]);
                }
            }
            return cmat;
        }

        // Add this matrix to a real matrix B.  [instance method]
        public ComplexMatrix plus(MatrixExtended bmat)
        {
            int nr = bmat.getNrow();
            int nc = bmat.getNcol();
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].plus(bmat.getElement(i, j));
                }
            }
            return cmat;
        }

        // Add this matrix to a real 2-D array.  [instance method]
        public ComplexMatrix plus(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].plus(bmat[i, j]);
                }
            }
            return cmat;
        }

        // Add matrices A and B [static method]
        public static ComplexMatrix plus(ComplexMatrix amat, ComplexMatrix bmat)
        {
            if ((amat.m_nrow != bmat.m_nrow) || (amat.m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = amat.m_nrow;
            int nc = amat.m_ncol;
            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = amat.m_matrix[i, j].plus(bmat.m_matrix[i, j]);
                }
            }
            return cmat;
        }

        // Add matrix B to this matrix [equivalence of +=]
        public void plusEquals(ComplexMatrix bmat)
        {
            if ((m_nrow != bmat.m_nrow) || (m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = bmat.m_nrow;
            int nc = bmat.m_ncol;

            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    m_matrix[i, j].plusEquals(bmat.m_matrix[i, j]);
                }
            }
        }

        // SUBTRACTION
        // Subtract matrix B from this matrix.   This matrix remains unaltered [instance method]
        public ComplexMatrix minus(ComplexMatrix bmat)
        {
            if ((m_nrow != bmat.m_nrow) || (m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = m_nrow;
            int nc = m_ncol;
            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].minus(bmat.m_matrix[i, j]);
                }
            }
            return cmat;
        }

        // Subtract  Comlex 2-D array from this matrix.  [instance method]
        public ComplexMatrix minus(ComplexClass[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].minus(bmat[i, j]);
                }
            }
            return cmat;
        }

        // Subtract a real matrix from a real matrix B.  [instance method]
        public ComplexMatrix minus(MatrixExtended bmat)
        {
            int nr = bmat.getNrow();
            int nc = bmat.getNcol();
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].minus(bmat.getElement(i, j));
                }
            }
            return cmat;
        }

        // Subtract a real 2-D array from this matrix.  [instance method]
        public ComplexMatrix minus(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = m_matrix[i, j].minus(bmat[i, j]);
                }
            }
            return cmat;
        }


        // Subtract matrix B from matrix A [static method]
        public static ComplexMatrix minus(ComplexMatrix amat, ComplexMatrix bmat)
        {
            if ((amat.m_nrow != bmat.m_nrow) || (amat.m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = amat.m_nrow;
            int nc = amat.m_ncol;
            ComplexMatrix cmat = new ComplexMatrix(nr, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    carray[i, j] = amat.m_matrix[i, j].minus(bmat.m_matrix[i, j]);
                }
            }
            return cmat;
        }

        // Subtract matrix B from this matrix [equivlance of -=]
        public void minusEquals(ComplexMatrix bmat)
        {
            if ((m_nrow != bmat.m_nrow) || (m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            int nr = bmat.m_nrow;
            int nc = bmat.m_ncol;

            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    m_matrix[i, j].minusEquals(bmat.m_matrix[i, j]);
                }
            }
        }

        // MULTIPLICATION
        // Multiply this complex matrix by a complex matrix.   [instance method]
        // This matrix remains unaltered.
        public ComplexMatrix times(ComplexMatrix bmat)
        {
            if (m_ncol != bmat.m_nrow)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            ComplexMatrix cmat = new ComplexMatrix(m_nrow, bmat.m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass sum = new ComplexClass();

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < bmat.m_ncol; j++)
                {
                    sum = ComplexClass.zero();
                    for (int k = 0; k < m_ncol; k++)
                    {
                        sum.plusEquals(m_matrix[i, k].times(bmat.m_matrix[k, j]));
                    }
                    carray[i, j] = ComplexClass.Copy(sum);
                }
            }
            return cmat;
        }

        // Multiply this complex matrix by a complex 2-D array.   [instance method]
        public ComplexMatrix times(ComplexClass[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if (m_ncol != nr)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            ComplexMatrix cmat = new ComplexMatrix(m_nrow, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass sum = new ComplexClass();

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    sum = ComplexClass.zero();
                    for (int k = 0; k < m_ncol; k++)
                    {
                        sum.plusEquals(m_matrix[i, k].times(bmat[k, j]));
                    }
                    carray[i, j] = ComplexClass.Copy(sum);
                }
            }
            return cmat;
        }

        // Multiply this complex matrix by a real matrix.   [instance method]
        // This matrix remains unaltered.
        public ComplexMatrix times(MatrixExtended bmat)
        {
            int nr = bmat.getNrow();
            int nc = bmat.getNcol();

            if (m_ncol != nr)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            ComplexMatrix cmat = new ComplexMatrix(m_nrow, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass sum = new ComplexClass();

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    sum = ComplexClass.zero();
                    for (int k = 0; k < m_ncol; k++)
                    {
                        sum.plusEquals(m_matrix[i, k].times(bmat.getElement(k, j)));
                    }
                    carray[i, j] = ComplexClass.Copy(sum);
                }
            }
            return cmat;
        }

        // Multiply this complex matrix by a real 2-D array.   [instance method]
        public ComplexMatrix times(double[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if (m_ncol != nr)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            ComplexMatrix cmat = new ComplexMatrix(m_nrow, nc);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass sum = new ComplexClass();

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    sum = ComplexClass.zero();
                    for (int k = 0; k < m_ncol; k++)
                    {
                        sum.plusEquals(m_matrix[i, k].times(bmat[k, j]));
                    }
                    carray[i, j] = ComplexClass.Copy(sum);
                }
            }
            return cmat;
        }

        // Multiply this complex matrix by a complex constant [instance method]
        // This matrix remains unaltered
        public ComplexMatrix times(ComplexClass constant)
        {
            ComplexMatrix cmat = new ComplexMatrix(m_nrow, m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    carray[i, j] = m_matrix[i, j].times(constant);
                }
            }
            return cmat;
        }

        // Multiply this complex matrix by a real (double) constant [instance method]
        // This matrix remains unaltered.
        public ComplexMatrix times(double constant)
        {
            ComplexMatrix cmat = new ComplexMatrix(m_nrow, m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass cconstant = new ComplexClass(constant, 0.0);

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    carray[i, j] = m_matrix[i, j].times(cconstant);
                }
            }
            return cmat;
        }

        // Multiply two complex matrices {static method]
        public static ComplexMatrix times(ComplexMatrix amat, ComplexMatrix bmat)
        {
            if (amat.m_ncol != bmat.m_nrow)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            ComplexMatrix cmat = new ComplexMatrix(amat.m_nrow, bmat.m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass sum = new ComplexClass();

            for (int i = 0; i < amat.m_nrow; i++)
            {
                for (int j = 0; j < bmat.m_ncol; j++)
                {
                    sum = ComplexClass.zero();
                    for (int k = 0; k < amat.m_ncol; k++)
                    {
                        sum.plusEquals(amat.m_matrix[i, k].times(bmat.m_matrix[k, j]));
                    }
                    carray[i, j] = ComplexClass.Copy(sum);
                }
            }
            return cmat;
        }


        // Multiply a complex matrix by a complex constant [static method]
        public static ComplexMatrix times(ComplexMatrix amat, ComplexClass constant)
        {
            ComplexMatrix cmat = new ComplexMatrix(amat.m_nrow, amat.m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();

            for (int i = 0; i < amat.m_nrow; i++)
            {
                for (int j = 0; j < amat.m_ncol; j++)
                {
                    carray[i, j] = amat.m_matrix[i, j].times(constant);
                }
            }
            return cmat;
        }

        // Multiply a complex matrix by a real (double) constant [static method]
        public static ComplexMatrix times(ComplexMatrix amat, double constant)
        {
            ComplexMatrix cmat = new ComplexMatrix(amat.m_nrow, amat.m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass cconstant = new ComplexClass(constant, 0.0);

            for (int i = 0; i < amat.m_nrow; i++)
            {
                for (int j = 0; j < amat.m_ncol; j++)
                {
                    carray[i, j] = amat.m_matrix[i, j].times(cconstant);
                }
            }
            return cmat;
        }

        // Multiply this matrix by a complex matrix [equivalence of *=]
        public void timesEquals(ComplexMatrix bmat)
        {
            if (m_ncol != bmat.m_nrow)
            {
                throw new ArgumentException("Nonconformable matrices");
            }

            ComplexMatrix cmat = new ComplexMatrix(m_nrow, bmat.m_ncol);
            ComplexClass[,] carray = cmat.getArrayReference();
            ComplexClass sum = new ComplexClass();

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < bmat.m_ncol; j++)
                {
                    sum = ComplexClass.zero();
                    for (int k = 0; k < m_ncol; k++)
                    {
                        sum.plusEquals(m_matrix[i, k].times(bmat.m_matrix[k, j]));
                    }
                    carray[i, j] = ComplexClass.Copy(sum);
                }
            }

            m_nrow = cmat.m_nrow;
            m_ncol = cmat.m_ncol;
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j] = cmat.m_matrix[i, j];
                }
            }
        }


        // Multiply this matrix by a complex constant [equivalence of *=]
        public void timesEquals(ComplexClass constant)
        {
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j].timesEquals(constant);
                }
            }
        }

        // Multiply this matrix by a real (double) constant [equivalence of *=]
        public void timesEquals(double constant)
        {
            ComplexClass cconstant = new ComplexClass(constant, 0.0);

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    m_matrix[i, j].timesEquals(cconstant);
                }
            }
        }

        // DIVISION
        // Divide this ComplexMatrix by a ComplexMatrix - instance method.
        public ComplexMatrix over(ComplexMatrix bmat)
        {
            if ((m_nrow != bmat.m_nrow) || (m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            return times(bmat.inverse());
        }

        // Divide this matrix by a ComplexClass 2-D array - instance method.
        public ComplexMatrix over(ComplexClass[,] bmat)
        {
            int nr = bmat.GetLength(0);
            int nc = bmat.GetLength(1);
            if ((m_nrow != nr) || (m_ncol != nc))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }

            ComplexMatrix cmat = new ComplexMatrix(bmat);
            return times(cmat.inverse());
        }

        // Divide this ComplexMatrix by a Matrix_non_stable - instance method.
        public ComplexMatrix over(MatrixExtended bmat)
        {
            ComplexMatrix pmat = toComplexMatrix(bmat);
            return over(pmat);
        }

        // Divide this ComplexMatrix by a 2D array of double - instance method.
        public ComplexMatrix over(double[,] bmat)
        {
            ComplexMatrix pmat = toComplexMatrix(bmat);
            return over(pmat);
        }

        // Divide this ComplexMatrix by a ComplexMatrix - static method.
        public ComplexMatrix over(ComplexMatrix amat, ComplexMatrix bmat)
        {
            if ((amat.m_nrow != bmat.m_nrow) || (amat.m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            return amat.times(bmat.inverse());
        }

        // Divide this ComplexMatrix by a ComplexMatrix [equivalence of /=]
        public void overEquals(ComplexMatrix bmat)
        {
            if ((m_nrow != bmat.m_nrow) || (m_ncol != bmat.m_ncol))
            {
                throw new ArgumentException("Array dimensions do not agree");
            }
            ComplexMatrix cmat = new ComplexMatrix(bmat);
            timesEquals(cmat.inverse());
        }

        // INVERSE
        // Inverse of a square complex matrix [instance method]
        public ComplexMatrix inverse()
        {
            int n = m_nrow;
            if (n != m_ncol)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            ComplexMatrix invmat = new ComplexMatrix(n, n);

            if (n == 1)
            {
                ComplexClass[,] hold = getArrayCopy();
                if (hold[0, 0].isZero())
                {
                    throw new ArgumentException("Matrix_non_stable is singular");
                }
                hold[0, 0] = ComplexClass.plusOne().over(hold[0, 0]);
                invmat = new ComplexMatrix(hold);
            }
            else
            {
                if (n == 2)
                {
                    ComplexClass[,] hold = getArrayCopy();
                    ComplexClass det = (hold[0, 0].times(hold[1, 1])).minus(hold[0, 1].times(hold[1, 0]));
                    if (det.isZero())
                    {
                        throw new ArgumentException("Matrix_non_stable is singular");
                    }

                    ComplexClass[,] hold2 = ComplexClass.twoDarray(2, 2);
                    hold2[0, 0] = hold[1, 1].over(det);
                    hold2[1, 1] = hold[0, 0].over(det);
                    hold2[1, 0] = hold[1, 0].negate().over(det);
                    hold2[0, 1] = hold[0, 1].negate().over(det);
                    invmat = new ComplexMatrix(hold2);
                }
                else
                {
                    ComplexClass[] col = new ComplexClass[n];
                    ComplexClass[] xvec = new ComplexClass[n];
                    ComplexClass[,] invarray = invmat.getArrayReference();
                    ComplexMatrix ludmat;

                    ludmat = luDecomp();
                    for (int j = 0; j < n; j++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            col[i] = ComplexClass.zero();
                        }
                        col[j] = ComplexClass.plusOne();
                        xvec = ludmat.luBackSub(col);
                        for (int i = 0; i < n; i++)
                        {
                            invarray[i, j] = ComplexClass.Copy(xvec[i]);
                        }
                    }
                }
            }
            return invmat;
        }

        // Inverse of a square complex matrix [static method]
        public static ComplexMatrix inverse(ComplexMatrix amat)
        {
            int n = amat.m_nrow;
            if (n != amat.m_ncol)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }

            ComplexMatrix invmat = new ComplexMatrix(n, n);

            if (n == 1)
            {
                ComplexClass[,] hold = amat.getArrayCopy();
                if (hold[0, 0].isZero())
                {
                    throw new ArgumentException("Matrix_non_stable is singular");
                }
                hold[0, 0] = ComplexClass.plusOne().over(hold[0, 0]);
                invmat = new ComplexMatrix(hold);
            }
            else
            {
                if (n == 2)
                {
                    ComplexClass[,] hold = amat.getArrayCopy();
                    ComplexClass det = (hold[0, 0].times(hold[1, 1])).minus(hold[0, 1].times(hold[1, 0]));
                    if (det.isZero())
                    {
                        throw new ArgumentException("Matrix_non_stable is singular");
                    }

                    ComplexClass[,] hold2 = ComplexClass.twoDarray(2, 2);
                    hold2[0, 0] = hold[1, 1].over(det);
                    hold2[1, 1] = hold[0, 0].over(det);
                    hold2[1, 0] = hold[1, 0].negate().over(det);
                    hold2[0, 1] = hold[0, 1].negate().over(det);
                    invmat = new ComplexMatrix(hold2);
                }
                else
                {
                    ComplexClass[] col = new ComplexClass[n];
                    ComplexClass[] xvec = new ComplexClass[n];
                    ComplexClass[,] invarray = invmat.getArrayReference();
                    ComplexMatrix ludmat;

                    ludmat = amat.luDecomp();
                    for (int j = 0; j < n; j++)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            col[i] = ComplexClass.zero();
                        }
                        col[j] = ComplexClass.plusOne();
                        xvec = ludmat.luBackSub(col);
                        for (int i = 0; i < n; i++)
                        {
                            invarray[i, j] = ComplexClass.Copy(xvec[i]);
                        }
                    }
                }
            }
            return invmat;
        }

        // TRANSPOSE
        // Transpose of a complex matrix [instance method]
        public ComplexMatrix transpose()
        {
            ComplexMatrix tmat = new ComplexMatrix(m_ncol, m_nrow);
            ComplexClass[,] tarray = tmat.getArrayReference();
            for (int i = 0; i < m_ncol; i++)
            {
                for (int j = 0; j < m_nrow; j++)
                {
                    tarray[i, j] = ComplexClass.Copy(m_matrix[j, i]);
                }
            }
            return tmat;
        }

        // Transpose of a complex matrix [static method]
        public static ComplexMatrix transpose(ComplexMatrix amat)
        {
            ComplexMatrix tmat = new ComplexMatrix(amat.m_ncol, amat.m_nrow);
            ComplexClass[,] tarray = tmat.getArrayReference();
            for (int i = 0; i < amat.m_ncol; i++)
            {
                for (int j = 0; j < amat.m_nrow; j++)
                {
                    tarray[i, j] = ComplexClass.Copy(amat.m_matrix[j, i]);
                }
            }
            return tmat;
        }

        // COMPLEX CONJUGATE
        //ComplexClass Conjugate of a complex matrix [instance method]
        public ComplexMatrix conjugate()
        {
            ComplexMatrix conj = Copy(this);
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    conj.m_matrix[i, j] = m_matrix[i, j].conjugate();
                }
            }
            return conj;
        }

        //ComplexClass Conjugate of a complex matrix [static method]
        public static ComplexMatrix conjugate(ComplexMatrix amat)
        {
            ComplexMatrix conj = Copy(amat);
            for (int i = 0; i < amat.m_nrow; i++)
            {
                for (int j = 0; j < amat.m_ncol; j++)
                {
                    conj.m_matrix[i, j] = amat.m_matrix[i, j].conjugate();
                }
            }
            return conj;
        }

        // ADJOIN
        // Adjoin of a complex matrix [instance method]
        public ComplexMatrix adjoin()
        {
            ComplexMatrix adj = Copy(this);
            adj = adj.transpose();
            adj = adj.conjugate();
            return adj;
        }

        // Adjoin of a complex matrix [static method]
        public ComplexMatrix adjoin(ComplexMatrix amat)
        {
            ComplexMatrix adj = Copy(amat);
            adj = adj.transpose();
            adj = adj.conjugate();
            return adj;
        }

        // OPPOSITE
        // Opposite of a complex matrix [instance method]
        public ComplexMatrix opposite()
        {
            ComplexMatrix opp = Copy(this);
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    opp.m_matrix[i, j] = m_matrix[i, j].times(ComplexClass.minusOne());
                }
            }
            return opp;
        }

        // Opposite of a complex matrix [static method]
        public static ComplexMatrix opposite(ComplexMatrix amat)
        {
            ComplexMatrix opp = Copy(amat);
            for (int i = 0; i < amat.m_nrow; i++)
            {
                for (int j = 0; j < amat.m_ncol; j++)
                {
                    opp.m_matrix[i, j] = amat.m_matrix[i, j].times(ComplexClass.minusOne());
                }
            }
            return opp;
        }

        // TRACE
        // Trace of a complex matrix [instance method]
        public ComplexClass trace()
        {
            ComplexClass trac = new ComplexClass(0.0, 0.0);
            for (int i = 0; i < Math.Min(m_ncol, m_ncol); i++)
            {
                trac.plusEquals(m_matrix[i, i]);
            }
            return trac;
        }

        // Trace of a complex matrix [static method]
        public static ComplexClass trace(ComplexMatrix amat)
        {
            ComplexClass trac = new ComplexClass(0.0, 0.0);
            for (int i = 0; i < Math.Min(amat.m_ncol, amat.m_ncol); i++)
            {
                trac.plusEquals(amat.m_matrix[i, i]);
            }
            return trac;
        }

        // DETERMINANT
        //  Returns the determinant of a complex square matrix [instance method]
        public ComplexClass determinant()
        {
            int n = m_nrow;
            if (n != m_ncol)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            ComplexClass det = new ComplexClass();
            ComplexMatrix ludmat;

            ludmat = luDecomp();
            det.reset(ludmat.m_dswap, 0.0);
            for (int j = 0; j < n; j++)
            {
                det.timesEquals(ludmat.m_matrix[j, j]);
            }
            return det;
        }

        //  Returns the determinant of a complex square matrix [static method]
        public static ComplexClass determinant(ComplexMatrix amat)
        {
            int n = amat.m_nrow;
            if (n != amat.m_ncol)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            ComplexClass det = new ComplexClass();
            ComplexMatrix ludmat;

            ludmat = amat.luDecomp();
            det.reset(ludmat.m_dswap, 0.0);
            for (int j = 0; j < n; j++)
            {
                det.timesEquals(ludmat.m_matrix[j, j]);
            }
            return det;
        }

        // Returns the Log(determinant) of a complex square matrix [instance method].
        // Useful if determinant() underflows or overflows.
        public ComplexClass logDeterminant()
        {
            int n = m_nrow;
            if (n != m_ncol)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            ComplexClass det = new ComplexClass();
            ComplexMatrix ludmat;

            ludmat = luDecomp();
            det.reset(ludmat.m_dswap, 0.0);
            det = ComplexClass.Log(det);
            for (int j = 0; j < n; j++)
            {
                det.plusEquals(ComplexClass.Log(ludmat.m_matrix[j, j]));
            }
            return det;
        }

        // Returns the Log(determinant) of a complex square matrix [static method].
        // Useful if determinant() underflows or overflows.
        public static ComplexClass logDeterminant(ComplexMatrix amat)
        {
            int n = amat.m_nrow;
            if (n != amat.m_ncol)
            {
                throw new ArgumentException("Matrix_non_stable is not square");
            }
            ComplexClass det = new ComplexClass();
            ComplexMatrix ludmat;

            ludmat = amat.luDecomp();
            det.reset(ludmat.m_dswap, 0.0);
            det = ComplexClass.Log(det);
            for (int j = 0; j < n; j++)
            {
                det.plusEquals(ComplexClass.Log(ludmat.m_matrix[j, j]));
            }
            return det;
        }

        // REDUCED ROW ECHELON FORM
        public ComplexMatrix reducedRowEchelonForm()
        {
            ComplexClass[,] mat = ComplexClass.twoDarray(m_nrow, m_ncol);
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    mat[i, j] = m_matrix[i, j];
                }
            }

            int leadingCoeff = 0;
            int rowPointer = 0;

            bool testOuter = true;
            while (testOuter)
            {
                int counter = rowPointer;
                bool testInner = true;
                while (testInner && mat[counter, leadingCoeff].Equals(ComplexClass.zero()))
                {
                    counter++;
                    if (counter == m_nrow)
                    {
                        counter = rowPointer;
                        leadingCoeff++;
                        if (leadingCoeff == m_ncol)
                        {
                            testInner = false;
                        }
                    }
                }
                if (testInner)
                {
                    mat = ArrayHelper.SwapRowFromArray(
                        mat,
                        rowPointer,
                        counter);

                    ComplexClass pivot = mat[rowPointer, leadingCoeff];

                    for (int j = 0; j < m_ncol; j++)
                    {
                        mat[rowPointer, j] = mat[rowPointer, j].over(pivot);
                    }

                    for (int i = 0; i < m_nrow; i++)
                    {
                        if (i != rowPointer)
                        {
                            pivot = mat[i, leadingCoeff];
                            for (int j = 0; j < m_ncol; j++)
                            {
                                mat[i, j] = mat[i, j].minus(pivot.times(mat[rowPointer, j]));
                            }
                        }
                    }
                    leadingCoeff++;
                    if (leadingCoeff >= m_ncol)
                    {
                        testOuter = false;
                    }
                }
                rowPointer++;
                if (rowPointer >= m_nrow || !testInner)
                {
                    testOuter = false;
                }
            }

            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    if (mat[i, j].getReal() == -0.0)
                    {
                        mat[i, j].reset(0.0, mat[i, j].getImag());
                    }
                    if (mat[i, j].getImag() == -0.0)
                    {
                        mat[i, j].reset(mat[i, j].getReal(), 0.0);
                    }
                }
            }
            return new ComplexMatrix(mat);
        }

        // FROBENIUS NORM of a complex matrix
        // Sometimes referred to as the EUCLIDEAN NORM
        public double frobeniusNorm()
        {
            double norm = 0.0D;
            for (int i = 0; i < m_nrow; i++)
            {
                for (int j = 0; j < m_ncol; j++)
                {
                    norm = Fmath.hypot(norm, ComplexClass.Abs(m_matrix[i, j]));
                }
            }
            return norm;
        }

        // ONE NORM of a complex matrix
        public double oneNorm()
        {
            double norm = 0.0D;
            double sum = 0.0D;
            for (int i = 0; i < m_nrow; i++)
            {
                sum = 0.0D;
                for (int j = 0; j < m_ncol; j++)
                {
                    sum += ComplexClass.Abs(m_matrix[i, j]);
                }
                norm = Math.Max(norm, sum);
            }
            return norm;
        }

        // INFINITY NORM of a complex matrix
        public double infinityNorm()
        {
            double norm = 0.0D;
            double sum = 0.0D;
            for (int i = 0; i < m_nrow; i++)
            {
                sum = 0.0D;
                for (int j = 0; j < m_ncol; j++)
                {
                    sum += ComplexClass.Abs(m_matrix[i, j]);
                }
                norm = Math.Max(norm, sum);
            }
            return norm;
        }


        // LU DECOMPOSITION OF COMPLEX MATRIX A
        // For details of LU decomposition
        // See Numerical Recipes, The Art of Scientific Computing
        // by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
        // Cambridge University Press,   http://www.nr.com/
        // ComplexMatrix ludmat is the returned LU decompostion
        // int[] index is the vector of row permutations
        // dswap returns +1.0 for even number of row interchanges
        //       returns -1.0 for odd number of row interchanges
        public ComplexMatrix luDecomp()
        {
            if (m_nrow != m_ncol)
            {
                throw new ArgumentException("A matrix is not square");
            }
            int n = m_nrow;
            int imax = 0;
            double dum = 0.0D, temp = 0.0D, big = 0.0D;
            double[] vv = new double[n];
            ComplexClass sum = new ComplexClass();
            ComplexClass dumm = new ComplexClass();

            ComplexMatrix ludmat = Copy(this);
            ComplexClass[,] ludarray = ludmat.getArrayReference();

            ludmat.m_dswap = 1.0;
            for (int i = 0; i < n; i++)
            {
                big = 0.0;
                for (int j = 0; j < n; j++)
                {
                    if ((temp = ComplexClass.Abs(ludarray[i, j])) > big)
                    {
                        big = temp;
                    }
                }
                if (big == 0.0)
                {
                    throw new ArithmeticException("Singular matrix");
                }
                vv[i] = 1.0/big;
            }
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < j; i++)
                {
                    sum = ComplexClass.Copy(ludarray[i, j]);
                    for (int k = 0; k < i; k++)
                    {
                        sum.minusEquals(ludarray[i, k].times(ludarray[k, j]));
                    }
                    ludarray[i, j] = ComplexClass.Copy(sum);
                }
                big = 0.0;
                for (int i = j; i < n; i++)
                {
                    sum = ComplexClass.Copy(ludarray[i, j]);
                    for (int k = 0; k < j; k++)
                    {
                        sum.minusEquals(ludarray[i, k].times(ludarray[k, j]));
                    }
                    ludarray[i, j] = ComplexClass.Copy(sum);
                    if ((dum = vv[i]*ComplexClass.Abs(sum)) >= big)
                    {
                        big = dum;
                        imax = i;
                    }
                }
                if (j != imax)
                {
                    for (int k = 0; k < n; k++)
                    {
                        dumm = ComplexClass.Copy(ludarray[imax, k]);
                        ludarray[imax, k] = ComplexClass.Copy(ludarray[j, k]);
                        ludarray[j, k] = ComplexClass.Copy(dumm);
                    }
                    ludmat.m_dswap = -ludmat.m_dswap;
                    vv[imax] = vv[j];
                }
                ludmat.m_index[j] = imax;

                if (ludarray[j, j].isZero())
                {
                    ludarray[j, j].reset(TINY, TINY);
                }
                if (j != n - 1)
                {
                    dumm = ComplexClass.over(1.0, ludarray[j, j]);
                    for (int i = j + 1; i < n; i++)
                    {
                        ludarray[i, j].timesEquals(dumm);
                    }
                }
            }
            return ludmat;
        }

        // Solves the set of n linear complex equations A.X=B using not A but its LU decomposition
        // ComplexClass bvec is the vector B (input)
        // ComplexClass xvec is the vector X (output)
        // index is the permutation vector produced by luDecomp()
        public ComplexClass[] luBackSub(ComplexClass[] bvec)
        {
            int ii = 0, ip = 0;
            int n = bvec.Length;
            if (n != m_ncol)
            {
                throw new ArgumentException("vector length is not equal to matrix dimension");
            }
            if (m_ncol != m_nrow)
            {
                throw new ArgumentException("matrix is not square");
            }
            ComplexClass sum = new ComplexClass();
            ComplexClass[] xvec = new ComplexClass[n];
            for (int i = 0; i < n; i++)
            {
                xvec[i] = ComplexClass.Copy(bvec[i]);
            }
            for (int i = 0; i < n; i++)
            {
                ip = m_index[i];
                sum = ComplexClass.Copy(xvec[ip]);
                xvec[ip] = ComplexClass.Copy(xvec[i]);
                if (ii == 0)
                {
                    for (int j = ii; j <= i - 1; j++)
                    {
                        sum.minusEquals(m_matrix[i, j].times(xvec[j]));
                    }
                }
                else
                {
                    if (sum.isZero())
                    {
                        ii = i;
                    }
                }
                xvec[i] = ComplexClass.Copy(sum);
            }
            for (int i = n - 1; i >= 0; i--)
            {
                sum = ComplexClass.Copy(xvec[i]);
                for (int j = i + 1; j < n; j++)
                {
                    sum.minusEquals(m_matrix[i, j].times(xvec[j]));
                }
                xvec[i] = sum.over(m_matrix[i, i]);
            }
            return xvec;
        }

        // Solves the set of n linear complex equations A.X=B
        // ComplexClass bvec is the vector B (input)
        // ComplexClass xvec is the vector X (output)
        public ComplexClass[] solveLinearSet(ComplexClass[] bvec)
        {
            ComplexMatrix ludmat;

            ludmat = luDecomp();
            return ludmat.luBackSub(bvec);
        }
    }
}
