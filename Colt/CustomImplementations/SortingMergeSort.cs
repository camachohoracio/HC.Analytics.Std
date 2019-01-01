#region

using System;
using HC.Analytics.Colt.doubleAlgo;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class SortingMergeSort : SortingDoubleAlgo
    {
        public new void runSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            Sorting.mergeSort(a, fromIndex, toIndex, c);
        }

        public new void runSort(int fromIndex, int toIndex, IntComparator c, Swapper swapper)
        {
            GenericSorting.mergeSort(fromIndex, toIndex, c, swapper);
        }
    }
}
