#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries
{
    public class SimpleDateComparer : IComparer<DateTime>
    {
        #region IComparer<DateTime> Members

        public int Compare(DateTime x, DateTime y)
        {
            return x.CompareTo(y);
        }

        #endregion
    }
}
