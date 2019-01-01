#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionLimitOfOne : DoubleFunction
    {
        private readonly double m_dblLimit;

        public DoubleFunctionLimitOfOne(double dblLimit)
        {
            m_dblLimit = dblLimit;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return a > m_dblLimit ? a : 1;
        }

        #endregion
    }
}
