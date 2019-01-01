#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix1D4_ : WrapperDoubleMatrix1D
    {
        private readonly int _stride;

        public WrapperDoubleMatrix1D4_(
            DoubleMatrix1D doubleMatrix1D,
            int stride)
            : base(
                doubleMatrix1D)
        {
            _stride = stride;
        }

        public override double getQuick(int index)
        {
            return content.get(index*_stride);
        }

        public override void setQuick(int index, double value)
        {
            content.set(index*_stride, value);
        }
    }
}
