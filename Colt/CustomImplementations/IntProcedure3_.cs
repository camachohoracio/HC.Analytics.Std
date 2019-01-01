#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure3_ : IntProcedure
    {
        private readonly IntArrayList list_;

        public IntProcedure3_(
            IntArrayList list)
        {
            list_ = list;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            list_.Add(key);
            return true;
        }

        #endregion
    }
}
