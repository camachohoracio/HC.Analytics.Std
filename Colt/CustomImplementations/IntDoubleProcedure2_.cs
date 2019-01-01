#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure2_ : IntDoubleProcedure
    {
        private readonly AbstractIntDoubleMap map_;

        public IntDoubleProcedure2_(
            AbstractIntDoubleMap map)
        {
            map_ = map;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            map_.put(key, value);
            return true;
        }

        #endregion
    }
}
