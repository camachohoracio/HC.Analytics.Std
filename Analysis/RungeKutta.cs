#region

using System;
using HC.Analytics.ConvertClasses;

#endregion

namespace HC.Analytics.Analysis
{
/*
*   Class RungeKutta
*       requires interfaces DerivFunction and DerivnFunction
*
*   Contains the methods for the Runge-Kutta procedures for solving
*   single or solving sets of ordinary differential equations (ODEs)
*   [draws heavily on the approach adopted in Numerical Recipes
*   (C language version)http://www.nr.com]
*
*   A single ODE is supplied by means of an interface,
*       DerivFunction
*   A set of ODEs is supplied by means of an interface,
*       DerivnFunction
*
*   WRITTEN BY: Dr Michael Thomas Flanagan
*
*   DATE:	    February 2002
*   UPDATES:    22 June 2003,  April 2004,
*               15 September 2006 (to incorporate improvements suggested by Klaus Benary [Klaus.Benary@gede.de])
*               11 April 2007,  25 April 2007,   4 July 2008,   26-31 January 2010
*
*   DOCUMENTATION:
*   See Michael Thomas Flanagan's Java library on-line web page:
*   http://www.ee.ucl.ac.uk/~mflanaga/java/RungeKutta.html
*   http://www.ee.ucl.ac.uk/~mflanaga/java/
*
*   Copyright (c) 2002 - 2010
*
*   PERMISSION TO COPY:
*   Permission to use, copy and modify this software and its documentation for
*   NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
*   to the author, Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all copies.
*
*   Dr Michael Thomas Flanagan makes no representations about the suitability
*   or fitness of the software for any or for a particular purpose.
*   Michael Thomas Flanagan shall not be liable for any damages suffered
*   as a result of using, modifying or distributing this software or its derivatives.
*
***************************************************************************************/


// Class for Runge-Kutta solution of ordinary differential equations
    public class RungeKutta
    {
        private static double m_decrementFactor = -0.25;
        // factor used in calculating a step size decrement in Fehlberg and CashKarp procedures

        private static double m_incrementFactor = -0.2;
        // factor used in calculating a step size increment in Fehlberg and CashKarp procedures

        private static int m_nStepsMultiplier = 1000; // multiplied by number of steps to give internally calculated
        //   maximum allowed iterations in adaptive step methods

        private static double m_safetyFactor = 0.9;
        // safety factor for Runge Kutta Fehlberg and CashKarp tolerance checks as

        private double m_absTol = 1.0e-3;
        // tolerance additive factor to ensure non-zeto tolerance in adaptive step methods

        private int m_maxIter = -1; // maximum iterations allowed in adaptive step methods
        private int m_nIter; // number of iterations taken
        private int m_nODE; // number of ODEs
        private double m_relTol = 1.0e-5; // tolerance multiplicative factor in adaptive step methods
        private double m_step = double.NaN; // step size

        private double m_x0 = double.NaN; // initial value of x
        private double m_xn = double.NaN; //  value of x
        private double m_y0 = double.NaN; // initial value of y; single ODE
        private double[] m_yy0; // initial values of y; multiple ODEs

        public void setInitialValueOfX(double x0)
        {
            m_x0 = x0;
        }

        public void setFinalValueOfX(double xn)
        {
            m_xn = xn;
        }

        public void setInitialValueOfY(double y0)
        {
            setInitialValuesOfY(y0);
        }

        public void setInitialValueOfY(double[] yy0)
        {
            setInitialValuesOfY(yy0);
        }

        public void setInitialValuesOfY(double y0)
        {
            m_y0 = y0;
            m_yy0 = new double[1];
            m_yy0[0] = y0;
            m_nODE = 1;
        }

        public void setInitialValuesOfY(double[] yy0)
        {
            m_yy0 = yy0;
            m_nODE = yy0.Length;
            if (m_nODE == 1)
            {
                m_y0 = yy0[0];
            }
        }

        public void setStepSize(double step)
        {
            m_step = step;
        }

        public void setToleranceScalingFactor(double relTol)
        {
            m_relTol = relTol;
        }

        public void setToleranceAdditionFactor(double absTol)
        {
            m_absTol = absTol;
        }

        public void setMaximumIterations(int maxIter)
        {
            m_maxIter = maxIter;
        }

        public int getNumberOfIterations()
        {
            return m_nIter;
        }

        public static void resetNstepsMultiplier(int multiplier)
        {
            m_nStepsMultiplier = multiplier;
        }

        // Fourth order Runge-Kutta for a single ordinary differential equation
        // Non-static method
        public double fourthOrder(DerivFunction g)
        {
            if (double.IsNaN(m_x0))
            {
                throw new ArgumentException("No initial x value has been entered");
            }
            if (double.IsNaN(m_xn))
            {
                throw new ArgumentException("No  x value has been entered");
            }
            if (double.IsNaN(m_y0))
            {
                throw new ArgumentException("No initial y value has been entered");
            }
            if (double.IsNaN(m_step))
            {
                throw new ArgumentException("No step size has been entered");
            }

            double k1 = 0.0D, k2 = 0.0D, k3 = 0.0D, k4 = 0.0D;
            double x = 0.0D, y = m_y0;

            // Calculate nsteps
            double ns = (m_xn - m_x0)/m_step;
            ns = Converter.rint(ns);
            int nsteps = (int) ns; // number of steps
            m_nIter = nsteps;
            double stepUsed = (m_xn - m_x0)/ns;

            for (int i = 0; i < nsteps; i++)
            {
                x = m_x0 + i*stepUsed;

                k1 = stepUsed*g.deriv(x, y);
                k2 = stepUsed*g.deriv(x + stepUsed/2, y + k1/2);
                k3 = stepUsed*g.deriv(x + stepUsed/2, y + k2/2);
                k4 = stepUsed*g.deriv(x + stepUsed, y + k3);

                y += k1/6 + k2/3 + k3/3 + k4/6;
            }
            return y;
        }


        // Fourth order Runge-Kutta for a single ordinary differential equation (ODE)
        // Static method
        public static double fourthOrder(DerivFunction g, double x0, double y0, double xn, double h)
        {
            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValueOfY(y0);
            rk.setStepSize(h);

            return rk.fourthOrder(g);
        }

        // Fourth order Runge-Kutta for n (nODE) ordinary differential equations (ODE)
        // Non-static method
        public double[] fourthOrder(DerivnFunction g)
        {
            if (double.IsNaN(m_x0))
            {
                throw new ArgumentException("No initial x value has been entered");
            }
            if (double.IsNaN(m_xn))
            {
                throw new ArgumentException("No  x value has been entered");
            }
            if (m_yy0 == null)
            {
                throw new ArgumentException("No initial y values have been entered");
            }
            if (double.IsNaN(m_step))
            {
                throw new ArgumentException("No step size has been entered");
            }

            double[] k1 = new double[m_nODE];
            double[] k2 = new double[m_nODE];
            double[] k3 = new double[m_nODE];
            double[] k4 = new double[m_nODE];
            double[] y = new double[m_nODE];
            double[] yd = new double[m_nODE];
            double[] dydx = new double[m_nODE];
            double x = 0.0D;

            // Calculate nsteps
            double ns = (m_xn - m_x0)/m_step;
            ns = Converter.rint(ns);
            int nsteps = (int) ns;
            m_nIter = nsteps;
            double stepUsed = (m_xn - m_x0)/ns;

            // initialise
            for (int i = 0; i < m_nODE; i++)
            {
                y[i] = m_yy0[i];
            }

            // iteration over allowed steps
            for (int j = 0; j < nsteps; j++)
            {
                x = m_x0 + j*stepUsed;
                dydx = g.derivn(x, y);
                for (int i = 0; i < m_nODE; i++)
                {
                    k1[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + k1[i]/2;
                }
                dydx = g.derivn(x + stepUsed/2, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k2[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + k2[i]/2;
                }
                dydx = g.derivn(x + stepUsed/2, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k3[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + k3[i];
                }
                dydx = g.derivn(x + stepUsed, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k4[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    y[i] += k1[i]/6 + k2[i]/3 + k3[i]/3 + k4[i]/6;
                }
            }
            return y;
        }

        // Fourth order Runge-Kutta for n ordinary differential equations (ODE)
        // Static method
        public static double[] fourthOrder(DerivnFunction g, double x0, double[] y0, double xn, double h)
        {
            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValuesOfY(y0);
            rk.setStepSize(h);

            return rk.fourthOrder(g);
        }


        // Runge-Kutta-Cash-Karp for a single ordinary differential equation (ODE)
        // Non-static method
        public double cashKarp(DerivFunction g)
        {
            if (double.IsNaN(m_x0))
            {
                throw new ArgumentException("No initial x value has been entered");
            }
            if (double.IsNaN(m_xn))
            {
                throw new ArgumentException("No  x value has been entered");
            }
            if (double.IsNaN(m_y0))
            {
                throw new ArgumentException("No initial y value has been entered");
            }
            if (double.IsNaN(m_step))
            {
                throw new ArgumentException("No step size has been entered");
            }

            double k1 = 0.0D, k2 = 0.0D, k3 = 0.0D, k4 = 0.0D, k5 = 0.0D, k6 = 0.0D;
            double y = m_y0, y5 = 0.0D, y6 = 0.0D, yd = 0.0D, dydx = 0.0D;
            double x = m_x0, err = 0.0D, delta = 0.0D, tol = 0.0D;
            int counter = 0;

            if (m_maxIter == -1)
            {
                m_maxIter = (int) (m_nStepsMultiplier*(m_xn - m_x0)/m_step);
            }
            double stepUsed = m_step;


            while (x < m_xn)
            {
                counter++;
                if (counter > m_maxIter)
                {
                    throw new ArithmeticException("Maximum number of iterations exceeded");
                }
                dydx = g.deriv(x, y);
                k1 = stepUsed*dydx;

                yd = y + k1/5.0;
                dydx = g.deriv(x + stepUsed/5.0, yd);
                k2 = stepUsed*dydx;

                yd = y + (3.0*k1 + 9.0*k2)/40.0;
                dydx = g.deriv(x + 3.0*stepUsed/10.0, yd);
                k3 = stepUsed*dydx;

                yd = y + (3.0*k1 - 9.0*k2 + 12.0*k3)/10.0;
                dydx = g.deriv(x + 3.0*stepUsed/5.0, yd);
                k4 = stepUsed*dydx;

                yd = y - 11.0*k1/54.0 + 5.0*k2/2.0 - 70.0*k3/27.0 + 35.0*k4/27.0;
                dydx = g.deriv(x + stepUsed, yd);
                k5 = stepUsed*dydx;

                yd = y + 1631.0*k1/55296.0 + 175.0*k2/512.0 + 575.0*k3/13824.0 + 44275.0*k4/110592.0 + 253.0*k5/4096.0;
                dydx = g.deriv(x + 7.0*stepUsed/8.0, yd);
                k6 = stepUsed*dydx;

                y5 = y + 2825.0*k1/27648.0 + 18575.0*k3/48384.0 + 13525.0*k4/55296.0 + 277.0*k5/14336.0 + k6/4.0;
                y6 = y + 37*k1/378.0 + 250.0*k3/621.0 + 125.0*k4/594.0 + 512.0*k6/1771.0;
                err = Math.Abs(y6 - y5);
                tol = err/(Math.Abs(y5)*m_relTol + m_absTol);
                if (tol <= 1.0)
                {
                    x += stepUsed;
                    delta = m_safetyFactor*Math.Pow(tol, m_incrementFactor);
                    if (delta > 4.0)
                    {
                        stepUsed *= 4.0;
                    }
                    else if (delta > 1.0)
                    {
                        stepUsed *= delta;
                    }
                    if (x + stepUsed > m_xn)
                    {
                        stepUsed = m_xn - x;
                    }
                    y = y5;
                }
                else
                {
                    delta = m_safetyFactor*Math.Pow(tol, m_decrementFactor);
                    if (delta < 0.1)
                    {
                        stepUsed *= 0.1;
                    }
                    else
                    {
                        stepUsed *= delta;
                    }
                }
            }
            m_nIter = counter;
            return y;
        }

        // Runge-Kutta-Cash-Karp for a single ordinary differential equation (ODE)
        // Static method
        public static double cashKarp(DerivFunction g, double x0, double y0, double xn, double h, double absTol,
                                      double relTol, int maxIter)
        {
            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValueOfY(y0);
            rk.setStepSize(h);
            rk.setToleranceScalingFactor(relTol);
            rk.setToleranceAdditionFactor(absTol);
            rk.setMaximumIterations(maxIter);

            return rk.cashKarp(g);
        }

        // Runge-Kutta-Cash-Karp for a single ordinary differential equation (ODE)
        // Static method
        // maximum iteration default option
        public static double cashKarp(DerivFunction g, double x0, double y0, double xn, double h, double absTol,
                                      double relTol)
        {
            int maxIter = (int) (m_nStepsMultiplier*(xn - x0)/h);

            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValueOfY(y0);
            rk.setStepSize(h);
            rk.setToleranceScalingFactor(relTol);
            rk.setToleranceAdditionFactor(absTol);
            rk.setMaximumIterations(maxIter);

            return rk.cashKarp(g);
        }

        // Runge-Kutta-Cash-Karp for n ordinary differential equations (ODEs)
        // Non-static method
        public double[] cashKarp(DerivnFunction g)
        {
            if (double.IsNaN(m_x0))
            {
                throw new ArgumentException("No initial x value has been entered");
            }
            if (double.IsNaN(m_xn))
            {
                throw new ArgumentException("No  x value has been entered");
            }
            if (m_yy0 == null)
            {
                throw new ArgumentException("No initial y values have been entered");
            }
            if (double.IsNaN(m_step))
            {
                throw new ArgumentException("No step size has been entered");
            }

            double[] k1 = new double[m_nODE];
            double[] k2 = new double[m_nODE];
            double[] k3 = new double[m_nODE];
            double[] k4 = new double[m_nODE];
            double[] k5 = new double[m_nODE];
            double[] k6 = new double[m_nODE];
            double[] y = new double[m_nODE];
            double[] y6 = new double[m_nODE];
            double[] y5 = new double[m_nODE];
            double[] yd = new double[m_nODE];
            double[] dydx = new double[m_nODE];

            double err = 0.0D, maxerr = 0.0D, delta = 0.0D, tol = 1.0D;
            int counter = 0;

            // initialise
            for (int i = 0; i < m_nODE; i++)
            {
                y[i] = m_yy0[i];
            }
            double x = m_x0;
            if (m_maxIter == -1)
            {
                m_maxIter = (int) (m_nStepsMultiplier*(m_xn - m_x0)/m_step);
            }
            double stepUsed = m_step;

            while (x < m_xn)
            {
                counter++;
                if (counter > m_maxIter)
                {
                    throw new ArithmeticException("Maximum number of iterations exceeded");
                }

                dydx = g.derivn(x, y);
                for (int i = 0; i < m_nODE; i++)
                {
                    k1[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + k1[i]/5.0;
                }
                dydx = g.derivn(x + stepUsed/5.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k2[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + (3.0*k1[i] + 9.0*k2[i])/40.0;
                }
                dydx = g.derivn(x + 3.0*stepUsed/10.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k3[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + (3.0*k1[i] - 9.0*k2[i] + 12.0*k3[i])/10.0;
                }
                dydx = g.derivn(x + 3.0*stepUsed/5.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k4[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] - 11.0*k1[i]/54.0 + 5.0*k2[i]/2.0 - 70.0*k3[i]/27.0 + 35.0*k4[i]/27.0;
                }
                dydx = g.derivn(x + stepUsed, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k5[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + 1631.0*k1[i]/55296.0 + 175.0*k2[i]/512.0 + 575.0*k3[i]/13824.0 +
                            44275.0*k4[i]/110592.0 + 253.0*k5[i]/4096.0;
                }
                dydx = g.derivn(x + 7.0*stepUsed/8.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k6[i] = stepUsed*dydx[i];
                }

                maxerr = 0.0D;
                for (int i = 0; i < m_nODE; i++)
                {
                    y5[i] = y[i] + 2825.0*k1[i]/27648.0 + 18575.0*k3[i]/48384.0 + 13525.0*k4[i]/55296.0 +
                            277.0*k5[i]/14336.0 + k6[i]/4.0;
                    y6[i] = y[i] + 37*k1[i]/378.0 + 250.0*k3[i]/621.0 + 125.0*k4[i]/594.0 + 512.0*k6[i]/1771.0;
                    err = Math.Abs(y6[i] - y5[i]);
                    tol = Math.Abs(y5[i])*m_relTol + m_absTol;
                    maxerr = Math.Max(maxerr, err/tol);
                }
                if (maxerr <= 1.0D)
                {
                    x += stepUsed;
                    delta = m_safetyFactor*Math.Pow(maxerr, m_incrementFactor);
                    if (delta > 4.0)
                    {
                        stepUsed *= 4.0;
                    }
                    else if (delta > 1.0)
                    {
                        stepUsed *= delta;
                    }
                    if (x + stepUsed > m_xn)
                    {
                        stepUsed = m_xn - x;
                    }
                    y = (double[]) y5.Clone();
                }
                else
                {
                    delta = m_safetyFactor*Math.Pow(maxerr, m_decrementFactor);
                    if (delta < 0.1D)
                    {
                        stepUsed *= 0.1;
                    }
                    else
                    {
                        stepUsed *= delta;
                    }
                }
            }
            m_nIter = counter;
            return y;
        }

        // Runge-Kutta-Cash-Karp for n ordinary differential equations (ODEs)
        // Static method
        public static double[] cashKarp(DerivnFunction g, double x0, double[] y0, double xn, double h, double absTol,
                                        double relTol, int maxIter)
        {
            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValuesOfY(y0);
            rk.setStepSize(h);
            rk.setToleranceScalingFactor(relTol);
            rk.setToleranceAdditionFactor(absTol);
            rk.setMaximumIterations(maxIter);

            return rk.cashKarp(g);
        }

        public static double[] cashKarp(DerivnFunction g, double x0, double[] y0, double xn, double h, double absTol,
                                        double relTol)
        {
            double nsteps = (xn - x0)/h;
            int maxIter = (int) nsteps*m_nStepsMultiplier;

            return cashKarp(g, x0, y0, xn, h, absTol, relTol, maxIter);
        }

        // Runge-Kutta-Fehlberg for a single ordinary differential equation (ODE)
        // Non-static method
        public double fehlberg(DerivFunction g)
        {
            if (double.IsNaN(m_x0))
            {
                throw new ArgumentException("No initial x value has been entered");
            }
            if (double.IsNaN(m_xn))
            {
                throw new ArgumentException("No  x value has been entered");
            }
            if (double.IsNaN(m_y0))
            {
                throw new ArgumentException("No initial y value has been entered");
            }
            if (double.IsNaN(m_step))
            {
                throw new ArgumentException("No step size has been entered");
            }

            double k1 = 0.0D, k2 = 0.0D, k3 = 0.0D, k4 = 0.0D, k5 = 0.0D, k6 = 0.0D;
            double x = m_x0, y = m_y0, y5 = 0.0D, y6 = 0.0D, err = 0.0D, delta = 0.0D, tol = 0.0D;
            int counter = 0;

            if (m_maxIter == -1)
            {
                m_maxIter = (int) (m_nStepsMultiplier*(m_xn - m_x0)/m_step);
            }
            double stepUsed = m_step;

            while (x < m_xn)
            {
                counter++;
                if (counter > m_maxIter)
                {
                    throw new ArithmeticException("Maximum number of iterations exceeded");
                }
                k1 = stepUsed*g.deriv(x, y);
                k2 = stepUsed*g.deriv(x + stepUsed/4.0, y + k1/4.0);
                k3 = stepUsed*g.deriv(x + 3.0*stepUsed/8.0, y + (3.0*k1 + 9.0*k2)/32.0);
                k4 = stepUsed*g.deriv(x + 12.0*stepUsed/13.0, y + (1932.0*k1 - 7200.0*k2 + 7296.0*k3)/2197.0);
                k5 = stepUsed*g.deriv(x + stepUsed, y + 439.0*k1/216.0 - 8.0*k2 + 3680.0*k3/513.0 - 845*k4/4104.0);
                k6 = stepUsed*
                     g.deriv(x + 0.5*stepUsed,
                             y - 8.0*k1/27.0 + 2.0*k2 - 3544.0*k3/2565.0 + 1859.0*k4/4104.0 - 11.0*k5/40.0);

                y5 = y + 25.0*k1/216.0 + 1408.0*k3/2565.0 + 2197.0*k4/4104.0 - k5/5.0;
                y6 = y + 16.0*k1/135.0 + 6656.0*k3/12825.0 + 28561.0*k4/56430.0 - 9.0*k5/50.0 + 2.0*k6/55.0;
                err = Math.Abs(y6 - y5);
                tol = err/(Math.Abs(y5)*m_relTol + m_absTol);
                if (tol <= 1.0)
                {
                    x += stepUsed;
                    delta = m_safetyFactor*Math.Pow(tol, m_incrementFactor);
                    if (delta > 4.0)
                    {
                        stepUsed *= 4.0;
                    }
                    else if (delta < 1.0)
                    {
                        stepUsed *= delta;
                    }
                    if (x + stepUsed > m_xn)
                    {
                        stepUsed = m_xn - x;
                    }
                    y = y5;
                }
                else
                {
                    delta = m_safetyFactor*Math.Pow(tol, m_decrementFactor);
                    if (delta < 0.1)
                    {
                        stepUsed *= 0.1;
                    }
                    else
                    {
                        stepUsed *= delta;
                    }
                }
            }
            m_nIter = counter;
            return y;
        }

        // Runge-Kutta-Fehlberg for a single ordinary differential equation (ODE)
        // Static method
        public static double fehlberg(DerivFunction g, double x0, double y0, double xn, double h, double absTol,
                                      double relTol, int maxIter)
        {
            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValueOfY(y0);
            rk.setStepSize(h);
            rk.setToleranceScalingFactor(relTol);
            rk.setToleranceAdditionFactor(absTol);
            rk.setMaximumIterations(maxIter);

            return rk.fehlberg(g);
        }

        // Runge-Kutta-Fehlberg for a single ordinary differential equation (ODE)
        // Maximum iteration default option
        // Static method
        public static double fehlberg(DerivFunction g, double x0, double y0, double xn, double h, double absTol,
                                      double relTol)
        {
            double nsteps = (xn - x0)/h;
            int maxIter = (int) nsteps*m_nStepsMultiplier;

            return fehlberg(g, x0, y0, xn, h, absTol, relTol, maxIter);
        }

        // Runge-Kutta-Fehlberg for n ordinary differential equations (ODEs)
        // Non-static method
        public double[] fehlberg(DerivnFunction g)
        {
            if (double.IsNaN(m_x0))
            {
                throw new ArgumentException("No initial x value has been entered");
            }
            if (double.IsNaN(m_xn))
            {
                throw new ArgumentException("No  x value has been entered");
            }
            if (m_yy0 == null)
            {
                throw new ArgumentException("No initial y values have been entered");
            }
            if (double.IsNaN(m_step))
            {
                throw new ArgumentException("No step size has been entered");
            }

            double[] k1 = new double[m_nODE];
            double[] k2 = new double[m_nODE];
            double[] k3 = new double[m_nODE];
            double[] k4 = new double[m_nODE];
            double[] k5 = new double[m_nODE];
            double[] k6 = new double[m_nODE];
            double[] y = new double[m_nODE];
            double[] y6 = new double[m_nODE];
            double[] y5 = new double[m_nODE];
            double[] yd = new double[m_nODE];
            double[] dydx = new double[m_nODE];

            double err = 0.0D, maxerr = 0.0D, delta = 0.0D, tol = 1.0D;
            int counter = 0;

            // initialise
            for (int i = 0; i < m_nODE; i++)
            {
                y[i] = m_yy0[i];
            }
            double x = m_x0;
            if (m_maxIter == -1)
            {
                m_maxIter = (int) (m_nStepsMultiplier*(m_xn - m_x0)/m_step);
            }
            double stepUsed = m_step;

            while (x < m_xn)
            {
                counter++;
                if (counter > m_maxIter)
                {
                    throw new ArithmeticException("Maximum number of iterations exceeded");
                }
                dydx = g.derivn(x, y);
                for (int i = 0; i < m_nODE; i++)
                {
                    k1[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + k1[i]/4.0;
                }
                dydx = g.derivn(x + stepUsed/4.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k2[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + (3.0*k1[i] + 9.0*k2[i])/32.0;
                }
                dydx = g.derivn(x + 3.0*stepUsed/8.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k3[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + (1932.0*k1[i] - 7200.0*k2[i] + 7296.0*k3[i])/2197.0;
                }
                dydx = g.derivn(x + 12.0*stepUsed/13.0, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k4[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] + 439.0*k1[i]/216.0 - 8.0*k2[i] + 3680.0*k3[i]/513.0 - 845*k4[i]/4104.0;
                }
                dydx = g.derivn(x + stepUsed, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k5[i] = stepUsed*dydx[i];
                }

                for (int i = 0; i < m_nODE; i++)
                {
                    yd[i] = y[i] - 8.0*k1[i]/27.0 + 2.0*k2[i] - 3544.0*k3[i]/2565.0 + 1859.0*k4[i]/4104.0 -
                            11.0*k5[i]/40.0;
                }
                dydx = g.derivn(x + 0.5*stepUsed, yd);
                for (int i = 0; i < m_nODE; i++)
                {
                    k6[i] = stepUsed*dydx[i];
                }

                maxerr = 0.0D;
                for (int i = 0; i < m_nODE; i++)
                {
                    y5[i] = y[i] + 25.0*k1[i]/216.0 + 1408.0*k3[i]/2565.0 + 2197.0*k4[i]/4104.0 - k5[i]/5.0;
                    y6[i] = y[i] + 16.0*k1[i]/135.0 + 6656.0*k3[i]/12825.0 + 28561.0*k4[i]/56430.0 - 9.0*k5[i]/50.0 +
                            2.0*k6[i]/55.0;
                    err = Math.Abs(y6[i] - y5[i]);
                    tol = y5[i]*m_relTol + m_absTol;
                    maxerr = Math.Max(maxerr, err/tol);
                }

                if (maxerr <= 1.0D)
                {
                    x += stepUsed;
                    delta = m_safetyFactor*Math.Pow(maxerr, m_incrementFactor);
                    if (delta > 4.0)
                    {
                        stepUsed *= 4.0;
                    }
                    else if (delta > 1.0)
                    {
                        stepUsed *= delta;
                    }
                    if (x + stepUsed > m_xn)
                    {
                        stepUsed = m_xn - x;
                    }
                    y = (double[]) y5.Clone();
                }
                else
                {
                    delta = m_safetyFactor*Math.Pow(maxerr, m_decrementFactor);
                    if (delta < 0.1)
                    {
                        stepUsed *= 0.1;
                    }
                    else
                    {
                        stepUsed *= delta;
                    }
                }
            }
            m_nIter = counter;
            return y;
        }

        // Runge-Kutta-Fehlberg for n (nODE) ordinary differential equations (ODEs)
        // Static method
        public static double[] fehlberg(DerivnFunction g, double x0, double[] y0, double xn, double h, double absTol,
                                        double relTol, int maxIter)
        {
            RungeKutta rk = new RungeKutta();
            rk.setInitialValueOfX(x0);
            rk.setFinalValueOfX(xn);
            rk.setInitialValuesOfY(y0);
            rk.setStepSize(h);
            rk.setToleranceScalingFactor(relTol);
            rk.setToleranceAdditionFactor(absTol);
            rk.setMaximumIterations(maxIter);

            return rk.fehlberg(g);
        }

        // Runge-Kutta-Fehlberg for n (nODE) ordinary differential equations (ODEs)
        // Static method
        // maximum iteration default option
        public static double[] fehlberg(DerivnFunction g, double x0, double[] y0, double xn, double h, double absTol,
                                        double relTol)
        {
            double nsteps = (xn - x0)/h;
            int maxIter = (int) nsteps*m_nStepsMultiplier;

            return fehlberg(g, x0, y0, xn, h, absTol, relTol, maxIter);
        }
    }
}
