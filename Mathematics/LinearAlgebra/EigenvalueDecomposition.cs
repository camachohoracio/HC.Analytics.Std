#region

using System;

#endregion

namespace HC.Analytics.Mathematics.LinearAlgebra
{
    /** Eigenvalues and eigenvectors of a real matrix. 
    <P>
        If A is symmetric, then A = V*D*V' where the eigenvalue matrix D is
        diagonal and the eigenvector matrix V is orthogonal.
        I.e. A = V.times(D.times(V.transpose())) and 
        V.times(V.transpose()) equals the identity matrix.
    <P>
        If A is not symmetric, then the eigenvalue matrix D is block diagonal
        with the real eigenvalues in 1-by-1 blocks and any complex eigenvalues,
        lambda + i*mu, in 2-by-2 blocks, [lambda, mu; -mu, lambda].  The
        columns of V represent the eigenvectors in the sense that A*V = V*D,
        i.e. A.times(V) equals V.times(D).  The matrix V may be badly
        conditioned, or even singular, so the validity of the equation
        A = V*D*inverse(V) depends upon V.cond().
    **/

    [Serializable]
    public class EigenvalueDecomposition
    {
        #region Members

        /* ------------------------
           Class variables
         * ------------------------ */

        /** Row and column dimension (square matrix).
        @serial matrix dimension.
        */

        /** Symmetry flag.
        @serial internal symmetry flag.
        */
        private readonly bool m_blnIssymmetric;

        /** Arrs for internal storage of eigenvalues.
        @serial internal storage of eigenvalues.
        */
        private readonly double[] m_dblD;
        private readonly double[] m_dblE;

        /** Arr for internal storage of eigenvectors.
        @serial internal storage of eigenvectors.
        */

        /** Arr for internal storage of nonsymmetric Hessenberg form.
        @serial internal storage of nonsymmetric Hessenberg form.
        */
        private readonly double[,] m_dblHArr;

        /** Working storage for nonsymmetric algorithm.
        @serial working storage for nonsymmetric algorithm.
        */
        private readonly double[] m_dblOrtArr;
        private readonly double[,] m_dblVArr;
        private readonly int m_intN;

        #endregion

        /* ------------------------
           Private Methods
         * ------------------------ */

        // Symmetric Householder reduction to tridiagonal form.
        private double cdivi;
        private double cdivr;

        public EigenvalueDecomposition(MatrixClass Arg)
        {
            double[,] A = Arg.GetArr();
            m_intN = Arg.GetColumnDimension();
            m_dblVArr = new double[m_intN,m_intN];
            m_dblD = new double[m_intN];
            m_dblE = new double[m_intN];

            m_blnIssymmetric = true;
            for (int j = 0; (j < m_intN) & m_blnIssymmetric; j++)
            {
                for (int i = 0; (i < m_intN) & m_blnIssymmetric; i++)
                {
                    m_blnIssymmetric = (A[i, j] == A[j, i]);
                }
            }

            if (m_blnIssymmetric)
            {
                for (int i = 0; i < m_intN; i++)
                {
                    for (int j = 0; j < m_intN; j++)
                    {
                        m_dblVArr[i, j] = A[i, j];
                    }
                }

                // Tridiagonalize.
                tred2();

                // Diagonalize.
                tql2();
            }
            else
            {
                m_dblHArr = new double[m_intN,m_intN];
                m_dblOrtArr = new double[m_intN];

                for (int j = 0; j < m_intN; j++)
                {
                    for (int i = 0; i < m_intN; i++)
                    {
                        m_dblHArr[i, j] = A[i, j];
                    }
                }

                // Reduce to Hessenberg form.
                orthes();

                // Reduce Hessenberg to real Schur form.
                hqr2();
            }
        }

        private void tred2()
        {
            //  This is derived from the Algol procedures tred2 by
            //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
            //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
            //  Fortran subroutine in EISPACK.

            for (int j = 0; j < m_intN; j++)
            {
                m_dblD[j] = m_dblVArr[m_intN - 1, j];
            }

            // Householder reduction to tridiagonal form.

            for (int i = m_intN - 1; i > 0; i--)
            {
                // Scale to avoid under/overflow.

                double scale = 0.0;
                double h = 0.0;
                for (int k = 0; k < i; k++)
                {
                    scale = scale + Math.Abs(m_dblD[k]);
                }
                if (scale == 0.0)
                {
                    m_dblE[i] = m_dblD[i - 1];
                    for (int j = 0; j < i; j++)
                    {
                        m_dblD[j] = m_dblVArr[i - 1, j];
                        m_dblVArr[i, j] = 0.0;
                        m_dblVArr[j, i] = 0.0;
                    }
                }
                else
                {
                    // Generate Householder vector.

                    for (int k = 0; k < i; k++)
                    {
                        m_dblD[k] /= scale;
                        h += m_dblD[k]*m_dblD[k];
                    }
                    double f = m_dblD[i - 1];
                    double g = Math.Sqrt(h);
                    if (f > 0)
                    {
                        g = -g;
                    }
                    m_dblE[i] = scale*g;
                    h = h - f*g;
                    m_dblD[i - 1] = f - g;
                    for (int j = 0; j < i; j++)
                    {
                        m_dblE[j] = 0.0;
                    }

                    // Apply similarity transformation to remaining columns.

                    for (int j = 0; j < i; j++)
                    {
                        f = m_dblD[j];
                        m_dblVArr[j, i] = f;
                        g = m_dblE[j] + m_dblVArr[j, j]*f;
                        for (int k = j + 1; k <= i - 1; k++)
                        {
                            g += m_dblVArr[k, j]*m_dblD[k];
                            m_dblE[k] += m_dblVArr[k, j]*f;
                        }
                        m_dblE[j] = g;
                    }
                    f = 0.0;
                    for (int j = 0; j < i; j++)
                    {
                        m_dblE[j] /= h;
                        f += m_dblE[j]*m_dblD[j];
                    }
                    double hh = f/(h + h);
                    for (int j = 0; j < i; j++)
                    {
                        m_dblE[j] -= hh*m_dblD[j];
                    }
                    for (int j = 0; j < i; j++)
                    {
                        f = m_dblD[j];
                        g = m_dblE[j];
                        for (int k = j; k <= i - 1; k++)
                        {
                            m_dblVArr[k, j] -= (f*m_dblE[k] + g*m_dblD[k]);
                        }
                        m_dblD[j] = m_dblVArr[i - 1, j];
                        m_dblVArr[i, j] = 0.0;
                    }
                }
                m_dblD[i] = h;
            }

            // Accumulate transformations.

            for (int i = 0; i < m_intN - 1; i++)
            {
                m_dblVArr[m_intN - 1, i] = m_dblVArr[i, i];
                m_dblVArr[i, i] = 1.0;
                double h = m_dblD[i + 1];
                if (h != 0.0)
                {
                    for (int k = 0; k <= i; k++)
                    {
                        m_dblD[k] = m_dblVArr[k, i + 1]/h;
                    }
                    for (int j = 0; j <= i; j++)
                    {
                        double g = 0.0;
                        for (int k = 0; k <= i; k++)
                        {
                            g += m_dblVArr[k, i + 1]*m_dblVArr[k, j];
                        }
                        for (int k = 0; k <= i; k++)
                        {
                            m_dblVArr[k, j] -= g*m_dblD[k];
                        }
                    }
                }
                for (int k = 0; k <= i; k++)
                {
                    m_dblVArr[k, i + 1] = 0.0;
                }
            }
            for (int j = 0; j < m_intN; j++)
            {
                m_dblD[j] = m_dblVArr[m_intN - 1, j];
                m_dblVArr[m_intN - 1, j] = 0.0;
            }
            m_dblVArr[m_intN - 1, m_intN - 1] = 1.0;
            m_dblE[0] = 0.0;
        }

        // Symmetric tridiagonal QL algorithm.

        private void tql2()
        {
            //  This is derived from the Algol procedures tql2, by
            //  Bowdler, Martin, Reinsch, and Wilkinson, Handbook for
            //  Auto. Comp., Vol.ii-Linear Algebra, and the corresponding
            //  Fortran subroutine in EISPACK.

            for (int i = 1; i < m_intN; i++)
            {
                m_dblE[i - 1] = m_dblE[i];
            }
            m_dblE[m_intN - 1] = 0.0;

            double f = 0.0;
            double tst1 = 0.0;
            double eps = Math.Pow(2.0, -52.0);
            for (int l = 0; l < m_intN; l++)
            {
                // Find small subdiagonal element

                tst1 = Math.Max(tst1, Math.Abs(m_dblD[l]) + Math.Abs(m_dblE[l]));
                int m = l;
                while (m < m_intN)
                {
                    if (Math.Abs(m_dblE[m]) <= eps*tst1)
                    {
                        break;
                    }
                    m++;
                }

                // If m == l, d[l] is an eigenvalue,
                // otherwise, iterate.

                if (m > l)
                {
                    int iter = 0;
                    do
                    {
                        iter = iter + 1; // (Could check iteration count here.)

                        // Compute implicit shift

                        double g = m_dblD[l];
                        double p = (m_dblD[l + 1] - g)/(2.0*m_dblE[l]);
                        double r = MathHelper.Hypot(p, 1.0);
                        if (p < 0)
                        {
                            r = -r;
                        }
                        m_dblD[l] = m_dblE[l]/(p + r);
                        m_dblD[l + 1] = m_dblE[l]*(p + r);
                        double dl1 = m_dblD[l + 1];
                        double h = g - m_dblD[l];
                        for (int i = l + 2; i < m_intN; i++)
                        {
                            m_dblD[i] -= h;
                        }
                        f = f + h;

                        // Implicit QL transformation.

                        p = m_dblD[m];
                        double c = 1.0;
                        double c2 = c;
                        double c3 = c;
                        double el1 = m_dblE[l + 1];
                        double s = 0.0;
                        double s2 = 0.0;
                        for (int i = m - 1; i >= l; i--)
                        {
                            c3 = c2;
                            c2 = c;
                            s2 = s;
                            g = c*m_dblE[i];
                            h = c*p;
                            r = MathHelper.Hypot(p, m_dblE[i]);
                            m_dblE[i + 1] = s*r;
                            s = m_dblE[i]/r;
                            c = p/r;
                            p = c*m_dblD[i] - s*g;
                            m_dblD[i + 1] = h + s*(c*g + s*m_dblD[i]);

                            // Accumulate transformation.

                            for (int k = 0; k < m_intN; k++)
                            {
                                h = m_dblVArr[k, i + 1];
                                m_dblVArr[k, i + 1] = s*m_dblVArr[k, i] + c*h;
                                m_dblVArr[k, i] = c*m_dblVArr[k, i] - s*h;
                            }
                        }
                        p = -s*s2*c3*el1*m_dblE[l]/dl1;
                        m_dblE[l] = s*p;
                        m_dblD[l] = c*p;

                        // Check for convergence.
                    }
                    while (Math.Abs(m_dblE[l]) > eps*tst1);
                }
                m_dblD[l] = m_dblD[l] + f;
                m_dblE[l] = 0.0;
            }

            // Sort eigenvalues and corresponding vectors.

            for (int i = 0; i < m_intN - 1; i++)
            {
                int k = i;
                double p = m_dblD[i];
                for (int j = i + 1; j < m_intN; j++)
                {
                    if (m_dblD[j] < p)
                    {
                        k = j;
                        p = m_dblD[j];
                    }
                }
                if (k != i)
                {
                    m_dblD[k] = m_dblD[i];
                    m_dblD[i] = p;
                    for (int j = 0; j < m_intN; j++)
                    {
                        p = m_dblVArr[j, i];
                        m_dblVArr[j, i] = m_dblVArr[j, k];
                        m_dblVArr[j, k] = p;
                    }
                }
            }
        }

        // Nonsymmetric reduction to Hessenberg form.

        private void orthes()
        {
            //  This is derived from the Algol procedures orthes and ortran,
            //  by Martin and Wilkinson, Handbook for Auto. Comp.,
            //  Vol.ii-Linear Algebra, and the corresponding
            //  Fortran subroutines in EISPACK.

            int low = 0;
            int high = m_intN - 1;

            for (int m = low + 1; m <= high - 1; m++)
            {
                // Scale column.

                double scale = 0.0;
                for (int i = m; i <= high; i++)
                {
                    scale = scale + Math.Abs(m_dblHArr[i, m - 1]);
                }
                if (scale != 0.0)
                {
                    // Compute Householder transformation.

                    double h = 0.0;
                    for (int i = high; i >= m; i--)
                    {
                        m_dblOrtArr[i] = m_dblHArr[i, m - 1]/scale;
                        h += m_dblOrtArr[i]*m_dblOrtArr[i];
                    }
                    double g = Math.Sqrt(h);
                    if (m_dblOrtArr[m] > 0)
                    {
                        g = -g;
                    }
                    h = h - m_dblOrtArr[m]*g;
                    m_dblOrtArr[m] = m_dblOrtArr[m] - g;

                    // Apply Householder similarity transformation
                    // H = (I-u*u'/h)*H*(I-u*u')/h)

                    for (int j = m; j < m_intN; j++)
                    {
                        double f = 0.0;
                        for (int i = high; i >= m; i--)
                        {
                            f += m_dblOrtArr[i]*m_dblHArr[i, j];
                        }
                        f = f/h;
                        for (int i = m; i <= high; i++)
                        {
                            m_dblHArr[i, j] -= f*m_dblOrtArr[i];
                        }
                    }

                    for (int i = 0; i <= high; i++)
                    {
                        double f = 0.0;
                        for (int j = high; j >= m; j--)
                        {
                            f += m_dblOrtArr[j]*m_dblHArr[i, j];
                        }
                        f = f/h;
                        for (int j = m; j <= high; j++)
                        {
                            m_dblHArr[i, j] -= f*m_dblOrtArr[j];
                        }
                    }
                    m_dblOrtArr[m] = scale*m_dblOrtArr[m];
                    m_dblHArr[m, m - 1] = scale*g;
                }
            }

            // Accumulate transformations (Algol's ortran).

            for (int i = 0; i < m_intN; i++)
            {
                for (int j = 0; j < m_intN; j++)
                {
                    m_dblVArr[i, j] = (i == j ? 1.0 : 0.0);
                }
            }

            for (int m = high - 1; m >= low + 1; m--)
            {
                if (m_dblHArr[m, m - 1] != 0.0)
                {
                    for (int i = m + 1; i <= high; i++)
                    {
                        m_dblOrtArr[i] = m_dblHArr[i, m - 1];
                    }
                    for (int j = m; j <= high; j++)
                    {
                        double g = 0.0;
                        for (int i = m; i <= high; i++)
                        {
                            g += m_dblOrtArr[i]*m_dblVArr[i, j];
                        }
                        // Double division avoids possible underflow
                        g = (g/m_dblOrtArr[m])/m_dblHArr[m, m - 1];
                        for (int i = m; i <= high; i++)
                        {
                            m_dblVArr[i, j] += g*m_dblOrtArr[i];
                        }
                    }
                }
            }
        }


        // Complex scalar division.

        private void cdiv(double xr, double xi, double yr, double yi)
        {
            double r, d;
            if (Math.Abs(yr) > Math.Abs(yi))
            {
                r = yi/yr;
                d = yr + r*yi;
                cdivr = (xr + r*xi)/d;
                cdivi = (xi - r*xr)/d;
            }
            else
            {
                r = yr/yi;
                d = yi + r*yr;
                cdivr = (r*xr + xi)/d;
                cdivi = (r*xi - xr)/d;
            }
        }


        // Nonsymmetric reduction from Hessenberg to real Schur form.

        private void hqr2()
        {
            //  This is derived from the Algol procedure hqr2,
            //  by Martin and Wilkinson, Handbook for Auto. Comp.,
            //  Vol.ii-Linear Algebra, and the corresponding
            //  Fortran subroutine in EISPACK.

            // Initialize

            int nn = m_intN;
            int n = nn - 1;
            int low = 0;
            int high = nn - 1;
            double eps = Math.Pow(2.0, -52.0);
            double exshift = 0.0;
            double p = 0, q = 0, r = 0, s = 0, z = 0, t, w, x, y;

            // Store roots isolated by balanc and compute matrix norm

            double norm = 0.0;
            for (int i = 0; i < nn; i++)
            {
                if (i < low | i > high)
                {
                    m_dblD[i] = m_dblHArr[i, i];
                    m_dblE[i] = 0.0;
                }
                for (int j = Math.Max(i - 1, 0); j < nn; j++)
                {
                    norm = norm + Math.Abs(m_dblHArr[i, j]);
                }
            }

            // Outer loop over eigenvalue index

            int iter = 0;
            while (n >= low)
            {
                // Look for single small sub-diagonal element

                int l = n;
                while (l > low)
                {
                    s = Math.Abs(m_dblHArr[l - 1, l - 1]) + Math.Abs(m_dblHArr[l, l]);
                    if (s == 0.0)
                    {
                        s = norm;
                    }
                    if (Math.Abs(m_dblHArr[l, l - 1]) < eps*s)
                    {
                        break;
                    }
                    l--;
                }

                // Check for convergence
                // One root found

                if (l == n)
                {
                    m_dblHArr[n, n] = m_dblHArr[n, n] + exshift;
                    m_dblD[n] = m_dblHArr[n, n];
                    m_dblE[n] = 0.0;
                    n--;
                    iter = 0;

                    // Two roots found
                }
                else if (l == n - 1)
                {
                    w = m_dblHArr[n, n - 1]*m_dblHArr[n - 1, n];
                    p = (m_dblHArr[n - 1, n - 1] - m_dblHArr[n, n])/2.0;
                    q = p*p + w;
                    z = Math.Sqrt(Math.Abs(q));
                    m_dblHArr[n, n] = m_dblHArr[n, n] + exshift;
                    m_dblHArr[n - 1, n - 1] = m_dblHArr[n - 1, n - 1] + exshift;
                    x = m_dblHArr[n, n];

                    // Real pair

                    if (q >= 0)
                    {
                        if (p >= 0)
                        {
                            z = p + z;
                        }
                        else
                        {
                            z = p - z;
                        }
                        m_dblD[n - 1] = x + z;
                        m_dblD[n] = m_dblD[n - 1];
                        if (z != 0.0)
                        {
                            m_dblD[n] = x - w/z;
                        }
                        m_dblE[n - 1] = 0.0;
                        m_dblE[n] = 0.0;
                        x = m_dblHArr[n, n - 1];
                        s = Math.Abs(x) + Math.Abs(z);
                        p = x/s;
                        q = z/s;
                        r = Math.Sqrt(p*p + q*q);
                        p = p/r;
                        q = q/r;

                        // Row modification

                        for (int j = n - 1; j < nn; j++)
                        {
                            z = m_dblHArr[n - 1, j];
                            m_dblHArr[n - 1, j] = q*z + p*m_dblHArr[n, j];
                            m_dblHArr[n, j] = q*m_dblHArr[n, j] - p*z;
                        }

                        // Column modification

                        for (int i = 0; i <= n; i++)
                        {
                            z = m_dblHArr[i, n - 1];
                            m_dblHArr[i, n - 1] = q*z + p*m_dblHArr[i, n];
                            m_dblHArr[i, n] = q*m_dblHArr[i, n] - p*z;
                        }

                        // Accumulate transformations

                        for (int i = low; i <= high; i++)
                        {
                            z = m_dblVArr[i, n - 1];
                            m_dblVArr[i, n - 1] = q*z + p*m_dblVArr[i, n];
                            m_dblVArr[i, n] = q*m_dblVArr[i, n] - p*z;
                        }

                        // Complex pair
                    }
                    else
                    {
                        m_dblD[n - 1] = x + p;
                        m_dblD[n] = x + p;
                        m_dblE[n - 1] = z;
                        m_dblE[n] = -z;
                    }
                    n = n - 2;
                    iter = 0;

                    // No convergence yet
                }
                else
                {
                    // Form shift

                    x = m_dblHArr[n, n];
                    y = 0.0;
                    w = 0.0;
                    if (l < n)
                    {
                        y = m_dblHArr[n - 1, n - 1];
                        w = m_dblHArr[n, n - 1]*m_dblHArr[n - 1, n];
                    }

                    // Wilkinson's original ad hoc shift

                    if (iter == 10)
                    {
                        exshift += x;
                        for (int i = low; i <= n; i++)
                        {
                            m_dblHArr[i, i] -= x;
                        }
                        s = Math.Abs(m_dblHArr[n, n - 1]) + Math.Abs(m_dblHArr[n - 1, n - 2]);
                        x = y = 0.75*s;
                        w = -0.4375*s*s;
                    }

                    // MATLAB's new ad hoc shift

                    if (iter == 30)
                    {
                        s = (y - x)/2.0;
                        s = s*s + w;
                        if (s > 0)
                        {
                            s = Math.Sqrt(s);
                            if (y < x)
                            {
                                s = -s;
                            }
                            s = x - w/((y - x)/2.0 + s);
                            for (int i = low; i <= n; i++)
                            {
                                m_dblHArr[i, i] -= s;
                            }
                            exshift += s;
                            x = y = w = 0.964;
                        }
                    }

                    iter = iter + 1; // (Could check iteration count here.)

                    // Look for two consecutive small sub-diagonal elements

                    int m = n - 2;
                    while (m >= l)
                    {
                        z = m_dblHArr[m, m];
                        r = x - z;
                        s = y - z;
                        p = (r*s - w)/m_dblHArr[m + 1, m] + m_dblHArr[m, m + 1];
                        q = m_dblHArr[m + 1, m + 1] - z - r - s;
                        r = m_dblHArr[m + 2, m + 1];
                        s = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
                        p = p/s;
                        q = q/s;
                        r = r/s;
                        if (m == l)
                        {
                            break;
                        }
                        if (Math.Abs(m_dblHArr[m, m - 1])*(Math.Abs(q) + Math.Abs(r)) <
                            eps*(Math.Abs(p)*(Math.Abs(m_dblHArr[m - 1, m - 1]) + Math.Abs(z) +
                                              Math.Abs(m_dblHArr[m + 1, m + 1]))))
                        {
                            break;
                        }
                        m--;
                    }

                    for (int i = m + 2; i <= n; i++)
                    {
                        m_dblHArr[i, i - 2] = 0.0;
                        if (i > m + 2)
                        {
                            m_dblHArr[i, i - 3] = 0.0;
                        }
                    }

                    // Double QR step involving rows l:n and columns m:n

                    for (int k = m; k <= n - 1; k++)
                    {
                        bool notlast = (k != n - 1);
                        if (k != m)
                        {
                            p = m_dblHArr[k, k - 1];
                            q = m_dblHArr[k + 1, k - 1];
                            r = (notlast ? m_dblHArr[k + 2, k - 1] : 0.0);
                            x = Math.Abs(p) + Math.Abs(q) + Math.Abs(r);
                            if (x != 0.0)
                            {
                                p = p/x;
                                q = q/x;
                                r = r/x;
                            }
                        }
                        if (x == 0.0)
                        {
                            break;
                        }
                        s = Math.Sqrt(p*p + q*q + r*r);
                        if (p < 0)
                        {
                            s = -s;
                        }
                        if (s != 0)
                        {
                            if (k != m)
                            {
                                m_dblHArr[k, k - 1] = -s*x;
                            }
                            else if (l != m)
                            {
                                m_dblHArr[k, k - 1] = -m_dblHArr[k, k - 1];
                            }
                            p = p + s;
                            x = p/s;
                            y = q/s;
                            z = r/s;
                            q = q/p;
                            r = r/p;

                            // Row modification

                            for (int j = k; j < nn; j++)
                            {
                                p = m_dblHArr[k, j] + q*m_dblHArr[k + 1, j];
                                if (notlast)
                                {
                                    p = p + r*m_dblHArr[k + 2, j];
                                    m_dblHArr[k + 2, j] = m_dblHArr[k + 2, j] - p*z;
                                }
                                m_dblHArr[k, j] = m_dblHArr[k, j] - p*x;
                                m_dblHArr[k + 1, j] = m_dblHArr[k + 1, j] - p*y;
                            }

                            // Column modification

                            for (int i = 0; i <= Math.Min(n, k + 3); i++)
                            {
                                p = x*m_dblHArr[i, k] + y*m_dblHArr[i, k + 1];
                                if (notlast)
                                {
                                    p = p + z*m_dblHArr[i, k + 2];
                                    m_dblHArr[i, k + 2] = m_dblHArr[i, k + 2] - p*r;
                                }
                                m_dblHArr[i, k] = m_dblHArr[i, k] - p;
                                m_dblHArr[i, k + 1] = m_dblHArr[i, k + 1] - p*q;
                            }

                            // Accumulate transformations

                            for (int i = low; i <= high; i++)
                            {
                                p = x*m_dblVArr[i, k] + y*m_dblVArr[i, k + 1];
                                if (notlast)
                                {
                                    p = p + z*m_dblVArr[i, k + 2];
                                    m_dblVArr[i, k + 2] = m_dblVArr[i, k + 2] - p*r;
                                }
                                m_dblVArr[i, k] = m_dblVArr[i, k] - p;
                                m_dblVArr[i, k + 1] = m_dblVArr[i, k + 1] - p*q;
                            }
                        } // (s != 0)
                    } // k loop
                } // check convergence
            } // while (n >= low)

            // Backsubstitute to find vectors of upper triangular form

            if (norm == 0.0)
            {
                return;
            }

            for (n = nn - 1; n >= 0; n--)
            {
                p = m_dblD[n];
                q = m_dblE[n];

                // Real vector

                if (q == 0)
                {
                    int l = n;
                    m_dblHArr[n, n] = 1.0;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        w = m_dblHArr[i, i] - p;
                        r = 0.0;
                        for (int j = l; j <= n; j++)
                        {
                            r = r + m_dblHArr[i, j]*m_dblHArr[j, n];
                        }
                        if (m_dblE[i] < 0.0)
                        {
                            z = w;
                            s = r;
                        }
                        else
                        {
                            l = i;
                            if (m_dblE[i] == 0.0)
                            {
                                if (w != 0.0)
                                {
                                    m_dblHArr[i, n] = -r/w;
                                }
                                else
                                {
                                    m_dblHArr[i, n] = -r/(eps*norm);
                                }

                                // Solve real equations
                            }
                            else
                            {
                                x = m_dblHArr[i, i + 1];
                                y = m_dblHArr[i + 1, i];
                                q = (m_dblD[i] - p)*(m_dblD[i] - p) + m_dblE[i]*m_dblE[i];
                                t = (x*s - z*r)/q;
                                m_dblHArr[i, n] = t;
                                if (Math.Abs(x) > Math.Abs(z))
                                {
                                    m_dblHArr[i + 1, n] = (-r - w*t)/x;
                                }
                                else
                                {
                                    m_dblHArr[i + 1, n] = (-s - y*t)/z;
                                }
                            }

                            // Overflow control

                            t = Math.Abs(m_dblHArr[i, n]);
                            if ((eps*t)*t > 1)
                            {
                                for (int j = i; j <= n; j++)
                                {
                                    m_dblHArr[j, n] = m_dblHArr[j, n]/t;
                                }
                            }
                        }
                    }

                    // Complex vector
                }
                else if (q < 0)
                {
                    int l = n - 1;

                    // Last vector component imaginary so matrix is triangular

                    if (Math.Abs(m_dblHArr[n, n - 1]) > Math.Abs(m_dblHArr[n - 1, n]))
                    {
                        m_dblHArr[n - 1, n - 1] = q/m_dblHArr[n, n - 1];
                        m_dblHArr[n - 1, n] = -(m_dblHArr[n, n] - p)/m_dblHArr[n, n - 1];
                    }
                    else
                    {
                        cdiv(0.0, -m_dblHArr[n - 1, n], m_dblHArr[n - 1, n - 1] - p, q);
                        m_dblHArr[n - 1, n - 1] = cdivr;
                        m_dblHArr[n - 1, n] = cdivi;
                    }
                    m_dblHArr[n, n - 1] = 0.0;
                    m_dblHArr[n, n] = 1.0;
                    for (int i = n - 2; i >= 0; i--)
                    {
                        double ra, sa, vr, vi;
                        ra = 0.0;
                        sa = 0.0;
                        for (int j = l; j <= n; j++)
                        {
                            ra = ra + m_dblHArr[i, j]*m_dblHArr[j, n - 1];
                            sa = sa + m_dblHArr[i, j]*m_dblHArr[j, n];
                        }
                        w = m_dblHArr[i, i] - p;

                        if (m_dblE[i] < 0.0)
                        {
                            z = w;
                            r = ra;
                            s = sa;
                        }
                        else
                        {
                            l = i;
                            if (m_dblE[i] == 0)
                            {
                                cdiv(-ra, -sa, w, q);
                                m_dblHArr[i, n - 1] = cdivr;
                                m_dblHArr[i, n] = cdivi;
                            }
                            else
                            {
                                // Solve complex equations

                                x = m_dblHArr[i, i + 1];
                                y = m_dblHArr[i + 1, i];
                                vr = (m_dblD[i] - p)*(m_dblD[i] - p) + m_dblE[i]*m_dblE[i] - q*q;
                                vi = (m_dblD[i] - p)*2.0*q;
                                if (vr == 0.0 & vi == 0.0)
                                {
                                    vr = eps*norm*(Math.Abs(w) + Math.Abs(q) +
                                                   Math.Abs(x) + Math.Abs(y) + Math.Abs(z));
                                }
                                cdiv(x*r - z*ra + q*sa, x*s - z*sa - q*ra, vr, vi);
                                m_dblHArr[i, n - 1] = cdivr;
                                m_dblHArr[i, n] = cdivi;
                                if (Math.Abs(x) > (Math.Abs(z) + Math.Abs(q)))
                                {
                                    m_dblHArr[i + 1, n - 1] = (-ra - w*m_dblHArr[i, n - 1] + q*m_dblHArr[i, n])/x;
                                    m_dblHArr[i + 1, n] = (-sa - w*m_dblHArr[i, n] - q*m_dblHArr[i, n - 1])/x;
                                }
                                else
                                {
                                    cdiv(-r - y*m_dblHArr[i, n - 1], -s - y*m_dblHArr[i, n], z, q);
                                    m_dblHArr[i + 1, n - 1] = cdivr;
                                    m_dblHArr[i + 1, n] = cdivi;
                                }
                            }

                            // Overflow control

                            t = Math.Max(Math.Abs(m_dblHArr[i, n - 1]), Math.Abs(m_dblHArr[i, n]));
                            if ((eps*t)*t > 1)
                            {
                                for (int j = i; j <= n; j++)
                                {
                                    m_dblHArr[j, n - 1] = m_dblHArr[j, n - 1]/t;
                                    m_dblHArr[j, n] = m_dblHArr[j, n]/t;
                                }
                            }
                        }
                    }
                }
            }

            // Vectors of isolated roots

            for (int i = 0; i < nn; i++)
            {
                if (i < low | i > high)
                {
                    for (int j = i; j < nn; j++)
                    {
                        m_dblVArr[i, j] = m_dblHArr[i, j];
                    }
                }
            }

            // Back transformation to get eigenvectors of original matrix

            for (int j = nn - 1; j >= low; j--)
            {
                for (int i = low; i <= high; i++)
                {
                    z = 0.0;
                    for (int k = low; k <= Math.Min(j, high); k++)
                    {
                        z = z + m_dblVArr[i, k]*m_dblHArr[k, j];
                    }
                    m_dblVArr[i, j] = z;
                }
            }
        }


        /* ------------------------
           Constructor
         * ------------------------ */

        /** Check for symmetry, then construct the eigenvalue decomposition
        @param A    Square matrix
        @return     Structure to access D and V.
        */

        /* ------------------------
           Public Methods
         * ------------------------ */

        /** Return the eigenvector matrix
        @return     V
        */

        public MatrixClass getV()
        {
            return new MatrixClass(m_dblVArr, m_intN, m_intN);
        }

        /** Return the real parts of the eigenvalues
        @return     real(diag(D))
        */

        public double[] getRealEigenvalues()
        {
            return m_dblD;
        }

        /** Return the imaginary parts of the eigenvalues
        @return     imag(diag(D))
        */

        public double[] getImagEigenvalues()
        {
            return m_dblE;
        }

        /** Return the block diagonal eigenvalue matrix
        @return     D
        */

        public MatrixClass getD()
        {
            MatrixClass X = new MatrixClass(m_intN, m_intN);
            double[,] D = X.GetArr();
            for (int i = 0; i < m_intN; i++)
            {
                for (int j = 0; j < m_intN; j++)
                {
                    D[i, j] = 0.0;
                }
                D[i, i] = m_dblD[i];
                if (m_dblE[i] > 0)
                {
                    D[i, i + 1] = m_dblE[i];
                }
                else if (m_dblE[i] < 0)
                {
                    D[i, i - 1] = m_dblE[i];
                }
            }
            return X;
        }
    }
}
