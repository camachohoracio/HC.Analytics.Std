#region

using System;
using System.Collections;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, Copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package cern.colt;

    ////import IComparer;

    ////import ByteComparator;
    ////import CharComparator;
    ////import DoubleComparator;
    ////import FloatComparator;
    ////import IntComparator;
    ////import LongComparator;
    ////import ShortComparator;
    /**
     * Quicksorts, mergesorts and binary searches; complements <tt>Arrays</tt>.
     * Contains, for example, the quicksort on Comparators and Comparables, which are still missing in <tt>Arrays</tt> of JDK 1.2.
     * Also provides mergesorts for types not supported in <tt>Arrays</tt>, as well as a couple of other methods for primitive arrays.
     * The quicksorts and mergesorts are the JDK 1.2 V1.26 algorithms, modified as necessary.
     *
     * @see GenericSorting
     * @see doublealgo.Sorting
     * @see Arrays
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 03-Jul-99
     */

    [Serializable]
    public class Sorting : Object
    {
        private static int MEDIUM = 40;
        private static int SMALL = 7;
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(byte[] list, byte key, int from, int to)
        {
            byte midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(char[] list, char key, int from, int to)
        {
            char midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(double[] list, double key, int from, int to)
        {
            double midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(float[] list, float key, int from, int to)
        {
            float midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(int[] list, int key, int from, int to)
        {
            int midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.

            /*
            // even for very short lists (0,1,2,3 elems) this is only 10% faster
            while (from<=to && list[from++] < key) ;
            if (from<=to) {
                if (list[--from] == key) return from;
            }
            return -(from + 1);
            */
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(long[] list, long key, int from, int to)
        {
            long midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm. The list must be sorted into ascending order
         * according to the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * If the list is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which instance
         * will be found.
         *
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @param comparator the comparator by which the list is sorted.
         * @throws ClassCastException if the list contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         * @see IComparer
         */

        public static int binarySearchFromTo(
            Object[] list,
            Object key,
            int from,
            int to,
            IComparer comparator)
        {
            Object midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                int cmp = comparator.Compare(midVal, key);

                if (cmp < 0)
                {
                    from = mid + 1;
                }
                else if (cmp > 0)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(short[] list, short key, int from, int to)
        {
            short midVal;
            while (from <= to)
            {
                int mid = (from + to)/2;
                midVal = list[mid];
                if (midVal < key)
                {
                    from = mid + 1;
                }
                else if (midVal > key)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        /**
         * Generically searches the list for the specified value using
         * the binary search algorithm.  The list must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the list contains multiple elements
         * equal to the specified key, there is no guarantee which of the multiple elements
         * will be found.
         *
         * @param list the list to be searched.
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the list;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the list: the index of the first
         *	       element greater than the key, or <tt>list.Length</tt>, if all
         *	       elements in the list are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Arrays
         */

        public static int binarySearchFromTo(int from, int to, IntComparator comp)
        {
            int dummy = 0;
            while (from <= to)
            {
                int mid = (from + to)/2;
                int comparison = comp.Compare(dummy, mid);
                if (comparison < 0)
                {
                    from = mid + 1;
                }
                else if (comparison > 0)
                {
                    to = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(from + 1); // key not found.
        }

        private static int lower_bound(int[] array, int first, int last, int x)
        {
            int len = last - first;
            while (len > 0)
            {
                int half = len/2;
                int middle = first + half;
                if (array[middle] < x)
                {
                    first = middle + 1;
                    len -= half + 1;
                }
                else
                {
                    len = half;
                }
            }
            return first;
        }

        private static int upper_bound(int[] array, int first, int last, int x)
        {
            int len = last - first;
            while (len > 0)
            {
                int half = len/2;
                int middle = first + half;
                if (x < array[middle])
                {
                    len = half;
                }
                else
                {
                    first = middle + 1;
                    len -= half + 1;
                }
            }
            return first;
        }

        private static void inplace_merge(int[] array, int first, int middle, int last)
        {
            if (first >= middle || middle >= last)
            {
                return;
            }
            if (last - first == 2)
            {
                if (array[middle] < array[first])
                {
                    int tmp = array[first];
                    array[first] = array[middle];
                    array[middle] = tmp;
                }
                return;
            }
            int firstCut;
            int secondCut;
            if (middle - first > last - middle)
            {
                firstCut = first + (middle - first)/2;
                secondCut = lower_bound(array, middle, last, array[firstCut]);
            }
            else
            {
                secondCut = middle + (last - middle)/2;
                firstCut = upper_bound(array, first, middle, array[secondCut]);
            }

            //rotate(array, firstCut, middle, secondCut);
            // is manually inlined for speed (jitter inlining seems to work only for small call depths, even if methods are "static private")
            // speedup = 1.7
            // begin inline
            int first2 = firstCut;
            int middle2 = middle;
            int last2 = secondCut;
            if (middle2 != first2 && middle2 != last2)
            {
                int first1 = first2;
                int last1 = middle2;
                int tmp;
                while (first1 < --last1)
                {
                    tmp = array[first1];
                    array[last1] = array[first1];
                    array[first1++] = tmp;
                }
                first1 = middle2;
                last1 = last2;
                while (first1 < --last1)
                {
                    tmp = array[first1];
                    array[last1] = array[first1];
                    array[first1++] = tmp;
                }
                first1 = first2;
                last1 = last2;
                while (first1 < --last1)
                {
                    tmp = array[first1];
                    array[last1] = array[first1];
                    array[first1++] = tmp;
                }
            }
            // end inline


            middle = firstCut + (secondCut - middle);
            inplace_merge(array, first, firstCut, middle);
            inplace_merge(array, middle, secondCut, last);
        }

        /**
     * Returns the index of the median of the three indexed chars.
     */

        private static int med3(byte[] x, int a, int b, int c, ByteComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(char[] x, int a, int b, int c, CharComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(double[] x, int a, int b, int c, DoubleComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(float[] x, int a, int b, int c, FloatComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(int[] x, int a, int b, int c, IntComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(long[] x, int a, int b, int c, LongComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(Object[] x, int a, int b, int c)
        {
            int ab = ((IComparable) x[a]).CompareTo(x[b]);
            int ac = ((IComparable) x[a]).CompareTo(x[c]);
            int bc = ((IComparable) x[b]).CompareTo(x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(Object[] x, int a, int b, int c,
                                IComparer comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Returns the index of the median of the three indexed chars.
         */

        private static int med3(short[] x, int a, int b, int c, ShortComparator comp)
        {
            int ab = comp.Compare(x[a], x[b]);
            int ac = comp.Compare(x[a], x[c]);
            int bc = comp.Compare(x[b], x[c]);
            return (ab < 0
                        ?
                            (bc < 0 ? b : ac < 0 ? c : a)
                        :
                            (bc > 0 ? b : ac > 0 ? c : a));
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(byte[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            byte[] aux = (byte[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(byte[] a, int fromIndex, int toIndex, ByteComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            byte[] aux = (byte[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(char[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            char[] aux = (char[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(char[] a, int fromIndex, int toIndex, CharComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            char[] aux = (char[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(double[] a, int fromIndex, int toIndex)
        {
            mergeSort2(a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(double[] a, int fromIndex, int toIndex, DoubleComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            double[] aux = (double[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(float[] a, int fromIndex, int toIndex)
        {
            mergeSort2(a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(float[] a, int fromIndex, int toIndex, FloatComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            float[] aux = (float[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(int[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            int[] aux = (int[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            int[] aux = (int[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(long[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            long[] aux = (long[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(long[] a, int fromIndex, int toIndex, LongComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            long[] aux = (long[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSort(short[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            short[] aux = (short[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void mergeSort(short[] a, int fromIndex, int toIndex, ShortComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            short[] aux = (short[]) a.Clone();
            mergeSort1(aux, a, fromIndex, toIndex, c);
        }

        private static void mergeSort1(byte[] src, byte[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(byte[] src, byte[] dest, int low, int high, ByteComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(char[] src, char[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(char[] src, char[] dest, int low, int high, CharComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(double[] src, double[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(double[] src, double[] dest, int low, int high, DoubleComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(float[] src, float[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(float[] src, float[] dest, int low, int high, FloatComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(int[] src, int[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(int[] src, int[] dest, int low, int high, IntComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(long[] src, long[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(long[] src, long[] dest, int low, int high, LongComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(short[] src, short[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid);
            mergeSort1(dest, src, mid, high);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort1(short[] src, short[] dest, int low, int high, ShortComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                {
                    for (int j = i; j > low && c.Compare(dest[j - 1], dest[j]) > 0; j--)
                    {
                        swap(dest, j, j - 1);
                    }
                }
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high)/2;
            mergeSort1(dest, src, low, mid, c);
            mergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just Copy from src to dest.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c.Compare(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c.Compare(src[p], src[q]) <= 0)
                {
                    dest[i] = src[p++];
                }
                else
                {
                    dest[i] = src[q++];
                }
            }
        }

        private static void mergeSort2(double[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);

            long NEG_ZERO_BITS = BitConverter.DoubleToInt64Bits(-0.0d);
            /*
             * The sort is done in three phases to avoid the expense of using
             * NaN and -0.0 aware comparisons during the main sort.
             */

            /*
             * Preprocessing phase:  Move any NaN's to end of array, count the
             * number of -0.0's, and turn them into 0.0's. 
             */
            int numNegZeros = 0;
            int i = fromIndex, n = toIndex;
            while (i < n)
            {
                if (a[i] != a[i])
                {
                    a[i] = a[--n];
                    a[n] = double.NaN;
                }
                else
                {
                    if (a[i] == 0 && BitConverter.DoubleToInt64Bits(a[i]) == NEG_ZERO_BITS)
                    {
                        a[i] = 0.0d;
                        numNegZeros++;
                    }
                    i++;
                }
            }

            // Main sort phase: mergesort everything but the NaN's
            double[] aux = (double[]) a.Clone();
            mergeSort1(aux, a, fromIndex, n);

            // Postprocessing phase: change 0.0's to -0.0's as required
            if (numNegZeros != 0)
            {
                int j = new DoubleArrayList(a).binarySearchFromTo(0.0d, fromIndex, n - 1); // posn of ANY zero
                do
                {
                    j--;
                }
                while (j >= 0 && a[j] == 0.0d);

                // j is now one less than the index of the FIRST zero
                for (int k = 0; k < numNegZeros; k++)
                {
                    a[++j] = -0.0d;
                }
            }
        }

        private static void mergeSort2(float[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            int NEG_ZERO_BITS = (int) BitConverter.DoubleToInt64Bits(-0.0f);
            /*
             * The sort is done in three phases to avoid the expense of using
             * NaN and -0.0 aware comparisons during the main sort.
             */

            /*
             * Preprocessing phase:  Move any NaN's to end of array, count the
             * number of -0.0's, and turn them into 0.0's. 
             */
            int numNegZeros = 0;
            int i = fromIndex, n = toIndex;
            while (i < n)
            {
                if (a[i] != a[i])
                {
                    a[i] = a[--n];
                    a[n] = float.NaN;
                }
                else
                {
                    if (a[i] == 0 && (int) BitConverter.DoubleToInt64Bits(a[i]) == NEG_ZERO_BITS)
                    {
                        a[i] = 0.0f;
                        numNegZeros++;
                    }
                    i++;
                }
            }

            // Main sort phase: mergesort everything but the NaN's
            float[] aux = (float[]) a.Clone();
            mergeSort1(aux, a, fromIndex, n);

            // Postprocessing phase: change 0.0's to -0.0's as required
            if (numNegZeros != 0)
            {
                int j = new FloatArrayList(a).binarySearchFromTo(0.0f, fromIndex, n - 1); // posn of ANY zero
                do
                {
                    j--;
                }
                while (j >= 0 && a[j] == 0.0f);

                // j is now one less than the index of the FIRST zero
                for (int k = 0; k < numNegZeros; k++)
                {
                    a[++j] = -0.0f;
                }
            }
        }

        /**
         * Sorts the specified range of the specified array of elements.
         *
         * <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void mergeSortInPlace(int[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            int Length = toIndex - fromIndex;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = fromIndex; i < toIndex; i++)
                {
                    for (int j = i; j > fromIndex && a[j - 1] > a[j]; j--)
                    {
                        int tmp = a[j];
                        a[j] = a[j - 1];
                        a[j - 1] = tmp;
                    }
                }
                return;
            }

            // Recursively sort halves
            int mid = (fromIndex + toIndex)/2;
            mergeSortInPlace(a, fromIndex, mid);
            mergeSortInPlace(a, mid, toIndex);

            // If list is already sorted, nothing left to do.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (a[mid - 1] <= a[mid])
            {
                return;
            }

            // Merge sorted halves 
            //jal.INT.Sorting.inplace_merge(a, fromIndex, mid, toIndex);
            inplace_merge(a, fromIndex, mid, toIndex);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(byte[] a, int fromIndex, int toIndex, ByteComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(char[] a, int fromIndex, int toIndex, CharComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(double[] a, int fromIndex, int toIndex, DoubleComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(float[] a, int fromIndex, int toIndex, FloatComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(long[] a, int fromIndex, int toIndex, LongComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified range of the receiver into
         * ascending order, according to the <i>natural ordering</i> of its
         * elements.  All elements in this range must implement the
         * <tt>IComparable</tt> interface.  Furthermore, all elements in this range
         * must be <i>mutually comparable</i> (that is, <tt>e1.CompareTo(e2)</tt>
         * must not throw a <tt>ClassCastException</tt> for any elements
         * <tt>e1</tt> and <tt>e2</tt> in the array).<p>
         *
         * The sorting algorithm is a tuned quicksort, adapted from Jon
         * L. Bentley and M. Douglas McIlroy's "Engineering a Sort Function",
         * Software-Practice and Experience, Vol. 23(11) P. 1249-1265 (November
         * 1993).  This algorithm offers n*log(n) performance on many data sets
         * that cause other quicksorts to degrade to quadratic performance.
         *
         * @param a the array to be sorted.
         */

        public static void quickSort(Object[] a)
        {
            quickSort1(a, 0, a.Length);
        }

        /**
         * Sorts the specified range of the receiver into
         * ascending order, according to the <i>natural ordering</i> of its
         * elements.  All elements in this range must implement the
         * <tt>IComparable</tt> interface.  Furthermore, all elements in this range
         * must be <i>mutually comparable</i> (that is, <tt>e1.CompareTo(e2)</tt>
         * must not throw a <tt>ClassCastException</tt> for any elements
         * <tt>e1</tt> and <tt>e2</tt> in the array).<p>
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         */

        public static void quickSort(Object[] a, int fromIndex, int toIndex)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex);
        }

        /**
         * Sorts the specified range of the specified array according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the receiver.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(
            Object[] a,
            int fromIndex,
            int toIndex,
            IComparer c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified array according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param a the array to be sorted.
         * @param c the comparator to determine the order of the receiver.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(Object[] a, IComparer c)
        {
            quickSort1(a, 0, a.Length, c);
        }

        /**
         * Sorts the specified range of the specified array of elements according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * @param a the array to be sorted.
         * @param fromIndex the index of the first element (inclusive) to be
         *        sorted.
         * @param toIndex the index of the last element (exclusive) to be sorted.
         * @param c the comparator to determine the order of the array.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws HCException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         */

        public static void quickSort(short[] a, int fromIndex, int toIndex, ShortComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(byte[] x, int off, int len, ByteComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            byte v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(char[] x, int off, int len, CharComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            char v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(double[] x, int off, int len, DoubleComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            double v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(float[] x, int off, int len, FloatComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            float v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(int[] x, int off, int len, IntComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            int v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(long[] x, int off, int len, LongComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            long v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(Object[] x, int off, int len)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && ((IComparable) x[j - 1]).CompareTo(x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s);
                    m = med3(x, m - s, m, m + s);
                    n = med3(x, n - 2*s, n - s, n);
                }
                m = med3(x, l, m, n); // Mid-size, med of 3
            }
            IComparable v = (IComparable) x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = ((IComparable) x[b]).CompareTo(v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = ((IComparable) x[c]).CompareTo(v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(Object[] x, int off, int len,
                                       IComparer comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            Object v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Sorts the specified sub-array of chars into ascending order.
         */

        private static void quickSort1(short[] x, int off, int len, ShortComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                {
                    for (int j = i; j > off && comp.Compare(x[j - 1], x[j]) > 0; j--)
                    {
                        swap(x, j, j - 1);
                    }
                }
                return;
            }

            // Choose a partition element, v
            int m = off + len/2; // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {
                    // Big arrays, pseudomedian of 9
                    int s = len/8;
                    l = med3(x, l, l + s, l + 2*s, comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - 2*s, n - s, n, comp);
                }
                m = med3(x, l, m, n, comp); // Mid-size, med of 3
            }
            short v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp.Compare(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, a++, b);
                    }
                    b++;
                }
                while (c >= b && (comparison = comp.Compare(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                    {
                        swap(x, c, d--);
                    }
                    c--;
                }
                if (b > c)
                {
                    break;
                }
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int s_, n_ = off + len;
            s_ = Math.Min(a - off, b - a);
            vecswap(x, off, b - s_, s_);
            s_ = Math.Min(d - c, n_ - d - 1);
            vecswap(x, b, n_ - s_, s_);

            // Recursively sort non-partition-elements
            if ((s_ = b - a) > 1)
            {
                quickSort1(x, off, s_, comp);
            }
            if ((s_ = d - c) > 1)
            {
                quickSort1(x, n_ - s_, s_, comp);
            }
        }

        /**
         * Check that fromIndex and toIndex are in range, and throw an
         * appropriate exception if they aren't.
         */

        private static void rangeCheck(int arrayLen, int fromIndex, int toIndex)
        {
            if (fromIndex > toIndex)
            {
                throw new ArgumentException("fromIndex(" + fromIndex +
                                            ") > toIndex(" + toIndex + ")");
            }
            if (fromIndex < 0)
            {
                throw new HCException(fromIndex.ToString());
            }
            if (toIndex > arrayLen)
            {
                throw new HCException(toIndex.ToString());
            }
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(byte[] x, int a, int b)
        {
            byte t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(char[] x, int a, int b)
        {
            char t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(double[] x, int a, int b)
        {
            double t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(float[] x, int a, int b)
        {
            float t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(int[] x, int a, int b)
        {
            int t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(long[] x, int a, int b)
        {
            long t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(Object[] x, int a, int b)
        {
            Object t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a] with x[b].
         */

        private static void swap(short[] x, int a, int b)
        {
            short t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(byte[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(char[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(double[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(float[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(int[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(long[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(Object[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }

        /**
         * Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
         */

        private static void vecswap(short[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
            {
                swap(x, a, b);
            }
        }
    }
}
