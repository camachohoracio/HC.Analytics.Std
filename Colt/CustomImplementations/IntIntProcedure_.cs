#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntProcedure_ : IntIntProcedure
    {
        private readonly int m_intValue;

        public IntIntProcedure_(int intValue)
        {
            m_intValue = intValue;
        }

        #region IntIntProcedure Members

        public bool Apply(int iterKey, int iterValue)
        {
            return (m_intValue != iterValue);
        }

        #endregion
    }
}
