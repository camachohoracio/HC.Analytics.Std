#region

using System;

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
    ////import ByteArrayList;
    ////import LongArrayList;
    ////import ObjectArrayList;
    /**
    Hash map holding (key,value) associations of type <tt>(long-->Object)</tt>; Automatically grows and shrinks as needed; Implemented using open addressing with double hashing.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.

    Overrides many methods for performance reasons only.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    @see	    HashMap
    */

    [Serializable]
    public class OpenLongObjectHashMap : AbstractLongObjectMap
    {
        /**
        * The hash table keys.
        * @serial
        */


        public static byte FREE;
        public static byte FULL = 1;
        public static byte REMOVED = 2;
        public int freeEntries;
        public byte[] m_state;
        public Object[] m_values;
        public long[] table;

        /**
         * Constructs an empty map with default capacity and default load factors.
         */

        public OpenLongObjectHashMap()
            : this(m_defaultCapacity)
        {
        }

        /**
         * Constructs an empty map with the specified initial capacity and default load factors.
         *
         * @param      initialCapacity   the initial capacity of the map.
         * @throws     ArgumentException if the initial capacity is less
         *             than zero.
         */

        public OpenLongObjectHashMap(int initialCapacity) :
            this(initialCapacity, m_defaultMinLoadFactor, m_defaultMaxLoadFactor)
        {
        }

        /**
         * Constructs an empty map with
         * the specified initial capacity and the specified minimum and maximum load factor.
         *
         * @param      initialCapacity   the initial capacity.
         * @param      minLoadFactor        the minimum load factor.
         * @param      maxLoadFactor        the maximum load factor.
         * @throws	ArgumentException if <tt>initialCapacity < 0 || (minLoadFactor < 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor <= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</tt>.
         */

        public OpenLongObjectHashMap(int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            setUp(initialCapacity, minLoadFactor, maxLoadFactor);
        }

        /**
         * Removes all (key,value) associations from the receiver.
         * Implicitly calls <tt>trimToSize()</tt>.
         */

        public override void Clear()
        {
            new ByteArrayList(m_state).fillFromToWith(0, m_state.Length - 1, FREE);
            new ObjectArrayList(m_values).fillFromToWith(0, m_state.Length - 1, null); // delta

            m_distinct = 0;
            freeEntries = table.Length; // delta
            trimToSize();
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return  a deep copy of the receiver.
         */

        public new Object Clone()
        {
            OpenLongObjectHashMap copy = (OpenLongObjectHashMap) base.Clone();
            copy.table = (long[]) copy.table.Clone();
            copy.m_values = (Object[]) copy.m_values.Clone();
            copy.m_state = (byte[]) copy.m_state.Clone();
            return copy;
        }

        /**
         * Returns <tt>true</tt> if the receiver contains the specified key.
         *
         * @return <tt>true</tt> if the receiver contains the specified key.
         */

        public new bool containsKey(long key)
        {
            return indexOfKey(key) >= 0;
        }

        /**
         * Returns <tt>true</tt> if the receiver contains the specified value.
         *
         * @return <tt>true</tt> if the receiver contains the specified value.
         */

        public new bool containsValue(Object value)
        {
            return indexOfValue(value) >= 0;
        }

        /**
         * Ensures that the receiver can hold at least the specified number of associations without needing to allocate new internal memory.
         * If necessary, allocates new internal memory and increases the capacity of the receiver.
         * <p>
         * This method never need be called; it is for performance tuning only.
         * Calling this method before <tt>put()</tt>ing a large number of associations boosts performance,
         * because the receiver will grow only once instead of potentially many times and hash collisions get less probable.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public new void ensureCapacity(int minCapacity)
        {
            if (table.Length < minCapacity)
            {
                int newCapacity = nextPrime(minCapacity);
                rehash(newCapacity);
            }
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

        public override bool forEachKey(LongProcedure procedure)
        {
            for (int i = table.Length; i-- > 0;)
            {
                if (m_state[i] == FULL)
                {
                    if (!procedure.Apply(table[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Applies a procedure to each (key,value) pair of the receiver, if any.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         *
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all keys where iterated over, <tt>true</tt> otherwise. 
         */

        public new bool forEachPair(LongObjectProcedure procedure)
        {
            for (int i = table.Length; i-- > 0;)
            {
                if (m_state[i] == FULL)
                {
                    if (!procedure.Apply(table[i], m_values[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Returns the value associated with the specified key.
         * It is often a good idea to first check with {@link #containsKey(long)} whether the given key has a value associated or not, i.e. whether there exists an association for the given key or not.
         *
         * @param key the key to be searched for.
         * @return the value associated with the specified key; <tt>null</tt> if no such key is present.
         */

        public override Object get(long key)
        {
            int i = indexOfKey(key);
            if (i < 0)
            {
                return null; //not contained
            }
            return m_values[i];
        }

        /**
         * @param key the key to be added to the receiver.
         * @return the index where the key would need to be inserted, if it is not already contained.
         * Returns -index-1 if the key is already contained at slot index.
         * Therefore, if the returned index < 0, then it is already contained at slot -index-1.
         * If the returned index >= 0, then it is NOT already contained and should be inserted at slot index.
         */

        public int indexOfInsertion(long key)
        {
            long[] tab = table;
            byte[] stat = m_state;
            int Length = tab.Length;

            int hash = HashFunctions.hash(key) & 0x7FFFFFFF;
            int i = hash%Length;
            int decrement = hash%(Length - 2);
            // double hashing, see http://www.eece.unm.edu/faculty/heileman/hash/node4.html
            //int decrement = (hash / Length) % Length;
            if (decrement == 0)
            {
                decrement = 1;
            }

            // stop if we find a removed or free slot, or if we find the key itself
            // do NOT skip over removed slots (yes, open addressing is like that...)
            while (stat[i] == FULL && tab[i] != key)
            {
                i -= decrement;
                //hashCollisions++;
                if (i < 0)
                {
                    i += Length;
                }
            }

            if (stat[i] == REMOVED)
            {
                // stop if we find a free slot, or if we find the key itself.
                // do skip over removed slots (yes, open addressing is like that...)
                // assertion: there is at least one FREE slot.
                int j = i;
                while (stat[i] != FREE && (stat[i] == REMOVED || tab[i] != key))
                {
                    i -= decrement;
                    //hashCollisions++;
                    if (i < 0)
                    {
                        i += Length;
                    }
                }
                if (stat[i] == FREE)
                {
                    i = j;
                }
            }


            if (stat[i] == FULL)
            {
                // key already contained at slot i.
                // return a negative number identifying the slot.
                return -i - 1;
            }
            // not already contained, should be inserted at slot i.
            // return a number >= 0 identifying the slot.
            return i;
        }

        /**
         * @param key the key to be searched in the receiver.
         * @return the index where the key is contained in the receiver, returns -1 if the key was not found.
         */

        public int indexOfKey(long key)
        {
            long[] tab = table;
            byte[] stat = m_state;
            int Length = tab.Length;

            int hash = HashFunctions.hash(key) & 0x7FFFFFFF;
            int i = hash%Length;
            int decrement = hash%(Length - 2);
            // double hashing, see http://www.eece.unm.edu/faculty/heileman/hash/node4.html
            //int decrement = (hash / Length) % Length;
            if (decrement == 0)
            {
                decrement = 1;
            }

            // stop if we find a free slot, or if we find the key itself.
            // do skip over removed slots (yes, open addressing is like that...)
            while (stat[i] != FREE && (stat[i] == REMOVED || tab[i] != key))
            {
                i -= decrement;
                //hashCollisions++;
                if (i < 0)
                {
                    i += Length;
                }
            }

            if (stat[i] == FREE)
            {
                return -1; // not found
            }
            return i; //found, return index where key is contained
        }

        /**
         * @param value the value to be searched in the receiver.
         * @return the index where the value is contained in the receiver, returns -1 if the value was not found.
         */

        public int indexOfValue(Object value)
        {
            Object[] val = m_values;
            byte[] stat = m_state;

            for (int i = stat.Length; --i >= 0;)
            {
                if (stat[i] == FULL && val[i] == value)
                {
                    return i;
                }
            }

            return -1; // not found
        }

        /**
         * Returns the first key the given value is associated with.
         * It is often a good idea to first check with {@link #containsValue(Object)} whether there exists an association from a key to this value.
         * Search order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(LongProcedure)}.
         *
         * @param value the value to search for.
         * @return the first key for which holds <tt>get(key) == value</tt>; 
         *		   returns <tt>long.MinValue</tt> if no such key exists.
         */

        public new long keyOf(Object value)
        {
            //returns the first key found; there may be more matching keys, however.
            int i = indexOfValue(value);
            if (i < 0)
            {
                return long.MinValue;
            }
            return table[i];
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

        public new void keys(LongArrayList list)
        {
            list.setSize(m_distinct);
            long[] elements = list.elements();

            long[] tab = table;
            byte[] stat = m_state;

            int j = 0;
            for (int i = tab.Length; i-- > 0;)
            {
                if (stat[i] == FULL)
                {
                    elements[j++] = tab[i];
                }
            }
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

        public new void pairsMatching(LongObjectProcedure condition, LongArrayList keyList, ObjectArrayList valueList)
        {
            keyList.Clear();
            valueList.Clear();

            for (int i = table.Length; i-- > 0;)
            {
                if (m_state[i] == FULL && condition.Apply(table[i], m_values[i]))
                {
                    keyList.Add(table[i]);
                    valueList.Add(m_values[i]);
                }
            }
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

        public override bool put(long key, Object value)
        {
            int i = indexOfInsertion(key);
            if (i < 0)
            {
                //already contained
                i = -i - 1;
                m_values[i] = value;
                return false;
            }

            if (m_distinct > m_highWaterMark)
            {
                int newCapacity = chooseGrowCapacity(m_distinct + 1, m_minLoadFactor, m_maxLoadFactor);
                rehash(newCapacity);
                return put(key, value);
            }

            table[i] = key;
            m_values[i] = value;
            if (m_state[i] == FREE)
            {
                freeEntries--;
            }
            m_state[i] = FULL;
            m_distinct++;

            if (freeEntries < 1)
            {
                //delta
                int newCapacity = chooseGrowCapacity(m_distinct + 1, m_minLoadFactor, m_maxLoadFactor);
                rehash(newCapacity);
            }

            return true;
        }

        /**
         * Rehashes the contents of the receiver into a new table
         * with a smaller or larger capacity.
         * This method is called automatically when the
         * number of keys in the receiver exceeds the high water mark or falls below the low water mark.
         */

        public void rehash(int newCapacity)
        {
            int oldCapacity = table.Length;
            //if (oldCapacity == newCapacity) return;

            long[] oldTable = table;
            Object[] oldValues = m_values;
            byte[] oldState = m_state;

            long[] newTable = new long[newCapacity];
            Object[] newValues = new Object[newCapacity];
            byte[] newState = new byte[newCapacity];

            m_lowWaterMark = chooseLowWaterMark(newCapacity, m_minLoadFactor);
            m_highWaterMark = chooseHighWaterMark(newCapacity, m_maxLoadFactor);

            table = newTable;
            m_values = newValues;
            m_state = newState;
            freeEntries = newCapacity - m_distinct; // delta

            for (int i = oldCapacity; i-- > 0;)
            {
                if (oldState[i] == FULL)
                {
                    long element = oldTable[i];
                    int index = indexOfInsertion(element);
                    newTable[index] = element;
                    newValues[index] = oldValues[i];
                    newState[index] = FULL;
                }
            }
        }

        /**
         * Removes the given key with its associated element from the receiver, if present.
         *
         * @param key the key to be removed from the receiver.
         * @return <tt>true</tt> if the receiver contained the specified key, <tt>false</tt> otherwise.
         */

        public override bool removeKey(long key)
        {
            int i = indexOfKey(key);
            if (i < 0)
            {
                return false; // key not contained
            }

            m_state[i] = REMOVED;
            m_values[i] = null; // delta
            m_distinct--;

            if (m_distinct < m_lowWaterMark)
            {
                int newCapacity = chooseShrinkCapacity(m_distinct, m_minLoadFactor, m_maxLoadFactor);
                rehash(newCapacity);
            }

            return true;
        }

        /**
         * Initializes the receiver.
         *
         * @param      initialCapacity   the initial capacity of the receiver.
         * @param      minLoadFactor        the minLoadFactor of the receiver.
         * @param      maxLoadFactor        the maxLoadFactor of the receiver.
         * @throws	ArgumentException if <tt>initialCapacity < 0 || (minLoadFactor < 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor <= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</tt>.
         */

        public new void setUp(int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            int capacity = initialCapacity;
            base.setUp(capacity, minLoadFactor, maxLoadFactor);
            capacity = nextPrime(capacity);
            if (capacity == 0)
            {
                capacity = 1; // open addressing needs at least one FREE slot at any time.
            }

            table = new long[capacity];
            m_values = new Object[capacity];
            m_state = new byte[capacity];

            // memory will be exhausted long before this pathological case happens, anyway.
            m_minLoadFactor = minLoadFactor;
            if (capacity == PrimeFinder.largestPrime)
            {
                maxLoadFactor = 1.0;
            }
            else
            {
                m_maxLoadFactor = maxLoadFactor;
            }

            m_distinct = 0;
            freeEntries = capacity; // delta

            // lowWaterMark will be established upon first expansion.
            // establishing it now (upon instance construction) would immediately make the table shrink upon first put(...).
            // After all the idea of an "initialCapacity" implies violating lowWaterMarks when an object is young.
            // See ensureCapacity(...)
            m_lowWaterMark = 0;
            m_highWaterMark = chooseHighWaterMark(capacity, maxLoadFactor);
        }

        /**
         * Trims the capacity of the receiver to be the receiver's current 
         * size. Releases any superfluous internal memory. An application can use this operation to minimize the 
         * storage of the receiver.
         */

        public new void trimToSize()
        {
            // * 1.2 because open addressing's performance exponentially degrades beyond that point
            // so that even rehashing the table can take very long
            int newCapacity = nextPrime((int) (1 + 1.2*Size()));
            if (table.Length > newCapacity)
            {
                rehash(newCapacity);
            }
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

        public new void values(ObjectArrayList list)
        {
            list.setSize(m_distinct);
            Object[] elements = list.elements();

            Object[] val = m_values;
            byte[] stat = m_state;

            int j = 0;
            for (int i = stat.Length; i-- > 0;)
            {
                if (stat[i] == FULL)
                {
                    elements[j++] = val[i];
                }
            }
        }
    }
}
