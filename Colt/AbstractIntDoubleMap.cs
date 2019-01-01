#region

using System;
using System.Text;
using HC.Analytics.Colt.CustomImplementations;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, Copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package map;

    ////import IntDoubleProcedure;
    ////import IntProcedure;
    ////import DoubleArrayList;
    ////import IntArrayList;
    /**
    Abstract base class for hash maps holding (key,value) associations of type <tt>(int-->double)</tt>.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation</b>:
    <p>
    Almost all methods are expressed in terms of {@link #forEachKey(IntProcedure)}. 
    As such they are fully functional, but inefficient. Override them in subclasses if necessary.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    @see	    HashMap
    */

    [Serializable]
    public abstract class AbstractIntDoubleMap : AbstractMap
    {
        //public static int hashCollisions = 0; // for debug only
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */

        /**
        Assigns the result of a function to each value; <tt>v[i] = function(v[i])</tt>.

        @param function a function object taking as argument the current association's value.
        */

        public void assign(DoubleFunction function)
        {
            Copy().forEachPair(
                new IntDoubleProcedure_(function, this));
        }

        /**
         * Clears the receiver, then adds all (key,value) pairs of <tt>other</tt>values to it.
         *
         * @param other the other map to be copied into the receiver.
         */

        public void assign(AbstractIntDoubleMap other)
        {
            Clear();
            other.forEachPair(
                new IntDoubleProcedure2_(this));
        }

        /**
         * Returns <tt>true</tt> if the receiver contains the specified key.
         *
         * @return <tt>true</tt> if the receiver contains the specified key.
         */

        public bool containsKey(int key)
        {
            return !forEachKey(
                        new IntProcedure_(this, key) //;
                        //{
                        //    public bool Apply(int iterKey) {
                        //        return (key != iterKey);
                        //    }
                        //}
                        );
        }

        /**
         * Returns <tt>true</tt> if the receiver contains the specified value.
         *
         * @return <tt>true</tt> if the receiver contains the specified value.
         */

        public bool containsValue(double value)
        {
            return !forEachPair(
                        new IntDoubleProcedure3_(value)
                        );
        }

        /**
         * Returns a deep Copy of the receiver; uses <code>Clone()</code> and casts the result.
         *
         * @return  a deep Copy of the receiver.
         */

        public AbstractIntDoubleMap Copy()
        {
            return (AbstractIntDoubleMap) Clone();
        }

        /**
         * Compares the specified object with this map for equality.  Returns
         * <tt>true</tt> if the given object is also a map and the two maps
         * represent the same mappings.  More formally, two maps <tt>m1</tt> and
         * <tt>m2</tt> represent the same mappings iff
         * <pre>
         * m1.forEachPair(
         *		new IntDoubleProcedure() {
         *			public bool Apply(int key, double value) {
         *				return m2.containsKey(key) && m2.get(key) == value;
         *			}
         *		}
         *	)
         * &&
         * m2.forEachPair(
         *		new IntDoubleProcedure() {
         *			public bool Apply(int key, double value) {
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

        public new bool Equals(Object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is AbstractIntDoubleMap))
            {
                return false;
            }
            AbstractIntDoubleMap other = (AbstractIntDoubleMap) obj;
            if (other.Size() != Size())
            {
                return false;
            }

            return
                forEachPair(
                    new IntDoubleProcedure4_(other)
                    )
                &&
                other.forEachPair(
                    new IntDoubleProcedure5_(this)
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
        public abstract bool forEachKey(IntProcedure procedure);
        /**
         * Applies a procedure to each (key,value) pair of the receiver, if any.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
         *
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all keys where iterated over, <tt>true</tt> otherwise. 
         */

        public bool forEachPair(IntDoubleProcedure procedure)
        {
            return forEachKey(
                new IntProcedure2_(
                    this,
                    procedure)
                );
        }

        /**
         * Returns the value associated with the specified key.
         * It is often a good idea to first check with {@link #containsKey(int)} whether the given key has a value associated or not, i.e. whether there exists an association for the given key or not.
         *
         * @param key the key to be searched for.
         * @return the value associated with the specified key; <tt>0</tt> if no such key is present.
         */
        public abstract double get(int key);
        /**
         * Returns the first key the given value is associated with.
         * It is often a good idea to first check with {@link #containsValue(double)} whether there exists an association from a key to this value.
         * Search order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
         *
         * @param value the value to search for.
         * @return the first key for which holds <tt>get(key) == value</tt>; 
         *		   returns <tt>int.MinValue</tt> if no such key exists.
         */

        public int keyOf(double value)
        {
            int[] foundKey = new int[1];
            bool notFound = forEachPair(
                new IntDoubleProcedure6_(
                    this,
                    value,
                    foundKey)
                );
            if (notFound)
            {
                return int.MinValue;
            }
            return foundKey[0];
        }

        /**
         * Returns a list filled with all keys contained in the receiver.
         * The returned list has a size that Equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
         * <p>
         * This method can be used to iterate over the keys of the receiver.
         *
         * @return the keys.
         */

        public IntArrayList keys()
        {
            IntArrayList list = new IntArrayList(Size());
            keys(list);
            return list;
        }

        /**
         * Fills all keys contained in the receiver into the specified list.
         * Fills the list, starting at index 0.
         * After this call returns the specified list has a new size that Equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
         * <p>
         * This method can be used to iterate over the keys of the receiver.
         *
         * @param list the list to be filled, can have any size.
         */

        public void keys(IntArrayList list)
        {
            list.Clear();
            forEachKey(new IntProcedure3_(list));
        }

        /**
         * Fills all keys <i>sorted ascending by their associated value</i> into the specified list.
         * Fills into the list, starting at index 0.
         * After this call returns the specified list has a new size that Equals <tt>Size()</tt>.
         * Primary sort criterium is "value", secondary sort criterium is "key". 
         * This means that if any two values are equal, the smaller key comes first.
         * <p>
         * <b>Example:</b>
         * <br>
         * <tt>keys = (8,7,6), values = (1,2,2) --> keyList = (8,6,7)</tt>
         *
         * @param keyList the list to be filled, can have any size.
         */

        public void keysSortedByValue(IntArrayList keyList)
        {
            pairsSortedByValue(keyList, new DoubleArrayList(Size()));
        }

        /**
        Fills all pairs satisfying a given condition into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists both have a new size, the number of pairs satisfying the condition.
        Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        IntDoubleProcedure condition = new IntDoubleProcedure() { // match even keys only
            public bool Apply(int key, double value) { return key%2==0; }
        }
        keys = (8,7,6), values = (1,2,2) --> keyList = (6,8), valueList = (2,1)</tt>
        </pre>

        @param condition    the condition to be matched. Takes the current key as first and the current value as second argument.
        @param keyList the list to be filled with keys, can have any size.
        @param valueList the list to be filled with values, can have any size.
        */

        public void pairsMatching(
            IntDoubleProcedure condition,
            IntArrayList keyList,
            DoubleArrayList valueList)
        {
            keyList.Clear();
            valueList.Clear();

            forEachPair(
                new IntDoubleProcedure7_(condition,
                                         keyList,
                                         valueList));
        }

        /**
         * Fills all keys and values <i>sorted ascending by key</i> into the specified lists.
         * Fills into the lists, starting at index 0.
         * After this call returns the specified lists both have a new size that Equals <tt>Size()</tt>.
         * <p>
         * <b>Example:</b>
         * <br>
         * <tt>keys = (8,7,6), values = (1,2,2) --> keyList = (6,7,8), valueList = (2,2,1)</tt>
         *
         * @param keyList the list to be filled with keys, can have any size.
         * @param valueList the list to be filled with values, can have any size.
         */

        public void pairsSortedByKey(IntArrayList keyList, DoubleArrayList valueList)
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
         * Fills all keys and values <i>sorted ascending by value</i> into the specified lists.
         * Fills into the lists, starting at index 0.
         * After this call returns the specified lists both have a new size that Equals <tt>Size()</tt>.
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

        public void pairsSortedByValue(
            IntArrayList keyList,
            DoubleArrayList valueList)
        {
            keys(keyList);
            values(valueList);

            int[] k = keyList.elements();
            double[] v = valueList.elements();

            Swapper swapper = new Swapper_(k, v);

            IntComparator comp = new IntComparator_(k, v);

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
        public abstract bool put(int key, double value);
        /**
         * Removes the given key with its associated element from the receiver, if present.
         *
         * @param key the key to be removed from the receiver.
         * @return <tt>true</tt> if the receiver contained the specified key, <tt>false</tt> otherwise.
         */
        public abstract bool removeKey(int key);
        /**
         * Returns a string representation of the receiver, containing
         * the string representation of each key-value pair, sorted ascending by key.
         */

        public override string ToString()
        {
            IntArrayList theKeys = keys();
            string tmp = theKeys + Environment.NewLine;
            theKeys.Sort();

            StringBuilder buf = new StringBuilder(tmp);
            //StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = theKeys.Size() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                int key = theKeys.get(i);
                buf.Append(key);
                buf.Append("->");
                buf.Append(get(key));
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
         * the string representation of each key-value pair, sorted ascending by value.
         */

        public string toStringByValue()
        {
            IntArrayList theKeys = new IntArrayList();
            keysSortedByValue(theKeys);

            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = theKeys.Size() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                int key = theKeys.get(i);
                buf.Append(key);
                buf.Append("->");
                buf.Append(get(key));
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
         * The returned list has a size that Equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
         * <p>
         * This method can be used to iterate over the values of the receiver.
         *
         * @return the values.
         */

        public DoubleArrayList values()
        {
            DoubleArrayList list = new DoubleArrayList(Size());
            values(list);
            return list;
        }

        /**
         * Fills all values contained in the receiver into the specified list.
         * Fills the list, starting at index 0.
         * After this call returns the specified list has a new size that Equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(IntProcedure)}.
         * <p>
         * This method can be used to iterate over the values of the receiver.
         *
         * @param list the list to be filled, can have any size.
         */

        public void values(DoubleArrayList list)
        {
            list.Clear();
            forEachKey(
                new IntProcedure4_(list));
        }
    }
}
