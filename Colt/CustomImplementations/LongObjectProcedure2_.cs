#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongObjectProcedure2_ : LongObjectProcedure
    {
        private readonly AbstractLongObjectMap m_other;

        public LongObjectProcedure2_(AbstractLongObjectMap other)
        {
            m_other = other;
        }

        #region LongObjectProcedure Members

        public bool Apply(long key, Object value)
        {
            return m_other.containsKey(key) && m_other.get(key) == value;
        }

        #endregion
    }
}
