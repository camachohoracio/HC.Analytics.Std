#region

using System;
using HC.Analytics.Colt.doubleAlgo;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator14_ : IntComparator
    {
        private readonly DoubleMatrix1D m_vector;
        private SortingDoubleAlgo m_sorting;

        public IntComparator14_(
            DoubleMatrix1D vector,
            SortingDoubleAlgo sorting)
        {
            m_vector = vector;
            m_sorting = sorting;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            double av = m_vector.getQuick(a);
            double bv = m_vector.getQuick(b);
            if (double.IsNaN(av) || double.IsNaN(bv))
            {
                return SortingDoubleAlgo.compareNaN(av, bv); // swap NaNs to the end
            }
            return av < bv ? -1 : (av == bv ? 0 : 1);
        }

        #endregion
    }
}
