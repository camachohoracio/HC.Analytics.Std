#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure6_ : IntDoubleProcedure
    {
        private readonly int[] foundKey_;
        private readonly double value_;
        private AbstractIntDoubleMap map_;

        public IntDoubleProcedure6_(
            AbstractIntDoubleMap map,
            double value,
            int[] foundKey)
        {
            map_ = map;
            value_ = value;
            foundKey_ = foundKey;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int iterKey, double iterValue)
        {
            bool found = value_ == iterValue;
            if (found)
            {
                foundKey_[0] = iterKey;
            }
            return !found;
        }

        #endregion
    }
}
