#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix1D2_ : WrapperDoubleMatrix1D
    {
        private new readonly int m_intSize;

        public WrapperDoubleMatrix1D2_(
            DoubleMatrix1D doubleMatrix1D,
            int size) : base(
                doubleMatrix1D)
        {
            m_intSize = size;
        }

        public double GetQuick(int index)
        {
            return content.get(m_intSize - 1 - index);
        }

        public void SetQuick(int index, double value)
        {
            content.set(m_intSize - 1 - index, value);
        }
    }
}
