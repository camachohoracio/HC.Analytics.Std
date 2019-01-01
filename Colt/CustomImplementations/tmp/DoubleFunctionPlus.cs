#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionPlus : DoubleFunction
    {
        private readonly double b_;

        public DoubleFunctionPlus(double b)
        {
            b_ = b;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return a + b_;
        }

        #endregion
    }
}
