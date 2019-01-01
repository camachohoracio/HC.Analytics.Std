#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix2D2_ : WrapperDoubleMatrix2D
    {
        private int columns_;
        private DoubleMatrix2D content_;

        public WrapperDoubleMatrix2D2_(
            DoubleMatrix2D content,
            int columns) :
                base(content)
        {
            content_ = content;
            columns_ = columns;
        }

        public override double getQuick(int row, int column)
        {
            return content.get(column, row);
        }

        public override void setQuick(int row, int column, double value)
        {
            content.set(column, row, value);
        }
    }
}
