#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongObjectProcedure4_ : LongObjectProcedure
    {
        private readonly LongObjectProcedure m_condition;
        private readonly LongArrayList m_keyList;
        private readonly ObjectArrayList m_valueList;

        public LongObjectProcedure4_(
            LongObjectProcedure condition,
            LongArrayList keyList,
            ObjectArrayList valueList)
        {
            m_condition = condition;
            m_keyList = keyList;
            m_valueList = valueList;
        }

        #region LongObjectProcedure Members

        public bool Apply(long key, Object value)
        {
            if (m_condition.Apply(key, value))
            {
                m_keyList.Add(key);
                m_valueList.Add(value);
            }
            return true;
        }

        #endregion
    }
}
