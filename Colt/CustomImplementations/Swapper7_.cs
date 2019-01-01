#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper7_ : Swapper
    {
        private readonly int[] x_;
        private readonly double[] y_;
        private readonly double[] z_;

        public Swapper7_(
            int[] x,
            double[] y,
            double[] z)
        {
            x_ = x;
            y_ = y;
            z_ = z;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            int t1;
            double t2, t3;
            t1 = x_[a];
            x_[a] = x_[b];
            x_[b] = t1;
            t2 = y_[a];
            y_[a] = y_[b];
            y_[b] = t2;
            t3 = z_[a];
            z_[a] = z_[b];
            z_[b] = t3;
        }

        #endregion
    }
}
