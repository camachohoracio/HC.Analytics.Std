#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntObjectProcedure6_ : IntObjectProcedure
    {
        private readonly object m_value;

        public IntObjectProcedure6_(
            object value)
        {
            m_value = value;
        }

        #region IntObjectProcedure Members

        public bool Apply(int iterKey, Object iterValue)
        {
            return (m_value != iterValue);
        }

        #endregion
    }
}
