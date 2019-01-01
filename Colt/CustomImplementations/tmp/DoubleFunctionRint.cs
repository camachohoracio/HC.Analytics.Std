#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionRint : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Round(a, 0);
        }

        #endregion
    }
}
