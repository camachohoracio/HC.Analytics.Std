#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure4_ : IntDoubleProcedure
    {
        private readonly AbstractIntDoubleMap other_;

        public IntDoubleProcedure4_(
            AbstractIntDoubleMap other)
        {
            other_ = other;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            return other_.containsKey(key) && other_.get(key) == value;
        }

        #endregion
    }
}
