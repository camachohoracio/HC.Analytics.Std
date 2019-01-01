#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper9_ : Swapper
    {
        private readonly long[] m_k;
        private readonly object[] m_v;

        public Swapper9_(
            object[] v,
            long[] k)
        {
            m_v = v;
            m_k = k;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            long t2;
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
