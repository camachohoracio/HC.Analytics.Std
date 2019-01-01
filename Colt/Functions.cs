#region

using System;
using HC.Analytics.Colt.CustomImplementations.tmp;
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
    ////package cern.jet.math;

    ////import DoubleDoubleFunction;
    ////import DoubleDoubleProcedure;
    ////import DoubleFunction;
    ////import DoubleProcedure;

    ////import com.imsl.math.Sfun;
    /** 
    Function objects to be passed to generic methods. Contains the functions of 
      {@link java.lang.Math} as function objects, as 
      well as a few more basic functions.
    <p>Function objects conveniently allow to express arbitrary functions in a generic 
      manner. Essentially, a function object is an object that can perform a function 
      on some arguments. It has a minimal interface: a method <tt>apply</tt> that 
      takes the arguments, computes something and returns some result value. Function 
      objects are comparable to function pointers in C used for call-backs.
    <p>Unary functions are of type {@link DoubleFunction}, binary functions 
      of type {@link DoubleDoubleFunction}. All can be retrieved via <tt>public 
      static </tt> variables named after the function. 
    Unary predicates are of type {@link DoubleProcedure}, binary predicates 
      of type {@link DoubleDoubleProcedure}. All can be retrieved via <tt>public 
      static </tt> variables named <tt>isXXX</tt>. 

    <p> Binary functions and predicates also exist as unary functions with the second argument being 
      fixed to a constant. These are generated and retrieved via factory methods (again 
      with the same name as the function). Example: 
    <ul>
      <li><tt>Functions.pow</tt> gives the function <tt>a<sup>b</sup></tt>.
      <li><tt>Functions.pow.Apply(2,3)==8</tt>.
      <li><tt>Functions.pow(3)</tt> gives the function <tt>a<sup>3</sup></tt>.
      <li><tt>Functions.pow(3).Apply(2)==8</tt>.
    </ul>
    More general, any binary function can be made an unary functions by fixing either 
    the first or the second argument. See methods {@link #bindArg1(DoubleDoubleFunction,double)} 
    and {@link #bindArg2(DoubleDoubleFunction,double)}. The order of arguments 
    can be swapped so that the first argument becomes the second and vice-versa. See 
    method {@link #swapArgs(DoubleDoubleFunction)}. Example: 
    <ul>
    <li><tt>Functions.pow</tt> gives the function <tt>a<sup>b</sup></tt>.
    <li><tt>Functions.bindArg2(Functions.pow,3)</tt> gives the function <tt>x<sup>3</sup></tt>.
    <li><tt>Functions.bindArg1(Functions.pow,3)</tt> gives the function <tt>3<sup>x</sup></tt>.
    <li><tt>Functions.swapArgs(Functions.pow)</tt> gives the function <tt>b<sup>a</sup></tt>.
    </ul>
    <p>
    Even more general, functions can be chained (composed, assembled). Assume we have two unary 
      functions <tt>g</tt> and <tt>h</tt>. The unary function <tt>g(h(a))</tt> applying 
      both in sequence can be generated via {@link #chain(DoubleFunction,DoubleFunction)}:
    <ul>
    <li><tt>Functions.chain(g,h);</tt>
    </ul> 
      Assume further we have a binary function <tt>f</tt>. The binary function <tt>g(f(a,b))</tt> 
      can be generated via {@link #chain(DoubleFunction,DoubleDoubleFunction)}:
    <ul>
    <li><tt>Functions.chain(g,f);</tt>
    </ul>
      The binary function <tt>f(g(a),h(b))</tt> 
      can be generated via {@link #chain(DoubleDoubleFunction,DoubleFunction,DoubleFunction)}:
    <ul>
    <li><tt>Functions.chain(f,g,h);</tt>
    </ul>
    Arbitrarily complex functions can be composed from these building blocks. For example
    <tt>sin(a) + cos<sup>2</sup>(b)</tt> can be specified as follows:
    <ul>
    <li><tt>chain(plus,sin,chain(square,cos));</tt>
    </ul> 
    or, of course, as 
    <pre>
    new DoubleDoubleFunction() {
    &nbsp;&nbsp;&nbsp;public  double Apply(double a, double b) { return Math.Sin(a) + Math.Pow(Math.Cos(b),2); }
    }
    </pre>
    <p>
    For aliasing see {@link #functions}.
    Try this
    <table>
    <td class="PRE"> 
    <pre>
    // should yield 1.4399560356056456 in all cases
    double a = 0.5; 
    double b = 0.2;
    double v = Math.Sin(a) + Math.Pow(Math.Cos(b),2);
    PrintToScreen.WriteLine(v);
    Functions F = Functions.functions;
    DoubleDoubleFunction f = Functions.chain(Functions.plus,Functions.sin,Functions.chain(Functions.square,Functions.cos));
    PrintToScreen.WriteLine(f.Apply(a,b));
    DoubleDoubleFunction g = new DoubleDoubleFunction() {
    &nbsp;&nbsp;&nbsp;public double Apply(double a, double b) { return Math.Sin(a) + Math.Pow(Math.Cos(b),2); }
    };
    PrintToScreen.WriteLine(g.Apply(a,b));
    </pre>
    </td>
    </table>

    <p>
    <H3>Performance</H3>

    Surprise. Using modern non-adaptive JITs such as SunJDK 1.2.2 (java -classic) 
      there seems to be no or only moderate performance penalty in using function 
      objects in a loop over traditional code in a loop. For complex nested function 
      objects (e.g. <tt>Functions.chain(Functions.abs,Functions.chain(Functions.plus,Functions.sin,Functions.chain(Functions.square,Functions.cos)))</tt>) 
      the penalty is zero, for trivial functions (e.g. <tt>Functions.plus</tt>) the penalty 
      is often acceptable.
    <center>
      <table border cellpadding="3" cellspacing="0" align="center">
        <tr valign="middle" bgcolor="#33CC66" nowrap align="center"> 
          <td nowrap colspan="7"> <font size="+2">Iteration Performance [million function 
            evaluations per second]</font><br>
            <font size="-1">Pentium Pro 200 Mhz, SunJDK 1.2.2, NT, java -classic, 
            </font></td>
        </tr>
        <tr valign="middle" bgcolor="#66CCFF" nowrap align="center"> 
          <td nowrap bgcolor="#FF9966" rowspan="2">&nbsp;</td>
          <td bgcolor="#FF9966" colspan="2"> 
            <p> 30000000 iterations</p>
          </td>
          <td bgcolor="#FF9966" colspan="2"> 3000000 iterations (10 times less)</td>
          <td bgcolor="#FF9966" colspan="2">&nbsp;</td>
        </tr>
        <tr valign="middle" bgcolor="#66CCFF" nowrap align="center"> 
          <td bgcolor="#FF9966"> <tt>Functions.plus</tt></td>
          <td bgcolor="#FF9966"><tt>a+b</tt></td>
          <td bgcolor="#FF9966"> <tt>Functions.chain(Functions.abs,Functions.chain(Functions.plus,Functions.sin,Functions.chain(Functions.square,Functions.cos)))</tt></td>
          <td bgcolor="#FF9966"> <tt>Math.Abs(Math.Sin(a) + Math.Pow(Math.Cos(b),2))</tt></td>
          <td bgcolor="#FF9966">&nbsp;</td>
          <td bgcolor="#FF9966">&nbsp;</td>
        </tr>
        <tr valign="middle" bgcolor="#66CCFF" nowrap align="center"> 
          <td nowrap bgcolor="#FF9966">&nbsp;</td>
          <td nowrap>10.8</td>
          <td nowrap>29.6</td>
          <td nowrap>0.43</td>
          <td nowrap>0.35</td>
          <td nowrap>&nbsp;</td>
          <td nowrap>&nbsp;</td>
        </tr>
      </table></center>


    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class Functions : Object
    {
        /**
        Little trick to allow for "aliasing", that is, renaming this class.
        Writing code like
        <p>
        <tt>Functions.chain(Functions.plus,Functions.sin,Functions.chain(Functions.square,Functions.cos));</tt>
        <p>
        is a bit awkward, to say the least.
        Using the aliasing you can instead write
        <p>
        <tt>Functions F = Functions.functions; <br>
        Functions.chain(Functions.plus,Functions.sin,Functions.chain(Functions.square,Functions.cos));</tt>
        */

        /*****************************
         * <H3>Unary functions</H3>
         *****************************/
        /**
         * Function that returns <tt>Math.Abs(a)</tt>.
         */
        public static DoubleDoubleProcedure isGreater_ = new DoubleDoubleProcedureIsGreater();
        public static DoubleFunction m_absm = new DoubleFunctionAbs();

        /**
         * Function that returns <tt>Math.acos(a)</tt>.
         */
        public static DoubleFunction m_acos = new DoubleFunctionACos();

        /**
         * Function that returns <tt>com.imsl.math.Sfun.acosh(a)</tt>.
         */
        /*
        public static  DoubleFunction acosh = new DoubleFunction() {
            public  double Apply(double a) { return Sfun.acosh(a); }
        };
        */

        /**
         * Function that returns <tt>Math.asin(a)</tt>.
         */
        public static DoubleFunction m_asin = new DoubleFunctionASin();

        /**
         * Function that returns <tt>com.imsl.math.Sfun.asinh(a)</tt>.
         */
        /*
        public static  DoubleFunction asinh = new DoubleFunction() {
            public  double Apply(double a) { return Sfun.asinh(a); }
        };
        */

        /**
         * Function that returns <tt>Math.atan(a)</tt>.
         */
        public static DoubleFunction m_atan = new DoubleFunctionATan();

        /**
         * Function that returns <tt>com.imsl.math.Sfun.atanh(a)</tt>.
         */
        /*
        public static  DoubleFunction atanh = new DoubleFunction() {
            public  double Apply(double a) { return Sfun.atanh(a); }
        };
        */

        /**
         * Function that returns <tt>Math.Ceiling(a)</tt>.
         */

        /**
         * Function that returns <tt>com.imsl.math.Sfun.tanh(a)</tt>.
         */
        /*
        public static  DoubleFunction tanh = new DoubleFunction() {
            public  double Apply(double a) { return Sfun.tanh(a); }
        };
        */

        /**
         * Function that returns <tt>Math.toDegrees(a)</tt>.
         */
        /*
        public static  DoubleFunction toDegrees = new DoubleFunction() {
            public  double Apply(double a) { return Math.toDegrees(a); }
        };
        */

        /**
         * Function that returns <tt>Math.toRadians(a)</tt>.
         */
        /*
        public static  DoubleFunction toRadians = new DoubleFunction() {
            public  double Apply(double a) { return Math.toRadians(a); }
        };		
        */


        /*****************************
         * <H3>Binary functions</H3>
         *****************************/

        /**
         * Function that returns <tt>Math.atan2(a,b)</tt>.
         */
        public static DoubleDoubleFunction m_atan2 = new DoubleDoubleFunctionATan2();
        public static DoubleFunction m_ceil = new DoubleFunctionCeil();

        /**
         * Function that returns <tt>com.imsl.math.Sfun.logBeta(a,b)</tt>.
         */
        /*
        public static  DoubleDoubleFunction logBeta = new DoubleDoubleFunction() {
            public  double Apply(double a, double b) { return Sfun.logBeta(a,b); }
        };
        */


        /**
         * Function that returns <tt>a < b ? -1 : a > b ? 1 : 0</tt>.
         */
        public static DoubleDoubleFunction m_compare = new DoubleDoubleFunctionCompare();
        public static DoubleFunction m_cos = new DoubleFunctionCos();

        /**
         * Function that returns <tt>a / b</tt>.
         */
        public static DoubleDoubleFunction m_div = new DoubleDoubleFunctionDiv();

        /**
         * Function that returns <tt>a == b ? 1 : 0</tt>.
         */
        public static DoubleDoubleFunction m_equals = new DoubleDoubleFunctionEquals();
        public static DoubleFunction m_exp = new DoubleFunctionExp();

        /**
         * Function that returns <tt>Math.Floor(a)</tt>.
         */
        public static DoubleFunction m_floor = new DoubleFunctionFloor();
        public static Functions m_functions = new Functions();

        /**
         * Function that returns <tt>a > b ? 1 : 0</tt>.
         */
        public static DoubleDoubleFunction m_greater = new DoubleDoubleFunctionGreater();
        public static DoubleFunction m_identity = new DoubleFunctionIdentity();

        /**
         * Function that returns <tt>Math.IEEEremainder(a,b)</tt>.
         */
        public static DoubleDoubleFunction m_IeeeRemainder = new DoubleDoubleFunctionIeeeRemainder();
        public static DoubleFunction m_inv = new DoubleFunctionInverse();

        /**
         * Function that returns <tt>a == b</tt>.
         */
        public static DoubleDoubleProcedure m_isEqual = new DoubleDoubleProcedureIsEqual();

        /**
         * Function that returns <tt>a < b</tt>.
         */
        public static DoubleDoubleProcedure m_isLess = new DoubleDoubleProcedureIsLess();

        /**
         * Function that returns <tt>a > b</tt>.
         */

        /**
         * Function that returns <tt>a < b ? 1 : 0</tt>.
         */
        public static DoubleDoubleFunction m_less = new DoubleDoubleFunctionLess();

        /**
         * Function that returns <tt>Math.Log(a) / Math.Log(b)</tt>.
         */
        public static DoubleDoubleFunction m_lg = new DoubleDoubleFunctionLogAB();
        public static DoubleFunction m_log = new DoubleFunctionLog();

        /**
         * Function that returns <tt>com.imsl.math.Sfun.log10(a)</tt>.
         */
        /*
        public static  DoubleFunction log10 = new DoubleFunction() {
            public  double Apply(double a) { return Sfun.log10(a); }
        };
        */

        /**
         * Function that returns <tt>Math.Log(a) / Math.Log(2)</tt>.
         */
        public static DoubleFunction m_log2 = new DoubleFunctionLogA();

        /**
         * Function that returns <tt>Math.Max(a,b)</tt>.
         */
        public static DoubleDoubleFunction m_max = new DoubleDoubleFunctionMax();

        /**
         * Function that returns <tt>Math.Min(a,b)</tt>.
         */
        public static DoubleDoubleFunction m_min = new DoubleDoubleFunctionMin();

        /**
         * Function that returns <tt>a - b</tt>.
         */
        public static DoubleDoubleFunction m_minus = new DoubleDoubleFunctionMinus();
        /*
        new DoubleDoubleFunction() {
            public  double Apply(double a, double b) { return a - b; }
        };
        */

        /**
         * Function that returns <tt>a % b</tt>.
         */
        public static DoubleDoubleFunction m_mod = new DoubleDoubleFunctionMod();

        /**
         * Function that returns <tt>a * b</tt>.
         */
        public static DoubleDoubleFunction m_mult = new DoubleDoubleFunctionMult();
        public static DoubleFunction m_neg = new DoubleFunctionNeg();

        /**
         * Function that returns <tt>a + b</tt>.
         */
        public static DoubleDoubleFunction m_plus = plusMult(1);
        /*
        new DoubleDoubleFunction() {
            public  double Apply(double a, double b) { return a + b; }
        };
        */

        /**
         * Function that returns <tt>Math.Abs(a) + Math.Abs(b)</tt>.
         */
        public static DoubleDoubleFunction m_plusAbs = new DoubleDoubleFunctionPlusAbs();

        /**
         * Function that returns <tt>Math.Pow(a,b)</tt>.
         */
        public static DoubleDoubleFunction m_pow = new DoubleDoubleFunctionPow();
        public static DoubleFunction m_rint = new DoubleFunctionRint();

        /**
         * Function that returns <tt>a < 0 ? -1 : a > 0 ? 1 : 0</tt>.
         */
        public static DoubleFunction m_sign = new DoubleFunctionSign();

        /**
         * Function that returns <tt>Math.Sin(a)</tt>.
         */
        public static DoubleFunction m_sin = new DoubleFunctionSin();

        /**
         * Function that returns <tt>com.imsl.math.Sfun.sinh(a)</tt>.
         */
        /*
        public static  DoubleFunction sinh = new DoubleFunction() {
            public  double Apply(double a) { return Sfun.sinh(a); }
        };
        */

        /**
         * Function that returns <tt>Math.Sqrt(a)</tt>.
         */
        public static DoubleFunction m_sqrt = new DoubleFunctionSqrt();

        /**
         * Function that returns <tt>a * a</tt>.
         */
        public static DoubleFunction m_square = new DoubleFunctionSquare();

        /**
         * Function that returns <tt>Math.tan(a)</tt>.
         */
        public static DoubleFunction m_tan = new DoubleFunctionTan();
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Constructs a function that returns <tt>(from<=a && a<=to) ? 1 : 0</tt>.
         * <tt>a</tt> is a variable, <tt>from</tt> and <tt>to</tt> are fixed.
         */

        public static DoubleFunction between(double from, double to)
        {
            return new DoubleFunctionBetween(from, to);
        }

        /**
         * Constructs a unary function from a binary function with the first operand (argument) fixed to the given constant <tt>c</tt>.
         * The second operand is variable (free).
         * 
         * @param function a binary function taking operands in the form <tt>function.Apply(c,var)</tt>.
         * @return the unary function <tt>function(c,var)</tt>.
         */

        public static DoubleFunction bindArg1(DoubleDoubleFunction function, double c)
        {
            return new DoubleFunctionBindArg1(function, c);
        }

        /**
         * Constructs a unary function from a binary function with the second operand (argument) fixed to the given constant <tt>c</tt>.
         * The first operand is variable (free).
         * 
         * @param function a binary function taking operands in the form <tt>function.Apply(var,c)</tt>.
         * @return the unary function <tt>function(var,c)</tt>.
         */

        public static DoubleFunction bindArg2(DoubleDoubleFunction function, double c)
        {
            return new DoubleFunctionBindArg2(function, c);
        }

        /**
         * Constructs the function <tt>f( g(a), h(b) )</tt>.
         * 
         * @param f a binary function.
         * @param g a unary function.
         * @param h a unary function.
         * @return the binary function <tt>f( g(a), h(b) )</tt>.
         */

        public static DoubleDoubleFunction chain(
            DoubleDoubleFunction f,
            DoubleFunction g,
            DoubleFunction h)
        {
            return new DoubleDoubleFunctionChain(
                f,
                g,
                h);
        }

        /**
         * Constructs the function <tt>g( h(a,b) )</tt>.
         * 
         * @param g a unary function.
         * @param h a binary function.
         * @return the unary function <tt>g( h(a,b) )</tt>.
         */

        public static DoubleDoubleFunction chain(DoubleFunction g, DoubleDoubleFunction h)
        {
            return new DoubleDoubleFunctionChain2(g, h);
        }

        /**
         * Constructs the function <tt>g( h(a) )</tt>.
         * 
         * @param g a unary function.
         * @param h a unary function.
         * @return the unary function <tt>g( h(a) )</tt>.
         */

        public static DoubleFunction chain(DoubleFunction g, DoubleFunction h)
        {
            return new DoubleFunctionChain3(g, h);
        }

        /**
         * Constructs a function that returns <tt>a < b ? -1 : a > b ? 1 : 0</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction Compare(double b)
        {
            return new DoubleFunctionCompare(b);
        }

        /**
         * Constructs a function that returns the constant <tt>c</tt>.
         */

        public static DoubleFunction constant(double c)
        {
            return new DoubleFunctionConstant(c);
        }

        /**
         * Demonstrates usage of this class.
         */

        public static void demo1()
        {
            Functions F = m_functions;
            double a = 0.5;
            double b = 0.2;
            double v = Math.Sin(a) + Math.Pow(Math.Cos(b), 2);

            PrintToScreen.WriteLine(v);
            DoubleDoubleFunction f = chain(m_plus, m_sin, chain(m_square, m_cos));

            //DoubleDoubleFunction f = Functions.chain(plus,sin,Functions.chain(square,cos));
            PrintToScreen.WriteLine(f.Apply(a, b));
            //DoubleDoubleFunction g = new DoubleDoubleFunction() {
            //    public  double Apply(double x, double y) { return Math.Sin(x) + Math.Pow(Math.Cos(y),2); }
            //};
            //PrintToScreen.WriteLine(g.Apply(a,b));
            DoubleFunction m = plus(3);
            DoubleFunction n = plus(4);
            PrintToScreen.WriteLine(m.Apply(0));
            PrintToScreen.WriteLine(n.Apply(0));
        }

        /**
         * Benchmarks and demonstrates usage of trivial and complex functions.
         */

        public static void demo2(int size)
        {
            Functions F = m_functions;
            PrintToScreen.WriteLine("\n\n");
            double a = 0.0;
            double b = 0.0;
            double v = Math.Abs(Math.Sin(a) + Math.Pow(Math.Cos(b), 2));
            //double v = Math.Sin(a) + Math.Pow(Math.Cos(b),2);
            //double v = a + b;
            PrintToScreen.WriteLine(v);

            //DoubleDoubleFunction f = Functions.chain(Functions.plus,Functions.identity,Functions.identity);
            DoubleDoubleFunction f = chain(
                m_absm,
                chain(
                    m_plus,
                    m_sin,
                    chain(
                        m_square,
                        m_cos)));
            //DoubleDoubleFunction f = Functions.chain(Functions.plus,Functions.sin,Functions.chain(Functions.square,Functions.cos));
            //DoubleDoubleFunction f = Functions.plus;

            PrintToScreen.WriteLine(f.Apply(a, b));
            //DoubleDoubleFunction g = new DoubleDoubleFunction() {
            //    public  double Apply(double x, double y) { return Math.Abs(Math.Sin(x) + Math.Pow(Math.Cos(y),2)); }
            //    //public  double Apply(double x, double y) { return x+y; }
            //};
            //PrintToScreen.WriteLine(g.Apply(a,b));

            // emptyLoop
            Timer emptyLoop = new Timer().start();
            a = 0;
            b = 0;
            double sum = 0;
            for (int i = size; --i >= 0;)
            {
                sum += a;
                a++;
                b++;
            }
            emptyLoop.stop().display();
            PrintToScreen.WriteLine("empty sum=" + sum);

            Timer timer = new Timer().start();
            a = 0;
            b = 0;
            sum = 0;
            for (int i = size; --i >= 0;)
            {
                sum += Math.Abs(Math.Sin(a) + Math.Pow(Math.Cos(b), 2));
                //sum += a + b;
                a++;
                b++;
            }
            timer.stop().display();
            PrintToScreen.WriteLine("evals / sec = " + size/timer.minus(emptyLoop).seconds());
            PrintToScreen.WriteLine("sum=" + sum);

            timer.reset().start();
            a = 0;
            b = 0;
            sum = 0;
            for (int i = size; --i >= 0;)
            {
                sum += f.Apply(a, b);
                a++;
                b++;
            }
            timer.stop().display();
            PrintToScreen.WriteLine("evals / sec = " + size/timer.minus(emptyLoop).seconds());
            PrintToScreen.WriteLine("sum=" + sum);

            timer.reset().start();
            a = 0;
            b = 0;
            sum = 0;
            for (int i = size; --i >= 0;)
            {
                //sum += g.Apply(a, b);
                a++;
                b++;
            }
            timer.stop().display();
            PrintToScreen.WriteLine("evals / sec = " + size/timer.minus(emptyLoop).seconds());
            PrintToScreen.WriteLine("sum=" + sum);
        }

        /**
         * Constructs a function that returns <tt>a / b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction div(double b)
        {
            return mult(1/b);
        }

        /**
         * Constructs a function that returns <tt>a == b ? 1 : 0</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction Equals(double b)
        {
            return new DoubleFunctionEquals(b);
        }

        /**
         * Constructs a function that returns <tt>a > b ? 1 : 0</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction greater(double b)
        {
            return new DoubleFunctionGreater(b);
        }

        /**
         * Constructs a function that returns <tt>Math.IEEEremainder(a,b)</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction IEEEremainder(double b)
        {
            return new DoubleFunctionIEeeRemainder(b);
        }

        /**
         * Constructs a function that returns <tt>from<=a && a<=to</tt>.
         * <tt>a</tt> is a variable, <tt>from</tt> and <tt>to</tt> are fixed.
         */

        public static DoubleProcedure isBetween(double from, double to)
        {
            return new DoubleProcedureBetween(from, to);
        }

        /**
         * Constructs a function that returns <tt>a == b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleProcedure isEqual(double b)
        {
            return new DoubleProcedureEquals(b);
        }

        /**
         * Constructs a function that returns <tt>a > b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleProcedure isGreater(double b)
        {
            return new DoubleProcedureGreater(b);
        }

        /**
         * Constructs a function that returns <tt>a < b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleProcedure isLess(double b)
        {
            return new DoubleProcedureIsLess(b);
        }

        /**
         * Constructs a function that returns <tt>a < b ? 1 : 0</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction less(double b)
        {
            return new DoubleFunctionLess(b);
        }

        /**
         * Constructs a function that returns <tt><tt>Math.Log(a) / Math.Log(b)</tt></tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction lg(double b)
        {
            return new DoubleFunctionLogAB(b);
        }

        /**
         * Tests various methods of this class.
         */

        public static void main(string[] args)
        {
            int size = int.Parse(args[0]);
            demo2(size);
            //demo1();
        }

        /**
         * Constructs a function that returns <tt>Math.Max(a,b)</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction max(double b)
        {
            return new DoubleFunctionMax(b);
        }

        /**
         * Constructs a function that returns <tt>Math.Min(a,b)</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction min(double b)
        {
            return new DoubleFunctionMin(b);
        }

        /**
         * Constructs a function that returns <tt>a - b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction minus(double b)
        {
            return plus(-b);
        }

        /**
         * Constructs a function that returns <tt>a - b*constant</tt>.
         * <tt>a</tt> and <tt>b</tt> are variables, <tt>constant</tt> is fixed.
         */

        public static DoubleDoubleFunction minusMult(double constant)
        {
            return plusMult(-constant);
        }

        /**
         * Constructs a function that returns <tt>a % b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction mod(double b)
        {
            return new DoubleFunctionMod(b);
        }

        /**
         * Constructs a function that returns <tt>a * b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction mult(double b)
        {
            return new Mult(b);
            /*
            return new DoubleFunction() {
                public  double Apply(double a) { return a * b; }
            };
            */
        }

        /**
         * Constructs a function that returns <tt>a + b</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction plus(double b)
        {
            return new DoubleFunctionPlus(b);
        }

        /**
         * Constructs a function that returns <tt>a + b*constant</tt>.
         * <tt>a</tt> and <tt>b</tt> are variables, <tt>constant</tt> is fixed.
         */

        public static DoubleDoubleFunction plusMult(double constant)
        {
            return new PlusMult(constant);
            /*
            return new DoubleDoubleFunction() {
                public  double Apply(double a, double b) { return a + b*constant; }
            };
            */
        }

        /**
         * Constructs a function that returns <tt>Math.Pow(a,b)</tt>.
         * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
         */

        public static DoubleFunction pow(double b)
        {
            return new DoubleFunctionPow(b);
        }

        /**
         * Constructs a function that returns a new uniform random number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).
         * Currently the engine is {@link MersenneTwister}
         * and is seeded with the current time.
         * <p>
         * Note that any random engine derived from {@link RngWrapper} and any random distribution derived from {@link AbstractDistribution} are function objects, because they implement the proper interfaces.
         * Thus, if you are not happy with the default, just pass your favourite random generator to function evaluating methods.
         */

        public static DoubleFunction random()
        {
            return new MersenneTwister(new DateTime());
        }

        /**
         * Constructs a function that returns the number rounded to the given precision; <tt>Math.rint(a/precision)*precision</tt>.
         * Examples:
         * <pre>
         * precision = 0.01 rounds 0.012 --> 0.01, 0.018 --> 0.02
         * precision = 10   rounds 123   --> 120 , 127   --> 130
         * </pre>
         */

        public static DoubleFunction round(double precision)
        {
            return new DoubleFunctionRound(precision);
        }

        /**
         * Constructs a function that returns <tt>function.Apply(b,a)</tt>, i.e. applies the function with the first operand as second operand and the second operand as first operand.
         * 
         * @param function a function taking operands in the form <tt>function.Apply(a,b)</tt>.
         * @return the binary function <tt>function(b,a)</tt>.
         */

        public static DoubleDoubleFunction swapArgs(DoubleDoubleFunction function)
        {
            return new DoubleDoubleFunctionSwapArgs(function);
        }
    }
}
