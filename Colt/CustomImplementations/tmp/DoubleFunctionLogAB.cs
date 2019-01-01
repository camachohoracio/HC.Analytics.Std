#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionLogAB : DoubleFunction
    {
        private readonly double b_;

        private readonly double logInv; // cached for speed

        public DoubleFunctionLogAB(double b)
        {
            b_ = b;
            logInv = 1/Math.Log(b_); // cached for speed
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Log(a)*logInv;
        }

        #endregion
    }
}
