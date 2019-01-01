#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleProcedureIsLess : DoubleProcedure
    {
        private readonly double b_;

        public DoubleProcedureIsLess(double b)
        {
            b_ = b;
        }

        #region DoubleProcedure Members

        public bool Apply(double a)
        {
            return a < b_;
        }

        #endregion
    }
}
