#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator23_ : IntComparator
    {
        private readonly ObjectMatrix1D m_col;

        public IntComparator23_(
            ObjectMatrix1D col)
        {
            m_col = col;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            IComparable av = (IComparable) (m_col.getQuick(a));
            IComparable bv = (IComparable) (m_col.getQuick(b));
            int r = av.CompareTo(bv);
            return r < 0 ? -1 : (r > 0 ? 1 : 0);
        }

        #endregion
    }
}
