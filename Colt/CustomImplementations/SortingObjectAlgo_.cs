#region

using System;
using HC.Analytics.Colt.objectAlgo;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class SortingObjectAlgo_ : SortingObjectAlgo
    {
        public new void runSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            Sorting.mergeSort(
                a,
                fromIndex,
                toIndex,
                c);
        }

        public new void runSort(int fromIndex, int toIndex, IntComparator c, Swapper swapper)
        {
            GenericSorting.mergeSort(fromIndex, toIndex, c, swapper);
        }
    }
}
