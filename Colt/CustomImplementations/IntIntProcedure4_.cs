#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntProcedure4_ : IntIntProcedure
    {
        private readonly int[] m_foundKey;
        private readonly int m_value;

        public IntIntProcedure4_(
            int value,
            int[] foundKey)
        {
            m_value = value;
            m_foundKey = foundKey;
        }

        #region IntIntProcedure Members

        public bool Apply(int iterKey, int iterValue)
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
