#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator4_ : IntComparator
    {
        private readonly ObjectMatrix1D m_columnView;
        private readonly int[] m_g;
        private readonly Object[] m_splitters;

        public IntComparator4_(
            Object[] splitters,
            ObjectMatrix1D columnView,
            int[] g)
        {
            m_splitters = splitters;
            m_columnView = columnView;
            m_g = g;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            IComparable av = (IComparable) (m_splitters[a]);
            IComparable bv = (IComparable) (m_columnView.getQuick(m_g[b]));
            int r = av.CompareTo(bv);
            return r < 0 ? -1 : (r == 0 ? 0 : 1);
        }

        #endregion
    }
}
