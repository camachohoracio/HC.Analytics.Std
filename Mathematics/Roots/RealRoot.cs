#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics.Complex;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.Roots
{
    /*
    *   Class RealRoot
    *
    *   Contains methods for finding a real root
    *
    *   The function whose root is to be determined is supplied
    *   by means of an interface, RealRootFunction,
    *   if no derivative required
    *
    *   The function whose root is to be determined is supplied
    *   by means of an interface, RealRootDerivFunction,
    *   as is the first derivative if a derivative is required
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:   18 May 2003
    *   UPDATE: May 2003 - March 2008,  23-24 September 2008,  30 January 2010
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web page:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/RealRoot.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    *   Copyright (c) 2003 - 2010    Michael Thomas Flanagan
    *
    * Permission to use, Copy and modify this software and its documentation for NON-COMMERCIAL purposes is granted, without fee,
    * provided that an acknowledgement to the author, Dr Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all copies
    * and associated documentation or publications.
    *
    * Redistributions of the source code of this source code, or parts of the source codes, must retain the above copyright notice, this list of conditions
    * and the following disclaimer and requires written permission from the Michael Thomas Flanagan:
    *
    * Redistribution in binary form of all or parts of this class must reproduce the above copyright notice, this list of conditions and
    * the following disclaimer in the documentation and/or other materials provided with the distribution and requires written permission from the Michael Thomas Flanagan:
    *
    * Dr Michael Thomas Flanagan makes no representations about the suitability or fitness of the software for any or for a particular purpose.
    * Dr Michael Thomas Flanagan shall not be liable for any damages suffered as a result of using, modifying or distributing this software
    * or its derivatives.
    *
    ***************************************************************************************/


    // RealRoot class
    public class RealRoot
    {
        // INSTANCE VARIABLES

        private static int m_maximumStaticBoundsExtension = 100; // number of times that the bounds may be extended
        // by the difference separating them if the root is
        // found not to be bounded (static methods)
        private static bool m_noStaticBoundExtensions;
        // = true if number of no extension to the  bounds allowed (static methods)

        private static bool m_noStaticLowerBoundExtensions;
        // = true if number of no extension to the lower bound allowed (static methods)

        private static bool m_noStaticUpperBoundExtensions;
        // = true if number of no extension to the upper bound allowed (static methods)

        private static double m_realTol = 1e-14; // tolerance as imag/real in deciding whether a root is real

        private static int m_staticIterMax = 3000;
        // maximum number of iterations allowed in root search (static methods)

        private static bool m_staticReturnNaN;
        // if true exceptions resulting from a bound being NaN do not halt the prorgam but return NaN

        private bool m_blnSupressLimitReachedMessage;
        // if true, supresses printing of the iteration limit reached message

        private bool m_blnSupressNaNmessage; // if = true the bound is NaN root returned as NaN message supressed
        private double m_estimate; // estimate for Newton-Raphson method

        private int m_iterMax = 3000; // maximum number of iterations allowed in root search
        private int m_iterN; // number of iterations taken in root search
        private double m_lowerBound; // lower bound for bisection and false position methods
        private int m_maximumBoundsExtension = 100; // number of times that the bounds may be extended
        // by the difference separating them if the root is
        // found not to be bounded
        private bool m_noBoundExtensions; // = true if number of no extension to the  bounds allowed
        private bool m_noLowerBoundExtensions; // = true if number of no extension to the lower bound allowed
        private bool m_noUpperBoundExtensions; // = true if number of no extension to the upper bound allowed

        private bool m_returnNaN;
        // if true exceptions resulting from a bound being NaN do not halt the prorgam but return NaN

        private double m_root = double.NaN; // root to be found
        private double m_tol = 1e-9; // tolerance in determining convergence upon a root
        private double m_upperBound; // upper bound for bisection and false position methods

        // required by PsRandom and Stat classes calling RealRoot

        // CONSTRUCTOR

        // INSTANCE METHODS

        // Set lower bound
        public void setLowerBound(double lower)
        {
            m_lowerBound = lower;
        }

        // Set lower bound
        public void setUpperBound(double upper)
        {
            m_upperBound = upper;
        }

        // Reset exception handling for NaN bound flag to true
        // when flag returnNaN = true exceptions resulting from a bound being NaN do not halt the prorgam but return NaN
        // required by PsRandom and Stat classes calling RealRoot
        public void resetNaNexceptionToTrue()
        {
            m_returnNaN = true;
        }

        // Reset exception handling for NaN bound flag to false
        // when flag returnNaN = false exceptions resulting from a bound being NaN  halts the prorgam
        // required by PsRandom and Stat classes calling RealRoot
        public void resetNaNexceptionToFalse()
        {
            m_returnNaN = false;
        }

        // Supress NaN bound message
        // if supressNaNmessage = true the bound is NaN root returned as NaN message supressed
        public void supressNaNmessage()
        {
            m_blnSupressNaNmessage = true;
        }

        // Allow  NaN bound message
        // if supressNaNmessage = false the bound is NaN root returned as NaN message is written
        public void allowNaNmessage()
        {
            m_blnSupressNaNmessage = false;
        }

        // Set estimate
        public void setEstimate(double estimate)
        {
            m_estimate = estimate;
        }

        // Reset the default tolerance
        public void setTolerance(double tolerance)
        {
            m_tol = tolerance;
        }

        // Get the default tolerance
        public double getTolerance()
        {
            return m_tol;
        }

        // Reset the maximum iterations allowed
        public void setIterMax(int imax)
        {
            m_iterMax = imax;
        }

        // Get the maximum iterations allowed
        public int getIterMax()
        {
            return m_iterMax;
        }

        // Get the number of iterations taken
        public int getIterN()
        {
            return m_iterN;
        }

        // Reset the maximum number of bounds extensions
        public void setmaximumStaticBoundsExtension(int maximumBoundsExtension)
        {
            m_maximumBoundsExtension = maximumBoundsExtension;
        }

        // Prevent extensions to the supplied bounds
        public void noBoundsExtensions()
        {
            m_noBoundExtensions = true;
            m_noLowerBoundExtensions = true;
            m_noUpperBoundExtensions = true;
        }

        // Prevent extension to the lower bound
        public void noLowerBoundExtension()
        {
            m_noLowerBoundExtensions = true;
            if (m_noUpperBoundExtensions)
            {
                m_noBoundExtensions = true;
            }
        }

        // Prevent extension to the upper bound
        public void noUpperBoundExtension()
        {
            m_noUpperBoundExtensions = true;
            if (m_noLowerBoundExtensions)
            {
                m_noBoundExtensions = true;
            }
        }

        // Supresses printing of the iteration limit reached message
        // USE WITH CARE - added only to accomadate a specific application using this class!!!!!
        public void supressLimitReachedMessage()
        {
            m_blnSupressLimitReachedMessage = true;
        }

        // Combined bisection and Inverse Quadratic Interpolation method
        // bounds already entered
        public double brent(RealRootFunction g)
        {
            return brent(g, m_lowerBound, m_upperBound);
        }

        // Combined bisection and Inverse Quadratic Interpolation method
        // bounds supplied as arguments
        public double brent(RealRootFunction g, double lower, double upper)
        {
            m_lowerBound = lower;
            m_upperBound = upper;

            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }

            bool testConv = true; // convergence test: becomes false on convergence
            m_iterN = 0;
            double temp = 0.0D;

            if (upper < lower)
            {
                temp = upper;
                upper = lower;
                lower = temp;
            }

            // calculate the function value at the estimate of the higher bound to x
            double fu = g.function(upper);
            // calculate the function value at the estimate of the lower bound of x
            double fl = g.function(lower);
            if (double.IsNaN(fl))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "Realroot: brent: lower bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "Realroot: brent: upper bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }

            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noBoundExtensions)
                    {
                        string message = "RealRoot.brent: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(message);
                        }
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumBoundsExtension)
                        {
                            string message =
                                "RealRoot.brent: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            if (!m_blnSupressNaNmessage)
                            {
                                PrintToScreen.WriteLine(message);
                            }
                            return double.NaN;
                        }
                        if (!m_noLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            fl = g.function(lower);
                        }
                        if (!m_noUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            fu = g.function(upper);
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                m_root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                m_root = upper;
                testConv = false;
            }

            // Function at mid-point of initial estimates
            double mid = (lower + upper)/2.0D; // mid point (bisect) or new x estimate (Inverse Quadratic Interpolation)
            double lastMidB = mid; // last succesful mid point
            double fm = g.function(mid);
            double diff = mid - lower; // difference between successive estimates of the root
            double fmB = fm; // last succesful mid value function value
            double lastMid = mid;
            bool lastMethod = true;
            // true; last method = Inverse Quadratic Interpolation, false; last method = bisection method
            bool nextMethod = true;
            // true; next method = Inverse Quadratic Interpolation, false; next method = bisection method

            // search
            double rr = 0.0D, ss = 0.0D, tt = 0.0D, pp = 0.0D, qq = 0.0D; // interpolation variables
            while (testConv)
            {
                // test for convergence
                if (fm == 0.0D || Math.Abs(diff) < m_tol)
                {
                    testConv = false;
                    if (fm == 0.0D)
                    {
                        m_root = lastMid;
                    }
                    else
                    {
                        if (Math.Abs(diff) < m_tol)
                        {
                            m_root = mid;
                        }
                    }
                }
                else
                {
                    lastMethod = nextMethod;
                    // test for succesfull inverse quadratic interpolation
                    if (lastMethod)
                    {
                        if (mid < lower || mid > upper)
                        {
                            // inverse quadratic interpolation failed
                            nextMethod = false;
                        }
                        else
                        {
                            fmB = fm;
                            lastMidB = mid;
                        }
                    }
                    else
                    {
                        nextMethod = true;
                    }
                    if (nextMethod)
                    {
                        // inverse quadratic interpolation
                        fl = g.function(lower);
                        fm = g.function(mid);
                        fu = g.function(upper);
                        rr = fm/fu;
                        ss = fm/fl;
                        tt = fl/fu;
                        pp = ss*(tt*(rr - tt)*(upper - mid) - (1.0D - rr)*(mid - lower));
                        qq = (tt - 1.0D)*(rr - 1.0D)*(ss - 1.0D);
                        lastMid = mid;
                        diff = pp/qq;
                        mid = mid + diff;
                    }
                    else
                    {
                        // Bisection procedure
                        fm = fmB;
                        mid = lastMidB;
                        if (fm*fl > 0.0D)
                        {
                            lower = mid;
                            fl = fm;
                        }
                        else
                        {
                            upper = mid;
                            fu = fm;
                        }
                        lastMid = mid;
                        mid = (lower + upper)/2.0D;
                        fm = g.function(mid);
                        diff = mid - lastMid;
                        fmB = fm;
                        lastMidB = mid;
                    }
                }
                m_iterN++;
                if (m_iterN > m_iterMax)
                {
                    if (!m_blnSupressLimitReachedMessage)
                    {
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(
                                "Class: RealRoot; method: brent; maximum number of iterations exceeded - root at this point, " +
                                Fmath.truncate(mid, 4) + ", returned");
                        }
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) +
                                                    ", tolerance = " + m_tol);
                        }
                    }
                    m_root = mid;
                    testConv = false;
                }
            }
            return m_root;
        }

        // bisection method
        // bounds already entered
        public double bisect(RealRootFunction g)
        {
            return bisect(g, m_lowerBound, m_upperBound);
        }

        // bisection method
        public double bisect(RealRootFunction g, double lower, double upper)
        {
            m_lowerBound = lower;
            m_upperBound = upper;

            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }
            if (upper < lower)
            {
                double temp = upper;
                upper = lower;
                lower = temp;
            }

            bool testConv = true; // convergence test: becomes false on convergence
            m_iterN = 0; // number of iterations
            double diff = 1e300; // abs(difference between the last two successive mid-pint x values)

            // calculate the function value at the estimate of the higher bound to x
            double fu = g.function(upper);
            // calculate the function value at the estimate of the lower bound of x
            double fl = g.function(lower);
            if (double.IsNaN(fl))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: bisect: lower bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: bisect: upper bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }
            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noBoundExtensions)
                    {
                        string message = "RealRoot.bisect: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(message);
                        }
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumBoundsExtension)
                        {
                            string message =
                                "RealRoot.bisect: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            if (!m_blnSupressNaNmessage)
                            {
                                PrintToScreen.WriteLine(message);
                            }
                            return double.NaN;
                        }
                        if (!m_noLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            fl = g.function(lower);
                        }
                        if (!m_noUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            fu = g.function(upper);
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                m_root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                m_root = upper;
                testConv = false;
            }

            // start search
            double mid = (lower + upper)/2.0D; // mid-point
            double lastMid = 1e300; // previous mid-point
            double fm = g.function(mid);
            while (testConv)
            {
                if (fm == 0.0D || diff < m_tol)
                {
                    testConv = false;
                    m_root = mid;
                }
                if (fm*fl > 0.0D)
                {
                    lower = mid;
                    fl = fm;
                }
                else
                {
                    upper = mid;
                    fu = fm;
                }
                lastMid = mid;
                mid = (lower + upper)/2.0D;
                fm = g.function(mid);
                diff = Math.Abs(mid - lastMid);
                m_iterN++;
                if (m_iterN > m_iterMax)
                {
                    if (!m_blnSupressLimitReachedMessage)
                    {
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(
                                "Class: RealRoot; method: bisect; maximum number of iterations exceeded - root at this point, " +
                                Fmath.truncate(mid, 4) + ", returned");
                        }
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) +
                                                    ", tolerance = " + m_tol);
                        }
                    }
                    m_root = mid;
                    testConv = false;
                }
            }
            return m_root;
        }

        // false position  method
        // bounds already entered
        public double falsePosition(RealRootFunction g)
        {
            return falsePosition(g, m_lowerBound, m_upperBound);
        }

        // false position  method
        public double falsePosition(RealRootFunction g, double lower, double upper)
        {
            m_lowerBound = lower;
            m_upperBound = upper;

            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }
            if (upper < lower)
            {
                double temp = upper;
                upper = lower;
                lower = temp;
            }

            bool testConv = true; // convergence test: becomes false on convergence
            m_iterN = 0; // number of iterations
            double diff = 1e300; // abs(difference between the last two successive mid-pint x values)

            // calculate the function value at the estimate of the higher bound to x
            double fu = g.function(upper);
            // calculate the function value at the estimate of the lower bound of x
            double fl = g.function(lower);
            if (double.IsNaN(fl))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: fals: ePositionlower bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: falsePosition: upper bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }

            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noBoundExtensions)
                    {
                        string message = "RealRoot.falsePosition: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(message);
                        }
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumBoundsExtension)
                        {
                            string message =
                                "RealRoot.falsePosition: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            if (!m_blnSupressNaNmessage)
                            {
                                PrintToScreen.WriteLine(message);
                            }
                            return double.NaN;
                        }
                        if (!m_noLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            fl = g.function(lower);
                        }
                        if (!m_noUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            fu = g.function(upper);
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                m_root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                m_root = upper;
                testConv = false;
            }

            // start search
            double mid = lower + (upper - lower)*Math.Abs(fl)/(Math.Abs(fl) + Math.Abs(fu)); // mid-point
            double lastMid = 1e300; // previous mid-point
            double fm = g.function(mid);
            while (testConv)
            {
                if (fm == 0.0D || diff < m_tol)
                {
                    testConv = false;
                    m_root = mid;
                }
                if (fm*fl > 0.0D)
                {
                    lower = mid;
                    fl = fm;
                }
                else
                {
                    upper = mid;
                    fu = fm;
                }
                lastMid = mid;
                mid = lower + (upper - lower)*Math.Abs(fl)/(Math.Abs(fl) + Math.Abs(fu)); // mid-point
                fm = g.function(mid);
                diff = Math.Abs(mid - lastMid);
                m_iterN++;
                if (m_iterN > m_iterMax)
                {
                    if (!m_blnSupressLimitReachedMessage)
                    {
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(
                                "Class: RealRoot; method: falsePostion; maximum number of iterations exceeded - root at this point, " +
                                Fmath.truncate(mid, 4) + ", returned");
                        }
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) +
                                                    ", tolerance = " + m_tol);
                        }
                    }
                    m_root = mid;
                    testConv = false;
                }
            }
            return m_root;
        }

        // Combined bisection and Newton Raphson method
        // bounds already entered
        public double bisectNewtonRaphson(RealRootDerivFunction g)
        {
            return bisectNewtonRaphson(g, m_lowerBound, m_upperBound);
        }

        // Combined bisection and Newton Raphson method
        // default accuracy used
        public double bisectNewtonRaphson(RealRootDerivFunction g, double lower, double upper)
        {
            m_lowerBound = lower;
            m_upperBound = upper;

            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }

            bool testConv = true; // convergence test: becomes false on convergence
            m_iterN = 0; // number of iterations
            double temp = 0.0D;

            if (upper < lower)
            {
                temp = upper;
                upper = lower;
                lower = temp;
            }

            // calculate the function value at the estimate of the higher bound to x
            double[] f = g.function(upper);
            double fu = f[0];
            // calculate the function value at the estimate of the lower bound of x
            f = g.function(lower);
            double fl = f[0];
            if (double.IsNaN(fl))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: bisectNewtonRaphson: lower bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: bisectNewtonRaphson: upper bound returned NaN as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }

            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noBoundExtensions)
                    {
                        string message =
                            "RealRoot.bisectNewtonRaphson: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(message);
                        }
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumBoundsExtension)
                        {
                            string message =
                                "RealRoot.bisectNewtonRaphson: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            if (!m_blnSupressNaNmessage)
                            {
                                PrintToScreen.WriteLine(message);
                            }
                            return double.NaN;
                        }
                        if (!m_noLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            f = g.function(lower);
                            fl = f[0];
                        }
                        if (!m_noUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            f = g.function(upper);
                            fu = f[0];
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                m_root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                m_root = upper;
                testConv = false;
            }

            // Function at mid-point of initial estimates
            double mid = (lower + upper)/2.0D; // mid point (bisect) or new x estimate (Newton-Raphson)
            double lastMidB = mid; // last succesful mid point
            f = g.function(mid);
            double diff = f[0]/f[1]; // difference between successive estimates of the root
            double fm = f[0];
            double fmB = fm; // last succesful mid value function value
            double lastMid = mid;
            mid = mid - diff;
            bool lastMethod = true; // true; last method = Newton Raphson, false; last method = bisection method
            bool nextMethod = true; // true; next method = Newton Raphson, false; next method = bisection method

            // search
            while (testConv)
            {
                // test for convergence
                if (fm == 0.0D || Math.Abs(diff) < m_tol)
                {
                    testConv = false;
                    if (fm == 0.0D)
                    {
                        m_root = lastMid;
                    }
                    else
                    {
                        if (Math.Abs(diff) < m_tol)
                        {
                            m_root = mid;
                        }
                    }
                }
                else
                {
                    lastMethod = nextMethod;
                    // test for succesfull Newton-Raphson
                    if (lastMethod)
                    {
                        if (mid < lower || mid > upper)
                        {
                            // Newton Raphson failed
                            nextMethod = false;
                        }
                        else
                        {
                            fmB = fm;
                            lastMidB = mid;
                        }
                    }
                    else
                    {
                        nextMethod = true;
                    }
                    if (nextMethod)
                    {
                        // Newton-Raphson procedure
                        f = g.function(mid);
                        fm = f[0];
                        diff = f[0]/f[1];
                        lastMid = mid;
                        mid = mid - diff;
                    }
                    else
                    {
                        // Bisection procedure
                        fm = fmB;
                        mid = lastMidB;
                        if (fm*fl > 0.0D)
                        {
                            lower = mid;
                            fl = fm;
                        }
                        else
                        {
                            upper = mid;
                            fu = fm;
                        }
                        lastMid = mid;
                        mid = (lower + upper)/2.0D;
                        f = g.function(mid);
                        fm = f[0];
                        diff = mid - lastMid;
                        fmB = fm;
                        lastMidB = mid;
                    }
                }
                m_iterN++;
                if (m_iterN > m_iterMax)
                {
                    if (!m_blnSupressLimitReachedMessage)
                    {
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(
                                "Class: RealRoot; method: bisectNewtonRaphson; maximum number of iterations exceeded - root at this point, " +
                                Fmath.truncate(mid, 4) + ", returned");
                        }
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) +
                                                    ", tolerance = " + m_tol);
                        }
                    }
                    m_root = mid;
                    testConv = false;
                }
            }
            return m_root;
        }

        // Newton Raphson method
        // estimate already entered
        public double newtonRaphson(RealRootDerivFunction g)
        {
            return newtonRaphson(g, m_estimate);
        }

        // Newton Raphson method
        public double newtonRaphson(RealRootDerivFunction g, double x)
        {
            m_estimate = x;
            bool testConv = true; // convergence test: becomes false on convergence
            m_iterN = 0; // number of iterations
            double diff = 1e300; // difference between the last two successive mid-pint x values

            // calculate the function and derivative value at the initial estimate  x
            double[] f = g.function(x);
            if (double.IsNaN(f[0]))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: newtonRaphson: NaN returned as the function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("NaN returned as the function value");
                }
            }
            if (double.IsNaN(f[1]))
            {
                if (m_returnNaN)
                {
                    if (!m_blnSupressNaNmessage)
                    {
                        PrintToScreen.WriteLine(
                            "RealRoot: newtonRaphson: NaN returned as the derivative function value - NaN returned as root");
                    }
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("NaN returned as the derivative function value");
                }
            }


            // search
            while (testConv)
            {
                diff = f[0]/f[1];
                if (f[0] == 0.0D || Math.Abs(diff) < m_tol)
                {
                    m_root = x;
                    testConv = false;
                }
                else
                {
                    x -= diff;
                    f = g.function(x);
                    if (double.IsNaN(f[0]))
                    {
                        throw new ArithmeticException("NaN returned as the function value");
                    }
                    if (double.IsNaN(f[1]))
                    {
                        throw new ArithmeticException("NaN returned as the derivative function value");
                    }
                    if (double.IsNaN(f[0]))
                    {
                        if (m_returnNaN)
                        {
                            if (!m_blnSupressNaNmessage)
                            {
                                PrintToScreen.WriteLine(
                                    "RealRoot: bisect: NaN as the function value - NaN returned as root");
                            }
                            return double.NaN;
                        }
                        else
                        {
                            throw new ArithmeticException("NaN as the function value");
                        }
                    }
                    if (double.IsNaN(f[1]))
                    {
                        if (m_returnNaN)
                        {
                            if (!m_blnSupressNaNmessage)
                            {
                                PrintToScreen.WriteLine("NaN as the function value - NaN returned as root");
                            }
                            return double.NaN;
                        }
                        else
                        {
                            throw new ArithmeticException("NaN as the function value");
                        }
                    }
                }
                m_iterN++;
                if (m_iterN > m_iterMax)
                {
                    if (!m_blnSupressLimitReachedMessage)
                    {
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine(
                                "Class: RealRoot; method: newtonRaphson; maximum number of iterations exceeded - root at this point, " +
                                Fmath.truncate(x, 4) + ", returned");
                        }
                        if (!m_blnSupressNaNmessage)
                        {
                            PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) +
                                                    ", tolerance = " + m_tol);
                        }
                    }
                    m_root = x;
                    testConv = false;
                }
            }
            return m_root;
        }

        // STATIC METHODS

        // Reset the maximum iterations allowed  for static methods
        public void setStaticIterMax(int imax)
        {
            m_staticIterMax = imax;
        }

        // Get the maximum iterations allowed  for static methods
        public int getStaticIterMax()
        {
            return m_staticIterMax;
        }

        // Reset the maximum number of bounds extensions for static methods
        public void setStaticMaximumStaticBoundsExtension(int maximumBoundsExtension)
        {
            m_maximumStaticBoundsExtension = maximumBoundsExtension;
        }

        // Prevent extensions to the supplied bounds for static methods
        public void noStaticBoundsExtensions()
        {
            m_noStaticBoundExtensions = true;
            m_noStaticLowerBoundExtensions = true;
            m_noStaticUpperBoundExtensions = true;
        }

        // Prevent extension to the lower bound for static methods
        public void noStaticLowerBoundExtension()
        {
            m_noStaticLowerBoundExtensions = true;
            if (m_noStaticUpperBoundExtensions)
            {
                m_noStaticBoundExtensions = true;
            }
        }

        // Prevent extension to the upper bound for static methods
        public void noStaticUpperBoundExtension()
        {
            m_noStaticUpperBoundExtensions = true;
            if (m_noStaticLowerBoundExtensions)
            {
                m_noStaticBoundExtensions = true;
            }
        }

        // Reset exception handling for NaN bound flag to true for static methods
        // when flag returnNaN = true exceptions resulting from a bound being NaN do not halt the prorgam but return NaN
        // required by PsRandom and Stat classes calling RealRoot
        public void resetStaticNaNexceptionToTrue()
        {
            m_staticReturnNaN = true;
        }

        // Reset exception handling for NaN bound flag to false for static methods
        // when flag returnNaN = false exceptions resulting from a bound being NaN  halts the prorgam
        // required by PsRandom and Stat classes calling RealRoot
        public void resetStaticNaNexceptionToFalse()
        {
            m_staticReturnNaN = false;
        }


        // Combined bisection and Inverse Quadratic Interpolation method
        // bounds supplied as arguments
        public static double brent(RealRootFunction g, double lower, double upper, double tol)
        {
            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }

            double root = double.NaN;
            bool testConv = true; // convergence test: becomes false on convergence
            int iterN = 0;
            double temp = 0.0D;

            if (upper < lower)
            {
                temp = upper;
                upper = lower;
                lower = temp;
            }

            // calculate the function value at the estimate of the higher bound to x
            double fu = g.function(upper);
            // calculate the function value at the estimate of the lower bound of x
            double fl = g.function(lower);
            if (double.IsNaN(fl))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "Realroot: brent: lower bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "Realroot: brent: upper bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }

            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noStaticBoundExtensions)
                    {
                        string message = "RealRoot.brent: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        PrintToScreen.WriteLine(message);
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumStaticBoundsExtension)
                        {
                            string message =
                                "RealRoot.brent: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumStaticBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            PrintToScreen.WriteLine(message);
                            return double.NaN;
                        }
                        if (!m_noStaticLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            fl = g.function(lower);
                        }
                        if (!m_noStaticUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            fu = g.function(upper);
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                root = upper;
                testConv = false;
            }

            // Function at mid-point of initial estimates
            double mid = (lower + upper)/2.0D; // mid point (bisect) or new x estimate (Inverse Quadratic Interpolation)
            double lastMidB = mid; // last succesful mid point
            double fm = g.function(mid);
            double diff = mid - lower; // difference between successive estimates of the root
            double fmB = fm; // last succesful mid value function value
            double lastMid = mid;
            bool lastMethod = true;
            // true; last method = Inverse Quadratic Interpolation, false; last method = bisection method
            bool nextMethod = true;
            // true; next method = Inverse Quadratic Interpolation, false; next method = bisection method

            // search
            double rr = 0.0D, ss = 0.0D, tt = 0.0D, pp = 0.0D, qq = 0.0D; // interpolation variables
            while (testConv)
            {
                // test for convergence
                if (fm == 0.0D || Math.Abs(diff) < tol)
                {
                    testConv = false;
                    if (fm == 0.0D)
                    {
                        root = lastMid;
                    }
                    else
                    {
                        if (Math.Abs(diff) < tol)
                        {
                            root = mid;
                        }
                    }
                }
                else
                {
                    lastMethod = nextMethod;
                    // test for succesfull inverse quadratic interpolation
                    if (lastMethod)
                    {
                        if (mid < lower || mid > upper)
                        {
                            // inverse quadratic interpolation failed
                            nextMethod = false;
                        }
                        else
                        {
                            fmB = fm;
                            lastMidB = mid;
                        }
                    }
                    else
                    {
                        nextMethod = true;
                    }
                    if (nextMethod)
                    {
                        // inverse quadratic interpolation
                        fl = g.function(lower);
                        fm = g.function(mid);
                        fu = g.function(upper);
                        rr = fm/fu;
                        ss = fm/fl;
                        tt = fl/fu;
                        pp = ss*(tt*(rr - tt)*(upper - mid) - (1.0D - rr)*(mid - lower));
                        qq = (tt - 1.0D)*(rr - 1.0D)*(ss - 1.0D);
                        lastMid = mid;
                        diff = pp/qq;
                        mid = mid + diff;
                    }
                    else
                    {
                        // Bisection procedure
                        fm = fmB;
                        mid = lastMidB;
                        if (fm*fl > 0.0D)
                        {
                            lower = mid;
                            fl = fm;
                        }
                        else
                        {
                            upper = mid;
                            fu = fm;
                        }
                        lastMid = mid;
                        mid = (lower + upper)/2.0D;
                        fm = g.function(mid);
                        diff = mid - lastMid;
                        fmB = fm;
                        lastMidB = mid;
                    }
                }
                iterN++;
                if (iterN > m_staticIterMax)
                {
                    PrintToScreen.WriteLine(
                        "Class: RealRoot; method: brent; maximum number of iterations exceeded - root at this point, " +
                        Fmath.truncate(mid, 4) + ", returned");
                    PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) + ", tolerance = " +
                                            tol);
                    root = mid;
                    testConv = false;
                }
            }
            return root;
        }


        // bisection method
        // tolerance supplied
        public static double bisect(RealRootFunction g, double lower, double upper, double tol)
        {
            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }
            if (upper < lower)
            {
                double temp = upper;
                upper = lower;
                lower = temp;
            }

            double root = double.NaN; // variable to hold the returned root
            bool testConv = true; // convergence test: becomes false on convergence
            int iterN = 0; // number of iterations
            double diff = 1e300; // abs(difference between the last two successive mid-pint x values)

            // calculate the function value at the estimate of the higher bound to x
            double fu = g.function(upper);
            // calculate the function value at the estimate of the lower bound of x
            double fl = g.function(lower);
            if (double.IsNaN(fl))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: bisect: lower bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: bisect: upper bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }
            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noStaticBoundExtensions)
                    {
                        string message = "RealRoot.bisect: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        PrintToScreen.WriteLine(message);
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumStaticBoundsExtension)
                        {
                            string message =
                                "RealRoot.bisect: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumStaticBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            PrintToScreen.WriteLine(message);
                            return double.NaN;
                        }
                        if (!m_noStaticLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            fl = g.function(lower);
                        }
                        if (!m_noStaticUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            fu = g.function(upper);
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                root = upper;
                testConv = false;
            }

            // start search
            double mid = (lower + upper)/2.0D; // mid-point
            double lastMid = 1e300; // previous mid-point
            double fm = g.function(mid);
            while (testConv)
            {
                if (fm == 0.0D || diff < tol)
                {
                    testConv = false;
                    root = mid;
                }
                if (fm*fl > 0.0D)
                {
                    lower = mid;
                    fl = fm;
                }
                else
                {
                    upper = mid;
                    fu = fm;
                }
                lastMid = mid;
                mid = (lower + upper)/2.0D;
                fm = g.function(mid);
                diff = Math.Abs(mid - lastMid);
                iterN++;
                if (iterN > m_staticIterMax)
                {
                    PrintToScreen.WriteLine(
                        "Class: RealRoot; method: bisect; maximum number of iterations exceeded - root at this point, " +
                        Fmath.truncate(mid, 4) + ", returned");
                    PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) + ", tolerance = " +
                                            tol);
                    root = mid;
                    testConv = false;
                }
            }
            return root;
        }


        // false position  method
        // tolerance supplied
        public static double falsePosition(RealRootFunction g, double lower, double upper, double tol)
        {
            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }
            if (upper < lower)
            {
                double temp = upper;
                upper = lower;
                lower = temp;
            }

            double root = double.NaN; // variable to hold the returned root
            bool testConv = true; // convergence test: becomes false on convergence
            int iterN = 0; // number of iterations
            double diff = 1e300; // abs(difference between the last two successive mid-pint x values)

            // calculate the function value at the estimate of the higher bound to x
            double fu = g.function(upper);
            // calculate the function value at the estimate of the lower bound of x
            double fl = g.function(lower);
            if (double.IsNaN(fl))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: fals: ePositionlower bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: falsePosition: upper bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }

            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noStaticBoundExtensions)
                    {
                        string message = "RealRoot.falsePosition: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        PrintToScreen.WriteLine(message);
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumStaticBoundsExtension)
                        {
                            string message =
                                "RealRoot.falsePosition: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumStaticBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            PrintToScreen.WriteLine(message);
                            return double.NaN;
                        }
                        if (!m_noStaticLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            fl = g.function(lower);
                        }
                        if (!m_noStaticUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            fu = g.function(upper);
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                root = upper;
                testConv = false;
            }

            // start search
            double mid = lower + (upper - lower)*Math.Abs(fl)/(Math.Abs(fl) + Math.Abs(fu)); // mid-point
            double lastMid = 1e300; // previous mid-point
            double fm = g.function(mid);
            while (testConv)
            {
                if (fm == 0.0D || diff < tol)
                {
                    testConv = false;
                    root = mid;
                }
                if (fm*fl > 0.0D)
                {
                    lower = mid;
                    fl = fm;
                }
                else
                {
                    upper = mid;
                    fu = fm;
                }
                lastMid = mid;
                mid = lower + (upper - lower)*Math.Abs(fl)/(Math.Abs(fl) + Math.Abs(fu)); // mid-point
                fm = g.function(mid);
                diff = Math.Abs(mid - lastMid);
                iterN++;
                if (iterN > m_staticIterMax)
                {
                    PrintToScreen.WriteLine(
                        "Class: RealRoot; method: falsePostion; maximum number of iterations exceeded - root at this point, " +
                        Fmath.truncate(mid, 4) + ", returned");
                    PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) + ", tolerance = " +
                                            tol);
                    root = mid;
                    testConv = false;
                }
            }
            return root;
        }


        // Combined bisection and Newton Raphson method
        // tolerance supplied
        public static double bisectNewtonRaphson(RealRootDerivFunction g, double lower, double upper, double tol)
        {
            // check upper>lower
            if (upper == lower)
            {
                throw new ArgumentException("upper cannot equal lower");
            }

            double root = double.NaN;
            bool testConv = true; // convergence test: becomes false on convergence
            int iterN = 0; // number of iterations
            double temp = 0.0D;

            if (upper < lower)
            {
                temp = upper;
                upper = lower;
                lower = temp;
            }

            // calculate the function value at the estimate of the higher bound to x
            double[] f = g.function(upper);
            double fu = f[0];
            // calculate the function value at the estimate of the lower bound of x
            f = g.function(lower);
            double fl = f[0];
            if (double.IsNaN(fl))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: bisectNewtonRaphson: lower bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("lower bound returned NaN as the function value");
                }
            }
            if (double.IsNaN(fu))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: bisectNewtonRaphson: upper bound returned NaN as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("upper bound returned NaN as the function value");
                }
            }

            // check that the root has been bounded and extend bounds if not and extension allowed
            bool testBounds = true;
            int numberOfBoundsExtension = 0;
            double initialBoundsDifference = (upper - lower)/2.0D;
            while (testBounds)
            {
                if (fu*fl <= 0.0D)
                {
                    testBounds = false;
                }
                else
                {
                    if (m_noStaticBoundExtensions)
                    {
                        string message =
                            "RealRoot.bisectNewtonRaphson: root not bounded and no extension to bounds allowed\n";
                        message += "NaN returned";
                        PrintToScreen.WriteLine(message);
                        return double.NaN;
                    }
                    else
                    {
                        numberOfBoundsExtension++;
                        if (numberOfBoundsExtension > m_maximumStaticBoundsExtension)
                        {
                            string message =
                                "RealRoot.bisectNewtonRaphson: root not bounded and maximum number of extension to bounds allowed, " +
                                m_maximumStaticBoundsExtension + ", exceeded\n";
                            message += "NaN returned";
                            PrintToScreen.WriteLine(message);
                            return double.NaN;
                        }
                        if (!m_noStaticLowerBoundExtensions)
                        {
                            lower -= initialBoundsDifference;
                            f = g.function(lower);
                            fl = f[0];
                        }
                        if (!m_noStaticUpperBoundExtensions)
                        {
                            upper += initialBoundsDifference;
                            f = g.function(upper);
                            fu = f[0];
                        }
                    }
                }
            }

            // check initial values for true root value
            if (fl == 0.0D)
            {
                root = lower;
                testConv = false;
            }
            if (fu == 0.0D)
            {
                root = upper;
                testConv = false;
            }

            // Function at mid-point of initial estimates
            double mid = (lower + upper)/2.0D; // mid point (bisect) or new x estimate (Newton-Raphson)
            double lastMidB = mid; // last succesful mid point
            f = g.function(mid);
            double diff = f[0]/f[1]; // difference between successive estimates of the root
            double fm = f[0];
            double fmB = fm; // last succesful mid value function value
            double lastMid = mid;
            mid = mid - diff;
            bool lastMethod = true; // true; last method = Newton Raphson, false; last method = bisection method
            bool nextMethod = true; // true; next method = Newton Raphson, false; next method = bisection method

            // search
            while (testConv)
            {
                // test for convergence
                if (fm == 0.0D || Math.Abs(diff) < tol)
                {
                    testConv = false;
                    if (fm == 0.0D)
                    {
                        root = lastMid;
                    }
                    else
                    {
                        if (Math.Abs(diff) < tol)
                        {
                            root = mid;
                        }
                    }
                }
                else
                {
                    lastMethod = nextMethod;
                    // test for succesfull Newton-Raphson
                    if (lastMethod)
                    {
                        if (mid < lower || mid > upper)
                        {
                            // Newton Raphson failed
                            nextMethod = false;
                        }
                        else
                        {
                            fmB = fm;
                            lastMidB = mid;
                        }
                    }
                    else
                    {
                        nextMethod = true;
                    }
                    if (nextMethod)
                    {
                        // Newton-Raphson procedure
                        f = g.function(mid);
                        fm = f[0];
                        diff = f[0]/f[1];
                        lastMid = mid;
                        mid = mid - diff;
                    }
                    else
                    {
                        // Bisection procedure
                        fm = fmB;
                        mid = lastMidB;
                        if (fm*fl > 0.0D)
                        {
                            lower = mid;
                            fl = fm;
                        }
                        else
                        {
                            upper = mid;
                            fu = fm;
                        }
                        lastMid = mid;
                        mid = (lower + upper)/2.0D;
                        f = g.function(mid);
                        fm = f[0];
                        diff = mid - lastMid;
                        fmB = fm;
                        lastMidB = mid;
                    }
                }
                iterN++;
                if (iterN > m_staticIterMax)
                {
                    PrintToScreen.WriteLine(
                        "Class: RealRoot; method: bisectNewtonRaphson; maximum number of iterations exceeded - root at this point, " +
                        Fmath.truncate(mid, 4) + ", returned");
                    PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) + ", tolerance = " +
                                            tol);
                    root = mid;
                    testConv = false;
                }
            }
            return root;
        }


        // Newton Raphson method
        public static double newtonRaphson(RealRootDerivFunction g, double x, double tol)
        {
            double root = double.NaN;
            bool testConv = true; // convergence test: becomes false on convergence
            int iterN = 0; // number of iterations
            double diff = 1e300; // difference between the last two successive mid-pint x values

            // calculate the function and derivative value at the initial estimate  x
            double[] f = g.function(x);
            if (double.IsNaN(f[0]))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: newtonRaphson: NaN returned as the function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("NaN returned as the function value");
                }
            }
            if (double.IsNaN(f[1]))
            {
                if (m_staticReturnNaN)
                {
                    PrintToScreen.WriteLine(
                        "RealRoot: newtonRaphson: NaN returned as the derivative function value - NaN returned as root");
                    return double.NaN;
                }
                else
                {
                    throw new ArithmeticException("NaN returned as the derivative function value");
                }
            }


            // search
            while (testConv)
            {
                diff = f[0]/f[1];
                if (f[0] == 0.0D || Math.Abs(diff) < tol)
                {
                    root = x;
                    testConv = false;
                }
                else
                {
                    x -= diff;
                    f = g.function(x);
                    if (double.IsNaN(f[0]))
                    {
                        throw new ArithmeticException("NaN returned as the function value");
                    }
                    if (double.IsNaN(f[1]))
                    {
                        throw new ArithmeticException("NaN returned as the derivative function value");
                    }
                    if (double.IsNaN(f[0]))
                    {
                        if (m_staticReturnNaN)
                        {
                            PrintToScreen.WriteLine(
                                "RealRoot: NewtonRaphson: NaN as the function value - NaN returned as root");
                            return double.NaN;
                        }
                        else
                        {
                            throw new ArithmeticException("NaN as the function value");
                        }
                    }
                    if (double.IsNaN(f[1]))
                    {
                        if (m_staticReturnNaN)
                        {
                            PrintToScreen.WriteLine("NaN as the function value - NaN returned as root");
                            return double.NaN;
                        }
                        else
                        {
                            throw new ArithmeticException("NaN as the function value");
                        }
                    }
                }
                iterN++;
                if (iterN > m_staticIterMax)
                {
                    PrintToScreen.WriteLine(
                        "Class: RealRoot; method: newtonRaphson; maximum number of iterations exceeded - root at this point, " +
                        Fmath.truncate(x, 4) + ", returned");
                    PrintToScreen.WriteLine("Last mid-point difference = " + Fmath.truncate(diff, 4) + ", tolerance = " +
                                            tol);
                    root = x;
                    testConv = false;
                }
            }
            return root;
        }

        // ROOTS OF A QUADRATIC EQUATION
        // c + bx + ax^2 = 0
        // roots returned in root[]
        // 4ac << b*b accomodated by these methods
        // roots returned as double in an List if roots are real
        // roots returned as Complex in an List if any root is complex
        public static List<Object> quadratic(double c, double b, double a)
        {
            List<Object> roots = new List<Object>(2);

            double bsquared = b*b;
            double fourac = 4.0*a*c;
            if (bsquared < fourac)
            {
                Mathematics.Complex.ComplexClass[] croots = ComplexPoly.quadratic(c, b, a);
                roots.Add("complex");
                roots.Add(croots);
            }
            else
            {
                double[] droots = new double[2];
                double bsign = Fmath.sign(b);
                double qsqrt = Math.Sqrt(bsquared - fourac);
                qsqrt = -0.5*(b + bsign*qsqrt);
                droots[0] = qsqrt/a;
                droots[1] = c/qsqrt;
                roots.Add("real");
                roots.Add(droots);
            }
            return roots;
        }


        // ROOTS OF A CUBIC EQUATION
        // a + bx + cx^2 + dx^3 = 0
        // roots returned as double in an List if roots are real
        // roots returned as Complex in an List if any root is complex
        public static List<Object> cubic(double a, double b, double c, double d)
        {
            List<Object> roots = new List<Object>(2);

            double aa = c/d;
            double bb = b/d;
            double cc = a/d;
            double bigQ = (aa*aa - 3.0*bb)/9.0;
            double bigQcubed = bigQ*bigQ*bigQ;
            double bigR = (2.0*aa*aa*aa - 9.0*aa*bb + 27.0*cc)/54.0;
            double bigRsquared = bigR*bigR;

            if (bigRsquared >= bigQcubed)
            {
                Mathematics.Complex.ComplexClass[] croots = ComplexPoly.cubic(a, b, c, d);
                roots.Add("complex");
                roots.Add(croots);
            }
            else
            {
                double[] droots = new double[3];
                double theta = Math.Acos(bigR/Math.Sqrt(bigQcubed));
                double aover3 = aa/3.0;
                double qterm = -2.0*Math.Sqrt(bigQ);

                droots[0] = qterm*Math.Cos(theta/3.0) - aover3;
                droots[1] = qterm*Math.Cos((theta + 2.0*Math.PI)/3.0) - aover3;
                droots[2] = qterm*Math.Cos((theta - 2.0*Math.PI)/3.0) - aover3;
                roots.Add("real");
                roots.Add(droots);
            }
            return roots;
        }

        // ROOTS OF A POLYNOMIAL
        // For general details of root searching and a discussion of the rounding errors
        // see Numerical Recipes, The Art of Scientific Computing
        // by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
        // Cambridge University Press,   http://www.nr.com/

        // Calculate the roots  of a real polynomial
        // initial root estimate is zero [for deg>3]
        // roots are not olished [for deg>3]
        public static List<Object> polynomial(double[] coeff)
        {
            bool polish = true;
            double estx = 0.0;
            return polynomial(coeff, polish, estx);
        }

        // Calculate the roots  of a real polynomial
        // initial root estimate is zero [for deg>3]
        // roots are polished [for deg>3]
        public static List<Object> polynomial(double[] coeff, bool polish)
        {
            double estx = 0.0;
            return polynomial(coeff, polish, estx);
        }

        // Calculate the roots  of a real polynomial
        // initial root estimate is estx [for deg>3]
        // roots are not polished [for deg>3]
        public static List<Object> polynomial(double[] coeff, double estx)
        {
            bool polish = true;
            return polynomial(coeff, polish, estx);
        }

        // Calculate the roots  of a real polynomial
        // initial root estimate is estx [for deg>3]
        // roots are polished [for deg>3]
        public static List<Object> polynomial(double[] coeff, bool polish, double estx)
        {
            int nCoeff = coeff.Length;
            if (nCoeff < 2)
            {
                throw new ArgumentException("a minimum of two coefficients is required");
            }
            List<Object> roots = new List<Object>(nCoeff);
            bool realRoots = true;

            // check for zero roots
            int nZeros = 0;
            int ii = 0;
            bool testZero = true;
            while (testZero)
            {
                if (coeff[ii] == 0.0)
                {
                    nZeros++;
                    ii++;
                }
                else
                {
                    testZero = false;
                }
            }

            // Repack coefficients
            int nCoeffWz = nCoeff - nZeros;
            double[] coeffWz = new double[nCoeffWz];
            if (nZeros > 0)
            {
                for (int i = 0; i < nCoeffWz; i++)
                {
                    coeffWz[i] = coeff[i + nZeros];
                }
            }
            else
            {
                for (int i = 0; i < nCoeffWz; i++)
                {
                    coeffWz[i] = coeff[i];
                }
            }

            // Calculate non-zero roots
            List<Object> temp = new List<Object>(2);
            double[] cdreal = null;
            switch (nCoeffWz)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    temp.Add("real");
                    double[] dtemp = {-coeffWz[0]/coeffWz[1]};
                    temp.Add(dtemp);
                    break;
                case 3:
                    temp = quadratic(coeffWz[0], coeffWz[1], coeffWz[2]);
                    if (((string) temp[0]).Equals("complex"))
                    {
                        realRoots = false;
                    }
                    break;
                case 4:
                    temp = cubic(coeffWz[0], coeffWz[1], coeffWz[2], coeffWz[3]);
                    if (((string) temp[0]).Equals("complex"))
                    {
                        realRoots = false;
                    }
                    break;
                default:
                    ComplexPoly cp = new ComplexPoly(coeffWz);
                    Mathematics.Complex.ComplexClass[] croots = cp.roots(polish, new Mathematics.Complex.ComplexClass(estx, 0.0));
                    cdreal = new double[nCoeffWz - 1];
                    int counter = 0;
                    for (int i = 0; i < (nCoeffWz - 1); i++)
                    {
                        if (croots[i].getImag()/croots[i].getReal() < m_realTol)
                        {
                            cdreal[i] = croots[i].getReal();
                            counter++;
                        }
                    }
                    if (counter == (nCoeffWz - 1))
                    {
                        temp.Add("real");
                        temp.Add(cdreal);
                    }
                    else
                    {
                        temp.Add("complex");
                        temp.Add(croots);
                        realRoots = false;
                    }
                    break;
            }

            // Pack roots into returned List
            if (nZeros == 0)
            {
                roots = temp;
            }
            else
            {
                if (realRoots)
                {
                    double[] dtemp1 = new double[nCoeff - 1];
                    double[] dtemp2 = (double[]) temp[1];
                    for (int i = 0; i < nCoeffWz - 1; i++)
                    {
                        dtemp1[i] = dtemp2[i];
                    }
                    for (int i = 0; i < nZeros; i++)
                    {
                        dtemp1[i + nCoeffWz - 1] = 0.0;
                    }
                    roots.Add("real");
                    roots.Add(dtemp1);
                }
                else
                {
                    Mathematics.Complex.ComplexClass[] dtemp1 = Mathematics.Complex.ComplexClass.oneDarray(nCoeff - 1);
                    Mathematics.Complex.ComplexClass[] dtemp2 = (Mathematics.Complex.ComplexClass[])temp[1];
                    for (int i = 0; i < nCoeffWz - 1; i++)
                    {
                        dtemp1[i] = dtemp2[i];
                    }
                    for (int i = 0; i < nZeros; i++)
                    {
                        dtemp1[i + nCoeffWz - 1] = new Mathematics.Complex.ComplexClass(0.0, 0.0);
                    }
                    roots.Add("complex");
                    roots.Add(dtemp1);
                }
            }

            return roots;
        }

        // Reset the criterion for deciding a that a root, calculated as Complex, is real
        // Default option; imag/real <1e-14
        // this method allows thew value of 1e-14 to be reset
        public void resetRealTest(double ratio)
        {
            m_realTol = ratio;
        }
    }
}
