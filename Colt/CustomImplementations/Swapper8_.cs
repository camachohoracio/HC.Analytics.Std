#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper8_ : Swapper
    {
        private readonly int[] m_k;
        private readonly object[] m_v;

        public Swapper8_(
            object[] v,
            int[] k)
        {
            m_v = v;
            m_k = k;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            int t2;
            Object t1;
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
