#region

using System;
using System.Collections.Generic;
using HC.Core.Helpers;

//using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics
{
    /*
    *   Class   Fmath
    *
    *   USAGE:  Mathematical class that supplements java.lang.Math and contains:
    *               the main physical constants
    *               trigonemetric functions absent from java.lang.Math
    *               some useful additional mathematical functions
    *               some conversion functions
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:    June 2002
    *   AMENDED: 6 January 2006, 12 April 2006, 5 May 2006, 28 July 2006, 27 December 2006,
    *            29 March 2007, 29 April 2007, 2,9,15 & 26 June 2007, 20 October 2007, 4-6 December 2007
    *            27 February 2008, 25 April 2008, 26 April 2008, 13 May 2008, 25/26 May 2008, 3-7 July 2008
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web pages:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/Fmath.html
    *
    *   Copyright (c) 2002 - 2008
    *
    *   PERMISSION TO COPY:
    *   Permission to use, Copy and modify this software and its documentation for
    *   NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
    *   to the author, Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all copies.
    *
    *   Dr Michael Thomas Flanagan makes no representations about the suitability
    *   or fitness of the software for any or for a particular purpose.
    *   Michael Thomas Flanagan shall not be liable for any damages suffered
    *   as a result of using, modifying or distributing this software or its derivatives.
    *
    ***************************************************************************************/

    public class Fmath
    {
        // PHYSICAL CONSTANTS

        public static double C_LIGHT = 2.99792458e8; /*      m s^-1          */
        public static double E = Math.E; /*  2.718281828459045D  */
        public static double EPSILON_0 = 8.854187817e-12; /*      F m^-1          */
        public static double EULER_CONSTANT_GAMMA = 0.5772156649015627;
        public static double F_FARADAY = 9.6485341539e4; /*      C mol^-1        */
        public static double H_PLANCK = 6.6260687652e-34; /*      J s             */
        public static double H_PLANCK_RED = H_PLANCK/(2*Math.PI); /*      J s             */
        public static double K_BOLTZMANN = 1.380650324e-23; /*      J K^-1          */
        public static double M_ELECTRON = 9.1093818872e-31; /*      kg              */
        public static double M_NEUTRON = 1.6749271613e-27; /*      kg              */
        public static double M_PROTON = 1.6726215813e-27; /*      kg              */
        public static double MU_0 = Math.PI*4e-7; /*      H m^-1 (N A^-2) */
        public static double N_AVAGADRO = 6.0221419947e23; /*      mol^-1          */

        // MATHEMATICAL CONSTANTS
        public static double PI = Math.PI; /*  3.141592653589793D  */
        public static double Q_ELECTRON = -1.60217646263e-19; /*      C               */
        public static double R_GAS = 8.31447215; /*      J K^-1 mol^-1   */
        public static double T_ABS = -273.15; /*      Celsius         */

        // HashMap for 'arithmetic integer' recognition nmethod
        //private static  Map<Object,Object> integers = new HashMap<Object,Object>();
        //static{
        //    integers.put(int.class, (int.MaxValue));
        //    integers.put(long.class, (long.MaxValue));
        //    integers.put(byte.class, (byte.MaxValue));
        //    integers.put(short.class, (short.MaxValue));
        //    integers.put(int.class, (-1));
        //}

        // METHODS

        // LOGARITHMS
        // Log to base 10 of a double number
        public static double Log10(double a)
        {
            return Math.Log(a)/Math.Log(10.0D);
        }

        // Log to base 10 of a float number
        public static float Log10(float a)
        {
            return (float) (Math.Log(a)/Math.Log(10.0D));
        }

        // Base 10 antilog of a double
        public static double antilog10(double x)
        {
            return Math.Pow(10.0D, x);
        }

        // Base 10 antilog of a float
        public static float antilog10(float x)
        {
            return (float) Math.Pow(10.0D, x);
        }

        // Log to base e of a double number
        public static double Log(double a)
        {
            return Math.Log(a);
        }

        // Log to base e of a float number
        public static float Log(float a)
        {
            return (float) Math.Log(a);
        }

        // Base e antilog of a double
        public static double antilog(double x)
        {
            return Math.Exp(x);
        }

        // Base e antilog of a float
        public static float antilog(float x)
        {
            return (float) Math.Exp(x);
        }

        // Log to base 2 of a double number
        public static double log2(double a)
        {
            return Math.Log(a)/Math.Log(2.0D);
        }

        // Log to base 2 of a float number
        public static float log2(float a)
        {
            return (float) (Math.Log(a)/Math.Log(2.0D));
        }

        // Base 2 antilog of a double
        public static double antilog2(double x)
        {
            return Math.Pow(2.0D, x);
        }

        // Base 2 antilog of a float
        public static float antilog2(float x)
        {
            return (float) Math.Pow(2.0D, x);
        }

        // Log to base b of a double number and double base
        public static double Log10(double a, double b)
        {
            return Math.Log(a)/Math.Log(b);
        }

        // Log to base b of a double number and int base
        public static double Log10(double a, int b)
        {
            return Math.Log(a)/Math.Log(b);
        }

        // Log to base b of a float number and flaot base
        public static float Log10(float a, float b)
        {
            return (float) (Math.Log(a)/Math.Log(b));
        }

        // Log to base b of a float number and int base
        public static float Log10(float a, int b)
        {
            return (float) (Math.Log(a)/Math.Log(b));
        }

        // SQUARES
        // Square of a double number
        public static double square(double a)
        {
            return a*a;
        }

        // Square of a float number
        public static float square(float a)
        {
            return a*a;
        }

        // Square of an int number
        public static int square(int a)
        {
            return a*a;
        }

        // Square of a long number
        public static long square(long a)
        {
            return a*a;
        }


        // factorial of n
        // argument and return are long, therefore limited to 0<=n<=20
        // see below for double argument
        public static long factorial(long n)
        {
            if (n < 0)
            {
                throw new ArgumentException("n must be a positive integer");
            }
            if (n > 20)
            {
                throw new ArgumentException("n must less than 21 to avoid long integer overflow\nTry double argument");
            }
            long f = 1;
            long iCount = 2L;
            while (iCount <= n)
            {
                f *= iCount;
                iCount += 1L;
            }
            return f;
        }

        // factorial of n
        // Argument is of type int
        public static int factorial(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException(
                    "\nn must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
            }
            int f = 1;
            int iCount = 1;
            while (iCount.CompareTo(n) != 1)
            {
                f = f*iCount;
                iCount++;
            }
            return f;
        }

        // factorial of n
        // Argument is of type double but must be, numerically, an integer
        // factorial returned as double but is, numerically, should be an integer
        // numerical rounding may makes this an approximation after n = 21
        public static double factorial(double n)
        {
            if (n < 0.0 || (n - Math.Floor(n)) != 0)
            {
                throw new ArgumentException(
                    "\nn must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
            }
            double f = 1.0D;
            double iCount = 2.0D;
            while (iCount <= n)
            {
                f *= iCount;
                iCount += 1.0D;
            }
            return f;
        }


        // log to base e of the factorial of n
        // log[e](factorial) returned as double
        // numerical rounding may makes this an approximation
        public static double logFactorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException(
                    "\nn must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
            }
            double f = 0.0D;
            for (int i = 2; i <= n; i++)
            {
                f += Math.Log(i);
            }
            return f;
        }

        // log to base e of the factorial of n
        // Argument is of type double but must be, numerically, an integer
        // log[e](factorial) returned as double
        // numerical rounding may makes this an approximation
        public static double logFactorial(long n)
        {
            if (n < 0L)
            {
                throw new ArgumentException(
                    "\nn must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
            }
            double f = 0.0D;
            long iCount = 2L;
            while (iCount <= n)
            {
                f += Math.Log(iCount);
                iCount += 1L;
            }
            return f;
        }

        // log to base e of the factorial of n
        // Argument is of type double but must be, numerically, an integer
        // log[e](factorial) returned as double
        // numerical rounding may makes this an approximation
        public static double logFactorial(double n)
        {
            if (n < 0 || (n - Math.Floor(n)) != 0)
            {
                throw new ArgumentException(
                    "\nn must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
            }
            double f = 0.0D;
            double iCount = 2.0D;
            while (iCount <= n)
            {
                f += Math.Log(iCount);
                iCount += 1.0D;
            }
            return f;
        }


        // SIGN
        /*      returns -1 if x < 0 else returns 1   */
        //  double version
        public static double sign(double x)
        {
            if (x < 0.0)
            {
                return -1.0;
            }
            else
            {
                return 1.0;
            }
        }

        /*      returns -1 if x < 0 else returns 1   */
        //  float version
        public static float sign(float x)
        {
            if (x < 0.0F)
            {
                return -1.0F;
            }
            else
            {
                return 1.0F;
            }
        }

        /*      returns -1 if x < 0 else returns 1   */
        //  int version
        public static int sign(int x)
        {
            if (x < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /*      returns -1 if x < 0 else returns 1   */
        // long version
        public static long sign(long x)
        {
            if (x < 0)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        // ADDITIONAL TRIGONOMETRIC FUNCTIONS

        // Returns the Length of the hypotenuse of a and b
        // i.e. sqrt(a*a+b*b) [without unecessary overflow or underflow]
        // double version
        public static double hypot(double aa, double bb)
        {
            double amod = Math.Abs(aa);
            double bmod = Math.Abs(bb);
            double cc = 0.0D, ratio = 0.0D;
            if (amod == 0.0)
            {
                cc = bmod;
            }
            else
            {
                if (bmod == 0.0)
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

        // Returns the Length of the hypotenuse of a and b
        // i.e. sqrt(a*a+b*b) [without unecessary overflow or underflow]
        // float version
        public static float hypot(float aa, float bb)
        {
            return (float) hypot(aa, (double) bb);
        }

        // Angle (in radians) subtended at coordinate C
        // given x, y coordinates of all apices, A, B and C, of a triangle
        public static double angle(double xAtA, double yAtA, double xAtB, double yAtB, double xAtC, double yAtC)
        {
            double ccos = Cos(xAtA, yAtA, xAtB, yAtB, xAtC, yAtC);
            return Math.Acos(ccos);
        }

        // Angle (in radians) between sides sideA and sideB given all side lengths of a triangle
        public static double angle(double sideAC, double sideBC, double sideAB)
        {
            double ccos = Cos(sideAC, sideBC, sideAB);
            return Math.Acos(ccos);
        }

        // Sine of angle subtended at coordinate C
        // given x, y coordinates of all apices, A, B and C, of a triangle
        public static double Sin(double xAtA, double yAtA, double xAtB, double yAtB, double xAtC, double yAtC)
        {
            double angle = Fmath.angle(xAtA, yAtA, xAtB, yAtB, xAtC, yAtC);
            return Math.Sin(angle);
        }

        // Sine of angle between sides sideA and sideB given all side lengths of a triangle
        public static double Sin(double sideAC, double sideBC, double sideAB)
        {
            double angle = Fmath.angle(sideAC, sideBC, sideAB);
            return Math.Sin(angle);
        }

        // Sine given angle in radians
        // for completion - returns Math.Sin(arg)
        public static double Sin(double arg)
        {
            return Math.Sin(arg);
        }

        // Inverse sine
        // Fmath.Asin Checks limits - Java Math.Asin returns NaN if without limits
        public static double asin(double a)
        {
            if (a < -1.0D && a > 1.0D)
            {
                throw new ArgumentException("Fmath.Asin argument (" + a + ") must be >= -1.0 and <= 1.0");
            }
            return Math.Asin(a);
        }

        // Cosine of angle subtended at coordinate C
        // given x, y coordinates of all apices, A, B and C, of a triangle
        public static double Cos(double xAtA, double yAtA, double xAtB, double yAtB, double xAtC, double yAtC)
        {
            double sideAC = hypot(xAtA - xAtC, yAtA - yAtC);
            double sideBC = hypot(xAtB - xAtC, yAtB - yAtC);
            double sideAB = hypot(xAtA - xAtB, yAtA - yAtB);
            return Cos(sideAC, sideBC, sideAB);
        }

        // Cosine of angle between sides sideA and sideB given all side lengths of a triangle
        public static double Cos(double sideAC, double sideBC, double sideAB)
        {
            return 0.5D*(sideAC/sideBC + sideBC/sideAC - (sideAB/sideAC)*(sideAB/sideBC));
        }

        // Cosine given angle in radians
        // for completion - returns Java Math.Cos(arg)
        public static double Cos(double arg)
        {
            return Math.Cos(arg);
        }

        // Inverse cosine
        // Fmath.Asin Checks limits - Java Math.Asin returns NaN if without limits
        public static double Acos(double a)
        {
            if (a < -1.0D || a > 1.0D)
            {
                throw new ArgumentException("Fmath.Acos argument (" + a + ") must be >= -1.0 and <= 1.0");
            }
            return Math.Acos(a);
        }

        // Tangent of angle subtended at coordinate C
        // given x, y coordinates of all apices, A, B and C, of a triangle
        public static double tan(double xAtA, double yAtA, double xAtB, double yAtB, double xAtC, double yAtC)
        {
            double angle = Fmath.angle(xAtA, yAtA, xAtB, yAtB, xAtC, yAtC);
            return Math.Tan(angle);
        }

        // Tangent of angle between sides sideA and sideB given all side lengths of a triangle
        public static double tan(double sideAC, double sideBC, double sideAB)
        {
            double angle = Fmath.angle(sideAC, sideBC, sideAB);
            return Math.Tan(angle);
        }

        // Tangent given angle in radians
        // for completion - returns Math.Tan(arg)
        public static double tan(double arg)
        {
            return Math.Tan(arg);
        }

        // Inverse tangent
        // for completion - returns Math.Atan(arg)
        public static double Atan(double a)
        {
            return Math.Atan(a);
        }

        // Inverse tangent - ratio numerator and denominator provided
        // for completion - returns Math.Atan2(arg)
        public static double Atan2(double a, double b)
        {
            return Math.Atan2(a, b);
        }

        // Cotangent
        public static double cot(double a)
        {
            return 1.0D/Math.Tan(a);
        }

        // Inverse cotangent
        public static double acot(double a)
        {
            return Math.Atan(1.0D/a);
        }

        // Inverse cotangent - ratio numerator and denominator provided
        public static double acot2(double a, double b)
        {
            return Math.Atan2(b, a);
        }

        // Secant
        public static double sec(double a)
        {
            return 1.0/Math.Cos(a);
        }

        // Inverse secant
        public static double asec(double a)
        {
            if (a < 1.0D && a > -1.0D)
            {
                throw new ArgumentException("asec argument (" + a + ") must be >= 1 or <= -1");
            }
            return Math.Acos(1.0/a);
        }

        // Cosecant
        public static double csc(double a)
        {
            return 1.0D/Math.Sin(a);
        }

        // Inverse cosecant
        public static double acsc(double a)
        {
            if (a < 1.0D && a > -1.0D)
            {
                throw new ArgumentException("acsc argument (" + a + ") must be >= 1 or <= -1");
            }
            return Math.Asin(1.0/a);
        }

        // Exsecant
        public static double exsec(double a)
        {
            return (1.0/Math.Cos(a) - 1.0D);
        }

        // Inverse exsecant
        public static double aexsec(double a)
        {
            if (a < 0.0D && a > -2.0D)
            {
                throw new ArgumentException("aexsec argument (" + a + ") must be >= 0.0 and <= -2");
            }
            return Math.Asin(1.0D/(1.0D + a));
        }

        // Versine
        public static double vers(double a)
        {
            return (1.0D - Math.Cos(a));
        }

        // Inverse  versine
        public static double avers(double a)
        {
            if (a < 0.0D && a > 2.0D)
            {
                throw new ArgumentException("avers argument (" + a + ") must be <= 2 and >= 0");
            }
            return Math.Acos(1.0D - a);
        }

        // Coversine
        public static double covers(double a)
        {
            return (1.0D - Math.Sin(a));
        }

        // Inverse coversine
        public static double acovers(double a)
        {
            if (a < 0.0D && a > 2.0D)
            {
                throw new ArgumentException("acovers argument (" + a + ") must be <= 2 and >= 0");
            }
            return Math.Asin(1.0D - a);
        }

        // Haversine
        public static double hav(double a)
        {
            return 0.5D*vers(a);
        }

        // Inverse haversine
        public static double ahav(double a)
        {
            if (a < 0.0D && a > 1.0D)
            {
                throw new ArgumentException("ahav argument (" + a + ") must be >= 0 and <= 1");
            }
            return Acos(1.0D - 2.0D*a);
        }

        // Unnormalised sinc (unnormalised sine cardinal)   Sin(x)/x
        public static double sinc(double a)
        {
            if (Math.Abs(a) < 1e-40)
            {
                return 1.0D;
            }
            else
            {
                return Math.Sin(a)/a;
            }
        }

        // Normalised sinc (normalised sine cardinal)  Sin(pi.x)/(pi.x)
        public static double nsinc(double a)
        {
            if (Math.Abs(a) < 1e-40)
            {
                return 1.0D;
            }
            else
            {
                return Math.Sin(Math.PI*a)/(Math.PI*a);
            }
        }

        //Hyperbolic sine of a double number
        public static double sinh(double a)
        {
            return 0.5D*(Math.Exp(a) - Math.Exp(-a));
        }

        // Inverse hyperbolic sine of a double number
        public static double asinh(double a)
        {
            double sgn = 1.0D;
            if (a < 0.0D)
            {
                sgn = -1.0D;
                a = -a;
            }
            return sgn*Math.Log(a + Math.Sqrt(a*a + 1.0D));
        }

        //Hyperbolic cosine of a double number
        public static double cosh(double a)
        {
            return 0.5D*(Math.Exp(a) + Math.Exp(-a));
        }

        // Inverse hyperbolic cosine of a double number
        public static double acosh(double a)
        {
            if (a < 1.0D)
            {
                throw new ArgumentException("acosh real number argument (" + a + ") must be >= 1");
            }
            return Math.Log(a + Math.Sqrt(a*a - 1.0D));
        }

        //Hyperbolic tangent of a double number
        public static double tanh(double a)
        {
            return sinh(a)/cosh(a);
        }

        // Inverse hyperbolic tangent of a double number
        public static double atanh(double a)
        {
            double sgn = 1.0D;
            if (a < 0.0D)
            {
                sgn = -1.0D;
                a = -a;
            }
            if (a > 1.0D)
            {
                throw new ArgumentException("atanh real number argument (" + sgn*a + ") must be >= -1 and <= 1");
            }
            return 0.5D*sgn*(Math.Log(1.0D + a) - Math.Log(1.0D - a));
        }

        //Hyperbolic cotangent of a double number
        public static double coth(double a)
        {
            return 1.0D/tanh(a);
        }

        // Inverse hyperbolic cotangent of a double number
        public static double acoth(double a)
        {
            double sgn = 1.0D;
            if (a < 0.0D)
            {
                sgn = -1.0D;
                a = -a;
            }
            if (a < 1.0D)
            {
                throw new ArgumentException("acoth real number argument (" + sgn*a + ") must be <= -1 or >= 1");
            }
            return 0.5D*sgn*(Math.Log(1.0D + a) - Math.Log(a - 1.0D));
        }

        //Hyperbolic secant of a double number
        public static double sech(double a)
        {
            return 1.0D/cosh(a);
        }

        // Inverse hyperbolic secant of a double number
        public static double asech(double a)
        {
            if (a > 1.0D || a < 0.0D)
            {
                throw new ArgumentException("asech real number argument (" + a + ") must be >= 0 and <= 1");
            }
            return 0.5D*(Math.Log(1.0D/a + Math.Sqrt(1.0D/(a*a) - 1.0D)));
        }

        //Hyperbolic cosecant of a double number
        public static double csch(double a)
        {
            return 1.0D/sinh(a);
        }

        // Inverse hyperbolic cosecant of a double number
        public static double acsch(double a)
        {
            double sgn = 1.0D;
            if (a < 0.0D)
            {
                sgn = -1.0D;
                a = -a;
            }
            return 0.5D*sgn*(Math.Log(1.0/a + Math.Sqrt(1.0D/(a*a) + 1.0D)));
        }

        // MANTISSA ROUNDING (TRUNCATING)
        // returns a value of xDouble truncated to trunc double places
        public static double truncate(double xDouble, int trunc)
        {
            return NumberHelper.truncate(xDouble, trunc);
        }

        // returns a value of xFloat truncated to trunc double places
        public static float truncate(float xFloat, int trunc)
        {
            return NumberHelper.truncate(xFloat, trunc);
        }


        private static string truncateProcedure(string xValue, int trunc)
        {
            return NumberHelper.truncateProcedure(xValue, trunc);
        }

        // Returns true if x is infinite, i.e. is equal to either plus or minus infinity
        // x is double
        public static bool isInfinity(double x)
        {
            bool test = false;
            if (x == double.PositiveInfinity || x == double.NegativeInfinity)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x is infinite, i.e. is equal to either plus or minus infinity
        // x is float
        public static bool isInfinity(float x)
        {
            bool test = false;
            if (x == double.PositiveInfinity || x == double.NegativeInfinity)
            {
                test = true;
            }
            return test;
        }

        public static bool isPlusInfinity(double x)
        {
            return NumberHelper.isPlusInfinity(x);
        }

        public static bool isPlusInfinity(float x)
        {
            return NumberHelper.isPlusInfinity(x);
        }

        public static bool isMinusInfinity(double x)
        {
            return NumberHelper.isMinusInfinity(x);
        }

        public static bool isMinusInfinity(float x)
        {
            return NumberHelper.isMinusInfinity(x);
        }

        // Returns true if x Equals y
        // x and y are double
        // x may be float within range, PLUS_INFINITY, NegativeInfinity, or NaN
        // NB!! This method treats two NaNs as equal
        public static bool isEqual(double x, double y)
        {
            bool test = false;
            if (double.IsNaN(x))
            {
                if (double.IsNaN(y))
                {
                    test = true;
                }
            }
            else
            {
                if (isPlusInfinity(x))
                {
                    if (isPlusInfinity(y))
                    {
                        test = true;
                    }
                }
                else
                {
                    if (isMinusInfinity(x))
                    {
                        if (isMinusInfinity(y))
                        {
                            test = true;
                        }
                    }
                    else
                    {
                        if (x == y)
                        {
                            test = true;
                        }
                    }
                }
            }
            return test;
        }

        // Returns true if x Equals y
        // x and y are float
        // x may be float within range, PLUS_INFINITY, NegativeInfinity, or NaN
        // NB!! This method treats two NaNs as equal
        public static bool isEqual(float x, float y)
        {
            bool test = false;
            if (float.IsNaN(x))
            {
                if (float.IsNaN(y))
                {
                    test = true;
                }
            }
            else
            {
                if (isPlusInfinity(x))
                {
                    if (isPlusInfinity(y))
                    {
                        test = true;
                    }
                }
                else
                {
                    if (isMinusInfinity(x))
                    {
                        if (isMinusInfinity(y))
                        {
                            test = true;
                        }
                    }
                    else
                    {
                        if (x == y)
                        {
                            test = true;
                        }
                    }
                }
            }
            return test;
        }

        // Returns true if x Equals y
        // x and y are int
        public static bool isEqual(int x, int y)
        {
            bool test = false;
            if (x == y)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y
        // x and y are char
        public static bool isEqual(char x, char y)
        {
            bool test = false;
            if (x == y)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y
        // x and y are Strings
        public static bool isEqual(string x, string y)
        {
            bool test = false;
            if (x.Equals(y))
            {
                test = true;
            }
            return test;
        }

        // IS EQUAL WITHIN LIMITS
        // Returns true if x Equals y within limits plus or minus limit
        // x and y are double
        public static bool isEqualWithinLimits(double x, double y, double limit)
        {
            bool test = false;
            if (Math.Abs(x - y) <= Math.Abs(limit))
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within limits plus or minus limit
        // x and y are float
        public static bool isEqualWithinLimits(float x, float y, float limit)
        {
            bool test = false;
            if (Math.Abs(x - y) <= Math.Abs(limit))
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within limits plus or minus limit
        // x and y are long
        public static bool isEqualWithinLimits(long x, long y, long limit)
        {
            bool test = false;
            if (Math.Abs(x - y) <= Math.Abs(limit))
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within limits plus or minus limit
        // x and y are int
        //public static bool isEqualWithinLimits(int x, int y, int limit){
        //    bool test=false;
        //    if(Math.Abs(x-y)<=Math.Abs(limit))test=true;
        //    return test;
        //}

        // Returns true if x Equals y within limits plus or minus limit
        // x and y are int
        public static bool isEqualWithinLimits(int x, int y, int limit)
        {
            bool test = false;

            if (Math.Abs(x - y) - Math.Abs(limit) <= 0)
            {
                test = true;
            }
            return test;
        }


        // IS EQUAL WITHIN A PERCENTAGE
        // Returns true if x Equals y within a percentage of the mean
        // x and y are double
        public static bool isEqualWithinPerCent(double x, double y, double perCent)
        {
            bool test = false;
            double limit = Math.Abs((x + y)*perCent/200.0D);
            if (Math.Abs(x - y) <= limit)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within a percentage of the mean
        // x and y are float
        public static bool isEqualWithinPerCent(float x, float y, float perCent)
        {
            bool test = false;
            double limit = Math.Abs((x + y)*perCent/200.0F);
            if (Math.Abs(x - y) <= limit)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within a percentage of the mean
        // x and y are long, percentage provided as double
        public static bool isEqualWithinPerCent(long x, long y, double perCent)
        {
            bool test = false;
            double limit = Math.Abs((x + y)*perCent/200.0D);
            if (Math.Abs(x - y) <= limit)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within a percentage of the mean
        // x and y are long, percentage provided as int
        public static bool isEqualWithinPerCent(long x, long y, long perCent)
        {
            bool test = false;
            double limit = Math.Abs((x + y)*(double) perCent/200.0D);
            if (Math.Abs(x - y) <= limit)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within a percentage of the mean
        // x and y are int, percentage provided as double
        public static bool isEqualWithinPerCent(int x, int y, double perCent)
        {
            bool test = false;
            double limit = Math.Abs((x + y)*perCent/200.0D);
            if (Math.Abs(x - y) <= limit)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x Equals y within a percentage of the mean
        // x and y are int, percentage provided as int
        //public static bool isEqualWithinPerCent(int x, int y, int perCent){
        //    bool test=false;
        //    double limit = Math.Abs((double)(x+y)*(double)perCent/200.0D);
        //    if(Math.Abs(x-y)<=limit)test=true;
        //    return test;
        //}

        // Returns true if x Equals y within a percentage of the mean
        // x and y are BigDInteger, percentage provided as int
        public static bool isEqualWithinPerCent(
            int x,
            int y,
            int perCent)
        {
            bool test = false;
            double limit = (x + y)*(perCent)*(0.005);
            if (Math.Abs(x - y).CompareTo(Math.Abs(limit)) <= 0)
            {
                test = true;
            }
            return test;
        }


        // IS AN INTEGER
        // Returns true if x is, arithmetically, an integer
        // Returns false if x is not, arithmetically, an integer
        public static bool isInteger(double x)
        {
            bool retn = false;
            double xfloor = Math.Floor(x);
            if ((x - xfloor) == 0.0D)
            {
                retn = true;
            }
            return retn;
        }

        // Returns true if all elements in the array x are, arithmetically, integers
        // Returns false if any element in the array x is not, arithmetically, an integer
        public static bool isInteger(double[] x)
        {
            bool retn = true;
            bool test = true;
            int ii = 0;
            while (test)
            {
                double xfloor = Math.Floor(x[ii]);
                if ((x[ii] - xfloor) != 0.0D)
                {
                    retn = false;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii == x.Length)
                    {
                        test = false;
                    }
                }
            }
            return retn;
        }

        public static bool isInteger(decimal[] x)
        {
            bool retn = true;
            bool test = true;
            int ii = 0;
            while (test)
            {
                decimal xfloor = Math.Floor(x[ii]);
                if ((x[ii] - xfloor) != 0)
                {
                    retn = false;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii == x.Length)
                    {
                        test = false;
                    }
                }
            }
            return retn;
        }

        // Returns true if x is, arithmetically, an integer
        // Returns false if x is not, arithmetically, an integer
        public static bool isInteger(float x)
        {
            bool ret = false;
            float xfloor = (float) Math.Floor(x);
            if ((x - xfloor) == 0.0F)
            {
                ret = true;
            }
            return ret;
        }


        // Returns true if all elements in the array x are, arithmetically, integers
        // Returns false if any element in the array x is not, arithmetically, an integer
        public static bool isInteger(float[] x)
        {
            bool retn = true;
            bool test = true;
            int ii = 0;
            while (test)
            {
                float xfloor = (float) Math.Floor(x[ii]);
                if ((x[ii] - xfloor) != 0.0D)
                {
                    retn = false;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii == x.Length)
                    {
                        test = false;
                    }
                }
            }
            return retn;
        }

        //public static bool isInteger (Number numberAsObject){
        //    bool test = integers.containsKey(numberAsObject.getClass());
        //    if(!test){
        //        if(numberAsObject is double){
        //            double dd = numberAsObject;
        //            test = Fmath.isInteger(dd);
        //        }
        //        if(numberAsObject is double){
        //            float dd = numberAsObject;
        //            test = Fmath.isInteger(dd);
        //        }
        //        if(numberAsObject is double){
        //            double dd = numberAsObject;
        //            test = Fmath.isInteger(dd);
        //        }
        //    }
        //    return test;
        //}

        //public static bool isInteger (Number[] numberAsObject){
        //    bool testall = true;
        //    for(int i=0; i<numberAsObject.Length; i++){
        //        bool test = integers.containsKey(numberAsObject[i].getClass());
        //        if(!test){
        //            if(numberAsObject[i] is double){
        //                double dd = numberAsObject[i];
        //                test = Fmath.isInteger(dd);
        //                if(!test)testall = false;
        //            }
        //            if(numberAsObject[i] is double){
        //                float dd = numberAsObject[i];
        //                test = Fmath.isInteger(dd);
        //                if(!test)testall = false;
        //            }
        //            if(numberAsObject[i] is double){
        //                double dd = numberAsObject[i];
        //                test = Fmath.isInteger(dd);
        //                if(!test)testall = false;
        //            }
        //        }
        //    }
        //    return testall;
        //}

        // IS EVEN
        // Returns true if x is an even number, false if x is an odd number
        // x is int
        public static bool isEven(int x)
        {
            bool test = false;
            if (x%2 == 0.0D)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x is an even number, false if x is an odd number
        // x is float but must hold an integer value
        public static bool isEven(float x)
        {
            double y = Math.Floor(x);
            if ((x - y) != 0.0D)
            {
                throw new ArgumentException("the argument is not an integer");
            }
            bool test = false;
            y = Math.Floor(x/2.0F);
            if (((x/2.0F) - y) == 0.0D)
            {
                test = true;
            }
            return test;
        }

        // Returns true if x is an even number, false if x is an odd number
        // x is double but must hold an integer value
        public static bool isEven(double x)
        {
            double y = Math.Floor(x);
            if ((x - y) != 0.0D)
            {
                throw new ArgumentException("the argument is not an integer");
            }
            bool test = false;
            y = Math.Floor(x/2.0F);
            if ((x/2.0D - y) == 0.0D)
            {
                test = true;
            }
            return test;
        }

        // IS ODD
        // Returns true if x is an odd number, false if x is an even number
        // x is int
        public static bool isOdd(int x)
        {
            bool test = true;
            if (x%2 == 0.0D)
            {
                test = false;
            }
            return test;
        }

        // Returns true if x is an odd number, false if x is an even number
        // x is float but must hold an integer value
        public static bool isOdd(float x)
        {
            double y = Math.Floor(x);
            if ((x - y) != 0.0D)
            {
                throw new ArgumentException("the argument is not an integer");
            }
            bool test = true;
            y = Math.Floor(x/2.0F);
            if (((x/2.0F) - y) == 0.0D)
            {
                test = false;
            }
            return test;
        }

        // Returns true if x is an odd number, false if x is an even number
        // x is double but must hold an integer value
        public static bool isOdd(double x)
        {
            double y = Math.Floor(x);
            if ((x - y) != 0.0D)
            {
                throw new ArgumentException("the argument is not an integer");
            }
            bool test = true;
            y = Math.Floor(x/2.0F);
            if ((x/2.0D - y) == 0.0D)
            {
                test = false;
            }
            return test;
        }

        // LEAP YEAR
        // Returns true if year (argument) is a leap year
        public static bool leapYear(int year)
        {
            bool test = false;

            if (year%4 != 0)
            {
                test = false;
            }
            else
            {
                if (year%400 == 0)
                {
                    test = true;
                }
                else
                {
                    if (year%100 == 0)
                    {
                        test = false;
                    }
                    else
                    {
                        test = true;
                    }
                }
            }
            return test;
        }

        // COMPUTER TIME
        // Returns milliseconds since 0 hours 0 minutes 0 seconds on 1 Jan 1970
        public static long dateToJavaMilliS(int year, int month, int day, int hour, int min, int sec)
        {
            long[] monthDays = {0L, 31L, 28L, 31L, 30L, 31L, 30L, 31L, 31L, 30L, 31L, 30L, 31L};
            long ms = 0L;

            long yearDiff = 0L;
            int yearTest = year - 1;
            while (yearTest >= 1970)
            {
                yearDiff += 365;
                if (leapYear(yearTest))
                {
                    yearDiff++;
                }
                yearTest--;
            }
            yearDiff *= 24L*60L*60L*1000L;

            long monthDiff = 0L;
            int monthTest = month - 1;
            while (monthTest > 0)
            {
                monthDiff += monthDays[monthTest];
                if (leapYear(year))
                {
                    monthDiff++;
                }
                monthTest--;
            }

            monthDiff *= 24L*60L*60L*1000L;

            ms = yearDiff + monthDiff + day*24L*60L*60L*1000L + hour*60L*60L*1000L + min*60L*1000L + sec*1000L;

            return ms;
        }

        // DEPRECATED METHODS
        // Several methods have been revised and moved to classes ArrayMaths, Conv or PrintToScreen

        // ARRAY MAXIMUM  (deprecated - see ArryMaths class)
        // Maximum of a 1D array of doubles, aa
        public static double maximum(double[] aa)
        {
            int n = aa.Length;
            double aamax = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] > aamax)
                {
                    aamax = aa[i];
                }
            }
            return aamax;
        }

        // Maximum of a 1D array of floats, aa
        public static float maximum(float[] aa)
        {
            int n = aa.Length;
            float aamax = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] > aamax)
                {
                    aamax = aa[i];
                }
            }
            return aamax;
        }

        // Maximum of a 1D array of ints, aa
        public static int maximum(int[] aa)
        {
            int n = aa.Length;
            int aamax = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] > aamax)
                {
                    aamax = aa[i];
                }
            }
            return aamax;
        }

        // Maximum of a 1D array of longs, aa
        public static long maximum(long[] aa)
        {
            long n = aa.Length;
            long aamax = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] > aamax)
                {
                    aamax = aa[i];
                }
            }
            return aamax;
        }

        // Minimum of a 1D array of doubles, aa
        public static double minimum(double[] aa)
        {
            int n = aa.Length;
            double aamin = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] < aamin)
                {
                    aamin = aa[i];
                }
            }
            return aamin;
        }

        // Minimum of a 1D array of floats, aa
        public static float minimum(float[] aa)
        {
            int n = aa.Length;
            float aamin = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] < aamin)
                {
                    aamin = aa[i];
                }
            }
            return aamin;
        }

        // ARRAY MINIMUM (deprecated - see ArryMaths class)
        // Minimum of a 1D array of ints, aa
        public static int minimum(int[] aa)
        {
            int n = aa.Length;
            int aamin = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] < aamin)
                {
                    aamin = aa[i];
                }
            }
            return aamin;
        }

        // Minimum of a 1D array of longs, aa
        public static long minimum(long[] aa)
        {
            long n = aa.Length;
            long aamin = aa[0];
            for (int i = 1; i < n; i++)
            {
                if (aa[i] < aamin)
                {
                    aamin = aa[i];
                }
            }
            return aamin;
        }

        // MAXIMUM DISTANCE BETWEEN ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Maximum distance between elements of a 1D array of doubles, aa
        public static double maximumDifference(double[] aa)
        {
            return maximum(aa) - minimum(aa);
        }

        // Maximum distance between elements of a 1D array of floats, aa
        public static float maximumDifference(float[] aa)
        {
            return maximum(aa) - minimum(aa);
        }

        // Maximum distance between elements of a 1D array of long, aa
        public static long maximumDifference(long[] aa)
        {
            return maximum(aa) - minimum(aa);
        }

        // Maximum distance between elements of a 1D array of ints, aa
        public static int maximumDifference(int[] aa)
        {
            return maximum(aa) - minimum(aa);
        }


        // MINIMUM DISTANCE BETWEEN ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Minimum distance between elements of a 1D array of doubles, aa
        public static double minimumDifference(double[] aa)
        {
            double[] sorted = selectionSort(aa);
            double n = aa.Length;
            double diff = sorted[1] - sorted[0];
            double minDiff = diff;
            for (int i = 1; i < n - 1; i++)
            {
                diff = sorted[i + 1] - sorted[i];
                if (diff < minDiff)
                {
                    minDiff = diff;
                }
            }
            return minDiff;
        }

        // Minimum distance between elements of a 1D array of floats, aa
        public static float minimumDifference(float[] aa)
        {
            float[] sorted = selectionSort(aa);
            float n = aa.Length;
            float diff = sorted[1] - sorted[0];
            float minDiff = diff;
            for (int i = 1; i < n - 1; i++)
            {
                diff = sorted[i + 1] - sorted[i];
                if (diff < minDiff)
                {
                    minDiff = diff;
                }
            }
            return minDiff;
        }

        // Minimum distance between elements of a 1D array of longs, aa
        public static long minimumDifference(long[] aa)
        {
            long[] sorted = selectionSort(aa);
            long n = aa.Length;
            long diff = sorted[1] - sorted[0];
            long minDiff = diff;
            for (int i = 1; i < n - 1; i++)
            {
                diff = sorted[i + 1] - sorted[i];
                if (diff < minDiff)
                {
                    minDiff = diff;
                }
            }
            return minDiff;
        }

        // Minimum distance between elements of a 1D array of ints, aa
        public static int minimumDifference(int[] aa)
        {
            int[] sorted = selectionSort(aa);
            int n = aa.Length;
            int diff = sorted[1] - sorted[0];
            int minDiff = diff;
            for (int i = 1; i < n - 1; i++)
            {
                diff = sorted[i + 1] - sorted[i];
                if (diff < minDiff)
                {
                    minDiff = diff;
                }
            }
            return minDiff;
        }

        // REVERSE ORDER OF ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Reverse the order of the elements of a 1D array of doubles, aa
        public static double[] reverseArray(double[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[n - 1 - i];
            }
            return bb;
        }

        // Reverse the order of the elements of a 1D array of floats, aa
        public static float[] reverseArray(float[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[n - 1 - i];
            }
            return bb;
        }

        // Reverse the order of the elements of a 1D array of ints, aa
        public static int[] reverseArray(int[] aa)
        {
            int n = aa.Length;
            int[] bb = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[n - 1 - i];
            }
            return bb;
        }

        // Reverse the order of the elements of a 1D array of longs, aa
        public static long[] reverseArray(long[] aa)
        {
            int n = aa.Length;
            long[] bb = new long[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[n - 1 - i];
            }
            return bb;
        }

        // Reverse the order of the elements of a 1D array of char, aa
        public static char[] reverseArray(char[] aa)
        {
            int n = aa.Length;
            char[] bb = new char[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[n - 1 - i];
            }
            return bb;
        }

        // ABSOLUTE VALUE OF ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // return absolute values of an array of doubles
        public static double[] arrayAbs(double[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Abs(aa[i]);
            }
            return bb;
        }

        // return absolute values of an array of floats
        public static float[] arrayAbs(float[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Abs(aa[i]);
            }
            return bb;
        }

        // return absolute values of an array of long
        public static long[] arrayAbs(long[] aa)
        {
            int n = aa.Length;
            long[] bb = new long[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Abs(aa[i]);
            }
            return bb;
        }

        // return absolute values of an array of int
        public static int[] arrayAbs(int[] aa)
        {
            int n = aa.Length;
            int[] bb = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Abs(aa[i]);
            }
            return bb;
        }

        // MULTIPLY ARRAY ELEMENTS BY A CONSTANT  (deprecated - see ArryMaths class)
        // multiply all elements by a constant double[] by double -> double[]
        public static double[] arrayMultByConstant(double[] aa, double constant)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i]*constant;
            }
            return bb;
        }

        // multiply all elements by a constant int[] by double -> double[]
        public static double[] arrayMultByConstant(int[] aa, double constant)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i]*constant;
            }
            return bb;
        }

        // multiply all elements by a constant double[] by int -> double[]
        public static double[] arrayMultByConstant(double[] aa, int constant)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i]*constant;
            }
            return bb;
        }

        // multiply all elements by a constant int[] by int -> double[]
        public static double[] arrayMultByConstant(int[] aa, int constant)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (aa[i]*constant);
            }
            return bb;
        }

        // LOG10 OF ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Log to base 10 of all elements of an array of doubles
        public static double[] log10Elements(double[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Log10(aa[i]);
            }
            return bb;
        }

        // Log to base 10 of all elements of an array of floats
        public static float[] log10Elements(float[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (float) Math.Log10(aa[i]);
            }
            return bb;
        }

        // NATURAL LOG OF ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Log to base e of all elements of an array of doubles
        public static double[] lnElements(double[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Log10(aa[i]);
            }
            return bb;
        }

        // Log to base e of all elements of an array of floats
        public static float[] lnElements(float[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (float) Math.Log10(aa[i]);
            }
            return bb;
        }

        // SQUARE ROOT OF ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Square root all elements of an array of doubles
        public static double[] squareRootElements(double[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Sqrt(aa[i]);
            }
            return bb;
        }

        // Square root all elements of an array of floats
        public static float[] squareRootElements(float[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (float) Math.Sqrt(aa[i]);
            }
            return bb;
        }

        // POWER OF ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // Raise all elements of an array of doubles to a double power
        public static double[] raiseElementsToPower(double[] aa, double power)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Pow(aa[i], power);
            }
            return bb;
        }

        // Raise all elements of an array of doubles to an int power
        public static double[] raiseElementsToPower(double[] aa, int power)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = Math.Pow(aa[i], power);
            }
            return bb;
        }

        // Raise all elements of an array of floats to a float power
        public static float[] raiseElementsToPower(float[] aa, float power)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (float) Math.Pow(aa[i], power);
            }
            return bb;
        }

        // Raise all elements of an array of floats to an int power
        public static float[] raiseElementsToPower(float[] aa, int power)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (float) Math.Pow(aa[i], power);
            }
            return bb;
        }

        // INVERT ARRAY ELEMENTS  (deprecated - see ArryMaths class)
        // invert all elements of an array of doubles
        public static double[] invertElements(double[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = 1.0D/aa[i];
            }
            return bb;
        }

        // invert all elements of an array of floats
        public static float[] invertElements(float[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = 1.0F/aa[i];
            }
            return bb;
        }


        // FIND INDICES OF ARRAY ELEMENTS EQUAL TO A VALUE  (deprecated - see ArryMaths class)
        // finds the indices of the elements equal to a given value in an array of doubles
        // returns null if none found
        public static int[] indicesOf(double[] array, double value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of floats
        // returns null if none found
        public static int[] indicesOf(float[] array, float value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of longs
        // returns null if none found
        public static int[] indicesOf(long[] array, long value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of ints
        // returns null if none found
        public static int[] indicesOf(int[] array, int value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of shorts
        // returns null if none found
        public static int[] indicesOf(short[] array, short value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of bytes
        // returns null if none found
        public static int[] indicesOf(byte[] array, byte value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of chars
        // returns null if none found
        public static int[] indicesOf(char[] array, char value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of Strings
        // returns null if none found
        public static int[] indicesOf(string[] array, string value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // finds the indices of the elements equal to a given value in an array of Objectss
        // returns null if none found
        public static int[] indicesOf(Object[] array, Object value)
        {
            int[] indices = null;
            int numberOfIndices = 0;
            List<int> arrayl = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                {
                    numberOfIndices++;
                    arrayl.Add(i);
                }
            }
            if (numberOfIndices != 0)
            {
                indices = new int[numberOfIndices];
                for (int i = 0; i < numberOfIndices; i++)
                {
                    indices[i] = arrayl[i];
                }
            }
            return indices;
        }

        // FIND FIRST INDEX OF ARRAY ELEMENT EQUAL TO A VALUE  (deprecated - see ArryMaths class)
        // finds the index of the first occurence of the element equal to a given value in an array of doubles
        // returns -1 if none found
        public static int IndexOf(double[] array, double value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of floats
        // returns -1 if none found
        public static int IndexOf(float[] array, float value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of longs
        // returns -1 if none found
        public static int IndexOf(long[] array, long value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of ints
        // returns -1 if none found
        public static int IndexOf(int[] array, int value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of bytes
        // returns -1 if none found
        public static int IndexOf(byte[] array, byte value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of shorts
        // returns -1 if none found
        public static int IndexOf(short[] array, short value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of chars
        // returns -1 if none found
        public static int IndexOf(char[] array, char value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter] == value)
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of Strings
        // returns -1 if none found
        public static int IndexOf(string[] array, string value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter].Equals(value))
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // finds the index of the first occurence of the element equal to a given value in an array of Objects
        // returns -1 if none found
        public static int IndexOf(Object[] array, Object value)
        {
            int index = -1;
            bool test = true;
            int counter = 0;
            while (test)
            {
                if (array[counter].Equals(value))
                {
                    index = counter;
                    test = false;
                }
                else
                {
                    counter++;
                    if (counter >= array.Length)
                    {
                        test = false;
                    }
                }
            }
            return index;
        }

        // FIND  VALUE OF AND FIND VALUE OF ARRAY ELEMENTS NEAREST TO A VALUE  (deprecated - see ArryMaths class)
        // finds the value of nearest element value in array to the argument value
        public static double nearestElementValue(double[] array, double value)
        {
            double diff = Math.Abs(array[0] - value);
            double nearest = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (Math.Abs(array[i] - value) < diff)
                {
                    diff = Math.Abs(array[i] - value);
                    nearest = array[i];
                }
            }
            return nearest;
        }

        // finds the index of nearest element value in array to the argument value
        public static int nearestElementIndex(double[] array, double value)
        {
            double diff = Math.Abs(array[0] - value);
            int nearest = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (Math.Abs(array[i] - value) < diff)
                {
                    diff = Math.Abs(array[i] - value);
                    nearest = i;
                }
            }
            return nearest;
        }

        // finds the value of nearest lower element value in array to the argument value
        public static double nearestLowerElementValue(double[] array, double value)
        {
            double diff0 = 0.0D;
            double diff1 = 0.0D;
            double nearest = 0.0D;
            int ii = 0;
            bool test = true;
            double min = array[0];
            while (test)
            {
                if (array[ii] < min)
                {
                    min = array[ii];
                }
                if ((value - array[ii]) >= 0.0D)
                {
                    diff0 = value - array[ii];
                    nearest = array[ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = min;
                        diff0 = min - value;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = value - array[i];
                if (diff1 >= 0.0D && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = array[i];
                }
            }
            return nearest;
        }

        // finds the index of nearest lower element value in array to the argument value
        public static int nearestLowerElementIndex(double[] array, double value)
        {
            double diff0 = 0.0D;
            double diff1 = 0.0D;
            int nearest = 0;
            int ii = 0;
            bool test = true;
            double min = array[0];
            int minI = 0;
            while (test)
            {
                if (array[ii] < min)
                {
                    min = array[ii];
                    minI = ii;
                }
                if ((value - array[ii]) >= 0.0D)
                {
                    diff0 = value - array[ii];
                    nearest = ii;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = minI;
                        diff0 = min - value;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = value - array[i];
                if (diff1 >= 0.0D && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = i;
                }
            }
            return nearest;
        }

        // finds the value of nearest higher element value in array to the argument value
        public static double nearestHigherElementValue(double[] array, double value)
        {
            double diff0 = 0.0D;
            double diff1 = 0.0D;
            double nearest = 0.0D;
            int ii = 0;
            bool test = true;
            double max = array[0];
            while (test)
            {
                if (array[ii] > max)
                {
                    max = array[ii];
                }
                if ((array[ii] - value) >= 0.0D)
                {
                    diff0 = value - array[ii];
                    nearest = array[ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = max;
                        diff0 = value - max;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = array[i] - value;
                if (diff1 >= 0.0D && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = array[i];
                }
            }
            return nearest;
        }

        // finds the index of nearest higher element value in array to the argument value
        public static int nearestHigherElementIndex(double[] array, double value)
        {
            double diff0 = 0.0D;
            double diff1 = 0.0D;
            int nearest = 0;
            int ii = 0;
            bool test = true;
            double max = array[0];
            int maxI = 0;
            while (test)
            {
                if (array[ii] > max)
                {
                    max = array[ii];
                    maxI = ii;
                }
                if ((array[ii] - value) >= 0.0D)
                {
                    diff0 = value - array[ii];
                    nearest = ii;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = maxI;
                        diff0 = value - max;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = array[i] - value;
                if (diff1 >= 0.0D && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = i;
                }
            }
            return nearest;
        }


        // finds the value of nearest element value in array to the argument value
        public static int nearestElementValue(int[] array, int value)
        {
            int diff = Math.Abs(array[0] - value);
            int nearest = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (Math.Abs(array[i] - value) < diff)
                {
                    diff = Math.Abs(array[i] - value);
                    nearest = array[i];
                }
            }
            return nearest;
        }

        // finds the index of nearest element value in array to the argument value
        public static int nearestElementIndex(int[] array, int value)
        {
            int diff = Math.Abs(array[0] - value);
            int nearest = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (Math.Abs(array[i] - value) < diff)
                {
                    diff = Math.Abs(array[i] - value);
                    nearest = i;
                }
            }
            return nearest;
        }

        // finds the value of nearest lower element value in array to the argument value
        public static int nearestLowerElementValue(int[] array, int value)
        {
            int diff0 = 0;
            int diff1 = 0;
            int nearest = 0;
            int ii = 0;
            bool test = true;
            int min = array[0];
            while (test)
            {
                if (array[ii] < min)
                {
                    min = array[ii];
                }
                if ((value - array[ii]) >= 0)
                {
                    diff0 = value - array[ii];
                    nearest = array[ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = min;
                        diff0 = min - value;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = value - array[i];
                if (diff1 >= 0 && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = array[i];
                }
            }
            return nearest;
        }

        // finds the index of nearest lower element value in array to the argument value
        public static int nearestLowerElementIndex(int[] array, int value)
        {
            int diff0 = 0;
            int diff1 = 0;
            int nearest = 0;
            int ii = 0;
            bool test = true;
            int min = array[0];
            int minI = 0;
            while (test)
            {
                if (array[ii] < min)
                {
                    min = array[ii];
                    minI = ii;
                }
                if ((value - array[ii]) >= 0)
                {
                    diff0 = value - array[ii];
                    nearest = ii;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = minI;
                        diff0 = min - value;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = value - array[i];
                if (diff1 >= 0 && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = i;
                }
            }
            return nearest;
        }

        // finds the value of nearest higher element value in array to the argument value
        public static int nearestHigherElementValue(int[] array, int value)
        {
            int diff0 = 0;
            int diff1 = 0;
            int nearest = 0;
            int ii = 0;
            bool test = true;
            int max = array[0];
            while (test)
            {
                if (array[ii] > max)
                {
                    max = array[ii];
                }
                if ((array[ii] - value) >= 0)
                {
                    diff0 = value - array[ii];
                    nearest = array[ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = max;
                        diff0 = value - max;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = array[i] - value;
                if (diff1 >= 0 && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = array[i];
                }
            }
            return nearest;
        }

        // finds the index of nearest higher element value in array to the argument value
        public static int nearestHigherElementIndex(int[] array, int value)
        {
            int diff0 = 0;
            int diff1 = 0;
            int nearest = 0;
            int ii = 0;
            bool test = true;
            int max = array[0];
            int maxI = 0;
            while (test)
            {
                if (array[ii] > max)
                {
                    max = array[ii];
                    maxI = ii;
                }
                if ((array[ii] - value) >= 0)
                {
                    diff0 = value - array[ii];
                    nearest = ii;
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii > array.Length - 1)
                    {
                        nearest = maxI;
                        diff0 = value - max;
                        test = false;
                    }
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                diff1 = array[i] - value;
                if (diff1 >= 0 && diff1 < diff0)
                {
                    diff0 = diff1;
                    nearest = i;
                }
            }
            return nearest;
        }

        // SUM OF ALL ELEMENTS  (deprecated - see ArryMaths class)
        // Sum of all array elements - double array
        public static double arraySum(double[] array)
        {
            double sum = 0.0D;
            foreach (double i in array)
            {
                sum += i;
            }
            return sum;
        }

        // Sum of all array elements - float array
        public static float arraySum(float[] array)
        {
            float sum = 0.0F;
            foreach (float i in array)
            {
                sum += i;
            }
            return sum;
        }

        // Sum of all array elements - int array
        public static int arraySum(int[] array)
        {
            int sum = 0;
            foreach (int i in array)
            {
                sum += i;
            }
            return sum;
        }

        // Sum of all array elements - long array
        public static long arraySum(long[] array)
        {
            long sum = 0L;
            foreach (long i in array)
            {
                sum += i;
            }
            return sum;
        }

        // Sum of all positive array elements - long array
        public static long arrayPositiveElementsSum(long[] array)
        {
            long sum = 0L;
            foreach (long i in array)
            {
                if (i > 0)
                {
                    sum += i;
                }
            }
            return sum;
        }


        // PRODUCT OF ALL ELEMENTS  (deprecated - see ArryMaths class)
        // Product of all array elements - double array
        public static double arrayProduct(double[] array)
        {
            double product = 1.0D;
            foreach (double i in array)
            {
                product *= i;
            }
            return product;
        }

        // Product of all array elements - float array
        public static float arrayProduct(float[] array)
        {
            float product = 1.0F;
            foreach (float i in array)
            {
                product *= i;
            }
            return product;
        }

        // Product of all array elements - int array
        public static int arrayProduct(int[] array)
        {
            int product = 1;
            foreach (int i in array)
            {
                product *= i;
            }
            return product;
        }

        // Product of all array elements - long array
        public static long arrayProduct(long[] array)
        {
            long product = 1L;
            foreach (long i in array)
            {
                product *= i;
            }
            return product;
        }

        // CONCATENATE TWO ARRAYS  (deprecated - see ArryMaths class)
        // Concatenate two double arrays
        public static double[] concatenate(double[] aa, double[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            double[] cc = new double[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }
            return cc;
        }

        // Concatenate two float arrays
        public static float[] concatenate(float[] aa, float[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            float[] cc = new float[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // Concatenate two int arrays
        public static int[] concatenate(int[] aa, int[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            int[] cc = new int[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // Concatenate two long arrays
        public static long[] concatenate(long[] aa, long[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            long[] cc = new long[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // Concatenate two short arrays
        public static short[] concatenate(short[] aa, short[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            short[] cc = new short[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }
            return cc;
        }

        // Concatenate two byte arrays
        public static byte[] concatenate(byte[] aa, byte[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            byte[] cc = new byte[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // Concatenate two char arrays
        public static char[] concatenate(char[] aa, char[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            char[] cc = new char[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // Concatenate two string arrays
        public static string[] concatenate(string[] aa, string[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            string[] cc = new string[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // Concatenate two Object arrays
        public static Object[] concatenate(Object[] aa, Object[] bb)
        {
            int aLen = aa.Length;
            int bLen = bb.Length;
            int cLen = aLen + bLen;
            Object[] cc = new Object[cLen];
            for (int i = 0; i < aLen; i++)
            {
                cc[i] = aa[i];
            }
            for (int i = 0; i < bLen; i++)
            {
                cc[i + aLen] = bb[i];
            }

            return cc;
        }

        // RECAST ARRAY TYPE  (deprecated - see Conv class)
        // recast an array of float as doubles
        public static double[] floatTOdouble(float[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of int as double
        public static double[] intTOdouble(int[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of int as float
        public static float[] intTOfloat(int[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of int as long
        public static long[] intTOlong(int[] aa)
        {
            int n = aa.Length;
            long[] bb = new long[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of long as double
        // BEWARE POSSIBLE LOSS OF PRECISION
        public static double[] longTOdouble(long[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of long as float
        // BEWARE POSSIBLE LOSS OF PRECISION
        public static float[] longTOfloat(long[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of short as double
        public static double[] shortTOdouble(short[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of short as float
        public static float[] shortTOfloat(short[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of short as long
        public static long[] shortTOlong(short[] aa)
        {
            int n = aa.Length;
            long[] bb = new long[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of short as int
        public static int[] shortTOint(short[] aa)
        {
            int n = aa.Length;
            int[] bb = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of byte as double
        public static double[] byteTOdouble(byte[] aa)
        {
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of byte as float
        public static float[] byteTOfloat(byte[] aa)
        {
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of byte as long
        public static long[] byteTOlong(byte[] aa)
        {
            int n = aa.Length;
            long[] bb = new long[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of byte as int
        public static int[] byteTOint(byte[] aa)
        {
            int n = aa.Length;
            int[] bb = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of byte as short
        public static short[] byteTOshort(byte[] aa)
        {
            int n = aa.Length;
            short[] bb = new short[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }
            return bb;
        }

        // recast an array of double as int
        // BEWARE OF LOSS OF PRECISION
        public static int[] doubleTOint(double[] aa)
        {
            int n = aa.Length;
            int[] bb = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (int) aa[i];
            }
            return bb;
        }

        // PRINT ARRAY TO SCREEN (deprecated - see PrintToScreen class)
        // print an array of doubles to screen
        // No line returns except at the end
        public static void print(double[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of doubles to screen
        // with line returns
        public static void println(double[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of floats to screen
        // No line returns except at the end
        public static void print(float[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of floats to screen
        // with line returns
        public static void println(float[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of ints to screen
        // No line returns except at the end
        public static void print(int[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of ints to screen
        // with line returns
        public static void println(int[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of longs to screen
        // No line returns except at the end
        public static void print(long[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of longs to screen
        // with line returns
        public static void println(long[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of char to screen
        // No line returns except at the end
        public static void print(char[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of char to screen
        // with line returns
        public static void println(char[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of string to screen
        // No line returns except at the end
        public static void print(string[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of Strings to screen
        // with line returns
        public static void println(string[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of shorts to screen
        // No line returns except at the end
        public static void print(short[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of shorts to screen
        // with line returns
        public static void println(short[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }

        // print an array of bytes to screen
        // No line returns except at the end
        public static void print(byte[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                Console.Write(aa[i] + "   ");
            }
            PrintToScreen.WriteLine();
        }

        // print an array of bytes to screen
        // with line returns
        public static void println(byte[] aa)
        {
            for (int i = 0; i < aa.Length; i++)
            {
                PrintToScreen.WriteLine(aa[i] + "   ");
            }
        }


        // print a 2D array of doubles to screen
        public static void print(double[,] aa)
        {
            for (int i = 0; i < aa.GetLength(0); i++)
            {
                Fmath.print(
                    ArrayHelper.GetRowCopy<double>(
                        aa,
                        i));
            }
        }

        // SORT ELEMENTS OF ARRAY  (deprecated - see ArryMaths class)
        // sort elements in an array of doubles into ascending order
        // using selection sort method
        // returns List containing the original array, the sorted array
        //  and an array of the indices of the sorted array
        public static List<Object> selectSortVector(double[] aa)
        {
            List<Object> list = selectSortArrayList(aa);
            List<Object> ret = null;
            if (list != null)
            {
                int n = list.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(list[i]);
                }
            }
            return ret;
        }


        // sort elements in an array of doubles into ascending order
        // using selection sort method
        // returns List containing the original array, the sorted array
        //  and an array of the indices of the sorted array
        public static List<Object> selectSortArrayList(double[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            double holdb = 0.0D;
            int holdi = 0;
            double[] bb = new double[n];
            int[] indices = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
                indices[i] = i;
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdb = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = holdb;
                holdi = indices[index];
                indices[index] = indices[lastIndex];
                indices[lastIndex] = holdi;
            }
            List<Object> arrayl = new List<Object>();
            arrayl.Add(aa);
            arrayl.Add(bb);
            arrayl.Add(indices);
            return arrayl;
        }

        // sort elements in an array of doubles into ascending order
        // using selection sort method
        public static double[] selectionSort(double[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            double hold = 0.0D;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                hold = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = hold;
            }
            return bb;
        }

        // sort elements in an array of floats into ascending order
        // using selection sort method
        public static float[] selectionSort(float[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            float hold = 0.0F;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                hold = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = hold;
            }
            return bb;
        }

        // sort elements in an array of ints into ascending order
        // using selection sort method
        public static int[] selectionSort(int[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int hold = 0;
            int[] bb = new int[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                hold = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = hold;
            }
            return bb;
        }

        // sort elements in an array of longs into ascending order
        // using selection sort method
        public static long[] selectionSort(long[] aa)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            long hold = 0L;
            long[] bb = new long[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                hold = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = hold;
            }
            return bb;
        }

        // sort elements in an array of doubles into ascending order
        // using selection sort method
        // aa - the original array - not altered
        // bb - the sorted array
        // indices - an array of the original indices of the sorted array
        public static void selectionSort(double[] aa, double[] bb, int[] indices)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            double holdb = 0.0D;
            int holdi = 0;
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
                indices[i] = i;
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdb = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = holdb;
                holdi = indices[index];
                indices[index] = indices[lastIndex];
                indices[lastIndex] = holdi;
            }
        }

        // sort the elements of an array of doubles into ascending order with matching switches in an array of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(double[] aa, double[] bb, double[] cc, double[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            double holdx = 0.0D;
            double holdy = 0.0D;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of floats into ascending order with matching switches in an array of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(float[] aa, float[] bb, float[] cc, float[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            float holdx = 0.0F;
            float holdy = 0.0F;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an longs of doubles into ascending order with matching switches in an array of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(long[] aa, long[] bb, long[] cc, long[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            long holdx = 0L;
            long holdy = 0L;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of ints into ascending order with matching switches in an array of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(int[] aa, int[] bb, int[] cc, int[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            int holdx = 0;
            int holdy = 0;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of doubles into ascending order with matching switches in an array of long of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(double[] aa, long[] bb, double[] cc, long[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            double holdx = 0.0D;
            long holdy = 0L;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of long into ascending order with matching switches in an array of double of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(long[] aa, double[] bb, long[] cc, double[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            long holdx = 0L;
            double holdy = 0.0D;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of doubles into ascending order with matching switches in an array of int of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(double[] aa, int[] bb, double[] cc, int[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            double holdx = 0.0D;
            int holdy = 0;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of int into ascending order with matching switches in an array of double of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(int[] aa, double[] bb, int[] cc, double[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            int holdx = 0;
            double holdy = 0.0D;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of long into ascending order with matching switches in an array of int of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(long[] aa, int[] bb, long[] cc, int[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            long holdx = 0L;
            int holdy = 0;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }

        // sort the elements of an array of int into ascending order with matching switches in an array of long of the Length
        // using selection sort method
        // array determining the order is the first argument
        // matching array  is the second argument
        // sorted arrays returned as third and fourth arguments respectively
        public static void selectionSort(int[] aa, long[] bb, int[] cc, long[] dd)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (n != m)
            {
                throw new ArgumentException("First argument array, aa, (Length = " + n +
                                            ") and the second argument array, bb, (Length = " + m +
                                            ") should be the same Length");
            }
            int nn = cc.Length;
            if (nn < n)
            {
                throw new ArgumentException("The third argument array, cc, (Length = " + nn +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int mm = dd.Length;
            if (mm < m)
            {
                throw new ArgumentException("The fourth argument array, dd, (Length = " + mm +
                                            ") should be at least as long as the second argument array, bb, (Length = " +
                                            m + ")");
            }

            int holdx = 0;
            long holdy = 0L;


            for (int i = 0; i < n; i++)
            {
                cc[i] = aa[i];
                dd[i] = bb[i];
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (cc[i] < cc[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = cc[index];
                cc[index] = cc[lastIndex];
                cc[lastIndex] = holdx;
                holdy = dd[index];
                dd[index] = dd[lastIndex];
                dd[lastIndex] = holdy;
            }
        }


        // sort elements in an array of doubles (first argument) into ascending order
        // using selection sort method
        // returns the sorted array as second argument
        //  and an array of the indices of the sorted array as the third argument
        // same as corresponding selectionSort - retained for backward compatibility
        public static void selectSort(double[] aa, double[] bb, int[] indices)
        {
            int index = 0;
            int lastIndex = -1;
            int n = aa.Length;
            int m = bb.Length;
            if (m < n)
            {
                throw new ArgumentException("The second argument array, bb, (Length = " + m +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }
            int k = indices.Length;
            if (m < n)
            {
                throw new ArgumentException("The third argument array, indices, (Length = " + k +
                                            ") should be at least as long as the first argument array, aa, (Length = " +
                                            n + ")");
            }

            double holdb = 0.0D;
            int holdi = 0;
            for (int i = 0; i < n; i++)
            {
                bb[i] = aa[i];
                indices[i] = i;
            }

            while (lastIndex != n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (bb[i] < bb[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdb = bb[index];
                bb[index] = bb[lastIndex];
                bb[lastIndex] = holdb;
                holdi = indices[index];
                indices[index] = indices[lastIndex];
                indices[lastIndex] = holdi;
            }
        }


        // UNIT CONVERSIONS (deprecated - see Conv class)

        // Converts radians to degrees
        public static double radToDeg(double rad)
        {
            return rad*180.0D/Math.PI;
        }

        // Converts degrees to radians
        public static double degToRad(double deg)
        {
            return deg*Math.PI/180.0D;
        }

        // Converts frequency (Hz) to radial frequency
        public static double frequencyToRadialFrequency(double frequency)
        {
            return 2.0D*Math.PI*frequency;
        }

        // Converts radial frequency to frequency (Hz)
        public static double radialFrequencyToFrequency(double radial)
        {
            return radial/(2.0D*Math.PI);
        }

        // Converts electron volts(eV) to corresponding wavelength in nm
        public static double evToNm(double ev)
        {
            return 1e+9*C_LIGHT/(-ev*Q_ELECTRON/H_PLANCK);
        }

        // Converts wavelength in nm to matching energy in eV
        public static double nmToEv(double nm)
        {
            return C_LIGHT/(-nm*1e-9)*H_PLANCK/Q_ELECTRON;
        }

        // Converts moles per litre to percentage weight by volume
        public static double molarToPercentWeightByVol(double molar, double molWeight)
        {
            return molar*molWeight/10.0D;
        }

        // Converts percentage weight by volume to moles per litre
        public static double percentWeightByVolToMolar(double perCent, double molWeight)
        {
            return perCent*10.0D/molWeight;
        }

        // Converts Celsius to Kelvin
        public static double celsiusToKelvin(double cels)
        {
            return cels - T_ABS;
        }

        // Converts Kelvin to Celsius
        public static double kelvinToCelsius(double kelv)
        {
            return kelv + T_ABS;
        }

        // Converts Celsius to Fahrenheit
        public static double celsiusToFahren(double cels)
        {
            return cels*(9.0/5.0) + 32.0;
        }

        // Converts Fahrenheit to Celsius
        public static double fahrenToCelsius(double fahr)
        {
            return (fahr - 32.0)*5.0/9.0;
        }

        // Converts calories to Joules
        public static double calorieToJoule(double cal)
        {
            return cal*4.1868;
        }

        // Converts Joules to calories
        public static double jouleToCalorie(double joule)
        {
            return joule*0.23884;
        }

        // Converts grams to ounces
        public static double gramToOunce(double gm)
        {
            return gm/28.3459;
        }

        // Converts ounces to grams
        public static double ounceToGram(double oz)
        {
            return oz*28.3459;
        }

        // Converts kilograms to pounds
        public static double kgToPound(double kg)
        {
            return kg/0.4536;
        }

        // Converts pounds to kilograms
        public static double poundToKg(double pds)
        {
            return pds*0.4536;
        }

        // Converts kilograms to tons
        public static double kgToTon(double kg)
        {
            return kg/1016.05;
        }

        // Converts tons to kilograms
        public static double tonToKg(double tons)
        {
            return tons*1016.05;
        }

        // Converts millimetres to inches
        public static double millimetreToInch(double mm)
        {
            return mm/25.4;
        }

        // Converts inches to millimetres
        public static double inchToMillimetre(double in_)
        {
            return in_*25.4;
        }

        // Converts feet to metres
        public static double footToMetre(double ft)
        {
            return ft*0.3048;
        }

        // Converts metres to feet
        public static double metreToFoot(double metre)
        {
            return metre/0.3048;
        }

        // Converts yards to metres
        public static double yardToMetre(double yd)
        {
            return yd*0.9144;
        }

        // Converts metres to yards
        public static double metreToYard(double metre)
        {
            return metre/0.9144;
        }

        // Converts miles to kilometres
        public static double mileToKm(double mile)
        {
            return mile*1.6093;
        }

        // Converts kilometres to miles
        public static double kmToMile(double km)
        {
            return km/1.6093;
        }

        // Converts UK gallons to litres
        public static double gallonToLitre(double gall)
        {
            return gall*4.546;
        }

        // Converts litres to UK gallons
        public static double litreToGallon(double litre)
        {
            return litre/4.546;
        }

        // Converts UK quarts to litres
        public static double quartToLitre(double quart)
        {
            return quart*1.137;
        }

        // Converts litres to UK quarts
        public static double litreToQuart(double litre)
        {
            return litre/1.137;
        }

        // Converts UK pints to litres
        public static double pintToLitre(double pint)
        {
            return pint*0.568;
        }

        // Converts litres to UK pints
        public static double litreToPint(double litre)
        {
            return litre/0.568;
        }

        // Converts UK gallons per mile to litres per kilometre
        public static double gallonPerMileToLitrePerKm(double gallPmile)
        {
            return gallPmile*2.825;
        }

        // Converts litres per kilometre to UK gallons per mile
        public static double litrePerKmToGallonPerMile(double litrePkm)
        {
            return litrePkm/2.825;
        }

        // Converts miles per UK gallons to kilometres per litre
        public static double milePerGallonToKmPerLitre(double milePgall)
        {
            return milePgall*0.354;
        }

        // Converts kilometres per litre to miles per UK gallons
        public static double kmPerLitreToMilePerGallon(double kmPlitre)
        {
            return kmPlitre/0.354;
        }

        // Converts UK fluid ounce to American fluid ounce
        public static double fluidOunceUKtoUS(double flOzUK)
        {
            return flOzUK*0.961;
        }

        // Converts American fluid ounce to UK fluid ounce
        public static double fluidOunceUStoUK(double flOzUS)
        {
            return flOzUS*1.041;
        }

        // Converts UK pint to American liquid pint
        public static double pintUKtoUS(double pintUK)
        {
            return pintUK*1.201;
        }

        // Converts American liquid pint to UK pint
        public static double pintUStoUK(double pintUS)
        {
            return pintUS*0.833;
        }

        // Converts UK quart to American liquid quart
        public static double quartUKtoUS(double quartUK)
        {
            return quartUK*1.201;
        }

        // Converts American liquid quart to UK quart
        public static double quartUStoUK(double quartUS)
        {
            return quartUS*0.833;
        }

        // Converts UK gallon to American gallon
        public static double gallonUKtoUS(double gallonUK)
        {
            return gallonUK*1.201;
        }

        // Converts American gallon to UK gallon
        public static double gallonUStoUK(double gallonUS)
        {
            return gallonUS*0.833;
        }

        // Converts UK pint to American cup
        public static double pintUKtoCupUS(double pintUK)
        {
            return pintUK/0.417;
        }

        // Converts American cup to UK pint
        public static double cupUStoPintUK(double cupUS)
        {
            return cupUS*0.417;
        }

        // Calculates body mass index (BMI) from height (m) and weight (kg)
        public static double calcBMImetric(double height, double weight)
        {
            return weight/(height*height);
        }

        // Calculates body mass index (BMI) from height (ft) and weight (lbs)
        public static double calcBMIimperial(double height, double weight)
        {
            height = footToMetre(height);
            weight = poundToKg(weight);
            return weight/(height*height);
        }

        // Calculates weight (kg) to give a specified BMI for a given height (m)
        public static double calcWeightFromBMImetric(double bmi, double height)
        {
            return bmi*height*height;
        }

        // Calculates weight (lbs) to give a specified BMI for a given height (ft)
        public static double calcWeightFromBMIimperial(double bmi, double height)
        {
            height = footToMetre(height);
            double weight = bmi*height*height;
            weight = kgToPound(weight);
            return weight;
        }
    }
}
