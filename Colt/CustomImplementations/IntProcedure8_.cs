#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure8_ : IntProcedure
    {
        private readonly AbstractIntObjectMap m_abstractIntObjectMap;
        private readonly IntObjectProcedure m_procedure;

        public IntProcedure8_(
            IntObjectProcedure procedure,
            AbstractIntObjectMap abstractIntObjectMap)
        {
            m_procedure = procedure;
            m_abstractIntObjectMap = abstractIntObjectMap;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            return m_procedure.Apply(key, m_abstractIntObjectMap.get(key));
        }

        #endregion
    }
}
