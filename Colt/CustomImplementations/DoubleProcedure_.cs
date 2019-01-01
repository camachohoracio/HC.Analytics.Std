#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleProcedure_ : DoubleProcedure
    {
        private readonly double m_dblKey;

        public DoubleProcedure_(double dblKey)
        {
            m_dblKey = dblKey;
        }

        #region DoubleProcedure Members

        public bool Apply(double iterKey)
        {
            return (m_dblKey != iterKey);
        }

        #endregion
    }
}
