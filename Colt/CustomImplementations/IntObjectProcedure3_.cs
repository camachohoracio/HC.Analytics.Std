#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntObjectProcedure3_ : IntObjectProcedure
    {
        private readonly IntObjectProcedure m_condition;
        private readonly IntArrayList m_keyList;
        private readonly ObjectArrayList m_valueList;

        public IntObjectProcedure3_(
            IntObjectProcedure condition,
            IntArrayList keyList,
            ObjectArrayList valueList)
        {
            m_condition = condition;
            m_keyList = keyList;
            m_valueList = valueList;
        }

        #region IntObjectProcedure Members

        public bool Apply(int key, Object value)
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
