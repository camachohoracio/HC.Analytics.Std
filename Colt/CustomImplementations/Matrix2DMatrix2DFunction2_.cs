#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Matrix2DMatrix2DFunction2_ : Matrix2DMatrix2DFunction
    {
        private readonly DoubleDoubleFunction m_function;
        private readonly Blas m_seqBlas;

        public Matrix2DMatrix2DFunction2_(
            Blas seqBlas,
            DoubleDoubleFunction function)
        {
            m_seqBlas = seqBlas;
            m_function = function;
        }

        #region Matrix2DMatrix2DFunction Members

        public double Apply(DoubleMatrix2D AA, DoubleMatrix2D BB)
        {
            m_seqBlas.assign(AA, BB, m_function);
            return 0;
        }

        #endregion
    }
}
