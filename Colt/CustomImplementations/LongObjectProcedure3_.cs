#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongObjectProcedure3_ : LongObjectProcedure
    {
        private readonly long[] m_foundKey;
        private readonly object m_value;

        public LongObjectProcedure3_(
            long[] foundKey,
            object value)
        {
            m_foundKey = foundKey;
            m_value = value;
        }

        #region LongObjectProcedure Members

        public bool Apply(long iterKey, Object iterValue)
        {
            bool found = m_value == iterValue;
            if (found)
            {
                m_foundKey[0] = iterKey;
            }
            return !found;
        }

        #endregion
    }
}
