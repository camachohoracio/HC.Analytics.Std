#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongProcedure3_ : LongProcedure
    {
        private readonly LongArrayList m_list;

        public LongProcedure3_(
            LongArrayList list)
        {
            m_list = list;
        }

        #region LongProcedure Members

        public bool Apply(long key)
        {
            m_list.Add(key);
            return true;
        }

        #endregion
    }
}
