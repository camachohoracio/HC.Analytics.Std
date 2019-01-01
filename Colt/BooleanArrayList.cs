#region

using System;
using System.Collections;
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

    ////import BooleanProcedure;
    /**
    Resizable list holding <code>bool</code> elements; implemented with arrays.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    */

    [Serializable]
    public class BooleanArrayList : AbstractBooleanList
    {
        /**
         * The array buffer into which the elements of the list are stored.
         * The capacity of the list is the Length of this array buffer.
         * @serial
         */
        public bool[] m_elements;
        /**
         * Constructs an empty list.
         */

        public BooleanArrayList()
            : this(10)
        {
        }

        /**
         * Constructs a list containing the specified elements. 
         * The initial m_intSize and capacity of the list is the Length of the array.
         *
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
         * 
         * @param elements the array to be backed by the the constructed list
         */

        public BooleanArrayList(bool[] elements_)
        {
            elements(elements_);
        }

        /**
         * Constructs an empty list with the specified initial capacity.
         *
         * @param   initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.
         */

        public BooleanArrayList(int initialCapacity)
            : this(new bool[initialCapacity])
        {
            setSizeRaw(0);
        }

        /**
         * Appends the specified element to the end of this list.
         *
         * @param element element to be appended to this list.
         */

        public new void Add(bool element)
        {
            // overridden for performance only.
            if (m_intSize == m_elements.Length)
            {
                ensureCapacity(m_intSize + 1);
            }
            m_elements[m_intSize++] = element;
        }

        /**
         * Inserts the specified element before the specified position into the receiver. 
         * Shifts the element currently at that position (if any) and
         * any subsequent elements to the right.
         *
         * @param index index before which the specified element is to be inserted (must be in [0,m_intSize]).
         * @param element element to be inserted.
         * @exception HCException index is out of range (<tt>index &lt; 0 || index &gt; Size()</tt>).
         */

        public new void beforeInsert(int index, bool element)
        {
            // overridden for performance only.
            if (index > m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
            }
            ensureCapacity(m_intSize + 1);
            Array.Copy(m_elements, index, m_elements, index + 1, m_intSize - index);
            m_elements[index] = element;
            m_intSize++;
        }

        /**
         * Returns a deep copy of the receiver. 
         *
         * @return  a deep copy of the receiver.
         */

        public new Object Clone()
        {
            // overridden for performance only.
            BooleanArrayList Clone = new BooleanArrayList((bool[]) m_elements.Clone());
            Clone.setSizeRaw(m_intSize);
            return Clone;
        }

        /**
         * Returns a deep copy of the receiver; uses <code>Clone()</code> and casts the result.
         *
         * @return  a deep copy of the receiver.
         */

        public BooleanArrayList Copy()
        {
            return (BooleanArrayList) Clone();
        }

        /**
         * Sorts the specified range of the receiver into ascending numerical order (<tt>false &lt; true</tt>). 
         *
         * The sorting algorithm is a count sort. This algorithm offers guaranteed
         * O(n) performance without auxiliary memory.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         */

        public void countSortFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);

            bool[] theElements = m_elements;
            int trues = 0;
            for (int i = from; i <= to;)
            {
                if (theElements[i++])
                {
                    trues++;
                }
            }

            int falses = to - from + 1 - trues;
            if (falses > 0)
            {
                fillFromToWith(from, from + falses - 1, false);
            }
            if (trues > 0)
            {
                fillFromToWith(from + falses, from + falses - 1 + trues, true);
            }
        }

        /**
         * Returns the elements currently stored, including invalid elements between m_intSize and capacity, if any.
         *
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
         *
         * @return the elements currently stored.
         */

        public new bool[] elements()
        {
            return m_elements;
        }

        /**
         * Sets the receiver's elements to be the specified array (not a copy of it).
         *
         * The m_intSize and capacity of the list is the Length of the array.
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
         *
         * @param elements the new elements to be stored.
         * @return the receiver itself.
         */

        public new AbstractBooleanList elements(bool[] elements)
        {
            m_elements = elements;
            m_intSize = elements.Length;
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
         * same m_intSize, and all corresponding pairs of elements in the two Lists are identical.
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
            if (!(otherObj is BooleanArrayList))
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
            BooleanArrayList other = (BooleanArrayList) otherObj;
            if (Size() != other.Size())
            {
                return false;
            }

            bool[] theElements = elements();
            bool[] otherElements = other.elements();
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

        public new bool forEach(BooleanProcedure procedure)
        {
            // overridden for performance only.
            bool[] theElements = m_elements;
            int theSize = m_intSize;

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

        public new bool get(int index)
        {
            // overridden for performance only.
            if (index >= m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
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

        public override bool getQuick(int index)
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

        public new int indexOfFromTo(bool element, int from, int to)
        {
            // overridden for performance only.
            if (m_intSize == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, m_intSize);

            bool[] theElements = m_elements;
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

        public new int lastIndexOfFromTo(bool element, int from, int to)
        {
            // overridden for performance only.
            if (m_intSize == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, m_intSize);

            bool[] theElements = m_elements;
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
         * Sorts the specified range of the receiver into ascending order (<tt>false &lt; true</tt>). 
         *
         * The sorting algorithm is <b>not</b> a mergesort, but rather a countsort.
         * This algorithm offers guaranteed O(n) performance.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void mergeSortFromTo(int from, int to)
        {
            countSortFromTo(from, to);
        }

        /**
         * Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
         * @param from the index of the first element (inclusive).
         * @param to the index of the last element (inclusive).
         * @return a new list
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new AbstractBooleanList partFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return new BooleanArrayList(0);
            }

            checkRangeFromTo(from, to, m_intSize);

            bool[] part = new bool[to - from + 1];
            Array.Copy(m_elements, from, part, 0, to - from + 1);
            return new BooleanArrayList(part);
        }

        /**
         * Sorts the specified range of the receiver into ascending order (<tt>false &lt; true</tt>). 
         *
         * The sorting algorithm is <b>not</b> a quicksort, but rather a countsort.
         * This algorithm offers guaranteed O(n) performance.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void quickSortFromTo(int from, int to)
        {
            countSortFromTo(from, to);
        }

        /**
        * Removes from the receiver all elements that are contained in the specified list.
        * Tests for identity.
        *
        * @param other the other list.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public new bool removeAll(AbstractBooleanList other)
        {
            // overridden for performance only.
            if (!(other is BooleanArrayList))
            {
                return base.removeAll(other);
            }

            /* There are two possibilities to do the thing
               a) use other.IndexOf(...)
               b) sort other, then use other.binarySearch(...)
	   
               Let's try to figure out which one is faster. Let M=m_intSize, N=other.m_intSize, then
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
            bool[] theElements = m_elements;
            int mySize = Size();

            double N = other.Size();
            double M = mySize;
            if ((N + M)*Arithmetic.log2(N) < M*N)
            {
                // it is faster to sort other before searching in it
                BooleanArrayList sortedList = (BooleanArrayList) other.Clone();
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

        public new void replaceFromToWithFrom(int from, int to, AbstractBooleanList other, int otherFrom)
        {
            // overridden for performance only.
            if (!(other is BooleanArrayList))
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
                Array.Copy(((BooleanArrayList) other).m_elements, otherFrom, m_elements, from, Length);
            }
        }

        /**
        * Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        * In other words, removes from the receiver all of its elements that are not contained in the
        * specified other list. 
        * @param other the other list to test against.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public new bool retainAll(AbstractBooleanList other)
        {
            // overridden for performance only.
            if (!(other is BooleanArrayList))
            {
                return base.retainAll(other);
            }

            /* There are two possibilities to do the thing
               a) use other.IndexOf(...)
               b) sort other, then use other.binarySearch(...)
	   
               Let's try to figure out which one is faster. Let M=m_intSize, N=other.m_intSize, then
               a) takes O(M*N) steps
               b) takes O(N*logN + M*logN) steps (sorting is O(N*logN) and binarySearch is O(logN))

               Hence, if N*logN + M*logN < M*N, we use b) otherwise we use a).
            */
            int limit = other.Size() - 1;
            int j = 0;
            bool[] theElements = m_elements;
            int mySize = Size();

            double N = other.Size();
            double M = mySize;
            if ((N + M)*Arithmetic.log2(N) < M*N)
            {
                // it is faster to sort other before searching in it
                BooleanArrayList sortedList = (BooleanArrayList) other.Clone();
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

        public override void replaceFromWith(int from, CollectionBase other)
        {
            throw new NotImplementedException();
        }

        public new void reverse()
        {
            // overridden for performance only.
            bool tmp;
            int limit = m_intSize/2;
            int j = m_intSize - 1;

            bool[] theElements = m_elements;
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

        public new void set(int index, bool element)
        {
            // overridden for performance only.
            if (index >= m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
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

        public override void setQuick(int index, bool element)
        {
            m_elements[index] = element;
        }

        /**
         * Randomly permutes the part of the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive). 
         * @param from the index of the first element (inclusive) to be permuted.
         * @param to the index of the last element (inclusive) to be permuted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new void shuffleFromTo(int from, int to)
        {
            // overridden for performance only.
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);

            RngWrapper gen = new RngWrapper();
            bool tmpElement;
            bool[] theElements = m_elements;
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
         * The sorting algorithm is countsort.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public new void sortFromTo(int from, int to)
        {
            countSortFromTo(from, to);
        }

        /**
         * Trims the capacity of the receiver to be the receiver's current 
         * m_intSize. Releases any superfluos internal memory. An application can use this operation to minimize the 
         * storage of the receiver.
         */

        public new void trimToSize()
        {
            m_elements = Arrays.trimToCapacity(m_elements, Size());
        }
    }
}
