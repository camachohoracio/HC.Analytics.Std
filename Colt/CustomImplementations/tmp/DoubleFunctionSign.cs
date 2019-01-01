#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionSign : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return a < 0 ? -1 : a > 0 ? 1 : 0;
        }

        #endregion
    }
}
