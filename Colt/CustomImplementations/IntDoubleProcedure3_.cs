#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure3_ : IntDoubleProcedure
    {
        private readonly double value_;

        public IntDoubleProcedure3_(
            double value)
        {
            value_ = value;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int iterKey, double iterValue)
        {
            return (value_ != iterValue);
        }

        #endregion
    }
}
