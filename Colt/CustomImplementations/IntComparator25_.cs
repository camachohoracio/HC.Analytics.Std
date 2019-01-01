#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator25_ : IntComparator
    {
        private readonly ObjectMatrix1D m_sliceView;

        public IntComparator25_(
            ObjectMatrix1D sliceView)
        {
            m_sliceView = sliceView;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            IComparable av = (IComparable) (m_sliceView.getQuick(a));
            IComparable bv = (IComparable) (m_sliceView.getQuick(b));
            int r = av.CompareTo(bv);
            return r < 0 ? -1 : (r > 0 ? 1 : 0);
        }

        #endregion
    }
}
