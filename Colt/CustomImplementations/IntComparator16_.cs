#region

using System;
using HC.Analytics.Colt.doubleAlgo;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator16_ : IntComparator
    {
        private readonly double[] m_aggregates;
        private SortingDoubleAlgo m_sorting;

        public IntComparator16_(
            double[] aggregates,
            SortingDoubleAlgo sorting)
        {
            m_sorting = sorting;
            m_aggregates = aggregates;
        }

        #region IntComparator Members

        public int Compare(int x, int y)
        {
            double a = m_aggregates[x];
            double b = m_aggregates[y];
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return SortingDoubleAlgo.compareNaN(a, b); // swap NaNs to the end
            }
            return a < b ? -1 : (a == b) ? 0 : 1;
        }

        #endregion
    }
}
