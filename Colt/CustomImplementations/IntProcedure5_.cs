#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure5_ : IntProcedure
    {
        private readonly int m_intKey;

        public IntProcedure5_(
            int intKey)
        {
            m_intKey = intKey;
        }

        #region IntProcedure Members

        public bool Apply(int iterKey)
        {
            return (m_intKey != iterKey);
        }

        #endregion
    }
}
