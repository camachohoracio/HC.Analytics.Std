#region

using System;
using HC.Analytics.ConvertClasses;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.Complex
{
    public class ComplexClass
    {
        // default value = j
        private static bool m_infOption = true; // option determining how infinity is handled
        private static char m_jori = 'j'; // i or j in a + j.b or a + i.b representaion
        private double m_imag; // Imaginary part of a complex number
        private double m_real; // Real part of a complex number
        // if true (default option):
        //  multiplication with either complex number with either part = infinity returns infinity
        //      unless the one complex number is zero in both parts
        //  division by a complex number with either part = infinity returns zero
        //      unless the dividend is also infinite in either part
        // if false:
        //      standard arithmetic performed


/*********************************************************/

        // CONSTRUCTORS
        // default constructor - real and imag = zero
        public ComplexClass()
        {
            m_real = 0.0D;
            m_imag = 0.0D;
        }

        // constructor - initialises both real and imag
        public ComplexClass(double real, double imag)
        {
            m_real = real;
            m_imag = imag;
        }

        // constructor - initialises  real, imag = 0.0
        public ComplexClass(double real)
        {
            m_real = real;
            m_imag = 0.0D;
        }

        public ComplexClass(decimal real)
        {
            m_real = (double) real;
            m_imag = 0.0D;
        }

        public ComplexClass(long real)
        {
            m_real = real;
            m_imag = 0.0D;
        }

        // constructor - initialises both real and imag to the values of an existing Complex
        public ComplexClass(ComplexClass c)
        {
            m_real = c.m_real;
            m_imag = c.m_imag;
        }

/*********************************************************/

        // PUBLIC METHODS

        // SET VALUES
        // Set the value of real
        public void setReal(double real)
        {
            m_real = real;
        }

        // Set the value of imag
        public void setImag(double imag)
        {
            m_imag = imag;
        }

        // Set the values of real and imag
        public void reset(double real, double imag)
        {
            m_real = real;
            m_imag = imag;
        }

        // Set real and imag given the modulus and argument (in radians)
        public void polarRad(double mod, double arg)
        {
            m_real = mod*Math.Cos(arg);
            m_imag = mod*Math.Sin(arg);
        }

        // Set real and imag given the modulus and argument (in radians)
        // retained for compatibility
        public void polar(double mod, double arg)
        {
            m_real = mod*Math.Cos(arg);
            m_imag = mod*Math.Sin(arg);
        }

        // Set real and imag given the modulus and argument (in degrees)
        public void polarDeg(double mod, double arg)
        {
            arg = Converter.toRadians(arg);
            m_real = mod*Math.Cos(arg);
            m_imag = mod*Math.Sin(arg);
        }

        // GET VALUES
        // Get the value of real
        public double getReal()
        {
            return m_real;
        }

        // Get the value of imag
        public double getImag()
        {
            return m_imag;
        }

        // INPUT AND OUTPUT

        // READ A COMPLEX NUMBER
        // Read a complex number from the keyboard console after a prompt message
        // in a string format compatible with Complex.parse,
        // e.g 2+j3, 2 + j3, 2+i3, 2 + i3
        // prompt = Prompt message to vdu
        public static ComplexClass readComplex(string prompt)
        {
            int ch = ' ';
            string cstring = "";
            bool done = false;

            Console.Write(prompt + " ");
            //System.out.flush();

            while (!done)
            {
                try
                {
                    ch = Console.Read();
                    if (ch < 0 || (char) ch == '\n')
                    {
                        done = true;
                    }
                    else
                    {
                        cstring = cstring + (char) ch;
                    }
                }
                catch
                {
                    done = true;
                }
            }
            return parseComplex(cstring);
        }

        // Read a complex number from the keyboard console after a prompt message (with string default option)
        // in a string format compatible with Complex.parse,
        // e.g 2+j3, 2 + j3, 2+i3, 2 + i3
        // prompt = Prompt message to vdu
        // dflt = default value
        public static ComplexClass readComplex(string prompt, string dflt)
        {
            int ch = ' ';
            string cstring = "";
            bool done = false;

            Console.Write(prompt + " [default value = " + dflt + "]  ");
            //System.out.flush();

            int i = 0;
            while (!done)
            {
                try
                {
                    ch = Console.Read();
                    if (ch < 0 || (char) ch == '\n' || (char) ch == '\r')
                    {
                        if (i == 0)
                        {
                            cstring = dflt;
                            if ((char) ch == '\r')
                            {
                                ch = Console.Read();
                            }
                        }
                        done = true;
                    }
                    else
                    {
                        cstring = cstring + (char) ch;
                        i++;
                    }
                }
                catch
                {
                    done = true;
                }
            }
            return parseComplex(cstring);
        }

        // Read a complex number from the keyboard console after a prompt message (with Complex default option)
        // in a string format compatible with Complex.parse,
        // e.g 2+j3, 2 + j3, 2+i3, 2 + i3
        // prompt = Prompt message to vdu
        // dflt = default value
        public static ComplexClass readComplex(string prompt, ComplexClass dflt)
        {
            int ch = ' ';
            string cstring = "";
            bool done = false;

            Console.Write(prompt + " [default value = " + dflt + "]  ");
            //System.out.flush();

            int i = 0;
            while (!done)
            {
                try
                {
                    ch = Console.Read();
                    if (ch < 0 || (char) ch == '\n' || (char) ch == '\r')
                    {
                        if (i == 0)
                        {
                            if ((char) ch == '\r')
                            {
                                ch = Console.Read();
                            }
                            return dflt;
                        }
                        done = true;
                    }
                    else
                    {
                        cstring = cstring + (char) ch;
                        i++;
                    }
                }
                catch
                {
                    done = true;
                }
            }
            return parseComplex(cstring);
        }


        // Read a complex number from the keyboard console without a prompt message
        // in a string format compatible with Complex.parse,
        // e.g 2+j3, 2 + j3, 2+i3, 2 + i3
        // prompt = Prompt message to vdu
        public static ComplexClass readComplex()
        {
            int ch = ' ';
            string cstring = "";
            bool done = false;

            Console.Write(" ");
            //System.out.flush();

            while (!done)
            {
                try
                {
                    ch = Console.Read();
                    if (ch < 0 || (char) ch == '\n')
                    {
                        done = true;
                    }
                    else
                    {
                        cstring = cstring + (char) ch;
                    }
                }
                catch
                {
                    done = true;
                }
            }
            return parseComplex(cstring);
        }

        // PRINT A COMPLEX NUMBER
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

        // PRINT AN ARRAY OF COMLEX NUMBERS
        // Print an array to terminal window with text (message) and a line return
        public static void println(string message, ComplexClass[] aa)
        {
            PrintToScreen.WriteLine(message);
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "  ");
            }
        }

        // Print an array to terminal window without text (message) but with a line return
        public static void println(ComplexClass[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "  ");
            }
        }

        // Print an array to terminal window with text (message) but no line returns except at the end
        public static void print(string message, ComplexClass[] aa)
        {
            Console.Write(message + " ");
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // Print an array to terminal window without text (message) but with no line returns except at the end
        public static void print(ComplexClass[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "  ");
            }
            PrintToScreen.WriteLine();
        }

        // TRUNCATION
        // Rounds the mantissae of both the real and imaginary parts of Complex to prec places
        // Static method
        public static ComplexClass truncate(ComplexClass x, int prec)
        {
            if (prec < 0)
            {
                return x;
            }

            double xR = x.getReal();
            double xI = x.getImag();
            ComplexClass y = new ComplexClass();

            xR = Fmath.truncate(xR, prec);
            xI = Fmath.truncate(xI, prec);

            y.reset(xR, xI);

            return y;
        }

        // instance method
        public ComplexClass truncate(int prec)
        {
            if (prec < 0)
            {
                return this;
            }

            double xR = getReal();
            double xI = getImag();
            ComplexClass y = new ComplexClass();

            xR = Fmath.truncate(xR, prec);
            xI = Fmath.truncate(xI, prec);

            y.reset(xR, xI);

            return y;
        }


        // CONVERSIONS
        // Format a complex number as a string, a + jb or a + ib[instance method]
        // < value of real > < + or - > < j or i> < value of imag >
        // Choice of j or i is set by Complex.seti() or Complex.setj()
        // j is the default option for j or i
        // Overides java.lang.string.ToString()
        public override string ToString()
        {
            char ch = '+';
            if (m_imag < 0.0D)
            {
                ch = '-';
            }
            return m_real + " " + ch + " " + m_jori + Math.Abs(m_imag);
        }

        // Format a complex number as a string, a + jb or a + ib [static method]
        // See static method above for comments
        public static string ToString(ComplexClass aa)
        {
            char ch = '+';
            if (aa.m_imag < 0.0D)
            {
                ch = '-';
            }
            return aa.m_real + " " + ch + m_jori + Math.Abs(aa.m_imag);
        }

        // Sets the representation of the square root of minus one to j in Strings
        public static void setj()
        {
            m_jori = 'j';
        }

        // Sets the representation of the square root of minus one to i in Strings
        public static void seti()
        {
            m_jori = 'i';
        }

        // Returns the representation of the square root of minus one (j or i) set for Strings
        public static char getjori()
        {
            return m_jori;
        }

        // Parse a string to obtain Complex
        // accepts strings 'real''s''sign''s''x''imag'
        // where x may be i or j and s may be no spaces or any number of spaces
        // and sign may be + or -
        // e.g.  2+j3, 2 + j3, 2+i3, 2 + i3
        public static ComplexClass parseComplex(string ss)
        {
            ComplexClass aa = new ComplexClass();
            ss = ss.Trim();
            double first = 1.0D;
            if (ss[0] == '-')
            {
                first = -1.0D;
                ss = ss.Substring(1);
            }

            int i = ss['j'];
            if (i == -1)
            {
                i = ss['i'];
            }
            if (i == -1)
            {
                throw new FormatException("no i or j found");
            }

            int imagSign = 1;
            int j = ss['+'];

            if (j == -1)
            {
                j = ss['-'];
                if (j > -1)
                {
                    imagSign = -1;
                }
            }
            if (j == -1)
            {
                throw new FormatException("no + or - found");
            }

            int r0 = 0;
            int r1 = j;
            int i0 = i + 1;
            int i1 = ss.Length;
            string sreal = ss.Substring(r0, r1);
            string simag = ss.Substring(i0, i1);
            aa.m_real = first*double.Parse(sreal);
            aa.m_imag = imagSign*double.Parse(simag);
            return aa;
        }

        // Same method as parseComplex
        // Overides java.lang.Object.valueOf()
        public static ComplexClass valueOf(string ss)
        {
            return parseComplex(ss);
        }

        // Return a HASH CODE for the Complex number
        // Overides java.lang.Object.hashCode()
        public int hashCode()
        {
            long lreal = (long) m_real;
            long limag = (long) m_imag;
            int hreal = (int) (lreal ^ (lreal*32));
            int himag = (int) (limag ^ (limag*32));
            return 7*(hreal/10) + 3*(himag/10);
        }

        // SWAP
        // Swaps two complex numbers
        public static void swap(ComplexClass aa, ComplexClass bb)
        {
            double holdAreal = aa.m_real;
            double holdAimag = aa.m_imag;
            aa.reset(bb.m_real, bb.m_imag);
            bb.reset(holdAreal, holdAimag);
        }


        // ARRAYS

        // Create a one dimensional array of Complex objects of Length n
        // all real = 0 and all imag = 0
        public static ComplexClass[] oneDarray(int n)
        {
            ComplexClass[] a = new ComplexClass[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = zero();
            }
            return a;
        }

        // Create a one dimensional array of Complex objects of Length n
        // all real = a and all imag = b
        public static ComplexClass[] oneDarray(int n, double a, double b)
        {
            ComplexClass[] c = new ComplexClass[n];
            for (int i = 0; i < n; i++)
            {
                c[i] = zero();
                c[i].reset(a, b);
            }
            return c;
        }

        // Arithmetic mean of a one dimensional array of complex numbers
        public static ComplexClass mean(ComplexClass[] aa)
        {
            int n = aa.Length;
            ComplexClass sum = new ComplexClass(0.0D, 0.0D);
            for (int i = 0; i < n; i++)
            {
                sum = sum.plus(aa[i]);
            }
            return sum.over(n);
        }

        // Create a one dimensional array of Complex objects of Length n
        // all = the Complex constant
        public static ComplexClass[] oneDarray(int n, ComplexClass constant)
        {
            ComplexClass[] c = new ComplexClass[n];
            for (int i = 0; i < n; i++)
            {
                c[i] = Copy(constant);
            }
            return c;
        }

        // Create a two dimensional array of Complex objects of dimensions n and m
        // all real = zero and all imag = zero
        public static ComplexClass[,] twoDarray(int n, int m)
        {
            ComplexClass[,] a = new ComplexClass[n,m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    a[i, j] = zero();
                }
            }
            return a;
        }

        // Create a two dimensional array of Complex objects of dimensions n and m
        // all real = a and all imag = b
        public static ComplexClass[,] twoDarray(int n, int m, double a, double b)
        {
            ComplexClass[,] c = new ComplexClass[n,m];
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

        // Create a two dimensional array of Complex objects of dimensions n and m
        // all  =  the Complex constant
        public static ComplexClass[,] twoDarray(int n, int m, ComplexClass constant)
        {
            ComplexClass[,] c = new ComplexClass[n,m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    c[i, j] = Copy(constant);
                }
            }
            return c;
        }

        // Create a three dimensional array of Complex objects of dimensions n,  m and l
        // all real = zero and all imag = zero
        public static ComplexClass[,,] threeDarray(int n, int m, int l)
        {
            ComplexClass[,,] a = new ComplexClass[n,m,l];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    for (int k = 0; k < l; k++)
                    {
                        a[i, j, k] = zero();
                    }
                }
            }
            return a;
        }

        // Create a three dimensional array of Complex objects of dimensions n, m and l
        // all real = a and all imag = b
        public static ComplexClass[,,] threeDarray(int n, int m, int l, double a, double b)
        {
            ComplexClass[,,] c = new ComplexClass[n,m,l];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    for (int k = 0; k < l; k++)
                    {
                        c[i, j, k] = zero();
                        c[i, j, k].reset(a, b);
                    }
                }
            }
            return c;
        }

        // Create a three dimensional array of Complex objects of dimensions n, m and l
        // all  =  the Complex constant
        public static ComplexClass[,,] threeDarray(int n, int m, int l, ComplexClass constant)
        {
            ComplexClass[,,] c = new ComplexClass[n,m,l];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    for (int k = 0; k < l; k++)
                    {
                        c[i, j, k] = Copy(constant);
                    }
                }
            }
            return c;
        }

        // COPY
        // Copy a single complex number [static method]
        public static ComplexClass Copy(ComplexClass a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                ComplexClass b = new ComplexClass();
                b.m_real = a.m_real;
                b.m_imag = a.m_imag;
                return b;
            }
        }

        // Copy a single complex number [instance method]
        public ComplexClass Copy()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                ComplexClass b = new ComplexClass();
                b.m_real = m_real;
                b.m_imag = m_imag;
                return b;
            }
        }


        // Copy a 1D array of complex numbers (deep Copy)
        // static metod
        public static ComplexClass[] Copy(ComplexClass[] a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                int n = a.Length;
                ComplexClass[] b = oneDarray(n);
                for (int i = 0; i < n; i++)
                {
                    b[i] = Copy(a[i]);
                }
                return b;
            }
        }

        // Copy a 2D array of complex numbers (deep Copy)
        public static ComplexClass[,] Copy(ComplexClass[,] a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                int n = a.GetLength(0);
                int m = a.GetLength(1);
                ComplexClass[,] b = twoDarray(n, m);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        b[i, j] = Copy(a[i, j]);
                    }
                }
                return b;
            }
        }

        // Copy a 3D array of complex numbers (deep Copy)
        public static ComplexClass[,,] Copy(ComplexClass[,,] a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                int n = a.GetLength(0);
                int m = a.GetLength(1);
                int l = a.GetLength(2);
                ComplexClass[,,] b = threeDarray(n, m, l);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        for (int k = 0; k < l; k++)
                        {
                            b[i, j, k] = Copy(a[i, j, k]);
                        }
                    }
                }
                return b;
            }
        }

        // CLONE
        // Overrides Java.Object method Clone
        // Copy a single complex number [instance method]
        public Object Clone()
        {
            Object ret = null;

            if (this != null)
            {
                ComplexClass b = new ComplexClass();
                b.m_real = m_real;
                b.m_imag = m_imag;
                ret = b;
            }

            return ret;
        }

        // ADDITION
        // Add two Complex numbers [static method]
        public static ComplexClass plus(ComplexClass a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a.m_real + b.m_real;
            c.m_imag = a.m_imag + b.m_imag;
            return c;
        }

        // Add a double to a Complex number [static method]
        public static ComplexClass plus(ComplexClass a, double b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a.m_real + b;
            c.m_imag = a.m_imag;
            return c;
        }

        // Add a Complex number to a double [static method]
        public static ComplexClass plus(double a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a + b.m_real;
            c.m_imag = b.m_imag;
            return c;
        }

        // Add a double number to a double and return sum as Complex [static method]
        public static ComplexClass plus(double a, double b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a + b;
            c.m_imag = 0.0D;
            return c;
        }

        // Add a Complex number to this Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass plus(ComplexClass a)
        {
            ComplexClass b = new ComplexClass();
            b.m_real = m_real + a.m_real;
            b.m_imag = m_imag + a.m_imag;
            return b;
        }

        // Add double number to this Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass plus(double a)
        {
            ComplexClass b = new ComplexClass();
            b.m_real = m_real + a;
            b.m_imag = m_imag;
            return b;
        }

        // Add a Complex number to this Complex number and replace this with the sum
        public void plusEquals(ComplexClass a)
        {
            m_real += a.m_real;
            m_imag += a.m_imag;
        }

        // Add double number to this Complex number and replace this with the sum
        public void plusEquals(double a)
        {
            m_real += a;
            //m_imag=m_imag;
        }

        //  SUBTRACTION
        // Subtract two Complex numbers [static method]
        public static ComplexClass minus(ComplexClass a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a.m_real - b.m_real;
            c.m_imag = a.m_imag - b.m_imag;
            return c;
        }

        // Subtract a double from a Complex number [static method]
        public static ComplexClass minus(ComplexClass a, double b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a.m_real - b;
            c.m_imag = a.m_imag;
            return c;
        }

        // Subtract a Complex number from a double [static method]
        public static ComplexClass minus(double a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a - b.m_real;
            c.m_imag = -b.m_imag;
            return c;
        }

        // Subtract a double number to a double and return difference as Complex [static method]
        public static ComplexClass minus(double a, double b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a - b;
            c.m_imag = 0.0D;
            return c;
        }

        // Subtract a Complex number from this Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass minus(ComplexClass a)
        {
            ComplexClass b = new ComplexClass();
            b.m_real = m_real - a.m_real;
            b.m_imag = m_imag - a.m_imag;
            return b;
        }

        // Subtract a double number from this Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass minus(double a)
        {
            ComplexClass b = new ComplexClass();
            b.m_real = m_real - a;
            b.m_imag = m_imag;
            return b;
        }

        // Subtract this Complex number from a double number [instance method]
        // this Complex number remains unaltered
        public ComplexClass transposedMinus(double a)
        {
            ComplexClass b = new ComplexClass();
            b.m_real = a - m_real;
            b.m_imag = m_imag;
            return b;
        }

        // Subtract a Complex number from this Complex number and replace this by the difference
        public void minusEquals(ComplexClass a)
        {
            m_real -= a.m_real;
            m_imag -= a.m_imag;
        }

        // Subtract a double number from this Complex number and replace this by the difference
        public void minusEquals(double a)
        {
            m_real -= a;
            //m_imag=m_imag;
        }

        // MULTIPLICATION
        // Sets the infinity handling option in multiplication and division
        // infOption -> true; standard arithmetic overriden - see above (instance variable definitions) for details
        // infOption -> false: standard arithmetic used
        public static void setInfOption(bool infOpt)
        {
            m_infOption = infOpt;
        }

        // Sets the infinity handling option in multiplication and division
        // opt = 0:   infOption -> true; standard arithmetic overriden - see above (instance variable definitions) for details
        // opt = 1:   infOption -> false: standard arithmetic used
        public static void setInfOption(int opt)
        {
            if (opt < 0 || opt > 1)
            {
                throw new ArgumentException("opt must be 0 or 1");
            }
            m_infOption = true;
            if (opt == 1)
            {
                m_infOption = false;
            }
        }

        // Gets the infinity handling option in multiplication and division
        // infOption -> true; standard arithmetic overriden - see above (instance variable definitions) for details
        // infOption -> false: standard arithmetic used
        public static bool getInfOption()
        {
            return m_infOption;
        }

        // Multiply two Complex numbers [static method]
        public static ComplexClass times(ComplexClass a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass(0.0D, 0.0D);
            if (m_infOption)
            {
                if (a.isInfinite() && !b.isZero())
                {
                    c.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return c;
                }
                if (b.isInfinite() && !a.isZero())
                {
                    c.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return c;
                }
            }

            c.m_real = a.m_real*b.m_real - a.m_imag*b.m_imag;
            c.m_imag = a.m_real*b.m_imag + a.m_imag*b.m_real;
            return c;
        }

        // Multiply a Complex number by a double [static method]
        public static ComplexClass times(ComplexClass a, double b)
        {
            ComplexClass c = new ComplexClass();
            if (m_infOption)
            {
                if (a.isInfinite() && b != 0.0D)
                {
                    c.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return c;
                }
                if (Fmath.isInfinity(b) && !a.isZero())
                {
                    c.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return c;
                }
            }
            c.m_real = a.m_real*b;
            c.m_imag = a.m_imag*b;
            return c;
        }

        // Multiply a double by a Complex number [static method]
        public static ComplexClass times(double a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            if (m_infOption)
            {
                if (b.isInfinite() && a != 0.0D)
                {
                    c.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return c;
                }
                if (Fmath.isInfinity(a) && !b.isZero())
                {
                    c.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return c;
                }
            }

            c.m_real = a*b.m_real;
            c.m_imag = a*b.m_imag;
            return c;
        }

        // Multiply a double number to a double and return product as Complex [static method]
        public static ComplexClass times(double a, double b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a*b;
            c.m_imag = 0.0D;
            return c;
        }

        // Multiply this Complex number by a Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass times(ComplexClass a)
        {
            ComplexClass b = new ComplexClass();
            if (m_infOption)
            {
                if (isInfinite() && !a.isZero())
                {
                    b.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return b;
                }
                if (a.isInfinite() && !isZero())
                {
                    b.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return b;
                }
            }

            b.m_real = m_real*a.m_real - m_imag*a.m_imag;
            b.m_imag = m_real*a.m_imag + m_imag*a.m_real;
            return b;
        }

        // Multiply this Complex number by a double [instance method]
        // this Complex number remains unaltered
        public ComplexClass times(double a)
        {
            ComplexClass b = new ComplexClass();
            if (m_infOption)
            {
                if (isInfinite() && a != 0.0D)
                {
                    b.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return b;
                }
                if (Fmath.isInfinity(a) && !isZero())
                {
                    b.reset(double.PositiveInfinity, double.PositiveInfinity);
                    return b;
                }
            }

            b.m_real = m_real*a;
            b.m_imag = m_imag*a;
            return b;
        }

        // Multiply this Complex number by a Complex number and replace this by the product
        public void timesEquals(ComplexClass a)
        {
            ComplexClass b = new ComplexClass();
            bool test = true;
            if (m_infOption)
            {
                if ((isInfinite() && !a.isZero()) || (a.isInfinite() && !isZero()))
                {
                    m_real = double.PositiveInfinity;
                    m_imag = double.PositiveInfinity;
                    test = false;
                }
            }
            if (test)
            {
                b.m_real = a.m_real*m_real - a.m_imag*m_imag;
                b.m_imag = a.m_real*m_imag + a.m_imag*m_real;
                m_real = b.m_real;
                m_imag = b.m_imag;
            }
        }

        // Multiply this Complex number by a double and replace this by the product
        public void timesEquals(double a)
        {
            bool test = true;
            if (m_infOption)
            {
                if ((isInfinite() && a != 0.0D) || (Fmath.isInfinity(a) && !isZero()))
                {
                    m_real = double.PositiveInfinity;
                    m_imag = double.PositiveInfinity;
                    test = false;
                }
            }
            if (test)
            {
                m_real = m_real*a;
                m_imag = m_imag*a;
            }
        }


        // DIVISION
        // Division of two Complex numbers a/b [static method]
        public static ComplexClass over(ComplexClass a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass(0.0D, 0.0D);
            if (m_infOption && !a.isInfinite() && b.isInfinite())
            {
                return c;
            }

            double denom = 0.0D, ratio = 0.0D;
            if (a.isZero())
            {
                if (b.isZero())
                {
                    c.m_real = double.NaN;
                    c.m_imag = double.NaN;
                }
                else
                {
                    c.m_real = 0.0D;
                    c.m_imag = 0.0D;
                }
            }
            else
            {
                if (Math.Abs(b.m_real) >= Math.Abs(b.m_imag))
                {
                    ratio = b.m_imag/b.m_real;
                    denom = b.m_real + b.m_imag*ratio;
                    c.m_real = (a.m_real + a.m_imag*ratio)/denom;
                    c.m_imag = (a.m_imag - a.m_real*ratio)/denom;
                }
                else
                {
                    ratio = b.m_real/b.m_imag;
                    denom = b.m_real*ratio + b.m_imag;
                    c.m_real = (a.m_real*ratio + a.m_imag)/denom;
                    c.m_imag = (a.m_imag*ratio - a.m_real)/denom;
                }
            }
            return c;
        }

        // Division of a Complex number, a, by a double, b [static method]
        public static ComplexClass over(ComplexClass a, double b)
        {
            ComplexClass c = new ComplexClass(0.0D, 0.0D);
            if (m_infOption && Fmath.isInfinity(b))
            {
                return c;
            }

            c.m_real = a.m_real/b;
            c.m_imag = a.m_imag/b;
            return c;
        }

        // Division of a double, a, by a Complex number, b  [static method]
        public static ComplexClass over(double a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            if (m_infOption && !Fmath.isInfinity(a) && b.isInfinite())
            {
                return c;
            }

            double denom, ratio;

            if (a == 0.0D)
            {
                if (b.isZero())
                {
                    c.m_real = double.NaN;
                    c.m_imag = double.NaN;
                }
                else
                {
                    c.m_real = 0.0D;
                    c.m_imag = 0.0D;
                }
            }
            else
            {
                if (Math.Abs(b.m_real) >= Math.Abs(b.m_imag))
                {
                    ratio = b.m_imag/b.m_real;
                    denom = b.m_real + b.m_imag*ratio;
                    c.m_real = a/denom;
                    c.m_imag = -a*ratio/denom;
                }
                else
                {
                    ratio = b.m_real/b.m_imag;
                    denom = b.m_real*ratio + b.m_imag;
                    c.m_real = a*ratio/denom;
                    c.m_imag = -a/denom;
                }
            }
            return c;
        }

        // Divide a double number by a double and return quotient as Complex [static method]
        public static ComplexClass over(double a, double b)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a/b;
            c.m_imag = 0.0;
            return c;
        }

        // Division of this Complex number by a Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass over(ComplexClass a)
        {
            ComplexClass b = new ComplexClass(0.0D, 0.0D);
            if (m_infOption && !isInfinite() && a.isInfinite())
            {
                return b;
            }

            double denom = 0.0D, ratio = 0.0D;
            if (Math.Abs(a.m_real) >= Math.Abs(a.m_imag))
            {
                ratio = a.m_imag/a.m_real;
                denom = a.m_real + a.m_imag*ratio;
                b.m_real = (m_real + m_imag*ratio)/denom;
                b.m_imag = (m_imag - m_real*ratio)/denom;
            }
            else
            {
                ratio = a.m_real/a.m_imag;
                denom = a.m_real*ratio + a.m_imag;
                b.m_real = (m_real*ratio + m_imag)/denom;
                b.m_imag = (m_imag*ratio - m_real)/denom;
            }
            return b;
        }

        // Division of this Complex number by a double [instance method]
        // this Complex number remains unaltered
        public ComplexClass over(double a)
        {
            ComplexClass b = new ComplexClass(0.0D, 0.0D);

            b.m_real = m_real/a;
            b.m_imag = m_imag/a;
            return b;
        }

        // Division of a double by this Complex number [instance method]
        // this Complex number remains unaltered
        public ComplexClass transposedOver(double a)
        {
            ComplexClass c = new ComplexClass(0.0D, 0.0D);
            if (m_infOption && !Fmath.isInfinity(a) && isInfinite())
            {
                return c;
            }

            double denom = 0.0D, ratio = 0.0D;
            if (Math.Abs(m_real) >= Math.Abs(m_imag))
            {
                ratio = m_imag/m_real;
                denom = m_real + m_imag*ratio;
                c.m_real = a/denom;
                c.m_imag = -a*ratio/denom;
            }
            else
            {
                ratio = m_real/m_imag;
                denom = m_real*ratio + m_imag;
                c.m_real = a*ratio/denom;
                c.m_imag = -a/denom;
            }
            return c;
        }

        // Division of this Complex number by a Complex number and replace this by the quotient
        public void overEquals(ComplexClass b)
        {
            ComplexClass c = new ComplexClass(0.0D, 0.0D);

            bool test = true;
            if (m_infOption && !isInfinite() && b.isInfinite())
            {
                m_real = 0.0D;
                m_imag = 0.0D;
                test = false;
            }
            if (test)
            {
                double denom = 0.0D, ratio = 0.0D;
                if (Math.Abs(b.m_real) >= Math.Abs(b.m_imag))
                {
                    ratio = b.m_imag/b.m_real;
                    denom = b.m_real + b.m_imag*ratio;
                    c.m_real = (m_real + m_imag*ratio)/denom;
                    c.m_imag = (m_imag - m_real*ratio)/denom;
                }
                else
                {
                    ratio = b.m_real/b.m_imag;
                    denom = b.m_real*ratio + b.m_imag;
                    c.m_real = (m_real*ratio + m_imag)/denom;
                    c.m_imag = (m_imag*ratio - m_real)/denom;
                }
                m_real = c.m_real;
                m_imag = c.m_imag;
            }
        }

        // Division of this Complex number by a double and replace this by the quotient
        public void overEquals(double a)
        {
            m_real = m_real/a;
            m_imag = m_imag/a;
        }

        // RECIPROCAL
        // Returns the reciprocal (1/a) of a Complex number (a) [static method]
        public static ComplexClass inverse(ComplexClass a)
        {
            ComplexClass b = new ComplexClass(0.0D, 0.0D);
            if (m_infOption && a.isInfinite())
            {
                return b;
            }

            b = over(1.0D, a);
            return b;
        }

        // Returns the reciprocal (1/a) of a Complex number (a) [instance method]
        public ComplexClass inverse()
        {
            ComplexClass b = new ComplexClass(0.0D, 0.0D);
            b = over(1.0D, this);
            return b;
        }

        // FURTHER MATHEMATICAL FUNCTIONS

        // Negates a Complex number [static method]
        public static ComplexClass negate(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = -a.m_real;
            c.m_imag = -a.m_imag;
            return c;
        }

        // Negates a Complex number [instance method]
        public ComplexClass negate()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = -m_real;
            c.m_imag = -m_imag;
            return c;
        }

        // Absolute value (modulus) of a complex number [static method]
        public static double Abs(ComplexClass a)
        {
            double rmod = Math.Abs(a.m_real);
            double imod = Math.Abs(a.m_imag);
            double ratio = 0.0D;
            double res = 0.0D;

            if (rmod == 0.0D)
            {
                res = imod;
            }
            else
            {
                if (imod == 0.0D)
                {
                    res = rmod;
                }
                if (rmod >= imod)
                {
                    ratio = a.m_imag/a.m_real;
                    res = rmod*Math.Sqrt(1.0D + ratio*ratio);
                }
                else
                {
                    ratio = a.m_real/a.m_imag;
                    res = imod*Math.Sqrt(1.0D + ratio*ratio);
                }
            }
            return res;
        }

        // Absolute value (modulus) of a complex number [instance method]
        public double Abs()
        {
            double rmod = Math.Abs(m_real);
            double imod = Math.Abs(m_imag);
            double ratio = 0.0D;
            double res = 0.0D;

            if (rmod == 0.0D)
            {
                res = imod;
            }
            else
            {
                if (imod == 0.0D)
                {
                    res = rmod;
                }
                if (rmod >= imod)
                {
                    ratio = m_imag/m_real;
                    res = rmod*Math.Sqrt(1.0D + ratio*ratio);
                }
                else
                {
                    ratio = m_real/m_imag;
                    res = imod*Math.Sqrt(1.0D + ratio*ratio);
                }
            }
            return res;
        }


        // Square of the absolute value (modulus) of a complex number [static method]
        public static double squareAbs(ComplexClass a)
        {
            return a.m_real*a.m_real + a.m_imag*a.m_imag;
        }

        // Square of the absolute value (modulus) of a complex number [instance method]
        public double squareAbs()
        {
            return m_real*m_real + m_imag*m_imag;
        }

        // Argument of a complex number (in radians) [static method]
        public static double arg(ComplexClass a)
        {
            return Math.Atan2(a.m_imag, a.m_real);
        }

        // Argument of a complex number (in radians)[instance method]
        public double arg()
        {
            return Math.Atan2(m_imag, m_real);
        }

        // Argument of a complex number (in radians) [static method]
        public static double argRad(ComplexClass a)
        {
            return Math.Atan2(a.m_imag, a.m_real);
        }

        // Argument of a complex number (in radians)[instance method]
        public double argRad()
        {
            return Math.Atan2(m_imag, m_real);
        }

        // Argument of a complex number (in degrees) [static method]
        public static double argDeg(ComplexClass a)
        {
            return Converter.toDegrees(Math.Atan2(a.m_imag, a.m_real));
        }

        // Argument of a complex number (in degrees)[instance method]
        public double argDeg()
        {
            return Converter.toDegrees(Math.Atan2(m_imag, m_real));
        }

        // Complex conjugate of a complex number [static method]
        public static ComplexClass conjugate(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = a.m_real;
            c.m_imag = -a.m_imag;
            return c;
        }

        // Complex conjugate of a complex number [instance method]
        public ComplexClass conjugate()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = m_real;
            c.m_imag = -m_imag;
            return c;
        }

        // Returns the Length of the hypotenuse of a and b i.e. sqrt(abs(a)*abs(a)+abs(b)*abs(b))
        // where a and b are Complex [without unecessary overflow or underflow]
        public static double hypot(ComplexClass aa, ComplexClass bb)
        {
            double amod = Abs(aa);
            double bmod = Abs(bb);
            double cc = 0.0D, ratio = 0.0D;

            if (amod == 0.0D)
            {
                cc = bmod;
            }
            else
            {
                if (bmod == 0.0D)
                {
                    cc = amod;
                }
                else
                {
                    if (amod >= bmod)
                    {
                        ratio = bmod/amod;
                        cc = amod*Math.Sqrt(1.0 + ratio*ratio);
                    }
                    else
                    {
                        ratio = amod/bmod;
                        cc = bmod*Math.Sqrt(1.0 + ratio*ratio);
                    }
                }
            }
            return cc;
        }

        // Exponential of a complex number (instance method)
        public ComplexClass Exp()
        {
            return Exp(this);
        }

        // Exponential of a complex number (static method)
        public static ComplexClass Exp(ComplexClass aa)
        {
            ComplexClass z = new ComplexClass();

            double a = aa.m_real;
            double b = aa.m_imag;

            if (b == 0.0D)
            {
                z.m_real = Math.Exp(a);
                z.m_imag = 0.0D;
            }
            else
            {
                if (a == 0D)
                {
                    z.m_real = Math.Cos(b);
                    z.m_imag = Math.Sin(b);
                }
                else
                {
                    double c = Math.Exp(a);
                    z.m_real = c*Math.Cos(b);
                    z.m_imag = c*Math.Sin(b);
                }
            }
            return z;
        }

        // Exponential of a real number returned as a complex number
        public static ComplexClass Exp(double aa)
        {
            ComplexClass bb = new ComplexClass(aa, 0.0D);
            return Exp(bb);
        }

        // Returns Exp(j*arg) where arg is real (a double)
        public static ComplexClass expPlusJayArg(double arg)
        {
            ComplexClass argc = new ComplexClass(0.0D, arg);
            return Exp(argc);
        }

        // Returns Exp(-j*arg) where arg is real (a double)
        public static ComplexClass expMinusJayArg(double arg)
        {
            ComplexClass argc = new ComplexClass(0.0D, -arg);
            return Exp(argc);
        }

        // Principal value of the natural log of an Complex number (instance method)
        public ComplexClass Log()
        {
            double a = m_real;
            double b = m_imag;
            ComplexClass c = new ComplexClass();

            c.m_real = Math.Log(Abs(this));
            c.m_imag = Math.Atan2(b, a);

            return c;
        }

        // Principal value of the natural log of an Complex number
        public static ComplexClass Log(ComplexClass aa)
        {
            double a = aa.m_real;
            double b = aa.m_imag;
            ComplexClass c = new ComplexClass();

            c.m_real = Math.Log(Abs(aa));
            c.m_imag = Math.Atan2(b, a);

            return c;
        }

        // Roots
        // Principal value of the square root of a complex number (instance method)
        public ComplexClass Sqrt()
        {
            return Sqrt(this);
        }


        // Principal value of the square root of a complex number
        public static ComplexClass Sqrt(ComplexClass aa)
        {
            double a = aa.m_real;
            double b = aa.m_imag;
            ComplexClass c = new ComplexClass();

            if (b == 0.0D)
            {
                if (a >= 0.0D)
                {
                    c.m_real = Math.Sqrt(a);
                    c.m_imag = 0.0D;
                }
                else
                {
                    c.m_real = 0.0D;
                    c.m_imag = Math.Sqrt(-a);
                }
            }
            else
            {
                double w, ratio;
                double amod = Math.Abs(a);
                double bmod = Math.Abs(b);
                if (amod >= bmod)
                {
                    ratio = b/a;
                    w = Math.Sqrt(amod)*Math.Sqrt(0.5D*(1.0D + Math.Sqrt(1.0D + ratio*ratio)));
                }
                else
                {
                    ratio = a/b;
                    w = Math.Sqrt(bmod)*Math.Sqrt(0.5D*(Math.Abs(ratio) + Math.Sqrt(1.0D + ratio*ratio)));
                }
                if (a >= 0.0)
                {
                    c.m_real = w;
                    c.m_imag = b/(2.0D*w);
                }
                else
                {
                    if (b >= 0.0)
                    {
                        c.m_imag = w;
                        c.m_real = b/(2.0D*c.m_imag);
                    }
                    else
                    {
                        c.m_imag = -w;
                        c.m_real = b/(2.0D*c.m_imag);
                    }
                }
            }
            return c;
        }

        // Principal value of the nth root of a complex number (n = integer > 1) [instance method]
        public ComplexClass nthRoot(int n)
        {
            return nthRoot(this, n);
        }


        // Principal value of the nth root of a complex number (n = integer > 1) [static method]
        public static ComplexClass nthRoot(ComplexClass aa, int n)
        {
            ComplexClass c = new ComplexClass();
            if (n == 0)
            {
                c = new ComplexClass(double.PositiveInfinity, 0.0);
            }
            else
            {
                if (n == 1)
                {
                    c = aa;
                }
                else
                {
                    c = Exp((Log(aa)).over(n));
                }
            }

            return c;
        }

        // Powers
        // Square of a complex number (static method)
        public static ComplexClass square(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            c.m_real = aa.m_real*aa.m_real - aa.m_imag*aa.m_imag;
            c.m_imag = 2.0D*aa.m_real*aa.m_imag;
            return c;
        }

        // Square of a complex number (instance method)
        public ComplexClass square()
        {
            return times(this);
        }

        // returns a Complex number raised to a Complex power (instance method)
        public ComplexClass Pow(ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            if (isZero())
            {
                if (b.m_imag == 0)
                {
                    if (b.m_real == 0)
                    {
                        c = new ComplexClass(1.0, 0.0);
                    }
                    else
                    {
                        if (b.m_real > 0.0)
                        {
                            c = new ComplexClass(0.0, 0.0);
                        }
                        else
                        {
                            if (b.m_real < 0.0)
                            {
                                c = new ComplexClass(double.PositiveInfinity, 0.0);
                            }
                        }
                    }
                }
                else
                {
                    c = Exp(b.times(Log(this)));
                }
            }
            else
            {
                c = Exp(b.times(Log(this)));
            }

            return c;
        }

        // returns a Complex number raised to a Complex power
        public static ComplexClass Pow(ComplexClass a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            if (a.isZero())
            {
                if (b.m_imag == 0)
                {
                    if (b.m_real == 0)
                    {
                        c = new ComplexClass(1.0, 0.0);
                    }
                    else
                    {
                        if (a.m_real > 0.0)
                        {
                            c = new ComplexClass(0.0, 0.0);
                        }
                        else
                        {
                            if (a.m_real < 0.0)
                            {
                                c = new ComplexClass(double.PositiveInfinity, 0.0);
                            }
                        }
                    }
                }
                else
                {
                    c = Exp(b.times(Log(a)));
                }
            }
            else
            {
                c = Exp(b.times(Log(a)));
            }

            return c;
        }

        // returns a Complex number raised to a double power [instance method]
        public ComplexClass Pow(double b)
        {
            return powDouble(this, b);
        }

        // returns a Complex number raised to a double power
        public static ComplexClass Pow(ComplexClass a, double b)
        {
            return powDouble(a, b);
        }

        // returns a Complex number raised to an integer, i.e. int, power [instance method]
        public ComplexClass Pow(int n)
        {
            double b = n;
            return powDouble(this, b);
        }

        // returns a Complex number raised to an integer, i.e. int, power
        public static ComplexClass Pow(ComplexClass a, int n)
        {
            double b = n;
            return powDouble(a, b);
        }

        // returns a double raised to a Complex power
        public static ComplexClass Pow(double a, ComplexClass b)
        {
            ComplexClass c = new ComplexClass();
            if (a == 0)
            {
                if (b.m_imag == 0)
                {
                    if (b.m_real == 0)
                    {
                        c = new ComplexClass(1.0, 0.0);
                    }
                    else
                    {
                        if (b.m_real > 0.0)
                        {
                            c = new ComplexClass(0.0, 0.0);
                        }
                        else
                        {
                            if (b.m_real < 0.0)
                            {
                                c = new ComplexClass(double.PositiveInfinity, 0.0);
                            }
                        }
                    }
                }
                else
                {
                    double z = Math.Pow(a, b.m_real);
                    c = ComplexClass.Exp(ComplexClass.times(plusJay(), b.m_imag*Math.Log(a)));
                    c = times(z, c);
                }
            }
            else
            {
                double z = Math.Pow(a, b.m_real);
                c = ComplexClass.Exp(ComplexClass.times(plusJay(), b.m_imag*Math.Log(a)));
                c = times(z, c);
            }

            return c;
        }

        // Complex trigonometric functions

        // Sine of an Complex number
        public ComplexClass Sin()
        {
            return Sin(this);
        }

        public static ComplexClass Sin(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            double a = aa.m_real;
            double b = aa.m_imag;
            c.m_real = Math.Sin(a)*Fmath.cosh(b);
            c.m_imag = Math.Cos(a)*Fmath.sinh(b);
            return c;
        }

        // Cosine of an Complex number
        public ComplexClass Cos()
        {
            return Cos(this);
        }

        public static ComplexClass Cos(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            double a = aa.m_real;
            double b = aa.m_imag;
            c.m_real = Math.Cos(a)*Fmath.cosh(b);
            c.m_imag = -Math.Sin(a)*Fmath.sinh(b);
            return c;
        }

        // Secant of an Complex number
        public ComplexClass sec()
        {
            return sec(this);
        }

        public static ComplexClass sec(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            double a = aa.m_real;
            double b = aa.m_imag;
            c.m_real = Math.Cos(a)*Fmath.cosh(b);
            c.m_imag = -Math.Sin(a)*Fmath.sinh(b);
            return c.inverse();
        }

        // Cosecant of an Complex number
        public ComplexClass csc()
        {
            return csc(this);
        }

        public static ComplexClass csc(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            double a = aa.m_real;
            double b = aa.m_imag;
            c.m_real = Math.Sin(a)*Fmath.cosh(b);
            c.m_imag = Math.Cos(a)*Fmath.sinh(b);
            return c.inverse();
        }

        // Tangent of an Complex number
        public ComplexClass tan()
        {
            return tan(this);
        }

        public static ComplexClass tan(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            //double denom = 0.0D;
            double a = aa.m_real;
            double b = aa.m_imag;

            ComplexClass x = new ComplexClass(Math.Sin(a)*Fmath.cosh(b), Math.Cos(a)*Fmath.sinh(b));
            ComplexClass y = new ComplexClass(Math.Cos(a)*Fmath.cosh(b), -Math.Sin(a)*Fmath.sinh(b));
            c = over(x, y);
            return c;
        }

        // Cotangent of an Complex number
        public ComplexClass cot()
        {
            return cot(this);
        }

        public static ComplexClass cot(ComplexClass aa)
        {
            ComplexClass c = new ComplexClass();
            //double denom = 0.0D;
            double a = aa.m_real;
            double b = aa.m_imag;

            ComplexClass x = new ComplexClass(Math.Sin(a)*Fmath.cosh(b), Math.Cos(a)*Fmath.sinh(b));
            ComplexClass y = new ComplexClass(Math.Cos(a)*Fmath.cosh(b), -Math.Sin(a)*Fmath.sinh(b));
            c = over(y, x);
            return c;
        }

        // Exsecant of an Complex number
        public ComplexClass exsec()
        {
            return exsec(this);
        }

        public static ComplexClass exsec(ComplexClass aa)
        {
            return sec(aa).minus(1.0D);
        }

        // Versine of an Complex number
        public ComplexClass vers()
        {
            return vers(this);
        }

        public static ComplexClass vers(ComplexClass aa)
        {
            return plusOne().minus(Cos(aa));
        }

        // Coversine of an Complex number
        public ComplexClass covers()
        {
            return covers(this);
        }

        public static ComplexClass covers(ComplexClass aa)
        {
            return plusOne().minus(Sin(aa));
        }

        // Haversine of an Complex number
        public ComplexClass hav()
        {
            return hav(this);
        }

        public static ComplexClass hav(ComplexClass aa)
        {
            return vers(aa).over(2.0D);
        }

        // Hyperbolic sine of a Complex number
        public ComplexClass sinh()
        {
            return sinh(this);
        }

        public static ComplexClass sinh(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = a.times(plusJay());
            c = (minusJay()).times(Sin(c));
            return c;
        }

        // Hyperbolic cosine of a Complex number
        public ComplexClass cosh()
        {
            return cosh(this);
        }

        public static ComplexClass cosh(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = a.times(plusJay());
            c = Cos(c);
            return c;
        }

        // Hyperbolic tangent of a Complex number
        public ComplexClass tanh()
        {
            return tanh(this);
        }

        public static ComplexClass tanh(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = (sinh(a)).over(cosh(a));
            return c;
        }

        // Hyperbolic cotangent of a Complex number
        public ComplexClass coth()
        {
            return coth(this);
        }

        public static ComplexClass coth(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = (cosh(a)).over(sinh(a));
            return c;
        }

        // Hyperbolic secant of a Complex number
        public ComplexClass sech()
        {
            return sech(this);
        }

        public static ComplexClass sech(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = (cosh(a)).inverse();
            return c;
        }

        // Hyperbolic cosecant of a Complex number
        public ComplexClass csch()
        {
            return csch(this);
        }

        public static ComplexClass csch(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = (sinh(a)).inverse();
            return c;
        }


        // Inverse sine of a Complex number
        public ComplexClass asin()
        {
            return asin(this);
        }

        public static ComplexClass asin(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = Sqrt(minus(1.0D, square(a)));
            c = (plusJay().times(a)).plus(c);
            c = minusJay().times(Log(c));
            return c;
        }

        // Inverse cosine of a Complex number
        public ComplexClass Acos()
        {
            return Acos(this);
        }

        public static ComplexClass Acos(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = Sqrt(minus(square(a), 1.0));
            c = a.plus(c);
            c = minusJay().times(Log(c));
            return c;
        }

        // Inverse tangent of a Complex number
        public ComplexClass Atan()
        {
            return Atan(this);
        }

        public static ComplexClass Atan(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            ComplexClass d = new ComplexClass();

            c = plusJay().plus(a);
            d = plusJay().minus(a);
            c = c.over(d);
            c = Log(c);
            c = plusJay().times(c);
            c = c.over(2.0D);
            return c;
        }

        // Inverse cotangent of a Complex number
        public ComplexClass acot()
        {
            return acot(this);
        }

        public static ComplexClass acot(ComplexClass a)
        {
            return Atan(a.inverse());
        }

        // Inverse secant of a Complex number
        public ComplexClass asec()
        {
            return asec(this);
        }

        public static ComplexClass asec(ComplexClass a)
        {
            return Acos(a.inverse());
        }

        // Inverse cosecant of a Complex number
        public ComplexClass acsc()
        {
            return acsc(this);
        }

        public static ComplexClass acsc(ComplexClass a)
        {
            return asin(a.inverse());
        }

        // Inverse exsecant of a Complex number
        public ComplexClass aexsec()
        {
            return aexsec(this);
        }

        public static ComplexClass aexsec(ComplexClass a)
        {
            ComplexClass c = a.plus(1.0D);
            return asin(c.inverse());
        }

        // Inverse versine of a Complex number
        public ComplexClass avers()
        {
            return avers(this);
        }

        public static ComplexClass avers(ComplexClass a)
        {
            ComplexClass c = plusOne().plus(a);
            return Acos(c);
        }

        // Inverse coversine of a Complex number
        public ComplexClass acovers()
        {
            return acovers(this);
        }

        public static ComplexClass acovers(ComplexClass a)
        {
            ComplexClass c = plusOne().plus(a);
            return asin(c);
        }

        // Inverse haversine of a Complex number
        public ComplexClass ahav()
        {
            return ahav(this);
        }

        public static ComplexClass ahav(ComplexClass a)
        {
            ComplexClass c = plusOne().minus(a.times(2.0D));
            return Acos(c);
        }

        // Inverse hyperbolic sine of a Complex number
        public ComplexClass asinh()
        {
            return asinh(this);
        }

        public static ComplexClass asinh(ComplexClass a)
        {
            ComplexClass c = new ComplexClass(0.0D, 0.0D);
            c = Sqrt(square(a).plus(1.0D));
            c = a.plus(c);
            c = Log(c);

            return c;
        }

        // Inverse hyperbolic cosine of a Complex number
        public ComplexClass acosh()
        {
            return acosh(this);
        }

        public static ComplexClass acosh(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            c = Sqrt(square(a).minus(1.0D));
            c = a.plus(c);
            c = Log(c);
            return c;
        }

        // Inverse hyperbolic tangent of a Complex number
        public ComplexClass atanh()
        {
            return atanh(this);
        }

        public static ComplexClass atanh(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            ComplexClass d = new ComplexClass();
            c = plusOne().plus(a);
            d = plusOne().minus(a);
            c = c.over(d);
            c = Log(c);
            c = c.over(2.0D);
            return c;
        }

        // Inverse hyperbolic cotangent of a Complex number
        public ComplexClass acoth()
        {
            return acoth(this);
        }

        public static ComplexClass acoth(ComplexClass a)
        {
            ComplexClass c = new ComplexClass();
            ComplexClass d = new ComplexClass();
            c = plusOne().plus(a);
            d = a.plus(1.0D);
            c = c.over(d);
            c = Log(c);
            c = c.over(2.0D);
            return c;
        }

        // Inverse hyperbolic secant of a Complex number
        public ComplexClass asech()
        {
            return asech(this);
        }

        public static ComplexClass asech(ComplexClass a)
        {
            ComplexClass c = a.inverse();
            ComplexClass d = (square(a)).minus(1.0D);
            return Log(c.plus(Sqrt(d)));
        }

        // Inverse hyperbolic cosecant of a Complex number
        public ComplexClass acsch()
        {
            return acsch(this);
        }

        public static ComplexClass acsch(ComplexClass a)
        {
            ComplexClass c = a.inverse();
            ComplexClass d = (square(a)).plus(1.0D);
            return Log(c.plus(Sqrt(d)));
        }


        // LOGICAL FUNCTIONS
        // Returns true if the Complex number has a zero imaginary part, i.e. is a real number
        public static bool isReal(ComplexClass a)
        {
            bool test = false;
            if (a.m_imag == 0.0D)
            {
                test = true;
            }
            return test;
        }

        public bool isReal()
        {
            bool test = false;
            if (Math.Abs(m_imag) == 0.0D)
            {
                test = true;
            }
            return test;
        }

        // Returns true if the Complex number has a zero real and a zero imaginary part
        // i.e. has a zero modulus
        public static bool isZero(ComplexClass a)
        {
            bool test = false;
            if (Math.Abs(a.m_real) == 0.0D && Math.Abs(a.m_imag) == 0.0D)
            {
                test = true;
            }
            return test;
        }

        public bool isZero()
        {
            bool test = false;
            if (Math.Abs(m_real) == 0.0D && Math.Abs(m_imag) == 0.0D)
            {
                test = true;
            }
            return test;
        }

        // Returns true if either the real or the imaginary part of the Complex number
        // is equal to plus infinity
        public bool isPlusInfinity()
        {
            bool test = false;
            if (m_real == double.PositiveInfinity || m_imag == double.PositiveInfinity)
            {
                test = true;
            }
            return test;
        }

        public static bool isPlusInfinity(ComplexClass a)
        {
            bool test = false;
            if (a.m_real == double.PositiveInfinity || a.m_imag == double.PositiveInfinity)
            {
                test = true;
            }
            return test;
        }

        // Returns true if either the real or the imaginary part of the Complex number
        // is equal to minus infinity
        public bool isMinusInfinity()
        {
            bool test = false;
            if (m_real == double.NegativeInfinity || m_imag == double.NegativeInfinity)
            {
                test = true;
            }
            return test;
        }

        public static bool isMinusInfinity(ComplexClass a)
        {
            bool test = false;
            if (a.m_real == double.NegativeInfinity || a.m_imag == double.NegativeInfinity)
            {
                test = true;
            }
            return test;
        }


        // Returns true if either the real or the imaginary part of the Complex number
        // is equal to either infinity or minus plus infinity
        public static bool isInfinite(ComplexClass a)
        {
            bool test = false;
            if (a.m_real == double.PositiveInfinity || a.m_imag == double.PositiveInfinity)
            {
                test = true;
            }
            if (a.m_real == double.NegativeInfinity || a.m_imag == double.NegativeInfinity)
            {
                test = true;
            }
            return test;
        }

        public bool isInfinite()
        {
            bool test = false;
            if (m_real == double.PositiveInfinity || m_imag == double.PositiveInfinity)
            {
                test = true;
            }
            if (m_real == double.NegativeInfinity || m_imag == double.NegativeInfinity)
            {
                test = true;
            }
            return test;
        }


        // Returns true if the Complex number is NaN (Not a Number)
        // i.e. is the result of an uninterpretable mathematical operation
        public static bool IsNaN(ComplexClass a)
        {
            bool test = false;
            if (Double.IsNaN(a.m_real) || Double.IsNaN(a.m_imag))
            {
                test = true;
            }
            return test;
        }

        public bool IsNaN()
        {
            bool test = false;
            if (Double.IsNaN(m_real) || double.IsNaN(m_imag))
            {
                test = true;
            }
            return test;
        }

        // Returns true if two Complex number are identical
        // Follows the Sun Java convention of treating all NaNs as equal
        // i.e. does not satisfies the IEEE 754 specification
        // but does let hashtables operate properly
        public bool Equals(ComplexClass a)
        {
            bool test = false;
            if (IsNaN() && a.IsNaN())
            {
                test = true;
            }
            else
            {
                if (m_real == a.m_real && m_imag == a.m_imag)
                {
                    test = true;
                }
            }
            return test;
        }

        public bool isEqual(ComplexClass a)
        {
            bool test = false;
            if (IsNaN() && a.IsNaN())
            {
                test = true;
            }
            else
            {
                if (m_real == a.m_real && m_imag == a.m_imag)
                {
                    test = true;
                }
            }
            return test;
        }


        public static bool isEqual(ComplexClass a, ComplexClass b)
        {
            bool test = false;
            if (IsNaN(a) && IsNaN(b))
            {
                test = true;
            }
            else
            {
                if (a.m_real == b.m_real && a.m_imag == b.m_imag)
                {
                    test = true;
                }
            }
            return test;
        }


        // returns true if the differences between the real and imaginary parts of two complex numbers
        // are less than fract times the larger real and imaginary part
        public bool equalsWithinLimits(ComplexClass a, double fract)
        {
            return isEqualWithinLimits(a, fract);
        }

        public bool isEqualWithinLimits(ComplexClass a, double fract)
        {
            bool test = false;

            double rt = getReal();
            double ra = a.getReal();
            double it = getImag();
            double ia = a.getImag();
            double rdn = 0.0D;
            double idn = 0.0D;
            double rtest = 0.0D;
            double itest = 0.0D;

            if (rt == 0.0D && it == 0.0D && ra == 0.0D && ia == 0.0D)
            {
                test = true;
            }
            if (!test)
            {
                rdn = Math.Abs(rt);
                if (Math.Abs(ra) > rdn)
                {
                    rdn = Math.Abs(ra);
                }
                if (rdn == 0.0D)
                {
                    rtest = 0.0;
                }
                else
                {
                    rtest = Math.Abs(ra - rt)/rdn;
                }
                idn = Math.Abs(it);
                if (Math.Abs(ia) > idn)
                {
                    idn = Math.Abs(ia);
                }
                if (idn == 0.0D)
                {
                    itest = 0.0;
                }
                else
                {
                    itest = Math.Abs(ia - it)/idn;
                }
                if (rtest < fract && itest < fract)
                {
                    test = true;
                }
            }

            return test;
        }

        public static bool isEqualWithinLimits(ComplexClass a, ComplexClass b, double fract)
        {
            bool test = false;

            double rb = b.getReal();
            double ra = a.getReal();
            double ib = b.getImag();
            double ia = a.getImag();
            double rdn = 0.0D;
            double idn = 0.0D;

            if (ra == 0.0D && ia == 0.0D && rb == 0.0D && ib == 0.0D)
            {
                test = true;
            }
            if (!test)
            {
                rdn = Math.Abs(rb);
                if (Math.Abs(ra) > rdn)
                {
                    rdn = Math.Abs(ra);
                }
                idn = Math.Abs(ib);
                if (Math.Abs(ia) > idn)
                {
                    idn = Math.Abs(ia);
                }
                if (Math.Abs(ra - rb)/rdn < fract && Math.Abs(ia - ia)/idn < fract)
                {
                    test = true;
                }
            }

            return test;
        }

        // SOME USEFUL NUMBERS
        // returns the number zero (0) as a complex number
        public static ComplexClass zero()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = 0.0D;
            c.m_imag = 0.0D;
            return c;
        }

        // returns the number one (+1) as a complex number
        public static ComplexClass plusOne()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = 1.0D;
            c.m_imag = 0.0D;
            return c;
        }

        // returns the number minus one (-1) as a complex number
        public static ComplexClass minusOne()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = -1.0D;
            c.m_imag = 0.0D;
            return c;
        }

        // returns plus j
        public static ComplexClass plusJay()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = 0.0D;
            c.m_imag = 1.0D;
            return c;
        }

        // returns minus j
        public static ComplexClass minusJay()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = 0.0D;
            c.m_imag = -1.0D;
            return c;
        }

        // returns pi as a Complex number
        public static ComplexClass pi()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = Math.PI;
            c.m_imag = 0.0D;
            return c;
        }

        // returns 2.pi.j
        public static ComplexClass twoPiJay()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = 0.0D;
            c.m_imag = 2.0D*Math.PI;
            return c;
        }

        // infinity + infinity.j
        public static ComplexClass plusInfinity()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = double.PositiveInfinity;
            c.m_imag = double.PositiveInfinity;
            return c;
        }

        // -infinity - infinity.j
        public static ComplexClass minusInfinity()
        {
            ComplexClass c = new ComplexClass();
            c.m_real = double.NegativeInfinity;
            c.m_imag = double.NegativeInfinity;
            return c;
        }

        // PRIVATE METHODS
        // returns a Complex number raised to a double power
        // this method is used for calculation within this class file
        // see above for corresponding public method
        private static ComplexClass powDouble(ComplexClass a, double b)
        {
            ComplexClass z = new ComplexClass();
            double re = a.m_real;
            double im = a.m_imag;

            if (a.isZero())
            {
                if (b == 0.0)
                {
                    z = new ComplexClass(1.0, 0.0);
                }
                else
                {
                    if (b > 0.0)
                    {
                        z = new ComplexClass(0.0, 0.0);
                    }
                    else
                    {
                        if (b < 0.0)
                        {
                            z = new ComplexClass(double.PositiveInfinity, 0.0);
                        }
                    }
                }
            }
            else
            {
                if (im == 0.0D && re > 0.0D)
                {
                    z.m_real = Math.Pow(re, b);
                    z.m_imag = 0.0D;
                }
                else
                {
                    if (re == 0.0D)
                    {
                        z = Exp(times(b, Log(a)));
                    }
                    else
                    {
                        double c = Math.Pow(re*re + im*im, b/2.0D);
                        double th = Math.Atan2(im, re);
                        z.m_real = c*Math.Cos(b*th);
                        z.m_imag = c*Math.Sin(b*th);
                    }
                }
            }
            return z;
        }
    }
}
