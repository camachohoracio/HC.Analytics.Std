#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper5_ : Swapper
    {
        private readonly int[] g_;

        public Swapper5_(int[] g)
        {
            g_ = g;
        }

        #region Swapper Members

        public void swap(int b, int c)
        {
            int tmp = g_[b];
            g_[b] = g_[c];
            g_[c] = tmp;
        }

        #endregion
    }
}
