#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionMax : DoubleFunction
    {
        private readonly double b_;

        public DoubleFunctionMax(double b)
        {
            b_ = b;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Max(a, b_);
        }

        #endregion
    }
}
