#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleProcedureMod2 : DoubleProcedure
    {
        #region DoubleProcedure Members

        public bool Apply(double a)
        {
            return a%2 == 0;
        }

        #endregion
    }
}
