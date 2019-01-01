#region

using System;
using HC.Analytics.Probability.Random;

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

////import DenseDoubleMatrix1D;
////import SparseDoubleMatrix1D;
/**
Factory for convenient construction of 1-d matrices holding <tt>double</tt> cells.
Use idioms like <tt>DoubleFactory1D.dense.make(1000)</tt> to construct dense matrices, 
<tt>DoubleFactory1D.sparse.make(1000)</tt> to construct sparse matrices.

If the factory is used frequently it might be useful to streamline the notation. 
For example by aliasing:
<table>
<td class="PRE"> 
<pre>
DoubleFactory1D F = DoubleFactory1D.dense;
Functions.make(1000);
Functions.descending(10);
Functions.random(3);
...
</pre>
</td>
</table>

@author wolfgang.hoschek@cern.ch
@version 1.0, 09/24/99
*/

    [Serializable]
    public class DoubleFactory1D : PersistentObject
    {
        /**
	 * A factory producing dense matrices.
	 */
        public static DoubleFactory1D dense = new DoubleFactory1D();

        /**
	 * A factory producing sparse matrices.
	 */
        public static DoubleFactory1D sparse = new DoubleFactory1D();
/**
 * Makes this class non instantiable, but still let's others inherit from it.
 */
/**
C = A||B; Constructs a new matrix which is the concatenation of two other matrices.
Example: <tt>0 1</tt> Append <tt>3 4</tt> --> <tt>0 1 3 4</tt>.
*/

        public DoubleMatrix1D Append(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            // concatenate
            DoubleMatrix1D matrix = make(A.Size() + B.Size());
            matrix.viewPart(0, A.Size()).assign(A);
            matrix.viewPart(A.Size(), B.Size()).assign(B);
            return matrix;
        }

/**
Constructs a matrix with cells having ascending values.
For debugging purposes.
Example: <tt>0 1 2</tt>
*/

        public DoubleMatrix1D ascending(int size)
        {
            Functions F = Functions.m_functions;
            return descending(size).assign(Functions.chain(Functions.m_neg, Functions.minus(size)));
        }

/**
Constructs a matrix with cells having descending values.
For debugging purposes.
Example: <tt>2 1 0</tt> 
*/

        public DoubleMatrix1D descending(int size)
        {
            DoubleMatrix1D matrix = make(size);
            int v = 0;
            for (int i = size; --i >= 0;)
            {
                matrix.setQuick(i, v++);
            }
            return matrix;
        }

/**
 * Constructs a matrix with the given cell values.
 * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
 *
 * @param values The values to be filled into the new matrix.
 */

        public DoubleMatrix1D make(double[] values)
        {
            if (this == sparse)
            {
                return new SparseDoubleMatrix1D(values);
            }
            else
            {
                return new DenseDoubleMatrix1D(values);
            }
        }

/**
Constructs a matrix which is the concatenation of all given parts.
Cells are copied.
*/

        public DoubleMatrix1D make(DoubleMatrix1D[] parts)
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

            DoubleMatrix1D vector = make(size);
            size = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                vector.viewPart(size, parts[i].Size()).assign(parts[i]);
                size += parts[i].Size();
            }

            return vector;
        }

/**
 * Constructs a matrix with the given shape, each cell initialized with zero.
 */

        public DoubleMatrix1D make(int size)
        {
            if (this == sparse)
            {
                return new SparseDoubleMatrix1D(size);
            }
            return new DenseDoubleMatrix1D(size);
        }

/**
 * Constructs a matrix with the given shape, each cell initialized with the given value.
 */

        public DoubleMatrix1D make(int size, double initialValue)
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

        public DoubleMatrix1D make(
            AbstractDoubleList values)
        {
            int size = values.Size();
            DoubleMatrix1D vector = make(size);
            for (int i = size; --i >= 0;)
            {
                vector.set(i, values.get(i));
            }
            return vector;
        }

/**
 * Constructs a matrix with uniformly distributed values in <tt>(0,1)</tt> (exclusive).
 */

        public DoubleMatrix1D random(int size)
        {
            return make(size).assign(Functions.random());
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

        public DoubleMatrix1D repeat(DoubleMatrix1D A, int repeat)
        {
            int size = A.Size();
            DoubleMatrix1D matrix = make(repeat*size);
            for (int i = repeat; --i >= 0;)
            {
                matrix.viewPart(size*i, size).assign(A);
            }
            return matrix;
        }

/**
 * Constructs a randomly sampled matrix with the given shape.
 * Randomly picks exactly <tt>Math.Round(size*nonZeroFraction)</tt> cells and initializes them to <tt>value</tt>, all the rest will be initialized to zero.
 * Note that this is not the same as setting each cell with probability <tt>nonZeroFraction</tt> to <tt>value</tt>.
 * @throws ArgumentException if <tt>nonZeroFraction < 0 || nonZeroFraction > 1</tt>.
 * @see RandomSampler
 */

        public DoubleMatrix1D sample(int size, double value, double nonZeroFraction)
        {
            double epsilon = 1e-09;
            if (nonZeroFraction < 0 - epsilon || nonZeroFraction > 1 + epsilon)
            {
                throw new ArgumentException();
            }
            if (nonZeroFraction < 0)
            {
                nonZeroFraction = 0;
            }
            if (nonZeroFraction > 1)
            {
                nonZeroFraction = 1;
            }

            DoubleMatrix1D matrix = make(size);

            int n = (int) Math.Round(size*nonZeroFraction);
            if (n == 0)
            {
                return matrix;
            }

            RandomSamplingAssistant sampler =
                new RandomSamplingAssistant(
                    n,
                    size,
                    new RngWrapper());
            for (int i = size; --i >= 0;)
            {
                if (sampler.sampleNextElement())
                {
                    matrix.set(i, value);
                }
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

        public DoubleArrayList toList(DoubleMatrix1D values)
        {
            int size = values.Size();
            DoubleArrayList list = new DoubleArrayList(size);
            list.setSize(size);
            for (int i = size; --i >= 0;)
            {
                list.set(i, values.get(i));
            }
            return list;
        }
    }
}
