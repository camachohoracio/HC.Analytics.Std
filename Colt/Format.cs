#region

using System;
using System.Text;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Colt
{
    /*
        Format - printf style formatting for Java
        Copyright (C) 1995...2002 Cay S. Horstmann (http://horstmann.com)

        This library is free software; you can redistribute it and/or
        modify it under the terms of the GNU Lesser General Public
        License as published by the Free Software Foundation; either
        version 2.1 of the License, or (at your option) any later version.

        This library is distributed in the hope that it will be useful,
        but WITHOUT ANY WARRANTY; without even the implied warranty of
        MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
        Lesser General Public License for more details.

        You should have received a copy of the GNU Lesser General Public
        License along with this library; if not, write to the Free Software
        Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
    */

    ////package corejava;

    ////import java.io.*;

    /**
       A class for formatting numbers that follows <tt>printf</tt> conventions.

       Also , C-like <tt>atoi</tt> and <tt>atof</tt> functions

       @version 1.22 2002-11-16
       @author Cay Horstmann

       1998-09-14: Fixed a number of bugs.
            1.Formatting the most extreme negative number (-9223372036854775808L) printed with 2 leading minus signs.
            2.Printing 0 with a %e or %g format did not work.
            3.Printing numbers that were closer to 1 than the number of requested decimal places rounded down rather than up, e.g. formatting 1.999 with %.2f printed 1.00. (This one is pretty serious, of course.)
            4.Printing with precision 0 (e.g %10.0f) didn't work.
            5.Printing a string with a precision that exceeded the string Length (e.g. print "Hello" with %20.10s) caused a StringIndexOutOfBounds error.
       1998-10-21: Changed method names from print to printf
       2000-06-09: Moved to //package com.horstmann; no longer part of
       Core Java
       2000-06-09: Fixed a number of bugs.
            1.Printing 100.0 with %e printed 10.0e1, not 1.0e2
            2.Printing Inf and NaN didn't work.
       2000-06-09: Coding guideline cleanup
       2002-11-16: Move to //package com.horstmann.format; licensed under LGPL
*/

    [Serializable]
    public class Format
    {
        /**
	  Formats the number following <tt>printf</tt> conventions.
	  Main limitation: Can only handle one format parameter at a time
	  Use multiple Format objects to format more than one number
	  @param s the format string following printf conventions
	  The string has a prefix, a format code and a suffix. The prefix and suffix
	  become part of the formatted output. The format code directs the
	  formatting of the (single) parameter to be formatted. The code has the
	  following structure
	  <ul>
	  <li> a % (required)
	  <li> a modifier (optional)
	  <dl>
	  <dt> + <dd> forces display of + for positive numbers
	  <dt> 0 <dd> show leading zeroes
	  <dt> - <dd> align left in the field
	  <dt> space <dd> prepend a space in front of positive numbers
	  <dt> # <dd> use "alternate" format. Add 0 or 0x for octal or hexadecimal numbers. Don't suppress trailing zeroes in general floating point format.
	  </dl>
	  <li> an integer denoting field width (optional)
	  <li> a period followed by an integer denoting precision (optional)
	  <li> a format descriptor (required)
	  <dl>
	  <dt>f <dd> floating point number in fixed format
	  <dt>e, E <dd> floating point number in exponential notation (scientific format). The E format results in an uppercase E for the exponent (1.14130E+003), the e format in a lowercase e.
	  <dt>g, G <dd> floating point number in general format (fixed format for small numbers, exponential format for large numbers). Trailing zeroes are suppressed. The G format results in an uppercase E for the exponent (if any), the g format in a lowercase e.
	  <dt>d, i <dd> integer in decimal
	  <dt>x <dd> integer in hexadecimal
	  <dt>o <dd> integer in octal
	  <dt>s <dd> string
	  <dt>c <dd> character
	  </dl>
	  </ul>
	  @exception ArgumentException if bad format
   */

        private readonly bool alternate;
        private readonly char fmt; // one of cdeEfgGiosxXos
        private readonly bool leadingZeroes;
        private readonly bool leftAlign;
        private readonly string post;
        private readonly string pre;
        private readonly bool showPlus;
        private readonly bool showSpace;
        private readonly int width;
        private int precision;

        public Format(string s)
        {
            width = 0;
            precision = -1;
            pre = "";
            post = "";
            leadingZeroes = false;
            showPlus = false;
            alternate = false;
            showSpace = false;
            leftAlign = false;
            fmt = ' ';

            //int state = 0;
            int Length = s.Length;
            int parseState = 0;
            // 0 = prefix, 1 = flags, 2 = width, 3 = precision,
            // 4 = format, 5 = end
            int i = 0;

            while (parseState == 0)
            {
                if (i >= Length)
                {
                    parseState = 5;
                }
                else if (s[i] == '%')
                {
                    if (i < Length - 1)
                    {
                        if (s[i + 1] == '%')
                        {
                            pre = pre + '%';
                            i++;
                        }
                        else
                        {
                            parseState = 1;
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    pre = pre + s[i];
                }
                i++;
            }
            while (parseState == 1)
            {
                if (i >= Length)
                {
                    parseState = 5;
                }
                else if (s[i] == ' ')
                {
                    showSpace = true;
                }
                else if (s[i] == '-')
                {
                    leftAlign = true;
                }
                else if (s[i] == '+')
                {
                    showPlus = true;
                }
                else if (s[i] == '0')
                {
                    leadingZeroes = true;
                }
                else if (s[i] == '#')
                {
                    alternate = true;
                }
                else
                {
                    parseState = 2;
                    i--;
                }
                i++;
            }
            while (parseState == 2)
            {
                if (i >= Length)
                {
                    parseState = 5;
                }
                else if ('0' <= s[i] && s[i] <= '9')
                {
                    width = width*10 + s[i] - '0';
                    i++;
                }
                else if (s[i] == '.')
                {
                    parseState = 3;
                    precision = 0;
                    i++;
                }
                else
                {
                    parseState = 4;
                }
            }
            while (parseState == 3)
            {
                if (i >= Length)
                {
                    parseState = 5;
                }
                else if ('0' <= s[i] && s[i] <= '9')
                {
                    precision = precision*10 + s[i] - '0';
                    i++;
                }
                else
                {
                    parseState = 4;
                }
            }
            if (parseState == 4)
            {
                if (i >= Length)
                {
                    parseState = 5;
                }
                else
                {
                    fmt = s[i];
                }
                i++;
            }
            if (i < Length)
            {
                post = s.Substring(i, Length);
            }
        }

        /**
	 prints a formatted number following printf conventions
	 @param fmt the format string
	 @param x the double to print
   */

        public static void printf(string fmt, double x)
        {
            Console.Write(new Format(fmt).format(x));
        }

        /**
	  prints a formatted number following printf conventions
	  @param fmt the format string
	  @param x the int to print
   */

        public static void printf(string fmt, int x)
        {
            Console.Write(new Format(fmt).format(x));
        }

        /**
	  prints a formatted number following printf conventions
	  @param fmt the format string
	  @param x the long to print
   */

        public static void printf(string fmt, long x)
        {
            Console.Write(new Format(fmt).format(x));
        }

        /**
	  prints a formatted number following printf conventions
	  @param fmt the format string
	  @param x the character to print
   */

        public static void printf(string fmt, char x)
        {
            Console.Write(new Format(fmt).format(x));
        }

        /**
	  prints a formatted number following printf conventions
	  @param fmt the format string
	  @param x a string to print
   */

        public static void printf(string fmt, string x)
        {
            Console.Write(new Format(fmt).format(x));
        }

        /**
	  Converts a string of digits (decimal, octal or hex) to an integer
	  @param s a string
	  @return the numeric value of the prefix of s representing a base_ 10 integer
   */

        public static int atoi(string s)
        {
            return (int) atol(s);
        }

        /**
	 Converts a string of digits (decimal, octal or hex) to a long integer
	 @param s a string
	 @return the numeric value of the prefix of s representing a base_ 10 integer
  */

        public static long atol(string s)
        {
            int i = 0;

            while (i < s.Length && char.IsWhiteSpace(s[i]))
            {
                i++;
            }
            if (i < s.Length && s[i] == '0')
            {
                if (i + 1 < s.Length && (s[i + 1] == 'x' || s[i + 1] == 'X'))
                {
                    return Parse(s.Substring(i + 2), 16);
                }
                else
                {
                    return Parse(s, 8);
                }
            }
            else
            {
                return Parse(s, 10);
            }
        }

        private static long Parse(string s, int base_)
        {
            int i = 0;
            int sign = 1;
            long r = 0;

            while (i < s.Length && char.IsWhiteSpace(s[i]))
            {
                i++;
            }
            if (i < s.Length && s[i] == '-')
            {
                sign = -1;
                i++;
            }
            else if (i < s.Length && s[i] == '+')
            {
                i++;
            }
            while (i < s.Length)
            {
                char ch = s[i];
                if ('0' <= ch && ch < '0' + base_)
                {
                    r = r*base_ + ch - '0';
                }
                else if ('A' <= ch && ch < 'A' + base_ - 10)
                {
                    r = r*base_ + ch - 'A' + 10;
                }
                else if ('a' <= ch && ch < 'a' + base_ - 10)
                {
                    r = r*base_ + ch - 'a' + 10;
                }
                else
                {
                    return r*sign;
                }
                i++;
            }
            return r*sign;
        }

        /**
	  Converts a string of digits to a <tt>double</tt>
	  @param s a string
   */

        public static double atof(string s)
        {
            int i = 0;
            int sign = 1;
            double r = 0; // integer part
            //double f = 0; // fractional part
            double p = 1; // exponent of fractional part
            int state = 0; // 0 = int part, 1 = frac part

            while (i < s.Length && char.IsWhiteSpace(s[i]))
            {
                i++;
            }
            if (i < s.Length && s[i] == '-')
            {
                sign = -1;
                i++;
            }
            else if (i < s.Length && s[i] == '+')
            {
                i++;
            }
            while (i < s.Length)
            {
                char ch = s[i];
                if ('0' <= ch && ch <= '9')
                {
                    if (state == 0)
                    {
                        r = r*10 + ch - '0';
                    }
                    else if (state == 1)
                    {
                        p = p/10;
                        r = r + p*(ch - '0');
                    }
                }
                else if (ch == '.')
                {
                    if (state == 0)
                    {
                        state = 1;
                    }
                    else
                    {
                        return sign*r;
                    }
                }
                else if (ch == 'e' || ch == 'E')
                {
                    long e = (int) Parse(s.Substring(i + 1), 10);
                    return sign*r*Math.Pow(10, e);
                }
                else
                {
                    return sign*r;
                }
                i++;
            }
            return sign*r;
        }

        /**
	  Formats a <tt>double</tt> into a string (like sprintf in C)
	  @param x the number to format
	  @return the formatted string
	  @exception ArgumentException if bad argument
   */

        public string format(double x)
        {
            string r;
            if (precision < 0)
            {
                precision = 6;
            }
            int s = 1;
            if (x < 0)
            {
                x = -x;
                s = -1;
            }
            if (Double.IsNaN(x))
            {
                r = "NaN";
            }
            else if (x == Double.PositiveInfinity)
            {
                r = "Inf";
            }
            else if (fmt == 'f')
            {
                r = fixedFormat(x);
            }
            else if (fmt == 'e' || fmt == 'E' || fmt == 'g' || fmt == 'G')
            {
                r = expFormat(x);
            }
            else
            {
                throw new HCException();
            }

            return pad(sign(s, r));
        }

        /**
	  Formats an integer into a string (like sprintf in C)
	  @param x the number to format
	  @return the formatted string
   */

        public string format(int x)
        {
            long lx = x;
            if (fmt == 'o' || fmt == 'x' || fmt == 'X')
            {
                lx &= 0xFFFFFFFFL;
            }
            return format(lx);
        }

        /**
	  Formats a long integer into a string (like sprintf in C)
	  @param x the number to format
	  @return the formatted string
   */

        public string format(long x)
        {
            string r;
            int s = 0;
            if (fmt == 'd' || fmt == 'i')
            {
                if (x < 0)
                {
                    r = ("" + x).Substring(1);
                    s = -1;
                }
                else
                {
                    r = "" + x;
                    s = 1;
                }
            }
            else if (fmt == 'o')
            {
                r = convert(x, 3, 7, "01234567");
            }
            else if (fmt == 'x')
            {
                r = convert(x, 4, 15, "0123456789abcdef");
            }
            else if (fmt == 'X')
            {
                r = convert(x, 4, 15, "0123456789ABCDEF");
            }
            else
            {
                throw new HCException();
            }

            return pad(sign(s, r));
        }

        /**
	  Formats a character into a string (like sprintf in C)
	  @param x the value to format
	  @return the formatted string
   */

        public string format(char c)
        {
            if (fmt != 'c')
            {
                throw new HCException();
            }

            string r = "" + c;
            return pad(r);
        }

        /**
	  Formats a string into a larger string (like sprintf in C)
	  @param x the value to format
	  @return the formatted string
   */

        public string format(string s)
        {
            if (fmt != 's')
            {
                throw new HCException();
            }
            if (precision >= 0 && precision < s.Length)
            {
                s = s.Substring(0, precision);
            }
            return pad(s);
        }


        /**
	  a test stub for the format class
   */

        public static void main(string[] a)
        {
            double x = 1.23456789012;
            double y = 123;
            double z = 1.2345e30;
            double w = 1.02;
            double u = 1.234e-5;
            double v = 10.0;
            int d = 0xCAFE;
            printf("x = |%f|\n", x);
            printf("u = |%20f|\n", u);
            printf("x = |% .5f|\n", x);
            printf("w = |%20.5f|\n", w);
            printf("x = |%020.5f|\n", x);
            printf("x = |%+20.5f|\n", x);
            printf("x = |%+020.5f|\n", x);
            printf("x = |% 020.5f|\n", x);
            printf("y = |%#+20.5f|\n", y);
            printf("y = |%-+20.5f|\n", y);
            printf("z = |%20.5f|\n", z);

            printf("x = |%e|\n", x);
            printf("u = |%20e|\n", u);
            printf("x = |% .5e|\n", x);
            printf("w = |%20.5e|\n", w);
            printf("x = |%020.5e|\n", x);
            printf("x = |%+20.5e|\n", x);
            printf("x = |%+020.5e|\n", x);
            printf("x = |% 020.5e|\n", x);
            printf("y = |%#+20.5e|\n", y);
            printf("y = |%-+20.5e|\n", y);
            printf("v = |%12.5e|\n", v);

            printf("x = |%g|\n", x);
            printf("z = |%g|\n", z);
            printf("w = |%g|\n", w);
            printf("u = |%g|\n", u);
            printf("y = |%.2g|\n", y);
            printf("y = |%#.2g|\n", y);

            printf("d = |%d|\n", d);
            printf("d = |%20d|\n", d);
            printf("d = |%020d|\n", d);
            printf("d = |%+20d|\n", d);
            printf("d = |% 020d|\n", d);
            printf("d = |%-20d|\n", d);
            printf("d = |%20.8d|\n", d);
            printf("d = |%x|\n", d);
            printf("d = |%20X|\n", d);
            printf("d = |%#20x|\n", d);
            printf("d = |%020X|\n", d);
            printf("d = |%20.8x|\n", d);
            printf("d = |%o|\n", d);
            printf("d = |%020o|\n", d);
            printf("d = |%#20o|\n", d);
            printf("d = |%#020o|\n", d);
            printf("d = |%20.12o|\n", d);

            printf("s = |%-20s|\n", "Hello");
            printf("s = |%-20c|\n", '!');

            // regression test to confirm fix of reported bugs

            Format.printf("|%i|\n", long.MinValue);

            printf("|%6.2e|\n", 0.0);
            printf("|%6.2g|\n", 0.0);

            printf("|%6.2f|\n", 9.99);
            printf("|%6.2f|\n", 9.999);
            printf("|%.2f|\n", 1.999);

            printf("|%6.0f|\n", 9.999);
            printf("|%20.10s|\n", "Hello");
            d = -1;
            printf("-1 = |%X|\n", d);

            printf("100 = |%e|\n", 100.0); // 2000-06-09
            printf("1/0 = |%f|\n", 1.0/0.0);
            printf("-1/0 = |%e|\n", -1.0/0.0);
            printf("0/0 = |%g|\n", 0.0/0.0);
        }

        private static string repeat(char c, int n)
        {
            if (n <= 0)
            {
                return "";
            }
            StringBuilder s = new StringBuilder(n);
            for (int i = 0; i < n; i++)
            {
                s.Append(c);
            }
            return s.ToString();
        }

        private static string convert(long x, int n, int m, string d)
        {
            if (x == 0)
            {
                return "0";
            }
            string r = "";
            while (x != 0)
            {
                r = d[(int) (x & m)] + r;
                x = x >> n;
            }
            return r;
        }

        private string pad(string r)
        {
            string p = repeat(' ', width - r.Length);
            if (leftAlign)
            {
                return pre + r + p + post;
            }
            else
            {
                return pre + p + r + post;
            }
        }

        private string sign(int s, string r)
        {
            string p = "";
            if (s < 0)
            {
                p = "-";
            }
            else if (s > 0)
            {
                if (showPlus)
                {
                    p = "+";
                }
                else if (showSpace)
                {
                    p = " ";
                }
            }
            else
            {
                if (fmt == 'o' && alternate && r.Length > 0 && r[0] != '0')
                {
                    p = "0";
                }
                else if (fmt == 'x' && alternate)
                {
                    p = "0x";
                }
                else if (fmt == 'X' && alternate)
                {
                    p = "0X";
                }
            }
            int w = 0;
            if (leadingZeroes)
            {
                w = width;
            }
            else if ((fmt == 'd' || fmt == 'i' || fmt == 'x' || fmt == 'X' || fmt == 'o')
                     && precision > 0)
            {
                w = precision;
            }

            return p + repeat('0', w - p.Length - r.Length) + r;
        }

        private string fixedFormat(double d)
        {
            bool removeTrailing
                = (fmt == 'G' || fmt == 'g') && !alternate;
            // remove trailing zeroes and decimal point

            if (d > 0x7FFFFFFFFFFFFFFFL)
            {
                return expFormat(d);
            }
            if (precision == 0)
            {
                return (long) (d + 0.5) + (removeTrailing ? "" : ".");
            }

            long whole = (long) d;
            double fr = d - whole; // fractional part
            if (fr >= 1 || fr < 0)
            {
                return expFormat(d);
            }

            double factor = 1;
            string leadingZeroes = "";
            for (int i = 1; i <= precision && factor <= 0x7FFFFFFFFFFFFFFFL; i++)
            {
                factor *= 10;
                leadingZeroes = leadingZeroes + "0";
            }
            long l = (long) (factor*fr + 0.5);
            if (l >= factor)
            {
                l = 0;
                whole++;
            } // CSH 10-25-97

            string z = leadingZeroes + l;
            int intLength = z.Length - (z.Length - precision);
            z = "." + z.Substring(z.Length - precision - 1, intLength);

            if (removeTrailing)
            {
                int t = z.Length - 1;
                while (t >= 0 && z[t] == '0')
                {
                    t--;
                }
                if (t >= 0 && z[t] == '.')
                {
                    t--;
                }
                z = z.Substring(0, t + 1);
            }

            return whole + z;
        }

        private string expFormat(double d)
        {
            string f = "";
            int e = 0;
            double dd = d;
            double factor = 1;
            if (d != 0)
            {
                while (dd >= 10)
                {
                    e++;
                    factor /= 10;
                    dd = dd/10;
                } // 2000-06-09
                while (dd < 1)
                {
                    e--;
                    factor *= 10;
                    dd = dd*10;
                }
            }
            if ((fmt == 'g' || fmt == 'G') && e >= -4 && e < precision)
            {
                return fixedFormat(d);
            }

            d = d*factor;
            f = f + fixedFormat(d);

            if (fmt == 'e' || fmt == 'g')
            {
                f = f + "e";
            }
            else
            {
                f = f + "E";
            }

            string p = "000";
            if (e >= 0)
            {
                f = f + "+";
                p = p + e;
            }
            else
            {
                f = f + "-";
                p = p + (-e);
            }

            return f + p.Substring(p.Length - 3, p.Length);
        }
    }
}
