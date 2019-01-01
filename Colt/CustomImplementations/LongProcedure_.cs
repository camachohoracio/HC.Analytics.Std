#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongProcedure_ : LongProcedure
    {
        private readonly long m_key;

        public LongProcedure_(long key)
        {
            m_key = key;
        }

        #region LongProcedure Members

        public bool Apply(long iterKey)
        {
            return (m_key != iterKey);
        }

        #endregion
    }
}
