#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator21_ : IntComparator
    {
        private readonly ObjectMatrix1D m_vector;

        public IntComparator21_(
            ObjectMatrix1D vector)
        {
            m_vector = vector;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            IComparable av = (IComparable) (m_vector.getQuick(a));
            IComparable bv = (IComparable) (m_vector.getQuick(b));
            int r = av.CompareTo(bv);
            return r < 0 ? -1 : (r > 0 ? 1 : 0);
        }

        #endregion
    }
}
