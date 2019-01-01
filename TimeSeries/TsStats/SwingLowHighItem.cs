using System;
using System.Collections.Generic;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class SwingLowHighItem : IComparer<SwingLowHighItem>
    {
        public DateTime Date
        {
            get;
            set;
        }
        public double Value
        {
            get;
            set;
        }

        public int Compare(SwingLowHighItem x, SwingLowHighItem y)
        {
            int intCompare = x.Value.CompareTo(y.Value);
            if (intCompare == 0)
            {
                intCompare = x.GetHashCode().CompareTo(y.GetHashCode());
                if (intCompare == 0)
                {
                    return x.Date.CompareTo(y.Date);
                }
            }
            return intCompare;
        }

        public TsRow2D ConvertToTsRow()
        {
            return new TsRow2D(Date,Value);
        }
    }
}