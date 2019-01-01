#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class DoubleProcedure3_ : DoubleProcedure
    {
        private readonly DoubleArrayList m_list;

        public DoubleProcedure3_(
            DoubleArrayList list)
        {
            m_list = list;
        }

        #region DoubleProcedure Members

        public bool Apply(double key)
        {
            m_list.Add(key);
            return true;
        }

        #endregion
    }
}
