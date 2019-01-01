#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper_ : Swapper
    {
        private readonly int[] k_;
        private readonly double[] v_;

        public Swapper_(
            int[] k,
            double[] v)
        {
            k_ = k;
            v_ = v;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            int t2;
            double t1;
            t1 = v_[a];
            v_[a] = v_[b];
            v_[b] = t1;
            t2 = k_[a];
            k_[a] = k_[b];
            k_[b] = t2;
        }

        #endregion
    }
}
