#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionLogA : DoubleFunction
    {
        // 1.0 / Math.Log(2) == 1.4426950408889634

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Log(a)*1.4426950408889634;
        }

        #endregion
    }
}
