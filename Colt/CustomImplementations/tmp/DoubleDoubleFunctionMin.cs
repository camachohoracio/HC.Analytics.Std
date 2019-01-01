#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionMin : DoubleDoubleFunction
    {
        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return Math.Min(a, b);
        }

        #endregion
    }
}
