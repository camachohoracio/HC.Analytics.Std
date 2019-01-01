#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction4_ : IntIntDoubleFunction
    {
        private readonly RCDoubleMatrix2D m_function;

        public IntIntDoubleFunction4_(
            RCDoubleMatrix2D function)
        {
            m_function = function;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_function.setQuick(i, j, value);
            return value;
        }

        #endregion
    }
}
