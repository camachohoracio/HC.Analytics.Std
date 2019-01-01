#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure5_ : IntDoubleProcedure
    {
        private readonly AbstractIntDoubleMap map_;

        public IntDoubleProcedure5_(
            AbstractIntDoubleMap map)
        {
            map_ = map;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            return map_.containsKey(key) && map_.get(key) == value;
        }

        #endregion
    }
}
