#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Complex;
using HC.Core.Helpers;
using HC.Core.Io;

#endregion

namespace HC.Analytics.Analysis
{
    /*
    *   Fourier Transform
    *
    *   This class contains the method for performing a
    *   Fast Fourier Transform (FFT) and associated methods
    *   e.g. for estimation of a power spectrum, for windowing data,
    *   obtaining a time-frequency representation.
    *   Basic FFT method is adapted from the Numerical Recipes
    *   methods written in the C language:
    *   Numerical Recipes in C, The Art of Scientific Computing,
    *   W.H. Press, S.A. Teukolsky, W.T. Vetterling & B.P. Flannery,
    *   Cambridge University Press, 2nd Edition (1992) pp 496 - 558.
    *   (http://www.nr.com/).
    *
    *   AUTHOR: Dr Michael Thomas Flanagan
    *   DATE:       20 December 2003
    *   UPDATES:    26 July 2004, 31 August 2004, 15 June 2005, 27 January 2006
    *   UPDATES:    18 February 2006  method correlation correction (thanks to Daniel Mader, Universt�t Freiburg -- IMTEK)
                    7 July 2008
    *
    *   DOCUMENTATION:
    *   See Michael Thomas Flanagan's Java library on-line web page:
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/FourierTranasform.html
    *   http://www.ee.ucl.ac.uk/~mflanaga/java/
    *
    *
    *   Copyright (c) 2003 - 2008  Michael Thomas Flanagan
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

    public class FourierTransform
    {
        #region Members

        private static int numberOfWarnings = 9; // Number of warnings
        private static long serialVersionUID = 1L; // serial version unique identifier

        private readonly bool[] warning = new bool[numberOfWarnings];
        // warnings - if warning[x] = true warningText[x] is printed

        private readonly string[] windowNames = {
                                                    "no windowing applied", "Rectangular (square, box-car)",
                                                    "Bartlett (triangular)", "Welch", "Hann (Hanning)", "Hamming",
                                                    "Kaiser"
                                                    , "Gaussian"
                                                };

        private ComplexClass[] m_complexCorr; // corresponding array to hold the data to be correlated with first data set
        private ComplexClass[] m_complexData; // array to hold the input data as a set of ComplexClass numbers
        private bool m_complexDataSet; // if true - the complex data input array has been filled, if false - it has not.
        private bool m_correlateDone; // = false - correlation has not been called
        private double[,] m_correlationArray; // first row - array to hold time lags

        private bool m_dataAltered; // set to true if originalDataLength altered, e.g. by point deletion or padding.

        // real and imaginary parts, e.g. real_0 imag_0, real_1 imag_1, for the fast Fourier Transform method
        private double[] m_dblFftCorr; // corresponding array to hold the  data to be correlated with first data set

        private double[] m_dblFftCorrWindow;
        // corresponding array to hold the data to be correlated with first data set

        private double[] m_dblFftData; // array to hold a data set of complex numbers arranged as alternating
        private double m_deltaT = 1.0D; // Sampling period (needed only for true graphical output)
        private bool m_deltaTset; // true if sampling period has been set

        private int m_fftDataLength;
        // working data length - usually the smallest power of two that is either equal to originalDataLength or larger than originalDataLength

        private bool m_fftDataSet; // if true - the fftData array has been filled, if false - it has not.

        private double[] m_fftDataWindow; // array holding fftData array elements multiplied by the windowing weights
        //private bool m_fftDone; // = false - basicFft has not been called
        //private double[] m_fftResp; // corresponding array to hold the  response to be convolved with first data set
        private double m_gaussianAlpha = 2.5D; //  Gaussian window constant, alpha
        private double m_kaiserAlpha = 2.0D; //  Kaiser window constant, alpha
        private int m_numShortFreq; // number of frequency points in short time Fourier transform
        private int m_numShortTimes; // number of time points in short time Fourier transform

        private int m_originalDataLength;
        // original data length value; the working data length may be altered by deletion or padding

        private bool m_overlap; //	Data segment overlap option
        private int m_plotLineOption; // PlotPowerSpectrum line option
        // = 0 points linked by straight line [default option]
        // = 1 cubic spline interpolation
        // = 2 no line - only points

        private int m_plotPointOption; // PlotPowerSpectrum point option

        // = true  - basicFft has been called

        private double[,] m_powerSpectrumEstimate; // first row - array to hold frequencies
        // second row - array to hold estimated power density (psd) spectrum
        private bool m_powSpecDone; // = false - PowerSpectrum has not been called
        // = true  - PowerSpectrum has been called
        private int m_psdNumberOfPoints; // Number of points in the estimated power spectrum
        private bool m_segLenSet; //  true of segment length has been set

        private int m_segmentLength; //	Number of of data points in a segment
        private int m_segmentNumber = 1; //	Number of segments into which the data has been split
        //	= true; overlap by half segment length - smallest spectral variance per data point
        //		good where data already recorded and data reduction is after the process
        //	= false;  no overlap - smallest spectral variance per conputer operation
        //		good for real time data collection where data reduction is computer limited
        private bool m_segNumSet; //  true if segment number has been set
        //  first row = blank cell followed by time vector
        //  first column = blank cell followed by frequency vector
        //  each cell is then the m_mean square amplitude at that frequency and time
        private bool m_shortTimeDone; // = true when short time Fourier Transform has been performed
        private string m_shortTitle = " "; // Short Time Fourier Transform graph title
        private double m_sumOfSquaredWeights; //  Sum of the windowing weights
        private double[,] m_timeFrequency; // matrix of time against frequency m_mean square powers from shoert time FT
        private ComplexClass[] m_transformedDataComplex; // transformed data set of ComplexClass numbers
        private double[] m_transformedDataFft; // transformed data set of double adjacent real and imaginary parts
        private bool m_windowApplied; //  = true when data has been multiplied by windowing weights, otherwise = false
        //private string m_windowName; // current window name
        private int m_windowOption; //	Window Option
        private double[] weights; //  windowing weights
        //private bool m_windowSet; //  = true when a windowing option has been chosen, otherwise = false

        #endregion

        #region Constructors

        // constructors
        // No initialisation of the data variables
        public FourierTransform()
        {
            //m_windowName = windowNames[0];
            for (int i = 0; i < numberOfWarnings; i++)
            {
                warning[i] = false;
            }
        }

        // constuctor entering a data array of real numbers
        public FourierTransform(double[] realData)
        {
            //m_windowName = windowNames[0];
            m_originalDataLength = realData.Length;
            m_fftDataLength = nextPowerOfTwo(m_originalDataLength);
            m_complexData = ComplexClass.oneDarray(m_fftDataLength);
            for (int i = 0; i < m_originalDataLength; i++)
            {
                m_complexData[i].setReal(realData[i]);
                m_complexData[i].setImag(0.0D);
            }
            for (int i = m_originalDataLength; i < m_fftDataLength; i++)
            {
                m_complexData[i].reset(0.0D, 0.0D);
            }
            m_complexDataSet = true;

            m_dblFftData = new double[2*m_fftDataLength];
            int j = 0;
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_dblFftData[j] = m_complexData[i].getReal();
                j++;
                m_dblFftData[j] = 0.0D;
                j++;
            }
            m_fftDataSet = true;

            m_fftDataWindow = new double[2*m_fftDataLength];
            weights = new double[m_fftDataLength];
            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

            m_transformedDataFft = new double[2*m_fftDataLength];
            m_transformedDataComplex = ComplexClass.oneDarray(m_fftDataLength);
            m_segmentLength = m_fftDataLength;

            for (int i = 0; i < numberOfWarnings; i++)
            {
                warning[i] = false;
            }
        }

        // constuctor entering a data array of complex numbers
        public FourierTransform(ComplexClass[] data)
        {
            //m_windowName = windowNames[0];
            m_originalDataLength = data.Length;
            m_fftDataLength = nextPowerOfTwo(m_originalDataLength);
            m_complexData = ComplexClass.oneDarray(m_fftDataLength);
            for (int i = 0; i < m_originalDataLength; i++)
            {
                m_complexData[i] = data[i].Copy();
            }
            for (int i = m_originalDataLength; i < m_fftDataLength; i++)
            {
                m_complexData[i].reset(0.0D, 0.0D);
            }
            m_complexDataSet = true;

            m_dblFftData = new double[2*m_fftDataLength];
            int j = 0;
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_dblFftData[j] = m_complexData[i].getReal();
                j++;
                m_dblFftData[j] = m_complexData[i].getImag();
                j++;
            }
            m_fftDataSet = true;

            m_fftDataWindow = new double[2*m_fftDataLength];
            weights = new double[m_fftDataLength];
            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

            m_transformedDataFft = new double[2*m_fftDataLength];
            m_transformedDataComplex = ComplexClass.oneDarray(m_fftDataLength);
            m_segmentLength = m_fftDataLength;

            for (int i = 0; i < numberOfWarnings; i++)
            {
                warning[i] = false;
            }
        }

        #endregion

        // Enter a data array of real numbers
        public void setData(double[] realData)
        {
            m_originalDataLength = realData.Length;
            m_fftDataLength = nextPowerOfTwo(m_originalDataLength);
            m_complexData = ComplexClass.oneDarray(m_fftDataLength);
            for (int i = 0; i < m_originalDataLength; i++)
            {
                m_complexData[i].setReal(realData[i]);
                m_complexData[i].setImag(0.0D);
            }
            for (int i = m_originalDataLength; i < m_fftDataLength; i++)
            {
                m_complexData[i].reset(0.0D, 0.0D);
            }
            m_complexDataSet = true;

            m_dblFftData = new double[2*m_fftDataLength];
            int j = 0;
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_dblFftData[j] = m_complexData[i].getReal();
                j++;
                m_dblFftData[j] = 0.0D;
                j++;
            }
            m_fftDataSet = true;

            m_fftDataWindow = new double[2*m_fftDataLength];
            weights = new double[m_fftDataLength];
            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

            m_transformedDataFft = new double[2*m_fftDataLength];
            m_transformedDataComplex = ComplexClass.oneDarray(m_fftDataLength);

            if (m_segNumSet)
            {
                setSegmentNumber(m_segmentNumber);
            }
            else
            {
                if (m_segLenSet)
                {
                    setSegmentLength(m_segmentLength);
                }
                else
                {
                    m_segmentLength = m_fftDataLength;
                }
            }
        }

        // Enter a data array of complex numbers
        public void setData(ComplexClass[] data)
        {
            m_originalDataLength = data.Length;
            m_fftDataLength = nextPowerOfTwo(m_originalDataLength);
            m_complexData = ComplexClass.oneDarray(m_fftDataLength);
            for (int i = 0; i < m_originalDataLength; i++)
            {
                m_complexData[i] = data[i].Copy();
            }
            for (int i = m_originalDataLength; i < m_fftDataLength; i++)
            {
                m_complexData[i].reset(0.0D, 0.0D);
            }
            m_complexDataSet = true;

            m_dblFftData = new double[2*m_fftDataLength];
            int j = 0;
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_dblFftData[j] = m_complexData[i].getReal();
                j++;
                m_dblFftData[j] = m_complexData[i].getImag();
                j++;
            }
            m_fftDataSet = true;

            m_fftDataWindow = new double[2*m_fftDataLength];
            weights = new double[m_fftDataLength];
            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

            m_transformedDataFft = new double[2*m_fftDataLength];
            m_transformedDataComplex = ComplexClass.oneDarray(m_fftDataLength);

            if (m_segNumSet)
            {
                setSegmentNumber(m_segmentNumber);
            }
            else
            {
                if (m_segLenSet)
                {
                    setSegmentLength(m_segmentLength);
                }
                else
                {
                    m_segmentLength = m_fftDataLength;
                }
            }
        }

        // Enter a data array of adjacent alternating real and imaginary parts for fft method, fastFourierTransform
        public void setFftData(double[] fftdata)
        {
            if (fftdata.Length%2 != 0)
            {
                throw new ArgumentException("data length must be an even number");
            }

            m_originalDataLength = fftdata.Length/2;
            m_fftDataLength = nextPowerOfTwo(m_originalDataLength);
            m_dblFftData = new double[2*m_fftDataLength];
            for (int i = 0; i < 2*m_originalDataLength; i++)
            {
                m_dblFftData[i] = fftdata[i];
            }
            for (int i = 2*m_originalDataLength; i < 2*m_fftDataLength; i++)
            {
                m_dblFftData[i] = 0.0D;
            }
            m_fftDataSet = true;

            m_complexData = ComplexClass.oneDarray(m_fftDataLength);
            int j = -1;
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_complexData[i].setReal(m_dblFftData[++j]);
                m_complexData[i].setImag(m_dblFftData[++j]);
            }
            m_complexDataSet = true;

            m_fftDataWindow = new double[2*m_fftDataLength];
            weights = new double[m_fftDataLength];
            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

            m_transformedDataFft = new double[2*m_fftDataLength];
            m_transformedDataComplex = ComplexClass.oneDarray(m_fftDataLength);

            if (m_segNumSet)
            {
                setSegmentNumber(m_segmentNumber);
            }
            else
            {
                if (m_segLenSet)
                {
                    setSegmentLength(m_segmentLength);
                }
                else
                {
                    m_segmentLength = m_fftDataLength;
                }
            }
        }

        // Get the input data array as ComplexClass
        public ComplexClass[] getComplexInputData()
        {
            if (!m_complexDataSet)
            {
                PrintToScreen.WriteLine("complex data set not entered or calculated - null returned");
            }
            return m_complexData;
        }

        // Get the input data array as adjacent real and imaginary pairs
        public double[] GetAlternateInputData()
        {
            if (!m_fftDataSet)
            {
                PrintToScreen.WriteLine("fft data set not entered or calculted - null returned");
            }
            return m_dblFftData;
        }

        // Get the windowed input data array as windowed adjacent real and imaginary pairs
        public double[] getAlternateWindowedInputData()
        {
            if (!m_fftDataSet)
            {
                PrintToScreen.WriteLine("fft data set not entered or calculted - null returned");
            }
            if (!m_fftDataSet)
            {
                PrintToScreen.WriteLine("fft data set not entered or calculted - null returned");
            }
            if (!m_windowApplied)
            {
                PrintToScreen.WriteLine("fft data set has not been multiplied by windowing weights");
            }
            return m_fftDataWindow;
        }

        // get the original number of data points
        public int getOriginalDataLength()
        {
            return m_originalDataLength;
        }

        // get the actual number of data points
        public int getUsedDataLength()
        {
            return m_fftDataLength;
        }

        // Set a samplimg period
        public void setDeltaT(double deltaT)
        {
            m_deltaT = deltaT;
            m_deltaTset = true;
        }

        // Get the samplimg period
        public double getDeltaT()
        {
            double ret = 0.0D;
            if (m_deltaTset)
            {
                ret = m_deltaT;
            }
            else
            {
                PrintToScreen.WriteLine("detaT has not been set - zero returned");
            }
            return ret;
        }

        // Set a Rectangular window option
        public void setRectangular()
        {
            m_windowOption = 1;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Bartlett window option
        public void setBartlett()
        {
            m_windowOption = 2;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Welch window option
        public void setWelch()
        {
            m_windowOption = 3;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Hann window option
        public void setHann()
        {
            m_windowOption = 4;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Hamming window option
        public void setHamming()
        {
            m_windowOption = 5;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Kaiser window option
        public void setKaiser(double alpha)
        {
            m_kaiserAlpha = alpha;
            m_windowOption = 6;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Kaiser window option
        // default option for alpha
        public void setKaiser()
        {
            m_windowOption = 6;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Gaussian window option
        public void setGaussian(double alpha)
        {
            if (alpha < 2.0D)
            {
                alpha = 2.0D;
                PrintToScreen.WriteLine(
                    "setGaussian; alpha must be greater than or equal to 2 - alpha has been reset to 2");
            }
            m_gaussianAlpha = alpha;
            m_windowOption = 7;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Set a Gaussian window option
        // default option for alpha
        public void setGaussian()
        {
            m_windowOption = 7;
            //m_windowSet = true;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = true;
            }
        }

        // Remove windowing
        public void removeWindow()
        {
            m_windowOption = 0;
            //m_windowSet = false;
            if (m_fftDataSet)
            {
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
                m_windowApplied = false;
            }
        }

        // Applies a window to the data
        private double windowData(double[] data, double[] window, double[] weight)
        {
            int m = data.Length;
            int n = m/2 - 1;
            int j = 0;
            double sum = 0.0D;
            switch (m_windowOption)
            {
                    // 0.  No windowing applied or remove windowing
                case 0:
                    // 1.  Rectangular
                case 1:
                    for (int i = 0; i <= n; i++)
                    {
                        weight[i] = 1.0D;
                        window[j] = data[j++];
                        window[j] = data[j++];
                    }
                    sum = n + 1;
                    break;
                    // 2.  Bartlett
                case 2:
                    for (int i = 0; i <= n; i++)
                    {
                        weight[i] = 1.0D - Math.Abs((i - n/2)/n/2);
                        sum += weight[i]*weight[i];
                        window[j] = data[j++]*weight[i];
                        window[j] = data[j++]*weight[i];
                    }
                    break;
                    // 3.  Welch
                case 3:
                    for (int i = 0; i <= n; i++)
                    {
                        weight[i] = 1.0D - Fmath.square((i - n/2)/n/2);
                        sum += weight[i]*weight[i];
                        window[j] = data[j++]*weight[i];
                        window[j] = data[j++]*weight[i];
                    }
                    break;
                    // 4.  Hann
                case 4:
                    for (int i = 0; i <= n; i++)
                    {
                        weight[i] = (1.0D - Math.Cos(2.0D*i*Math.PI/n))/2.0D;
                        sum += weight[i]*weight[i];
                        window[j] = data[j++]*weight[i];
                        window[j] = data[j++]*weight[i];
                    }
                    break;
                    // 5.  Hamming
                case 5:
                    for (int i = 0; i <= n; i++)
                    {
                        weight[i] = 0.54D + 0.46D*Math.Cos(2.0D*i*Math.PI/n);
                        sum += weight[i]*weight[i];
                        window[j] = data[j++]*weight[i];
                        window[j] = data[j++]*weight[i];
                    }
                    break;
                    // 6.  Kaiser
                case 6:
                    double denom = modBesselIo(Math.PI*m_kaiserAlpha);
                    double numer = 0.0D;
                    for (int i = 0; i <= n; i++)
                    {
                        numer = modBesselIo(Math.PI*m_kaiserAlpha*Math.Sqrt(1.0D - Fmath.square(2.0D*i/n - 1.0D)));
                        weight[i] = numer/denom;
                        sum += weight[i]*weight[i];
                        window[j] = data[j++]*weight[i];
                        window[j] = data[j++]*weight[i];
                    }
                    break;
                    // 6.  Kaiser
                case 7:
                    for (int i = 0; i <= n; i++)
                    {
                        weight[i] = Math.Exp(-0.5D*Fmath.square(m_gaussianAlpha*(2*i - n)/n));
                        sum += weight[i]*weight[i];
                        window[j] = data[j++]*weight[i];
                        window[j] = data[j++]*weight[i];
                    }
                    break;
            }
            return sum;
        }

        // return modified Bessel Function of the zeroth order (for Kaiser window)
        //   after numerical Recipe's bessi0
        //   - Abramowitz and Stegun coeeficients
        public static double modBesselIo(double arg)
        {
            double absArg = 0.0D;
            double poly = 0.0D;
            double bessel = 0.0D;

            if ((absArg = Math.Abs(arg)) < 3.75)
            {
                poly = arg/3.75;
                poly *= poly;
                bessel = 1.0D +
                         poly*
                         (3.5156229D +
                          poly*
                          (3.08989424D + poly*(1.2067492D + poly*(0.2659732 + poly*(0.360768e-1 + poly*0.45813e-2)))));
            }
            else
            {
                bessel = (Math.Exp(absArg)/Math.Sqrt(absArg))*
                         (0.39894228D +
                          poly*
                          (0.1328592e-1D +
                           poly*
                           (0.225319e-2 +
                            poly*
                            (-0.157565e-2 +
                             poly*
                             (0.916281e-2 +
                              poly*(-0.2057706e-1 + poly*(0.2635537e-1 + poly*(-0.1647633e-1 + poly*0.392377e-2))))))));
            }
            return bessel;
        }

        // get window option - see above for options
        public string getWindowOption()
        {
            string option = " ";
            switch (m_windowOption)
            {
                case 0:
                    option = "No windowing applied";
                    break;
                case 1:
                    option = "Rectangular";
                    break;
                case 2:
                    option = "Bartlett";
                    break;
                case 3:
                    option = "Welch";
                    break;
                case 4:
                    option = "Hann";
                    break;
                case 5:
                    option = "Hamming";
                    break;
                case 6:
                    option = "Kaiser";
                    break;
                case 7:
                    option = "Gaussian";
                    break;
            }
            return option;
        }

        // Get the windowing weights
        public double[] getWeights()
        {
            return weights;
        }

        // set the number of segments
        public void setSegmentNumber(int sNum)
        {
            m_segmentNumber = sNum;
            m_segNumSet = true;
            if (m_segLenSet)
            {
                m_segLenSet = false;
            }
        }

        // set the segment length
        public void setSegmentLength(int sLen)
        {
            m_segmentLength = sLen;
            m_segLenSet = true;
            if (m_segNumSet)
            {
                m_segNumSet = false;
            }
        }

        // check and set up the segments
        private void checkSegmentDetails()
        {
            if (!m_fftDataSet)
            {
                throw new ArgumentException("No fft data has been entered or calculated");
            }
            if (m_fftDataLength < 2)
            {
                throw new ArgumentException("More than one point, MANY MORE, are needed");
            }

            // check if data number is even
            if (m_fftDataLength%2 != 0)
            {
                PrintToScreen.WriteLine("Number of data points must be an even number");
                PrintToScreen.WriteLine("last point deleted");
                m_fftDataLength -= 1;
                m_dataAltered = true;
                warning[0] = true;
            }

            // check segmentation with no overlap
            if (m_segNumSet && !m_overlap)
            {
                if (m_fftDataLength%m_segmentNumber == 0)
                {
                    int segL = m_fftDataLength/m_segmentNumber;
                    if (checkPowerOfTwo(segL))
                    {
                        m_segmentLength = segL;
                        m_segLenSet = true;
                    }
                    else
                    {
                        PrintToScreen.WriteLine("segment length is not an integer power of two");
                        PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                        warning[1] = true;
                        m_segmentNumber = 1;
                        m_segmentLength = m_fftDataLength;
                        m_segLenSet = true;
                    }
                }
                else
                {
                    PrintToScreen.WriteLine("total data length divided by the number of segments is not an integer");
                    PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                    warning[2] = true;
                    m_segmentNumber = 1;
                    m_segmentLength = m_fftDataLength;
                    m_segLenSet = true;
                }
            }

            if (m_segLenSet && !m_overlap)
            {
                if (m_fftDataLength%m_segmentLength == 0)
                {
                    if (checkPowerOfTwo(m_segmentLength))
                    {
                        m_segmentNumber = m_fftDataLength/m_segmentLength;
                        m_segNumSet = true;
                    }
                    else
                    {
                        PrintToScreen.WriteLine("segment length is not an integer power of two");
                        PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                        warning[1] = true;
                        m_segmentNumber = 1;
                        m_segmentLength = m_fftDataLength;
                        m_segNumSet = true;
                    }
                }
                else
                {
                    PrintToScreen.WriteLine("total data length divided by the segment length is not an integer");
                    PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                    warning[3] = true;
                    m_segmentNumber = 1;
                    m_segmentLength = m_fftDataLength;
                    m_segNumSet = true;
                }
            }

            // check segmentation with overlap
            if (m_segNumSet && m_overlap)
            {
                if (m_fftDataLength%(m_segmentNumber + 1) == 0)
                {
                    int segL = 2*m_fftDataLength/(m_segmentNumber + 1);
                    if (checkPowerOfTwo(segL))
                    {
                        m_segmentLength = segL;
                        m_segLenSet = true;
                    }
                    else
                    {
                        PrintToScreen.WriteLine("segment length is not an integer power of two");
                        PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                        warning[1] = true;
                        m_segmentNumber = 1;
                        m_segmentLength = m_fftDataLength;
                        m_segLenSet = true;
                        m_overlap = false;
                    }
                }
                else
                {
                    PrintToScreen.WriteLine(
                        "total data length divided by the number of segments plus one is not an integer");
                    PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                    warning[4] = true;
                    m_segmentNumber = 1;
                    m_segmentLength = m_fftDataLength;
                    m_segLenSet = true;
                    m_overlap = false;
                }
            }

            if (m_segLenSet && m_overlap)
            {
                if ((2*m_fftDataLength)%m_segmentLength == 0)
                {
                    if (checkPowerOfTwo(m_segmentLength))
                    {
                        m_segmentNumber = (2*m_fftDataLength)/m_segmentLength - 1;
                        m_segNumSet = true;
                    }
                    else
                    {
                        PrintToScreen.WriteLine("segment length is not an integer power of two");
                        PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                        warning[1] = true;
                        m_segmentNumber = 1;
                        m_segmentLength = m_fftDataLength;
                        m_segNumSet = true;
                        m_overlap = false;
                    }
                }
                else
                {
                    PrintToScreen.WriteLine(
                        "twice the total data length divided by the segment length is not an integer");
                    PrintToScreen.WriteLine("segment length reset to total data length, i.e. no segmentation");
                    warning[5] = true;
                    m_segmentNumber = 1;
                    m_segmentLength = m_fftDataLength;
                    m_segNumSet = true;
                    m_overlap = false;
                }
            }

            if (!m_segNumSet && !m_segLenSet)
            {
                m_segmentNumber = 1;
                m_segNumSet = true;
                m_overlap = false;
            }

            if (m_overlap && m_segmentNumber < 2)
            {
                PrintToScreen.WriteLine("Overlap is not possible with less than two segments.");
                PrintToScreen.WriteLine("Overlap option has been reset to 'no overlap' i.e. to false.");
                m_overlap = false;
                m_segmentNumber = 1;
                m_segNumSet = true;
                warning[6] = true;
            }

            // check no segmentation option
            int segLno = 0;
            int segNno = 0;
            int segLov = 0;
            int segNov = 0;

            if (m_segmentNumber == 1)
            {
                // check if data number is a power of two
                if (!checkPowerOfTwo(m_fftDataLength))
                {
                    bool test0 = true;
                    bool test1 = true;
                    bool test2 = true;
                    int newL = 0;
                    int ii = 2;
                    // not a power of two - check segmentation options
                    // no overlap option
                    while (test0)
                    {
                        newL = m_fftDataLength/ii;
                        if (checkPowerOfTwo(newL) && (m_fftDataLength%ii) == 0)
                        {
                            test0 = false;
                            segLno = newL;
                            segNno = ii;
                        }
                        else
                        {
                            if (newL < 2)
                            {
                                test1 = false;
                                test0 = false;
                            }
                            else
                            {
                                ii++;
                            }
                        }
                    }
                    test0 = true;
                    ii = 2;
                    // overlap option
                    while (test0)
                    {
                        newL = 2*(m_fftDataLength/(ii + 1));
                        if (checkPowerOfTwo(newL) && (m_fftDataLength%(ii + 1)) == 0)
                        {
                            test0 = false;
                            segLov = newL;
                            segNov = ii;
                        }
                        else
                        {
                            if (newL < 2)
                            {
                                test2 = false;
                                test0 = false;
                            }
                            else
                            {
                                ii++;
                            }
                        }
                    }
                    // compare overlap and no overlap options
                    bool setSegment = true;
                    int segL = 0;
                    int segN = 0;
                    bool ovrlp = false;
                    if (test1)
                    {
                        if (test2)
                        {
                            if (segLov > segLno)
                            {
                                segL = segLov;
                                segN = segNov;
                                ovrlp = true;
                            }
                            else
                            {
                                segL = segLno;
                                segN = segNno;
                                ovrlp = false;
                            }
                        }
                        else
                        {
                            segL = segLno;
                            segN = segNno;
                            ovrlp = false;
                        }
                    }
                    else
                    {
                        if (test2)
                        {
                            segL = segLov;
                            segN = segNov;
                            ovrlp = true;
                        }
                        else
                        {
                            setSegment = false;
                        }
                    }

                    // compare segmentation and zero padding
                    if (setSegment && (m_originalDataLength - segL <= m_fftDataLength - m_originalDataLength))
                    {
                        PrintToScreen.WriteLine("Data length is not an integer power of two");
                        PrintToScreen.WriteLine("Data cannot be transformed as a single segment");
                        PrintToScreen.Write("The data has been split into " + segN + " segments of length " + segL);
                        if (ovrlp)
                        {
                            PrintToScreen.WriteLine(" with 50% overlap");
                        }
                        else
                        {
                            PrintToScreen.WriteLine(" with no overlap");
                        }
                        m_segmentLength = segL;
                        m_segmentNumber = segN;
                        m_overlap = ovrlp;
                        warning[7] = true;
                    }
                    else
                    {
                        PrintToScreen.WriteLine("Data length is not an integer power of two");
                        if (m_dataAltered)
                        {
                            PrintToScreen.WriteLine(
                                "Deleted point has been restored and the data has been padded with zeros to give a power of two length");
                            warning[0] = false;
                        }
                        else
                        {
                            PrintToScreen.WriteLine("Data has been padded with zeros to give a power of two length");
                        }
                        //m_fftDataLength = fftDataLength;
                        warning[8] = true;
                    }
                }
            }
        }

        private void printWarnings(FileOutput fout)
        {
            if (warning[0])
            {
                fout.println("WARNING!");
                fout.println("Number of data points must be an even number");
                fout.println("The last point was deleted");
                fout.println();
            }

            if (warning[1])
            {
                fout.println("WARNING!");
                fout.println("Segment length was not an integer power of two");
                fout.println("Segment length was reset to total data length, i.e. no segmentation");
                fout.println();
            }

            if (warning[2])
            {
                fout.println("WARNING!");
                fout.println("Total data length divided by the number of segments was not an integer");
                fout.println("Segment length was reset to total data length, i.e. no segmentation");
                fout.println();
            }

            if (warning[3])
            {
                fout.println("WARNING!");
                fout.println("Total data length divided by the segment length was not an integer");
                fout.println("Segment length was reset to total data length, i.e. no segmentation");
                fout.println();
            }

            if (warning[4])
            {
                fout.println("WARNING!");
                fout.println("Total data length divided by the number of segments plus one was not an integer");
                fout.println("Segment length was reset to total data length, i.e. no segmentation");
                fout.println();
            }

            if (warning[5])
            {
                fout.println("WARNING!");
                fout.println("Twice the total data length divided by the segment length was not an integer");
                fout.println("Segment length was reset to total data length, i.e. no segmentation");
                fout.println();
            }

            if (warning[6])
            {
                fout.println("WARNING!");
                fout.println("Overlap is not possible with less than two segments");
                fout.println("Overlap option has been reset to 'no overlap' i.e. to false");
                fout.println();
            }

            if (warning[7])
            {
                fout.println("WARNING!");
                fout.println("Data length was not an integer power of two");
                fout.println("The data could not be transformed as a single segment");
                fout.print("The data has been split into " + m_segmentNumber + " segment/s of length " + m_segmentLength);
                if (m_overlap)
                {
                    fout.println(" with 50% overlap");
                }
                else
                {
                    fout.println(" with no overlap");
                }
                fout.println();
            }

            if (warning[8])
            {
                fout.println("WARNING!");
                fout.println("Data length was not an integer power of two");
                fout.println("Data has been padded with " + (m_fftDataLength - m_originalDataLength) +
                             " zeros to give an integer power of two length");
                fout.println();
            }
        }

        // get the number of segments
        public int getSegmentNumber()
        {
            return m_segmentNumber;
        }

        // get the segment length
        public int getSegmentLength()
        {
            return m_segmentLength;
        }

        // set overlap option - see above (head of program comment lines) for option description
        public void setOverlapOption(bool overlapOpt)
        {
            bool old = m_overlap;
            m_overlap = overlapOpt;
            if (old != m_overlap)
            {
                if (m_fftDataSet)
                {
                    setSegmentNumber(m_segmentNumber);
                }
            }
        }

        // get overlap option - see above for options
        public bool getOverlapOption()
        {
            return m_overlap;
        }

        // calculate the number of data points given the:
        // segment length (segLen), number of segments (segNum)
        // and the overlap option (overlap: true - overlap, false - no overlap)
        public static int calcDataLength(bool overlap, int segLen, int segNum)
        {
            if (overlap)
            {
                return (segNum + 1)*segLen/2;
            }
            else
            {
                return segNum*segLen;
            }
        }

        // Method for performing a Fast Fourier Transform
        public void Transform()
        {
            // set up data array
            int isign = 1;
            if (!m_fftDataSet)
            {
                throw new ArgumentException("No data has been entered for the Fast Fourier Transform");
            }
            if (m_originalDataLength != m_fftDataLength)
            {
                PrintToScreen.WriteLine("Fast Fourier Transform data length ," + m_originalDataLength +
                                        ", is not an integer power of two");
                PrintToScreen.WriteLine(
                    "WARNING!!! Data has been padded with zeros to fill to nearest integer power of two length " +
                    m_fftDataLength);
            }

            // Perform fft
            double[] hold = new double[m_fftDataLength*2];
            for (int i = 0; i < m_fftDataLength*2; i++)
            {
                hold[i] = m_fftDataWindow[i];
            }
            basicFft(hold, m_fftDataLength, isign);
            for (int i = 0; i < m_fftDataLength*2; i++)
            {
                m_transformedDataFft[i] = hold[i];
            }

            // fill transformed data arrays
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_transformedDataComplex[i].reset(m_transformedDataFft[2*i], m_transformedDataFft[2*i + 1]);
            }
        }

        // Method for performing an inverse Fast Fourier Transform
        public void Inverse()
        {
            // set up data array
            int isign = -1;
            if (!m_fftDataSet)
            {
                throw new ArgumentException("No data has been entered for the inverse Fast Fourier Transform");
            }
            if (m_originalDataLength != m_fftDataLength)
            {
                PrintToScreen.WriteLine("Fast Fourier Transform data length ," + m_originalDataLength +
                                        ", is not an integer power of two");
                PrintToScreen.WriteLine(
                    "WARNING!!! Data has been padded with zeros to fill to nearest integer power of two length " +
                    m_fftDataLength);
            }

            // Perform inverse fft
            double[] hold = new double[m_fftDataLength*2];
            for (int i = 0; i < m_fftDataLength*2; i++)
            {
                hold[i] = m_fftDataWindow[i];
            }
            basicFft(hold, m_fftDataLength, isign);

            for (int i = 0; i < m_fftDataLength*2; i++)
            {
                m_transformedDataFft[i] = hold[i]/m_fftDataLength;
            }

            // fill transformed data arrays
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_transformedDataComplex[i].reset(m_transformedDataFft[2*i], m_transformedDataFft[2*i + 1]);
            }
        }

        // Base method for performing a Fast Fourier Transform
        // Based on the Numerical Recipes procedure four1
        // If isign is set to +1 this method replaces fftData[0 to 2*nn-1] by its discrete Fourier Transform
        // If isign is set to -1 this method replaces fftData[0 to 2*nn-1] by nn times its inverse discrete Fourier Transform
        // nn MUST be an integer power of 2.  This is not checked for in this method, fastFourierTransform(...), for speed.
        // If not checked for by the calling method, e.g. powerSpectrum(...) does, the method checkPowerOfTwo() may be used to check 
        // The real and imaginary parts of the data are stored adjacently
        // i.e. fftData[0] holds the real part, fftData[1] holds the corresponding imaginary part of a data point
        // data array and data array length over 2 (nn) transferred as arguments
        // result NOT returned to transformedDataFft
        // Based on the Numerical Recipes procedure four1
        public void basicFft(double[] data, int nn, int isign)
        {
            double dtemp = 0.0D, wtemp = 0.0D, tempr = 0.0D, tempi = 0.0D;
            double theta = 0.0D, wr = 0.0D, wpr = 0.0D, wpi = 0.0D, wi = 0.0D;
            int istep = 0, m = 0, mmax = 0;
            int n = nn << 1;
            int j = 1;
            int jj = 0;
            for (int i = 1; i < n; i += 2)
            {
                jj = j - 1;
                if (j > i)
                {
                    int ii = i - 1;
                    dtemp = data[jj];
                    data[jj] = data[ii];
                    data[ii] = dtemp;
                    dtemp = data[jj + 1];
                    data[jj + 1] = data[ii + 1];
                    data[ii + 1] = dtemp;
                }
                m = n >> 1;
                while (m >= 2 && j > m)
                {
                    j -= m;
                    m >>= 1;
                }
                j += m;
            }
            mmax = 2;
            while (n > mmax)
            {
                istep = mmax << 1;
                theta = isign*(6.28318530717959D/mmax);
                wtemp = Math.Sin(0.5D*theta);
                wpr = -2.0D*wtemp*wtemp;
                wpi = Math.Sin(theta);
                wr = 1.0D;
                wi = 0.0D;
                for (m = 1; m < mmax; m += 2)
                {
                    for (int i = m; i <= n; i += istep)
                    {
                        int ii = i - 1;
                        jj = ii + mmax;
                        tempr = wr*data[jj] - wi*data[jj + 1];
                        tempi = wr*data[jj + 1] + wi*data[jj];
                        data[jj] = data[ii] - tempr;
                        data[jj + 1] = data[ii + 1] - tempi;
                        data[ii] += tempr;
                        data[ii + 1] += tempi;
                    }
                    wr = (wtemp = wr)*wpr - wi*wpi + wr;
                    wi = wi*wpr + wtemp*wpi + wi;
                }
                mmax = istep;
            }
        }

        // Get the transformed data as ComplexClass
        public ComplexClass[] getTransformedDataAsComplex()
        {
            return m_transformedDataComplex;
        }

        // Get the transformed data array as adjacent real and imaginary pairs
        public double[] getTransformedDataAsAlternate()
        {
            return m_transformedDataFft;
        }

        // Performs and returns results a fft power spectrum density (psd) estimation
        // of unsegmented, segmented or segemented and overlapped data
        // data in array fftDataWindow
        public double[,] powerSpectrum()
        {
            checkSegmentDetails();

            m_psdNumberOfPoints = m_segmentLength/2;
            m_powerSpectrumEstimate = new double[2,m_psdNumberOfPoints];

            if (!m_overlap && m_segmentNumber < 2)
            {
                // Unsegmented and non-overlapped data

                // set up data array
                int isign = 1;
                if (!m_fftDataSet)
                {
                    throw new ArgumentException("No data has been entered for the Fast Fourier Transform");
                }
                if (!checkPowerOfTwo(m_fftDataLength))
                {
                    throw new ArgumentException("Fast Fourier Transform data length ," + m_fftDataLength +
                                                ", is not an integer power of two");
                }

                // perform fft
                double[] hold = new double[m_fftDataLength*2];
                for (int i = 0; i < m_fftDataLength*2; i++)
                {
                    hold[i] = m_fftDataWindow[i];
                }
                basicFft(hold, m_fftDataLength, isign);
                for (int i = 0; i < m_fftDataLength*2; i++)
                {
                    m_transformedDataFft[i] = hold[i];
                }

                // fill transformed data arrays
                for (int i = 0; i < m_fftDataLength; i++)
                {
                    m_transformedDataComplex[i].reset(m_transformedDataFft[2*i], m_transformedDataFft[2*i + 1]);
                }

                // obtain weighted m_mean square amplitudes
                m_powerSpectrumEstimate[1, 0] = Fmath.square(hold[0]) + Fmath.square(hold[1]);
                for (int i = 1; i < m_psdNumberOfPoints; i++)
                {
                    m_powerSpectrumEstimate[1, i] = Fmath.square(hold[2*i]) + Fmath.square(hold[2*i + 1]) +
                                                    Fmath.square(hold[2*m_segmentLength - 2*i]) +
                                                    Fmath.square(hold[2*m_segmentLength - 2*i + 1]);
                }

                // Normalise
                for (int i = 0; i < m_psdNumberOfPoints; i++)
                {
                    m_powerSpectrumEstimate[1, i] = 2.0D*m_powerSpectrumEstimate[1, i]/
                                                    (m_fftDataLength*m_sumOfSquaredWeights);
                }

                // Calculate frequencies
                for (int i = 0; i < m_psdNumberOfPoints; i++)
                {
                    m_powerSpectrumEstimate[0, i] = i/(m_segmentLength*m_deltaT);
                }
            }
            else
            {
                // Segmented or segmented and overlapped data
                m_powerSpectrumEstimate = powerSpectrumSeg();
            }

            m_powSpecDone = true;

            return m_powerSpectrumEstimate;
        }

        // Performs and returns results a fft power spectrum density (psd) estimation
        // of unsegmented, segmented or segemented and overlaped data
        // data read in from a text file
        public double[,] powerSpectrum(string fileName)
        {
            if (!checkPowerOfTwo(m_segmentLength))
            {
                throw new ArgumentException("Fast Fourier Transform segment length ," + m_segmentLength +
                                            ", is not an integer power of two");
            }

            //Object fin = new Object(fileName);

            m_psdNumberOfPoints = m_segmentLength/2;
            m_powerSpectrumEstimate = new double[2,m_psdNumberOfPoints];
            m_fftDataLength = calcDataLength(m_overlap, m_segmentLength, m_segmentNumber);

            if (!m_overlap && m_segmentNumber < 2)
            {
                // Unsegmented and non-overlapped data

                // read in data
                m_dblFftData = new double[2*m_fftDataLength];
                int j = -1;
                for (int i = 0; i < m_segmentLength; i++)
                {
                    m_dblFftData[++j] = System.Convert.ToDouble(Console.ReadLine());
                    m_dblFftData[++j] = System.Convert.ToDouble(Console.ReadLine());
                }

                m_complexData = ComplexClass.oneDarray(m_fftDataLength);
                j = -1;
                for (int i = 0; i < m_fftDataLength; i++)
                {
                    m_complexData[i].setReal(m_dblFftData[++j]);
                    m_complexData[i].setImag(m_dblFftData[++j]);
                }

                m_fftDataWindow = new double[2*m_fftDataLength];
                m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

                // perform fft
                int isign = 1;
                double[] hold = new double[m_fftDataLength*2];
                for (int i = 0; i < m_fftDataLength*2; i++)
                {
                    hold[i] = m_fftDataWindow[i];
                }
                basicFft(hold, m_fftDataLength, isign);
                for (int i = 0; i < m_fftDataLength*2; i++)
                {
                    m_transformedDataFft[i] = hold[i];
                }

                // fill transformed data arrays
                for (int i = 0; i < m_fftDataLength; i++)
                {
                    m_transformedDataComplex[i].reset(m_transformedDataFft[2*i], m_transformedDataFft[2*i + 1]);
                }

                // obtain weighted m_mean square amplitudes
                m_powerSpectrumEstimate[1, 0] = Fmath.square(hold[0]) + Fmath.square(hold[1]);
                for (int i = 1; i < m_psdNumberOfPoints; i++)
                {
                    m_powerSpectrumEstimate[1, i] = Fmath.square(hold[2*i]) + Fmath.square(hold[2*i + 1]) +
                                                    Fmath.square(hold[2*m_segmentLength - 2*i]) +
                                                    Fmath.square(hold[2*m_segmentLength - 2*i + 1]);
                }

                // Normalise
                for (int i = 0; i < m_psdNumberOfPoints; i++)
                {
                    m_powerSpectrumEstimate[1, i] = 2.0D*m_powerSpectrumEstimate[1, i]/
                                                    (m_fftDataLength*m_sumOfSquaredWeights);
                }

                // Calculate frequencies
                for (int i = 0; i < m_psdNumberOfPoints; i++)
                {
                    m_powerSpectrumEstimate[0, i] = i/(m_segmentLength*m_deltaT);
                }
            }
            else
            {
                // Segmented or segmented and overlapped data
                m_powerSpectrumEstimate = powerSpectrumSeg(null);
            }

            m_powSpecDone = true;

            return m_powerSpectrumEstimate;
        }


        // Performs and returns results a fft power spectrum density (psd) estimation of segmented or segemented and overlaped data
        // Data in fftDataWindow array
        // Private method for PowerSpectrum (see above)
        private double[,] powerSpectrumSeg()
        {
            // set up segment details
            int segmentStartIndex = 0;
            int segmentStartIncrement = m_segmentLength;
            if (m_overlap)
            {
                segmentStartIncrement /= 2;
            }
            double[] data = new double[2*m_segmentLength]; // holds data and transformed data for working segment
            m_psdNumberOfPoints = m_segmentLength/2; // number of PSD points
            double[] segPSD = new double[m_psdNumberOfPoints]; // holds psd for working segment
            double[,] avePSD = new double[2,m_psdNumberOfPoints]; // first row - frequencies
            // second row - accumaltes psd for averaging and then the averaged psd

            // initialis psd array and transform option
            for (int j = 0; j < m_psdNumberOfPoints; j++)
            {
                avePSD[1, j] = 0.0D;
            }
            int isign = 1;

            // loop through segments
            for (int i = 1; i <= m_segmentNumber; i++)
            {
                // collect segment data
                for (int j = 0; j < 2*m_segmentLength; j++)
                {
                    data[j] = m_dblFftData[segmentStartIndex + j];
                }

                // window data
                if (i == 1)
                {
                    m_sumOfSquaredWeights = windowData(data, data, weights);
                }
                else
                {
                    int k = 0;
                    for (int j = 0; j < m_segmentLength; j++)
                    {
                        data[k] = data[k]*weights[j];
                        data[++k] = data[k]*weights[j];
                        ++k;
                    }
                }

                // perform fft on windowed segment
                basicFft(data, m_segmentLength, isign);

                // obtain weighted m_mean square amplitudes
                segPSD[0] = Fmath.square(data[0]) + Fmath.square(data[1]);
                for (int j = 1; j < m_psdNumberOfPoints; j++)
                {
                    segPSD[j] = Fmath.square(data[2*j]) + Fmath.square(data[2*j + 1]) +
                                Fmath.square(data[2*m_segmentLength - 2*j]) +
                                Fmath.square(data[2*m_segmentLength - 2*j + 1]);
                }

                // Normalise
                for (int j = 0; j < m_psdNumberOfPoints; j++)
                {
                    segPSD[j] = 2.0D*segPSD[j]/(m_segmentLength*m_sumOfSquaredWeights);
                }

                // accumalate for averaging
                for (int j = 0; j < m_psdNumberOfPoints; j++)
                {
                    avePSD[1, j] += segPSD[j];
                }

                // increment segment start index
                segmentStartIndex += segmentStartIncrement;
            }

            // average all segments
            for (int j = 0; j < m_psdNumberOfPoints; j++)
            {
                avePSD[1, j] /= m_segmentNumber;
            }

            // Calculate frequencies
            for (int i = 0; i < m_psdNumberOfPoints; i++)
            {
                avePSD[0, i] = i/(m_segmentLength*m_deltaT);
            }

            return avePSD;
        }

        // Performs and returns results a fft power spectrum density (psd) estimation of segmented or segemented and overlaped data
        // Data read in from a text file
        // Private method for PowerSpectrum(fileName) (see above)
        private double[,] powerSpectrumSeg(Object fin)
        {
            // set up segment details
            double[] data = new double[2*m_segmentLength]; // holds data and transformed data for working segment
            weights = new double[m_segmentLength]; // windowing weights for segment
            double[] hold = new double[2*m_segmentLength]; // working array
            m_psdNumberOfPoints = m_segmentLength/2; // number of PSD points
            double[] segPSD = new double[m_psdNumberOfPoints]; // holds psd for working segment
            double[,] avePSD = new double[2,m_psdNumberOfPoints]; // first row - frequencies
            // second row - accumaltes psd for averaging and then the averaged psd

            // initialise psd array and fft option
            for (int j = 0; j < m_psdNumberOfPoints; j++)
            {
                avePSD[1, j] = 0.0D;
            }
            int isign = 1;

            // calculate window weights
            m_sumOfSquaredWeights = windowData(hold, hold, weights);

            if (m_overlap)
            {
                // overlapping segments

                // read in first half segment
                for (int j = 0; j < m_segmentLength; j++)
                {
                    data[j] = System.Convert.ToDouble(Console.ReadLine());
                }

                // loop through segments
                for (int i = 1; i <= m_segmentNumber; i++)
                {
                    // read in next half segment
                    for (int j = 0; j < m_segmentLength; j++)
                    {
                        data[j + m_segmentLength] = System.Convert.ToDouble(Console.ReadLine());
                    }

                    // window data
                    int k = -1;
                    for (int j = 0; j < m_segmentLength; j++)
                    {
                        data[++k] = data[k]*weights[j];
                        data[++k] = data[k]*weights[j];
                    }

                    // perform fft on windowed segment
                    basicFft(data, m_segmentLength, isign);

                    // obtain weighted m_mean square amplitudes
                    segPSD[0] = Fmath.square(data[0]) + Fmath.square(data[1]);
                    for (int j = 1; j < m_psdNumberOfPoints; j++)
                    {
                        segPSD[j] = Fmath.square(data[2*j]) + Fmath.square(data[2*j + 1]) +
                                    Fmath.square(data[2*m_segmentLength - 2*j]) +
                                    Fmath.square(data[2*m_segmentLength - 2*j + 1]);
                    }

                    // Normalise
                    for (int j = 0; j < m_psdNumberOfPoints; j++)
                    {
                        segPSD[j] = 2.0D*segPSD[j]/(m_segmentLength*m_sumOfSquaredWeights);
                    }

                    // accumalate for averaging
                    for (int j = 0; j < m_psdNumberOfPoints; j++)
                    {
                        avePSD[1, j] += segPSD[j];
                    }

                    // shift half segment
                    for (int j = 0; j < m_segmentLength; j++)
                    {
                        data[j] = data[j + m_segmentLength];
                    }
                }
            }
            else
            {
                // No overlap

                // loop through segments
                for (int i = 1; i <= m_segmentNumber; i++)
                {
                    // read in segment data
                    for (int j = 0; j < 2*m_segmentLength; j++)
                    {
                        data[j] = System.Convert.ToDouble(Console.ReadLine());
                    }

                    // window data
                    int k = -1;
                    for (int j = 0; j < m_segmentLength; j++)
                    {
                        data[++k] = data[k]*weights[j];
                        data[++k] = data[k]*weights[j];
                    }

                    // perform fft on windowed segment
                    basicFft(data, m_segmentLength, isign);

                    // obtain weighted m_mean square amplitudes
                    segPSD[0] = Fmath.square(data[0]) + Fmath.square(data[1]);
                    for (int j = 1; j < m_psdNumberOfPoints; j++)
                    {
                        segPSD[j] = Fmath.square(data[2*j]) + Fmath.square(data[2*j + 1]) +
                                    Fmath.square(data[2*m_segmentLength - 2*j]) +
                                    Fmath.square(data[2*m_segmentLength - 2*j + 1]);
                    }

                    // Normalise
                    for (int j = 1; j < m_psdNumberOfPoints; j++)
                    {
                        segPSD[j] = 2.0D*segPSD[j]/(m_segmentLength*m_sumOfSquaredWeights);
                    }

                    // accumalate for averaging
                    for (int j = 0; j < m_psdNumberOfPoints; j++)
                    {
                        avePSD[1, j] += segPSD[j];
                    }
                }
            }

            // average all segments
            for (int j = 0; j < m_psdNumberOfPoints; j++)
            {
                avePSD[1, j] /= m_segmentNumber;
            }

            // Calculate frequencies
            for (int i = 0; i < m_psdNumberOfPoints; i++)
            {
                avePSD[0, i] = i/(m_segmentLength*m_deltaT);
            }

            return avePSD;
        }

        // Get the power spectrum
        public double[,] getpowerSpectrumEstimate()
        {
            if (!m_powSpecDone)
            {
                PrintToScreen.WriteLine("getpowerSpectrumEstimate - powerSpectrum has not been called - null returned");
            }
            return m_powerSpectrumEstimate;
        }


        // get the number of power spectrum frequency points
        public int getNumberOfPsdPoints()
        {
            return m_psdNumberOfPoints;
        }

        // Print the power spectrum to a text file
        // default file name
        public void printPowerSpectrum()
        {
            string filename = "FourierTransformPSD.txt";
            printPowerSpectrum(filename);
        }

        // Print the power spectrum to a text file
        public void printPowerSpectrum(string filename)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            FileOutput fout = new FileOutput(filename);
            fout.println("Power Spectrum Density Estimate Output File from FourierTransform");
            fout.dateAndTimeln(filename);
            string title = "Window: " + windowNames[m_windowOption];
            if (m_windowOption == 6)
            {
                title += ", alpha = " + m_kaiserAlpha;
            }
            if (m_windowOption == 7)
            {
                title += ", alpha = " + m_gaussianAlpha;
            }
            fout.println(title);
            fout.printtab("Number of segments = ");
            fout.println(m_segmentNumber);
            fout.printtab("Segment length = ");
            fout.println(m_segmentLength);
            if (m_segmentNumber > 1)
            {
                if (m_overlap)
                {
                    fout.printtab("Segments overlap by 50%");
                }
                else
                {
                    fout.printtab("Segments do not overlap");
                }
            }

            fout.println();
            printWarnings(fout);

            fout.printtab("Frequency");
            fout.println("Mean Square");
            fout.printtab("(cycles per");
            fout.println("Amplitude");
            if (m_deltaTset)
            {
                fout.printtab("unit time)");
            }
            else
            {
                fout.printtab("gridpoint)");
            }
            fout.println(" ");

            int n = m_powerSpectrumEstimate.GetLength(1);
            for (int i = 0; i < n; i++)
            {
                fout.printtab(Fmath.truncate(m_powerSpectrumEstimate[0, i], 4));
                fout.println(Fmath.truncate(m_powerSpectrumEstimate[1, i], 4));
            }
            fout.close();
        }

        // Display a plot of the power spectrum from the given point number
        // no graph title provided
        public void plotPowerSpectrum(int lowPoint)
        {
            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerSpectrum(lowPoint, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Display a plot of the power spectrum from the given point number
        // graph title provided
        public void plotPowerSpectrum(int lowPoint, string graphTitle)
        {
            plotPowerSpectrum(lowPoint, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Display a plot of the power spectrum within a defined points window
        // no graph title provided
        public void plotPowerSpectrum(int lowPoint, int highPoint)
        {
            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerSpectrum(lowPoint, highPoint, graphTitle);
        }

        // Display a plot of the power spectrum within a defined points window
        // Graph title provided
        public void plotPowerSpectrum(int lowPoint, int highPoint, string graphTitle)
        {
            if (!m_powSpecDone)
            {
                PrintToScreen.WriteLine("plotPowerSpectrum - powerSpectrum has not been called - no plot displayed");
            }
            else
            {
                int n = m_powerSpectrumEstimate.GetLength(1) - 1;
                if (lowPoint < 0 || lowPoint >= n)
                {
                    lowPoint = 0;
                }
                if (highPoint < 0 || highPoint > n)
                {
                    highPoint = n;
                }
                plotPowerSpectrumLinear(lowPoint, highPoint, graphTitle);
            }
        }

        // Display a plot of the power spectrum from a given frequency
        // no graph title provided
        public void plotPowerSpectrum(double lowFreq)
        {
            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerSpectrum(lowFreq, graphTitle);
        }


        // Display a plot of the power spectrum from a given frequency
        // graph title provided
        public void plotPowerSpectrum(double lowFreq, string graphTitle)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            double highFreq = m_powerSpectrumEstimate[1, m_powerSpectrumEstimate.GetLength(1) - 1];
            plotPowerSpectrum(lowFreq, highFreq, graphTitle);
        }


        // Display a plot of the power spectrum within a defined frequency window
        // no graph title provided
        public void plotPowerSpectrum(double lowFreq, double highFreq)
        {
            if (!m_powSpecDone)
            {
                PrintToScreen.WriteLine("plotPowerSpectrum - powerSpectrum has not been called - no plot displayed");
            }
            else
            {
                string graphTitle = "Estimation of Power Spectrum Density";
                plotPowerSpectrum(lowFreq, highFreq, graphTitle);
            }
        }

        // Display a plot of the power spectrum within a defined frequency window
        // graph title provided
        public void plotPowerSpectrum(double lowFreq, double highFreq, string graphTitle)
        {
            if (!m_powSpecDone)
            {
                PrintToScreen.WriteLine("plotPowerSpectrum - powerSpectrum has not been called - no plot displayed");
            }
            else
            {
                int low = 0;
                int high = 0;
                if (!m_deltaTset)
                {
                    PrintToScreen.WriteLine("plotPowerSpectrum - deltaT has not been set");
                    PrintToScreen.WriteLine("full spectrum plotted");
                }
                else
                {
                    int ii = 0;
                    int n = m_powerSpectrumEstimate.GetLength(1) - 1;
                    bool test = true;
                    if (lowFreq == -1.0D)
                    {
                        low = 1;
                    }
                    else
                    {
                        while (test)
                        {
                            if (m_powerSpectrumEstimate[0, ii] > lowFreq)
                            {
                                low = ii - 1;
                                if (low < 0)
                                {
                                    low = 0;
                                }
                                test = false;
                            }
                            else
                            {
                                ii++;
                                if (ii >= n)
                                {
                                    low = 0;
                                    PrintToScreen.WriteLine("plotPowerSpectrum - lowFreq out of range -  reset to zero");
                                    test = false;
                                }
                            }
                        }
                    }
                    test = true;
                    ii = 0;
                    while (test)
                    {
                        if (m_powerSpectrumEstimate[0, ii] > highFreq)
                        {
                            high = ii - 1;
                            if (high < 0)
                            {
                                PrintToScreen.WriteLine(
                                    "plotPowerSpectrum - highFreq out of range -  reset to highest value");
                                high = n;
                            }
                            test = false;
                        }
                        else
                        {
                            ii++;
                            if (ii >= n)
                            {
                                high = n;
                                PrintToScreen.WriteLine(
                                    "plotPowerSpectrum - highFreq out of range -  reset to highest value");
                                test = false;
                            }
                        }
                    }
                    plotPowerSpectrumLinear(low, high, graphTitle);
                }
            }
        }


        // Display a plot of the power spectrum
        // no graph title provided
        public void plotPowerSpectrum()
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerSpectrumLinear(0, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Display a plot of the power spectrum
        public void plotPowerSpectrum(string graphTitle)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            plotPowerSpectrumLinear(0, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Prepare a plot of the power spectrum (linear)
        private void plotPowerSpectrumLinear(int low, int high, string graphTitle)
        {
            int nData = m_powerSpectrumEstimate.GetLength(1);
            int nNew = high - low + 1;
            double[,] spectrum = new double[2,nNew];
            for (int i = 0; i < nNew; i++)
            {
                spectrum[0, i] = m_powerSpectrumEstimate[0, i + low];
                spectrum[1, i] = m_powerSpectrumEstimate[1, i + low];
            }
            string yLegend = "Mean Square Amplitude";

            plotPowerDisplay(spectrum, low, high, graphTitle, yLegend);
        }

        // Display a log plot of the power spectrum from the given point number
        // no graph title provided
        public void plotPowerLog(int lowPoint)
        {
            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerLog(lowPoint, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Display a log plot of the power spectrum from the given point number
        // graph title provided
        public void plotPowerLog(int lowPoint, string graphTitle)
        {
            plotPowerLog(lowPoint, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Display a log plot of the power spectrum within a defined points window
        // no graph title provided
        public void plotPowerLog(int lowPoint, int highPoint)
        {
            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerLog(lowPoint, highPoint, graphTitle);
        }

        // Display a plot of the power spectrum within a defined points window
        // Graph title provided
        public void plotPowerLog(int lowPoint, int highPoint, string graphTitle)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            int n = m_powerSpectrumEstimate.GetLength(1) - 1;
            if (lowPoint < 0 || lowPoint >= n)
            {
                lowPoint = 0;
            }
            if (highPoint < 0 || highPoint > n)
            {
                highPoint = n;
            }
            plotPowerSpectrumLog(lowPoint, highPoint, graphTitle);
        }

        // Display a plot of the power spectrum from a given frequency
        // no graph title provided
        public void plotPowerLog(double lowFreq)
        {
            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerLog(lowFreq, graphTitle);
        }


        // Display a log plot of the power spectrum from a given frequency
        // graph title provided
        public void plotPowerLog(double lowFreq, string graphTitle)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            double highFreq = m_powerSpectrumEstimate[1, m_powerSpectrumEstimate.GetLength(1) - 1];
            plotPowerLog(lowFreq, highFreq, graphTitle);
        }

        // Display a plot of the power spectrum within a defined frequency window
        // no graph title provided
        public void plotPowerLog(double lowFreq, double highFreq)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerLog(lowFreq, highFreq, graphTitle);
        }

        // Display a log plot of the power spectrum within a defined frequency window
        // graph title provided
        public void plotPowerLog(double lowFreq, double highFreq, string graphTitle)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            int low = 0;
            int high = 0;
            if (!m_deltaTset)
            {
                PrintToScreen.WriteLine("plotPowerLog - deltaT has not been set");
                PrintToScreen.WriteLine("full spectrum plotted");
            }
            else
            {
                int ii = 0;
                int n = m_powerSpectrumEstimate.GetLength(1) - 1;
                bool test = true;
                if (lowFreq == -1.0D)
                {
                    low = 1;
                }
                else
                {
                    while (test)
                    {
                        if (m_powerSpectrumEstimate[0, ii] > lowFreq)
                        {
                            low = ii - 1;
                            if (low < 0)
                            {
                                low = 0;
                            }
                            test = false;
                        }
                        else
                        {
                            ii++;
                            if (ii >= n)
                            {
                                low = 0;
                                PrintToScreen.WriteLine("plotPowerLog - lowFreq out of range -  reset to zero");
                                test = false;
                            }
                        }
                    }
                }
                test = true;
                ii = 0;
                while (test)
                {
                    if (m_powerSpectrumEstimate[0, ii] > highFreq)
                    {
                        high = ii - 1;
                        if (high < 0)
                        {
                            PrintToScreen.WriteLine("plotPowerLog - highFreq out of range -  reset to highest value");
                            high = n;
                        }
                        test = false;
                    }
                    else
                    {
                        ii++;
                        if (ii >= n)
                        {
                            high = n;
                            PrintToScreen.WriteLine(
                                "plotPowerSpectrum - highFreq out of range -  reset to highest value");
                            test = false;
                        }
                    }
                }
                plotPowerSpectrumLog(low, high, graphTitle);
            }
        }

        // Display a log plot of the power spectrum
        // no graph title provided
        public void plotPowerLog()
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            string graphTitle = "Estimation of Power Spectrum Density";
            plotPowerSpectrumLog(0, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Display a log plot of the power spectrum
        public void plotPowerLog(string graphTitle)
        {
            if (!m_powSpecDone)
            {
                powerSpectrum();
            }

            plotPowerSpectrumLog(0, m_powerSpectrumEstimate.GetLength(1) - 1, graphTitle);
        }

        // Prepare a plot of the power spectrum (log)
        private void plotPowerSpectrumLog(int low, int high, string graphTitle)
        {
            int nData = m_powerSpectrumEstimate.GetLength(1);
            int nNew = high - low + 1;
            double[,] spectrum = new double[2,nNew];
            for (int i = 0; i < nNew; i++)
            {
                spectrum[0, i] = m_powerSpectrumEstimate[0, i + low];
                spectrum[1, i] = m_powerSpectrumEstimate[1, i + low];
            }

            // Find minimum of amplitudes that is not zero
            // find first non-zero value
            bool test = true;
            int ii = 0;
            double minimum = 0.0D;
            while (test)
            {
                if (spectrum[1, ii] > 0.0D)
                {
                    minimum = spectrum[1, ii];
                    test = false;
                }
                else
                {
                    ii++;
                    if (ii >= nNew)
                    {
                        test = false;
                        PrintToScreen.WriteLine("plotPowerSpectrumLog:  no non-zero amplitudes");
                    }
                }
            }

            // Find minimum
            for (int i = ii + 1; i < nNew; i++)
            {
                if (spectrum[1, i] < minimum)
                {
                    minimum = spectrum[1, i];
                }
            }

            // Replace zeros with minimum
            for (int i = 0; i < nNew; i++)
            {
                if (spectrum[1, i] <= 0.0D)
                {
                    spectrum[1, i] = minimum;
                }
            }

            // Take log to base 10
            for (int i = 0; i < nNew; i++)
            {
                spectrum[1, i] =
                    Fmath.Log10(spectrum[1, i]);
            }

            // call display method
            string yLegend = "Log10(Mean Square Amplitude)";
            plotPowerDisplay(spectrum, low, high, graphTitle, yLegend);
        }


        // Display a plot of the power spectrum
        private void plotPowerDisplay(double[,] spectrum, int low, int high, string graphTitle, string yLegend)
        {
            //PlotGraph pg = new PlotGraph(spectrum);
            //graphTitle = graphTitle + "  [plot between points " + low + " and " + high + "]";
            //pg.setGraphTitle(graphTitle);
            //string graphTitle2 = "Window: " + windowNames[windowOption];
            //if (windowOption == 6) graphTitle2 += " - alpha = " + kaiserAlpha;
            //if (windowOption == 7) graphTitle2 += " - alpha = " + gaussianAlpha;
            //graphTitle2 += ", " + segmentNumber + " segment/s of length " + segmentLength;
            //if (segmentNumber > 1)
            //{
            //    if (overlap)
            //    {
            //        graphTitle2 += ", segments overlap by 50%";
            //    }
            //    else
            //    {
            //        graphTitle2 += ", segments do not overlap";
            //    }
            //}

            //pg.setGraphTitle2(graphTitle2);
            //pg.setXaxisLegend("Frequency");
            //if (deltaTset)
            //{
            //    pg.setXaxisUnitsName("cycles per unit time");
            //}
            //else
            //{
            //    pg.setXaxisUnitsName("cycles per grid point");
            //}
            //pg.setYaxisLegend(yLegend);

            //switch (plotLineOption)
            //{
            //    case 0: pg.setLine(3);
            //        break;
            //    case 1: pg.setLine(1);
            //        break;
            //    case 2: pg.setLine(2);
            //        break;
            //    default: pg.setLine(3);
            //}

            //switch (plotPointOption)
            //{
            //    case 0: pg.setPoint(0);
            //        break;
            //    case 1: pg.setPoint(4);
            //        break;
            //    default: pg.setPoint(0);
            //}

            //pg.plot();
        }

        // Set the line option in plotting the power spectrum or correlation
        // = 0 join points with straight lines
        // = 1 cubic spline interpolation
        // = 3 no line - only points
        public void setPlotLineOption(int lineOpt)
        {
            m_plotLineOption = lineOpt;
        }

        // Get the line option in ploting the power spectrum or correlation
        // = 0 join points with straight lines
        // = 1 cubic spline interpolation
        // = 3 no line - only points
        public int getPlotLineOption()
        {
            return m_plotLineOption;
        }

        // Set the point option in plotting the power spectrum or correlation
        // = 0 no point symbol
        // = 1 filled circles
        public void setPlotPointOption(int pointOpt)
        {
            m_plotPointOption = pointOpt;
        }

        // Get the point option in plotting the power spectrum or correlation
        // = 0 no point symbol
        // = 1 filled circles
        public int getPlotPointOption()
        {
            return m_plotPointOption;
        }


        // Return correlation of data already entered with data passed as this method's argument
        // data must be real
        public double[,] correlate(double[] data)
        {
            int nLen = data.Length;
            if (!m_fftDataSet)
            {
                throw new ArgumentException("No data has been previously entered");
            }
            if (nLen != m_originalDataLength)
            {
                throw new ArgumentException("The two data sets to be correlated are of different length");
            }
            if (!checkPowerOfTwo(nLen))
            {
                throw new ArgumentException(
                    "The length of the correlation data sets is not equal to an integer power of two");
            }

            m_complexCorr = ComplexClass.oneDarray(nLen);
            for (int i = 0; i < nLen; i++)
            {
                m_complexCorr[i].setReal(data[i]);
                m_complexCorr[i].setImag(0.0D);
            }

            m_dblFftCorr = new double[2*nLen];
            int j = -1;
            for (int i = 0; i < nLen; i++)
            {
                m_dblFftCorr[++j] = data[i];
                m_dblFftCorr[++j] = 0.0D;
            }

            return correlation(nLen);
        }

        // Return correlation of data1 and data2 passed as this method's arguments
        // data must be real
        public double[,] correlate(double[] data1, double[] data2)
        {
            int nLen = data1.Length;
            int nLen2 = data2.Length;
            if (nLen != nLen2)
            {
                throw new ArgumentException("The two data sets to be correlated are of different length");
            }
            if (!checkPowerOfTwo(nLen))
            {
                throw new ArgumentException(
                    "The length of the correlation data sets is not equal to an integer power of two");
            }

            m_fftDataLength = nLen;
            m_complexData = ComplexClass.oneDarray(m_fftDataLength);
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_complexData[i].setReal(data1[i]);
                m_complexData[i].setImag(0.0D);
            }

            m_dblFftData = new double[2*m_fftDataLength];
            int j = 0;
            for (int i = 0; i < m_fftDataLength; i++)
            {
                m_dblFftData[j] = data1[i];
                j++;
                m_dblFftData[j] = 0.0D;
                j++;
            }
            m_fftDataSet = true;

            m_fftDataWindow = new double[2*m_fftDataLength];
            weights = new double[m_fftDataLength];
            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);

            m_transformedDataFft = new double[2*m_fftDataLength];
            m_transformedDataComplex = ComplexClass.oneDarray(m_fftDataLength);

            m_complexCorr = ComplexClass.oneDarray(nLen);
            for (int i = 0; i < nLen; i++)
            {
                m_complexCorr[i].setReal(data2[i]);
                m_complexCorr[i].setImag(0.0D);
            }

            m_dblFftCorr = new double[2*nLen];
            j = -1;
            for (int i = 0; i < nLen; i++)
            {
                m_dblFftCorr[++j] = data2[i];
                m_dblFftCorr[++j] = 0.0D;
            }

            return correlation(nLen);
        }

        // Returns the correlation of the data in fftData and fftCorr
        private double[,] correlation(int nLen)
        {
            m_fftDataWindow = new double[2*nLen];
            m_dblFftCorrWindow = new double[2*nLen];
            weights = new double[nLen];

            m_sumOfSquaredWeights = windowData(m_dblFftData, m_fftDataWindow, weights);
            windowData(m_dblFftCorr, m_dblFftCorrWindow, weights);

            // Perform fft on first set of stored data
            int isign = 1;
            double[] hold1 = new double[2*nLen];
            for (int i = 0; i < nLen*2; i++)
            {
                hold1[i] = m_fftDataWindow[i];
            }
            basicFft(hold1, nLen, isign);

            // Perform fft on second set of stored data
            isign = 1;
            double[] hold2 = new double[2*nLen];
            for (int i = 0; i < nLen*2; i++)
            {
                hold2[i] = m_dblFftCorrWindow[i];
            }
            basicFft(hold2, nLen, isign);

            // multiply hold1 by complex congugate of hold2
            double[] hold3 = new double[2*nLen];
            int j = 0;
            for (int i = 0; i < nLen; i++)
            {
                hold3[j] = (hold1[j]*hold2[j] + hold1[j + 1]*hold2[j + 1])/nLen;
                hold3[j + 1] = (-hold1[j]*hold2[j + 1] + hold1[j + 1]*hold2[j])/nLen;
                j += 2;
            }

            // Inverse transform -> correlation
            isign = -1;
            basicFft(hold3, nLen, isign);

            // fill correlation array
            for (int i = 0; i < 2*nLen; i++)
            {
                m_transformedDataFft[i] = hold3[i];
            }
            m_correlationArray = new double[2,nLen];
            j = 0;
            int k = nLen;
            for (int i = nLen/2 + 1; i < nLen; i++)
            {
                m_correlationArray[1, j] = hold3[k]/nLen;
                j++;
                k += 2;
            }
            k = 0;
            for (int i = 0; i < nLen/2; i++)
            {
                m_correlationArray[1, j] = hold3[k]/nLen;
                j++;
                k += 2;
            }

            // calculate time lags
            m_correlationArray[0, 0] = -(nLen/2.0)*m_deltaT;
            for (int i = 1; i < nLen; i++)
            {
                m_correlationArray[0, i] = m_correlationArray[0, i - 1] + m_deltaT;
            }

            m_correlateDone = true;
            return m_correlationArray;
        }

        // Get the correlation
        public double[,] getCorrelation()
        {
            if (!m_correlateDone)
            {
                PrintToScreen.WriteLine("getCorrelation - correlation has not been called - no correlation returned");
            }
            return m_correlationArray;
        }

        // Print the correlation to a text file
        // default file name
        public void printCorrelation()
        {
            string filename = "Correlation.txt";
            printCorrelation(filename);
        }

        // Print the correlation to a text file
        public void printCorrelation(string filename)
        {
            if (!m_correlateDone)
            {
                PrintToScreen.WriteLine("printCorrelation - correlate has not been called - no file printed");
            }
            else
            {
                FileOutput fout = new FileOutput(filename);
                fout.println("Correlation Output File from FourierTransform");
                fout.dateAndTimeln(filename);
                string title = "Window: " + windowNames[m_windowOption];
                if (m_windowOption == 6)
                {
                    title += ", alpha = " + m_kaiserAlpha;
                }
                if (m_windowOption == 7)
                {
                    title += ", alpha = " + m_gaussianAlpha;
                }
                fout.println(title);
                fout.printtab("Data length = ");
                fout.println(m_fftDataLength);
                fout.println();

                fout.printtab("Time lag");
                fout.println("Correlation");
                if (m_deltaTset)
                {
                    fout.printtab("/unit time");
                }
                else
                {
                    fout.printtab("/grid interval)");
                }
                fout.println("Coefficient");

                int n = m_correlationArray.GetLength(1);
                for (int i = 0; i < n; i++)
                {
                    fout.printtab(Fmath.truncate(m_correlationArray[0, i], 4));
                    fout.println(Fmath.truncate(m_correlationArray[1, i], 4));
                }
                fout.close();
            }
        }


        // Display a plot of the correlation
        // no graph title provided
        public void plotCorrelation()
        {
            if (!m_correlateDone)
            {
                PrintToScreen.WriteLine("plotCorrelation - correlation has not been called - no plot displayed");
            }
            else
            {
                string graphTitle = "Correlation Plot";
                plotCorrelation(graphTitle);
            }
        }


        // Display a plot of the correlation
        public void plotCorrelation(string graphTitle)
        {
            //if (!correlateDone)
            //{
            //    PrintToScreen.WriteLine("plotCorrelation - correlate has not been called - no plot displayed");
            //}
            //else
            //{

            //    PlotGraph pg = new PlotGraph(correlationArray);
            //    pg.setGraphTitle(graphTitle);
            //    string graphTitle2 = "Window: " + windowNames[windowOption];
            //    if (windowOption == 6) graphTitle2 += " - alpha = " + kaiserAlpha;
            //    if (windowOption == 7) graphTitle2 += " - alpha = " + gaussianAlpha;

            //    pg.setGraphTitle2(graphTitle2);
            //    pg.setXaxisLegend("Correlation Lag");
            //    if (deltaTset)
            //    {
            //        pg.setXaxisUnitsName("unit time");
            //    }
            //    else
            //    {
            //        pg.setXaxisUnitsName("grid interval");
            //    }
            //    pg.setYaxisLegend("Correlation coefficient");

            //    switch (plotLineOption)
            //    {
            //        case 0: pg.setLine(3);
            //            break;
            //        case 1: pg.setLine(1);
            //            break;
            //        case 2: pg.setLine(2);
            //            break;
            //        default: pg.setLine(3);
            //    }

            //    switch (plotPointOption)
            //    {
            //        case 0: pg.setPoint(0);
            //            break;
            //        case 1: pg.setPoint(4);
            //            break;
            //        default: pg.setPoint(0);
            //    }

            //    pg.plot();
            //}
        }

        // Performs  a fft power spectrum density (psd) estimation
        // on a moving window throughout the original data set
        // returning the results as a frequency time matrix
        // windowLength is the length of the window in time units
        public double[,] shortTime(double windowTime)
        {
            int windowLength = (int) Math.Round(windowTime/m_deltaT);
            if (!checkPowerOfTwo(windowLength))
            {
                int low = lastPowerOfTwo(windowLength);
                int high = nextPowerOfTwo(windowLength);

                if ((windowLength - low) <= (high - windowLength))
                {
                    windowLength = low;
                    if (low == 0)
                    {
                        windowLength = high;
                    }
                }
                else
                {
                    windowLength = high;
                }
                PrintToScreen.WriteLine("Method - shortTime");
                PrintToScreen.WriteLine("Window length, provided as time, " + windowTime +
                                        ", did not convert to an integer power of two data points");
                PrintToScreen.WriteLine("A value of " + ((windowLength - 1)*m_deltaT) + " was substituted");
            }

            return shortTime(windowLength);
        }

        // Performs  a fft power spectrum density (psd) estimation
        // on a moving window throughout the original data set
        // returning the results as a frequency time matrix
        // windowLength is the number of points in the window
        public double[,] shortTime(int windowLength)
        {
            if (!checkPowerOfTwo(windowLength))
            {
                throw new ArgumentException("Moving window data length ," + windowLength +
                                            ", is not an integer power of two");
            }
            if (!m_fftDataSet)
            {
                throw new ArgumentException("No data has been entered for the Fast Fourier Transform");
            }
            if (windowLength > m_originalDataLength)
            {
                throw new ArgumentException("The window length, " + windowLength + ", is greater than the data length, " +
                                            m_originalDataLength + ".");
            }

            // if no window option has been set - default = Gaussian with alpha = 2.5
            if (m_windowOption == 0)
            {
                setGaussian();
            }
            // set up time-frequency matrix
            //  first row = blank cell followed by time vector
            //  first column = blank cell followed by frequency vector
            //  each cell is then the m_mean square amplitude at that frequency and time
            m_numShortTimes = m_originalDataLength - windowLength + 1;
            m_numShortFreq = windowLength/2;
            m_timeFrequency = new double[m_numShortFreq + 1,m_numShortTimes + 1];
            m_timeFrequency[0, 0] = 0.0D;
            m_timeFrequency[0, 1] = (windowLength - 1)*m_deltaT/2.0D;
            for (int i = 2; i <= m_numShortTimes; i++)
            {
                m_timeFrequency[0, i] = m_timeFrequency[0, i - 1] + m_deltaT;
            }
            for (int i = 0; i < m_numShortFreq; i++)
            {
                m_timeFrequency[i + 1, 0] = i/(windowLength*m_deltaT);
            }

            // set up window details
            m_segmentLength = windowLength;
            int windowStartIndex = 0;
            double[] data = new double[2*windowLength]; // holds data and transformed data for working window
            double[] winPSD = new double[m_numShortFreq]; // holds psd for working window
            int isign = 1;

            // loop through time shifts
            for (int i = 1; i <= m_numShortTimes; i++)
            {
                // collect window data
                for (int j = 0; j < 2*windowLength; j++)
                {
                    data[j] = m_dblFftData[windowStartIndex + j];
                }

                // window data
                if (i == 1)
                {
                    m_sumOfSquaredWeights = windowData(data, data, weights);
                }
                else
                {
                    int k = 0;
                    for (int j = 0; j < m_segmentLength; j++)
                    {
                        data[k] = data[k]*weights[j];
                        data[++k] = data[k]*weights[j];
                        ++k;
                    }
                }

                // perform fft on windowed segment
                basicFft(data, windowLength, isign);

                // obtain weighted m_mean square amplitudes
                winPSD[0] = Fmath.square(data[0]) + Fmath.square(data[1]);
                for (int j = 1; j < m_numShortFreq; j++)
                {
                    winPSD[j] = Fmath.square(data[2*j]) + Fmath.square(data[2*j + 1]) +
                                Fmath.square(data[2*windowLength - 2*j]) + Fmath.square(data[2*windowLength - 2*j + 1]);
                }

                // Normalise and place in time-frequency matrix
                for (int j = 0; j < m_numShortFreq; j++)
                {
                    m_timeFrequency[j + 1, i] = 2.0D*winPSD[j]/(windowLength*m_sumOfSquaredWeights);
                }

                // increment segment start index
                windowStartIndex += 2;
            }

            m_shortTimeDone = true;
            return m_timeFrequency;
        }

        // Return time frequency matrix
        public double[,] getTimeFrequencyMatrix()
        {
            if (!m_shortTimeDone)
            {
                throw new ArgumentException("No short time Fourier transform has been performed");
            }
            return m_timeFrequency;
        }

        // Return number of times in short time Fourier transform
        public int getShortTimeNumberOfTimes()
        {
            if (!m_shortTimeDone)
            {
                throw new ArgumentException("No short time Fourier transform has been performed");
            }
            return m_numShortTimes;
        }

        // Return number of frequencies in short time Fourier transform
        public int getShortTimeNumberOfFrequencies()
        {
            if (!m_shortTimeDone)
            {
                throw new ArgumentException("No short time Fourier transform has been performed");
            }
            return m_numShortFreq;
        }

        // Return number of points in short time Fourier transform window
        public int getShortTimeWindowLength()
        {
            if (!m_shortTimeDone)
            {
                throw new ArgumentException("No short time Fourier transform has been performed");
            }
            return m_segmentLength;
        }

        // Print the short time Fourier transform to a text file
        // default file name
        public void printShortTime()
        {
            string filename = "ShortTime.txt";
            printShortTime(filename);
        }

        // Print the short time Fourier transform to a text file
        public void printShortTime(string filename)
        {
            if (!m_shortTimeDone)
            {
                PrintToScreen.WriteLine("printShortTime- shortTime has not been called - no file printed");
            }
            else
            {
                FileOutput fout = new FileOutput(filename);
                fout.println("Short Time Fourier Transform Output File from FourierTransform");
                fout.dateAndTimeln(filename);
                string title = "Window: " + windowNames[m_windowOption];
                if (m_windowOption == 6)
                {
                    title += ", alpha = " + m_kaiserAlpha;
                }
                if (m_windowOption == 7)
                {
                    title += ", alpha = " + m_gaussianAlpha;
                }
                fout.println(title);
                fout.printtab("Data length = ");
                fout.println(m_originalDataLength);
                fout.printtab("Delta T = ");
                fout.println(m_deltaT);
                fout.printtab("Window length (points) = ");
                fout.println(m_segmentLength);
                fout.printtab("Window length (time units) = ");
                fout.println((m_segmentLength - 1)*m_deltaT);
                fout.printtab("Number of frequency points = ");
                fout.println(m_numShortFreq);
                fout.printtab("Number of time points = ");
                fout.println(m_numShortTimes);

                // Average points if output would be greater than a text file line length
                bool checkAve = false;
                int newTp = m_numShortTimes;
                int maxN = 100;
                int nAve = m_numShortTimes/maxN;
                int nLast = m_numShortTimes%maxN;
                if (m_numShortTimes > 127)
                {
                    checkAve = true;
                    if (nLast > 0)
                    {
                        nAve += 1;
                        newTp = maxN;
                        nLast = m_numShortTimes - nAve*(newTp - 1);
                    }
                    else
                    {
                        newTp = maxN;
                        nLast = nAve;
                    }
                    if (nLast != nAve)
                    {
                        fout.println("In the output below, each of the first " + (newTp - 2) +
                                     " magnitude points, along the time axis, is the average of " + nAve +
                                     " calculated points");
                        fout.println("The last point is the average of " + nLast + " calculated points");
                    }
                    else
                    {
                        fout.println("In the output below, each magnitude point is the average of " + nAve +
                                     " calculated points");
                    }
                    fout.println(
                        "The data, without averaging, may be accessed using the method getTimeFrequencyMatrix()");
                }
                fout.println();

                fout.println("first row = times");
                fout.println("first column = frequencies");
                fout.println("all other cells = m_mean square amplitudes at the corresponding time and frequency");
                if (checkAve)
                {
                    double sum = 0.0D;
                    int start = 1;
                    int workingAve = nAve;
                    for (int i = 0; i <= m_numShortFreq; i++)
                    {
                        fout.printtab(Fmath.truncate(m_timeFrequency[i, 0], 4));
                        start = 1;
                        for (int j = 1; j <= newTp; j++)
                        {
                            workingAve = nAve;
                            if (j == newTp)
                            {
                                workingAve = nLast;
                            }
                            sum = 0.0D;
                            for (int k = start; k <= (start + workingAve - 1); k++)
                            {
                                sum += m_timeFrequency[i, k];
                            }
                            sum /= workingAve;
                            fout.printtab(Fmath.truncate(sum, 4));
                            start += workingAve;
                        }
                        fout.println();
                    }
                }
                else
                {
                    for (int i = 0; i <= m_numShortFreq; i++)
                    {
                        for (int j = 0; j <= newTp; j++)
                        {
                            fout.printtab(Fmath.truncate(m_timeFrequency[i, j], 4));
                        }
                        fout.println();
                    }
                }
                fout.close();
            }
        }

        // The paint method to draw the graph for plotShortTime.
        public void paint(Object g)
        {
            // Call graphing method
            graph(g);
        }

        // Set up the window and show graph for short time Fourier transform
        // user provided graph title
        public void plotShortTime(string title)
        {
            m_shortTitle = title;
            plotShortTime();
        }

        // Set up the window and show graph for short time Fourier transform
        // No user provided graph title
        public void plotShortTime()
        {
            //// Create the window object
            //JFrame window = new JFrame("Michael T Flanagan's plotting program - FourierTransform.plotShortTime");

            //// Set the initial size of the graph window
            //setSize(800, 600);

            //// Set background colour
            //window.getContentPane().setBackground(Color.white);

            //// Choose close box
            //window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

            //// Add graph canvas
            //window.getContentPane().add("Center", this);

            //// Set the window up
            //window.pack();
            //window.setResizable(true);
            //window.toFront();

            //// Show the window
            //window.setVisible(true);
        }


        // graph method for plotShortTime short time Fourier Transform as a contour plot
        public void graph(Object g)
        {
            //// graph axes positions
            //int xLen = 512;
            //int yLen = 256;
            //int yTop = 100;
            //int xBot = 100;
            //int numBands = 18;
            //// colours for contour map
            //Color[] color = new Color[numBands + 1];
            //color[18] = Color.black;
            //color[17] = Color.darkGray;
            //color[16] = Color.gray;
            //color[15] = Color.lightGray;
            //color[14] = Color.red.darker();
            //color[13] = Color.red;
            //color[12] = Color.magenta.darker();
            //color[11] = Color.magenta;
            //color[10] = Color.pink;
            //color[9] = Color.pink.darker();
            //color[8] = Color.orange.darker();
            //color[7] = Color.orange;
            //color[6] = Color.yellow;
            //color[5] = Color.green;
            //color[4] = Color.green.darker();
            //color[3] = Color.cyan;
            //color[2] = Color.cyan.darker();
            //color[1] = Color.blue;
            //color[0] = Color.blue.darker();

            //// Check and set parameters in case need to average or expand to match fixed x-axis pixels
            //int pixelsPerXpoint = 0;
            //int xTp = 0;
            //int xAve = 0;
            //int xLast = 0;
            //bool xCheck = true;
            //if (numShortTimes <= xLen)
            //{
            //    pixelsPerXpoint = xLen / numShortTimes;
            //    xLen = pixelsPerXpoint * numShortTimes;
            //    xTp = numShortTimes;
            //}
            //else
            //{
            //    xCheck = false;
            //    pixelsPerXpoint = 1;
            //    xTp = numShortTimes;
            //    xAve = numShortTimes / xLen;
            //    xLast = numShortTimes % xLen;
            //    if (xLast > 0)
            //    {
            //        xAve += 1;
            //        xTp = numShortTimes / xAve + 1;
            //        xLast = numShortTimes - xAve * (xTp - 1);
            //    }
            //    else
            //    {
            //        xTp = numShortTimes / xAve;
            //        xLast = xAve;
            //    }
            //    xLen = xTp;
            //}

            //// Check and set parameters in case need to average or expand to match fixed y-axis pixels
            //int pixelsPerYpoint = 0;
            //int yTp = 0;
            //int yAve = 0;
            //int yLast = 0;
            //bool yCheck = true;

            //if (numShortFreq <= yLen)
            //{
            //    pixelsPerYpoint = yLen / numShortFreq;
            //    yLen = pixelsPerYpoint * numShortFreq;
            //    yTp = numShortFreq;
            //}
            //else
            //{
            //    yCheck = false;
            //    pixelsPerYpoint = 1;
            //    yTp = numShortFreq;
            //    yAve = numShortFreq / yLen;
            //    yLast = numShortFreq % yLen;
            //    if (yLast > 0)
            //    {
            //        yAve += 1;
            //        yTp = numShortFreq / yAve + 1;
            //        yLast = numShortFreq - yAve * (yTp - 1);
            //    }
            //    else
            //    {
            //        yTp = numShortFreq / yAve;
            //        yLast = yAve;
            //    }
            //    yLen = yTp;
            //}

            //// Complete axes positions
            //int yBot = yTop + yLen;
            //int xTop = xBot + xLen;

            //// declare contour map arrays
            //double[,] averages = new double[yTp, xTp];
            //int[,] pixels = new int[yTp, xTp];
            //double[] times = new double[xTp];
            //int[] timesPixels = new int[xTp];
            //double[] freqs = new double[yTp];
            //int[] freqPixels = new int[yTp];

            //double[,] hold = new double[numShortFreq, xTp];

            //// If necessary average or expand to match fixed y-axis pixels
            //if (xCheck)
            //{
            //    for (int i = 0; i <= numShortFreq; i++)
            //    {
            //        for (int j = 1; j <= numShortTimes; j++)
            //        {
            //            if (i == 0)
            //            {
            //                times[j - 1] = timeFrequency[0, j];
            //            }
            //            else
            //            {
            //                hold[i - 1, j - 1] = timeFrequency[i, j];
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    double sum = 0.0D;
            //    int start = 1;
            //    int workingAve = xAve;
            //    for (int i = 0; i <= numShortFreq; i++)
            //    {
            //        start = 1;
            //        for (int j = 1; j <= xTp; j++)
            //        {
            //            workingAve = xAve;
            //            if (j == xTp) workingAve = xLast;
            //            sum = 0.0D;
            //            for (int k = start; k <= (start + workingAve - 1); k++)
            //            {
            //                sum += timeFrequency[i, k];
            //            }
            //            if (i == 0)
            //            {
            //                times[j - 1] = sum / workingAve;
            //            }
            //            else
            //            {
            //                hold[i - 1, j - 1] = sum / workingAve;
            //            }
            //            start += workingAve;
            //        }
            //    }
            //}

            //// If necessary average or expand to match fixed x-axis pixels
            //if (yCheck)
            //{
            //    for (int i = 0; i < numShortFreq; i++)
            //    {
            //        freqs[i] = timeFrequency[i + 1, 0];
            //        for (int j = 0; j < xTp; j++)
            //        {
            //            averages[i, j] = hold[i, j];
            //        }
            //    }
            //}
            //else
            //{
            //    double sum = 0.0D;
            //    double sFreq = 0.0D;
            //    int start = 0;
            //    int workingAve = yAve;
            //    for (int i = 0; i < xTp; i++)
            //    {
            //        start = 0;
            //        for (int j = 0; j < yTp; j++)
            //        {
            //            workingAve = yAve;
            //            if (j == yTp - 1) workingAve = yLast;
            //            sum = 0.0D;
            //            sFreq = 0.0D;
            //            for (int k = start; k <= (start + workingAve - 1); k++)
            //            {
            //                sum += hold[k, i];
            //                sFreq += timeFrequency[k + 1, 0];
            //            }
            //            averages[j, i] = sum;
            //            freqs[j] = sFreq / workingAve;
            //            start += workingAve;
            //        }
            //    }
            //}

            //// Calculate contour bands
            //double Max = averages[0, 0];
            //double Min = Max;
            //for (int i = 0; i < yTp; i++)
            //{
            //    for (int j = 0; j < xTp; j++)
            //    {
            //        if (averages[i, j] > Max) Max = averages[i, j];
            //        if (averages[i, j] < Min) Min = averages[i, j];
            //    }
            //}

            //double bandZero = 0.0D;
            //if (Min > 0.1D * Max) bandZero = 0.99D * Min;
            //double bandWidth = (1.01D * Max - 0.99D * Min) / numBands;
            //double[] band = new double[numBands];
            //band[0] = bandZero + bandWidth;
            //for (int i = 1; i < numBands; i++)
            //{
            //    band[i] = band[i - 1] + bandWidth;
            //}
            //bool test = true;
            //for (int i = 0; i < yTp; i++)
            //{
            //    for (int j = 0; j < xTp; j++)
            //    {
            //        test = true;
            //        int k = 0;
            //        while (test)
            //        {
            //            if (averages[i, j] <= band[k])
            //            {
            //                pixels[i, j] = k;
            //                test = false;
            //            }
            //            else
            //            {
            //                k++;
            //            }
            //        }
            //    }
            //}

            //// Plot contour coloured bands
            //int yPixels = 0;
            //int xPixels = 0;
            //int yInner = 0;
            //int xInner = 0;
            //int xx = xBot;
            //int yy = yTop;
            //for (int i = 0; i < yTp; i++)
            //{
            //    for (int j = 0; j < xTp; j++)
            //    {
            //        yInner = 0;
            //        for (int k = 0; k < pixelsPerYpoint; k++)
            //        {
            //            xInner = 0;
            //            for (int l = 0; l < pixelsPerXpoint; l++)
            //            {
            //                g.setColor(color[pixels[i, j]]);
            //                xx = xBot + (xPixels + xInner);
            //                yy = yBot - (yPixels + yInner);
            //                g.drawLine(xx, yy, xx, yy);
            //                xInner++;
            //            }
            //            yInner++;
            //        }
            //        xPixels += xInner;
            //    }
            //    yPixels += yInner;
            //    xPixels = 0;
            //}

            //// draw axes
            //g.setColor(color[numBands]);
            //g.drawLine(xBot, yBot, xBot, yTop);
            //g.drawLine(xTop, yBot, xTop, yTop);
            //g.drawLine(xBot, yBot, xTop, yBot);
            //g.drawLine(xBot, yTop, xTop, yTop);

            //// calculate axis legends and units
            //int yInc = yLen / 4;
            //int yScale = numShortFreq / 4;
            //double yUnits = yInc * (freqs[1] - freqs[0]) / (pixelsPerYpoint * yScale);
            //string[] yArray = new string[5];
            //int yArr = 0;
            //yArray[0] = "0  ";
            //for (int i = 1; i < 5; i++)
            //{
            //    yArr += yScale;
            //    yArray[i] = yArr + "  ";
            //}
            //xx = xBot;
            //yy = yBot;
            //int yWord = 6 * (yArray[4].Length() + 1);
            //for (int i = 0; i < 5; i++)
            //{
            //    g.drawLine(xx - 5, yy, xx, yy);
            //    g.drawString(yArray[i], xx - yWord, yy + 4);
            //    yy -= yInc;
            //}

            //int xInc = xLen / 8;
            //int xScale = numShortTimes / 8;
            //double xUnits = xInc * (times[1] - times[0]) / (pixelsPerXpoint * xScale);
            //string[] xArray = new string[9];
            //int xArr = 0;
            //xArray[0] = "0 ";
            //for (int i = 1; i < 9; i++)
            //{
            //    xArr += xScale;
            //    xArray[i] = xArr + " ";
            //}
            //xx = xBot;
            //yy = yBot;
            //for (int i = 0; i < 9; i++)
            //{
            //    g.drawLine(xx, yy, xx, yy + 5);
            //    g.drawString(xArray[i], xx - 4, yy + 20);
            //    xx += xInc;
            //}

            //// write graph and axis legends and units
            //g.drawString("Short Time Fourier Transfer Time-Frequency Plot", xBot - 80, yTop - 80);
            //g.drawString(shortTitle, xBot - 80, yTop - 60);

            //string yAxis = "Frequency / (" + Fmath.truncate(yUnits, 3) + " cycles per time unit)";
            //g.drawString(yAxis, xBot - 60, yTop - 20);
            //string xAxis = "Time / (" + Fmath.truncate(xUnits, 3) + " time units)";
            //g.drawString(xAxis, xBot, yBot + 40);
            //string totalTime = "Total time = " + (Fmath.truncate((xLen * (times[1] - times[0])) / pixelsPerXpoint, 3)) + " time units";
            //g.drawString(totalTime, xBot, yBot + 80);

            //string totalFreq = "Frequecy range = 0 to " + (Fmath.truncate((yLen * (freqs[1] - freqs[0])) / pixelsPerYpoint, 3)) + " cycles per time unit";
            //g.drawString(totalFreq, xBot, yBot + 100);

            //g.drawString("Widow length = " + Fmath.truncate((segmentLength - 1) * deltaT, 3) + " time units", xBot, yBot + 120);
            //string filter = "Window filter = " + windowNames[windowOption];
            //if (windowOption == 6) filter += ", alpha = " + kaiserAlpha;
            //if (windowOption == 7) filter += ", alpha = " + gaussianAlpha;
            //g.drawString(filter, xBot, yBot + 140);

            //// draw contour key
            //yy = yBot + 100;
            //xx = xTop + 40;
            //double ss = Fmath.truncate(bandZero, 3);
            //for (int i = 0; i < numBands; i++)
            //{
            //    double ff = Fmath.truncate(band[i], 3);
            //    g.setColor(color[numBands]);
            //    g.drawString(ss + " - " + ff, xx + 25, yy);
            //    ss = ff;
            //    g.setColor(color[i]);
            //    for (int j = 0; j < 20; j++)
            //    {
            //        yy = yy - 1;
            //        g.drawLine(xx, yy, xx + 20, yy);
            //    }
            //}
            //g.setColor(Color.black);
            //g.drawString("Mean square", xx + 25, yy - 25);
            //g.drawString("amplitudes ", xx + 25, yy - 10);
        }

        // returns nearest power of two that is equal to or lower than argument length
        public static int lastPowerOfTwo(int len)
        {
            bool test0 = true;
            while (test0)
            {
                if (checkPowerOfTwo(len))
                {
                    test0 = false;
                }
                else
                {
                    len--;
                }
            }
            return len;
        }

        // returns nearest power of two that is equal to or higher than argument length
        public static int nextPowerOfTwo(int len)
        {
            bool test0 = true;
            while (test0)
            {
                if (checkPowerOfTwo(len))
                {
                    test0 = false;
                }
                else
                {
                    len++;
                }
            }
            return len;
        }

        // Checks whether the argument n is a power of 2
        public static bool checkPowerOfTwo(int n)
        {
            bool test = true;
            int m = n;
            while (test && m > 1)
            {
                if ((m%2) != 0)
                {
                    test = false;
                }
                else
                {
                    m /= 2;
                }
            }
            return test;
        }

        // Checks whether the argument n is an integer times a integer power of 2
        // returns integer multiplier if true
        // returns zero if false
        public static int checkIntegerTimesPowerOfTwo(int n)
        {
            bool testOuter1 = true;
            bool testInner1 = true;
            bool testInner2 = true;
            bool testReturn = true;

            int m = n;
            int j = 1;
            int mult = 0;

            while (testOuter1)
            {
                testInner1 = checkPowerOfTwo(m);
                if (testInner1)
                {
                    testReturn = true;
                    testOuter1 = false;
                }
                else
                {
                    testInner2 = true;
                    while (testInner2)
                    {
                        m /= ++j;
                        if (m < 1)
                        {
                            testInner2 = false;
                            testInner1 = false;
                            testOuter1 = false;
                            testReturn = false;
                        }
                        else
                        {
                            if ((m%2) == 0)
                            {
                                testInner2 = false;
                            }
                        }
                    }
                }
            }
            if (testReturn)
            {
                mult = j;
            }
            return mult;
        }

        // Return the serial version unique identifier
        public static long getSerialVersionUID()
        {
            return serialVersionUID;
        }

        public static FourierTransform Convolve(
            FourierTransform fourierTransform1,
            FourierTransform fourierTransform2)
        {
            //
            // transform both fouriers
            //
            fourierTransform1.Transform();
            fourierTransform2.Transform();

            double[] dblArr1 =
                fourierTransform1.getTransformedDataAsAlternate();
            double[] dblArr2 =
                fourierTransform2.getTransformedDataAsAlternate();
            double[] dblArr3 = new double[dblArr1.Length];

            for (int i = 0; i < dblArr1.Length; i++)
            {
                dblArr3[i] = dblArr1[i]*dblArr2[i];
            }

            ComplexClass[] complexArr1 =
                fourierTransform1.getTransformedDataAsComplex();
            ComplexClass[] complexArr2 =
                fourierTransform2.getTransformedDataAsComplex();
            ComplexClass[] complexArr3 =
                new ComplexClass[complexArr1.Length];

            for (int i = 0; i < dblArr1.Length; i++)
            {
                complexArr3[i] = complexArr1[i].times(complexArr2[i]);
            }

            FourierTransform fourierTransform3 =
                new FourierTransform(complexArr3);

            fourierTransform3.m_transformedDataComplex = complexArr3;
            fourierTransform3.m_transformedDataFft = dblArr3;
            fourierTransform3.Inverse();
            return fourierTransform3;
        }
    }
}
