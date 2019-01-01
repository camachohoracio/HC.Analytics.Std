#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure11_ : IntProcedure
    {
        private readonly IntArrayList m_list;

        public IntProcedure11_(
            IntArrayList list)
        {
            m_list = list;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            m_list.Add(key);
            return true;
        }

        #endregion
    }
}
