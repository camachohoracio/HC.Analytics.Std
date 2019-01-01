#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction3_ : IntIntDoubleFunction
    {
        private readonly DoubleFunction m_function;

        public IntIntDoubleFunction3_(
            DoubleFunction function)
        {
            m_function = function;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            return m_function.Apply(value);
        }

        #endregion
    }
}
