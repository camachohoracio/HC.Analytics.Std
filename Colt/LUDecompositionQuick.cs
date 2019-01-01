#region

using System;
using System.Text;
using HC.Analytics.Colt.CustomImplementations.tmp;

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
    //package Appendlinalg;

    ////import DoubleMatrix1D;
    ////import DoubleMatrix2D;
    /** 
    A low level version of {@link LUDecomposition}, avoiding unnecessary memory allocation and copying.
    The input to <tt>decompose</tt> methods is overriden with the result (LU).
    The input to <tt>solve</tt> methods is overriden with the result (X).
    In addition to <tt>LUDecomposition</tt>, this class also includes a faster variant of the decomposition, specialized for tridiagonal (and hence also diagonal) matrices,
    as well as a solver tuned for vectors.
    Its disadvantage is that it is a bit more difficult to use than <tt>LUDecomposition</tt>. 
    Thus, you may want to disregard this class and come back later, if a need for speed arises.
    <p>
    An instance of this class remembers the result of its last decomposition.
    Usage pattern is as follows: Create an instance of this class, call a decompose method, 
    then retrieve the decompositions, determinant, and/or solve as many equation problems as needed.
    Once another matrix needs to be LU-decomposed, you need not create a new instance of this class. 
    Start again by calling a decompose method, then retrieve the decomposition and/or solve your equations, and so on.
    In case a <tt>LU</tt> matrix is already available, call method <tt>setLU</tt> instead of <tt>decompose</tt> and proceed with solving et al.
    <p>
    If a matrix shall not be overriden, use <tt>matrix.Copy()</tt> and hand the the copy to methods.
    <p>
    For an <tt>m x n</tt> matrix <tt>A</tt> with <tt>m >= n</tt>, the LU decomposition is an <tt>m x n</tt>
    unit lower triangular matrix <tt>L</tt>, an <tt>n x n</tt> upper triangular matrix <tt>U</tt>,
    and a permutation vector <tt>piv</tt> of Length <tt>m</tt> so that <tt>A(piv,:) = L*U</tt>;
    If <tt>m < n</tt>, then <tt>L</tt> is <tt>m x m</tt> and <tt>U</tt> is <tt>m x n</tt>.
    <P>
    The LU decomposition with pivoting always exists, even if the matrix is
    singular, so the decompose methods will never fail.  The primary use of the
    LU decomposition is in the solution of square systems of simultaneous
    linear equations.
    Attempting to solve such a system will throw an exception if <tt>isNonsingular()</tt> returns false.
    <p>
    */

    [Serializable]
    public class LUDecompositionQuick
    {
        //static long serialVersionUID = 1020;
        /** Array for internal storage of decomposition.
        @serial internal array storage.
        */
        public Algebra algebra;
        public bool isNonSingular;
        public DoubleMatrix2D m_LU;

        /** pivot sign.
        @serial pivot sign.
        */

        /** Internal storage of pivot vector.
        @serial pivot vector.
        */
        public int[] piv;
        public int pivsign;

        public int[] work1;
        public int[] work2;
        public double[] workDouble;

        /**
        Constructs and returns a new LU Decomposition object with default tolerance <tt>1.0E-9</tt> for singularity detection.
        */

        public LUDecompositionQuick()
            : this(Property.DEFAULT.tolerance())
        {
        }

        /**
        Constructs and returns a new LU Decomposition object which uses the given tolerance for singularity detection; 
        */

        public LUDecompositionQuick(double tolerance)
        {
            algebra = new Algebra(tolerance);
        }

        /**
        Decomposes matrix <tt>A</tt> into <tt>L</tt> and <tt>U</tt> (in-place).
        Upon return <tt>A</tt> is overridden with the result <tt>LU</tt>, such that <tt>L*U = A</tt>.
        Uses a "left-looking", dot-product, Crout/Doolittle algorithm.
        @param  A   any matrix.
        */

        public void decompose(DoubleMatrix2D A)
        {
            int CUT_OFF = 10;
            // setup
            m_LU = A;
            int m = A.Rows();
            int n = A.Columns();

            // setup pivot vector
            if (piv == null || piv.Length != m)
            {
                piv = new int[m];
            }
            for (int i = m; --i >= 0;)
            {
                piv[i] = i;
            }
            pivsign = 1;

            if (m*n == 0)
            {
                setLU(m_LU);
                return; // nothing to do
            }

            //precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] LUrows = new DoubleMatrix1D[m];
            for (int i = 0; i < m; i++)
            {
                LUrows[i] = m_LU.viewRow(i);
            }

            IntArrayList nonZeroIndexes = new IntArrayList(); // sparsity
            DoubleMatrix1D LUcolj = m_LU.viewColumn(0).like(); // blocked column j
            Mult multFunction = Mult.mult(0);

            // Outer loop.
            for (int j = 0; j < n; j++)
            {
                // blocking (make copy of j-th column to localize references)
                LUcolj.assign(m_LU.viewColumn(j));

                // sparsity detection
                int maxCardinality = m/CUT_OFF; // == heuristic depending on speedup
                LUcolj.getNonZeros(nonZeroIndexes, null, maxCardinality);
                int cardinality = nonZeroIndexes.Size();
                bool sparse = (cardinality < maxCardinality);

                // Apply previous transformations.
                for (int i = 0; i < m; i++)
                {
                    int kmax = Math.Min(i, j);
                    double s;
                    if (sparse)
                    {
                        s = LUrows[i].zDotProduct(LUcolj, 0, kmax, nonZeroIndexes);
                    }
                    else
                    {
                        s = LUrows[i].zDotProduct(LUcolj, 0, kmax);
                    }
                    double before = LUcolj.getQuick(i);
                    double after = before - s;
                    LUcolj.setQuick(i, after); // LUcolj is a copy
                    m_LU.setQuick(i, j, after); // this is the original
                    if (sparse)
                    {
                        if (before == 0 && after != 0)
                        {
                            // nasty bug fixed!
                            int pos = nonZeroIndexes.binarySearch(i);
                            pos = -pos - 1;
                            nonZeroIndexes.beforeInsert(pos, i);
                        }
                        if (before != 0 && after == 0)
                        {
                            nonZeroIndexes.remove(nonZeroIndexes.binarySearch(i));
                        }
                    }
                }

                // Find pivot and exchange if necessary.
                int p = j;
                if (p < m)
                {
                    double max = Math.Abs(LUcolj.getQuick(p));
                    for (int i = j + 1; i < m; i++)
                    {
                        double v = Math.Abs(LUcolj.getQuick(i));
                        if (v > max)
                        {
                            p = i;
                            max = v;
                        }
                    }
                }
                if (p != j)
                {
                    LUrows[p].swap(LUrows[j]);
                    int k = piv[p];
                    piv[p] = piv[j];
                    piv[j] = k;
                    pivsign = -pivsign;
                }

                // Compute multipliers.
                double jj;
                if (j < m && (jj = m_LU.getQuick(j, j)) != 0.0)
                {
                    multFunction.m_multiplicator = 1/jj;
                    m_LU.viewColumn(j).viewPart(j + 1, m - (j + 1)).assign(multFunction);
                }
            }
            setLU(m_LU);
        }

        /**
        Decomposes the banded and square matrix <tt>A</tt> into <tt>L</tt> and <tt>U</tt> (in-place).
        Upon return <tt>A</tt> is overridden with the result <tt>LU</tt>, such that <tt>L*U = A</tt>.
        Currently supports diagonal and tridiagonal matrices, all other cases fall through to {@link #decompose(DoubleMatrix2D)}.
        @param semiBandwidth == 1 --> A is diagonal, == 2 --> A is tridiagonal.
        @param  A   any matrix.
        */

        public void decompose(DoubleMatrix2D A, int semiBandwidth)
        {
            if (!algebra.property().isSquare(A) || semiBandwidth < 0 || semiBandwidth > 2)
            {
                decompose(A);
                return;
            }
            // setup
            m_LU = A;
            int m = A.Rows();
            int n = A.Columns();

            // setup pivot vector
            if (piv == null || piv.Length != m)
            {
                piv = new int[m];
            }
            for (int i = m; --i >= 0;)
            {
                piv[i] = i;
            }
            pivsign = 1;

            if (m*n == 0)
            {
                setLU(A);
                return; // nothing to do
            }

            //if (semiBandwidth == 1) { // A is diagonal; nothing to do
            if (semiBandwidth == 2)
            {
                // A is tridiagonal
                // currently no pivoting !
                if (n > 1)
                {
                    A.setQuick(1, 0, A.getQuick(1, 0)/A.getQuick(0, 0));
                }

                for (int i = 1; i < n; i++)
                {
                    double ei = A.getQuick(i, i) - A.getQuick(i, i - 1)*A.getQuick(i - 1, i);
                    A.setQuick(i, i, ei);
                    if (i < n - 1)
                    {
                        A.setQuick(i + 1, i, A.getQuick(i + 1, i)/ei);
                    }
                }
            }
            setLU(A);
        }

        /** 
        Returns the determinant, <tt>det(A)</tt>.
        @exception  ArgumentException  if <tt>A.Rows() != A.Columns()</tt> (Matrix must be square).
        */

        public double det()
        {
            int m_ = m();
            int n_ = n();
            if (m_ != n_)
            {
                throw new ArgumentException("Matrix must be square.");
            }

            if (!isNonsingular())
            {
                return 0; // avoid rounding errors
            }

            double det = pivsign;
            for (int j = 0; j < n_; j++)
            {
                det *= m_LU.getQuick(j, j);
            }
            return det;
        }

        /** 
        Returns pivot permutation vector as a one-dimensional double array
        @return     (double) piv
        */

        public double[] getDoublePivot()
        {
            int m_ = m();
            double[] vals = new double[m_];
            for (int i = 0; i < m_; i++)
            {
                vals[i] = piv[i];
            }
            return vals;
        }

        /** 
        Returns the lower triangular factor, <tt>L</tt>.
        @return     <tt>L</tt>
        */

        public DoubleMatrix2D getL()
        {
            return lowerTriangular(m_LU.Copy());
        }

        /** 
        Returns a copy of the combined lower and upper triangular factor, <tt>LU</tt>.
        @return     <tt>LU</tt>
        */

        public DoubleMatrix2D getLU()
        {
            return m_LU.Copy();
        }

        /** 
        Returns the pivot permutation vector (not a copy of it).
        @return     piv
        */

        public int[] getPivot()
        {
            return piv;
        }

        /** 
        Returns the upper triangular factor, <tt>U</tt>.
        @return     <tt>U</tt>
        */

        public DoubleMatrix2D getU()
        {
            return upperTriangular(m_LU.Copy());
        }

        /** 
        Returns whether the matrix is nonsingular (has an inverse).
        @return true if <tt>U</tt>, and hence <tt>A</tt>, is nonsingular; false otherwise.
        */

        public bool isNonsingular()
        {
            return isNonSingular;
        }

        /** 
        Returns whether the matrix is nonsingular.
        @return true if <tt>matrix</tt> is nonsingular; false otherwise.
        */

        public bool isNonsingular(DoubleMatrix2D matrix)
        {
            int m = matrix.Rows();
            int n = matrix.Columns();
            double epsilon = algebra.property().tolerance(); // consider numerical instability
            for (int j = Math.Min(n, m); --j >= 0;)
            {
                //if (matrix.getQuick(j,j) == 0) return false;
                if (Math.Abs(matrix.getQuick(j, j)) <= epsilon)
                {
                    return false;
                }
            }
            return true;
        }

        /**
        Modifies the matrix to be a lower triangular matrix.
        <p>
        <b>Examples:</b> 
        <table border="0">
          <tr nowrap> 
            <td valign="top">3 x 5 matrix:<br>
              9, 9, 9, 9, 9<br>
              9, 9, 9, 9, 9<br>
              9, 9, 9, 9, 9 </td>
            <td align="center">triang.Upper<br>
              ==></td>
            <td valign="top">3 x 5 matrix:<br>
              9, 9, 9, 9, 9<br>
              0, 9, 9, 9, 9<br>
              0, 0, 9, 9, 9</td>
          </tr>
          <tr nowrap> 
            <td valign="top">5 x 3 matrix:<br>
              9, 9, 9<br>
              9, 9, 9<br>
              9, 9, 9<br>
              9, 9, 9<br>
              9, 9, 9 </td>
            <td align="center">triang.Upper<br>
              ==></td>
            <td valign="top">5 x 3 matrix:<br>
              9, 9, 9<br>
              0, 9, 9<br>
              0, 0, 9<br>
              0, 0, 0<br>
              0, 0, 0</td>
          </tr>
          <tr nowrap> 
            <td valign="top">3 x 5 matrix:<br>
              9, 9, 9, 9, 9<br>
              9, 9, 9, 9, 9<br>
              9, 9, 9, 9, 9 </td>
            <td align="center">triang.Lower<br>
              ==></td>
            <td valign="top">3 x 5 matrix:<br>
              1, 0, 0, 0, 0<br>
              9, 1, 0, 0, 0<br>
              9, 9, 1, 0, 0</td>
          </tr>
          <tr nowrap> 
            <td valign="top">5 x 3 matrix:<br>
              9, 9, 9<br>
              9, 9, 9<br>
              9, 9, 9<br>
              9, 9, 9<br>
              9, 9, 9 </td>
            <td align="center">triang.Lower<br>
              ==></td>
            <td valign="top">5 x 3 matrix:<br>
              1, 0, 0<br>
              9, 1, 0<br>
              9, 9, 1<br>
              9, 9, 9<br>
              9, 9, 9</td>
          </tr>
        </table>

        @return <tt>A</tt> (for convenience only).
        @see #triangulateUpper(DoubleMatrix2D)
        */

        public DoubleMatrix2D lowerTriangular(DoubleMatrix2D A)
        {
            int rows = A.Rows();
            int columns = A.Columns();
            int min = Math.Min(rows, columns);
            for (int r = min; --r >= 0;)
            {
                for (int c = min; --c >= 0;)
                {
                    if (r < c)
                    {
                        A.setQuick(r, c, 0);
                    }
                    else if (r == c)
                    {
                        A.setQuick(r, c, 1);
                    }
                }
            }
            if (columns > rows)
            {
                A.viewPart(0, min, rows, columns - min).assign(0);
            }

            return A;
        }

        /**
         *
         */

        public int m()
        {
            return m_LU.Rows();
        }

        /**
         *
         */

        public int n()
        {
            return m_LU.Columns();
        }

        /** 
        Sets the combined lower and upper triangular factor, <tt>LU</tt>.
        The parameter is not checked; make sure it is indeed a proper LU decomposition.
        */

        public void setLU(DoubleMatrix2D LU)
        {
            m_LU = LU;
            isNonSingular = isNonsingular(LU);
        }

        /** 
        Solves the system of equations <tt>A*X = B</tt> (in-place).
        Upon return <tt>B</tt> is overridden with the result <tt>X</tt>, such that <tt>L*U*X = B(piv)</tt>.
        @param  B   A vector with <tt>B.Size() == A.Rows()</tt>.
        @exception  ArgumentException if </tt>B.Size() != A.Rows()</tt>.
        @exception  ArgumentException  if A is singular, that is, if <tt>!isNonsingular()</tt>.
        @exception  ArgumentException  if <tt>A.Rows() < A.Columns()</tt>.
        */

        public void solve(DoubleMatrix1D B)
        {
            algebra.property().checkRectangular(m_LU);
            int m2 = m();
            int n2 = n();
            if (B.Size() != m2)
            {
                throw new ArgumentException("Matrix dimensions must agree.");
            }
            if (!isNonsingular())
            {
                throw new ArgumentException("Matrix is singular.");
            }


            // right hand side with pivoting
            // Matrix Xmat = B.getMatrix(piv,0,nx-1);
            if (workDouble == null || workDouble.Length < m2)
            {
                workDouble = new double[m2];
            }
            algebra.permute(B, piv, workDouble);

            if (m2*n2 == 0)
            {
                return; // nothing to do
            }

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n2; k++)
            {
                double f = B.getQuick(k);
                if (f != 0)
                {
                    for (int i = k + 1; i < n2; i++)
                    {
                        // B[i] -= B[k]*LU[i,k];
                        double v = m_LU.getQuick(i, k);
                        if (v != 0)
                        {
                            B.setQuick(i, B.getQuick(i) - f*v);
                        }
                    }
                }
            }

            // Solve U*B = Y;
            for (int k = n2 - 1; k >= 0; k--)
            {
                // B[k] /= LU[k,k] 
                B.setQuick(k, B.getQuick(k)/m_LU.getQuick(k, k));
                double f = B.getQuick(k);
                if (f != 0)
                {
                    for (int i = 0; i < k; i++)
                    {
                        // B[i] -= B[k]*LU[i,k];
                        double v = m_LU.getQuick(i, k);
                        if (v != 0)
                        {
                            B.setQuick(i, B.getQuick(i) - f*v);
                        }
                    }
                }
            }
        }

        /** 
        Solves the system of equations <tt>A*X = B</tt> (in-place).
        Upon return <tt>B</tt> is overridden with the result <tt>X</tt>, such that <tt>L*U*X = B(piv,:)</tt>.
        @param  B   A matrix with as many rows as <tt>A</tt> and any number of columns.
        @exception  ArgumentException if </tt>B.Rows() != A.Rows()</tt>.
        @exception  ArgumentException  if A is singular, that is, if <tt>!isNonsingular()</tt>.
        @exception  ArgumentException  if <tt>A.Rows() < A.Columns()</tt>.
        */

        public void solve(DoubleMatrix2D B)
        {
            int CUT_OFF = 10;
            algebra.property().checkRectangular(m_LU);
            int m2 = m();
            int n2 = n();
            if (B.Rows() != m2)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!isNonsingular())
            {
                throw new ArgumentException("Matrix is singular.");
            }


            // right hand side with pivoting
            // Matrix Xmat = B.getMatrix(piv,0,nx-1);
            if (work1 == null || work1.Length < m2)
            {
                work1 = new int[m2];
            }
            //if (work2 == null || work2.Length < m) work2 = new int[m];
            algebra.permuteRows(B, piv, work1);

            if (m2*n2 == 0)
            {
                return; // nothing to do
            }
            int nx = B.Columns();

            //precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] Brows = new DoubleMatrix1D[n2];
            for (int k = 0; k < n2; k++)
            {
                Brows[k] = B.viewRow(k);
            }

            // transformations
            Mult div = Mult.div(0);
            PlusMult minusMult = PlusMult.minusMult(0);

            IntArrayList nonZeroIndexes = new IntArrayList(); // sparsity
            DoubleMatrix1D Browk = DoubleFactory1D.dense.make(nx); // blocked row k

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n2; k++)
            {
                // blocking (make copy of k-th row to localize references)		
                Browk.assign(Brows[k]);

                // sparsity detection
                int maxCardinality = nx/CUT_OFF; // == heuristic depending on speedup
                Browk.getNonZeros(nonZeroIndexes, null, maxCardinality);
                int cardinality = nonZeroIndexes.Size();
                bool sparse = (cardinality < maxCardinality);

                for (int i = k + 1; i < n2; i++)
                {
                    //for (int j = 0; j < nx; j++) B[i,j] -= B[k,j]*LU[i,k];
                    //for (int j = 0; j < nx; j++) B.set(i,j, B.get(i,j) - B.get(k,j)*LU.get(i,k));

                    minusMult.m_multiplicator = -m_LU.getQuick(i, k);
                    if (minusMult.m_multiplicator != 0)
                    {
                        if (sparse)
                        {
                            Brows[i].assign(Browk, minusMult, nonZeroIndexes);
                        }
                        else
                        {
                            Brows[i].assign(Browk, minusMult);
                        }
                    }
                }
            }

            // Solve U*B = Y;
            for (int k = n2 - 1; k >= 0; k--)
            {
                // for (int j = 0; j < nx; j++) B[k,j] /= LU[k,k];
                // for (int j = 0; j < nx; j++) B.set(k,j, B.get(k,j) / LU.get(k,k));
                div.m_multiplicator = 1/m_LU.getQuick(k, k);
                Brows[k].assign(div);

                // blocking
                if (Browk == null)
                {
                    Browk = DoubleFactory1D.dense.make(B.Columns());
                }
                Browk.assign(Brows[k]);

                // sparsity detection
                int maxCardinality = nx/CUT_OFF; // == heuristic depending on speedup
                Browk.getNonZeros(nonZeroIndexes, null, maxCardinality);
                int cardinality = nonZeroIndexes.Size();
                bool sparse = (cardinality < maxCardinality);

                //Browk.getNonZeros(nonZeroIndexes,null);
                //bool sparse = nonZeroIndexes.Size() < nx/10;

                for (int i = 0; i < k; i++)
                {
                    // for (int j = 0; j < nx; j++) B[i,j] -= B[k,j]*LU[i,k];
                    // for (int j = 0; j < nx; j++) B.set(i,j, B.get(i,j) - B.get(k,j)*LU.get(i,k));

                    minusMult.m_multiplicator = -m_LU.getQuick(i, k);
                    if (minusMult.m_multiplicator != 0)
                    {
                        if (sparse)
                        {
                            Brows[i].assign(Browk, minusMult, nonZeroIndexes);
                        }
                        else
                        {
                            Brows[i].assign(Browk, minusMult);
                        }
                    }
                }
            }
        }

        /** 
        Solves <tt>A*X = B</tt>.
        @param  B   A matrix with as many rows as <tt>A</tt> and any number of columns.
        @return     <tt>X</tt> so that <tt>L*U*X = B(piv,:)</tt>.
        @exception  ArgumentException if </tt>B.Rows() != A.Rows()</tt>.
        @exception  ArgumentException  if A is singular, that is, if <tt>!isNonsingular()</tt>.
        @exception  ArgumentException  if <tt>A.Rows() < A.Columns()</tt>.
        */

        private void solveOld(DoubleMatrix2D B)
        {
            algebra.property().checkRectangular(m_LU);
            int m2 = m();
            int n2 = n();
            if (B.Rows() != m2)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!isNonsingular())
            {
                throw new ArgumentException("Matrix is singular.");
            }

            // Copy right hand side with pivoting
            int nx = B.Columns();

            if (work1 == null || work1.Length < m2)
            {
                work1 = new int[m2];
            }
            //if (work2 == null || work2.Length < m) work2 = new int[m];
            algebra.permuteRows(B, piv, work1);

            // Solve L*Y = B(piv,:) --> Y (Y is modified B)
            for (int k = 0; k < n2; k++)
            {
                for (int i = k + 1; i < n2; i++)
                {
                    double mult = m_LU.getQuick(i, k);
                    if (mult != 0)
                    {
                        for (int j = 0; j < nx; j++)
                        {
                            //B[i,j] -= B[k,j]*LU[i,k];
                            B.setQuick(i, j, B.getQuick(i, j) - B.getQuick(k, j)*mult);
                        }
                    }
                }
            }
            // Solve U*X = Y; --> X (X is modified B)
            for (int k = n2 - 1; k >= 0; k--)
            {
                double mult = 1/m_LU.getQuick(k, k);
                if (mult != 1)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        //B[k,j] /= LU[k,k];
                        B.setQuick(k, j, B.getQuick(k, j)*mult);
                    }
                }
                for (int i = 0; i < k; i++)
                {
                    mult = m_LU.getQuick(i, k);
                    if (mult != 0)
                    {
                        for (int j = 0; j < nx; j++)
                        {
                            //B[i,j] -= B[k,j]*LU[i,k];
                            B.setQuick(i, j, B.getQuick(i, j) - B.getQuick(k, j)*mult);
                        }
                    }
                }
            }
        }

        /**
        Returns a string with (propertyName, propertyValue) pairs.
        Useful for debugging or to quickly get the rough picture.
        For example,
        <pre>
        rank          : 3
        trace         : 0
        </pre>
        */

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            string unknown = "Illegal operation or error: ";

            buf.Append("-----------------------------------------------------------------------------\n");
            buf.Append("LUDecompositionQuick(A) --> isNonSingular(A), det(A), pivot, L, U, inverse(A)\n");
            buf.Append("-----------------------------------------------------------------------------\n");

            buf.Append("isNonSingular = ");
            try
            {
                buf.Append((isNonsingular()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\ndet = ");
            try
            {
                buf.Append((det()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\npivot = ");
            try
            {
                buf.Append((new IntArrayList(getPivot())));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\nL = ");
            try
            {
                buf.Append((getL()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\nU = ");
            try
            {
                buf.Append((getU()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\ninverse(A) = ");
            DoubleMatrix2D identity = DoubleFactory2D.dense.identity(m_LU.Rows());
            try
            {
                solve(identity);
                buf.Append((identity));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            return buf.ToString();
        }

        /**
        Modifies the matrix to be an upper triangular matrix.
        @return <tt>A</tt> (for convenience only).
        @see #triangulateLower(DoubleMatrix2D)
        */

        public DoubleMatrix2D upperTriangular(DoubleMatrix2D A)
        {
            int rows = A.Rows();
            int columns = A.Columns();
            int min = Math.Min(rows, columns);
            for (int r = min; --r >= 0;)
            {
                for (int c = min; --c >= 0;)
                {
                    if (r > c)
                    {
                        A.setQuick(r, c, 0);
                    }
                }
            }
            if (columns < rows)
            {
                A.viewPart(min, 0, rows - min, columns).assign(0);
            }

            return A;
        }

        /** 
        Returns pivot permutation vector as a one-dimensional double array
        @return     (double) piv
        */

        private double[] xgetDoublePivot()
        {
            int m_ = m();
            double[] vals = new double[m_];
            for (int i = 0; i < m_; i++)
            {
                vals[i] = piv[i];
            }
            return vals;
        }
    }
}
