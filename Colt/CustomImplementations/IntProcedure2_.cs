#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure2_ : IntProcedure
    {
        private readonly AbstractIntDoubleMap map_;
        private readonly IntDoubleProcedure procedure_;

        public IntProcedure2_(
            AbstractIntDoubleMap map,
            IntDoubleProcedure procedure)
        {
            map_ = map;
            procedure_ = procedure;
        }

        #region IntProcedure Members

        public bool Apply(int key)
        {
            return procedure_.Apply(key, map_.get(key));
        }

        #endregion
    }
}
