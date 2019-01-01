#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionSwapArgs : DoubleDoubleFunction
    {
        private readonly DoubleDoubleFunction function_;

        public DoubleDoubleFunctionSwapArgs(DoubleDoubleFunction function)
        {
            function_ = function;
        }

        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return function_.Apply(b, a);
        }

        #endregion
    }
}
