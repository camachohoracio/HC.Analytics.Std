#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntObjectProcedure2_ : IntObjectProcedure
    {
        private readonly int[] m_foundKey;
        private readonly object m_value;

        public IntObjectProcedure2_(
            object value,
            int[] foundKey)
        {
            m_value = value;
            m_foundKey = foundKey;
        }

        #region IntObjectProcedure Members

        public bool Apply(int iterKey, Object iterValue)
        {
            bool found = m_value == iterValue;
            if (found)
            {
                m_foundKey[0] = iterKey;
            }
            return !found;
        }

        #endregion
    }
}
