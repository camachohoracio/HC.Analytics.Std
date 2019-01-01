#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator11_ : IntComparator
    {
        private readonly long[] m_k;
        private readonly Object[] m_v;

        public IntComparator11_(
            long[] k,
            Object[] v)
        {
            m_k = k;
            m_v = v;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            int ab = ((IComparable) m_v[a]).CompareTo(m_v[b]);
            return ab < 0 ? -1 : ab > 0 ? 1 : (m_k[a] < m_k[b] ? -1 : (m_k[a] == m_k[b] ? 0 : 1));
            //return v[a]<v[b] ? -1 : v[a]>v[b] ? 1 : (k[a]<k[b] ? -1 : (k[a]==k[b] ? 0 : 1));
        }

        #endregion
    }
}
