#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix2D4_ : WrapperDoubleMatrix2D
    {
        private new readonly int m_intRows;

        public WrapperDoubleMatrix2D4_(
            DoubleMatrix2D doubleMatrix2D,
            int rows)
            : base(
                doubleMatrix2D)
        {
            m_intRows = rows;
        }

        public override double getQuick(int row, int column)
        {
            return content.get(m_intRows - 1 - row, column);
        }

        public override void setQuick(int row, int column, double value)
        {
            content.set(m_intRows - 1 - row, column, value);
        }
    }
}
