#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator15_ : IntComparator
    {
        private readonly DoubleComparator m_c;
        private readonly DoubleMatrix1D m_vector;

        public IntComparator15_(
            DoubleMatrix1D vector,
            DoubleComparator c)
        {
            m_vector = vector;
            m_c = c;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            return m_c.Compare(m_vector.getQuick(a), m_vector.getQuick(b));
        }

        #endregion
    }
}
