#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionLess : DoubleFunction
    {
        private readonly double b_;

        public DoubleFunctionLess(double b)
        {
            b_ = b;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return a < b_ ? 1 : 0;
        }

        #endregion
    }
}
