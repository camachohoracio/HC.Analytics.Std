#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class LongProcedure2_ : LongProcedure
    {
        private readonly AbstractLongObjectMap m_abstractLongObjectMap;
        private readonly LongObjectProcedure m_procedure;

        public LongProcedure2_(
            LongObjectProcedure procedure,
            AbstractLongObjectMap abstractLongObjectMap)
        {
            m_procedure = procedure;
            m_abstractLongObjectMap = abstractLongObjectMap;
        }

        #region LongProcedure Members

        public bool Apply(long key)
        {
            return m_procedure.Apply(key,
                                     m_abstractLongObjectMap.get(key));
        }

        #endregion
    }
}
