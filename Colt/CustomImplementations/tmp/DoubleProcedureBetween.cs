#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleProcedureBetween : DoubleProcedure
    {
        private readonly double from_;
        private readonly double to_;

        public DoubleProcedureBetween(double from, double to)
        {
            from_ = from;
            to_ = to;
        }

        #region DoubleProcedure Members

        public bool Apply(double a)
        {
            return from_ <= a && a <= to_;
        }

        #endregion
    }
}
