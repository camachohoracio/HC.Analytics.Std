#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionBindArg2 : DoubleFunction
    {
        private readonly double c_;
        private readonly DoubleDoubleFunction function_;

        public DoubleFunctionBindArg2(DoubleDoubleFunction function, double c)
        {
            function_ = function;
            c_ = c;
        }

        #region DoubleFunction Members

        public double Apply(double var)
        {
            return function_.Apply(var, c_);
        }

        #endregion
    }
}
