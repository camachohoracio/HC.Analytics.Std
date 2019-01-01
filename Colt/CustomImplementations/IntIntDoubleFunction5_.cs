#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction5_ : IntIntDoubleFunction
    {
        private readonly TridiagonalDoubleMatrix2D m_tridiagonalDoubleMatrix2D;

        public IntIntDoubleFunction5_(
            TridiagonalDoubleMatrix2D tridiagonalDoubleMatrix2D)
        {
            m_tridiagonalDoubleMatrix2D = tridiagonalDoubleMatrix2D;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_tridiagonalDoubleMatrix2D.setQuick(i, j, value);
            return value;
        }

        #endregion
    }
}
