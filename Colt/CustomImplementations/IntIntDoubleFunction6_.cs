#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction6_ : IntIntDoubleFunction
    {
        private readonly double m_dblAlpha;
        private readonly TridiagonalDoubleMatrix2D m_tridiagonalDoubleMatrix2D;

        public IntIntDoubleFunction6_(
            TridiagonalDoubleMatrix2D tridiagonalDoubleMatrix2D,
            double dblAlpha)
        {
            m_tridiagonalDoubleMatrix2D = tridiagonalDoubleMatrix2D;
            m_dblAlpha = dblAlpha;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_tridiagonalDoubleMatrix2D.setQuick(i, j,
                                                 m_tridiagonalDoubleMatrix2D.getQuick(i, j) + m_dblAlpha*value);
            return value;
        }

        #endregion
    }
}
