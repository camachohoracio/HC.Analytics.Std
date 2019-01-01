#region

using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries
{
    public class TimeSeriesDateComparer : IComparer<TsRow2D>
    {
        #region IComparer<TsRow2D> Members

        public int Compare(TsRow2D x, TsRow2D y)
        {
            return x.Time.CompareTo(y.Time);
        }

        #endregion
    }
}
