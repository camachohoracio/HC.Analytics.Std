#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator18_ : IntComparator
    {
        private readonly DoubleMatrix1DComparator m_c;
        private readonly DoubleMatrix1D[] m_views;

        public IntComparator18_(
            DoubleMatrix1DComparator c,
            DoubleMatrix1D[] views)
        {
            m_c = c;
            m_views = views;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            //return c.Compare(matrix.viewRow(a), matrix.viewRow(b));
            return m_c.Compare(m_views[a], m_views[b]);
        }

        #endregion
    }
}
