#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntObjectProcedure_ : IntObjectProcedure
    {
        private readonly AbstractIntObjectMap m_other;

        public IntObjectProcedure_(AbstractIntObjectMap other)
        {
            m_other = other;
        }

        #region IntObjectProcedure Members

        public bool Apply(int key, Object value)
        {
            return m_other.containsKey(key) && m_other.get(key) == value;
        }

        #endregion
    }
}
