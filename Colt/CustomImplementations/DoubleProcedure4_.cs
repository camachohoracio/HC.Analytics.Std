#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleProcedure4_ : DoubleProcedure
    {
        private readonly AbstractDoubleIntMap m_abstractDoubleIntMap;
        private readonly IntArrayList m_list;

        public DoubleProcedure4_(
            IntArrayList list,
            AbstractDoubleIntMap abstractDoubleIntMap)
        {
            m_list = list;
            m_abstractDoubleIntMap = abstractDoubleIntMap;
        }

        #region DoubleProcedure Members

        public bool Apply(double key)
        {
            m_list.Add(
                m_abstractDoubleIntMap.get(key));
            return true;
        }

        #endregion
    }
}
