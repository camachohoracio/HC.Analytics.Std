#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Functions.Beta;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Probability.Distributions.Continuous;
using HC.Analytics.Statistics;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Io;
using Converter = HC.Analytics.ConvertClasses.Converter;

#endregion

namespace HC.Analytics.Analysis
{
    /*
    *   Class Regression
    *
    *   Contains methods for simple linear regression
    *   (straight line), for multiple linear regression,
    *   for fitting data to a polynomial and for non-linear
    *   regression (Nelder and Mead Simplex method) for both user
    *   supplied functions and for a wide range of standard functios
    *
    *   The sum of squares function needed by the non-linear regression methods
    *   non-linear regression methods is supplied by means of the interfaces,
    *   RegressionFunction or RegressionFunction2
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:	    February 2002
    *   MODIFIED:   7 January 2006,  28 July 2006, 9 August 2006, 4 November 2006
    *               21 November 2006, 21 December 2006, 14 April 2007, 9 June 2007,
    *               25 July 2007, 23/24 August 2007, 14 September 2007, 28 December 2007
    *               18-26 March 2008, 7 April 2008, 27 April 2008, 10/12/19 May 2008,
    *               5-6 July 2004, 28 July 2008, 29 August 2008, 5 September 2008,
    *               6 October 2008, 13-15 October 2009, 13 November 2009, 10 December 2009
    *               20 December 2009, 12 January 2010
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web page:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/Regression.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    * Copyright (c) 2002 - 2009 Michael Thomas Flanagan
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

    // Regression class
    public class RegressionAn
    {
        protected static double m_histTol = 1.0001D;
        protected double[] bestSd;
        protected double m_adjustedF = double.NaN; // Adjusted Multiple correlation coefficient F ratio
        protected double m_adjustedR = double.NaN; // adjusted coefficient of determination
        protected double m_adjustedR2 = double.NaN; // adjustedR2 = adjustedR*adjustedR
        protected double[] m_best; // best estimates vector of the unknown parameters

        // standard deviation estimates of the best estimates of the unknown parameters

        protected bool m_blnIgnoreDofFcheck;
        // when set to true, the check on whether degrees of freedom are greater than zero is ignored

        protected bool m_blnSupressErrorMessages; // = true if some designated error messages are to be supressed
        protected bool m_blnSupressPrint; // = true if print results is to be supressed
        protected bool m_blnSupressYYplot; // = true if plot of experimental versus calculated is to be supressed
        protected double m_cCoeff = 0.5D; // Nelder and Mead simplex contraction coefficient

        protected double m_chiSquare = double.NaN; // chi  square (observed-calculated)^2/variance
        protected int m_constraintMethod; // constraint method number
        protected double[] m_constraints; // single parameter constraint values
        protected double m_constraintTolerance = 1e-4; // tolerance in constraining parameter/s to a fixed_ value
        protected double[,] m_corrCoeff; // Correlation coefficient matrix
        protected double[,] m_covar; // Covariance matrix
        protected int m_degreesOfFreedom; // degrees of freedom = nData - nTerms
        protected double m_delta = 1e-4; // Fractional step in numerical differentiation
        protected double m_dStep = 0.5D; // Nelder and Mead simplex default step value
        protected double m_eCoeff = 2.0D; // Nelder and Mead simplex extension coefficient
        protected int m_field = 13; // field width on output to text files
        protected bool[] m_fixed_; // true if above values[i] is fixed_, false if it is not
        protected double m_fMin = -1.0D; // Nelder and Mead simplex minimum value

        protected bool m_frechetWeibull = true; // Frechet Weibull switch - if true Frechet, if false Weibull
        protected double m_fTol = 1e-9; // Nelder and Mead simplex convergence tolerance
        protected double[,] m_grad; // Non-linear regression gradients
        protected string m_graphTitle = " "; // user supplied graph title
        protected string m_graphTitle2 = " "; // second line graph title
        protected bool m_invertFlag = true; // Hessian Matrix_non_stable ('linear' non-linear statistics) check
        protected double m_kayValue; // rate parameter value in Erlang distribution (method 35)
        protected int m_konvge = 3; // Nelder and Mead simplex number of restarts allowed
        protected int m_kRestart; // Nelder and Mead simplex number of restarts taken
        protected int m_lastMethod = -1; // code indicating the last regression procedure attempted

        protected double m_lastSSnoConstraint;
        // Last sum of the squares of the residuals with no constraint penalty

        protected bool m_legendCheck; // = true if above legends overwritten by user supplied legends
        protected bool m_linNonLin = true; // if true linear method, if false non-linear method
        protected int m_maxConstraintIndex = -1; // maximum index of constrained parameter/s
        protected int m_minTest; // Nelder and Mead minimum test
        protected double m_multipleF = double.NaN; // Multiple correlation coefficient F ratio

        protected bool m_multipleY;
        // = true if y variable consists of more than set of data each needing a different calculation in RegressionFunction

        protected int m_nConstraints; // number of single parameter constraints
        protected int m_nData; // number of y data points (nData0 times the number of y arrays)
        protected int m_nData0; // number of y data points inputted (in a single array if multiple y arrays)
        protected bool m_nFactorOption; // = true  varaiance, covariance and standard deviation denominator = n
        protected int m_nIter; // Nelder and Mead simplex number of iterations performed
        protected bool m_nlrStatus = true; // Status of non-linear regression on exiting regression method
        protected int m_nMax = 3000; // Nelder and Mead simplex maximum number of iterations
        protected int m_nSumConstraints; // number of multiple parameter constraints
        protected int m_nTerms; // number of unknown parameters to be estimated
        protected int m_nXarrays = 1; // number of x arrays
        protected int m_nYarrays = 1; // number of y arrays
        protected string[] m_paraName; // names of parameters, eg, m_mean, m_sd; c[0], c[1], c[2] . . .
        protected List<Object> m_penalties = new List<Object>(); // 3 method index,
        protected bool m_penalty; // true if single parameter penalty function is included
        // number of single parameter constraints,
        // then repeated for each constraint:
        //  penalty parameter index,
        //  below or above constraint flag,
        //  constraint boundary value

        // number of multiple parameter constraints,
        // then repeated for each constraint:
        //  number of parameters in summation
        //  penalty parameter indices,
        //  summation signs
        //  below or above constraint flag,
        //  constraint boundary value
        protected int[] m_penaltyCheck; // = -1 values below the single constraint boundary not allowed
        // = +1 values above the single constraint boundary not allowed
        protected int[] m_penaltyParam; // indices of paramaters subject to single parameter constraint
        protected double m_penaltyWeight = 1.0e30; // weight for the penalty functions

        protected bool m_plotOpt = true;
        // if true - plot of calculated values is cubic spline interpolation between the calculated values

        protected bool m_posVarFlag = true; // Hessian Matrix_non_stable ('linear' non-linear statistics) check
        protected int m_prec = 4; // number of places to which double variables are truncated on output to text files
        protected double[] m_pseudoSd; // Pseudo-nonlinear m_sd
        protected double[] m_pValues; // p-values of the best estimates

        protected double m_rCoeff = 1.0D; // Nelder and Mead simplex reflection coefficient
        protected double m_reducedChiSquare = double.NaN; // reduced chi square
        protected double[] m_residual; // residuals
        protected double[] m_residualW; // weighted residuals
        protected double m_sampleR = double.NaN; // coefficient of determination
        protected double m_sampleR2 = double.NaN; // sampleR2 = sampleR*sampleR
        protected double[] m_scale; // values to scale initial estimate (see scaleOpt above)

        protected bool m_scaleFlag = true;
        // if true ordinate scale factor, Ao, included as unknown in fitting to special functions

        protected int m_scaleOpt; //  if = 0; no scaling of initial estimates
        protected double m_simplexSd; // simplex standard deviation

        protected double[] m_startH; // Nelder and Mead simplex unscaled initial estimates
        protected double[] m_startSH; // Nelder and Mead simplex scaled initial estimates
        protected bool m_statFlag = true; // if true - statistical method called
        protected double[] m_stepH; // Nelder and Mead simplex unscaled initial step values
        protected double[] m_stepSH; // Nelder and Mead simplex scaled initial step values
        protected double[] m_sumConstraints; // multiple parameter constraint values
        protected double m_sumOfSquares = double.NaN; // Sum of the squares of the residuals

        protected List<Object> m_sumPenalties =
            new List<Object>(); // constraint method index,

        protected bool m_sumPenalty; // true if multiple parameter penalty function is included
        protected int[] m_sumPenaltyCheck; // = -1 values below the multiple constraint boundary not allowed
        protected int[] m_sumPenaltyNumber; // number of paramaters in each multiple parameter constraint
        protected int[,] m_sumPenaltyParam; // indices of paramaters subject to multiple parameter constraint
        protected double[,] m_sumPlusOrMinus; // valueall before each parameter in multiple parameter summation
        protected bool m_trueFreq; // true if xData values are true frequencies, e.g. in a fit to Gaussian
        protected double[] m_tValues; // t-values of the best estimates
        protected bool m_userSupplied = true; // = true  - user supplies the initial estimates for non-linear regression

        // if false - no statistical analysis

        // when set to true - the index of the y value is passed to the function in Regression function

        protected double[] m_values; // values entered into gaussianfixed_
        protected double[] m_weight; // weighting factors

        protected int m_weightFlag;
        // weighting flag  - weightOpt = false, weightFlag = 0;  weightOpt = true, weightFlag = 1

        protected bool m_weightOpt; // weighting factor option
        protected string[] m_weightWord = {"", "Weighted "};
        protected double[,] m_xData; // x  data values
        protected string m_xLegend = "x axis values"; // x axis legend in X-Y plot
        protected double[] m_yCalc; // calculated y values using the regrssion coefficients
        protected double[] m_yData; // y  data values
        protected string m_yLegend = "y axis values"; // y axis legend in X-Y plot
        protected double m_yScaleFactor = 1.0D; // y axis factor - set if scaleFlag (above) = false
        protected bool m_zeroCheck; // true if any best estimate value is zero

        //CONSRUCTORS

        // Default constructor - primarily facilitating the subclass ImpedSpecRegression
        public RegressionAn()
        {
        }


        // Constructor with data with x as 2D array and weights provided
        public RegressionAn(double[,] xData, double[] yData, double[] weight)
        {
            int n = weight.Length;
            m_nData0 = yData.Length;
            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x and y as 2D arrays and weights provided
        public RegressionAn(double[,] xxData, double[,] yyData, double[,] wWeight)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            int nX1 = xxData.GetLength(0);
            int nX2 = xxData.GetLength(1);
            double[] yData = new double[nY1*nY2];
            double[] weight = new double[nY1*nY2];
            double[,] xData = new double[nY1*nY2,nX1];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                int nX = xxData.GetLength(1);
                if (nY != nX)
                {
                    throw new HCException("multiple y arrays must be of the same Length as the x array Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    xData[ii, i] = xxData[i, j];
                    weight[ii] = wWeight[i, j];
                    ii++;
                }
            }
            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x as 1D array and weights provided
        public RegressionAn(double[] xxData, double[] yData, double[] weight)
        {
            m_nData0 = yData.Length;
            int n = xxData.Length;
            int m = weight.Length;
            double[,] xData = new double[1,n];
            for (int i = 0; i < n; i++)
            {
                xData[0, i] = xxData[i];
            }

            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x as 1D array and y as 2D array and weights provided
        public RegressionAn(double[] xxData, double[,] yyData, double[,] wWeight)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            double[] yData = new double[nY1*nY2];
            double[] weight = new double[nY1*nY2];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    weight[ii] = wWeight[i, j];
                    ii++;
                }
            }
            int n = xxData.Length;
            if (n != nY2)
            {
                throw new HCException("x and y data lengths must be the same");
            }
            double[,] xData = new double[1,nY1*n];
            ii = 0;
            for (int j = 0; j < nY1; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    xData[0, ii] = xxData[i];
                    ii++;
                }
            }

            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x as 2D array and no weights provided
        public RegressionAn(double[,] xData, double[] yData)
        {
            m_nData0 = yData.Length;
            int n = yData.Length;
            double[] weight = new double[n];

            m_weightOpt = false;
            m_weightFlag = 0;
            for (int i = 0; i < n; i++)
            {
                weight[i] = 1.0D;
            }

            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x and y as 2D arrays and no weights provided
        public RegressionAn(double[,] xxData, double[,] yyData)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            int nX1 = xxData.GetLength(0);
            int nX2 = xxData.GetLength(1);
            double[] yData = new double[nY1*nY2];
            if (nY1 != nX1)
            {
                throw new HCException("Multiple xData and yData arrays of different overall dimensions not supported");
            }
            double[,] xData = new double[1,nY1*nY2];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                int nX = xxData.GetLength(1);
                if (nY != nX)
                {
                    throw new HCException("multiple y arrays must be of the same Length as the x array Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    xData[0, ii] = xxData[i, j];
                    ii++;
                }
            }

            int n = yData.Length;
            double[] weight = new double[n];

            m_weightOpt = false;
            for (int i = 0; i < n; i++)
            {
                weight[i] = 1.0D;
            }
            m_weightFlag = 0;

            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x as 1D array and no weights provided
        public RegressionAn(double[] xxData, double[] yData)
        {
            m_nData0 = yData.Length;
            int n = xxData.Length;
            double[,] xData = new double[1,n];
            double[] weight = new double[n];

            for (int i = 0; i < n; i++)
            {
                xData[0, i] = xxData[i];
            }

            m_weightOpt = false;
            m_weightFlag = 0;
            for (int i = 0; i < n; i++)
            {
                weight[i] = 1.0D;
            }

            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data with x as 1D array and y as a 2D array and no weights provided
        public RegressionAn(double[] xxData, double[,] yyData)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            double[] yData = new double[nY1*nY2];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    ii++;
                }
            }

            double[,] xData = new double[1,nY1*nY2];
            double[] weight = new double[nY1*nY2];

            ii = 0;
            int n = xxData.Length;
            for (int j = 0; j < nY1; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    xData[0, ii] = xxData[i];
                    weight[ii] = 1.0D;
                    ii++;
                }
            }
            m_weightOpt = false;
            m_weightFlag = 0;

            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data as a single array that has to be binned
        // bin width and value of the low point of the first bin provided
        public RegressionAn(double[] xxData, double binWidth, double binZero)
        {
            double[,] data = histogramBins(xxData, binWidth, binZero);
            int n = data.GetLength(1);
            m_nData0 = n;
            double[,] xData = new double[1,n];
            double[] yData = new double[n];
            double[] weight = new double[n];
            for (int i = 0; i < n; i++)
            {
                xData[0, i] = data[0, i];
                yData[i] = data[1, i];
            }
            bool flag = setTrueFreqWeights(yData, weight);
            if (flag)
            {
                m_trueFreq = true;
                m_weightOpt = true;
                m_weightFlag = 1;
            }
            else
            {
                m_trueFreq = false;
                m_weightOpt = false;
                m_weightFlag = 0;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Constructor with data as a single array that has to be binned
        // bin width provided
        public RegressionAn(double[] xxData, double binWidth)
        {
            double[,] data = histogramBins(xxData, binWidth);
            int n = data.GetLength(1);
            m_nData0 = n;
            double[,] xData = new double[1,n];
            double[] yData = new double[n];
            double[] weight = new double[n];
            for (int i = 0; i < n; i++)
            {
                xData[0, i] = data[0, i];
                yData[i] = data[1, i];
            }
            bool flag = setTrueFreqWeights(yData, weight);
            if (flag)
            {
                m_trueFreq = true;
                m_weightOpt = true;
                m_weightFlag = 1;
            }
            else
            {
                m_trueFreq = false;
                m_weightOpt = false;
                m_weightFlag = 0;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Check entered weights for zeros.
        // If more than 40% are zero or less than zero, all weights replaced by unity
        // If less than 40% are zero or less than zero, the zero or negative weights are replaced by the average of their nearest neighbours
        protected double[] checkForZeroWeights(double[] weight)
        {
            m_weightOpt = true;
            int nZeros = 0;
            int n = weight.Length;

            for (int i = 0; i < n; i++)
            {
                if (weight[i] <= 0.0)
                {
                    nZeros++;
                }
            }
            double perCentZeros = 100.0*nZeros/n;
            if (perCentZeros > 40.0)
            {
                PrintToScreen.WriteLine(perCentZeros + "% of the weights are zero or less; all weights set to 1.0");
                for (int i = 0; i < n; i++)
                {
                    weight[i] = 1.0D;
                }
                m_weightOpt = false;
            }
            else
            {
                if (perCentZeros > 0.0D)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (weight[i] <= 0.0)
                        {
                            if (i == 0)
                            {
                                int ii = 1;
                                bool test = true;
                                while (test)
                                {
                                    if (weight[ii] > 0.0D)
                                    {
                                        double ww = weight[0];
                                        weight[0] = weight[ii];
                                        PrintToScreen.WriteLine("weight at point " + i + ", " + ww + ", replaced by " +
                                                                weight[i]);
                                        test = false;
                                    }
                                    else
                                    {
                                        ii++;
                                    }
                                }
                            }
                            if (i == (n - 1))
                            {
                                int ii = n - 2;
                                bool test = true;
                                while (test)
                                {
                                    if (weight[ii] > 0.0D)
                                    {
                                        double ww = weight[i];
                                        weight[i] = weight[ii];
                                        PrintToScreen.WriteLine("weight at point " + i + ", " + ww + ", replaced by " +
                                                                weight[i]);
                                        test = false;
                                    }
                                    else
                                    {
                                        ii--;
                                    }
                                }
                            }
                            if (i > 0 && i < (n - 2))
                            {
                                double lower = 0.0;
                                double upper = 0.0;
                                int ii = i - 1;
                                bool test = true;
                                while (test)
                                {
                                    if (weight[ii] > 0.0D)
                                    {
                                        lower = weight[ii];
                                        test = false;
                                    }
                                    else
                                    {
                                        ii--;
                                        if (ii == 0)
                                        {
                                            test = false;
                                        }
                                    }
                                }
                                ii = i + 1;
                                test = true;
                                while (test)
                                {
                                    if (weight[ii] > 0.0D)
                                    {
                                        upper = weight[ii];
                                        test = false;
                                    }
                                    else
                                    {
                                        ii++;
                                        if (ii == (n - 1))
                                        {
                                            test = false;
                                        }
                                    }
                                }
                                double ww = weight[i];
                                if (lower == 0.0)
                                {
                                    weight[i] = upper;
                                }
                                else
                                {
                                    if (upper == 0.0)
                                    {
                                        weight[i] = lower;
                                    }
                                    else
                                    {
                                        weight[i] = (lower + upper)/2.0;
                                    }
                                }
                                PrintToScreen.WriteLine("weight at point " + i + ", " + ww + ", replaced by " +
                                                        weight[i]);
                            }
                        }
                    }
                }
            }
            return weight;
        }

        // Enter data methods
        // Enter data with x as 2D array and weights provided
        public void enterData(double[,] xData, double[] yData, double[] weight)
        {
            int n = weight.Length;
            m_nData0 = yData.Length;
            m_weightOpt = true;
            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x and y as 2D arrays and weights provided
        public void enterData(double[,] xxData, double[,] yyData, double[,] wWeight)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            int nX1 = xxData.GetLength(0);
            int nX2 = xxData.GetLength(1);
            double[] yData = new double[nY1*nY2];
            double[] weight = new double[nY1*nY2];
            double[,] xData = new double[nY1*nY2,nX1];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                int nX = xxData.GetLength(1);
                if (nY != nX)
                {
                    throw new HCException("multiple y arrays must be of the same Length as the x array Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    xData[ii, i] = xxData[i, j];
                    weight[ii] = wWeight[i, j];
                    ii++;
                }
            }

            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x as 1D array and weights provided
        public void enterData(double[] xxData, double[] yData, double[] weight)
        {
            m_nData0 = yData.Length;
            int n = xxData.Length;
            int m = weight.Length;
            double[,] xData = new double[1,n];
            for (int i = 0; i < n; i++)
            {
                xData[0, i] = xxData[i];
            }

            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x as 1D array and y as 2D array and weights provided
        public void enterData(double[] xxData, double[,] yyData, double[,] wWeight)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            double[] yData = new double[nY1*nY2];
            double[] weight = new double[nY1*nY2];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    weight[ii] = wWeight[i, j];
                    ii++;
                }
            }
            int n = xxData.Length;
            if (n != nY2)
            {
                throw new HCException("x and y data lengths must be the same");
            }
            double[,] xData = new double[1,nY1*n];
            ii = 0;
            for (int j = 0; j < nY1; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    xData[0, ii] = xxData[i];
                    ii++;
                }
            }

            weight = checkForZeroWeights(weight);
            if (m_weightOpt)
            {
                m_weightFlag = 1;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x as 2D array and no weights provided
        public void enterData(double[,] xData, double[] yData)
        {
            m_nData0 = yData.Length;
            int n = yData.Length;
            double[] weight = new double[n];

            m_weightOpt = false;
            for (int i = 0; i < n; i++)
            {
                weight[i] = 1.0D;
            }
            m_weightFlag = 0;
            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x and y as 2D arrays and no weights provided
        public void enterData(double[,] xxData, double[,] yyData)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            int nX1 = xxData.GetLength(0);
            int nX2 = xxData.GetLength(1);
            double[] yData = new double[nY1*nY2];
            double[,] xData = new double[nY1*nY2,nX1];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                int nX = xxData.GetLength(1);
                if (nY != nX)
                {
                    throw new HCException("multiple y arrays must be of the same Length as the x array Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    xData[ii, i] = xxData[i, j];
                    ii++;
                }
            }

            int n = yData.Length;
            double[] weight = new double[n];

            m_weightOpt = false;
            for (int i = 0; i < n; i++)
            {
                weight[i] = 1.0D;
            }
            m_weightFlag = 0;

            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x as 1D array and no weights provided
        public void enterData(double[] xxData, double[] yData)
        {
            m_nData0 = yData.Length;
            int n = xxData.Length;
            double[,] xData = new double[1,n];
            double[] weight = new double[n];

            for (int i = 0; i < n; i++)
            {
                xData[0, i] = xxData[i];
            }

            m_weightOpt = false;
            for (int i = 0; i < n; i++)
            {
                weight[i] = 1.0D;
            }
            m_weightFlag = 0;

            setDefaultValues(xData, yData, weight);
        }

        // Enter data with x as 1D array and y as a 2D array and no weights provided
        public void enterData(double[] xxData, double[,] yyData)
        {
            m_multipleY = true;
            int nY1 = yyData.GetLength(0);
            m_nYarrays = nY1;
            int nY2 = yyData.GetLength(1);
            m_nData0 = nY2;
            double[] yData = new double[nY1*nY2];
            int ii = 0;
            for (int i = 0; i < nY1; i++)
            {
                int nY = yyData.GetLength(1);
                if (nY != nY2)
                {
                    throw new HCException("multiple y arrays must be of the same Length");
                }
                for (int j = 0; j < nY2; j++)
                {
                    yData[ii] = yyData[i, j];
                    ii++;
                }
            }

            double[,] xData = new double[1,nY1*nY2];
            double[] weight = new double[nY1*nY2];

            ii = 0;
            int n = xxData.Length;
            for (int j = 0; j < nY1; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    xData[0, ii] = xxData[i];
                    weight[ii] = 1.0D;
                    ii++;
                }
            }
            m_weightOpt = false;
            m_weightFlag = 0;

            setDefaultValues(xData, yData, weight);
        }

        // Enter data as a single array that has to be binned
        // bin width and value of the low point of the first bin provided
        public void enterData(double[] xxData, double binWidth, double binZero)
        {
            double[,] data = histogramBins(xxData, binWidth, binZero);
            int n = data.GetLength(1);
            m_nData0 = n;
            double[,] xData = new double[1,n];
            double[] yData = new double[n];
            double[] weight = new double[n];
            for (int i = 0; i < n; i++)
            {
                xData[0, i] = data[0, i];
                yData[i] = data[1, i];
            }
            bool flag = setTrueFreqWeights(yData, weight);
            if (flag)
            {
                m_trueFreq = true;
                m_weightOpt = true;
                m_weightFlag = 1;
            }
            else
            {
                m_trueFreq = false;
                m_weightOpt = false;
                m_weightFlag = 0;
            }
            setDefaultValues(xData, yData, weight);
        }

        // Enter data as a single array that has to be binned
        // bin width provided
        public void enterData(double[] xxData, double binWidth)
        {
            double[,] data = histogramBins(xxData, binWidth);
            int n = data.GetLength(1);
            m_nData0 = n;
            double[,] xData = new double[1,n];
            double[] yData = new double[n];
            double[] weight = new double[n];
            for (int i = 0; i < n; i++)
            {
                xData[0, i] = data[0, i];
                yData[i] = data[1, i];
            }
            bool flag = setTrueFreqWeights(yData, weight);
            if (flag)
            {
                m_trueFreq = true;
                m_weightOpt = true;
                m_weightFlag = 0;
            }
            else
            {
                m_trueFreq = false;
                m_weightOpt = false;
                m_weightFlag = 0;
            }
            setDefaultValues(xData, yData, weight);
        }


        protected static bool setTrueFreqWeights(double[] yData, double[] weight)
        {
            int nData = yData.Length;
            bool flag = true;
            //bool unityWeight = false;

            // Set all weights to square root of frequency of occurence
            for (int ii = 0; ii < nData; ii++)
            {
                weight[ii] = Math.Sqrt(Math.Abs(yData[ii]));
            }

            // Check for zero weights and take average of neighbours as weight if it is zero
            for (int ii = 0; ii < nData; ii++)
            {
                double last = 0.0D;
                double next = 0.0D;
                if (weight[ii] == 0)
                {
                    // find previous non-zero value
                    bool testLast = true;
                    int iLast = ii - 1;
                    while (testLast)
                    {
                        if (iLast < 0)
                        {
                            testLast = false;
                        }
                        else
                        {
                            if (weight[iLast] == 0.0D)
                            {
                                iLast--;
                            }
                            else
                            {
                                last = weight[iLast];
                                testLast = false;
                            }
                        }
                    }

                    // find next non-zero value
                    bool testNext = true;
                    int iNext = ii + 1;
                    while (testNext)
                    {
                        if (iNext >= nData)
                        {
                            testNext = false;
                        }
                        else
                        {
                            if (weight[iNext] == 0.0D)
                            {
                                iNext++;
                            }
                            else
                            {
                                next = weight[iNext];
                                testNext = false;
                            }
                        }
                    }

                    // Take average
                    weight[ii] = (last + next)/2.0D;
                }
            }
            return flag;
        }

        // Set data and default values
        protected void setDefaultValues(double[,] xData, double[] yData, double[] weight)
        {
            m_nData = yData.Length;
            m_nXarrays = xData.GetLength(0);
            m_nTerms = m_nXarrays;
            yData = new double[m_nData];
            m_yCalc = new double[m_nData];
            weight = new double[m_nData];
            m_residual = new double[m_nData];
            m_residualW = new double[m_nData];
            xData = new double[m_nXarrays,m_nData];
            int n = weight.Length;
            if (n != m_nData)
            {
                throw new HCException("The weight and the y data lengths do not agree");
            }
            for (int i = 0; i < m_nData; i++)
            {
                yData[i] = yData[i];
                weight[i] = weight[i];
            }
            for (int j = 0; j < m_nXarrays; j++)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    xData[j, i] = xData[j, i];
                }
            }
        }

        // Set standard deviation, variance and covariance denominators to n
        public void setDenominatorToN()
        {
            m_nFactorOption = true;
            Stat.setStaticDenominatorToN();
        }

        // Set standard deviation, variance and covariance denominators to n
        public void setDenominatorToNminusOne()
        {
            m_nFactorOption = false;
            Stat.setStaticDenominatorToNminusOne();
        }


        // Supress printing of results
        public void supressPrint()
        {
            m_blnSupressPrint = true;
        }

        // Supress plot of calculated versus experimental values
        public void supressYYplot()
        {
            m_blnSupressYYplot = true;
        }

        // Supress convergence and chiSquare error messages
        public void supressErrorMessages()
        {
            m_blnSupressErrorMessages = true;
        }

        // Ignore check on whether degrtees of freedom are greater than zero
        public void ignoreDofFcheck()
        {
            m_blnIgnoreDofFcheck = true;
        }

        // Supress the statistical analysis
        public void supressStats()
        {
            m_statFlag = false;
        }

        // Reinstate statistical analysis
        public void reinstateStats()
        {
            m_statFlag = true;
        }

        // Reset the ordinate scale factor option
        // true - Ao is unkown to be found by regression procedure
        // false - Ao set to unity
        public void setYscaleOption(bool flag)
        {
            m_scaleFlag = flag;
            if (flag == false)
            {
                m_yScaleFactor = 1.0D;
            }
        }

        // Reset the ordinate scale factor option
        // true - Ao is unkown to be found by regression procedure
        // false - Ao set to unity
        // retained for backward compatibility
        public void setYscale(bool flag)
        {
            m_scaleFlag = flag;
            if (flag == false)
            {
                m_yScaleFactor = 1.0D;
            }
        }

        // Reset the ordinate scale factor option
        // true - Ao is unkown to be found by regression procedure
        // false - Ao set to given value
        public void setYscaleFactor(double scale)
        {
            m_scaleFlag = false;
            m_yScaleFactor = scale;
        }

        // Get the ordinate scale factor option
        // true - Ao is unkown
        // false - Ao set to unity
        public bool getYscaleOption()
        {
            return m_scaleFlag;
        }

        // Get the ordinate scale factor option
        // true - Ao is unkown
        // false - Ao set to unity
        // retained to ensure backward compatibility
        public bool getYscale()
        {
            return m_scaleFlag;
        }

        // Reset the true frequency test, trueFreq
        // true if yData values are true frequencies, e.g. in a fit to Gaussian; false if not
        // if true chiSquarePoisson (see above) is also calculated
        public void setTrueFreq(bool trFr)
        {
            bool trFrOld = m_trueFreq;
            m_trueFreq = trFr;
            if (trFr)
            {
                bool flag = setTrueFreqWeights(m_yData, m_weight);
                if (flag)
                {
                    m_trueFreq = true;
                    m_weightOpt = true;
                }
                else
                {
                    m_trueFreq = false;
                    m_weightOpt = false;
                }
            }
            else
            {
                if (trFrOld)
                {
                    for (int i = 0; i < m_weight.Length; i++)
                    {
                        m_weight[i] = 1.0D;
                    }
                    m_weightOpt = false;
                }
            }
        }

        // Get the true frequency test, trueFreq
        public bool getTrueFreq()
        {
            return m_trueFreq;
        }

        // Reset the x axis legend
        public void setXlegend(string legend)
        {
            m_xLegend = legend;
            m_legendCheck = true;
        }

        // Reset the y axis legend
        public void setYlegend(string legend)
        {
            m_yLegend = legend;
            m_legendCheck = true;
        }

        // Set the title
        public void setTitle(string title)
        {
            m_graphTitle = title;
        }

        // Multiple linear regression with intercept (including y = ax + b)
        // y = a + b.x1 + c.x2 + d.x3 + . . .
        public void linear()
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 0;
            m_linNonLin = true;
            m_nTerms = m_nXarrays + 1;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }
            double[,] aa = new double[m_nTerms,m_nData];

            for (int j = 0; j < m_nData; j++)
            {
                aa[0, j] = 1.0D;
            }
            for (int i = 1; i < m_nTerms; i++)
            {
                for (int j = 0; j < m_nData; j++)
                {
                    aa[i, j] = m_xData[i - 1, j];
                }
            }
            m_best = new double[m_nTerms];
            bestSd = new double[m_nTerms];
            m_tValues = new double[m_nTerms];
            m_pValues = new double[m_nTerms];
            generalLinear(aa);
            if (!m_blnIgnoreDofFcheck)
            {
                generalLinearStats(aa);
            }
        }

        // Multiple linear regression with intercept (including y = ax + b)
        // plus plot and output file
        // y = a + b.x1 + c.x2 + d.x3 + . . .
        // legends provided
        public void linearPlot(string xLegend, string yLegend)
        {
            m_xLegend = xLegend;
            m_yLegend = yLegend;
            m_legendCheck = true;
            linear();
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY();
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Multiple linear regression with intercept (including y = ax + b)
        // plus plot and output file
        // y = a + b.x1 + c.x2 + d.x3 + . . .
        // no legends provided
        public void linearPlot()
        {
            linear();
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY();
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Polynomial fitting
        // y = a + b.x + c.x^2 + d.x^3 + . . .
        public void polynomial(int deg)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            if (m_nXarrays > 1)
            {
                throw new HCException("This class will only perform a polynomial regression on a single x array");
            }
            if (deg < 1)
            {
                throw new HCException("Polynomial degree must be greater than zero");
            }
            m_lastMethod = 1;
            m_linNonLin = true;
            m_nTerms = deg + 1;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }
            double[,] aa = new double[m_nTerms,m_nData];

            for (int j = 0; j < m_nData; j++)
            {
                aa[0, j] = 1.0D;
            }
            for (int j = 0; j < m_nData; j++)
            {
                aa[1, j] = m_xData[0, j];
            }

            for (int i = 2; i < m_nTerms; i++)
            {
                for (int j = 0; j < m_nData; j++)
                {
                    aa[i, j] = Math.Pow(m_xData[0, j], i);
                }
            }
            m_best = new double[m_nTerms];
            bestSd = new double[m_nTerms];
            m_tValues = new double[m_nTerms];
            m_pValues = new double[m_nTerms];
            generalLinear(aa);
            if (!m_blnIgnoreDofFcheck)
            {
                generalLinearStats(aa);
            }
        }


        // Polynomial fitting plus plot and output file
        // y = a + b.x + c.x^2 + d.x^3 + . . .
        // legends provided
        public void polynomialPlot(int n, string xLegend, string yLegend)
        {
            m_xLegend = xLegend;
            m_yLegend = yLegend;
            m_legendCheck = true;
            polynomial(n);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = plotXY();
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Polynomial fitting plus plot and output file
        // y = a + b.x + c.x^2 + d.x^3 + . . .
        // No legends provided
        public void polynomialPlot(int n)
        {
            polynomial(n);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = plotXY();
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Generalised linear regression
        // y = a.f1(x) + b.f2(x) + c.f3(x) + . . .
        public void linearGeneral()
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 2;

            m_linNonLin = true;
            m_nTerms = m_nXarrays;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }
            m_best = new double[m_nTerms];
            bestSd = new double[m_nTerms];
            m_tValues = new double[m_nTerms];
            m_pValues = new double[m_nTerms];
            generalLinear(m_xData);
            if (!m_blnIgnoreDofFcheck)
            {
                generalLinearStats(m_xData);
            }
        }

        // Generalised linear regression plus plot and output file
        // y = a.f1(x) + b.f2(x) + c.f3(x) + . . .
        // legends provided
        public void linearGeneralPlot(string xLegend, string yLegend)
        {
            m_xLegend = xLegend;
            m_yLegend = yLegend;
            m_legendCheck = true;
            linearGeneral();
            if (!m_blnSupressPrint)
            {
                print();
            }
            if (!m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Generalised linear regression plus plot and output file
        // y = a.f1(x) + b.f2(x) + c.f3(x) + . . .
        // No legends provided
        public void linearGeneralPlot()
        {
            linearGeneral();
            if (!m_blnSupressPrint)
            {
                print();
            }
            if (!m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Generalised linear regression (protected method called by linear(), linearGeneral() and polynomial())
        protected void generalLinear(double[,] xd)
        {
            if (m_nData <= m_nTerms && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Number of unknown parameters is greater than or equal to the number of data points");
            }
            //double sde = 0.0D;
            double sum = 0.0D;
            //double yCalctemp = 0.0D;
            double[,] a = new double[m_nTerms,m_nTerms];
            double[,] h = new double[m_nTerms,m_nTerms];
            double[] b = new double[m_nTerms];
            double[] coeff = new double[m_nTerms];

            // set statistic arrays to NaN if df check ignored
            if (m_blnIgnoreDofFcheck)
            {
                bestSd = new double[m_nTerms];
                m_pseudoSd = new double[m_nTerms];
                m_tValues = new double[m_nTerms];
                m_pValues = new double[m_nTerms];

                m_covar = new double[m_nTerms,m_nTerms];
                m_corrCoeff = new double[m_nTerms,m_nTerms];
                ;
                for (int i = 0; i < m_nTerms; i++)
                {
                    bestSd[i] = double.NaN;
                    m_pseudoSd[i] = double.NaN;
                    for (int j = 0; j < m_nTerms; j++)
                    {
                        m_covar[i, j] = double.NaN;
                        m_corrCoeff[i, j] = double.NaN;
                    }
                }
            }

            for (int i = 0; i < m_nTerms; ++i)
            {
                sum = 0.0D;
                for (int j = 0; j < m_nData; ++j)
                {
                    sum += m_yData[j]*xd[i, j]/Fmath.square(m_weight[j]);
                }
                b[i] = sum;
            }
            for (int i = 0; i < m_nTerms; ++i)
            {
                for (int j = 0; j < m_nTerms; ++j)
                {
                    sum = 0.0;
                    for (int k = 0; k < m_nData; ++k)
                    {
                        sum += xd[i, k]*xd[j, k]/Fmath.square(m_weight[k]);
                    }
                    a[j, i] = sum;
                }
            }
            MatrixExtended aa = new MatrixExtended(a);
            if (m_blnSupressErrorMessages)
            {
                aa.supressErrorMessage();
            }
            coeff = aa.solveLinearSet(b);

            for (int i = 0; i < m_nTerms; i++)
            {
                m_best[i] = coeff[i];
            }
        }

        // Generalised linear regression statistics (protected method called by linear(), linearGeneral() and polynomial())
        protected void generalLinearStats(double[,] xd)
        {
            double sde = 0.0D, sum = 0.0D, yCalctemp = 0.0D;
            double[,] a = new double[m_nTerms,m_nTerms];
            double[,] h = new double[m_nTerms,m_nTerms];
            double[,] stat = new double[m_nTerms,m_nTerms];
            double[,] cov = new double[m_nTerms,m_nTerms];
            m_covar = new double[m_nTerms,m_nTerms];
            m_corrCoeff = new double[m_nTerms,m_nTerms];
            double[] coeffSd = new double[m_nTerms];
            double[] coeff = new double[m_nTerms];

            for (int i = 0; i < m_nTerms; i++)
            {
                coeff[i] = m_best[i];
            }

            if (m_weightOpt)
            {
                m_chiSquare = 0.0D;
            }
            m_sumOfSquares = 0.0D;
            for (int i = 0; i < m_nData; ++i)
            {
                yCalctemp = 0.0;
                for (int j = 0; j < m_nTerms; ++j)
                {
                    yCalctemp += coeff[j]*xd[j, i];
                }
                m_yCalc[i] = yCalctemp;
                yCalctemp -= m_yData[i];
                m_residual[i] = yCalctemp;
                m_residualW[i] = yCalctemp/m_weight[i];
                if (m_weightOpt)
                {
                    m_chiSquare += Fmath.square(yCalctemp/m_weight[i]);
                }
                m_sumOfSquares += Fmath.square(yCalctemp);
            }
            if (m_weightOpt || m_trueFreq)
            {
                m_reducedChiSquare = m_chiSquare/(m_degreesOfFreedom);
            }
            double varY = m_sumOfSquares/(m_degreesOfFreedom);
            double sdY = Math.Sqrt(varY);

            if (m_sumOfSquares == 0.0D)
            {
                for (int i = 0; i < m_nTerms; i++)
                {
                    coeffSd[i] = 0.0D;
                    for (int j = 0; j < m_nTerms; j++)
                    {
                        m_covar[i, j] = 0.0D;
                        if (i == j)
                        {
                            m_corrCoeff[i, j] = 1.0D;
                        }
                        else
                        {
                            m_corrCoeff[i, j] = 0.0D;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_nTerms; ++i)
                {
                    for (int j = 0; j < m_nTerms; ++j)
                    {
                        sum = 0.0;
                        for (int k = 0; k < m_nData; ++k)
                        {
                            if (m_weightOpt)
                            {
                                sde = m_weight[k];
                            }
                            else
                            {
                                sde = sdY;
                            }
                            sum += xd[i, k]*xd[j, k]/Fmath.square(sde);
                        }
                        h[j, i] = sum;
                    }
                }
                MatrixExtended hh = new MatrixExtended(h);
                if (m_blnSupressErrorMessages)
                {
                    hh.supressErrorMessage();
                }
                hh = hh.inverse();
                stat = hh.getArrayCopy();
                for (int j = 0; j < m_nTerms; ++j)
                {
                    coeffSd[j] = Math.Sqrt(stat[j, j]);
                }

                for (int i = 0; i < m_nTerms; i++)
                {
                    for (int j = 0; j < m_nTerms; j++)
                    {
                        m_covar[i, j] = stat[i, j];
                    }
                }

                for (int i = 0; i < m_nTerms; i++)
                {
                    for (int j = 0; j < m_nTerms; j++)
                    {
                        if (i == j)
                        {
                            m_corrCoeff[i, j] = 1.0D;
                        }
                        else
                        {
                            m_corrCoeff[i, j] = m_covar[i, j]/(coeffSd[i]*coeffSd[j]);
                        }
                    }
                }
            }

            for (int i = 0; i < m_nTerms; i++)
            {
                bestSd[i] = coeffSd[i];
                m_tValues[i] = m_best[i]/bestSd[i];
                double atv = Math.Abs(m_tValues[i]);
                m_pValues[i] = 1.0 - Stat.studentTcdf(-atv, atv, m_degreesOfFreedom);
            }

            if (m_nXarrays == 1 && m_nYarrays == 1)
            {
                m_sampleR = Stat.CorrCoeff(
                    ArrayHelper.GetRowCopy<double>(
                        m_xData,
                        0),
                    m_yData,
                    m_weight);
                m_sampleR2 = m_sampleR*m_sampleR;
                m_adjustedR = m_sampleR;
                m_adjustedR2 = m_sampleR2;
            }
            else
            {
                multCorrelCoeff(m_yData, m_yCalc, m_weight);
            }
        }


        // Nelder and Mead Simplex Simplex Non-linear Regression
        protected void nelderMead(Object regFun, double[] start, double[] step, double fTol, int nMax)
        {
            int np = start.Length; // number of unknown parameters;
            if (m_maxConstraintIndex >= np)
            {
                throw new HCException("You have entered more constrained parameters (" + m_maxConstraintIndex +
                                    ") than minimisation parameters (" + np + ")");
            }
            m_nlrStatus = true; // -> false if convergence criterion not met
            m_nTerms = np; // number of parameters whose best estimates are to be determined
            int nnp = np + 1; // number of simplex apices
            m_lastSSnoConstraint = 0.0D; // last sum of squares without a penalty constraint being applied

            if (m_scaleOpt < 2)
            {
                m_scale = new double[np]; // scaling factors
            }
            if (m_scaleOpt == 2 && m_scale.Length != start.Length)
            {
                throw new HCException("scale array and initial estimate array are of different lengths");
            }
            if (step.Length != start.Length)
            {
                throw new HCException("step array Length " + step.Length + " and initial estimate array Length " +
                                    start.Length + " are of different");
            }

            // check for zero step sizes
            for (int i = 0; i < np; i++)
            {
                if (step[i] == 0.0D)
                {
                    throw new HCException("step " + i + " size is zero");
                }
            }

            // set statistic arrays to NaN if degrees of freedom check ignored
            if (m_blnIgnoreDofFcheck)
            {
                bestSd = new double[m_nTerms];
                m_pseudoSd = new double[m_nTerms];
                m_tValues = new double[m_nTerms];
                m_pValues = new double[m_nTerms];

                m_covar = new double[m_nTerms,m_nTerms];
                m_corrCoeff = new double[m_nTerms,m_nTerms];
                ;
                for (int i = 0; i < m_nTerms; i++)
                {
                    bestSd[i] = double.NaN;
                    m_pseudoSd[i] = double.NaN;
                    for (int j = 0; j < m_nTerms; j++)
                    {
                        m_covar[i, j] = double.NaN;
                        m_corrCoeff[i, j] = double.NaN;
                    }
                }
            }

            // set up arrays
            m_startH = new double[np]; // holding array of unscaled initial start values
            m_stepH = new double[np]; // unscaled initial step values
            m_startSH = new double[np]; // holding array of scaled initial start values
            m_stepSH = new double[np]; // scaled initial step values
            double[] pmin = new double[np]; // Nelder and Mead Pmin
            m_best = new double[np]; // best estimates array
            bestSd = new double[np]; // m_sd of best estimates array
            m_tValues = new double[np]; // t-value of best estimates array
            m_pValues = new double[np]; // p-value of best estimates array

            double[,] pp = new double[nnp,nnp]; //Nelder and Mead P
            double[] yy = new double[nnp]; //Nelder and Mead y
            double[] pbar = new double[nnp]; //Nelder and Mead P with bar superscript
            double[] pstar = new double[nnp]; //Nelder and Mead P*
            double[] p2star = new double[nnp]; //Nelder and Mead P**

            // m_mean of absolute values of yData (for testing for minimum)
            double yabsmean = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                yabsmean += Math.Abs(m_yData[i]);
            }
            yabsmean /= m_nData;

            // Set any single parameter constraint parameters
            if (m_penalty)
            {
                int itemp = (int) m_penalties[1];
                m_nConstraints = itemp;
                m_penaltyParam = new int[m_nConstraints];
                m_penaltyCheck = new int[m_nConstraints];
                m_constraints = new double[m_nConstraints];
                double dtemp = 0;
                int j = 2;

                for (int i = 0; i < m_nConstraints; i++)
                {
                    itemp = (int) m_penalties[j];
                    m_penaltyParam[i] = itemp;
                    j++;
                    itemp = (int) m_penalties[j];
                    m_penaltyCheck[i] = itemp;
                    j++;
                    dtemp = (double) m_penalties[j];
                    m_constraints[i] = dtemp;
                    j++;
                }
            }

            // Set any multiple parameters constraint parameters
            if (m_sumPenalty)
            {
                int intSumPenaltiesLenght = ((int[]) m_sumPenalties[0]).Length;

                int itemp = (int) m_sumPenalties[1];
                m_nSumConstraints = itemp;
                m_sumPenaltyParam = new int[
                    m_nSumConstraints,
                    intSumPenaltiesLenght];
                m_sumPlusOrMinus = new double[
                    m_nSumConstraints,
                    intSumPenaltiesLenght];
                m_sumPenaltyCheck = new int[m_nSumConstraints];
                m_sumPenaltyNumber = new int[m_nSumConstraints];
                m_sumConstraints = new double[m_nSumConstraints];
                int[] itempArray = null;
                double[] dtempArray = null;
                double dtemp = 0;
                int j = 2;

                for (int i = 0; i < m_nSumConstraints; i++)
                {
                    itemp = (int) m_sumPenalties[j];
                    m_sumPenaltyNumber[i] = itemp;
                    j++;
                    itempArray = (int[]) m_sumPenalties[j];
                    ArrayHelper.SetRow(
                        m_sumPenaltyParam,
                        itempArray,
                        i);

                    j++;
                    dtempArray = (double[]) m_sumPenalties[j];
                    ArrayHelper.SetRow(
                        m_sumPlusOrMinus,
                        dtempArray,
                        i);
                    j++;
                    itemp = (int) m_sumPenalties[j];
                    m_sumPenaltyCheck[i] = itemp;
                    j++;
                    dtemp = (double) m_sumPenalties[j];
                    m_sumConstraints[i] = dtemp;
                    j++;
                }
            }

            // Store unscaled start and step values
            for (int i = 0; i < np; i++)
            {
                m_startH[i] = start[i];
                m_stepH[i] = step[i];
            }

            // scale initial estimates and step sizes
            if (m_scaleOpt > 0)
            {
                bool testzero = false;
                for (int i = 0; i < np; i++)
                {
                    if (start[i] == 0.0D)
                    {
                        testzero = true;
                    }
                }
                if (testzero)
                {
                    PrintToScreen.WriteLine("Neler and Mead Simplex: a start value of zero precludes scaling");
                    PrintToScreen.WriteLine("Regression performed without scaling");
                    m_scaleOpt = 0;
                }
            }
            switch (m_scaleOpt)
            {
                case 0: // No scaling carried out
                    for (int i = 0; i < np; i++)
                    {
                        m_scale[i] = 1.0D;
                    }
                    break;
                case 1: // All parameters scaled to unity
                    for (int i = 0; i < np; i++)
                    {
                        m_scale[i] = 1.0/start[i];
                        step[i] = step[i]/start[i];
                        start[i] = 1.0D;
                    }
                    break;
                case 2: // Each parameter scaled by a user provided factor
                    for (int i = 0; i < np; i++)
                    {
                        step[i] *= m_scale[i];
                        start[i] *= m_scale[i];
                    }
                    break;
                default:
                    throw new HCException("Scaling factor option " + m_scaleOpt + " not recognised");
            }

            // set class member values
            m_fTol = fTol;
            m_nMax = nMax;
            m_nIter = 0;
            for (int i = 0; i < np; i++)
            {
                m_startSH[i] = start[i];
                m_stepSH[i] = step[i];
                m_scale[i] = m_scale[i];
            }

            // initial simplex
            double sho = 0.0D;
            for (int i = 0; i < np; ++i)
            {
                sho = start[i];
                pstar[i] = sho;
                p2star[i] = sho;
                pmin[i] = sho;
            }

            int jcount = m_konvge; // count of number of restarts still available

            for (int i = 0; i < np; ++i)
            {
                pp[i, nnp - 1] = start[i];
            }
            yy[nnp - 1] = sumSquares(regFun, start);
            for (int j = 0; j < np; ++j)
            {
                start[j] = start[j] + step[j];

                for (int i = 0; i < np; ++i)
                {
                    pp[i, j] = start[i];
                }
                yy[j] = sumSquares(regFun, start);
                start[j] = start[j] - step[j];
            }

            // loop over allowed number of iterations

            double ynewlo = 0.0D; // current value lowest y
            double ystar = 0.0D; // Nelder and Mead y*
            double y2star = 0.0D; // Nelder and Mead y**
            double ylo = 0.0D; // Nelder and Mead y(low)
            double fMin; // function value at minimum

            int ilo = 0; // index of lowest apex
            int ihi = 0; // index of highest apex
            int ln = 0; // counter for a check on low and high apices
            bool test = true; // test becomes false on reaching minimum

            // variables used in calculating the variance of the simplex at a putative minimum
            double curMin = 00D; // m_sd of the values at the simplex apices
            double sumnm = 0.0D; // for calculating the m_mean of the apical values
            double zn = 0.0D; // for calculating the summation of their differences from the m_mean
            double summnm = 0.0D; // for calculating the variance

            while (test)
            {
                // Determine h
                ylo = yy[0];
                ynewlo = ylo;
                ilo = 0;
                ihi = 0;
                for (int i = 1; i < nnp; ++i)
                {
                    if (yy[i] < ylo)
                    {
                        ylo = yy[i];
                        ilo = i;
                    }
                    if (yy[i] > ynewlo)
                    {
                        ynewlo = yy[i];
                        ihi = i;
                    }
                }
                // Calculate pbar
                for (int i = 0; i < np; ++i)
                {
                    zn = 0.0D;
                    for (int j = 0; j < nnp; ++j)
                    {
                        zn += pp[i, j];
                    }
                    zn -= pp[i, ihi];
                    pbar[i] = zn/np;
                }

                // Calculate p=(1+alpha).pbar-alpha.ph {Reflection}
                for (int i = 0; i < np; ++i)
                {
                    pstar[i] = (1.0 + m_rCoeff)*pbar[i] - m_rCoeff*pp[i, ihi];
                }

                // Calculate y*
                ystar = sumSquares(regFun, pstar);

                ++m_nIter;

                // check for y*<yi
                if (ystar < ylo)
                {
                    // Calculate p**=(1+gamma).p*-gamma.pbar {Extension}
                    for (int i = 0; i < np; ++i)
                    {
                        p2star[i] = pstar[i]*(1.0D + m_eCoeff) - m_eCoeff*pbar[i];
                    }
                    // Calculate y**
                    y2star = sumSquares(regFun, p2star);
                    ++m_nIter;
                    if (y2star < ylo)
                    {
                        // Replace ph by p**
                        for (int i = 0; i < np; ++i)
                        {
                            pp[i, ihi] = p2star[i];
                        }
                        yy[ihi] = y2star;
                    }
                    else
                    {
                        //Replace ph by p*
                        for (int i = 0; i < np; ++i)
                        {
                            pp[i, ihi] = pstar[i];
                        }
                        yy[ihi] = ystar;
                    }
                }
                else
                {
                    // Check y*>yi, i!=h
                    ln = 0;
                    for (int i = 0; i < nnp; ++i)
                    {
                        if (i != ihi && ystar > yy[i])
                        {
                            ++ln;
                        }
                    }
                    if (ln == np)
                    {
                        // y*>= all yi; Check if y*>yh
                        if (ystar <= yy[ihi])
                        {
                            // Replace ph by p*
                            for (int i = 0; i < np; ++i)
                            {
                                pp[i, ihi] = pstar[i];
                            }
                            yy[ihi] = ystar;
                        }
                        // Calculate p** =beta.ph+(1-beta)pbar  {Contraction}
                        for (int i = 0; i < np; ++i)
                        {
                            p2star[i] = m_cCoeff*pp[i, ihi] + (1.0 - m_cCoeff)*pbar[i];
                        }
                        // Calculate y**
                        y2star = sumSquares(regFun, p2star);
                        ++m_nIter;
                        // Check if y**>yh
                        if (y2star > yy[ihi])
                        {
                            //Replace all pi by (pi+pl)/2

                            for (int j = 0; j < nnp; ++j)
                            {
                                for (int i = 0; i < np; ++i)
                                {
                                    pp[i, j] = 0.5*(pp[i, j] + pp[i, ilo]);
                                    pmin[i] = pp[i, j];
                                }
                                yy[j] = sumSquares(regFun, pmin);
                            }
                            m_nIter += nnp;
                        }
                        else
                        {
                            // Replace ph by p**
                            for (int i = 0; i < np; ++i)
                            {
                                pp[i, ihi] = p2star[i];
                            }
                            yy[ihi] = y2star;
                        }
                    }
                    else
                    {
                        // replace ph by p*
                        for (int i = 0; i < np; ++i)
                        {
                            pp[i, ihi] = pstar[i];
                        }
                        yy[ihi] = ystar;
                    }
                }

                // test for convergence
                // calculte m_sd of simplex and determine the minimum point
                sumnm = 0.0;
                ynewlo = yy[0];
                ilo = 0;
                for (int i = 0; i < nnp; ++i)
                {
                    sumnm += yy[i];
                    if (ynewlo > yy[i])
                    {
                        ynewlo = yy[i];
                        ilo = i;
                    }
                }
                sumnm /= (nnp);
                summnm = 0.0;
                for (int i = 0; i < nnp; ++i)
                {
                    zn = yy[i] - sumnm;
                    summnm += zn*zn;
                }
                curMin = Math.Sqrt(summnm/np);

                // test simplex m_sd
                switch (m_minTest)
                {
                    case 0:
                        // terminate if the standard deviation of the sum of squares [unweighted data] or of the chi square values [weighted data]
                        // at the apices of the simplex is less than the tolerance, fTol
                        if (curMin < fTol)
                        {
                            test = false;
                        }
                        break;
                    case 1:
                        // terminate if the reduced chi square [weighted data] or the reduced sum of squares [unweighted data] at the lowest apex
                        // of the simplex is less than the m_mean of the absolute values of the dependent variable (y values) multiplied by the tolerance, fTol.
                        if (Math.Sqrt(ynewlo/m_degreesOfFreedom) < yabsmean*fTol)
                        {
                            test = false;
                        }
                        break;
                    default:
                        throw new HCException("Simplex standard deviation test option " + m_minTest + " not recognised");
                }
                m_sumOfSquares = ynewlo;
                if (!test)
                {
                    // temporary store of best estimates
                    for (int i = 0; i < np; ++i)
                    {
                        pmin[i] = pp[i, ilo];
                    }
                    yy[nnp - 1] = ynewlo;
                    // store simplex m_sd
                    m_simplexSd = curMin;
                    // test for restart
                    --jcount;
                    if (jcount > 0)
                    {
                        test = true;
                        for (int j = 0; j < np; ++j)
                        {
                            pmin[j] = pmin[j] + step[j];
                            for (int i = 0; i < np; ++i)
                            {
                                pp[i, j] = pmin[i];
                            }
                            yy[j] = sumSquares(regFun, pmin);
                            pmin[j] = pmin[j] - step[j];
                        }
                    }
                }

                // test for reaching allowed number of iterations
                if (test && m_nIter > nMax)
                {
                    if (!m_blnSupressErrorMessages)
                    {
                        PrintToScreen.WriteLine("Maximum iteration number reached, in Regression.simplex(...)");
                        PrintToScreen.WriteLine("without the convergence criterion being satisfied");
                        PrintToScreen.WriteLine("Current parameter estimates and sum of squares values returned");
                    }
                    m_nlrStatus = false;
                    // store current estimates
                    for (int i = 0; i < np; ++i)
                    {
                        pmin[i] = pp[i, ilo];
                    }
                    yy[nnp - 1] = ynewlo;
                    test = false;
                }
            }

            //  store of the best estimates, function value at the minimum and number of restarts
            for (int i = 0; i < np; ++i)
            {
                pmin[i] = pp[i, ilo];
                m_best[i] = pmin[i]/m_scale[i];
                m_scale[i] = 1.0D; // unscale for statistical methods
            }
            fMin = ynewlo;
            m_kRestart = m_konvge - jcount;

            // perform statistical analysis if possible and requested
            if (m_statFlag)
            {
                if (!m_blnIgnoreDofFcheck)
                {
                    pseudoLinearStats(regFun);
                }
            }
            else
            {
                for (int i = 0; i < np; ++i)
                {
                    bestSd[i] = double.NaN;
                }
            }
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        public void simplex(RegressionFunction g, double[] start, double[] step, double fTol, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fTol, nMax);
        }


        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        public void simplexPlot(RegressionFunction g, double[] start, double[] step, double fTol, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            Object regFun = g;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fTol, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  maximum iterations
        public void simplex(RegressionFunction g, double[] start, double[] step, double fTol)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            int nMaxx = m_nMax;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fTol, nMaxx);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default  maximum iterations
        public void simplexPlot(RegressionFunction g, double[] start, double[] step, double fTol)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start, step, fTol);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        public void simplex(RegressionFunction g, double[] start, double[] step, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            double fToll = m_fTol;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fToll, nMax);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        public void simplexPlot(RegressionFunction g, double[] start, double[] step, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start, step, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        // Default  maximum iterations
        public void simplex(RegressionFunction g, double[] start, double[] step)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            double fToll = m_fTol;
            int nMaxx = m_nMax;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fToll, nMaxx);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        // Default  maximum iterations
        public void simplexPlot(RegressionFunction g, double[] start, double[] step)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start, step);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default step option - all step[i] = dStep
        public void simplex(RegressionFunction g, double[] start, double fTol, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fTol, nMax);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default step option - all step[i] = dStep
        public void simplexPlot(RegressionFunction g, double[] start, double fTol, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start, fTol, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplex(RegressionFunction g, double[] start, double fTol)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            int nMaxx = m_nMax;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fTol, nMaxx);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplexPlot(RegressionFunction g, double[] start, double fTol)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start, fTol);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        // Default step option - all step[i] = dStep
        public void simplex(RegressionFunction g, double[] start, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            double fToll = m_fTol;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fToll, nMax);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        // Default step option - all step[i] = dStep
        public void simplexPlot(RegressionFunction g, double[] start, int nMax)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplex(RegressionFunction g, double[] start)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplex2 should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            int nMaxx = m_nMax;
            double fToll = m_fTol;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fToll, nMaxx);
        }

        // Nelder and Mead Simplex Simplex Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplexPlot(RegressionFunction g, double[] start)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y arrays\nsimplexPlot2 should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex(g, start);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }


        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        public void simplex2(RegressionFunction2 g, double[] start, double[] step, double fTol, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fTol, nMax);
        }


        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        public void simplexPlot2(RegressionFunction2 g, double[] start, double[] step, double fTol, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fTol, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  maximum iterations
        public void simplex2(RegressionFunction2 g, double[] start, double[] step, double fTol)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            int nMaxx = m_nMax;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fTol, nMaxx);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default  maximum iterations
        public void simplexPlot2(RegressionFunction2 g, double[] start, double[] step, double fTol)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start, step, fTol);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        public void simplex2(RegressionFunction2 g, double[] start, double[] step, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            double fToll = m_fTol;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fToll, nMax);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        public void simplexPlot2(RegressionFunction2 g, double[] start, double[] step, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start, step, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        // Default  maximum iterations
        public void simplex2(RegressionFunction2 g, double[] start, double[] step)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            double fToll = m_fTol;
            int nMaxx = m_nMax;
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, step, fToll, nMaxx);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        // Default  maximum iterations
        public void simplexPlot2(RegressionFunction2 g, double[] start, double[] step)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start, step);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default step option - all step[i] = dStep
        public void simplex2(RegressionFunction2 g, double[] start, double fTol, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fTol, nMax);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default step option - all step[i] = dStep
        public void simplexPlot2(RegressionFunction2 g, double[] start, double fTol, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start, fTol, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplex2(RegressionFunction2 g, double[] start, double fTol)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            int nMaxx = m_nMax;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fTol, nMaxx);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplexPlot2(RegressionFunction2 g, double[] start, double fTol)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start, fTol);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        // Default step option - all step[i] = dStep
        public void simplex2(RegressionFunction2 g, double[] start, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            double fToll = m_fTol;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fToll, nMax);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        // Default step option - all step[i] = dStep
        public void simplexPlot2(RegressionFunction2 g, double[] start, int nMax)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start, nMax);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Nelder and Mead simplex
        // Default  tolerance
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplex2(RegressionFunction2 g, double[] start)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            Object regFun = g;
            int n = start.Length;
            int nMaxx = m_nMax;
            double fToll = m_fTol;
            double[] stepp = new double[n];
            for (int i = 0; i < n; i++)
            {
                stepp[i] = m_dStep*start[i];
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - start.Length;
            nelderMead(regFun, start, stepp, fToll, nMaxx);
        }

        // Nelder and Mead Simplex Simplex2 Non-linear Regression
        // plus plot and output file
        // Default  tolerance
        // Default  maximum iterations
        // Default step option - all step[i] = dStep
        public void simplexPlot2(RegressionFunction2 g, double[] start)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_lastMethod = 3;
            m_userSupplied = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            simplex2(g, start);
            if (!m_blnSupressPrint)
            {
                print();
            }
            int flag = 0;
            if (m_xData.GetLength(0) < 2)
            {
                flag = plotXY2(g);
            }
            if (flag != -2 && !m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        // Calculate the sum of squares of the residuals for non-linear regression
        protected double sumSquares(Object regFun, double[] x)
        {
            RegressionFunction g1 = null;
            RegressionFunction2 g2 = null;
            if (m_multipleY)
            {
                g2 = (RegressionFunction2) regFun;
            }
            else
            {
                g1 = (RegressionFunction) regFun;
            }

            double ss = -3.0D;
            double[] param = new double[m_nTerms];
            double[] xd = new double[m_nXarrays];
            // rescale for calcultion of the function
            for (int i = 0; i < m_nTerms; i++)
            {
                param[i] = x[i]/m_scale[i];
            }

            // single parameter penalty functions
            double tempFunctVal = m_lastSSnoConstraint;
            bool test = true;
            if (m_penalty)
            {
                int k = 0;
                for (int i = 0; i < m_nConstraints; i++)
                {
                    k = m_penaltyParam[i];
                    switch (m_penaltyCheck[i])
                    {
                        case -1: // parameter constrained to lie above a given constraint value
                            if (param[k] < m_constraints[i])
                            {
                                ss = tempFunctVal + m_penaltyWeight*Fmath.square(m_constraints[i] - param[k]);
                                test = false;
                            }
                            break;
                        case 0: // parameter constrained to lie within a given tolerance about a constraint value
                            if (param[k] < m_constraints[i]*(1.0 - m_constraintTolerance))
                            {
                                ss = tempFunctVal +
                                     m_penaltyWeight*
                                     Fmath.square(m_constraints[i]*(1.0 - m_constraintTolerance) - param[k]);
                                test = false;
                            }
                            if (param[k] > m_constraints[i]*(1.0 + m_constraintTolerance))
                            {
                                ss = tempFunctVal +
                                     m_penaltyWeight*
                                     Fmath.square(param[k] - m_constraints[i]*(1.0 + m_constraintTolerance));
                                test = false;
                            }
                            break;
                        case 1: // parameter constrained to lie below a given constraint value
                            if (param[k] > m_constraints[i])
                            {
                                ss = tempFunctVal + m_penaltyWeight*Fmath.square(param[k] - m_constraints[i]);
                                test = false;
                            }
                            break;
                        default:
                            throw new HCException("The " + i + "th penalty check " + m_penaltyCheck[i] + " not recognised");
                    }
                }
            }

            // multiple parameter penalty functions
            if (m_sumPenalty)
            {
                int kk = 0;
                double pSign = 0;
                for (int i = 0; i < m_nSumConstraints; i++)
                {
                    double sumPenaltySum = 0.0D;
                    for (int j = 0; j < m_sumPenaltyNumber[i]; j++)
                    {
                        kk = m_sumPenaltyParam[i, j];
                        pSign = m_sumPlusOrMinus[i, j];
                        sumPenaltySum += param[kk]*pSign;
                    }
                    switch (m_sumPenaltyCheck[i])
                    {
                        case -1: // designated 'parameter sum' constrained to lie above a given constraint value
                            if (sumPenaltySum < m_sumConstraints[i])
                            {
                                ss = tempFunctVal + m_penaltyWeight*Fmath.square(m_sumConstraints[i] - sumPenaltySum);
                                test = false;
                            }
                            break;
                        case 0:
                            // designated 'parameter sum' constrained to lie within a given tolerance about a given constraint value
                            if (sumPenaltySum < m_sumConstraints[i]*(1.0 - m_constraintTolerance))
                            {
                                ss = tempFunctVal +
                                     m_penaltyWeight*
                                     Fmath.square(m_sumConstraints[i]*(1.0 - m_constraintTolerance) - sumPenaltySum);
                                test = false;
                            }
                            if (sumPenaltySum > m_sumConstraints[i]*(1.0 + m_constraintTolerance))
                            {
                                ss = tempFunctVal +
                                     m_penaltyWeight*
                                     Fmath.square(sumPenaltySum - m_sumConstraints[i]*(1.0 + m_constraintTolerance));
                                test = false;
                            }
                            break;
                        case 1: // designated 'parameter sum' constrained to lie below a given constraint value
                            if (sumPenaltySum > m_sumConstraints[i])
                            {
                                ss = tempFunctVal + m_penaltyWeight*Fmath.square(sumPenaltySum - m_sumConstraints[i]);
                                test = false;
                            }
                            break;
                        default:
                            throw new HCException("The " + i + "th summation penalty check " + m_sumPenaltyCheck[i] +
                                                " not recognised");
                    }
                }
            }

            // call function calculation and calculate the sum of squares if constraints have not intervened
            if (test)
            {
                ss = 0.0D;
                for (int i = 0; i < m_nData; i++)
                {
                    for (int j = 0; j < m_nXarrays; j++)
                    {
                        xd[j] = m_xData[j, i];
                    }
                    if (!m_multipleY)
                    {
                        ss += Fmath.square((m_yData[i] - g1.function(param, xd))/m_weight[i]);
                    }
                    else
                    {
                        ss += Fmath.square((m_yData[i] - g2.function(param, xd, i))/m_weight[i]);
                    }
                }
                m_lastSSnoConstraint = ss;
            }

            // return sum of squares
            return ss;
        }


        // add a single parameter constraint boundary for the non-linear regression
        public void addConstraint(int paramIndex, int conDir, double constraint)
        {
            m_penalty = true;

            // First element reserved for method number if other methods than 'cliff' are added later
            if (m_penalties.Count == 0)
            {
                m_penalties.Add(m_constraintMethod);
            }

            // add constraint
            if (m_penalties.Count == 1)
            {
                m_penalties.Add(1);
            }
            else
            {
                int nPC = ((int) m_penalties[1]);
                nPC++;
                m_penalties[1] = nPC;
            }
            m_penalties.Add(paramIndex);
            m_penalties.Add(conDir);
            m_penalties.Add(constraint);
            if (paramIndex > m_maxConstraintIndex)
            {
                m_maxConstraintIndex = paramIndex;
            }
        }


        // add a multiple parameter constraint boundary for the non-linear regression
        public void addConstraint(int[] paramIndices, int[] plusOrMinus, int conDir, double constraint)
        {
            ArrayMaths am = new ArrayMaths(plusOrMinus);
            double[] dpom = am.getArray_as_double();
            addConstraint(paramIndices, dpom, conDir, constraint);
        }

        // add a multiple parameter constraint boundary for the non-linear regression
        public void addConstraint(int[] paramIndices, double[] plusOrMinus, int conDir, double constraint)
        {
            int nCon = paramIndices.Length;
            int nPorM = plusOrMinus.Length;
            if (nCon != nPorM)
            {
                throw new HCException("num of parameters, " + nCon + ", does not equal number of parameter signs, " +
                                    nPorM);
            }

            m_sumPenalty = true;

            // First element reserved for method number if other methods than 'cliff' are added later
            if (m_sumPenalties.Count == 0)
            {
                m_sumPenalties.Add(m_constraintMethod);
            }

            // add constraint
            if (m_sumPenalties.Count == 1)
            {
                m_sumPenalties.Add(1);
            }
            else
            {
                int nPC = ((int) m_sumPenalties[1]);
                nPC++;
                m_sumPenalties[1] = nPC;
            }
            m_sumPenalties.Add(nCon);
            m_sumPenalties.Add(paramIndices);
            m_sumPenalties.Add(plusOrMinus);
            m_sumPenalties.Add(conDir);
            m_sumPenalties.Add(constraint);
            ArrayMaths am = new ArrayMaths(paramIndices);
            int maxI = am.getMaximum_as_int();
            if (maxI > m_maxConstraintIndex)
            {
                m_maxConstraintIndex = maxI;
            }
        }


        // remove all constraint boundaries for the non-linear regression
        public void removeConstraints()
        {
            // check if single parameter constraints already set
            if (m_penalties.Count != 0)
            {
                int m = m_penalties.Count;

                // remove single parameter constraints
                for (int i = m - 1; i >= 0; i--)
                {
                    m_penalties.RemoveAt(i);
                }
            }
            m_penalty = false;
            m_nConstraints = 0;

            // check if mutiple parameter constraints already set
            if (m_sumPenalties.Count != 0)
            {
                int m = m_sumPenalties.Count;

                // remove multiple parameter constraints
                for (int i = m - 1; i >= 0; i--)
                {
                    m_sumPenalties.RemoveAt(i);
                }
            }
            m_sumPenalty = false;
            m_nSumConstraints = 0;
            m_maxConstraintIndex = -1;
        }

        // Reset the tolerance used in a fixed_ value constraint
        public void setConstraintTolerance(double tolerance)
        {
            m_constraintTolerance = tolerance;
        }


        //  linear statistics applied to a non-linear regression
        protected int pseudoLinearStats(Object regFun)
        {
            double f1 = 0.0D, f2 = 0.0D, f3 = 0.0D, f4 = 0.0D; // intermdiate values in numerical differentiation
            int flag = 0; // returned as 0 if method fully successful;
            // negative if partially successful or unsuccessful: check posVarFlag and invertFlag
            //  -1  posVarFlag or invertFlag is false;
            //  -2  posVarFlag and invertFlag are false
            int np = m_nTerms;

            double[] f = new double[np];
            double[] pmin = new double[np];
            double[] coeffSd = new double[np];
            double[] xd = new double[m_nXarrays];
            double[,] stat = new double[np,np];
            m_pseudoSd = new double[np];

            //double temp = 0;

            m_grad = new double[np,2];
            m_covar = new double[np,np];
            m_corrCoeff = new double[np,np];

            // Get best estimates
            pmin = (double[]) m_best.Clone();

            // gradient both sides of the minimum
            double hold0 = 1.0D;
            double hold1 = 1.0D;
            for (int i = 0; i < np; ++i)
            {
                for (int k = 0; k < np; ++k)
                {
                    f[k] = pmin[k];
                }
                hold0 = pmin[i];
                if (hold0 == 0.0D)
                {
                    hold0 = m_stepH[i];
                    m_zeroCheck = true;
                }
                f[i] = hold0*(1.0D - m_delta);
                m_lastSSnoConstraint = m_sumOfSquares;
                f1 = sumSquares(regFun, f);
                f[i] = hold0*(1.0 + m_delta);
                m_lastSSnoConstraint = m_sumOfSquares;
                f2 = sumSquares(regFun, f);
                m_grad[i, 0] = (m_fMin - f1)/Math.Abs(m_delta*hold0);
                m_grad[i, 1] = (f2 - m_fMin)/Math.Abs(m_delta*hold0);
            }

            // second patial derivatives at the minimum
            m_lastSSnoConstraint = m_sumOfSquares;
            for (int i = 0; i < np; ++i)
            {
                for (int j = 0; j < np; ++j)
                {
                    for (int k = 0; k < np; ++k)
                    {
                        f[k] = pmin[k];
                    }
                    hold0 = f[i];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[i];
                        m_zeroCheck = true;
                    }
                    f[i] = hold0*(1.0 + m_delta/2.0D);
                    hold0 = f[j];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[j];
                        m_zeroCheck = true;
                    }
                    f[j] = hold0*(1.0 + m_delta/2.0D);
                    m_lastSSnoConstraint = m_sumOfSquares;
                    f1 = sumSquares(regFun, f);
                    f[i] = pmin[i];
                    f[j] = pmin[j];
                    hold0 = f[i];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[i];
                        m_zeroCheck = true;
                    }
                    f[i] = hold0*(1.0 - m_delta/2.0D);
                    hold0 = f[j];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[j];
                        m_zeroCheck = true;
                    }
                    f[j] = hold0*(1.0 + m_delta/2.0D);
                    m_lastSSnoConstraint = m_sumOfSquares;
                    f2 = sumSquares(regFun, f);
                    f[i] = pmin[i];
                    f[j] = pmin[j];
                    hold0 = f[i];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[i];
                        m_zeroCheck = true;
                    }
                    f[i] = hold0*(1.0 + m_delta/2.0D);
                    hold0 = f[j];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[j];
                        m_zeroCheck = true;
                    }
                    f[j] = hold0*(1.0 - m_delta/2.0D);
                    m_lastSSnoConstraint = m_sumOfSquares;
                    f3 = sumSquares(regFun, f);
                    f[i] = pmin[i];
                    f[j] = pmin[j];
                    hold0 = f[i];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[i];
                        m_zeroCheck = true;
                    }
                    f[i] = hold0*(1.0 - m_delta/2.0D);
                    hold0 = f[j];
                    if (hold0 == 0.0D)
                    {
                        hold0 = m_stepH[j];
                        m_zeroCheck = true;
                    }
                    f[j] = hold0*(1.0 - m_delta/2.0D);
                    m_lastSSnoConstraint = m_sumOfSquares;
                    f4 = sumSquares(regFun, f);
                    stat[i, j] = (f1 - f2 - f3 + f4)/(m_delta*m_delta);
                }
            }

            double ss = 0.0D;
            double sc = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                for (int j = 0; j < m_nXarrays; j++)
                {
                    xd[j] = m_xData[j, i];
                }
                if (m_multipleY)
                {
                    m_yCalc[i] = ((RegressionFunction2) regFun).function(pmin, xd, i);
                }
                else
                {
                    m_yCalc[i] = ((RegressionFunction) regFun).function(pmin, xd);
                }
                m_residual[i] = m_yCalc[i] - m_yData[i];
                ss += Fmath.square(m_residual[i]);
                m_residualW[i] = m_residual[i]/m_weight[i];
                sc += Fmath.square(m_residualW[i]);
            }
            m_sumOfSquares = ss;
            double varY = ss/(m_nData - np);
            double sdY = Math.Sqrt(varY);
            if (m_weightOpt || m_trueFreq)
            {
                m_chiSquare = sc;
                m_reducedChiSquare = sc/(m_nData - np);
            }

            // calculate reduced sum of squares
            double red = 1.0D;
            if (!m_weightOpt && !m_trueFreq)
            {
                red = m_sumOfSquares/(m_nData - np);
            }

            // calculate pseudo errors  -  reduced sum of squares over second partial derivative
            for (int i = 0; i < np; i++)
            {
                m_pseudoSd[i] = (2.0D*m_delta*red*Math.Abs(pmin[i]))/(m_grad[i, 1] - m_grad[i, 0]);
                if (m_pseudoSd[i] >= 0.0D)
                {
                    m_pseudoSd[i] = Math.Sqrt(m_pseudoSd[i]);
                }
                else
                {
                    m_pseudoSd[i] = double.NaN;
                }
            }

            // calculate covariance matrix
            if (np == 1)
            {
                hold0 = pmin[0];
                if (hold0 == 0.0D)
                {
                    hold0 = m_stepH[0];
                }
                stat[0, 0] = 1.0D/stat[0, 0];
                m_covar[0, 0] = stat[0, 0]*red*hold0*hold0;
                if (m_covar[0, 0] >= 0.0D)
                {
                    coeffSd[0] = Math.Sqrt(m_covar[0, 0]);
                    m_corrCoeff[0, 0] = 1.0D;
                }
                else
                {
                    coeffSd[0] = double.NaN;
                    m_corrCoeff[0, 0] = double.NaN;
                    m_posVarFlag = false;
                }
            }
            else
            {
                MatrixExtended cov = new MatrixExtended(stat);
                if (m_blnSupressErrorMessages)
                {
                    cov.supressErrorMessage();
                }
                double determinant = cov.determinant();
                if (determinant == 0)
                {
                    m_invertFlag = false;
                }
                else
                {
                    cov = cov.inverse();
                    m_invertFlag = cov.getMatrixCheck();
                }
                if (m_invertFlag == false)
                {
                    flag--;
                }
                stat = cov.getArrayCopy();

                m_posVarFlag = true;
                if (m_invertFlag)
                {
                    for (int i = 0; i < np; ++i)
                    {
                        hold0 = pmin[i];
                        if (hold0 == 0.0D)
                        {
                            hold0 = m_stepH[i];
                        }
                        for (int j = i; j < np; ++j)
                        {
                            hold1 = pmin[j];
                            if (hold1 == 0.0D)
                            {
                                hold1 = m_stepH[j];
                            }
                            m_covar[i, j] = 2.0D*stat[i, j]*red*hold0*hold1;
                            m_covar[j, i] = m_covar[i, j];
                        }
                        if (m_covar[i, i] >= 0.0D)
                        {
                            coeffSd[i] = Math.Sqrt(m_covar[i, i]);
                        }
                        else
                        {
                            coeffSd[i] = double.NaN;
                            m_posVarFlag = false;
                        }
                    }

                    for (int i = 0; i < np; ++i)
                    {
                        for (int j = 0; j < np; ++j)
                        {
                            if ((coeffSd[i] != double.NaN) && (coeffSd[j] != double.NaN))
                            {
                                m_corrCoeff[i, j] = m_covar[i, j]/(coeffSd[i]*coeffSd[j]);
                            }
                            else
                            {
                                m_corrCoeff[i, j] = double.NaN;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < np; ++i)
                    {
                        for (int j = 0; j < np; ++j)
                        {
                            m_covar[i, j] = double.NaN;
                            m_corrCoeff[i, j] = double.NaN;
                        }
                        coeffSd[i] = double.NaN;
                    }
                }
            }
            if (m_posVarFlag == false)
            {
                flag--;
            }

            for (int i = 0; i < m_nTerms; i++)
            {
                bestSd[i] = coeffSd[i];
                m_tValues[i] = m_best[i]/bestSd[i];
                double atv = Math.Abs(m_tValues[i]);
                m_pValues[i] = 1.0 - Stat.studentTcdf(-atv, atv, m_degreesOfFreedom);
            }

            multCorrelCoeff(m_yData, m_yCalc, m_weight);

            return flag;
        }

        // Print the results of the regression
        // File name provided
        // prec = truncation precision
        public void print(string filename, int prec)
        {
            m_prec = prec;
            print(filename);
        }

        // Print the results of the regression
        // No file name provided
        // prec = truncation precision
        public void print(int prec)
        {
            m_prec = prec;
            string filename = "RegressionOutput.txt";
            print(filename);
        }

        // Print the results of the regression
        // File name provided
        // default value for truncation precision
        public void print(string filename)
        {
            if (filename.IndexOf('.') == -1)
            {
                filename = filename + ".txt";
            }
            FileOutput fout = new FileOutput(filename, 'n');
            fout.dateAndTimeln(filename);
            PrintToScreen.WriteLine(m_graphTitle);
            m_paraName = new string[m_nTerms];
            if (m_lastMethod == 38)
            {
                m_paraName = new string[3];
            }
            if (m_weightOpt)
            {
                PrintToScreen.WriteLine("Weighted Least Squares Minimisation");
            }
            else
            {
                PrintToScreen.WriteLine("Unweighted Least Squares Minimisation");
            }
            switch (m_lastMethod)
            {
                case 0:
                    PrintToScreen.WriteLine("Linear Regression with intercept");
                    PrintToScreen.WriteLine("y = c[0] + c[1]*x1 + c[2]*x2 +c[3]*x3 + . . .");
                    for (int i = 0; i < m_nTerms; i++)
                    {
                        m_paraName[i] = "c[" + i + "]";
                    }
                    linearPrint(fout);
                    break;
                case 1:
                    PrintToScreen.WriteLine("Polynomial (with degree = " + (m_nTerms - 1) +
                                            ") Fitting Linear Regression");
                    PrintToScreen.WriteLine("y = c[0] + c[1]*x + c[2]*x^2 +c[3]*x^3 + . . .");
                    for (int i = 0; i < m_nTerms; i++)
                    {
                        m_paraName[i] = "c[" + i + "]";
                    }
                    linearPrint(fout);
                    break;
                case 2:
                    PrintToScreen.WriteLine("Generalised linear regression");
                    PrintToScreen.WriteLine("y = c[0]*f1(x) + c[1]*f2(x) + c[2]*f3(x) + . . .");
                    for (int i = 0; i < m_nTerms; i++)
                    {
                        m_paraName[i] = "c[" + i + "]";
                    }
                    linearPrint(fout);
                    break;
                case 3:
                    PrintToScreen.WriteLine("Nelder and Mead Simplex Non-linear Regression");
                    PrintToScreen.WriteLine("y = f(x1, x2, x3 . . ., c[0], c[1], c[2] . . .");
                    PrintToScreen.WriteLine("y is non-linear with respect to the c[i]");
                    for (int i = 0; i < m_nTerms; i++)
                    {
                        m_paraName[i] = "c[" + i + "]";
                    }
                    nonLinearPrint(fout);
                    break;
                case 4:
                    PrintToScreen.WriteLine("Fitting to a Normal (Gaussian) distribution");
                    PrintToScreen.WriteLine("y = (yscale/(m_sd.Sqrt(2.pi)).Exp(0.5.square((x-m_mean)/m_sd))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mean";
                    m_paraName[1] = "m_sd";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 5:
                    PrintToScreen.WriteLine("Fitting to a Lorentzian distribution");
                    PrintToScreen.WriteLine("y = (yscale/pi).(gamma/2)/((x-m_mean)^2+(gamma/2)^2)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mean";
                    m_paraName[1] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 6:
                    PrintToScreen.WriteLine("Fitting to a Poisson distribution");
                    PrintToScreen.WriteLine("y = yscale.m_mu^k.Exp(-m_mu)/m_mu!");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mean";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 7:
                    PrintToScreen.WriteLine(
                        "Fitting to a Two Parameter Minimum Order Statistic Gumbel [Type 1 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = (yscale/m_sigma)*Exp((x - m_mu)/m_sigma))*Exp(-Exp((x-m_mu)/m_sigma))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 8:
                    PrintToScreen.WriteLine(
                        "Fitting to a Two Parameter Maximum Order Statistic Gumbel [Type 1 Extreme Value] Distribution");
                    PrintToScreen.WriteLine(
                        "y = (yscale/m_sigma)*Exp(-(x - m_mu)/m_sigma))*Exp(-Exp(-(x-m_mu)/m_sigma))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 9:
                    PrintToScreen.WriteLine(
                        "Fitting to a One Parameter Minimum Order Statistic Gumbel [Type 1 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = (yscale)*Exp(x/m_sigma))*Exp(-Exp(x/m_sigma))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 10:
                    PrintToScreen.WriteLine(
                        "Fitting to a One Parameter Maximum Order Statistic Gumbel [Type 1 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = (yscale)*Exp(-x/m_sigma))*Exp(-Exp(-x/m_sigma))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 11:
                    PrintToScreen.WriteLine(
                        "Fitting to a Standard Minimum Order Statistic Gumbel [Type 1 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = (yscale)*Exp(x))*Exp(-Exp(x))");
                    PrintToScreen.WriteLine("Linear regression used to fit y = yscale*z where z = Exp(x))*Exp(-Exp(x)))");
                    if (m_scaleFlag)
                    {
                        m_paraName[0] = "y scale";
                    }
                    linearPrint(fout);
                    break;
                case 12:
                    PrintToScreen.WriteLine(
                        "Fitting to a Standard Maximum Order Statistic Gumbel [Type 1 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = (yscale)*Exp(-x))*Exp(-Exp(-x))");
                    PrintToScreen.WriteLine(
                        "Linear regression used to fit y = yscale*z where z = Exp(-x))*Exp(-Exp(-x)))");
                    if (m_scaleFlag)
                    {
                        m_paraName[0] = "y scale";
                    }
                    linearPrint(fout);
                    break;
                case 13:
                    PrintToScreen.WriteLine("Fitting to a Three Parameter Frechet [Type 2 Extreme Value] Distribution");
                    PrintToScreen.WriteLine(
                        "y = yscale.(gamma/m_sigma)*((x - m_mu)/m_sigma)^(-gamma-1)*Exp(-((x-m_mu)/m_sigma)^-gamma");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "m_sigma";
                    m_paraName[2] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[3] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 14:
                    PrintToScreen.WriteLine("Fitting to a Two parameter Frechet [Type2  Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = yscale.(gamma/m_sigma)*(x/m_sigma)^(-gamma-1)*Exp(-(x/m_sigma)^-gamma");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_sigma";
                    m_paraName[1] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 15:
                    PrintToScreen.WriteLine("Fitting to a Standard Frechet [Type 2 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = yscale.gamma*(x)^(-gamma-1)*Exp(-(x)^-gamma");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 16:
                    PrintToScreen.WriteLine("Fitting to a Three parameter Weibull [Type 3 Extreme Value] Distribution");
                    PrintToScreen.WriteLine(
                        "y = yscale.(gamma/m_sigma)*((x - m_mu)/m_sigma)^(gamma-1)*Exp(-((x-m_mu)/m_sigma)^gamma");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "m_sigma";
                    m_paraName[2] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[3] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 17:
                    PrintToScreen.WriteLine("Fitting to a Two parameter Weibull [Type 3 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = yscale.(gamma/m_sigma)*(x/m_sigma)^(gamma-1)*Exp(-(x/m_sigma)^gamma");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_sigma";
                    m_paraName[1] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 18:
                    PrintToScreen.WriteLine("Fitting to a Standard Weibull [Type 3 Extreme Value] Distribution");
                    PrintToScreen.WriteLine("y = yscale.gamma*(x)^(gamma-1)*Exp(-(x)^gamma");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 19:
                    PrintToScreen.WriteLine("Fitting to a Two parameter Exponential Distribution");
                    PrintToScreen.WriteLine("y = (yscale/m_sigma)*Exp(-(x-m_mu)/m_sigma)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 20:
                    PrintToScreen.WriteLine("Fitting to a One parameter Exponential Distribution");
                    PrintToScreen.WriteLine("y = (yscale/m_sigma)*Exp(-x/m_sigma)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 21:
                    PrintToScreen.WriteLine("Fitting to a Standard Exponential Distribution");
                    PrintToScreen.WriteLine("y = yscale*Exp(-x)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    if (m_scaleFlag)
                    {
                        m_paraName[0] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 22:
                    PrintToScreen.WriteLine("Fitting to a Rayleigh Distribution");
                    PrintToScreen.WriteLine("y = (yscale/m_sigma)*(x/m_sigma)*Exp(-0.5*(x/m_sigma)^2)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 23:
                    PrintToScreen.WriteLine("Fitting to a Two Parameter Pareto Distribution");
                    PrintToScreen.WriteLine("y = yscale*(alpha*beta^alpha)/(x^(alpha+1))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "alpha";
                    m_paraName[1] = "beta";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 24:
                    PrintToScreen.WriteLine("Fitting to a One Parameter Pareto Distribution");
                    PrintToScreen.WriteLine("y = yscale*(alpha)/(x^(alpha+1))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "alpha";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 25:
                    PrintToScreen.WriteLine("Fitting to a Sigmoidal Threshold Function");
                    PrintToScreen.WriteLine("y = yscale/(1 + Exp(-slopeTerm(x - theta)))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "slope term";
                    m_paraName[1] = "theta";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 26:
                    PrintToScreen.WriteLine("Fitting to a Rectangular Hyperbola");
                    PrintToScreen.WriteLine("y = yscale.x/(theta + x)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "theta";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 27:
                    PrintToScreen.WriteLine("Fitting to a Scaled Heaviside Step Function");
                    PrintToScreen.WriteLine("y = yscale.H(x - theta)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "theta";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 28:
                    PrintToScreen.WriteLine("Fitting to a Hill/Sips Sigmoid");
                    PrintToScreen.WriteLine("y = yscale.x^n/(theta^n + x^n)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "theta";
                    m_paraName[1] = "n";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 29:
                    PrintToScreen.WriteLine("Fitting to a Shifted Pareto Distribution");
                    PrintToScreen.WriteLine("y = yscale*(alpha*beta^alpha)/((x-theta)^(alpha+1))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "alpha";
                    m_paraName[1] = "beta";
                    m_paraName[2] = "theta";
                    if (m_scaleFlag)
                    {
                        m_paraName[3] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 30:
                    PrintToScreen.WriteLine("Fitting to a Logistic distribution");
                    PrintToScreen.WriteLine("y = yscale*Exp(-(x-m_mu)/beta)/(beta*(1 + Exp(-(x-m_mu)/beta))^2");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "beta";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 31:
                    PrintToScreen.WriteLine("Fitting to a Beta distribution - [0, 1] interval");
                    PrintToScreen.WriteLine("y = yscale*x^(alpha-1)*(1-x)^(beta-1)/B(alpha, beta)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "alpha";
                    m_paraName[1] = "beta";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 32:
                    PrintToScreen.WriteLine("Fitting to a Beta distribution - [Min, Max] interval");
                    PrintToScreen.WriteLine(
                        "y = yscale*(x-Min)^(alpha-1)*(Max-x)^(beta-1)/(B(alpha, beta)*(Max-Min)^(alpha+beta-1)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "alpha";
                    m_paraName[1] = "beta";
                    m_paraName[2] = "Min";
                    m_paraName[3] = "Max";
                    if (m_scaleFlag)
                    {
                        m_paraName[4] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 33:
                    PrintToScreen.WriteLine("Fitting to a Three Parameter Gamma distribution");
                    PrintToScreen.WriteLine(
                        "y = yscale*((x-m_mu)/beta)^(gamma-1)*Exp(-(x-m_mu)/beta)/(beta*Gamma(gamma))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "beta";
                    m_paraName[2] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[3] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 34:
                    PrintToScreen.WriteLine("Fitting to a Standard Gamma distribution");
                    PrintToScreen.WriteLine("y = yscale*x^(gamma-1)*Exp(-x)/Gamma(gamma)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 35:
                    PrintToScreen.WriteLine("Fitting to an Erang distribution");
                    PrintToScreen.WriteLine("y = yscale*lambda^k*x^(k-1)*Exp(-x*lambda)/(k-1)!");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "lambda";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 36:
                    PrintToScreen.WriteLine("Fitting to a two parameter log-normal distribution");
                    PrintToScreen.WriteLine("y = (yscale/(x.m_sigma.Sqrt(2.pi)).Exp(0.5.square((Log(x)-muu)/m_sigma))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mu";
                    m_paraName[1] = "m_sigma";
                    if (m_scaleFlag)
                    {
                        m_paraName[2] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 37:
                    PrintToScreen.WriteLine("Fitting to a three parameter log-normal distribution");
                    PrintToScreen.WriteLine(
                        "y = (yscale/((x-alpha).beta.Sqrt(2.pi)).Exp(0.5.square((Log(x-alpha)/gamma)/beta))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "alpha";
                    m_paraName[1] = "beta";
                    m_paraName[2] = "gamma";
                    if (m_scaleFlag)
                    {
                        m_paraName[3] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 38:
                    PrintToScreen.WriteLine("Fitting to a Normal (Gaussian) distribution with fixed_ parameters");
                    PrintToScreen.WriteLine("y = (yscale/(m_sd.Sqrt(2.pi)).Exp(0.5.square((x-m_mean)/m_sd))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "m_mean";
                    m_paraName[1] = "m_sd";
                    m_paraName[2] = "y scale";
                    nonLinearPrint(fout);
                    break;
                case 39:
                    PrintToScreen.WriteLine("Fitting to a EC50 dose response curve");
                    PrintToScreen.WriteLine("y = bottom + (top - bottom)/(1 + (x/EC50)^HillSlope)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "bottom";
                    m_paraName[1] = "top";
                    m_paraName[2] = "EC50";
                    m_paraName[3] = "Hill Slope";
                    nonLinearPrint(fout);
                    break;
                case 40:
                    PrintToScreen.WriteLine("Fitting to a LogEC50 dose response curve");
                    PrintToScreen.WriteLine("y = bottom + (top - bottom)/(1 + 10^((logEC50 - x).HillSlope))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "bottom";
                    m_paraName[1] = "top";
                    m_paraName[2] = "LogEC50";
                    m_paraName[3] = "Hill Slope";
                    nonLinearPrint(fout);
                    break;
                case 41:
                    PrintToScreen.WriteLine(
                        "Fitting to a EC50 dose response curve - bottom constrained to be zero or positive");
                    PrintToScreen.WriteLine("y = bottom + (top - bottom)/(1 + (x/EC50)^HillSlope)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "bottom";
                    m_paraName[1] = "top";
                    m_paraName[2] = "EC50";
                    m_paraName[3] = "Hill Slope";
                    nonLinearPrint(fout);
                    break;
                case 42:
                    PrintToScreen.WriteLine(
                        "Fitting to a LogEC50 dose response curve - bottom constrained to be zero or positive");
                    PrintToScreen.WriteLine("y = bottom + (top - bottom)/(1 + 10^((logEC50 - x).HillSlope))");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "bottom";
                    m_paraName[1] = "top";
                    m_paraName[2] = "LogEC50";
                    m_paraName[3] = "Hill Slope";
                    nonLinearPrint(fout);
                    break;
                case 43:
                    PrintToScreen.WriteLine("Fitting to an exponential");
                    PrintToScreen.WriteLine("y = yscale.Exp(A.x)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "A";
                    if (m_scaleFlag)
                    {
                        m_paraName[1] = "y scale";
                    }
                    nonLinearPrint(fout);
                    break;
                case 44:
                    PrintToScreen.WriteLine("Fitting to multiple exponentials");
                    PrintToScreen.WriteLine("y = Sum[Ai.Exp(Bi.x)], i=1 to n");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    for (int i = 0; i < m_nTerms; i += 2)
                    {
                        m_paraName[i] = "A[" + (i + 1) + "]";
                        m_paraName[i + 1] = "B[" + (i + 1) + "]";
                    }
                    nonLinearPrint(fout);
                    break;
                case 45:
                    PrintToScreen.WriteLine("Fitting to one minus an exponential");
                    PrintToScreen.WriteLine("y = A(1 - Exp(B.x)");
                    PrintToScreen.WriteLine("Nelder and Mead Simplex used to fit the data");
                    m_paraName[0] = "A";
                    m_paraName[1] = "B";
                    nonLinearPrint(fout);
                    break;

                default:
                    throw new HCException("Method number (lastMethod) not found");
            }

            fout.close();
        }

        // Print the results of the regression
        // No file name provided
        public void print()
        {
            string filename = "RegressOutput.txt";
            print(filename);
        }

        // protected method - print linear regression output
        protected void linearPrint(FileOutput fout)
        {
            if (m_legendCheck)
            {
                fout.println();
                fout.println("x1 = " + m_xLegend);
                fout.println("y  = " + m_yLegend);
            }

            fout.println();
            fout.printtab(" ", m_field);
            fout.printtab("Best", m_field);
            fout.printtab("Error", m_field);
            fout.printtab("Coefficient of", m_field);
            fout.printtab("t-value  ", m_field);
            fout.println("p-value");

            fout.printtab(" ", m_field);
            fout.printtab("Estimate", m_field);
            fout.printtab("        ", m_field);
            fout.printtab("variation (%)", m_field);
            fout.printtab("t ", m_field);
            fout.println("P > |t|");

            for (int i = 0; i < m_nTerms; i++)
            {
                fout.printtab(m_paraName[i], m_field);
                fout.printtab(Fmath.truncate(m_best[i], m_prec), m_field);
                fout.printtab(Fmath.truncate(bestSd[i], m_prec), m_field);
                fout.printtab(Fmath.truncate(Math.Abs(bestSd[i]*100.0D/m_best[i]), m_prec), m_field);
                fout.printtab(Fmath.truncate(m_tValues[i], m_prec), m_field);
                fout.println(Fmath.truncate((m_pValues[i]), m_prec));
            }
            fout.println();

            int ii = 0;
            if (m_lastMethod < 2)
            {
                ii = 1;
            }
            for (int i = 0; i < m_nXarrays; i++)
            {
                fout.printtab("x" + (i + ii), m_field);
            }
            fout.printtab("y(expl)", m_field);
            fout.printtab("y(calc)", m_field);
            fout.printtab("weight", m_field);
            fout.printtab("residual", m_field);
            fout.println("residual");

            for (int i = 0; i < m_nXarrays; i++)
            {
                fout.printtab(" ", m_field);
            }
            fout.printtab(" ", m_field);
            fout.printtab(" ", m_field);
            fout.printtab(" ", m_field);
            fout.printtab("(unweighted)", m_field);
            fout.println("(weighted)");


            for (int i = 0; i < m_nData; i++)
            {
                for (int j = 0; j < m_nXarrays; j++)
                {
                    fout.printtab(Fmath.truncate(m_xData[j, i], m_prec), m_field);
                }
                fout.printtab(Fmath.truncate(m_yData[i], m_prec), m_field);
                fout.printtab(Fmath.truncate(m_yCalc[i], m_prec), m_field);
                fout.printtab(Fmath.truncate(m_weight[i], m_prec), m_field);
                fout.printtab(Fmath.truncate(m_residual[i], m_prec), m_field);
                fout.println(Fmath.truncate(m_residualW[i], m_prec));
            }
            fout.println();
            fout.println("Sum of squares " + Fmath.truncate(m_sumOfSquares, m_prec));
            if (m_trueFreq)
            {
                fout.printtab("Chi Square (Poissonian bins)");
                fout.println(Fmath.truncate(m_chiSquare, m_prec));
                fout.printtab("Reduced Chi Square (Poissonian bins)");
                fout.println(Fmath.truncate(m_reducedChiSquare, m_prec));
                fout.printtab("Chi Square (Poissonian bins) Probability");
                fout.println(Fmath.truncate((1.0D - Stat.chiSquareProb(m_chiSquare, m_nData - m_nXarrays)), m_prec));
            }
            else
            {
                if (m_weightOpt)
                {
                    fout.printtab("Chi Square");
                    fout.println(Fmath.truncate(m_chiSquare, m_prec));
                    fout.printtab("Reduced Chi Square");
                    fout.println(Fmath.truncate(m_reducedChiSquare, m_prec));
                    fout.printtab("Chi Square Probability");
                    fout.println(Fmath.truncate(getchiSquareProb(), m_prec));
                }
            }
            fout.println(" ");
            fout.println("Correlation: x - y data");
            if (m_nXarrays > 1)
            {
                fout.printtab(m_weightWord[m_weightFlag] + "Multiple Sample Correlation Coefficient (R)");
                fout.println(Fmath.truncate(m_sampleR, m_prec));
                fout.printtab(m_weightWord[m_weightFlag] + "Multiple Sample Correlation Coefficient Squared (R^2)");
                fout.println(Fmath.truncate(m_sampleR2, m_prec));
                if (m_sampleR2 <= 1.0D)
                {
                    fout.printtab(m_weightWord[m_weightFlag] + "Multiple Correlation Coefficient F-test ratio");
                    fout.println(Fmath.truncate(m_multipleF, m_prec));
                    fout.printtab(m_weightWord[m_weightFlag] + "Multiple Correlation Coefficient F-test probability");
                    fout.println(Fmath.truncate(Stat.fTestProb(m_multipleF, m_nXarrays - 1, m_nData - m_nXarrays),
                                                m_prec));
                }
                fout.printtab(m_weightWord[m_weightFlag] + "Adjusted Multiple Sample Correlation Coefficient (adjR)");
                fout.println(Fmath.truncate(m_adjustedR, m_prec));
                fout.printtab(m_weightWord[m_weightFlag] +
                              "Adjusted Multiple Sample Correlation Coefficient Squared (adjR*adjR)");
                fout.println(Fmath.truncate(m_adjustedR2, m_prec));
                if (m_sampleR2 <= 1.0D)
                {
                    fout.printtab(m_weightWord[m_weightFlag] + "Adjusted Multiple Correlation Coefficient F-test ratio");
                    fout.println(Fmath.truncate(m_adjustedF, m_prec));
                    fout.printtab(m_weightWord[m_weightFlag] +
                                  "Adjusted Multiple Correlation Coefficient F-test probability");
                    fout.println(Fmath.truncate(Stat.fTestProb(m_adjustedF, m_nXarrays - 1, m_nData - m_nXarrays),
                                                m_prec));
                }
            }
            else
            {
                fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient (R)");
                fout.println(Fmath.truncate(m_sampleR, m_prec));
                fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient Squared (R^2)");
                fout.println(Fmath.truncate(m_sampleR2, m_prec));
                if (m_sampleR2 <= 1.0D)
                {
                    fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient Probability");
                    fout.println(Fmath.truncate(Stat.linearCorrCoeffProb(m_sampleR, m_nData - m_nTerms), m_prec));
                }
            }

            fout.println(" ");
            fout.println("Correlation: y(experimental) - y(calculated");
            fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient");
            double ccyy = Stat.CorrCoeff(m_yData, m_yCalc, m_weight);

            fout.println(Fmath.truncate(ccyy, m_prec));
            fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient Probability");
            fout.println(Fmath.truncate(Stat.linearCorrCoeffProb(ccyy, m_nData - 1), m_prec));


            fout.println(" ");
            fout.printtab("Degrees of freedom");
            fout.println(m_nData - m_nTerms);
            fout.printtab("Number of data points");
            fout.println(m_nData);
            fout.printtab("Number of estimated paramaters");
            fout.println(m_nTerms);

            fout.println();
            if (m_chiSquare != 0.0D)
            {
                fout.println("Correlation coefficients");
                fout.printtab(" ", m_field);
                for (int i = 0; i < m_nTerms; i++)
                {
                    fout.printtab(m_paraName[i], m_field);
                }
                fout.println();

                for (int j = 0; j < m_nTerms; j++)
                {
                    fout.printtab(m_paraName[j], m_field);
                    for (int i = 0; i < m_nTerms; i++)
                    {
                        fout.printtab(
                            Fmath.truncate(
                                m_corrCoeff[i, j],
                                m_prec),
                            m_field);
                    }
                    fout.println();
                }
            }

            fout.println();
            fout.println("End of file");

            fout.close();
        }

        // protected method - print non-linear regression output
        protected void nonLinearPrint(FileOutput fout)
        {
            if (m_userSupplied)
            {
                fout.println();
                fout.println("Initial estimates were supplied by the user");
            }
            else
            {
                fout.println("Initial estimates were calculated internally");
            }

            switch (m_scaleOpt)
            {
                case 1:
                    fout.println();
                    fout.println("Initial estimates were scaled to unity within the regression");
                    break;
                case 2:
                    fout.println();
                    fout.println(
                        "Initial estimates were scaled with user supplied scaling factors within the regression");
                    break;
            }

            if (m_legendCheck)
            {
                fout.println();
                fout.println("x1 = " + m_xLegend);
                fout.println("y  = " + m_yLegend);
            }

            fout.println();
            if (!m_nlrStatus)
            {
                fout.println("Convergence criterion was not satisfied");
                fout.println(
                    "The following results are, or a derived from, the current estimates on exiting the regression method");
                fout.println();
            }

            fout.println("Estimated parameters");
            fout.println(
                "The statistics are obtained assuming that the model behaves as a linear model about the minimum.");
            fout.println(
                "The Hessian matrix is calculated as the numerically derived second derivatives of chi square with respect to all pairs of parameters.");
            if (m_zeroCheck)
            {
                fout.println(
                    "The best estimate/s equal to zero were replaced by the step size in the numerical differentiation!!!");
            }
            fout.println("Consequentlty treat the statistics with great caution");
            if (!m_posVarFlag)
            {
                fout.println("Covariance matrix contains at least one negative diagonal element");
                fout.println(" - all variances are dubious");
                fout.println(" - may not be at a minimum");
            }
            if (!m_invertFlag)
            {
                fout.println("Hessian matrix is singular");
                fout.println(" - variances cannot be calculated");
                fout.println(" - may not be at a minimum");
            }

            fout.println(" ");
            if (!m_scaleFlag)
            {
                fout.println("The ordinate scaling factor [yscale, Ao] has been set equal to " + m_yScaleFactor);
                fout.println(" ");
            }
            if (m_lastMethod == 35)
            {
                fout.println(
                    "The integer rate parameter, k, was varied in unit steps to obtain a minimum sum of squares");
                fout.println("This value of k was " + m_kayValue);
                fout.println(" ");
            }

            fout.printtab(" ", m_field);
            if (m_invertFlag)
            {
                fout.printtab("Best", m_field);
                fout.printtab("Estimate of", m_field);
                fout.printtab("Coefficient", m_field);
                fout.printtab("t-value", m_field);
                fout.println("p-value");
            }
            else
            {
                fout.println("Best");
            }

            if (m_invertFlag)
            {
                fout.printtab(" ", m_field);
                fout.printtab("estimate", m_field);
                fout.printtab("the error", m_field);
                fout.printtab("of", m_field);
                fout.printtab("t", m_field);
                fout.println("P > |t|");
            }
            else
            {
                fout.printtab(" ", m_field);
                fout.println("estimate");
            }

            if (m_invertFlag)
            {
                fout.printtab(" ", m_field);
                fout.printtab(" ", m_field);
                fout.printtab(" ", m_field);
                fout.println("variation (%)");
            }
            else
            {
                fout.println("   ");
            }

            if (m_lastMethod == 38)
            {
                int nT = 3;
                int ii = 0;
                for (int i = 0; i < nT; i++)
                {
                    fout.printtab(m_paraName[i], m_field);
                    if (m_fixed_[i])
                    {
                        fout.printtab(m_values[i]);
                        fout.println(" fixed parameter");
                    }
                    else
                    {
                        if (m_invertFlag)
                        {
                            fout.printtab(Fmath.truncate(m_best[ii], m_prec), m_field);
                            fout.printtab(Fmath.truncate(bestSd[ii], m_prec), m_field);
                            fout.printtab(Fmath.truncate(Math.Abs(bestSd[ii]*100.0D/m_best[ii]), m_prec), m_field);
                            fout.printtab(Fmath.truncate(m_tValues[ii], m_prec), m_field);
                            fout.println(Fmath.truncate(m_pValues[ii], m_prec));
                        }
                        else
                        {
                            fout.println(Fmath.truncate(m_best[ii], m_prec));
                        }
                        ii++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_nTerms; i++)
                {
                    if (m_invertFlag)
                    {
                        fout.printtab(m_paraName[i], m_field);
                        fout.printtab(Fmath.truncate(m_best[i], m_prec), m_field);
                        fout.printtab(Fmath.truncate(bestSd[i], m_prec), m_field);
                        fout.printtab(Fmath.truncate(Math.Abs(bestSd[i]*100.0D/m_best[i]), m_prec), m_field);
                        fout.printtab(Fmath.truncate(m_tValues[i], m_prec), m_field);
                        fout.println(Fmath.truncate(m_pValues[i], m_prec));
                    }
                    else
                    {
                        fout.printtab(m_paraName[i], m_field);
                        fout.println(Fmath.truncate(m_best[i], m_prec));
                    }
                }
            }
            fout.println();

            fout.printtab(" ", m_field);
            fout.printtab("Best", m_field);
            fout.printtab("Pre-Min", m_field);
            fout.printtab("Post-Min", m_field);
            fout.printtab("Initial", m_field);
            fout.printtab("Fractional", m_field);
            fout.println("Scaling");

            fout.printtab(" ", m_field);
            fout.printtab("estimate", m_field);
            fout.printtab("gradient", m_field);
            fout.printtab("gradient", m_field);
            fout.printtab("estimate", m_field);
            fout.printtab("step", m_field);
            fout.println("factor");


            if (m_lastMethod == 38)
            {
                int nT = 3;
                int ii = 0;
                for (int i = 0; i < nT; i++)
                {
                    fout.printtab(m_paraName[i], m_field);
                    if (m_fixed_[i])
                    {
                        fout.printtab(m_values[i]);
                        fout.println(" fixed parameter");
                    }
                    else
                    {
                        fout.printtab(Fmath.truncate(m_best[ii], m_prec), m_field);
                        fout.printtab(Fmath.truncate(m_grad[ii, 0], m_prec), m_field);
                        fout.printtab(Fmath.truncate(m_grad[ii, 1], m_prec), m_field);
                        fout.printtab(Fmath.truncate(m_startH[ii], m_prec), m_field);
                        fout.printtab(Fmath.truncate(m_stepH[ii], m_prec), m_field);
                        fout.println(Fmath.truncate(m_scale[ii], m_prec));
                        ii++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_nTerms; i++)
                {
                    fout.printtab(m_paraName[i], m_field);
                    fout.printtab(Fmath.truncate(m_best[i], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_grad[i, 0], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_grad[i, 1], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_startH[i], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_stepH[i], m_prec), m_field);
                    fout.println(Fmath.truncate(m_scale[i], m_prec));
                }
            }
            fout.println();


            ErrorProp ePeak = null;
            ErrorProp eYscale = null;
            if (m_scaleFlag)
            {
                switch (m_lastMethod)
                {
                    case 4:
                        ErrorProp eSigma = new ErrorProp(m_best[1], bestSd[1]);
                        eYscale = new ErrorProp(m_best[2]/Math.Sqrt(2.0D*Math.PI), bestSd[2]/Math.Sqrt(2.0D*Math.PI));
                        ePeak = eYscale.over(eSigma);
                        fout.printsp("Calculated estimate of the peak value = ");
                        fout.println(ErrorProp.truncate(ePeak, m_prec));
                        break;
                    case 5:
                        ErrorProp eGamma = new ErrorProp(m_best[1], bestSd[1]);
                        eYscale = new ErrorProp(2.0D*m_best[2]/Math.PI, 2.0D*bestSd[2]/Math.PI);
                        ePeak = eYscale.over(eGamma);
                        fout.printsp("Calculated estimate of the peak value = ");
                        fout.println(ErrorProp.truncate(ePeak, m_prec));
                        break;
                }
            }
            if (m_lastMethod == 25)
            {
                fout.printsp("Calculated estimate of the maximum gradient = ");
                if (m_scaleFlag)
                {
                    fout.println(Fmath.truncate(m_best[0]*m_best[2]/4.0D, m_prec));
                }
                else
                {
                    fout.println(Fmath.truncate(m_best[0]*m_yScaleFactor/4.0D, m_prec));
                }
            }
            if (m_lastMethod == 28)
            {
                fout.printsp("Calculated estimate of the maximum gradient = ");
                if (m_scaleFlag)
                {
                    fout.println(Fmath.truncate(m_best[1]*m_best[2]/(4.0D*m_best[0]), m_prec));
                }
                else
                {
                    fout.println(Fmath.truncate(m_best[1]*m_yScaleFactor/(4.0D*m_best[0]), m_prec));
                }
                fout.printsp("Calculated estimate of the Ka, i.e. theta raised to the power n = ");
                fout.println(Fmath.truncate(Math.Pow(m_best[0], m_best[1]), m_prec));
            }
            fout.println();

            int kk = 0;
            for (int j = 0; j < m_nYarrays; j++)
            {
                if (m_multipleY)
                {
                    fout.println("Y array " + j);
                }

                for (int i = 0; i < m_nXarrays; i++)
                {
                    fout.printtab("x" + i, m_field);
                }

                fout.printtab("y(expl)", m_field);
                fout.printtab("y(calc)", m_field);
                fout.printtab("weight", m_field);
                fout.printtab("residual", m_field);
                fout.println("residual");

                for (int i = 0; i < m_nXarrays; i++)
                {
                    fout.printtab(" ", m_field);
                }
                fout.printtab(" ", m_field);
                fout.printtab(" ", m_field);
                fout.printtab(" ", m_field);
                fout.printtab("(unweighted)", m_field);
                fout.println("(weighted)");
                for (int i = 0; i < m_nData0; i++)
                {
                    for (int jj = 0; jj < m_nXarrays; jj++)
                    {
                        fout.printtab(Fmath.truncate(m_xData[jj, kk], m_prec), m_field);
                    }
                    fout.printtab(Fmath.truncate(m_yData[kk], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_yCalc[kk], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_weight[kk], m_prec), m_field);
                    fout.printtab(Fmath.truncate(m_residual[kk], m_prec), m_field);
                    fout.println(Fmath.truncate(m_residualW[kk], m_prec));
                    kk++;
                }
                fout.println();
            }

            fout.printtab("Sum of squares of the unweighted residuals");
            fout.println(Fmath.truncate(m_sumOfSquares, m_prec));
            if (m_trueFreq)
            {
                fout.printtab("Chi Square (Poissonian bins)");
                fout.println(Fmath.truncate(m_chiSquare, m_prec));
                fout.printtab("Reduced Chi Square (Poissonian bins)");
                fout.println(Fmath.truncate(m_reducedChiSquare, m_prec));
                fout.printtab("Chi Square (Poissonian bins) Probability");
                fout.println(Fmath.truncate(1.0D - Stat.chiSquareProb(m_reducedChiSquare, m_degreesOfFreedom), m_prec));
            }
            else
            {
                if (m_weightOpt)
                {
                    fout.printtab("Chi Square");
                    fout.println(Fmath.truncate(m_chiSquare, m_prec));
                    fout.printtab("Reduced Chi Square");
                    fout.println(Fmath.truncate(m_reducedChiSquare, m_prec));
                    fout.printtab("Chi Square Probability");
                    fout.println(Fmath.truncate(getchiSquareProb(), m_prec));
                }
            }

            fout.println(" ");
            fout.println("Correlation: x - y data");
            if (m_nXarrays > 1)
            {
                fout.printtab(m_weightWord[m_weightFlag] + "Multiple Sample Correlation Coefficient (R)");
                fout.println(Fmath.truncate(m_sampleR, m_prec));
                fout.printtab(m_weightWord[m_weightFlag] + "Multiple Sample Correlation Coefficient Squared (R^2)");
                fout.println(Fmath.truncate(m_sampleR2, m_prec));
                if (m_sampleR2 <= 1.0D)
                {
                    fout.printtab(m_weightWord[m_weightFlag] + "Multiple Sample Correlation Coefficient F-test ratio");
                    fout.println(Fmath.truncate(m_multipleF, m_prec));
                    fout.printtab(m_weightWord[m_weightFlag] +
                                  "Multiple Sample Correlation Coefficient F-test probability");
                    fout.println(Stat.fTestProb(m_multipleF, m_nXarrays - 1, m_nData - m_nXarrays));
                }
                fout.printtab(m_weightWord[m_weightFlag] + "Adjusted Multiple Sample Correlation Coefficient (adjR)");
                fout.println(Fmath.truncate(m_adjustedR, m_prec));
                fout.printtab(m_weightWord[m_weightFlag] +
                              "Adjusted Multiple Sample Correlation Coefficient Squared (adjR*adjR)");
                fout.println(Fmath.truncate(m_adjustedR2, m_prec));
                if (m_sampleR2 <= 1.0D)
                {
                    fout.printtab(m_weightWord[m_weightFlag] + "Multiple Sample Correlation Coefficient F-test ratio");
                    fout.println(Fmath.truncate(m_adjustedF, m_prec));
                    fout.printtab(m_weightWord[m_weightFlag] +
                                  "Multiple Sample Correlation Coefficient F-test probability");
                    fout.println(Stat.fTestProb(m_adjustedF, m_nXarrays - 1, m_nData - m_nXarrays));
                }
            }
            else
            {
                fout.printtab(m_weightWord[m_weightFlag] + "Sample Correlation Coefficient (R)");
                fout.println(Fmath.truncate(m_sampleR, m_prec));
                fout.printtab(m_weightWord[m_weightFlag] + "Sample Correlation Coefficient Squared (R^2)");
                fout.println(Fmath.truncate(m_sampleR2, m_prec));
            }

            fout.println(" ");
            fout.println("Correlation: y(experimental) - y(calculated)");
            fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient");
            double ccyy = Stat.CorrCoeff(m_yData, m_yCalc, m_weight);
            fout.println(Fmath.truncate(ccyy, m_prec));
            fout.printtab(m_weightWord[m_weightFlag] + "Linear Correlation Coefficient Probability");
            fout.println(Fmath.truncate(Stat.linearCorrCoeffProb(ccyy, m_nData - 1), m_prec));

            fout.println(" ");
            fout.printtab("Degrees of freedom");
            fout.println(m_degreesOfFreedom);
            fout.printtab("Number of data points");
            fout.println(m_nData);
            fout.printtab("Number of estimated paramaters");
            fout.println(m_nTerms);

            fout.println();

            if (m_posVarFlag && m_invertFlag && m_chiSquare != 0.0D)
            {
                fout.println("Parameter - parameter correlation coefficients");
                fout.printtab(" ", m_field);
                for (int i = 0; i < m_nTerms; i++)
                {
                    fout.printtab(m_paraName[i], m_field);
                }
                fout.println();

                for (int j = 0; j < m_nTerms; j++)
                {
                    fout.printtab(m_paraName[j], m_field);
                    for (int i = 0; i < m_nTerms; i++)
                    {
                        fout.printtab(Fmath.truncate(m_corrCoeff[i, j], m_prec), m_field);
                    }
                    fout.println();
                }
                fout.println();
            }

            fout.println();
            fout.printtab("Number of iterations taken");
            fout.println(m_nIter);
            fout.printtab("Maximum number of iterations allowed");
            fout.println(m_nMax);
            fout.printtab("Number of restarts taken");
            fout.println(m_kRestart);
            fout.printtab("Maximum number of restarts allowed");
            fout.println(m_konvge);
            fout.printtab("Standard deviation of the simplex at the minimum");
            fout.println(Fmath.truncate(m_simplexSd, m_prec));
            fout.printtab("Convergence tolerance");
            fout.println(m_fTol);
            switch (m_minTest)
            {
                case 0:
                    fout.println("simplex m_sd < the tolerance times the m_mean of the absolute values of the y values");
                    break;
                case 1:
                    fout.println("simplex m_sd < the tolerance");
                    break;
                case 2:
                    fout.println("simplex m_sd < the tolerance times the square root(sum of squares/degrees of freedom");
                    break;
            }
            fout.println("Step used in numerical differentiation to obtain Hessian matrix");
            fout.println("d(parameter) = parameter*" + m_delta);

            fout.println();
            fout.println("End of file");
            fout.close();
        }

        // plot calculated y against experimental y
        // title provided
        public void plotYY(string title)
        {
            //graphTitle = title;
            //int ncurves = 2;
            //int npoints = nData0;
            //double[,] data = PlotGraph.data(ncurves, npoints);

            //int kk = 0;
            //for (int jj = 0; jj < nYarrays; jj++)
            //{

            //    // fill first curve with experimental versus best fit values
            //    for (int i = 0; i < nData0; i++)
            //    {
            //        data[0, i] = yData[kk];
            //        data[1, i] = yCalc[kk];
            //        kk++;
            //    }

            //    // Create a title
            //    string title0 = setGandPtitle(graphTitle);
            //    if (multipleY) title0 = title0 + "y array " + jj;
            //    string title1 = "Calculated versus experimental y values";

            //    // Calculate best fit straight line between experimental and best fit values
            //    Regression yyRegr = new Regression(yData, yCalc, weight);
            //    yyRegr.linear();
            //    double[] coef = yyRegr.getCoeff();
            //    data[2, 0] = Fmath.minimum(yData);
            //    data[3, 0] = coef[0] + coef[1] * data[2, 0];
            //    data[2, 1] = Fmath.maximum(yData);
            //    data[3, 1] = coef[0] + coef[1] * data[2, 1];

            //    PlotGraph pg = new PlotGraph(data);

            //    pg.setGraphTitle(title0);
            //    pg.setGraphTitle2(title1);
            //    pg.setXaxisLegend("Experimental y value");
            //    pg.setYaxisLegend("Calculated y value");
            //    int[] popt = { 1, 0 };
            //    pg.setPoint(popt);
            //    int[] lopt = { 0, 3 };
            //    pg.setLine(lopt);

            //    pg.plot();
            //}
        }

        //Creates a title
        protected string setGandPtitle(string title)
        {
            string title1 = "";
            switch (m_lastMethod)
            {
                case 0:
                    title1 = "Linear regression (with intercept): " + title;
                    break;
                case 1:
                    title1 = "Linear(polynomial with degree = " + (m_nTerms - 1) + ") regression: " + title;
                    break;
                case 2:
                    title1 = "General linear regression: " + title;
                    break;
                case 3:
                    title1 = "Non-linear (simplex) regression: " + title;
                    break;
                case 4:
                    title1 = "Fit to a Gaussian distribution: " + title;
                    break;
                case 5:
                    title1 = "Fit to a Lorentzian distribution: " + title;
                    break;
                case 6:
                    title1 = "Fit to a Poisson distribution: " + title;
                    break;
                case 7:
                    title1 = "Fit to a Two Parameter Minimum Order Statistic Gumbel distribution: " + title;
                    break;
                case 8:
                    title1 = "Fit to a two Parameter Maximum Order Statistic Gumbel distribution: " + title;
                    break;
                case 9:
                    title1 = "Fit to a One Parameter Minimum Order Statistic Gumbel distribution: " + title;
                    break;
                case 10:
                    title1 = "Fit to a One Parameter Maximum Order Statistic Gumbel distribution: " + title;
                    break;
                case 11:
                    title1 = "Fit to a Standard Minimum Order Statistic Gumbel distribution: " + title;
                    break;
                case 12:
                    title1 = "Fit to a Standard Maximum Order Statistic Gumbel distribution: " + title;
                    break;
                case 13:
                    title1 = "Fit to a Three Parameter Frechet distribution: " + title;
                    break;
                case 14:
                    title1 = "Fit to a Two Parameter Frechet distribution: " + title;
                    break;
                case 15:
                    title1 = "Fit to a Standard Frechet distribution: " + title;
                    break;
                case 16:
                    title1 = "Fit to a Three Parameter Weibull distribution: " + title;
                    break;
                case 17:
                    title1 = "Fit to a Two Parameter Weibull distribution: " + title;
                    break;
                case 18:
                    title1 = "Fit to a Standard Weibull distribution: " + title;
                    break;
                case 19:
                    title1 = "Fit to a Two Parameter Exponential distribution: " + title;
                    break;
                case 20:
                    title1 = "Fit to a One Parameter Exponential distribution: " + title;
                    break;
                case 21:
                    title1 = "Fit to a Standard exponential distribution: " + title;
                    break;
                case 22:
                    title1 = "Fit to a Rayleigh distribution: " + title;
                    break;
                case 23:
                    title1 = "Fit to a Two Parameter Pareto distribution: " + title;
                    break;
                case 24:
                    title1 = "Fit to a One Parameter Pareto distribution: " + title;
                    break;
                case 25:
                    title1 = "Fit to a Sigmoid Threshold Function: " + title;
                    break;
                case 26:
                    title1 = "Fit to a Rectangular Hyperbola: " + title;
                    break;
                case 27:
                    title1 = "Fit to a Scaled Heaviside Step Function: " + title;
                    break;
                case 28:
                    title1 = "Fit to a Hill/Sips Sigmoid: " + title;
                    break;
                case 29:
                    title1 = "Fit to a Shifted Pareto distribution: " + title;
                    break;
                case 30:
                    title1 = "Fit to a Logistic distribution: " + title;
                    break;
                case 31:
                    title1 = "Fit to a Beta distribution - interval [0, 1]: " + title;
                    break;
                case 32:
                    title1 = "Fit to a Beta distribution - interval [Min, Max]: " + title;
                    break;
                case 33:
                    title1 = "Fit to a Three Parameter Gamma distribution]: " + title;
                    break;
                case 34:
                    title1 = "Fit to a Standard Gamma distribution]: " + title;
                    break;
                case 35:
                    title1 = "Fit to an Erlang distribution]: " + title;
                    break;
                case 36:
                    title1 = "Fit to an two parameter log-normal distribution]: " + title;
                    break;
                case 37:
                    title1 = "Fit to an three parameter log-normal distribution]: " + title;
                    break;
                case 38:
                    title1 = "Fit to a Gaussian distribution with fixed_ parameters: " + title;
                    break;
                case 39:
                    title1 = "Fit to a EC50 dose response curve: " + title;
                    break;
                case 40:
                    title1 = "Fit to a LogEC50 dose response curve: " + title;
                    break;
                case 41:
                    title1 = "Fit to a EC50 dose response curve - bottom constrained [>= 0]: " + title;
                    break;
                case 42:
                    title1 = "Fit to a LogEC50 dose response curve - bottom constrained [>= 0]: " + title;
                    break;
                case 43:
                    title1 = "Fit to an exponential yscale.Exp(A.x): " + title;
                    break;
                case 44:
                    title1 = "Fit to multiple exponentials sum[Ai.Exp(Bi.x)]: " + title;
                    break;
                case 45:
                    title1 = "Fit to an exponential A.(1 - Exp(B.x): " + title;
                    break;

                default:
                    title1 = " " + title;
                    break;
            }
            return title1;
        }

        // plot calculated y against experimental y
        // no title provided
        public void plotYY()
        {
            plotYY(m_graphTitle);
        }

        // plot experimental x against experimental y and against calculated y
        // linear regression data
        // title provided
        protected int plotXY(string title)
        {
            m_graphTitle = title;
            int flag = 0;
            if (!m_linNonLin && m_nTerms > 0)
            {
                PrintToScreen.WriteLine(
                    "You attempted to use Regression.plotXY() for a non-linear regression without providing the function reference (pointer) in the plotXY argument list");
                PrintToScreen.WriteLine("No plot attempted");
                flag = -1;
                return flag;
            }
            flag = plotXYlinear(title);
            return flag;
        }

        // plot experimental x against experimental y and against calculated y
        // Linear regression data
        // no title provided
        public int plotXY()
        {
            int flag = plotXY(m_graphTitle);
            return flag;
        }

        // plot experimental x against experimental y and against calculated y
        // non-linear regression data
        // title provided
        // matching simplex
        protected int plotXY(RegressionFunction g, string title)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y array\nplotXY2 should have been called");
            }
            Object regFun = g;
            int flag = plotXYnonlinear(regFun, title);
            return flag;
        }

        // plot experimental x against experimental y and against calculated y
        // non-linear regression data
        // title provided
        // matching simplex2
        protected int plotXY2(RegressionFunction2 g, string title)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nsimplex should have been called");
            }
            m_graphTitle = title;
            Object regFun = g;
            int flag = plotXYnonlinear(regFun, title);
            return flag;
        }

        // plot experimental x against experimental y and against calculated y
        // non-linear regression data
        // no title provided
        // matches simplex
        protected int plotXY(RegressionFunction g)
        {
            if (m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle multiply dimensioned y array\nplotXY2 should have been called");
            }
            Object regFun = g;
            int flag = plotXYnonlinear(regFun, m_graphTitle);
            return flag;
        }

        // plot experimental x against experimental y and against calculated y
        // non-linear regression data
        // no title provided
        // matches simplex2
        protected int plotXY2(RegressionFunction2 g)
        {
            if (!m_multipleY)
            {
                throw new HCException(
                    "This method cannot handle singly dimensioned y array\nplotXY should have been called");
            }
            Object regFun = g;
            int flag = plotXYnonlinear(regFun, m_graphTitle);
            return flag;
        }

        // Add legends option
        public void addLegends()
        {
            //int ans = JOptionPane.showConfirmDialog(null, "Do you wish to add your own legends to the x and y axes", "Axis Legends", JOptionPane.YES_NO_OPTION, JOptionPane.QUESTION_MESSAGE);
            //if (ans == 0)
            //{
            //    xLegend = JOptionPane.showInputDialog("Type the legend for the abscissae (x-axis) [first data set]");
            //    yLegend = JOptionPane.showInputDialog("Type the legend for the ordinates (y-axis) [second data set]");
            //    legendCheck = true;
            //}
        }

        // protected method for plotting experimental x against experimental y and against calculated y
        // Linear regression
        // title provided
        protected int plotXYlinear(string title)
        {
            return -1;

            //graphTitle = title;
            //int flag = 0;  //Returned as 0 if plot data can be plotted, -1 if not, -2 if tried multiple regression plot
            //if (nXarrays > 1)
            //{
            //    PrintToScreen.WriteLine("You attempted to use Regression.plotXY() for a multiple regression");
            //    PrintToScreen.WriteLine("No plot attempted");
            //    flag = -2;
            //    return flag;
            //}

            //int ncurves = 2;
            //int npoints = 200;
            //if (npoints < nData0) npoints = nData0;
            //if (lastMethod == 11 || lastMethod == 12 || lastMethod == 21) npoints = nData0;
            //double[,] data = PlotGraph.data(ncurves, npoints);
            //double xmin = Fmath.minimum(xData[0]);
            //double xmax = Fmath.maximum(xData[0]);
            //double inc = (xmax - xmin) / (double)(npoints - 1);
            //string title1 = " ";
            //string title2 = " ";

            //for (int i = 0; i < nData0; i++)
            //{
            //    data[0, i] = xData[0, i];
            //    data[1, i] = yData[i];
            //}

            //data[2, 0] = xmin;
            //for (int i = 1; i < npoints; i++) data[2, i] = data[2, i - 1] + inc;
            //if (nTerms == 0)
            //{
            //    switch (lastMethod)
            //    {
            //        case 11: title1 = "No regression: Minimum Order Statistic Standard Gumbel (y = Exp(x)Exp(-Exp(x))): " + graphTitle;
            //            title2 = " points - experimental values;   line - theoretical curve;   no parameters to be estimated";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++) data[3, i] = yCalc[i];
            //            break;
            //        case 12: title1 = "No regression:  Maximum Order Statistic Standard Gumbel (y = Exp(-x)Exp(-Exp(-x))): " + graphTitle;
            //            title2 = " points - experimental values;   line - theoretical curve;   no parameters to be estimated";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++) data[3, i] = yCalc[i];
            //            break;
            //        case 21: title1 = "No regression:  Standard Exponential (y = Exp(-x)): " + graphTitle;
            //            title2 = " points - experimental values;   line - theoretical curve;   no parameters to be estimated";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++) data[3, i] = yCalc[i];
            //            break;
            //    }

            //}
            //else
            //{
            //    switch (lastMethod)
            //    {
            //        case 0: title1 = "Linear regression  (y = a + b.x): " + graphTitle;
            //            title2 = " points - experimental values;   line - best fit curve";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++) data[3, i] = best[0] + best[1] * data[2, i];
            //            break;
            //        case 1: title1 = "Linear (polynomial with degree = " + (nTerms - 1) + ") regression: " + graphTitle;
            //            title2 = " points - experimental values;   line - best fit curve";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                double sum = best[0];
            //                for (int j = 1; j < nTerms; j++) sum += best[j] * Math.Pow(data[2, i], j);
            //                data[3, i] = sum;
            //            }
            //            break;
            //        case 2: title1 = "Linear regression  (y = a.x): " + graphTitle;
            //            title2 = " points - experimental values;   line - best fit curve";
            //            if (nXarrays == 1)
            //            {
            //                if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //                for (int i = 0; i < npoints; i++) data[3, i] = best[0] * data[2, i];
            //            }
            //            else
            //            {
            //                PrintToScreen.WriteLine("Regression.plotXY(linear): lastMethod, " + lastMethod + ",cannot be plotted in two dimensions");
            //                PrintToScreen.WriteLine("No plot attempted");
            //                flag = -1;
            //            }
            //            break;
            //        case 11: title1 = "Linear regression: Minimum Order Statistic Standard Gumbel (y = a.z where z = Exp(x)Exp(-Exp(x))): " + graphTitle;
            //            title2 = " points - experimental values;   line - best fit curve";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++) data[3, i] = best[0] * Math.Exp(data[2, i]) * Math.Exp(-Math.Exp(data[2, i]));
            //            break;
            //        case 12: title1 = "Linear regression:  Maximum Order Statistic Standard Gumbel (y = a.z where z=Exp(-x)Exp(-Exp(-x))): " + graphTitle;
            //            title2 = " points - experimental values;   line - best fit curve";
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";
            //            for (int i = 0; i < npoints; i++) data[3, i] = best[0] * Math.Exp(-data[2, i]) * Math.Exp(-Math.Exp(-data[2, i]));
            //            break;
            //        default: PrintToScreen.WriteLine("Regression.plotXY(linear): lastMethod, " + lastMethod + ", either not recognised or cannot be plotted in two dimensions");
            //            PrintToScreen.WriteLine("No plot attempted");
            //            flag = -1;
            //            return flag;
            //    }
            //}

            //PlotGraph pg = new PlotGraph(data);

            //pg.setGraphTitle(title1);
            //pg.setGraphTitle2(title2);
            //pg.setXaxisLegend(xLegend);
            //pg.setYaxisLegend(yLegend);
            //int[] popt = { 1, 0 };
            //pg.setPoint(popt);
            //int[] lopt = { 0, 3 };
            //pg.setLine(lopt);
            //if (weightOpt) pg.setErrorBars(0, weight);
            //pg.plot();

            //return flag;
        }

        // protected method for plotting experimental x against experimental y and against calculated y
        // Non-linear regression
        // title provided
        public int plotXYnonlinear(Object regFun, string title)
        {
            return -1;
            //graphTitle = title;
            //RegressionFunction g1 = null;
            //RegressionFunction2 g2 = null;
            //if (multipleY)
            //{
            //    g2 = (RegressionFunction2)regFun;
            //}
            //else
            //{
            //    g1 = (RegressionFunction)regFun;
            //}

            //int flag = 0;  //Returned as 0 if plot data can be plotted, -1 if not

            //if (lastMethod < 3)
            //{
            //    PrintToScreen.WriteLine("Regression.plotXY(non-linear): lastMethod, " + lastMethod + ", either not recognised or cannot be plotted in two dimensions");
            //    PrintToScreen.WriteLine("No plot attempted");
            //    flag = -1;
            //    return flag;
            //}

            //if (nXarrays > 1)
            //{
            //    PrintToScreen.WriteLine("Multiple Linear Regression with more than one independent variable cannot be plotted in two dimensions");
            //    PrintToScreen.WriteLine("plotYY() called instead of plotXY()");
            //    plotYY(title);
            //    flag = -2;
            //}
            //else
            //{
            //    if (multipleY)
            //    {
            //        int ncurves = 2;
            //        int npoints = 200;
            //        if (npoints < nData0) npoints = nData0;
            //        string title1, title2;
            //        int kk = 0;
            //        double[] wWeight = new double[nData0];
            //        for (int jj = 0; jj < nYarrays; jj++)
            //        {
            //            double[,] data = PlotGraph.data(ncurves, npoints);
            //            for (int i = 0; i < nData0; i++)
            //            {
            //                data[0, i] = xData[0, kk];
            //                data[1, i] = yData[kk];
            //                wWeight[i] = weight[kk];
            //                kk++;
            //            }
            //            double xmin = Fmath.minimum(xData[0]);
            //            double xmax = Fmath.maximum(xData[0]);
            //            double inc = (xmax - xmin) / (double)(npoints - 1);
            //            data[2, 0] = xmin;
            //            for (int i = 1; i < npoints; i++) data[2, i] = data[2, i - 1] + inc;
            //            double[] xd = new double[nXarrays];
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                xd[0] = data[2, i];
            //                data[3, i] = g2.function(best, xd, jj * nData0);
            //            }

            //            // Create a title
            //            title1 = setGandPtitle(title);
            //            title2 = " points - experimental values;   line - best fit curve;  y data array " + jj;
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";

            //            PlotGraph pg = new PlotGraph(data);

            //            pg.setGraphTitle(title1);
            //            pg.setGraphTitle2(title2);
            //            pg.setXaxisLegend(xLegend);
            //            pg.setYaxisLegend(yLegend);
            //            int[] popt = { 1, 0 };
            //            pg.setPoint(popt);
            //            int[] lopt = { 0, 3 };
            //            pg.setLine(lopt);
            //            if (weightOpt) pg.setErrorBars(0, wWeight);

            //            pg.plot();
            //        }
            //    }
            //    else
            //    {
            //        int ncurves = 2;
            //        int npoints = 200;
            //        if (npoints < nData0) npoints = nData0;
            //        if (lastMethod == 6) npoints = nData0;
            //        string title1, title2;
            //        double[,] data = PlotGraph.data(ncurves, npoints);
            //        for (int i = 0; i < nData0; i++)
            //        {
            //            data[0, i] = xData[0, i];
            //            data[1, i] = yData[i];
            //        }
            //        if (lastMethod == 6)
            //        {
            //            double[] xd = new double[nXarrays];
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                data[2, i] = data[0, i];
            //                xd[0] = data[2, i];
            //                data[3, i] = g1.function(best, xd);
            //            }
            //        }
            //        else
            //        {
            //            double xmin = Fmath.minimum(xData[0]);
            //            double xmax = Fmath.maximum(xData[0]);
            //            double inc = (xmax - xmin) / (double)(npoints - 1);
            //            data[2, 0] = xmin;
            //            for (int i = 1; i < npoints; i++) data[2, i] = data[2, i - 1] + inc;
            //            double[] xd = new double[nXarrays];
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                xd[0] = data[2, i];
            //                data[3, i] = g1.function(best, xd);
            //            }
            //        }

            //        // Create a title
            //        title1 = setGandPtitle(title);
            //        title2 = " points - experimental values;   line - best fit curve";
            //        if (weightOpt) title2 = title2 + ";   error bars - weighting factors";

            //        PlotGraph pg = new PlotGraph(data);

            //        pg.setGraphTitle(title1);
            //        pg.setGraphTitle2(title2);
            //        pg.setXaxisLegend(xLegend);
            //        pg.setYaxisLegend(yLegend);
            //        int[] popt = { 1, 0 };
            //        pg.setPoint(popt);
            //        int[] lopt = { 0, 3 };
            //        pg.setLine(lopt);

            //        if (weightOpt) pg.setErrorBars(0, weight);

            //        pg.plot();
            //    }
            //}
            //return flag;
        }

        // protected method for plotting experimental x against experimental y and against calculated y
        // Non-linear regression
        // all parameters fixed_
        public int plotXYfixed_(Object regFun, string title)
        {
            return -1;
            //graphTitle = title;
            //RegressionFunction g1 = null;
            //RegressionFunction2 g2 = null;
            //if (multipleY)
            //{
            //    g2 = (RegressionFunction2)regFun;
            //}
            //else
            //{
            //    g1 = (RegressionFunction)regFun;
            //}

            //int flag = 0;  //Returned as 0 if plot data can be plotted, -1 if not

            //if (lastMethod < 3)
            //{
            //    PrintToScreen.WriteLine("Regression.plotXY(non-linear): lastMethod, " + lastMethod + ", either not recognised or cannot be plotted in two dimensions");
            //    PrintToScreen.WriteLine("No plot attempted");
            //    flag = -1;
            //    return flag;
            //}


            //if (nXarrays > 1)
            //{
            //    PrintToScreen.WriteLine("Multiple Linear Regression with more than one independent variable cannot be plotted in two dimensions");
            //    PrintToScreen.WriteLine("plotYY() called instead of plotXY()");
            //    plotYY(title);
            //    flag = -2;
            //}
            //else
            //{
            //    if (multipleY)
            //    {
            //        int ncurves = 2;
            //        int npoints = 200;
            //        if (npoints < nData0) npoints = nData0;
            //        string title1, title2;
            //        int kk = 0;
            //        double[] wWeight = new double[nData0];
            //        for (int jj = 0; jj < nYarrays; jj++)
            //        {
            //            double[,] data = PlotGraph.data(ncurves, npoints);
            //            for (int i = 0; i < nData0; i++)
            //            {
            //                data[0, i] = xData[0, kk];
            //                data[1, i] = yData[kk];
            //                wWeight[i] = weight[kk];
            //                kk++;
            //            }
            //            double xmin = Fmath.minimum(xData[0]);
            //            double xmax = Fmath.maximum(xData[0]);
            //            double inc = (xmax - xmin) / (double)(npoints - 1);
            //            data[2, 0] = xmin;
            //            for (int i = 1; i < npoints; i++) data[2, i] = data[2, i - 1] + inc;
            //            double[] xd = new double[nXarrays];
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                xd[0] = data[2, i];
            //                data[3, i] = g2.function(values, xd, jj * nData0);
            //            }

            //            // Create a title
            //            title1 = setGandPtitle(title);
            //            title2 = " points - experimental values;   line - best fit curve;  y data array " + jj;
            //            if (weightOpt) title2 = title2 + ";   error bars - weighting factors";

            //            PlotGraph pg = new PlotGraph(data);

            //            pg.setGraphTitle(title1);
            //            pg.setGraphTitle2(title2);
            //            pg.setXaxisLegend(xLegend);
            //            pg.setYaxisLegend(yLegend);
            //            int[] popt = { 1, 0 };
            //            pg.setPoint(popt);
            //            int[] lopt = { 0, 3 };
            //            pg.setLine(lopt);
            //            if (weightOpt) pg.setErrorBars(0, wWeight);

            //            pg.plot();
            //        }
            //    }
            //    else
            //    {
            //        int ncurves = 2;
            //        int npoints = 200;
            //        if (npoints < nData0) npoints = nData0;
            //        if (lastMethod == 6) npoints = nData0;
            //        string title1, title2;
            //        double[,] data = PlotGraph.data(ncurves, npoints);
            //        for (int i = 0; i < nData0; i++)
            //        {
            //            data[0, i] = xData[0, i];
            //            data[1, i] = yData[i];
            //        }
            //        if (lastMethod == 6)
            //        {
            //            double[] xd = new double[nXarrays];
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                data[2, i] = data[0, i];
            //                xd[0] = data[2, i];
            //                data[3, i] = g1.function(values, xd);
            //            }
            //        }
            //        else
            //        {
            //            double xmin = Fmath.minimum(xData[0]);
            //            double xmax = Fmath.maximum(xData[0]);
            //            double inc = (xmax - xmin) / (double)(npoints - 1);
            //            data[2, 0] = xmin;
            //            for (int i = 1; i < npoints; i++) data[2, i] = data[2, i - 1] + inc;
            //            double[] xd = new double[nXarrays];
            //            for (int i = 0; i < npoints; i++)
            //            {
            //                xd[0] = data[2, i];
            //                data[3, i] = g1.function(values, xd);
            //            }
            //        }

            //        // Create a title
            //        title1 = setGandPtitle(title);
            //        title2 = " points - experimental values;   line - best fit curve";
            //        if (weightOpt) title2 = title2 + ";   error bars - weighting factors";

            //        PlotGraph pg = new PlotGraph(data);

            //        pg.setGraphTitle(title1);
            //        pg.setGraphTitle2(title2);
            //        pg.setXaxisLegend(xLegend);
            //        pg.setYaxisLegend(yLegend);
            //        int[] popt = { 1, 0 };
            //        pg.setPoint(popt);
            //        int[] lopt = { 0, 3 };
            //        pg.setLine(lopt);

            //        if (weightOpt) pg.setErrorBars(0, weight);

            //        pg.plot();
            //    }
            //}
            //return flag;
        }


        // Get the non-linear regression status
        // true if convergence was achieved
        // false if convergence not achieved before maximum number of iterations
        //  current values then returned
        public bool getNlrStatus()
        {
            return m_nlrStatus;
        }

        // Reset scaling factors (scaleOpt 0 and 1, see below for scaleOpt 2)
        public void setScale(int n)
        {
            if (n < 0 || n > 1)
            {
                throw new HCException(
                    "The argument must be 0 (no scaling) 1(initial estimates all scaled to unity) or the array of scaling factors");
            }
            m_scaleOpt = n;
        }

        // Reset scaling factors (scaleOpt 2, see above for scaleOpt 0 and 1)
        public void setScale(double[] sc)
        {
            m_scale = sc;
            m_scaleOpt = 2;
        }

        // Get scaling factors
        public double[] getScale()
        {
            return m_scale;
        }

        // Reset the non-linear regression convergence test option
        public void setMinTest(int n)
        {
            if (n < 0 || n > 1)
            {
                throw new HCException("minTest must be 0 or 1");
            }
            m_minTest = n;
        }

        // Get the non-linear regression convergence test option
        public int getMinTest()
        {
            return m_minTest;
        }

        // Get the simplex m_sd at the minimum
        public double getSimplexSd()
        {
            return m_simplexSd;
        }

        // Get the best estimates of the unknown parameters
        public double[] getBestEstimates()
        {
            return (double[]) m_best.Clone();
        }

        // Get the best estimates of the unknown parameters
        public double[] getCoeff()
        {
            return (double[]) m_best.Clone();
        }

        // Get the estimates of the standard deviations of the best estimates of the unknown parameters
        public double[] getbestestimatesStandardDeviations()
        {
            return (double[]) bestSd.Clone();
        }

        // Get the estimates of the errors of the best estimates of the unknown parameters
        public double[] getBestEstimatesStandardDeviations()
        {
            return (double[]) bestSd.Clone();
        }

        // Get the estimates of the errors of the best estimates of the unknown parameters
        public double[] getCoeffSd()
        {
            return (double[]) bestSd.Clone();
        }

        // Get the estimates of the errors of the best estimates of the unknown parameters
        public double[] getBestEstimatesErrors()
        {
            return (double[]) bestSd.Clone();
        }

        // Get the unscaled initial estimates of the unknown parameters
        public double[] getInitialEstimates()
        {
            return (double[]) m_startH.Clone();
        }

        // Get the scaled initial estimates of the unknown parameters
        public double[] getScaledInitialEstimates()
        {
            return (double[]) m_startSH.Clone();
        }

        // Get the unscaled initial step sizes
        public double[] getInitialSteps()
        {
            return (double[]) m_stepH.Clone();
        }

        // Get the scaled initial step sizesp
        public double[] getScaledInitialSteps()
        {
            return (double[]) m_stepSH.Clone();
        }

        // Get the cofficients of variations of the best estimates of the unknown parameters
        public double[] getCoeffVar()
        {
            double[] coeffVar = new double[m_nTerms];

            for (int i = 0; i < m_nTerms; i++)
            {
                coeffVar[i] = bestSd[i]*100.0D/m_best[i];
            }
            return coeffVar;
        }

        // Get the pseudo-estimates of the errors of the best estimates of the unknown parameters
        public double[] getPseudoSd()
        {
            return (double[]) m_pseudoSd.Clone();
        }

        // Get the pseudo-estimates of the errors of the best estimates of the unknown parameters
        public double[] getPseudoErrors()
        {
            return (double[]) m_pseudoSd.Clone();
        }

        // Get the t-values of the best estimates
        public double[] getTvalues()
        {
            return (double[]) m_tValues.Clone();
        }

        // Get the p-values of the best estimates
        public double[] getPvalues()
        {
            return (double[]) m_pValues.Clone();
        }


        // Get the inputted x values
        public double[,] getXdata()
        {
            return (double[,]) m_xData.Clone();
        }

        // Get the inputted y values
        public double[] getYdata()
        {
            return (double[]) m_yData.Clone();
        }

        // Get the calculated y values
        public double[] getYcalc()
        {
            double[] temp = new double[m_nData];
            for (int i = 0; i < m_nData; i++)
            {
                temp[i] = m_yCalc[i];
            }
            return temp;
        }

        // Get the unweighted residuals, y(experimental) - y(calculated)
        public double[] getResiduals()
        {
            double[] temp = new double[m_nData];
            for (int i = 0; i < m_nData; i++)
            {
                temp[i] = m_yData[i] - m_yCalc[i];
            }
            return temp;
        }

        // Get the weighted residuals, (y(experimental) - y(calculated))/weight
        public double[] getWeightedResiduals()
        {
            double[] temp = new double[m_nData];
            for (int i = 0; i < m_nData; i++)
            {
                temp[i] = (m_yData[i] - m_yCalc[i])/m_weight[i];
            }
            return temp;
        }

        // Get the unweighted sum of squares of the residuals
        public double getSumOfSquares()
        {
            return m_sumOfSquares;
        }

        // Get the chi square estimate
        public double getChiSquare()
        {
            double ret = 0.0D;
            if (m_weightOpt)
            {
                ret = m_chiSquare;
            }
            else
            {
                PrintToScreen.WriteLine(
                    "Chi Square cannot be calculated as data are neither true frequencies nor weighted");
                PrintToScreen.WriteLine("A value of -1 is returned as Chi Square");
                ret = -1.0D;
            }
            return ret;
        }

        // Get the reduced chi square estimate
        public double getReducedChiSquare()
        {
            double ret = 0.0D;
            if (m_weightOpt)
            {
                ret = m_reducedChiSquare;
            }
            else
            {
                PrintToScreen.WriteLine(
                    "A Reduced Chi Square cannot be calculated as data are neither true frequencies nor weighted");
                PrintToScreen.WriteLine("A value of -1 is returned as Reduced Chi Square");
                ret = -1.0D;
            }
            return ret;
        }

        // Get the chi square probablity
        public double getchiSquareProb()
        {
            double ret = 0.0D;
            if (m_weightOpt)
            {
                ret = 1.0D - Stat.chiSquareProb(m_chiSquare, m_nData - m_nXarrays);
            }
            else
            {
                PrintToScreen.WriteLine(
                    "A Chi Square probablity cannot be calculated as data are neither true frequencies nor weighted");
                PrintToScreen.WriteLine("A value of -1 is returned as Reduced Chi Square");
                ret = -1.0D;
            }
            return ret;
        }

        // Get the covariance matrix
        public double[,] getCovMatrix()
        {
            return m_covar;
        }

        // Get the correlation coefficient matrix
        public double[,] getCorrCoeffMatrix()
        {
            return m_corrCoeff;
        }

        // Get the number of iterations in nonlinear regression
        public int getNiter()
        {
            return m_nIter;
        }


        // Set the maximum number of iterations allowed in nonlinear regression
        public void setNmax(int nmax)
        {
            m_nMax = nmax;
        }

        // Get the maximum number of iterations allowed in nonlinear regression
        public int getNmax()
        {
            return m_nMax;
        }

        // Get the number of restarts in nonlinear regression
        public int getNrestarts()
        {
            return m_kRestart;
        }

        // Set the maximum number of restarts allowed in nonlinear regression
        public void setNrestartsMax(int nrs)
        {
            m_konvge = nrs;
        }

        // Get the maximum number of restarts allowed in nonlinear regression
        public int getNrestartsMax()
        {
            return m_konvge;
        }

        // Get the degrees of freedom
        public double getDegFree()
        {
            return (m_degreesOfFreedom);
        }

        // Reset the Nelder and Mead reflection coefficient [alpha]
        public void setNMreflect(double refl)
        {
            m_rCoeff = refl;
        }

        // Get the Nelder and Mead reflection coefficient [alpha]
        public double getNMreflect()
        {
            return m_rCoeff;
        }

        // Reset the Nelder and Mead extension coefficient [beta]
        public void setNMextend(double ext)
        {
            m_eCoeff = ext;
        }

        // Get the Nelder and Mead extension coefficient [beta]
        public double getNMextend()
        {
            return m_eCoeff;
        }

        // Reset the Nelder and Mead contraction coefficient [gamma]
        public void setNMcontract(double con)
        {
            m_cCoeff = con;
        }

        // Get the Nelder and Mead contraction coefficient [gamma]
        public double getNMcontract()
        {
            return m_cCoeff;
        }

        // Set the non-linear regression tolerance
        public void setTolerance(double tol)
        {
            m_fTol = tol;
        }


        // Get the non-linear regression tolerance
        public double getTolerance()
        {
            return m_fTol;
        }

        // Get the non-linear regression pre and post minimum gradients
        public double[,] getGrad()
        {
            return m_grad;
        }

        // Set the non-linear regression fractional step size used in numerical differencing
        public void setDelta(double delta)
        {
            m_delta = delta;
        }

        // Get the non-linear regression fractional step size used in numerical differencing
        public double getDelta()
        {
            return m_delta;
        }

        // Get the non-linear regression statistics Hessian matrix inversion status flag
        public bool getInversionCheck()
        {
            return m_invertFlag;
        }

        // Get the non-linear regression statistics Hessian matrix inverse diagonal status flag
        public bool getPosVarCheck()
        {
            return m_posVarFlag;
        }


        // UnitTest of an additional terms  {extra sum of squares]
        // return F-ratio, probability, order check and values provided in order used, as List
        public static List<Object> testOfAdditionalTerms(double chiSquareR, int nParametersR, double chiSquareF,
                                                         int nParametersF, int nPoints)
        {
            List<Object> res = testOfAdditionalTerms_ArrayList(chiSquareR, nParametersR, chiSquareF, nParametersF,
                                                               nPoints);
            List<Object> ret = null;
            if (res != null)
            {
                int n = ret.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }

        // UnitTest of an additional terms  {extra sum of squares]
        // return F-ratio, probability, order check and values provided in order used, as List
        public static List<Object> testOfAdditionalTerms_Vector(double chiSquareR, int nParametersR, double chiSquareF,
                                                                int nParametersF, int nPoints)
        {
            return testOfAdditionalTerms(chiSquareR, nParametersR, chiSquareF, nParametersF, nPoints);
        }


        // UnitTest of an additional terms  {extra sum of squares]
        // return F-ratio, probability, order check and values provided in order used, as List
        public static List<Object> testOfAdditionalTerms_ArrayList(double chiSquareR, int nParametersR,
                                                                   double chiSquareF, int nParametersF, int nPoints)
        {
            int degFreedomR = nPoints - nParametersR;
            int degFreedomF = nPoints - nParametersF;

            // Check that model 2 has the lowest degrees of freedom
            bool reversed = false;
            if (degFreedomR < degFreedomF)
            {
                reversed = true;
                double holdD = chiSquareR;
                chiSquareR = chiSquareF;
                chiSquareF = holdD;
                int holdI = nParametersR;
                nParametersR = nParametersF;
                nParametersF = holdI;
                degFreedomR = nPoints - nParametersR;
                degFreedomF = nPoints - nParametersF;
                PrintToScreen.WriteLine("package flanagan.analysis; class Regression; method testAdditionalTerms");
                PrintToScreen.WriteLine(
                    "the order of the chi-squares has been reversed to give a second chi- square with the lowest degrees of freedom");
            }
            int degFreedomD = degFreedomR - degFreedomF;

            // F ratio
            double numer = (chiSquareR - chiSquareF)/degFreedomD;
            double denom = chiSquareF/degFreedomF;
            double fRatio = numer/denom;

            // Probability
            double fProb = 1.0D;
            if (chiSquareR > chiSquareF)
            {
                fProb = Stat.fTestProb(fRatio, degFreedomD, degFreedomF);
            }

            // Return arraylist
            List<Object> arrayl = new List<Object>();
            arrayl.Add(fRatio);
            arrayl.Add(fProb);
            arrayl.Add(reversed);
            arrayl.Add(chiSquareR);
            arrayl.Add(nParametersR);
            arrayl.Add(chiSquareF);
            arrayl.Add(nParametersF);
            arrayl.Add(nPoints);

            return arrayl;
        }

        // UnitTest of an additional terms  {extra sum of squares]
        // return F-ratio only
        public double testOfAdditionalTermsFratio(double chiSquareR, int nParametersR, double chiSquareF,
                                                  int nParametersF, int nPoints)
        {
            int degFreedomR = nPoints - nParametersR;
            int degFreedomF = nPoints - nParametersF;

            // Check that model 2 has the lowest degrees of freedom
            //bool reversed = false;
            if (degFreedomR < degFreedomF)
            {
                //reversed = true;
                double holdD = chiSquareR;
                chiSquareR = chiSquareF;
                chiSquareF = holdD;
                int holdI = nParametersR;
                nParametersR = nParametersF;
                nParametersF = holdI;
                degFreedomR = nPoints - nParametersR;
                degFreedomF = nPoints - nParametersF;
                PrintToScreen.WriteLine("package flanagan.analysis; class Regression; method testAdditionalTermsFratio");
                PrintToScreen.WriteLine(
                    "the order of the chi-squares has been reversed to give a second chi- square with the lowest degrees of freedom");
            }
            int degFreedomD = degFreedomR - degFreedomF;

            // F ratio
            double numer = (chiSquareR - chiSquareF)/degFreedomD;
            double denom = chiSquareF/degFreedomF;
            double fRatio = numer/denom;

            return fRatio;
        }

        // UnitTest of an additional terms  {extra sum of squares]
        // return F-distribution probablity only
        public double testOfAdditionalTermsFprobabilty(double chiSquareR, int nParametersR, double chiSquareF,
                                                       int nParametersF, int nPoints)
        {
            int degFreedomR = nPoints - nParametersR;
            int degFreedomF = nPoints - nParametersF;

            // Check that model 2 has the lowest degrees of freedom
            //bool reversed = false;
            if (degFreedomR < degFreedomF)
            {
                //reversed = true;
                double holdD = chiSquareR;
                chiSquareR = chiSquareF;
                chiSquareF = holdD;
                int holdI = nParametersR;
                nParametersR = nParametersF;
                nParametersF = holdI;
                degFreedomR = nPoints - nParametersR;
                degFreedomF = nPoints - nParametersF;
                PrintToScreen.WriteLine(
                    "package flanagan.analysis; class Regression; method testAdditionalTermsFprobability");
                PrintToScreen.WriteLine(
                    "the order of the chi-squares has been reversed to give a second chi- square with the lowest degrees of freedom");
            }
            int degFreedomD = degFreedomR - degFreedomF;

            // F ratio
            double numer = (chiSquareR - chiSquareF)/degFreedomD;
            double denom = chiSquareF/degFreedomF;
            double fRatio = numer/denom;

            // Probability
            double fProb = 1.0D;
            if (chiSquareR > chiSquareF)
            {
                fProb = Stat.fTestProb(fRatio, degFreedomD, degFreedomF);
            }

            return fProb;
        }


        // FIT TO SPECIAL FUNCTIONS
        // Fit to a Poisson distribution
        public void poisson()
        {
            m_userSupplied = false;
            fitPoisson(0);
        }

        // Fit to a Poisson distribution
        public void poissonPlot()
        {
            m_userSupplied = false;
            fitPoisson(1);
        }

        protected void fitPoisson(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 6;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // Check all abscissae are integers
            for (int i = 0; i < m_nData; i++)
            {
                if (m_xData[0, i] - Math.Floor(m_xData[0, i]) != 0.0D)
                {
                    throw new HCException("all abscissae must be, mathematically, integer values");
                }
            }

            // Calculate  x value at peak y (estimate of the distribution mode)
            List<Object> ret1 = dataSign(m_yData);
            double tempd = 0;
            int tempi = 0;
            tempi = (int) ret1[5];
            int peaki = tempi;
            double mean = m_xData[0, peaki];

            // Calculate peak value
            tempd = (double) ret1[4];
            double peak = tempd;

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = mean;
            if (m_scaleFlag)
            {
                start[1] = peak/(Math.Exp(mean*Math.Log(mean) - Stat.logFactorial(mean))*Math.Exp(-mean));
            }
            step[0] = 0.1D*start[0];
            if (step[0] == 0.0D)
            {
                List<Object> ret0 = dataSign(
                    ArrayHelper.GetRowCopy<double>(
                        m_xData,
                        0));
                double tempdd = 0;
                tempdd = (double) ret0[2];
                double xmax = tempdd;
                if (xmax == 0.0D)
                {
                    tempdd = (double) ret0[0];
                    xmax = tempdd;
                }
                step[0] = xmax*0.1D;
            }
            if (m_scaleFlag)
            {
                step[1] = 0.1D*start[1];
            }

            // Nelder and Mead Simplex Regression
            PoissonFunction f = new PoissonFunction();
            addConstraint(1, -1, 0.0D);
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;

            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }
                // Plot results
                m_plotOpt = false;
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }


        // FIT TO A NORMAL (GAUSSIAN) DISTRIBUTION

        // Fit to a Gaussian
        public void gaussian()
        {
            m_userSupplied = false;
            fitGaussian(0);
        }

        public void normal()
        {
            m_userSupplied = false;
            fitGaussian(0);
        }

        // Fit to a Gaussian
        public void gaussianPlot()
        {
            m_userSupplied = false;
            fitGaussian(1);
        }

        // Fit to a Gaussian
        public void normalPlot()
        {
            m_userSupplied = false;
            fitGaussian(1);
        }

        // Fit data to a Gaussian (normal) probability function
        protected void fitGaussian(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 4;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitGaussian(): This implementation of the Gaussian distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // Calculate  x value at peak y (estimate of the Gaussian m_mean)
            List<Object> ret1 = dataSign(m_yData);
            int tempi = 0;
            tempi = (int) ret1[5];
            int peaki = tempi;
            double mean = m_xData[0, peaki];

            // Calculate an estimate of the m_sd
            double sd = Math.Sqrt(2.0)*
                        halfWidth(
                            ArrayHelper.GetRowCopy<double>(
                                m_xData,
                                0),
                            m_yData);

            // Calculate estimate of y scale
            tempd = (double) ret1[4];
            double ym = tempd;
            ym = ym*sd*Math.Sqrt(2.0D*Math.PI);

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = mean;
            start[1] = sd;
            if (m_scaleFlag)
            {
                start[2] = ym;
            }
            step[0] = 0.1D*sd;
            step[1] = 0.1D*start[1];
            if (step[1] == 0.0D)
            {
                List<Object> ret0 = dataSign(
                    ArrayHelper.GetRowCopy<double>(
                        m_xData,
                        0));
                double tempdd = 0;
                tempdd = (double) ret0[2];
                double xmax = tempdd;
                if (xmax == 0.0D)
                {
                    tempdd = (double) ret0[0];
                    xmax = tempdd;
                }
                step[0] = xmax*0.1D;
            }
            if (m_scaleFlag)
            {
                step[2] = 0.1D*start[1];
            }

            // Nelder and Mead Simplex Regression
            GaussianFunction f = new GaussianFunction();
            addConstraint(1, -1, 0.0D);
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;

            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        // Fit data to a Gaussian (normal) probability function
        // with option to fix some of the parameters
        // parameter order - m_mean, m_sd, scale factor
        public void gaussian(double[] initialEstimates, bool[] fixed_)
        {
            m_userSupplied = true;
            fitGaussianfixed_(initialEstimates, fixed_, 0);
        }

        // Fit to a Gaussian
        // with option to fix some of the parameters
        // parameter order - m_mean, m_sd, scale factor
        public void normal(double[] initialEstimates, bool[] fixed_)
        {
            m_userSupplied = true;
            fitGaussianfixed_(initialEstimates, fixed_, 0);
        }

        // Fit to a Gaussian
        // with option to fix some of the parameters
        // parameter order - m_mean, m_sd, scale factor
        public void gaussianPlot(double[] initialEstimates, bool[] fixed_)
        {
            m_userSupplied = true;
            fitGaussianfixed_(initialEstimates, fixed_, 1);
        }

        // Fit to a Gaussian
        // with option to fix some of the parameters
        // parameter order - m_mean, m_sd, scale factor
        public void normalPlot(double[] initialEstimates, bool[] fixed_)
        {
            m_userSupplied = true;
            fitGaussianfixed_(initialEstimates, fixed_, 1);
        }


        // Fit data to a Gaussian (normal) probability function
        // with option to fix some of the parameters
        // parameter order - m_mean, m_sd, scale factor
        protected void fitGaussianfixed_(double[] initialEstimates, bool[] fixed_, int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 38;
            m_values = initialEstimates;
            m_fixed_ = fixed_;
            m_scaleFlag = true;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitGaussian(): This implementation of the Gaussian distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // Create instance of GaussianFunctionfixed_
            GaussianFunctionfixed_ f = new GaussianFunctionfixed_();
            f.fixed_ = fixed_;
            f.param = initialEstimates;

            // Determine unknowns
            int nT = m_nTerms;
            for (int i = 0; i < m_nTerms; i++)
            {
                if (fixed_[i])
                {
                    nT--;
                }
            }
            if (nT == 0)
            {
                if (plotFlag == 0)
                {
                    throw new HCException(
                        "At least one parameter must be available for variation by the Regression procedure or GauasianPlot should have been called and not Gaussian");
                }
                else
                {
                    plotFlag = 3;
                }
            }

            double[] start = new double[nT];
            double[] step = new double[nT];
            bool[] constraint = new bool[nT];

            // Fill arrays needed by the Simplex
            double[] tmpArr = ArrayHelper.GetRowCopy<double>(
                m_xData,
                0);

            double xMin = Fmath.minimum(
                tmpArr);

            double xMax = Fmath.maximum(
                tmpArr);

            double yMax = Fmath.maximum(
                m_yData);

            if (initialEstimates[2] == 0.0D)
            {
                if (fixed_[2])
                {
                    throw new HCException("Scale factor has been fixed_ at zero");
                }
                else
                {
                    initialEstimates[2] = yMax;
                }
            }
            int ii = 0;
            for (int i = 0; i < m_nTerms; i++)
            {
                if (!fixed_[i])
                {
                    start[ii] = initialEstimates[i];
                    step[ii] = start[ii]*0.1D;
                    if (step[ii] == 0.0D)
                    {
                        step[ii] = (xMax - xMin)*0.1D;
                    }
                    constraint[ii] = false;
                    if (i == 1)
                    {
                        constraint[ii] = true;
                    }
                    ii++;
                }
            }
            m_nTerms = nT;

            // Nelder and Mead Simplex Regression
            for (int i = 0; i < m_nTerms; i++)
            {
                if (constraint[i])
                {
                    addConstraint(i, -1, 0.0D);
                }
            }
            Object regFun2 = f;
            if (plotFlag != 3)
            {
                nelderMead(regFun2, start, step, m_fTol, m_nMax);
            }

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (plotFlag == 3)
            {
                // Plot results
                int flag = plotXYfixed_(regFun2, "Gaussian distribution - all parameters fixed_");
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        // FIT TO LOG-NORMAL DISTRIBUTIONS (TWO AND THREE PARAMETERS)

        // TWO PARAMETER LOG-NORMAL DISTRIBUTION
        // Fit to a two parameter log-normal distribution
        public void logNormal()
        {
            fitLogNormalTwoPar(0);
        }

        public void logNormalTwoPar()
        {
            fitLogNormalTwoPar(0);
        }

        // Fit to a two parameter log-normal distribution and plot result
        public void logNormalPlot()
        {
            fitLogNormalTwoPar(1);
        }

        public void logNormalTwoParPlot()
        {
            fitLogNormalTwoPar(1);
        }

        // Fit data to a two parameterlog-normal probability function
        protected void fitLogNormalTwoPar(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 36;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitLogNormalTwoPar(): This implementation of the two parameter log-nprmal distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // Calculate  x value at peak y
            List<Object> ret1 = dataSign(m_yData);
            int tempi = 0;
            tempi = (int) ret1[5];
            int peaki = tempi;
            double mean = m_xData[0, peaki];

            // Calculate an estimate of the m_mu
            double mu = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                mu += Math.Log(m_xData[0, i]);
            }
            mu /= m_nData;

            // Calculate estimate of m_sigma
            double sigma = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                sigma += Fmath.square(Math.Log(m_xData[0, i]) - mu);
            }
            sigma = Math.Sqrt(sigma/m_nData);

            // Calculate estimate of y scale
            tempd = (double) ret1[4];
            double ym = tempd;
            ym = ym*Math.Exp(mu - sigma*sigma/2);

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = mu;
            start[1] = sigma;
            if (m_scaleFlag)
            {
                start[2] = ym;
            }
            step[0] = 0.1D*start[0];
            step[1] = 0.1D*start[1];
            if (step[0] == 0.0D)
            {
                List<Object> ret0 = dataSign(
                    ArrayHelper.GetRowCopy<double>(
                        m_xData,
                        0));
                double tempdd = 0;
                tempdd = (double) ret0[2];
                double xmax = tempdd;
                if (xmax == 0.0D)
                {
                    tempdd = (double) ret0[0];
                    xmax = tempdd;
                }
                step[0] = xmax*0.1D;
            }
            if (step[0] == 0.0D)
            {
                List<Object> ret0 =
                    dataSign(
                        ArrayHelper.GetRowCopy<double>(
                            m_xData,
                            0));
                double tempdd = 0;
                tempdd = (double) ret0[2];
                double xmax = tempdd;
                if (xmax == 0.0D)
                {
                    tempdd = (double) ret0[0];
                    xmax = tempdd;
                }
                step[1] = xmax*0.1D;
            }
            if (m_scaleFlag)
            {
                step[2] = 0.1D*start[2];
            }

            // Nelder and Mead Simplex Regression
            LogNormalTwoParFunction f = new LogNormalTwoParFunction();
            addConstraint(1, -1, 0.0D);
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }


        // THREE PARAMETER LOG-NORMAL DISTRIBUTION
        // Fit to a three parameter log-normal distribution
        public void logNormalThreePar()
        {
            fitLogNormalThreePar(0);
        }

        // Fit to a three parameter log-normal distribution and plot result
        public void logNormalThreeParPlot()
        {
            fitLogNormalThreePar(1);
        }

        // Fit data to a three parameter log-normal probability function
        protected void fitLogNormalThreePar(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 37;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 4;
            if (!m_scaleFlag)
            {
                m_nTerms = 3;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitLogNormalThreePar(): This implementation of the three parameter log-normal distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // Calculate  x value at peak y
            List<Object> ret1 = dataSign(m_yData);
            int tempi = 0;
            tempi = (int) ret1[5];
            int peaki = tempi;
            double mean = m_xData[0, peaki];

            // Calculate an estimate of the gamma
            double gamma = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                gamma += m_xData[0, i];
            }
            gamma /= m_nData;

            // Calculate estimate of beta
            double beta = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                beta += Fmath.square(Math.Log(m_xData[0, i]) - Math.Log(gamma));
            }
            beta = Math.Sqrt(beta/m_nData);

            // Calculate estimate of alpha
            List<Object> ret0 = dataSign(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0));
            double tempdd = 0;
            tempdd = (double) ret0[0];
            double xmin = tempdd;
            tempdd = (double) ret0[2];
            double xmax = tempdd;
            double alpha = xmin - (xmax - xmin)/100.0D;
            ;
            if (xmin == 0.0D)
            {
                alpha -= (xmax - xmin)/100.0D;
            }


            // Calculate estimate of y scale
            tempd = (double) ret1[4];
            double ym = tempd;
            ym = ym*(gamma + alpha)*Math.Exp(-beta*beta/2);

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = alpha;
            start[1] = beta;
            start[2] = gamma;
            if (m_scaleFlag)
            {
                start[3] = ym;
            }
            step[0] = 0.1D*start[0];
            step[1] = 0.1D*start[1];
            step[2] = 0.1D*start[2];
            for (int i = 0; i < 3; i++)
            {
                if (step[i] == 0.0D)
                {
                    step[i] = xmax*0.1D;
                }
            }
            if (m_scaleFlag)
            {
                step[3] = 0.1D*start[3];
            }

            // Nelder and Mead Simplex Regression
            LogNormalThreeParFunction f = new LogNormalThreeParFunction();
            addConstraint(0, +1, xmin);
            addConstraint(1, -1, 0.0D);
            addConstraint(2, -1, 0.0D);

            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }


        // FIT TO A LORENTZIAN DISTRIBUTION

        // Fit data to a lorentzian
        public void lorentzian()
        {
            fitLorentzian(0);
        }

        public void lorentzianPlot()
        {
            fitLorentzian(1);
        }

        protected void fitLorentzian(int allTest)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 5;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitLorentzian(): This implementation of the Lorentzian distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // Calculate  x value at peak y (estimate of the distribution mode)
            List<object> ret1 = dataSign(m_yData);
            int tempi = 0;
            tempi = (int) ret1[5];
            int peaki = tempi;
            double mean = m_xData[0, peaki];

            // Calculate an estimate of the half-height width
            double sd = halfWidth(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData);

            // Calculate estimate of y scale
            tempd = (double) ret1[4];
            double ym = tempd;
            ym = ym*sd*Math.PI/2.0D;

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = mean;
            start[1] = sd*0.9D;
            if (m_scaleFlag)
            {
                start[2] = ym;
            }
            step[0] = 0.2D*sd;
            if (step[0] == 0.0D)
            {
                List<Object> ret0 = dataSign(
                    ArrayHelper.GetRowCopy<double>(
                        m_xData,
                        0));
                double tempdd = 0;
                tempdd = (double) ret0[2];
                double xmax = tempdd;
                if (xmax == 0.0D)
                {
                    tempdd = (double) ret0[0];
                    xmax = tempdd;
                }
                step[0] = xmax*0.1D;
            }
            step[1] = 0.2D*start[1];
            if (m_scaleFlag)
            {
                step[2] = 0.2D*start[2];
            }

            // Nelder and Mead Simplex Regression
            LorentzianFunction f = new LorentzianFunction();
            addConstraint(1, -1, 0.0D);
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (allTest == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }


        // Static method allowing fitting of a data array to one or several of the above distributions
        public static void fitOneOrSeveralDistributions(double[] array)
        {
            int numberOfPoints = array.Length; // number of points
            double maxValue = Fmath.maximum(array); // maximum value of distribution
            double minValue = Fmath.minimum(array); // minimum value of distribution
            double span = maxValue - minValue; // span of distribution

            // Calculation of number of bins and bin width
            int numberOfBins = (int) Math.Ceiling(Math.Sqrt(numberOfPoints));
            double binWidth = span/numberOfBins;
            double averagePointsPerBin = numberOfPoints/(double) numberOfBins;

            // Option for altering bin width
            string comment = "Maximum value:  " + maxValue + Environment.NewLine;
            comment += "Minimum value:  " + minValue + Environment.NewLine;
            comment += "Suggested bin width:  " + binWidth + Environment.NewLine;
            comment += "Giving an average points per bin:  " + averagePointsPerBin + Environment.NewLine;
            comment += "If you wish to change the bin width enter the new value below \n";
            comment += "and click on OK\n";
            comment += "If you do NOT wish to change the bin width simply click on OK";
            PrintToScreen.WriteLine(comment + ", default = " + binWidth);
            binWidth = System.Convert.ToDouble(Console.ReadLine());

            // Create output file
            comment = "Input the name of the output text file\n";
            comment += "[Do not forget the extension, e.g.   .txt]";
            PrintToScreen.WriteLine(comment +
                                    ", default: fitOneOrSeveralDistributionsOutput.txt");
            string outputTitle =
                Console.ReadLine();
            FileOutput fout = new FileOutput(outputTitle, 'n');
            PrintToScreen.WriteLine("Fitting a set of data to one or more distributions");
            PrintToScreen.WriteLine("Class Regression/Stat: method fitAllDistributions");
            fout.dateAndTimeln();
            PrintToScreen.WriteLine();
            fout.printtab("Number of points: ");
            PrintToScreen.WriteLine(numberOfPoints);
            fout.printtab("Minimum value: ");
            PrintToScreen.WriteLine(minValue);
            fout.printtab("Maximum value: ");
            PrintToScreen.WriteLine(maxValue);
            fout.printtab("Number of bins: ");
            PrintToScreen.WriteLine(numberOfBins);
            fout.printtab("Bin width: ");
            PrintToScreen.WriteLine(binWidth);
            fout.printtab("Average number of points per bin: ");
            PrintToScreen.WriteLine(averagePointsPerBin);
            PrintToScreen.WriteLine();

            // Choose distributions and perform regression
            string[] comments = {
                                    "Gaussian Distribution", "Two parameter Log-normal Distribution",
                                    "Three parameter Log-normal Distribution", "Logistic Distribution",
                                    "Lorentzian Distribution",
                                    "Type 1 Extreme Distribution - Gumbel minimum order statistic",
                                    "Type 1 Extreme Distribution - Gumbel maximum order statistic",
                                    "Type 2 Extreme Distribution - Frechet", "Type 3 Extreme Distribution - Weibull",
                                    "Type 3 Extreme Distribution - Exponential Distribution",
                                    "Type 3 Extreme Distribution - Rayleigh Distribution", "Pareto Distribution",
                                    "Beta Distribution", "Gamma Distribution", "Erlang Distribution", "exit"
                                };
            string[] boxTitles = {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "exit"};
            string headerComment = "Choose next distribution to be fitted by clicking on box number";
            int defaultBox = 1;
            bool testDistType = true;
            RegressionAn reg = null;
            double[] coeff = null;
            while (testDistType)
            {
                PrintToScreen.WriteLine(headerComment);
                PrintToScreen.WriteLine(comments);
                PrintToScreen.WriteLine(boxTitles);
                PrintToScreen.WriteLine(defaultBox);
                int opt = System.Convert.ToInt32(Console.ReadLine());
                switch (opt)
                {
                    case 1: // Gaussian
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.gaussianPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("NORMAL (GAUSSIAN) DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Mean [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Standard deviation [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 2: // Two parameter Log-normal
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.logNormalTwoParPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("LOG-NORMAL DISTRIBUTION (two parameter statistic)");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Shape parameter [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 3: // Three parameter Log-normal
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.logNormalThreeParPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("LOG-NORMAL DISTRIBUTION (three parameter statistic)");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [alpha] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Shape parameter [beta] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scale parameter [gamma] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[3]);
                        regressionDetails(fout, reg);
                        break;
                    case 4: // Logistic
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.logisticPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("LOGISTIC DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [beta] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 5: // Lorentzian
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.lorentzianPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("LORENTZIAN DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Mean [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Half-height parameter [Gamma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 6: // Gumbel [minimum]
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.gumbelMinPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("TYPE 1 (GUMBEL) EXTREME DISTRIBUTION [MINIMUM ORDER STATISTIC]");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 7: // Gumbel [maximum]
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.gumbelMaxPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("TYPE 1 (GUMBEL) EXTREME DISTRIBUTION [MAXIMUM ORDER STATISTIC]");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 8: // Frechet
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.frechetPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("TYPE 2 (FRECHET) EXTREME DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Shape parameter [gamma] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[3]);
                        regressionDetails(fout, reg);
                        break;
                    case 9: // Weibull
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.weibullPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("TYPE 3 (WEIBULL) EXTREME DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Shape parameter [gamma] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[3]);
                        regressionDetails(fout, reg);
                        break;
                    case 10: // Exponential
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.exponentialPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("EXPONENTIAL DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [m_sigma] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        regressionDetails(fout, reg);
                        break;
                    case 11: // Rayleigh
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.rayleighPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("RAYLEIGH DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Scale parameter [beta] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        regressionDetails(fout, reg);
                        break;
                    case 12: // Pareto
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.paretoThreeParPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("PARETO DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Shape parameter [alpha] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [beta] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Threshold parameter [theta] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[3]);
                        regressionDetails(fout, reg);
                        break;
                    case 13: // Beta
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.betaMinMaxPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("BETA DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Shape parameter [alpha] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Shape parameter [beta] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("minimum limit [Min] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        fout.printtab("maximum limit [Max] ");
                        PrintToScreen.WriteLine(coeff[3]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[4]);
                        regressionDetails(fout, reg);
                        break;
                    case 14: // Gamma
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.gammaPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("GAMMA DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Location parameter [m_mu] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Scale parameter [beta] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        fout.printtab("Shape parameter [gamma] ");
                        PrintToScreen.WriteLine(coeff[2]);
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[3]);
                        regressionDetails(fout, reg);
                        break;
                    case 15: // Erlang
                        reg = new RegressionAn(array, binWidth);
                        reg.supressPrint();
                        reg.erlangPlot();
                        coeff = reg.getCoeff();
                        PrintToScreen.WriteLine("ERLANG DISTRIBUTION");
                        PrintToScreen.WriteLine("Best Estimates:");
                        fout.printtab("Shape parameter [lambda] ");
                        PrintToScreen.WriteLine(coeff[0]);
                        fout.printtab("Rate parameter [k] ");
                        PrintToScreen.WriteLine(reg.getKayValue());
                        fout.printtab("Scaling factor [Ao] ");
                        PrintToScreen.WriteLine(coeff[1]);
                        regressionDetails(fout, reg);
                        break;
                    case 16: // exit
                    default:
                        fout.close();
                        break;
                        //testDistType = false;
                }
            }
        }

        // Output method for fitOneOrSeveralDistributions
        protected static void regressionDetails(FileOutput fout, RegressionAn reg)
        {
            fout.println();
            fout.println("Regression details:");
            fout.printtab("Chi squared: ");
            fout.println(reg.getChiSquare());
            fout.printtab("Reduced chi squared: ");
            fout.println(reg.getReducedChiSquare());
            fout.printtab("Sum of squares: ");
            fout.println(reg.getSumOfSquares());
            fout.printtab("Degrees of freedom: ");
            fout.println(reg.getDegFree());
            fout.printtab("Number of iterations: ");
            fout.println(reg.getNiter());
            fout.printtab("maximum number of iterations allowed: ");
            fout.println(reg.getNmax());
            fout.println();
            fout.println();
        }


        // Calculate the multiple correlation coefficient
        protected void multCorrelCoeff(double[] yy, double[] yyCalc, double[] ww)
        {
            // sum of reciprocal weights squared
            double sumRecipW = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                sumRecipW += 1.0D/Fmath.square(ww[i]);
            }

            // weighted m_mean of yy
            double my = 0.0D;
            for (int j = 0; j < m_nData; j++)
            {
                my += yy[j]/Fmath.square(ww[j]);
            }
            my /= sumRecipW;


            // weighted m_mean of residuals
            double mr = 0.0D;
            double[] residuals = new double[m_nData];
            for (int j = 0; j < m_nData; j++)
            {
                residuals[j] = yy[j] - yyCalc[j];
                mr += residuals[j]/Fmath.square(ww[j]);
            }
            mr /= sumRecipW;

            // calculate yy weighted sum of squares
            double s2yy = 0.0D;
            for (int k = 0; k < m_nData; k++)
            {
                s2yy += Fmath.square((yy[k] - my)/ww[k]);
            }

            // calculate residual weighted sum of squares
            double s2r = 0.0D;
            for (int k = 0; k < m_nData; k++)
            {
                s2r += Fmath.square((residuals[k] - mr)/ww[k]);
            }

            // calculate multiple coefficient of determination
            m_sampleR2 = 1.0D - s2r/s2yy;
            m_sampleR = Math.Sqrt(m_sampleR2);

            // Calculate adjusted multiple coefficient of determination
            m_adjustedR2 = ((m_nData - 1)*m_sampleR2 - m_nXarrays)/(m_nData - m_nXarrays - 1);
            m_adjustedR = Math.Sqrt(m_adjustedR2);

            // F-ratio
            if (m_nXarrays > 1)
            {
                m_multipleF = m_sampleR2*(m_nData - m_nXarrays)/((1.0D - m_sampleR2)*(m_nXarrays - 1));
                m_adjustedF = m_adjustedR2*(m_nData - m_nXarrays)/((1.0D - m_adjustedR2)*(m_nXarrays - 1));
            }
        }

        // Get the Sample Correlation Coefficient
        public double getSampleR()
        {
            return m_sampleR;
        }

        // Get the Sample Correlation Coefficient Squared
        public double getSampleR2()
        {
            return m_sampleR2;
        }

        // Get the Adjusted Sample Correlation Coefficient
        public double getAdjustedR()
        {
            return m_adjustedR;
        }

        // Get the Adjusted Sample Correlation Coefficient Squared
        public double getAdjustedR2()
        {
            return m_adjustedR2;
        }

        // Get the Multiple Correlation Coefficient F ratio
        public double getMultipleF()
        {
            if (m_nXarrays == 1)
            {
                PrintToScreen.WriteLine(
                    "Regression.getMultipleF - The regression is not a multple regession: NaN returned");
            }
            return m_multipleF;
        }

        // check data arrays for sign, maximum, minimum and peak
        protected static List<Object> dataSign(double[] data)
        {
            List<Object> ret = new List<Object>();
            int n = data.Length;

            double max = data[0]; // maximum
            int maxi = 0; // index of above
            double min = data[0]; // minimum
            int mini = 0; // index of above
            double peak = 0.0D; // peak: larger of maximum and any abs(negative minimum)
            int peaki = -1; // index of above
            int signFlag = -1; // 0 all positive; 1 all negative; 2 positive and negative
            double shift = 0.0D; // shift to make all positive if a mixture of positive and negative
            double mean = 0.0D; // m_mean value
            int signCheckZero = 0; // number of zero values
            int signCheckNeg = 0; // number of positive values
            int signCheckPos = 0; // number of negative values

            for (int i = 0; i < n; i++)
            {
                mean = +data[i];
                if (data[i] > max)
                {
                    max = data[i];
                    maxi = i;
                }
                if (data[i] < min)
                {
                    min = data[i];
                    mini = i;
                }
                if (data[i] == 0.0D)
                {
                    signCheckZero++;
                }
                if (data[i] > 0.0D)
                {
                    signCheckPos++;
                }
                if (data[i] < 0.0D)
                {
                    signCheckNeg++;
                }
            }
            mean /= n;

            if ((signCheckZero + signCheckPos) == n)
            {
                peak = max;
                peaki = maxi;
                signFlag = 0;
            }
            else
            {
                if ((signCheckZero + signCheckNeg) == n)
                {
                    peak = min;
                    peaki = mini;
                    signFlag = 1;
                }
                else
                {
                    peak = max;
                    peaki = maxi;
                    if (-min > max)
                    {
                        peak = min;
                        peak = mini;
                    }
                    signFlag = 2;
                    shift = -min;
                }
            }

            // transfer results to the List
            ret.Add(min);
            ret.Add(mini);
            ret.Add(max);
            ret.Add(maxi);
            ret.Add(peak);
            ret.Add(peaki);
            ret.Add(signFlag);
            ret.Add(shift);
            ret.Add(mean);
            ret.Add(signCheckZero);
            ret.Add(signCheckPos);
            ret.Add(signCheckNeg);


            return ret;
        }

        public void frechet()
        {
            fitFrechet(0, 0);
        }

        public void frechetPlot()
        {
            fitFrechet(1, 0);
        }

        public void frechetTwoPar()
        {
            fitFrechet(0, 1);
        }

        public void frechetTwoParPlot()
        {
            fitFrechet(1, 1);
        }

        public void frechetStandard()
        {
            fitFrechet(0, 2);
        }

        public void frechetStandardPlot()
        {
            fitFrechet(1, 2);
        }

        protected void fitFrechet(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 0:
                    m_lastMethod = 13;
                    m_nTerms = 4;
                    break;
                case 1:
                    m_lastMethod = 14;
                    m_nTerms = 3;
                    break;
                case 2:
                    m_lastMethod = 15;
                    m_nTerms = 2;
                    break;
            }
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }
            m_frechetWeibull = true;
            fitFrechetWeibull(allTest, typeFlag);
        }

        // method for fitting data to either a Frechet or a Weibull distribution
        protected void fitFrechetWeibull(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            int tempi = 0;
            tempi = (int) retY[5];
            int peaki = tempi;
            tempd = (double) retY[8];
            double mean = tempd;

            // check for infinity
            bool testInf = true;
            double dof = m_degreesOfFreedom;
            while (testInf)
            {
                if (infinityCheck(yPeak, peaki))
                {
                    dof--;
                    if (dof < 1 && !m_blnIgnoreDofFcheck)
                    {
                        throw new HCException("The effective degrees of freedom have been reduced to zero");
                    }
                    retY = dataSign(m_yData);
                    tempd = (double) retY[4];
                    yPeak = tempd;
                    tempi = (int) retY[5];
                    peaki = tempi;
                    tempd = (double) retY[8];
                    mean = tempd;
                }
                else
                {
                    testInf = false;
                }
            }

            // check sign of y data
            string ss = "Weibull";
            if (m_frechetWeibull)
            {
                ss = "Frechet";
            }
            bool ySignFlag = false;
            if (yPeak < 0.0D)
            {
                reverseYsign(ss);
                retY = dataSign(m_yData);
                yPeak = -yPeak;
                ySignFlag = true;
            }

            // check y values for all very small values
            bool magCheck = false;
            double magScale = checkYallSmall(yPeak, ss);
            if (magScale != 1.0D)
            {
                magCheck = true;
                yPeak = 1.0D;
            }

            // minimum value of x
            List<Object> retX = dataSign(
                ArrayHelper.GetRowCopy<double>(
                    m_xData,
                    0));
            tempd = (double) retX[0];
            double xMin = tempd;

            // maximum value of x
            tempd = (double) retX[2];
            double xMax = tempd;

            // Calculate  x value at peak y (estimate of the 'distribution mode')
            double distribMode = m_xData[0, peaki];

            // Calculate an estimate of the half-height width
            double sd = Math.Log(2.0D)*halfWidth(
                                           ArrayHelper.GetRowCopy<double>(
                                               m_xData,
                                               0),
                                           m_yData);

            // Save x-y-w data
            double[] xx = new double[m_nData];
            double[] yy = new double[m_nData];
            double[] ww = new double[m_nData];

            for (int i = 0; i < m_nData; i++)
            {
                xx[i] = m_xData[0, i];
                yy[i] = m_yData[i];
                ww[i] = m_weight[i];
            }

            // Calculate the cumulative probability and return ordinate scaling factor estimate
            double[] cumX = new double[m_nData];
            double[] cumY = new double[m_nData];
            double[] cumW = new double[m_nData];
            ErrorProp[] cumYe = ErrorProp.oneDarray(m_nData);
            double yScale = calculateCumulativeValues(cumX, cumY, cumW, cumYe, peaki, yPeak, distribMode, ss);

            //Calculate loglog v log transforms
            if (m_frechetWeibull)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    cumYe[i] = ErrorProp.over(1.0D, cumYe[i]);
                    cumYe[i] = ErrorProp.Log(cumYe[i]);
                    cumYe[i] = ErrorProp.Log(cumYe[i]);
                    cumY[i] = cumYe[i].getValue();
                    cumW[i] = cumYe[i].getError();
                }
            }
            else
            {
                for (int i = 0; i < m_nData; i++)
                {
                    cumYe[i] = ErrorProp.minus(1.0D, cumYe[i]);
                    cumYe[i] = ErrorProp.over(1.0D, cumYe[i]);
                    cumYe[i] = ErrorProp.Log(cumYe[i]);
                    cumYe[i] = ErrorProp.Log(cumYe[i]);
                    cumY[i] = cumYe[i].getValue();
                    cumW[i] = cumYe[i].getError();
                }
            }

            // Fill data arrays with transformed data
            for (int i = 0; i < m_nData; i++)
            {
                m_xData[0, i] = cumX[i];
                m_yData[i] = cumY[i];
                m_weight[i] = cumW[i];
            }
            bool weightOptHold = m_weightOpt;
            m_weightOpt = true;

            // Nelder and Mead Simplex Regression for semi-linearised Frechet or Weibull
            // disable statistical analysis
            bool statFlagHold = m_statFlag;
            m_statFlag = false;

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                start[i] = 1.0D;
                step[i] = 0.2D;
            }
            //double[] gammamin = null;
            //double gammat = 0;
            switch (typeFlag)
            {
                case 0:
                    start[0] = xMin - Math.Abs(0.1D*xMin); //m_mu
                    start[1] = sd; //m_sigma
                    start[2] = 4.0; // gamma
                    // step sizes
                    step[0] = 0.2D*start[0];
                    if (step[0] == 0.0D)
                    {
                        List<Object> ret0 = dataSign(
                            ArrayHelper.GetRowCopy<double>(
                                m_xData,
                                0));
                        double tempdd = 0;
                        tempdd = (double) ret0[2];
                        double xmax = tempdd;
                        if (xmax == 0.0D)
                        {
                            tempdd = (double) ret0[0];
                            xmax = tempdd;
                        }
                        step[0] = xmax*0.1D;
                    }
                    step[1] = 0.2D*start[1];
                    step[2] = 0.5D*start[2];
                    addConstraint(0, +1, xMin);
                    addConstraint(1, -1, 0.0D);
                    addConstraint(2, -1, 0.0D);
                    break;
                case 1:
                    start[0] = sd; //m_sigma
                    start[1] = 4.0; // gamma
                    // step sizes
                    step[0] = 0.2D*start[0];
                    step[1] = 0.5D*start[1];
                    addConstraint(0, -1, 0.0D);
                    addConstraint(1, -1, 0.0D);
                    break;
                case 2:
                    start[0] = 4.0; // gamma
                    // step size
                    step[0] = 0.5D*start[0];
                    addConstraint(0, -1, 0.0D);
                    break;
            }

            // Create instance of loglog function and perform regression
            if (m_frechetWeibull)
            {
                FrechetFunctionTwo f = new FrechetFunctionTwo();
                f.typeFlag = typeFlag;
                Object regFun2 = f;
                PrintToScreen.WriteLine("pppp " + start[0] + "   " + start[1] + "   " + start[2]);

                nelderMead(regFun2, start, step, m_fTol, m_nMax);
            }
            else
            {
                WeibullFunctionTwo f = new WeibullFunctionTwo();
                f.typeFlag = typeFlag;
                Object regFun2 = f;
                nelderMead(regFun2, start, step, m_fTol, m_nMax);
            }

            // Get best estimates of loglog regression
            double[] ests = (double[]) m_best.Clone();

            // Nelder and Mead Simplex Regression for Frechet or Weibull
            // using best estimates from loglog regression as initial estimates

            // re-enable statistical analysis if statFlag was set to true
            m_statFlag = statFlagHold;

            // restore data reversing the loglog transform but maintaining any sign reversals
            m_weightOpt = weightOptHold;
            for (int i = 0; i < m_nData; i++)
            {
                m_xData[0, i] = xx[i];
                m_yData[i] = yy[i];
                m_weight[i] = ww[i];
            }

            // Fill arrays needed by the Simplex
            switch (typeFlag)
            {
                case 0:
                    start[0] = ests[0]; //m_mu
                    start[1] = ests[1]; //m_sigma
                    start[2] = ests[2]; //gamma
                    if (m_scaleFlag)
                    {
                        start[3] = 1.0/yScale; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    if (step[0] == 0.0D)
                    {
                        List<Object> ret0 = dataSign(
                            ArrayHelper.GetRowCopy<double>(
                                m_xData,
                                0));
                        double tempdd = 0;
                        tempdd = (double) ret0[2];
                        double xmax = tempdd;
                        if (xmax == 0.0D)
                        {
                            tempdd = (double) ret0[0];
                            xmax = tempdd;
                        }
                        step[0] = xmax*0.1D;
                    }
                    step[1] = 0.1D*start[1];
                    step[2] = 0.1D*start[2];
                    if (m_scaleFlag)
                    {
                        step[3] = 0.1D*start[3];
                    }
                    break;
                case 1:
                    start[0] = ests[0]; //m_sigma
                    start[1] = ests[1]; //gamma
                    if (m_scaleFlag)
                    {
                        start[2] = 1.0/yScale; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    step[1] = 0.1D*start[1];
                    if (m_scaleFlag)
                    {
                        step[2] = 0.1D*start[2];
                    }
                    break;
                case 2:
                    start[0] = ests[0]; //gamma
                    if (m_scaleFlag)
                    {
                        start[1] = 1.0/yScale; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    if (m_scaleFlag)
                    {
                        step[1] = 0.1D*start[1];
                    }
                    break;
            }

            // Create instance of Frechet function and perform regression
            if (m_frechetWeibull)
            {
                FrechetFunctionOne ff = new FrechetFunctionOne();
                ff.typeFlag = typeFlag;
                ff.scaleOption = m_scaleFlag;
                ff.scaleFactor = m_yScaleFactor;
                Object regFun3 = ff;
                nelderMead(regFun3, start, step, m_fTol, m_nMax);
                if (allTest == 1)
                {
                    // Print results
                    if (!m_blnSupressPrint)
                    {
                        print();
                    }
                    // Plot results
                    int flag = plotXY(ff);
                    if (flag != -2 && !m_blnSupressYYplot)
                    {
                        plotYY();
                    }
                }
            }
            else
            {
                WeibullFunctionOne ff = new WeibullFunctionOne();
                ff.typeFlag = typeFlag;
                ff.scaleOption = m_scaleFlag;
                ff.scaleFactor = m_yScaleFactor;
                Object regFun3 = ff;
                nelderMead(regFun3, start, step, m_fTol, m_nMax);
                if (allTest == 1)
                {
                    // Print results
                    if (!m_blnSupressPrint)
                    {
                        print();
                    }
                    // Plot results
                    int flag = plotXY(ff);
                    if (flag != -2 && !m_blnSupressYYplot)
                    {
                        plotYY();
                    }
                }
            }

            // restore data
            m_weightOpt = weightOptHold;
            if (magCheck)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = yy[i]/magScale;
                    if (m_weightOpt)
                    {
                        m_weight[i] = ww[i]/magScale;
                    }
                }
            }
            if (ySignFlag)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        // Check for y value = infinity
        public bool infinityCheck(double yPeak, int peaki)
        {
            bool flag = false;
            if (yPeak == 1.0D/0.0D || yPeak == -1.0D/0.0D)
            {
                int ii = peaki + 1;
                if (peaki == m_nData - 1)
                {
                    ii = peaki - 1;
                }
                m_xData[0, peaki] = m_xData[0, ii];
                m_yData[peaki] = m_yData[ii];
                m_weight[peaki] = m_weight[ii];
                PrintToScreen.WriteLine("An infinty has been removed at point " + peaki);
                flag = true;
            }
            return flag;
        }

        // reverse sign of y values if negative
        public void reverseYsign(string ss)
        {
            PrintToScreen.WriteLine("This implementation of the " + ss +
                                    " distributions takes only positive y values\n(noise taking low values below zero are allowed)");
            PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
            for (int i = 0; i < m_nData; i++)
            {
                m_yData[i] = -m_yData[i];
            }
        }

        // check y values for all y are very small value
        public double checkYallSmall(double yPeak, string ss)
        {
            double magScale = 1.0D;
            double recipYpeak = Fmath.truncate(1.0/yPeak, 4);
            if (yPeak < 1e-4)
            {
                PrintToScreen.WriteLine(ss + " fitting: The ordinate axis (y axis) has been rescaled by " + recipYpeak +
                                        " to reduce rounding errors");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] *= recipYpeak;
                    if (m_weightOpt)
                    {
                        m_weight[i] *= recipYpeak;
                    }
                }
                magScale = recipYpeak;
            }
            return magScale;
        }

        // Calculate cumulative values for distributions with a single independent variable
        // Entered parameters
        // peaki - index of the y value peak
        // yPeak - y value of the y peak
        // distribMode - x value at peak y (estimate of the 'distribution mode')
        // ss - name of the distribution to be fitted, e.g. "Frechet"
        // Returns:
        // return statement - an estimate of the scaling factor
        // cumX - x data as a one dimensional array with zero values replaced by average of adjacent values
        // cumY - cumulative y values
        // cumW - cumulative y weight values
        // cumYe - cumulative Y values as ErrorProp
        public double calculateCumulativeValues(
            double[] cumX,
            double[] cumY,
            double[] cumW,
            ErrorProp[] cumYe,
            int peaki,
            double yPeak,
            double distribMode,
            string ss)
        {
            // Put independent values into a one-dimensional array
            cumX[0] = m_xData[0, 0];
            for (int i = 1; i < m_nData; i++)
            {
                cumX[i] = m_xData[0, i];
            }

            // Create an array of ErrorProps from the independent values and their weights
            ErrorProp[] yE = ErrorProp.oneDarray(m_nData);
            for (int i = 0; i < m_nData; i++)
            {
                yE[i].reset(m_yData[i], m_weight[i]);
            }

            // check on shape of data for first step of cumulative calculation
            if (peaki != 0)
            {
                if (peaki == m_nData - 1)
                {
                    PrintToScreen.WriteLine("The data does not cover a wide enough range of x values to fit to a " + ss +
                                            " distribution with any accuracy");
                    PrintToScreen.WriteLine(
                        "The regression will be attempted but you should treat any result with great caution");
                }
                if (m_yData[0] < m_yData[1]*0.5D && m_yData[0] > distribMode*0.02D)
                {
                    ErrorProp x0 = new ErrorProp(0.0D, 0.0D);
                    x0 = yE[0].times(m_xData[0, 1] - m_xData[0, 0]);
                    x0 = x0.over(yE[1].minus(yE[0]));
                    x0 = ErrorProp.minus(m_xData[0, 0], x0);
                    if (m_yData[0] >= 0.9D*yPeak)
                    {
                        x0 = (x0.plus(m_xData[0, 0])).over(2.0D);
                    }
                    if (x0.getValue() < 0.0D)
                    {
                        x0.reset(0.0D, 0.0D);
                    }
                    cumYe[0] = yE[0].over(2.0D);
                    cumYe[0] = cumYe[0].times(ErrorProp.minus(m_xData[0, 0], x0));
                }
                else
                {
                    cumYe[0].reset(0.0D, m_weight[0]);
                }
            }
            else
            {
                cumYe[0].reset(0.0D, m_weight[0]);
            }

            // cumulative calculation for rest of the points (trapezium approximation)
            for (int i = 1; i < m_nData; i++)
            {
                cumYe[i] = yE[i].plus(yE[i - 1]);
                cumYe[i] = cumYe[i].over(2.0D);
                cumYe[i] = cumYe[i].times(m_xData[0, i] - m_xData[0, i - 1]);
                cumYe[i] = cumYe[i].plus(cumYe[i - 1]);
            }

            // check on shape of data for  step of cumulative calculation
            ErrorProp cumYtotal = cumYe[m_nData - 1].Copy();
            if (peaki == m_nData - 1)
            {
                cumYtotal = cumYtotal.times(2.0D);
            }
            else
            {
                if (m_yData[m_nData - 1] < m_yData[m_nData - 2]*0.5D && m_yData[m_nData - 1] > distribMode*0.02D)
                {
                    ErrorProp xn = new ErrorProp();
                    xn = yE[m_nData - 1].times(m_xData[0, m_nData - 2] - m_xData[0, m_nData - 1]);
                    xn = xn.over(yE[m_nData - 2].minus(yE[m_nData - 1]));
                    xn = ErrorProp.minus(m_xData[0, m_nData - 1], xn);
                    if (m_yData[0] >= 0.9D*yPeak)
                    {
                        xn = (xn.plus(m_xData[0, m_nData - 1])).over(2.0D);
                    }
                    cumYtotal =
                        cumYtotal.plus(ErrorProp.times(0.5D, (yE[m_nData - 1].times(xn.minus(m_xData[0, m_nData - 1])))));
                }
            }

            // Fill cumulative Y and W arrays
            for (int i = 0; i < m_nData; i++)
            {
                cumY[i] = cumYe[i].getValue();
                cumW[i] = cumYe[i].getError();
            }

            // estimate y scaling factor
            double yScale = 1.0D/cumYtotal.getValue();
            for (int i = 0; i < m_nData; i++)
            {
                cumYe[i] = cumYe[i].over(cumYtotal);
            }

            // check for zero and negative  values
            int jj = 0;
            bool test = true;
            for (int i = 0; i < m_nData; i++)
            {
                if (cumYe[i].getValue() <= 0.0D)
                {
                    if (i <= jj)
                    {
                        test = true;
                        jj = i;
                        while (test)
                        {
                            jj++;
                            if (jj >= m_nData)
                            {
                                throw new ArithmeticException("all zero cumulative data!!");
                            }
                            if (cumYe[jj].getValue() > 0.0D)
                            {
                                cumYe[i] = cumYe[jj].Copy();
                                cumX[i] = cumX[jj];
                                test = false;
                            }
                        }
                    }
                    else
                    {
                        if (i == m_nData - 1)
                        {
                            cumYe[i] = cumYe[i - 1].Copy();
                            cumX[i] = cumX[i - 1];
                        }
                        else
                        {
                            cumYe[i] = cumYe[i - 1].plus(cumYe[i + 1]);
                            cumYe[i] = cumYe[i].over(2.0D);
                            cumX[i] = (cumX[i - 1] + cumX[i + 1])/2.0D;
                        }
                    }
                }
            }

            // check for unity value
            jj = m_nData - 1;
            for (int i = m_nData - 1; i >= 0; i--)
            {
                if (cumYe[i].getValue() >= 1.0D)
                {
                    if (i >= jj)
                    {
                        test = true;
                        jj = m_nData - 1;
                        while (test)
                        {
                            jj--;
                            if (jj < 0)
                            {
                                throw new ArithmeticException("all unity cumulative data!!");
                            }
                            if (cumYe[jj].getValue() < 1.0D)
                            {
                                cumYe[i] = cumYe[jj].Copy();
                                cumX[i] = cumX[jj];
                                test = false;
                            }
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            cumYe[i] = cumYe[i + 1].Copy();
                            cumX[i] = cumX[i + 1];
                        }
                        else
                        {
                            cumYe[i] = cumYe[i - 1].plus(cumYe[i + 1]);
                            cumYe[i] = cumYe[i].over(2.0D);
                            cumX[i] = (cumX[i - 1] + cumX[i + 1])/2.0D;
                        }
                    }
                }
            }

            return yScale;
        }

        public void weibull()
        {
            fitWeibull(0, 0);
        }

        public void weibullPlot()
        {
            fitWeibull(1, 0);
        }

        public void weibullTwoPar()
        {
            fitWeibull(0, 1);
        }

        public void weibullTwoParPlot()
        {
            fitWeibull(1, 1);
        }

        public void weibullStandard()
        {
            fitWeibull(0, 2);
        }

        public void weibullStandardPlot()
        {
            fitWeibull(1, 2);
        }

        protected void fitWeibull(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 0:
                    m_lastMethod = 16;
                    m_nTerms = 4;
                    break;
                case 1:
                    m_lastMethod = 17;
                    m_nTerms = 3;
                    break;
                case 2:
                    m_lastMethod = 18;
                    m_nTerms = 2;
                    break;
            }
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }
            m_frechetWeibull = false;
            fitFrechetWeibull(allTest, typeFlag);
        }

        public void gumbelMin()
        {
            fitGumbel(0, 0);
        }

        public void gumbelMinPlot()
        {
            fitGumbel(1, 0);
        }

        public void gumbelMax()
        {
            fitGumbel(0, 1);
        }

        public void gumbelMaxPlot()
        {
            fitGumbel(1, 1);
        }

        public void gumbelMinOnePar()
        {
            fitGumbel(0, 2);
        }

        public void gumbelMinOneParPlot()
        {
            fitGumbel(1, 2);
        }

        public void gumbelMaxOnePar()
        {
            fitGumbel(0, 3);
        }

        public void gumbelMaxOneParPlot()
        {
            fitGumbel(1, 3);
        }

        public void gumbelMinStandard()
        {
            fitGumbel(0, 4);
        }

        public void gumbelMinStandardPlot()
        {
            fitGumbel(1, 4);
        }

        public void gumbelMaxStandard()
        {
            fitGumbel(0, 5);
        }

        public void gumbelMaxStandardPlot()
        {
            fitGumbel(1, 5);
        }

        // No parameters set for estimation
        // Correlation coefficient and plot
        protected void noParameters(string ss)
        {
            PrintToScreen.WriteLine(ss + " Regression");
            PrintToScreen.WriteLine("No parameters set for estimation");
            PrintToScreen.WriteLine("Theoretical curve obtained");
            string filename1 = "RegressOutput.txt";
            string filename2 = "RegressOutputN.txt";
            FileOutput fout = new FileOutput(filename1, 'n');
            PrintToScreen.WriteLine("Results printed to the file " + filename2);
            fout.dateAndTimeln(filename1);
            PrintToScreen.WriteLine("No parameters set for estimation");
            switch (m_lastMethod)
            {
                case 11:
                    PrintToScreen.WriteLine("Minimal Standard Gumbel p(x) = Exp(x)Exp(-Exp(x))");
                    for (int i = 0; i < m_nData; i++)
                    {
                        m_yCalc[i] = Math.Exp(m_xData[0, i])*Math.Exp(-Math.Exp(m_xData[0, i]));
                    }
                    break;
                case 12:
                    PrintToScreen.WriteLine("Maximal Standard Gumbel p(x) = Exp(-x)Exp(-Exp(-x))");
                    for (int i = 0; i < m_nData; i++)
                    {
                        m_yCalc[i] = Math.Exp(-m_xData[0, i])*Math.Exp(-Math.Exp(-m_xData[0, i]));
                    }
                    break;
                case 21:
                    PrintToScreen.WriteLine("Standard Exponential p(x) = Exp(-x)");
                    for (int i = 0; i < m_nData; i++)
                    {
                        m_yCalc[i] = Math.Exp(-m_xData[0, i]);
                    }
                    break;
            }
            m_sumOfSquares = 0.0D;
            m_chiSquare = 0.0D;
            double temp = 0.0D;
            for (int i = 0; i < m_nData; i++)
            {
                temp = Fmath.square(m_yData[i] - m_yCalc[i]);
                m_sumOfSquares += temp;
                m_chiSquare += temp/Fmath.square(m_weight[i]);
            }
            double corrCoeff = Stat.CorrCoeff(m_yData, m_yCalc);
            fout.printtab("Correlation Coefficient");
            PrintToScreen.WriteLine(Fmath.truncate(corrCoeff, m_prec));
            fout.printtab("Correlation Coefficient Probability");
            PrintToScreen.WriteLine(Fmath.truncate(1.0D - Stat.linearCorrCoeffProb(corrCoeff, m_degreesOfFreedom - 1),
                                                   m_prec));

            fout.printtab("Sum of Squares");
            PrintToScreen.WriteLine(Fmath.truncate(m_sumOfSquares, m_prec));
            if (m_weightOpt || m_trueFreq)
            {
                fout.printtab("Chi Square");
                PrintToScreen.WriteLine(Fmath.truncate(m_chiSquare, m_prec));
                fout.printtab("chi square probability");
                PrintToScreen.WriteLine(Fmath.truncate(Stat.chiSquareProb(m_chiSquare, m_degreesOfFreedom - 1), m_prec));
            }
            PrintToScreen.WriteLine(" ");

            fout.printtab("x", m_field);
            fout.printtab("p(x) [expl]", m_field);
            fout.printtab("p(x) [calc]", m_field);
            PrintToScreen.WriteLine("residual");

            for (int i = 0; i < m_nData; i++)
            {
                fout.printtab(Fmath.truncate(m_xData[0, i], m_prec), m_field);
                fout.printtab(Fmath.truncate(m_yData[i], m_prec), m_field);
                fout.printtab(Fmath.truncate(m_yCalc[i], m_prec), m_field);
                PrintToScreen.WriteLine(Fmath.truncate(m_yData[i] - m_yCalc[i], m_prec));
            }
            fout.close();
            plotXY();
            if (!m_blnSupressYYplot)
            {
                plotYY();
            }
        }

        protected void fitGumbel(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 0:
                    m_lastMethod = 7;
                    m_nTerms = 3;
                    break;
                case 1:
                    m_lastMethod = 8;
                    m_nTerms = 3;
                    break;
                case 2:
                    m_lastMethod = 9;
                    m_nTerms = 2;
                    break;
                case 3:
                    m_lastMethod = 10;
                    m_nTerms = 2;
                    break;
                case 4:
                    m_lastMethod = 11;
                    m_nTerms = 1;
                    break;
                case 5:
                    m_lastMethod = 12;
                    m_nTerms = 1;
                    break;
            }
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }
            if (m_nTerms == 0)
            {
                noParameters("Gumbel");
            }
            else
            {
                // order data into ascending order of the abscissae
                sort(
                    ArrayHelper.GetRowCopy(
                        m_xData,
                        0),
                    m_yData,
                    m_weight);

                // check sign of y data
                double tempd = 0;
                List<Object> retY = dataSign(m_yData);
                tempd = (double) retY[4];
                double yPeak = tempd;
                bool yFlag = false;

                if (yPeak < 0.0D)
                {
                    PrintToScreen.WriteLine(
                        "Regression.fitGumbel(): This implementation of the Gumbel distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                    PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                    for (int i = 0; i < m_nData; i++)
                    {
                        m_yData[i] = -m_yData[i];
                    }
                    retY = dataSign(m_yData);
                    yFlag = true;
                }

                // check  x data
                List<Object> retX = dataSign(
                    ArrayHelper.GetRowCopy(
                        m_xData,
                        0));
                int tempi = 0;

                // Calculate  x value at peak y (estimate of the 'distribution mode')
                tempi = (int) retY[5];
                int peaki = tempi;
                double distribMode = m_xData[0, peaki];

                // Calculate an estimate of the half-height width
                double sd = halfWidth(
                    ArrayHelper.GetRowCopy(
                        m_xData,
                        0),
                    m_yData);

                // Nelder and Mead Simplex Regression for Gumbel
                // Fill arrays needed by the Simplex
                double[] start = new double[m_nTerms];
                double[] step = new double[m_nTerms];
                switch (typeFlag)
                {
                    case 0:
                    case 1:
                        start[0] = distribMode; //m_mu
                        start[1] = sd*Math.Sqrt(6.0D)/Math.PI; //m_sigma
                        if (m_scaleFlag)
                        {
                            start[2] = yPeak*start[1]*Math.Exp(1); //y axis scaling factor
                        }
                        step[0] = 0.1D*start[0];
                        if (step[0] == 0.0D)
                        {
                            List<Object> ret0 = dataSign(
                                ArrayHelper.GetRowCopy(
                                    m_xData,
                                    0));
                            double tempdd = 0;
                            tempdd = (double) ret0[2];
                            double xmax = tempdd;
                            if (xmax == 0.0D)
                            {
                                tempdd = (double) ret0[0];
                                xmax = tempdd;
                            }
                            step[0] = xmax*0.1D;
                        }
                        step[1] = 0.1D*start[1];
                        if (m_scaleFlag)
                        {
                            step[2] = 0.1D*start[2];
                        }

                        // Add constraints
                        addConstraint(1, -1, 0.0D);
                        break;
                    case 2:
                    case 3:
                        start[0] = sd*Math.Sqrt(6.0D)/Math.PI; //m_sigma
                        if (m_scaleFlag)
                        {
                            start[1] = yPeak*start[0]*Math.Exp(1); //y axis scaling factor
                        }
                        step[0] = 0.1D*start[0];
                        if (m_scaleFlag)
                        {
                            step[1] = 0.1D*start[1];
                        }
                        // Add constraints
                        addConstraint(0, -1, 0.0D);
                        break;
                    case 4:
                    case 5:
                        if (m_scaleFlag)
                        {
                            start[0] = yPeak*Math.Exp(1); //y axis scaling factor
                            step[0] = 0.1D*start[0];
                        }
                        break;
                }

                // Create instance of Gumbel function
                GumbelFunction ff = new GumbelFunction();

                // Set minimum type / maximum type option
                ff.typeFlag = typeFlag;

                // Set ordinate scaling option
                ff.scaleOption = m_scaleFlag;
                ff.scaleFactor = m_yScaleFactor;

                if (typeFlag < 4)
                {
                    // Perform simplex regression
                    Object regFun3 = ff;
                    nelderMead(regFun3, start, step, m_fTol, m_nMax);

                    if (allTest == 1)
                    {
                        // Print results
                        if (!m_blnSupressPrint)
                        {
                            print();
                        }

                        // Plot results
                        int flag = plotXY(ff);
                        if (flag != -2 && !m_blnSupressYYplot)
                        {
                            plotYY();
                        }
                    }
                }
                else
                {
                    // calculate exp exp term
                    double[,] xxx = new double[1,m_nData];
                    double aa = 1.0D;
                    if (typeFlag == 5)
                    {
                        aa = -1.0D;
                    }
                    for (int i = 0; i < m_nData; i++)
                    {
                        xxx[0, i] = Math.Exp(aa*m_xData[0, i])*Math.Exp(-Math.Exp(aa*m_xData[0, i]));
                    }

                    // perform linear regression
                    m_linNonLin = true;
                    generalLinear(xxx);

                    if (!m_blnSupressPrint)
                    {
                        print();
                    }
                    if (!m_blnSupressYYplot)
                    {
                        plotYY();
                    }
                    plotXY();

                    m_linNonLin = false;
                }

                if (yFlag)
                {
                    // restore data
                    for (int i = 0; i < m_nData - 1; i++)
                    {
                        m_yData[i] = -m_yData[i];
                    }
                }
            }
        }

        // sort elements x, y and w arrays of doubles into ascending order of the x array
        // using selection sort method
        protected static void sort(double[] x, double[] y, double[] w)
        {
            int index = 0;
            int lastIndex = -1;
            int n = x.Length;
            double holdx = 0.0D;
            double holdy = 0.0D;
            double holdw = 0.0D;

            while (lastIndex < n - 1)
            {
                index = lastIndex + 1;
                for (int i = lastIndex + 2; i < n; i++)
                {
                    if (x[i] < x[index])
                    {
                        index = i;
                    }
                }
                lastIndex++;
                holdx = x[index];
                x[index] = x[lastIndex];
                x[lastIndex] = holdx;
                holdy = y[index];
                y[index] = y[lastIndex];
                y[lastIndex] = holdy;
                holdw = w[index];
                w[index] = w[lastIndex];
                w[lastIndex] = holdw;
            }
        }

        // returns rough estimate of half-height width
        protected static double halfWidth(double[] xData, double[] yData)
        {
            // Find index of maximum value and calculate half maximum height
            double ymax = yData[0];
            int imax = 0;
            int n = xData.Length;

            for (int i = 1; i < n; i++)
            {
                if (yData[i] > ymax)
                {
                    ymax = yData[i];
                    imax = i;
                }
            }
            ymax /= 2.0D;

            // Find index of point at half maximum value on the low side of the maximum
            double halfXlow = -1.0D;
            double halfYlow = -1.0D;
            double temp = -1.0D;
            int ihl = -1;
            if (imax > 0)
            {
                ihl = imax - 1;
                halfYlow = Math.Abs(ymax - yData[ihl]);
                for (int i = imax - 2; i >= 0; i--)
                {
                    temp = Math.Abs(ymax - yData[i]);
                    if (temp < halfYlow)
                    {
                        halfYlow = temp;
                        ihl = i;
                    }
                }
                halfXlow = Math.Abs(xData[ihl] - xData[imax]);
            }

            // Find index of point at half maximum value on the high side of the maximum
            double halfXhigh = -1.0D;
            double halfYhigh = -1.0D;
            temp = -1.0D;
            int ihh = -1;
            if (imax < n - 1)
            {
                ihh = imax + 1;
                halfYhigh = Math.Abs(ymax - yData[ihh]);
                for (int i = imax + 2; i < n; i++)
                {
                    temp = Math.Abs(ymax - yData[i]);
                    if (temp < halfYhigh)
                    {
                        halfYhigh = temp;
                        ihh = i;
                    }
                }
                halfXhigh = Math.Abs(xData[ihh] - xData[imax]);
            }

            // Calculate width at half height
            double halfw = 0.0D;
            if (ihl != -1)
            {
                halfw += halfXlow;
            }
            if (ihh != -1)
            {
                halfw += halfXhigh;
            }

            return halfw;
        }

        //  FIT TO A SIMPLE EXPOPNENTIAL

        // method for fitting data to a simple exponential
        public void exponentialSimple()
        {
            fitsexponentialSimple(0);
        }

        // method for fitting data to a simple exponential
        public void exponentialSimplePlot()
        {
            fitsexponentialSimple(1);
        }

        // method for fitting data to a simple exponential
        protected void fitsexponentialSimple(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 43;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2;
            if (!m_scaleFlag)
            {
                m_nTerms = 1;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // Estimate  of yscale and A - linear transform
            int nLen = m_yData.Length;
            int nLin = nLen;
            bool[] zeros = new bool[nLen];
            for (int i = 0; i < nLen; i++)
            {
                zeros[i] = true;
                if (m_xData[0, i] <= 0.0D || m_yData[i] <= 0.0D)
                {
                    zeros[i] = false;
                    nLin--;
                }
            }
            double[] xlin = new double[nLin];
            double[] ylin = new double[nLin];
            double[] wlin = new double[nLin];
            int counter = 0;
            for (int i = 0; i < nLen; i++)
            {
                if (zeros[i])
                {
                    xlin[counter] = Math.Log(m_xData[0, i]);
                    ylin[counter] = Math.Log(m_yData[i]);
                    wlin[counter] = Math.Abs(m_weight[i]/m_yData[i]);
                    counter++;
                }
            }

            RegressionAn reglin = new RegressionAn(xlin, ylin, wlin);
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            if (m_scaleFlag)
            {
                reglin.linear();
                double[] coeff = reglin.getBestEstimates();
                double[] errrs = reglin.getBestEstimatesErrors();

                // initial estimates
                start[0] = coeff[1];
                start[1] = Math.Exp(coeff[0]);

                // initial step sizes
                step[0] = errrs[1]/2.0;
                step[1] = errrs[0]*start[0]/2.0;
                if (step[0] <= 0.0 || double.IsNaN(step[0]))
                {
                    step[0] = Math.Abs(start[0]*0.1);
                }
                if (step[1] <= 0.0 || double.IsNaN(step[1]))
                {
                    step[1] = Math.Abs(start[1]*0.1);
                }
            }
            else
            {
                reglin.linearGeneral();
                double[] coeff = reglin.getBestEstimates();
                double[] errrs = reglin.getBestEstimatesErrors();

                // initial estimates
                start[0] = coeff[1];

                // initial step sizes
                step[0] = errrs[1]/2.0;
                if (step[0] <= 0.0 || double.IsNaN(step[0]))
                {
                    step[0] = Math.Abs(start[0]*0.1);
                }
            }

            // Nelder and Mead Simplex Regression
            ExponentialSimpleFunction f = new ExponentialSimpleFunction();
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }


        //  FIT TO MULTIPLE EXPOPNENTIALS

        // method for fitting data to mutiple exponentials
        // initial estimates calculated internally
        public void exponentialMultiple(int nExps)
        {
            m_userSupplied = false;
            fitsexponentialMultiple(nExps, 0);
        }

        // method for fitting data to a multiple exponentials
        // initial estimates calculated internally
        public void exponentialMultiplePlot(int nExps)
        {
            m_userSupplied = false;
            fitsexponentialMultiple(nExps, 1);
        }

        // method for fitting data to mutiple exponentials
        // user supplied initial estimates
        public void exponentialMultiple(int nExps, double[] AandBs)
        {
            m_userSupplied = true;
            fitsexponentialMultiple(nExps, 0, AandBs);
        }

        // method for fitting data to a multiple exponentials
        // user supplied initial estimates
        public void exponentialMultiplePlot(int nExps, double[] AandBs)
        {
            m_userSupplied = true;
            fitsexponentialMultiple(nExps, 1, AandBs);
        }

        // method for fitting data to a multiple exponentials
        // initial estimates calculated internally
        protected void fitsexponentialMultiple(int nExps, int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 44;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2*nExps;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // Estimate  of yscale and A - linear transform
            int nLen = m_yData.Length;
            int nLin = nLen;
            bool[] zeros = new bool[nLen];
            for (int i = 0; i < nLen; i++)
            {
                zeros[i] = true;
                if (m_xData[0, i] <= 0.0D || m_yData[i] <= 0.0D)
                {
                    zeros[i] = false;
                    nLin--;
                }
            }
            double[] xlin = new double[nLin];
            double[] ylin = new double[nLin];
            double[] wlin = new double[nLin];
            int counter = 0;
            for (int i = 0; i < nLen; i++)
            {
                if (zeros[i])
                {
                    xlin[counter] = Math.Log(m_xData[0, i]);
                    ylin[counter] = Math.Log(m_yData[i]);
                    wlin[counter] = Math.Abs(m_weight[i]/m_yData[i]);
                    counter++;
                }
            }

            RegressionAn reglin = new RegressionAn(xlin, ylin, wlin);
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];

            reglin.linear();
            double[] coeff = reglin.getBestEstimates();
            double[] errrs = reglin.getBestEstimatesErrors();

            for (int i = 0; i < m_nTerms; i += 2)
            {
                // initial estimates
                start[i] = Math.Exp(coeff[0])/m_nTerms;
                start[i + 1] = coeff[1];

                // initial step sizes
                step[i] = errrs[0]*start[i]/2.0;
                step[i + 1] = errrs[1]/2.0;
                if (step[i] <= 0.0 || double.IsNaN(step[i]))
                {
                    step[i] = Math.Abs(start[i]*0.1);
                }
                if (step[i + 1] <= 0.0 || double.IsNaN(step[i + 1]))
                {
                    step[i + 1] = Math.Abs(start[i + 1]*0.1);
                }
            }

            // Nelder and Mead Simplex Regression
            ExponentialMultipleFunction f = new ExponentialMultipleFunction();
            f.nExps = m_nTerms;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // method for fitting data to a multiple exponentials
        // user supplied initial estimates calculated
        protected void fitsexponentialMultiple(int nExps, int plotFlag, double[] aAndBs)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 44;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2*nExps;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];

            for (int i = 0; i < m_nTerms; i += 2)
            {
                // initial estimates
                start[i] = aAndBs[i];

                // initial step sizes
                step[i] = Math.Abs(start[i]*0.1);
            }

            // Nelder and Mead Simplex Regression
            ExponentialMultipleFunction f = new ExponentialMultipleFunction();
            f.nExps = m_nTerms;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        //  FIT TO One MINUS A SIMPLE EXPOPNENTIAL

        // method for fitting data to 1 - exponential
        public void oneMinusExponential()
        {
            fitsoneMinusExponential(0);
        }

        // method for fitting data to 1 - exponential
        public void oneMinusExponentialPlot()
        {
            fitsoneMinusExponential(1);
        }

        // method for fitting data to 1 - exponential
        protected void fitsoneMinusExponential(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 45;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // initial step sizes
            ArrayMaths am = new ArrayMaths(m_yData);
            double maxY = am.maximum();
            double minY = am.minimum();
            double testDirection = 1.0;
            double maxYhalf = maxY/2.0;

            if (Math.Abs(minY) > Math.Abs(maxY))
            {
                testDirection = -1.0;
                maxY = minY;
                maxYhalf = minY/2.0;
            }
            double timeHalf = double.NaN;
            bool test = true;
            int ii = 0;
            while (test)
            {
                if (m_yData[ii] == maxYhalf)
                {
                    timeHalf = m_xData[0, ii] - m_xData[0, 0];
                    test = false;
                }
                else
                {
                    if (m_yData[ii] < maxYhalf && m_yData[ii + 1] > maxYhalf)
                    {
                        timeHalf = (m_xData[0, ii] + m_xData[0, ii + 1])/2.0 - m_xData[0, 0];
                        test = false;
                    }
                    else
                    {
                        if (m_yData[ii] > maxYhalf && m_yData[ii + 1] < maxYhalf)
                        {
                            timeHalf = (m_xData[0, ii] + m_xData[0, ii + 1])/2.0 - m_xData[0, 0];
                            test = false;
                        }
                        else
                        {
                            ii++;
                            if (ii >= m_nData - 1)
                            {
                                test = false;
                            }
                        }
                    }
                }
            }

            if (double.IsNaN(timeHalf))
            {
                timeHalf = am.maximumDifference();
            }

            double guessB = -testDirection/timeHalf;
            double[] start = {maxY, guessB};
            double[] step = {Math.Abs(start[0]/5.0), Math.Abs(start[1]/5.0)};

            // Nelder and Mead Simplex Regression
            OneMinusExponentialFunction f = new OneMinusExponentialFunction();
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);
            double ss0 = m_sumOfSquares;
            double[] bestEstimates0 = m_best;

            // Repeat with A and B guess of opposite sign
            start[0] = -maxY;
            start[1] = -guessB;
            step[0] = Math.Abs(start[0]/5.0);
            step[1] = Math.Abs(start[1]/5.0);
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            // Choose better result
            if (m_sumOfSquares > ss0)
            {
                start[0] = bestEstimates0[0];
                start[1] = bestEstimates0[1];
                step[0] = Math.Abs(start[0]/20.0);
                step[1] = Math.Abs(start[1]/20.0);
                nelderMead(regFun2, start, step, m_fTol, m_nMax);
            }

            // Plotting
            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        //  FIT TO AN EXPOPNENTIAL DISTRIBUTION

        public void exponential()
        {
            fitExponential(0, 0);
        }

        public void exponentialPlot()
        {
            fitExponential(1, 0);
        }

        public void exponentialOnePar()
        {
            fitExponential(0, 1);
        }

        public void exponentialOneParPlot()
        {
            fitExponential(1, 1);
        }

        public void exponentialStandard()
        {
            fitExponential(0, 2);
        }

        public void exponentialStandardPlot()
        {
            fitExponential(1, 2);
        }

        protected void fitExponential(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 0:
                    m_lastMethod = 19;
                    m_nTerms = 3;
                    break;
                case 1:
                    m_lastMethod = 20;
                    m_nTerms = 2;
                    break;
                case 2:
                    m_lastMethod = 21;
                    m_nTerms = 1;
                    break;
            }
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }
            if (m_nTerms == 0)
            {
                noParameters("Exponential");
            }
            else
            {
                // Save x-y-w data
                double[] xx = new double[m_nData];
                double[] yy = new double[m_nData];
                double[] ww = new double[m_nData];

                for (int i = 0; i < m_nData; i++)
                {
                    xx[i] = m_xData[0, i];
                    yy[i] = m_yData[i];
                    ww[i] = m_weight[i];
                }

                // order data into ascending order of the abscissae
                sort(
                    ArrayHelper.GetRowCopy(
                        m_xData,
                        0),
                    m_yData,
                    m_weight);

                // check y data
                double tempd = 0;
                List<Object> retY = dataSign(m_yData);
                tempd = (double) retY[4];
                double yPeak = tempd;
                int tempi = 0;
                tempi = (int) retY[5];
                int peaki = tempi;

                // check sign of y data
                string ss = "Exponential";
                bool ySignFlag = false;
                if (yPeak < 0.0D)
                {
                    reverseYsign(ss);
                    retY = dataSign(m_yData);
                    yPeak = -yPeak;
                    ySignFlag = true;
                }

                // check y values for all very small values
                bool magCheck = false;
                double magScale = checkYallSmall(yPeak, ss);
                if (magScale != 1.0D)
                {
                    magCheck = true;
                    yPeak = 1.0D;
                }

                // minimum value of x
                List<Object> retX = dataSign(
                    ArrayHelper.GetRowCopy(
                        m_xData, 0));
                tempd = (double) retX[0];
                double xMin = tempd;

                // estimate of m_sigma
                double yE = yPeak/Math.Exp(1.0D);
                if (m_yData[0] < yPeak)
                {
                    yE = (yPeak + m_yData[0])/(2.0D*Math.Exp(1.0D));
                }
                double yDiff = Math.Abs(m_yData[0] - yE);
                double yTest = 0.0D;
                int iE = 0;
                for (int i = 1; i < m_nData; i++)
                {
                    yTest = Math.Abs(m_yData[i] - yE);
                    if (yTest < yDiff)
                    {
                        yDiff = yTest;
                        iE = i;
                    }
                }
                double sigma = m_xData[0, iE] - m_xData[0, 0];

                // Nelder and Mead Simplex Regression
                double[] start = new double[m_nTerms];
                double[] step = new double[m_nTerms];

                // Fill arrays needed by the Simplex
                switch (typeFlag)
                {
                    case 0:
                        start[0] = xMin*0.9; //m_mu
                        start[1] = sigma; //m_sigma
                        if (m_scaleFlag)
                        {
                            start[2] = yPeak*sigma; //y axis scaling factor
                        }
                        step[0] = 0.1D*start[0];
                        if (step[0] == 0.0D)
                        {
                            List<Object> ret0 = dataSign(
                                ArrayHelper.GetRowCopy(
                                    m_xData, 0));
                            double tempdd = 0;
                            tempdd = (double) ret0[2];
                            double xmax = tempdd;
                            if (xmax == 0.0D)
                            {
                                tempdd = (double) ret0[0];
                                xmax = tempdd;
                            }
                            step[0] = xmax*0.1D;
                        }
                        step[1] = 0.1D*start[1];
                        if (m_scaleFlag)
                        {
                            step[2] = 0.1D*start[2];
                        }
                        break;
                    case 1:
                        start[0] = sigma; //m_sigma
                        if (m_scaleFlag)
                        {
                            start[1] = yPeak*sigma; //y axis scaling factor
                        }
                        step[0] = 0.1D*start[0];
                        if (m_scaleFlag)
                        {
                            step[1] = 0.1D*start[1];
                        }
                        break;
                    case 2:
                        if (m_scaleFlag)
                        {
                            start[0] = yPeak; //y axis scaling factor
                            step[0] = 0.1D*start[0];
                        }
                        break;
                }

                // Create instance of Exponential function and perform regression
                ExponentialFunction ff = new ExponentialFunction();
                ff.typeFlag = typeFlag;
                ff.scaleOption = m_scaleFlag;
                ff.scaleFactor = m_yScaleFactor;
                Object regFun3 = ff;
                nelderMead(regFun3, start, step, m_fTol, m_nMax);

                if (allTest == 1)
                {
                    // Print results
                    if (!m_blnSupressPrint)
                    {
                        print();
                    }
                    // Plot results
                    int flag = plotXY(ff);
                    if (flag != -2 && !m_blnSupressYYplot)
                    {
                        plotYY();
                    }
                }

                // restore data
                if (magCheck)
                {
                    for (int i = 0; i < m_nData; i++)
                    {
                        m_yData[i] = yy[i]/magScale;
                        if (m_weightOpt)
                        {
                            m_weight[i] = ww[i]/magScale;
                        }
                    }
                }
                if (ySignFlag)
                {
                    for (int i = 0; i < m_nData; i++)
                    {
                        m_yData[i] = -m_yData[i];
                    }
                }
            }
        }

        // check for zero and negative  values
        public void checkZeroNeg(double[] xx, double[] yy, double[] ww)
        {
            int jj = 0;
            bool test = true;
            for (int i = 0; i < m_nData; i++)
            {
                if (yy[i] <= 0.0D)
                {
                    if (i <= jj)
                    {
                        test = true;
                        jj = i;
                        while (test)
                        {
                            jj++;
                            if (jj >= m_nData)
                            {
                                throw new ArithmeticException("all zero cumulative data!!");
                            }
                            if (yy[jj] > 0.0D)
                            {
                                yy[i] = yy[jj];
                                xx[i] = xx[jj];
                                ww[i] = ww[jj];
                                test = false;
                            }
                        }
                    }
                    else
                    {
                        if (i == m_nData - 1)
                        {
                            yy[i] = yy[i - 1];
                            xx[i] = xx[i - 1];
                            ww[i] = ww[i - 1];
                        }
                        else
                        {
                            yy[i] = (yy[i - 1] + yy[i + 1])/2.0D;
                            xx[i] = (xx[i - 1] + xx[i + 1])/2.0D;
                            ww[i] = (ww[i - 1] + ww[i + 1])/2.0D;
                        }
                    }
                }
            }
        }

        public void rayleigh()
        {
            fitRayleigh(0, 0);
        }

        public void rayleighPlot()
        {
            fitRayleigh(1, 0);
        }

        protected void fitRayleigh(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 22;
            m_userSupplied = false;
            m_nTerms = 2;
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }


            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            int tempi = 0;
            tempi = (int) retY[5];
            int peaki = tempi;

            // check sign of y data
            string ss = "Rayleigh";
            bool ySignFlag = false;
            if (yPeak < 0.0D)
            {
                reverseYsign(ss);
                retY = dataSign(m_yData);
                yPeak = -yPeak;
                ySignFlag = true;
            }

            // check y values for all very small values
            bool magCheck = false;
            double magScale = checkYallSmall(yPeak, ss);
            if (magScale != 1.0D)
            {
                magCheck = true;
                yPeak = 1.0D;
            }

            // Save x-y-w data
            double[] xx = new double[m_nData];
            double[] yy = new double[m_nData];
            double[] ww = new double[m_nData];

            for (int i = 0; i < m_nData; i++)
            {
                xx[i] = m_xData[0, i];
                yy[i] = m_yData[i];
                ww[i] = m_weight[i];
            }

            // minimum value of x
            List<Object> retX = dataSign(
                ArrayHelper.GetRowCopy(
                    m_xData, 0));
            tempd = (double) retX[0];
            double xMin = tempd;

            // maximum value of x
            tempd = (double) retX[2];
            double xMax = tempd;

            // Calculate  x value at peak y (estimate of the 'distribution mode')
            double distribMode = m_xData[0, peaki];

            // Calculate an estimate of the half-height width
            double sd = Math.Log(2.0D)*halfWidth(
                                           ArrayHelper.GetRowCopy(m_xData, 0), m_yData);

            // Calculate the cumulative probability and return ordinate scaling factor estimate
            double[] cumX = new double[m_nData];
            double[] cumY = new double[m_nData];
            double[] cumW = new double[m_nData];
            ErrorProp[] cumYe = ErrorProp.oneDarray(m_nData);
            double yScale = calculateCumulativeValues(cumX, cumY, cumW, cumYe, peaki, yPeak, distribMode, ss);

            //Calculate log  transform
            for (int i = 0; i < m_nData; i++)
            {
                cumYe[i] = ErrorProp.minus(1.0D, cumYe[i]);
                cumYe[i] = ErrorProp.over(1.0D, cumYe[i]);
                cumYe[i] = ErrorProp.Log(cumYe[i]);
                cumY[i] = cumYe[i].getValue();
                cumW[i] = cumYe[i].getError();
            }

            // Fill data arrays with transformed data
            for (int i = 0; i < m_nData; i++)
            {
                m_xData[0, i] = cumX[i];
                m_yData[i] = cumY[i];
                m_weight[i] = cumW[i];
            }
            bool weightOptHold = m_weightOpt;
            m_weightOpt = true;

            // Nelder and Mead Simplex Regression for semi-linearised Rayleigh
            // disable statistical analysis
            m_statFlag = false;

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                start[i] = 1.0D;
                step[i] = 0.2D;
            }
            start[0] = sd; //m_sigma
            step[0] = 0.2D;
            addConstraint(0, -1, 0.0D);

            // Create instance of log function and perform regression
            RayleighFunctionTwo f = new RayleighFunctionTwo();
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            // Get best estimates of log regression
            double[] ests = (double[]) m_best.Clone();

            // enable statistical analysis
            m_statFlag = true;

            // restore data reversing the loglog transform but maintaining any sign reversals
            m_weightOpt = weightOptHold;
            for (int i = 0; i < m_nData; i++)
            {
                m_xData[0, i] = xx[i];
                m_yData[i] = yy[i];
                m_weight[i] = ww[i];
            }

            // Fill arrays needed by the Simplex
            start[0] = ests[0]; //m_sigma
            if (m_scaleFlag)
            {
                start[1] = 1.0/yScale; //y axis scaling factor
            }
            step[0] = 0.1D*start[0];
            if (m_scaleFlag)
            {
                step[1] = 0.1D*start[1];
            }


            // Create instance of Rayleigh function and perform regression
            RayleighFunctionOne ff = new RayleighFunctionOne();
            ff.scaleOption = m_scaleFlag;
            ff.scaleFactor = m_yScaleFactor;
            Object regFun3 = ff;
            nelderMead(regFun3, start, step, m_fTol, m_nMax);

            if (allTest == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }
                // Plot results
                int flag = plotXY(ff);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            // restore data
            if (magCheck)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = yy[i]/magScale;
                    if (m_weightOpt)
                    {
                        m_weight[i] = ww[i]/magScale;
                    }
                }
            }
            if (ySignFlag)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        // Shifted Pareto
        public void paretoShifted()
        {
            fitPareto(0, 3);
        }

        public void paretoThreePar()
        {
            fitPareto(0, 3);
        }

        public void paretoShiftedPlot()
        {
            fitPareto(1, 3);
        }

        public void paretoThreeParPlot()
        {
            fitPareto(1, 3);
        }

        // Two Parameter Pareto
        public void paretoTwoPar()
        {
            fitPareto(0, 2);
        }

        // Deprecated
        public void pareto()
        {
            fitPareto(0, 2);
        }

        public void paretoTwoParPlot()
        {
            fitPareto(1, 2);
        }

        // Deprecated
        public void paretoPlot()
        {
            fitPareto(1, 2);
        }

        // One Parameter Pareto
        public void paretoOnePar()
        {
            fitPareto(0, 1);
        }

        public void paretoOneParPlot()
        {
            fitPareto(1, 1);
        }

        // method for fitting data to a Pareto distribution
        protected void fitPareto(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 3:
                    m_lastMethod = 29;
                    m_nTerms = 4;
                    break;
                case 2:
                    m_lastMethod = 23;
                    m_nTerms = 3;
                    break;
                case 1:
                    m_lastMethod = 24;
                    m_nTerms = 2;
                    break;
            }

            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }
            m_linNonLin = false;
            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }
            string ss = "Pareto";

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(
                    m_xData,
                    0),
                m_yData,
                m_weight);

            // check y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            int tempi = 0;
            tempi = (int) retY[5];
            int peaki = tempi;

            // check for infinity
            if (infinityCheck(yPeak, peaki))
            {
                retY = dataSign(m_yData);
                tempd = (double) retY[4];
                yPeak = tempd;
                tempi = 0;
                tempi = (int) retY[5];
                peaki = tempi;
            }

            // check sign of y data
            bool ySignFlag = false;
            if (yPeak < 0.0D)
            {
                reverseYsign(ss);
                retY = dataSign(m_yData);
                yPeak = -yPeak;
                ySignFlag = true;
            }

            // check y values for all very small values
            bool magCheck = false;
            double magScale = checkYallSmall(yPeak, ss);
            if (magScale != 1.0D)
            {
                magCheck = true;
                yPeak = 1.0D;
            }

            // minimum value of x
            List<Object> retX = dataSign(
                ArrayHelper.GetRowCopy(m_xData, 0));
            tempd = (double) retX[0];
            double xMin = tempd;

            // maximum value of x
            tempd = (double) retX[2];
            double xMax = tempd;

            // Calculate  x value at peak y (estimate of the 'distribution mode')
            double distribMode = m_xData[0, peaki];

            // Calculate an estimate of the half-height width
            double sd = Math.Log(2.0D)*halfWidth(
                                           ArrayHelper.GetRowCopy(m_xData, 0), m_yData);

            // Save x-y-w data
            double[] xx = new double[m_nData];
            double[] yy = new double[m_nData];
            double[] ww = new double[m_nData];

            for (int i = 0; i < m_nData; i++)
            {
                xx[i] = m_xData[0, i];
                yy[i] = m_yData[i];
                ww[i] = m_weight[i];
            }

            // Calculate the cumulative probability and return ordinate scaling factor estimate
            double[] cumX = new double[m_nData];
            double[] cumY = new double[m_nData];
            double[] cumW = new double[m_nData];
            ErrorProp[] cumYe = ErrorProp.oneDarray(m_nData);
            double yScale = calculateCumulativeValues(cumX, cumY, cumW, cumYe, peaki, yPeak, distribMode, ss);

            //Calculate l - cumlative probability
            for (int i = 0; i < m_nData; i++)
            {
                cumYe[i] = ErrorProp.minus(1.0D, cumYe[i]);
                cumY[i] = cumYe[i].getValue();
                cumW[i] = cumYe[i].getError();
            }

            // Fill data arrays with transformed data
            for (int i = 0; i < m_nData; i++)
            {
                m_xData[0, i] = cumX[i];
                m_yData[i] = cumY[i];
                m_weight[i] = cumW[i];
            }
            bool weightOptHold = m_weightOpt;
            m_weightOpt = true;

            // Nelder and Mead Simplex Regression for Pareto estimated cdf
            // disable statistical analysis
            m_statFlag = false;

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                start[i] = 1.0D;
                step[i] = 0.2D;
            }
            switch (typeFlag)
            {
                case 3:
                    start[0] = 2; //alpha
                    start[1] = xMin*0.9D; //beta
                    if (xMin < 0)
                    {
                        //theta
                        start[2] = -xMin*1.1D;
                    }
                    else
                    {
                        start[2] = xMin*0.01;
                    }
                    if (start[1] < 0.0D)
                    {
                        start[1] = 0.0D;
                    }
                    step[0] = 0.2D*start[0];
                    step[1] = 0.2D*start[1];
                    if (step[1] == 0.0D)
                    {
                        double xmax = xMax;
                        if (xmax == 0.0D)
                        {
                            xmax = xMin;
                        }
                        step[1] = xmax*0.1D;
                    }
                    addConstraint(0, -1, 0.0D);
                    addConstraint(1, -1, 0.0D);
                    addConstraint(1, +1, xMin);
                    break;
                case 2:
                    if (xMin < 0)
                    {
                        PrintToScreen.WriteLine(
                            "Method: FitParetoTwoPar/FitParetoTwoParPlot\nNegative data values present\nFitParetoShifted/FitParetoShiftedPlot would have been more appropriate");
                    }
                    start[0] = 2; //alpha
                    start[1] = xMin*0.9D; //beta
                    if (start[1] < 0.0D)
                    {
                        start[1] = 0.0D;
                    }
                    step[0] = 0.2D*start[0];
                    step[1] = 0.2D*start[1];
                    if (step[1] == 0.0D)
                    {
                        double xmax = xMax;
                        if (xmax == 0.0D)
                        {
                            xmax = xMin;
                        }
                        step[1] = xmax*0.1D;
                    }
                    addConstraint(0, -1, 0.0D);
                    addConstraint(1, -1, 0.0D);
                    break;
                case 1:
                    if (xMin < 0)
                    {
                        PrintToScreen.WriteLine(
                            "Method: FitParetoOnePar/FitParetoOneParPlot\nNegative data values present\nFitParetoShifted/FitParetoShiftedPlot would have been more appropriate");
                    }
                    start[0] = 2; //alpha
                    step[0] = 0.2D*start[0];
                    addConstraint(0, -1, 0.0D);
                    addConstraint(1, -1, 0.0D);
                    break;
            }

            // Create instance of cdf function and perform regression
            ParetoFunctionTwo f = new ParetoFunctionTwo();
            f.typeFlag = typeFlag;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            // Get best estimates of cdf regression
            double[] ests = (double[]) m_best.Clone();

            // Nelder and Mead Simplex Regression for Pareto
            // using best estimates from cdf regression as initial estimates

            // enable statistical analysis
            m_statFlag = true;

            // restore data reversing the cdf transform but maintaining any sign reversals
            m_weightOpt = weightOptHold;
            for (int i = 0; i < m_nData; i++)
            {
                m_xData[0, i] = xx[i];
                m_yData[i] = yy[i];
                m_weight[i] = ww[i];
            }

            // Fill arrays needed by the Simplex
            switch (typeFlag)
            {
                case 3:
                    start[0] = ests[0]; //alpha
                    if (start[0] <= 0.0D)
                    {
                        if (start[0] == 0.0D)
                        {
                            start[0] = 1.0D;
                        }
                        else
                        {
                            start[0] = Math.Min(1.0D, -start[0]);
                        }
                    }
                    start[1] = ests[1]; //beta
                    if (start[1] <= 0.0D)
                    {
                        if (start[1] == 0.0D)
                        {
                            start[1] = 1.0D;
                        }
                        else
                        {
                            start[1] = Math.Min(1.0D, -start[1]);
                        }
                    }
                    start[2] = ests[2];
                    if (m_scaleFlag)
                    {
                        start[3] = 1.0/yScale; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    step[1] = 0.1D*start[1];
                    if (step[1] == 0.0D)
                    {
                        double xmax = xMax;
                        if (xmax == 0.0D)
                        {
                            xmax = xMin;
                        }
                        step[1] = xmax*0.1D;
                    }
                    if (m_scaleFlag)
                    {
                        step[2] = 0.1D*start[2];
                    }
                    break;
                case 2:
                    start[0] = ests[0]; //alpha
                    if (start[0] <= 0.0D)
                    {
                        if (start[0] == 0.0D)
                        {
                            start[0] = 1.0D;
                        }
                        else
                        {
                            start[0] = Math.Min(1.0D, -start[0]);
                        }
                    }
                    start[1] = ests[1]; //beta
                    if (start[1] <= 0.0D)
                    {
                        if (start[1] == 0.0D)
                        {
                            start[1] = 1.0D;
                        }
                        else
                        {
                            start[1] = Math.Min(1.0D, -start[1]);
                        }
                    }
                    if (m_scaleFlag)
                    {
                        start[2] = 1.0/yScale; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    step[1] = 0.1D*start[1];
                    if (step[1] == 0.0D)
                    {
                        double xmax = xMax;
                        if (xmax == 0.0D)
                        {
                            xmax = xMin;
                        }
                        step[1] = xmax*0.1D;
                    }
                    if (m_scaleFlag)
                    {
                        step[2] = 0.1D*start[2];
                    }
                    break;
                case 1:
                    start[0] = ests[0]; //alpha
                    if (start[0] <= 0.0D)
                    {
                        if (start[0] == 0.0D)
                        {
                            start[0] = 1.0D;
                        }
                        else
                        {
                            start[0] = Math.Min(1.0D, -start[0]);
                        }
                    }
                    if (m_scaleFlag)
                    {
                        start[1] = 1.0/yScale; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    if (m_scaleFlag)
                    {
                        step[1] = 0.1D*start[1];
                    }
                    break;
            }

            // Create instance of Pareto function and perform regression
            ParetoFunctionOne ff = new ParetoFunctionOne();
            ff.typeFlag = typeFlag;
            ff.scaleOption = m_scaleFlag;
            ff.scaleFactor = m_yScaleFactor;
            Object regFun3 = ff;
            nelderMead(regFun3, start, step, m_fTol, m_nMax);

            if (allTest == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }
                // Plot results
                int flag = plotXY(ff);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            // restore data
            m_weightOpt = weightOptHold;
            if (magCheck)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = yy[i]/magScale;
                    if (m_weightOpt)
                    {
                        m_weight[i] = ww[i]/magScale;
                    }
                }
            }
            if (ySignFlag)
            {
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }


        // method for fitting data to a sigmoid threshold function
        public void sigmoidThreshold()
        {
            fitSigmoidThreshold(0);
        }

        // method for fitting data to a sigmoid threshold function with plot and print out
        public void sigmoidThresholdPlot()
        {
            fitSigmoidThreshold(1);
        }


        // method for fitting data to a sigmoid threshold function
        protected void fitSigmoidThreshold(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 25;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // Estimate  of theta
            double yymin = Fmath.minimum(m_yData);
            double yymax = Fmath.maximum(m_yData);
            int dirFlag = 1;
            if (yymin < 0)
            {
                dirFlag = -1;
            }
            double yyymid = (yymax - yymin)/2.0D;
            double yyxmidl = m_xData[0, 0];
            int ii = 1;
            int nLen = m_yData.Length;
            bool test = true;
            while (test)
            {
                if (m_yData[ii] >= dirFlag*yyymid)
                {
                    yyxmidl = m_xData[0, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nLen)
                    {
                        yyxmidl = Stat.mean(
                            ArrayHelper.GetRowCopy(
                                m_xData, 0));
                        ii = nLen - 1;
                        test = false;
                    }
                }
            }
            double yyxmidh = m_xData[0, nLen - 1];
            int jj = nLen - 1;
            test = true;
            while (test)
            {
                if (m_yData[jj] <= dirFlag*yyymid)
                {
                    yyxmidh = m_xData[0, jj];
                    test = false;
                }
                else
                {
                    jj--;
                    if (jj < 0)
                    {
                        yyxmidh = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        jj = 1;
                        test = false;
                    }
                }
            }
            int thetaPos = (ii + jj)/2;
            double theta0 = m_xData[0, thetaPos];

            // estimate of slope
            double thetaSlope1 = 2.0D*(m_yData[nLen - 1] - theta0)/(m_xData[0, nLen - 1] - m_xData[0, thetaPos]);
            double thetaSlope2 = 2.0D*theta0/(m_xData[0, thetaPos] - m_xData[0, nLen - 1]);
            double thetaSlope = Math.Max(thetaSlope1, thetaSlope2);

            // initial estimates
            double[] start = new double[m_nTerms];
            start[0] = 4.0D*thetaSlope;
            if (dirFlag == 1)
            {
                start[0] /= yymax;
            }
            else
            {
                start[0] /= yymin;
            }
            start[1] = theta0;
            if (m_scaleFlag)
            {
                if (dirFlag == 1)
                {
                    start[2] = yymax;
                }
                else
                {
                    start[2] = yymin;
                }
            }

            // initial step sizes
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                step[i] = 0.1*start[i];
            }
            if (step[0] == 0.0D)
            {
                step[0] = 0.1*(m_xData[0, nLen - 1] - m_xData[0, 0])/(m_yData[nLen - 1] - m_yData[0]);
            }
            if (step[1] == 0.0D)
            {
                step[1] = (m_xData[0, nLen - 1] - m_xData[0, 0])/20.0D;
            }
            if (m_scaleFlag)
            {
                if (step[2] == 0.0D)
                {
                    step[2] = 0.1*(m_yData[nLen - 1] - m_yData[0]);
                }
            }

            // Nelder and Mead Simplex Regression
            SigmoidThresholdFunction f = new SigmoidThresholdFunction();
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // method for fitting data to a Hill/Sips Sigmoid
        public void sigmoidHillSips()
        {
            fitsigmoidHillSips(0);
        }

        // method for fitting data to a Hill/Sips Sigmoid with plot and print out
        public void sigmoidHillSipsPlot()
        {
            fitsigmoidHillSips(1);
        }

        // method for fitting data to a Hill/Sips Sigmoid
        protected void fitsigmoidHillSips(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 28;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // Estimate  of theta
            double yymin = Fmath.minimum(m_yData);
            double yymax = Fmath.maximum(m_yData);
            int dirFlag = 1;
            if (yymin < 0)
            {
                dirFlag = -1;
            }
            double yyymid = (yymax - yymin)/2.0D;
            double yyxmidl = m_xData[0, 0];
            int ii = 1;
            int nLen = m_yData.Length;
            bool test = true;
            while (test)
            {
                if (m_yData[ii] >= dirFlag*yyymid)
                {
                    yyxmidl = m_xData[0, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nLen)
                    {
                        yyxmidl = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        ii = nLen - 1;
                        test = false;
                    }
                }
            }
            double yyxmidh = m_xData[0, nLen - 1];
            int jj = nLen - 1;
            test = true;
            while (test)
            {
                if (m_yData[jj] <= dirFlag*yyymid)
                {
                    yyxmidh = m_xData[0, jj];
                    test = false;
                }
                else
                {
                    jj--;
                    if (jj < 0)
                    {
                        yyxmidh = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        jj = 1;
                        test = false;
                    }
                }
            }
            int thetaPos = (ii + jj)/2;
            double theta0 = m_xData[0, thetaPos];

            // initial estimates
            double[] start = new double[m_nTerms];
            start[0] = theta0;
            start[1] = 1;
            if (m_scaleFlag)
            {
                if (dirFlag == 1)
                {
                    start[2] = yymax;
                }
                else
                {
                    start[2] = yymin;
                }
            }

            // initial step sizes
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                step[i] = 0.1*start[i];
            }
            if (step[0] == 0.0D)
            {
                step[0] = (m_xData[0, nLen - 1] - m_xData[0, 0])/20.0D;
            }
            if (m_scaleFlag)
            {
                if (step[2] == 0.0D)
                {
                    step[2] = 0.1*(m_yData[nLen - 1] - m_yData[0]);
                }
            }

            // Nelder and Mead Simplex Regression
            SigmoidHillSipsFunction f = new SigmoidHillSipsFunction();
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // method for fitting data to a EC50 dose response curve
        public void ec50()
        {
            fitEC50(0);
        }

        // method for fitting data to a EC50 dose response curve with plot and print out
        public void ec50Plot()
        {
            fitEC50(1);
        }

        // method for fitting data to a EC50 dose response curve
        // bottom constrained to zero or positive values
        public void ec50constrained()
        {
            fitEC50(2);
        }

        // method for fitting data to a EC50 dose response curve with plot and print out
        // bottom constrained to zero or positive values
        public void ec50constrainedPlot()
        {
            fitEC50(3);
        }

        // method for fitting data to a logEC50 dose response curve
        protected void fitEC50(int cpFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            int plotFlag = 0;
            bool constrained = false;
            m_userSupplied = false;
            switch (cpFlag)
            {
                case 0:
                    m_lastMethod = 39;
                    plotFlag = 0;
                    break;
                case 1:
                    m_lastMethod = 39;
                    plotFlag = 1;
                    break;
                case 2:
                    m_lastMethod = 41;
                    plotFlag = 0;
                    constrained = true;
                    break;
                case 3:
                    m_lastMethod = 41;
                    plotFlag = 1;
                    constrained = true;
                    break;
            }

            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 4;
            m_scaleFlag = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // Estimate of bottom and top
            double bottom = Fmath.minimum(m_yData);
            double top = Fmath.maximum(m_yData);

            // Estimate of EC50
            int dirFlag = 1;
            double yyymid = (top - bottom)/2.0D;
            double yyxmidl = m_xData[0, 0];
            int ii = 1;
            int nLen = m_yData.Length;
            bool test = true;
            while (test)
            {
                if (m_yData[ii] >= dirFlag*yyymid)
                {
                    yyxmidl = m_xData[0, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nLen)
                    {
                        yyxmidl = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        ii = nLen - 1;
                        test = false;
                    }
                }
            }
            double yyxmidh = m_xData[0, nLen - 1];
            int jj = nLen - 1;
            test = true;
            while (test)
            {
                if (m_yData[jj] <= dirFlag*yyymid)
                {
                    yyxmidh = m_xData[0, jj];
                    test = false;
                }
                else
                {
                    jj--;
                    if (jj < 0)
                    {
                        yyxmidh = Stat.mean(
                            ArrayHelper.GetRowCopy(
                                m_xData, 0));
                        jj = 1;
                        test = false;
                    }
                }
            }
            int thetaPos = (ii + jj)/2;
            double EC50 = m_xData[0, thetaPos];

            // estimate of slope
            double thetaSlope1 = 2.0D*(m_yData[nLen - 1] - EC50)/(m_xData[0, nLen - 1] - m_xData[0, thetaPos]);
            double thetaSlope2 = 2.0D*EC50/(m_xData[0, thetaPos] - m_xData[0, nLen - 1]);
            double hillSlope = Math.Max(thetaSlope1, thetaSlope2);

            // initial estimates
            double[] start = new double[m_nTerms];
            start[0] = bottom;
            start[1] = top;
            start[2] = EC50;
            start[3] = -hillSlope;

            // initial step sizes
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                step[i] = 0.1*start[i];
            }
            if (step[0] == 0.0D)
            {
                step[0] = 0.1*(m_yData[nLen - 1] - m_yData[0]);
            }
            if (step[1] == 0.0D)
            {
                step[1] = 0.1*(m_yData[nLen - 1] - m_yData[0]) + m_yData[nLen - 1];
            }
            if (step[2] == 0.0D)
            {
                step[2] = 0.05*(m_xData[0, nLen - 1] - m_xData[0, 0]);
            }
            if (step[3] == 0.0D)
            {
                step[3] = 0.1*(m_xData[0, nLen - 1] - m_xData[0, 0])/(m_yData[nLen - 1] - m_yData[0]);
            }

            // Constrained option
            if (constrained)
            {
                addConstraint(0, -1, 0.0D);
            }

            // Nelder and Mead Simplex Regression
            EC50Function f = new EC50Function();
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // method for fitting data to a logEC50 dose response curve
        public void logEC50()
        {
            fitLogEC50(0);
        }

        // method for fitting data to a logEC50 dose response curve with plot and print out
        public void logEC50Plot()
        {
            fitLogEC50(1);
        }

        // method for fitting data to a logEC50 dose response curve
        // bottom constrained to zero or positive values
        public void logEC50constrained()
        {
            fitLogEC50(2);
        }

        // method for fitting data to a logEC50 dose response curve with plot and print out
        // bottom constrained to zero or positive values
        public void logEC50constrainedPlot()
        {
            fitLogEC50(3);
        }

        // method for fitting data to a logEC50 dose response curve
        protected void fitLogEC50(int cpFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            int plotFlag = 0;
            bool constrained = false;
            m_userSupplied = false;
            switch (cpFlag)
            {
                case 0:
                    m_lastMethod = 40;
                    plotFlag = 0;
                    break;
                case 1:
                    m_lastMethod = 40;
                    plotFlag = 1;
                    break;
                case 2:
                    m_lastMethod = 42;
                    plotFlag = 0;
                    constrained = true;
                    break;
                case 3:
                    m_lastMethod = 42;
                    plotFlag = 1;
                    constrained = true;
                    break;
            }
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 4;
            m_scaleFlag = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // Estimate of bottom and top
            double bottom = Fmath.minimum(m_yData);
            double top = Fmath.maximum(m_yData);

            // Estimate of LogEC50
            int dirFlag = 1;
            double yyymid = (top - bottom)/2.0D;
            double yyxmidl = m_xData[0, 0];
            int ii = 1;
            int nLen = m_yData.Length;
            bool test = true;
            while (test)
            {
                if (m_yData[ii] >= dirFlag*yyymid)
                {
                    yyxmidl = m_xData[0, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nLen)
                    {
                        yyxmidl = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        ii = nLen - 1;
                        test = false;
                    }
                }
            }
            double yyxmidh = m_xData[0, nLen - 1];
            int jj = nLen - 1;
            test = true;
            while (test)
            {
                if (m_yData[jj] <= dirFlag*yyymid)
                {
                    yyxmidh = m_xData[0, jj];
                    test = false;
                }
                else
                {
                    jj--;
                    if (jj < 0)
                    {
                        yyxmidh = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        jj = 1;
                        test = false;
                    }
                }
            }
            int thetaPos = (ii + jj)/2;
            double logEC50 = m_xData[0, thetaPos];

            // estimate of slope
            double thetaSlope1 = 2.0D*(m_yData[nLen - 1] - logEC50)/(m_xData[0, nLen - 1] - m_xData[0, thetaPos]);
            double thetaSlope2 = 2.0D*logEC50/(m_xData[0, thetaPos] - m_xData[0, nLen - 1]);
            double hillSlope = Math.Max(thetaSlope1, thetaSlope2);

            // initial estimates
            double[] start = new double[m_nTerms];
            start[0] = bottom;
            start[1] = top;
            start[2] = logEC50;
            start[3] = hillSlope;

            // initial step sizes
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                step[i] = 0.1*start[i];
            }
            if (step[0] == 0.0D)
            {
                step[0] = 0.1*(m_yData[nLen - 1] - m_yData[0]);
            }
            if (step[1] == 0.0D)
            {
                step[1] = 0.1*(m_yData[nLen - 1] - m_yData[0]) + m_yData[nLen - 1];
            }
            if (step[2] == 0.0D)
            {
                step[2] = 0.05*(m_xData[0, nLen - 1] - m_xData[0, 0]);
            }
            if (step[3] == 0.0D)
            {
                step[3] = 0.1*(m_xData[0, nLen - 1] - m_xData[0, 0])/(m_yData[nLen - 1] - m_yData[0]);
            }

            // Constrained option
            if (constrained)
            {
                addConstraint(0, -1, 0.0D);
            }

            // Nelder and Mead Simplex Regression
            LogEC50Function f = new LogEC50Function();
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // method for fitting data to a rectangular hyberbola
        public void rectangularHyperbola()
        {
            fitRectangularHyperbola(0);
        }

        // method for fitting data to a rectangular hyberbola with plot and print out
        public void rectangularHyperbolaPlot()
        {
            fitRectangularHyperbola(1);
        }

        // method for fitting data to a rectangular hyperbola
        protected void fitRectangularHyperbola(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 26;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2;
            if (!m_scaleFlag)
            {
                m_nTerms = 1;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // Estimate  of theta
            double yymin = Fmath.minimum(m_yData);
            double yymax = Fmath.maximum(m_yData);
            int dirFlag = 1;
            if (yymin < 0)
            {
                dirFlag = -1;
            }
            double yyymid = (yymax - yymin)/2.0D;
            double yyxmidl = m_xData[0, 0];
            int ii = 1;
            int nLen = m_yData.Length;
            bool test = true;
            while (test)
            {
                if (m_yData[ii] >= dirFlag*yyymid)
                {
                    yyxmidl = m_xData[0, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nLen)
                    {
                        yyxmidl = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        ii = nLen - 1;
                        test = false;
                    }
                }
            }
            double yyxmidh = m_xData[0, nLen - 1];
            int jj = nLen - 1;
            test = true;
            while (test)
            {
                if (m_yData[jj] <= dirFlag*yyymid)
                {
                    yyxmidh = m_xData[0, jj];
                    test = false;
                }
                else
                {
                    jj--;
                    if (jj < 0)
                    {
                        yyxmidh = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        jj = 1;
                        test = false;
                    }
                }
            }
            int thetaPos = (ii + jj)/2;
            double theta0 = m_xData[0, thetaPos];

            // initial estimates
            double[] start = new double[m_nTerms];
            start[0] = theta0;
            if (m_scaleFlag)
            {
                if (dirFlag == 1)
                {
                    start[1] = yymax;
                }
                else
                {
                    start[1] = yymin;
                }
            }

            // initial step sizes
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                step[i] = 0.1*start[i];
            }
            if (step[0] == 0.0D)
            {
                step[0] = (m_xData[0, nLen - 1] - m_xData[0, 0])/20.0D;
            }
            if (m_scaleFlag)
            {
                if (step[1] == 0.0D)
                {
                    step[1] = 0.1*(m_yData[nLen - 1] - m_yData[0]);
                }
            }

            // Nelder and Mead Simplex Regression
            RectangularHyperbolaFunction f = new RectangularHyperbolaFunction();
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // method for fitting data to a scaled Heaviside Step Function
        public void stepFunction()
        {
            fitStepFunction(0);
        }

        // method for fitting data to a scaled Heaviside Step Function with plot and print out
        public void stepFunctionPlot()
        {
            fitStepFunction(1);
        }

        // method for fitting data to a scaled Heaviside Step Function
        protected void fitStepFunction(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 27;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 2;
            if (!m_scaleFlag)
            {
                m_nTerms = 1;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // Estimate  of theta
            double yymin = Fmath.minimum(m_yData);
            double yymax = Fmath.maximum(m_yData);
            int dirFlag = 1;
            if (yymin < 0)
            {
                dirFlag = -1;
            }
            double yyymid = (yymax - yymin)/2.0D;
            double yyxmidl = m_xData[0, 0];
            int ii = 1;
            int nLen = m_yData.Length;
            bool test = true;
            while (test)
            {
                if (m_yData[ii] >= dirFlag*yyymid)
                {
                    yyxmidl = m_xData[0, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nLen)
                    {
                        yyxmidl = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        ii = nLen - 1;
                        test = false;
                    }
                }
            }
            double yyxmidh = m_xData[0, nLen - 1];
            int jj = nLen - 1;
            test = true;
            while (test)
            {
                if (m_yData[jj] <= dirFlag*yyymid)
                {
                    yyxmidh = m_xData[0, jj];
                    test = false;
                }
                else
                {
                    jj--;
                    if (jj < 0)
                    {
                        yyxmidh = Stat.mean(ArrayHelper.GetRowCopy(
                                                m_xData, 0));
                        jj = 1;
                        test = false;
                    }
                }
            }
            int thetaPos = (ii + jj)/2;
            double theta0 = m_xData[0, thetaPos];

            // initial estimates
            double[] start = new double[m_nTerms];
            start[0] = theta0;
            if (m_scaleFlag)
            {
                if (dirFlag == 1)
                {
                    start[1] = yymax;
                }
                else
                {
                    start[1] = yymin;
                }
            }

            // initial step sizes
            double[] step = new double[m_nTerms];
            for (int i = 0; i < m_nTerms; i++)
            {
                step[i] = 0.1*start[i];
            }
            if (step[0] == 0.0D)
            {
                step[0] = (m_xData[0, nLen - 1] - m_xData[0, 0])/20.0D;
            }
            if (m_scaleFlag)
            {
                if (step[1] == 0.0D)
                {
                    step[1] = 0.1*(m_yData[nLen - 1] - m_yData[0]);
                }
            }

            // Nelder and Mead Simplex Regression
            StepFunctionFunction f = new StepFunctionFunction();
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }
        }

        // Fit to a Logistic
        public void logistic()
        {
            fitLogistic(0);
        }

        // Fit to a Logistic
        public void logisticPlot()
        {
            fitLogistic(1);
        }

        // Fit data to a Logistic probability function
        protected void fitLogistic(int plotFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 30;
            m_userSupplied = false;
            m_linNonLin = false;
            m_zeroCheck = false;
            m_nTerms = 3;
            if (!m_scaleFlag)
            {
                m_nTerms = 2;
            }
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(m_xData, 0), m_yData, m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitLogistic(): This implementation of the Logistic distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // Calculate  x value at peak y (estimate of the Logistic m_mean)
            List<Object> ret1 = dataSign(m_yData);
            int tempi = 0;
            tempi = (int) ret1[5];
            int peaki = tempi;
            double mu = m_xData[0, peaki];

            // Calculate an estimate of the beta
            double beta = Math.Sqrt(6.0D)*halfWidth(ArrayHelper.GetRowCopy(m_xData, 0), m_yData)/Math.PI;

            // Calculate estimate of y scale
            tempd = (double) ret1[4];
            double ym = tempd;
            ym = ym*beta*Math.Sqrt(2.0D*Math.PI);

            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = mu;
            start[1] = beta;
            if (m_scaleFlag)
            {
                start[2] = ym;
            }
            step[0] = 0.1D*beta;
            step[1] = 0.1D*start[1];
            if (step[1] == 0.0D)
            {
                List<Object> ret0 = dataSign(ArrayHelper.GetRowCopy(
                                                 m_xData, 0));
                double tempdd = 0;
                tempdd = (double) ret0[2];
                double xmax = tempdd;
                if (xmax == 0.0D)
                {
                    tempdd = (double) ret0[0];
                    xmax = tempdd;
                }
                step[0] = xmax*0.1D;
            }
            if (m_scaleFlag)
            {
                step[2] = 0.1D*start[2];
            }

            // Nelder and Mead Simplex Regression
            LogisticFunction f = new LogisticFunction();
            addConstraint(1, -1, 0.0D);
            f.scaleOption = m_scaleFlag;
            f.scaleFactor = m_yScaleFactor;
            Object regFun2 = f;
            nelderMead(regFun2, start, step, m_fTol, m_nMax);

            if (plotFlag == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(f);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        public void beta()
        {
            fitBeta(0, 0);
        }

        public void betaPlot()
        {
            fitBeta(1, 0);
        }

        public void betaMinMax()
        {
            fitBeta(0, 1);
        }

        public void betaMinMaxPlot()
        {
            fitBeta(1, 1);
        }

        protected void fitBeta(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 0:
                    m_lastMethod = 31;
                    m_nTerms = 3;
                    break;
                case 1:
                    m_lastMethod = 32;
                    m_nTerms = 5;
                    break;
            }
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }

            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(
                     m_xData, 0), m_yData, m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitBeta(): This implementation of the Beta distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // check  x data
            List<Object> retX = dataSign(ArrayHelper.GetRowCopy(m_xData, 0));
            int tempi = 0;

            // Calculate  x value at peak y (estimate of the 'distribution mode')
            tempi = (int) retY[5];
            int peaki = tempi;
            double distribMode = m_xData[0, peaki];

            // minimum value
            tempd = (double) retX[0];
            double minX = tempd;
            // maximum value
            tempd = (double) retX[2];
            double maxX = tempd;
            // m_mean value
            tempd = (double) retX[8];
            double meanX = tempd;


            // test that data is within range
            if (typeFlag == 0)
            {
                if (minX < 0.0D)
                {
                    PrintToScreen.WriteLine("Regression: beta: data points must be greater than or equal to 0");
                    PrintToScreen.WriteLine("method betaMinMax used in place of method beta");
                    typeFlag = 1;
                    m_lastMethod = 32;
                    m_nTerms = 5;
                }
                if (maxX > 1.0D)
                {
                    PrintToScreen.WriteLine("Regression: beta: data points must be less than or equal to 1");
                    PrintToScreen.WriteLine("method betaMinMax used in place of method beta");
                    typeFlag = 1;
                    m_lastMethod = 32;
                    m_nTerms = 5;
                }
            }

            // Calculate an estimate of the alpha, beta and scale factor
            double dMode = distribMode;
            double dMean = meanX;
            if (typeFlag == 1)
            {
                dMode = (distribMode - minX*0.9D)/(maxX*1.2D - minX*0.9D);
                dMean = (meanX - minX*0.9D)/(maxX*1.2D - minX*0.9D);
            }
            double alphaGuess = 2.0D*dMode*dMean/(dMode - dMean);
            if (alphaGuess < 1.3)
            {
                alphaGuess = 1.6D;
            }
            double betaGuess = alphaGuess*(1.0D - dMean)/dMean;
            if (betaGuess <= 1.3)
            {
                betaGuess = 1.6D;
            }
            double scaleGuess = 0.0D;
            if (typeFlag == 0)
            {
                scaleGuess = yPeak/BetaDist.PdfStatic(alphaGuess, betaGuess, distribMode);
            }
            else
            {
                scaleGuess = yPeak/BetaDist.betaPDF(minX, maxX, alphaGuess, betaGuess, distribMode);
            }
            if (scaleGuess < 0)
            {
                scaleGuess = 1;
            }


            // Nelder and Mead Simplex Regression for Gumbel
            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            switch (typeFlag)
            {
                case 0:
                    start[0] = alphaGuess; //alpha
                    start[1] = betaGuess; //beta
                    if (m_scaleFlag)
                    {
                        start[2] = scaleGuess; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    step[1] = 0.1D*start[1];
                    if (m_scaleFlag)
                    {
                        step[2] = 0.1D*start[2];
                    }

                    // Add constraints
                    addConstraint(0, -1, 1.0D);
                    addConstraint(1, -1, 1.0D);
                    break;
                case 1:
                    start[0] = alphaGuess; //alpha
                    start[1] = betaGuess; //beta
                    start[2] = 0.9D*minX; // Min
                    start[3] = 1.1D*maxX; // Max
                    if (m_scaleFlag)
                    {
                        start[4] = scaleGuess; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    step[1] = 0.1D*start[1];
                    step[2] = 0.1D*start[2];
                    step[3] = 0.1D*start[3];
                    if (m_scaleFlag)
                    {
                        step[4] = 0.1D*start[4];
                    }

                    // Add constraints
                    addConstraint(0, -1, 1.0D);
                    addConstraint(1, -1, 1.0D);
                    addConstraint(2, +1, minX);
                    addConstraint(3, -1, maxX);
                    break;
            }

            // Create instance of Beta function
            BetaFunction ff = new BetaFunction();

            // Set minimum maximum type option
            ff.typeFlag = typeFlag;

            // Set ordinate scaling option
            ff.scaleOption = m_scaleFlag;
            ff.scaleFactor = m_yScaleFactor;

            // Perform simplex regression
            Object regFun3 = ff;
            nelderMead(regFun3, start, step, m_fTol, m_nMax);

            if (allTest == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(ff);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        public void gamma()
        {
            fitGamma(0, 0);
        }

        public void gammaPlot()
        {
            fitGamma(1, 0);
        }

        public void gammaStandard()
        {
            fitGamma(0, 1);
        }

        public void gammaStandardPlot()
        {
            fitGamma(1, 1);
        }

        protected void fitGamma(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_userSupplied = false;
            switch (typeFlag)
            {
                case 0:
                    m_lastMethod = 33;
                    m_nTerms = 4;
                    break;
                case 1:
                    m_lastMethod = 34;
                    m_nTerms = 2;
                    break;
            }
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }

            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(ArrayHelper.GetRowCopy(m_xData, 0), m_yData, m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitGamma(): This implementation of the Gamma distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // check  x data
            List<Object> retX = dataSign(ArrayHelper.GetRowCopy(m_xData, 0));
            int tempi = 0;

            // Calculate  x value at peak y (estimate of the 'distribution mode')
            tempi = (int) retY[5];
            int peaki = tempi;
            double distribMode = m_xData[0, peaki];

            // minimum value
            tempd = (double) retX[0];
            double minX = tempd;
            // maximum value
            tempd = (double) retX[2];
            double maxX = tempd;
            // m_mean value
            tempd = (double) retX[8];
            double meanX = tempd;


            // test that data is within range
            if (typeFlag == 1)
            {
                if (minX < 0.0D)
                {
                    PrintToScreen.WriteLine("Regression: gammaStandard: data points must be greater than or equal to 0");
                    PrintToScreen.WriteLine("method gamma used in place of method gammaStandard");
                    typeFlag = 0;
                    m_lastMethod = 33;
                    m_nTerms = 2;
                }
            }

            // Calculate an estimate of the m_mu, beta, gamma and scale factor
            double muGuess = 0.8D*minX;
            if (muGuess == 0.0D)
            {
                muGuess = -0.1D;
            }
            double betaGuess = meanX - distribMode;
            if (betaGuess <= 0.0D)
            {
                betaGuess = 1.0D;
            }
            double gammaGuess = (meanX + muGuess)/betaGuess;
            if (typeFlag == 1)
            {
                gammaGuess = meanX;
            }
            if (gammaGuess <= 0.0D)
            {
                gammaGuess = 1.0D;
            }
            double scaleGuess = 0.0D;
            if (typeFlag == 0)
            {
                scaleGuess = yPeak/GammaCfdDist.gammaPDF(muGuess, betaGuess, gammaGuess, distribMode);
            }
            else
            {
                scaleGuess = yPeak/GammaCfdDist.gammaPDF(gammaGuess, distribMode);
            }
            if (scaleGuess < 0)
            {
                scaleGuess = 1;
            }


            // Nelder and Mead Simplex Regression for Gamma
            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            switch (typeFlag)
            {
                case 1:
                    start[0] = gammaGuess; //gamma
                    if (m_scaleFlag)
                    {
                        start[1] = scaleGuess; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    if (m_scaleFlag)
                    {
                        step[1] = 0.1D*start[1];
                    }

                    // Add constraints
                    addConstraint(0, -1, 0.0D);
                    break;
                case 0:
                    start[0] = muGuess; // m_mu
                    start[1] = betaGuess; // beta
                    start[2] = gammaGuess; // gamma
                    if (m_scaleFlag)
                    {
                        start[3] = scaleGuess; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    step[1] = 0.1D*start[1];
                    step[2] = 0.1D*start[2];
                    if (m_scaleFlag)
                    {
                        step[3] = 0.1D*start[3];
                    }

                    // Add constraints
                    addConstraint(1, -1, 0.0D);
                    addConstraint(2, -1, 0.0D);
                    break;
            }

            // Create instance of Gamma function
            GammaFunction ff = new GammaFunction();

            // Set type option
            ff.typeFlag = typeFlag;

            // Set ordinate scaling option
            ff.scaleOption = m_scaleFlag;
            ff.scaleFactor = m_yScaleFactor;

            // Perform simplex regression
            Object regFun3 = ff;
            nelderMead(regFun3, start, step, m_fTol, m_nMax);

            if (allTest == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(ff);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        // Fit to an Erlang Distribution
        public void erlang()
        {
            fitErlang(0, 0);
        }

        public void erlangPlot()
        {
            fitErlang(1, 0);
        }

        protected void fitErlang(int allTest, int typeFlag)
        {
            if (m_multipleY)
            {
                throw new HCException("This method cannot handle multiply dimensioned y arrays");
            }
            m_lastMethod = 35;
            m_userSupplied = false;
            int nTerms0 = 2; // number of erlang terms
            int nTerms1 = 4; // number of gamma terms - initial estimates procedure
            m_nTerms = nTerms1;
            if (!m_scaleFlag)
            {
                m_nTerms = m_nTerms - 1;
            }

            m_zeroCheck = false;
            m_degreesOfFreedom = m_nData - m_nTerms;
            if (m_degreesOfFreedom < 1 && !m_blnIgnoreDofFcheck)
            {
                throw new HCException("Degrees of freedom must be greater than 0");
            }

            // order data into ascending order of the abscissae
            sort(
                ArrayHelper.GetRowCopy(m_xData, 0), m_yData, m_weight);

            // check sign of y data
            double tempd = 0;
            List<Object> retY = dataSign(m_yData);
            tempd = (double) retY[4];
            double yPeak = tempd;
            bool yFlag = false;
            if (yPeak < 0.0D)
            {
                PrintToScreen.WriteLine(
                    "Regression.fitGamma(): This implementation of the Erlang distribution takes only positive y values\n(noise taking low values below zero are allowed)");
                PrintToScreen.WriteLine("All y values have been multiplied by -1 before fitting");
                for (int i = 0; i < m_nData; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
                retY = dataSign(m_yData);
                yFlag = true;
            }

            // check  x data
            List<Object> retX = dataSign(
                ArrayHelper.GetRowCopy(m_xData, 0));
            int tempi = 0;

            // Calculate  x value at peak y (estimate of the 'distribution mode')
            tempi = (int) retY[5];
            int peaki = tempi;
            double distribMode = m_xData[0, peaki];

            // minimum value
            tempd = (double) retX[0];
            double minX = tempd;
            // maximum value
            tempd = (double) retX[2];
            double maxX = tempd;
            // m_mean value
            tempd = (double) retX[8];
            double meanX = tempd;


            // test that data is within range
            if (minX < 0.0D)
            {
                throw new HCException("data points must be greater than or equal to 0");
            }

            // FIT TO GAMMA DISTRIBUTION TO OBTAIN INITIAL ESTIMATES
            // Calculate an estimate of the m_mu, beta, gamma and scale factor
            double muGuess = 0.8D*minX;
            if (muGuess == 0.0D)
            {
                muGuess = -0.1D;
            }
            double betaGuess = meanX - distribMode;
            if (betaGuess <= 0.0D)
            {
                betaGuess = 1.0D;
            }
            double gammaGuess = (meanX + muGuess)/betaGuess;
            if (typeFlag == 1)
            {
                gammaGuess = meanX;
            }
            if (gammaGuess <= 0.0D)
            {
                gammaGuess = 1.0D;
            }
            double scaleGuess = 0.0D;
            scaleGuess = yPeak/GammaCfdDist.gammaPDF(muGuess, betaGuess, gammaGuess, distribMode);
            if (scaleGuess < 0)
            {
                scaleGuess = 1;
            }


            // Nelder and Mead Simplex Regression for Gamma
            // Fill arrays needed by the Simplex
            double[] start = new double[m_nTerms];
            double[] step = new double[m_nTerms];
            start[0] = muGuess; // m_mu
            start[1] = betaGuess; // beta
            start[2] = gammaGuess; // gamma
            if (m_scaleFlag)
            {
                start[3] = scaleGuess; //y axis scaling factor
            }

            step[0] = 0.1D*start[0];
            step[1] = 0.1D*start[1];
            step[2] = 0.1D*start[2];
            if (m_scaleFlag)
            {
                step[3] = 0.1D*start[3];
            }

            // Add constraints
            addConstraint(1, -1, 0.0D);
            addConstraint(2, -1, 0.0D);

            // Create instance of Gamma function
            GammaFunction ff = new GammaFunction();

            // Set type option
            ff.typeFlag = typeFlag;

            // Set ordinate scaling option
            ff.scaleOption = m_scaleFlag;
            ff.scaleFactor = m_yScaleFactor;

            // Perform simplex regression
            Object regFun3 = ff;
            nelderMead(regFun3, start, step, m_fTol, m_nMax);

            // FIT TO ERLANG DISTRIBUTION USING GAMMA BEST ESTIMATES AS INITIAL ESTIMATES
            // AND VARYING RATE PARAMETER BY UNIT STEPS
            removeConstraints();

            // Initial estimates
            double[] bestGammaEst = getCoeff();

            // Swap from Gamma dimensions to Erlang dimensions
            m_nTerms = nTerms0;
            start = new double[m_nTerms];
            step = new double[m_nTerms];
            if (bestGammaEst[3] < 0.0)
            {
                bestGammaEst[3] *= -1.0;
            }

            // initial estimates
            start[0] = 1.0D/bestGammaEst[1]; // lambda
            if (m_scaleFlag)
            {
                start[1] = bestGammaEst[3]; //y axis scaling factor
            }

            step[0] = 0.1D*start[0];
            if (m_scaleFlag)
            {
                step[1] = 0.1D*start[1];
            }

            // Add constraints
            addConstraint(0, -1, 0.0D);

            // fix initial integer rate parameter
            double kay0 = Math.Round(bestGammaEst[2]);
            double kay = kay0;

            // Create instance of Erlang function
            ErlangFunction ef = new ErlangFunction();

            // Set ordinate scaling option
            ef.scaleOption = m_scaleFlag;
            ef.scaleFactor = m_yScaleFactor;
            ef.kay = kay;

            // Fit stepping up
            bool testKay = true;
            double ssMin = double.NaN;
            double upSS = double.NaN;
            double upKay = double.NaN;
            double kayFinal = double.NaN;
            int iStart = 1;
            int ssSame = 0;

            while (testKay)
            {
                // Perform simplex regression
                Object regFun4_ = ef;

                nelderMead(regFun4_, start, step, m_fTol, m_nMax);
                double sumOfSquares = getSumOfSquares();
                if (iStart == 1)
                {
                    iStart = 2;
                    ssMin = sumOfSquares;
                    kay = kay + 1;
                    start[0] = 1.0D/bestGammaEst[1]; // lambda
                    if (m_scaleFlag)
                    {
                        start[1] = bestGammaEst[3]; //y axis scaling factor
                    }
                    step[0] = 0.1D*start[0];
                    if (m_scaleFlag)
                    {
                        step[1] = 0.1D*start[1];
                    }
                    addConstraint(0, -1, 0.0D);
                    ef.kay = kay;
                }
                else
                {
                    if (sumOfSquares <= ssMin)
                    {
                        if (sumOfSquares == ssMin)
                        {
                            ssSame++;
                            if (ssSame == 10)
                            {
                                upSS = ssMin;
                                upKay = kay - 5;
                                testKay = false;
                            }
                        }
                        ssMin = sumOfSquares;
                        kay = kay + 1;
                        start[0] = 1.0D/bestGammaEst[1]; // lambda
                        if (m_scaleFlag)
                        {
                            start[1] = bestGammaEst[3]; //y axis scaling factor
                        }
                        step[0] = 0.1D*start[0];
                        if (m_scaleFlag)
                        {
                            step[1] = 0.1D*start[1];
                        }
                        addConstraint(0, -1, 0.0D);
                        ef.kay = kay;
                    }
                    else
                    {
                        upSS = ssMin;
                        upKay = kay - 1;
                        testKay = false;
                    }
                }
            }

            if (kay0 == 1)
            {
                kayFinal = upKay;
            }
            else
            {
                // Fit stepping down
                iStart = 1;
                testKay = true;
                ssMin = double.NaN;
                double downSS = double.NaN;
                double downKay = double.NaN;
                // initial estimates
                start[0] = 1.0D/bestGammaEst[1]; // lambda
                if (m_scaleFlag)
                {
                    start[1] = bestGammaEst[3]; //y axis scaling factor
                }
                step[0] = 0.1D*start[0];
                if (m_scaleFlag)
                {
                    step[1] = 0.1D*start[1];
                }
                // Add constraints
                addConstraint(0, -1, 0.0D);
                kay = kay0;
                ef.kay = kay;

                while (testKay)
                {
                    // Perform simplex regression
                    Object regFun5_ = ef;

                    nelderMead(regFun5_, start, step, m_fTol, m_nMax);
                    double sumOfSquares = getSumOfSquares();
                    if (iStart == 1)
                    {
                        iStart = 2;
                        ssMin = sumOfSquares;
                        kay = kay - 1;
                        if (Converter.rint(kay) < 1L)
                        {
                            downSS = ssMin;
                            downKay = kay + 1;
                            testKay = false;
                        }
                        else
                        {
                            start[0] = 1.0D/bestGammaEst[1]; // lambda
                            if (m_scaleFlag)
                            {
                                start[1] = bestGammaEst[3]; //y axis scaling factor
                            }
                            step[0] = 0.1D*start[0];
                            if (m_scaleFlag)
                            {
                                step[1] = 0.1D*start[1];
                            }
                            addConstraint(0, -1, 0.0D);
                            ef.kay = kay;
                        }
                    }
                    else
                    {
                        if (sumOfSquares <= ssMin)
                        {
                            ssMin = sumOfSquares;
                            kay = kay - 1;
                            if (Converter.rint(kay) < 1L)
                            {
                                downSS = ssMin;
                                downKay = kay + 1;
                                testKay = false;
                            }
                            else
                            {
                                start[0] = 1.0D/bestGammaEst[1]; // lambda
                                if (m_scaleFlag)
                                {
                                    start[1] = bestGammaEst[3]; //y axis scaling factor
                                }
                                step[0] = 0.1D*start[0];
                                if (m_scaleFlag)
                                {
                                    step[1] = 0.1D*start[1];
                                }
                                addConstraint(0, -1, 0.0D);
                                ef.kay = kay;
                            }
                        }
                        else
                        {
                            downSS = ssMin;
                            downKay = kay + 1;
                            testKay = false;
                        }
                    }
                }
                if (downSS < upSS)
                {
                    kayFinal = downKay;
                }
                else
                {
                    kayFinal = upKay;
                }
            }

            // Penultimate fit
            // initial estimates
            start[0] = 1.0D/bestGammaEst[1]; // lambda
            if (m_scaleFlag)
            {
                start[1] = bestGammaEst[3]; //y axis scaling factor
            }

            step[0] = 0.1D*start[0];
            if (m_scaleFlag)
            {
                step[1] = 0.1D*start[1];
            }

            // Add constraints
            addConstraint(0, -1, 0.0D);

            // Set function variables
            ef.scaleOption = m_scaleFlag;
            ef.scaleFactor = m_yScaleFactor;
            ef.kay = Math.Round(kayFinal);
            m_kayValue = Math.Round(kayFinal);

            // Perform penultimate regression
            Object regFun4 = ef;

            nelderMead(regFun4, start, step, m_fTol, m_nMax);
            double[] coeff = getCoeff();

            // Final fit

            // initial estimates
            start[0] = coeff[0]; // lambda
            if (m_scaleFlag)
            {
                start[1] = coeff[1]; //y axis scaling factor
            }

            step[0] = 0.1D*start[0];
            if (m_scaleFlag)
            {
                step[1] = 0.1D*start[1];
            }

            // Add constraints
            addConstraint(0, -1, 0.0D);

            // Set function variables
            ef.scaleOption = m_scaleFlag;
            ef.scaleFactor = m_yScaleFactor;
            ef.kay = Math.Round(kayFinal);
            m_kayValue = Math.Round(kayFinal);

            // Perform  regression
            Object regFun5 = ef;

            nelderMead(regFun5, start, step, m_fTol, m_nMax);

            if (allTest == 1)
            {
                // Print results
                if (!m_blnSupressPrint)
                {
                    print();
                }

                // Plot results
                int flag = plotXY(ef);
                if (flag != -2 && !m_blnSupressYYplot)
                {
                    plotYY();
                }
            }

            if (yFlag)
            {
                // restore data
                for (int i = 0; i < m_nData - 1; i++)
                {
                    m_yData[i] = -m_yData[i];
                }
            }
        }

        // return Erlang rate parameter (k) value
        public double getKayValue()
        {
            return m_kayValue;
        }


        // HISTOGRAM METHODS
        // Distribute data into bins to obtain histogram
        // zero bin position and upper limit provided
        public static double[,] histogramBins(double[] data, double binWidth, double binZero, double binUpper)
        {
            int n = 0; // new array Length
            int m = data.Length; // old array Length;
            for (int i = 0; i < m; i++)
            {
                if (data[i] <= binUpper)
                {
                    n++;
                }
            }
            if (n != m)
            {
                double[] newData = new double[n];
                int j = 0;
                for (int i = 0; i < m; i++)
                {
                    if (data[i] <= binUpper)
                    {
                        newData[j] = data[i];
                        j++;
                    }
                }
                PrintToScreen.WriteLine((m - n) +
                                        " data points, above histogram upper limit, excluded in Stat.histogramBins");
                return histogramBins(newData, binWidth, binZero);
            }
            else
            {
                return histogramBins(data, binWidth, binZero);
            }
        }

        // Distribute data into bins to obtain histogram
        // zero bin position provided
        public static double[,] histogramBins(double[] data, double binWidth, double binZero)
        {
            double dmax = Fmath.maximum(data);
            int nBins = (int) Math.Ceiling((dmax - binZero)/binWidth);
            if (binZero + nBins*binWidth > dmax)
            {
                nBins++;
            }
            int nPoints = data.Length;
            int[] dataCheck = new int[nPoints];
            for (int i = 0; i < nPoints; i++)
            {
                dataCheck[i] = 0;
            }
            double[] binWall = new double[nBins + 1];
            binWall[0] = binZero;
            for (int i = 1; i <= nBins; i++)
            {
                binWall[i] = binWall[i - 1] + binWidth;
            }
            double[,] binFreq = new double[2,nBins];
            for (int i = 0; i < nBins; i++)
            {
                binFreq[0, i] = (binWall[i] + binWall[i + 1])/2.0D;
                binFreq[1, i] = 0.0D;
            }
            bool test = true;

            for (int i = 0; i < nPoints; i++)
            {
                test = true;
                int j = 0;
                while (test)
                {
                    if (j == nBins - 1)
                    {
                        if (data[i] >= binWall[j] && data[i] <= binWall[j + 1]*(1.0D + m_histTol))
                        {
                            binFreq[1, j] += 1.0D;
                            dataCheck[i] = 1;
                            test = false;
                        }
                    }
                    else
                    {
                        if (data[i] >= binWall[j] && data[i] < binWall[j + 1])
                        {
                            binFreq[1, j] += 1.0D;
                            dataCheck[i] = 1;
                            test = false;
                        }
                    }
                    if (test)
                    {
                        if (j == nBins - 1)
                        {
                            test = false;
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
            }
            int nMissed = 0;
            for (int i = 0; i < nPoints; i++)
            {
                if (dataCheck[i] == 0)
                {
                    nMissed++;
                    PrintToScreen.WriteLine("p " + i + " " + data[i] + " " + binWall[0] + " " + binWall[nBins]);
                }
            }
            if (nMissed > 0)
            {
                PrintToScreen.WriteLine(nMissed +
                                        " data points, outside histogram limits, excluded in Stat.histogramBins");
            }
            return binFreq;
        }

        // Distribute data into bins to obtain histogram
        // zero bin position calculated
        public static double[,] histogramBins(double[] data, double binWidth)
        {
            double dmin = Fmath.minimum(data);
            double dmax = Fmath.maximum(data);
            double span = dmax - dmin;
            double binZero = dmin;
            int nBins = (int) Math.Ceiling(span/binWidth);
            double histoSpan = (nBins)*binWidth;
            double rem = histoSpan - span;
            if (rem >= 0)
            {
                binZero -= rem/2.0D;
            }
            else
            {
                if (Math.Abs(rem)/span > m_histTol)
                {
                    // readjust binWidth
                    bool testBw = true;
                    double incr = m_histTol/nBins;
                    int iTest = 0;
                    while (testBw)
                    {
                        binWidth += incr;
                        histoSpan = (nBins)*binWidth;
                        rem = histoSpan - span;
                        if (rem < 0)
                        {
                            iTest++;
                            if (iTest > 1000)
                            {
                                testBw = false;
                                PrintToScreen.WriteLine(
                                    "histogram method could not encompass all data within histogram\nContact Michael thomas Flanagan");
                            }
                        }
                        else
                        {
                            testBw = false;
                        }
                    }
                }
            }

            return histogramBins(data, binWidth, binZero);
        }
    }

    //  CLASSES TO EVALUATE THE SPECIAL FUNCTIONS


    // Class to evaluate the Gausian (normal) function y = (yscale/m_sd.Sqrt(2.pi)).Exp(-0.5[(x - xmean)/m_sd]^2).
    internal class GaussianFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double y = (yScale/(p[1]*Math.Sqrt(2.0D*Math.PI)))*Math.Exp(-0.5D*Fmath.square((x[0] - p[0])/p[1]));
            return y;
        }

        #endregion
    }

    // Class to evaluate the Gausian (normal) function y = (yscale/m_sd.Sqrt(2.pi)).Exp(-0.5[(x - xmean)/m_sd]^2).
    // Some parameters may be fixed_
    internal class GaussianFunctionfixed_ : RegressionFunction
    {
        public bool[] fixed_ = new bool[3];
        public double[] param = new double[3];

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            int ii = 0;
            for (int i = 0; i < 3; i++)
            {
                if (!fixed_[i])
                {
                    param[i] = p[ii];
                    ii++;
                }
            }

            double y = (param[2]/(param[1]*Math.Sqrt(2.0D*Math.PI)))*
                       Math.Exp(-0.5D*Fmath.square((x[0] - param[0])/param[1]));
            return y;
        }

        #endregion
    }

    // Class to evaluate the two parameter log-normal function y = (yscale/x.m_sigma.Sqrt(2.pi)).Exp(-0.5[(Log(x) - m_mu)/m_sd]^2).
    internal class LogNormalTwoParFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double y = (yScale/(x[0]*p[1]*Math.Sqrt(2.0D*Math.PI)))*
                       Math.Exp(-0.5D*Fmath.square((Math.Log(x[0]) - p[0])/p[1]));
            return y;
        }

        #endregion
    }

    // Class to evaluate the three parameter log-normal function y = (yscale/(x-alpha).beta.sqrt(2.pi)).exp(-0.5[(log((x-alpha)/gamma)/m_sd]^2).
    internal class LogNormalThreeParFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[3];
            }
            double y = (yScale/((x[0] - p[0])*p[1]*Math.Sqrt(2.0D*Math.PI)))*
                       Math.Exp(-0.5D*Fmath.square(Math.Log((x[0] - p[0])/p[2])/p[1]));
            return y;
        }

        #endregion
    }


    // Class to evaluate the Lorentzian function
    // y = (yscale/pi).(gamma/2)/((x - m_mu)^2+(gamma/2)^2).
    internal class LorentzianFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double y = (yScale/Math.PI)*(p[1]/2.0D)/(Fmath.square(x[0] - p[0]) + Fmath.square(p[1]/2.0D));
            return y;
        }

        #endregion
    }

    // Class to evaluate the Poisson function
    // y = yscale.(m_mu^k).Exp(-m_mu)/k!.
    internal class PoissonFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[1];
            }
            double y = yScale*Math.Pow(p[0], x[0])*Math.Exp(-p[0])/Stat.factorial(x[0]);
            return y;
        }

        #endregion
    }

    // Class to evaluate the Gumbel function
    internal class GumbelFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 0 -> Minimum Mode Gumbel
        // reset to 1 -> Maximum Mode Gumbel
        // reset to 2 -> one parameter Minimum Mode Gumbel
        // reset to 3 -> one parameter Maximum Mode Gumbel
        // reset to 4 -> standard Minimum Mode Gumbel
        // reset to 5 -> standard Maximum Mode Gumbel

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            double arg = 0.0D;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 0:
                    // y = yscale*(1/gamma)*Exp((x-m_mu)/gamma)*Exp(-Exp((x-m_mu)/gamma))
                    arg = (x[0] - p[0])/p[1];
                    if (scaleOption)
                    {
                        yScale = p[2];
                    }
                    y = (yScale/p[1])*Math.Exp(arg)*Math.Exp(-(Math.Exp(arg)));
                    break;
                case 1:
                    // y = yscale*(1/gamma)*Exp((m_mu-x)/gamma)*Exp(-Exp((m_mu-x)/gamma))
                    arg = (p[0] - x[0])/p[1];
                    if (scaleOption)
                    {
                        yScale = p[2];
                    }
                    y = (yScale/p[1])*Math.Exp(arg)*Math.Exp(-(Math.Exp(arg)));
                    break;
                case 2:
                    // y = yscale*(1/gamma)*Exp((x)/gamma)*Exp(-Exp((x)/gamma))
                    arg = x[0]/p[0];
                    if (scaleOption)
                    {
                        yScale = p[1];
                    }
                    y = (yScale/p[0])*Math.Exp(arg)*Math.Exp(-(Math.Exp(arg)));
                    break;
                case 3:
                    // y = yscale*(1/gamma)*Exp((-x)/gamma)*Exp(-Exp((-x)/gamma))
                    arg = -x[0]/p[0];
                    if (scaleOption)
                    {
                        yScale = p[1];
                    }
                    y = (yScale/p[0])*Math.Exp(arg)*Math.Exp(-(Math.Exp(arg)));
                    break;
                case 4:
                    // y = yscale*Exp(x)*Exp(-Exp(x))
                    if (scaleOption)
                    {
                        yScale = p[0];
                    }
                    y = yScale*Math.Exp(x[0])*Math.Exp(-(Math.Exp(x[0])));
                    break;
                case 5:
                    // y = yscale*Exp(-x)*Exp(-Exp(-x))
                    if (scaleOption)
                    {
                        yScale = p[0];
                    }
                    y = yScale*Math.Exp(-x[0])*Math.Exp(-(Math.Exp(-x[0])));
                    break;
            }
            return y;
        }

        #endregion
    }

    // Class to evaluate the Frechet function
    // y = yscale.(gamma/m_sigma)*((x - m_mu)/m_sigma)^(-gamma-1)*Exp(-((x-m_mu)/m_sigma)^-gamma
    internal class FrechetFunctionOne : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 0 -> Three Parameter Frechet
        // reset to 1 -> Two Parameter Frechet
        // reset to 2 -> Standard Frechet

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 0:
                    if (x[0] >= p[0])
                    {
                        double arg = (x[0] - p[0])/p[1];
                        if (scaleOption)
                        {
                            yScale = p[3];
                        }
                        y = yScale*(p[2]/p[1])*Math.Pow(arg, -p[2] - 1.0D)*Math.Exp(-Math.Pow(arg, -p[2]));
                    }
                    break;
                case 1:
                    if (x[0] >= 0.0D)
                    {
                        double arg = x[0]/p[0];
                        if (scaleOption)
                        {
                            yScale = p[2];
                        }
                        y = yScale*(p[1]/p[0])*Math.Pow(arg, -p[1] - 1.0D)*Math.Exp(-Math.Pow(arg, -p[1]));
                    }
                    break;
                case 2:
                    if (x[0] >= 0.0D)
                    {
                        double arg = x[0];
                        if (scaleOption)
                        {
                            yScale = p[1];
                        }
                        y = yScale*p[0]*Math.Pow(arg, -p[0] - 1.0D)*Math.Exp(-Math.Pow(arg, -p[0]));
                    }
                    break;
            }
            return y;
        }

        #endregion
    }

    // Class to evaluate the semi-linearised Frechet function
    // Log(Log(1/(1-Cumulative y) = gamma*Log((x-m_mu)/m_sigma)
    internal class FrechetFunctionTwo : RegressionFunction
    {
        public int typeFlag; // set to 0 -> Three Parameter Frechet
        // reset to 1 -> Two Parameter Frechet
        // reset to 2 -> Standard Frechet

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            switch (typeFlag)
            {
                case 0:
                    y = -p[2]*Math.Log(Math.Abs(x[0] - p[0])/p[1]);
                    break;
                case 1:
                    y = -p[1]*Math.Log(Math.Abs(x[0])/p[0]);
                    break;
                case 2:
                    y = -p[0]*Math.Log(Math.Abs(x[0]));
                    break;
            }

            return y;
        }

        #endregion
    }

    // Class to evaluate the Weibull function
    // y = yscale.(gamma/m_sigma)*((x - m_mu)/m_sigma)^(gamma-1)*Exp(-((x-m_mu)/m_sigma)^gamma
    internal class WeibullFunctionOne : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 0 -> Three Parameter Weibull
        // reset to 1 -> Two Parameter Weibull
        // reset to 2 -> Standard Weibull

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 0:
                    if (x[0] >= p[0])
                    {
                        double arg = (x[0] - p[0])/p[1];
                        if (scaleOption)
                        {
                            yScale = p[3];
                        }
                        y = yScale*(p[2]/p[1])*Math.Pow(arg, p[2] - 1.0D)*Math.Exp(-Math.Pow(arg, p[2]));
                    }
                    break;
                case 1:
                    if (x[0] >= 0.0D)
                    {
                        double arg = x[0]/p[0];
                        if (scaleOption)
                        {
                            yScale = p[2];
                        }
                        y = yScale*(p[1]/p[0])*Math.Pow(arg, p[1] - 1.0D)*Math.Exp(-Math.Pow(arg, p[1]));
                    }
                    break;
                case 2:
                    if (x[0] >= 0.0D)
                    {
                        double arg = x[0];
                        if (scaleOption)
                        {
                            yScale = p[1];
                        }
                        y = yScale*p[0]*Math.Pow(arg, p[0] - 1.0D)*Math.Exp(-Math.Pow(arg, p[0]));
                    }
                    break;
            }
            return y;
        }

        #endregion
    }

    // Class to evaluate the semi-linearised Weibull function
    // Log(Log(1/(1-Cumulative y) = gamma*Log((x-m_mu)/m_sigma)
    internal class WeibullFunctionTwo : RegressionFunction
    {
        public int typeFlag; // set to 0 -> Three Parameter Weibull
        // reset to 1 -> Two Parameter Weibull
        // reset to 2 -> Standard Weibull

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            switch (typeFlag)
            {
                case 0:
                    y = p[2]*Math.Log(Math.Abs(x[0] - p[0])/p[1]);
                    break;
                case 1:
                    y = p[1]*Math.Log(Math.Abs(x[0])/p[0]);
                    break;
                case 2:
                    y = p[0]*Math.Log(Math.Abs(x[0]));
                    break;
            }

            return y;
        }

        #endregion
    }

    // Class to evaluate the Rayleigh function
    // y = (yscale/m_sigma)*(x/m_sigma)*Exp(-0.5((x-m_mu)/m_sigma)^2
    internal class RayleighFunctionOne : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[1];
            }
            if (x[0] >= 0.0D)
            {
                double arg = x[0]/p[0];
                y = (yScale/p[0])*arg*Math.Exp(-0.5D*Math.Pow(arg, 2));
            }
            return y;
        }

        #endregion
    }


    // Class to evaluate the semi-linearised Rayleigh function
    // Log(1/(1-Cumulative y) = 0.5*(x/m_sigma)^2
    internal class RayleighFunctionTwo : RegressionFunction
    {
        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.5D*Math.Pow(x[0]/p[0], 2);
            return y;
        }

        #endregion
    }

    // class to evaluate a simple exponential function
    internal class ExponentialSimpleFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[1];
            }
            double y = yScale*Math.Exp(p[0]*x[0]);
            return y;
        }

        #endregion
    }

    // class to evaluate multiple exponentials function
    internal class ExponentialMultipleFunction : RegressionFunction
    {
        public int nExps;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0;
            for (int i = 0; i < nExps; i += 2)
            {
                y += p[i]*Math.Exp(p[i + 1]*x[0]);
            }
            return y;
        }

        #endregion
    }

    // class to evaluate 1 - exponential function
    internal class OneMinusExponentialFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[0];
            }
            double y = yScale*(1 - Math.Exp(p[1]*x[0]));
            return y;
        }

        #endregion
    }

    // class to evaluate a exponential distribution function
    internal class ExponentialFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 0 -> Two Parameter Exponential
        // reset to 1 -> One Parameter Exponential
        // reset to 2 -> Standard Exponential

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 0:
                    if (x[0] >= p[0])
                    {
                        if (scaleOption)
                        {
                            yScale = p[2];
                        }
                        double arg = (x[0] - p[0])/p[1];
                        y = (yScale/p[1])*Math.Exp(-arg);
                    }
                    break;
                case 1:
                    if (x[0] >= 0.0D)
                    {
                        double arg = x[0]/p[0];
                        if (scaleOption)
                        {
                            yScale = p[1];
                        }
                        y = (yScale/p[0])*Math.Exp(-arg);
                    }
                    break;
                case 2:
                    if (x[0] >= 0.0D)
                    {
                        double arg = x[0];
                        if (scaleOption)
                        {
                            yScale = p[0];
                        }
                        y = yScale*Math.Exp(-arg);
                    }
                    break;
            }
            return y;
        }

        #endregion
    }

    // class to evaluate a Pareto scaled pdf
    internal class ParetoFunctionOne : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 3 -> Shifted Pareto
        // set to 2 -> Two Parameter Pareto
        // set to 1 -> One Parameter Pareto

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 3:
                    if (x[0] >= p[1] + p[2])
                    {
                        if (scaleOption)
                        {
                            yScale = p[3];
                        }
                        y = yScale*p[0]*Math.Pow(p[1], p[0])/Math.Pow((x[0] - p[2]), p[0] + 1.0D);
                    }
                    break;
                case 2:
                    if (x[0] >= p[1])
                    {
                        if (scaleOption)
                        {
                            yScale = p[2];
                        }
                        y = yScale*p[0]*Math.Pow(p[1], p[0])/Math.Pow(x[0], p[0] + 1.0D);
                    }
                    break;
                case 1:
                    if (x[0] >= 1.0D)
                    {
                        double arg = x[0]/p[0];
                        if (scaleOption)
                        {
                            yScale = p[1];
                        }
                        y = yScale*p[0]/Math.Pow(x[0], p[0] + 1.0D);
                    }
                    break;
            }
            return y;
        }

        #endregion
    }

    // class to evaluate a Pareto cdf
    internal class ParetoFunctionTwo : RegressionFunction
    {
        public int typeFlag; // set to 3 -> Shifted Pareto
        // set to 2 -> Two Parameter Pareto
        // set to 1 -> One Parameter Pareto

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            switch (typeFlag)
            {
                case 3:
                    if (x[0] >= p[1] + p[2])
                    {
                        y = 1.0D - Math.Pow(p[1]/(x[0] - p[2]), p[0]);
                    }
                    break;
                case 2:
                    if (x[0] >= p[1])
                    {
                        y = 1.0D - Math.Pow(p[1]/x[0], p[0]);
                    }
                    break;
                case 1:
                    if (x[0] >= 1.0D)
                    {
                        y = 1.0D - Math.Pow(1.0D/x[0], p[0]);
                    }
                    break;
            }
            return y;
        }

        #endregion
    }

    // class to evaluate a Sigmoidal threshold function
    internal class SigmoidThresholdFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double y = yScale/(1.0D + Math.Exp(-p[0]*(x[0] - p[1])));
            return y;
        }

        #endregion
    }

    // class to evaluate a Rectangular Hyberbola
    internal class RectangularHyperbolaFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double y = yScale*x[0]/(p[0] + x[0]);
            return y;
        }

        #endregion
    }

    // class to evaluate a scaled Heaviside Step Function
    internal class StepFunctionFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[1];
            }
            double y = 0.0D;
            if (x[0] > p[0])
            {
                y = yScale;
            }
            return y;
        }

        #endregion
    }

    // class to evaluate a Hill or Sips sigmoidal function
    internal class SigmoidHillSipsFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double xterm = Math.Pow(x[0], p[1]);
            double y = yScale*xterm/(Math.Pow(p[0], p[1]) + xterm);
            return y;
        }

        #endregion
    }

    // Class to evaluate the Logistic probability function y = yscale*Exp(-(x-m_mu)/beta)/(beta*(1 + Exp(-(x-m_mu)/beta))^2.
    internal class LogisticFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double yScale = scaleFactor;
            if (scaleOption)
            {
                yScale = p[2];
            }
            double y = yScale*Fmath.square(Fmath.sech((x[0] - p[0])/(2.0D*p[1])))/(4.0D*p[1]);
            return y;
        }

        #endregion
    }

    // class to evaluate a Beta scaled pdf
    internal class BetaFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 0 -> Beta Distibution - [0, 1] interval
        // set to 1 -> Beta Distibution - [Min, Max] interval

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 0:
                    if (scaleOption)
                    {
                        yScale = p[2];
                    }
                    y = yScale*Math.Pow(x[0], p[0] - 1.0D)*Math.Pow(1.0D - x[0], p[1] - 1.0D)/
                        BetaFunct.betaFunction(p[0], p[1]);
                    break;
                case 1:
                    if (scaleOption)
                    {
                        yScale = p[4];
                    }
                    y = yScale*Math.Pow(x[0] - p[2], p[0] - 1.0D)*Math.Pow(p[3] - x[0], p[1] - 1.0D)/
                        BetaFunct.betaFunction(p[0], p[1]);
                    y = y/Math.Pow(p[3] - p[2], p[0] + p[1] - 1.0D);
                    break;
            }
            return y;
        }

        #endregion
    }

    // class to evaluate a Gamma scaled pdf
    internal class GammaFunction : RegressionFunction
    {
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;
        public int typeFlag; // set to 0 -> Three parameter Gamma Distribution
        // set to 1 -> Standard Gamma Distribution

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = 0.0D;
            //bool test = false;
            double yScale = scaleFactor;

            switch (typeFlag)
            {
                case 0:
                    if (scaleOption)
                    {
                        yScale = p[3];
                    }
                    double xTerm = (x[0] - p[0])/p[1];
                    y = yScale*Math.Pow(xTerm, p[2] - 1.0D)*Math.Exp(-xTerm)/(p[1]*GammaFunct.Gamma(p[2]));
                    break;
                case 1:
                    if (scaleOption)
                    {
                        yScale = p[1];
                    }
                    y = yScale*Math.Pow(x[0], p[0] - 1.0D)*Math.Exp(-x[0])/GammaFunct.Gamma(p[0]);
                    break;
            }
            return y;
        }

        #endregion
    }

    // class to evaluate a Erlang scaled pdf
    // rate parameter is fixed_
    internal class ErlangFunction : RegressionFunction
    {
        public double kay = 1.0D; // rate parameter
        public double scaleFactor = 1.0D;
        public bool scaleOption = true;

        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            //bool test = false;
            double yScale = scaleFactor;

            if (scaleOption)
            {
                yScale = p[1];
            }

            double y = kay*Math.Log(p[0]) + (kay - 1)*Math.Log(x[0]) - x[0]*p[0] - Fmath.logFactorial(kay - 1);
            y = yScale*Math.Exp(y);

            return y;
        }

        #endregion
    }

    // class to evaluate a EC50 function
    internal class EC50Function : RegressionFunction
    {
        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = p[0] + (p[1] - p[0])/(1.0D + Math.Pow(x[0]/p[2], p[3]));
            return y;
        }

        #endregion
    }

    // class to evaluate a LogEC50 function
    internal class LogEC50Function : RegressionFunction
    {
        #region RegressionFunction Members

        public double function(double[] p, double[] x)
        {
            double y = p[0] + (p[1] - p[0])/(1.0D + Math.Pow(10.0D, (p[2] - x[0])*p[3]));
            return y;
        }

        #endregion
    }
}
