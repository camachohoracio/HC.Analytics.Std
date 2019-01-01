#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure10_ : IntProcedure
    {
        private readonly AbstractIntObjectMap m_abstractIntObjectMap;
        private readonly ObjectArrayList m_objectArrayList;

        public IntProcedure10_(
            ObjectArrayList objectArrayList,
            AbstractIntObjectMap abstractIntObjectMap)
        {
            m_objectArrayList = objectArrayList;
            m_abstractIntObjectMap = abstractIntObjectMap;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            m_objectArrayList.Add(
                m_abstractIntObjectMap.get(key));
            return true;
        }

        #endregion
    }
}
