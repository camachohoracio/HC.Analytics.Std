#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongObjectProcedure_ : LongObjectProcedure
    {
        private readonly object m_value;

        public LongObjectProcedure_(object value)
        {
            m_value = value;
        }

        #region LongObjectProcedure Members

        public bool Apply(long iterKey, Object iterValue)
        {
            return (m_value != iterValue);
        }

        #endregion
    }
}
