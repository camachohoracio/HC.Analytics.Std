#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Random;
using HC.Analytics.Statistics;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Analysis
{
    /*
    *   Class   ComplexErrorProp
    *
    *   Defines an object describing a complex number in which there are
    *   errors associted with real and imaginary parts aqnd includes the
    *   methods for propagating the error in standard arithmetic operations
    *   for both uncorrelated errors only.
    *
    *   AUTHOR: Dr Michael Thomas Flanagan
    *
    *   DATE: 27 April 2004
    *   UPDATE: 19 January 2005, 28 May 2007
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's library on-line web pages:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/ComplexErrorProp.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    *   Copyright (c) April 2004, May 2007  Michael Thomas Flanagan
    *
    *   PERMISSION TO COPY:
    *   Permission to use, copy and modify this software and its documentation for
    *   NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
    *   to the author, Michael Thomas Flanagan at www.ucl.ee.ac.uk/~mflanaga, appears in all copies.
    *
    *   Dr Michael Thomas Flanagan makes no representations about the suitability
    *   or fitness of the software for any or for a particular purpose.
    *   Michael Thomas Flanagan shall not be liable for any damages suffered
    *   as a result of using, modifying or distributing this software or its derivatives.
    *
    ***************************************************************************************/

    public class ComplexErrorProp
    {
        private static int m_monteCarloLength = 10000; // length of Monte Carlo simulation arrays
        private double m_corrCoeff; // correlation coefficient between real and imaginary parts
        private ErrorProp m_eImag = new ErrorProp(); // Imaginary part of a complex number
        private ErrorProp m_eReal = new ErrorProp(); // Real part of a complex number

/*********************************************************/

        // CONSTRUCTORS
        // default constructor - value and error of real and imag = zero
        // no correlation between real and imaginary parts
        public ComplexErrorProp()
        {
            m_eReal.reset(0.0D, 0.0D);
            m_eImag.reset(0.0D, 0.0D);
            m_corrCoeff = 0.0D;
        }

        // constructor - initialises both real and imag with ErrorProp - no correlation between real and imaginary parts
        public ComplexErrorProp(ErrorProp eReal, ErrorProp eImag)
        {
            eReal = eReal.Copy();
            eImag = eImag.Copy();
            m_corrCoeff = 0.0D;
        }

        // constructor - initialises both real and imag with ErrorProp with correlation between real and imaginary parts
        public ComplexErrorProp(ErrorProp eReal, ErrorProp eImag, double corrCoeff)
        {
            m_eReal = eReal.Copy();
            m_eImag = eImag.Copy();
            m_corrCoeff = corrCoeff;
        }

        // constructor - initialises both real and imag with doubles - no correlation between real and imaginary parts
        public ComplexErrorProp(double eRealValue, double eRealError, double eImagValue, double eImagError)
        {
            m_eReal.reset(eRealValue, eRealError);
            m_eImag.reset(eImagValue, eImagError);
            m_corrCoeff = 0.0D;
        }

        // constructor - initialises both real and imag with doubles with correlation between real and imaginary parts
        public ComplexErrorProp(double eRealValue, double eRealError, double eImagValue, double eImagError,
                                double corrCoeff)
        {
            m_eReal.reset(eRealValue, eRealError);
            m_eImag.reset(eImagValue, eImagError);
            m_corrCoeff = corrCoeff;
        }

/*********************************************************/

        // PUBLIC METHODS

        // SET VALUES
        // Set the values of real and imag - no correlation between real and imaginary
        public void reset(ErrorProp eReal, ErrorProp eImag)
        {
            eReal = eReal.Copy();
            eImag = eImag.Copy();
            m_corrCoeff = 0.0D;
        }

        // Set the values of real and imag with correlation between real and imaginary
        public void reset(ErrorProp eReal, ErrorProp eImag, double corrCoeff)
        {
            m_eReal = eReal.Copy();
            m_eImag = eImag.Copy();
            m_corrCoeff = corrCoeff;
        }

        // Set the values of real and imag - no correlation between real and imaginary
        public void reset(double eRealValue, double eRealError, double eImagValue, double eImagError)
        {
            m_eReal.setValue(eRealValue);
            m_eReal.setError(eRealError);
            m_eImag.setValue(eImagValue);
            m_eImag.setError(eImagError);
            m_corrCoeff = 0.0D;
        }


        // Set the values of real and imag with correlation between real and imaginary
        public void reset(double eRealValue, double eRealError, double eImagValue, double eImagError, double corrCoeff)
        {
            m_eReal.setValue(eRealValue);
            m_eReal.setError(eRealError);
            m_eImag.setValue(eImagValue);
            m_eImag.setError(eImagError);
            m_corrCoeff = corrCoeff;
        }

        // Set the values of magnitude and phase - no correlation between real and imaginary parts
        public void polar(ErrorProp eMag, ErrorProp ePhase)
        {
            polar(eMag, ePhase, 0.0D);
        }

        // Set the values of magnitude and phase with correlation between real and imaginary parts
        public void polar(ErrorProp eMag, ErrorProp ePhase, double corrCoeff)
        {
            // calculate values and errors
            ErrorProp a = new ErrorProp();
            a = eMag.times(ErrorProp.Cos(ePhase), corrCoeff);
            m_eReal = a;
            a = eMag.times(ErrorProp.Sin(ePhase), corrCoeff);
            m_eImag = a;

            // calculate the new correlation coefficient
            UnivNormalDist rr = new UnivNormalDist(0, 1,
                                                   new RngWrapper());
            double[,] ran = rr.correlatedGaussianArrays(eMag.getValue(), ePhase.getValue(), eMag.getError(),
                                                        ePhase.getError(), corrCoeff, m_monteCarloLength);

            double[] rV = new double[m_monteCarloLength];
            double[] iV = new double[m_monteCarloLength];
            for (int i = 0; i < m_monteCarloLength; i++)
            {
                rV[i] = ran[0, i]*Math.Cos(ran[1, i]);
                iV[i] = ran[0, i]*Math.Sin(ran[1, i]);
            }

            corrCoeff = calcRho(rV, iV);
        }

        /// calculates the correlation coefficient between x and y
        public static double calcRho(double[] x, double[] y)
        {
            int n = x.Length;
            if (n != y.Length)
            {
                throw new ArgumentException("length of x and y must be the same");
            }

            double meanX = 0.0D;
            double meanY = 0.0D;
            for (int i = 0; i < n; i++)
            {
                meanX += x[i];
                meanY += y[i];
            }
            meanX /= n;
            meanY /= n;
            double varX = 0.0D;
            double varY = 0.0D;
            double covarXY = 0.0D;
            for (int i = 0; i < n; i++)
            {
                varX += Fmath.square(x[i] - meanX);
                varY += Fmath.square(y[i] - meanY);
                covarXY += (x[i] - meanX)*(y[i] - meanY);
            }
            varX = Math.Sqrt(varX/(n - 1));
            varY = Math.Sqrt(varY/(n - 1));
            covarXY = covarXY/(n - 1);

            return covarXY/(varX*varY);
        }

        // Set the values of magnitude and phase - no correlation between real and imaginary parts
        public void polar(double eMagValue, double eMagError, double ePhaseValue, double ePhaseError)
        {
            ErrorProp eMag = new ErrorProp(eMagValue, eMagError);
            ErrorProp ePhase = new ErrorProp(ePhaseValue, ePhaseError);
            polar(eMag, ePhase, 0.0D);
        }

        // Set the values of magnitude and phase with correlation between real and imaginary parts
        public void polar(double eMagValue, double eMagError, double ePhaseValue, double ePhaseError, double corrCoeff)
        {
            ErrorProp eMag = new ErrorProp(eMagValue, eMagError);
            ErrorProp ePhase = new ErrorProp(ePhaseValue, ePhaseError);
            polar(eMag, ePhase, corrCoeff);
        }

        // Set the value of real
        public void setReal(ErrorProp eReal)
        {
            eReal = eReal.Copy();
        }

        // Set the value of real
        public void setReal(double eRealValue, double eRealError)
        {
            m_eReal.setValue(eRealValue);
            m_eReal.setError(eRealError);
        }

        // Set the value of imag
        public void setImag(ErrorProp eImag)
        {
            eImag = eImag.Copy();
        }

        // Set the value of imag
        public void setImag(double eImagValue, double eImagError)
        {
            m_eImag.setValue(eImagValue);
            m_eImag.setError(eImagError);
        }

        // Set the values of an error free double as ComplexErrorProp
        public void setDouble(double errorFree)
        {
            m_eReal.reset(errorFree, 0.0D);
            m_eImag.reset(0.0D, 0.0D);
        }

        // Set the value of the correlation coefficient between the real and imaginary parts
        public void setCorrCoeff(double corrCoeff)
        {
            m_corrCoeff = corrCoeff;
        }

        // set value of the Monte Carlo simulation array length
        public static void setMonteCarloLength(int length)
        {
            m_monteCarloLength = length;
        }


        // GET VALUES
        // Get the real part
        public ErrorProp getReal()
        {
            return m_eReal.Copy();
        }

        // Get the value of real
        public double getRealValue()
        {
            return m_eReal.getValue();
        }

        // Get the error of real
        public double getRealError()
        {
            return m_eReal.getError();
        }

        // Get the imag part
        public ErrorProp getImag()
        {
            return m_eImag.Copy();
        }

        // Get the value of imag
        public double getImagValue()
        {
            return m_eImag.getValue();
        }

        // Get the error of eImag
        public double getImagError()
        {
            return m_eImag.getError();
        }

        // Get the correlation coefficient
        public double getCorrCoeff()
        {
            return m_corrCoeff;
        }

        // Get value of the Monte Carlo simulation array length
        public static int getMonteCarloLength()
        {
            return m_monteCarloLength;
        }

        // COPY
        // Copy a single complex error prop number [static method]
        public static ComplexErrorProp Copy(ComplexErrorProp a)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                ComplexErrorProp b = new ComplexErrorProp();
                b.m_eReal = a.m_eReal.Copy();
                b.m_eImag = a.m_eImag.Copy();
                return b;
            }
        }

        // Copy a single complex error prop number [instance method]
        public ComplexErrorProp Copy()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                ComplexErrorProp b = new ComplexErrorProp();
                b.m_eReal = m_eReal.Copy();
                b.m_eImag = m_eImag.Copy();
                return b;
            }
        }

        //CLONE
        // Clone a single complex error prop number
        public Object clone()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                ComplexErrorProp b = new ComplexErrorProp();
                b.m_eReal = m_eReal.Copy();
                b.m_eImag = m_eImag.Copy();
                return b;
            }
        }

        // ADDITION
        // Add two  ComplexErrorProp numbers [static method]
        public static ComplexErrorProp plus(ComplexErrorProp a, ComplexErrorProp b)
        {
            ComplexErrorProp c = new ComplexErrorProp();
            c.m_eReal = a.m_eReal.plus(b.m_eReal);
            c.m_eImag = a.m_eImag.plus(b.m_eImag);
            return c;
        }

        //Add a  ComplexErrorProp number to this  ComplexErrorProp number [instance method]
        // this  ComplexErrorProp number remains unaltered
        public ComplexErrorProp plus(ComplexErrorProp a)
        {
            ComplexErrorProp b = new ComplexErrorProp();
            b.m_eReal = m_eReal.plus(a.m_eReal);
            b.m_eImag = m_eImag.plus(a.m_eImag);
            return b;
        }

        //  SUBTRACTION
        //Subtract two  ComplexErrorProp numbers [static method]
        public static ComplexErrorProp minus(ComplexErrorProp a, ComplexErrorProp b)
        {
            ComplexErrorProp c = new ComplexErrorProp();
            c.m_eReal = a.m_eReal.minus(b.m_eReal);
            c.m_eImag = a.m_eImag.minus(b.m_eImag);
            return c;
        }

        //Subtract a  ComplexErrorProp number from this  ComplexErrorProp number [instance method]
        // this  ComplexErrorProp number remains unaltered
        public ComplexErrorProp minus(ComplexErrorProp a)
        {
            ComplexErrorProp b = new ComplexErrorProp();
            b.m_eReal = m_eReal.minus(a.m_eReal);
            b.m_eImag = m_eImag.minus(a.m_eImag);
            return b;
        }

        // MULTIPLICATION
        //Multiply two  ComplexErrorProp numbers [static method]
        public static ComplexErrorProp times(ComplexErrorProp a, ComplexErrorProp b)
        {
            ComplexErrorProp c = new ComplexErrorProp();
            c.m_eReal = a.m_eReal.times(b.m_eReal).minus(a.m_eImag.times(b.m_eImag));
            c.m_eImag = a.m_eReal.times(b.m_eImag).plus(a.m_eImag.times(b.m_eReal));
            return c;
        }

        //Multiply two  ComplexErrorProp numbers [instance method]
        public ComplexErrorProp times(ComplexErrorProp b)
        {
            ComplexErrorProp c = new ComplexErrorProp();
            c.m_eReal = m_eReal.times(b.m_eReal).minus(m_eImag.times(b.m_eImag));
            c.m_eImag = m_eReal.times(b.m_eImag).plus(m_eImag.times(b.m_eReal));
            return c;
        }

        //Multiply this ComplexErrorProp number by a ComplexErrorProp number and replace this by the product
        public void timesEquals(ComplexErrorProp a)
        {
            ComplexErrorProp b = new ComplexErrorProp();
            b.m_eReal = a.m_eReal.times(m_eReal).minus(a.m_eImag.times(m_eImag));
            b.m_eImag = a.m_eReal.times(m_eImag).plus(a.m_eImag.times(m_eReal));
            m_eReal = b.m_eReal.Copy();
            m_eImag = b.m_eImag.Copy();
        }


        // DIVISION
        //Division of two ComplexErrorProp numbers a/b [static method]
        public static ComplexErrorProp over(ComplexErrorProp a, ComplexErrorProp b)
        {
            ComplexErrorProp c = new ComplexErrorProp();
            double[] aReal = UnivNormalDist.NextDoubleArr(a.m_eReal.getValue(), a.m_eReal.getError(), m_monteCarloLength);
            double[] aImag = UnivNormalDist.NextDoubleArr(a.m_eImag.getValue(), a.m_eImag.getError(), m_monteCarloLength);
            double[] bReal = UnivNormalDist.NextDoubleArr(b.m_eReal.getValue(), b.m_eReal.getError(), m_monteCarloLength);
            double[] bImag = UnivNormalDist.NextDoubleArr(b.m_eImag.getValue(), b.m_eImag.getError(), m_monteCarloLength);
            double[] rat = new double[m_monteCarloLength];
            double[] denm = new double[m_monteCarloLength];
            double[] cReal = new double[m_monteCarloLength];
            double[] cImag = new double[m_monteCarloLength];

            for (int i = 0; i < m_monteCarloLength; i++)
            {
                if (Math.Abs(bReal[i]) >= Math.Abs(bImag[i]))
                {
                    rat[i] = bImag[i]/bReal[i];
                    denm[i] = bReal[i] + bImag[i]*rat[i];
                    cReal[i] = (aReal[i] + aImag[i]*rat[i])/denm[i];
                    cImag[i] = (aImag[i] - aReal[i]*rat[i])/denm[i];
                }
                else
                {
                    rat[i] = bReal[i]/bImag[i];
                    denm[i] = bReal[i]*rat[i] + bImag[i];
                    cReal[i] = (aReal[i]*rat[i] + aImag[i])/denm[i];
                    cImag[i] = (aImag[i]*rat[i] - aReal[i])/denm[i];
                }
            }
            double cRealSum = 0.0D;
            double cImagSum = 0.0D;
            double cRealErrorSum = 0.0D;
            double cImagErrorSum = 0.0D;
            for (int i = 0; i < m_monteCarloLength; i++)
            {
                cRealSum += cReal[i];
                cImagSum += cImag[i];
            }
            cRealSum /= m_monteCarloLength;
            cImagSum /= m_monteCarloLength;
            for (int i = 0; i < m_monteCarloLength; i++)
            {
                cRealErrorSum += Fmath.square(cRealSum - cReal[i]);
                cImagErrorSum += Fmath.square(cImagSum - cImag[i]);
            }
            cRealErrorSum = Math.Sqrt(cRealErrorSum/(m_monteCarloLength - 1));
            cImagErrorSum = Math.Sqrt(cImagErrorSum/(m_monteCarloLength - 1));
            c.m_eReal.setError(cRealErrorSum);
            c.m_eImag.setError(cImagErrorSum);

            double denom = 0.0D;
            double ratio = 0.0D;
            if (Math.Abs(b.m_eReal.getValue()) >= Math.Abs(b.m_eImag.getValue()))
            {
                ratio = b.m_eImag.getValue()/b.m_eReal.getValue();
                denom = b.m_eReal.getValue() + b.m_eImag.getValue()*ratio;
                c.m_eReal.setValue((a.m_eReal.getValue() + a.m_eImag.getValue()*ratio)/denom);
                c.m_eImag.setValue((a.m_eImag.getValue() - a.m_eReal.getValue()*ratio)/denom);
            }
            else
            {
                ratio = b.m_eReal.getValue()/b.m_eImag.getValue();
                denom = b.m_eReal.getValue()*ratio + b.m_eImag.getValue();
                c.m_eReal.setValue((a.m_eReal.getValue()*ratio + a.m_eImag.getValue())/denom);
                c.m_eImag.setValue((a.m_eImag.getValue()*ratio - a.m_eReal.getValue())/denom);
            }
            return c;
        }

        //Division of this ComplexErrorProp number by a ComplexErrorProp number [instance method]
        // this ComplexErrorProp number remains unaltered
        public ComplexErrorProp over(ComplexErrorProp b)
        {
            ComplexErrorProp c = new ComplexErrorProp();
            double[] aReal = UnivNormalDist.NextDoubleArr(m_eReal.getValue(), m_eReal.getError(), m_monteCarloLength);
            double[] aImag = UnivNormalDist.NextDoubleArr(m_eImag.getValue(), m_eImag.getError(), m_monteCarloLength);
            double[] bReal = UnivNormalDist.NextDoubleArr(b.m_eReal.getValue(), b.m_eReal.getError(), m_monteCarloLength);
            double[] bImag = UnivNormalDist.NextDoubleArr(b.m_eImag.getValue(), b.m_eImag.getError(), m_monteCarloLength);
            double[] rat = new double[m_monteCarloLength];
            double[] denm = new double[m_monteCarloLength];
            double[] cReal = new double[m_monteCarloLength];
            double[] cImag = new double[m_monteCarloLength];

            for (int i = 0; i < m_monteCarloLength; i++)
            {
                if (Math.Abs(bReal[i]) >= Math.Abs(bImag[i]))
                {
                    rat[i] = bImag[i]/bReal[i];
                    denm[i] = bReal[i] + bImag[i]*rat[i];
                    cReal[i] = (aReal[i] + aImag[i]*rat[i])/denm[i];
                    cImag[i] = (aImag[i] - aReal[i]*rat[i])/denm[i];
                }
                else
                {
                    rat[i] = bReal[i]/bImag[i];
                    denm[i] = bReal[i]*rat[i] + bImag[i];
                    cReal[i] = (aReal[i]*rat[i] + aImag[i])/denm[i];
                    cImag[i] = (aImag[i]*rat[i] - aReal[i])/denm[i];
                }
            }
            double cRealSum = 0.0D;
            double cImagSum = 0.0D;
            double cRealErrorSum = 0.0D;
            double cImagErrorSum = 0.0D;
            for (int i = 0; i < m_monteCarloLength; i++)
            {
                cRealSum += cReal[i];
                cImagSum += cImag[i];
            }
            cRealSum /= m_monteCarloLength;
            cImagSum /= m_monteCarloLength;
            for (int i = 0; i < m_monteCarloLength; i++)
            {
                cRealErrorSum += Fmath.square(cRealSum - cReal[i]);
                cImagErrorSum += Fmath.square(cImagSum - cImag[i]);
            }
            cRealErrorSum = Math.Sqrt(cRealErrorSum/(m_monteCarloLength - 1));
            cImagErrorSum = Math.Sqrt(cImagErrorSum/(m_monteCarloLength - 1));
            c.m_eReal.setError(cRealErrorSum);
            c.m_eImag.setError(cImagErrorSum);

            double denom = 0.0D;
            double ratio = 0.0D;
            if (Math.Abs(b.m_eReal.getValue()) >= Math.Abs(b.m_eImag.getValue()))
            {
                ratio = b.m_eImag.getValue()/b.m_eReal.getValue();
                denom = b.m_eReal.getValue() + b.m_eImag.getValue()*ratio;
                c.m_eReal.setValue((m_eReal.getValue() + m_eImag.getValue()*ratio)/denom);
                c.m_eImag.setValue((m_eImag.getValue() - m_eReal.getValue()*ratio)/denom);
            }
            else
            {
                ratio = b.m_eReal.getValue()/b.m_eImag.getValue();
                denom = b.m_eReal.getValue()*ratio + b.m_eImag.getValue();
                c.m_eReal.setValue((m_eReal.getValue()*ratio + m_eImag.getValue())/denom);
                c.m_eImag.setValue((m_eImag.getValue()*ratio - m_eReal.getValue())/denom);
            }
            return c;
        }

        //Exponential [static method]
        public static ComplexErrorProp Exp(ComplexErrorProp aa)
        {
            ComplexErrorProp bb = new ComplexErrorProp();
            ErrorProp pre = ErrorProp.Exp(aa.m_eReal);
            bb.m_eReal = pre.times(ErrorProp.Cos(aa.m_eImag), aa.m_corrCoeff);
            bb.m_eImag = pre.times(ErrorProp.Sin(aa.m_eImag), aa.m_corrCoeff);
            return bb;
        }

        //Exponential [instance method]
        public ComplexErrorProp Exp()
        {
            ComplexErrorProp bb = new ComplexErrorProp();
            ErrorProp pre = ErrorProp.Exp(m_eReal);
            bb.m_eReal = pre.times(ErrorProp.Cos(m_eImag), m_corrCoeff);
            bb.m_eImag = pre.times(ErrorProp.Sin(m_eImag), m_corrCoeff);
            return bb;
        }

        //Absolute value (modulus) [static method]
        public static ErrorProp abs(ComplexErrorProp aa)
        {
            ErrorProp bb = new ErrorProp();
            double realV = aa.m_eReal.getValue();
            double imagV = aa.m_eImag.getValue();

            double rmod = Math.Abs(realV);
            double imod = Math.Abs(imagV);
            double ratio = 0.0D;
            double res = 0.0D;

            if (rmod == 0.0)
            {
                res = imod;
            }
            else
            {
                if (imod == 0.0)
                {
                    res = rmod;
                }
                if (rmod >= imod)
                {
                    ratio = imagV/realV;
                    res = rmod*Math.Sqrt(1.0 + ratio*ratio);
                }
                else
                {
                    ratio = realV/imagV;
                    res = imod*Math.Sqrt(1.0 + ratio*ratio);
                }
            }
            bb.setValue(res);

            double realE = aa.m_eReal.getError();
            double imagE = aa.m_eImag.getError();
            res = hypotWithRho(2.0D*realE*realV, 2.0D*imagE*imagV, aa.m_corrCoeff);
            bb.setError(res);

            return bb;
        }

        //Absolute value (modulus) [instance method]
        public ErrorProp abs()
        {
            ErrorProp aa = new ErrorProp();
            double realV = m_eReal.getValue();
            double imagV = m_eImag.getValue();

            double rmod = Math.Abs(realV);
            double imod = Math.Abs(imagV);
            double ratio = 0.0D;
            double res = 0.0D;

            if (rmod == 0.0)
            {
                res = imod;
            }
            else
            {
                if (imod == 0.0)
                {
                    res = rmod;
                }
                if (rmod >= imod)
                {
                    ratio = imagV/realV;
                    res = rmod*Math.Sqrt(1.0 + ratio*ratio);
                }
                else
                {
                    ratio = realV/imagV;
                    res = imod*Math.Sqrt(1.0 + ratio*ratio);
                }
            }
            aa.setValue(res);

            double realE = m_eReal.getError();
            double imagE = m_eImag.getError();
            res = hypotWithRho(2.0D*realE*realV, 2.0D*imagE*imagV, m_corrCoeff);
            aa.setError(res);

            return aa;
        }

        //Argument of a ComplexErrorProp  [static method]
        public static ErrorProp arg(ComplexErrorProp a)
        {
            ErrorProp b = new ErrorProp();
            b = ErrorProp.Atan2(a.m_eReal, a.m_eImag, a.m_corrCoeff);
            return b;
        }

        //Argument of a ComplexErrorProp  [instance method]
        public ErrorProp arg(double rho)
        {
            ErrorProp a = new ErrorProp();
            a = ErrorProp.Atan2(m_eReal, m_eImag, m_corrCoeff);
            return a;
        }

        // Returns sqrt(a*a+b*b + 2*a*b*rho) [without unecessary overflow or underflow]
        public static double hypotWithRho(double aa, double bb, double rho)
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
                        cc = amod*Math.Sqrt(1.0 + ratio*ratio + 2.0*rho*ratio);
                    }
                    else
                    {
                        ratio = amod/bmod;
                        cc = bmod*Math.Sqrt(1.0 + ratio*ratio + 2.0*rho*ratio);
                    }
                }
            }
            return cc;
        }

        // TRUNCATION
        // Rounds the mantissae of both the value and error parts of Errorprop to prec places
        public static ComplexErrorProp truncate(ComplexErrorProp x, int prec)
        {
            if (prec < 0)
            {
                return x;
            }

            double rV = x.m_eReal.getValue();
            double rE = x.m_eReal.getError();
            double iV = x.m_eImag.getValue();
            double iE = x.m_eImag.getError();
            ComplexErrorProp y = new ComplexErrorProp();

            rV = Fmath.truncate(rV, prec);
            rE = Fmath.truncate(rE, prec);
            iV = Fmath.truncate(iV, prec);
            iE = Fmath.truncate(iE, prec);

            y.reset(rV, rE, iV, iE);
            return y;
        }

        // instance method
        public ComplexErrorProp truncate(int prec)
        {
            if (prec < 0)
            {
                return this;
            }

            double rV = m_eReal.getValue();
            double rE = m_eReal.getError();
            double iV = m_eImag.getValue();
            double iE = m_eImag.getError();

            ComplexErrorProp y = new ComplexErrorProp();

            rV = Fmath.truncate(rV, prec);
            rE = Fmath.truncate(rE, prec);
            iV = Fmath.truncate(iV, prec);
            iE = Fmath.truncate(iE, prec);

            y.reset(rV, rE, iV, iE);
            return y;
        }

        // CONVERSIONS
        // Format a ComplexErrorProp number as a string
        // Overides java.lang.string.ToString()
        public override string ToString()
        {
            return "Real part: " + m_eReal.getValue() + ", error = " + m_eReal.getError() + "; Imaginary part: " +
                   m_eImag.getValue() + ", error = " + m_eImag.getError();
        }

        // Format a ComplexErrorProp number as a string
        // See static method above for comments
        public static string ToString(ComplexErrorProp aa)
        {
            return "Real part: " + aa.m_eReal.getValue() + ", error = " + aa.m_eReal.getError() + "; Imaginary part: " +
                   aa.m_eImag.getValue() + ", error = " + aa.m_eImag.getError();
        }

        //PRINT AN COMPLEX ERROR NUMBER
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
            PrintToScreen.Write(message + " " + ToString());
        }

        // Print to terminal window without text (message) and without line return
        public void print()
        {
            PrintToScreen.Write(" " + ToString());
        }
    }
}
