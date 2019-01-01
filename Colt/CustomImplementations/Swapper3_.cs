#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper3_ : Swapper
    {
        private readonly DoubleMatrix2D m_A;

        public Swapper3_(DoubleMatrix2D A)
        {
            m_A = A;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            m_A.viewRow(a).swap(m_A.viewRow(b));
        }

        #endregion
    }
}
