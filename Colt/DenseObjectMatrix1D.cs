#region

using System;
using HC.Core.Exceptions;

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
    //package Appendimpl;

    ////import AppendObjectMatrix1D;
    ////import ObjectMatrix2D;
    /**
    Dense 1-d matrix (aka <i>vector</i>) holding <tt>Object</tt> elements.
    First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    <p>
    <b>Implementation:</b>
    <p>
    Internally holds one single contigous one-dimensional array. 
    Note that this implementation is not lock.
    <p>
    <b>Memory requirements:</b>
    <p>
    <tt>memory [bytes] = 8*Size()</tt>.
    Thus, a 1000000 matrix uses 8 MB.
    <p>
    <b>Time complexity:</b>
    <p>
    <tt>O(1)</tt> (i.e. constant time) for the basic operations
    <tt>get</tt>, <tt>getQuick</tt>, <tt>set</tt>, <tt>setQuick</tt> and <tt>m_intSize</tt>,
    <p>
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DenseObjectMatrix1D : ObjectMatrix1D
    {
        /**
          * The elements of this matrix.
          */
        public Object[] m_elements;
        /**
         * Constructs a matrix with a copy of the given values.
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param values The values to be filled into the new matrix.
         */

        public DenseObjectMatrix1D(Object[] values)
            : this(values.Length)
        {
            assign(values);
        }

        /**
         * Constructs a matrix with a given number of cells.
         * All entries are initially <tt>0</tt>.
         * @param m_intSize the number of cells the matrix shall have.
         * @throws ArgumentException if <tt>m_intSize<0</tt>.
         */

        public DenseObjectMatrix1D(int m_intSize)
        {
            setUp(m_intSize);
            m_elements = new Object[m_intSize];
        }

        /**
         * Constructs a matrix view with the given parameters.
         * @param m_intSize the number of cells the matrix shall have.
         * @param elements the cells.
         * @param zero the index of the first element.
         * @param m_intStride the number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
         * @throws ArgumentException if <tt>m_intSize<0</tt>.
         */

        public DenseObjectMatrix1D(int m_intSize, Object[] elements, int zero, int m_intStride)
        {
            setUp(m_intSize, zero, m_intStride);
            m_elements = elements;
            isNoView = false;
        }

        /**
         * Sets all cells to the state specified by <tt>values</tt>.
         * <tt>values</tt> is required to have the same number of cells as the receiver.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param    values the values to be filled into the cells.
         * @return <tt>this</tt> (for convenience only).
         * @throws ArgumentException if <tt>values.Length != Size()</tt>.
         */

        public new ObjectMatrix1D assign(Object[] values)
        {
            if (isNoView)
            {
                if (values.Length != m_intSize)
                {
                    throw new ArgumentException("Must have same number of cells: Length=" + values.Length + "Size()=" +
                                                Size());
                }
                Array.Copy(values, 0, m_elements, 0, values.Length);
            }
            else
            {
                base.assign(values);
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[i] = function(x[i])</tt>.
        (Iterates downwards from <tt>[Size()-1]</tt> to <tt>[0]</tt>).
        <p>
        <b>Example:</b>
        <pre>
        // change each cell to its sine
        matrix =   0.5      1.5      2.5       3.5 
        matrix.assign(Functions.sin);
        -->
        matrix ==  0.479426 0.997495 0.598472 -0.350783
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param function a function object taking as argument the current cell's value.
        @return <tt>this</tt> (for convenience only).
        @see Functions
        */

        public new ObjectMatrix1D assign(ObjectFunction function)
        {
            int s = m_intStride;
            int i = index(0);
            Object[] elems = m_elements;
            if (m_elements == null)
            {
                throw new HCException();
            }

            // the general case x[i] = f(x[i])
            for (int k = m_intSize; --k >= 0;)
            {
                elems[i] = function.Apply(elems[i]);
                i += s;
            }
            return this;
        }

        /**
         * Replaces all cell values of the receiver with the values of another matrix.
         * Both matrices must have the same m_intSize.
         * If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
         *
         * @param     source   the source matrix to copy from (may be identical to the receiver).
         * @return <tt>this</tt> (for convenience only).
         * @throws	ArgumentException if <tt>Size() != other.Size()</tt>.
         */

        public new ObjectMatrix1D assign(ObjectMatrix1D source)
        {
            // overriden for performance only
            if (!(source is DenseObjectMatrix1D))
            {
                return base.assign(source);
            }
            DenseObjectMatrix1D other = (DenseObjectMatrix1D) source;
            if (other == this)
            {
                return this;
            }
            checkSize(other);
            if (isNoView && other.isNoView)
            {
                // quickest
                Array.Copy(other.m_elements, 0, m_elements, 0, m_elements.Length);
                return this;
            }
            if (haveSharedCells(other))
            {
                ObjectMatrix1D c = other.Copy();
                if (!(c is DenseObjectMatrix1D))
                {
                    // should not happen
                    return base.assign(source);
                }
                other = (DenseObjectMatrix1D) c;
            }

            Object[] elems = m_elements;
            Object[] otherElems = other.m_elements;
            if (m_elements == null || otherElems == null)
            {
                throw new HCException();
            }
            int s = m_intStride;
            int ys = other.m_intStride;

            int index_ = index(0);
            int otherIndex = other.index(0);
            for (int k = m_intSize; --k >= 0;)
            {
                elems[index_] = otherElems[otherIndex];
                index_ += s;
                otherIndex += ys;
            }
            return this;
        }

        /**
        Assigns the result of a function to each cell; <tt>x[i] = function(x[i],y[i])</tt>.
        (Iterates downwards from <tt>[Size()-1]</tt> to <tt>[0]</tt>).
        <p>
        <b>Example:</b>
        <pre>
        // assign x[i] = x[i]<sup>y[i]</sup>
        m1 = 0 1 2 3;
        m2 = 0 2 4 6;
        m1.assign(m2, Functions.pow);
        -->
        m1 == 1 1 16 729

        // for non-standard functions there is no shortcut: 
        m1.assign(m2,
        &nbsp;&nbsp;&nbsp;new ObjectObjectFunction() {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public Object Apply(Object x, Object y) { return Math.Pow(x,y); }
        &nbsp;&nbsp;&nbsp;}
        );
        </pre>
        For further examples, see the <a href="package-summary.html#FunctionObjects">//package doc</a>.

        @param y the secondary matrix to operate on.
        @param function a function object taking as first argument the current cell's value of <tt>this</tt>,
        and as second argument the current cell's value of <tt>y</tt>,
        @return <tt>this</tt> (for convenience only).
        @throws	ArgumentException if <tt>Size() != y.Size()</tt>.
        @see Functions
        */

        public new ObjectMatrix1D assign(ObjectMatrix1D y, ObjectObjectFunction function)
        {
            // overriden for performance only
            if (!(y is DenseObjectMatrix1D))
            {
                return base.assign(y, function);
            }
            DenseObjectMatrix1D other = (DenseObjectMatrix1D) y;
            checkSize(y);
            Object[] elems = m_elements;
            Object[] otherElems = other.m_elements;
            if (m_elements == null || otherElems == null)
            {
                throw new HCException();
            }
            int s = m_intStride;
            int ys = other.m_intStride;

            int index_ = index(0);
            int otherIndex = other.index(0);

            // the general case x[i] = f(x[i],y[i])		
            for (int k = m_intSize; --k >= 0;)
            {
                elems[index_] = function.Apply(elems[index_], otherElems[otherIndex]);
                index_ += s;
                otherIndex += ys;
            }
            return this;
        }

        /**
         * Returns the matrix cell value at coordinate <tt>index</tt>.
         *
         * <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>index&lt;0 || index&gt;=Size()</tt>.
         *
         * @param     index   the index of the cell.
         * @return    the value of the specified cell.
         */

        public override Object getQuick(int index)
        {
            //if (debug) if (index<0 || index>=m_intSize) checkIndex(index);
            //return elements[index(index)];
            // manually inlined:
            return m_elements[m_intZero + index*m_intStride];
        }

        /**
         * Returns <tt>true</tt> if both matrices share at least one identical cell.
         */

        public new bool haveSharedCellsRaw(ObjectMatrix1D other)
        {
            if (other is SelectedDenseObjectMatrix1D)
            {
                SelectedDenseObjectMatrix1D otherMatrix = (SelectedDenseObjectMatrix1D) other;
                return m_elements == otherMatrix.m_elements;
            }
            else if (other is DenseObjectMatrix1D)
            {
                DenseObjectMatrix1D otherMatrix = (DenseObjectMatrix1D) other;
                return m_elements == otherMatrix.m_elements;
            }
            return false;
        }

        /**
         * Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
         * You may want to override this method for performance.
         *
         * @param     rank   the rank of the element.
         */

        public new int index(int rank)
        {
            // overriden for manual inlining only
            //return _offset(_rank(rank));
            return m_intZero + rank*m_intStride;
        }

        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified m_intSize.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix1D</tt> the new matrix must also be of type <tt>DenseObjectMatrix1D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix1D</tt> the new matrix must also be of type <tt>SparseObjectMatrix1D</tt>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param m_intSize the number of cell the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */

        public override ObjectMatrix1D like(int m_intSize)
        {
            return new DenseObjectMatrix1D(m_intSize);
        }

        /**
         * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
         * For example, if the receiver is an instance of type <tt>DenseObjectMatrix1D</tt> the new matrix must be of type <tt>DenseObjectMatrix2D</tt>,
         * if the receiver is an instance of type <tt>SparseObjectMatrix1D</tt> the new matrix must be of type <tt>SparseObjectMatrix2D</tt>, etc.
         *
         * @param rows the number of rows the matrix shall have.
         * @param columns the number of columns the matrix shall have.
         * @return  a new matrix of the corresponding dynamic type.
         */

        public override ObjectMatrix2D like2D(int rows, int columns)
        {
            return new DenseObjectMatrix2D(rows, columns);
        }

        /**
         * Sets the matrix cell at coordinate <tt>index</tt> to the specified value.
         *
         * <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <tt>index&lt;0 || index&gt;=Size()</tt>.
         *
         * @param     index   the index of the cell.
         * @param    value the value to be filled into the specified cell.
         */

        public override void setQuick(int index, Object value)
        {
            //if (debug) if (index<0 || index>=m_intSize) checkIndex(index);
            //elements[index(index)] = value;
            // manually inlined:
            m_elements[m_intZero + index*m_intStride] = value;
        }

        /**
        Swaps each element <tt>this[i]</tt> with <tt>other[i]</tt>.
        @throws ArgumentException if <tt>Size() != other.Size()</tt>.
        */

        public new void swap(ObjectMatrix1D other)
        {
            // overriden for performance only
            if (!(other is DenseObjectMatrix1D))
            {
                base.swap(other);
            }
            DenseObjectMatrix1D y = (DenseObjectMatrix1D) other;
            if (y == this)
            {
                return;
            }
            checkSize(y);

            Object[] elems = m_elements;
            Object[] otherElems = y.m_elements;
            if (m_elements == null || otherElems == null)
            {
                throw new HCException();
            }
            int s = m_intStride;
            int ys = y.m_intStride;

            int index2 = index(0);
            int otherIndex = y.index(0);
            for (int k = m_intSize; --k >= 0;)
            {
                Object tmp = elems[index2];
                elems[index2] = otherElems[otherIndex];
                otherElems[otherIndex] = tmp;
                index2 += s;
                otherIndex += ys;
            }
            return;
        }

        /**
        Fills the cell values into the specified 1-dimensional array.
        The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        After this call returns the array <tt>values</tt> has the form 
        <br>
        <tt>for (int i=0; i < Size(); i++) values[i] = get(i);</tt>

        @throws ArgumentException if <tt>values.Length < Size()</tt>.
        */

        public new void ToArray(Object[] values)
        {
            if (values.Length < m_intSize)
            {
                throw new ArgumentException("values too small");
            }
            if (isNoView)
            {
                Array.Copy(m_elements, 0, values, 0, m_elements.Length);
            }
            else
            {
                base.ToArray(values);
            }
        }

        /**
         * Construct and returns a new selection view.
         *
         * @param offsets the offsets of the visible elements.
         * @return  a new view.
         */

        public override ObjectMatrix1D viewSelectionLike(int[] offsets)
        {
            return new SelectedDenseObjectMatrix1D(
                m_elements,
                offsets);
        }
    }
}
