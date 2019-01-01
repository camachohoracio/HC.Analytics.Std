#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator13_ : IntComparator
    {
        private readonly int[] m_k;
        private readonly int[] m_v;

        public IntComparator13_(
            int[] k,
            int[] v)
        {
            m_k = k;
            m_v = v;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            return m_v[a] < m_v[b] ? -1 : m_v[a] > m_v[b] ? 1 : (m_k[a] < m_k[b] ? -1 : (m_k[a] == m_k[b] ? 0 : 1));
        }

        #endregion
    }
}
