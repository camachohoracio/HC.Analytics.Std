#region

using System;
using System.Text;

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
    ////package cern.colt;

    /**
     * Array manipulations; complements <tt>Arrays</tt>.
     *
     * @see Arrays
     * @see Sorting
     *
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 03-Jul-99
     */

    [Serializable]
    public class Arrays : Object
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can 
         * hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static byte[] ensureCapacity(byte[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            byte[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new byte[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static char[] ensureCapacity(char[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            char[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new char[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static double[] ensureCapacity(double[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            double[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new double[newCapacity];
                //for (int i = oldCapacity; --i >= 0; ) newArray[i] = array[i];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static float[] ensureCapacity(float[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            float[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new float[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static int[] ensureCapacity(int[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            int[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new int[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static long[] ensureCapacity(long[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            long[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new long[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static Object[] ensureCapacity(Object[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            Object[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new Object[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static short[] ensureCapacity(short[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            short[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new short[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Ensures that a given array can hold up to <tt>minCapacity</tt> elements.
         *
         * Returns the identical array if it can hold at least the number of elements specified.
         * Otherwise, returns a new array with increased capacity containing the same elements, ensuring  
         * that it can hold at least the number of elements specified by 
         * the minimum capacity argument.
         *
         * @param   minCapacity   the desired minimum capacity.
         */

        public static bool[] ensureCapacity(bool[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            bool[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity*3)/2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new bool[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(byte[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(char[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(double[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(float[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(int[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(long[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(Object[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(short[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(bool[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static byte[] trimToCapacity(byte[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                byte[] oldArray = array;
                array = new byte[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static char[] trimToCapacity(char[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                char[] oldArray = array;
                array = new char[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static double[] trimToCapacity(double[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                double[] oldArray = array;
                array = new double[maxCapacity];

                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static float[] trimToCapacity(float[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                float[] oldArray = array;
                array = new float[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static int[] trimToCapacity(int[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                int[] oldArray = array;
                array = new int[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static long[] trimToCapacity(long[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                long[] oldArray = array;
                array = new long[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static Object[] trimToCapacity(Object[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                Object[] oldArray = array;
                array = new Object[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static short[] trimToCapacity(short[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                short[] oldArray = array;
                array = new short[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }

        /**
         * Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
         * An application can use this operation to minimize array storage.
         * <p>
         * Returns the identical array if <tt>array.Length &lt;= maxCapacity</tt>.
         * Otherwise, returns a new array with a Length of <tt>maxCapacity</tt>
         * containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
         *
         * @param   maxCapacity   the desired maximum capacity.
         */

        public static bool[] trimToCapacity(bool[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                bool[] oldArray = array;
                array = new bool[maxCapacity];
                Array.Copy(oldArray, 0, array, 0, maxCapacity);
            }
            return array;
        }
    }
}
