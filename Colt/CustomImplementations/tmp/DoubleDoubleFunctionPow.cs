#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionPow : DoubleDoubleFunction
    {
        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return Math.Pow(a, b);
        }

        #endregion
    }
}
