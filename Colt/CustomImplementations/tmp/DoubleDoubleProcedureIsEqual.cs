#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleDoubleProcedureIsEqual : DoubleDoubleProcedure
    {
        #region DoubleDoubleProcedure Members

        public bool Apply(double a, double b)
        {
            return a == b;
        }

        #endregion
    }
}
