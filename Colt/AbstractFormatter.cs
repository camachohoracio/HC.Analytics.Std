#region

using System;
using System.Text;
using HC.Core.Exceptions;
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
    ////package impl;

    /** 
    Abstract base class for flexible, well human readable matrix print formatting.
    Value type independent.
    A single cell is formatted via a format string.
    Columns can be aligned left, centered, right and by decimal point. 
    <p>A column can be broader than specified by the parameter <tt>minColumnWidth</tt> 
      (because a cell may not fit into that width) but a column is never smaller than 
      <tt>minColumnWidth</tt>. Normally one does not need to specify <tt>minColumnWidth</tt>.
    Cells in a row are separated by a separator string, similar separators can be set for rows and slices.
    For more info, see the concrete subclasses.
 
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public abstract class AbstractFormatter : PersistentObject
    {
        /**
         * The alignment string aligning the cells of a column to the left.
         */

        /**
         * The alignment string aligning the cells of a column to its center.
         */
        public static string CENTER = "center";

        /**
         * The alignment string aligning the cells of a column to the right.
         */

        /**
         * The alignment string aligning the cells of a column to the decimal point.
         */
        public static string DECIMAL = "decimal";

        /**
         * The default minimum number of characters a column may have; currently <tt>1</tt>.
         */

        /**
         * The default string separating any two columns from another; currently <tt>" "</tt>.
         */
        public static string DEFAULT_COLUMN_SEPARATOR = " ";
        public static int DEFAULT_MIN_COLUMN_WIDTH = 1;

        /**
         * The default string separating any two rows from another; currently <tt>Environment.NewLine</tt>.
         */
        public static string DEFAULT_ROW_SEPARATOR = Environment.NewLine;

        /**
         * The default string separating any two slices from another; currently <tt>"\n\n"</tt>.
         */
        public static string DEFAULT_SLICE_SEPARATOR = "\n\n";
        public static string LEFT = "left";
        private static string[] m_blanksCache; // for efficient string manipulations

        public static FormerFactory m_factory = new FormerFactory();
        public static string RIGHT = "right";


        /**
         * The default format string for formatting a single cell value; currently <tt>"%G"</tt>.
         */
        public string m_alignment = LEFT;

        /**
         * The default format string for formatting a single cell value; currently <tt>"%G"</tt>.
         */

        /**
         * The default string separating any two columns from another; currently <tt>" "</tt>.
         */
        public string m_columnSeparator = DEFAULT_COLUMN_SEPARATOR;
        public int m_minColumnWidth = DEFAULT_MIN_COLUMN_WIDTH;
        public bool m_printShape = true;

        /**
         * The default string separating any two rows from another; currently <tt>Environment.NewLine</tt>.
         */
        public string m_rowSeparator = DEFAULT_ROW_SEPARATOR;

        /**
         * The default string separating any two slices from another; currently <tt>"\n\n"</tt>.
         */
        public string m_sliceSeparator = DEFAULT_SLICE_SEPARATOR;
        public string m_strFormat = "%G";

        /**
         * Tells whether string representations are to be preceded with summary of the shape; currently <tt>true</tt>.
         */

        static AbstractFormatter()
        {
            setupBlanksCache();
        }

        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Modifies the strings in a column of the string matrix to be aligned (left,centered,right,decimal).
         */

        public void align(string[,] strings)
        {
            int rows = strings.GetLength(0);
            int columns = 0;
            if (rows > 0)
            {
                columns = strings.GetLength(1);
            }

            int[] maxColWidth = new int[columns];
            int[] maxColLead = null;
            bool isDecimal = m_alignment.Equals(DECIMAL);
            if (isDecimal)
            {
                maxColLead = new int[columns];
            }
            //int[] maxColTrail = new int[columns];

            // for each column, determine alignment parameters
            for (int column = 0; column < columns; column++)
            {
                int maxWidth = m_minColumnWidth;
                int maxLead = int.MinValue;
                //int maxTrail = int.MinValue;
                for (int row = 0; row < rows; row++)
                {
                    string s = strings[row, column];
                    maxWidth = Math.Max(maxWidth, s.Length);
                    if (isDecimal)
                    {
                        maxLead = Math.Max(maxLead, lead(s));
                    }
                    //maxTrail = Math.Max(maxTrail, trail(s));
                }
                maxColWidth[column] = maxWidth;
                if (isDecimal)
                {
                    maxColLead[column] = maxLead;
                }
                //maxColTrail[column] = maxTrail;
            }

            // format each row according to alignment parameters
            //StringBuilder total = new StringBuilder();
            for (int row = 0; row < rows; row++)
            {
                alignRow(
                    ArrayHelper.GetRowCopy(
                        strings, row),
                    maxColWidth,
                    maxColLead);
            }
        }

        /**
         * Converts a row into a string.
         */

        public int alignmentCode(string alignment)
        {
            //{-1,0,1,2} = {left,centered,right,decimal point}
            if (alignment.Equals(LEFT))
            {
                return -1;
            }
            else if (alignment.Equals(CENTER))
            {
                return 0;
            }
            else if (alignment.Equals(RIGHT))
            {
                return 1;
            }
            else if (alignment.Equals(DECIMAL))
            {
                return 2;
            }
            else
            {
                throw new ArgumentException("unknown alignment: " + alignment);
            }
        }

        /**
         * Modifies the strings the string matrix to be aligned (left,centered,right,decimal).
         */

        public void alignRow(
            string[] row,
            int[] maxColWidth,
            int[] maxColLead)
        {
            int align = alignmentCode(m_alignment); //{-1,0,1,2} = {left,centered,right,decimal point}
            StringBuilder s = new StringBuilder();

            int columns = row.Length;
            for (int column = 0; column < columns; column++)
            {
                //s.setLength(0);
                string c = row[column];
                //if (alignment==1) {
                if (m_alignment.Equals(RIGHT))
                {
                    s.Append(blanks(maxColWidth[column] - s.Length));
                    s.Append(c);
                }
                    //else if (alignment==2) {
                else if (m_alignment.Equals(DECIMAL))
                {
                    s.Append(blanks(maxColLead[column] - lead(c)));
                    s.Append(c);
                    s.Append(blanks(maxColWidth[column] - s.Length));
                }
                    //else if (align==0) {
                else if (m_alignment.Equals(CENTER))
                {
                    s.Append(blanks((maxColWidth[column] - c.Length)/2));
                    s.Append(c);
                    s.Append(blanks(maxColWidth[column] - s.Length));
                }
                    //else if (align<0) {
                else if (m_alignment.Equals(LEFT))
                {
                    s.Append(c);
                    s.Append(blanks(maxColWidth[column] - s.Length));
                }
                else
                {
                    throw new HCException();
                }

                row[column] = s.ToString();
            }
        }

        /**
         * Returns a string with <tt>Length</tt> blanks.
         */

        public string blanks(int Length)
        {
            if (Length < 0)
            {
                Length = 0;
            }
            if (Length < m_blanksCache.Length)
            {
                return m_blanksCache[Length];
            }

            StringBuilder buf = new StringBuilder(Length);
            for (int k = 0; k < Length; k++)
            {
                buf.Append(' ');
            }
            return buf.ToString();
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo1()
        {
            /*
            // parameters
            Object[,] values = {
                {3,     0,        -3.4, 0},
                {5.1   ,0,        +3.0123456789, 0},
                {16.37, 0.0,       2.5, 0},
                {-16.3, 0,        -3.012345678E-4, -1},
                {1236.3456789, 0,  7, -1.2}
            };
            string[] formats =         {"%G", "%1.10G", "%f", "%1.2f", "%0.2e", null};


            // now the processing
            int size = formats.Length;
            ObjectMatrix2D matrix = ObjectFactory2D.dense.make(values);
            string[] strings = new string[size];
            string[] sourceCodes = new string[size];
            string[] htmlStrings = new string[size];
            string[] htmlSourceCodes = new string[size];

            for (int i=0; i<size; i++) {
                string format = formats[i];
                strings[i] = ToString(matrix,format);
                sourceCodes[i] = toSourceCode(matrix,format);

                // may not compile because of packages not included in the distribution
                //htmlStrings[i] = matrixpattern.Converting.toHTML(strings[i]);
                //htmlSourceCodes[i] = matrixpattern.Converting.toHTML(sourceCodes[i]);
            }

            PrintToScreen.WriteLine("original:\n"+ToString(matrix));

            // may not compile because of packages not included in the distribution
            for (int i=0; i<size; i++) {
                //PrintToScreen.WriteLine("\nhtmlString("+formats[i]+"):\n"+htmlStrings[i]);
                //PrintToScreen.WriteLine("\nhtmlSourceCode("+formats[i]+"):\n"+htmlSourceCodes[i]);
            }

            for (int i=0; i<size; i++) {
                PrintToScreen.WriteLine("\nstring("+formats[i]+"):\n"+strings[i]);
                PrintToScreen.WriteLine("\nsourceCode("+formats[i]+"):\n"+sourceCodes[i]);
            }
            */
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo2()
        {
            /*
            // parameters
            Object[] values = {
                //5, 0.0, -0.0, -Object.NaN, Object.NaN, 0.0/0.0, Object.NegativeInfinity, Object.PositiveInfinity, Object.MinValue, Object.MaxValue
                5, 0.0, -0.0, -Object.NaN, Object.NaN, 0.0/0.0, Object.MinValue, Object.MaxValue , Object.NegativeInfinity, Object.PositiveInfinity
                //Object.MinValue, Object.MaxValue //, Object.NegativeInfinity, Object.PositiveInfinity
            };
            //string[] formats =         {"%G", "%1.10G", "%f", "%1.2f", "%0.2e"};
            string[] formats =         {"%G", "%1.19G"};


            // now the processing
            int size = formats.Length;
            ObjectMatrix1D matrix = new DenseObjectMatrix1D(values);

            string[] strings = new string[size];
            //string[] javaStrings = new string[size];

            for (int i=0; i<size; i++) {
                string format = formats[i];
                strings[i] = ToString(matrix,format);
                for (int j=0; j<matrix.Size(); j++) {
                    PrintToScreen.WriteLine((matrix.get(j)));
                }
            }

            PrintToScreen.WriteLine("original:\n"+ToString(matrix));

            for (int i=0; i<size; i++) {
                PrintToScreen.WriteLine("\nstring("+formats[i]+"):\n"+strings[i]);
            }
            */
        }

        /**
         * Demonstrates how to use this class.
         */

        public static void demo3(int size, Object value)
        {
            /*
            Timer timer = new Timer();
            string s;
            StringBuilder buf;
            ObjectMatrix2D matrix = ObjectFactory2D.dense.make(size,size, value);

            timer.reset().start();
            buf = new StringBuilder();
            for (int i=size; --i >= 0; ) {
                for (int j=size; --j >= 0; ) {
                    buf.Append(matrix.getQuick(i,j));
                }
            }
            buf = null;
            timer.stop().display();

            timer.reset().start();
            corejava.Format format = new corejava.Format("%G");
            buf = new StringBuilder();
            for (int i=size; --i >= 0; ) {
                for (int j=size; --j >= 0; ) {
                    buf.Append(format.form(matrix.getQuick(i,j)));
                }
            }
            buf = null;
            timer.stop().display();

            timer.reset().start();
            s = Formatting.ToString(matrix, null);
            //PrintToScreen.WriteLine(s);
            s = null;
            timer.stop().display();

            timer.reset().start();
            s = Formatting.ToString(matrix, "%G");
            //PrintToScreen.WriteLine(s);
            s = null;
            timer.stop().display();
            */
        }

        /**
         * Converts a given cell to a string; no alignment considered.
         */
        public abstract string form(AbstractMatrix1D matrix, int index, Former formatter);
        /**
         * Returns a string representations of all cells; no alignment considered.
         */
        public abstract string[,] format(AbstractMatrix2D matrix);
        /**
         * Returns a string representations of all cells; no alignment considered.
         */

        public string[] formatRow(AbstractMatrix1D vector)
        {
            Former formatter = null;
            formatter = m_factory.create(m_strFormat);
            int s = vector.Size();
            string[] strings = new string[s];
            for (int i = 0; i < s; i++)
            {
                strings[i] = form(vector, i, formatter);
            }
            return strings;
        }

        /**
         * Returns the number of characters or the number of characters before the decimal point.
         */

        public int lead(string s)
        {
            return s.Length;
        }

        /**
         * Returns a string with the given character repeated <tt>Length</tt> times.
         */

        public string repeat(char character, int Length)
        {
            if (character == ' ')
            {
                return blanks(Length);
            }
            if (Length < 0)
            {
                Length = 0;
            }
            StringBuilder buf = new StringBuilder(Length);
            for (int k = 0; k < Length; k++)
            {
                buf.Append(character);
            }
            return buf.ToString();
        }

        /**
         * Sets the column alignment (left,center,right,decimal).
         * @param alignment the new alignment to be used; must be one of <tt>{LEFT,CENTER,RIGHT,DECIMAL}</tt>.
         */

        public void setAlignment(string alignment)
        {
            m_alignment = alignment;
        }

        /**
         * Sets the string separating any two columns from another.
         * @param columnSeparator the new columnSeparator to be used.
         */

        public void setColumnSeparator(string columnSeparator)
        {
            m_columnSeparator = columnSeparator;
        }

        /**
         * Sets the way a <i>single</i> cell value is to be formatted.
         * @param format the new format to be used.
         */

        public void setFormat(string format)
        {
            m_strFormat = format;
        }

        /**
         * Sets the minimum number of characters a column may have.
         * @param minColumnWidth the new minColumnWidth to be used.
         */

        public void setMinColumnWidth(int minColumnWidth)
        {
            if (minColumnWidth < 0)
            {
                throw new ArgumentException();
            }
            m_minColumnWidth = minColumnWidth;
        }

        /**
         * Specifies whether a string representation of a matrix is to be preceded with a summary of its shape.
         * @param printShape <tt>true</tt> shape summary is printed, otherwise not printed.
         */

        public void setPrintShape(bool printShape)
        {
            m_printShape = printShape;
        }

        /**
         * Sets the string separating any two rows from another.
         * @param rowSeparator the new rowSeparator to be used.
         */

        public void setRowSeparator(string rowSeparator)
        {
            m_rowSeparator = rowSeparator;
        }

        /**
         * Sets the string separating any two slices from another.
         * @param sliceSeparator the new sliceSeparator to be used.
         */

        public void setSliceSeparator(string sliceSeparator)
        {
            m_sliceSeparator = sliceSeparator;
        }

        /**
         * Cache for faster string processing.
         */

        public static void setupBlanksCache()
        {
            // Pre-fabricate 40 static strings with 0,1,2,..,39 blanks, for usage within method blanks(Length).
            // Now, we don't need to construct and fill them on demand, and garbage collect them again.
            // All 40 strings share the identical char[] array, only with different offset and Length --> somewhat smaller static memory footprint
            int size = 40;
            m_blanksCache = new string[size];
            StringBuilder buf = new StringBuilder(size);
            for (int i = size; --i >= 0;)
            {
                buf.Append(' ');
            }
            string str = buf.ToString();
            for (int i = size; --i >= 0;)
            {
                m_blanksCache[i] = str.Substring(0, i);
                //PrintToScreen.WriteLine(i+"-"+blanksCache[i]+"-");
            }
        }

        /**
         * Returns a short string representation describing the shape of the matrix.
         */

        public static string shape(AbstractMatrix1D matrix)
        {
            //return "Matrix1D of size="+matrix.Size();
            //return matrix.Size()+" element matrix";
            //return "matrix("+matrix.Size()+")";
            return matrix.Size() + " matrix";
        }

        /**
         * Returns a short string representation describing the shape of the matrix.
         */

        public static string shape(AbstractMatrix2D matrix)
        {
            return matrix.Rows() + " x " + matrix.Columns() + " matrix";
        }

        /**
         * Returns a short string representation describing the shape of the matrix.
         */

        public static string shape(AbstractMatrix3D matrix)
        {
            return matrix.Slices() + " x " + matrix.Rows() + " x " + matrix.Columns() + " matrix";
        }

        /**
         * Returns a single string representation of the given string matrix.
         * @param strings the matrix to be converted to a single string.
         */

        public string ToString(string[,] strings)
        {
            int rows = strings.GetLength(0);
            int columns = strings.GetLength(1) <= 0 ? 0 : strings.GetLength(1);

            StringBuilder total = new StringBuilder();
            StringBuilder s = new StringBuilder();
            for (int row = 0; row < rows; row++)
            {
                //s.setLength(0);
                for (int column = 0; column < columns; column++)
                {
                    s.Append(strings[row, column]);
                    if (column < columns - 1)
                    {
                        s.Append(m_columnSeparator);
                    }
                }
                total.Append(s);
                if (row < rows - 1)
                {
                    total.Append(m_rowSeparator);
                }
            }

            return total.ToString();
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(AbstractMatrix2D matrix)
        {
            string[,] strings = format(matrix);
            align(strings);
            StringBuilder total = new StringBuilder(ToString(strings));
            if (m_printShape)
            {
                total.Insert(0, shape(matrix) + Environment.NewLine);
            }
            return total.ToString();
        }
    }
}
