#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator_ : IntComparator
    {
        private readonly int[] k_;
        private readonly double[] v_;

        public IntComparator_(
            int[] k,
            double[] v)
        {
            k_ = k;
            v_ = v;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            return v_[a] < v_[b] ? -1 : v_[a] > v_[b] ? 1 : (k_[a] < k_[b] ? -1 : (k_[a] == k_[b] ? 0 : 1));
        }

        #endregion
    }
}
