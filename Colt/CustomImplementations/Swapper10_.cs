#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper10_ : Swapper
    {
        private readonly double[] m_k;
        private readonly int[] m_v;

        public Swapper10_(
            int[] v,
            double[] k)
        {
            m_v = v;
            m_k = k;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            int t1;
            double t2;
            t1 = m_v[a];
            m_v[a] = m_v[b];
            m_v[b] = t1;
            t2 = m_k[a];
            m_k[a] = m_k[b];
            m_k[b] = t2;
        }

        #endregion
    }
}
