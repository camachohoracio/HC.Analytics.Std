#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure12_ : IntProcedure
    {
        private readonly AbstractIntIntMap m_abstractIntIntMap;
        private readonly IntArrayList m_list;

        public IntProcedure12_(
            IntArrayList list,
            AbstractIntIntMap abstractIntIntMap)
        {
            m_list = list;
            m_abstractIntIntMap = abstractIntIntMap;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            m_list.Add(
                m_abstractIntIntMap.get(key));
            return true;
        }

        #endregion
    }
}
