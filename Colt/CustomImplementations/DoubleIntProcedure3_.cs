#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleIntProcedure3_ : DoubleIntProcedure
    {
        private readonly double[] m_foundKey;
        private readonly int m_intValue;

        public DoubleIntProcedure3_(
            double[] foundKey,
            int intValue)
        {
            m_foundKey = foundKey;
            m_intValue = intValue;
        }

        #region DoubleIntProcedure Members

        public bool Apply(double iterKey, int iterValue)
        {
            bool found = m_intValue == iterValue;
            if (found)
            {
                m_foundKey[0] = iterKey;
            }
            return !found;
        }

        #endregion
    }
}
