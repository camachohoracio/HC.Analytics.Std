#region

using System;
using System.Collections.Generic;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Analysis
{
    /*
    *   Class Integration
    *       interface IntegralFunction also required
    *
    *   Contains the methods for Gaussian-Legendre quadrature, the
    *   backward and forward rectangular rules and the trapezium rule
    *
    *   The function to be integrated is supplied by means of
    *       an interface, IntegralFunction
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:	 February 2002
    *   UPDATE:  22 June 2003, 16 July 2006, 25 April 2007, 2 May 2007, 4 July 2008, 22 September 2008
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web page:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/Integration.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    *   Copyright (c) 2002 - 2008 Michael Thomas Flanagan
    *
    * PERMISSION TO COPY:
    *
    * Permission to use, copy and modify this software and its documentation for NON-COMMERCIAL purposes is granted, without fee,
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

    // Numerical integration class
    public class Integration
    {
        private static readonly List<double[]> m_gaussQuadDistArrayList = new List<double[]>();
        // Gauss-Legendre distances

        private static readonly List<int> m_gaussQuadIndex = new List<int>(); // Gauss-Legendre indices

        private static readonly List<double[]> m_gaussQuadWeightArrayList = new List<double[]>();
        // Gauss-Legendre weights

        private static double m_trapAccuracy;
        // actual accuracy at which iterative trapezium is terminated as class variable

        private static int m_trapIntervals = 1;
        // number of intervals in trapezium at which accuracy was satisfied as class variable

        private bool m_blnSetGLpoints; // = true when glPoints set

        private bool m_blnSetLimits; // = true when limits set

        private int m_glPoints; // Number of points in the Gauss-Legendre integration
        private IntegralFunction m_integralFunc; // Function to be integrated

        private double m_integralSum; // Sum returned by the numerical integration method
        private double m_lowerLimit = double.NaN; // Lower integration limit
        private int m_maxIntervals; // maximum number of intervals allowed in iterative trapezium
        private int m_nIntervals; // Number of intervals in the rectangular rule integrations

        // Iterative trapezium rule
        private double m_requiredAccuracy; // required accuracy at which iterative trapezium is terminated
        private bool m_setFunction; // = true when IntegralFunction set
        private bool m_setIntegration; // = true when integration performed
        private bool m_setIntervals; // = true when nIntervals set

        private double m_trapeziumAccuracy;
        // actual accuracy at which iterative trapezium is terminated as instance variable

        private int m_trapeziumIntervals = 1;
        // number of intervals in trapezium at which accuracy was satisfied as instance variable

        private double m_upperLimit = double.NaN; // Upper integration limit

        // CONSTRUCTORS

        // Default constructor
        public Integration()
        {
        }

        // Constructor taking function to be integrated
        public Integration(IntegralFunction intFunc)
        {
            m_integralFunc = intFunc;
            m_setFunction = true;
        }

        // Constructor taking function to be integrated and the limits
        public Integration(IntegralFunction intFunc, double lowerLimit, double upperLimit)
        {
            m_integralFunc = intFunc;
            m_setFunction = true;
            m_lowerLimit = lowerLimit;
            m_upperLimit = upperLimit;
            m_blnSetLimits = true;
        }

        // SET METHODS

        // Set function to be integrated
        public void setIntegrationFunction(IntegralFunction intFunc)
        {
            m_integralFunc = intFunc;
            m_setFunction = true;
        }

        // Set limits
        public void setLimits(double lowerLimit, double upperLimit)
        {
            m_lowerLimit = lowerLimit;
            m_upperLimit = upperLimit;
            m_blnSetLimits = true;
        }

        // Set lower limit
        public void setLowerLimit(double lowerLimit)
        {
            m_lowerLimit = lowerLimit;
            if (!double.IsNaN(m_upperLimit))
            {
                m_blnSetLimits = true;
            }
        }

        // Set lower limit
        public void setlowerLimit(double lowerLimit)
        {
            m_lowerLimit = lowerLimit;
            if (!double.IsNaN(m_upperLimit))
            {
                m_blnSetLimits = true;
            }
        }

        // Set upper limit
        public void setUpperLimit(double upperLimit)
        {
            m_upperLimit = upperLimit;
            if (!double.IsNaN(m_lowerLimit))
            {
                m_blnSetLimits = true;
            }
        }

        // Set upper limit
        public void setupperLimit(double upperLimit)
        {
            m_upperLimit = upperLimit;
            if (!double.IsNaN(m_lowerLimit))
            {
                m_blnSetLimits = true;
            }
        }

        // Set number of points in the Gaussian Legendre integration
        public void setGLpoints(int nPoints)
        {
            m_glPoints = nPoints;
            m_blnSetGLpoints = true;
        }

        // Set number of intervals in trapezoidal, forward or backward rectangular integration
        public void setNintervals(int nIntervals)
        {
            m_nIntervals = nIntervals;
            m_setIntervals = true;
        }

        // GET METHODS

        // Get the sum returned by the numerical integration
        public double getIntegralSum()
        {
            if (!m_setIntegration)
            {
                throw new ArgumentException("No integration has been performed");
            }
            return m_integralSum;
        }

        // GAUSSIAN-LEGENDRE QUADRATURE

        // Numerical integration using n point Gaussian-Legendre quadrature (instance method)
        // All parametes preset
        public double gaussQuad()
        {
            if (!m_blnSetGLpoints)
            {
                throw new ArgumentException("Number of points not set");
            }
            if (!m_blnSetLimits)
            {
                throw new ArgumentException("One limit or both limits not set");
            }
            if (!m_setFunction)
            {
                throw new ArgumentException("No integral function has been set");
            }

            double[] gaussQuadDist = new double[m_glPoints];
            double[] gaussQuadWeight = new double[m_glPoints];
            double sum = 0.0D;
            double xplus = 0.5D*(m_upperLimit + m_lowerLimit);
            double xminus = 0.5D*(m_upperLimit - m_lowerLimit);
            double dx = 0.0D;
            bool test = true;
            int k = -1, kn = -1;

            // Get Gauss-Legendre coefficients, i.e. the weights and scaled distances
            // Check if coefficients have been already calculated on an earlier call
            if (m_gaussQuadIndex.Count > 0)
            {
                for (k = 0; k < m_gaussQuadIndex.Count; k++)
                {
                    int ki = m_gaussQuadIndex[k];
                    if (ki == m_glPoints)
                    {
                        test = false;
                        kn = k;
                    }
                }
            }

            if (test)
            {
                // Calculate and store coefficients
                gaussQuadCoeff(gaussQuadDist, gaussQuadWeight, m_glPoints);
                m_gaussQuadIndex.Add(m_glPoints);
                m_gaussQuadDistArrayList.Add(gaussQuadDist);
                m_gaussQuadWeightArrayList.Add(gaussQuadWeight);
            }
            else
            {
                // Recover coefficients
                gaussQuadDist = m_gaussQuadDistArrayList[kn];
                gaussQuadWeight = m_gaussQuadWeightArrayList[kn];
            }

            // Perform summation
            for (int i = 0; i < m_glPoints; i++)
            {
                dx = xminus*gaussQuadDist[i];
                sum += gaussQuadWeight[i]*m_integralFunc.function(xplus + dx);
            }
            m_integralSum = sum*xminus; // rescale
            m_setIntegration = true; // integration performed
            return m_integralSum; // return value
        }

        // Numerical integration using n point Gaussian-Legendre quadrature (instance method)
        // All parametes except the number of points in the Gauss-Legendre integration preset
        public double gaussQuad(int glPoints)
        {
            m_glPoints = glPoints;
            m_blnSetGLpoints = true;
            return gaussQuad();
        }

        // Numerical integration using n point Gaussian-Legendre quadrature (static method)
        // All parametes provided
        public static double gaussQuad(IntegralFunction intFunc, double lowerLimit, double upperLimit, int glPoints)
        {
            Integration intgrtn = new Integration(intFunc, lowerLimit, upperLimit);
            return intgrtn.gaussQuad(glPoints);
        }

        // Returns the distance (gaussQuadDist) and weight coefficients (gaussQuadCoeff)
        // for an n point Gauss-Legendre Quadrature.
        // The Gauss-Legendre distances, gaussQuadDist, are scaled to -1 to 1
        // See Numerical Recipes for details
        public static void gaussQuadCoeff(double[] gaussQuadDist, double[] gaussQuadWeight, int n)
        {
            double z = 0.0D, z1 = 0.0D;
            double pp = 0.0D, p1 = 0.0D, p2 = 0.0D, p3 = 0.0D;

            double eps = 3e-11; // set required precision
            double x1 = -1.0D; // lower limit
            double x2 = 1.0D; // upper limit

            //  Calculate roots
            // Roots are symmetrical - only half calculated
            int m = (n + 1)/2;
            double xm = 0.5D*(x2 + x1);
            double xl = 0.5D*(x2 - x1);

            // Loop for  each root
            for (int i = 1; i <= m; i++)
            {
                // Approximation of ith root
                z = Math.Cos(Math.PI*(i - 0.25D)/(n + 0.5D));

                // Refinement on above using Newton's method
                do
                {
                    p1 = 1.0D;
                    p2 = 0.0D;

                    // Legendre polynomial (p1, evaluated at z, p2 is polynomial of
                    //  one order lower) recurrence relationsip
                    for (int j = 1; j <= n; j++)
                    {
                        p3 = p2;
                        p2 = p1;
                        p1 = ((2.0D*j - 1.0D)*z*p2 - (j - 1.0D)*p3)/j;
                    }
                    pp = n*(z*p1 - p2)/(z*z - 1.0D); // Derivative of p1
                    z1 = z;
                    z = z1 - p1/pp; // Newton's method
                }
                while (Math.Abs(z - z1) > eps);

                gaussQuadDist[i - 1] = xm - xl*z; // Scale root to desired interval
                gaussQuadDist[n - i] = xm + xl*z; // Symmetric counterpart
                gaussQuadWeight[i - 1] = 2.0*xl/((1.0 - z*z)*pp*pp); // Compute weight
                gaussQuadWeight[n - i] = gaussQuadWeight[i - 1]; // Symmetric counterpart
            }
        }

        // TRAPEZIUM METHODS

        // Numerical integration using the trapeziodal rule (instance method)
        // all parameters preset
        public double trapezium()
        {
            if (!m_setIntervals)
            {
                throw new ArgumentException("Number of intervals not set");
            }
            if (!m_blnSetLimits)
            {
                throw new ArgumentException("One limit or both limits not set");
            }
            if (!m_setFunction)
            {
                throw new ArgumentException("No integral function has been set");
            }

            double y1 = 0.0D;
            double interval = (m_upperLimit - m_lowerLimit)/m_nIntervals;
            double x0 = m_lowerLimit;
            double x1 = m_lowerLimit + interval;
            double y0 = m_integralFunc.function(x0);
            m_integralSum = 0.0D;

            for (int i = 0; i < m_nIntervals; i++)
            {
                // adjust last interval for rounding errors
                if (x1 > m_upperLimit)
                {
                    x1 = m_upperLimit;
                    interval -= (x1 - m_upperLimit);
                }

                // perform summation
                y1 = m_integralFunc.function(x1);
                m_integralSum += 0.5D*(y0 + y1)*interval;
                x0 = x1;
                y0 = y1;
                x1 += interval;
            }
            m_setIntegration = true;
            return m_integralSum;
        }

        // Numerical integration using the trapeziodal rule (instance method)
        // all parameters except the number of intervals preset
        public double trapezium(int nIntervals)
        {
            m_nIntervals = nIntervals;
            m_setIntervals = true;
            return trapezium();
        }

        // Numerical integration using the trapeziodal rule (static method)
        // all parameters to be provided
        public static double trapezium(IntegralFunction intFunc, double lowerLimit, double upperLimit, int nIntervals)
        {
            Integration intgrtn = new Integration(intFunc, lowerLimit, upperLimit);
            return intgrtn.trapezium(nIntervals);
        }

        // Numerical integration using an iteration on the number of intervals in the trapeziodal rule
        // until two successive results differ by less than a predetermined accuracy times the penultimate result
        public double trapezium(double accuracy, int maxIntervals)
        {
            m_requiredAccuracy = accuracy;
            m_maxIntervals = maxIntervals;
            m_trapeziumIntervals = 1;

            double summ = trapezium(m_integralFunc, m_lowerLimit, m_upperLimit, 1);
            double oldSumm = summ;
            int i = 2;
            for (i = 2; i <= maxIntervals; i++)
            {
                summ = trapezium(m_integralFunc, m_lowerLimit, m_upperLimit, i);
                m_trapeziumAccuracy = Math.Abs((summ - oldSumm)/oldSumm);
                if (m_trapeziumAccuracy <= m_requiredAccuracy)
                {
                    break;
                }
                oldSumm = summ;
            }

            if (i > maxIntervals)
            {
                PrintToScreen.WriteLine(
                    "accuracy criterion was not met in Integration.trapezium - current sum was returned as result.");
                m_trapeziumIntervals = maxIntervals;
            }
            else
            {
                m_trapeziumIntervals = i;
            }
            m_trapIntervals = m_trapeziumIntervals;
            m_trapAccuracy = m_trapeziumAccuracy;
            return summ;
        }

        // Numerical integration using an iteration on the number of intervals in the trapeziodal rule (static method)
        // until two successive results differ by less than a predtermined accuracy times the penultimate result
        // All parameters to be provided
        public static double trapezium(IntegralFunction intFunc, double lowerLimit, double upperLimit, double accuracy,
                                       int maxIntervals)
        {
            Integration intgrtn = new Integration(intFunc, lowerLimit, upperLimit);
            return intgrtn.trapezium(accuracy, maxIntervals);
        }

        // Get the number of intervals at which accuracy was last met in trapezium if using the instance trapezium call
        public int getTrapeziumIntervals()
        {
            return m_trapeziumIntervals;
        }

        // Get the number of intervals at which accuracy was last met in trapezium if using static trapezium call
        public static int getTrapIntervals()
        {
            return m_trapIntervals;
        }

        // Get the actual accuracy acheived when the iterative trapezium calls were terminated, using the instance method
        public double getTrapeziumAccuracy()
        {
            return m_trapeziumAccuracy;
        }

        // Get the actual accuracy acheived when the iterative trapezium calls were terminated, using the static method
        public static double getTrapAccuracy()
        {
            return m_trapAccuracy;
        }

        // BACKWARD RECTANGULAR METHODS

        // Numerical integration using the backward rectangular rule (instance method)
        // All parameters preset
        public double backward()
        {
            if (!m_setIntervals)
            {
                throw new ArgumentException("Number of intervals not set");
            }
            if (!m_blnSetLimits)
            {
                throw new ArgumentException("One limit or both limits not set");
            }
            if (!m_setFunction)
            {
                throw new ArgumentException("No integral function has been set");
            }

            double interval = (m_upperLimit - m_lowerLimit)/m_nIntervals;
            double x = m_lowerLimit + interval;
            double y = m_integralFunc.function(x);
            m_integralSum = 0.0D;

            for (int i = 0; i < m_nIntervals; i++)
            {
                // adjust last interval for rounding errors
                if (x > m_upperLimit)
                {
                    x = m_upperLimit;
                    interval -= (x - m_upperLimit);
                }

                // perform summation
                y = m_integralFunc.function(x);
                m_integralSum += y*interval;
                x += interval;
            }

            m_setIntegration = true;
            return m_integralSum;
        }

        // Numerical integration using the backward rectangular rule (instance method)
        // all parameters except number of intervals preset
        public double backward(int nIntervals)
        {
            m_nIntervals = nIntervals;
            m_setIntervals = true;
            return backward();
        }

        // Numerical integration using the backward rectangular rule (static method)
        // all parameters must be provided
        public static double backward(IntegralFunction intFunc, double lowerLimit, double upperLimit, int nIntervals)
        {
            Integration intgrtn = new Integration(intFunc, lowerLimit, upperLimit);
            return intgrtn.backward(nIntervals);
        }

        // FORWARD RECTANGULAR METHODS

        // Numerical integration using the forward rectangular rule
        // all parameters preset
        public double forward()
        {
            double interval = (m_upperLimit - m_lowerLimit)/m_nIntervals;
            double x = m_lowerLimit;
            double y = m_integralFunc.function(x);
            m_integralSum = 0.0D;

            for (int i = 0; i < m_nIntervals; i++)
            {
                // adjust last interval for rounding errors
                if (x > m_upperLimit)
                {
                    x = m_upperLimit;
                    interval -= (x - m_upperLimit);
                }

                // perform summation
                y = m_integralFunc.function(x);
                m_integralSum += y*interval;
                x += interval;
            }
            m_setIntegration = true;
            return m_integralSum;
        }

        // Numerical integration using the forward rectangular rule
        // all parameters except number of intervals preset
        public double forward(int nIntervals)
        {
            m_nIntervals = nIntervals;
            m_setIntervals = true;
            return forward();
        }

        // Numerical integration using the forward rectangular rule (static method)
        // all parameters provided
        public static double forward(IntegralFunction integralFunc, double lowerLimit, double upperLimit, int nIntervals)
        {
            Integration intgrtn = new Integration(integralFunc, lowerLimit, upperLimit);
            return intgrtn.forward(nIntervals);
        }

        public static double foreward(IntegralFunction integralFunc, double lowerLimit, double upperLimit,
                                      int nIntervals)
        {
            Integration intgrtn = new Integration(integralFunc, lowerLimit, upperLimit);
            return intgrtn.forward(nIntervals);
        }
    }
}
