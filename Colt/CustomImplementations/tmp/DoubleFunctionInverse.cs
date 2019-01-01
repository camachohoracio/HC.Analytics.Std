#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionInverse : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return 1.0/a;
        }

        #endregion
    }
}
