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

    /**
    Status: Experimental; Do not use for production yet. Hash map holding (key,value) associations of type <tt>(int-->int)</tt>; Automatically grows and shrinks as needed; Implemented using open addressing with double hashing.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.

    Implements open addressing with double hashing, using "Brent's variation".
    Brent's variation slows insertions a bit down (not much) but reduces probes (collisions) for successful searches, in particular for large load factors.
    (It does not improve unsuccessful searches.)
    See D. Knuth, Searching and Sorting, 3rd ed., p.533-545
 
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    @see	    HashMap
    */

    [Serializable]
    public class QuickOpenIntIntHashMap : OpenIntIntHashMap
    {
        public int totalProbesSaved; // benchmark only
        /**
         * Constructs an empty map with default capacity and default load factors.
         */

        public QuickOpenIntIntHashMap()
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

        public QuickOpenIntIntHashMap(int initialCapacity)
            : this(initialCapacity, m_defaultMinLoadFactor, m_defaultMaxLoadFactor)
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

        public QuickOpenIntIntHashMap(int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            setUp(initialCapacity, minLoadFactor, maxLoadFactor);
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

        public override bool put(int key, int value)
        {
            /* 
               This is open addressing with double hashing, using "Brent's variation".
               Brent's variation slows insertions a bit down (not much) but reduces probes (collisions) for successful searches, in particular for large load factors.
               (It does not improve unsuccessful searches.)
               See D. Knuth, Searching and Sorting, 3rd ed., p.533-545
	
               h1(key) = hash % M
               h2(key) = decrement = Max(1, hash/M % M)
               M is prime = capacity = table.Length
               probing positions are table[(h1-j*h2) % M] for j=0,1,...
               (M and h2 could also be chosen differently, but h2 is required to be relative prime to M.)
            */

            int key0;
            int[] tab = m_table;
            byte[] stat = m_state;
            int Length = tab.Length;

            int hash = HashFunctions.hash(key) & 0x7FFFFFFF;
            int i = hash%Length;
            int decrement = (hash/Length)%Length;
            if (decrement == 0)
            {
                decrement = 1;
            }
            //PrintToScreen.WriteLine("insert search for (key,value)=("+key+","+value+") at i="+i+", dec="+decrement);

            // stop if we find a removed or free slot, or if we find the key itself
            // do NOT skip over removed slots (yes, open addressing is like that...)
            //int comp = comparisons;
            int t = 0; // the number of probes
            int p0 = i; // the first position to probe
            while (stat[i] == FULL && tab[i] != key)
            {
                t++;
                i -= decrement;
                //hashCollisions++;
                if (i < 0)
                {
                    i += Length;
                }
            }
            //if (comparisons-comp>0) PrintToScreen.WriteLine("probed "+(comparisons-comp)+" slots.");
            if (stat[i] == FULL)
            {
                // key already contained at slot i.
                m_values[i] = value;
                return false;
            }
            // not already contained, should be inserted at slot i.

            if (m_distinct > m_highWaterMark)
            {
                int newCapacity = chooseGrowCapacity(m_distinct + 1, m_minLoadFactor, m_maxLoadFactor);

                //Console.Write("grow rehashing ");
                //PrintToScreen.WriteLine("at distinct="+distinct+", capacity="+table.Length+" to newCapacity="+newCapacity+" ...");

                rehash(newCapacity);
                return put(key, value);
            }

            /*
            Brent's variation does a local reorganization to reduce probes. It essentially means:
            We test whether it is possible to move the association we probed first (table[p0]) out of the way.
            If this is possible, it will reduce probes for the key to be inserted, since it takes its place; it gets hit earlier.
            However, future probes for the key that we move out of the way will increase.
            Thus we only move it out of the way, if we have a net gain, that is, if we save more probes than we loose.
            For the first probe we safe more than we loose if the number of probes we needed was >=2 (t>=2).
            If the first probe cannot be moved out of the way, we try the next probe (p1).
            Now we safe more than we loose if t>=3.
            We repeat this until we find that we cannot gain or that we can indeed move p(x) out of the way.

            Note: Under the great majority of insertions t<=1, so the loop is entered very infrequently.
            */
            while (t > 1)
            {
                //PrintToScreen.WriteLine("t="+t);
                key0 = tab[p0];
                hash = HashFunctions.hash(key0) & 0x7FFFFFFF;
                decrement = (hash/Length)%Length;
                if (decrement == 0)
                {
                    decrement = 1;
                }
                int pc = p0 - decrement; // pc = (p0-j*decrement) % M, j=1,2,..
                if (pc < 0)
                {
                    pc += Length;
                }

                if (stat[pc] != FREE)
                {
                    // not a free slot, continue searching for free slot to move to, or break.
                    p0 = pc;
                    t--;
                }
                else
                {
                    // free or removed slot found, now move...
                    //PrintToScreen.WriteLine("copying p0="+p0+" to pc="+pc+", (key,val)=("+tab[p0]+","+values[p0]+"), saving "+(t-1)+" probes.");
                    totalProbesSaved += (t - 1);
                    tab[pc] = key0;
                    stat[pc] = FULL;
                    m_values[pc] = m_values[p0];
                    i = p0; // prepare to insert: table[p0]=key
                    t = 0; // break loop 
                }
            }

            //PrintToScreen.WriteLine("inserting at i="+i);
            m_table[i] = key;
            m_values[i] = value;
            if (m_state[i] == FREE)
            {
                m_freeEntries--;
            }
            m_state[i] = FULL;
            m_distinct++;

            if (m_freeEntries < 1)
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

        public new void rehash(int newCapacity)
        {
            int oldCapacity = m_table.Length;
            //if (oldCapacity == newCapacity) return;

            int[] oldTable = m_table;
            int[] oldValues = m_values;
            byte[] oldState = m_state;

            int[] newTable = new int[newCapacity];
            int[] newValues = new int[newCapacity];
            byte[] newState = new byte[newCapacity];

            m_lowWaterMark = chooseLowWaterMark(newCapacity, m_minLoadFactor);
            m_highWaterMark = chooseHighWaterMark(newCapacity, m_maxLoadFactor);

            m_table = newTable;
            m_values = newValues;
            m_state = newState;
            m_freeEntries = newCapacity - m_distinct; // delta

            int tmp = m_distinct;
            m_distinct = int.MinValue; // switch of watermarks
            for (int i = oldCapacity; i-- > 0;)
            {
                if (oldState[i] == FULL)
                {
                    put(oldTable[i], oldValues[i]);
                    /*
                    int element = oldTable[i];
                    int index = indexOfInsertion(element);
                    newTable[index]=element;
                    newValues[index]=oldValues[i];
                    newState[index]=FULL;
                    */
                }
            }
            m_distinct = tmp;
        }
    }
}
