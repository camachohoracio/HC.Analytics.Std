#region

using System;
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
    Sequential implementation of the Basic Linear Algebra System.

    @author wolfgang.hoschek@cern.ch
    @version 0.9, 16/04/2000 
    */

    [Serializable]
    public class SeqBlas : Blas
    {
        /**
        Little trick to allow for "aliasing", that is, renaming this class.
        Time and again writing code like
        <p>
        <tt>SeqBlas.blas.dgemm(...);</tt>
        <p>
        is a bit awkward. Using the aliasing you can instead write
        <p>
        <tt>Blas B = SeqBlas.blas; <br>
        B.dgemm(...);</tt>
        */

        private static Functions F = Functions.m_functions;
        public static Blas seqBlas = new SeqBlas();
        /**
        Makes this class non instantiable, but still let's others inherit from it.
        */

        #region Blas Members

        public void assign(DoubleMatrix2D A, DoubleFunction function)
        {
            A.assign(function);
        }

        public void assign(DoubleMatrix2D A, DoubleMatrix2D B, DoubleDoubleFunction function)
        {
            A.assign(B, function);
        }

        public double dasum(DoubleMatrix1D x)
        {
            return x.aggregate(Functions.m_plus, Functions.m_absm);
        }

        public void daxpy(double alpha, DoubleMatrix1D x, DoubleMatrix1D y)
        {
            y.assign(x, Functions.plusMult(alpha));
        }

        public void daxpy(double alpha, DoubleMatrix2D A, DoubleMatrix2D B)
        {
            B.assign(A, Functions.plusMult(alpha));
        }

        public void dcopy(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            y.assign(x);
        }

        public void dcopy(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            B.assign(A);
        }

        public double ddot(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            return x.zDotProduct(y);
        }

        public void dgemm(bool transposeA, bool transposeB, double alpha, DoubleMatrix2D A, DoubleMatrix2D B,
                          double beta, DoubleMatrix2D C)
        {
            A.zMult(B, C, alpha, beta, transposeA, transposeB);
        }

        public void dgemv(bool transposeA, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta,
                          DoubleMatrix1D y)
        {
            A.zMult(x, y, alpha, beta, transposeA);
        }

        public void dger(double alpha, DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix2D A)
        {
            PlusMult fun = PlusMult.plusMult(0);
            for (int i = A.Rows(); --i >= 0; )
            {
                fun.m_multiplicator = alpha * x.getQuick(i);
                A.viewRow(i).assign(y, fun);
            }
        }

        public double dnrm2(DoubleMatrix1D x)
        {
            return Math.Sqrt(Algebra.DEFAULT.norm2(x));
        }

        public void drot(DoubleMatrix1D x, DoubleMatrix1D y, double c, double s)
        {
            x.checkSize(y);
            DoubleMatrix1D tmp = x.Copy();

            x.assign(Functions.mult(c));
            x.assign(y, Functions.plusMult(s));

            y.assign(Functions.mult(c));
            y.assign(tmp, Functions.minusMult(s));
        }

        public void drotg(double a, double b, double[] rotvec)
        {
            double c, s, roe, scale, r, z, ra, rb;

            roe = b;

            if (Math.Abs(a) > Math.Abs(b))
            {
                roe = a;
            }

            scale = Math.Abs(a) + Math.Abs(b);

            if (scale != 0.0)
            {
                ra = a / scale;
                rb = b / scale;
                r = scale * Math.Sqrt(ra * ra + rb * rb);
                r = sign(1.0, roe) * r;
                c = a / r;
                s = b / r;
                z = 1.0;
                if (Math.Abs(a) > Math.Abs(b))
                {
                    z = s;
                }
                if ((Math.Abs(b) >= Math.Abs(a)) && (c != 0.0))
                {
                    z = 1.0 / c;
                }
            }
            else
            {
                c = 1.0;
                s = 0.0;
                r = 0.0;
                z = 0.0;
            }

            a = r;
            b = z;

            rotvec[0] = a;
            rotvec[1] = b;
            rotvec[2] = c;
            rotvec[3] = s;
        }

        public void dscal(double alpha, DoubleMatrix1D x)
        {
            x.assign(Functions.mult(alpha));
        }

        public void dscal(double alpha, DoubleMatrix2D A)
        {
            A.assign(Functions.mult(alpha));
        }

        public void dswap(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            y.swap(x);
        }

        public void dswap(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            //B.swap(A); not yet implemented
            A.checkShape(B);
            for (int i = A.Rows(); --i >= 0; )
            {
                A.viewRow(i).swap(B.viewRow(i));
            }
        }

        public void dsymv(bool isUpperTriangular, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta,
                          DoubleMatrix1D y)
        {
            if (isUpperTriangular)
            {
                A = A.viewDice();
            }
            Property.DEFAULT.checkSquare(A);
            int size = A.Rows();
            if (size != x.Size() || size != y.Size())
            {
                throw new ArgumentException(A.toStringShort() + ", " + x.toStringShort() + ", " + y.toStringShort());
            }
            DoubleMatrix1D tmp = x.like();
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j <= i; j++)
                {
                    sum += A.getQuick(i, j) * x.getQuick(j);
                }
                for (int j = i + 1; j < size; j++)
                {
                    sum += A.getQuick(j, i) * x.getQuick(j);
                }
                tmp.setQuick(i, alpha * sum + beta * y.getQuick(i));
            }
            y.assign(tmp);
        }

        public void dtrmv(bool isUpperTriangular, bool transposeA, bool isUnitTriangular, DoubleMatrix2D A,
                          DoubleMatrix1D x)
        {
            if (transposeA)
            {
                A = A.viewDice();
                isUpperTriangular = !isUpperTriangular;
            }

            Property.DEFAULT.checkSquare(A);
            int size = A.Rows();
            if (size != x.Size())
            {
                throw new ArgumentException(A.toStringShort() + ", " + x.toStringShort());
            }

            DoubleMatrix1D b = x.like();
            DoubleMatrix1D y = x.like();
            if (isUnitTriangular)
            {
                y.assign(1);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    y.setQuick(i, A.getQuick(i, i));
                }
            }

            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                if (!isUpperTriangular)
                {
                    for (int j = 0; j < i; j++)
                    {
                        sum += A.getQuick(i, j) * x.getQuick(j);
                    }
                    sum += y.getQuick(i) * x.getQuick(i);
                }
                else
                {
                    sum += y.getQuick(i) * x.getQuick(i);
                    for (int j = i + 1; j < size; j++)
                    {
                        sum += A.getQuick(i, j) * x.getQuick(j);
                    }
                }
                b.setQuick(i, sum);
            }
            x.assign(b);
        }

        public int idamax(DoubleMatrix1D x)
        {
            int maxIndex = -1;
            double maxValue = Double.MinValue;
            for (int i = x.Size(); --i >= 0; )
            {
                double v = Math.Abs(x.getQuick(i));
                if (v > maxValue)
                {
                    maxValue = v;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        #endregion

        /**
        Implements the FORTRAN sign (not sin) function.
        See the code for details.
        @param  a   a
        @param  b   b
        */

        private double sign(double a, double b)
        {
            if (b < 0.0)
            {
                return -Math.Abs(a);
            }
            else
            {
                return Math.Abs(a);
            }
        }
    }
}
