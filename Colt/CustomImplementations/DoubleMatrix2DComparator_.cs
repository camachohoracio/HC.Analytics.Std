#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleMatrix2DComparator_ : DoubleMatrix2DComparator
    {
        #region DoubleMatrix2DComparator Members

        public int Compare(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            return a.zSum() == b.zSum() ? 1 : 0;
        }

        #endregion
    }
}
