#region

using System;
using HC.Analytics.Mathematics;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Statistics
{
    /*
    *   Class   ErrorProp
    *
    *   Defines an object consisting of a variable and its associated standard
    *   deviation and the class includes the methods for propagating the error
    *   in standard arithmetic operations for both correlated and uncorrelated
    *   errors.
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:    October 2002
    *   UPDATE:  26 April 2004, 19 January 2005
    *
    *   See ComplexErrorProp for the propogation of errors in complex arithmetic
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web page:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/ErrorProp.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    *   Copyright (c) 2002 - 2008  Michael Thomas Flanagan
    *
    *   PERMISSION TO COPY:
    *
    *   Redistributions of this source code, or parts of, must retain the above
    *   copyright notice, this list of conditions and the following disclaimer.
    *
    *   Redistribution in binary form of all or parts of this class, must reproduce
    *   the above copyright, this list of conditions and the following disclaimer in
    *   the documentation and/or other materials provided with the distribution.
    *
    *   Permission to use, copy and modify this software and its documentation for
    *   NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
    *   to the author, Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all
    *   copies and associated documentation or publications.
    *
    *   Dr Michael Thomas Flanagan makes no representations about the suitability
    *   or fitness of the software for any or for a particular purpose.
    *   Michael Thomas Flanagan shall not be liable for any damages suffered
    *   as a result of using, modifying or distributing this software or its derivatives.
    *
    ***************************************************************************************/

    public class ErrorProp
    {
        // DATA VARIABLES
        private double m_error; // its standard deviation or an estimate of its standard deviation
        private double m_value; // number value


        // CONSTRUCTORS
        // default constructor
        public ErrorProp()
        {
            m_value = 0.0;
            m_error = 0.0;
        }

        // constructor with value and error initialised
        public ErrorProp(double value, double error)
        {
            m_value = value;
            m_error = error;
        }

        // constructor with value initialised
        public ErrorProp(double value)
        {
            m_value = value;
            m_error = 0.0;
        }

        // PUBLIC METHODS

        // SET VALUES
        // Set the value of value
        public void setValue(double value)
        {
            m_value = value;
        }

        // Set the value of error
        public void setError(double error)
        {
            m_error = error;
        }

        // Set the values of value and error
        public void reset(double value, double error)
        {
            m_value = value;
            m_error = error;
        }

        // GET VALUES
        // Get the value of value
        public double getValue()
        {
            return m_value;
        }

        // Get the value of error
        public double getError()
        {
            return m_error;
        }

        //PRINT AN ERROR NUMBER
        // Print to terminal window with text (message) and a line return
        public void println(string message)
        {
            PrintToScreen.WriteLine(message + " " + ToString());
        }

        // Print to terminal window without text (message) but with a line return
        public void println()
        {
            PrintToScreen.WriteLine(" " + ToString());
        }

        // Print to terminal window with text (message) but without line return
        public void print(string message)
        {
            Console.Write(message + " " + ToString());
        }

        // Print to terminal window without text (message) and without line return
        public void print()
        {
            Console.Write(" " + ToString());
        }

        // TRUNCATION
        // Rounds the mantissae of both the value and error parts of Errorprop to prec places
        public static ErrorProp truncate(ErrorProp x, int prec)
        {
            if (prec < 0)
            {
                return x;
            }

            double xV = x.getValue();
            double xS = x.getError();
            ErrorProp y = new ErrorProp();

            xV = Fmath.truncate(xV, prec);
            xS = Fmath.truncate(xS, prec);

            y.reset(xV, xS);

            return y;
        }

        // instance method
        public ErrorProp truncate(int prec)
        {
            if (prec < 0)
            {
                return this;
            }

            double xV = getValue();
            double xS = getError();
            ErrorProp y = new ErrorProp();

            xV = Fmath.truncate(xV, prec);
            xS = Fmath.truncate(xS, prec);

            y.reset(xV, xS);

            return y;
        }

        // CONVERSIONS
        // Format an ErrorProp number as a string
        // Overides java.lang.string.ToString()
        public override string ToString()
        {
            return m_value + ", error = " + m_error;
        }

        // Format an ErrorProp number as a string
        // See static method above for comments
        public static string ToString(ErrorProp aa)
        {
            return aa.m_value + ", error = " + aa.m_error;
        }

        // Return a HASH CODE for the ErrorProp number
        // Overides java.lang.Object.hashCode()
        public int hashCode()
        {
            long lvalue = (long) m_value;
            long lerror = (long) m_error;
            int hvalue = (int) (lvalue ^ (lvalue));
            int herror = (int) (lerror ^ (lerror));
            return 7*(hvalue/10) + 3*(herror/10);
        }


        // ARRAYS

        // Create a one dimensional array of ErrorProp objects of Length n
        // all values = 0 and all error's = 0
        public static ErrorProp[] oneDarray(int n)
        {
            ErrorProp[] a = new ErrorProp[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = zero();
            }
            return a;
        }

        // Create a one dimensional array of ErrorProp objects of Length n
        // all values = a and all error's = b
        public static ErrorProp[] oneDarray(int n, double a, double b)
        {
            ErrorProp[] c = new ErrorProp[n];
            for (int i = 0; i < n; i++)
            {
                c[i] = zero();
                c[i].reset(a, b);
            }
            return c;
        }

        // Create a one dimensional array of ErrorProp objects of Length n
        // all = the ErrorProp number named constant
        public static ErrorProp[] oneDarray(int n, ErrorProp constant)
        {
            ErrorProp[] c = new ErrorProp[n];
            for (int i = 0; i < n; i++)
            {
                c[i] = Copy(constant);
            }
            return c;
        }

        // Create a two dimensional array of ErrorProp objects of dimensions n and m
        // all values = zero and all error's = zero
        public static ErrorProp[,] twoDarray(int n, int m)
        {
            ErrorProp[,] a = new ErrorProp[n,m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    a[i, j] = zero();
                }
            }
            return a;
        }

        // Create a two dimensional array of ErrorProp objects of dimensions n and m
        // all values = a and all error's = b
        public static ErrorProp[,] twoDarray(int n, int m, double a, double b)
        {
            ErrorProp[,] c = new ErrorProp[n,m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    c[i, j] = zero();
                    c[i, j].reset(a, b);
                }
            }
            return c;
        }

        // Create a two dimensional array of ErrorProp objects of dimensions n and m
        // all  =  the ErrorProp number named constant
        public static ErrorProp[,] twoDarray(int n, int m, ErrorProp constant)
        {
            ErrorProp[,] c = new ErrorProp[n,m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    c[i, j] = Copy(constant);
                }
            }
            return c;
        }

        // COPY
        // Copy a single ErrorProp number [static method]
        public static ErrorProp Copy(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = a.m_value;
            b.m_error = a.m_error;
            return b;
        }

        // Copy a single ErrorProp number [instance method]
        public ErrorProp Copy()
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value;
            b.m_error = m_error;
            return b;
        }

        // Clone a single ErrorProp number
        public Object clone()
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value;
            b.m_error = m_error;
            return b;
        }


        // Copy a 1D array of ErrorProp numbers (deep copy)
        public static ErrorProp[] Copy(ErrorProp[] a)
        {
            int n = a.Length;
            ErrorProp[] b = oneDarray(n);
            for (int i = 0; i < n; i++)
            {
                b[i] = Copy(a[i]);
            }
            return b;
        }

        // Copy a 2D array of ErrorProp numbers (deep copy)
        public static ErrorProp[,] Copy(ErrorProp[,] a)
        {
            int n = a.GetLength(0);
            int m = a.GetLength(1);
            ErrorProp[,] b = twoDarray(n, m);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    b[i, j] = Copy(a[i, j]);
                }
            }
            return b;
        }

        // ADDITION
        // Add two ErrorProp numbers with correlation [instance method]
        public ErrorProp plus(ErrorProp a, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value + m_value;
            c.m_error = hypotWithCov(a.m_error, m_error, corrCoeff);
            return c;
        }

        // Add two ErrorProp numbers with correlation [static method]
        public static ErrorProp plus(ErrorProp a, ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value + b.m_value;
            c.m_error = hypotWithCov(a.m_error, b.m_error, corrCoeff);
            return c;
        }

        //Add a ErrorProp number to this ErrorProp number with no, i.e. zero, correlation [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp plus(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value + a.m_value;
            b.m_error = hypotWithCov(a.m_error, m_error, 0.0D);
            return b;
        }

        // Add two ErrorProp numbers with no, i.e. zero, correlation term [static method]
        public static ErrorProp plus(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value + b.m_value;
            c.m_error = hypotWithCov(a.m_error, b.m_error, 0.0D);
            return c;
        }

        // Add an error free double number to this ErrorProp number [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp plus(double a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value + a;
            b.m_error = Math.Abs(m_error);
            return b;
        }

        //Add a ErrorProp number to an error free double [static method]
        public static ErrorProp plus(double a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a + b.m_value;
            c.m_error = Math.Abs(b.m_error);
            return c;
        }

        //Add an error free double number to an error free double and return sum as ErrorProp [static method]
        public static ErrorProp plus(double a, double b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a + b;
            c.m_error = 0.0D;
            return c;
        }


        // Add a ErrorProp number to this ErrorProp number and replace this with the sum
        // with correlation term
        public void plusEquals(ErrorProp a, double corrCoeff)
        {
            m_value += a.m_value;
            m_error = hypotWithCov(a.m_error, m_error, corrCoeff);
        }

        // Add a ErrorProp number to this ErrorProp number and replace this with the sum
        // with no, i.e. zero, correlation term
        public void plusEquals(ErrorProp a)
        {
            m_value += a.m_value;
            m_error = Math.Sqrt(a.m_error*a.m_error + m_error*m_error);
            m_error = hypotWithCov(a.m_error, m_error, 0.0D);
        }

        //Add double number to this ErrorProp number and replace this with the sum
        public void plusEquals(double a)
        {
            m_value += a;
            m_error = Math.Abs(m_error);
        }

        // SUBTRACTION
        // Subtract an ErrorProp number from this ErrorProp number with correlation [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp minus(ErrorProp a, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = m_value - a.m_value;
            c.m_error = hypotWithCov(m_error, a.m_error, -corrCoeff);
            return c;
        }

        // Subtract ErrorProp number b from ErrorProp number a with correlation [static method]
        public static ErrorProp minus(ErrorProp a, ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value - b.m_value;
            c.m_error = hypotWithCov(a.m_error, b.m_error, -corrCoeff);
            return c;
        }

        // Subtract a ErrorProp number from this ErrorProp number with no, i.e. zero, correlation [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp minus(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value - a.m_value;
            b.m_error = hypotWithCov(a.m_error, m_error, 0.0D);
            return b;
        }

        // Subtract ErrorProp number b from ErrorProp number a with no, i.e. zero, correlation term [static method]
        public static ErrorProp minus(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value - b.m_value;
            c.m_error = hypotWithCov(a.m_error, b.m_error, 0.0D);
            return c;
        }

        // Subtract an error free double number from this ErrorProp number [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp minus(double a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value - a;
            b.m_error = Math.Abs(m_error);
            return b;
        }

        // Subtract a ErrorProp number b from an error free double a [static method]
        public static ErrorProp minus(double a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a - b.m_value;
            c.m_error = Math.Abs(b.m_error);
            return c;
        }

        //Subtract an error free double number b from an error free double a and return sum as ErrorProp [static method]
        public static ErrorProp minus(double a, double b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a - b;
            c.m_error = 0.0D;
            return c;
        }

        // Subtract a ErrorProp number to this ErrorProp number and replace this with the sum
        // with correlation term
        public void minusEquals(ErrorProp a, double corrCoeff)
        {
            m_value -= a.m_value;
            m_error = hypotWithCov(a.m_error, m_error, -corrCoeff);
        }

        // Subtract a ErrorProp number from this ErrorProp number and replace this with the sum
        // with no, i.e. zero, correlation term
        public void minusEquals(ErrorProp a)
        {
            m_value -= a.m_value;
            m_error = hypotWithCov(a.m_error, m_error, 0.0D);
        }

        // Subtract a double number from this ErrorProp number and replace this with the sum
        public void minusEquals(double a)
        {
            m_value -= a;
            m_error = Math.Abs(m_error);
        }

        //      MULTIPLICATION
        //Multiply two ErrorProp numbers with correlation [instance method]
        public ErrorProp times(ErrorProp a, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            double cov = corrCoeff*a.m_error*m_error;
            c.m_value = a.m_value*m_value;
            if (a.m_value == 0.0D)
            {
                c.m_error = a.m_error*m_value;
            }
            else
            {
                if (m_value == 0.0D)
                {
                    c.m_error = m_error*a.m_value;
                }
                else
                {
                    c.m_error = Math.Abs(c.m_value)*hypotWithCov(a.m_error/a.m_value, m_error/m_value, corrCoeff);
                }
            }
            return c;
        }

        //Multiply this ErrorProp number by a ErrorProp number [instance method]
        // with no, i.e. zero, correlation
        // this ErrorProp number remains unaltered
        public ErrorProp times(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value*a.m_value;
            if (a.m_value == 0.0D)
            {
                b.m_error = a.m_error*m_value;
            }
            else
            {
                if (m_value == 0.0D)
                {
                    b.m_error = m_error*a.m_value;
                }
                else
                {
                    b.m_error = Math.Abs(b.m_value)*hypotWithCov(a.m_error/a.m_value, m_error/m_value, 0.0D);
                }
            }
            return b;
        }

        //Multiply this ErrorProp number by a double [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp times(double a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value*a;
            b.m_error = Math.Abs(m_error*a);
            return b;
        }


        //Multiply two ErrorProp numbers with correlation [static method]
        public static ErrorProp times(ErrorProp a, ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            double cov = corrCoeff*a.m_error*b.m_error;
            c.m_value = a.m_value*b.m_value;
            if (a.m_value == 0.0D)
            {
                c.m_error = a.m_error*b.m_value;
            }
            else
            {
                if (b.m_value == 0.0D)
                {
                    c.m_error = b.m_error*a.m_value;
                }
                else
                {
                    c.m_error = Math.Abs(c.m_value)*hypotWithCov(a.m_error/a.m_value, b.m_error/b.m_value, corrCoeff);
                }
            }
            return c;
        }

        //Multiply two ErrorProp numbers with no, i.e. zero, correlation [static method]
        public static ErrorProp times(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value*b.m_value;
            if (a.m_value == 0.0D)
            {
                c.m_error = a.m_error*b.m_value;
            }
            else
            {
                if (b.m_value == 0.0D)
                {
                    c.m_error = b.m_error*a.m_value;
                }
                else
                {
                    c.m_error = Math.Abs(c.m_value)*hypotWithCov(a.m_error/a.m_value, b.m_error/b.m_value, 0.0D);
                }
            }
            return c;
        }

        //Multiply a double by a ErrorProp number [static method]
        public static ErrorProp times(double a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a*b.m_value;
            c.m_error = Math.Abs(a*b.m_error);
            return c;
        }

        //Multiply a double number by a double and return product as ErrorProp [static method]
        public static ErrorProp times(double a, double b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a*b;
            c.m_error = 0.0;
            return c;
        }

        //Multiply this ErrorProp number by an ErrorProp number and replace this by the product
        // with correlation
        public void timesEquals(ErrorProp a, double corrCoeff)
        {
            ErrorProp b = new ErrorProp();
            double cov = corrCoeff*m_error*a.m_error;
            b.m_value = m_value*a.m_value;
            if (a.m_value == 0.0D)
            {
                b.m_error = a.m_error*m_value;
            }
            else
            {
                if (m_value == 0.0D)
                {
                    b.m_error = m_error*a.m_value;
                }
                else
                {
                    b.m_error = Math.Abs(b.m_value)*hypotWithCov(a.m_error/a.m_value, m_error/m_value, corrCoeff);
                }
            }

            m_value = b.m_value;
            m_error = b.m_error;
        }

        //Multiply this ErrorProp number by an ErrorProp number and replace this by the product
        // with no, i.e. zero, correlation
        public void timesEquals(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value*a.m_value;
            if (a.m_value == 0.0D)
            {
                b.m_error = a.m_error*m_value;
            }
            else
            {
                if (m_value == 0.0D)
                {
                    b.m_error = m_error*a.m_value;
                }
                else
                {
                    b.m_error = Math.Abs(b.m_value)*hypotWithCov(a.m_error/a.m_value, m_error/m_value, 0.0D);
                }
            }

            m_value = b.m_value;
            m_error = b.m_error;
        }

        //Multiply this ErrorProp number by a double and replace this by the product
        public void timesEquals(double a)
        {
            m_value = m_value*a;
            m_error = Math.Abs(m_error*a);
        }

        // DIVISION
        // Division of this ErrorProp number by a ErrorProp number [instance method]
        // this ErrorProp number remains unaltered
        // with correlation
        public ErrorProp over(ErrorProp a, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = m_value/a.m_value;
            if (m_value == 0.0D)
            {
                c.m_error = m_error*a.m_value;
            }
            else
            {
                c.m_error = Math.Abs(c.m_value)*hypotWithCov(m_error/m_value, a.m_error/a.m_value, -corrCoeff);
            }
            return c;
        }

        // Division of two ErrorProp numbers a/b [static method]
        // with correlation
        public static ErrorProp over(ErrorProp a, ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value/b.m_value;
            if (a.m_value == 0.0D)
            {
                c.m_error = a.m_error*b.m_value;
            }
            else
            {
                c.m_error = Math.Abs(c.m_value)*hypotWithCov(a.m_error/a.m_value, b.m_error/b.m_value, -corrCoeff);
            }
            return c;
        }

        // Division of this ErrorProp number by a ErrorProp number [instance method]
        // this ErrorProp number remains unaltered
        // with no, i.e. zero, correlation
        public ErrorProp over(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value/a.m_value;
            b.m_error = Math.Abs(b.m_value)*hypotWithCov(a.m_error/a.m_value, m_error/m_value, 0.0);
            if (m_value == 0.0D)
            {
                b.m_error = m_error*b.m_value;
            }
            else
            {
                b.m_error = Math.Abs(b.m_value)*hypotWithCov(a.m_error/a.m_value, m_error/m_value, 0.0);
            }
            return b;
        }

        // Division of two ErrorProp numbers a/b [static method]
        // with no, i.e. zero, correlation
        public static ErrorProp over(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a.m_value/b.m_value;
            if (a.m_value == 0.0D)
            {
                c.m_error = a.m_error*b.m_value;
            }
            else
            {
                c.m_error = Math.Abs(c.m_value)*hypotWithCov(a.m_error/a.m_value, b.m_error/b.m_value, 0.0D);
            }

            return c;
        }

        //Division of this ErrorProp number by a double [instance method]
        // this ErrorProp number remains unaltered
        public ErrorProp over(double a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = m_value/a;
            b.m_error = Math.Abs(m_error/a);
            return b;
        }


        // Division of a double, a, by a ErrorProp number, b  [static method]
        public static ErrorProp over(double a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a/b.m_value;
            c.m_error = Math.Abs(a*b.m_error/(b.m_value*b.m_value));
            return c;
        }

        // Divide a double number by a double and return quotient as ErrorProp [static method]
        public static ErrorProp over(double a, double b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = a/b;
            c.m_error = 0.0;
            return c;
        }

        // Division of this ErrorProp number by a ErrorProp number and replace this by the quotient
        // with no, i.r. zero, correlation
        public void overEquals(ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = m_value/b.m_value;
            if (m_value == 0.0D)
            {
                c.m_error = m_error*b.m_value;
            }
            else
            {
                c.m_error = Math.Abs(c.m_value)*hypotWithCov(m_error/m_value, b.m_error/b.m_value, 0.0D);
            }
            m_value = c.m_value;
            m_error = c.m_error;
        }

        // Division of this ErrorProp number by a ErrorProp number and replace this by the quotient
        // with correlation
        public void overEquals(ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = m_value/b.m_value;
            if (m_value == 0.0D)
            {
                c.m_error = m_error*b.m_value;
            }
            else
            {
                c.m_error = Math.Abs(c.m_value)*hypotWithCov(m_error/m_value, b.m_error/b.m_value, -corrCoeff);
            }
            m_value = c.m_value;
            m_error = c.m_error;
        }

        //Division of this ErrorProp number by a double and replace this by the quotient
        public void overEquals(double a)
        {
            m_value = m_value/a;
            m_error = Math.Abs(m_error/a);
        }

        // RECIPROCAL
        // Returns the reciprocal (1/a) of a ErrorProp number (a) [instance method]
        public ErrorProp inverse()
        {
            ErrorProp b = over(1.0D, this);
            return b;
        }

        // Returns the reciprocal (1/a) of a ErrorProp number (a) [static method]
        public static ErrorProp inverse(ErrorProp a)
        {
            ErrorProp b = over(1.0, a);
            return b;
        }

        //FURTHER MATHEMATICAL FUNCTIONS

        // Returns the Length of the hypotenuse of a and b i.e. sqrt(a*a + b*b)
        // where a and b are ErrorProp [without unecessary overflow or underflow]
        // with correlation
        public static ErrorProp hypot(ErrorProp a, ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = Fmath.hypot(a.m_value, b.m_value);
            c.m_error = Math.Abs(hypotWithCov(a.m_error*a.m_value, b.m_error*b.m_value, corrCoeff)/c.m_value);
            return c;
        }

        // Returns the Length of the hypotenuse of a and b i.e. sqrt(a*a + b*b)
        // where a and b are ErrorProp [without unecessary overflow or underflow]
        // with no, i.e. zero, correlation
        public static ErrorProp hypot(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = Fmath.hypot(a.m_value, b.m_value);
            c.m_error = Math.Abs(hypotWithCov(a.m_error*a.m_value, b.m_error*b.m_value, 0.0D)/c.m_value);
            return c;
        }

        //Absolute value [static method]
        public static ErrorProp abs(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Abs(a.m_value);
            b.m_error = Math.Abs(a.m_error);
            return b;
        }

        //Absolute value  [instance method]
        public ErrorProp abs()
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Abs(m_value);
            b.m_error = Math.Abs(m_error);
            return b;
        }

        // Exponential
        public static ErrorProp Exp(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Exp(a.m_value);
            b.m_error = Math.Abs(b.m_value*a.m_error);
            return b;
        }

        // Natural log
        public static ErrorProp Log(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Log(a.m_value);
            b.m_error = Math.Abs(a.m_error/a.m_value);
            return b;
        }

        // log to base 10
        public static ErrorProp Log10(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.Log10(a.m_value);
            b.m_error = Math.Abs(a.m_error/(a.m_value*Math.Log(10.0D)));
            return b;
        }

        //Roots
        // Square root
        public static ErrorProp sqrt(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Sqrt(a.m_value);
            b.m_error = Math.Abs(a.m_error/(2.0D*a.m_value));
            return b;
        }

        // The nth root (n = integer > 1)
        public static ErrorProp nthRoot(ErrorProp a, int n)
        {
            if (n == 0)
            {
                throw new ArithmeticException("Division by zero (n = 0 - infinite root) attempted in ErrorProp.nthRoot");
            }
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Pow(a.m_value, 1.0/n);
            b.m_error = Math.Abs(a.m_error*Math.Pow(a.m_value, 1/n - 1)/(n));
            return b;
        }

        //Powers
        //Square [instance method]
        public ErrorProp square()
        {
            ErrorProp a = new ErrorProp(m_value, m_error);
            return a.times(a, 1.0D);
        }

        //Square [static method]
        public static ErrorProp square(ErrorProp a)
        {
            return a.times(a, 1.0D);
        }

        // returns an ErrorProp number raised to an error free power
        public static ErrorProp Pow(ErrorProp a, double b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = Math.Pow(a.m_value, b);
            c.m_error = Math.Abs(b*Math.Pow(a.m_value, b - 1.0));
            return c;
        }

        // returns an error free number raised to an ErrorProp power
        public static ErrorProp Pow(double a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = Math.Pow(a, b.m_value);
            c.m_error = Math.Abs(c.m_value*Math.Log(a)*b.m_error);
            return c;
        }

        // returns a ErrorProp number raised to a ErrorProp power
        // with correlation
        public static ErrorProp Pow(ErrorProp a, ErrorProp b, double corrCoeff)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = Math.Pow(a.m_value, b.m_value);
            c.m_error = hypotWithCov(a.m_error*b.m_value*Math.Pow(a.m_value, b.m_value - 1.0),
                                     b.m_error*Math.Log(a.m_value)*Math.Pow(a.m_value, b.m_value), corrCoeff);
            return c;
        }

        // returns a ErrorProp number raised to a ErrorProp power
        // with zero correlation
        public static ErrorProp Pow(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            c.m_value = Math.Pow(a.m_value, b.m_value);
            c.m_error = hypotWithCov(a.m_error*b.m_value*Math.Pow(a.m_value, b.m_value - 1.0),
                                     b.m_error*Math.Log(a.m_value)*Math.Pow(a.m_value, b.m_value), 0.0D);
            return c;
        }

        // ErrorProp trigonometric functions

        //Sine of an ErrorProp number
        public static ErrorProp Sin(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Sin(a.m_value);
            b.m_error = Math.Abs(a.m_error*Math.Cos(a.m_value));
            return b;
        }

        //Cosine of an ErrorProp number
        public static ErrorProp Cos(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Cos(a.m_value);
            b.m_error = Math.Abs(a.m_error*Math.Sin(a.m_value));
            return b;
        }

        //Tangent of an ErrorProp number
        public static ErrorProp tan(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Tan(a.m_value);
            b.m_error = Math.Abs(a.m_error*Fmath.square(Fmath.sec(a.m_value)));
            return b;
        }

        //Hyperbolic sine of a ErrorProp number
        public static ErrorProp sinh(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.sinh(a.m_value);
            b.m_error = Math.Abs(a.m_error*Fmath.cosh(a.m_value));
            return b;
        }

        //Hyperbolic cosine of a ErrorProp number
        public static ErrorProp cosh(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.cosh(a.m_value);
            b.m_error = Math.Abs(a.m_error*Fmath.sinh(a.m_value));
            return b;
        }

        //Hyperbolic tangent of a ErrorProp number
        public static ErrorProp tanh(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.tanh(a.m_value);
            b.m_error = Math.Abs(a.m_error*Fmath.square(Fmath.sech(a.m_value)));
            return b;
        }

        //Inverse sine of a ErrorProp number
        public static ErrorProp asin(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Asin(a.m_value);
            b.m_error = Math.Abs(a.m_error/Math.Sqrt(1.0D - a.m_value*a.m_value));
            return b;
        }

        //Inverse cosine of a ErrorProp number
        public static ErrorProp Acos(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Acos(a.m_value);
            b.m_error = Math.Abs(a.m_error/Math.Sqrt(1.0D - a.m_value*a.m_value));
            return b;
        }

        //Inverse tangent of a ErrorProp number
        public static ErrorProp Atan(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Math.Atan(a.m_value);
            b.m_error = Math.Abs(a.m_error/(1.0D + a.m_value*a.m_value));
            return b;
        }

        //Inverse tangent (Atan2) of a ErrorProp number - no correlation
        public static ErrorProp Atan2(ErrorProp a, ErrorProp b)
        {
            ErrorProp c = new ErrorProp();
            ErrorProp d = a.over(b);
            c.m_value = Math.Atan2(a.m_value, b.m_value);
            c.m_error = Math.Abs(d.m_error/(1.0D + d.m_value*d.m_value));
            return c;
        }

        //Inverse tangent (Atan2) of a ErrorProp number - correlation
        public static ErrorProp Atan2(ErrorProp a, ErrorProp b, double rho)
        {
            ErrorProp c = new ErrorProp();
            ErrorProp d = a.over(b, rho);
            c.m_value = Math.Atan2(a.m_value, b.m_value);
            c.m_error = Math.Abs(d.m_error/(1.0D + d.m_value*d.m_value));
            return c;
        }

        //Inverse hyperbolic sine of a ErrorProp number
        public static ErrorProp asinh(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.asinh(a.m_value);
            b.m_error = Math.Abs(a.m_error/Math.Sqrt(a.m_value*a.m_value + 1.0D));
            return b;
        }

        //Inverse hyperbolic cosine of a ErrorProp number
        public static ErrorProp acosh(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.acosh(a.m_value);
            b.m_error = Math.Abs(a.m_error/Math.Sqrt(a.m_value*a.m_value - 1.0D));
            return b;
        }

        //Inverse hyperbolic tangent of a ErrorProp number
        public static ErrorProp atanh(ErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b.m_value = Fmath.atanh(a.m_value);
            b.m_error = Math.Abs(a.m_error/(1.0D - a.m_value*a.m_value));
            return b;
        }

        // SOME USEFUL NUMBERS
        // returns the number zero (0) with zero error
        public static ErrorProp zero()
        {
            ErrorProp c = new ErrorProp();
            c.m_value = 0.0D;
            c.m_error = 0.0D;
            return c;
        }

        // returns the number one (+1) with zero error
        public static ErrorProp plusOne()
        {
            ErrorProp c = new ErrorProp();
            c.m_value = 1.0D;
            c.m_error = 0.0D;
            return c;
        }

        // returns the number minus one (-1) with zero error
        public static ErrorProp minusOne()
        {
            ErrorProp c = new ErrorProp();
            c.m_value = -1.0D;
            c.m_error = 0.0D;
            return c;
        }

        // Private methods
        // Safe calculation of sqrt(a*a + b*b + 2*r*a*b)
        private static double hypotWithCov(double a, double b, double r)
        {
            double pre = 0.0D, ratio = 0.0D, sgn = 0.0D;

            if (a == 0.0D && b == 0.0D)
            {
                return 0.0D;
            }
            if (Math.Abs(a) > Math.Abs(b))
            {
                pre = Math.Abs(a);
                ratio = b/a;
                sgn = Fmath.sign(a);
            }
            else
            {
                pre = Math.Abs(b);
                ratio = a/b;
                sgn = Fmath.sign(b);
            }
            return pre*Math.Sqrt(1.0D + ratio*(ratio + 2.0D*r*sgn));
        }
    }
}
