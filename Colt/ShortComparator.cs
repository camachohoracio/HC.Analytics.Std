#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package function;

    /*
    Copyright © 1999 CERN - European Organization for Nuclear Research.
    Permission to use, Copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    /**
     * A comparison function which imposes a <i>total ordering</i> on some
     * collection of elements.  Comparators can be passed to a sort method (such as
     * <tt>Sorting.quickSort</tt>) to allow precise control over the sort order.<p>
     *
     * Note: It is generally a good idea for comparators to implement
     * <tt>java.io.Serializable</tt>, as they may be used as ordering methods in
     * serializable data structures.  In
     * order for the data structure to serialize successfully, the comparator (if
     * provided) must implement <tt>Serializable</tt>.<p>
     *
     * @author  wolfgang.hoschek@cern.ch
     * @version 0.1 01/09/99
     * @see IComparer
     * @see Sorting
     */

    public interface ShortComparator
    {
        /**
         * Compares its two arguments for order.  Returns a negative integer,
         * zero, or a positive integer as the first argument is less than, equal
         * to, or greater than the second.<p>
         *
         * The implementor must ensure that <tt>sgn(Compare(x, y)) ==
         * -sgn(Compare(y, x))</tt> for all <tt>x</tt> and <tt>y</tt>.  (This
         * implies that <tt>Compare(x, y)</tt> must throw an exception if and only
         * if <tt>Compare(y, x)</tt> throws an exception.)<p>
         *
         * The implementor must also ensure that the relation is transitive:
         * <tt>((Compare(x, y)&gt;0) &amp;&amp; (Compare(y, z)&gt;0))</tt> implies
         * <tt>Compare(x, z)&gt;0</tt>.<p>
         *
         * Finally, the implementer must ensure that <tt>Compare(x, y)==0</tt>
         * implies that <tt>sgn(Compare(x, z))==sgn(Compare(y, z))</tt> for all
         * <tt>z</tt>.<p>
         *
         * 
         * @return a negative integer, zero, or a positive integer as the
         * 	       first argument is less than, equal to, or greater than the
         *	       second. 
         */
        int Compare(short o1, short o2);
        /**
         * 
         * Indicates whether some other object is &quot;equal to&quot; this
         * IComparer.  This method must obey the general contract of
         * <tt>Object.Equals(Object)</tt>.  Additionally, this method can return
         * <tt>true</tt> <i>only</i> if the specified Object is also a comparator
         * and it imposes the same ordering as this comparator.  Thus,
         * <code>comp1.0equals(comp2)</code> implies that <tt>sgn(comp1.Compare(o1,
         * o2))==sgn(comp2.Compare(o1, o2))</tt> for every element
         * <tt>o1</tt> and <tt>o2</tt>.<p>
         *
         * Note that it is <i>always</i> safe <i>not</i> to override
         * <tt>Object.Equals(Object)</tt>.  However, overriding this method may,
         * in some cases, improve performance by allowing programs to determine
         * that two distinct Comparators impose the same order.
         *
         * @param   obj   the reference object with which to Compare.
         * @return  <code>true</code> only if the specified object is also
         *		a comparator and it imposes the same ordering as this
         *		comparator.
         * @see     java.lang.Object#Equals(java.lang.Object)
         * @see java.lang.Object#hashCode()
         */
        bool Equals(Object obj);
    }
}
