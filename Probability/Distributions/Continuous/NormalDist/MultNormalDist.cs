#region

using System;
using System.Text;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class MultNormalDist : AbstractMultContDist
    {
        #region Constants

        private const int INT_RND_SEED = 24;

        #endregion

        #region Properties

        public double[] MeanArr
        {
            get { return m_meanVector.GetArr(); }
        }

        public double[,] CovArr
        {
            get { return m_covMatrix.GetArr(); }
        }

        public MatrixClass InvCovMatrix
        {
            get
            {
                if (m_invCovarianceMatrix == null)
                {
                    m_invCovarianceMatrix = CovMatrix.Inverse();
                }
                return m_invCovarianceMatrix;
            }
        }

        protected double DenominatorConstant { get; set; }

        /// <summary>
        /// Cholesky decomposition from covariance matrix
        /// </summary>
        protected MatrixClass CovMatrixChol
        {
            get
            {
                if (m_covMatrixChol == null)
                {
                    m_covMatrixChol = CovMatrix.CholeskyDecomposition();
                }
                return m_covMatrixChol;
            }
        }

        public int VariableCount
        {
            get { return m_intVariableCount; }
        }

        #endregion

        #region Memebers

        private MatrixClass m_covMatrix;
        private MatrixClass m_covMatrixChol;
        private double m_dblRanmvnEps = 1e-3;
        private double m_dblRanmvnError;
        private double m_dblRanmvnValue;
        private double m_dblRanmvnVarest;
        private MatrixClass m_invCovarianceMatrix;
        private Vector m_meanVector;
        protected UnivNormalDistStd m_univNormalDistStd;

        #endregion

        #region Constructors

        public MultNormalDist(
            double[] dblMeanArr,
            double[,] dblCovArr,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                new Vector(dblMeanArr),
                new MatrixClass(dblCovArr));
        }

        public MultNormalDist(
            Vector meanVector,
            MatrixClass covMatrix,
            RngWrapper rng)
            : base(rng)
        {
            SetState(meanVector,
                     covMatrix);
        }

        #endregion

        #region Parameters

        public Vector MeanVector
        {
            get { return m_meanVector; }
            set
            {
                m_meanVector = value;
                SetState(m_meanVector,
                         m_covMatrix);
            }
        }

        public MatrixClass CovMatrix
        {
            get { return m_covMatrix; }
            set
            {
                m_covMatrix = value;
                SetState(m_meanVector,
                         m_covMatrix);
            }
        }

        #endregion

        #region Initializaiton

        protected void SetState(
            Vector meanVector,
            MatrixClass covMatrix)
        {
            if (meanVector.GetDimension() !=
                covMatrix.GetColumnDimension())
            {
                throw new ArgumentException("Error. Parameter's dimensions must agree.");
            }

            m_intVariableCount = meanVector.GetDimension();

            m_meanVector = meanVector;
            m_covMatrix = covMatrix;
            m_univNormalDistStd =
                new UnivNormalDistStd(m_rng);
            m_invCovarianceMatrix = null;
            m_covMatrixChol = null;
            DenominatorConstant = Math.Sqrt(
                Math.Pow(2.0*Math.PI, VariableCount)*
                CovMatrix.Determinant());
        }

        #endregion

        #region Public

        public override double Pdf(
            Vector x)
        {
            // res means -> x - mean
            MatrixClass res = x.Minus(MeanVector);

            double value = -0.5*
                           res.Transpose().Times(InvCovMatrix).Times(res).Get(0, 0);

            double dblPdf = (Math.Exp(value))/
                                 DenominatorConstant;

            if(double.IsInfinity(dblPdf))
            {
                dblPdf = 1E100;
            }

            return dblPdf;
        }

        public override double Cdf(
            double[] dblLowArr,
            double[] dblHighArr)
        {
            if (dblLowArr.Length != dblHighArr.Length ||
                dblLowArr.Length != CovMatrix.GetColumnDimension() ||
                dblHighArr.Length != CovMatrix.GetRowDimension())
            {
                throw new ArgumentException("Matrix dimentions must agree");
            }

            for (int i = 0; i < dblLowArr.Length; i++)
            {
                if (dblLowArr[i] >= dblHighArr[i])
                {
                    return 0;
                }
            }
            double[] a = (double[]) dblLowArr.Clone();
            double[] b = (double[]) dblHighArr.Clone();

            // bring limits to zero-mean
            for (int i = 0; i < VariableCount; i++)
            {
                a[i] -= MeanVector.Get(i);
                b[i] -= MeanVector.Get(i);
            }
            double[,] c1 = CovMatrix.GetArrCopy();
            return ranmvn_solve(a, b, c1, m_dblRanmvnEps);
        }

        public override double Cdf(
            Vector lowLimitVector,
            Vector highLimitVector)
        {
            return Cdf(
                lowLimitVector.GetArr(),
                highLimitVector.GetArr());
        }

        public override double Cdf(double[] dblXArr)
        {
            double[] dblLowLimitArr = new double[VariableCount];
            for (int i = 0; i < VariableCount; i++)
            {
                dblLowLimitArr[i] = double.MaxValue;
            }
            return Cdf(dblLowLimitArr, dblXArr);
        }

        public override double Cdf(Vector xVector)
        {
            return Cdf(xVector.GetArr());
        }

        public override double Pdf(double[] dblXArr)
        {
            return Pdf(new Vector(dblXArr));
        }

        public override double[] NextDouble()
        {
            return NextDoubleVector().GetArr();
        }

        /**
         *  Insert the method's description here. Creation date: (1/26/00 1:07:50 PM)
         *
         *@param  sqrtSigma  double[,]
         *@return            double[]
         *@author:           <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public override Vector NextDoubleVector()
        {
            Vector y = m_univNormalDistStd.NextDoubleVector(VariableCount);
            return y.Times(CovMatrixChol).Plus(MeanVector);
        }

        public override string ToString()
        {
            StringBuilder strOut = new StringBuilder();
            strOut.Append("\tmean=\n\t\t[");
            for (int variable = 0; variable < VariableCount; variable++)
            {
                strOut.Append(MeanVector.Get(variable) + ", ");
            }
            strOut.Append("]\n");

            strOut.Append("\tcovariance=\n");
            for (int variable1 = 0; variable1 < VariableCount; variable1++)
            {
                for (int variable2 = 0; variable2 < VariableCount; variable2++)
                {
                    strOut.Append("\t\t|" + CovMatrix.Get(variable1, variable2) +
                                  ", ");
                }
                strOut.Append("|\n");
            }
            return strOut.ToString();
        }

        #endregion

        #region Private

        private double ranmvn_solve(
            double[] lower,
            double[] upper,
            double[,] c,
            double releps)
        {
            int n = lower.Length;
            double d0 = (lower[0] == Double.NegativeInfinity)
                            ? 0
                            :
                                m_univNormalDistStd.Cdf(lower[0]/c[0, 0]);
            double e0 = (upper[0] == Double.PositiveInfinity)
                            ? 1
                            :
                                m_univNormalDistStd.Cdf(upper[0]/c[0, 0]);
            int mpt = 250 + 10*n;
            int ivls = mpt;
            ranmvn_rcrude(n - 1, mpt, 0, e0, d0, lower, upper, c);
            double eps = Math.Min(m_dblRanmvnEps, releps*Math.Abs(m_dblRanmvnValue));
            int maxItr = 100000*n;
            while (m_dblRanmvnError > eps && ivls < maxItr)
            {
                ranmvn_rcrude(n - 1, 10*n, 1, e0, d0, lower, upper, c);
                eps = Math.Min(m_dblRanmvnEps, releps*Math.Abs(m_dblRanmvnValue));
                ivls += mpt;
            }
            //PrintToScreen.WriteLine(ivls+" "+ranmvn_value);
            if (Double.IsNaN(m_dblRanmvnValue))
            {
                m_dblRanmvnValue = 0.0;
                //throw new IllegalMonitorStateException("NAN");
            }
            lower = null;
            upper = null;
            c = null;
            return m_dblRanmvnValue;
        }

        private void ranmvn_rcrude(
            int ndim,
            int maxpts,
            int ir,
            double e0,
            double d0,
            double[] lower,
            double[] upper, double[,] cov)
        {
            double fun;
            double varprd;
            double findif;
            double finval;
            double[] x = new double[ndim];
            if (ir <= 0)
            {
                m_dblRanmvnVarest = 0;
                m_dblRanmvnValue = 0;
            }
            double varest = m_dblRanmvnVarest;
            double finest = m_dblRanmvnValue;
            finval = 0;
            double varsqr = 0;
            int npts = maxpts/2;
            for (int m = 1; m <= npts; m++)
            {
                x = m_rng.NextDouble(ndim);
                fun = ranmvn_mvnfnc(x, e0, d0, lower, upper, cov);
                for (int k = 0; k < ndim; k++)
                {
                    x[k] = 1 - x[k];
                }
                fun += ranmvn_mvnfnc(x, e0, d0, lower, upper, cov);
                fun /= 2.0;
                findif = (fun - finval)/m;
                varsqr = (m - 2)*varsqr/m + findif*findif;
                finval += findif;
            }
            varprd = varest*varsqr;
            m_dblRanmvnValue = finest + (finval - finest)/(1 + varprd);
            if (varsqr > 0)
            {
                varest = (1 + varprd)/varsqr;
            }
            m_dblRanmvnVarest = varest;
            m_dblRanmvnError = 3*Math.Sqrt(varsqr/(1 + varprd));
        }

        private double ranmvn_mvnfnc(
            double[] w,
            double e0,
            double d0,
            double[] lower,
            double[] upper,
            double[,] cov)
        {
            double d = d0;
            double e = e0;
            double prod = e0 - d0;
            int n = w.Length;
            double[] y = new double[n];
            for (int i = 1; i <= n; i++)
            {
                y[i - 1] = m_univNormalDistStd.CdfInv(d + w[i - 1]*(e - d));
                double sum = 0;
                for (int j = 0; j < i; j++)
                {
                    sum += cov[i, j]*y[j];
                }
                d = (lower[i] == Double.NegativeInfinity)
                        ? 0
                        : m_univNormalDistStd.Cdf((lower[i]
                                                   - sum)/cov[i, i]);
                e = (upper[i] == Double.PositiveInfinity)
                        ? 1
                        : m_univNormalDistStd.Cdf((upper[i]
                                                   - sum)/cov[i, i]);
                prod *= (e - d);
            }
            if (Double.IsNaN(prod))
            {
                throw new ArgumentException("prod is NAN");
            }
            y = null;
            return prod;
        }

        #endregion
    }
}
