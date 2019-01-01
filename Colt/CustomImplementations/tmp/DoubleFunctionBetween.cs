#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionBetween : DoubleFunction
    {
        private readonly double from_;
        private readonly double to_;

        public DoubleFunctionBetween(double from, double to)
        {
            from_ = from;
            to_ = to;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return (from_ <= a && a <= to_) ? 1 : 0;
        }

        #endregion
    }
}
