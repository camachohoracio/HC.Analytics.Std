#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper12_ : Swapper
    {
        private readonly double[] m_aggregates;
        private readonly int[] m_indexes;

        public Swapper12_(
            int[] indexes,
            double[] aggregates)
        {
            m_indexes = indexes;
            m_aggregates = aggregates;
        }

        #region Swapper Members

        public void swap(int x, int y)
        {
            int t1;
            double t2;
            t1 = m_indexes[x];
            m_indexes[x] = m_indexes[y];
            m_indexes[y] = t1;
            t2 = m_aggregates[x];
            m_aggregates[x] = m_aggregates[y];
            m_aggregates[y] = t2;
        }

        #endregion
    }
}
