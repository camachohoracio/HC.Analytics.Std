#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class BinFunction1D4_ : BinFunction1D
    {
        #region BinFunction1D Members

        public double Apply(DynamicBin1D bin)
        {
            return bin.min();
        }

        public string name()
        {
            return "Min";
        }

        #endregion
    }
}
