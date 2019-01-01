#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleProcedure2_ : DoubleProcedure
    {
        private readonly AbstractDoubleIntMap m_abstractDoubleIntMap;
        private readonly DoubleIntProcedure m_procedure;

        public DoubleProcedure2_(
            DoubleIntProcedure procedure,
            AbstractDoubleIntMap abstractDoubleIntMap)
        {
            m_procedure = procedure;
            m_abstractDoubleIntMap = abstractDoubleIntMap;
        }

        #region DoubleProcedure Members

        public bool Apply(double key)
        {
            return m_procedure.Apply(key, m_abstractDoubleIntMap.get(key));
        }

        #endregion
    }
}
