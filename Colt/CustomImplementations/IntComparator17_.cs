#region

using System;
using HC.Analytics.Colt.doubleAlgo;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator17_ : IntComparator
    {
        private readonly DoubleMatrix1D m_col;

        public IntComparator17_(
            DoubleMatrix1D col)
        {
            m_col = col;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            double av = m_col.getQuick(a);
            double bv = m_col.getQuick(b);

            if (double.IsNaN(av) || double.IsNaN(bv))
            {
                return SortingDoubleAlgo.compareNaN(av, bv); // swap NaNs to the end
            }
            return av < bv ? -1 : (av == bv ? 0 : 1);
        }

        #endregion
    }
}
