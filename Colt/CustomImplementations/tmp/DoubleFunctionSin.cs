#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionSin : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Sin(a);
        }

        #endregion
    }
}
