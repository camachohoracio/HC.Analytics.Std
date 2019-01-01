#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleIntProcedure2_ : DoubleIntProcedure
    {
        private readonly AbstractDoubleIntMap m_other;

        public DoubleIntProcedure2_(AbstractDoubleIntMap other)
        {
            m_other = other;
        }

        #region DoubleIntProcedure Members

        public bool Apply(double key, int value)
        {
            return m_other.containsKey(key) && m_other.get(key) == value;
        }

        #endregion
    }
}
