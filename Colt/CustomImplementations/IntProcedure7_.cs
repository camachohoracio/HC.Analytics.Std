#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure7_ : IntProcedure
    {
        private readonly int m_key;

        public IntProcedure7_(
            int key)
        {
            m_key = key;
        }

        #region IntProcedure Members

        public bool Apply(int iterKey)
        {
            return (m_key != iterKey);
        }

        #endregion
    }
}
