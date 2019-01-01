#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionMod : DoubleDoubleFunction
    {
        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return a%b;
        }

        #endregion
    }
}
