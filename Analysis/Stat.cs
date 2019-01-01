#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Complex;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Mathematics.Functions.Beta;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Mathematics.Roots;
using HC.Analytics.Probability.Distributions.Continuous;
using HC.Analytics.Probability.Distributions.Continuous.NormalDist;
using HC.Analytics.Probability.Random;
using HC.Analytics.Statistics;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Analysis
{
    /*
    *   Class   Stat
    *
    *   USAGE:  Statistical functions
    *
    *   WRITTEN BY: Dr Michael Thomas Flanagan
    *
    *   DATE:    June 2002 as part of Fmath
    *   AMENDED: 12 May 2003 Statistics separated out from Fmath as a new class
    *   DATE:    18 June 2005, 5 January 2006, 25 April 2006, 12, 21 November 2006
    *            4 December 2006 (renaming of m_cfd and pdf methods - older version also retained)
    *            31 December 2006, March 2007, 14 April 2007, 19 October 2007, 27 February 2008
    *            29 March 2008, 7 April 2008, 29 April 2008 - 13 May 2008, 22-31 May 2008,
    *            4-10 June 2008, 27 June 2008, 2-5 July 2008, 23 July 2008, 31 July 2008,
    *            2-4 August 2008,  20 August 2008, 5-10 September 2008, 19 September 2008,
    *            28-30 September 2008 (probability Plot moved to separate class, ProbabilityPlot)
    *            4-5 October 2008,  8-13 December 2008, 14 June 2009, 13-23 October 2009, 8 February 2010
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web page:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/Stat.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    *   Copyright (c) 2002 - 2009 Michael Thomas Flanagan
    *
    *   PERMISSION TO COPY:
    *
    *   Permission to use, copy and modify this software and its documentation for NON-COMMERCIAL purposes is granted, without fee,
    *   provided that an acknowledgement to the author, Dr Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all copies
    *   and associated documentation or publications.
    *
    *   Redistributions of the source code of this source code, or parts of the source codes, must retain the above copyright notice,
    +   this list of conditions and the following disclaimer and requires written permission from the Michael Thomas Flanagan:
    *
    *   Redistribution in binary form of all or parts of this class must reproduce the above copyright notice, this list of conditions and
    *   the following disclaimer in the documentation and/or other materials provided with the distribution and requires written permission
    *   from the Michael Thomas Flanagan:
    *
    *   Dr Michael Thomas Flanagan makes no representations about the suitability or fitness of the software for any or for a particular purpose.
    *   Dr Michael Thomas Flanagan shall not be liable for any damages suffered as a result of using, modifying or distributing this software
    *   or its derivatives.
    *
    ***************************************************************************************/

    public class Stat : ArrayMaths
    {
        // INSTANCE VARIABLES

        private static readonly double[] lgfCoeff = {
                                                        1.000000000190015, 76.18009172947146, -86.50532032941677,
                                                        24.01409824083091, -1.231739572450155, 0.1208650973866179E-2,
                                                        -0.5395239384953E-5
                                                    };

        public static double FPMIN = 1e-300;
        private static double histTol = 1.0001D;
        //private static double igfeps = 1e-8;
        //private static int igfiter = 1000;
        private static double lgfGamma = 5.0;
        private static int lgfN = 6;
        private static bool nEffOptionS = true; // = true  n replaced by effective sample number
        private static bool nFactorOptionS; // = true  varaiance, covariance and standard deviation denominator = n
        private static bool weightingOptionS = true; // = true  'little w' weights (uncertainties) used
        private ArrayMaths amWeights; // weights as ArrayMaths
        private bool lowerDone; // = true when lower oulier search completed even if no upper outliers found
        private List<Object> lowerOutlierDetails = new List<Object>(); // lower outlier search details

        private bool nEffOptionI = true; // = true  n replaced by effective sample number
        // = false n used as sample number
        private bool nEffReset; // = true when instance method resetting the nEff choice called
        private bool nFactorOptionI; // = true  varaiance, covariance and standard deviation denominator = n
        // = false varaiance, covariance and standard deviation denominator = n-1
        private bool nFactorReset; // = true when instance method resetting the denominator is called
        private bool upperDone; // = true when upper oulier search ct[])
        private List<Object> upperOutlierDetails = new List<Object>(); // upper outlier search details

        private bool weightingOptionI = true; // = true  'little w' weights (uncertainties) used
        // = false 'big W' weights (multiplicative factors) used
        private bool weightingReset; // = true when instance method resetting the nEff choice called

        private bool weightsSupplied; // = true if weights entered

        // CONSTRUCTORS
        public Stat()
        {
        }

        public Stat(double[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(float[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(long[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(int[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(short[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(byte[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(decimal[] xx)
            : base(xx)
        {
        }

        public Stat(ComplexClass[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(string[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(Object[] xx)
            : base(xx)
        {
            convertToHighest();
        }

        public Stat(List<Object> xx)
            : base(xx)
        {
            convertToHighest();
        }

        // Convert array to double if not ComplexClass, ,  decimal or long
        // Convert to decimal if long
        public new void convertToHighest()
        {
            switch (type)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 18:
                    double[] dd = getArray_as_double();
                    m_array.Clear();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(dd[i]);
                    }
                    double[] ww = new double[intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        ww[i] = 1.0D;
                    }
                    amWeights = new ArrayMaths(ww);
                    type = 1;
                    break;
                case 12:
                case 13:
                    decimal[] bd = getArray_as_decimal();
                    m_array.Clear();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(bd[i]);
                    }
                    decimal[] wd = new decimal[intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        wd[i] = decimal.One;
                    }
                    amWeights = new ArrayMaths(wd);
                    type = 12;
                    bd = null;
                    break;
                case 14:
                case 15:
                    ComplexClass[] cc = getArray_as_Complex();
                    m_array.Clear();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(cc[i]);
                    }
                    ComplexClass[] wc = new ComplexClass[intLength];
                    for (int i = 0; i < intLength; i++)
                    {
                        wc[i] = ComplexClass.plusOne();
                    }
                    amWeights = new ArrayMaths(wc);
                    type = 14;
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
        }


        // INSTANCE METHODS
        // Set weights to 'big W' - multiplicative factor
        public void setWeightsToBigW()
        {
            weightingOptionI = false;
            weightingReset = true;
        }

        // Set weights to 'little w' - uncertainties
        public void setWeightsToLittleW()
        {
            weightingOptionI = true;
            weightingReset = true;
        }

        // Set standard deviation, variance and covariance denominators to n
        public void setDenominatorToN()
        {
            nFactorOptionI = true;
            nFactorReset = true;
        }

        // Set standard deviation, variance and covariance denominators to n-1
        public void setDenominatorToNminusOne()
        {
            nFactorOptionI = false;
            nFactorReset = true;
        }

        // Repalce number of data points to the effective sample number in weighted calculations
        public void useEffectiveN()
        {
            nEffOptionI = true;
            nEffReset = true;
        }

        // Repalce the effective sample number in weighted calculations by the number of data points
        public void useTrueN()
        {
            nEffOptionI = false;
            nEffReset = true;
        }

        // Return the effective sample number
        public double effectiveSampleNumber()
        {
            return effectiveSampleNumber_as_double();
        }

        public double effectiveSampleNumber_as_double()
        {
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double nEff = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    nEff = effectiveSampleNumber(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    nEff =  (double)effectiveSampleNumber(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            weightingOptionS = holdW;
            return nEff;
        }

        public decimal effectiveSampleNumber_as_decimal()
        {
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            decimal nEff = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    nEff = effectiveSampleNumber(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            weightingOptionS = holdW;
            return nEff;
        }

        public ComplexClass effectiveSampleNumber_as_Complex()
        {
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            ComplexClass nEff = ComplexClass.zero();
            switch (type)
            {
                case 1:
                case 12:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    nEff = effectiveSampleNumber(cc);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            weightingOptionS = holdW;
            return nEff;
        }

        // Return the true sample number
        public int trueSampleNumber()
        {
            return intLength;
        }

        public int trueSampleNumber_as_int()
        {
            return intLength;
        }

        public double trueSampleNumber_as_double()
        {
            return intLength;
        }

        public decimal trueSampleNumber_as_decimal()
        {
            return intLength;
        }

        public ComplexClass trueSampleNumber_as_Complex()
        {
            return new ComplexClass(intLength, 0.0);
        }


        // CONVERSION OF WEIGHTING FACTORS (INSTANCE)
        // Converts weighting facors Wi to wi, i.e. to 1/sqrt(Wi)
        // DEPRECATED !!!
        public void convertBigWtoLittleW()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("convertBigWtoLittleW: no weights have been supplied - all weights set to unity");
            }
            else
            {
                amWeights = amWeights.oneOverSqrt();
            }
        }

        // ENTER AN ARRAY OF WEIGHTS
        public void setWeights(double[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(float[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(long[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(int[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(short[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(byte[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(decimal[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(ComplexClass[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(Object[] xx)
        {
            if (intLength != xx.Length)
            {
                throw new ArgumentException("Length of weights array, " + xx.Length +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        public void setWeights(List<Object> xx)
        {
            if (intLength != xx.Count)
            {
                throw new ArgumentException("Length of weights array, " + xx.Count +
                                            ", must be the same as the Length of the instance internal array, " +
                                            intLength);
            }
            ArrayMaths wm = new ArrayMaths(xx);
            convertWeights(wm);
            weightsSupplied = true;
        }

        private void convertWeights(ArrayMaths wm)
        {
            switch (type)
            {
                case 1:
                    switch (wm.typeIndex())
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            double[] w1 = wm.getArray_as_double();
                            amWeights = new ArrayMaths(w1);
                            break;
                        case 12:
                        case 13:
                            decimal[] a2 = getArray_as_decimal();
                            for (int i = 0; i < intLength; i++)
                            {
                                m_array.Add(a2[i]);
                            }
                            decimal[] w2 = wm.getArray_as_decimal();
                            amWeights = new ArrayMaths(w2);
                            a2 = null;
                            w2 = null;
                            break;
                        case 14:
                        case 15:
                            ComplexClass[] a3 = getArray_as_Complex();
                            for (int i = 0; i < intLength; i++)
                            {
                                m_array.Add(a3[i]);
                            }
                            ComplexClass[] w3 = wm.getArray_as_Complex();
                            amWeights = new ArrayMaths(w3);
                            break;
                        default:
                            throw new ArgumentException("This type number, " + type +
                                                        ", should not be possible here!!!!");
                    }
                    break;
                case 12:
                    switch (wm.typeIndex())
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            decimal[] w4 = wm.getArray_as_decimal();
                            amWeights = new ArrayMaths(w4);
                            w4 = null;
                            break;
                        case 12:
                        case 13:
                            decimal[] w5 = wm.getArray_as_decimal();
                            amWeights = new ArrayMaths(w5);
                            w5 = null;
                            break;
                        case 14:
                        case 15:
                            ComplexClass[] a6 = getArray_as_Complex();
                            for (int i = 0; i < intLength; i++)
                            {
                                m_array.Add(a6[i]);
                            }
                            ComplexClass[] w6 = wm.getArray_as_Complex();
                            amWeights = new ArrayMaths(w6);
                            break;
                        default:
                            throw new ArgumentException("This type number, " + type +
                                                        ", should not be possible here!!!!");
                    }
                    break;
                case 14:
                    ComplexClass[] a7 = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        m_array.Add(a7[i]);
                    }
                    ComplexClass[] w7 = wm.getArray_as_Complex();
                    amWeights = new ArrayMaths(w7);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
        }

        // ARITMETIC MEANS (INSTANCE)
        public double mean()
        {
            return mean_as_double();
        }

        public double mean_as_double()
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    for (int i = 0; i < intLength; i++)
                    {
                        mean += dd[i];
                    }
                    mean /= intLength;
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal meanbd = decimal.Zero;
                    for (int i = 0; i < intLength; i++)
                    {
                        meanbd = meanbd + bd[i];
                    }
                    meanbd = meanbd/intLength;
                    mean = (double)meanbd;
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public decimal mean_as_decimal()
        {
            decimal mean = decimal.Zero;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    double meand = 0.0D;
                    for (int i = 0; i < intLength; i++)
                    {
                        meand += dd[i];
                    }
                    meand /= intLength;
                    mean =  (decimal)(meand);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    for (int i = 0; i < intLength; i++)
                    {
                        mean = mean + bd[i];
                    }
                    mean = mean/intLength;
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public ComplexClass mean_as_Complex()
        {
            ComplexClass mean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    double meand = 0.0D;
                    for (int i = 0; i < intLength; i++)
                    {
                        meand += dd[i];
                    }
                    meand /= intLength;
                    mean = new ComplexClass(meand);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal meanbd = decimal.Zero;
                    for (int i = 0; i < intLength; i++)
                    {
                        meanbd = meanbd + bd[i];
                    }
                    meanbd = meanbd/intLength;
                    mean = new ComplexClass( meanbd);
                    bd = null;

                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        mean = mean.plus(cc[i]);
                    }
                    mean = mean.over(new ComplexClass( intLength));
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }


        // WEIGHTED ARITMETIC MEANS (INSTANCE)
        public double weightedMean()
        {
            return weightedMean_as_double();
        }

        public double weightedMean_as_double()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedMean_as_double: no weights supplied - unweighted m_mean returned");
                return mean_as_double();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double mean = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] wwd = amWeights.getArray_as_double();
                        mean = Stat.mean(dd, wwd);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wwb = amWeights.getArray_as_decimal();
                        mean =  (double)(Stat.mean(bd, wwb));
                        bd = null;
                        wwb = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public decimal weightedMean_as_decimal()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedMean_as_decimal: no weights supplied - unweighted m_mean returned");
                return mean_as_decimal();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                decimal mean = decimal.Zero;
                switch (type)
                {
                    case 1:
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wwb = amWeights.getArray_as_decimal();
                        mean = Stat.mean(bd, wwb);
                        bd = null;
                        wwb = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to decimal");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public ComplexClass weightedMean_as_Complex()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedMean_as_Complex: no weights supplied - unweighted m_mean returned");
                return mean_as_Complex();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                ComplexClass mean = ComplexClass.zero();
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] wwd = amWeights.getArray_as_double();
                        mean = new ComplexClass(Stat.mean(dd, wwd));
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wwb = amWeights.getArray_as_decimal();
                        mean = new ComplexClass((Stat.mean(bd, wwb)));
                        bd = null;
                        wwb = null;
                        break;
                    case 14:
                        ComplexClass[] cc = getArray_as_Complex();
                        ComplexClass[] wwc = amWeights.getArray_as_Complex();
                        mean = Stat.mean(cc, wwc);
                        break;
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        // SUBTRACT AN ARITMETIC MEAN FROM AN ARRAY (INSTANCE)
        public double[] subtractMean()
        {
            return subtractMean_as_double();
        }

        public double[] subtractMean_as_double()
        {
            double[] arrayminus = new double[intLength];
            switch (type)
            {
                case 1:
                    double meand = mean_as_double();
                    ArrayMaths amd = minus(meand);
                    arrayminus = amd.getArray_as_double();
                    break;
                case 12:
                    decimal meanb = mean_as_decimal();
                    ArrayMaths amb = minus(meanb);
                    arrayminus = amb.getArray_as_double();
                    meanb = 0;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return arrayminus;
        }

        public decimal[] subtractMean_as_decimal()
        {
            decimal[] arrayminus = new decimal[intLength];
            switch (type)
            {
                case 1:
                case 12:
                    decimal meanb = mean_as_decimal();
                    ArrayMaths amb = minus(meanb);
                    arrayminus = amb.getArray_as_decimal();
                    meanb = 0;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return arrayminus;
        }

        public ComplexClass[] subtractMean_as_Complex()
        {
            ComplexClass[] arrayminus = new ComplexClass[intLength];
            switch (type)
            {
                case 1:
                    double meand = mean_as_double();
                    ArrayMaths amd = minus(meand);
                    arrayminus = amd.getArray_as_Complex();
                    break;
                case 12:
                    decimal meanb = mean_as_decimal();
                    ArrayMaths amb = minus(meanb);
                    arrayminus = amb.getArray_as_Complex();
                    meanb = 0;
                    break;
                case 14:
                    ComplexClass meanc = mean_as_Complex();
                    ArrayMaths amc = minus(meanc);
                    arrayminus = amc.getArray_as_Complex();
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return arrayminus;
        }

        // SUBTRACT AN WEIGHTED ARITMETIC MEAN FROM AN ARRAY (INSTANCE)
        public double[] subtractWeightedMean()
        {
            return subtractWeightedMean_as_double();
        }

        public double[] subtractWeightedMean_as_double()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "subtractWeightedMean_as_double: no weights supplied - unweighted values returned");
                return subtractMean_as_double();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double[] arrayminus = new double[intLength];
                switch (type)
                {
                    case 1:
                        double meand = weightedMean_as_double();
                        ArrayMaths amd = minus(meand);
                        arrayminus = amd.getArray_as_double();
                        break;
                    case 12:
                        decimal meanb = weightedMean_as_decimal();
                        ArrayMaths amb = minus(meanb);
                        arrayminus = amb.getArray_as_double();
                        meanb = 0;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return arrayminus;
            }
        }

        public decimal[] subtractWeightedMean_as_decimal()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "subtractWeightedMean_as_decimal: no weights supplied - unweighted values returned");
                return subtractMean_as_decimal();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                decimal[] arrayminus = new decimal[intLength];
                switch (type)
                {
                    case 1:
                    case 12:
                        decimal meanb = weightedMean_as_decimal();
                        ArrayMaths amb = minus(meanb);
                        arrayminus = amb.getArray_as_decimal();
                        meanb = 0;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to decimal");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return arrayminus;
            }
        }

        public ComplexClass[] subtractWeightedMean_as_Complex()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "subtractWeightedMean_as_Complex: no weights supplied - unweighted values returned");
                return subtractMean_as_Complex();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                ComplexClass[] arrayminus = new ComplexClass[intLength];
                switch (type)
                {
                    case 1:
                        double meand = weightedMean_as_double();
                        ArrayMaths amd = minus(meand);
                        arrayminus = amd.getArray_as_Complex();
                        break;
                    case 12:
                        decimal meanb = weightedMean_as_decimal();
                        ArrayMaths amb = minus(meanb);
                        arrayminus = amb.getArray_as_Complex();
                        meanb = 0;
                        break;
                    case 14:
                        ComplexClass meanc = weightedMean_as_Complex();
                        ArrayMaths amc = minus(meanc);
                        arrayminus = amc.getArray_as_Complex();
                        break;
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return arrayminus;
            }
        }


        // GEOMETRIC MEAN(INSTANCE)
        public double geometricMean()
        {
            return geometricMean_as_double();
        }

        public double geometricMean_as_double()
        {
            double gmean = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    gmean = geometricMean(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    gmean = geometricMean(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot  be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return gmean;
        }

        public ComplexClass geometricMean_as_Complex()
        {
            ComplexClass gmean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                case 12:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    gmean = geometricMean(cc);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return gmean;
        }


        // WEIGHTED GEOMETRIC MEAN(INSTANCE)
        public double weightedGeometricMean()
        {
            return weightedGeometricMean_as_double();
        }

        public double weightedGeometricMean_as_double()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedGeometricMean_as_double: no weights supplied - unweighted value returned");
                return geometricMean_as_double();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double gmean = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = getArray_as_double();
                        gmean = geometricMean(dd, ww);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = getArray_as_decimal();
                        gmean = geometricMean(bd, wd);
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot  be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return gmean;
            }
        }

        public ComplexClass weightedGeometricMean_as_Complex()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedGeometricMean_as_Complex: no weights supplied - unweighted value returned");
                return geometricMean_as_Complex();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                ComplexClass gmean = ComplexClass.zero();
                switch (type)
                {
                    case 1:
                    case 12:
                    case 14:
                        ComplexClass[] cc = getArray_as_Complex();
                        ComplexClass[] ww = getArray_as_Complex();
                        gmean = geometricMean(cc, ww);
                        break;
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return gmean;
            }
        }

        // HARMONIC MEANS (INSTANCE)
        public double harmonicMean()
        {
            return harmonicMean_as_double();
        }

        public double harmonicMean_as_double()
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = harmonicMean(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean =  (double)(harmonicMean(bd));
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public decimal harmonicMean_as_decimal()
        {
            decimal mean = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = harmonicMean(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public ComplexClass harmonicMean_as_Complex()
        {
            ComplexClass mean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = new ComplexClass(harmonicMean(dd));
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = new ComplexClass((harmonicMean(bd)));
                    bd = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    mean = harmonicMean(cc);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        // WEIGHTED HARMONIC MEANS (INSTANCE)
        public double weightedHarmonicMean()
        {
            return weightedHarmonicMean_as_double();
        }

        public double weightedHarmonicMean_as_double()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedHarmonicMean_as_double: no weights supplied - unweighted m_mean returned");
                return harmonicMean_as_double();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double mean = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] wwd = amWeights.getArray_as_double();
                        mean = harmonicMean(dd, wwd);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wwb = amWeights.getArray_as_decimal();
                        mean =  (double)(harmonicMean(bd, wwb));
                        bd = null;
                        wwb = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public decimal weightedHarmonicMean_as_decimal()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedHarmonicMean_as_decimal: no weights supplied - unweighted m_mean returned");
                return harmonicMean_as_decimal();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                decimal mean = decimal.Zero;
                switch (type)
                {
                    case 1:
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wwb = amWeights.getArray_as_decimal();
                        mean = harmonicMean(bd, wwb);
                        bd = null;
                        wwb = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to decimal");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public ComplexClass weightedHarmonicMean_as_Complex()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedHarmonicMean_as_Complex: no weights supplied - unweighted m_mean returned");
                return harmonicMean_as_Complex();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                ComplexClass mean = ComplexClass.zero();
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] wwd = amWeights.getArray_as_double();
                        mean = new ComplexClass(harmonicMean(dd, wwd));
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wwb = amWeights.getArray_as_decimal();
                        mean = new ComplexClass((harmonicMean(bd, wwb)));
                        bd = null;
                        wwb = null;
                        break;
                    case 14:
                        ComplexClass[] cc = getArray_as_Complex();
                        ComplexClass[] wwc = amWeights.getArray_as_Complex();
                        mean = harmonicMean(cc, wwc);
                        break;
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        // GENERALIZED MEANS [POWER MEANS](INSTANCE)
        public double generalizedMean(double m)
        {
            return generalizedMean_as_double(m);
        }

        public double generalizedMean_as_double(double m)
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = generalizedMean(dd, m);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = generalizedMean(bd, m);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public double generalizedMean(decimal m)
        {
            return generalizedMean_as_double(m);
        }

        public double generalizedMean_as_double(decimal m)
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = generalizedMean(bd, m);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public ComplexClass generalizedMean_as_Complex(double m)
        {
            ComplexClass mean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = new ComplexClass(generalizedMean(dd, m));
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = new ComplexClass(generalizedMean(bd, m));
                    bd = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    mean = generalizedMean(cc, m);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public ComplexClass generalizedMean_as_Complex(ComplexClass m)
        {
            ComplexClass mean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                case 12:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    mean = generalizedMean(cc, m);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public double generalisedMean(double m)
        {
            return generalisedMean_as_double(m);
        }

        public double generalisedMean_as_double(double m)
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = generalisedMean(dd, m);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = generalisedMean(bd, m);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public double generalisedMean(decimal m)
        {
            return generalisedMean_as_double(m);
        }

        public double generalisedMean_as_double(decimal m)
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = generalisedMean(bd, m);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public ComplexClass generalisedMean_as_Complex(double m)
        {
            ComplexClass mean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = new ComplexClass(generalisedMean(dd, m));
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = new ComplexClass(generalisedMean(bd, m));
                    bd = null;
                    break;
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    mean = generalisedMean(cc, m);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public ComplexClass generalisedMean_as_Complex(ComplexClass m)
        {
            ComplexClass mean = ComplexClass.zero();
            switch (type)
            {
                case 1:
                case 12:
                case 14:
                    ComplexClass[] cc = getArray_as_Complex();
                    mean = generalisedMean(cc, m);
                    break;
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }


        // WEIGHTED GENERALIZED MEANS [WEIGHTED POWER MEANS](INSTANCE)
        public double weightedGeneralizedMean(double m)
        {
            return weightedGeneralizedMean_as_double(m);
        }

        public double weightedGeneralizedMean_as_double(double m)
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedGeneralizedMean_as_double: no weights supplied - unweighted m_mean returned");
                return generalizedMean_as_double(m);
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double mean = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = amWeights.getArray_as_double();
                        mean = generalisedMean(dd, ww, m);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        mean = generalisedMean(bd, wd, m);
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public double weightedGeneralizedMean(decimal m)
        {
            return weightedGeneralizedMean_as_double(m);
        }

        public double weightedGeneralizedMean_as_double(decimal m)
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedGeneralizedMean_as_double: no weights supplied - unweighted m_mean returned");
                return generalizedMean_as_double(m);
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double mean = 0.0D;
                switch (type)
                {
                    case 1:
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        mean = generalisedMean(bd, wd, m);
                        bd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to decimal");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public ComplexClass weightedGeneralizedMean_as_Complex(double m)
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedGeneralizedMean_as_Complex: no weights supplied - unweighted m_mean returned");
                return generalizedMean_as_Complex(m);
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                ComplexClass mean = ComplexClass.zero();
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = amWeights.getArray_as_double();
                        mean = new ComplexClass(generalisedMean(dd, ww, m));
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        mean = new ComplexClass(generalisedMean(bd, wd, m));
                        bd = null;
                        break;
                    case 14:
                        ComplexClass[] cc = getArray_as_Complex();
                        ComplexClass[] cw = amWeights.getArray_as_Complex();
                        mean = generalisedMean(cc, cw, m);
                        break;
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public ComplexClass weightedGeneralizedMean_as_Complex(ComplexClass m)
        {
            ComplexClass mean = ComplexClass.zero();
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedGeneralizedMean_as_dComplex: no weights supplied - unweighted m_mean returned");
                return generalizedMean_as_Complex(m);
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                switch (type)
                {
                    case 1:
                    case 12:
                    case 14:
                        ComplexClass[] cc = getArray_as_Complex();
                        ComplexClass[] cw = amWeights.getArray_as_Complex();
                        mean = generalisedMean(cc, cw, m);
                        break;
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return mean;
            }
        }

        public double weightedGeneralisedMean(double m)
        {
            return weightedGeneralizedMean_as_double(m);
        }

        public double weightedGeneralisedMean_as_double(double m)
        {
            return weightedGeneralizedMean_as_double(m);
        }

        public double weightedGeneralisedMean(decimal m)
        {
            return weightedGeneralizedMean_as_double(m);
        }

        public double weightedGeneralisedMean_as_double(decimal m)
        {
            return weightedGeneralizedMean_as_double(m);
        }

        public ComplexClass weightedGeneralisedMean_as_Complex(double m)
        {
            return weightedGeneralizedMean_as_Complex(m);
        }

        public ComplexClass weightedGeneralisedMean_as_Complex(ComplexClass m)
        {
            return weightedGeneralizedMean_as_Complex(m);
        }

        // INTERQUARTILE MEANS (INSTANCE)
        public double interQuartileMean()
        {
            return interQuartileMean_as_double();
        }

        public double interQuartileMean_as_double()
        {
            double mean = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    mean = interQuartileMean(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean =  (double)(interQuartileMean(bd));
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass interquartile m_mean is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        public decimal interQuartileMean_as_decimal()
        {
            decimal mean = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    mean = interQuartileMean(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass interquartile m_mean is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return mean;
        }

        // MEDIAN VALUE(INSTANCE)
        public double median()
        {
            return median_as_double();
        }

        public double median_as_double()
        {
            double median = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    median = Median.GetMedian(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    median =  (double)Median.GetMedian(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass median value not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return median;
        }

        public decimal median_as_decimal()
        {
            decimal median = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    median = Median.GetMedian(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass median value not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return median;
        }

        // ROOT MEAN SQUARE  (INSTANCE METHODS)
        public double rms()
        {
            double rms = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    rms = Stat.rms(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    rms = Stat.rms(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass root m_mean square is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return rms;
        }

        // WEIGHTED ROOT MEAN SQUARE  (INSTANCE METHODS)
        public double weightedRms()
        {
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedRms: no weights supplied - unweighted rms returned");
                return rms();
            }
            else
            {
                bool holdW = weightingOptionS;
                if (weightingReset)
                {
                    if (weightingOptionI)
                    {
                        weightingOptionS = true;
                    }
                    else
                    {
                        weightingOptionS = false;
                    }
                }
                double rms = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = amWeights.getArray_as_double();
                        rms = Stat.rms(dd, ww);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        rms = Stat.rms(bd, wd);
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass root m_mean square is not supported");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                weightingOptionS = holdW;
                return rms;
            }
        }


        // SKEWNESS (INSTANCE METHODS)
        // Moment skewness
        public double momentSkewness()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double skewness = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    skewness = Skewness.momentSkewness(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    skewness = Skewness.momentSkewness(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass skewness is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return skewness;
        }

        public double momentSkewness_as_double()
        {
            return momentSkewness();
        }

        // Median skewness
        public double medianSkewness()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double skewness = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    skewness = Skewness.medianSkewness(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    skewness = Skewness.medianSkewness(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass skewness is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return skewness;
        }

        public double medianSkewness_as_double()
        {
            return medianSkewness();
        }

        // quartile skewness as double
        public double quartileSkewness()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double skewness = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    skewness = Skewness.quartileSkewness(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    skewness =  (double)Skewness.quartileSkewness(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass skewness is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return skewness;
        }

        public double quartileSkewness_as_double()
        {
            return quartileSkewness();
        }

        // quartile skewness as decimal
        public decimal quartileSkewness_as_decimal()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            decimal skewness = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    skewness = Skewness.quartileSkewness(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass skewness is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return skewness;
        }


        // KURTOSIS (INSTANCE METHODS)
        public double kurtosis()
        {
            return kurtosis_as_double();
        }

        public double kurtosis_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double kurtosis = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    kurtosis = Kurtosis.kurtosis(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    kurtosis =  (double)(Kurtosis.kurtosis(bd));
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass kurtosis is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return kurtosis;
        }

        public double curtosis()
        {
            return kurtosis_as_double();
        }

        public double curtosis_as_double()
        {
            return kurtosis_as_double();
        }

        public double kurtosisExcess()
        {
            return kurtosisExcess_as_double();
        }

        public double excessKurtosis()
        {
            return kurtosisExcess_as_double();
        }

        public double excessCurtosis()
        {
            return kurtosisExcess_as_double();
        }

        public double kurtosisExcess_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double kurtosis = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    kurtosis = Kurtosis.kurtosisExcess(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    kurtosis =  (double)(Kurtosis.kurtosisExcess(bd));
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass kurtosis is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return kurtosis;
        }

        public double excessKurtosis_as_double()
        {
            return kurtosisExcess_as_double();
        }


        public double curtosisExcess()
        {
            return kurtosisExcess_as_double();
        }

        public double curtosisExcess_as_double()
        {
            return kurtosisExcess_as_double();
        }

        public double excessCurtosis_as_double()
        {
            return kurtosisExcess_as_double();
        }

        public decimal kurtosis_as_decimal()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            decimal kurtosis = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    kurtosis = Kurtosis.kurtosis(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass kurtosis is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return kurtosis;
        }

        public decimal curtosis_as_decimal()
        {
            return kurtosis_as_decimal();
        }


        public decimal kurtosisExcess_as_decimal()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            decimal kurtosis = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    kurtosis = Kurtosis.kurtosisExcess(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass kurtosis is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return kurtosis;
        }

        public decimal excessKurtosis_as_decimal()
        {
            return kurtosisExcess_as_decimal();
        }

        public decimal curtosisExcess_as_decimal()
        {
            return kurtosisExcess_as_decimal();
        }

        public decimal excessCurtosis_as_decimal()
        {
            return kurtosisExcess_as_decimal();
        }


        // VARIANCES (INSTANCE METHODS)
        public double variance()
        {
            return variance_as_double();
        }

        public double variance_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double variance = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    variance = Stat.variance(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    variance =  (double)(Stat.variance(bd));
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return variance;
        }

        public decimal variance_as_decimal()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            decimal variance = decimal.Zero;
            switch (type)
            {
                case 1:
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    variance = Stat.variance(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to decimal");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return variance;
        }

        public ComplexClass variance_as_Complex()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            ComplexClass variance = ComplexClass.zero();
            ComplexClass[] cc = getArray_as_Complex();
            variance = Stat.variance(cc);
            nFactorOptionS = hold;
            return variance;
        }

        public double variance_as_Complex_ConjugateCalcn()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            ComplexClass[] cc = getArray_as_Complex();
            double variance = varianceConjugateCalcn(cc);
            nFactorOptionS = hold;
            return variance;
        }

        public double variance_of_ComplexModuli()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_modulus_of_Complex();
            double variance = Stat.variance(re);
            nFactorOptionS = hold;
            return variance;
        }

        public double variance_of_ComplexRealParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_real_part_of_Complex();
            double variance = Stat.variance(re);
            nFactorOptionS = hold;
            return variance;
        }

        public double variance_of_ComplexImaginaryParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] im = array_as_imaginary_part_of_Complex();
            double variance = Stat.variance(im);
            nFactorOptionS = hold;
            return variance;
        }

        // WEIGHTED VARIANCES (INSTANCE METHODS)
        public double weightedVariance()
        {
            return weightedVariance_as_double();
        }

        public double weightedVariance_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }

            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_double: no weights supplied - unweighted value returned");
                varr = variance_as_double();
            }
            else
            {
                double weightedVariance = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = amWeights.getArray_as_double();
                        weightedVariance = variance(dd, ww);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        weightedVariance =  (double)(variance(bd, wd));
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                varr = weightedVariance;
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public decimal weightedVariance_as_decimal()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            decimal varr = decimal.Zero;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_decimal: no weights supplied - unweighted value returned");
                varr = variance_as_decimal();
            }
            else
            {
                decimal weightedVariance = decimal.Zero;
                switch (type)
                {
                    case 1:
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        weightedVariance = variance(bd, wd);
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to decimal");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                varr = weightedVariance;
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }


        public ComplexClass weightedVariance_as_Complex()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            ComplexClass varr = ComplexClass.zero();
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_Complex: no weights supplied - unweighted value returned");
                varr = variance_as_Complex();
            }
            else
            {
                ComplexClass weightedVariance = ComplexClass.zero();
                ComplexClass[] cc = getArray_as_Complex();
                ComplexClass[] wc = amWeights.getArray_as_Complex();
                weightedVariance = variance(cc, wc);
                varr = weightedVariance;
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedVariance_as_Complex_ConjugateCalcn()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_Complex: no weights supplied - unweighted value returned");
                varr = variance_as_Complex_ConjugateCalcn();
            }
            else
            {
                ComplexClass[] cc = getArray_as_Complex();
                ComplexClass[] wc = amWeights.getArray_as_Complex();
                varr = varianceConjugateCalcn(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedVariance_of_ComplexModuli()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_Complex: no weights supplied - unweighted value returned");
                varr = variance_of_ComplexModuli();
            }
            else
            {
                double[] cc = array_as_modulus_of_Complex();
                double[] wc = amWeights.array_as_modulus_of_Complex();
                varr = variance(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedVariance_of_ComplexRealParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_Complex: no weights supplied - unweighted value returned");
                varr = variance_of_ComplexRealParts();
            }
            else
            {
                double[] cc = array_as_real_part_of_Complex();
                double[] wc = amWeights.array_as_real_part_of_Complex();
                varr = variance(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedVariance_of_ComplexImaginaryParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine("weightedVariance_as_Complex: no weights supplied - unweighted value returned");
                varr = variance_of_ComplexImaginaryParts();
            }
            else
            {
                double[] cc = array_as_imaginary_part_of_Complex();
                double[] wc = amWeights.array_as_imaginary_part_of_Complex();
                varr = variance(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }


        // STANDARD DEVIATIONS (INSTANCE METHODS)
        public double standardDeviation()
        {
            return standardDeviation_as_double();
        }

        public double standardDeviation_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            double variance = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    variance = Stat.variance(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    variance =  (double)(Stat.variance(bd));
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return Math.Sqrt(variance);
        }

        public ComplexClass standardDeviation_as_Complex()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            ComplexClass variance = ComplexClass.zero();
            ComplexClass[] cc = getArray_as_Complex();
            variance = Stat.variance(cc);
            nFactorOptionS = hold;
            return ComplexClass.Sqrt(variance);
        }

        public double standardDeviation_as_Complex_ConjugateCalcn()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            ComplexClass[] cc = getArray_as_Complex();
            double variance = varianceConjugateCalcn(cc);
            nFactorOptionS = hold;
            return Math.Sqrt(variance);
        }

        public double standardDeviation_of_ComplexModuli()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_modulus_of_Complex();
            double standardDeviation = Stat.standardDeviation(re);
            nFactorOptionS = hold;
            return standardDeviation;
        }

        public double standardDeviation_of_ComplexRealParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_real_part_of_Complex();
            double standardDeviation = Stat.standardDeviation(re);
            nFactorOptionS = hold;
            return standardDeviation;
        }

        public double standardDeviation_of_ComplexImaginaryParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] im = array_as_imaginary_part_of_Complex();
            double standardDeviation = Stat.standardDeviation(im);
            nFactorOptionS = hold;
            return standardDeviation;
        }

        // WEIGHTED STANDARD DEVIATION (INSTANCE METHODS)
        public double weightedStandardDeviation()
        {
            return weightedStandardDeviation_as_double();
        }

        public double weightedStandardDeviation_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }

            double varr = 0.0;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardDeviation_as_double: no weights supplied - unweighted value returned");
                varr = standardDeviation_as_double();
            }
            else
            {
                double variance = 0.0D;
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = amWeights.getArray_as_double();
                        variance = Stat.variance(dd, ww);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        variance =  (double)(Stat.variance(bd, wd));
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                varr = Math.Sqrt(variance);
            }
            nFactorOptionS = hold;
            weightingOptionS = holdW;
            return varr;
        }


        public ComplexClass weightedStandardDeviation_as_Complex()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }

            ComplexClass varr = ComplexClass.zero();
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedtandardDeviationS_as_Complex: no weights supplied - unweighted value returned");
                varr = standardDeviation_as_Complex();
            }
            else
            {
                ComplexClass variance = ComplexClass.zero();
                ComplexClass[] cc = getArray_as_Complex();
                ComplexClass[] wc = amWeights.getArray_as_Complex();
                variance = Stat.variance(cc, wc);
                varr = ComplexClass.Sqrt(variance);
            }
            nFactorOptionS = hold;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedStandardDeviation_as_Complex_ConjugateCalcn()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedtandardDeviationS_as_Complex: no weights supplied - unweighted value returned");
                varr = standardDeviation_as_Complex_ConjugateCalcn();
            }
            else
            {
                double variance = double.NaN;
                ComplexClass[] cc = getArray_as_Complex();
                ComplexClass[] wc = amWeights.getArray_as_Complex();
                variance = varianceConjugateCalcn(cc, wc);
                varr = Math.Sqrt(variance);
            }
            nFactorOptionS = hold;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedStandardDeviation_of_ComplexModuli()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardDeviation_as_Complex: no weights supplied - unweighted value returned");
                varr = standardDeviation_of_ComplexModuli();
            }
            else
            {
                double[] cc = array_as_modulus_of_Complex();
                double[] wc = amWeights.array_as_modulus_of_Complex();
                varr = standardDeviation(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedStandardDeviation_of_ComplexRealParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardDeviation_as_Complex: no weights supplied - unweighted value returned");
                varr = standardDeviation_of_ComplexRealParts();
            }
            else
            {
                double[] cc = array_as_real_part_of_Complex();
                double[] wc = amWeights.array_as_real_part_of_Complex();
                varr = standardDeviation(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }


        public double weightedStandardDeviation_of_ComplexImaginaryParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardDeviation_as_Complex: no weights supplied - unweighted value returned");
                varr = standardDeviation_of_ComplexImaginaryParts();
            }
            else
            {
                double[] cc = array_as_imaginary_part_of_Complex();
                double[] wc = amWeights.array_as_imaginary_part_of_Complex();
                varr = standardDeviation(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }


        // STANDARD ERROR OF THE MEAN (INSTANCE METHODS)
        public double standardError()
        {
            return standardError_as_double();
        }

        public double standardError_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            double standardError = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    standardError = Stat.standardError(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    standardError = Stat.standardError(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass cannot be converted to double");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return standardError;
        }

        public ComplexClass standardError_as_Complex()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            ComplexClass standardError = ComplexClass.zero();
            ComplexClass[] cc = getArray_as_Complex();
            standardError = Stat.standardError(cc);
            nFactorOptionS = hold;
            return standardError;
        }

        public double standardError_as_Complex_ConjugateCalcn()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            ComplexClass[] cc = getArray_as_Complex();
            double standardError = standardErrorConjugateCalcn(cc);
            nFactorOptionS = hold;
            return standardError;
        }

        public double standardError_of_ComplexModuli()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_modulus_of_Complex();
            double standardError = Stat.standardError(re);
            nFactorOptionS = hold;
            return standardError;
        }

        public double standardError_of_ComplexRealParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_real_part_of_Complex();
            double standardError = Stat.standardError(re);
            nFactorOptionS = hold;
            return standardError;
        }

        public double standardError_of_ComplexImaginaryParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double[] re = array_as_imaginary_part_of_Complex();
            double standardError = Stat.standardError(re);
            nFactorOptionS = hold;
            return standardError;
        }

        // WEIGHTED STANDARD ERROR OF THE MEAN (INSTANCE METHODS)
        public double weightedStandardError()
        {
            return weightedStandardError_as_double();
        }

        public double weightedStandardError_as_double()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }

            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }

            double standardError = 0.0;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardError_as_double: no weights supplied - unweighted value returned");
                standardError = standardError_as_double();
            }
            else
            {
                switch (type)
                {
                    case 1:
                        double[] dd = getArray_as_double();
                        double[] ww = amWeights.getArray_as_double();
                        standardError = Stat.standardError(dd, ww);
                        break;
                    case 12:
                        decimal[] bd = getArray_as_decimal();
                        decimal[] wd = amWeights.getArray_as_decimal();
                        standardError = Stat.standardError(bd, wd);
                        bd = null;
                        wd = null;
                        break;
                    case 14:
                        throw new ArgumentException("ComplexClass cannot be converted to double");
                    default:
                        throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
                }
                standardError = Math.Sqrt(standardError);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return standardError;
        }


        public ComplexClass weightedStandarError_as_Complex()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }

            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }

            ComplexClass standardError = ComplexClass.zero();
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardError_as_Complex: no weights supplied - unweighted value returned");
                standardError = standardError_as_Complex();
            }
            else
            {
                ComplexClass[] cc = getArray_as_Complex();
                ComplexClass[] wc = amWeights.getArray_as_Complex();
                standardError = Stat.standardError(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;

            return standardError;
        }


        public double weightedStandarError_as_Complex_ConjugateCalcn()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }

            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }

            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double standardError = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardError_as_Complex: no weights supplied - unweighted value returned");
                standardError = standardError_as_Complex_ConjugateCalcn();
            }
            else
            {
                ComplexClass[] cc = getArray_as_Complex();
                ComplexClass[] wc = amWeights.getArray_as_Complex();
                standardError = standardErrorConjugateCalcn(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;

            return standardError;
        }

        public double weightedStandardError_of_ComplexModuli()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardError_as_Complex: no weights supplied - unweighted value returned");
                varr = standardError_of_ComplexModuli();
            }
            else
            {
                double[] cc = array_as_modulus_of_Complex();
                double[] wc = amWeights.array_as_modulus_of_Complex();
                varr = standardError(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }

        public double weightedStandardError_of_ComplexRealParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardError_as_Complex: no weights supplied - unweighted value returned");
                varr = standardError_of_ComplexRealParts();
            }
            else
            {
                double[] cc = array_as_real_part_of_Complex();
                double[] wc = amWeights.array_as_real_part_of_Complex();
                varr = standardError(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }


        public double weightedStandardError_of_ComplexImaginaryParts()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool hold2 = nEffOptionS;
            if (nEffReset)
            {
                if (nEffOptionI)
                {
                    nEffOptionS = true;
                }
                else
                {
                    nEffOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double varr = double.NaN;
            if (!weightsSupplied)
            {
                PrintToScreen.WriteLine(
                    "weightedStandardError_as_Complex: no weights supplied - unweighted value returned");
                varr = standardError_of_ComplexImaginaryParts();
            }
            else
            {
                double[] cc = array_as_imaginary_part_of_Complex();
                double[] wc = amWeights.array_as_imaginary_part_of_Complex();
                varr = standardError(cc, wc);
            }
            nFactorOptionS = hold;
            nEffOptionS = hold2;
            weightingOptionS = holdW;
            return varr;
        }


        // STANDARDIZE (INSTANCE METHODS)
        // Standardization of the internal array to a m_mean of 0 and a standard deviation of 1
        public double[] standardize()
        {
            double[] bb = null;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    bb = Standardize.standardize(dd);
                    break;
                case 14:
                    throw new ArgumentException("Standardization of ComplexClass is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return bb;
        }

        public double[] standardise()
        {
            return standardize();
        }


        // SCALE (INSTANCE METHODS)
        // Scale the internal array to a new m_mean  and a new standard deviation
        public double[] scale(double mean, double sd)
        {
            double[] bb = null;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    bb = Scale.scale(dd, mean, sd);
                    break;
                case 14:
                    throw new ArgumentException("Scaling of ComplexClass is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return bb;
        }


        // VOLATILITY (INSTANCE METHODS)
        public double volatilityLogChange()
        {
            double volatilityLogChange = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    volatilityLogChange = Stat.volatilityLogChange(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    volatilityLogChange = Stat.volatilityLogChange(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass volatilty is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return volatilityLogChange;
        }

        public double volatilityPerCentChange()
        {
            double volatilityPerCentChange = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    volatilityPerCentChange = Stat.volatilityPerCentChange(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    volatilityPerCentChange = Stat.volatilityPerCentChange(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass volatilty is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return volatilityPerCentChange;
        }

        //COEFFICIENT OF VARIATION
        public double coefficientOfVariation()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            double coefficientOfVariation = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    coefficientOfVariation = Stat.coefficientOfVariation(dd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    coefficientOfVariation = Stat.coefficientOfVariation(bd);
                    bd = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass coefficient of variation is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            return coefficientOfVariation;
        }

        public double weightedCoefficientOfVariation()
        {
            bool hold = nFactorOptionS;
            if (nFactorReset)
            {
                if (nFactorOptionI)
                {
                    nFactorOptionS = true;
                }
                else
                {
                    nFactorOptionS = false;
                }
            }
            bool holdW = weightingOptionS;
            if (weightingReset)
            {
                if (weightingOptionI)
                {
                    weightingOptionS = true;
                }
                else
                {
                    weightingOptionS = false;
                }
            }
            double coefficientOfVariation = 0.0D;
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    double[] wd = amWeights.getArray_as_double();
                    coefficientOfVariation = Stat.coefficientOfVariation(dd, wd);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    decimal[] bw = amWeights.getArray_as_decimal();
                    coefficientOfVariation = Stat.coefficientOfVariation(bd, bw);
                    bd = null;
                    bw = null;
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass coefficient of variation is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            nFactorOptionS = hold;
            weightingOptionS = holdW;
            return coefficientOfVariation;
        }

        // SHANNON ENTROPY (INSTANCE METHODS)
        // return Shannon entropy as bits
        public double shannonEntropy()
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = shannonEntropy(dd);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Shannon Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // return Shannon entropy as bits
        public double shannonEntropyBit()
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = shannonEntropy(dd);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Shannon Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // return Shannon entropy as nats
        public double shannonEntropyNat()
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = shannonEntropyNat(dd);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Shannon Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // return Shannon entropy as dits
        public double shannonEntropyDit()
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = shannonEntropyDit(dd);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Shannon Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // RENYI ENTROPY (INSTANCE METHODS)
        // return Renyi entropy as bits
        public double renyiEntropy(double alpha)
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = renyiEntropy(dd, alpha);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Renyi Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // return Renyi entropy as bits
        public double renyiEntropyBit(double alpha)
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = renyiEntropy(dd, alpha);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Renyi Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // return Renyi entropy as nats
        public double renyiEntropyNat(double alpha)
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = renyiEntropyNat(dd, alpha);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Renyi Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // return Renyi entropy as dits
        public double renyiEntropyDit(double alpha)
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = renyiEntropyDit(dd, alpha);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Renyi Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        // TSALLIS ENTROPY (INSTANCE METHODS)
        // return Tsallis entropy
        public double tsallisEntropyNat(double q)
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = tsallisEntropyNat(dd, q);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Tsallis Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }


        // GENERALIZED ENTROPY (INSTANCE METHODS)
        // return generalised entropy
        public double generalizedEntropyOneNat(double q, double r)
        {
            double entropy = 0.0D;
            switch (type)
            {
                case 1:
                case 12:
                    double[] dd = getArray_as_double();
                    entropy = generalizedEntropyOneNat(dd, q, r);
                    break;
                case 14:
                    throw new ArgumentException("ComplexClass Generalized Entropy is not meaningful");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            return entropy;
        }

        public double generalisedEntropyOneNat(double q, double r)
        {
            return generalizedEntropyOneNat(q, r);
        }

        // OUTLIER DETECTION (INSTANCE)
        // Anscombe test for a upper outlier
        public List<Object> upperOutliersAnscombe(double constant)
        {
            return upperOutliersAnscombe_as_double(constant);
        }

        // Anscombe test for a upper outlier
        public List<Object> upperOutliersAnscombe_as_double(double constant)
        {
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    upperOutlierDetails = upperOutliersAnscombeAsArrayList(dd, constant);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    List<Object> ret = new List<Object>();
                    ret = upperOutliersAnscombeAsArrayList(bd,  (decimal)(constant));
                    upperOutlierDetails.Add((int) ret[0]);
                    decimal[] bd1 = (decimal[]) ret[1];
                    ArrayMaths am1 = new ArrayMaths(bd1);
                    upperOutlierDetails.Add(am1.getArray_as_double());
                    upperOutlierDetails.Add(ret[2]);
                    decimal[] bd2 = (decimal[]) ret[3];
                    ArrayMaths am2 = new ArrayMaths(bd2);
                    upperOutlierDetails.Add(am2.getArray_as_double());
                    break;
                case 14:
                    throw new ArgumentException("Outlier detection of ComplexClass is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            upperDone = true;
            return upperOutlierDetails;
        }

        // Anscombe test for a upper outlier
        public List<Object> upperOutliersAnscombe(decimal constant)
        {
            return upperOutliersAnscombe_as_decimal(constant);
        }

        // Anscombe test for a upper outlier
        public List<Object> upperOutliersAnscombe_as_decimal(decimal constant)
        {
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    List<Object> ret = new List<Object>();
                    ret = upperOutliersAnscombeAsArrayList(dd,  (double)constant);
                    upperOutlierDetails.Add((int) ret[0]);
                    double[] dd1 = (double[]) ret[1];
                    ArrayMaths am1 = new ArrayMaths(dd1);
                    upperOutlierDetails.Add(am1.getArray_as_decimal());
                    upperOutlierDetails.Add(ret[2]);
                    double[] dd2 = (double[]) ret[3];
                    ArrayMaths am2 = new ArrayMaths(dd2);
                    upperOutlierDetails.Add(am2.getArray_as_decimal());
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    upperOutlierDetails = upperOutliersAnscombeAsArrayList(bd, constant);
                    break;
                case 14:
                    throw new ArgumentException("Outlier detection of ComplexClass is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            upperDone = true;
            return upperOutlierDetails;
        }


        public List<Object> upperOutliersAnscombe(long constant)
        {
            return upperOutliersAnscombe_as_decimal((constant));
        }

        public List<Object> upperOutliersAnscombe_as_decimal(long constant)
        {
            return upperOutliersAnscombe_as_decimal((constant));
        }

        // Anscombe test for a lower outlier
        public List<Object> lowerOutliersAnscombe(double constant)
        {
            return lowerOutliersAnscombe_as_double(constant);
        }

        // Anscombe test for a lower outlier
        public List<Object> lowerOutliersAnscombe_as_double(double constant)
        {
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    lowerOutlierDetails = lowerOutliersAnscombeAsArrayList(dd, constant);
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    List<Object> ret = new List<Object>();
                    ret = lowerOutliersAnscombeAsArrayList(bd,  (decimal)(constant));
                    lowerOutlierDetails.Add((int) ret[0]);
                    decimal[] bd1 = (decimal[]) ret[1];
                    ArrayMaths am1 = new ArrayMaths(bd1);
                    lowerOutlierDetails.Add(am1.getArray_as_double());
                    lowerOutlierDetails.Add(ret[2]);
                    decimal[] bd2 = (decimal[]) ret[3];
                    ArrayMaths am2 = new ArrayMaths(bd2);
                    lowerOutlierDetails.Add(am2.getArray_as_double());
                    break;
                case 14:
                    throw new ArgumentException("Outlier detection of ComplexClass is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            lowerDone = true;
            return lowerOutlierDetails;
        }

        public List<Object> lowerOutliersAnscombe(decimal constant)
        {
            return lowerOutliersAnscombe_as_decimal(constant);
        }


        public List<Object> lowerOutliersAnscombe_as_decimal(decimal constant)
        {
            switch (type)
            {
                case 1:
                    double[] dd = getArray_as_double();
                    List<Object> ret = new List<Object>();
                    ret = lowerOutliersAnscombeAsArrayList(dd, constant);
                    lowerOutlierDetails.Add((int) ret[0]);
                    double[] dd1 = (double[]) ret[1];
                    ArrayMaths am1 = new ArrayMaths(dd1);
                    lowerOutlierDetails.Add(am1.getArray_as_decimal());
                    lowerOutlierDetails.Add(ret[2]);
                    double[] dd2 = (double[]) ret[3];
                    ArrayMaths am2 = new ArrayMaths(dd2);
                    lowerOutlierDetails.Add(am2.getArray_as_decimal());
                    break;
                case 12:
                    decimal[] bd = getArray_as_decimal();
                    lowerOutlierDetails = lowerOutliersAnscombeAsArrayList(bd, constant);
                    break;
                case 14:
                    throw new ArgumentException("Outlier detection of ComplexClass is not supported");
                default:
                    throw new ArgumentException("This type number, " + type + ", should not be possible here!!!!");
            }
            lowerDone = true;
            return lowerOutlierDetails;
        }

        private List<object> lowerOutliersAnscombeAsArrayList(double[] dd, decimal constant)
        {
            throw new NotImplementedException();
        }

        public List<Object> lowerOutliersAnscombe(long constant)
        {
            return lowerOutliersAnscombe_as_decimal((constant));
        }

        public List<Object> lowerOutliersAnscombe_as_decimal(long constant)
        {
            return lowerOutliersAnscombe_as_decimal((constant));
        }


        // STATIC METHODS
        // WEIGHTING CHOICE (STATIC)
        // Set weights to 'big W' - multiplicative factor
        public static void setStaticWeightsToBigW()
        {
            weightingOptionS = false;
        }

        // Set weights to 'little w' - uncertainties
        public static void setStaticWeightsToLittleW()
        {
            weightingOptionS = true;
        }

        // CONVERSION OF WEIGHTING FACTORS
        // Converts weighting facors Wi to wi, i.e. to 1/sqrt(Wi)
        public static double[] convertBigWtoLittleW(double[] bigW)
        {
            ArrayMaths am1 = new ArrayMaths(bigW);
            ArrayMaths am2 = am1.oneOverSqrt();
            return am2.getArray_as_double();
        }

        public static float[] convertBigWtoLittleW(float[] bigW)
        {
            ArrayMaths am1 = new ArrayMaths(bigW);
            ArrayMaths am2 = am1.oneOverSqrt();
            return am2.getArray_as_float();
        }

        public static ComplexClass[] convertBigWtoLittleW(ComplexClass[] bigW)
        {
            ArrayMaths am1 = new ArrayMaths(bigW);
            ArrayMaths am2 = am1.oneOverSqrt();
            return am2.getArray_as_Complex();
        }

        public static double[] convertBigWtoLittleW(decimal[] bigW)
        {
            ArrayMaths am1 = new ArrayMaths(bigW);
            ArrayMaths am2 = am1.oneOverSqrt();
            return am2.getArray_as_double();
        }

        public static double[] convertBigWtoLittleW(long[] bigW)
        {
            ArrayMaths am1 = new ArrayMaths(bigW);
            ArrayMaths am2 = am1.oneOverSqrt();
            return am2.getArray_as_double();
        }

        // private weighting calculation
        // returns weight w
        // litte w to one over little w squared if uncertainties used
        private static double[] invertAndSquare(double[] ww)
        {
            double[] weight = (double[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am.Pow(2);
                am = am.invert();
                weight = am.array();
            }
            return weight;
        }

        private static float[] invertAndSquare(float[] ww)
        {
            float[] weight = (float[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_float();
            }
            return weight;
        }

        private static ComplexClass[] invertAndSquare(ComplexClass[] ww)
        {
            ComplexClass[] weight = (ComplexClass[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_Complex();
            }
            return weight;
        }

        private static decimal[] invertAndSquare(decimal[] ww)
        {
            decimal[] weight = (decimal[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_decimal();
            }
            return weight;
        }

        private static decimal[] invertAndSquare(long[] ww)
        {
            ArrayMaths am = new ArrayMaths(ww);
            decimal[] weight = am.array_as_decimal();
            if (weightingOptionS)
            {
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_decimal();
            }
            return weight;
        }


        // DENOMINATOR CHOICE (STATIC)
        // Set standard deviation, variance and covariance denominators to n
        public static void setStaticDenominatorToN()
        {
            nFactorOptionS = true;
        }

        // Set standard deviation, variance and covariance denominators to n
        public static void setStaticDenominatorToNminusOne()
        {
            nFactorOptionS = false;
        }


        // EFFECTIVE SAMPLE NUMBER
        // Repalce number of data points to the effective sample number in weighted calculations
        public static void useStaticEffectiveN()
        {
            nEffOptionS = true;
        }

        // Repalce the effective sample number in weighted calculations by the number of data points
        public static void useStaticTrueN()
        {
            nEffOptionS = false;
        }

        // Calculation of the effective sample number 
        public static double effectiveSampleNumber(double[] ww)
        {
            double[] weight = (double[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array();
            }
            int n = weight.Length;

            double nEff = n;
            if (nEffOptionS)
            {
                double sum2w = 0.0D;
                double sumw2 = 0.0D;
                for (int i = 0; i < n; i++)
                {
                    sum2w += weight[i];
                    sumw2 += weight[i]*weight[i];
                }
                sum2w *= sum2w;
                nEff = sum2w/sumw2;
            }
            return nEff;
        }

        // Calculation of the sample number (float)
        public static float effectiveSampleNumber(float[] ww)
        {
            float[] weight = (float[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_float();
            }
            int n = weight.Length;

            float nEff = n;
            if (nEffOptionS)
            {
                float sum2w = 0.0F;
                float sumw2 = 0.0F;
                for (int i = 0; i < n; i++)
                {
                    sum2w += weight[i];
                    sumw2 += weight[i]*weight[i];
                }
                sum2w *= sum2w;
                nEff = sum2w/sumw2;
            }
            return nEff;
        }

        // Calculation of the sample number (ComplexClass)
        public static ComplexClass effectiveSampleNumber(ComplexClass[] ww)
        {
            ComplexClass[] weight = (ComplexClass[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_Complex();
            }
            int n = weight.Length;

            ComplexClass nEff = new ComplexClass(n, 0.0);
            if (nEffOptionS)
            {
                ComplexClass sumw2 = ComplexClass.zero();
                ComplexClass sum2w = ComplexClass.zero();
                for (int i = 0; i < n; i++)
                {
                    sum2w = sum2w.plus(weight[i]);
                    sumw2 = sumw2.plus(weight[i].times(weight[i]));
                }
                sum2w = sum2w.times(sum2w);
                nEff = sum2w.over(sumw2);
            }
            return nEff;
        }

        // Calculation of the sample number (ComplexClass  - Conjugate formula)
        public static double effectiveSampleNumberConjugateCalcn(ComplexClass[] ww)
        {
            ComplexClass[] weight = (ComplexClass[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_Complex();
            }
            int n = weight.Length;

            double nEff = double.NaN;
            if (nEffOptionS)
            {
                ComplexClass sumw2 = ComplexClass.zero();
                ComplexClass sum2w = ComplexClass.zero();
                for (int i = 0; i < n; i++)
                {
                    sum2w = sum2w.plus(weight[i]);
                    sumw2 = sumw2.plus(weight[i].times(weight[i].conjugate()));
                }
                sum2w = sum2w.times(sum2w.conjugate());
                nEff = sum2w.getReal()/sumw2.getReal();
            }
            return nEff;
        }

        // Calculation of the sample number 
        public static decimal effectiveSampleNumber(decimal[] ww)
        {
            decimal[] weight = (decimal[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_decimal();
            }
            int n = weight.Length;

            decimal nEff = (n);
            if (nEffOptionS)
            {
                decimal sumw2 = decimal.Zero;
                decimal sum2w = decimal.Zero;
                for (int i = 0; i < n; i++)
                {
                    sum2w = sum2w + (weight[i]);
                    sumw2 = sumw2 + (weight[i]*(weight[i]));
                }
                sum2w = sum2w*(sum2w);
                nEff = sum2w/(sumw2);
                sumw2 = 0;
                sum2w = 0;
                weight = null;
            }
            return nEff;
        }

        public static decimal effectiveSampleNumber(long[] ww)
        {
            ArrayMaths am = new ArrayMaths(ww);
            decimal[] www = am.array_as_decimal();
            return effectiveSampleNumber(www);
        }


        // ARITMETIC MEANS (STATIC)

        // Arithmetic m_mean of a 1D array of doubles, aa
        public static double mean(double[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i];
            }
            return sum/(n);
        }

        // Arithmetic m_mean of a 1D array of floats, aa
        public static float mean(float[] aa)
        {
            int n = aa.Length;
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i];
            }
            return sum/(n);
        }

        // Arithmetic m_mean of a 1D array of int, aa
        public static double mean(long[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i];
            }
            return sum/(n);
        }

        // Arithmetic m_mean of a 1D array of int, aa
        public static double mean(int[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i];
            }
            return sum/(n);
        }

        // Arithmetic m_mean of a 1D array of short, aa
        public static double mean(short[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i];
            }
            return sum/(n);
        }

        // Arithmetic m_mean of a 1D array of byte, aa
        public static double mean(byte[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i];
            }
            return sum/(n);
        }

        // Arithmetic m_mean of a 1D array of ComplexClass, aa
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

        // Arithmetic m_mean of a 1D array of decimal, aa
        public static decimal mean(decimal[] aa)
        {
            int n = aa.Length;
            decimal sum = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sum = sum + (aa[i]);
            }
            return sum/((n));
        }

        // WEIGHTED ARITHMETIC MEANS (STATIC)
        // Weighted arithmetic m_mean of a 1D array of doubles, aa
        public static double mean(double[] aa, double[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double[] weight = (double[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array();
            }
            double sumx = 0.0D;
            double sumw = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sumx += aa[i]*weight[i];
                sumw += weight[i];
            }
            return sumx/sumw;
        }

        // Weighted arithmetic m_mean of a 1D array of floats, aa
        public static float mean(float[] aa, float[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            float[] weight = (float[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_float();
            }

            float sumx = 0.0F;
            float sumw = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sumx += aa[i]*weight[i];
                sumw += weight[i];
            }
            return sumx/sumw;
        }

        // Weighted arithmetic m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass mean(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ComplexClass[] weight = (ComplexClass[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_Complex();
            }
            ComplexClass sumx = ComplexClass.zero();
            ComplexClass sumw = ComplexClass.zero();
            for (int i = 0; i < n; i++)
            {
                sumx = sumx.plus(aa[i].times(weight[i]));
                sumw = sumw.plus(weight[i]);
            }
            return sumx.over(sumw);
        }

        // Weighted arithmetic m_mean of a 1D array of decimal, aa
        public static decimal mean(decimal[] aa, decimal[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            decimal[] weight = (decimal[]) ww.Clone();
            if (weightingOptionS)
            {
                ArrayMaths am = new ArrayMaths(ww);
                am = am.Pow(2);
                am = am.invert();
                weight = am.array_as_decimal();
            }

            decimal sumx = decimal.Zero;
            decimal sumw = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sumx = sumx + (aa[i]*(weight[i]));
                sumw = sumw + (weight[i]);
            }
            sumx = sumx/(sumw);
            sumw = 0;
            weight = null;
            return sumx;
        }

        // Weighted arithmetic m_mean of a 1D array of long, aa
        public static decimal mean(long[] aa, long[] ww)
        {
            ArrayMaths amaa = new ArrayMaths(aa);
            ArrayMaths amww = new ArrayMaths(ww);

            return mean(amaa.array_as_decimal(), amww.array_as_decimal());
        }

        // SUBTRACT THE MEAN (STATIC)
        // Subtract arithmetic m_mean of an array from data array elements
        public static double[] subtractMean(double[] array)
        {
            int n = array.Length;
            double mean = Stat.mean(array);
            double[] arrayMinusMean = new double[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i] - mean;
            }

            return arrayMinusMean;
        }

        // Subtract arithmetic m_mean of an array from data array elements
        public static float[] subtractMean(float[] array)
        {
            int n = array.Length;
            float mean = Stat.mean(array);
            float[] arrayMinusMean = new float[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i] - mean;
            }

            return arrayMinusMean;
        }


        // Subtract arithmetic m_mean of an array from data array elements
        public static decimal[] subtractMean(decimal[] array)
        {
            int n = array.Length;
            decimal mean = Stat.mean(array);
            decimal[] arrayMinusMean = new decimal[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i] - (mean);
            }
            mean = 0;
            return arrayMinusMean;
        }

        // Subtract arithmetic m_mean of an array from data array elements
        public static decimal[] subtractMean(long[] array)
        {
            int n = array.Length;
            decimal mean =  (decimal)Stat.mean(array);
            decimal[] arrayMinusMean = new decimal[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = ((array[i])) - (mean);
            }
            mean = 0;
            return arrayMinusMean;
        }

        // Subtract arithmetic m_mean of an array from data array elements
        public static ComplexClass[] subtractMean(ComplexClass[] array)
        {
            int n = array.Length;
            ComplexClass mean = Stat.mean(array);
            ComplexClass[] arrayMinusMean = new ComplexClass[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i].minus(mean);
            }

            return arrayMinusMean;
        }

        // Subtract weighted arirhmetic m_mean of an array from data array elements
        public static double[] subtractMean(double[] array, double[] weights)
        {
            int n = array.Length;
            double mean = Stat.mean(array, weights);
            double[] arrayMinusMean = new double[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i] - mean;
            }

            return arrayMinusMean;
        }

        // Subtract weighted arirhmetic m_mean of an array from data array elements
        public static float[] subtractMean(float[] array, float[] weights)
        {
            int n = array.Length;
            float mean = Stat.mean(array, weights);
            float[] arrayMinusMean = new float[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i] - mean;
            }

            return arrayMinusMean;
        }


        // Subtract weighted arirhmetic m_mean of an array from data array elements
        public static decimal[] subtractMean(decimal[] array, decimal[] weights)
        {
            int n = array.Length;
            decimal mean = Stat.mean(array, weights);
            decimal[] arrayMinusMean = new decimal[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i] - (mean);
            }
            mean = 0;
            return arrayMinusMean;
        }

        // Subtract weighted arirhmetic m_mean of an array from data array elements
        public static decimal[] subtractMean(long[] array, long[] weights)
        {
            int n = array.Length;
            decimal mean = Stat.mean(array, weights);
            decimal[] arrayMinusMean = new decimal[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = ((array[i])) - (mean);
            }
            mean = 0;
            return arrayMinusMean;
        }

        // Subtract weighted arirhmetic m_mean of an array from data array elements
        public static ComplexClass[] subtractMean(ComplexClass[] array, ComplexClass[] weights)
        {
            int n = array.Length;
            ComplexClass mean = Stat.mean(array, weights);
            ComplexClass[] arrayMinusMean = new ComplexClass[n];
            for (int i = 0; i < n; i++)
            {
                arrayMinusMean[i] = array[i].minus(mean);
            }

            return arrayMinusMean;
        }

        // GEOMETRIC MEANS (STATIC)

        // Geometric m_mean of a 1D array of decimal, aa
        public static double geometricMean(decimal[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += Math.Log( (double)aa[i]);
            }
            return Math.Exp(sum/n);
        }

        // Geometric m_mean of a 1D array of long, aa
        public static double geometricMean(long[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += Math.Log(aa[i]);
            }
            return Math.Exp(sum/n);
        }

        // Geometric m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass geometricMean(ComplexClass[] aa)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            for (int i = 0; i < n; i++)
            {
                sum = sum.plus(ComplexClass.Log(aa[i]));
            }
            return ComplexClass.Exp(sum.over(n));
        }

        // Geometric m_mean of a 1D array of doubles, aa
        public static double geometricMean(double[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += Math.Log(aa[i]);
            }
            return Math.Exp(sum/n);
        }

        // Geometric m_mean of a 1D array of floats, aa
        public static float geometricMean(float[] aa)
        {
            int n = aa.Length;
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += (float) Math.Log(aa[i]);
            }
            return (float) Math.Exp(sum/n);
        }

        // Weighted geometric m_mean of a 1D array of Complexs, aa
        public static ComplexClass geometricMean(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ComplexClass sumW = ComplexClass.zero();
            ComplexClass[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW = sumW.plus(weight[i]);
            }
            ComplexClass sum = ComplexClass.zero();
            for (int i = 0; i < n; i++)
            {
                sum = sum.plus(ComplexClass.Log(aa[i]).times(weight[i]));
            }
            return ComplexClass.Exp(sum.over(sumW));
        }

        // Weighted geometric m_mean of a 1D array of decimal, aa
        public static double geometricMean(decimal[] aa, decimal[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths weighting = new ArrayMaths(invertAndSquare(ww));
            double[] weight = weighting.array();

            double sumW = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sumW += weight[i];
            }
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += Math.Log( (double)aa[i])*weight[i];
            }
            return Math.Exp(sum/sumW);
        }

        // Weighted geometric m_mean of a 1D array of decimal, aa
        public static double geometricMean(long[] aa, long[] ww)
        {
            ArrayMaths amaa = new ArrayMaths(aa);
            ArrayMaths amww = new ArrayMaths(ww);
            return geometricMean(amaa.array_as_decimal(), amww.array_as_decimal());
        }

        // Weighted geometric m_mean of a 1D array of double, aa
        public static double geometricMean(double[] aa, double[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double sumW = 0.0D;
            double[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW += weight[i];
            }
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += Math.Log(aa[i])*weight[i];
            }
            return Math.Exp(sum/sumW);
        }

        // Weighted geometric m_mean of a 1D array of floats, aa
        public static float geometricMean(float[] aa, float[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            float sumW = 0.0F;
            float[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW += weight[i];
            }
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += (float) Math.Log(aa[i])*weight[i];
            }
            return (float) Math.Exp(sum/sumW);
        }

        // HARMONIC MEANS (STATIC)

        // Harmonic m_mean of a 1D array of decimal, aa
        public static decimal harmonicMean(decimal[] aa)
        {
            int n = aa.Length;
            decimal sum = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sum += (decimal.One/(aa[i]));
            }
            sum =  (( n))/(sum);
            return sum;
        }

        // Harmonic m_mean of a 1D array of long, aa
        public static decimal harmonicMean(long[] aa)
        {
            int n = aa.Length;
            ArrayMaths am = new ArrayMaths(aa);
            decimal[] bd = am.getArray_as_decimal();
            decimal sum = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sum = sum + (decimal.One/(bd[i]));
            }
            sum = ((n))/(sum);
            bd = null;
            return sum;
        }

        // Harmonic m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass harmonicMean(ComplexClass[] aa)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            for (int i = 0; i < n; i++)
            {
                sum = sum.plus(ComplexClass.plusOne().over(aa[i]));
            }
            sum = (new ComplexClass( n)).over(sum);
            return sum;
        }

        // Harmonic m_mean of a 1D array of doubles, aa
        public static double harmonicMean(double[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += 1.0D/aa[i];
            }
            return n/sum;
        }

        // Harmonic m_mean of a 1D array of floats, aa
        public static float harmonicMean(float[] aa)
        {
            int n = aa.Length;
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += 1.0F/aa[i];
            }
            return n/sum;
        }

        // Weighted harmonic m_mean of a 1D array of decimal, aa
        public static decimal harmonicMean(decimal[] aa, decimal[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            decimal sum = decimal.Zero;
            decimal sumW = decimal.Zero;
            decimal[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW = sumW + (weight[i]);
            }
            for (int i = 0; i < n; i++)
            {
                sum = sum + (weight[i]/(aa[i]));
            }
            sum = sumW/(sum);
            sumW = 0;
            weight = null;
            return sum;
        }

        // Weighted harmonic m_mean of a 1D array of long, aa
        public static decimal harmonicMean(long[] aa, long[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            ArrayMaths wm = new ArrayMaths(ww);
            return harmonicMean(am.getArray_as_decimal(), wm.getArray_as_decimal());
        }

        // Weighted harmonic m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass harmonicMean(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ComplexClass sum = ComplexClass.zero();
            ComplexClass sumW = ComplexClass.zero();
            ComplexClass[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW = sumW.plus(weight[i]);
            }
            for (int i = 0; i < n; i++)
            {
                sum = sum.plus(weight[i].over(aa[i]));
            }
            return sumW.over(sum);
        }

        // Weighted harmonic m_mean of a 1D array of doubles, aa
        public static double harmonicMean(double[] aa, double[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double sum = 0.0D;
            double sumW = 0.0D;
            double[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW += weight[i];
            }
            for (int i = 0; i < n; i++)
            {
                sum += weight[i]/aa[i];
            }
            return sumW/sum;
        }

        // Weighted harmonic m_mean of a 1D array of floats, aa
        public static float harmonicMean(float[] aa, float[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            float sum = 0.0F;
            float sumW = 0.0F;
            float[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumW += weight[i];
            }
            for (int i = 0; i < n; i++)
            {
                sum += weight[i]/aa[i];
            }
            return sumW/sum;
        }

        // GENERALIZED MEANS [POWER MEANS] (STATIC METHODS)

        // generalized m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass generalizedMean(ComplexClass[] aa, double m)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            if (m == 0.0D)
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Log(aa[i]));
                }
                return ComplexClass.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Pow(aa[i], m));
                }
                return ComplexClass.Pow(sum.over(n), 1.0D/m);
            }
        }

        // generalized m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass generalizedMean(ComplexClass[] aa, ComplexClass m)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            if (m.Equals(ComplexClass.zero()))
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Log(aa[i]));
                }
                return ComplexClass.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Pow(aa[i], m));
                }
                return ComplexClass.Pow(sum.over(n), ComplexClass.plusOne().over(m));
            }
        }

        // generalized m_mean of a 1D array of decimal, aa
        public static double generalizedMean(decimal[] aa, double m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalizedMean(dd, m);
        }

        // generalized m_mean of a 1D array of decimal, aa
        public static double generalizedMean(decimal[] aa, decimal m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalizedMean(dd, (double) m);
        }

        // generalized m_mean of a 1D array of long, aa
        public static double generalizedMean(long[] aa, double m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalizedMean(dd, m);
        }

        // generalized m_mean of a 1D array of long, aa
        public static double generalizedMean(long[] aa, long m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalizedMean(dd, m);
        }

        // generalized m_mean of a 1D array of doubles, aa
        public static double generalizedMean(double[] aa, double m)
        {
            int n = aa.Length;
            double sum = 0.0D;
            if (m == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum += Math.Log(aa[i]);
                }
                return Math.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum += Math.Pow(aa[i], m);
                }
                return Math.Pow(sum/(n), 1.0D/m);
            }
        }

        // generalized m_mean of a 1D array of floats, aa
        public static float generalizedMean(float[] aa, float m)
        {
            int n = aa.Length;
            float sum = 0.0F;
            if (m == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum += (float) Math.Log(aa[i]);
                }
                return (float) Math.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum += (float) Math.Pow(aa[i], m);
                }
                return (float) Math.Pow(sum/(n), 1.0F/m);
            }
        }


        // Generalised m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass generalisedMean(ComplexClass[] aa, double m)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            if (m == 0.0D)
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Log(aa[i]));
                }
                return ComplexClass.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Pow(aa[i], m));
                }
                return ComplexClass.Pow(sum.over(n), 1.0D/m);
            }
        }

        // Generalised m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass generalisedMean(ComplexClass[] aa, ComplexClass m)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            if (m.Equals(ComplexClass.zero()))
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Log(aa[i]));
                }
                return ComplexClass.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Pow(aa[i], m));
                }
                return ComplexClass.Pow(sum.over(n), ComplexClass.plusOne().over(m));
            }
        }

        // Generalised m_mean of a 1D array of decimal, aa
        public static double generalisedMean(decimal[] aa, double m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalisedMean(dd, m);
        }

        // Generalised m_mean of a 1D array of decimal, aa
        public static double generalisedMean(decimal[] aa, decimal m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalisedMean(dd,  (double)m);
        }

        // Generalised m_mean of a 1D array of long, aa
        public static double generalisedMean(long[] aa, double m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalisedMean(dd, m);
        }

        // Generalised m_mean of a 1D array of long, aa
        public static double generalisedMean(long[] aa, long m)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] dd = am.getArray_as_double();
            return generalisedMean(dd, m);
        }

        // Generalised m_mean of a 1D array of doubles, aa
        public static double generalisedMean(double[] aa, double m)
        {
            int n = aa.Length;
            double sum = 0.0D;
            if (m == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum += Math.Log(aa[i]);
                }
                return Math.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum += Math.Pow(aa[i], m);
                }
                return Math.Pow(sum/(n), 1.0D/m);
            }
        }

        // Generalised m_mean of a 1D array of floats, aa
        public static float generalisedMean(float[] aa, float m)
        {
            int n = aa.Length;
            float sum = 0.0F;
            if (m == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum += (float) Math.Log(aa[i]);
                }
                return (float) Math.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum += (float) Math.Pow(aa[i], m);
                }
                return (float) Math.Pow(sum/(n), 1.0F/m);
            }
        }

        // WEIGHTED GENERALIZED MEANS

        // weighted generalized m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass generalisedMean(ComplexClass[] aa, ComplexClass[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            ComplexClass sum = ComplexClass.zero();
            ComplexClass sumw = ComplexClass.zero();
            ComplexClass[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw = sumw.plus(weight[i]);
            }

            if (m == 0.0D)
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Log(weight[i].times(aa[i])).over(sumw));
                }
                return ComplexClass.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(weight[i].times(ComplexClass.Pow(aa[i], m)));
                }
                return ComplexClass.Pow(sum.over(sumw), 1.0D/m);
            }
        }

        // weighted generalized m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass generalisedMean(ComplexClass[] aa, ComplexClass[] ww, ComplexClass m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            ComplexClass sum = ComplexClass.zero();
            ComplexClass sumw = ComplexClass.zero();
            ComplexClass[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw = sumw.plus(weight[i]);
            }

            if (m.Equals(ComplexClass.zero()))
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(ComplexClass.Log(weight[i].times(aa[i])).over(sumw));
                }
                return ComplexClass.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum = sum.plus(weight[i].times(ComplexClass.Pow(aa[i], m)));
                }
                return ComplexClass.Pow(sum.over(sumw), ComplexClass.plusOne().over(m));
            }
        }

        // weighted generalized m_mean of a 1D array of decimal, aa
        public static double generalisedMean(decimal[] aa, decimal[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            ArrayMaths am1 = new ArrayMaths(aa);
            double[] dd = am1.getArray_as_double();
            ArrayMaths am2 = new ArrayMaths(ww);
            double[] wd = am2.getArray_as_double();
            return generalisedMean(dd, wd, m);
        }

        // weighted generalized m_mean of a 1D array of decimal, aa
        public static double generalisedMean(decimal[] aa, decimal[] ww, decimal m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            ArrayMaths am1 = new ArrayMaths(aa);
            double[] dd = am1.getArray_as_double();
            ArrayMaths am2 = new ArrayMaths(ww);
            double[] wd = am2.getArray_as_double();
            return generalisedMean(dd, wd,  (double)m);
        }

        // weighted generalized m_mean of a 1D array of long, aa
        public static double generalisedMean(long[] aa, long[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            ArrayMaths am1 = new ArrayMaths(aa);
            double[] dd = am1.getArray_as_double();
            ArrayMaths am2 = new ArrayMaths(ww);
            double[] wd = am2.getArray_as_double();
            return generalisedMean(dd, wd, m);
        }

        // weighted generalized m_mean of a 1D array of long, aa
        public static double generalisedMean(long[] aa, long[] ww, long m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            ArrayMaths am1 = new ArrayMaths(aa);
            double[] dd = am1.getArray_as_double();
            ArrayMaths am2 = new ArrayMaths(ww);
            double[] wd = am2.getArray_as_double();
            return generalisedMean(dd, wd, m);
        }

        // weighted generalized m_mean of a 1D array of doubles, aa
        public static double generalisedMean(double[] aa, double[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            double sum = 0.0D;
            double sumw = 0.0D;
            double[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw += weight[i];
            }

            if (m == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum += Math.Log(aa[i]*weight[i]/sumw);
                }
                return Math.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum += weight[i]*Math.Pow(aa[i], m);
                }
                return Math.Pow(sum/sumw, 1.0D/m);
            }
        }

        // weighted generalized m_mean of a 1D array of floats, aa
        public static float generalisedMean(float[] aa, float[] ww, float m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            float sum = 0.0F;
            float sumw = 0.0F;
            float[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw += weight[i];
            }
            if (m == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum += (float) Math.Log(aa[i]);
                }
                return (float) Math.Exp(sum);
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    sum += (float) Math.Pow(aa[i], m);
                }
                return (float) Math.Pow(sum/sumw, 1.0F/m);
            }
        }


        // weighted generalised m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass weightedGeneralisedMean(ComplexClass[] aa, ComplexClass[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass weightedGeneralisedMean(ComplexClass[] aa, ComplexClass[] ww, ComplexClass m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of decimal, aa
        public static double weightedGeneralisedMean(decimal[] aa, decimal[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of decimal, aa
        public static double weightedGeneralisedMean(decimal[] aa, decimal[] ww, decimal m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of long, aa
        public static double weightedGeneralisedMean(long[] aa, long[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of long, aa
        public static double weightedGeneralisedMean(long[] aa, long[] ww, long m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of doubles, aa
        public static double weightedGeneralisedMean(double[] aa, double[] ww, double m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }

        // weighted generalised m_mean of a 1D array of floats, aa
        public static float weightedGeneralisedMean(float[] aa, float[] ww, float m)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            return generalisedMean(aa, ww, m);
        }


        // INTERQUARTILE MEANS

        // Interquartile m_mean of a 1D array of decimal, aa
        public static decimal interQuartileMean(decimal[] aa)
        {
            int n = aa.Length;
            if (n < 4)
            {
                throw new ArgumentException("At least 4 array elements needed");
            }
            ArrayMaths am = new ArrayMaths(aa);
            ArrayMaths as_ = am.sort();
            decimal[] bb = as_.getArray_as_decimal();
            decimal sum = decimal.Zero;
            for (int i = n/4; i < 3*n/4; i++)
            {
                sum = sum + (bb[i]);
            }
            sum = sum*( (2.0M/n));
            bb = null;
            return sum;
        }

        // Interquartile m_mean of a 1D array of long, aa
        public static decimal interQuartileMean(long[] aa)
        {
            int n = aa.Length;
            if (n < 4)
            {
                throw new ArgumentException("At least 4 array elements needed");
            }
            ArrayMaths am = new ArrayMaths(aa);
            ArrayMaths as_ = am.sort();
            decimal[] bb = as_.getArray_as_decimal();
            decimal sum = decimal.Zero;
            for (int i = n/4; i < 3*n/4; i++)
            {
                sum = sum + (bb[i]);
            }
            sum = sum*( (2.0M/n));
            bb = null;
            return sum;
        }

        // Interquartile m_mean of a 1D array of doubles, aa
        public static double interQuartileMean(double[] aa)
        {
            int n = aa.Length;
            if (n < 4)
            {
                throw new ArgumentException("At least 4 array elements needed");
            }
            double[] bb = Fmath.selectionSort(aa);
            double sum = 0.0D;
            for (int i = n/4; i < 3*n/4; i++)
            {
                sum += bb[i];
            }
            return 2.0*sum/(n);
        }

        // Interquartile m_mean of a 1D array of floats, aa
        public static float interQuartileMean(float[] aa)
        {
            int n = aa.Length;
            if (n < 4)
            {
                throw new ArgumentException("At least 4 array elements needed");
            }
            float[] bb = Fmath.selectionSort(aa);
            float sum = 0.0F;
            for (int i = n/4; i < 3*n/4; i++)
            {
                sum += bb[i];
            }
            return 2.0F*sum/(n);
        }

        // ROOT MEAN SQUARES

        // Root m_mean square (rms) of a 1D array of doubles, aa
        public static double rms(double[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i]*aa[i];
            }
            return Math.Sqrt(sum/(n));
        }

        // Root m_mean square (rms) of a 1D array of floats, aa
        public static float rms(float[] aa)
        {
            int n = aa.Length;
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += aa[i]*aa[i];
            }
            sum /= n;

            return (float) Math.Sqrt(sum);
        }

        // Root m_mean square (rms) of a 1D array of decimal, aa
        public static double rms(decimal[] aa)
        {
            int n = aa.Length;
            decimal sum = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sum = sum + (aa[i]*(aa[i]));
            }
            sum = sum/(((n)));
            double ret = Math.Sqrt( (double)sum);
            sum = 0;
            return ret;
        }

        // Root m_mean square (rms) of a 1D array of long, aa
        public static double rms(long[] aa)
        {
            int n = aa.Length;
            decimal sum = decimal.Zero;
            decimal bd = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                bd = (aa[i]);
                sum = sum + (bd*(bd));
            }
            sum = sum/(((n)));
            double ret = Math.Sqrt( (double)sum);
            bd = 0;
            sum = 0;
            return ret;
        }

        // WEIGHTED ROOT MEAN SQUARES

        // Weighted root m_mean square (rms) of a 1D array of doubles, aa
        public static double rms(double[] aa, double[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            double sumw = 0.0D;
            double[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw += weight[i];
            }
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += weight[i]*aa[i]*aa[i];
            }
            return Math.Sqrt(sum/sumw);
        }

        // Weighted root m_mean square (rms) of a 1D array of floats, aa
        public static float rms(float[] aa, float[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            double sumw = 0.0F;
            float[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw += weight[i];
            }
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += weight[i]*aa[i]*aa[i];
            }
            return (float) Math.Sqrt(sum/sumw);
        }

        // Weighted root m_mean square (rms) of a 1D array of decimal, aa
        public static double rms(decimal[] aa, decimal[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }

            decimal sumw = decimal.Zero;
            decimal[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumw = sumw + (weight[i]);
            }

            decimal sum = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sum = sum + ((aa[i]*(aa[i]))*(weight[i]));
            }
            sum = sum/(sumw);
            double ret = Math.Sqrt( (double)sum);
            sum = 0;
            weight = null;
            return ret;
        }

        // Weighted root m_mean square (rms) of a 1D array of long, aa
        public static double rms(long[] aa, long[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }


            ArrayMaths amaa = new ArrayMaths(aa);
            ArrayMaths amww = new ArrayMaths(ww);
            return rms(amaa.array_as_decimal(), amww.array_as_decimal());
        }


        // STANDARD DEVIATIONS  (STATIC METHODS)

        // Standard deviation of a 1D array of decimals, aa
        public static double standardDeviation(decimal[] aa)
        {
            return Math.Sqrt( (double)variance(aa));
        }

        // Standard deviation of a 1D array of ComplexClass, aa
        public static ComplexClass standardDeviation(ComplexClass[] aa)
        {
            return ComplexClass.Sqrt(variance(aa));
        }

        // Standard deviation of a 1D array of ComplexClass, aa, conjugate formula
        public static double standardDeviationConjugateCalcn(ComplexClass[] aa)
        {
            return Math.Sqrt(varianceConjugateCalcn(aa));
        }

        // Standard deviation of the moduli of a 1D array of ComplexClass aa
        public static double standardDeviationModuli(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_modulus_of_Complex();
            double standardDeviation = Stat.standardDeviation(rl);
            return standardDeviation;
        }

        // Standard deviation of the real parts of a 1D array of ComplexClass aa
        public static double standardDeviationRealParts(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_real_part_of_Complex();
            double standardDeviation = Stat.standardDeviation(rl);
            return standardDeviation;
        }

        // Standard deviation of the imaginary parts of a 1D array of ComplexClass aa
        public static double standardDeviationImaginaryParts(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] im = am.array_as_imaginary_part_of_Complex();
            double standardDeviation = Stat.standardDeviation(im);
            return standardDeviation;
        }

        // Standard deviation of a 1D array of doubles, aa
        public static double standardDeviation(double[] aa)
        {
            return Math.Sqrt(variance(aa));
        }

        // Standard deviation of a 1D array of floats, aa
        public static float standardDeviation(float[] aa)
        {
            return (float) Math.Sqrt(variance(aa));
        }

        // Standard deviation of a 1D array of int, aa
        public static double standardDeviation(int[] aa)
        {
            return Math.Sqrt(variance(aa));
        }

        // Standard deviation of a 1D array of long, aa
        public static double standardDeviation(long[] aa)
        {
            return Math.Sqrt( (double)variance(aa));
        }

        // Weighted standard deviation of a 1D array of ComplexClass, aa
        public static ComplexClass standardDeviation(ComplexClass[] aa, ComplexClass[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            return ComplexClass.Sqrt(variance(aa, ww));
        }

        // Weighted standard deviation of a 1D array of ComplexClass, aa, using conjugate formula
        public static double standardDeviationConjugateCalcn(ComplexClass[] aa, ComplexClass[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            return Math.Sqrt(varianceConjugateCalcn(aa, ww));
        }

        // Weighted standard deviation of the moduli of a 1D array of ComplexClass aa
        public static double standardDeviationModuli(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_modulus_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_modulus_of_Complex();
            double standardDeviation = Stat.standardDeviation(rl, wt);
            return standardDeviation;
        }

        // Weighted standard deviation of the real parts of a 1D array of ComplexClass aa
        public static double standardDeviationRealParts(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_real_part_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_real_part_of_Complex();
            double standardDeviation = Stat.standardDeviation(rl, wt);
            return standardDeviation;
        }

        // Weighted standard deviation of the imaginary parts of a 1D array of ComplexClass aa
        public static double standardDeviationImaginaryParts(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] im = am.array_as_imaginary_part_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_imaginary_part_of_Complex();
            double standardDeviation = Stat.standardDeviation(im, wt);
            return standardDeviation;
        }


        // Weighted standard deviation of a 1D array of decimal, aa
        public static double standardDeviation(decimal[] aa, decimal[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            return Math.Sqrt( (double)variance(aa, ww));
        }

        // Weighted standard deviation of a 1D array of long, aa
        public static double standardDeviation(long[] aa, long[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            return Math.Sqrt((double)variance(aa, ww));
        }

        // Weighted standard deviation of a 1D array of doubles, aa
        public static double standardDeviation(double[] aa, double[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            return Math.Sqrt(variance(aa, ww));
        }

        // Weighted standard deviation of a 1D array of floats, aa
        public static float standardDeviation(float[] aa, float[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            return (float) Math.Sqrt(variance(aa, ww));
        }


        // VOLATILITIES

        // volatility   log  
        public static double volatilityLogChange(decimal[] array)
        {
            int n = array.Length - 1;
            double[] change = new double[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = Math.Log( (double)(array[i + 1]/(array[i])));
            }
            return standardDeviation(change);
        }

        // volatility   log  (long)
        public static double volatilityLogChange(long[] array)
        {
            int n = array.Length - 1;
            double[] change = new double[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = Math.Log((((array[i + 1]))*1.0/((array[i]))));
            }
            return standardDeviation(change);
        }

        // volatility   log  (doubles)
        public static double volatilityLogChange(double[] array)
        {
            int n = array.Length - 1;
            double[] change = new double[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = Math.Log(array[i + 1]/array[i]);
            }
            return standardDeviation(change);
        }

        // volatility   log  (floats)
        public static float volatilityLogChange(float[] array)
        {
            int n = array.Length - 1;
            float[] change = new float[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = (float) Math.Log(array[i + 1]/array[i]);
            }
            return standardDeviation(change);
        }

        // volatility   percentage 
        public static double volatilityPerCentChange(decimal[] array)
        {
            int n = array.Length - 1;
            double[] change = new double[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = (double) ((array[i + 1] + (array[i]))*(( (100.0M))/(array[i])));
            }
            return standardDeviation(change);
        }

        // volatility   percentage (integer)
        public static double volatilityPerCentChange(long[] array)
        {
            int n = array.Length - 1;
            double[] change = new double[n];
            ArrayMaths am = new ArrayMaths(array);
            decimal[] bd = am.getArray_as_decimal();
            for (int i = 0; i < n; i++)
            {
                change[i] =  (double)((bd[i + 1] + (bd[i]))*( (100.0M))/(bd[i]));
            }
            bd = null;
            return standardDeviation(change);
        }

        // volatility   percentage 
        public static double volatilityPerCentChange(double[] array)
        {
            int n = array.Length - 1;
            double[] change = new double[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = (array[i + 1] - array[i])*100.0D/array[i];
            }
            return standardDeviation(change);
        }

        // volatility   percentage (float)
        public static double volatilityPerCentChange(float[] array)
        {
            int n = array.Length - 1;
            float[] change = new float[n];
            for (int i = 0; i < n; i++)
            {
                change[i] = (array[i + 1] - array[i])*100.0F/array[i];
            }
            return standardDeviation(change);
        }


        // COEFFICIENT OF VARIATION

        // Coefficient of variation of an array of long
        public static double coefficientOfVariation(long[] array)
        {
            return 100.0D*standardDeviation(array)/Math.Abs(mean(array));
        }

        // Coefficient of variation of an array of decimals
        public static double coefficientOfVariation(decimal[] array)
        {
            return 100.0*
                   (standardDeviation(array)/ (double)Math.Abs( mean(array)));
        }

        // Coefficient of variation of an array of doubles
        public static double coefficientOfVariation(double[] array)
        {
            return 100.0D*standardDeviation(array)/Math.Abs(mean(array));
        }

        // Coefficient of variation of an array of float
        public static float coefficientOfVariation(float[] array)
        {
            return 100.0F*standardDeviation(array)/Math.Abs(mean(array));
        }


        // WEIGHTED COEFFICIENT OF VARIATION

        // Weighted coefficient of variation of an array of long
        public static double coefficientOfVariation(
            long[] array,
            long[] weight)
        {
            int n = array.Length;
            if (n != weight.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            weight.Length + " are different");
            }

            return 100.0D*standardDeviation(array, weight)/ (double)Math.Abs( mean(array, weight));
        }

        // Weighted coefficient of variation of an array of decimals
        public static double coefficientOfVariation(decimal[] array, decimal[] weight)
        {
            int n = array.Length;
            if (n != weight.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            weight.Length + " are different");
            }

            return 100.0D*standardDeviation(array, weight)/ (double)Math.Abs( mean(array, weight));
        }

        // Weighted coefficient of variation of an array of doubles
        public static double coefficientOfVariation(double[] array, double[] weight)
        {
            int n = array.Length;
            if (n != weight.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            weight.Length + " are different");
            }

            return 100.0D*standardDeviation(array, weight)/Math.Abs(mean(array, weight));
        }

        // Weighted coefficient of variation of an array of float
        public static float coefficientOfVariation(float[] array, float[] weight)
        {
            int n = array.Length;
            if (n != weight.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            weight.Length + " are different");
            }

            return 100.0F*standardDeviation(array, weight)/Math.Abs(mean(array, weight));
        }


        // VARIANCE
        // Static methods
        // Variance of a 1D array of decimals, aa
        public static decimal variance(decimal[] aa)
        {
            int n = aa.Length;
            decimal sum = decimal.Zero;
            decimal mean = Stat.mean(aa);
            for (int i = 0; i < n; i++)
            {
                decimal hold = aa[i] - (mean);
                sum = sum + (hold*(hold));
            }
            decimal ret = sum/(((n - 1)));
            if (nFactorOptionS)
            {
                ret = sum/((n));
            }
            sum = 0;
            mean = 0;
            return ret;
        }


        // Variance of a 1D array of ints, aa
        public static decimal variance(long[] aa)
        {
            int n = aa.Length;
            decimal sum = decimal.Zero;
            decimal mean = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sum = sum + ((aa[i]));
            }
            mean = sum/((n));
            sum = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                decimal hold = (aa[i]) - (mean);
                sum = sum + (hold*(hold));
            }
            decimal ret = sum/(((n - 1)));
            if (nFactorOptionS)
            {
                ret = sum/(n);
            }
            sum = 0;
            mean = 0;
            return ret;
        }

        // Variance of a 1D array of ComplexClass, aa
        public static ComplexClass variance(ComplexClass[] aa)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            ComplexClass mean = Stat.mean(aa);
            for (int i = 0; i < n; i++)
            {
                ComplexClass hold = new ComplexClass(aa[i]).minus(mean);
                sum = sum.plus(hold.times(hold));
            }
            ComplexClass ret = sum.over(new ComplexClass( (n - 1)));
            if (nFactorOptionS)
            {
                ret = sum.over(new ComplexClass( n));
            }
            return ret;
        }

        // Variance of a 1D array of ComplexClass, aa, using conjugate formula
        public static double varianceConjugateCalcn(ComplexClass[] aa)
        {
            int n = aa.Length;
            ComplexClass sum = ComplexClass.zero();
            ComplexClass mean = Stat.mean(aa);
            for (int i = 0; i < n; i++)
            {
                ComplexClass hold = new ComplexClass(aa[i]).minus(mean);
                sum = sum.plus(hold.times(hold.conjugate()));
            }
            double ret = sum.getReal()/(n - 1);
            if (nFactorOptionS)
            {
                ret = sum.getReal()/n;
            }
            return ret;
        }


        // Variance of the moduli of a 1D array of ComplexClass aa
        public static double varianceModuli(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_modulus_of_Complex();
            double variance = Stat.variance(rl);
            return variance;
        }

        // Variance of the real parts of a 1D array of ComplexClass aa
        public static double varianceRealParts(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_real_part_of_Complex();
            double variance = Stat.variance(rl);
            return variance;
        }

        // Variance of the imaginary parts of a 1D array of ComplexClass aa
        public static double varianceImaginaryParts(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] im = am.array_as_imaginary_part_of_Complex();
            double variance = Stat.variance(im);
            return variance;
        }

        // Variance of a 1D array of doubles, aa
        public static double variance(double[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            double mean = Stat.mean(aa);
            sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += Fmath.square(aa[i] - mean);
            }
            double ret = sum/((n - 1));
            if (nFactorOptionS)
            {
                ret = sum/(n);
            }
            return ret;
        }

        // Variance of a 1D array of floats, aa
        public static float variance(float[] aa)
        {
            int n = aa.Length;
            float sum = 0.0F;
            float mean = Stat.mean(aa);
            for (int i = 0; i < n; i++)
            {
                sum += Fmath.square(aa[i] - mean);
            }
            float ret = sum/((n - 1));
            if (nFactorOptionS)
            {
                ret = sum/(n);
            }
            return ret;
        }

        // Variance of a 1D array of int, aa
        public static double variance(int[] aa)
        {
            int n = aa.Length;
            double sum = 0.0D;
            double mean = Stat.mean(aa);
            for (int i = 0; i < n; i++)
            {
                sum += Fmath.square(aa[i] - mean);
            }
            double ret = sum/((n - 1));
            if (nFactorOptionS)
            {
                ret = sum/(n);
            }
            return ret;
        }

        // Weighted variance of a 1D array of doubles, aa
        public static double variance(double[] aa, double[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double nn = effectiveSampleNumber(ww);
            double nterm = nn/(nn - 1.0);
            if (nFactorOptionS)
            {
                nterm = 1.0;
            }

            double sumx = 0.0D, sumw = 0.0D, mean = 0.0D;
            double[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumx += aa[i]*weight[i];
                sumw += weight[i];
            }
            mean = sumx/sumw;
            sumx = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sumx += weight[i]*Fmath.square(aa[i] - mean);
            }
            return sumx*nterm/sumw;
        }

        // Weighted variance of a 1D array of floats, aa
        public static float variance(float[] aa, float[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            float nn = effectiveSampleNumber(ww);
            float nterm = nn/(nn - 1.0F);
            if (nFactorOptionS)
            {
                nterm = 1.0F;
            }

            float sumx = 0.0F, sumw = 0.0F, mean = 0.0F;
            float[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumx += aa[i]*weight[i];
                sumw += weight[i];
            }
            mean = sumx/sumw;
            sumx = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sumx += weight[i]*Fmath.square(aa[i] - mean);
            }
            return sumx*nterm/sumw;
        }

        // Weighted variance of a 1D array of ComplexClass aa
        public static ComplexClass variance(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ComplexClass nn = effectiveSampleNumber(ww);
            ComplexClass nterm = nn.over(nn.minus(1.0));
            if (nFactorOptionS)
            {
                nterm = ComplexClass.plusOne();
            }
            ComplexClass sumx = ComplexClass.zero();
            ComplexClass sumw = ComplexClass.zero();
            ComplexClass mean = ComplexClass.zero();
            ComplexClass[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumx = sumx.plus(aa[i].times(weight[i]));
                sumw = sumw.plus(weight[i]);
            }
            mean = sumx.over(sumw);
            sumx = ComplexClass.zero();
            for (int i = 0; i < n; i++)
            {
                ComplexClass hold = aa[i].minus(mean);
                sumx = sumx.plus(weight[i].times(hold).times(hold));
            }
            return (sumx.times(nterm)).over(sumw);
        }

        // Weighted variance of a 1D array of ComplexClass aa, using conjugate formula
        public static double varianceConjugateCalcn(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double nn = effectiveSampleNumberConjugateCalcn(ww);
            double nterm = nn/(nn - 1.0);
            if (nFactorOptionS)
            {
                nterm = 1.0;
            }
            ComplexClass sumx = ComplexClass.zero();
            ComplexClass sumw = ComplexClass.zero();
            ComplexClass sumwc = ComplexClass.zero();
            ComplexClass mean = ComplexClass.zero();
            Stat st = new Stat(ww);
            st = st.invert();
            ComplexClass[] weight = st.array_as_Complex();
            for (int i = 0; i < n; i++)
            {
                sumx = sumx.plus(aa[i].times(weight[i].times(weight[i])));
                sumw = sumw.plus(weight[i].times(weight[i]));
                sumwc = sumwc.plus(weight[i].times(weight[i].conjugate()));
            }
            mean = sumx.over(sumw);
            sumx = ComplexClass.zero();

            for (int i = 0; i < n; i++)
            {
                ComplexClass hold = aa[i].minus(mean);
                sumx = sumx.plus((weight[i].times(weight[i].conjugate())).times(hold).times(hold.conjugate()));
            }
            return nterm*((sumx.times(nterm)).over(sumwc)).getReal();
        }

        // Weighted variance of the moduli of a 1D array of ComplexClass aa
        public static double varianceModuli(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_modulus_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_modulus_of_Complex();
            double variance = Stat.variance(rl, wt);
            return variance;
        }

        // Weighted variance of the real parts of a 1D array of ComplexClass aa
        public static double varianceRealParts(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_real_part_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_real_part_of_Complex();
            double variance = Stat.variance(rl, wt);
            return variance;
        }

        // Weighted variance of the imaginary parts of a 1D array of ComplexClass aa
        public static double varianceImaginaryParts(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] im = am.array_as_imaginary_part_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_imaginary_part_of_Complex();
            double variance = Stat.variance(im, wt);
            return variance;
        }


        public static decimal variance(decimal[] aa, decimal[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            decimal nn = effectiveSampleNumber(ww);
            decimal nterm = nn/(nn - (decimal.One));
            if (nFactorOptionS)
            {
                nterm = decimal.One;
            }
            decimal sumx = decimal.Zero;
            decimal sumw = decimal.Zero;
            decimal mean = decimal.Zero;
            decimal[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumx = sumx + (aa[i]*(weight[i]));
                sumw = sumw + (weight[i]);
            }
            mean = sumx/(sumw);
            sumx = decimal.Zero;
            for (int i = 0; i < n; i++)
            {
                sumx = sumx + (weight[i]*(aa[i] - (mean))*(aa[i] - (mean)));
            }
            sumx = (sumx*(nterm)/(sumw));
            sumw = 0;
            mean = 0;
            weight = null;
            nn = 0;
            nterm = 0;
            return sumx;
        }

        public static decimal variance(long[] aa, long[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths aab = new ArrayMaths(aa);
            ArrayMaths wwb = new ArrayMaths(ww);
            return variance(aab.array_as_decimal(), wwb.array_as_decimal());
        }


        // STANDARD ERROR OF THE MEAN

        // Standard error of the m_mean of a 1D array of decimals, aa
        public static double standardError(decimal[] aa)
        {
            return Math.Sqrt((double)variance(aa)/aa.Length);
        }

        // Standard error of the m_mean of a 1D array of ints, aa
        public static double standardError(long[] aa)
        {
            return Math.Sqrt((double)variance(aa)/aa.Length);
        }

        // Standard error of the m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass standardError(ComplexClass[] aa)
        {
            return ComplexClass.Sqrt(variance(aa).over(aa.Length));
        }

        // Standard error of the m_mean of a 1D array of ComplexClass, aa, conjugate formula
        public static double standardErrorConjugateCalcn(ComplexClass[] aa)
        {
            return Math.Sqrt(varianceConjugateCalcn(aa)/aa.Length);
        }

        // Standard error of the moduli of a 1D array of ComplexClass aa
        public static double standardErrorModuli(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_modulus_of_Complex();
            return standardError(rl);
        }

        // Standard error of the real parts of a 1D array of ComplexClass aa
        public static double standardErrorRealParts(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_real_part_of_Complex();
            return standardError(rl);
        }

        // Standard error of the imaginary parts of a 1D array of ComplexClass aa
        public static double standardErrorImaginaryParts(ComplexClass[] aa)
        {
            ArrayMaths am = new ArrayMaths(aa);
            double[] im = am.array_as_imaginary_part_of_Complex();
            return standardError(im);
        }

        // Standard error of the m_mean of a 1D array of doubles, aa
        public static double standardError(double[] aa)
        {
            return Math.Sqrt(variance(aa)/aa.Length);
        }

        // Standard error of the m_mean of a 1D array of floats, aa
        public static float standardError(float[] aa)
        {
            return (float) Math.Sqrt(variance(aa)/aa.Length);
        }

        // Standard error of the m_mean of a 1D array of int, aa
        public static double standardError(int[] aa)
        {
            return Math.Sqrt(variance(aa)/aa.Length);
        }


        // Standard error of the weighted m_mean of a 1D array of ComplexClass, aa
        public static ComplexClass standardError(ComplexClass[] aa, ComplexClass[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ComplexClass effectiveNumber = effectiveSampleNumber(ww);
            return ComplexClass.Sqrt((variance(aa, ww)).over(effectiveNumber));
        }

        // Standard error of the weighted m_mean of a 1D array of ComplexClass, aa, using conjugate calculation
        public static double standardErrorConjugateCalcn(ComplexClass[] aa, ComplexClass[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double effectiveNumber = effectiveSampleNumberConjugateCalcn(ww);
            return Math.Sqrt(varianceConjugateCalcn(aa, ww)/effectiveNumber);
        }

        // Weighted standard error of the moduli of a 1D array of ComplexClass aa
        public static double standardErrorModuli(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_modulus_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_modulus_of_Complex();
            return standardError(rl, wt);
        }

        // Weighted standard error of the real parts of a 1D array of ComplexClass aa
        public static double standardErrorRealParts(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] rl = am.array_as_real_part_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_real_part_of_Complex();
            return standardError(rl, wt);
        }

        // Weighted standard error of the imaginary parts of a 1D array of ComplexClass aa
        public static double standardErrorImaginaryParts(ComplexClass[] aa, ComplexClass[] ww)
        {
            int n = aa.Length;
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + n + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            ArrayMaths am = new ArrayMaths(aa);
            double[] im = am.array_as_imaginary_part_of_Complex();
            ArrayMaths wm = new ArrayMaths(ww);
            double[] wt = wm.array_as_imaginary_part_of_Complex();
            return standardError(im, wt);
        }


        // Standard error of the weighted m_mean of a 1D array of decimal, aa
        public static double standardError(decimal[] aa, decimal[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double effectiveNumber = (double)(effectiveSampleNumber(ww));
            return Math.Sqrt((double)variance(aa, ww)/effectiveNumber);
        }

        // Standard error of the weighted m_mean of a 1D array of long, aa
        public static double standardError(long[] aa, long[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double effectiveNumber = (double)(effectiveSampleNumber(ww));
            return Math.Sqrt((double)variance(aa, ww)/effectiveNumber);
        }

        // Standard error of the weighted m_mean of a 1D array of doubles, aa
        public static double standardError(double[] aa, double[] ww)
        {
            if (aa.Length != ww.Length)
            {
                throw new ArgumentException("Length of variable array, " + aa.Length + " and Length of weight array, " +
                                            ww.Length + " are different");
            }
            double effectiveNumber = effectiveSampleNumber(ww);
            return Math.Sqrt(variance(aa, ww)/effectiveNumber);
        }

        // Standard error of the weighted m_mean of a 1D array of floats, aa
        public static float standardError(float[] aa, float[] ww)
        {
            float effectiveNumber = effectiveSampleNumber(ww);
            return (float) Math.Sqrt(variance(aa, ww)/effectiveNumber);
        }

        // COVARIANCE

        // Covariance of two 1D arrays of doubles, xx and yy
        public static double covariance(double[] xx, double[] yy)
        {
            int n = xx.Length;
            if (n != yy.Length)
            {
                throw new ArgumentException("Length of x variable array, " + n + " and Length of y array, " + yy.Length +
                                            " are different");
            }
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }

            double sumx = 0.0D, meanx = 0.0D;
            double sumy = 0.0D, meany = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sumx += xx[i];
                sumy += yy[i];
            }
            meanx = sumx/(n);
            meany = sumy/(n);
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += (xx[i] - meanx)*(yy[i] - meany);
            }
            return sum/(denom);
        }

        // Covariance of two 1D arrays of floats, xx and yy
        public static float covariance(float[] xx, float[] yy)
        {
            int n = xx.Length;
            if (n != yy.Length)
            {
                throw new ArgumentException("Length of x variable array, " + n + " and Length of y array, " + yy.Length +
                                            " are different");
            }
            float denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }

            float sumx = 0.0F, meanx = 0.0F;
            float sumy = 0.0F, meany = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sumx += xx[i];
                sumy += yy[i];
            }
            meanx = sumx/(n);
            meany = sumy/(n);
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                sum += (xx[i] - meanx)*(yy[i] - meany);
            }
            return sum/(denom);
        }

        // Covariance of two 1D arrays of ints, xx and yy
        public static double covariance(int[] xx, int[] yy)
        {
            int n = xx.Length;
            if (n != yy.Length)
            {
                throw new ArgumentException("Length of x variable array, " + n + " and Length of y array, " + yy.Length +
                                            " are different");
            }
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }

            double sumx = 0.0D, meanx = 0.0D;
            double sumy = 0.0D, meany = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sumx += xx[i];
                sumy += yy[i];
            }
            meanx = sumx/(n);
            meany = sumy/(n);
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += (xx[i] - meanx)*(yy[i] - meany);
            }
            return sum/(denom);
        }

        // Covariance of two 1D arrays of ints, xx and yy
        public static double covariance(long[] xx, long[] yy)
        {
            int n = xx.Length;
            if (n != yy.Length)
            {
                throw new ArgumentException("Length of x variable array, " + n + " and Length of y array, " + yy.Length +
                                            " are different");
            }
            double denom = (n - 1);
            if (nFactorOptionS)
            {
                denom = n;
            }

            double sumx = 0.0D, meanx = 0.0D;
            double sumy = 0.0D, meany = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sumx += xx[i];
                sumy += yy[i];
            }
            meanx = sumx/(n);
            meany = sumy/(n);
            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += (xx[i] - meanx)*(yy[i] - meany);
            }
            return sum/(denom);
        }

        // Weighted covariance of two 1D arrays of doubles, xx and yy with weights ww
        public static double covariance(double[] xx, double[] yy, double[] ww)
        {
            int n = xx.Length;
            if (n != yy.Length)
            {
                throw new ArgumentException("Length of x variable array, " + n + " and Length of y array, " + yy.Length +
                                            " are different");
            }
            if (n != ww.Length)
            {
                throw new ArgumentException("Length of x variable array, " + n + " and Length of weight array, " +
                                            yy.Length + " are different");
            }
            double nn = effectiveSampleNumber(ww);
            double nterm = nn/(nn - 1.0);
            if (nFactorOptionS)
            {
                nterm = 1.0;
            }
            double sumx = 0.0D, sumy = 0.0D, sumw = 0.0D, meanx = 0.0D, meany = 0.0D;
            double[] weight = invertAndSquare(ww);
            for (int i = 0; i < n; i++)
            {
                sumx += xx[i]*weight[i];
                sumy += yy[i]*weight[i];
                sumw += weight[i];
            }
            meanx = sumx/sumw;
            meany = sumy/sumw;

            double sum = 0.0D;
            for (int i = 0; i < n; i++)
            {
                sum += weight[i]*(xx[i] - meanx)*(yy[i] - meany);
            }
            return sum*nterm/sumw;
        }


        // CORRELATION COEFFICIENT

        // Calculate correlation coefficient
        // x y data as double
        public static double CorrCoeff(double[] xx, double[] yy)
        {
            try
            {if(xx.Length < 3)
            {
                return 0;
            }

                //double temp0 = 0.0D, temp1 = 0.0D; // working variables
                int nData = xx.Length;
                if (yy.Length != nData)
                {
                    throw new ArgumentException("array lengths must be equal");
                }
                //int df = nData - 1;
                // means
                double mx = 0.0D;
                double my = 0.0D;
                for (int i = 0; i < nData; i++)
                {
                    mx += xx[i];
                    my += yy[i];
                }
                mx /= nData;
                my /= nData;

                // calculate sample variances
                double s2xx = 0.0D;
                double s2yy = 0.0D;
                double s2xy = 0.0D;
                for (int i = 0; i < nData; i++)
                {
                    s2xx += Fmath.square(xx[i] - mx);
                    s2yy += Fmath.square(yy[i] - my);
                    s2xy += (xx[i] - mx)*(yy[i] - my);
                }

                // calculate corelation coefficient
                double dblDenom = Math.Sqrt(s2xx*s2yy);
                dblDenom = dblDenom == 0 ? 1 : dblDenom;
                double sampleR = s2xy/dblDenom;

                if(double.IsNaN(sampleR) ||
                    double.IsInfinity(sampleR))
                {
                    throw new HCException("Invalid correlation");
                }

                return sampleR;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        // Calculate correlation coefficient
        // x y data as float
        public static float CorrCoeff(float[] x, float[] y)
        {
            int nData = x.Length;
            if (y.Length != nData)
            {
                throw new ArgumentException("array lengths must be equal");
            }
            int n = x.Length;
            double[] xx = new double[n];
            double[] yy = new double[n];
            for (int i = 0; i < n; i++)
            {
                xx[i] = x[i];
                yy[i] = y[i];
            }
            return (float) CorrCoeff(xx, yy);
        }

        // Calculate correlation coefficient
        // x y data as int
        public static double CorrCoeff(int[] x, int[] y)
        {
            int n = x.Length;
            if (y.Length != n)
            {
                throw new ArgumentException("array lengths must be equal");
            }

            double[] xx = new double[n];
            double[] yy = new double[n];
            for (int i = 0; i < n; i++)
            {
                xx[i] = x[i];
                yy[i] = y[i];
            }
            return CorrCoeff(xx, yy);
        }

        // Calculate weighted correlation coefficient
        // x y data and weights w as double
        public static double CorrCoeff(double[] x, double[] y, double[] w)
        {
            int n = x.Length;
            if (y.Length != n)
            {
                throw new ArgumentException("x and y array lengths must be equal");
            }
            if (w.Length != n)
            {
                throw new ArgumentException("x and weight array lengths must be equal");
            }

            double sxy = covariance(x, y, w);
            double sx = variance(x, w);
            double sy = variance(y, w);
            return sxy/Math.Sqrt(sx*sy);
        }

        // Calculate correlation coefficient
        // Binary data x and y
        // Input is the frequency matrix, F, elements, f(i,j)
        // f(0,0) - element00 - frequency of x and y both = 1
        // f(0,1) - element01 - frequency of x = 0 and y = 1
        // f(1,0) - element10 - frequency of x = 1 and y = 0
        // f(1,1) - element11 - frequency of x and y both = 0
        public static double CorrCoeff(int element00, int element01, int element10, int element11)
        {
            return ((element00*element11 - element01*element10))/
                   Math.Sqrt(((element00 + element01)*(element10 + element11)*(element00 + element10)*
                              (element01 + element11)));
        }

        // Calculate correlation coefficient
        // Binary data x and y
        // Input is the frequency matrix, F
        // F(0,0) - frequency of x and y both = 1
        // F(0,1) - frequency of x = 0 and y = 1
        // F(1,0) - frequency of x = 1 and y = 0
        // F(1,1) - frequency of x and y both = 0
        public static double CorrCoeff(int[,] freqMatrix)
        {
            double element00 = freqMatrix[0, 0];
            double element01 = freqMatrix[0, 1];
            double element10 = freqMatrix[1, 0];
            double element11 = freqMatrix[1, 1];
            return ((element00*element11 - element01*element10))/
                   Math.Sqrt(((element00 + element01)*(element10 + element11)*(element00 + element10)*
                              (element01 + element11)));
        }

        // Linear correlation coefficient cumulative probablity
        // old name calls renamed method
        public static double linearCorrCoeffProb(double rCoeff, int nu)
        {
            return corrCoeffProb(rCoeff, nu);
        }

        // Linear correlation coefficient cumulative probablity
        public static double corrCoeffProb(double rCoeff, int nu)
        {
            if (Math.Abs(rCoeff) > 1.0D)
            {
                throw new ArgumentException("|Correlation coefficient| > 1 :  " + rCoeff);
            }

            // Create instances of the classes holding the function evaluation methods
            CorrCoeff cc = new CorrCoeff();

            // Assign values to constant in the function
            cc.a = (nu - 2.0D)/2.0D;


            double integral = Integration.gaussQuad(cc, Math.Abs(rCoeff), 1.0D, 128);

            double preterm = Math.Exp(LogGammaFunct.logGamma2((nu + 1.0D)/2.0) - LogGammaFunct.logGamma2(nu/2.0D))/
                             Math.Sqrt(Math.PI);

            return preterm*integral;
        }

        // Linear correlation coefficient single probablity
        // old name calls renamed method
        public static double linearCorrCoeff(double rCoeff, int nu)
        {
            return corrCoeffPDF(rCoeff, nu);
        }

        // Linear correlation coefficient single probablity
        public static double corrCoeffPDF(double rCoeff, int nu)
        {
            if (Math.Abs(rCoeff) > 1.0D)
            {
                throw new ArgumentException("|Correlation coefficient| > 1 :  " + rCoeff);
            }

            double a = (nu - 2.0D)/2.0D;
            double y = Math.Pow((1.0D - Fmath.square(rCoeff)), a);

            double preterm = Math.Exp(LogGammaFunct.logGamma2((nu + 1.0D)/2.0) - LogGammaFunct.logGamma2(nu/2.0D))/
                             Math.Sqrt(Math.PI);

            return preterm*y;
        }

        // Linear correlation coefficient single probablity
        public static double corrCoeffPdf(double rCoeff, int nu)
        {
            if (Math.Abs(rCoeff) > 1.0D)
            {
                throw new ArgumentException("|Correlation coefficient| > 1 :  " + rCoeff);
            }

            double a = (nu - 2.0D)/2.0D;
            double y = Math.Pow((1.0D - Fmath.square(rCoeff)), a);

            double preterm = Math.Exp(LogGammaFunct.logGamma2((nu + 1.0D)/2.0) - LogGammaFunct.logGamma2(nu/2.0D))/
                             Math.Sqrt(Math.PI);

            return preterm*y;
        }

        // SHANNON ENTROPY (STATIC METHODS)
        // Shannon Entropy returned as bits
        public static double shannonEntropy(double[] p)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }

            return am.minusxLog2x().getSum_as_double();
        }

        // Shannon Entropy returned as bits
        public static double shannonEntropyBit(double[] p)
        {
            return shannonEntropy(p);
        }

        // Shannon Entropy returned as nats (nits)
        public static double shannonEntropyNat(double[] p)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }

            return am.minusxLogEx().getSum_as_double();
        }

        // Shannon Entropy returned as dits
        public static double shannonEntropyDit(double[] p)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }

            return am.minusxLog10x().getSum_as_double();
        }

        // Binary Shannon Entropy returned as bits
        public static double binaryShannonEntropy(double p)
        {
            if (p > 1.0)
            {
                throw new ArgumentException("The probabiliy, " + p + ",  must be less than or equal to 1");
            }
            if (p < 0.0)
            {
                throw new ArgumentException("The probabiliy, " + p + ",  must be greater than or equal to 0");
            }
            double entropy = 0.0D;
            if (p > 0.0D && p < 1.0D)
            {
                entropy = -p*Fmath.log2(p) - (1 - p)*Fmath.log2(1 - p);
            }
            return entropy;
        }

        // Binary Shannon Entropy returned as bits
        public static double binaryShannonEntropyBit(double p)
        {
            return binaryShannonEntropy(p);
        }

        // Binary Shannon Entropy returned as nats (nits)
        public static double binaryShannonEntropyNat(double p)
        {
            if (p > 1.0)
            {
                throw new ArgumentException("The probabiliy, " + p + ",  must be less than or equal to 1");
            }
            if (p < 0.0)
            {
                throw new ArgumentException("The probabiliy, " + p + ",  must be greater than or equal to 0");
            }
            double entropy = 0.0D;
            if (p > 0.0D && p < 1.0D)
            {
                entropy = -p*Math.Log(p) - (1 - p)*Math.Log(1 - p);
            }
            return entropy;
        }

        // Binary Shannon Entropy returned as dits
        public static double binaryShannonEntropyDit(double p)
        {
            if (p > 1.0)
            {
                throw new ArgumentException("The probabiliy, " + p + ",  must be less than or equal to 1");
            }
            if (p < 0.0)
            {
                throw new ArgumentException("The probabiliy, " + p + ",  must be greater than or equal to 0");
            }
            double entropy = 0.0D;
            if (p > 0.0D && p < 1.0D)
            {
                entropy = -p*Math.Log10(p) - (1 - p)*Math.Log10(1 - p);
            }
            return entropy;
        }

        // RENYI ENTROPY
        // Renyi Entropy returned as bits
        public static double renyiEntropy(double[] p, double alpha)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }
            if (alpha < 0.0D)
            {
                throw new ArgumentException("alpha, " + alpha + ", must be greater than or equal to 0");
            }
            double entropy = 0.0;
            if (alpha == 0.0D)
            {
                entropy = Fmath.log2(p.Length);
            }
            else
            {
                if (alpha == 1.0D)
                {
                    entropy = shannonEntropy(p);
                }
                else
                {
                    if (Fmath.isPlusInfinity(alpha))
                    {
                        entropy = -Fmath.log2(max);
                    }
                    else
                    {
                        if (alpha <= 3000)
                        {
                            am = am.Pow(alpha);
                            bool testUnderFlow = false;
                            if (am.getMaximum_as_double() == double.MinValue)
                            {
                                testUnderFlow = true;
                            }
                            entropy = Fmath.log2(am.getSum_as_double())/(1.0D - alpha);
                            if (Fmath.isPlusInfinity(entropy) || testUnderFlow)
                            {
                                entropy = -Fmath.log2(max);
                                double entropyMin = entropy;
                                PrintToScreen.WriteLine(
                                    "Stat: renyiEntropy/renyiEntopyBit: underflow or overflow in calculating the entropy");
                                bool test1 = true;
                                bool test2 = true;
                                bool test3 = true;
                                int iter = 0;
                                double alpha2 = alpha/2.0;
                                double entropy2 = 0.0;
                                while (test3)
                                {
                                    while (test1)
                                    {
                                        ArrayMaths am2 = new ArrayMaths(p);
                                        am2 = am2.Pow(alpha2);
                                        entropy2 = Fmath.log2(am2.getSum_as_double())/(1.0D - alpha2);
                                        if (Fmath.isPlusInfinity(entropy2))
                                        {
                                            alpha2 /= 2.0D;
                                            iter++;
                                            if (iter == 100000)
                                            {
                                                test1 = false;
                                                test2 = false;
                                            }
                                        }
                                        else
                                        {
                                            test1 = false;
                                        }
                                    }
                                    double alphaTest = alpha2 + 40.0D*alpha/1000.0D;
                                    ArrayMaths am3 = new ArrayMaths(p);
                                    am3 = am3.Pow(alphaTest);
                                    double entropy3 = Fmath.log2(am3.getSum_as_double())/(1.0D - alphaTest);
                                    if (!Fmath.isPlusInfinity(entropy3))
                                    {
                                        test3 = false;
                                    }
                                    else
                                    {
                                        alpha2 /= 2.0D;
                                    }
                                }
                                double entropyLast = entropy2;
                                double alphaLast = alpha2;
                                List<double> extrap = new List<double>();
                                if (test2)
                                {
                                    double diff = alpha2/1000.0D;
                                    test1 = true;
                                    while (test1)
                                    {
                                        extrap.Add(alpha2);
                                        extrap.Add(entropy2);
                                        entropyLast = entropy2;
                                        alphaLast = alpha2;
                                        alpha2 += diff;
                                        ArrayMaths am2 = new ArrayMaths(p);
                                        am2 = am2.Pow(alpha2);
                                        entropy2 = Fmath.log2(am2.getSum_as_double())/(1.0D - alpha2);
                                        if (Fmath.isPlusInfinity(entropy2))
                                        {
                                            test1 = false;
                                            entropy2 = entropyLast;
                                            alpha2 = alphaLast;
                                        }
                                    }
                                }
                                int nex = extrap.Count/2 - 20;
                                double[] alphaex = new double[nex];
                                double[] entroex = new double[nex];
                                int ii = -1;
                                for (int i = 0; i < nex; i++)
                                {
                                    alphaex[i] = (extrap[++ii]);
                                    entroex[i] = Math.Log((extrap[++ii]) - entropyMin);
                                }
                                RegressionAn reg = new RegressionAn(alphaex, entroex);
                                reg.linear();
                                double[] param = reg.getCoeff();
                                entropy = Math.Exp(param[0] + param[1]*alpha) + entropyMin;


                                PrintToScreen.WriteLine("An interpolated entropy of " + entropy +
                                                        " returned (see documentation for exponential interpolation)");
                                PrintToScreen.WriteLine("Lowest calculable value =  " +
                                                        (Math.Exp(entroex[nex - 1]) + entropyMin) + ", alpha = " +
                                                        alphaex[nex - 1]);
                                PrintToScreen.WriteLine("Minimum entropy value =  " + entropyMin + ", alpha = infinity");
                            }
                        }
                        else
                        {
                            entropy = -Fmath.log2(max);
                            PrintToScreen.WriteLine(
                                "Stat: renyiEntropy/renyiEntropyBit: underflow or overflow in calculating the entropy");
                            PrintToScreen.WriteLine("An interpolated entropy of " + entropy +
                                                    " returned (see documentation for exponential interpolation)");
                        }
                    }
                }
            }
            return entropy;
        }

        // Renyi Entropy returned as nats
        public static double renyiEntropyNat(double[] p, double alpha)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }
            if (alpha < 0.0D)
            {
                throw new ArgumentException("alpha, " + alpha + ", must be greater than or equal to 0");
            }
            double entropy = 0.0;
            if (alpha == 0.0D)
            {
                entropy = Math.Log(p.Length);
            }
            else
            {
                if (alpha == 1.0D)
                {
                    entropy = shannonEntropy(p);
                }
                else
                {
                    if (Fmath.isPlusInfinity(alpha))
                    {
                        entropy = -Math.Log(max);
                    }
                    else
                    {
                        if (alpha <= 3000)
                        {
                            am = am.Pow(alpha);
                            bool testUnderFlow = false;
                            if (am.getMaximum_as_double() == double.MinValue)
                            {
                                testUnderFlow = true;
                            }
                            entropy = Math.Log(am.getSum_as_double())/(1.0D - alpha);
                            if (Fmath.isPlusInfinity(entropy) || testUnderFlow)
                            {
                                entropy = -Math.Log(max);
                                double entropyMin = entropy;
                                PrintToScreen.WriteLine(
                                    "Stat: renyiEntropyNat: underflow or overflow in calculating the entropy");
                                bool test1 = true;
                                bool test2 = true;
                                bool test3 = true;
                                int iter = 0;
                                double alpha2 = alpha/2.0;
                                double entropy2 = 0.0;
                                while (test3)
                                {
                                    while (test1)
                                    {
                                        ArrayMaths am2 = new ArrayMaths(p);
                                        am2 = am2.Pow(alpha2);
                                        entropy2 = Math.Log(am2.getSum_as_double())/(1.0D - alpha2);
                                        if (Fmath.isPlusInfinity(entropy2))
                                        {
                                            alpha2 /= 2.0D;
                                            iter++;
                                            if (iter == 100000)
                                            {
                                                test1 = false;
                                                test2 = false;
                                            }
                                        }
                                        else
                                        {
                                            test1 = false;
                                        }
                                    }
                                    double alphaTest = alpha2 + 40.0D*alpha/1000.0D;
                                    ArrayMaths am3 = new ArrayMaths(p);
                                    am3 = am3.Pow(alphaTest);
                                    double entropy3 = Math.Log(am3.getSum_as_double())/(1.0D - alphaTest);
                                    if (!Fmath.isPlusInfinity(entropy3))
                                    {
                                        test3 = false;
                                    }
                                    else
                                    {
                                        alpha2 /= 2.0D;
                                    }
                                }
                                double entropyLast = entropy2;
                                double alphaLast = alpha2;
                                List<double> extrap = new List<double>();
                                if (test2)
                                {
                                    double diff = alpha2/1000.0D;
                                    test1 = true;
                                    while (test1)
                                    {
                                        extrap.Add(alpha2);
                                        extrap.Add(entropy2);
                                        entropyLast = entropy2;
                                        alphaLast = alpha2;
                                        alpha2 += diff;
                                        ArrayMaths am2 = new ArrayMaths(p);
                                        am2 = am2.Pow(alpha2);
                                        entropy2 = Math.Log(am2.getSum_as_double())/(1.0D - alpha2);
                                        if (Fmath.isPlusInfinity(entropy2))
                                        {
                                            test1 = false;
                                            entropy2 = entropyLast;
                                            alpha2 = alphaLast;
                                        }
                                    }
                                }
                                int nex = extrap.Count/2 - 20;
                                double[] alphaex = new double[nex];
                                double[] entroex = new double[nex];
                                int ii = -1;
                                for (int i = 0; i < nex; i++)
                                {
                                    alphaex[i] = (extrap[++ii]);
                                    entroex[i] = Math.Log((extrap[++ii]) - entropyMin);
                                }
                                RegressionAn reg = new RegressionAn(alphaex, entroex);
                                reg.linear();
                                double[] param = reg.getCoeff();
                                entropy = Math.Exp(param[0] + param[1]*alpha) + entropyMin;


                                PrintToScreen.WriteLine("An interpolated entropy of " + entropy +
                                                        " returned (see documentation for exponential interpolation)");
                                PrintToScreen.WriteLine("Lowest calculable value =  " +
                                                        (Math.Exp(entroex[nex - 1]) + entropyMin) + ", alpha = " +
                                                        alphaex[nex - 1]);
                                PrintToScreen.WriteLine("Minimum entropy value =  " + entropyMin + ", alpha = infinity");
                            }
                        }
                        else
                        {
                            entropy = -Math.Log(max);
                            PrintToScreen.WriteLine(
                                "Stat: renyiEntropyNat: underflow or overflow in calculating the entropy");
                            PrintToScreen.WriteLine("An interpolated entropy of " + entropy +
                                                    " returned (see documentation for exponential interpolation)");
                        }
                    }
                }
            }
            return entropy;
        }

        // Renyi Entropy returned as dits
        public static double renyiEntropyDit(double[] p, double alpha)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }
            if (alpha < 0.0D)
            {
                throw new ArgumentException("alpha, " + alpha + ", must be greater than or equal to 0");
            }
            double entropy = 0.0;
            if (alpha == 0.0D)
            {
                entropy = Math.Log10(p.Length);
            }
            else
            {
                if (alpha == 1.0D)
                {
                    entropy = shannonEntropy(p);
                }
                else
                {
                    if (Fmath.isPlusInfinity(alpha))
                    {
                        entropy = -Math.Log10(max);
                    }
                    else
                    {
                        if (alpha <= 3000)
                        {
                            am = am.Pow(alpha);
                            bool testUnderFlow = false;
                            if (am.getMaximum_as_double() == double.MinValue)
                            {
                                testUnderFlow = true;
                            }
                            entropy = Math.Log10(am.getSum_as_double())/(1.0D - alpha);
                            if (Fmath.isPlusInfinity(entropy) || testUnderFlow)
                            {
                                entropy = -Math.Log10(max);
                                double entropyMin = entropy;
                                PrintToScreen.WriteLine(
                                    "Stat: renyiEntropyDit: underflow or overflow in calculating the entropy");
                                bool test1 = true;
                                bool test2 = true;
                                bool test3 = true;
                                int iter = 0;
                                double alpha2 = alpha/2.0;
                                double entropy2 = 0.0;
                                while (test3)
                                {
                                    while (test1)
                                    {
                                        ArrayMaths am2 = new ArrayMaths(p);
                                        am2 = am2.Pow(alpha2);
                                        entropy2 = Math.Log10(am2.getSum_as_double())/(1.0D - alpha2);
                                        if (Fmath.isPlusInfinity(entropy2))
                                        {
                                            alpha2 /= 2.0D;
                                            iter++;
                                            if (iter == 100000)
                                            {
                                                test1 = false;
                                                test2 = false;
                                            }
                                        }
                                        else
                                        {
                                            test1 = false;
                                        }
                                    }
                                    double alphaTest = alpha2 + 40.0D*alpha/1000.0D;
                                    ArrayMaths am3 = new ArrayMaths(p);
                                    am3 = am3.Pow(alphaTest);
                                    double entropy3 = Math.Log10(am3.getSum_as_double())/(1.0D - alphaTest);
                                    if (!Fmath.isPlusInfinity(entropy3))
                                    {
                                        test3 = false;
                                    }
                                    else
                                    {
                                        alpha2 /= 2.0D;
                                    }
                                }
                                double entropyLast = entropy2;
                                double alphaLast = alpha2;
                                List<double> extrap = new List<double>();
                                if (test2)
                                {
                                    double diff = alpha2/1000.0D;
                                    test1 = true;
                                    while (test1)
                                    {
                                        extrap.Add(alpha2);
                                        extrap.Add(entropy2);
                                        entropyLast = entropy2;
                                        alphaLast = alpha2;
                                        alpha2 += diff;
                                        ArrayMaths am2 = new ArrayMaths(p);
                                        am2 = am2.Pow(alpha2);
                                        entropy2 = Math.Log10(am2.getSum_as_double())/(1.0D - alpha2);
                                        if (Fmath.isPlusInfinity(entropy2))
                                        {
                                            test1 = false;
                                            entropy2 = entropyLast;
                                            alpha2 = alphaLast;
                                        }
                                    }
                                }
                                int nex = extrap.Count/2 - 20;
                                double[] alphaex = new double[nex];
                                double[] entroex = new double[nex];
                                int ii = -1;
                                for (int i = 0; i < nex; i++)
                                {
                                    alphaex[i] = (extrap[++ii]);
                                    entroex[i] = Math.Log10((extrap[++ii]) - entropyMin);
                                }
                                RegressionAn reg = new RegressionAn(alphaex, entroex);
                                reg.linear();
                                double[] param = reg.getCoeff();
                                entropy = Math.Exp(param[0] + param[1]*alpha) + entropyMin;


                                PrintToScreen.WriteLine("An interpolated entropy of " + entropy +
                                                        " returned (see documentation for exponential interpolation)");
                                PrintToScreen.WriteLine("Lowest calculable value =  " +
                                                        (Math.Exp(entroex[nex - 1]) + entropyMin) + ", alpha = " +
                                                        alphaex[nex - 1]);
                                PrintToScreen.WriteLine("Minimum entropy value =  " + entropyMin + ", alpha = infinity");
                            }
                        }
                        else
                        {
                            entropy = -Math.Log10(max);
                            PrintToScreen.WriteLine(
                                "Stat: renyiEntropyDit: underflow or overflow in calculating the entropy");
                            PrintToScreen.WriteLine("An interpolated entropy of " + entropy +
                                                    " returned (see documentation for exponential interpolation)");
                        }
                    }
                }
            }
            return entropy;
        }


        // Renyi Entropy returned as bits
        public static double renyiEntropyBit(double[] p, double alpha)
        {
            return renyiEntropy(p, alpha);
        }


        // TSALLIS ENTROPY (STATIC METHODS)
        // Tsallis Entropy
        public static double tsallisEntropyNat(double[] p, double q)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0D)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0D)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }

            if (q == 1.0D)
            {
                return shannonEntropyNat(p);
            }
            else
            {
                am = am.Pow(q);
                return (1.0D - am.getSum_as_double())/(q - 1.0D);
            }
        }

        // GENERALIZED ENTROPY (STATIC METHOD)
        public static double generalizedEntropyOneNat(double[] p, double q, double r)
        {
            ArrayMaths am = new ArrayMaths(p);
            double max = am.getMaximum_as_double();
            if (max > 1.0D)
            {
                throw new ArgumentException(
                    "All probabilites must be less than or equal to 1; the maximum supplied probabilty is " + max);
            }
            double min = am.getMinimum_as_double();
            if (min < 0.0D)
            {
                throw new ArgumentException(
                    "All probabilites must be greater than or equal to 0; the minimum supplied probabilty is " + min);
            }
            double total = am.getSum_as_double();
            if (!Fmath.isEqualWithinPerCent(total, 1.0D, 0.1D))
            {
                throw new ArgumentException(
                    "the probabilites must add up to 1 within an error of 0.1%; they add up to " + total);
            }
            if (r == 0.0D)
            {
                return renyiEntropyNat(p, q);
            }
            else
            {
                if (r == 1.0D)
                {
                    return tsallisEntropyNat(p, q);
                }
                else
                {
                    if (q == 1.0D)
                    {
                        double[] tsen = new double[10];
                        double[] tsqq = new double[10];
                        double qq = 0.995;
                        for (int i = 0; i < 5; i++)
                        {
                            ArrayMaths am1 = am.Pow(qq);
                            tsen[i] = (1.0D - Math.Pow(am1.getSum_as_double(), r))/(r*(qq - 1.0));
                            tsqq[i] = qq;
                            qq += 0.001;
                        }
                        qq = 1.001;
                        for (int i = 5; i < 10; i++)
                        {
                            ArrayMaths am1 = am.Pow(qq);
                            tsen[i] = (1.0D - Math.Pow(am1.getSum_as_double(), r))/(r*(qq - 1.0));
                            tsqq[i] = qq;
                            qq += 0.001;
                        }
                        RegressionAn reg = new RegressionAn(tsqq, tsen);
                        reg.polynomial(2);
                        double[] param = reg.getCoeff();
                        return param[0] + param[1] + param[2];
                    }
                    else
                    {
                        am = am.Pow(q);
                        return (1.0D - Math.Pow(am.getSum_as_double(), r))/(r*(q - 1.0));
                    }
                }
            }
        }

        public static double generalisedEntropyOneNat(double[] p, double q, double r)
        {
            return generalizedEntropyOneNat(p, q, r);
        }


        // HISTOGRAMS

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
                        if (data[i] >= binWall[j] && data[i] <= binWall[j + 1]*(1.0D + histTol))
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
                if (Math.Abs(rem)/span > histTol)
                {
                    // readjust binWidth
                    bool testBw = true;
                    double incr = histTol/nBins;
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

        // Distribute data into bins to obtain histogram and plot histogram
        // zero bin position and upper limit provided
        public static double[,] histogramBinsPlot(double[] data, double binWidth, double binZero, double binUpper)
        {
            string xLegend = null;
            return histogramBinsPlot(data, binWidth, binZero, binUpper, xLegend);
        }

        // Distribute data into bins to obtain histogram and plot histogram
        // zero bin position, upper limit and x-axis legend provided
        public static double[,] histogramBinsPlot(double[] data, double binWidth, double binZero, double binUpper,
                                                  string xLegend)
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
                return histogramBinsPlot(newData, binWidth, binZero, xLegend);
            }
            else
            {
                return histogramBinsPlot(data, binWidth, binZero, xLegend);
            }
        }

        // Distribute data into bins to obtain histogram and plot the histogram
        // zero bin position provided
        public static double[,] histogramBinsPlot(double[] data, double binWidth, double binZero)
        {
            string xLegend = null;
            return histogramBinsPlot(data, binWidth, binZero, xLegend);
        }

        // Distribute data into bins to obtain histogram and plot the histogram
        // zero bin position and x-axis legend provided
        public static double[,] histogramBinsPlot(
            double[] data,
            double binWidth,
            double binZero,
            string xLegend)
        {
            return null;
            //double[,] results = histogramBins(data, binWidth, binZero);
            //int nBins = results.GetLength(1);
            //int nPoints = nBins*3+1;
            //double[,] cdata = PlotGraph.data(1, nPoints);
            //cdata[0,0]=binZero;
            //cdata[1,0]=0.0D;
            //int k=1;
            //for(int i=0; i<nBins; i++){
            //    cdata[0,k]=cdata[0,k-1];
            //    cdata[1,k]=results[1,i];
            //    k++;
            //    cdata[0,k]=cdata[0,k-1]+binWidth;
            //    cdata[1,k]=results[1,i];
            //    k++;
            //    cdata[0,k]=cdata[0,k-1];
            //    cdata[1,k]=0.0D;
            //    k++;
            //}

            //PlotGraph pg = new PlotGraph(cdata);
            //pg.setGraphTitle("Histogram:  Bin Width = "+binWidth);
            //pg.setLine(3);
            //pg.setPoint(0);
            //pg.setYaxisLegend("Frequency");
            //if(xLegend!=null)pg.setXaxisLegend(xLegend);
            //pg.plot();

            //return results;
        }

        // Distribute data into bins to obtain histogram and plot the histogram
        // zero bin position calculated
        public static double[,] histogramBinsPlot(double[] data, double binWidth)
        {
            string xLegend = null;
            return histogramBinsPlot(data, binWidth, xLegend);
        }

        // Distribute data into bins to obtain histogram and plot the histogram
        // zero bin position calculated, x-axis legend provided
        public static double[,] histogramBinsPlot(double[] data, double binWidth, string xLegend)
        {
            double dmin = Fmath.minimum(data);
            double dmax = Fmath.maximum(data);
            double span = dmax - dmin;
            int nBins = (int) Math.Ceiling(span/binWidth);
            double rem = (nBins)*binWidth - span;
            double binZero = dmin - rem/2.0D;
            return histogramBinsPlot(data, binWidth, binZero, xLegend);
        }


        // Return the Lanczos constant gamma
        public static double getLanczosGamma()
        {
            return lgfGamma;
        }

        // Return the Lanczos constant N (number of coeeficients + 1)
        public static int getLanczosN()
        {
            return lgfN;
        }

        // Return the Lanczos coeeficients
        public static double[] getLanczosCoeff()
        {
            int n = getLanczosN() + 1;
            double[] coef = new double[n];
            for (int i = 0; i < n; i++)
            {
                coef[i] = lgfCoeff[i];
            }
            return coef;
        }

        // Return the nearest smallest representable floating point number to zero with mantissa rounded to 1.0
        public static double getFpmin()
        {
            return FPMIN;
        }

        // FACTORIALS

        // factorial of n
        // argument and return are integer, therefore limited to 0<=n<=12
        // see below for long and double arguments
        public static int factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("n must be a positive integer");
            }
            if (n > 12)
            {
                throw new ArgumentException("n must less than 13 to avoid integer overflow\nTry long or double argument");
            }
            int f = 1;
            for (int i = 2; i <= n; i++)
            {
                f *= i;
            }
            return f;
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
        // Argument is of type double but must be, numerically, an integer
        // factorial returned as double but is, numerically, should be an integer
        // numerical rounding may makes this an approximation after n = 21
        public static double factorial(double n)
        {
            if (n < 0 || (n - Math.Floor(n)) != 0)
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

        // factorial of n
        // Argument is of type decimal but must be, numerically, an integer
        public static decimal factorial(decimal n)
        {
            if (n.CompareTo(decimal.Zero) == -1 || !Fmath.isInteger((double)n))
            {
                throw new ArgumentException(
                    "\nn must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
            }
            decimal one = decimal.One;
            decimal f = one;
            decimal iCount = 2.0M;
            while (iCount.CompareTo(n) != 1)
            {
                f = f*(iCount);
                iCount = iCount + (one);
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
                throw new ArgumentException("\nn, " + n +
                                            ", must be a positive integer\nIs a Gamma funtion [Fmath.gamma(x)] more appropriate?");
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


        // ENGSET EQUATION

        // returns the probablility that a customer will be rejected due to lack of resources
        // offeredTraffic:  total offeredtraffic in Erlangs
        // totalResouces:   total number of resources in the system
        // numberOfSources: number of sources
        public static double engsetProbability(double offeredTraffic, double totalResources, double numberOfSources)
        {
            if (totalResources < 1)
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(totalResources))
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be, arithmetically, an integer");
            }
            if (numberOfSources < 1)
            {
                throw new ArgumentException("number of sources, " + numberOfSources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(numberOfSources))
            {
                throw new ArgumentException("number of sources, " + numberOfSources +
                                            ", must be, arithmetically, an integer");
            }
            if (totalResources > numberOfSources - 1)
            {
                throw new ArgumentException("total resources, " + totalResources +
                                            ", must be less than or  equal to the number of sources minus one, " +
                                            (numberOfSources - 1));
            }
            if (offeredTraffic >= numberOfSources)
            {
                throw new ArgumentException("Number of sources, " + numberOfSources +
                                            ", must be greater than the offered traffic, " + offeredTraffic);
            }

            double prob = 0.0D;
            if (totalResources == 0.0D)
            {
                prob = 1.0D;
            }
            else
            {
                if (offeredTraffic == 0.0D)
                {
                    prob = 0.0D;
                }
                else
                {
                    // Set boundaries to the probability
                    double lowerBound = 0.0D;
                    double upperBound = 1.0D;

                    // Create instance of Engset Probability Function
                    EngsetProb engProb = new EngsetProb();

                    // Set function variables
                    engProb.offeredTraffic = offeredTraffic;
                    engProb.totalResources = totalResources;
                    engProb.numberOfSources = numberOfSources;

                    // Perform a root search
                    RealRoot eprt = new RealRoot();

                    // Supress error message if iteration limit reached
                    eprt.supressLimitReachedMessage();

                    prob = eprt.bisect(engProb, lowerBound, upperBound);
                }
            }
            return prob;
        }

        public static double engsetProbability(double offeredTraffic, long totalResources, long numberOfSources)
        {
            return engsetProbability(offeredTraffic, totalResources,  numberOfSources);
        }

        public static double engsetProbability(double offeredTraffic, int totalResources, int numberOfSources)
        {
            return engsetProbability(offeredTraffic, totalResources,  numberOfSources);
        }

        // Engset equation
        // returns the maximum total traffic in Erlangs
        // blockingProbability:    probablility that a customer will be rejected due to lack of resources
        // totalResouces:   total number of resources in the system
        // numberOfSources: number of sources
        public static double engsetLoad(double blockingProbability, double totalResources, double numberOfSources)
        {
            if (totalResources < 1)
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(totalResources))
            {
                throw new ArgumentException("Total resources, " + totalResources +
                                            ", must be, arithmetically, an integer");
            }
            if (numberOfSources < 1)
            {
                throw new ArgumentException("number of sources, " + numberOfSources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(numberOfSources))
            {
                throw new ArgumentException("number of sources, " + numberOfSources +
                                            ", must be, arithmetically, an integer");
            }
            if (totalResources > numberOfSources - 1)
            {
                throw new ArgumentException("total resources, " + totalResources +
                                            ", must be less than or  equal to the number of sources minus one, " +
                                            (numberOfSources - 1));
            }

            // Create instance of the class holding the Engset Load equation
            EngsetLoad eLfunc = new EngsetLoad();

            // Set instance variables
            eLfunc.blockingProbability = blockingProbability;
            eLfunc.totalResources = totalResources;
            eLfunc.numberOfSources = numberOfSources;

            // lower bound
            double lowerBound = 0.0D;
            // upper bound
            double upperBound = numberOfSources*0.999999999;
            // required tolerance
            double tolerance = 1e-6;

            // Create instance of RealRoot
            RealRoot realR = new RealRoot();

            // Set tolerance
            realR.setTolerance(tolerance);

            // Set bounds limits
            realR.noLowerBoundExtension();
            realR.noUpperBoundExtension();

            // Supress error message if iteration limit reached
            realR.supressLimitReachedMessage();

            // call root searching method
            double root = realR.bisect(eLfunc, lowerBound, upperBound);

            return root;
        }

        public static double engsetLoad(double blockingProbability, long totalResources, long numberOfSources)
        {
            return engsetLoad(blockingProbability, totalResources,  numberOfSources);
        }

        public static double engsetLoad(double blockingProbability, int totalResources, int numberOfSources)
        {
            return engsetLoad(blockingProbability, totalResources,  numberOfSources);
        }

        // Engset equation
        // returns the resources bracketing a blocking probability for a given total traffic and number of sources
        // blockingProbability:    probablility that a customer will be rejected due to lack of resources
        // totalResouces:   total number of resources in the system
        // numberOfSources: number of sources
        public static double[] engsetResources(double blockingProbability, double offeredTraffic, double numberOfSources)
        {
            if (numberOfSources < 1)
            {
                throw new ArgumentException("number of sources, " + numberOfSources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(numberOfSources))
            {
                throw new ArgumentException("number of sources, " + numberOfSources +
                                            ", must be, arithmetically, an integer");
            }

            double[] ret = new double[9];
            long counter = 1;
            double lastProb = double.NaN;
            double prob = double.NaN;
            bool test = true;
            while (test)
            {
                prob = engsetProbability(offeredTraffic, counter, numberOfSources);
                if (prob <= blockingProbability)
                {
                    ret[0] = counter;
                    ret[1] = prob;
                    ret[2] = engsetLoad(blockingProbability, counter, numberOfSources);
                    ret[3] = (counter - 1);
                    ret[4] = lastProb;
                    ret[5] = engsetLoad(blockingProbability, (counter - 1), numberOfSources);
                    ret[6] = blockingProbability;
                    ret[7] = offeredTraffic;
                    ret[8] = numberOfSources;
                    test = false;
                }
                else
                {
                    lastProb = prob;
                    counter++;
                    if (counter > (long) numberOfSources - 1L)
                    {
                        PrintToScreen.WriteLine("Method engsetResources: no solution found below the (sources-1), " +
                                                (numberOfSources - 1));
                        for (int i = 0; i < 8; i++)
                        {
                            ret[i] = double.NaN;
                        }
                        test = false;
                    }
                }
            }
            return ret;
        }

        public static double[] engsetResources(double blockingProbability, double totalTraffic, long numberOfSources)
        {
            return engsetResources(blockingProbability, totalTraffic,  numberOfSources);
        }

        public static double[] engsetResources(double blockingProbability, double totalTraffic, int numberOfSources)
        {
            return engsetResources(blockingProbability, totalTraffic,  numberOfSources);
        }


        // Engset equation
        // returns the number of sources bracketing a blocking probability for a given total traffic and given resources
        // blockingProbability:    probablility that a customer will be rejected due to lack of resources
        // totalResouces:   total number of resources in the system
        // numberOfSources: number of sources
        public static double[] engsetSources(double blockingProbability, double offeredTraffic, double resources)
        {
            if (resources < 1)
            {
                throw new ArgumentException("resources, " + resources +
                                            ", must be an integer greater than or equal to 1");
            }
            if (!Fmath.isInteger(resources))
            {
                throw new ArgumentException("resources, " + resources + ", must be, arithmetically, an integer");
            }

            double[] ret = new double[9];
            long counter = (long) resources + 1L;
            double lastProb = double.NaN;
            double prob = double.NaN;
            bool test = true;
            while (test)
            {
                prob = engsetProbability(offeredTraffic, resources, counter);
                if (prob >= blockingProbability)
                {
                    ret[0] = counter;
                    ret[1] = prob;
                    ret[2] = engsetLoad(blockingProbability, resources, counter);
                    ret[3] = (counter - 1L);
                    ret[4] = lastProb;
                    if ((counter - 1L) >= (long) (resources + 1L))
                    {
                        ret[5] = engsetLoad(blockingProbability, resources, (counter - 1L));
                    }
                    else
                    {
                        ret[5] = double.NaN;
                    }
                    ret[6] = blockingProbability;
                    ret[7] = offeredTraffic;
                    ret[8] = resources;
                    test = false;
                }
                else
                {
                    lastProb = prob;
                    counter++;
                    if (counter >= long.MaxValue)
                    {
                        PrintToScreen.WriteLine("Method engsetResources: no solution found below " + long.MaxValue +
                                                "sources");
                        for (int i = 0; i < 8; i++)
                        {
                            ret[i] = double.NaN;
                        }
                        test = false;
                    }
                }
            }
            return ret;
        }

        public static double[] engsetSources(double blockingProbability, double totalTraffic, long resources)
        {
            return engsetSources(blockingProbability, totalTraffic,  resources);
        }

        public static double[] engsetSources(double blockingProbability, double totalTraffic, int resources)
        {
            return engsetSources(blockingProbability, totalTraffic,  resources);
        }


        // LOG-NORMAL DISTRIBUTIONS (TWO AND THEE PARAMETER DISTRIBUTIONS)

        // TWO PARAMETER LOG-NORMAL DISTRIBUTION

        // Two parameter log-normal cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double logNormalCDF(double mu, double sigma, double upperLimit)
        {
            if (sigma < 0)
            {
                throw new ArgumentException("The parameter m_sigma, " + sigma +
                                            ", must be greater than or equal to zero");
            }
            if (upperLimit <= 0)
            {
                return 0.0D;
            }
            else
            {
                return 0.5D*(1.0D + ErrorFunct.ErrorFunction((Math.Log(upperLimit) - mu)/(sigma*Math.Sqrt(2))));
            }
        }

        public static double logNormalTwoParCDF(double mu, double sigma, double upperLimit)
        {
            return logNormalCDF(mu, sigma, upperLimit);
        }


        // Two parameter log-normal cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double logNormalCDF(double mu, double sigma, double lowerLimit, double upperLimit)
        {
            if (sigma < 0)
            {
                throw new ArgumentException("The parameter m_sigma, " + sigma +
                                            ", must be greater than or equal to zero");
            }
            if (upperLimit < lowerLimit)
            {
                throw new ArgumentException("The upper limit, " + upperLimit + ", must be greater than the " +
                                            lowerLimit);
            }

            double arg1 = 0.0D;
            double arg2 = 0.0D;
            double cdf = 0.0D;

            if (lowerLimit != upperLimit)
            {
                if (upperLimit > 0.0D)
                {
                    arg1 = 0.5D*(1.0D + ErrorFunct.ErrorFunction((Math.Log(upperLimit) - mu)/(sigma*Math.Sqrt(2))));
                }
                if (lowerLimit > 0.0D)
                {
                    arg2 = 0.5D*(1.0D + ErrorFunct.ErrorFunction((Math.Log(lowerLimit) - mu)/(sigma*Math.Sqrt(2))));
                }
                cdf = arg1 - arg2;
            }

            return cdf;
        }

        public static double logNormalTwoParCDF(double mu, double sigma, double lowerLimit, double upperLimit)
        {
            return logNormalCDF(mu, sigma, lowerLimit, upperLimit);
        }

        // Log-Normal Inverse Cumulative Distribution Function
        // Two parameter
        public static double logNormalInverseCDF(double mu, double sigma, double prob)
        {
            double alpha = 0.0;
            double beta = sigma;
            double gamma = Math.Exp(mu);

            return LogNormalThreeParFunct.logNormalInverseCDF(
                alpha,
                beta,
                gamma,
                prob);
        }

        // Log-Normal Inverse Cumulative Distribution Function
        // Two parameter
        public static double logNormaltwoParInverseCDF(double mu, double sigma, double prob)
        {
            double alpha = 0.0;
            double beta = sigma;
            double gamma = Math.Exp(mu);

            return LogNormalThreeParFunct.logNormalInverseCDF(
                alpha,
                beta,
                gamma,
                prob);
        }


        // Two parameter log-normal probability density function
        public static double logNormalPDF(double mu, double sigma, double x)
        {
            if (sigma < 0)
            {
                throw new ArgumentException("The parameter m_sigma, " + sigma +
                                            ", must be greater than or equal to zero");
            }
            if (x < 0)
            {
                return 0.0D;
            }
            else
            {
                return Math.Exp(-0.5D*Fmath.square((Math.Log(x) - mu)/sigma))/(x*sigma*Math.Sqrt(2.0D*Math.PI));
            }
        }

        public static double logNormalTwoParPDF(double mu, double sigma, double x)
        {
            return logNormalPDF(mu, sigma, x);
        }


        // Two parameter log-normal m_mean
        public static double logNormalMean(double mu, double sigma)
        {
            return Math.Exp(mu + sigma*sigma/2.0D);
        }

        public static double logNormalTwoParMean(double mu, double sigma)
        {
            return Math.Exp(mu + sigma*sigma/2.0D);
        }

        // Two parameter log-normal standard deviation
        public static double logNormalStandardDeviation(double mu, double sigma)
        {
            return logNormalStandDev(mu, sigma);
        }

        // Two parameter log-normal standard deviation
        public static double logNormalStandDev(double mu, double sigma)
        {
            double sigma2 = sigma*sigma;
            return Math.Sqrt((Math.Exp(sigma2) - 1.0D)*Math.Exp(2.0D*mu + sigma2));
        }

        public static double logNormalTwoParStandardDeviation(double mu, double sigma)
        {
            return logNormalTwoParStandDev(mu, sigma);
        }

        public static double logNormalTwoParStandDev(double mu, double sigma)
        {
            double sigma2 = sigma*sigma;
            return Math.Sqrt((Math.Exp(sigma2) - 1.0D)*Math.Exp(2.0D*mu + sigma2));
        }

        // Two parameter log-normal mode
        public static double logNormalMode(double mu, double sigma)
        {
            return Math.Exp(mu - sigma*sigma);
        }

        public static double logNormalTwoParMode(double mu, double sigma)
        {
            return Math.Exp(mu - sigma*sigma);
        }

        // Two parameter log-normal median
        public static double logNormalMedian(double mu)
        {
            return Math.Exp(mu);
        }

        public static double logNormalTwoParMedian(double mu)
        {
            return Math.Exp(mu);
        }

        // Returns an array of two parameter log-normal random deviates - clock seed
        public static double[] logNormalRand(double mu, double sigma, int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("The number of random deviates required, " + n +
                                            ", must be greater than zero");
            }
            if (sigma < 0)
            {
                throw new ArgumentException("The parameter m_sigma, " + sigma +
                                            ", must be greater than or equal to zero");
            }
            LogNormalDist psr = new LogNormalDist(
                mu,
                sigma,
                new RngWrapper());
            return psr.NextDoubleArr(n);
        }

        public static double[] logNormalTwoParRand(double mu, double sigma, int n)
        {
            return logNormalRand(mu, sigma, n);
        }

        // LogNormal order statistic medians (n points)
        // Two parametrs
        public static double[] logNormalOrderStatisticMedians(double mu, double sigma, int n)
        {
            double alpha = 0.0;
            double beta = sigma;
            double gamma = Math.Exp(mu);

            return LogNormalDist_3Params.logNormalOrderStatisticMedians(
                alpha,
                beta,
                gamma,
                n);
        }

        // LogNormal order statistic medians (n points)
        // Two parametrs
        public static double[] logNormalTwoParOrderStatisticMedians(double mu, double sigma, int n)
        {
            return logNormalOrderStatisticMedians(mu, sigma, n);
        }


        // Returns an array of two parameter log-normal random deviates - user supplied seed
        public static double[] logNormalRand(double mu, double sigma, int n, long seed)
        {
            if (n <= 0)
            {
                throw new ArgumentException("The number of random deviates required, " + n +
                                            ", must be greater than zero");
            }
            if (sigma < 0)
            {
                throw new ArgumentException("The parameter m_sigma, " + sigma +
                                            ", must be greater than or equal to zero");
            }

            LogNormalDist psr =
                new LogNormalDist(
                    mu,
                    sigma,
                    new RngWrapper((int) seed));
            return psr.NextDoubleArr(n);
        }


        public static double[] logNormalTwoParRand(double mu, double sigma, int n, long seed)
        {
            return logNormalRand(mu, sigma, n, seed);
        }


        // Returns an array of three parameter log-normal random deviates - clock seed
        public static double[] logNormalThreeParRand(double alpha, double beta, double gamma, int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("The number of random deviates required, " + n +
                                            ", must be greater than zero");
            }
            if (beta < 0)
            {
                throw new ArgumentException("The parameter beta, " + beta + ", must be greater than or equal to zero");
            }
            return LogNormalDist_3Params.logNormalThreeParArray(alpha, beta, gamma, n);
        }


        // LOGISTIC DISTRIBUTION
        // TWO PARAMETERS (See below for three parameter distribution)

        // Logistic cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticCDF(double mu, double beta, double upperlimit)
        {
            return 0.5D*(1.0D + Math.Tanh((upperlimit - mu)/(2.0D*beta)));
        }

        // Logistic cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticTwoParCDF(double mu, double beta, double upperlimit)
        {
            return 0.5D*(1.0D + Math.Tanh((upperlimit - mu)/(2.0D*beta)));
        }


        // Logistic cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticProb(double mu, double beta, double upperlimit)
        {
            return 0.5D*(1.0D + Math.Tanh((upperlimit - mu)/(2.0D*beta)));
        }


        // Logistic cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticCDF(double mu, double beta, double lowerlimit, double upperlimit)
        {
            double arg1 = 0.5D*(1.0D + Math.Tanh((lowerlimit - mu)/(2.0D*beta)));
            double arg2 = 0.5D*(1.0D + Math.Tanh((upperlimit - mu)/(2.0D*beta)));
            return arg2 - arg1;
        }

        // Logistic cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticTwoParCDF(double mu, double beta, double lowerlimit, double upperlimit)
        {
            double arg1 = 0.5D*(1.0D + Math.Tanh((lowerlimit - mu)/(2.0D*beta)));
            double arg2 = 0.5D*(1.0D + Math.Tanh((upperlimit - mu)/(2.0D*beta)));
            return arg2 - arg1;
        }

        // Logistic cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticProb(double mu, double beta, double lowerlimit, double upperlimit)
        {
            double arg1 = 0.5D*(1.0D + Math.Tanh((lowerlimit - mu)/(2.0D*beta)));
            double arg2 = 0.5D*(1.0D + Math.Tanh((upperlimit - mu)/(2.0D*beta)));
            return arg2 - arg1;
        }


        // Logistic Inverse Cumulative Density Function
        public static double logisticTwoParInverseCDF(double mu, double beta, double prob)
        {
            return logisticInverseCDF(mu, beta, prob);
        }

        // Logistic Inverse Cumulative Density Function
        public static double logisticInverseCDF(double mu, double beta, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = double.NegativeInfinity;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu - beta*Math.Log(1.0/prob - 1.0);
                }
            }

            return icdf;
        }

        // Logistic probability density function density function
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticPDF(double mu, double beta, double x)
        {
            return Fmath.square(Fmath.sech((x - mu)/(2.0D*beta)))/(4.0D*beta);
        }

        // Logistic probability density function density function
        // m_mu  =  location parameter, beta = scale parameter
        public static double logisticTwoParPDF(double mu, double beta, double x)
        {
            return Fmath.square(Fmath.sech((x - mu)/(2.0D*beta)))/(4.0D*beta);
        }

        // Logistic probability density function
        // m_mu  =  location parameter, beta = scale parameter
        public static double logistic(double mu, double beta, double x)
        {
            return Fmath.square(Fmath.sech((x - mu)/(2.0D*beta)))/(4.0D*beta);
        }

        // Returns an array of logistic distribution random deviates - clock seed
        // m_mu  =  location parameter, beta = scale parameter
        public static double[] logisticTwoParRand(double mu, double beta, int n)
        {
            return logisticRand(mu, beta, n);
        }

        // Returns an array of logistic distribution random deviates - clock seed
        // m_mu  =  location parameter, beta = scale parameter
        public static double[] logisticRand(double mu, double beta, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = 2.0D*beta*Fmath.atanh(2.0D*rr.NextDouble() - 1.0D) + mu;
            }
            return ran;
        }

        // Returns an array of Logistic random deviates - user provided seed
        // m_mu  =  location parameter, beta = scale parameter
        public static double[] logisticTwoParRand(double mu, double beta, int n, long seed)
        {
            return logisticRand(mu, beta, n, seed);
        }


        // Returns an array of Logistic random deviates - user provided seed
        // m_mu  =  location parameter, beta = scale parameter
        public static double[] logisticRand(double mu, double beta, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = 2.0D*beta*Fmath.atanh(2.0D*rr.NextDouble() - 1.0D) + mu;
            }
            return ran;
        }

        // Logistic order statistic medians (n points)
        public static double[] logisticOrderStatisticMedians(double mu, double beta, int n)
        {
            double nn = n;
            double[] losm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                losm[i] = logisticInverseCDF(mu, beta, uosm[i]);
            }
            return losm;
        }

        // Logistic order statistic medians (n points)
        public static double[] logisticTwoParOrderStatisticMedians(double mu, double beta, int n)
        {
            double nn = n;
            double[] losm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                losm[i] = logisticInverseCDF(mu, beta, uosm[i]);
            }
            return losm;
        }

        // Logistic distribution m_mean
        public static double logisticMean(double mu)
        {
            return mu;
        }

        // Logistic distribution m_mean
        public static double logisticTwoParMean(double mu)
        {
            return mu;
        }

        // Logistic distribution standard deviation
        public static double logisticStandardDeviation(double beta)
        {
            return logisticStandDev(beta);
        }

        // Logistic distribution standard deviation
        public static double logisticStandDev(double beta)
        {
            return Math.Sqrt(Fmath.square(Math.PI*beta)/3.0D);
        }

        // Logistic distribution standard deviation
        public static double logisticTwoParStandardDeviation(double beta)
        {
            return Math.Sqrt(Fmath.square(Math.PI*beta)/3.0D);
        }

        // Logistic distribution mode
        public static double logisticMode(double mu)
        {
            return mu;
        }

        // Logistic distribution mode
        public static double logisticTwoParMode(double mu)
        {
            return mu;
        }

        // Logistic distribution median
        public static double logisticMedian(double mu)
        {
            return mu;
        }

        // Logistic distribution median
        public static double logisticTwoParMedian(double mu)
        {
            return mu;
        }


        // LORENTZIAN DISTRIBUTION (CAUCHY DISTRIBUTION)

        // Lorentzian cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        public static double lorentzianProb(double mu, double gamma, double upperlimit)
        {
            double arg = (upperlimit - mu)/(gamma/2.0D);
            return (1.0D/Math.PI)*(Math.Atan(arg) + Math.PI/2.0);
        }

        // Lorentzian cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double lorentzianCDF(double mu, double gamma, double lowerlimit, double upperlimit)
        {
            double arg1 = (upperlimit - mu)/(gamma/2.0D);
            double arg2 = (lowerlimit - mu)/(gamma/2.0D);
            return (1.0D/Math.PI)*(Math.Atan(arg1) - Math.Atan(arg2));
        }

        // Lorentzian cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double lorentzianProb(double mu, double gamma, double lowerlimit, double upperlimit)
        {
            double arg1 = (upperlimit - mu)/(gamma/2.0D);
            double arg2 = (lowerlimit - mu)/(gamma/2.0D);
            return (1.0D/Math.PI)*(Math.Atan(arg1) - Math.Atan(arg2));
        }

        // Lorentzian Inverse Cumulative Density Function
        public static double lorentzianInverseCDF(double mu, double gamma, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = double.NegativeInfinity;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu + gamma*Math.Tan(Math.PI*(prob - 0.5))/2.0;
                }
            }

            return icdf;
        }

        // Lorentzian probability density function
        public static double lorentzianPDF(double mu, double gamma, double x)
        {
            double arg = gamma/2.0D;
            return (1.0D/Math.PI)*arg/(Fmath.square(mu - x) + arg*arg);
        }

        // Lorentzian probability density function
        public static double lorentzian(double mu, double gamma, double x)
        {
            double arg = gamma/2.0D;
            return (1.0D/Math.PI)*arg/(Fmath.square(mu - x) + arg*arg);
        }


        // Returns an array of Lorentzian random deviates - clock seed
        // m_mu  =  the m_mean, gamma = half-height width, Length of array
        public static double[] lorentzianRand(double mu, double gamma, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Tan((rr.NextDouble() - 0.5)*Math.PI);
                ran[i] = ran[i]*gamma/2.0 + mu;
            }
            return ran;
        }

        // Returns an array of Lorentzian random deviates - user provided seed
        // m_mu  =  the m_mean, gamma = half-height width, Length of array
        public static double[] lorentzianRand(double mu, double gamma, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Tan((rr.NextDouble() - 0.5)*Math.PI);
                ran[i] = ran[i]*gamma/2.0 + mu;
            }
            return ran;
        }

        // Lorentzian order statistic medians (n points)
        public static double[] lorentzianOrderStatisticMedians(double mu, double gamma, int n)
        {
            double nn = n;
            double[] losm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                losm[i] = lorentzianInverseCDF(mu, gamma, uosm[i]);
            }
            return losm;
        }

        // POISSON DISTRIBUTION

        // Poisson Cumulative Distribution Function
        // probability that a number of Poisson random events will occur between 0 and k (inclusive)
        // k is an integer greater than equal to 1
        // m_mean  = m_mean of the Poisson distribution
        public static double poissonCDF(int k, double mean)
        {
            if (k < 1)
            {
                throw new ArgumentException("k must be an integer greater than or equal to 1");
            }
            return IncompleteGamaFunct.IncompleteGammaComplement(k, mean);
            //return Stat.incompleteGammaComplementary( k, m_mean);
        }

        // Poisson Cumulative Distribution Function
        // probability that a number of Poisson random events will occur between 0 and k (inclusive)
        // k is an integer greater than equal to 1
        // m_mean  = m_mean of the Poisson distribution
        public static double poissonProb(int k, double mean)
        {
            if (k < 1)
            {
                throw new ArgumentException("k must be an integer greater than or equal to 1");
            }
            return IncompleteGamaFunct.IncompleteGammaComplement(k, mean);
            //return Stat.incompleteGammaComplementary(k, m_mean);
        }

        // Poisson Probability Density Function
        // k is an integer greater than or equal to zero
        // m_mean  = m_mean of the Poisson distribution
        public static double poissonPDF(int k, double mean)
        {
            if (k < 0)
            {
                throw new ArgumentException("k must be an integer greater than or equal to 0");
            }
            return Math.Pow(mean, k)*Math.Exp(-mean)/factorial( k);
        }

        // Poisson Probability Density Function
        // k is an integer greater than or equal to zero
        // m_mean  = m_mean of the Poisson distribution
        public static double poisson(int k, double mean)
        {
            if (k < 0)
            {
                throw new ArgumentException("k must be an integer greater than or equal to 0");
            }
            return Math.Pow(mean, k)*Math.Exp(-mean)/factorial( k);
        }

        // Returns an array of Poisson random deviates - clock seed
        // m_mean  =  the m_mean,  n = Length of array
        // follows the ideas of Numerical Recipes
        public static double[] poissonRand(double mean, int n)
        {
            RngWrapper rr = new RngWrapper();
            double[] ran = poissonRandCalc(rr, mean, n);
            return ran;
        }

        // Returns an array of Poisson random deviates - user provided seed
        // m_mean  =  the m_mean,  n = Length of array
        // follows the ideas of Numerical Recipes
        public static double[] poissonRand(double mean, int n, long seed)
        {
            RngWrapper rr = new RngWrapper((int) seed);
            double[] ran = poissonRandCalc(rr, mean, n);
            return ran;
        }

        // Calculates and returns an array of Poisson random deviates
        private static double[] poissonRandCalc(RngWrapper rr, double mean, int n)
        {
            double[] ran = new double[n];
            double oldm = -1.0D;
            double expt = 0.0D;
            double em = 0.0D;
            double term = 0.0D;
            double sq = 0.0D;
            double lnMean = 0.0D;
            double yDev = 0.0D;

            if (mean < 12.0D)
            {
                for (int i = 0; i < n; i++)
                {
                    if (mean != oldm)
                    {
                        oldm = mean;
                        expt = Math.Exp(-mean);
                    }
                    em = -1.0D;
                    term = 1.0D;
                    do
                    {
                        ++em;
                        term *= rr.NextDouble();
                    }
                    while (term > expt);
                    ran[i] = em;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    if (mean != oldm)
                    {
                        oldm = mean;
                        sq = Math.Sqrt(2.0D*mean);
                        lnMean = Math.Log(mean);
                        expt = lnMean - LogGammaFunct.logGamma2(mean + 1.0D);
                    }
                    do
                    {
                        do
                        {
                            yDev = Math.Tan(Math.PI*rr.NextDouble());
                            em = sq*yDev + mean;
                        }
                        while (em < 0.0D);
                        em = Math.Floor(em);
                        term = 0.9D*(1.0D + yDev*yDev)*Math.Exp(em*lnMean - LogGammaFunct.logGamma2(em + 1.0D) - expt);
                    }
                    while (rr.NextDouble() > term);
                    ran[i] = em;
                }
            }
            return ran;
        }


        // CHI SQUARE DISTRIBUTION AND CHI SQUARE FUNCTIONS

        // Chi-Square Cumulative Distribution Function
        // probability that an observed chi-square value for a correct model should be less than chiSquare
        // m_nu  =  the degrees of freedom
        public static double chiSquareCDF(double chiSquare, int nu)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            return IncompleteGamaFunct.IncompleteGamma(nu/2.0D, chiSquare/2.0D);
        }

        // retained for compatability
        public static double chiSquareProb(double chiSquare, int nu)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            return IncompleteGamaFunct.IncompleteGamma(nu/2.0D, chiSquare/2.0D);
        }

        // Chi-Square Probability Density Function
        // m_nu  =  the degrees of freedom
        public static double chiSquarePDF(double chiSquare, int nu)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            double dnu = nu;
            return Math.Pow(0.5D, dnu/2.0D)*Math.Pow(chiSquare, dnu/2.0D - 1.0D)*Math.Exp(-chiSquare/2.0D)/
                   GammaFunct.Gamma(dnu/2.0D);
        }

        // Returns an array of Chi-Square random deviates - clock seed
        public static double[] chiSquareRand(int nu, int n)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            ChiSquareDist psr = new ChiSquareDist(
                nu,
                new RngWrapper());

            return psr.NextDoubleArr(n);
        }


        // Returns an array of Chi-Square random deviates - user supplied seed
        public static double[] chiSquareRand(int nu, int n, long seed)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            ChiSquareDist psr = new ChiSquareDist(
                nu,
                new RngWrapper((int) seed));

            return psr.NextDoubleArr(n);
        }

        // Chi-Square Distribution Mean
        // m_nu  =  the degrees of freedom
        public static double chiSquareMean(int nu)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            return nu;
        }

        // Chi-Square Distribution Mean
        // m_nu  =  the degrees of freedom
        public static double chiSquareMode(int nu)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            double mode = 0.0D;
            if (nu >= 2)
            {
                mode = nu - 2.0D;
            }
            return mode;
        }

        // Chi-Square Distribution Standard Deviation
        // m_nu  =  the degrees of freedom
        public static double chiSquareStandardDeviation(int nu)
        {
            return chiSquareStandDev(nu);
        }


        // Chi-Square Distribution Standard Deviation
        // m_nu  =  the degrees of freedom
        public static double chiSquareStandDev(int nu)
        {
            if (nu <= 0)
            {
                throw new ArgumentException("The degrees of freedom [m_nu], " + nu + ", must be greater than zero");
            }
            double dnu = nu;
            return Math.Sqrt(2.0D*dnu);
        }

        // Chi-Square Statistic
        public static double chiSquare(double[] observed, double[] expected, double[] variance)
        {
            int nObs = observed.Length;
            int nExp = expected.Length;
            int nVar = variance.Length;
            if (nObs != nExp)
            {
                throw new ArgumentException("observed array Length does not equal the expected array Length");
            }
            if (nObs != nVar)
            {
                throw new ArgumentException("observed array Length does not equal the variance array Length");
            }
            double chi = 0.0D;
            for (int i = 0; i < nObs; i++)
            {
                chi += Fmath.square(observed[i] - expected[i])/variance[i];
            }
            return chi;
        }

        // Chi-Square Statistic for Poisson distribution for frequency data
        // and Poisson distribution for each bin
        // double arguments
        public static double chiSquareFreq(double[] observedFreq, double[] expectedFreq)
        {
            int nObs = observedFreq.Length;
            int nExp = expectedFreq.Length;
            if (nObs != nExp)
            {
                throw new ArgumentException("observed array Length does not equal the expected array Length");
            }
            double chi = 0.0D;
            for (int i = 0; i < nObs; i++)
            {
                chi += Fmath.square(observedFreq[i] - expectedFreq[i])/expectedFreq[i];
            }
            return chi;
        }

        // Chi-Square Statistic for Poisson distribution for frequency data
        // and Poisson distribution for each bin
        // int arguments
        public static double chiSquareFreq(int[] observedFreq, int[] expectedFreq)
        {
            int nObs = observedFreq.Length;
            int nExp = expectedFreq.Length;
            if (nObs != nExp)
            {
                throw new ArgumentException("observed array Length does not equal the expected array Length");
            }
            double[] observ = new double[nObs];
            double[] expect = new double[nObs];
            for (int i = 0; i < nObs; i++)
            {
                observ[i] = observedFreq[i];
                expect[i] = expectedFreq[i];
            }

            return chiSquareFreq(observ, expect);
        }


        // BINOMIAL DISTRIBUTION AND BINOMIAL COEFFICIENTS

        // Returns the binomial cumulative distribution function
        public static double binomialCDF(double p, int n, int k)
        {
            if (p < 0.0D || p > 1.0D)
            {
                throw new ArgumentException("\np must lie between 0 and 1");
            }
            if (k < 0 || n < 0)
            {
                throw new ArgumentException("\nn and k must be greater than or equal to zero");
            }
            if (k > n)
            {
                throw new ArgumentException("\nk is greater than n");
            }
            return RegularizedBetaFunction.regularisedBetaFunction(k, n - k + 1, p);
        }

        // Returns the binomial cumulative distribution function
        public static double binomialProb(double p, int n, int k)
        {
            if (p < 0.0D || p > 1.0D)
            {
                throw new ArgumentException("\np must lie between 0 and 1");
            }
            if (k < 0 || n < 0)
            {
                throw new ArgumentException("\nn and k must be greater than or equal to zero");
            }
            if (k > n)
            {
                throw new ArgumentException("\nk is greater than n");
            }
            return RegularizedBetaFunction.regularisedBetaFunction(k, n - k + 1, p);
        }

        // Returns a binomial mass probabilty function
        public static double binomialPDF(double p, int n, int k)
        {
            if (k < 0 || n < 0)
            {
                throw new ArgumentException("\nn and k must be greater than or equal to zero");
            }
            if (k > n)
            {
                throw new ArgumentException("\nk is greater than n");
            }
            return Math.Floor(0.5D + Math.Exp(logFactorial(n) - logFactorial(k) - logFactorial(n - k)))*Math.Pow(p, k)*
                   Math.Pow(1.0D - p, n - k);
        }

        // Returns a binomial mass probabilty function
        public static double binomial(double p, int n, int k)
        {
            if (k < 0 || n < 0)
            {
                throw new ArgumentException("\nn and k must be greater than or equal to zero");
            }
            if (k > n)
            {
                throw new ArgumentException("\nk is greater than n");
            }
            return Math.Floor(0.5D + Math.Exp(logFactorial(n) - logFactorial(k) - logFactorial(n - k)))*Math.Pow(p, k)*
                   Math.Pow(1.0D - p, n - k);
        }

        // Returns a binomial Coefficient as a double
        public static double binomialCoeff(int n, int k)
        {
            if (k < 0 || n < 0)
            {
                throw new ArgumentException("\nn and k must be greater than or equal to zero");
            }
            if (k > n)
            {
                throw new ArgumentException("\nk is greater than n");
            }
            return Math.Floor(0.5D + Math.Exp(logFactorial(n) - logFactorial(k) - logFactorial(n - k)));
        }

        // Returns an array of n Binomial pseudorandom deviates from a binomial - clock seed
        //  distribution of nTrial trials each of probablity, prob,
        //  after 	bndlev 	Numerical Recipes in C - W.H. Press et al. (Cambridge)
        //		            2nd edition 1992 p295.
        public double[] binomialRand(double prob, int nTrials, int n)
        {
            if (nTrials < n)
            {
                throw new ArgumentException("Number of deviates requested, " + n +
                                            ", must be less than the number of trials, " + nTrials);
            }
            if (prob < 0.0D || prob > 1.0D)
            {
                throw new ArgumentException("The probablity provided, " + prob + ", must lie between 0 and 1)");
            }

            double[] ran = new double[n]; // array of deviates to be returned
            RngWrapper rr = new RngWrapper(); // instance of RngWrapper

            double binomialDeviate = 0.0D; // the binomial deviate to be returned
            double deviateMean = 0.0D; // m_mean of deviate to be produced
            double testDeviate = 0.0D; // test deviate
            double workingProb = 0.0; // working value of the probability
            double logProb = 0.0; // working value of the probability
            double probOld = -1.0D; // previous value of the working probability
            double probC = -1.0D; // complementary value of the working probability
            double logProbC = -1.0D; // log of the complementary value of the working probability
            int nOld = -1; // previous value of trials counter
            double enTrials = 0.0D; //  trials counter
            double oldGamma = 0.0D; // a previous log Gamma function value
            double tanW = 0.0D; // a working tangent
            double hold0 = 0.0D; // a working holding variable
            int jj; // counter

            double probOriginalValue = prob;
            for (int i = 0; i < n; i++)
            {
                prob = probOriginalValue;
                workingProb = (prob <= 0.5D ? prob : 1.0 - prob);
                // distribution invariant on swapping prob for 1 - prob
                deviateMean = nTrials*workingProb;

                if (nTrials < 25)
                {
                    // if number of trials greater than 25 use direct method
                    binomialDeviate = 0.0D;
                    for (jj = 1; jj <= nTrials; jj++)
                    {
                        if (rr.NextDouble() < workingProb)
                        {
                            ++binomialDeviate;
                        }
                    }
                }
                else if (deviateMean < 1.0D)
                {
                    // if fewer than 1 out of 25 events - Poisson approximation is accurate
                    double expOfMean = Math.Exp(-deviateMean);
                    testDeviate = 1.0D;
                    for (jj = 0; jj <= nTrials; jj++)
                    {
                        testDeviate *= rr.NextDouble();
                        if (testDeviate < expOfMean)
                        {
                            break;
                        }
                    }
                    binomialDeviate = (jj <= nTrials ? jj : nTrials);
                }
                else
                {
                    // use rejection method
                    if (nTrials != nOld)
                    {
                        // if nTrials has changed compute useful quantities
                        enTrials = nTrials;
                        oldGamma = LogGammaFunct.logGamma2(enTrials + 1.0D);
                        nOld = nTrials;
                    }
                    if (workingProb != probOld)
                    {
                        // if workingProb has changed compute useful quantities
                        probC = 1.0 - workingProb;
                        logProb = Math.Log(workingProb);
                        logProbC = Math.Log(probC);
                        probOld = workingProb;
                    }

                    double sq = Math.Sqrt(2.0*deviateMean*probC);
                    do
                    {
                        do
                        {
                            double angle = Math.PI*rr.NextDouble();
                            tanW = Math.Tan(angle);
                            hold0 = sq*tanW + deviateMean;
                        }
                        while (hold0 < 0.0D || hold0 >= (enTrials + 1.0D)); //rejection test
                        hold0 = Math.Floor(hold0); // integer value distribution
                        testDeviate = 1.2D*sq*(1.0D + tanW*tanW)*
                                      Math.Exp(oldGamma - LogGammaFunct.logGamma2(hold0 + 1.0D) -
                                               LogGammaFunct.logGamma2(enTrials - hold0 + 1.0D) + hold0*logProb +
                                               (enTrials - hold0)*logProbC);
                    }
                    while (rr.NextDouble() > testDeviate); // rejection test
                    binomialDeviate = hold0;
                }

                if (workingProb != prob)
                {
                    binomialDeviate = nTrials - binomialDeviate; // symmetry transformation
                }

                ran[i] = binomialDeviate;
            }

            return ran;
        }

        // Returns an array of n Binomial pseudorandom deviates from a binomial - user supplied seed
        //  distribution of nTrial trials each of probablity, prob,
        //  after 	bndlev 	Numerical Recipes in C - W.H. Press et al. (Cambridge)
        //		            2nd edition 1992 p295.
        public double[] binomialRand(double prob, int nTrials, int n, long seed)
        {
            if (nTrials < n)
            {
                throw new ArgumentException("Number of deviates requested, " + n +
                                            ", must be less than the number of trials, " + nTrials);
            }
            if (prob < 0.0D || prob > 1.0D)
            {
                throw new ArgumentException("The probablity provided, " + prob + ", must lie between 0 and 1)");
            }

            double[] ran = new double[n]; // array of deviates to be returned
            RngWrapper rr = new RngWrapper((int) seed); // instance of RngWrapper

            double binomialDeviate = 0.0D; // the binomial deviate to be returned
            double deviateMean = 0.0D; // m_mean of deviate to be produced
            double testDeviate = 0.0D; // test deviate
            double workingProb = 0.0; // working value of the probability
            double logProb = 0.0; // working value of the probability
            double probOld = -1.0D; // previous value of the working probability
            double probC = -1.0D; // complementary value of the working probability
            double logProbC = -1.0D; // log of the complementary value of the working probability
            int nOld = -1; // previous value of trials counter
            double enTrials = 0.0D; //  trials counter
            double oldGamma = 0.0D; // a previous log Gamma function value
            double tanW = 0.0D; // a working tangent
            double hold0 = 0.0D; // a working holding variable
            int jj; // counter

            double probOriginalValue = prob;
            for (int i = 0; i < n; i++)
            {
                prob = probOriginalValue;
                workingProb = (prob <= 0.5D ? prob : 1.0 - prob);
                // distribution invariant on swapping prob for 1 - prob
                deviateMean = nTrials*workingProb;

                if (nTrials < 25)
                {
                    // if number of trials greater than 25 use direct method
                    binomialDeviate = 0.0D;
                    for (jj = 1; jj <= nTrials; jj++)
                    {
                        if (rr.NextDouble() < workingProb)
                        {
                            ++binomialDeviate;
                        }
                    }
                }
                else if (deviateMean < 1.0D)
                {
                    // if fewer than 1 out of 25 events - Poisson approximation is accurate
                    double expOfMean = Math.Exp(-deviateMean);
                    testDeviate = 1.0D;
                    for (jj = 0; jj <= nTrials; jj++)
                    {
                        testDeviate *= rr.NextDouble();
                        if (testDeviate < expOfMean)
                        {
                            break;
                        }
                    }
                    binomialDeviate = (jj <= nTrials ? jj : nTrials);
                }
                else
                {
                    // use rejection method
                    if (nTrials != nOld)
                    {
                        // if nTrials has changed compute useful quantities
                        enTrials = nTrials;
                        oldGamma = LogGammaFunct.logGamma2(enTrials + 1.0D);
                        nOld = nTrials;
                    }
                    if (workingProb != probOld)
                    {
                        // if workingProb has changed compute useful quantities
                        probC = 1.0 - workingProb;
                        logProb = Math.Log(workingProb);
                        logProbC = Math.Log(probC);
                        probOld = workingProb;
                    }

                    double sq = Math.Sqrt(2.0*deviateMean*probC);
                    do
                    {
                        do
                        {
                            double angle = Math.PI*rr.NextDouble();
                            tanW = Math.Tan(angle);
                            hold0 = sq*tanW + deviateMean;
                        }
                        while (hold0 < 0.0D || hold0 >= (enTrials + 1.0D)); //rejection test
                        hold0 = Math.Floor(hold0); // integer value distribution
                        testDeviate = 1.2D*sq*(1.0D + tanW*tanW)*
                                      Math.Exp(oldGamma - LogGammaFunct.logGamma2(hold0 + 1.0D) -
                                               LogGammaFunct.logGamma2(enTrials - hold0 + 1.0D) + hold0*logProb +
                                               (enTrials - hold0)*logProbC);
                    }
                    while (rr.NextDouble() > testDeviate); // rejection test
                    binomialDeviate = hold0;
                }

                if (workingProb != prob)
                {
                    binomialDeviate = nTrials - binomialDeviate; // symmetry transformation
                }

                ran[i] = binomialDeviate;
            }

            return ran;
        }


        // F-DISTRIBUTION AND F-TEST

        // Returns the F-distribution probabilty for degrees of freedom df1, df2
        // F ratio provided
        public static double fCompCDF(double fValue, int df1, int df2)
        {
            if (df1 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu1, " + df1 + ", must be greater than zero");
            }
            if (df2 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu2, " + df2 + ", must be greater than zero");
            }
            if (fValue < 0)
            {
                throw new ArgumentException("the F-ratio, " + fValue + ", must be greater than or equal to zero");
            }
            double ddf1 = df1;
            double ddf2 = df2;
            double x = ddf2/(ddf2 + ddf1*fValue);

            return RegularizedBetaFunction.regularisedBetaFunction(df2/2.0D, df1/2.0D, x);
        }

        // retained fot compatibility
        public static double fTestProb(double fValue, int df1, int df2)
        {
            if (df1 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu1, " + df1 + ", must be greater than zero");
            }
            if (df2 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu2, " + df2 + ", must be greater than zero");
            }
            if (fValue < 0)
            {
                throw new ArgumentException("the F-ratio, " + fValue + ", must be greater than or equal to zero");
            }
            double ddf1 = df1;
            double ddf2 = df2;
            double x = ddf2/(ddf2 + ddf1*fValue);
            return RegularizedBetaFunction.regularisedBetaFunction(df2/2.0D, df1/2.0D, x);
        }

        // Returns the F-distribution probabilty for degrees of freedom df1, df2
        // numerator and denominator variances provided
        public static double fCompCDF(double var1, int df1, double var2, int df2)
        {
            if (df1 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu1, " + df1 + ", must be greater than zero");
            }
            if (df2 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu2, " + df2 + ", must be greater than zero");
            }
            if (var1 < 0)
            {
                throw new ArgumentException("the variance, var1" + var1 + ", must be greater than or equal to zero");
            }
            if (var1 <= 0)
            {
                throw new ArgumentException("the variance, var2" + var2 + ", must be greater than zero");
            }
            double fValue = var1/var2;
            double ddf1 = df1;
            double ddf2 = df2;
            double x = ddf2/(ddf2 + ddf1*fValue);
            return RegularizedBetaFunction.regularisedBetaFunction(df2/2.0D, df1/2.0D, x);
        }

        // retained fot compatibility
        public static double fTestProb(double var1, int df1, double var2, int df2)
        {
            if (df1 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu1, " + df1 + ", must be greater than zero");
            }
            if (df2 <= 0)
            {
                throw new ArgumentException("the degrees of freedom, nu2, " + df2 + ", must be greater than zero");
            }
            if (var1 < 0)
            {
                throw new ArgumentException("the variance, var1" + var1 + ", must be greater than or equal to zero");
            }
            if (var1 <= 0)
            {
                throw new ArgumentException("the variance, var2" + var2 + ", must be greater than zero");
            }
            double fValue = var1/var2;
            double ddf1 = df1;
            double ddf2 = df2;
            double x = ddf2/(ddf2 + ddf1*fValue);
            return RegularizedBetaFunction.regularisedBetaFunction(df2/2.0D, df1/2.0D, x);
        }


        // F-distribution Inverse Cumulative Distribution Function
        public static double fDistributionInverseCDF(int nu1, int nu2, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }

            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = 0.0;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    // Create instance of the class holding the F-distribution m_cfd function
                    FdistribtionFunct fdistn = new FdistribtionFunct();

                    // set function variables
                    fdistn.nu1 = nu1;
                    fdistn.nu2 = nu2;

                    // required tolerance
                    double tolerance = 1e-12;

                    // lower bound
                    double lowerBound = 0.0;

                    // upper bound
                    double upperBound = 2.0;

                    // Create instance of RealRoot
                    RealRoot realR = new RealRoot();

                    // Set extension limits
                    realR.noLowerBoundExtension();

                    // Set tolerance
                    realR.setTolerance(tolerance);

                    // Supress error messages and arrange for NaN to be returned as root if root not found
                    realR.resetNaNexceptionToTrue();
                    realR.supressLimitReachedMessage();
                    realR.supressNaNmessage();

                    //  set function m_cfd  variable
                    fdistn.cfd = prob;

                    // call root searching method
                    icdf = realR.bisect(fdistn, lowerBound, upperBound);
                }
            }
            return icdf;
        }


        // F-distribution order statistic medians (n points)
        public static double[] fDistributionOrderStatisticMedians(int nu1, int nu2, int n)
        {
            double nn = n;
            double[] gosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                gosm[i] = fDistributionInverseCDF(nu1, nu2, uosm[i]);
            }
            Stat st = new Stat(gosm);
            double mean = st.mean();
            double sigma = st.standardDeviation();
            gosm = Scale.scale(gosm, mean, sigma);
            return gosm;
        }


        // Returns the F-test value corresponding to a F-distribution probabilty, fProb,
        //   for degrees of freedom df1, df2
        public static double fTestValueGivenFprob(double fProb, int df1, int df2)
        {
            // Create an array F-test value array
            int fTestsNum = 100; // Length of array
            double[] fTestValues = new double[fTestsNum];
            fTestValues[0] = 0.0001D; // lowest array value
            fTestValues[fTestsNum - 1] = 10000.0D; // highest array value
            // calculate array increment - log scale
            double diff = (Fmath.Log10(fTestValues[fTestsNum - 1]) - Fmath.Log10(fTestValues[0]))/(fTestsNum - 1);
            // Fill array
            for (int j = 1; j < fTestsNum - 1; j++)
            {
                fTestValues[j] = Math.Pow(10.0D, (Fmath.Log10(fTestValues[j - 1]) + diff));
            }

            // calculate F test probability array corresponding to F-test value array
            double[] fTestProb = new double[fTestsNum];
            for (int j = 0; j < fTestsNum; j++)
            {
                fTestProb[j] = Stat.fTestProb(fTestValues[j], df1, df2);
            }

            // calculate F-test value for provided probability
            // using bisection procedure
            double fTest0 = 0.0D;
            double fTest1 = 0.0D;
            double fTest2 = 0.0D;

            // find bracket for the F-test probabilities and calculate F-UnitTest value from above arrays
            bool test0 = true;
            bool test1 = true;
            int i = 0;
            int endTest = 0;
            while (test0)
            {
                if (fProb == fTestProb[i])
                {
                    fTest0 = fTestValues[i];
                    test0 = false;
                    test1 = false;
                }
                else
                {
                    if (fProb > fTestProb[i])
                    {
                        test0 = false;
                        if (i > 0)
                        {
                            fTest1 = fTestValues[i - 1];
                            fTest2 = fTestValues[i];
                            endTest = -1;
                        }
                        else
                        {
                            fTest1 = fTestValues[i]/10.0D;
                            fTest2 = fTestValues[i];
                        }
                    }
                    else
                    {
                        i++;
                        if (i > fTestsNum - 1)
                        {
                            test0 = false;
                            fTest1 = fTestValues[i - 1];
                            fTest2 = 10.0D*fTestValues[i - 1];
                            endTest = 1;
                        }
                    }
                }
            }

            // call bisection method
            if (test1)
            {
                fTest0 = fTestBisect(fProb, fTest1, fTest2, df1, df2, endTest);
            }

            return fTest0;
        }

        // Bisection procedure for calculating and F-test value corresponding
        //   to a given F-test probability
        private static double fTestBisect(double fProb, double fTestLow, double fTestHigh, int df1, int df2, int endTest)
        {
            double funcLow = fProb - fTestProb(fTestLow, df1, df2);
            double funcHigh = fProb - fTestProb(fTestHigh, df1, df2);
            double fTestMid = 0.0D;
            double funcMid = 0.0;
            int nExtensions = 0;
            int nIter = 1000; // iterations allowed
            double check = fProb*1e-6; // tolerance for bisection
            bool test0 = true; // test for extending bracket
            bool test1 = true; // test for bisection procedure
            while (test0)
            {
                if (funcLow*funcHigh > 0.0D)
                {
                    if (endTest < 0)
                    {
                        nExtensions++;
                        if (nExtensions > 100)
                        {
                            PrintToScreen.WriteLine(
                                "Class: Stats\nMethod: fTestBisect\nProbability higher than range covered\nF-test value is less than " +
                                fTestLow);
                            PrintToScreen.WriteLine("This value was returned");
                            fTestMid = fTestLow;
                            test0 = false;
                            test1 = false;
                        }
                        fTestLow /= 10.0D;
                        funcLow = fProb - fTestProb(fTestLow, df1, df2);
                    }
                    else
                    {
                        nExtensions++;
                        if (nExtensions > 100)
                        {
                            PrintToScreen.WriteLine(
                                "Class: Stats\nMethod: fTestBisect\nProbability lower than range covered\nF-test value is greater than " +
                                fTestHigh);
                            PrintToScreen.WriteLine("This value was returned");
                            fTestMid = fTestHigh;
                            test0 = false;
                            test1 = false;
                        }
                        fTestHigh *= 10.0D;
                        funcHigh = fProb - fTestProb(fTestHigh, df1, df2);
                    }
                }
                else
                {
                    test0 = false;
                }

                int i = 0;
                while (test1)
                {
                    fTestMid = (fTestLow + fTestHigh)/2.0D;
                    funcMid = fProb - fTestProb(fTestMid, df1, df2);
                    if (Math.Abs(funcMid) < check)
                    {
                        test1 = false;
                    }
                    else
                    {
                        i++;
                        if (i > nIter)
                        {
                            PrintToScreen.WriteLine(
                                "Class: Stats\nMethod: fTestBisect\nmaximum number of iterations exceeded\ncurrent value of F-test value returned");
                            test1 = false;
                        }
                        if (funcMid*funcHigh > 0)
                        {
                            funcHigh = funcMid;
                            fTestHigh = fTestMid;
                        }
                        else
                        {
                            funcLow = funcMid;
                            fTestLow = fTestMid;
                        }
                    }
                }
            }
            return fTestMid;
        }

        // F-distribution pdf
        public double fPDF(double fValue, int nu1, int nu2)
        {
            double numer = Math.Pow(nu1*fValue, nu1)*Math.Pow(nu2, nu2);
            double dnu1 = nu1;
            double dnu2 = nu2;
            numer /= Math.Pow(dnu1*fValue + dnu2, dnu1 + dnu2);
            numer = Math.Sqrt(numer);
            double denom = fValue*BetaFunct.betaFunction(dnu1/2.0D, dnu2/2.0D);
            return numer/denom;
        }

        public double fPDF(double var1, int nu1, double var2, int nu2)
        {
            return fPDF(var1/var2, nu1, nu2);
        }

        // Returns an array of F-distribution random deviates - clock seed
        public static double[] fRand(int nu1, int nu2, int n)
        {
            if (nu1 <= 0)
            {
                throw new ArgumentException("The degrees of freedom [nu1], " + nu1 + ", must be greater than zero");
            }
            if (nu2 <= 0)
            {
                throw new ArgumentException("The degrees of freedom [nu2], " + nu2 + ", must be greater than zero");
            }

            FDist psr = new FDist(
                nu1,
                nu2,
                new RngWrapper());
            return psr.NextDoubleArr(n);
        }

        // Returns an array of F-distribution random deviates - user supplied seed
        public static double[] fRand(int nu1, int nu2, int n, long seed)
        {
            if (nu1 <= 0)
            {
                throw new ArgumentException("The degrees of freedom [nu1], " + nu1 + ", must be greater than zero");
            }
            if (nu2 <= 0)
            {
                throw new ArgumentException("The degrees of freedom [nu2], " + nu2 + ", must be greater than zero");
            }
            FDist psr = new FDist(
                nu1,
                nu2,
                new RngWrapper((int) seed));
            return psr.NextDoubleArr(n);
        }

        // STUDENT'S T DISTRIBUTION

        // Returns the Student's t probability density function
        public static double studentst(double tValue, int df)
        {
            return studentT(tValue, df);
        }

        // Returns the Student's t probability density function
        public static double studentT(double tValue, int df)
        {
            double ddf = df;
            double dfterm = (ddf + 1.0D)/2.0D;
            return ((GammaFunct.Gamma(dfterm)/GammaFunct.Gamma(ddf/2))/Math.Sqrt(ddf*Math.PI))*
                   Math.Pow(1.0D + tValue*tValue/ddf, -dfterm);
        }

        // Returns the Student's t probability density function
        public static double studentstPDF(double tValue, int df)
        {
            return studentTpdf(tValue, df);
        }

        // Returns the Student's t probability density function
        public static double studentTpdf(double tValue, int df)
        {
            double ddf = df;
            double dfterm = (ddf + 1.0D)/2.0D;
            return ((GammaFunct.Gamma(dfterm)/GammaFunct.Gamma(ddf/2))/Math.Sqrt(ddf*Math.PI))*
                   Math.Pow(1.0D + tValue*tValue/ddf, -dfterm);
        }

        // Returns the Student's t probability density function
        public static double studentTPDF(double tValue, int df)
        {
            double ddf = df;
            double dfterm = (ddf + 1.0D)/2.0D;
            return ((GammaFunct.Gamma(dfterm)/GammaFunct.Gamma(ddf/2))/Math.Sqrt(ddf*Math.PI))*
                   Math.Pow(1.0D + tValue*tValue/ddf, -dfterm);
        }


        // Returns the Student's t cumulative distribution function probability
        public static double studentstCDF(double tValue, int df)
        {
            return studentTcdf(tValue, df);
        }


        // Returns the Student's t cumulative distribution function probability
        public static double studentTProb(double tValue, int df)
        {
            if (tValue == double.PositiveInfinity)
            {
                return 1.0;
            }
            else
            {
                if (tValue == double.NegativeInfinity)
                {
                    return 0.0;
                }
                else
                {
                    double ddf = df;
                    double x = ddf/(ddf + tValue*tValue);
                    return 0.5D*(1.0D + (RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, 1) -
                                         RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, x))*
                                        Fmath.sign(tValue));
                }
            }
        }

        // Returns the Student's t cumulative distribution function probability
        public static double studentTcdf(double tValue, int df)
        {
            if (tValue == double.PositiveInfinity)
            {
                return 1.0;
            }
            else
            {
                if (tValue == double.NegativeInfinity)
                {
                    return 0.0;
                }
                else
                {
                    double ddf = df;
                    double x = ddf/(ddf + tValue*tValue);
                    return 0.5D*(1.0D + (
                                            RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, 1) -
                                            RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, x))*
                                        Fmath.sign(tValue));
                }
            }
        }

        // Returns the Student's t cumulative distribution function probability
        public static double studentTCDF(double tValue, int df)
        {
            if (tValue == double.PositiveInfinity)
            {
                return 1.0;
            }
            else
            {
                if (tValue == double.NegativeInfinity)
                {
                    return 0.0;
                }
                else
                {
                    double ddf = df;
                    double x = ddf/(ddf + tValue*tValue);
                    return 0.5D*
                           (1.0D +
                            (RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, 1) -
                             RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, x))*Fmath.sign(tValue));
                }
            }
        }

        // Returns the Student's t cumulative distribution function probability
        public static double studentTcdf(double tValueLower, double tValueUpper, int df)
        {
            if (tValueUpper == double.PositiveInfinity)
            {
                if (tValueLower == double.NegativeInfinity)
                {
                    return 1.0;
                }
                else
                {
                    if (tValueLower == double.PositiveInfinity)
                    {
                        return 0.0;
                    }
                    else
                    {
                        return (1.0 - studentTcdf(tValueLower, df));
                    }
                }
            }
            else
            {
                if (tValueLower == double.NegativeInfinity)
                {
                    if (tValueUpper == double.NegativeInfinity)
                    {
                        return 0.0;
                    }
                    else
                    {
                        return studentTcdf(tValueUpper, df);
                    }
                }
                else
                {
                    return studentTcdf(tValueUpper, df) - studentTcdf(tValueLower, df);
                }
            }
        }

        // Returns the P-value for a given Student's t value and degrees of freedom
        public static double pValue(double tValue, int df)
        {
            double abst = Math.Abs(tValue);
            return 1.0 - studentTcdf(-abst, abst, df);
        }

        // Returns the Student's t m_mean,  df = degrees of freedom
        public static double studentstMean(int df)
        {
            return studentTmean(df);
        }

        // Returns the Student's t m_mean,  df = degrees of freedom
        public static double studentTmean(int df)
        {
            double mean = double.NaN; // m_mean undefined for df = 1
            if (df > 1)
            {
                mean = 0.0D;
            }
            return mean;
        }

        // Returns the Student's t median
        public static double studentstMedian()
        {
            return 0.0;
        }

        // Returns the Student's t median
        public static double studentTmedian()
        {
            return 0.0;
        }

        // Returns the Student's t mode
        public static double studentstMode()
        {
            return 0.0;
        }

        // Returns the Student's t mode
        public static double studentTmode()
        {
            return 0.0;
        }

        // Returns the Student's t standard deviation,  df = degrees of freedom
        public static double studentstStandardDeviation(int df)
        {
            return studentTstandDev(df);
        }

        // Returns the Student's t standard deviation,  df = degrees of freedom
        public static double studentTstandDev(int df)
        {
            double standDev = double.PositiveInfinity;
            if (df > 2)
            {
                standDev = Math.Sqrt(df/(1.0 - df));
            }
            return standDev;
        }

        // Returns the A(t|n) distribution probabilty
        public static double probAtn(double tValue, int df)
        {
            double ddf = df;
            double x = ddf/(ddf + tValue*tValue);
            return 1.0D - RegularizedBetaFunction.regularisedBetaFunction(ddf/2.0D, 0.5D, x);
        }

        // Returns an array of Student's t random deviates - clock seed
        // m_nu  =  the degrees of freedom
        public static double[] studentstRand(int nu, int n)
        {
            return studentTRand(nu, n);
        }

        // Returns an array of Student's t random deviates - clock seed
        // m_nu  =  the degrees of freedom
        public static double[] studentTRand(int nu, int n)
        {
            TStudentDist psr = new TStudentDist(nu,
                                                new RngWrapper());
            return psr.NextDoubleArr(n);
        }

        // Returns an array of a Student's t random deviates - user supplied seed
        // m_nu  =  the degrees of freedom
        public static double[] studentstRand(int nu, int n, long seed)
        {
            return studentTrand(nu, n, seed);
        }

        // Returns an array of a Student's t random deviates - user supplied seed
        // m_nu  =  the degrees of freedom
        public static double[] studentTrand(int nu, int n, long seed)
        {
            TStudentDist psr = new TStudentDist(nu,
                                                new RngWrapper((int) seed));
            return psr.NextDoubleArr(n);
        }


        // GUMBEL (TYPE I EXTREME VALUE) DISTRIBUTION

        // Minimum Gumbel cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        public static double gumbelMinProbCDF(double mu, double sigma, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = -(upperlimit - mu)/sigma;
            return Math.Exp(-Math.Exp(arg));
        }

        // Minimum Gumbel cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        public static double gumbelMinProb(double mu, double sigma, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = -(upperlimit - mu)/sigma;
            return Math.Exp(-Math.Exp(arg));
        }

        // Maximum Gumbel cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        public static double gumbelMaxCDF(double mu, double sigma, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = -(upperlimit - mu)/sigma;
            return 1.0D - Math.Exp(-Math.Exp(arg));
        }

        // Maximum Gumbel cumulative distribution function
        // probability that a variate will assume a value less than the upperlimit
        public static double gumbelMaxProb(double mu, double sigma, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = -(upperlimit - mu)/sigma;
            return 1.0D - Math.Exp(-Math.Exp(arg));
        }


        // Gumbel (maximum order statistic) Inverse Cumulative Density Function
        public static double gumbelMaxInverseCDF(double mu, double sigma, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = double.NegativeInfinity;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu - sigma*Math.Log(Math.Log(1.0/prob));
                }
            }

            return icdf;
        }

        // Minimum Gumbel cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double gumbelMinCDF(double mu, double sigma, double lowerlimit, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg1 = -(lowerlimit - mu)/sigma;
            double arg2 = -(upperlimit - mu)/sigma;
            double term1 = Math.Exp(-Math.Exp(arg1));
            double term2 = Math.Exp(-Math.Exp(arg2));
            return term2 - term1;
        }

        // Minimum Gumbel cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double gumbelMinProb(double mu, double sigma, double lowerlimit, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg1 = -(lowerlimit - mu)/sigma;
            double arg2 = -(upperlimit - mu)/sigma;
            double term1 = Math.Exp(-Math.Exp(arg1));
            double term2 = Math.Exp(-Math.Exp(arg2));
            return term2 - term1;
        }

        // Maximum Gumbel cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double gumbelMaxCDF(double mu, double sigma, double lowerlimit, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = -Math.Exp(-Math.Exp(arg1));
            double term2 = -Math.Exp(-Math.Exp(arg2));
            return term2 - term1;
        }

        // Maximum Gumbel cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double gumbelMaxProb(double mu, double sigma, double lowerlimit, double upperlimit)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = -Math.Exp(-Math.Exp(arg1));
            double term2 = -Math.Exp(-Math.Exp(arg2));
            return term2 - term1;
        }

        // Gumbel (minimum order statistic) Inverse Cumulative Density Function
        public static double gumbelMinInverseCDF(double mu, double sigma, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = double.NegativeInfinity;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu + sigma*Math.Log(Math.Log(1.0/(1.0 - prob)));
                }
            }

            return icdf;
        }

        // Minimum Gumbel probability density function
        public static double gumbelMinPDF(double mu, double sigma, double x)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = (x - mu)/sigma;
            return (1.0D/sigma)*Math.Exp(arg)*Math.Exp(-Math.Exp(arg));
        }

        // Minimum Gumbel probability density function
        public static double gumbelMin(double mu, double sigma, double x)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = (x - mu)/sigma;
            return (1.0D/sigma)*Math.Exp(arg)*Math.Exp(-Math.Exp(arg));
        }

        // Maximum Gumbel probability density function
        public static double gumbelMaxPDF(double mu, double sigma, double x)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = -(x - mu)/sigma;
            return (1.0D/sigma)*Math.Exp(arg)*Math.Exp(-Math.Exp(arg));
        }

        // Maximum Gumbel probability density function
        public static double gumbelMax(double mu, double sigma, double x)
        {
            if (sigma < 0.0D)
            {
                throw new ArgumentException("m_sigma must be positive");
            }
            double arg = -(x - mu)/sigma;
            return (1.0D/sigma)*Math.Exp(arg)*Math.Exp(-Math.Exp(arg));
        }

        // Returns an array of minimal Gumbel (Type I EVD) random deviates - clock seed
        // m_mu  =  location parameter, m_sigma = scale parameter, n = Length of array
        public static double[] gumbelMinRand(double mu, double sigma, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Log(Math.Log(1.0D/(1.0D - rr.NextDouble())))*sigma + mu;
            }
            return ran;
        }

        // Returns an array of minimal Gumbel (Type I EVD) random deviates - user supplied seed
        // m_mu  =  location parameter, m_sigma = scale parameter, n = Length of array
        public static double[] gumbelMinRand(double mu, double sigma, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Log(Math.Log(1.0D/(1.0D - rr.NextDouble())))*sigma + mu;
            }
            return ran;
        }

        // Returns an array of maximal Gumbel (Type I EVD) random deviates - clock seed
        // m_mu  =  location parameter, m_sigma = scale parameter, n = Length of array
        public static double[] gumbelMaxRand(double mu, double sigma, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = mu - Math.Log(Math.Log(1.0D/(1.0D - rr.NextDouble())))*sigma;
            }
            return ran;
        }

        // Returns an array of maximal Gumbel (Type I EVD) random deviates - user supplied seed
        // m_mu  =  location parameter, m_sigma = scale parameter, n = Length of array
        public static double[] gumbelMaxRand(double mu, double sigma, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = mu - Math.Log(Math.Log(1.0D/(1.0D - rr.NextDouble())))*sigma;
            }
            return ran;
        }

        // Gumbel (minimum order statistic) order statistic medians (n points)
        public static double[] gumbelMinOrderStatisticMedians(double mu, double sigma, int n)
        {
            double nn = n;
            double[] gmosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                gmosm[i] = gumbelMinInverseCDF(mu, sigma, uosm[i]);
            }
            return gmosm;
        }

        // Gumbel (maximum order statistic) order statistic medians (n points)
        public static double[] gumbelMaxOrderStatisticMedians(double mu, double sigma, int n)
        {
            double nn = n;
            double[] gmosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                gmosm[i] = gumbelMaxInverseCDF(mu, sigma, uosm[i]);
            }
            return gmosm;
        }

        // Minimum Gumbel m_mean
        public static double gumbelMinMean(double mu, double sigma)
        {
            return mu - sigma*Fmath.EULER_CONSTANT_GAMMA;
        }

        // Maximum Gumbel m_mean
        public static double gumbelMaxMean(double mu, double sigma)
        {
            return mu + sigma*Fmath.EULER_CONSTANT_GAMMA;
        }

        // Minimum Gumbel standard deviation
        public static double gumbelMinStandardDeviation(double sigma)
        {
            return sigma*Math.PI/Math.Sqrt(6.0D);
        }

        // Minimum Gumbel standard deviation
        public static double gumbelMinStandDev(double sigma)
        {
            return sigma*Math.PI/Math.Sqrt(6.0D);
        }

        // Maximum Gumbel standard deviation
        public static double gumbelMaxStandardDeviation(double sigma)
        {
            return sigma*Math.PI/Math.Sqrt(6.0D);
        }

        // Maximum Gumbel standard deviation
        public static double gumbelMaxStandDev(double sigma)
        {
            return sigma*Math.PI/Math.Sqrt(6.0D);
        }

        // Minimum Gumbel mode
        public static double gumbelMinMode(double mu, double sigma)
        {
            return mu;
        }

        // Maximum Gumbel mode
        public static double gumbelMaxMode(double mu, double sigma)
        {
            return mu;
        }

        // Minimum Gumbel median
        public static double gumbelMinMedian(double mu, double sigma)
        {
            return mu + sigma*Math.Log(Math.Log(2.0D));
        }

        // Maximum Gumbel median
        public static double gumbelMaxMedian(double mu, double sigma)
        {
            return mu - sigma*Math.Log(Math.Log(2.0D));
        }


        // FRECHET (TYPE II EXTREME VALUE) DISTRIBUTION

        // Frechet cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double frechetProb(double mu, double sigma, double gamma, double upperlimit)
        {
            double arg = (upperlimit - mu)/sigma;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = Math.Exp(-Math.Pow(arg, -gamma));
            }
            return y;
        }


        // Frechet cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double frechetCDF(double mu, double sigma, double gamma, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = Math.Exp(-Math.Pow(arg1, -gamma));
            }
            if (arg2 >= 0.0D)
            {
                term2 = Math.Exp(-Math.Pow(arg2, -gamma));
            }
            return term2 - term1;
        }

        // Frechet cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double frechetProb(double mu, double sigma, double gamma, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = Math.Exp(-Math.Pow(arg1, -gamma));
            }
            if (arg2 >= 0.0D)
            {
                term2 = Math.Exp(-Math.Pow(arg2, -gamma));
            }
            return term2 - term1;
        }

        // Frechet Inverse Cumulative Density Function
        // Three parameter
        public static double frechetInverseCDF(double mu, double sigma, double gamma, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = double.NegativeInfinity;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu + sigma*Math.Pow(Math.Log(1.0/prob), -1.0/gamma);
                }
            }

            return icdf;
        }

        // Frechet Inverse Cumulative Density Function
        // Two parameter
        public static double frechetInverseCDF(double sigma, double gamma, double prob)
        {
            return frechetInverseCDF(0.0D, sigma, gamma, prob);
        }

        // Frechet Inverse Cumulative Density Function
        // Standard
        public static double frechetInverseCDF(double gamma, double prob)
        {
            return frechetInverseCDF(0.0D, 1.0D, gamma, prob);
        }


        // Frechet probability density function
        public static double frechetPDF(double mu, double sigma, double gamma, double x)
        {
            double arg = (x - mu)/sigma;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = (gamma/sigma)*Math.Pow(arg, -gamma - 1.0D)*Math.Exp(-Math.Pow(arg, -gamma));
            }
            return y;
        }

        // Frechet probability density function
        public static double frechet(double mu, double sigma, double gamma, double x)
        {
            double arg = (x - mu)/sigma;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = (gamma/sigma)*Math.Pow(arg, -gamma - 1.0D)*Math.Exp(-Math.Pow(arg, -gamma));
            }
            return y;
        }

        // Frechet order statistic medians (n points)
        // Three parameters
        public static double[] frechetOrderStatisticMedians(double mu, double sigma, double gamma, int n)
        {
            double nn = n;
            double[] fosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                fosm[i] = frechetInverseCDF(mu, sigma, gamma, uosm[i]);
            }
            return fosm;
        }

        // Frechet order statistic medians (n points)
        // Two parameters
        public static double[] frechetOrderStatisticMedians(double sigma, double gamma, int n)
        {
            return frechetOrderStatisticMedians(0.0D, sigma, gamma, n);
        }

        // Frechet order statistic medians (n points)
        // Standard
        public static double[] frechetOrderStatisticMedians(double gamma, int n)
        {
            return frechetOrderStatisticMedians(0.0D, 1.0D, gamma, n);
        }

        // Frechet m_mean
        public static double frechetMean(double mu, double sigma, double gamma)
        {
            double y = double.NaN;
            if (gamma > 1.0D)
            {
                y = mu + sigma*GammaFunct.Gamma(1.0D - 1.0D/gamma);
            }
            return y;
        }

        // Frechet standard deviation
        public static double frechetStandardDeviation(double sigma, double gamma)
        {
            return frechetStandDev(sigma, gamma);
        }


        // Frechet standard deviation
        public static double frechetStandDev(double sigma, double gamma)
        {
            double y = double.NaN;
            if (gamma > 2.0D)
            {
                y = GammaFunct.Gamma(1.0D - 2.0D/gamma) - Fmath.square(GammaFunct.Gamma(1.0D - 1.0D/gamma));
                y = sigma*Math.Sqrt(y);
            }
            return y;
        }

        // Frechet mode
        public static double frechetMode(double mu, double sigma, double gamma)
        {
            return mu + sigma*Math.Pow(gamma/(1.0D + gamma), 1.0D/gamma);
        }

        // Returns an array of Frechet (Type II EVD) random deviates - clock seed
        // m_mu  =  location parameter, m_sigma = cale parameter, gamma = shape parametern = Length of array
        public static double[] frechetRand(double mu, double sigma, double gamma, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Pow((1.0D/(Math.Log(1.0D/rr.NextDouble()))), 1.0D/gamma)*sigma + mu;
            }
            return ran;
        }

        // Returns an array of Frechet (Type II EVD) random deviates - user supplied seed
        // m_mu  =  location parameter, m_sigma = cale parameter, gamma = shape parametern = Length of array
        public static double[] frechetRand(double mu, double sigma, double gamma, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Pow((1.0D/(Math.Log(1.0D/rr.NextDouble()))), 1.0D/gamma)*sigma + mu;
            }
            return ran;
        }


        // WEIBULL (TYPE III EXTREME VALUE) DISTRIBUTION

        // Weibull cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double weibullCDF(double mu, double sigma, double gamma, double upperlimit)
        {
            double arg = (upperlimit - mu)/sigma;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = 1.0D - Math.Exp(-Math.Pow(arg, gamma));
            }
            return y;
        }

        // Weibull cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double weibullProb(double mu, double sigma, double gamma, double upperlimit)
        {
            double arg = (upperlimit - mu)/sigma;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = 1.0D - Math.Exp(-Math.Pow(arg, gamma));
            }
            return y;
        }


        // Weibull cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double weibullCDF(double mu, double sigma, double gamma, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = -Math.Exp(-Math.Pow(arg1, gamma));
            }
            if (arg2 >= 0.0D)
            {
                term2 = -Math.Exp(-Math.Pow(arg2, gamma));
            }
            return term2 - term1;
        }

        // Weibull cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double weibullProb(double mu, double sigma, double gamma, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = -Math.Exp(-Math.Pow(arg1, gamma));
            }
            if (arg2 >= 0.0D)
            {
                term2 = -Math.Exp(-Math.Pow(arg2, gamma));
            }
            return term2 - term1;
        }


        // Weibull Inverse Cumulative Density Function
        // Three parameter
        public static double weibullInverseCDF(double mu, double sigma, double gamma, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = mu;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu + sigma*(Math.Pow(-Math.Log(1.0 - prob), 1.0/gamma));
                }
            }

            return icdf;
        }

        // Weibull Inverse Cumulative Disrtibution Function
        public static double inverseWeibullCDF(double mu, double sigma, double gamma, double prob)
        {
            return weibullInverseCDF(mu, sigma, gamma, prob);
        }

        // Weibull Inverse Cumulative Density Function
        // Two parameter
        public static double weibullInverseCDF(double sigma, double gamma, double prob)
        {
            return weibullInverseCDF(0.0D, sigma, gamma, prob);
        }

        public static double inverseWeibullCDF(double sigma, double gamma, double prob)
        {
            return weibullInverseCDF(0.0, sigma, gamma, prob);
        }


        // Weibull Inverse Cumulative Density Function
        // Standard
        public static double weibullInverseCDF(double gamma, double prob)
        {
            return weibullInverseCDF(0.0D, 1.0D, gamma, prob);
        }

        public static double inverseWeibullCDF(double gamma, double prob)
        {
            return weibullInverseCDF(0.0D, 1.0D, gamma, prob);
        }

        // Weibull probability density function
        public static double weibullPDF(double mu, double sigma, double gamma, double x)
        {
            double arg = (x - mu)/sigma;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = (gamma/sigma)*Math.Pow(arg, gamma - 1.0D)*Math.Exp(-Math.Pow(arg, gamma));
            }
            return y;
        }

        // Weibull probability density function
        public static double weibull(double mu, double sigma, double gamma, double x)
        {
            double arg = (x - mu)/sigma;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = (gamma/sigma)*Math.Pow(arg, gamma - 1.0D)*Math.Exp(-Math.Pow(arg, gamma));
            }
            return y;
        }

        // Weibull m_mean
        public static double weibullMean(double mu, double sigma, double gamma)
        {
            return mu + sigma*GammaFunct.Gamma(1.0D/gamma + 1.0D);
        }

        // Weibull standard deviation
        public static double weibullStandardDeviation(double sigma, double gamma)
        {
            return weibullStandDev(sigma, gamma);
        }


        // Weibull standard deviation
        public static double weibullStandDev(double sigma, double gamma)
        {
            double y = GammaFunct.Gamma(2.0D/gamma + 1.0D) - Fmath.square(GammaFunct.Gamma(1.0D/gamma + 1.0D));
            return sigma*Math.Sqrt(y);
        }

        // Weibull mode
        public static double weibullMode(double mu, double sigma, double gamma)
        {
            double y = mu;
            if (gamma > 1.0D)
            {
                y = mu + sigma*Math.Pow((gamma - 1.0D)/gamma, 1.0D/gamma);
            }
            return y;
        }

        // Weibull median
        public static double weibullMedian(double mu, double sigma, double gamma)
        {
            return mu + sigma*Math.Pow(Math.Log(2.0D), 1.0D/gamma);
        }

        // Returns an array of Weibull (Type III EVD) random deviates - clock seed
        // m_mu  =  location parameter, m_sigma = cale parameter, gamma = shape parametern = Length of array
        public static double[] weibullRand(double mu, double sigma, double gamma, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Pow(-Math.Log(1.0D - rr.NextDouble()), 1.0D/gamma)*sigma + mu;
            }
            return ran;
        }

        // Returns an array of Weibull (Type III EVD) random deviates - user supplied seed
        // m_mu  =  location parameter, m_sigma = cale parameter, gamma = shape parametern = Length of array
        public static double[] weibullRand(double mu, double sigma, double gamma, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Pow(-Math.Log(1.0D - rr.NextDouble()), 1.0D/gamma)*sigma + mu;
            }
            return ran;
        }

        // Weibull order statistic medians (n points)
        // Three parameter
        public static double[] weibullOrderStatisticMedians(double mu, double sigma, double gamma, int n)
        {
            double nn = n;
            double[] wosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                wosm[i] = inverseWeibullCDF(mu, sigma, gamma, uosm[i]);
            }
            return wosm;
        }

        // Weibull order statistic medians for a m_mu of zero  (n points)
        // Two parameter
        public static double[] weibullOrderStatisticMedians(double sigma, double gamma, int n)
        {
            return weibullOrderStatisticMedians(0.0, sigma, gamma, n);
        }

        // Weibull order statistic medians for a m_mu of zero and a m_sigma of unity  (n points)
        // Standard
        public static double[] weibullOrderStatisticMedians(double gamma, int n)
        {
            return weibullOrderStatisticMedians(0.0, 1.0, gamma, n);
        }


        // EXPONENTIAL DISTRIBUTION

        // Exponential cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double exponentialCDF(double mu, double sigma, double upperlimit)
        {
            double arg = (upperlimit - mu)/sigma;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = 1.0D - Math.Exp(-arg);
            }
            return y;
        }

        // Exponential cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double exponentialProb(double mu, double sigma, double upperlimit)
        {
            double arg = (upperlimit - mu)/sigma;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = 1.0D - Math.Exp(-arg);
            }
            return y;
        }

        // Exponential cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double exponentialCDF(double mu, double sigma, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = -Math.Exp(-arg1);
            }
            if (arg2 >= 0.0D)
            {
                term2 = -Math.Exp(-arg2);
            }
            return term2 - term1;
        }

        // Exponential cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double exponentialProb(double mu, double sigma, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit - mu)/sigma;
            double arg2 = (upperlimit - mu)/sigma;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = -Math.Exp(-arg1);
            }
            if (arg2 >= 0.0D)
            {
                term2 = -Math.Exp(-arg2);
            }
            return term2 - term1;
        }

        // Exponential Inverse Cumulative Density Function
        public static double exponentialInverseCDF(double mu, double sigma, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = mu;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = mu - sigma*(Math.Log(1.0 - prob));
                }
            }

            return icdf;
        }

        // Exponential Inverse Cumulative Density Function
        public static double inverseExponentialCDF(double mu, double sigma, double prob)
        {
            return exponentialInverseCDF(mu, sigma, prob);
        }

        // Exponential probability density function
        public static double exponentialPDF(double mu, double sigma, double x)
        {
            double arg = (x - mu)/sigma;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = Math.Exp(-arg)/sigma;
            }
            return y;
        }

        // Exponential probability density function
        public static double exponential(double mu, double sigma, double x)
        {
            double arg = (x - mu)/sigma;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = Math.Exp(-arg)/sigma;
            }
            return y;
        }

        // Exponential m_mean
        public static double exponentialMean(double mu, double sigma)
        {
            return mu + sigma;
        }

        // Exponential standard deviation
        public static double exponentialStandardDeviation(double sigma)
        {
            return sigma;
        }

        // Exponential standard deviation
        public static double exponentialStandDev(double sigma)
        {
            return sigma;
        }


        // Exponential mode
        public static double exponentialMode(double mu)
        {
            return mu;
        }

        // Exponential median
        public static double exponentialMedian(double mu, double sigma)
        {
            return mu + sigma*Math.Log(2.0D);
        }

        // Returns an array of Exponential random deviates - clock seed
        // m_mu  =  location parameter, m_sigma = cale parameter, gamma = shape parametern = Length of array
        public static double[] exponentialRand(double mu, double sigma, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = mu - Math.Log(1.0D - rr.NextDouble())*sigma;
            }
            return ran;
        }

        // Returns an array of Exponential random deviates - user supplied seed
        // m_mu  =  location parameter, m_sigma = cale parameter, gamma = shape parametern = Length of array
        public static double[] exponentialRand(double mu, double sigma, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = mu - Math.Log(1.0D - rr.NextDouble())*sigma;
            }
            return ran;
        }

        // Exponential order statistic medians (n points)
        public static double[] exponentialOrderStatisticMedians(double mu, double sigma, int n)
        {
            double nn = n;
            double[] eosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                eosm[i] = inverseExponentialCDF(mu, sigma, uosm[i]);
            }
            return eosm;
        }


        // RAYLEIGH DISTRIBUTION

        // Rayleigh cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double rayleighCDF(double beta, double upperlimit)
        {
            double arg = (upperlimit)/beta;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = 1.0D - Math.Exp(-arg*arg/2.0D);
            }
            return y;
        }

        // Rayleigh cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double rayleighProb(double beta, double upperlimit)
        {
            double arg = (upperlimit)/beta;
            double y = 0.0D;
            if (arg > 0.0D)
            {
                y = 1.0D - Math.Exp(-arg*arg/2.0D);
            }
            return y;
        }

        // Rayleigh cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double rayleighCDF(double beta, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit)/beta;
            double arg2 = (upperlimit)/beta;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = -Math.Exp(-arg1*arg1/2.0D);
            }
            if (arg2 >= 0.0D)
            {
                term2 = -Math.Exp(-arg2*arg2/2.0D);
            }
            return term2 - term1;
        }

        // Rayleigh cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double rayleighProb(double beta, double lowerlimit, double upperlimit)
        {
            double arg1 = (lowerlimit)/beta;
            double arg2 = (upperlimit)/beta;
            double term1 = 0.0D, term2 = 0.0D;
            if (arg1 >= 0.0D)
            {
                term1 = -Math.Exp(-arg1*arg1/2.0D);
            }
            if (arg2 >= 0.0D)
            {
                term2 = -Math.Exp(-arg2*arg2/2.0D);
            }
            return term2 - term1;
        }

        // Rayleigh Inverse Cumulative Density Function
        public static double rayleighInverseCDF(double beta, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = 0.0;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = beta*(Math.Sqrt(-Math.Log(1.0 - prob)));
                }
            }

            return icdf;
        }

        // Rayleigh Inverse Cumulative Density Function
        public static double inverseRayleighCDF(double beta, double prob)
        {
            return rayleighInverseCDF(beta, prob);
        }


        // Rayleigh probability density function
        public static double rayleighPDF(double beta, double x)
        {
            double arg = x/beta;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = (arg/beta)*Math.Exp(-arg*arg/2.0D)/beta;
            }
            return y;
        }

        // Rayleigh probability density function
        public static double rayleigh(double beta, double x)
        {
            double arg = x/beta;
            double y = 0.0D;
            if (arg >= 0.0D)
            {
                y = (arg/beta)*Math.Exp(-arg*arg/2.0D)/beta;
            }
            return y;
        }

        // Rayleigh m_mean
        public static double rayleighMean(double beta)
        {
            return beta*Math.Sqrt(Math.PI/2.0D);
        }

        // Rayleigh standard deviation
        public static double rayleighStandardDeviation(double beta)
        {
            return beta*Math.Sqrt(2.0D - Math.PI/2.0D);
        }

        // Rayleigh standard deviation
        public static double rayleighStandDev(double beta)
        {
            return beta*Math.Sqrt(2.0D - Math.PI/2.0D);
        }

        // Rayleigh mode
        public static double rayleighMode(double beta)
        {
            return beta;
        }

        // Rayleigh median
        public static double rayleighMedian(double beta)
        {
            return beta*Math.Sqrt(Math.Log(2.0D));
        }

        // Returns an array of Rayleigh random deviates - clock seed
        // beta = scale parameter, n = Length of array
        public static double[] rayleighRand(double beta, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Sqrt(-2.0D*Math.Log(1.0D - rr.NextDouble()))*beta;
            }
            return ran;
        }

        // Returns an array of Rayleigh random deviates - user supplied seed
        // beta = scale parameter, n = Length of array
        public static double[] rayleighRand(double beta, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Sqrt(-2.0D*Math.Log(1.0D - rr.NextDouble()))*beta;
            }
            return ran;
        }

        // Rayleigh order statistic medians (n points)
        public static double[] rayleighOrderStatisticMedians(double beta, int n)
        {
            double nn = n;
            double[] rosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                rosm[i] = inverseRayleighCDF(beta, uosm[i]);
            }
            return rosm;
        }


        // PARETO DISTRIBUTION

        // Pareto cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double paretoCDF(double alpha, double beta, double upperlimit)
        {
            double y = 0.0D;
            if (upperlimit >= beta)
            {
                y = 1.0D - Math.Pow(beta/upperlimit, alpha);
            }
            return y;
        }

        // Pareto cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double paretoProb(double alpha, double beta, double upperlimit)
        {
            double y = 0.0D;
            if (upperlimit >= beta)
            {
                y = 1.0D - Math.Pow(beta/upperlimit, alpha);
            }
            return y;
        }

        // Pareto cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double paretoCDF(double alpha, double beta, double lowerlimit, double upperlimit)
        {
            double term1 = 0.0D, term2 = 0.0D;
            if (lowerlimit >= beta)
            {
                term1 = -Math.Pow(beta/lowerlimit, alpha);
            }
            if (upperlimit >= beta)
            {
                term2 = -Math.Pow(beta/upperlimit, alpha);
            }
            return term2 - term1;
        }

        // Pareto cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double paretoProb(double alpha, double beta, double lowerlimit, double upperlimit)
        {
            double term1 = 0.0D, term2 = 0.0D;
            if (lowerlimit >= beta)
            {
                term1 = -Math.Pow(beta/lowerlimit, alpha);
            }
            if (upperlimit >= beta)
            {
                term2 = -Math.Pow(beta/upperlimit, alpha);
            }
            return term2 - term1;
        }

        // Pareto Inverse Cumulative Density Function
        public static double paretoInverseCDF(double alpha, double beta, double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }
            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = beta;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    icdf = beta/Math.Pow((1.0 - prob), 1.0/alpha);
                }
            }

            return icdf;
        }

        // Pareto Inverse Cumulative Density Function
        public static double inverseParetoCDF(double alpha, double beta, double prob)
        {
            return paretoInverseCDF(alpha, beta, prob);
        }


        // Pareto probability density function
        public static double paretoPDF(double alpha, double beta, double x)
        {
            double y = 0.0D;
            if (x >= beta)
            {
                y = alpha*Math.Pow(beta, alpha)/Math.Pow(x, alpha + 1.0D);
            }
            return y;
        }

        // Pareto probability density function
        public static double pareto(double alpha, double beta, double x)
        {
            double y = 0.0D;
            if (x >= beta)
            {
                y = alpha*Math.Pow(beta, alpha)/Math.Pow(x, alpha + 1.0D);
            }
            return y;
        }

        // Pareto m_mean
        public static double paretoMean(double alpha, double beta)
        {
            double y = double.NaN;
            if (alpha > 1.0D)
            {
                y = alpha*beta/(alpha - 1);
            }
            return y;
        }

        // Pareto standard deviation
        public static double paretoStandardDeviation(double alpha, double beta)
        {
            double y = double.NaN;
            if (alpha > 1.0D)
            {
                y = alpha*Fmath.square(beta)/(Fmath.square(alpha - 1)*(alpha - 2));
            }
            return y;
        }

        // Pareto standard deviation
        public static double paretoStandDev(double alpha, double beta)
        {
            double y = double.NaN;
            if (alpha > 1.0D)
            {
                y = alpha*Fmath.square(beta)/(Fmath.square(alpha - 1)*(alpha - 2));
            }
            return y;
        }

        // Pareto mode
        public static double paretoMode(double beta)
        {
            return beta;
        }

        // Returns an array of Pareto random deviates - clock seed
        public static double[] paretoRand(double alpha, double beta, int n)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper();
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Pow(1.0D - rr.NextDouble(), -1.0D/alpha)*beta;
            }
            return ran;
        }

        // Returns an array of Pareto random deviates - user supplied seed
        public static double[] paretoRand(double alpha, double beta, int n, long seed)
        {
            double[] ran = new double[n];
            RngWrapper rr = new RngWrapper((int) seed);
            for (int i = 0; i < n; i++)
            {
                ran[i] = Math.Pow(1.0D - rr.NextDouble(), -1.0D/alpha)*beta;
            }
            return ran;
        }

        // Pareto order statistic medians (n points)
        public static double[] paretoOrderStatisticMedians(double alpha, double beta, int n)
        {
            double nn = n;
            double[] posm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                posm[i] = inverseParetoCDF(alpha, beta, uosm[i]);
            }
            return posm;
        }


        // FITTING DATA TO ABOVE DISTRIBUTIONS

        // Fit a data set to one, several or all of the above distributions (instance)
        public void fitOneOrSeveralDistributions()
        {
            double[] dd = getArray_as_double();
            RegressionAn.fitOneOrSeveralDistributions(dd);
        }

        // Fit a data set to one, several or all of the above distributions (static)
        public static void fitOneOrSeveralDistributions(double[] array)
        {
            RegressionAn.fitOneOrSeveralDistributions(array);
        }


        // OUTLIER TESTING (STATIC)

        // Anscombe test for a upper outlier - output as List
        public static List<Object> upperOutliersAnscombeAsVector(double[] values, double constant)
        {
            List<Object> res = upperOutliersAnscombeAsArrayList(values, constant);
            List<Object> ret = null;
            if (res != null)
            {
                int n = res.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }


        // Anscombe test for a upper outlier as List
        public static List<Object> upperOutliersAnscombe(double[] values, double constant)
        {
            return upperOutliersAnscombeAsVector(values, constant);
        }


        // Anscombe test for a upper outlier - output as List
        public static List<Object> upperOutliersAnscombeAsArrayList(
            double[] values,
            double constant)
        {
            Stat am = new Stat(values);
            double[] copy0 = am.getArray_as_double();
            double[] copy1 = am.getArray_as_double();
            int nValues = values.Length;
            int nValues0 = nValues;
            List<Object> outers = new List<Object>();
            int nOutliers = 0;
            bool test = true;

            while (test)
            {
                double mean = am.mean_as_double();
                double standDev = am.standardDeviation_as_double();
                double max = am.getMaximum_as_double();
                int maxIndex = am.getMaximumIndex();
                double statistic = (max - mean)/standDev;
                if (statistic > constant)
                {
                    outers.Add(max);
                    outers.Add(maxIndex);
                    nOutliers++;
                    copy1 = new double[nValues - 1];
                    for (int i = maxIndex; i < nValues - 1; i++)
                    {
                        copy1[i] = copy0[i + 1];
                    }

                    nValues--;
                    am = new Stat((double[]) copy1.Clone());
                }
                else
                {
                    test = false;
                }
            }

            double[] outliers = null;
            int[] outIndices = null;

            if (nOutliers > 0)
            {
                outliers = new double[nOutliers];
                outIndices = new int[nOutliers];
                for (int i = 0; i < nOutliers; i++)
                {
                    outliers[i] = ((double)outers[2*i]);
                    outIndices[i] = ((int) outers[2*i + 1]);
                }
            }

            List<Object> ret = new List<Object>(4);
            ret.Add(nOutliers);
            ret.Add(outliers);
            ret.Add(outIndices);
            ret.Add(copy1);
            return ret;
        }

        // Anscombe test for a upper outlier - output as List
        public static List<Object> upperOutliersAnscombeAsVector(decimal[] values, decimal constant)
        {
            List<Object> res = upperOutliersAnscombeAsArrayList(values, constant);
            List<Object> ret = null;
            if (res != null)
            {
                int n = res.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }


        // Anscombe test for a upper outlier as List
        public static List<Object> upperOutliersAnscombe(decimal[] values, decimal constant)
        {
            return upperOutliersAnscombeAsVector(values, constant);
        }


        // Anscombe test for a upper outlier - output as List
        public static List<Object> upperOutliersAnscombeAsArrayList(decimal[] values, decimal constant)
        {
            Stat am = new Stat(values);
            decimal[] copy0 = am.getArray_as_decimal();
            decimal[] copy1 = am.getArray_as_decimal();
            int nValues = values.Length;
            int nValues0 = nValues;
            List<Object> outers = new List<Object>();
            int nOutliers = 0;
            bool test = true;
            while (test)
            {
                decimal mean = am.mean_as_decimal();
                decimal variance = am.variance_as_decimal();
                decimal max = am.getMaximum_as_decimal();
                int maxIndex = am.getMaximumIndex();
                decimal statistic = (max - (mean))/(variance);
                if (statistic.CompareTo(constant*(constant)) == 1)
                {
                    outers.Add(max);
                    outers.Add(maxIndex);
                    nOutliers++;
                    copy1 = new decimal[nValues - 1];
                    for (int i = maxIndex; i < nValues - 1; i++)
                    {
                        copy1[i] = copy0[i + 1];
                    }

                    nValues--;
                    am = new Stat((double[]) copy1.Clone());
                }
                else
                {
                    copy0 = null;
                    test = false;
                }
            }

            decimal[] outliers = null;
            int[] outIndices = null;

            if (nOutliers > 0)
            {
                outliers = new decimal[nOutliers];
                outIndices = new int[nOutliers];
                for (int i = 0; i < nOutliers; i++)
                {
                    outliers[i] = ((decimal)outers[2*i]);
                    outIndices[i] = ((int) outers[2*i + 1]);
                }
            }

            List<Object> ret = new List<Object>(4);
            ret.Add(nOutliers);
            ret.Add(outliers);
            ret.Add(outIndices);
            ret.Add(copy1);
            return ret;
        }


        // Anscombe test for a upper outlier - output as List
        public static List<Object> upperOutliersAnscombeAsVector(long[] values, long constant)
        {
            List<Object> res = upperOutliersAnscombeAsArrayList(values, constant);
            List<Object> ret = null;
            if (res != null)
            {
                int n = res.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }


        // Anscombe test for a upper outlier as List
        public static List<Object> upperOutliersAnscombe(long[] values, long constant)
        {
            return upperOutliersAnscombeAsVector(values, constant);
        }


        // Anscombe test for a upper outlier - output as List
        public static List<Object> upperOutliersAnscombeAsArrayList(long[] values, long constant)
        {
            ArrayMaths am = new ArrayMaths(values);
            decimal[] bd = am.getArray_as_decimal();
            decimal cd = constant;
            return upperOutliersAnscombeAsArrayList(bd, cd);
        }


        // Anscombe test for a lower outlier - output as List
        public static List<Object> lowerOutliersAnscombeAsVector(double[] values, double constant)
        {
            List<Object> res = lowerOutliersAnscombeAsArrayList(values, constant);
            List<Object> ret = null;
            if (res != null)
            {
                int n = res.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }


        // Anscombe test for a lower outlier as List
        public static List<Object> lowerOutliersAnscombe(double[] values, double constant)
        {
            return upperOutliersAnscombeAsVector(values, constant);
        }

        // Anscombe test for a lower outlier
        public static List<Object> lowerOutliersAnscombeAsArrayList(double[] values, double constant)
        {
            Stat am = new Stat(values);
            double[] copy0 = am.getArray_as_double();
            double[] copy1 = am.getArray_as_double();
            int nValues = values.Length;
            int nValues0 = nValues;
            List<Object> outers = new List<Object>();
            int nOutliers = 0;
            bool test = true;

            while (test)
            {
                double mean = am.mean_as_double();
                double standDev = am.standardDeviation_as_double();
                double min = am.getMinimum_as_double();
                int minIndex = am.getMinimumIndex();
                double statistic = (mean - min)/standDev;
                if (statistic > constant)
                {
                    outers.Add(min);
                    outers.Add(minIndex);
                    nOutliers++;
                    copy1 = new double[nValues - 1];
                    for (int i = minIndex; i < nValues - 1; i++)
                    {
                        copy1[i] = copy0[i + 1];
                    }

                    nValues--;
                    am = new Stat((double[]) copy1.Clone());
                }
                else
                {
                    test = false;
                }
            }

            double[] outliers = null;
            int[] outIndices = null;

            if (nOutliers > 0)
            {
                outliers = new double[nOutliers];
                outIndices = new int[nOutliers];
                for (int i = 0; i < nOutliers; i++)
                {
                    outliers[i] = ((double)outers[2*i]);
                    outIndices[i] = ((int) outers[2*i + 1]);
                }
            }

            List<Object> ret = new List<Object>();
            ret.Add(nOutliers);
            ret.Add(outliers);
            ret.Add(outIndices);
            ret.Add(copy1);
            return ret;
        }

        // Anscombe test for a lower outlier - output as List
        public static List<Object> lowerOutliersAnscombeAsVector(decimal[] values, decimal constant)
        {
            List<Object> res = lowerOutliersAnscombeAsArrayList(values, constant);
            List<Object> ret = null;
            if (res != null)
            {
                int n = res.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }

        // Anscombe test for a lower outlier as List
        public static List<Object> lowerOutliersAnscombe(decimal[] values, decimal constant)
        {
            return upperOutliersAnscombeAsVector(values, constant);
        }

        // Anscombe test for a lower outlier
        public static List<Object> lowerOutliersAnscombeAsArrayList(decimal[] values, decimal constant)
        {
            Stat am = new Stat(values);
            decimal[] copy0 = am.getArray_as_decimal();
            decimal[] copy1 = am.getArray_as_decimal();
            int nValues = values.Length;
            int nValues0 = nValues;
            List<Object> outers = new List<Object>();
            int nOutliers = 0;
            bool test = true;
            while (test)
            {
                decimal mean = am.mean_as_decimal();
                decimal variance = am.variance_as_decimal();
                decimal min = am.getMinimum_as_decimal();
                int minIndex = am.getMinimumIndex();
                decimal statistic = (mean - (min))/(variance);
                if (statistic.CompareTo(constant*(constant)) == 1)
                {
                    outers.Add(min);
                    outers.Add(minIndex);
                    nOutliers++;
                    copy1 = new decimal[nValues - 1];
                    for (int i = minIndex; i < nValues - 1; i++)
                    {
                        copy1[i] = copy0[i + 1];
                    }

                    nValues--;
                    am = new Stat((double[]) copy1.Clone());
                }
                else
                {
                    mean = 0;
                    variance = 0;
                    statistic = 0;
                    copy0 = null;
                    test = false;
                }
            }

            decimal[] outliers = null;
            int[] outIndices = null;

            if (nOutliers > 0)
            {
                outliers = new decimal[nOutliers];
                outIndices = new int[nOutliers];
                for (int i = 0; i < nOutliers; i++)
                {
                    outliers[i] = ((decimal)outers[2*i]);
                    outIndices[i] = ((int) outers[2*i + 1]);
                }
            }

            List<Object> ret = new List<Object>();
            ret.Add(nOutliers);
            ret.Add(outliers);
            ret.Add(outIndices);
            ret.Add(copy1);
            return ret;
        }


        // Anscombe test for a lower outlier - output as List
        public static List<Object> lowerOutliersAnscombeAsVector(long[] values, long constant)
        {
            List<Object> res = lowerOutliersAnscombeAsArrayList(values, constant);
            List<Object> ret = null;
            if (res != null)
            {
                int n = res.Count;
                ret = new List<Object>(n);
                for (int i = 0; i < n; i++)
                {
                    ret.Add(res[i]);
                }
            }
            return ret;
        }

        // Anscombe test for a lower outlier as List
        public static List<Object> lowerOutliersAnscombe(long[] values, long constant)
        {
            return upperOutliersAnscombeAsVector(values, constant);
        }

        // Anscombe test for a lower outlier
        public static List<Object> lowerOutliersAnscombeAsArrayList(long[] values, long constant)
        {
            ArrayMaths am = new ArrayMaths(values);
            decimal[] bd = am.getArray_as_decimal();
            decimal cd = constant;
            return lowerOutliersAnscombeAsArrayList(bd, cd);
        }


        //  METHODS OVERRIDING ArrayMaths METHODS
        // DEEP COPY
        // Copy to a new instance of Stat
        public new Stat Copy()
        {
            Stat am = new Stat();

            if (amWeights == null)
            {
                am.amWeights = null;
            }
            else
            {
                am.amWeights = amWeights;
            }
            am.weightsSupplied = weightsSupplied;
            am.upperOutlierDetails = new List<Object>();
            if (upperOutlierDetails.Count != 0)
            {
                int hold0 = (int) upperOutlierDetails[0];
                am.upperOutlierDetails.Add(hold0);
                am.upperOutlierDetails.Add(upperOutlierDetails[1]);
                int[] hold2 = (int[]) upperOutlierDetails[2];
                am.upperOutlierDetails.Add(hold2);
                am.upperOutlierDetails.Add(upperOutlierDetails[3]);
            }
            am.upperDone = upperDone;
            am.lowerOutlierDetails = new List<Object>();
            if (lowerOutlierDetails.Count != 0)
            {
                int hold0 = (int) lowerOutlierDetails[0];
                am.lowerOutlierDetails.Add(hold0);
                am.lowerOutlierDetails.Add(lowerOutlierDetails[1]);
                int[] hold2 = (int[]) lowerOutlierDetails[2];
                am.lowerOutlierDetails.Add(hold2);
                am.lowerOutlierDetails.Add(lowerOutlierDetails[3]);
            }
            am.lowerDone = lowerDone;

            am.intLength = intLength;
            am.maxIndex = maxIndex;
            am.minIndex = minIndex;
            am.sumDone = sumDone;
            am.productDone = productDone;
            am.sumlongToDouble = sumlongToDouble;
            am.productlongToDouble = productlongToDouble;
            am.type = type;
            if (originalTypes == null)
            {
                am.originalTypes = null;
            }
            else
            {
                am.originalTypes = (int[]) originalTypes.Clone();
            }
            if (sortedIndices == null)
            {
                am.sortedIndices = null;
            }
            else
            {
                am.sortedIndices = (int[]) sortedIndices.Clone();
            }
            am.blnSuppressMessages = blnSuppressMessages;
            am.m_minmax = new List<Object>();
            if (m_minmax.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                        double dd = ((double)m_minmax[0]);
                        am.m_minmax.Add(dd);
                        dd = ((double)m_minmax[1]);
                        am.m_minmax.Add(dd);
                        break;
                    case 4:
                    case 5:
                        long ll = ((long) m_minmax[0]);
                        am.m_minmax.Add(ll);
                        ll = ((long) m_minmax[1]);
                        am.m_minmax.Add((ll));
                        break;
                    case 2:
                    case 3:
                        float ff = ((float) m_minmax[0]);
                        am.m_minmax.Add(ff);
                        ff = ((float) m_minmax[1]);
                        am.m_minmax.Add(ff);
                        break;
                    case 6:
                    case 7:
                        int ii = ((int) m_minmax[0]);
                        am.m_minmax.Add(ii);
                        ii = ((int) m_minmax[1]);
                        am.m_minmax.Add(ii);
                        break;
                    case 8:
                    case 9:
                        short ss = ((short) m_minmax[0]);
                        am.m_minmax.Add(ss);
                        ss = (short) m_minmax[1];
                        am.m_minmax.Add(ss);
                        break;
                    case 10:
                    case 11:
                        byte bb = ((byte) m_minmax[0]);
                        am.m_minmax.Add((bb));
                        ss = ((byte) m_minmax[1]);
                        am.m_minmax.Add(((bb)));
                        break;
                    case 12:
                        decimal bd = (decimal) m_minmax[0];
                        am.m_minmax.Add(bd);
                        bd =  (decimal)m_minmax[1];
                        am.m_minmax.Add(bd);
                        bd = 0;
                        break;
                    case 13:
                        long bi = (long) m_minmax[0];
                        am.m_minmax.Add(bi);
                        bi = (long) m_minmax[1];
                        am.m_minmax.Add(bi);
                        bi = 0;
                        break;
                    case 16:
                    case 17:
                        int iii = ((int) m_minmax[0]);
                        am.m_minmax.Add(iii);
                        iii = ((int) m_minmax[1]);
                        am.m_minmax.Add(iii);
                        break;
                }
            }

            am.summ = new List<Object>();
            if (summ.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 18:
                        double dd = ((double)summ[0]);
                        am.summ.Add(dd);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        if (sumlongToDouble)
                        {
                            double dd2 = ((double)summ[0]);
                            am.summ.Add(dd2);
                        }
                        else
                        {
                            long ll = ((long) summ[0]);
                            am.summ.Add((ll));
                        }
                        break;
                    case 12:
                        decimal bd = (decimal) summ[0];
                        am.summ.Add(bd);
                        break;
                    case 13:
                        long bi = (long) summ[0];
                        am.summ.Add(bi);
                        break;
                    case 14:
                        ComplexClass cc = (ComplexClass) summ[0];
                        am.summ.Add(cc);
                        break;
                    case 15:
                    default:
                        throw new ArgumentException("Data type not identified by this method");
                }
            }

            am.productt = new List<Object>();
            if (productt.Count != 0)
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 18:
                        double dd = ((double)productt[0]);
                        am.productt.Add(dd);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 16:
                    case 17:
                        if (sumlongToDouble)
                        {
                            double dd2 = ((double)productt[0]);
                            am.productt.Add(dd2);
                        }
                        else
                        {
                            long ll = ((long) productt[0]);
                            am.productt.Add((ll));
                        }
                        break;
                    case 12:
                        decimal bd =(decimal)  productt[0];
                        am.productt.Add(bd);
                        break;
                    case 13:
                        long bi = (long) productt[0];
                        am.productt.Add(bi);
                        break;
                    case 14:
                        ComplexClass cc = (ComplexClass) productt[0];
                        am.productt.Add(cc);
                        break;
                    case 15:
                    default:
                        throw new ArgumentException("Data type not identified by this method");
                }
            }


            switch (type)
            {
                case 0:
                case 1:
                    double[] dd = (double[]) getArray_as_double().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(dd[i]);
                    }
                    break;
                case 2:
                case 3:
                    float[] ff = (float[]) getArray_as_float().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ff[i]);
                    }
                    break;
                case 4:
                case 5:
                    long[] ll = (long[]) getArray_as_long().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((ll[i]));
                    }
                    break;
                case 6:
                case 7:
                    int[] ii = (int[]) getArray_as_int().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ii[i]);
                    }
                    break;
                case 8:
                case 9:
                    short[] ss = (short[]) getArray_as_short().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ss[i]);
                    }
                    break;
                case 10:
                case 11:
                    byte[] bb = (byte[]) getArray_as_byte().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bb[i]);
                    }
                    break;
                case 12:
                    decimal[] bd = (decimal[]) getArray_as_decimal().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bd[i]);
                    }
                    break;
                case 13:
                    long[] bi = (long[]) getArray_as_int().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(bi[i]);
                    }
                    break;
                case 14:
                    ComplexClass[] ccc = getArray_as_Complex();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(ccc[i].Copy());
                    }
                    break;
                case 15:
                case 16:
                case 17:
                    char[] cc = (char[]) getArray_as_char().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add((cc[i]));
                    }
                    break;
                case 18:
                    string[] sss = (string[]) getArray_as_string().Clone();
                    for (int i = 0; i < intLength; i++)
                    {
                        am.m_array.Add(sss[i]);
                    }
                    break;
            }

            return am;
        }

        public new Stat plus(double constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(float constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(long constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(int constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(short constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(byte constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(char constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(decimal constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(ComplexClass constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(string constant)
        {
            return base.plus(constant).toStat();
        }

        public new Stat plus(Stat arrays)
        {
            return base.plus(arrays).toStat();
        }

        public new Stat plus(ArrayMaths arraym)
        {
            return base.plus(arraym).toStat();
        }

        public new Stat plus(List<Object> arrayl)
        {
            return base.plus(arrayl).toStat();
        }

        public new Stat plus(double[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(float[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(long[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(int[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(short[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(byte[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(char[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(decimal[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(ComplexClass[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat plus(string[] array)
        {
            return base.plus(array).toStat();
        }

        public new Stat minus(double constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(float constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(long constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(int constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(short constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(byte constant)
        {
            return base.minus(constant).toStat();
        }

        public Stat minus(char constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(decimal constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(ComplexClass constant)
        {
            return base.minus(constant).toStat();
        }

        public new Stat minus(Stat arrays)
        {
            return base.minus(arrays).toStat();
        }

        public new Stat minus(ArrayMaths arraym)
        {
            return base.minus(arraym).toStat();
        }

        public new Stat minus(List<Object> vec)
        {
            return base.minus(vec).toStat();
        }

        public new Stat minus(double[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(float[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(long[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(int[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(short[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(byte[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(decimal[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat minus(ComplexClass[] array)
        {
            return base.minus(array).toStat();
        }

        public new Stat times(double constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(float constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(long constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(int constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(short constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(byte constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(decimal constant)
        {
            return base.times(constant).toStat();
        }

        public new Stat times(ComplexClass constant)
        {
            return base.times(constant).toStat();
        }


        public new Stat over(double constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(float constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(long constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(int constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(short constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(byte constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(decimal constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat over(ComplexClass constant)
        {
            return base.over(constant).toStat();
        }

        public new Stat Pow(double n)
        {
            return base.Pow(n).toStat();
        }

        public new Stat Pow(float n)
        {
            return base.Pow(n).toStat();
        }

        public new Stat Pow(long n)
        {
            return base.Pow(n).toStat();
        }

        public new Stat Pow(int n)
        {
            return base.Pow(n).toStat();
        }

        public new Stat Pow(short n)
        {
            return base.Pow(n).toStat();
        }

        public new Stat Pow(byte n)
        {
            return base.Pow(n).toStat();
        }

        public Stat Pow(decimal n)
        {
            return base.Pow((double)n).toStat();
        }

        public new Stat sqrt()
        {
            return base.sqrt().toStat();
        }

        public new Stat oneOverSqrt()
        {
            return base.oneOverSqrt().toStat();
        }

        public new Stat abs()
        {
            return base.abs().toStat();
        }

        public new Stat Log()
        {
            return base.Log().toStat();
        }

        public new Stat log2()
        {
            return base.log2().toStat();
        }

        public new Stat Log10()
        {
            return base.Log10().toStat();
        }

        public new Stat antilog10()
        {
            return base.antilog10().toStat();
        }

        public new Stat xLog2x()
        {
            return base.xLog2x().toStat();
        }

        public new Stat xLogEx()
        {
            return base.xLogEx().toStat();
        }

        public new Stat xLog10x()
        {
            return base.xLog10x().toStat();
        }

        public new Stat minusxLog2x()
        {
            return base.minusxLog2x().toStat();
        }

        public new Stat minusxLogEx()
        {
            return base.minusxLogEx().toStat();
        }

        public new Stat minusxLog10x()
        {
            return base.minusxLog10x().toStat();
        }

        public new Stat Exp()
        {
            return base.Exp().toStat();
        }

        public new Stat invert()
        {
            return base.invert().toStat();
        }

        public new Stat negate()
        {
            return base.negate().toStat();
        }

        public new Stat sort()
        {
            return base.sort().toStat();
        }

        public new Stat sort(int[] indices)
        {
            return base.sort(indices).toStat();
        }

        public new Stat reverse()
        {
            return base.reverse().toStat();
        }

        public new Stat concatenate(Stat xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(ArrayMaths xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(double[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(float[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(long[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(int[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(short[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(byte[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(char[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(string[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(decimal[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat concatenate(ComplexClass[] xx)
        {
            return base.concatenate(xx).toStat();
        }

        public new Stat truncate(int n)
        {
            return base.truncate(n).toStat();
        }

        public new Stat Floor()
        {
            return base.Floor().toStat();
        }

        public new Stat Ceiling()
        {
            return base.Ceiling().toStat();
        }

        public new Stat rint()
        {
            return base.rint().toStat();
        }

        public new Stat randomize()
        {
            return base.randomize().toStat();
        }

        public new Stat randomise()
        {
            return base.randomize().toStat();
        }
    }

    // CLASSES NEEDED BY METHODS IN THE ABOVE Stat CLASS

    // Class to evaluate the linear correlation coefficient probablity function
    // Needed in calls to Integration.gaussQuad
    internal class CorrCoeff : IntegralFunction
    {
        public double a;

        #region IntegralFunction Members

        public double function(double x)
        {
            double y = Math.Pow((1.0D - x*x), a);
            return y;
        }

        #endregion
    }

    // Class to evaluate the normal distribution function
    internal class GaussianFunct : RealRootFunction
    {
        public double m_cfd;
        public double m_mean;
        public double m_sd;

        #region RealRootFunction Members

        public double function(double x)
        {
            UnivNormalDist univNormalDist =
                new UnivNormalDist(m_mean, m_sd,
                                   new RngWrapper());

            double y = m_cfd - univNormalDist.Cdf(x);

            return y;
        }

        #endregion
    }

    // Class to evaluate the Student's t-function
    internal class StudentTfunct : RealRootFunction
    {
        public double m_cfd;
        public int m_nu;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = m_cfd - Stat.studentTcdf(x, m_nu);

            return y;
        }

        #endregion
    }


    // Class to evaluate the Engset probability equation
    internal class EngsetProb : RealRootFunction
    {
        public double numberOfSources;
        public double offeredTraffic;
        public double totalResources;

        #region RealRootFunction Members

        public double function(double x)
        {
            double mTerm = offeredTraffic/(numberOfSources - offeredTraffic*(1.0D - x));
            double pNumer = Stat.logFactorial(numberOfSources - 1) - Stat.logFactorial(totalResources) -
                            Stat.logFactorial(numberOfSources - 1 - totalResources);
            double pDenom = 0.0D;
            double iDenom = 0.0D;
            double iCount = 0.0D;
            double pTerm = 0.0D;

            while (iCount <= totalResources)
            {
                iDenom = Stat.logFactorial(numberOfSources - 1) - Stat.logFactorial(iCount) -
                         Stat.logFactorial(numberOfSources - 1 - iCount);
                iDenom += (iCount - totalResources)*Math.Log(mTerm);
                pDenom += Math.Exp(iDenom);
                iCount += 1.0D;
            }
            pTerm = Math.Exp(pNumer - Math.Log(pDenom));

            return x - pTerm;
        }

        #endregion
    }


    // Class to evaluate the  Engset load equation
    internal class EngsetLoad : RealRootFunction
    {
        public double blockingProbability;
        public double numberOfSources;
        public double totalResources;

        #region RealRootFunction Members

        public double function(double x)
        {
            return blockingProbability - Stat.engsetProbability(x, totalResources, numberOfSources);
        }

        #endregion
    }

    // Class to evaluate the chi-square distribution function
    internal class ChiSquareFunct : RealRootFunction
    {
        public double m_cfd;
        public int m_nu;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = m_cfd - Stat.chiSquareCDF(x, m_nu);

            return y;
        }

        #endregion
    }

    // Class to evaluate the F-distribution function
    internal class FdistribtionFunct : RealRootFunction
    {
        public double cfd;
        public int nu1;
        public int nu2;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = cfd - (1.0 - Stat.fCompCDF(x, nu1, nu2));
            // double y = m_cfd - Stat.fCompCDF(x, nu1, nu2);

            return y;
        }

        #endregion
    }

    // Class to evaluate the two parameter log-normal distribution function
    internal class LogNormalTwoParFunct : RealRootFunction
    {
        public double m_cfd;
        public double m_mu;
        public double m_sigma;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = m_cfd - Stat.logNormalCDF(m_mu, m_sigma, x);

            return y;
        }

        #endregion
    }
}
