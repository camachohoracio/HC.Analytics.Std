#region

using System;
using System.Collections;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator22_ : IntComparator
    {
        private readonly IComparer m_c;
        private readonly ObjectMatrix1D m_vector;

        public IntComparator22_(
            ObjectMatrix1D vector,
            IComparer c)
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
