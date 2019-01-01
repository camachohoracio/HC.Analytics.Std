#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix2D_ : WrapperDoubleMatrix2D
    {
        private readonly int columns_;
        private DoubleMatrix2D content_;

        public WrapperDoubleMatrix2D_(
            DoubleMatrix2D content,
            int columns) :
                base(content)
        {
            content_ = content;
            columns_ = columns;
        }

        public override double getQuick(int row, int column)
        {
            return content.get(row, columns_ - 1 - column);
        }

        public override void setQuick(int row, int column, double value)
        {
            content.set(row, columns_ - 1 - column, value);
        }
    }
}
