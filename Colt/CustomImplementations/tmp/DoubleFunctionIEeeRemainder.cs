#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionIEeeRemainder : DoubleFunction
    {
        private readonly double b_;

        public DoubleFunctionIEeeRemainder(double b)
        {
            b_ = b;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.IEEERemainder(a, b_);
        }

        #endregion
    }
}
