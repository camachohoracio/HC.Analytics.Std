#region

using System;

#endregion

namespace HC.Analytics.Colt
{
/*
Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
is hereby granted without fee, provided that the above copyright notice appear in all copies and 
that both that copyright notice and this permission notice appear in supporting documentation. 
CERN makes no representations about the suitability of this software for any purpose. 
It is provided "as is" without expressed or implied warranty.
*/
////package matrix;

////import DenseDoubleMatrix3D;
////import SparseDoubleMatrix3D;
/**
Factory for convenient construction of 3-d matrices holding <tt>double</tt> cells. 
Use idioms like <tt>DoubleFactory3D.dense.make(4,4,4)</tt> to construct dense matrices, 
<tt>DoubleFactory3D.sparse.make(4,4,4)</tt> to construct sparse matrices.

If the factory is used frequently it might be useful to streamline the notation. 
For example by aliasing:
<table>
<td class="PRE"> 
<pre>
DoubleFactory3D F = DoubleFactory3D.dense;
Functions.make(4,4,4);
Functions.descending(10,20,5);
Functions.random(4,4,5);
...
</pre>
</td>
</table>

@author wolfgang.hoschek@cern.ch
@version 1.0, 09/24/99
*/

    [Serializable]
    public class DoubleFactory3D : PersistentObject
    {
        /**
	 * A factory producing dense matrices.
	 */
        public static DoubleFactory3D dense = new DoubleFactory3D();

        /**
	 * A factory producing sparse matrices.
	 */
        public static DoubleFactory3D sparse = new DoubleFactory3D();
/**
 * Makes this class non instantiable, but still let's others inherit from it.
 */
/**
 * Constructs a matrix with cells having ascending values.
 * For debugging purposes.
 */

        public DoubleMatrix3D ascending(int slices, int rows, int columns)
        {
            Functions F = Functions.m_functions;
            return
                descending(slices, rows, columns).assign(Functions.chain(Functions.m_neg,
                                                                         Functions.minus(slices*rows*columns)));
        }

/**
 * Constructs a matrix with cells having descending values.
 * For debugging purposes.
 */

        public DoubleMatrix3D descending(int slices, int rows, int columns)
        {
            DoubleMatrix3D matrix = make(slices, rows, columns);
            int v = 0;
            for (int slice = slices; --slice >= 0;)
            {
                for (int row = rows; --row >= 0;)
                {
                    for (int column = columns; --column >= 0;)
                    {
                        matrix.setQuick(slice, row, column, v++);
                    }
                }
            }
            return matrix;
        }

/**
 * Constructs a matrix with the given cell values.
 * <tt>values</tt> is required to have the form <tt>values[slice,row,column]</tt>
 * and have exactly the same number of slices, rows and columns as the receiver.
 * <p>
 * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
 *
 * @param    values the values to be filled into the cells.
 * @return <tt>this</tt> (for convenience only).
 * @throws ArgumentException if <tt>values.Length != Slices() || for any 0 &lt;= slice &lt; Slices(): values[slice].Length != Rows()</tt>.
 * @throws ArgumentException if <tt>for any 0 &lt;= column &lt; Columns(): values[slice,row].Length != Columns()</tt>.
 */

        public DoubleMatrix3D make(double[,,] values)
        {
            if (this == sparse)
            {
                return new SparseDoubleMatrix3D(values);
            }
            return new DenseDoubleMatrix3D(values);
        }

/**
 * Constructs a matrix with the given shape, each cell initialized with zero.
 */

        public DoubleMatrix3D make(int slices, int rows, int columns)
        {
            if (this == sparse)
            {
                return new SparseDoubleMatrix3D(slices, rows, columns);
            }
            return new DenseDoubleMatrix3D(slices, rows, columns);
        }

/**
 * Constructs a matrix with the given shape, each cell initialized with the given value.
 */

        public DoubleMatrix3D make(int slices, int rows, int columns, double initialValue)
        {
            return make(slices, rows, columns).assign(initialValue);
        }

/**
 * Constructs a matrix with uniformly distributed values in <tt>(0,1)</tt> (exclusive).
 */

        public DoubleMatrix3D random(int slices, int rows, int columns)
        {
            return make(slices, rows, columns).assign(Functions.random());
        }
    }
}
