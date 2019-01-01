#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleComparator_ : DoubleComparator
    {
        #region DoubleComparator Members

        public int Compare(double a, double b)
        {
            double as_ = Math.Sin(a);
            double bs = Math.Sin(b);
            return as_ < bs ? -1 : as_ == bs ? 0 : 1;
        }

        #endregion
    }
}
