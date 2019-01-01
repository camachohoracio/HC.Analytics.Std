#region

using System;
using System.Text;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt.objectAlgo
{
    //package cern.colt.matrix.objectalgo;

    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //import cern.colt.matrix.ObjectMatrix1D;
    //import cern.colt.matrix.ObjectMatrix2D;
    //import cern.colt.matrix.ObjectMatrix3D;
    //import cern.colt.matrix.impl.AbstractFormatter;
    //import cern.colt.matrix.impl.AbstractMatrix1D;
    //import cern.colt.matrix.impl.AbstractMatrix2D;
    //import cern.colt.matrix.impl.Former;
    /** 
    Flexible, well human readable matrix print formatting.
    Each cell is converted using {@link Object#ToString()}.
    For examples see {@link cern.colt.matrix.doublealgo.Formatter doublealgo.Formatter} which is just the same except that it operates on doubles.

    @author wolfgang.hoschek@cern.ch
    @version 1.1, 11/22/99
    */

    [Serializable]
    public class FormatterObjectAlgo : AbstractFormatter
    {
        /**
         * Constructs and returns a matrix formatter with alignment <tt>LEFT</tt>.
         */

        public FormatterObjectAlgo()
            : this(LEFT)
        {
        }

        /**
         * Constructs and returns a matrix formatter.
         * @param alignment the given alignment used to align a column.
         */

        public FormatterObjectAlgo(string alignment)
        {
            setAlignment(alignment);
        }

        /**
         * Converts a given cell to a string; no alignment considered.
         */

        public override string form(AbstractMatrix1D matrix, int index, Former formatter)
        {
            return form((ObjectMatrix1D) matrix, index, formatter);
        }

        /**
         * Converts a given cell to a string; no alignment considered.
         */

        public string form(ObjectMatrix1D matrix, int index, Former formatter)
        {
            Object value = matrix.get(index);
            if (value == null)
            {
                return "";
            }
            return value.ToString();
        }

        /**
         * Returns a string representations of all cells; no alignment considered.
         */

        public override string[,] format(AbstractMatrix2D matrix)
        {
            return format((ObjectMatrix2D) matrix);
        }

        /**
         * Returns a string representations of all cells; no alignment considered.
         */

        public string[,] format(ObjectMatrix2D matrix)
        {
            string[,] strings = new string[matrix.Rows(),matrix.Columns()];
            for (int row = matrix.Rows(); --row >= 0;)
            {
                ArrayHelper.SetRow(
                    strings,
                    formatRow(matrix.viewRow(row)),
                    row);
            }
            return strings;
        }

        /**
         * Returns a string <tt>s</tt> such that <tt>Object[] m = s</tt> is a legal Java statement.
         * @param matrix the matrix to format.
         */

        public string toSourceCode(ObjectMatrix1D matrix)
        {
            Formatter copy = (Formatter) Clone();
            copy.setPrintShape(false);
            copy.setColumnSeparator(", ");
            string lead = "{";
            string trail = "};";
            return lead + copy.ToString(matrix) + trail;
        }

        /**
         * Returns a string <tt>s</tt> such that <tt>Object[] m = s</tt> is a legal Java statement.
         * @param matrix the matrix to format.
         */

        public string toSourceCode(ObjectMatrix2D matrix)
        {
            Formatter copy = (Formatter) Clone();
            string b3 = blanks(3);
            copy.setPrintShape(false);
            copy.setColumnSeparator(", ");
            copy.setRowSeparator("},\n" + b3 + "{");
            string lead = "{\n" + b3 + "{";
            string trail = "}\n};";
            return lead + copy.ToString(matrix) + trail;
        }

        /**
         * Returns a string <tt>s</tt> such that <tt>Object[] m = s</tt> is a legal Java statement.
         * @param matrix the matrix to format.
         */

        public string toSourceCode(ObjectMatrix3D matrix)
        {
            Formatter copy = (Formatter) Clone();
            string b3 = blanks(3);
            string b6 = blanks(6);
            copy.setPrintShape(false);
            copy.setColumnSeparator(", ");
            copy.setRowSeparator("},\n" + b6 + "{");
            copy.setSliceSeparator("}\n" + b3 + "},\n" + b3 + "{\n" + b6 + "{");
            string lead = "{\n" + b3 + "{\n" + b6 + "{";
            string trail = "}\n" + b3 + "}\n}";
            return lead + copy.ToString(matrix) + trail;
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public new string ToString(AbstractMatrix2D matrix)
        {
            return ToString((ObjectMatrix2D) matrix);
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(ObjectMatrix1D matrix)
        {
            ObjectMatrix2D easy = matrix.like2D(1, matrix.Size());
            easy.viewRow(0).assign(matrix);
            return ToString(easy);
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(ObjectMatrix2D matrix)
        {
            return base.ToString(matrix);
        }

        /**
         * Returns a string representation of the given matrix.
         * @param matrix the matrix to convert.
         */

        public string ToString(ObjectMatrix3D matrix)
        {
            StringBuilder buf = new StringBuilder();
            bool oldPrintShape = m_printShape;
            m_printShape = false;
            for (int slice = 0; slice < matrix.Slices(); slice++)
            {
                if (slice != 0)
                {
                    buf.Append(m_sliceSeparator);
                }
                buf.Append(ToString(matrix.viewSlice(slice)));
            }
            m_printShape = oldPrintShape;
            if (m_printShape)
            {
                buf.Insert(0, shape(matrix) + Environment.NewLine);
            }
            return buf.ToString();
        }

        /**
        Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        Pass <tt>null</tt> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

        @param matrix The matrix to format.
        @param rowNames The headers of all rows (to be put to the left of the matrix).
        @param columnNames The headers of all columns (to be put to above the matrix).
        @param rowAxisName The label of the y-axis.
        @param columnAxisName The label of the x-axis.
        @param title The overall title of the matrix to be formatted.
        @return the matrix converted to a string.
        */

        public string toTitleString(ObjectMatrix2D matrix, string[] rowNames, string[] columnNames, string rowAxisName,
                                    string columnAxisName, string title)
        {
            if (matrix.Size() == 0)
            {
                return "Empty matrix";
            }
            string oldFormat = m_strFormat;
            m_strFormat = LEFT;

            int rows = matrix.Rows();
            int columns = matrix.Columns();

            // determine how many rows and columns are needed
            int r = 0;
            int c = 0;
            r += (columnNames == null ? 0 : 1);
            c += (rowNames == null ? 0 : 1);
            c += (rowAxisName == null ? 0 : 1);
            c += (rowNames != null || rowAxisName != null ? 1 : 0);

            int height = r + Math.Max(rows, rowAxisName == null ? 0 : rowAxisName.Length);
            int width = c + columns;

            // make larger matrix holding original matrix and naming strings
            ObjectMatrix2D titleMatrix = matrix.like(height, width);

            // insert original matrix into larger matrix
            titleMatrix.viewPart(r, c, rows, columns).assign(matrix);

            // insert column axis name in leading row
            if (r > 0)
            {
                titleMatrix.viewRow(0).viewPart(c, columns).assign(columnNames);
            }

            // insert row axis name in leading column
            if (rowAxisName != null)
            {
                string[] rowAxisStrings = new string[rowAxisName.Length];
                for (int i = rowAxisName.Length; --i >= 0;)
                {
                    rowAxisStrings[i] = rowAxisName.Substring(i, i + 1);
                }
                titleMatrix.viewColumn(0).viewPart(r, rowAxisName.Length).assign(rowAxisStrings);
            }
            // insert row names in next leading columns
            if (rowNames != null)
            {
                titleMatrix.viewColumn(c - 2).viewPart(r, rows).assign(rowNames);
            }

            // insert vertical "---------" separator line in next leading column
            if (c > 0)
            {
                titleMatrix.viewColumn(c - 2 + 1).viewPart(0, rows + r).assign("|");
            }

            // convert the large matrix to a string
            bool oldPrintShape = m_printShape;
            m_printShape = false;
            string str = ToString(titleMatrix);
            m_printShape = oldPrintShape;

            // insert horizontal "--------------" separator line
            StringBuilder total = new StringBuilder(str);
            if (columnNames != null)
            {
                int i = str.IndexOf(m_rowSeparator);
                total.Insert(i + 1, repeat('-', i) + m_rowSeparator);
            }
            else if (columnAxisName != null)
            {
                int i = str.IndexOf(m_rowSeparator);
                total.Insert(0, repeat('-', i) + m_rowSeparator);
            }

            // insert line for column axis name
            if (columnAxisName != null)
            {
                int j = 0;
                if (c > 0)
                {
                    j = str.IndexOf('|');
                }
                string s = blanks(j);
                if (c > 0)
                {
                    s = s + "| ";
                }
                s = s + columnAxisName + Environment.NewLine;
                total.Insert(0, s);
            }

            // insert title
            if (title != null)
            {
                total.Insert(0, title + Environment.NewLine);
            }

            m_strFormat = oldFormat;

            return total.ToString();
        }

        /**
        Returns a string representation of the given matrix with axis as well as rows and columns labeled.
        Pass <tt>null</tt> to one or more parameters to indicate that the corresponding decoration element shall not appear in the string converted matrix.

        @param matrix The matrix to format.
        @param sliceNames The headers of all slices (to be put above each slice).
        @param rowNames The headers of all rows (to be put to the left of the matrix).
        @param columnNames The headers of all columns (to be put to above the matrix).
        @param sliceAxisName The label of the z-axis (to be put above each slice).
        @param rowAxisName The label of the y-axis.
        @param columnAxisName The label of the x-axis.
        @param title The overall title of the matrix to be formatted.
        @return the matrix converted to a string.
        */

        public string toTitleString(ObjectMatrix3D matrix, string[] sliceNames, string[] rowNames, string[] columnNames,
                                    string sliceAxisName, string rowAxisName, string columnAxisName, string title)
        {
            if (matrix.Size() == 0)
            {
                return "Empty matrix";
            }
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < matrix.Slices(); i++)
            {
                if (i != 0)
                {
                    buf.Append(m_sliceSeparator);
                }
                buf.Append(toTitleString(matrix.viewSlice(i), rowNames, columnNames, rowAxisName, columnAxisName,
                                         title + Environment.NewLine + sliceAxisName + "=" + sliceNames[i]));
            }
            return buf.ToString();
        }
    }
}
