#region

using System;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

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

    ////import DenseDoubleMatrix2D;
    ////import RCDoubleMatrix2D;
    ////import SparseDoubleMatrix2D;
    /**
    Factory for convenient construction of 2-d matrices holding <tt>double</tt> 
      cells. Also provides convenient methods to compose (concatenate) and decompose 
      (split) matrices from/to constituent blocks. </p>
    <p>&nbsp; </p>
    <table border="0" cellspacing="0">
      <tr align="left" valign="top"> 
        <td><i>Construction</i></td>
        <td>Use idioms like <tt>DoubleFactory2D.dense.make(4,4)</tt> to construct 
          dense matrices, <tt>DoubleFactory2D.sparse.make(4,4)</tt> to construct sparse 
          matrices.</td>
      </tr>
      <tr align="left" valign="top"> 
        <td><i> Construction with initial values </i></td>
        <td>Use other <tt>make</tt> methods to construct matrices with given initial 
          values. </td>
      </tr>
      <tr align="left" valign="top"> 
        <td><i> Appending rows and columns </i></td>
        <td>Use methods {@link #appendColumns(DoubleMatrix2D,DoubleMatrix2D) appendColumns}, 
          {@link #appendColumns(DoubleMatrix2D,DoubleMatrix2D) appendRows} and {@link 
          #repeat(DoubleMatrix2D,int,int) repeat} to Append rows and columns. </td>
      </tr>
      <tr align="left" valign="top"> 
        <td><i> General block matrices </i></td>
        <td>Use methods {@link #compose(DoubleMatrix2D[,]) compose} and {@link #decompose(DoubleMatrix2D[,],DoubleMatrix2D) 
          decompose} to work with general block matrices. </td>
      </tr>
      <tr align="left" valign="top"> 
        <td><i> Diagonal matrices </i></td>
        <td>Use methods {@link #diagonal(DoubleMatrix1D) diagonal(vector)}, {@link 
          #diagonal(DoubleMatrix2D) diagonal(matrix)} and {@link #identity(int) identity} 
          to work with diagonal matrices. </td>
      </tr>
      <tr align="left" valign="top"> 
        <td><i> Diagonal block matrices </i></td>
        <td>Use method {@link #composeDiagonal(DoubleMatrix2D,DoubleMatrix2D,DoubleMatrix2D) 
          composeDiagonal} to work with diagonal block matrices. </td>
      </tr>
      <tr align="left" valign="top"> 
        <td><i>RngWrapper</i></td>
        <td>Use methods {@link #random(int,int) random} and {@link #sample(int,int,double,double) 
          sample} to construct random matrices. </td>
      </tr>
    </table>
    <p>&nbsp;</p>
    <p>If the factory is used frequently it might be useful to streamline the notation. 
      For example by aliasing: </p>
    <table>
      <td class="PRE"> 
        <pre>
    DoubleFactory2D F = DoubleFactory2D.dense;
    Functions.make(4,4);
    Functions.descending(10,20);
    Functions.random(4,4);
    ...
    </pre>
      </td>
    </table>

    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class DoubleFactory2D : PersistentObject
    {
        /**
         * A factory producing dense matrices.
         */
        public static DoubleFactory2D dense = new DoubleFactory2D();

        /**
         * A factory producing sparse hash matrices.
         */

        /**
         * A factory producing sparse row compressed matrices.
         */
        public static DoubleFactory2D rowCompressed = new DoubleFactory2D();
        public static DoubleFactory2D sparse = new DoubleFactory2D();

        /*
         * A factory producing sparse row compressed modified matrices.
         */
        //public static  DoubleFactory2D rowCompressedModified = new DoubleFactory2D();
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
        C = A||B; Constructs a new matrix which is the column-wise concatenation of two other matrices.
        <pre>
        0 1 2
        3 4 5
        appendColumns
        6 7
        8 9
        -->
        0 1 2 6 7 
        3 4 5 8 9
        </pre>
        */

        public DoubleMatrix2D appendColumns(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            // force both to have maximal shared number of rows.
            if (B.Rows() > A.Rows())
            {
                B = B.viewPart(0, 0, A.Rows(), B.Columns());
            }
            else if (B.Rows() < A.Rows())
            {
                A = A.viewPart(0, 0, B.Rows(), A.Columns());
            }

            // concatenate
            int ac = A.Columns();
            int bc = B.Columns();
            int r = A.Rows();
            DoubleMatrix2D matrix = make(r, ac + bc);
            matrix.viewPart(0, 0, r, ac).assign(A);
            matrix.viewPart(0, ac, r, bc).assign(B);
            return matrix;
        }

        /**
        C = A||B; Constructs a new matrix which is the row-wise concatenation of two other matrices.
        <pre>
        0 1 
        2 3 
        4 5
        appendRows
        6 7
        8 9
        -->
        0 1 
        2 3 
        4 5
        6 7
        8 9
        </pre>
        */

        public DoubleMatrix2D appendRows(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            // force both to have maximal shared number of columns.
            if (B.Columns() > A.Columns())
            {
                B = B.viewPart(0, 0, B.Rows(), A.Columns());
            }
            else if (B.Columns() < A.Columns())
            {
                A = A.viewPart(0, 0, A.Rows(), B.Columns());
            }

            // concatenate
            int ar = A.Rows();
            int br = B.Rows();
            int c = A.Columns();
            DoubleMatrix2D matrix = make(ar + br, c);
            matrix.viewPart(0, 0, ar, c).assign(A);
            matrix.viewPart(ar, 0, br, c).assign(B);
            return matrix;
        }

        /**
        Constructs a matrix with cells having ascending values.
        For debugging purposes.
        Example:
        <pre>
        0 1 2 
        3 4 5
        </pre>
        */

        public DoubleMatrix2D ascending(int rows, int columns)
        {
            Functions F = Functions.m_functions;
            return descending(rows, columns).assign(Functions.chain(Functions.m_neg, Functions.minus(columns*rows)));
        }

        /**
        Checks whether the given array is rectangular, that is, whether all rows have the same number of columns.
        @throws ArgumentException if the array is not rectangular.
        */

        public static void checkRectangularShape(double[,] array)
        {
            int columns = -1;
            for (int row = array.GetLength(0); --row >= 0;)
            {
                if (array != null)
                {
                    if (columns == -1)
                    {
                        columns = array.GetLength(1);
                    }
                    if (array.GetLength(1) != columns)
                    {
                        throw new ArgumentException("All rows of array must have same number of columns.");
                    }
                }
            }
        }

        /**
        Checks whether the given array is rectangular, that is, whether all rows have the same number of columns.
        @throws ArgumentException if the array is not rectangular.
        */

        public static void checkRectangularShape(DoubleMatrix2D[,] array)
        {
            int columns = -1;
            for (int row = array.GetLength(0); --row >= 0;)
            {
                if (array != null)
                {
                    if (columns == -1)
                    {
                        columns = array.GetLength(1);
                    }
                    if (array.GetLength(1) != columns)
                    {
                        throw new ArgumentException("All rows of array must have same number of columns.");
                    }
                }
            }
        }

        /**
        Constructs a block matrix made from the given parts.
        The inverse to method {@link #decompose(DoubleMatrix2D[,], DoubleMatrix2D)}.
        <p>
        All matrices of a given column within <tt>parts</tt> must have the same number of columns.
        All matrices of a given row within <tt>parts</tt> must have the same number of rows.
        Otherwise an <tt>ArgumentException</tt> is thrown. 
        Note that <tt>null</tt>s within <tt>parts[row,col]</tt> are an exception to this rule: they are ignored.
        Cells are copied.
        Example:
        <table border="1" cellspacing="0">
          <tr align="left" valign="top"> 
            <td><tt>Code</tt></td>
            <td><tt>Result</tt></td>
          </tr>
          <tr align="left" valign="top"> 
            <td> 
              <pre>
        DoubleMatrix2D[,] parts1 = 
        {
        &nbsp;&nbsp;&nbsp;{ null,        make(2,2,1), null        },
        &nbsp;&nbsp;&nbsp;{ make(4,4,2), null,        make(4,3,3) },
        &nbsp;&nbsp;&nbsp;{ null,        make(2,2,4), null        }
        };
        PrintToScreen.WriteLine(compose(parts1));
        </pre>
            </td>
            <td><tt>8&nbsp;x&nbsp;9&nbsp;matrix<br>
              0&nbsp;0&nbsp;0&nbsp;0&nbsp;1&nbsp;1&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0&nbsp;1&nbsp;1&nbsp;0&nbsp;0&nbsp;0<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;0&nbsp;0&nbsp;3&nbsp;3&nbsp;3<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;0&nbsp;0&nbsp;3&nbsp;3&nbsp;3<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;0&nbsp;0&nbsp;3&nbsp;3&nbsp;3<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;0&nbsp;0&nbsp;3&nbsp;3&nbsp;3<br>
              0&nbsp;0&nbsp;0&nbsp;0&nbsp;4&nbsp;4&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0&nbsp;4&nbsp;4&nbsp;0&nbsp;0&nbsp;0</tt></td>
          </tr>
          <tr align="left" valign="top"> 
            <td> 
              <pre>
        DoubleMatrix2D[,] parts3 = 
        {
        &nbsp;&nbsp;&nbsp;{ identity(3),               null,                        },
        &nbsp;&nbsp;&nbsp;{ null,                      identity(3).viewColumnFlip() },
        &nbsp;&nbsp;&nbsp;{ identity(3).viewRowFlip(), null                         }
        };
        PrintToScreen.WriteLine(Environment.NewLine+make(parts3));
        </pre>
            </td>
            <td><tt>9&nbsp;x&nbsp;6&nbsp;matrix<br>
              1&nbsp;0&nbsp;0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;1&nbsp;0&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;1&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;0&nbsp;0&nbsp;1<br>
              0&nbsp;0&nbsp;0&nbsp;0&nbsp;1&nbsp;0<br>
              0&nbsp;0&nbsp;0&nbsp;1&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;1&nbsp;0&nbsp;0&nbsp;0<br>
              0&nbsp;1&nbsp;0&nbsp;0&nbsp;0&nbsp;0<br>
              1&nbsp;0&nbsp;0&nbsp;0&nbsp;0&nbsp;0 </tt></td>
          </tr>
          <tr align="left" valign="top"> 
            <td> 
              <pre>
        DoubleMatrix2D A = ascending(2,2);
        DoubleMatrix2D B = descending(2,2);
        DoubleMatrix2D _ = null;

        DoubleMatrix2D[,] parts4 = 
        {
        &nbsp;&nbsp;&nbsp;{ A, _, A, _ },
        &nbsp;&nbsp;&nbsp;{ _, A, _, B }
        };
        PrintToScreen.WriteLine(Environment.NewLine+make(parts4));
        </pre>
            </td>
            <td><tt>4&nbsp;x&nbsp;8&nbsp;matrix<br>
              1&nbsp;2&nbsp;0&nbsp;0&nbsp;1&nbsp;2&nbsp;0&nbsp;0<br>
              3&nbsp;4&nbsp;0&nbsp;0&nbsp;3&nbsp;4&nbsp;0&nbsp;0<br>
              0&nbsp;0&nbsp;1&nbsp;2&nbsp;0&nbsp;0&nbsp;3&nbsp;2<br>
              0&nbsp;0&nbsp;3&nbsp;4&nbsp;0&nbsp;0&nbsp;1&nbsp;0 </tt></td>
          </tr>
          <tr align="left" valign="top"> 
            <td> 
              <pre>
        DoubleMatrix2D[,] parts2 = 
        {
        &nbsp;&nbsp;&nbsp;{ null,        make(2,2,1), null        },
        &nbsp;&nbsp;&nbsp;{ make(4,4,2), null,        make(4,3,3) },
        &nbsp;&nbsp;&nbsp;{ null,        make(2,3,4), null        }
        };
        PrintToScreen.WriteLine(Environment.NewLine+Factory2D.make(parts2));
        </pre>
            </td>
            <td><tt>ArgumentException<br>
              A[0,1].cols != A[2,1].cols<br>
              (2 != 3)</tt></td>
          </tr>
        </table>
        @throws ArgumentException subject to the conditions outlined above.
        */

        public DoubleMatrix2D compose(DoubleMatrix2D[,] parts)
        {
            checkRectangularShape(parts);
            int rows = parts.GetLength(0);
            int columns = 0;
            if (parts.GetLength(0) > 0)
            {
                columns = parts.GetLength(1);
            }
            DoubleMatrix2D empty = make(0, 0);

            if (rows == 0 || columns == 0)
            {
                return empty;
            }

            // determine maximum column width of each column
            int[] maxWidths = new int[columns];
            for (int column = columns; --column >= 0;)
            {
                int maxWidth = 0;
                for (int row = rows; --row >= 0;)
                {
                    DoubleMatrix2D part = parts[row, column];
                    if (part != null)
                    {
                        int width = part.Columns();
                        if (maxWidth > 0 && width > 0 && width != maxWidth)
                        {
                            throw new ArgumentException("Different number of columns.");
                        }
                        maxWidth = Math.Max(maxWidth, width);
                    }
                }
                maxWidths[column] = maxWidth;
            }

            // determine row height of each row
            int[] maxHeights = new int[rows];
            for (int row = rows; --row >= 0;)
            {
                int maxHeight = 0;
                for (int column = columns; --column >= 0;)
                {
                    DoubleMatrix2D part = parts[row, column];
                    if (part != null)
                    {
                        int height = part.Rows();
                        if (maxHeight > 0 && height > 0 && height != maxHeight)
                        {
                            throw new ArgumentException("Different number of rows.");
                        }
                        maxHeight = Math.Max(maxHeight, height);
                    }
                }
                maxHeights[row] = maxHeight;
            }


            // shape of result 
            int resultRows = 0;
            for (int row = rows; --row >= 0;)
            {
                resultRows += maxHeights[row];
            }
            int resultCols = 0;
            for (int column = columns; --column >= 0;)
            {
                resultCols += maxWidths[column];
            }

            DoubleMatrix2D matrix = make(resultRows, resultCols);

            // copy
            int r = 0;
            for (int row = 0; row < rows; row++)
            {
                int c = 0;
                for (int column = 0; column < columns; column++)
                {
                    DoubleMatrix2D part = parts[row, column];
                    if (part != null)
                    {
                        matrix.viewPart(r, c, part.Rows(), part.Columns()).assign(part);
                    }
                    c += maxWidths[column];
                }
                r += maxHeights[row];
            }

            return matrix;
        }

        /**
        Constructs a diagonal block matrix from the given parts (the <i>direct sum</i> of two matrices).
        That is the concatenation
        <pre>
        A 0
        0 B
        </pre>
        (The direct sum has <tt>A.Rows()+B.Rows()</tt> rows and <tt>A.Columns()+B.Columns()</tt> columns).
        Cells are copied.
        @return a new matrix which is the direct sum.
        */

        public DoubleMatrix2D composeDiagonal(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            int ar = A.Rows();
            int ac = A.Columns();
            int br = B.Rows();
            int bc = B.Columns();
            DoubleMatrix2D sum = make(ar + br, ac + bc);
            sum.viewPart(0, 0, ar, ac).assign(A);
            sum.viewPart(ar, ac, br, bc).assign(B);
            return sum;
        }

        /**
        Constructs a diagonal block matrix from the given parts.
        The concatenation has the form
        <pre>
        A 0 0
        0 B 0
        0 0 C
        </pre>
        from the given parts.
        Cells are copied.
        */

        public DoubleMatrix2D composeDiagonal(DoubleMatrix2D A, DoubleMatrix2D B, DoubleMatrix2D C)
        {
            DoubleMatrix2D diag = make(A.Rows() + B.Rows() + C.Rows(), A.Columns() + B.Columns() + C.Columns());
            diag.viewPart(0, 0, A.Rows(), A.Columns()).assign(A);
            diag.viewPart(A.Rows(), A.Columns(), B.Rows(), B.Columns()).assign(B);
            diag.viewPart(A.Rows() + B.Rows(), A.Columns() + B.Columns(), C.Rows(), C.Columns()).assign(C);
            return diag;
        }

        /**
        Splits a block matrix into its constituent blocks; Copies blocks of a matrix into the given parts.
        The inverse to method {@link #compose(DoubleMatrix2D[,])}.
        <p>
        All matrices of a given column within <tt>parts</tt> must have the same number of columns.
        All matrices of a given row within <tt>parts</tt> must have the same number of rows.
        Otherwise an <tt>ArgumentException</tt> is thrown. 
        Note that <tt>null</tt>s within <tt>parts[row,col]</tt> are an exception to this rule: they are ignored.
        Cells are copied.
        Example:
        <table border="1" cellspacing="0">
          <tr align="left" valign="top"> 
            <td><tt>Code</tt></td>
            <td><tt>matrix</tt></td>
            <td><tt>--&gt; parts </tt></td>
          </tr>
          <tr align="left" valign="top"> 
            <td> 
              <pre>
        DoubleMatrix2D matrix = ... ;
        DoubleMatrix2D _ = null;
        DoubleMatrix2D A,B,C,D;
        A = make(2,2); B = make (4,4);
        C = make(4,3); D = make (2,2);
        DoubleMatrix2D[,] parts = 
        {
        &nbsp;&nbsp;&nbsp;{ _, A, _ },
        &nbsp;&nbsp;&nbsp;{ B, _, C },
        &nbsp;&nbsp;&nbsp;{ _, D, _ }
        };
        decompose(parts,matrix);
        PrintToScreen.WriteLine(&quot;\nA = &quot;+A);
        PrintToScreen.WriteLine(&quot;\nB = &quot;+B);
        PrintToScreen.WriteLine(&quot;\nC = &quot;+C);
        PrintToScreen.WriteLine(&quot;\nD = &quot;+D);
        </pre>
            </td>
            <td><tt>8&nbsp;x&nbsp;9&nbsp;matrix<br>
              9&nbsp;9&nbsp;9&nbsp;9&nbsp;1&nbsp;1&nbsp;9&nbsp;9&nbsp;9<br>
              9&nbsp;9&nbsp;9&nbsp;9&nbsp;1&nbsp;1&nbsp;9&nbsp;9&nbsp;9<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;9&nbsp;9&nbsp;3&nbsp;3&nbsp;3<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;9&nbsp;9&nbsp;3&nbsp;3&nbsp;3<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;9&nbsp;9&nbsp;3&nbsp;3&nbsp;3<br>
              2&nbsp;2&nbsp;2&nbsp;2&nbsp;9&nbsp;9&nbsp;3&nbsp;3&nbsp;3<br>
              9&nbsp;9&nbsp;9&nbsp;9&nbsp;4&nbsp;4&nbsp;9&nbsp;9&nbsp;9<br>
              9&nbsp;9&nbsp;9&nbsp;9&nbsp;4&nbsp;4&nbsp;9&nbsp;9&nbsp;9</tt></td>
            <td> 
              <p><tt>A = 2&nbsp;x&nbsp;2&nbsp;matrix<br>
                1&nbsp;1<br>
                1&nbsp;1</tt></p>
              <p><tt>B = 4&nbsp;x&nbsp;4&nbsp;matrix<br>
                2&nbsp;2&nbsp;2&nbsp;2<br>
                2&nbsp;2&nbsp;2&nbsp;2<br>
                2&nbsp;2&nbsp;2&nbsp;2<br>
                2&nbsp;2&nbsp;2&nbsp;2</tt></p>
              <p><tt>C = 4&nbsp;x&nbsp;3&nbsp;matrix<br>
                3&nbsp;3&nbsp;3<br>
                3&nbsp;3&nbsp;3<br>
                </tt><tt>3&nbsp;3&nbsp;3<br>
                </tt><tt>3&nbsp;3&nbsp;3</tt></p>
              <p><tt>D = 2&nbsp;x&nbsp;2&nbsp;matrix<br>
                4&nbsp;4<br>
                4&nbsp;4</tt></p>
              </td>
          </tr>
        </table>
        @throws ArgumentException subject to the conditions outlined above.
        */

        public void decompose(DoubleMatrix2D[,] parts, DoubleMatrix2D matrix)
        {
            checkRectangularShape(parts);
            int rows = parts.GetLength(0);
            int columns = 0;
            if (parts.GetLength(0) > 0)
            {
                columns = parts.GetLength(1);
            }
            if (rows == 0 || columns == 0)
            {
                return;
            }

            // determine maximum column width of each column
            int[] maxWidths = new int[columns];
            for (int column = columns; --column >= 0;)
            {
                int maxWidth = 0;
                for (int row = rows; --row >= 0;)
                {
                    DoubleMatrix2D part = parts[row, column];
                    if (part != null)
                    {
                        int width = part.Columns();
                        if (maxWidth > 0 && width > 0 && width != maxWidth)
                        {
                            throw new ArgumentException("Different number of columns.");
                        }
                        maxWidth = Math.Max(maxWidth, width);
                    }
                }
                maxWidths[column] = maxWidth;
            }

            // determine row height of each row
            int[] maxHeights = new int[rows];
            for (int row = rows; --row >= 0;)
            {
                int maxHeight = 0;
                for (int column = columns; --column >= 0;)
                {
                    DoubleMatrix2D part = parts[row, column];
                    if (part != null)
                    {
                        int height = part.Rows();
                        if (maxHeight > 0 && height > 0 && height != maxHeight)
                        {
                            throw new ArgumentException("Different number of rows.");
                        }
                        maxHeight = Math.Max(maxHeight, height);
                    }
                }
                maxHeights[row] = maxHeight;
            }


            // shape of result parts
            int resultRows = 0;
            for (int row = rows; --row >= 0;)
            {
                resultRows += maxHeights[row];
            }
            int resultCols = 0;
            for (int column = columns; --column >= 0;)
            {
                resultCols += maxWidths[column];
            }

            if (matrix.Rows() < resultRows || matrix.Columns() < resultCols)
            {
                throw new ArgumentException("Parts larger than matrix.");
            }

            // copy
            int r = 0;
            for (int row = 0; row < rows; row++)
            {
                int c = 0;
                for (int column = 0; column < columns; column++)
                {
                    DoubleMatrix2D part = parts[row, column];
                    if (part != null)
                    {
                        part.assign(matrix.viewPart(r, c, part.Rows(), part.Columns()));
                    }
                    c += maxWidths[column];
                }
                r += maxHeights[row];
            }
        }

        /**
         * Demonstrates usage of this class.
         */

        public void demo1()
        {
            PrintToScreen.WriteLine("\n\n");
            DoubleMatrix2D[,] parts1 =
                {
                    {null, make(2, 2, 1), null},
                    {make(4, 4, 2), null, make(4, 3, 3)},
                    {null, make(2, 2, 4), null}
                };
            PrintToScreen.WriteLine(Environment.NewLine + compose(parts1));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(make(parts1).ToString()));

            /*
            //
            illegal 2 != 3
            DoubleMatrix2D[,] parts2 = 
            {
                { null,        make(2,2,1), null        },
                { make(4,4,2), null,        make(4,3,3) },
                { null,        make(2,3,4), null        }
            };
            PrintToScreen.WriteLine(Environment.NewLine+make(parts2));
            */

            DoubleMatrix2D[,] parts3 =
                {
                    {identity(3), null,},
                    {null, identity(3).viewColumnFlip()},
                    {identity(3).viewRowFlip(), null}
                };
            PrintToScreen.WriteLine(Environment.NewLine + compose(parts3));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(make(parts3).ToString()));

            DoubleMatrix2D A = ascending(2, 2);
            DoubleMatrix2D B = descending(2, 2);
            DoubleMatrix2D _ = null;

            DoubleMatrix2D[,] parts4 =
                {
                    {A, _, A, _},
                    {_, A, _, B}
                };
            PrintToScreen.WriteLine(Environment.NewLine + compose(parts4));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(make(parts4).ToString()));
        }

        /**
         * Demonstrates usage of this class.
         */

        public void demo2()
        {
            PrintToScreen.WriteLine("\n\n");
            DoubleMatrix2D matrix;
            DoubleMatrix2D A, B, C, D; //, E, F, G;
            DoubleMatrix2D _ = null;
            A = make(2, 2, 1);
            B = make(4, 4, 2);
            C = make(4, 3, 3);
            D = make(2, 2, 4);
            DoubleMatrix2D[,] parts1 =
                {
                    {_, A, _},
                    {B, _, C},
                    {_, D, _}
                };
            matrix = compose(parts1);
            PrintToScreen.WriteLine(Environment.NewLine + matrix);

            A.assign(9);
            B.assign(9);
            C.assign(9);
            D.assign(9);
            decompose(parts1, matrix);
            PrintToScreen.WriteLine(A);
            PrintToScreen.WriteLine(B);
            PrintToScreen.WriteLine(C);
            PrintToScreen.WriteLine(D);
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(make(parts1).ToString()));

            /*
            //
            illegal 2 != 3
            DoubleMatrix2D[,] parts2 = 
            {
                { null,        make(2,2,1), null        },
                { make(4,4,2), null,        make(4,3,3) },
                { null,        make(2,3,4), null        }
            };
            PrintToScreen.WriteLine(Environment.NewLine+Factory2D.make(parts2));
            */

            /*
            DoubleMatrix2D[,] parts3 = 
            {
                { identity(3),               null,                        },
                { null,                      identity(3).viewColumnFlip() },
                { identity(3).viewRowFlip(), null                         }
            };
            PrintToScreen.WriteLine(Environment.NewLine+make(parts3));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(make(parts3).ToString()));

            DoubleMatrix2D A = ascending(2,2);
            DoubleMatrix2D B = descending(2,2);
            DoubleMatrix2D _ = null;

            DoubleMatrix2D[,] parts4 = 
            {
                { A, _, A, _ },
                { _, A, _, B }
            };
            PrintToScreen.WriteLine(Environment.NewLine+make(parts4));
            //PrintToScreen.WriteLine(Environment.NewLine+matrixpattern.Converting.toHTML(make(parts4).ToString()));
            */
        }

        /**
        Constructs a matrix with cells having descending values.
        For debugging purposes.
        Example:
        <pre>
        5 4 3 
        2 1 0
        </pre>
        */

        public DoubleMatrix2D descending(int rows, int columns)
        {
            DoubleMatrix2D matrix = make(rows, columns);
            int v = 0;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    matrix.setQuick(row, column, v++);
                }
            }
            return matrix;
        }

        /**
        Constructs a new diagonal matrix whose diagonal elements are the elements of <tt>vector</tt>.
        Cells values are copied. The new matrix is not a view.
        Example:
        <pre>
        5 4 3 -->
        5 0 0
        0 4 0
        0 0 3
        </pre>
        @return a new matrix.
        */

        public DoubleMatrix2D diagonal(DoubleMatrix1D vector)
        {
            int size = vector.Size();
            DoubleMatrix2D diag = make(size, size);
            for (int i = size; --i >= 0;)
            {
                diag.setQuick(i, i, vector.getQuick(i));
            }
            return diag;
        }

        /**
        Constructs a new vector consisting of the diagonal elements of <tt>A</tt>.
        Cells values are copied. The new vector is not a view.
        Example:
        <pre>
        5 0 0 9
        0 4 0 9
        0 0 3 9
        --> 5 4 3
        </pre>
        @param A the matrix, need not be square.
        @return a new vector.
        */

        public DoubleMatrix1D diagonal(DoubleMatrix2D A)
        {
            int min = Math.Min(A.Rows(), A.Columns());
            DoubleMatrix1D diag = make1D(min);
            for (int i = min; --i >= 0;)
            {
                diag.setQuick(i, A.getQuick(i, i));
            }
            return diag;
        }

        /**
         * Constructs an identity matrix (having ones on the diagonal and zeros elsewhere).
         */

        public DoubleMatrix2D identity(int rowsAndColumns)
        {
            DoubleMatrix2D matrix = make(rowsAndColumns, rowsAndColumns);
            for (int i = rowsAndColumns; --i >= 0;)
            {
                matrix.setQuick(i, i, 1);
            }
            return matrix;
        }

        /**
         * Constructs a matrix with the given cell values.
         * <tt>values</tt> is required to have the form <tt>values[row,column]</tt>
         * and have exactly the same number of columns in every row.
         * <p>
         * The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
         *
         * @param values The values to be filled into the new matrix.
         * @throws ArgumentException if <tt>for any 1 &lt;= row &lt; values.Length: values.GetLength(1) != values[row-1].Length</tt>.
         */

        public DoubleMatrix2D make(double[,] values)
        {
            if (this == sparse)
            {
                return new SparseDoubleMatrix2D(values);
            }
            else
            {
                return new DenseDoubleMatrix2D(values);
            }
        }

        /** 
        Construct a matrix from a one-dimensional column-major packed array, ala Fortran.
        Has the form <tt>matrix.get(row,column) == values[row + column*rows]</tt>.
        The values are copied.

        @param values One-dimensional array of doubles, packed by columns (ala Fortran).
        @param rows  the number of rows.
        @exception  ArgumentException <tt>values.Length</tt> must be a multiple of <tt>rows</tt>.
        */

        public DoubleMatrix2D make(double[] values, int rows)
        {
            int columns = (rows != 0 ? values.Length/rows : 0);
            if (rows*columns != values.Length)
            {
                throw new ArgumentException("Array Length must be a multiple of m.");
            }

            DoubleMatrix2D matrix = make(rows, columns);
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    matrix.setQuick(row, column, values[row + column*rows]);
                }
            }
            return matrix;
        }

        /**
         * Constructs a matrix with the given shape, each cell initialized with zero.
         */

        public DoubleMatrix2D make(int rows, int columns)
        {
            if (this == sparse)
            {
                return new SparseDoubleMatrix2D(rows, columns);
            }
            if (this == rowCompressed)
            {
                return new RCDoubleMatrix2D(rows, columns);
            }
                //if (this==rowCompressedModified) return new RCMDoubleMatrix2D(rows,columns);
            else
            {
                return new DenseDoubleMatrix2D(rows, columns);
            }
        }

        /**
         * Constructs a matrix with the given shape, each cell initialized with the given value.
         */

        public DoubleMatrix2D make(int rows, int columns, double initialValue)
        {
            if (initialValue == 0)
            {
                return make(rows, columns);
            }
            return make(rows, columns).assign(initialValue);
        }

        /**
         * Constructs a 1d matrix of the right dynamic type.
         */

        public DoubleMatrix1D make1D(int size)
        {
            return make(0, 0).like1D(size);
        }

        /**
         * Constructs a matrix with uniformly distributed values in <tt>(0,1)</tt> (exclusive).
         */

        public DoubleMatrix2D random(int rows, int columns)
        {
            return make(rows, columns).assign(Functions.random());
        }

        /**
        C = A||A||..||A; Constructs a new matrix which is duplicated both along the row and column dimension.
        Example:
        <pre>
        0 1
        2 3
        repeat(2,3) -->
        0 1 0 1 0 1
        2 3 2 3 2 3
        0 1 0 1 0 1
        2 3 2 3 2 3
        </pre>
        */

        public DoubleMatrix2D repeat(DoubleMatrix2D A, int rowRepeat, int columnRepeat)
        {
            int r = A.Rows();
            int c = A.Columns();
            DoubleMatrix2D matrix = make(r*rowRepeat, c*columnRepeat);
            for (int i = rowRepeat; --i >= 0;)
            {
                for (int j = columnRepeat; --j >= 0;)
                {
                    matrix.viewPart(r*i, c*j, r, c).assign(A);
                }
            }
            return matrix;
        }

        /**
         * Constructs a randomly sampled matrix with the given shape.
         * Randomly picks exactly <tt>Math.Round(rows*columns*nonZeroFraction)</tt> cells and initializes them to <tt>value</tt>, all the rest will be initialized to zero.
         * Note that this is not the same as setting each cell with probability <tt>nonZeroFraction</tt> to <tt>value</tt>.
         * Note: The random seed is a constant.
         * @throws ArgumentException if <tt>nonZeroFraction < 0 || nonZeroFraction > 1</tt>.
         * @see RandomSampler
         */

        public DoubleMatrix2D sample(int rows, int columns, double value, double nonZeroFraction)
        {
            DoubleMatrix2D matrix = make(rows, columns);
            sample(matrix, value, nonZeroFraction);
            return matrix;
        }

        /**
         * Modifies the given matrix to be a randomly sampled matrix.
         * Randomly picks exactly <tt>Math.Round(rows*columns*nonZeroFraction)</tt> cells and initializes them to <tt>value</tt>, all the rest will be initialized to zero.
         * Note that this is not the same as setting each cell with probability <tt>nonZeroFraction</tt> to <tt>value</tt>.
         * Note: The random seed is a constant.
         * @throws ArgumentException if <tt>nonZeroFraction < 0 || nonZeroFraction > 1</tt>.
         * @see RandomSampler
         */

        public DoubleMatrix2D sample(DoubleMatrix2D matrix, double value, double nonZeroFraction)
        {
            int rows = matrix.Rows();
            int columns = matrix.Columns();
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

            matrix.assign(0);

            int size = rows*columns;
            int n = (int) Math.Round(size*nonZeroFraction);
            if (n == 0)
            {
                return matrix;
            }

            RandomSamplingAssistant sampler = new RandomSamplingAssistant(n, size, new RngWrapper());
            for (int i = 0; i < size; i++)
            {
                if (sampler.sampleNextElement())
                {
                    int row = (i/columns);
                    int column = (i%columns);
                    matrix.set(row, column, value);
                }
            }

            return matrix;
        }
    }
}
