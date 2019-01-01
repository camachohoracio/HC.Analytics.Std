#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionChain3 : DoubleFunction
    {
        private readonly DoubleFunction g_;
        private readonly DoubleFunction h_;

        public DoubleFunctionChain3(
            DoubleFunction g,
            DoubleFunction h)
        {
            g_ = g;
            h_ = h;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return g_.Apply(h_.Apply(a));
        }

        #endregion
    }
}
