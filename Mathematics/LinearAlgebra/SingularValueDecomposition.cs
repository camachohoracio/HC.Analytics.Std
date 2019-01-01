#region

using System;

#endregion

namespace HC.Analytics.Mathematics.LinearAlgebra
{
    /** Singular Value Decomposition.
    <P>
    For an m-by-n matrix A with m >= n, the singular value decomposition is
    an m-by-n orthogonal matrix U, an n-by-n diagonal matrix S, and
    an n-by-n orthogonal matrix V so that A = U*S*V'.
    <P>
    The singular values, sigma[k] = S[k][k], are ordered so that
    sigma[0] >= sigma[1] >= ... >= sigma[n-1].
    <P>
    The singular value decompostion always exists, so the constructor will
    never fail.  The matrix condition number and the effective numerical
    rank can be computed from this decomposition.
    */

    [Serializable]
    public class SingularValueDecomposition
    {
        #region Members

        /* ------------------------
           Class variables
         * ------------------------ */

        /** Arrs for internal storage of U and V.
        @serial internal storage of U.
        @serial internal storage of V.
        */
        private readonly double[] m_dblSArr;
        private readonly double[,] m_dblUArr;
        private readonly double[,] m_dblVArr;

        /** Arr for internal storage of singular values.
        @serial internal storage of singular values.
        */

        /** Row and column dimensions.
        @serial row dimension.
        @serial column dimension.
        */
        private readonly int m_intM;
        private readonly int m_intN;

        /* ------------------------
           Constructor
         * ------------------------ */

        /** Construct the singular value decomposition
        @param A    Rectangular matrix
        @return     Structure to access U, S and V.
        */

        #endregion

        public SingularValueDecomposition(MatrixClass Arg)
        {
            // Derived from LINPACK code.
            // Initialize.
            double[,] A = Arg.GetArrCopy();
            m_intM = Arg.GetRowDimension();
            m_intN = Arg.GetColumnDimension();

            /* Apparently the failing cases are only a proper subset of (m<n), 
	 so let's not throw error.  Correct fix to come later?
      if (m<n) {
	  throw new ArgumentException("Jama SVD only works for m >= n"); }
      */
            int nu = Math.Min(m_intM, m_intN);
            m_dblSArr = new double[Math.Min(m_intM + 1, m_intN)];
            m_dblUArr = new double[m_intM,nu];
            m_dblVArr = new double[m_intN,m_intN];
            double[] e = new double[m_intN];
            double[] work = new double[m_intM];
            bool wantu = true;
            bool wantv = true;

            // Reduce A to bidiagonal form, storing the diagonal elements
            // in s and the base-diagonal elements in e.

            int nct = Math.Min(m_intM - 1, m_intN);
            int nrt = Math.Max(0, Math.Min(m_intN - 2, m_intM));
            for (int k = 0; k < Math.Max(nct, nrt); k++)
            {
                if (k < nct)
                {
                    // Compute the transformation for the k-th column and
                    // place the k-th diagonal in s[k].
                    // Compute 2-norm of k-th column without under/overflow.
                    m_dblSArr[k] = 0;
                    for (int i = k; i < m_intM; i++)
                    {
                        m_dblSArr[k] = MathHelper.Hypot(m_dblSArr[k], A[i, k]);
                    }
                    if (m_dblSArr[k] != 0.0)
                    {
                        if (A[k, k] < 0.0)
                        {
                            m_dblSArr[k] = -m_dblSArr[k];
                        }
                        for (int i = k; i < m_intM; i++)
                        {
                            A[i, k] /= m_dblSArr[k];
                        }
                        A[k, k] += 1.0;
                    }
                    m_dblSArr[k] = -m_dblSArr[k];
                }
                for (int j = k + 1; j < m_intN; j++)
                {
                    if ((k < nct) & (m_dblSArr[k] != 0.0))
                    {
                        // Apply the transformation.

                        double t = 0;
                        for (int i = k; i < m_intM; i++)
                        {
                            t += A[i, k]*A[i, j];
                        }
                        t = -t/A[k, k];
                        for (int i = k; i < m_intM; i++)
                        {
                            A[i, j] += t*A[i, k];
                        }
                    }

                    // Place the k-th row of A into e for the
                    // subsequent calculation of the row transformation.

                    e[j] = A[k, j];
                }
                if (wantu & (k < nct))
                {
                    // Place the transformation in U for subsequent back
                    // multiplication.

                    for (int i = k; i < m_intM; i++)
                    {
                        m_dblUArr[i, k] = A[i, k];
                    }
                }
                if (k < nrt)
                {
                    // Compute the k-th row transformation and place the
                    // k-th base-diagonal in e[k].
                    // Compute 2-norm without under/overflow.
                    e[k] = 0;
                    for (int i = k + 1; i < m_intN; i++)
                    {
                        e[k] = MathHelper.Hypot(e[k], e[i]);
                    }
                    if (e[k] != 0.0)
                    {
                        if (e[k + 1] < 0.0)
                        {
                            e[k] = -e[k];
                        }
                        for (int i = k + 1; i < m_intN; i++)
                        {
                            e[i] /= e[k];
                        }
                        e[k + 1] += 1.0;
                    }
                    e[k] = -e[k];
                    if ((k + 1 < m_intM) & (e[k] != 0.0))
                    {
                        // Apply the transformation.

                        for (int i = k + 1; i < m_intM; i++)
                        {
                            work[i] = 0.0;
                        }
                        for (int j = k + 1; j < m_intN; j++)
                        {
                            for (int i = k + 1; i < m_intM; i++)
                            {
                                work[i] += e[j]*A[i, j];
                            }
                        }
                        for (int j = k + 1; j < m_intN; j++)
                        {
                            double t = -e[j]/e[k + 1];
                            for (int i = k + 1; i < m_intM; i++)
                            {
                                A[i, j] += t*work[i];
                            }
                        }
                    }
                    if (wantv)
                    {
                        // Place the transformation in V for subsequent
                        // back multiplication.

                        for (int i = k + 1; i < m_intN; i++)
                        {
                            m_dblVArr[i, k] = e[i];
                        }
                    }
                }
            }

            // Set up the final bidiagonal matrix or order p.

            int p = Math.Min(m_intN, m_intM + 1);
            if (nct < m_intN)
            {
                m_dblSArr[nct] = A[nct, nct];
            }
            if (m_intM < p)
            {
                m_dblSArr[p - 1] = 0.0;
            }
            if (nrt + 1 < p)
            {
                e[nrt] = A[nrt, p - 1];
            }
            e[p - 1] = 0.0;

            // If required, generate U.

            if (wantu)
            {
                for (int j = nct; j < nu; j++)
                {
                    for (int i = 0; i < m_intM; i++)
                    {
                        m_dblUArr[i, j] = 0.0;
                    }
                    m_dblUArr[j, j] = 1.0;
                }
                for (int k = nct - 1; k >= 0; k--)
                {
                    if (m_dblSArr[k] != 0.0)
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            double t = 0;
                            for (int i = k; i < m_intM; i++)
                            {
                                t += m_dblUArr[i, k]*m_dblUArr[i, j];
                            }
                            t = -t/m_dblUArr[k, k];
                            for (int i = k; i < m_intM; i++)
                            {
                                m_dblUArr[i, j] += t*m_dblUArr[i, k];
                            }
                        }
                        for (int i = k; i < m_intM; i++)
                        {
                            m_dblUArr[i, k] = -m_dblUArr[i, k];
                        }
                        m_dblUArr[k, k] = 1.0 + m_dblUArr[k, k];
                        for (int i = 0; i < k - 1; i++)
                        {
                            m_dblUArr[i, k] = 0.0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m_intM; i++)
                        {
                            m_dblUArr[i, k] = 0.0;
                        }
                        m_dblUArr[k, k] = 1.0;
                    }
                }
            }

            // If required, generate V.

            if (wantv)
            {
                for (int k = m_intN - 1; k >= 0; k--)
                {
                    if ((k < nrt) & (e[k] != 0.0))
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            double t = 0;
                            for (int i = k + 1; i < m_intN; i++)
                            {
                                t += m_dblVArr[i, k]*m_dblVArr[i, j];
                            }
                            t = -t/m_dblVArr[k + 1, k];
                            for (int i = k + 1; i < m_intN; i++)
                            {
                                m_dblVArr[i, j] += t*m_dblVArr[i, k];
                            }
                        }
                    }
                    for (int i = 0; i < m_intN; i++)
                    {
                        m_dblVArr[i, k] = 0.0;
                    }
                    m_dblVArr[k, k] = 1.0;
                }
            }

            // Main iteration loop for the singular values.

            int pp = p - 1;
            int iter = 0;
            double eps = Math.Pow(2.0, -52.0);
            double tiny = Math.Pow(2.0, -966.0);
            while (p > 0)
            {
                int k, kase;

                // Here is where a test for too many iterations would go.

                // This section of the program inspects for
                // negligible elements in the s and e arrays.  On
                // completion the variables kase and k are set as follows.

                // kase = 1     if s(p) and e[k-1] are negligible and k<p
                // kase = 2     if s(k) is negligible and k<p
                // kase = 3     if e[k-1] is negligible, k<p, and
                //              s(k), ..., s(p) are not negligible (qr step).
                // kase = 4     if e(p-1) is negligible (convergence).

                for (k = p - 2; k >= -1; k--)
                {
                    if (k == -1)
                    {
                        break;
                    }
                    if (Math.Abs(e[k]) <=
                        tiny + eps*(Math.Abs(m_dblSArr[k]) + Math.Abs(m_dblSArr[k + 1])))
                    {
                        e[k] = 0.0;
                        break;
                    }
                }
                if (k == p - 2)
                {
                    kase = 4;
                }
                else
                {
                    int ks;
                    for (ks = p - 1; ks >= k; ks--)
                    {
                        if (ks == k)
                        {
                            break;
                        }
                        double t = (ks != p ? Math.Abs(e[ks]) : 0) +
                                   (ks != k + 1 ? Math.Abs(e[ks - 1]) : 0);
                        if (Math.Abs(m_dblSArr[ks]) <= tiny + eps*t)
                        {
                            m_dblSArr[ks] = 0.0;
                            break;
                        }
                    }
                    if (ks == k)
                    {
                        kase = 3;
                    }
                    else if (ks == p - 1)
                    {
                        kase = 1;
                    }
                    else
                    {
                        kase = 2;
                        k = ks;
                    }
                }
                k++;

                // Perform the task indicated by kase.

                switch (kase)
                {
                        // Deflate negligible s(p).

                    case 1:
                        {
                            double f = e[p - 2];
                            e[p - 2] = 0.0;
                            for (int j = p - 2; j >= k; j--)
                            {
                                double t = MathHelper.Hypot(m_dblSArr[j], f);
                                double cs = m_dblSArr[j]/t;
                                double sn = f/t;
                                m_dblSArr[j] = t;
                                if (j != k)
                                {
                                    f = -sn*e[j - 1];
                                    e[j - 1] = cs*e[j - 1];
                                }
                                if (wantv)
                                {
                                    for (int i = 0; i < m_intN; i++)
                                    {
                                        t = cs*m_dblVArr[i, j] + sn*m_dblVArr[i, p - 1];
                                        m_dblVArr[i, p - 1] = -sn*m_dblVArr[i, j] + cs*m_dblVArr[i, p - 1];
                                        m_dblVArr[i, j] = t;
                                    }
                                }
                            }
                        }
                        break;

                        // Split at negligible s(k).

                    case 2:
                        {
                            double f = e[k - 1];
                            e[k - 1] = 0.0;
                            for (int j = k; j < p; j++)
                            {
                                double t = MathHelper.Hypot(m_dblSArr[j], f);
                                double cs = m_dblSArr[j]/t;
                                double sn = f/t;
                                m_dblSArr[j] = t;
                                f = -sn*e[j];
                                e[j] = cs*e[j];
                                if (wantu)
                                {
                                    for (int i = 0; i < m_intM; i++)
                                    {
                                        t = cs*m_dblUArr[i, j] + sn*m_dblUArr[i, k - 1];
                                        m_dblUArr[i, k - 1] = -sn*m_dblUArr[i, j] + cs*m_dblUArr[i, k - 1];
                                        m_dblUArr[i, j] = t;
                                    }
                                }
                            }
                        }
                        break;

                        // Perform one qr step.

                    case 3:
                        {
                            // Calculate the shift.

                            double scale = Math.Max(Math.Max(Math.Max(Math.Max(
                                                                          Math.Abs(m_dblSArr[p - 1]),
                                                                          Math.Abs(m_dblSArr[p - 2])),
                                                                      Math.Abs(e[p - 2])),
                                                             Math.Abs(m_dblSArr[k])), Math.Abs(e[k]));
                            double sp = m_dblSArr[p - 1]/scale;
                            double spm1 = m_dblSArr[p - 2]/scale;
                            double epm1 = e[p - 2]/scale;
                            double sk = m_dblSArr[k]/scale;
                            double ek = e[k]/scale;
                            double b = ((spm1 + sp)*(spm1 - sp) + epm1*epm1)/2.0;
                            double c = (sp*epm1)*(sp*epm1);
                            double shift = 0.0;
                            if ((b != 0.0) | (c != 0.0))
                            {
                                shift = Math.Sqrt(b*b + c);
                                if (b < 0.0)
                                {
                                    shift = -shift;
                                }
                                shift = c/(b + shift);
                            }
                            double f = (sk + sp)*(sk - sp) + shift;
                            double g = sk*ek;

                            // Chase zeros.

                            for (int j = k; j < p - 1; j++)
                            {
                                double t = MathHelper.Hypot(f, g);
                                double cs = f/t;
                                double sn = g/t;
                                if (j != k)
                                {
                                    e[j - 1] = t;
                                }
                                f = cs*m_dblSArr[j] + sn*e[j];
                                e[j] = cs*e[j] - sn*m_dblSArr[j];
                                g = sn*m_dblSArr[j + 1];
                                m_dblSArr[j + 1] = cs*m_dblSArr[j + 1];
                                if (wantv)
                                {
                                    for (int i = 0; i < m_intN; i++)
                                    {
                                        t = cs*m_dblVArr[i, j] + sn*m_dblVArr[i, j + 1];
                                        m_dblVArr[i, j + 1] = -sn*m_dblVArr[i, j] + cs*m_dblVArr[i, j + 1];
                                        m_dblVArr[i, j] = t;
                                    }
                                }
                                t = MathHelper.Hypot(f, g);
                                cs = f/t;
                                sn = g/t;
                                m_dblSArr[j] = t;
                                f = cs*e[j] + sn*m_dblSArr[j + 1];
                                m_dblSArr[j + 1] = -sn*e[j] + cs*m_dblSArr[j + 1];
                                g = sn*e[j + 1];
                                e[j + 1] = cs*e[j + 1];
                                if (wantu && (j < m_intM - 1))
                                {
                                    for (int i = 0; i < m_intM; i++)
                                    {
                                        t = cs*m_dblUArr[i, j] + sn*m_dblUArr[i, j + 1];
                                        m_dblUArr[i, j + 1] = -sn*m_dblUArr[i, j] + cs*m_dblUArr[i, j + 1];
                                        m_dblUArr[i, j] = t;
                                    }
                                }
                            }
                            e[p - 2] = f;
                            iter = iter + 1;
                        }
                        break;

                        // Convergence.

                    case 4:
                        {
                            // Make the singular values positive.

                            if (m_dblSArr[k] <= 0.0)
                            {
                                m_dblSArr[k] = (m_dblSArr[k] < 0.0 ? -m_dblSArr[k] : 0.0);
                                if (wantv)
                                {
                                    for (int i = 0; i <= pp; i++)
                                    {
                                        m_dblVArr[i, k] = -m_dblVArr[i, k];
                                    }
                                }
                            }

                            // Order the singular values.

                            while (k < pp)
                            {
                                if (m_dblSArr[k] >= m_dblSArr[k + 1])
                                {
                                    break;
                                }
                                double t = m_dblSArr[k];
                                m_dblSArr[k] = m_dblSArr[k + 1];
                                m_dblSArr[k + 1] = t;
                                if (wantv && (k < m_intN - 1))
                                {
                                    for (int i = 0; i < m_intN; i++)
                                    {
                                        t = m_dblVArr[i, k + 1];
                                        m_dblVArr[i, k + 1] = m_dblVArr[i, k];
                                        m_dblVArr[i, k] = t;
                                    }
                                }
                                if (wantu && (k < m_intM - 1))
                                {
                                    for (int i = 0; i < m_intM; i++)
                                    {
                                        t = m_dblUArr[i, k + 1];
                                        m_dblUArr[i, k + 1] = m_dblUArr[i, k];
                                        m_dblUArr[i, k] = t;
                                    }
                                }
                                k++;
                            }
                            iter = 0;
                            p--;
                        }
                        break;
                }
            }
        }

        /* ------------------------
           Public Methods
         * ------------------------ */

        /** Return the left singular vectors
        @return     U
        */

        public MatrixClass getU()
        {
            return new MatrixClass(m_dblUArr, m_intM, Math.Min(m_intM + 1, m_intN));
        }

        /** Return the right singular vectors
        @return     V
        */

        public MatrixClass getV()
        {
            return new MatrixClass(m_dblVArr, m_intN, m_intN);
        }

        /** Return the one-dimensional array of singular values
        @return     diagonal of S.
        */

        public double[] getSingularValues()
        {
            return m_dblSArr;
        }

        /** Return the diagonal matrix of singular values
        @return     S
        */

        public MatrixClass getS()
        {
            MatrixClass X = new MatrixClass(m_intN, m_intN);
            double[,] S = X.GetArr();
            for (int i = 0; i < m_intN; i++)
            {
                for (int j = 0; j < m_intN; j++)
                {
                    S[i, j] = 0.0;
                }
                S[i, i] = m_dblSArr[i];
            }
            return X;
        }

        /** Two norm
        @return     Max(S)
        */

        public double norm2()
        {
            return m_dblSArr[0];
        }

        /** Two norm condition number
        @return     Max(S)/Min(S)
        */

        public double cond()
        {
            return m_dblSArr[0]/m_dblSArr[Math.Min(m_intM, m_intN) - 1];
        }

        /** Effective numerical matrix rank
        @return     Number of nonnegligible singular values.
        */

        public int rank()
        {
            double eps = Math.Pow(2.0, -52.0);
            double tol = Math.Max(m_intM, m_intN)*m_dblSArr[0]*eps;
            int r = 0;
            for (int i = 0; i < m_dblSArr.Length; i++)
            {
                if (m_dblSArr[i] > tol)
                {
                    r++;
                }
            }
            return r;
        }
    }
}
