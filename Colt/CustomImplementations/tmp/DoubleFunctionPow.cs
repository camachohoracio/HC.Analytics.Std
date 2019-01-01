#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionPow : DoubleFunction
    {
        private readonly double b_;

        public DoubleFunctionPow(double b)
        {
            b_ = b;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Pow(a, b_);
        }

        #endregion
    }
}
