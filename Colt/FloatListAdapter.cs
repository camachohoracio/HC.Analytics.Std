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
    //package adapter;

    ////import AbstractFloatList;
    /**
     * Adapter that permits an {@link AbstractFloatList} to be viewed and treated as a JDK 1.2 {@link AbstractList}.
     * Makes the contained list compatible with the JDK 1.2 Collections Framework.
     * <p>
     * Any attempt to pass elements other than <tt>java.lang.Number</tt> to setter methods will throw a <tt>java.lang.ClassCastException</tt>.
     * <tt>java.lang.Number</tt> is used to convert objects into primitive values which are then stored in the backing templated list.
     * Getter methods return <tt>java.lang.Float</tt> objects.
     */

    [Serializable]
    public class FloatListAdapter
    {
        public AbstractFloatList m_content;
        /**
         * Constructs a list backed by the specified content list.
         */

        public FloatListAdapter(AbstractFloatList content)
        {
            m_content = content;
        }

        /**
         * Inserts the specified element at the specified position in this list
         * (optional operation).  Shifts the element currently at that position
         * (if any) and any subsequent elements to the right (adds one to their
         * indices).<p>
         *
         * @param index index at which the specified element is to be inserted.
         * @param element element to be inserted.
         * 
         * @throws ClassCastException if the class of the specified element
         * 		  prevents it from being added to this list.
         * @throws ArgumentException if some aspect of the specified
         *		  element prevents it from being added to this list.
         * @throws HCException index is out of range (<tt>index &lt;
         *		  0 || index &gt; Size()</tt>).
         */

        public void Add(int index, Object element)
        {
            m_content.beforeInsert(index, value(element));
            //modCount++;
        }

        /**
         * Returns the element at the specified position in this list.
         *
         * @param index index of element to return.
         * 
         * @return the element at the specified position in this list.
         * @throws HCException if the given index is out of range
         * 		  (<tt>index &lt; 0 || index &gt;= Size()</tt>).
         */

        public Object get(int index)
        {
            return object_(m_content.get(index));
        }

        /**
         * Transforms an element of a primitive data type to an object. 
         */

        public static Object object_(float element)
        {
            return element;
        }

        /**
         * Removes the element at the specified position in this list (optional
         * operation).  Shifts any subsequent elements to the left (subtracts one
         * from their indices).  Returns the element that was removed from the
         * list.<p>
         *
         * @param index the index of the element to remove.
         * @return the element previously at the specified position.
         * 
         * @throws HCException if the specified index is out of
         * 		  range (<tt>index &lt; 0 || index &gt;= Size()</tt>).
         */

        public Object remove(int index)
        {
            Object old = get(index);
            m_content.remove(index);
            //modCount++;
            return old;
        }

        /**
         * Replaces the element at the specified position in this list with the
         * specified element (optional operation). <p>
         *
         * @param index index of element to replace.
         * @param element element to be stored at the specified position.
         * @return the element previously at the specified position.
         * 
         * @throws ClassCastException if the class of the specified element
         * 		  prevents it from being added to this list.
         * @throws ArgumentException if some aspect of the specified
         *		  element prevents it from being added to this list.
         * 
         * @throws HCException if the specified index is out of
         *            range (<tt>index &lt; 0 || index &gt;= Size()</tt>).
         */

        public Object set(int index, Object element)
        {
            Object old = get(index);
            m_content.set(index, value(element));
            return old;
        }

        /**
         * Returns the number of elements in this list.
         *
         * @return  the number of elements in this list.
         */

        public int Size()
        {
            return m_content.Size();
        }

        /**
         * Transforms an object element to a primitive data type. 
         */

        public static float value(Object element)
        {
            return (float) element;
        }
    }
}
