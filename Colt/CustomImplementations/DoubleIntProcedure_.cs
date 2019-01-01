#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleIntProcedure_ : DoubleIntProcedure
    {
        private readonly int m_intValue;

        public DoubleIntProcedure_(int intValue)
        {
            m_intValue = intValue;
        }

        #region DoubleIntProcedure Members

        public bool Apply(double iterKey, int iterValue)
        {
            return (m_intValue != iterValue);
        }

        #endregion
    }
}
