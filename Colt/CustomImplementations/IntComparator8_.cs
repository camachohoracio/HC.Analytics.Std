#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator8_ : IntComparator
    {
        private readonly int[] x_;

        public IntComparator8_(int[] x)
        {
            x_ = x;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            return x_[a] == x_[b] ? 0 : (x_[a] < x_[b] ? -1 : 1);
        }

        #endregion
    }
}
