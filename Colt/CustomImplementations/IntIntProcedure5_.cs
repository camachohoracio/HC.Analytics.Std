#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntProcedure5_ : IntIntProcedure
    {
        private readonly IntIntProcedure m_condition;
        private readonly IntArrayList m_keyList;
        private readonly IntArrayList m_valueList;

        public IntIntProcedure5_(
            IntIntProcedure condition,
            IntArrayList keyList,
            IntArrayList valueList)
        {
            m_condition = condition;
            m_keyList = keyList;
            m_valueList = valueList;
        }

        #region IntIntProcedure Members

        public bool Apply(int key, int value)
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
