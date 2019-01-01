#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionMin : DoubleFunction
    {
        private readonly double b_;

        public DoubleFunctionMin(double b)
        {
            b_ = b;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Min(a, b_);
        }

        #endregion
    }
}
