#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package list;

    ////import LongProcedure;
    /**
    Resizable list holding <code>long</code> elements; implemented with arrays.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    */

    [Serializable]
    public class LongArrayList : AbstractLongList
    {
        /**
         * The array buffer into which the elements of the list are stored.
         * The capacity of the list is the Length of this array buffer.
         * @serial
         */
        public long[] m_elements;
        /**
         * Constructs an empty list.
         */

        public LongArrayList()
            : this(10)
        {
        }

        /**
         * Constructs a list containing the specified elements. 
         * The initial size and capacity of the list is the Length of the array.
         *
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
         * 
         * @param elements the array to be backed by the the constructed list
         */

        public LongArrayList(long[] elements_)
        {
            elements(elements_);
        }

        /**
         * Constructs an empty list with the specified initial capacity.
         *
         * @param   initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.
         */

        public LongArrayList(int initialCapacity)
            : this(new long[initialCapacity])
        {
            setSizeRaw(0);
        }

        /**
         * Appends the specified element to the end of this list.
         *
         * @param element element to be appended to this list.
         */

        public new void Add(long element)
        {
            // overridden for performance only.
            if (size == m_elements.Length)
            {
                ensureCapacity(size + 1);
            }
            m_elements[size++] = element;
        }

        /**
         * Inserts the specified element before the specified position into the receiver. 
         * Shifts the element currently at that position (if any) and
         * any subsequent elements to the right.
         *
         * @param index index before which the specified element is to be inserted (must be in [0,size]).
         * @param element element to be inserted.
         * @exception HCException index is out of range (<tt>index &lt; 0 || index &gt; Size()</tt>).
         */

        public new void beforeInsert(int index, long element)
        {
            // overridden for performance only.
            if (index > size || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + size);
            }
            ensureCapacity(size + 1);
            Array.Copy(m_elements, index, m_elements, index + 1, size - index);
            m_elements[index] = element;
            size++;
        }

        /**
         * Searches the receiver for the specified value using
         * the binary search algorithm.  The receiver must <strong>must</strong> be
         * sorted (as by the sort method) prior to making this call.  If
         * it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the receiver contains multiple elements
         * equal to the specified object, there is no guarantee which instance
         * will be found.
         *
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return index of the search key, if it is contained in the receiver;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the receiver: the index of the first
         *	       element greater than the key, or <tt>receiver.Size()</tt>, if all
         *	       elements in the receiver are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see Sorting
         * @see Arrays
         */

        public new int binarySearchFromTo(long key, int from, int to)
        {
            return Sorting.binarySearchFromTo(m_elements, key, from, to);
        }

        /**
         * Returns a deep copy of the receiver. 
         *
         * @return  a deep copy of the receiver.
         */

        public new Object Clone()
        {
            // overridden for performance only.
            LongArrayList Clone = new LongArrayList((long[]) m_elements.Clone());
            Clone.setSizeRaw(size);
            return Clone;
        }

        /**
         * Returns a deep copy of the receiver; uses <code>Clone()</code> and casts the result.
         *
         * @return  a deep copy of the receiver.
         */

        public LongArrayList Copy()
        {
            return (LongArrayList) Clone();
        }

        /**
        * Sorts the specified range of the receiver into ascending numerical order. 
        *
        * The sorting algorithm is a count sort. This algorithm offers guaranteed
        * <dt>Performance: O(Max(n,max-min+1)).
        * <dt>Space requirements: int[max-min+1] buffer.
        * <p>This algorithm is only applicable if max-min+1 is not large!
        * But if applicable, it usually outperforms quicksort by a factor of 3-4.
        *
        * @param from the index of the first element (inclusive) to be sorted.
        * @param to the index of the last element (inclusive) to be sorted.
        * @param min the smallest element contained in the range.
        * @param max the largest element contained in the range.
        */

        public void countSortFromTo(int from, int to, long min, long max)
        {
            if (size == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, size);

            int width = (int) (max - min + 1);

            int[] counts = new int[width];
            long[] theElements = m_elements;
            for (int i = from; i <= to;)
            {
                counts[(int) (theElements[i++] - min)]++;
            }

            int fromIndex = from;
            long val = min;
            for (int i = 0; i < width; i++, val++)
            {
                int c = counts[i];
                if (c > 0)
                {
                    if (c == 1)
                    {
                        theElements[fromIndex++] = val;
                    }
                    else
                    {
                        int toIndex = fromIndex + c - 1;
                        fillFromToWith(fromIndex, toIndex, val);
                        fromIndex = toIndex + 1;
                    }
                }
            }
        }

        /**
         * Returns the elements currently stored, including invalid elements between size and capacity, if any.
         *
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
         *
         * @return the elements currently stored.
         */

        public new long[] elements()
        {
            return m_elements;
        }

        /**
         * Sets the receiver's elements to be the specified array (not a copy of it).
         *
         * The size and capacity of the list is the Length of the array.
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
         *
         * @param elements the new elements to be stored.
         * @return the receiver itself.
         */

        public new AbstractLongList elements(long[] elements)
        {
            m_elements = elements;
            size = elements.Length;
            return this;
        }

        /**
         * Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
         * If necessary, allocates new internal memory and increases the capacity of the receiver.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public override void ensureCapacity(int minCapacity)
        {
            m_elements = Arrays.ensureCapacity(m_elements, minCapacity);
        }

        /**
         * Compares the specified Object with the receiver.  
         * Returns true if and only if the specified Object is also an ArrayList of the same type, both Lists have the
         * same size, and all corresponding pairs of elements in the two Lists are identical.
         * In other words, two Lists are defined to be equal if they contain the
         * same elements in the same order.
         *
         * @param otherObj the Object to be compared for equality with the receiver.
         * @return true if the specified Object is equal to the receiver.
         */

        public new bool Equals(Object otherObj)
        {
            //delta
            // overridden for performance only.
            if (!(otherObj is LongArrayList))
            {
                return base.Equals(otherObj);
            }
            if (this == otherObj)
            {
                return true;
            }
            if (otherObj == null)
            {
                return false;
            }
            LongArrayList other = (LongArrayList) otherObj;
            if (Size() != other.Size())
            {
                return false;
            }

            long[] theElements = elements();
            long[] otherElements = other.elements();
            for (int i = Size(); --i >= 0;)
            {
                if (theElements[i] != otherElements[i])
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Applies a procedure to each element of the receiver, if any.
         * Starts at index 0, moving rightwards.
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        public new bool forEach(LongProcedure procedure)
        {
            // overridden for performance only.
            long[] theElements = m_elements;
            int theSize = size;

            for (int i = 0; i < theSize;)
            {
                if (!procedure.Apply(theElements[i++]))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns the element at the specified position in the receiver.
         *
         * @param index index of element to return.
         * @exception HCException index is out of range (index
         * 		  &lt; 0 || index &gt;= Size()).
         */

        public new long get(int index)
        {
            // overridden for performance only.
            if (index >= size || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + size);
            }
            return m_elements[index];
        }

        /**
         * Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditions. 
         * Provided with invalid parameters this method may return invalid elements without throwing any exception!
         * <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
         * Precondition (unchecked): <tt>index &gt;= 0 && index &lt; Size()</tt>.
         *
         * @param index index of element to return.
         */

        public override long getQuick(int index)
        {
            return m_elements[index];
        }

        /**
         * Returns the index of the first occurrence of the specified
         * element. Returns <code>-1</code> if the receiver does not contain this element.
         * Searches between <code>from</code>, inclusive and <code>to</code>, inclusive.
         * Tests for identity.
         *
         * @param element element to search for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return  the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new int indexOfFromTo(long element, int from, int to)
        {
            // overridden for performance only.
            if (size == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, size);

            long[] theElements = m_elements;
            for (int i = from; i <= to; i++)
            {
                if (element == theElements[i])
                {
                    return i;
                } //found
            }
            return -1; //not found
        }

        /**
         * Returns the index of the last occurrence of the specified
         * element. Returns <code>-1</code> if the receiver does not contain this element.
         * Searches beginning at <code>to</code>, inclusive until <code>from</code>, inclusive.
         * Tests for identity.
         *
         * @param element element to search for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @return  the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new int lastIndexOfFromTo(long element, int from, int to)
        {
            // overridden for performance only.
            if (size == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, size);

            long[] theElements = m_elements;
            for (int i = to; i >= from; i--)
            {
                if (element == theElements[i])
                {
                    return i;
                } //found
            }
            return -1; //not found
        }

        /**
         * Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
         * @param from the index of the first element (inclusive).
         * @param to the index of the last element (inclusive).
         * @return a new list
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new AbstractLongList partFromTo(int from, int to)
        {
            if (size == 0)
            {
                return new LongArrayList(0);
            }

            checkRangeFromTo(from, to, size);

            long[] part = new long[to - from + 1];
            Array.Copy(m_elements, from, part, 0, to - from + 1);
            return new LongArrayList(part);
        }

        /**
        * Removes from the receiver all elements that are contained in the specified list.
        * Tests for identity.
        *
        * @param other the other list.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public new bool removeAll(AbstractLongList other)
        {
            // overridden for performance only.
            if (!(other is LongArrayList))
            {
                return base.removeAll(other);
            }

            /* There are two possibilities to do the thing
               a) use other.IndexOf(...)
               b) sort other, then use other.binarySearch(...)
	   
               Let's try to figure out which one is faster. Let M=size, N=other.size, then
               a) takes O(M*N) steps
               b) takes O(N*logN + M*logN) steps (sorting is O(N*logN) and binarySearch is O(logN))
 
               Hence, if N*logN + M*logN < M*N, we use b) otherwise we use a).
            */
            if (other.Size() == 0)
            {
                return false;
            } //nothing to do
            int limit = other.Size() - 1;
            int j = 0;
            long[] theElements = m_elements;
            int mySize = Size();

            double N = other.Size();
            double M = mySize;
            if ((N + M)*Arithmetic.log2(N) < M*N)
            {
                // it is faster to sort other before searching in it
                LongArrayList sortedList = (LongArrayList) other.Clone();
                sortedList.quickSort();

                for (int i = 0; i < mySize; i++)
                {
                    if (sortedList.binarySearchFromTo(theElements[i], 0, limit) < 0)
                    {
                        theElements[j++] = theElements[i];
                    }
                }
            }
            else
            {
                // it is faster to search in other without sorting
                for (int i = 0; i < mySize; i++)
                {
                    if (other.indexOfFromTo(theElements[i], 0, limit) < 0)
                    {
                        theElements[j++] = theElements[i];
                    }
                }
            }

            bool modified = (j != mySize);
            setSize(j);
            return modified;
        }

        /**
         * Replaces a number of elements in the receiver with the same number of elements of another list.
         * Replaces elements in the receiver, between <code>from</code> (inclusive) and <code>to</code> (inclusive),
         * with elements of <code>other</code>, starting from <code>otherFrom</code> (inclusive).
         *
         * @param from the position of the first element to be replaced in the receiver
         * @param to the position of the last element to be replaced in the receiver
         * @param other list holding elements to be copied into the receiver.
         * @param otherFrom position of first element within other list to be copied.
         */

        public new void replaceFromToWithFrom(int from, int to, AbstractLongList other, int otherFrom)
        {
            // overridden for performance only.
            if (!(other is LongArrayList))
            {
                // slower
                base.replaceFromToWithFrom(from, to, other, otherFrom);
                return;
            }
            int Length = to - from + 1;
            if (Length > 0)
            {
                checkRangeFromTo(from, to, Size());
                checkRangeFromTo(otherFrom, otherFrom + Length - 1, other.Size());
                Array.Copy(((LongArrayList) other).m_elements, otherFrom, m_elements, from, Length);
            }
        }

        /**
        * Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        * In other words, removes from the receiver all of its elements that are not contained in the
        * specified other list. 
        * @param other the other list to test against.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public new bool retainAll(AbstractLongList other)
        {
            // overridden for performance only.
            if (!(other is LongArrayList))
            {
                return base.retainAll(other);
            }

            /* There are two possibilities to do the thing
               a) use other.IndexOf(...)
               b) sort other, then use other.binarySearch(...)
	   
               Let's try to figure out which one is faster. Let M=size, N=other.size, then
               a) takes O(M*N) steps
               b) takes O(N*logN + M*logN) steps (sorting is O(N*logN) and binarySearch is O(logN))

               Hence, if N*logN + M*logN < M*N, we use b) otherwise we use a).
            */
            int limit = other.Size() - 1;
            int j = 0;
            long[] theElements = m_elements;
            int mySize = Size();

            double N = other.Size();
            double M = mySize;
            if ((N + M)*Arithmetic.log2(N) < M*N)
            {
                // it is faster to sort other before searching in it
                LongArrayList sortedList = (LongArrayList) other.Clone();
                sortedList.quickSort();

                for (int i = 0; i < mySize; i++)
                {
                    if (sortedList.binarySearchFromTo(theElements[i], 0, limit) >= 0)
                    {
                        theElements[j++] = theElements[i];
                    }
                }
            }
            else
            {
                // it is faster to search in other without sorting
                for (int i = 0; i < mySize; i++)
                {
                    if (other.indexOfFromTo(theElements[i], 0, limit) >= 0)
                    {
                        theElements[j++] = theElements[i];
                    }
                }
            }

            bool modified = (j != mySize);
            setSize(j);
            return modified;
        }

        /**
         * Reverses the elements of the receiver.
         * Last becomes first, second last becomes second first, and so on.
         */

        public override void reverse()
        {
            // overridden for performance only.
            long tmp;
            int limit = size/2;
            int j = size - 1;

            long[] theElements = m_elements;
            for (int i = 0; i < limit;)
            {
                //swap
                tmp = theElements[i];
                theElements[i++] = theElements[j];
                theElements[j--] = tmp;
            }
        }

        /**
         * Replaces the element at the specified position in the receiver with the specified element.
         *
         * @param index index of element to replace.
         * @param element element to be stored at the specified position.
         * @exception HCException index is out of range (index
         * 		  &lt; 0 || index &gt;= Size()).
         */

        public new void set(int index, long element)
        {
            // overridden for performance only.
            if (index >= size || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + size);
            }
            m_elements[index] = element;
        }

        /**
         * Replaces the element at the specified position in the receiver with the specified element; <b>WARNING:</b> Does not check preconditions.
         * Provided with invalid parameters this method may access invalid indexes without throwing any exception!
         * <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
         * Precondition (unchecked): <tt>index &gt;= 0 && index &lt; Size()</tt>.
         *
         * @param index index of element to replace.
         * @param element element to be stored at the specified position.
         */

        public override void setQuick(int index, long element)
        {
            m_elements[index] = element;
        }

        /**
         * Randomly permutes the part of the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive). 
         * @param from the index of the first element (inclusive) to be permuted.
         * @param to the index of the last element (inclusive) to be permuted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void shuffleFromTo(int from, int to)
        {
            // overridden for performance only.
            if (size == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, size);

            RngWrapper gen = new RngWrapper();
            long tmpElement;
            long[] theElements = m_elements;
            int random;
            for (int i = from; i < to; i++)
            {
                random = gen.NextInt(i, to);

                //swap(i, random)
                tmpElement = theElements[random];
                theElements[random] = theElements[i];
                theElements[i] = tmpElement;
            }
        }

        /**
         * Sorts the specified range of the receiver into ascending order. 
         *
         * The sorting algorithm is dynamically chosen according to the characteristics of the data set.
         * Currently quicksort and countsort are considered.
         * Countsort is not always applicable, but if applicable, it usually outperforms quicksort by a factor of 3-4.
         *
         * <p>Best case performance: O(N).
         * <dt>Worst case performance: O(N^2) (a degenerated quicksort).
         * <dt>Best case space requirements: 0 KB. 
         * <dt>Worst case space requirements: 40 KB.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new void sortFromTo(int from, int to)
        {
            /* 
             * Computes min and max and decides on this basis.
             * In practice the additional overhead is very small compared to the potential gains.
             */
            int widthThreshold = 10000; // never consider options resulting in outrageous memory allocations.

            if (size == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, size);

            // determine minimum and maximum.
            long min = m_elements[from];
            long max = m_elements[from];

            long[] theElements = m_elements;
            for (int i = from + 1; i <= to;)
            {
                long elem = theElements[i++];
                if (elem > max)
                {
                    max = elem;
                }
                else if (elem < min)
                {
                    min = elem;
                }
            }

            // try to figure out which option is fastest.
            double N = to - (double) from + 1.0;
            double quickSortEstimate = N*Math.Log(N)/0.6931471805599453;
            // O(N*log(N,base=2)) ; ln(2)=0.6931471805599453

            double width = max - (double) min + 1.0;
            double countSortEstimate = Math.Max(width, N); // O(Max(width,N))

            if (width < widthThreshold && countSortEstimate < quickSortEstimate)
            {
                countSortFromTo(from, to, min, max);
            }
            else
            {
                quickSortFromTo(from, to);
            }
        }

        /**
         * Trims the capacity of the receiver to be the receiver's current 
         * size. Releases any superfluous internal memory. An application can use this operation to minimize the 
         * storage of the receiver.
         */

        public new void trimToSize()
        {
            m_elements = Arrays.trimToCapacity(m_elements, Size());
        }
    }
}
