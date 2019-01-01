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
    For an <tt>m x n</tt> matrix <tt>A</tt> with <tt>m >= n</tt>, the QR decomposition is an <tt>m x n</tt>
    orthogonal matrix <tt>Q</tt> and an <tt>n x n</tt> upper triangular matrix <tt>R</tt> so that
    <tt>A = Q*R</tt>.
    <P>
    The QR decompostion always exists, even if the matrix does not have
    full rank, so the constructor will never fail.  The primary use of the
    QR decomposition is in the least squares solution of nonsquare systems
    of simultaneous linear equations.  This will fail if <tt>isFullRank()</tt>
    returns <tt>false</tt>.
    */

    [Serializable]
    public class QRDecomposition
    {
        //static long serialVersionUID = 1020;
        /** Array for internal storage of decomposition.
        @serial internal array storage.
        */
        //private double[,] QR;

        /** Row and column dimensions.
        @serial column dimension.
        @serial row dimension.
        */
        private readonly int m;
        private readonly int n;
        private readonly DoubleMatrix2D QR;

        /** Array for internal storage of diagonal of R.
        @serial diagonal of R.
        */
        private readonly DoubleMatrix1D Rdiag;
        /** 
        Constructs and returns a new QR decomposition object;  computed by Householder reflections;
        The decomposed matrices can be retrieved via instance methods of the returned decomposition object.
        @param A    A rectangular matrix.
        @return     a decomposition object to access <tt>R</tt> and the Householder vectors <tt>H</tt>, and to compute <tt>Q</tt>.
        @throws ArgumentException if <tt>A.Rows() < A.Columns()</tt>.
        */

        public QRDecomposition(DoubleMatrix2D A)
        {
            Property.DEFAULT.checkRectangular(A);

            Functions F = Functions.m_functions;
            // Initialize.
            QR = A.Copy();
            m = A.Rows();
            n = A.Columns();
            Rdiag = A.like1D(n);
            //Rdiag = new double[n];
            DoubleDoubleFunction hypot = Algebra.hypotFunction();

            // precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] QRcolumns = new DoubleMatrix1D[n];
            DoubleMatrix1D[] QRcolumnsPart = new DoubleMatrix1D[n];
            for (int k = 0; k < n; k++)
            {
                QRcolumns[k] = QR.viewColumn(k);
                QRcolumnsPart[k] = QR.viewColumn(k).viewPart(k, m - k);
            }

            // Main loop.
            for (int k = 0; k < n; k++)
            {
                //DoubleMatrix1D QRcolk = QR.viewColumn(k).viewPart(k,m-k);
                // Compute 2-norm of k-th column without under/overflow.

                double nrm = 0;
                //if (k<m) nrm = QRcolumnsPart[k].aggregate(hypot,Functions.identity);

                for (int i = k; i < m; i++)
                {
                    // fixes bug reported by hong.44@osu.edu
                    nrm = Algebra.hypot(nrm, QR.getQuick(i, k));
                }


                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (QR.getQuick(k, k) < 0)
                    {
                        nrm = -nrm;
                    }
                    QRcolumnsPart[k].assign(Functions.div(nrm));
                    /*
                    for (int i = k; i < m; i++) {
                       QR[i,k] /= nrm;
                    }
                    */

                    QR.setQuick(k, k, QR.getQuick(k, k) + 1);

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++)
                    {
                        DoubleMatrix1D QRcolj = QR.viewColumn(j).viewPart(k, m - k);
                        double s = QRcolumnsPart[k].zDotProduct(QRcolj);
                        /*
                        // fixes bug reported by John Chambers
                        DoubleMatrix1D QRcolj = QR.viewColumn(j).viewPart(k,m-k);
                        double s = QRcolumnsPart[k].zDotProduct(QRcolumns[j]);
                        double s = 0.0; 
                        for (int i = k; i < m; i++) {
                          s += QR[i,k]*QR[i,j];
                        }
                        */
                        s = -s/QR.getQuick(k, k);
                        //QRcolumnsPart[j].assign(QRcolumns[k], Functions.plusMult(s));

                        for (int i = k; i < m; i++)
                        {
                            QR.setQuick(i, j, QR.getQuick(i, j) + s*QR.getQuick(i, k));
                        }
                    }
                }
                Rdiag.setQuick(k, -nrm);
            }
        }

        /** 
        Returns the Householder vectors <tt>H</tt>.
        @return A lower trapezoidal matrix whose columns define the householder reflections.
        */

        public DoubleMatrix2D getH()
        {
            return Algebra.DEFAULT.trapezoidalLower(QR.Copy());
        }

        /** 
        Generates and returns the (economy-sized) orthogonal factor <tt>Q</tt>.
        @return <tt>Q</tt>
        */

        public DoubleMatrix2D getQ()
        {
            Functions F = Functions.m_functions;
            DoubleMatrix2D Q = QR.like();
            //double[,] Q = X.getArray();
            for (int k = n - 1; k >= 0; k--)
            {
                DoubleMatrix1D QRcolk = QR.viewColumn(k).viewPart(k, m - k);
                Q.setQuick(k, k, 1);
                for (int j = k; j < n; j++)
                {
                    if (QR.getQuick(k, k) != 0)
                    {
                        DoubleMatrix1D Qcolj = Q.viewColumn(j).viewPart(k, m - k);
                        double s = QRcolk.zDotProduct(Qcolj);
                        s = -s/QR.getQuick(k, k);
                        Qcolj.assign(QRcolk, Functions.plusMult(s));
                    }
                }
            }
            return Q;
        }

        /** 
        Returns the upper triangular factor, <tt>R</tt>.
        @return <tt>R</tt>
        */

        public DoubleMatrix2D getR()
        {
            DoubleMatrix2D R = QR.like(n, n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i < j)
                    {
                        R.setQuick(i, j, QR.getQuick(i, j));
                    }
                    else if (i == j)
                    {
                        R.setQuick(i, j, Rdiag.getQuick(i));
                    }
                    else
                    {
                        R.setQuick(i, j, 0);
                    }
                }
            }
            return R;
        }

        /** 
        Returns whether the matrix <tt>A</tt> has full rank.
        @return true if <tt>R</tt>, and hence <tt>A</tt>, has full rank.
        */

        public bool hasFullRank()
        {
            for (int j = 0; j < n; j++)
            {
                if (Rdiag.getQuick(j) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /** 
        Least squares solution of <tt>A*X = B</tt>; <tt>returns X</tt>.
        @param B    A matrix with as many rows as <tt>A</tt> and any number of columns.
        @return     <tt>X</tt> that minimizes the two norm of <tt>Q*R*X - B</tt>.
        @exception  ArgumentException  if <tt>B.Rows() != A.Rows()</tt>.
        @exception  ArgumentException  if <tt>!hasFullRank()</tt> (<tt>A</tt> is rank deficient).
        */

        public DoubleMatrix2D solve(DoubleMatrix2D B)
        {
            Functions F = Functions.m_functions;
            if (B.Rows() != m)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!hasFullRank())
            {
                throw new ArgumentException("Matrix is rank deficient.");
            }

            // Copy right hand side
            int nx = B.Columns();
            DoubleMatrix2D X = B.Copy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < nx; j++)
                {
                    double s = 0.0;
                    for (int i = k; i < m; i++)
                    {
                        s += QR.getQuick(i, k)*X.getQuick(i, j);
                    }
                    s = -s/QR.getQuick(k, k);
                    for (int i = k; i < m; i++)
                    {
                        X.setQuick(i, j, X.getQuick(i, j) + s*QR.getQuick(i, k));
                    }
                }
            }
            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X.setQuick(k, j, X.getQuick(k, j)/Rdiag.getQuick(k));
                }
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X.setQuick(i, j, X.getQuick(i, j) - X.getQuick(k, j)*QR.getQuick(i, k));
                    }
                }
            }
            return X.viewPart(0, 0, n, nx);
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

            buf.Append("-----------------------------------------------------------------\n");
            buf.Append("QRDecomposition(A) --> hasFullRank(A), H, Q, R, pseudo inverse(A)\n");
            buf.Append("-----------------------------------------------------------------\n");

            buf.Append("hasFullRank = ");
            try
            {
                buf.Append((hasFullRank()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\nH = ");
            try
            {
                buf.Append((getH()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\nQ = ");
            try
            {
                buf.Append((getQ()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\nR = ");
            try
            {
                buf.Append((getR()));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            buf.Append("\n\npseudo inverse(A) = ");
            try
            {
                buf.Append((solve(DoubleFactory2D.dense.identity(QR.Rows()))));
            }
            catch (ArgumentException exc)
            {
                buf.Append(unknown + exc.Message);
            }

            return buf.ToString();
        }
    }
}
