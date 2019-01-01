#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleMatrix1DComparator2_ : DoubleMatrix1DComparator
    {
        #region DoubleMatrix1DComparator Members

        public int Compare(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            double a = Statistic.bin(x).median();
            double b = Statistic.bin(y).median();
            //double a = x.aggregate(Functions.plus,Functions.log);
            //double b = y.aggregate(Functions.plus,Functions.log);
            return a < b ? -1 : (a == b) ? 0 : 1;
        }

        #endregion
    }
}
