#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix2D7_ : WrapperDoubleMatrix2D
    {
        private new readonly int m_columnStride;
        private new readonly int m_rowStride;

        public WrapperDoubleMatrix2D7_(
            WrapperDoubleMatrix2D wrapperDoubleMatrix2D,
            int _rowStride, int _columnStride) :
                base(wrapperDoubleMatrix2D)
        {
            m_rowStride = _rowStride;
            m_columnStride = _columnStride;
        }

        public override double getQuick(int row, int column)
        {
            return content.get(m_rowStride*row, m_columnStride*column);
        }

        public override void setQuick(int row, int column, double value)
        {
            content.set(m_rowStride*row, m_columnStride*column, value);
        }
    }
}
