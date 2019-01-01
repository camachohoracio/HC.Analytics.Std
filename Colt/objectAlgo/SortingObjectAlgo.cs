#region

using System;
using System.Collections;
using HC.Analytics.Colt.CustomImplementations;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt.objectAlgo
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package matrix.objectalgo;

    //import function.IntComparator;
    //import matrix.ObjectMatrix1D;
    //import matrix.ObjectMatrix2D;
    //import matrix.ObjectMatrix3D;
    /**
    Matrix quicksorts and mergesorts.
    Use idioms like <tt>SortingObjectAlgo.quickSort.Sort(...)</tt> and <tt>SortingObjectAlgo.mergeSort.Sort(...)</tt>.
    <p>
    This is another case demonstrating one primary goal of this library: Delivering easy to use, yet very efficient APIs.
    The sorts return convenient <i>sort views</i>.
    This enables the usage of algorithms which scale well with the problem size:
    For example, sorting a 1000000 x 10000 or a 1000000 x 100 x 100 matrix performs just as fast as sorting a 1000000 x 1 matrix.
    This is so, because internally the algorithms only move around integer indexes, they do not physically move around entire rows or slices.
    The original matrix is left unaffected.
    <p>
    The quicksort is a derivative of the JDK 1.2 V1.26 algorithms (which are, in turn, based on Bentley's and McIlroy's fine work).
    The mergesort is a derivative of the JAL algorithms, with optimisations taken from the JDK algorithms.
    Mergesort is <i>stable</i> (by definition), while quicksort is not.
    A stable sort is, for example, helpful, if matrices are sorted successively 
    by multiple columns. It preserves the relative position of equal elements.
 
    @see GenericSorting
    @see SortingObjectAlgo
    @see java.util.Arrays

    @author wolfgang.hoschek@cern.ch
    @version 1.1, 25/May/2000
    */

    [Serializable]
    public class SortingObjectAlgo : PersistentObject
    {
        /**
         * A prefabricated quicksort.
         */

        /**
         * A prefabricated mergesort.
         */
        public static SortingObjectAlgo mergeSort = new SortingObjectAlgo_();
        public static SortingObjectAlgo quickSort = new SortingObjectAlgo(); // already has quicksort implemented
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */

        public void runSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            Sorting.quickSort(a, fromIndex, toIndex, c);
        }

        public void runSort(int fromIndex, int toIndex, IntComparator c, Swapper swapper)
        {
            GenericSorting.quickSort(fromIndex, toIndex, c, swapper);
        }

        /**
        Sorts the vector into ascending order, according to the <i>natural ordering</i>.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To sort ranges use sub-ranging views. To sort descending, use flip views ...
        <p>
        <b>Example:</b> 
        <table border="1" cellspacing="0">
          <tr nowrap> 
            <td valign="top"><tt> 7, 1, 3, 1<br>
              </tt></td>
            <td valign="top"> 
              <p><tt> ==&gt; 1, 1, 3, 7<br>
                The vector IS NOT SORTED.<br>
                The new VIEW IS SORTED.</tt></p>
            </td>
          </tr>
        </table>

        @param vector the vector to be sorted.
        @return a new sorted vector (matrix) view. 
                <b>Note that the original matrix is left unaffected.</b>
        */

        public ObjectMatrix1D Sort(ObjectMatrix1D vector)
        {
            int[] indexes = new int[vector.Size()]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;)
            {
                indexes[i] = i;
            }

            IntComparator comp = new IntComparator21_(vector);

            runSort(indexes, 0, indexes.Length, comp);

            return vector.viewSelection(indexes);
        }

        /**
        Sorts the vector into ascending order, according to the order induced by the specified comparator.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        The algorithm compares two cells at a time, determinining whether one is smaller, equal or larger than the other.
        To sort ranges use sub-ranging views. To sort descending, use flip views ...
        <p>
        <b>Example:</b>
        <pre>
        // sort by sinus of cells
        ObjectComparator comp = new ObjectComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(Object a, Object b) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Object as = Math.sin(a); Object bs = Math.sin(b);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return as < bs ? -1 : as == bs ? 0 : 1;
        &nbsp;&nbsp;&nbsp;}
        };
        sorted = quickSort(vector,comp);
        </pre>

        @param vector the vector to be sorted.
        @param c the comparator to determine the order.
        @return a new matrix view sorted as specified.
                <b>Note that the original vector (matrix) is left unaffected.</b>
        */

        public ObjectMatrix1D Sort(ObjectMatrix1D vector, IComparer c)
        {
            int[] indexes = new int[vector.Size()]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;)
            {
                indexes[i] = i;
            }

            IntComparator comp = new IntComparator22_(vector, c);

            runSort(indexes, 0, indexes.Length, comp);

            return vector.viewSelection(indexes);
        }

        /**
        Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the given column.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To sort ranges use sub-ranging views. To sort columns by rows, use dice views. To sort descending, use flip views ...
        <p>
        <b>Example:</b> 
        <table border="1" cellspacing="0">
          <tr nowrap> 
            <td valign="top"><tt>4 x 2 matrix: <br>
              7, 6<br>
              5, 4<br>
              3, 2<br>
              1, 0 <br>
              </tt></td>
            <td align="left" valign="top"> 
              <p><tt>column = 0;<br>
                view = quickSort(matrix,column);<br>
                System.out.println(view); </tt><tt><br>
                ==> </tt></p>
              </td>
            <td valign="top"> 
              <p><tt>4 x 2 matrix:<br>
                1, 0<br>
                3, 2<br>
                5, 4<br>
                7, 6</tt><br>
                The matrix IS NOT SORTED.<br>
                The new VIEW IS SORTED.</p>
              </td>
          </tr>
        </table>

        @param matrix the matrix to be sorted.
        @param column the index of the column inducing the order.
        @return a new matrix view having rows sorted by the given column.
                <b>Note that the original matrix is left unaffected.</b>
        @throws HCException if <tt>column < 0 || column >= matrix.Columns()</tt>.
        */

        public ObjectMatrix2D Sort(ObjectMatrix2D matrix, int column)
        {
            if (column < 0 || column >= matrix.Columns())
            {
                throw new HCException("column=" + column + ", matrix=" + AbstractFormatter.shape(matrix));
            }

            int[] rowIndexes = new int[matrix.Rows()]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowIndexes[i] = i;
            }

            ObjectMatrix1D col = matrix.viewColumn(column);
            IntComparator comp = new IntComparator23_(col);

            runSort(rowIndexes, 0, rowIndexes.Length, comp);

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.viewSelection(rowIndexes, null);
        }

        /**
        Sorts the matrix rows according to the order induced by the specified comparator.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        The algorithm compares two rows (1-d matrices) at a time, determinining whether one is smaller, equal or larger than the other.
        To sort ranges use sub-ranging views. To sort columns by rows, use dice views. To sort descending, use flip views ...
        <p>
        <b>Example:</b>
        <pre>
        // sort by sum of values in a row
        ObjectMatrix1DComparator comp = new ObjectMatrix1DComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(ObjectMatrix1D a, ObjectMatrix1D b) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Object as = a.zSum(); Object bs = b.zSum();
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return as < bs ? -1 : as == bs ? 0 : 1;
        &nbsp;&nbsp;&nbsp;}
        };
        sorted = quickSort(matrix,comp);
        </pre>

        @param matrix the matrix to be sorted.
        @param c the comparator to determine the order.
        @return a new matrix view having rows sorted as specified.
                <b>Note that the original matrix is left unaffected.</b>
        */

        public ObjectMatrix2D Sort(ObjectMatrix2D matrix, ObjectMatrix1DComparator c)
        {
            int[] rowIndexes = new int[matrix.Rows()]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowIndexes[i] = i;
            }

            ObjectMatrix1D[] views = new ObjectMatrix1D[matrix.Rows()]; // precompute views for speed
            for (int i = views.Length; --i >= 0;)
            {
                views[i] = matrix.viewRow(i);
            }

            IntComparator comp = new IntComparator24_(views, c);

            runSort(rowIndexes, 0, rowIndexes.Length, comp);

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.viewSelection(rowIndexes, null);
        }

        /**
        Sorts the matrix slices into ascending order, according to the <i>natural ordering</i> of the matrix values in the given <tt>[row,column]</tt> position.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To sort ranges use sub-ranging views. To sort by other dimensions, use dice views. To sort descending, use flip views ...
        <p>
        The algorithm compares two 2-d slices at a time, determinining whether one is smaller, equal or larger than the other.
        Comparison is based on the cell <tt>[row,column]</tt> within a slice.
        Let <tt>A</tt> and <tt>B</tt> be two 2-d slices. Then we have the following rules
        <ul>
        <li><tt>A &lt;  B  iff A.get(row,column) &lt;  B.get(row,column)</tt>
        <li><tt>A == B iff A.get(row,column) == B.get(row,column)</tt>
        <li><tt>A &gt;  B  iff A.get(row,column) &gt;  B.get(row,column)</tt>
        </ul>

        @param matrix the matrix to be sorted.
        @param row the index of the row inducing the order.
        @param column the index of the column inducing the order.
        @return a new matrix view having slices sorted by the values of the slice view <tt>matrix.viewRow(row).viewColumn(column)</tt>.
                <b>Note that the original matrix is left unaffected.</b>
        @throws HCException if <tt>row < 0 || row >= matrix.Rows() || column < 0 || column >= matrix.Columns()</tt>.
        */

        public ObjectMatrix3D Sort(ObjectMatrix3D matrix, int row, int column)
        {
            if (row < 0 || row >= matrix.Rows())
            {
                throw new HCException("row=" + row + ", matrix=" + AbstractFormatter.shape(matrix));
            }
            if (column < 0 || column >= matrix.Columns())
            {
                throw new HCException("column=" + column + ", matrix=" + AbstractFormatter.shape(matrix));
            }

            int[] sliceIndexes = new int[matrix.Slices()]; // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;)
            {
                sliceIndexes[i] = i;
            }

            ObjectMatrix1D sliceView = matrix.viewRow(row).viewColumn(column);
            IntComparator comp = new IntComparator25_(sliceView);

            runSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.viewSelection(sliceIndexes, null, null);
        }

        /**
        Sorts the matrix slices according to the order induced by the specified comparator.
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        The algorithm compares two slices (2-d matrices) at a time, determinining whether one is smaller, equal or larger than the other.
        To sort ranges use sub-ranging views. To sort by other dimensions, use dice views. To sort descending, use flip views ...
        <p>
        <b>Example:</b>
        <pre>
        // sort by sum of values in a slice
        ObjectMatrix2DComparator comp = new ObjectMatrix2DComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(ObjectMatrix2D a, ObjectMatrix2D b) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Object as = a.zSum(); Object bs = b.zSum();
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return as < bs ? -1 : as == bs ? 0 : 1;
        &nbsp;&nbsp;&nbsp;}
        };
        sorted = quickSort(matrix,comp);
        </pre>

        @param matrix the matrix to be sorted.
        @param c the comparator to determine the order.
        @return a new matrix view having slices sorted as specified.
                <b>Note that the original matrix is left unaffected.</b>
        */

        public ObjectMatrix3D Sort(ObjectMatrix3D matrix, ObjectMatrix2DComparator c)
        {
            int[] sliceIndexes = new int[matrix.Slices()]; // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;)
            {
                sliceIndexes[i] = i;
            }

            ObjectMatrix2D[] views = new ObjectMatrix2D[matrix.Slices()]; // precompute views for speed
            for (int i = views.Length; --i >= 0;)
            {
                views[i] = matrix.viewSlice(i);
            }

            IntComparator comp = new IntComparator26_(c, views);

            runSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.viewSelection(sliceIndexes, null, null);
        }
    }
}
