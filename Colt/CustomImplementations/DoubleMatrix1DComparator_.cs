#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleMatrix1DComparator_ : DoubleMatrix1DComparator
    {
        #region DoubleMatrix1DComparator Members

        public int Compare(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            double as_ = a.zSum();
            double bs = b.zSum();
            return as_ < bs ? -1 : as_ == bs ? 0 : 1;
        }

        #endregion
    }
}
