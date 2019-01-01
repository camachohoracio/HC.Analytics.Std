#region

using System;
using System.Text;
using HC.Analytics.Colt.CustomImplementations;

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
    //package map;

    ////import LongObjectProcedure;
    ////import LongProcedure;
    ////import LongArrayList;
    ////import ObjectArrayList;
    /**
    Abstract base class for hash maps holding (key,value) associations of type <tt>(long-->Object)</tt>.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation</b>:
    <p>
    Almost all methods are expressed in terms of {@link #forEachKey(LongProcedure)}. 
    As such they are fully functional, but inefficient. Override them in subclasses if necessary.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    @see	    HashMap
    */

    [Serializable]
    public abstract class AbstractLongObjectMap : AbstractMap
    {
        //public static int hashCollisions = 0; // for debug only
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Returns <tt>true</tt> if the receiver contains the specified key.
         *
         * @return <tt>true</tt> if the receiver contains the specified key.
         */

        public bool containsKey(long key)
        {
            return !forEachKey(
                        new LongProcedure_(key));
        }

        /**
         * Returns <tt>true</tt> if the receiver contains the specified value.
         * Tests for identity.
         *
         * @return <tt>true</tt> if the receiver contains the specified value.
         */

        public bool containsValue(Object value)
        {
            return !forEachPair(
                        new LongObjectProcedure_(value));
        }

        /**
         * Returns a deep copy of the receiver; uses <code>Clone()</code> and casts the result.
         *
         * @return  a deep copy of the receiver.
         */

        public AbstractLongObjectMap Copy()
        {
            return (AbstractLongObjectMap) Clone();
        }

        /**
         * Compares the specified object with this map for equality.  Returns
         * <tt>true</tt> if the given object is also a map and the two maps
         * represent the same mappings.  More formally, two maps <tt>m1</tt> and
         * <tt>m2</tt> represent the same mappings iff
         * <pre>
         * m1.forEachPair(
         *		new LongObjectProcedure() {
         *			public bool Apply(long key, Object value) {
         *				return m2.containsKey(key) && m2.get(key) == value;
         *			}
         *		}
         *	)
         * &&
         * m2.forEachPair(
         *		new LongObjectProcedure() {
         *			public bool Apply(long key, Object value) {
         *				return m1.containsKey(key) && m1.get(key) == value;
         *			}
         *		}
         *	);
         * </pre>
         *
         * This implementation first checks if the specified object is this map;
         * if so it returns <tt>true</tt>.  Then, it checks if the specified
         * object is a map whose size is identical to the size of this set; if
         * not, it it returns <tt>false</tt>.  If so, it applies the iteration as described above.
         *
         * @param obj object to be compared for equality with this map.
         * @return <tt>true</tt> if the specified object is equal to this map.
         */

        public override bool Equals(Object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is AbstractLongObjectMap))
            {
                return false;
            }
            AbstractLongObjectMap other = (AbstractLongObjectMap) obj;
            if (other.Size() != Size())
            {
                return false;
            }

            return
                forEachPair(
                    new LongObjectProcedure2_(other)
                    )
                &&
                other.forEachPair(
                    new LongObjectProcedure2_(this)
                    );
        }

        /**
         * Applies a procedure to each key of the receiver, if any.
         * Note: Iterates over the keys in no particular order.
         * Subclasses can define a particular order, for example, "sorted by key".
         * All methods which <i>can</i> be expressed in terms of this method (most methods can) <i>must guarantee</i> to use the <i>same</i> order defined by this method, even if it is no particular order.
         * This is necessary so that, for example, methods <tt>keys</tt> and <tt>values</tt> will yield association pairs, not two uncorrelated lists.
         *
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all keys where iterated over, <tt>true</tt> otherwise. 
         */
        public abstract bool forEachKey(LongProcedure procedure);
        /**
         * Applies a procedure to each (key,value) pair of the receiver, if any.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         *
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all keys where iterated over, <tt>true</tt> otherwise. 
         */

        public bool forEachPair(LongObjectProcedure procedure)
        {
            return forEachKey(
                new LongProcedure2_(procedure, this)
                );
        }

        /**
         * Returns the value associated with the specified key.
         * It is often a good idea to first check with {@link #containsKey(long)} whether the given key has a value associated or not, i.e. whether there exists an association for the given key or not.
         *
         * @param key the key to be searched for.
         * @return the value associated with the specified key; <tt>null</tt> if no such key is present.
         */
        public abstract Object get(long key);
        /**
         * Returns the first key the given value is associated with.
         * It is often a good idea to first check with {@link #containsValue(Object)} whether there exists an association from a key to this value.
         * Search order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         *
         * @param value the value to search for.
         * @return the first key for which holds <tt>get(key) == value</tt>; 
         *		   returns <tt>long.MinValue</tt> if no such key exists.
         */

        public long keyOf(Object value)
        {
            long[] foundKey = new long[1];
            bool notFound = forEachPair(
                new LongObjectProcedure3_(foundKey, value)
                );
            if (notFound)
            {
                return long.MinValue;
            }
            return foundKey[0];
        }

        /**
         * Returns a list filled with all keys contained in the receiver.
         * The returned list has a size that equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         * <p>
         * This method can be used to iterate over the keys of the receiver.
         *
         * @return the keys.
         */

        public LongArrayList keys()
        {
            LongArrayList list = new LongArrayList(Size());
            keys(list);
            return list;
        }

        /**
         * Fills all keys contained in the receiver into the specified list.
         * Fills the list, starting at index 0.
         * After this call returns the specified list has a new size that equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         * <p>
         * This method can be used to iterate over the keys of the receiver.
         *
         * @param list the list to be filled, can have any size.
         */

        public void keys(LongArrayList list)
        {
            list.Clear();
            forEachKey(
                new LongProcedure3_(list)
                );
        }

        /**
         * Fills all keys <i>sorted ascending by their associated value</i> into the specified list.
         * Fills into the list, starting at index 0.
         * After this call returns the specified list has a new size that equals <tt>Size()</tt>.
         * Primary sort criterium is "value", secondary sort criterium is "key". 
         * This means that if any two values are equal, the smaller key comes first.
         * <p>
         * <b>Example:</b>
         * <br>
         * <tt>keys = (8,7,6), values = (1,2,2) --> keyList = (8,6,7)</tt>
         *
         * @param keyList the list to be filled, can have any size.
         */

        public void keysSortedByValue(LongArrayList keyList)
        {
            pairsSortedByValue(keyList, new ObjectArrayList(Size()));
        }

        /**
        Fills all pairs satisfying a given condition into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists both have a new size, the number of pairs satisfying the condition.
        Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        LongObjectProcedure condition = new LongObjectProcedure() { // match even keys only
            public bool Apply(long key, Object value) { return key%2==0; }
        }
        keys = (8,7,6), values = (1,2,2) --> keyList = (6,8), valueList = (2,1)</tt>
        </pre>

        @param condition    the condition to be matched. Takes the current key as first and the current value as second argument.
        @param keyList the list to be filled with keys, can have any size.
        @param valueList the list to be filled with values, can have any size.
        */

        public void pairsMatching(LongObjectProcedure condition, LongArrayList keyList, ObjectArrayList valueList)
        {
            keyList.Clear();
            valueList.Clear();

            forEachPair(
                new LongObjectProcedure4_(
                    condition,
                    keyList,
                    valueList)
                );
        }

        /**
         * Fills all keys and values <i>sorted ascending by key</i> into the specified lists.
         * Fills into the lists, starting at index 0.
         * After this call returns the specified lists both have a new size that equals <tt>Size()</tt>.
         * <p>
         * <b>Example:</b>
         * <br>
         * <tt>keys = (8,7,6), values = (1,2,2) --> keyList = (6,7,8), valueList = (2,2,1)</tt>
         *
         * @param keyList the list to be filled with keys, can have any size.
         * @param valueList the list to be filled with values, can have any size.
         */

        public void pairsSortedByKey(LongArrayList keyList, ObjectArrayList valueList)
        {
            keys(keyList);
            keyList.Sort();
            valueList.setSize(keyList.Size());
            for (int i = keyList.Size(); --i >= 0;)
            {
                valueList.setQuick(i, get(keyList.getQuick(i)));
            }
        }

        /**
         * Fills all keys and values <i>sorted ascending by value according to natural ordering</i> into the specified lists.
         * Fills into the lists, starting at index 0.
         * After this call returns the specified lists both have a new size that equals <tt>Size()</tt>.
         * Primary sort criterium is "value", secondary sort criterium is "key". 
         * This means that if any two values are equal, the smaller key comes first.
         * <p>
         * <b>Example:</b>
         * <br>
         * <tt>keys = (8,7,6), values = (1,2,2) --> keyList = (8,6,7), valueList = (1,2,2)</tt>
         *
         * @param keyList the list to be filled with keys, can have any size.
         * @param valueList the list to be filled with values, can have any size.
         */

        public void pairsSortedByValue(LongArrayList keyList, ObjectArrayList valueList)
        {
            keys(keyList);
            values(valueList);

            long[] k = keyList.elements();
            Object[] v = valueList.elements();
            Swapper swapper = new Swapper9_(v, k);

            IntComparator comp = new IntComparator11_(k, v);

            GenericSorting.quickSort(0, keyList.Size(), comp, swapper);
        }

        /**
         * Associates the given key with the given value.
         * Replaces any old <tt>(key,someOtherValue)</tt> association, if existing.
         *
         * @param key the key the value shall be associated with.
         * @param value the value to be associated.
         * @return <tt>true</tt> if the receiver did not already contain such a key;
         *         <tt>false</tt> if the receiver did already contain such a key - the new value has now replaced the formerly associated value.
         */
        public abstract bool put(long key, Object value);
        /**
         * Removes the given key with its associated element from the receiver, if present.
         *
         * @param key the key to be removed from the receiver.
         * @return <tt>true</tt> if the receiver contained the specified key, <tt>false</tt> otherwise.
         */
        public abstract bool removeKey(long key);
        /**
         * Returns a string representation of the receiver, containing
         * the string representation of each key-value pair, sorted ascending by key.
         */

        public override string ToString()
        {
            LongArrayList theKeys = keys();
            theKeys.Sort();

            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = theKeys.Size() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                long key = theKeys.get(i);
                buf.Append((key));
                buf.Append("->");
                buf.Append((get(key)));
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the receiver, containing
         * the string representation of each key-value pair, sorted ascending by value, according to natural ordering.
         */

        public string toStringByValue()
        {
            LongArrayList theKeys = new LongArrayList();
            keysSortedByValue(theKeys);

            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = theKeys.Size() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                long key = theKeys.get(i);
                buf.Append((key));
                buf.Append("->");
                buf.Append((get(key)));
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a list filled with all values contained in the receiver.
         * The returned list has a size that equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         * <p>
         * This method can be used to iterate over the values of the receiver.
         *
         * @return the values.
         */

        public ObjectArrayList values()
        {
            ObjectArrayList list = new ObjectArrayList(Size());
            values(list);
            return list;
        }

        /**
         * Fills all values contained in the receiver into the specified list.
         * Fills the list, starting at index 0.
         * After this call returns the specified list has a new size that equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         * <p>
         * This method can be used to iterate over the values of the receiver.
         *
         * @param list the list to be filled, can have any size.
         */

        public void values(ObjectArrayList list)
        {
            list.Clear();
            forEachKey(
                new LongProcedure4_(list, this));
        }
    }
}
