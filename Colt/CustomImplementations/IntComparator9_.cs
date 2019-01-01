#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator9_ : IntComparator
    {
        private readonly double[] m_y;
        private readonly double[] m_z;

        public IntComparator9_(
            double[] y,
            double[] z)
        {
            m_y = y;
            m_z = z;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            if (m_y[a] == m_y[b])
            {
                return m_z[a] == m_z[b] ? 0 : (m_z[a] < m_z[b] ? -1 : 1);
            }
            return m_y[a] < m_y[b] ? -1 : 1;
        }

        #endregion
    }
}
