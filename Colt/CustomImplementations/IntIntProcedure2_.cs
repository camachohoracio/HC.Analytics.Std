#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntProcedure2_ : IntIntProcedure
    {
        private readonly AbstractIntIntMap m_other;

        public IntIntProcedure2_(
            AbstractIntIntMap other)
        {
            m_other = other;
        }

        #region IntIntProcedure Members

        public bool Apply(int key, int value)
        {
            return m_other.containsKey(key) && m_other.get(key) == value;
        }

        #endregion
    }
}
