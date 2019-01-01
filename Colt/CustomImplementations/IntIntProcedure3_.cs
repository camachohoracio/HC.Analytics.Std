#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntProcedure3_ : IntProcedure
    {
        private readonly AbstractIntIntMap m_abstractIntIntMap;
        private readonly IntIntProcedure m_procedure;

        public IntIntProcedure3_(
            IntIntProcedure procedure,
            AbstractIntIntMap abstractIntIntMap)
        {
            m_procedure = procedure;
            m_abstractIntIntMap = abstractIntIntMap;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            return m_procedure.Apply(key,
                                     m_abstractIntIntMap.get(key));
        }

        #endregion
    }
}
