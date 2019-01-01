#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleIntProcedure4_ : DoubleIntProcedure
    {
        private readonly DoubleIntProcedure m_condition;
        private readonly DoubleArrayList m_keyList;
        private readonly IntArrayList m_valueList;

        public DoubleIntProcedure4_(
            DoubleIntProcedure condition,
            DoubleArrayList keyList,
            IntArrayList valueList)
        {
            m_condition = condition;
            m_keyList = keyList;
            m_valueList = valueList;
        }

        #region DoubleIntProcedure Members

        public bool Apply(double key, int value)
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
