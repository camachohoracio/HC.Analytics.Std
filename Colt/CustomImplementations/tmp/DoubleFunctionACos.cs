#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionACos : DoubleFunction
    {
        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Acos(a);
        }

        #endregion
    }
}
