#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleProcedureGreater : DoubleProcedure
    {
        private readonly double b_;

        public DoubleProcedureGreater(double b)
        {
            b_ = b;
        }

        #region DoubleProcedure Members

        public bool Apply(double a)
        {
            return a > b_;
        }

        #endregion
    }
}
