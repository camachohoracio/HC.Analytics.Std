#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionLogAB : DoubleDoubleFunction
    {
        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return Math.Log(a)/Math.Log(b);
        }

        #endregion
    }
}
