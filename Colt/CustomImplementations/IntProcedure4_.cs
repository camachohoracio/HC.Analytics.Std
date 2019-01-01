#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure4_ : IntProcedure
    {
        private readonly DoubleArrayList list_;

        public IntProcedure4_(
            DoubleArrayList list)
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
