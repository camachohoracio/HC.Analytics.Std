#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleMatrix2DComparator2_ : DoubleMatrix2DComparator
    {
        #region DoubleMatrix2DComparator Members

        public int Compare(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            double as_ = a.zSum();
            double bs = b.zSum();
            return as_ < bs ? -1 : as_ == bs ? 0 : 1;
        }

        #endregion
    }
}
