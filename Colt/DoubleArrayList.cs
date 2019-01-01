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
    Permission to use, Copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package list;

    ////import DoubleProcedure;
    /**
    Resizable list holding <code>double</code> elements; implemented with arrays.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    */

    [Serializable]
    public class DoubleArrayList : AbstractDoubleList
    {
        /**
         * The array buffer into which the elements of the list are stored.
         * The capacity of the list is the Length of this array buffer.
         * @serial
         */
        public double[] m_elements;
        /**
         * Constructs an empty list.
         */

        public DoubleArrayList()
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

        public DoubleArrayList(double[] elements_)
        {
            elements(elements_);
        }

        /**
         * Constructs an empty list with the specified initial capacity.
         *
         * @param   initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.
         */

        public DoubleArrayList(int initialCapacity)
            : this(new double[initialCapacity])
        {
            setSizeRaw(0);
        }

        /**
         * Appends the specified element to the end of this list.
         *
         * @param element element to be appended to this list.
         */

        public new void Add(double element)
        {
            // overridden for performance only.  
            if (m_size == m_elements.Length)
            {
                ensureCapacity(m_size + 1);
            }
            m_elements[m_size++] = element;
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

        public new void beforeInsert(int index, double element)
        {
            // overridden for performance only.
            if (m_size == index)
            {
                Add(element);
                return;
            }
            if (index > m_size || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_size);
            }
            ensureCapacity(m_size + 1);
            Array.Copy(m_elements, index, m_elements, index + 1, m_size - index);
            m_elements[index] = element;
            m_size++;
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

        public new int binarySearchFromTo(double key, int from, int to)
        {
            return Sorting.binarySearchFromTo(m_elements, key, from, to);
        }

        /**
         * Returns a deep Copy of the receiver. 
         *
         * @return  a deep Copy of the receiver.
         */

        public new Object Clone()
        {
            // overridden for performance only.
            DoubleArrayList Clone = new DoubleArrayList((double[]) m_elements.Clone());
            Clone.setSizeRaw(m_size);
            return Clone;
        }

        /**
         * Returns a deep Copy of the receiver; uses <code>Clone()</code> and casts the result.
         *
         * @return  a deep Copy of the receiver.
         */

        public DoubleArrayList Copy()
        {
            return (DoubleArrayList) Clone();
        }

        /**
         * Returns the elements currently stored, including invalid elements between size and capacity, if any.
         *
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the returned array directly via the [] operator, be sure you know what you're doing.
         *
         * @return the elements currently stored.
         */

        public new double[] elements()
        {
            return m_elements;
        }

        /**
         * Sets the receiver's elements to be the specified array (not a Copy of it).
         *
         * The size and capacity of the list is the Length of the array.
         * <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
         * So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
         *
         * @param elements the new elements to be stored.
         * @return the receiver itself.
         */

        public new AbstractDoubleList elements(double[] elements)
        {
            m_elements = elements;
            m_size = elements.Length;
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
            if (!(otherObj is DoubleArrayList))
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
            DoubleArrayList other = (DoubleArrayList) otherObj;
            if (Size() != other.Size())
            {
                return false;
            }

            double[] theElements = elements();
            double[] otherElements = other.elements();
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

        public new bool forEach(DoubleProcedure procedure)
        {
            // overridden for performance only.
            double[] theElements = m_elements;
            int theSize = m_size;

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

        public new double get(int index)
        {
            // overridden for performance only.
            if (index >= m_size || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_size);
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

        public override double getQuick(int index)
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

        public new int indexOfFromTo(double element, int from, int to)
        {
            // overridden for performance only.
            if (m_size == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, m_size);

            double[] theElements = m_elements;
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

        public new int lastIndexOfFromTo(double element, int from, int to)
        {
            // overridden for performance only.
            if (m_size == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, m_size);

            double[] theElements = m_elements;
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

        public new AbstractDoubleList partFromTo(int from, int to)
        {
            if (m_size == 0)
            {
                return new DoubleArrayList(0);
            }

            checkRangeFromTo(from, to, m_size);

            double[] part = new double[to - from + 1];
            Array.Copy(m_elements, from, part, 0, to - from + 1);
            return new DoubleArrayList(part);
        }

        /**
        * Removes from the receiver all elements that are contained in the specified list.
        * Tests for identity.
        *
        * @param other the other list.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public new bool removeAll(AbstractDoubleList other)
        {
            // overridden for performance only.
            if (!(other is DoubleArrayList))
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
            double[] theElements = m_elements;
            int mySize = Size();

            double N = other.Size();
            double M = mySize;
            if ((N + M)*Arithmetic.log2(N) < M*N)
            {
                // it is faster to sort other before searching in it
                DoubleArrayList sortedList = (DoubleArrayList) other.Clone();
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

        public new void replaceFromToWithFrom(int from, int to, AbstractDoubleList other, int otherFrom)
        {
            // overridden for performance only.
            if (!(other is DoubleArrayList))
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
                Array.Copy(((DoubleArrayList) other).m_elements, otherFrom, m_elements, from, Length);
            }
        }

        /**
        * Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        * In other words, removes from the receiver all of its elements that are not contained in the
        * specified other list. 
        * @param other the other list to test against.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public new bool retainAll(AbstractDoubleList other)
        {
            // overridden for performance only.
            if (!(other is DoubleArrayList))
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
            double[] theElements = m_elements;
            int mySize = Size();

            double N = other.Size();
            double M = mySize;
            if ((N + M)*Arithmetic.log2(N) < M*N)
            {
                // it is faster to sort other before searching in it
                DoubleArrayList sortedList = (DoubleArrayList) other.Clone();
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

        public new void reverse()
        {
            // overridden for performance only.
            double tmp;
            int limit = m_size/2;
            int j = m_size - 1;

            double[] theElements = m_elements;
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

        public new void set(int index, double element)
        {
            // overridden for performance only.
            if (index >= m_size || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_size);
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

        public override void setQuick(int index, double element)
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
            if (m_size == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_size);

            RngWrapper gen = new RngWrapper();
            double tmpElement;
            double[] theElements = m_elements;
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
         * Trims the capacity of the receiver to be the receiver's current 
         * size. Releases any superfluos internal memory. An application can use this operation to minimize the 
         * storage of the receiver.
         */

        public new void trimToSize()
        {
            m_elements = Arrays.trimToCapacity(m_elements, Size());
        }
    }
}
