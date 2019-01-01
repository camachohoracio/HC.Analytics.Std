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
////package matrix;

//////import DenseObjectMatrix1D;
//////import SparseObjectMatrix1D;
/**
Factory for convenient construction of 1-d matrices holding <tt>Object</tt> cells.
Use idioms like <tt>ObjectFactory1D.dense.make(1000)</tt> to construct dense matrices, 
<tt>ObjectFactory1D.sparse.make(1000)</tt> to construct sparse matrices.

If the factory is used frequently it might be useful to streamline the notation. 
For example by aliasing:
<table>
<td class="PRE"> 
<pre>
ObjectFactory1D F = ObjectFactory1D.dense;
Functions.make(1000);
...
</pre>
</td>
</table>

@author wolfgang.hoschek@cern.ch
@version 1.0, 09/24/99
*/

    [Serializable]
    public class ObjectFactory1D : PersistentObject
    {
        /**
	 * A factory producing dense matrices.
	 */
        public static ObjectFactory1D dense = new ObjectFactory1D();

        /**
	 * A factory producing sparse matrices.
	 */
        public static ObjectFactory1D sparse = new ObjectFactory1D();
/**
 * Makes this class non instantiable, but still let's others inherit from it.
 */
/**
C = A||B; Constructs a new matrix which is the concatenation of two other matrices.
Example: <tt>0 1</tt> Append <tt>3 4</tt> --> <tt>0 1 3 4</tt>.
*/

        public ObjectMatrix1D Append(ObjectMatrix1D A, ObjectMatrix1D B)
        {
            // concatenate
            ObjectMatrix1D matrix = make(A.Size() + B.Size());
            matrix.viewPart(0, A.Size()).assign(A);
            matrix.viewPart(A.Size(), B.Size()).assign(B);
            return matrix;
        }

/**
Constructs a matrix which is the concatenation of all given parts.
Cells are copied.
*/

        public ObjectMatrix1D make(ObjectMatrix1D[] parts)
        {
            if (parts.Length == 0)
            {
                return make(0);
            }

            int size = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                size += parts[i].Size();
            }

            ObjectMatrix1D vector = make(size);
            size = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                vector.viewPart(size, parts[i].Size()).assign(parts[i]);
                size += parts[i].Size();
            }

            return vector;
        }

/**
 * Constructs a matrix with the given cell values.
 * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
 *
 * @param values The values to be filled into the new matrix.
 */

        public ObjectMatrix1D make(Object[] values)
        {
            if (this == sparse)
            {
                return new SparseObjectMatrix1D(values);
            }
            else
            {
                return new DenseObjectMatrix1D(values);
            }
        }

/**
 * Constructs a matrix with the given shape, each cell initialized with zero.
 */

        public ObjectMatrix1D make(int size)
        {
            if (this == sparse)
            {
                return new SparseObjectMatrix1D(size);
            }
            return new DenseObjectMatrix1D(size);
        }

/**
 * Constructs a matrix with the given shape, each cell initialized with the given value.
 */

        public ObjectMatrix1D make(int size, Object initialValue)
        {
            return make(size).assign(initialValue);
        }

/**
 * Constructs a matrix from the values of the given list.
 * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
 *
 * @param values The values to be filled into the new matrix.
 * @return a new matrix.
 */

        public ObjectMatrix1D make(ObjectArrayList values)
        {
            int size = values.Size();
            ObjectMatrix1D vector = make(size);
            for (int i = size; --i >= 0;)
            {
                vector.set(i, values.get(i));
            }
            return vector;
        }

/**
C = A||A||..||A; Constructs a new matrix which is concatenated <tt>repeat</tt> times.
Example:
<pre>
0 1
repeat(3) -->
0 1 0 1 0 1
</pre>
*/

        public ObjectMatrix1D repeat(ObjectMatrix1D A, int repeat)
        {
            int size = A.Size();
            ObjectMatrix1D matrix = make(repeat*size);
            for (int i = repeat; --i >= 0;)
            {
                matrix.viewPart(size*i, size).assign(A);
            }
            return matrix;
        }

/**
 * Constructs a list from the given matrix.
 * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the list, and vice-versa.
 *
 * @param values The values to be filled into the new list.
 * @return a new list.
 */

        public ObjectArrayList toList(ObjectMatrix1D values)
        {
            int size = values.Size();
            ObjectArrayList list = new ObjectArrayList(size);
            list.setSize(size);
            for (int i = size; --i >= 0;)
            {
                list.set(i, values.get(i));
            }
            return list;
        }
    }
}
