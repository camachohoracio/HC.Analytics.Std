#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix1D_ : WrapperDoubleMatrix1D
    {
        private readonly int m_intIndex;

        public WrapperDoubleMatrix1D_(
            DoubleMatrix1D doubleMatrix1D,
            int intIndex) :
                base(doubleMatrix1D)
        {
            m_intIndex = intIndex;
        }

        public override double getQuick(int i)
        {
            return content.get(m_intIndex + i);
        }

        public override void setQuick(int i, double value)
        {
            content.set(m_intIndex + i, value);
        }
    }
}
