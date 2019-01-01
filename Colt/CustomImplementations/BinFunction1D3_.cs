#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class BinFunction1D3_ : BinFunction1D
    {
        #region BinFunction1D Members

        public double Apply(DynamicBin1D bin)
        {
            return bin.median();
        }

        public string name()
        {
            return "Median";
        }

        #endregion
    }
}
