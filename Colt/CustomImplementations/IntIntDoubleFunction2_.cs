#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction2_ : IntIntDoubleFunction
    {
        private readonly double m_dblAlpha;
        private readonly SparseDoubleMatrix2D m_sparseDoubleMatrix2D;

        public IntIntDoubleFunction2_(
            SparseDoubleMatrix2D sparseDoubleMatrix2D,
            double dblAlpha)
        {
            m_sparseDoubleMatrix2D = sparseDoubleMatrix2D;
            m_dblAlpha = dblAlpha;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_sparseDoubleMatrix2D.setQuick(i, j,
                                            m_sparseDoubleMatrix2D.getQuick(i, j) + m_dblAlpha*value);
            return value;
        }

        #endregion
    }
}
