#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator20_ : IntComparator
    {
        private readonly DoubleMatrix2DComparator m_c;
        private readonly DoubleMatrix2D[] m_views;

        public IntComparator20_(
            DoubleMatrix2DComparator c,
            DoubleMatrix2D[] views)
        {
            m_c = c;
            m_views = views;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            //return c.Compare(matrix.viewSlice(a), matrix.viewSlice(b));
            return m_c.Compare(m_views[a], m_views[b]);
        }

        #endregion
    }
}
