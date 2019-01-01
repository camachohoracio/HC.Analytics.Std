#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionPlusAbs : DoubleDoubleFunction
    {
        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return Math.Abs(a) + Math.Abs(b);
        }

        #endregion
    }
}
