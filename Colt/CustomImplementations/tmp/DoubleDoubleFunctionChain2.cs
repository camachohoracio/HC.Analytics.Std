#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleFunctionChain2 : DoubleDoubleFunction
    {
        private readonly DoubleFunction g_;
        private readonly DoubleDoubleFunction h_;

        public DoubleDoubleFunctionChain2(
            DoubleFunction g,
            DoubleDoubleFunction h)
        {
            g_ = g;
            h_ = h;
        }

        #region DoubleDoubleFunction Members

        public double Apply(double a, double b)
        {
            return g_.Apply(h_.Apply(a, b));
        }

        #endregion
    }
}
