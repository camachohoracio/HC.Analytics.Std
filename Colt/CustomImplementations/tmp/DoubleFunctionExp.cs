#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionExp : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Exp(a);
        }

        #endregion
    }
}
