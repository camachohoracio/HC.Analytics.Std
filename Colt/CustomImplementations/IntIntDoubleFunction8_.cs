#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction8_ : IntIntDoubleFunction
    {
        private readonly TridiagonalDoubleMatrix2D m_tridiagonalDoubleMatrix2D;
        private readonly DoubleMatrix2D m_y;

        public IntIntDoubleFunction8_(
            TridiagonalDoubleMatrix2D tridiagonalDoubleMatrix2D,
            DoubleMatrix2D y)
        {
            m_tridiagonalDoubleMatrix2D = tridiagonalDoubleMatrix2D;
            m_y = y;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_tridiagonalDoubleMatrix2D.setQuick(i, j,
                                                 m_tridiagonalDoubleMatrix2D.getQuick(i, j)/m_y.getQuick(i, j));
            return value;
        }

        #endregion
    }
}
