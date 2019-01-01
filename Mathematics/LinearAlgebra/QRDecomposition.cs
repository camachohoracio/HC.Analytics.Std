#region

using System;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Mathematics.LinearAlgebra
{
    /** QR Decomposition.
    <P>
       For an m-by-n matrix A with m >= n, the QR decomposition is an m-by-n
       orthogonal matrix Q and an n-by-n upper triangular matrix R so that
       A = Q*R.
    <P>
       The QR decompostion always exists, even if the matrix does not have
       full rank, so the constructor will never fail.  The primary use of the
       QR decomposition is in the least squares solution of nonsquare systems
       of simultaneous linear equations.  This will fail if isFullRank()
       returns false.
    */

    [Serializable]
    public class QRDecomposition
    {
        /* ------------------------
           Class variables
         * ------------------------ */

        /** Arr for internal storage of decomposition.
        @serial internal array storage.
        */

        /** Row and column dimensions.
        @serial column dimension.
        @serial row dimension.
        */
        private readonly int m;
        private readonly int n;
        private readonly double[,] QR;

        /** Arr for internal storage of diagonal of R.
        @serial diagonal of R.
        */
        private readonly double[] Rdiag;

        /* ------------------------
           Constructor
         * ------------------------ */

        /** QR Decomposition, computed by Householder reflections.
        @param A    Rectangular matrix
        @return     Structure to access R and the Householder vectors and compute Q.
        */

        public QRDecomposition(MatrixClass A)
        {
            // Initialize.
            QR = A.GetArrCopy();
            m = A.GetRowDimension();
            n = A.GetColumnDimension();
            Rdiag = new double[n];

            // Main loop.
            for (int k = 0; k < n; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (int i = k; i < m; i++)
                {
                    nrm = MathHelper.Hypot(nrm, QR[i, k]);
                }

                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (QR[k, k] < 0)
                    {
                        nrm = -nrm;
                    }
                    for (int i = k; i < m; i++)
                    {
                        QR[i, k] /= nrm;
                    }
                    QR[k, k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++)
                    {
                        double s = 0.0;
                        for (int i = k; i < m; i++)
                        {
                            s += QR[i, k]*QR[i, j];
                        }
                        s = -s/QR[k, k];
                        for (int i = k; i < m; i++)
                        {
                            QR[i, j] += s*QR[i, k];
                        }
                    }
                }
                Rdiag[k] = -nrm;
            }
        }

        /* ------------------------
           Public Methods
         * ------------------------ */

        /** Is the matrix full rank?
        @return     true if R, and hence A, has full rank.
        */

        public bool IsFullRank()
        {
            for (int j = 0; j < n; j++)
            {
                if (Rdiag[j] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /** Return the Householder vectors
        @return     Lower trapezoidal matrix whose columns define the reflections
        */

        public MatrixClass getH()
        {
            MatrixClass X = new MatrixClass(m, n);
            double[,] H = X.GetArr();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i >= j)
                    {
                        H[i, j] = QR[i, j];
                    }
                    else
                    {
                        H[i, j] = 0.0;
                    }
                }
            }
            return X;
        }

        /** Return the upper triangular factor
        @return     R
        */

        public MatrixClass getR()
        {
            MatrixClass X = new MatrixClass(n, n);
            double[,] R = X.GetArr();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i < j)
                    {
                        R[i, j] = QR[i, j];
                    }
                    else if (i == j)
                    {
                        R[i, j] = Rdiag[i];
                    }
                    else
                    {
                        R[i, j] = 0.0;
                    }
                }
            }
            return X;
        }

        /** Generate and return the (economy-sized) orthogonal factor
        @return     Q
        */

        public MatrixClass getQ()
        {
            MatrixClass X = new MatrixClass(m, n);
            double[,] Q = X.GetArr();
            for (int k = n - 1; k >= 0; k--)
            {
                for (int i = 0; i < m; i++)
                {
                    Q[i, k] = 0.0;
                }
                Q[k, k] = 1.0;
                for (int j = k; j < n; j++)
                {
                    if (QR[k, k] != 0)
                    {
                        double s = 0.0;
                        for (int i = k; i < m; i++)
                        {
                            s += QR[i, k]*Q[i, j];
                        }
                        s = -s/QR[k, k];
                        for (int i = k; i < m; i++)
                        {
                            Q[i, j] += s*QR[i, k];
                        }
                    }
                }
            }
            return X;
        }

        /** Least squares solution of A*X = B
        @param B    A Matrix with as many rows as A and any number of columns.
        @return     X that minimizes the two norm of Q*R*X-B.
        @exception  HCException  Matrix row dimensions must agree.
        @exception  RuntimeException  Matrix is rank deficient.
        */

        public MatrixClass solve(MatrixClass B)
        {
            if (B.GetRowDimension() != m)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!IsFullRank())
            {
                throw new HCException("Matrix is rank deficient.");
            }

            // Copy right hand side
            int nx = B.GetColumnDimension();
            double[,] X = B.GetArrCopy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < nx; j++)
                {
                    double s = 0.0;
                    for (int i = k; i < m; i++)
                    {
                        s += QR[i, k]*X[i, j];
                    }
                    s = -s/QR[k, k];
                    for (int i = k; i < m; i++)
                    {
                        X[i, j] += s*QR[i, k];
                    }
                }
            }
            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X[k, j] /= Rdiag[k];
                }
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i, j] -= X[k, j]*QR[i, k];
                    }
                }
            }
            return (new MatrixClass(X, n, nx).getMatrix(0, n - 1, 0, nx - 1));
        }
    }
}
