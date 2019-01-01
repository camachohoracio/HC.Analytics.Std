#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionChain : DoubleDoubleFunction
    {
        private readonly DoubleDoubleFunction f_;
        private readonly DoubleFunction g_;
        private readonly DoubleFunction h_;

        public DoubleDoubleFunctionChain(
            DoubleDoubleFunction f,
            DoubleFunction g,
            DoubleFunction h)
        {
            f_ = f;
            g_ = g;
            h_ = h;
        }

        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return f_.Apply(g_.Apply(a), h_.Apply(b));
        }

        #endregion
    }
}
