#region

using System;
using HC.Analytics.Colt.doubleAlgo;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator19_ : IntComparator
    {
        private readonly DoubleMatrix1D m_sliceView;

        public IntComparator19_(
            DoubleMatrix1D sliceView)
        {
            m_sliceView = sliceView;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            double av = m_sliceView.getQuick(a);
            double bv = m_sliceView.getQuick(b);
            if (double.IsNaN(av) || double.IsNaN(bv))
            {
                return SortingDoubleAlgo.compareNaN(av, bv); // swap NaNs to the end
            }
            return av < bv ? -1 : (av == bv ? 0 : 1);
        }

        #endregion
    }
}
