#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class BinFunction1D11_ : BinFunction1D
    {
        private readonly double percentage_;

        public BinFunction1D11_(double percentage)
        {
            percentage_ = percentage;
        }

        #region BinFunction1D Members

        public double Apply(DynamicBin1D bin)
        {
            return bin.quantile(percentage_);
        }

        public string name()
        {
            return new FormerFactory().create("%1.2G").form(percentage_*100) + "% Q.";
        }

        #endregion
    }
}
