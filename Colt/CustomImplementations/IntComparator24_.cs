#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator24_ : IntComparator
    {
        private readonly ObjectMatrix1DComparator m_c;
        private readonly ObjectMatrix1D[] m_views;

        public IntComparator24_(
            ObjectMatrix1D[] views,
            ObjectMatrix1DComparator c)
        {
            m_views = views;
            m_c = c;
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
