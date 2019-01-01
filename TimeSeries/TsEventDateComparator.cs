using System.Collections.Generic;

namespace HC.Analytics.TimeSeries
{
    public class TsEventDateComparator : IComparer<ATsEvent>
    {
        public int Compare(ATsEvent x, ATsEvent y)
        {
            return x.Time.CompareTo(y.Time);
        }
    }
}
