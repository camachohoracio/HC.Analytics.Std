#region

using System;
using HC.Analytics.Colt.CustomImplementations;
using HC.Core.Exceptions;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt.doubleAlgo
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package matrix.doublealgo;

    //import IntComparator;
    //import DoubleFactory2D;
    //import matrix.DoubleFactory3D;
    //import matrix.DoubleMatrix1D;
    //import matrix.DoubleMatrix2D;
    //import matrix.DoubleMatrix3D;
    //import matrix.impl.DenseDoubleMatrix1D;
    /**
    Matrix quicksorts and mergesorts.
    Use idioms like <tt>Sorting.quickSort.Sort(...)</tt> and <tt>Sorting.mergeSort.Sort(...)</tt>.
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
    @see Sorting
    @see java.util.Arrays

    @author wolfgang.hoschek@cern.ch
    @version 1.1, 25/May/2000
    */

    [Serializable]
    public class SortingDoubleAlgo : PersistentObject
    {
        /**
         * A prefabricated quicksort.
         */

        /**
         * A prefabricated mergesort.
         */
        public static SortingDoubleAlgo mergeSort = new SortingMergeSort();
        public static SortingDoubleAlgo quickSort = new SortingDoubleAlgo(); // already has quicksort implemented
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Compare two values, one of which is assumed to be double.NaN
         */

        public static int compareNaN(double a, double b)
        {
            if (double.IsNaN(a) && double.IsNaN(b))
            {
                return 0; // NaN equals NaN
            }
            else if (double.IsNaN(a) && !double.IsNaN(b))
            {
                return 1; // e.g. NaN > 5
            }
            return -1; // e.g. 5 < NaN
        }

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

        public DoubleMatrix1D Sort(DoubleMatrix1D vector)
        {
            int[] indexes = new int[vector.Size()]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;)
            {
                indexes[i] = i;
            }

            IntComparator comp = new IntComparator14_(
                vector,
                this);

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
        DoubleComparator comp = new DoubleComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(double a, double b) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double as = Math.Sin(a); double bs = Math.Sin(b);
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

        public DoubleMatrix1D Sort(DoubleMatrix1D vector, DoubleComparator c)
        {
            int[] indexes = new int[vector.Size()]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;)
            {
                indexes[i] = i;
            }

            IntComparator comp = new IntComparator14_(vector, this);

            runSort(indexes, 0, indexes.Length, comp);

            return vector.viewSelection(indexes);
        }

        /**
        Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the virtual column <tt>aggregates</tt>;
        Particularly efficient when comparing expensive aggregates, because aggregates need not be recomputed time and again, as is the case for comparator based sorts.
        Essentially, this algorithm makes expensive comparisons cheap.
        Normally each element of <tt>aggregates</tt> is a summary measure of a row.
        Speedup over comparator based sorting = <tt>2*log(rows)</tt>, on average.
        For this operation, quicksort is usually faster.
        <p>
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To sort ranges use sub-ranging views. To sort columns by rows, use dice views. To sort descending, use flip views ...
        <p>
        <b>Example:</b>
        Each aggregate is the sum of a row
        <table border="1" cellspacing="0">
          <tr nowrap> 
            <td valign="top"><tt>4 x 2 matrix: <br>
              1, 1<br>
              5, 4<br>
              3, 0<br>
              4, 4 <br>
              </tt></td>
            <td align="left" valign="top"> 
              <tt>aggregates=<br>
              2<br>
              9<br>
              3<br>
              8<br>
              ==></tt></td>
            <td valign="top"> 
              <p><tt>4 x 2 matrix:<br>
                1, 1<br>
                3, 0<br>
                4, 4<br>
                5, 4</tt><br>
                The matrix IS NOT SORTED.<br>
                The new VIEW IS SORTED.</p>
              </td>
          </tr>
        </table>

        <table>
        <td class="PRE"> 
        <pre>
        // sort 10000 x 1000 matrix by sum of logarithms in a row (i.e. by geometric mean)
        DoubleMatrix2D matrix = new DenseDoubleMatrix2D(10000,1000);
        matrix.assign(new engine.MersenneTwister()); // initialized randomly
        Functions F = Functions.functions; // alias for convenience

        // THE QUICK VERSION (takes some 3 secs)
        // aggregates[i] = Sum(log(row));
        double[] aggregates = new double[matrix.Rows()];
        for (int i = matrix.Rows(); --i >= 0; ) aggregates[i] = matrix.viewRow(i).aggregate(Functions.plus, Functions.log);
        DoubleMatrix2D sorted = quickSort(matrix,aggregates);

        // THE SLOW VERSION (takes some 90 secs)
        DoubleMatrix1DComparator comparator = new DoubleMatrix1DComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(DoubleMatrix1D x, DoubleMatrix1D y) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a = x.aggregate(Functions.plus,Functions.log);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double b = y.aggregate(Functions.plus,Functions.log);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return a < b ? -1 : a==b ? 0 : 1;
        &nbsp;&nbsp;&nbsp;}
        };
        DoubleMatrix2D sorted = quickSort(matrix,comparator);
        </pre>
        </td>
        </table>

        @param matrix the matrix to be sorted.
        @param aggregates the values to sort on. (As a side effect, this array will also get sorted).
        @return a new matrix view having rows sorted.
                <b>Note that the original matrix is left unaffected.</b>
        @throws HCException if <tt>aggregates.Length != matrix.Rows()</tt>.
        */

        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, double[] aggregates)
        {
            int rows = matrix.Rows();

            if (aggregates.Length != rows)
            {
                throw new HCException("aggregates.Length != matrix.Rows()");
            }

            // set up index reordering
            int[] indexes = new int[rows];
            for (int i = rows; --i >= 0;)
            {
                indexes[i] = i;
            }

            // compares two aggregates at a time
            IntComparator comp = new IntComparator16_(aggregates, this);

            // swaps aggregates and reorders indexes
            Swapper swapper = new Swapper12_(indexes, aggregates);

            // sort indexes and aggregates
            runSort(0, rows, comp, swapper);

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.viewSelection(indexes, null);
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
                PrintToScreen.WriteLine(view); </tt><tt><br>
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

        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, int column)
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

            DoubleMatrix1D col = matrix.viewColumn(column);
            IntComparator comp = new IntComparator17_(col);

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
        DoubleMatrix1DComparator comp = new DoubleMatrix1DComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(DoubleMatrix1D a, DoubleMatrix1D b) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double as = a.zSum(); double bs = b.zSum();
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

        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, DoubleMatrix1DComparator c)
        {
            int[] rowIndexes = new int[matrix.Rows()]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowIndexes[i] = i;
            }

            DoubleMatrix1D[] views = new DoubleMatrix1D[matrix.Rows()]; // precompute views for speed
            for (int i = views.Length; --i >= 0;)
            {
                views[i] = matrix.viewRow(i);
            }

            IntComparator comp = new IntComparator18_(c, views);

            runSort(rowIndexes, 0, rowIndexes.Length, comp);

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.viewSelection(rowIndexes, null);
        }

        /**
        Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the values computed by applying the given aggregation function to each row;
        Particularly efficient when comparing expensive aggregates, because aggregates need not be recomputed time and again, as is the case for comparator based sorts.
        Essentially, this algorithm makes expensive comparisons cheap.
        Normally <tt>aggregates</tt> defines a summary measure of a row.
        Speedup over comparator based sorting = <tt>2*log(rows)</tt>, on average.
        <p>
        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        To sort ranges use sub-ranging views. To sort columns by rows, use dice views. To sort descending, use flip views ...
        <p>
        <b>Example:</b>
        Each aggregate is the sum of a row
        <table border="1" cellspacing="0">
          <tr nowrap> 
            <td valign="top"><tt>4 x 2 matrix: <br>
              1, 1<br>
              5, 4<br>
              3, 0<br>
              4, 4 <br>
              </tt></td>
            <td align="left" valign="top"> 
              <tt>aggregates=<br>
              BinFunctions1D.sum<br>
              ==></tt></td>
            <td valign="top"> 
              <p><tt>4 x 2 matrix:<br>
                1, 1<br>
                3, 0<br>
                4, 4<br>
                5, 4</tt><br>
                The matrix IS NOT SORTED.<br>
                The new VIEW IS SORTED.</p>
              </td>
          </tr>
        </table>

        <table>
        <td class="PRE"> 
        <pre>
        // sort 10000 x 1000 matrix by median or by sum of logarithms in a row (i.e. by geometric mean)
        DoubleMatrix2D matrix = new DenseDoubleMatrix2D(10000,1000);
        matrix.assign(new engine.MersenneTwister()); // initialized randomly
        Functions F = Functions.functions; // alias for convenience

        // THE QUICK VERSION (takes some 10 secs)
        DoubleMatrix2D sorted = quickSort(matrix,BinFunctions1D.median);
        //DoubleMatrix2D sorted = quickSort(matrix,BinFunctions1D.sumOfLogarithms);

        // THE SLOW VERSION (takes some 300 secs)
        DoubleMatrix1DComparator comparator = new DoubleMatrix1DComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(DoubleMatrix1D x, DoubleMatrix1D y) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a = matrix.Statistic.bin(x).median();
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double b = matrix.Statistic.bin(y).median();
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// double a = x.aggregate(Functions.plus,Functions.log);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// double b = y.aggregate(Functions.plus,Functions.log);
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return a < b ? -1 : a==b ? 0 : 1;
        &nbsp;&nbsp;&nbsp;}
        };
        DoubleMatrix2D sorted = quickSort(matrix,comparator);
        </pre>
        </td>
        </table>

        @param matrix the matrix to be sorted.
        @param aggregate the function to sort on; aggregates values in a row.
        @return a new matrix view having rows sorted.
                <b>Note that the original matrix is left unaffected.</b>
        */

        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, BinFunction1D aggregate)
        {
            // precompute aggregates over rows, as defined by "aggregate"

            // a bit clumsy, because Statistic.aggregate(...) is defined on columns, so we need to transpose views
            DoubleMatrix2D tmp = matrix.like(1, matrix.Rows());
            BinFunction1D[] func = {aggregate};
            Statistic.aggregate(
                matrix.viewDice(),
                func,
                tmp);
            double[] aggr = tmp.viewRow(0).ToArray();
            return Sort(matrix, aggr);
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

        public DoubleMatrix3D Sort(DoubleMatrix3D matrix, int row, int column)
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

            DoubleMatrix1D sliceView = matrix.viewRow(row).viewColumn(column);
            IntComparator comp = new IntComparator19_(sliceView);

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
        DoubleMatrix2DComparator comp = new DoubleMatrix2DComparator() {
        &nbsp;&nbsp;&nbsp;public int Compare(DoubleMatrix2D a, DoubleMatrix2D b) {
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double as = a.zSum(); double bs = b.zSum();
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

        public DoubleMatrix3D Sort(DoubleMatrix3D matrix, DoubleMatrix2DComparator c)
        {
            int[] sliceIndexes = new int[matrix.Slices()]; // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;)
            {
                sliceIndexes[i] = i;
            }

            DoubleMatrix2D[] views = new DoubleMatrix2D[matrix.Slices()]; // precompute views for speed
            for (int i = views.Length; --i >= 0;)
            {
                views[i] = matrix.viewSlice(i);
            }

            IntComparator comp = new IntComparator20_(c, views);

            runSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.viewSelection(sliceIndexes, null, null);
        }

        /**
         * Demonstrates advanced sorting.
         * Sorts by sum of row.
         */

        public static void zdemo1()
        {
            SortingDoubleAlgo sort = quickSort;
            DoubleMatrix2D matrix = DoubleFactory2D.dense.descending(4, 3);

            DoubleMatrix1DComparator comp = new DoubleMatrix1DComparator_();

            PrintToScreen.WriteLine("unsorted:" + matrix);
            PrintToScreen.WriteLine("sorted  :" + sort.Sort(matrix, comp));
        }

        /**
         * Demonstrates advanced sorting.
         * Sorts by sum of slice.
         */

        public static void zdemo2()
        {
            SortingDoubleAlgo sort = quickSort;
            DoubleMatrix3D matrix = DoubleFactory3D.dense.descending(4, 3, 2);
            DoubleMatrix2DComparator comp = new DoubleMatrix2DComparator2_();

            PrintToScreen.WriteLine("unsorted:" + matrix);
            PrintToScreen.WriteLine("sorted  :" + sort.Sort(matrix, comp));
        }

        /**
         * Demonstrates advanced sorting.
         * Sorts by sinus of cell values.
         */

        public static void zdemo3()
        {
            SortingDoubleAlgo sort = quickSort;
            double[] values = {0.5, 1.5, 2.5, 3.5};
            DoubleMatrix1D matrix = new DenseDoubleMatrix1D(values);
            DoubleComparator comp = new DoubleComparator_();
            PrintToScreen.WriteLine("unsorted:" + matrix);

            DoubleMatrix1D sorted = sort.Sort(matrix, comp);
            PrintToScreen.WriteLine("sorted  :" + sorted);

            // check whether it is really sorted
            sorted.assign(Functions.m_sin);
            /*
            sorted.assign(
                new function.DoubleFunction() {
                    public double Apply(double arg) { return Math.Sin(arg); }
                }
            );
            */
            PrintToScreen.WriteLine("sined  :" + sorted);
        }

        /**
         * Demonstrates applying functions.
         */

        public static void zdemo4()
        {
            double[] values1 = {0, 1, 2, 3};
            double[] values2 = {0, 2, 4, 6};
            DoubleMatrix1D matrix1 = new DenseDoubleMatrix1D(values1);
            DoubleMatrix1D matrix2 = new DenseDoubleMatrix1D(values2);
            PrintToScreen.WriteLine("m1:" + matrix1);
            PrintToScreen.WriteLine("m2:" + matrix2);

            matrix1.assign(
                matrix2,
                Functions.m_pow);

            /*
            matrix1.assign(matrix2,
                new function.DoubleDoubleFunction() {
                    public double Apply(double x, double y) { return Math.pow(x,y); }
                }
            );
            */

            PrintToScreen.WriteLine("applied:" + matrix1);
        }

        /**
         * Demonstrates sorting with precomputation of aggregates (median and sum of logarithms).
         */

        public static void zdemo5(int rows, int columns, bool print)
        {
            SortingDoubleAlgo sort = quickSort;
            // for reliable benchmarks, call this method twice: once with small dummy parameters to "warm up" the jitter, then with your real work-load

            PrintToScreen.WriteLine("\n\n");
            Console.Write("now initializing... ");
            Timer timer = new Timer().start();

            Functions F = Functions.m_functions;
            DoubleMatrix2D A = DoubleFactory2D.dense.make(rows, columns);
            A.assign(
                new DRand()); // initialize randomly
            timer.stop().display();

            // also benchmark copying in its several implementation flavours
            DoubleMatrix2D B = A.like();
            timer.reset().start();
            Console.Write("now copying... ");
            B.assign(A);
            timer.stop().display();

            timer.reset().start();
            Console.Write("now copying subrange... ");
            B.viewPart(0, 0, rows, columns).assign(A.viewPart(0, 0, rows, columns));
            timer.stop().display();
            //PrintToScreen.WriteLine(A);

            timer.reset().start();
            Console.Write("now copying selected... ");
            B.viewSelection(null, null).assign(A.viewSelection(null, null));
            timer.stop().display();

            Console.Write("now sorting - quick version with precomputation... ");
            timer.reset().start();
            // THE QUICK VERSION (takes some 10 secs)
            A = sort.Sort(A, BinFunctions1D.median);
            //A = sort.Sort(A,BinFunctions1D.sumLog);
            timer.stop().display();

            // check results for correctness
            // WARNING: be sure NOT TO PRINT huge matrices unless you have tons of main memory and time!!
            // so we just show the first 5 rows
            if (print)
            {
                int r = Math.Min(rows, 5);
                BinFunction1D[] funs = {BinFunctions1D.median, BinFunctions1D.sumLog, BinFunctions1D.geometricMean};
                string[] rowNames = new string[r];
                string[] columnNames = new string[columns];
                for (int i = columns; --i >= 0;)
                {
                    columnNames[i] = (i.ToString());
                }
                for (int i = r; --i >= 0;)
                {
                    rowNames[i] = (i.ToString());
                }
                PrintToScreen.WriteLine("first part of sorted result = \n" +
                                  (new FormatterDoubleAlgo(@"%G")).toTitleString(
                                      A.viewPart(0, 0, r, columns),
                                      rowNames,
                                      columnNames,
                                      null,
                                      null,
                                      null,
                                      funs));
            }


            Console.Write("now sorting - slow version... ");
            A = B;
            DoubleMatrix1DComparator fun = new DoubleMatrix1DComparator2_();

            timer.reset().start();
            A = sort.Sort(A, fun);
            timer.stop().display();
        }

        /**
         * Demonstrates advanced sorting.
         * Sorts by sum of row.
         */

        public static void zdemo6()
        {
            SortingDoubleAlgo sort = quickSort;
            double[,] values = {
                                   {3, 7, 0},
                                   {2, 1, 0},
                                   {2, 2, 0},
                                   {1, 8, 0},
                                   {2, 5, 0},
                                   {7, 0, 0},
                                   {2, 3, 0},
                                   {1, 0, 0},
                                   {4, 0, 0},
                                   {2, 0, 0}
                               };
            DoubleMatrix2D A = DoubleFactory2D.dense.make(values);
            DoubleMatrix2D B, C;
            /*
            DoubleMatrix1DComparator comp = new DoubleMatrix1DComparator() {
                public int Compare(DoubleMatrix1D a, DoubleMatrix1D b) {
                    double as = a.zSum(); double bs = b.zSum();
                    return as < bs ? -1 : as == bs ? 0 : 1;
                }
            };
            */
            PrintToScreen.WriteLine("\n\nunsorted:" + A);
            B = quickSort.Sort(A, 1);
            C = quickSort.Sort(B, 0);
            PrintToScreen.WriteLine("quick sorted  :" + C);

            B = mergeSort.Sort(A, 1);
            C = mergeSort.Sort(B, 0);
            PrintToScreen.WriteLine("merge sorted  :" + C);
        }

        /**
         * Demonstrates sorting with precomputation of aggregates, comparing mergesort with quicksort.
         */

        public static void zdemo7(int rows, int columns, bool print)
        {
            // for reliable benchmarks, call this method twice: once with small dummy parameters to "warm up" the jitter, then with your real work-load

            PrintToScreen.WriteLine("\n\n");
            PrintToScreen.WriteLine("now initializing... ");

            Functions F = Functions.m_functions;
            DoubleMatrix2D A = DoubleFactory2D.dense.make(rows, columns);
            A.assign(new DRand()); // initialize randomly
            DoubleMatrix2D B = A.Copy();

            double[] v1 = A.viewColumn(0).ToArray();
            double[] v2 = A.viewColumn(0).ToArray();
            Console.Write("now quick sorting... ");
            Timer timer = new Timer().start();
            quickSort.Sort(A, 0);
            timer.stop().display();

            Console.Write("now merge sorting... ");
            timer.reset().start();
            mergeSort.Sort(A, 0);
            timer.stop().display();

            Console.Write("now quick sorting with simple aggregation... ");
            timer.reset().start();
            quickSort.Sort(A, v1);
            timer.stop().display();

            Console.Write("now merge sorting with simple aggregation... ");
            timer.reset().start();
            mergeSort.Sort(A, v2);
            timer.stop().display();
        }
    }
}
