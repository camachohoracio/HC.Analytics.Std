#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionTan : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Tan(a);
        }

        #endregion
    }
}
