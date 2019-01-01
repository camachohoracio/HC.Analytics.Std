#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure7_ : IntDoubleProcedure
    {
        private readonly IntDoubleProcedure condition_;
        private readonly IntArrayList keyList_;
        private readonly DoubleArrayList valueList_;

        public IntDoubleProcedure7_(
            IntDoubleProcedure condition,
            IntArrayList keyList,
            DoubleArrayList valueList)
        {
            condition_ = condition;
            keyList_ = keyList;
            valueList_ = valueList;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            if (condition_.Apply(key, value))
            {
                keyList_.Add(key);
                valueList_.Add(value);
            }
            return true;
        }

        #endregion
    }
}
