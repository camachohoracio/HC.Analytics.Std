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

    ////import DoubleIntProcedure;
    ////import DoubleProcedure;
    ////import DoubleArrayList;
    ////import IntArrayList;
    /**
    Abstract base class for hash maps holding (key,value) associations of type <tt>(double-->int)</tt>.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation</b>:
    <p>
    Almost all methods are expressed in terms of {@link #forEachKey(DoubleProcedure)}. 
    As such they are fully functional, but inefficient. Override them in subclasses if necessary.

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    @see	    HashMap
    */

    [Serializable]
    public abstract class AbstractDoubleIntMap : AbstractMap
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

        public bool containsKey(double key)
        {
            return !forEachKey(
                        new DoubleProcedure_(key));
        }

        /**
         * Returns <tt>true</tt> if the receiver contains the specified value.
         *
         * @return <tt>true</tt> if the receiver contains the specified value.
         */

        public bool containsValue(int value)
        {
            return !forEachPair(
                        new DoubleIntProcedure_(value)
                        );
        }

        /**
         * Returns a deep copy of the receiver; uses <code>Clone()</code> and casts the result.
         *
         * @return  a deep copy of the receiver.
         */

        public AbstractDoubleIntMap Copy()
        {
            return (AbstractDoubleIntMap) Clone();
        }

        /**
         * Compares the specified object with this map for equality.  Returns
         * <tt>true</tt> if the given object is also a map and the two maps
         * represent the same mappings.  More formally, two maps <tt>m1</tt> and
         * <tt>m2</tt> represent the same mappings iff
         * <pre>
         * m1.forEachPair(
         *		new DoubleIntProcedure() {
         *			public bool Apply(double key, int value) {
         *				return m2.containsKey(key) && m2.get(key) == value;
         *			}
         *		}
         *	)
         * &&
         * m2.forEachPair(
         *		new DoubleIntProcedure() {
         *			public bool Apply(double key, int value) {
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

            if (!(obj is AbstractDoubleIntMap))
            {
                return false;
            }
            AbstractDoubleIntMap other = (AbstractDoubleIntMap) obj;
            if (other.Size() != Size())
            {
                return false;
            }

            return
                forEachPair(
                    new DoubleIntProcedure2_(other)
                    )
                &&
                other.forEachPair(
                    new DoubleIntProcedure2_(this)
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
        public abstract bool forEachKey(DoubleProcedure procedure);
        /**
         * Applies a procedure to each (key,value) pair of the receiver, if any.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
         *
         * @param procedure    the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. 
         * @return <tt>false</tt> if the procedure stopped before all keys where iterated over, <tt>true</tt> otherwise. 
         */

        public bool forEachPair(DoubleIntProcedure procedure)
        {
            return forEachKey(
                new DoubleProcedure2_(procedure, this)
                );
        }

        /**
         * Returns the value associated with the specified key.
         * It is often a good idea to first check with {@link #containsKey(double)} whether the given key has a value associated or not, i.e. whether there exists an association for the given key or not.
         *
         * @param key the key to be searched for.
         * @return the value associated with the specified key; <tt>0</tt> if no such key is present.
         */
        public abstract int get(double key);
        /**
         * Returns the first key the given value is associated with.
         * It is often a good idea to first check with {@link #containsValue(int)} whether there exists an association from a key to this value.
         * Search order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
         *
         * @param value the value to search for.
         * @return the first key for which holds <tt>get(key) == value</tt>; 
         *		   returns <tt>double.NaN</tt> if no such key exists.
         */

        public double keyOf(int value)
        {
            double[] foundKey = new double[1];
            bool notFound = forEachPair(
                new DoubleIntProcedure3_(
                    foundKey,
                    value)
                );
            if (notFound)
            {
                return double.NaN;
            }
            return foundKey[0];
        }

        /**
         * Returns a list filled with all keys contained in the receiver.
         * The returned list has a size that equals <tt>Size()</tt>.
         * Note: Keys are filled into the list in no particular order.
         * However, the order is <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
         * <p>
         * This method can be used to iterate over the keys of the receiver.
         *
         * @return the keys.
         */

        public DoubleArrayList keys()
        {
            DoubleArrayList list = new DoubleArrayList(Size());
            keys(list);
            return list;
        }

        /**
         * Fills all keys contained in the receiver into the specified list.
         * Fills the list, starting at index 0.
         * After this call returns the specified list has a new size that equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
         * <p>
         * This method can be used to iterate over the keys of the receiver.
         *
         * @param list the list to be filled, can have any size.
         */

        public void keys(DoubleArrayList list)
        {
            list.Clear();
            forEachKey(
                new DoubleProcedure3_(list)
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

        public void keysSortedByValue(DoubleArrayList keyList)
        {
            pairsSortedByValue(keyList, new IntArrayList(Size()));
        }

        /**
        Fills all pairs satisfying a given condition into the specified lists.
        Fills into the lists, starting at index 0.
        After this call returns the specified lists both have a new size, the number of pairs satisfying the condition.
        Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
        <p>
        <b>Example:</b>
        <br>
        <pre>
        DoubleIntProcedure condition = new DoubleIntProcedure() { // match even values only
            public bool Apply(double key, int value) { return value%2==0; }
        }
        keys = (8,7,6), values = (1,2,2) --> keyList = (6,8), valueList = (2,1)</tt>
        </pre>

        @param condition    the condition to be matched. Takes the current key as first and the current value as second argument.
        @param keyList the list to be filled with keys, can have any size.
        @param valueList the list to be filled with values, can have any size.
        */

        public void pairsMatching(DoubleIntProcedure condition, DoubleArrayList keyList, IntArrayList valueList)
        {
            keyList.Clear();
            valueList.Clear();

            forEachPair(
                new DoubleIntProcedure4_(
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

        public void pairsSortedByKey(DoubleArrayList keyList, IntArrayList valueList)
        {
            /*
            keys(keyList); 
            values(valueList);
	
             double[] k = keyList.elements();
             int[] v = valueList.elements();
            Swapper swapper = new Swapper() {
                public void swap(int a, int b) {
                    int t1;	double t2;
                    t1 = v[a]; v[a] = v[b]; v[b] = t1;
                    t2 = k[a]; k[a] = k[b];	k[b] = t2;
                }
            }; 

            IntComparator comp = new IntComparator() {
                public int Compare(int a, int b) {
                    return k[a]<k[b] ? -1 : k[a]==k[b] ? 0 : 1;
                }
            };
            MultiSorting.Sort(0,keyList.Size(),comp,swapper);
            */


            // this variant may be quicker
            //OpenDoubleIntHashMap.hashCollisions = 0;
            //PrintToScreen.WriteLine("collisions="+OpenDoubleIntHashMap.hashCollisions);
            keys(keyList);
            keyList.Sort();
            valueList.setSize(keyList.Size());
            for (int i = keyList.Size(); --i >= 0;)
            {
                valueList.setQuick(i, get(keyList.getQuick(i)));
            }
            //PrintToScreen.WriteLine("collisions="+OpenDoubleIntHashMap.hashCollisions);
        }

        /**
         * Fills all keys and values <i>sorted ascending by value</i> into the specified lists.
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

        public void pairsSortedByValue(DoubleArrayList keyList, IntArrayList valueList)
        {
            keys(keyList);
            values(valueList);

            double[] k = keyList.elements();
            int[] v = valueList.elements();
            Swapper swapper = new Swapper10_(v, k);

            IntComparator comp = new IntComparator12_(k, v);

            //OpenDoubleIntHashMap.hashCollisions = 0;
            GenericSorting.quickSort(0, keyList.Size(), comp, swapper);
            //PrintToScreen.WriteLine("collisions="+OpenDoubleIntHashMap.hashCollisions);
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
        public abstract bool put(double key, int value);
        /**
         * Removes the given key with its associated element from the receiver, if present.
         *
         * @param key the key to be removed from the receiver.
         * @return <tt>true</tt> if the receiver contained the specified key, <tt>false</tt> otherwise.
         */
        public abstract bool removeKey(double key);
        /**
         * Returns a string representation of the receiver, containing
         * the string representation of each key-value pair, sorted ascending by key.
         */

        public override string ToString()
        {
            DoubleArrayList theKeys = keys();
            theKeys.Sort();

            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = theKeys.Size() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                double key = theKeys.get(i);
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
         * the string representation of each key-value pair, sorted ascending by value.
         */

        public string toStringByValue()
        {
            DoubleArrayList theKeys = new DoubleArrayList();
            keysSortedByValue(theKeys);

            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = theKeys.Size() - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                double key = theKeys.get(i);
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
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
         * <p>
         * This method can be used to iterate over the values of the receiver.
         *
         * @return the values.
         */

        public IntArrayList values()
        {
            IntArrayList list = new IntArrayList(Size());
            values(list);
            return list;
        }

        /**
         * Fills all values contained in the receiver into the specified list.
         * Fills the list, starting at index 0.
         * After this call returns the specified list has a new size that equals <tt>Size()</tt>.
         * Iteration order is guaranteed to be <i>identical</i> to the order used by method {@link #forEachKey(DoubleProcedure)}.
         * <p>
         * This method can be used to iterate over the values of the receiver.
         *
         * @param list the list to be filled, can have any size.
         */

        public void values(IntArrayList list)
        {
            list.Clear();
            forEachKey(
                new DoubleProcedure4_(list, this)
                );
        }
    }
}
