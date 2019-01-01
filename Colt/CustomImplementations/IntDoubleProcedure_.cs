#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure_ : IntDoubleProcedure
    {
        private readonly DoubleFunction function_;
        private readonly AbstractIntDoubleMap map_;

        #region Constructors

        public IntDoubleProcedure_(
            AbstractIntDoubleMap map) : this(null,
                                             map)
        {
        }

        public IntDoubleProcedure_(
            DoubleFunction function,
            AbstractIntDoubleMap map)
        {
            function_ = function;
            map_ = map;
        }

        #endregion

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            map_.put(key, function_.Apply(value));
            return true;
        }

        #endregion
    }
}
