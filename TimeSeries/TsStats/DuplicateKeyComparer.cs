using System;
using System.Collections.Generic;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class SortedItem : IComparable
    {
        public int Index { get; set; }
        public double Value { get; set; }

        public int CompareTo(object obj)
        {
            var other = (SortedItem)obj;
            if (other.Value == Value)
            {
                return Index.CompareTo(other.Index);
            }
            return Value.CompareTo(other.Value);
        }
    }

    public class DuplicateKeyComparer<SortedItem>
        :
            IComparer<SortedItem> where SortedItem : IComparable
    {

        public int Compare(
            SortedItem x,
            SortedItem y)
        {
            return x.CompareTo(y);
        }

    }
}