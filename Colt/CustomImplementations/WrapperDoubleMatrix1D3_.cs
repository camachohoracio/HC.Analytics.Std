#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix1D3_ : WrapperDoubleMatrix1D
    {
        private readonly int[] m_idx;

        public WrapperDoubleMatrix1D3_(
            DoubleMatrix1D doubleMatrix1D,
            int[] idx) : base(
                doubleMatrix1D)
        {
            m_idx = idx;
        }

        public override double getQuick(int i)
        {
            return content.get(m_idx[i]);
        }

        public override void setQuick(int i, double value)
        {
            content.set(m_idx[i], value);
        }
    }
}
