#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator26_ : IntComparator
    {
        private readonly ObjectMatrix2DComparator m_c;
        private readonly ObjectMatrix2D[] m_views;

        public IntComparator26_(
            ObjectMatrix2DComparator c,
            ObjectMatrix2D[] views)
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
