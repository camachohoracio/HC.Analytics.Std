#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix2D3_ : WrapperDoubleMatrix2D
    {
        private readonly int m_intColumn;
        private readonly int m_intRow;


        public WrapperDoubleMatrix2D3_(
            WrapperDoubleMatrix2D wrapperDoubleMatrix2D,
            int intRow,
            int intColumn) :
                base(wrapperDoubleMatrix2D)
        {
            m_intRow = intRow;
            m_intColumn = intColumn;
        }

        public override double getQuick(int i, int j)
        {
            return content.get(m_intRow + i, m_intColumn + j);
        }

        public override void setQuick(int i, int j, double value)
        {
            content.set(m_intRow + i, m_intColumn + j, value);
        }
    }
}
