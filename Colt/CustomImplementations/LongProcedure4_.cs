#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongProcedure4_ : LongProcedure
    {
        private readonly AbstractLongObjectMap m_abstractLongObjectMap;
        private readonly ObjectArrayList m_list;

        public LongProcedure4_(
            ObjectArrayList list,
            AbstractLongObjectMap abstractLongObjectMap)
        {
            m_list = list;
            m_abstractLongObjectMap = abstractLongObjectMap;
        }

        #region LongProcedure Members

        public bool Apply(long key)
        {
            m_list.Add(m_abstractLongObjectMap.get(key));
            return true;
        }

        #endregion
    }
}
