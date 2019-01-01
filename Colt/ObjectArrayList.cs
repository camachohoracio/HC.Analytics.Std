#region

using System;
using System.Collections;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package list;

    ////import ObjectProcedure;
    /**
    Resizable list holding <code>Object</code> elements; implemented with arrays.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    */

    [Serializable]
    public class ObjectArrayList : AbstractList
    {
        /**
         * The array buffer into which the elements of the list are stored.
         * The capacity of the list is the Length of this array buffer.
         * @serial
         */
        public Object[] elements_;

        /**
         * The size of the list.
         * @serial
         */
        public int m_intSize;
        /**
         * Constructs an empty list.
         */

        public ObjectArrayList()
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

        public ObjectArrayList(Object[] elements_)
        {
            elements(elements_);
        }

        /**
         * Constructs an empty list with the specified initial capacity.
         *
         * @param   initialCapacity   the number of elements the receiver can hold without auto-expanding itself by allocating new internal memory.
         */

        public ObjectArrayList(int initialCapacity)
            : this(new Object[initialCapacity])
        {
            m_intSize = 0;
        }

        /**
         * Appends the specified element to the end of this list.
         *
         * @param element element to be appended to this list.
         */

        public void Add(Object element)
        {
            if (m_intSize == elements_.Length)
            {
                ensureCapacity(m_intSize + 1);
            }
            elements_[m_intSize++] = element;
        }

        /**
         * Appends the part of the specified list between <code>from</code> (inclusive) and <code>to</code> (inclusive) to the receiver.
         *
         * @param other the list to be added to the receiver.
         * @param from the index of the first element to be appended (inclusive).
         * @param to the index of the last element to be appended (inclusive).
         * @exception HCException index is out of range (<tt>other.Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=other.Size())</tt>).
         */

        public void addAllOfFromTo(ObjectArrayList other, int from, int to)
        {
            beforeInsertAllOfFromTo(m_intSize, other, from, to);
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

        public void beforeInsert(int index, Object element)
        {
            // overridden for performance only.
            if (index > m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
            }
            ensureCapacity(m_intSize + 1);
            Array.Copy(elements_, index, elements_, index + 1, m_intSize - index);
            elements_[index] = element;
            m_intSize++;
        }

        /**
         * Inserts the part of the specified list between <code>otherFrom</code> (inclusive) and <code>otherTo</code> (inclusive) before the specified position into the receiver. 
         * Shifts the element currently at that position (if any) and
         * any subsequent elements to the right.
         *
         * @param index index before which to Insert first element from the specified list (must be in [0,size])..
         * @param other list of which a part is to be inserted into the receiver.
         * @param from the index of the first element to be inserted (inclusive).
         * @param to the index of the last element to be inserted (inclusive).
         * @exception HCException index is out of range (<tt>other.Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=other.Size())</tt>).
         * @exception HCException index is out of range (<tt>index &lt; 0 || index &gt; Size()</tt>).
         */

        public void beforeInsertAllOfFromTo(int index, ObjectArrayList other, int from, int to)
        {
            int Length = to - from + 1;
            beforeInsertDummies(index, Length);
            replaceFromToWithFrom(index, index + Length - 1, other, from);
        }

        /**
         * Inserts Length dummies before the specified position into the receiver. 
         * Shifts the element currently at that position (if any) and
         * any subsequent elements to the right.
         *
         * @param index index before which to Insert dummies (must be in [0,size])..
         * @param Length number of dummies to be inserted.
         */

        public override void beforeInsertDummies(int index, int Length)
        {
            if (index > m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
            }
            if (Length > 0)
            {
                ensureCapacity(m_intSize + Length);

                Array.Copy(elements_, index, elements_, index + Length, m_intSize - index);
                m_intSize += Length;
            }
        }

        /**
         * Searches the receiver for the specified value using
         * the binary search algorithm. The receiver must be sorted into ascending order
         * according to the <i>natural ordering</i> of its elements (as by the sort method)
         * prior to making this call.  
         * If it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the receiver contains multiple elements
         * equal to the specified object, there is no guarantee which instance
         * will be found.
         *
         * @param key the value to be searched for.
         * @return index of the search key, if it is contained in the receiver;
         *	       otherwise, <tt>(-(<i>insertion point</i>) - 1)</tt>.  The <i>insertion
         *	       point</i> is defined as the the point at which the value would
         * 	       be inserted into the receiver: the index of the first
         *	       element greater than the key, or <tt>receiver.Size()</tt>, if all
         *	       elements in the receiver are less than the specified key.  Note
         *	       that this guarantees that the return value will be &gt;= 0 if
         *	       and only if the key is found.
         * @see IComparable
         * @see Arrays
         */

        public int binarySearch(Object key)
        {
            return binarySearchFromTo(key, 0, m_intSize - 1);
        }

        /**
         * Searches the receiver for the specified value using
         * the binary search algorithm. The receiver must be sorted into ascending order
         * according to the <i>natural ordering</i> of its elements (as by the sort method)
         * prior to making this call.  
         * If it is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the receiver contains multiple elements
         * equal to the specified object, there is no guarantee which instance
         * will be found.
         *
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
         * @see IComparable
         * @see Arrays
         */

        public int binarySearchFromTo(Object key, int from, int to)
        {
            int low = from;
            int high = to;

            while (low <= high)
            {
                int mid = (low + high)/2;
                Object midVal = elements_[mid];
                int cmp = ((IComparable) midVal).CompareTo(key);

                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else if (cmp > 0)
                {
                    high = mid - 1;
                }
                else
                {
                    return mid; // key found
                }
            }
            return -(low + 1); // key not found.
        }

        /**
         * Searches the receiver for the specified value using
         * the binary search algorithm. The receiver must be sorted into ascending order
         * according to the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
         *
         * If the receiver is not sorted, the results are undefined: in particular, the call
         * may enter an infinite loop.  If the receiver contains multiple elements
         * equal to the specified object, there is no guarantee which instance
         * will be found.
         *
         *
         * @param key the value to be searched for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @param comparator the comparator by which the receiver is sorted.
         * @throws ClassCastException if the receiver contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
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
         * @see IComparer
         */

        public int binarySearchFromTo(
            Object key,
            int from,
            int to,
            IComparer comparator)
        {
            return Sorting.binarySearchFromTo(
                elements_,
                key,
                from,
                to,
                comparator);
        }

        /**
         * Returns a copy of the receiver such that the copy and the receiver <i>share</i> the same elements, but do not share the same array to index them;
         * So modifying an object in the copy modifies the object in the receiver and vice versa;
         * However, structurally modifying the copy (for example changing its size, setting other objects at indexes, etc.) does not affect the receiver and vice versa.
         *
         * @return  a copy of the receiver.
         */

        public new Object Clone()
        {
            ObjectArrayList v = (ObjectArrayList) base.Clone();
            v.elements_ = (Object[]) elements_.Clone();
            return v;
        }

        /**
         * Returns true if the receiver contains the specified element.
         * Tests for equality or identity as specified by testForEquality.
         *
         * @param element element to search for.
         * @param testForEquality if true -> test for equality, otherwise for identity.
         */

        public bool contains(Object elem, bool testForEquality)
        {
            return indexOfFromTo(elem, 0, m_intSize - 1, testForEquality) >= 0;
        }

        /**
         * Returns a copy of the receiver; call <code>Clone()</code> and casts the result.
         * Returns a copy such that the copy and the receiver <i>share</i> the same elements, but do not share the same array to index them;
         * So modifying an object in the copy modifies the object in the receiver and vice versa;
         * However, structurally modifying the copy (for example changing its size, setting other objects at indexes, etc.) does not affect the receiver and vice versa.
         *
         * @return  a copy of the receiver.
         */

        public ObjectArrayList Copy()
        {
            return (ObjectArrayList) Clone();
        }

        /**
         * Deletes the first element from the receiver that matches the specified element.
         * Does nothing, if no such matching element is contained.
         *
         * Tests elements for equality or identity as specified by <tt>testForEquality</tt>.
         * When testing for equality, two elements <tt>e1</tt> and
         * <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
         * e1.0equals(e2))</tt>.)  
         *
         * @param testForEquality if true -> tests for equality, otherwise for identity.
         * @param element the element to be deleted.
         */

        public void delete(Object element, bool testForEquality)
        {
            int index = indexOfFromTo(element, 0, m_intSize - 1, testForEquality);
            if (index >= 0)
            {
                removeFromTo(index, index);
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

        public Object[] elements()
        {
            return elements_;
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

        public ObjectArrayList elements(Object[] elements)
        {
            elements_ = elements;
            m_intSize = elements.Length;
            return this;
        }

        /**
         * Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
         * If necessary, allocates new internal memory and increases the capacity of the receiver.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public void ensureCapacity(int minCapacity)
        {
            elements_ = Arrays.ensureCapacity(elements_, minCapacity);
        }

        /**
        * Compares the specified Object with the receiver for equality.
        * Returns true if and only if the specified Object is also an ObjectArrayList, both lists have the
        * same size, and all corresponding pairs of elements in the two lists are equal.
        * In other words, two lists are defined to be equal if they contain the
        * same elements in the same order.
        * Two elements <tt>e1</tt> and
        * <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        * e1.0equals(e2))</tt>.)  
        *
        * @param otherObj the Object to be compared for equality with the receiver.
        * @return true if the specified Object is equal to the receiver.
        */

        public new bool Equals(Object otherObj)
        {
            //delta
            return Equals(otherObj, true);
        }

        /**
        * Compares the specified Object with the receiver for equality.
        * Returns true if and only if the specified Object is also an ObjectArrayList, both lists have the
        * same size, and all corresponding pairs of elements in the two lists are the same.
        * In other words, two lists are defined to be equal if they contain the
        * same elements in the same order.
        * Tests elements for equality or identity as specified by <tt>testForEquality</tt>.
        * When testing for equality, two elements <tt>e1</tt> and
        * <tt>e2</tt> are <i>equal</i> if <tt>(e1==null ? e2==null :
        * e1.0equals(e2))</tt>.)  
        *
        * @param otherObj the Object to be compared for equality with the receiver.
        * @param testForEquality if true -> tests for equality, otherwise for identity.
        * @return true if the specified Object is equal to the receiver.
        */

        public bool Equals(Object otherObj, bool testForEquality)
        {
            //delta
            if (!(otherObj is ObjectArrayList))
            {
                return false;
            }
            if (this == otherObj)
            {
                return true;
            }
            if (otherObj == null)
            {
                return false;
            }
            ObjectArrayList other = (ObjectArrayList) otherObj;
            if (elements_ == other.elements())
            {
                return true;
            }
            if (m_intSize != other.Size())
            {
                return false;
            }

            Object[] otherElements = other.elements();
            Object[] theElements = elements_;
            if (!testForEquality)
            {
                for (int i = m_intSize; --i >= 0;)
                {
                    if (theElements[i] != otherElements[i])
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = m_intSize; --i >= 0;)
                {
                    if (!(theElements[i] == null ? otherElements[i] == null : theElements[i].Equals(otherElements[i])))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /**
         * Sets the specified range of elements in the specified array to the specified value.
         *
         * @param from the index of the first element (inclusive) to be filled with the specified value.
         * @param to the index of the last element (inclusive) to be filled with the specified value.
         * @param val the value to be stored in the specified elements of the receiver.
         */

        public void fillFromToWith(int from, int to, Object val)
        {
            checkRangeFromTo(from, to, m_intSize);
            for (int i = from; i <= to;)
            {
                setQuick(i++, val);
            }
        }

        /**
         * Applies a procedure to each element of the receiver, if any.
         * Starts at index 0, moving rightwards.
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. 
         */

        public bool forEach(ObjectProcedure procedure)
        {
            Object[] theElements = elements_;
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
         * @exception HCException index is out of range (index &lt; 0 || index &gt;= Size()).
         */

        public Object get(int index)
        {
            if (index >= m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
            }
            return elements_[index];
        }

        /**
         * Returns the element at the specified position in the receiver; <b>WARNING:</b> Does not check preconditions. 
         * Provided with invalid parameters this method may return invalid elements without throwing any exception!
         * <b>You should only use this method when you are absolutely sure that the index is within bounds.</b>
         * Precondition (unchecked): <tt>index &gt;= 0 && index &lt; Size()</tt>.
         *
         * @param index index of element to return.
         */

        public Object getQuick(int index)
        {
            return elements_[index];
        }

        /**
         * Returns the index of the first occurrence of the specified
         * element. Returns <code>-1</code> if the receiver does not contain this element.
         *
         * Tests for equality or identity as specified by testForEquality.
         *
         * @param testForEquality if <code>true</code> -> test for equality, otherwise for identity.
         * @return  the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.
         */

        public int IndexOf(Object element, bool testForEquality)
        {
            return indexOfFromTo(element, 0, m_intSize - 1, testForEquality);
        }

        /**
         * Returns the index of the first occurrence of the specified
         * element. Returns <code>-1</code> if the receiver does not contain this element.
         * Searches between <code>from</code>, inclusive and <code>to</code>, inclusive.
         *
         * Tests for equality or identity as specified by <code>testForEquality</code>.
         *
         * @param element element to search for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @param testForEquality if </code>true</code> -> test for equality, otherwise for identity.
         * @return  the index of the first occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public int indexOfFromTo(Object element, int from, int to, bool testForEquality)
        {
            if (m_intSize == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, m_intSize);

            Object[] theElements = elements_;
            if (testForEquality && element != null)
            {
                for (int i = from; i <= to; i++)
                {
                    if (element.Equals(theElements[i]))
                    {
                        return i;
                    } //found
                }
            }
            else
            {
                for (int i = from; i <= to; i++)
                {
                    if (element == theElements[i])
                    {
                        return i;
                    } //found
                }
            }
            return -1; //not found
        }

        /**
         * Determines whether the receiver is sorted ascending, according to the <i>natural ordering</i> of its
         * elements.  All elements in this range must implement the
         * <tt>IComparable</tt> interface.  Furthermore, all elements in this range
         * must be <i>mutually comparable</i> (that is, <tt>e1.CompareTo(e2)</tt>
         * must not throw a <tt>ClassCastException</tt> for any elements
         * <tt>e1</tt> and <tt>e2</tt> in the array).<p>
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @return <tt>true</tt> if the receiver is sorted ascending, <tt>false</tt> otherwise.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public bool isSortedFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return true;
            }
            checkRangeFromTo(from, to, m_intSize);

            Object[] theElements = elements_;
            for (int i = from + 1; i <= to; i++)
            {
                if (((IComparable) theElements[i]).CompareTo(theElements[i - 1]) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Returns the index of the last occurrence of the specified
         * element. Returns <code>-1</code> if the receiver does not contain this element.
         * Tests for equality or identity as specified by <code>testForEquality</code>.
         *
         * @param   element   the element to be searched for.
         * @param testForEquality if <code>true</code> -> test for equality, otherwise for identity.
         * @return  the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.
         */

        public int LastIndexOf(Object element, bool testForEquality)
        {
            return lastIndexOfFromTo(element, 0, m_intSize - 1, testForEquality);
        }

        /**
         * Returns the index of the last occurrence of the specified
         * element. Returns <code>-1</code> if the receiver does not contain this element.
         * Searches beginning at <code>to</code>, inclusive until <code>from</code>, inclusive.
         * Tests for equality or identity as specified by <code>testForEquality</code>.
         *
         * @param element element to search for.
         * @param from the leftmost search position, inclusive.
         * @param to the rightmost search position, inclusive.
         * @param testForEquality if <code>true</code> -> test for equality, otherwise for identity.
         * @return  the index of the last occurrence of the element in the receiver; returns <code>-1</code> if the element is not found.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public int lastIndexOfFromTo(Object element, int from, int to, bool testForEquality)
        {
            if (m_intSize == 0)
            {
                return -1;
            }
            checkRangeFromTo(from, to, m_intSize);

            Object[] theElements = elements_;
            if (testForEquality && element != null)
            {
                for (int i = to; i >= from; i--)
                {
                    if (element.Equals(theElements[i]))
                    {
                        return i;
                    } //found
                }
            }
            else
            {
                for (int i = to; i >= from; i--)
                {
                    if (element == theElements[i])
                    {
                        return i;
                    } //found
                }
            }
            return -1; //not found
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
         * This sort is guaranteed to be <i>stable</i>:  equal elements will
         * not be reordered as a result of the sort.<p>
         *
         * The sorting algorithm is a modified mergesort (in which the merge is
         * omitted if the highest element in the low sublist is less than the
         * lowest element in the high sublist).  This algorithm offers guaranteed
         * n*log(n) performance, and can approach linear performance on nearly
         * sorted lists.
         *
         * <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
         * It is generally better to call <tt>Sort()</tt> or <tt>sortFromTo(...)</tt> instead, because those methods automatically choose the best sorting algorithm.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void mergeSortFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);
            Array.Sort(elements_, from, to + 1);
        }

        /**
         * Sorts the receiver according
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
         * @param from the index of the first element (inclusive) to be
         *        sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @param c the comparator to determine the order of the receiver.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws ArrayIndexOutOfBoundsException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         * @see IComparer
         */

        public void mergeSortFromTo(
            int from,
            int to,
            IComparer c)
        {
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);
            Array.Sort(elements_, from, to + 1, c);
        }

        /**
         * Returns a new list of the part of the receiver between <code>from</code>, inclusive, and <code>to</code>, inclusive.
         * @param from the index of the first element (inclusive).
         * @param to the index of the last element (inclusive).
         * @return a new list
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
        */

        public ObjectArrayList partFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return new ObjectArrayList(0);
            }

            checkRangeFromTo(from, to, m_intSize);

            Object[] part = new Object[to - from + 1];
            Array.Copy(elements_, from, part, 0, to - from + 1);
            return new ObjectArrayList(part);
        }

        /**
         * Sorts the specified range of the receiver into
         * ascending order, according to the <i>natural ordering</i> of its
         * elements.  All elements in this range must implement the
         * <tt>IComparable</tt> interface.  Furthermore, all elements in this range
         * must be <i>mutually comparable</i> (that is, <tt>e1.CompareTo(e2)</tt>
         * must not throw a <tt>ClassCastException</tt> for any elements
         * <tt>e1</tt> and <tt>e2</tt> in the array).<p>
 
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * <p><b>You should never call this method unless you are sure that this particular sorting algorithm is the right one for your data set.</b>
         * It is generally better to call <tt>Sort()</tt> or <tt>sortFromTo(...)</tt> instead, because those methods automatically choose the best sorting algorithm.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void quickSortFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);
            Sorting.quickSort(elements_, from, to + 1);
        }

        /**
         * Sorts the receiver according
         * to the order induced by the specified comparator.  All elements in the
         * range must be <i>mutually comparable</i> by the specified comparator
         * (that is, <tt>c.Compare(e1, e2)</tt> must not throw a
         * <tt>ClassCastException</tt> for any elements <tt>e1</tt> and
         * <tt>e2</tt> in the range).<p>
 
         * The sorting algorithm is a tuned quicksort,
         * adapted from Jon L. Bentley and M. Douglas McIlroy's "Engineering a
         * Sort Function", Software-Practice and Experience, Vol. 23(11)
         * P. 1249-1265 (November 1993).  This algorithm offers n*log(n)
         * performance on many data sets that cause other quicksorts to degrade to
         * quadratic performance.
         *
         * @param from the index of the first element (inclusive) to be sorted.
         * @param to the index of the last element (inclusive) to be sorted.
         * @param c the comparator to determine the order of the receiver.
         * @throws ClassCastException if the array contains elements that are not
         *	       <i>mutually comparable</i> using the specified comparator.
         * @throws ArgumentException if <tt>fromIndex &gt; toIndex</tt>
         * @throws ArrayIndexOutOfBoundsException if <tt>fromIndex &lt; 0</tt> or
         *	       <tt>toIndex &gt; a.Length</tt>
         * @see IComparer
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public void quickSortFromTo(
            int from,
            int to,
            IComparer c)
        {
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);
            Sorting.quickSort(
                elements_,
                from,
                to + 1,
                c);
        }

        /**
        * Removes from the receiver all elements that are contained in the specified list.
        * Tests for equality or identity as specified by <code>testForEquality</code>.
        *
        * @param other the other list.
        * @param testForEquality if <code>true</code> -> test for equality, otherwise for identity.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public bool removeAll(ObjectArrayList other, bool testForEquality)
        {
            if (other.m_intSize == 0)
            {
                return false; //nothing to do
            }
            int limit = other.m_intSize - 1;
            int j = 0;
            Object[] theElements = elements_;
            for (int i = 0; i < m_intSize; i++)
            {
                if (other.indexOfFromTo(theElements[i], 0, limit, testForEquality) < 0)
                {
                    theElements[j++] = theElements[i];
                }
            }

            bool modified = (j != m_intSize);
            setSize(j);
            return modified;
        }

        /**
         * Removes from the receiver all elements whose index is between
         * <code>from</code>, inclusive and <code>to</code>, inclusive.  Shifts any succeeding
         * elements to the left (reduces their index).
         * This call shortens the list by <tt>(to - from + 1)</tt> elements.
         *
         * @param from index of first element to be removed.
         * @param to index of last element to be removed.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void removeFromTo(int from, int to)
        {
            checkRangeFromTo(from, to, m_intSize);
            int numMoved = m_intSize - to - 1;
            if (numMoved >= 0)
            {
                Array.Copy(elements_, to + 1, elements_, from, numMoved);
                fillFromToWith(from + numMoved, m_intSize - 1, null); //delta
            }
            int width = to - from + 1;
            if (width > 0)
            {
                m_intSize -= width;
            }
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

        public void replaceFromToWithFrom(int from, int to, ObjectArrayList other, int otherFrom)
        {
            int Length = to - from + 1;
            if (Length > 0)
            {
                checkRangeFromTo(from, to, m_intSize);
                checkRangeFromTo(otherFrom, otherFrom + Length - 1, other.m_intSize);
                Array.Copy(other.elements_, otherFrom, elements_, from, Length);
            }
        }

        /**
        * Replaces the part between <code>from</code> (inclusive) and <code>to</code> (inclusive) with the other list's
        * part between <code>otherFrom</code> and <code>otherTo</code>. 
        * Powerful (and tricky) method!
        * Both parts need not be of the same size (part A can both be smaller or larger than part B).
        * Parts may overlap.
        * Receiver and other list may (but most not) be identical.
        * If <code>from &gt; to</code>, then inserts other part before <code>from</code>.
        *
        * @param from the first element of the receiver (inclusive)
        * @param to the last element of the receiver (inclusive)
        * @param other the other list (may be identical with receiver)
        * @param otherFrom the first element of the other list (inclusive)
        * @param otherTo the last element of the other list (inclusive)
        *
        * <p><b>Examples:</b><pre>
        * a=[0, 1, 2, 3, 4, 5, 6, 7]
        * b=[50, 60, 70, 80, 90]
        * a.R(...)=a.replaceFromToWithFromTo(...)
        *
        * a.R(3,5,b,0,4)-->[0, 1, 2, 50, 60, 70, 80, 90, 6, 7]
        * a.R(1,6,b,0,4)-->[0, 50, 60, 70, 80, 90, 7]
        * a.R(0,6,b,0,4)-->[50, 60, 70, 80, 90, 7]
        * a.R(3,5,b,1,2)-->[0, 1, 2, 60, 70, 6, 7]
        * a.R(1,6,b,1,2)-->[0, 60, 70, 7]
        * a.R(0,6,b,1,2)-->[60, 70, 7]
        * a.R(5,3,b,0,4)-->[0, 1, 2, 3, 4, 50, 60, 70, 80, 90, 5, 6, 7]
        * a.R(5,0,b,0,4)-->[0, 1, 2, 3, 4, 50, 60, 70, 80, 90, 5, 6, 7]
        * a.R(5,3,b,1,2)-->[0, 1, 2, 3, 4, 60, 70, 5, 6, 7]
        * a.R(5,0,b,1,2)-->[0, 1, 2, 3, 4, 60, 70, 5, 6, 7]
        *
        * Extreme cases:
        * a.R(5,3,b,0,0)-->[0, 1, 2, 3, 4, 50, 5, 6, 7]
        * a.R(5,3,b,4,4)-->[0, 1, 2, 3, 4, 90, 5, 6, 7]
        * a.R(3,5,a,0,1)-->[0, 1, 2, 0, 1, 6, 7]
        * a.R(3,5,a,3,5)-->[0, 1, 2, 3, 4, 5, 6, 7]
        * a.R(3,5,a,4,4)-->[0, 1, 2, 4, 6, 7]
        * a.R(5,3,a,0,4)-->[0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 5, 6, 7]
        * a.R(0,-1,b,0,4)-->[50, 60, 70, 80, 90, 0, 1, 2, 3, 4, 5, 6, 7]
        * a.R(0,-1,a,0,4)-->[0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 5, 6, 7]
        * a.R(8,0,a,0,4)-->[0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4]
        * </pre>
        */

        public void replaceFromToWithFromTo(int from, int to, ObjectArrayList other, int otherFrom, int otherTo)
        {
            if (otherFrom > otherTo)
            {
                throw new HCException("otherFrom: " + otherFrom + ", otherTo: " + otherTo);
            }
            if (this == other && to - from != otherTo - otherFrom)
            {
                // avoid stumbling over my own feet
                replaceFromToWithFromTo(from, to, partFromTo(otherFrom, otherTo), 0, otherTo - otherFrom);
                return;
            }

            int Length = otherTo - otherFrom + 1;
            int diff = Length;
            int theLast = from - 1;

            //PrintToScreen.WriteLine("from="+from);
            //PrintToScreen.WriteLine("to="+to);
            //PrintToScreen.WriteLine("diff="+diff);

            if (to >= from)
            {
                diff -= (to - from + 1);
                theLast = to;
            }

            if (diff > 0)
            {
                beforeInsertDummies(theLast + 1, diff);
            }
            else
            {
                if (diff < 0)
                {
                    removeFromTo(theLast + diff, theLast - 1);
                }
            }

            if (Length > 0)
            {
                Array.Copy(other.elements_, otherFrom, elements_, from, Length);
            }
        }

        /**
         * Replaces the part of the receiver starting at <code>from</code> (inclusive) with all the elements of the specified collection.
         * Does not alter the size of the receiver.
         * Replaces exactly <tt>Math.Max(0,Math.Min(Size()-from, other.Size()))</tt> elements.
         *
         * @param from the index at which to copy the first element from the specified collection.
         * @param other ICollection to replace part of the receiver
         * @exception HCException index is out of range (index &lt; 0 || index &gt;= Size()).
         */

        public override void replaceFromWith(
            int from,
            CollectionBase other)
        {
            checkRange(from, m_intSize);
            IEnumerator e = other.GetEnumerator();
            int index = from;
            int limit = Math.Min(m_intSize - from, other.Count);
            for (int i = 0; i < limit; i++)
            {
                e.MoveNext();
                elements_[index++] = e.Current; //delta
            }
        }

        /**
        * Retains (keeps) only the elements in the receiver that are contained in the specified other list.
        * In other words, removes from the receiver all of its elements that are not contained in the
        * specified other list. 
        * Tests for equality or identity as specified by <code>testForEquality</code>.
        * @param other the other list to test against.
        * @param testForEquality if <code>true</code> -> test for equality, otherwise for identity.
        * @return <code>true</code> if the receiver changed as a result of the call.
        */

        public bool retainAll(ObjectArrayList other, bool testForEquality)
        {
            if (other.m_intSize == 0)
            {
                if (m_intSize == 0)
                {
                    return false;
                }
                setSize(0);
                return true;
            }

            int limit = other.m_intSize - 1;
            int j = 0;
            Object[] theElements = elements_;

            for (int i = 0; i < m_intSize; i++)
            {
                if (other.indexOfFromTo(theElements[i], 0, limit, testForEquality) >= 0)
                {
                    theElements[j++] = theElements[i];
                }
            }

            bool modified = (j != m_intSize);
            setSize(j);
            return modified;
        }

        /**
         * Reverses the elements of the receiver.
         * Last becomes first, second last becomes second first, and so on.
         */

        public override void reverse()
        {
            Object tmp;
            int limit = m_intSize/2;
            int j = m_intSize - 1;

            Object[] theElements = elements_;
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

        public void set(int index, Object element)
        {
            if (index >= m_intSize || index < 0)
            {
                throw new HCException("Index: " + index + ", Size: " + m_intSize);
            }
            elements_[index] = element;
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

        public void setQuick(int index, Object element)
        {
            elements_[index] = element;
        }

        /**
         * Randomly permutes the part of the receiver between <code>from</code> (inclusive) and <code>to</code> (inclusive). 
         * @param from the index of the first element (inclusive) to be permuted.
         * @param to the index of the last element (inclusive) to be permuted.
         * @exception HCException index is out of range (<tt>Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=Size())</tt>).
         */

        public override void shuffleFromTo(int from, int to)
        {
            if (m_intSize == 0)
            {
                return;
            }
            checkRangeFromTo(from, to, m_intSize);

            RngWrapper gen = new RngWrapper();
            Object tmpElement;
            Object[] theElements = elements_;
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
         * Returns the number of elements contained in the receiver.
         *
         * @returns  the number of elements contained in the receiver.
         */

        public override int Size()
        {
            return m_intSize;
        }

        /**
         * Returns a list which is a concatenation of <code>times</code> times the receiver.
         * @param times the number of times the receiver shall be copied.
         */

        public ObjectArrayList times(int times)
        {
            ObjectArrayList newList = new ObjectArrayList(times*m_intSize);
            for (int i = times; --i >= 0;)
            {
                newList.addAllOfFromTo(this, 0, Size() - 1);
            }
            return newList;
        }

        /**
         * Returns an array containing all of the elements in the receiver in the
         * correct order.  The runtime type of the returned array is that of the
         * specified array.  If the receiver fits in the specified array, it is
         * returned therein.  Otherwise, a new array is allocated with the runtime
         * type of the specified array and the size of the receiver.
         * <p>
         * If the receiver fits in the specified array with room to spare
         * (i.e., the array has more elements than the receiver),
         * the element in the array immediately following the end of the
         * receiver is set to null.  This is useful in determining the Length
         * of the receiver <em>only</em> if the caller knows that the receiver
         * does not contain any null elements.
         *
         * @param array the array into which the elements of the receiver are to
         *		be stored, if it is big enough; otherwise, a new array of the
         * 		same runtime type is allocated for this purpose.
         * @return an array containing the elements of the receiver.
         * @exception ArrayStoreException the runtime type of <tt>array</tt> is not a supertype
         * of the runtime type of every element in the receiver.
         */

        public Object[] ToArray(Object[] array)
        {
            if (array.Length < m_intSize)
            {
                array = (Object[]) Array.CreateInstance(array.GetType(), m_intSize);
            }

            Object[] theElements = elements_;
            for (int i = m_intSize; --i >= 0;)
            {
                array[i] = theElements[i];
            }

            if (array.Length > m_intSize)
            {
                array[m_intSize] = null;
            }

            return array;
        }

        /**
         * Returns a <code>ArrayList</code> containing all the elements in the receiver.
         */

        public override ArrayList toList()
        {
            int mySize = Size();
            Object[] theElements = elements_;
            ArrayList list = new ArrayList(mySize);
            for (int i = 0; i < mySize; i++)
            {
                list.Add(theElements[i]);
            }
            return list;
        }

        /**
        * Returns a string representation of the receiver, containing
        * the string representation of each element.
        */

        public override string ToString()
        {
            return Arrays.ToString(partFromTo(0, Size() - 1).elements());
        }

        /**
         * Trims the capacity of the receiver to be the receiver's current 
         * size. Releases any superfluos internal memory. An application can use this operation to minimize the 
         * storage of the receiver.
         */

        public new void trimToSize()
        {
            elements_ = Arrays.trimToCapacity(elements_, Size());
        }
    }
}
