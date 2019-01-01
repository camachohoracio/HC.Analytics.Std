#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure9_ : IntProcedure
    {
        private readonly IntArrayList m_intArrayList;

        public IntProcedure9_(
            IntArrayList intArrayList)
        {
            m_intArrayList = intArrayList;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            m_intArrayList.Add(key);
            return true;
        }

        #endregion
    }
}
