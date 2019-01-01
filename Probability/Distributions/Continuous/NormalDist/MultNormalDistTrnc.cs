#region

using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class MultNormalDistTrnc : MultNormalDist
    {
        #region Memebers

        private double[] m_dblHighLimitArr;
        private double[] m_dblLowLimitArr;

        #endregion

        #region Constrcutors

        public MultNormalDistTrnc(
            double[] dblMeanArr,
            double[,] dblCovArr,
            RngWrapper rng,
            double[] dblLowLimitArr,
            double[] dblHighLimitArr) : base(
                dblMeanArr,
                dblCovArr,
                rng)
        {
            SetState(
                dblLowLimitArr,
                dblHighLimitArr);
        }

        #endregion

        #region Initialization

        private void SetState(
            double[] dblLowLimitArr,
            double[] dblHighLimitArr)
        {
            m_dblLowLimitArr = dblLowLimitArr;
            m_dblHighLimitArr = dblHighLimitArr;
            // bring the limits to zero mean
            for (int i = 0; i < VariableCount; i++)
            {
                m_dblLowLimitArr[i] -= MeanVector.Get(i);
                m_dblLowLimitArr[i] -= MeanVector.Get(i);
            }
        }

        #endregion

        #region Public

        public override double[] NextDouble()
        {
            bool blnFlag = true;
            double[] t = null;
            while (blnFlag)
            {
                bool blnBreakLoop = false;
                t = mvnor_sqrtSigma();
                for (int i = 0; i < VariableCount; i++)
                {
                    if (t[i] < m_dblLowLimitArr[i] || t[i] > m_dblHighLimitArr[i])
                    {
                        blnBreakLoop = true;
                        break;
                    }
                }
                if (!blnBreakLoop)
                {
                    blnFlag = false;
                }
            }
            return t;
        }

        public override Vector NextDoubleVector()
        {
            return new Vector(NextDouble());
        }

        #endregion

        #region Private

        private double[] mvnor_sqrtSigma()
        {
            double[,] sqrtSigma = CovMatrixChol.GetArr();
            int n = sqrtSigma.GetLength(0);
            double[] y = m_univNormalDistStd.NextDoubleArr(n);
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = 0;
                for (int j = 0; j <= i; j++)
                {
                    x[i] += sqrtSigma[i, j]*y[j];
                }
            }
            return x;
        }

        #endregion
    }
}
