#region

using System;
using System.Collections;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright © 1999 CERN - European Organization for Nuclear Research.
    Permission to use, Copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    ////package list;

    /**
    Abstract base class for resizable collections holding objects or primitive data types such as <code>int</code>, <code>float</code>, etc.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Note that this implementation is not lock.</b>

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    @see     ArrayList
    @see	    Vector
    @see	    Arrays
    */
    //[Serializable]public abstract class AbstractCollection : Object , Cloneable, java.io.Serializable {
    [Serializable]
    public abstract class AbstractCollection : PersistentObject
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Removes all elements from the receiver.  The receiver will
         * be empty after this call returns.
         */
        public abstract void Clear();
        /**
         * Tests if the receiver has no elements.
         *
         * @return  <code>true</code> if the receiver has no elements;
         *          <code>false</code> otherwise.
         */

        public bool isEmpty()
        {
            return Size() == 0;
        }

        /**
         * Returns the number of elements contained in the receiver.
         *
         * @returns  the number of elements contained in the receiver.
         */
        public abstract int Size();
        /**
         * Returns a <code>ArrayList</code> containing all the elements in the receiver.
         */
        public abstract ArrayList toList();
        /**
        * Returns a string representation of the receiver, containing
        * the string representation of each element.
        */

        public override string ToString()
        {
            return toList().ToString();
        }
    }
}
