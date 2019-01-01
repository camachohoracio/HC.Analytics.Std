#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionNeg : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return -a;
        }

        #endregion
    }
}
