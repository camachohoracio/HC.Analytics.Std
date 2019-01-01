#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionConstant : DoubleFunction
    {
        private readonly double c_;

        public DoubleFunctionConstant(double c)
        {
            c_ = c;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return c_;
        }

        #endregion
    }
}
