#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Probability.Random;
using HC.Analytics.Statistics;
using HC.Analytics.TimeSeries.TsStats;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class UnivNormalDistStd : AbstractUnivContDist
    {
        #region Constants

        private const int INT_RND_SEED = 26;

        #endregion

        #region Constructors

        public UnivNormalDistStd(RngWrapper rng) : base(rng)
        {
        }

        #endregion

        #region Members

        private static double m_blnMoutput; // constant needed by Box-Mueller algorithm
        private static bool m_blnMoutputAvailable; // constant needed by Box-Mueller algorithm

        #endregion

        #region Constants

        private const double m_dblSqrth = 7.07106781186547524401E-1;
        /*************************************************
         *    COEFFICIENTS FOR METHOD  normalInverse()   *
         *************************************************/
        /* approximation for 0 <= |y - 0.5| <= 3/8 */

        private static readonly double[] m_dblP0Arr = {
                                                          -5.99633501014107895267E1,
                                                          9.80010754185999661536E1,
                                                          -5.66762857469070293439E1,
                                                          1.39312609387279679503E1,
                                                          -1.23916583867381258016E0,
                                                      };


        /* Approximation for interval z = sqrt(-2 log y ) between 2 and 8
         * i.e., y between Exp(-2) = .135 and Exp(-32) = 1.27e-14.
         */

        private static readonly double[] m_dblP1Arr = {
                                                          4.05544892305962419923E0,
                                                          3.15251094599893866154E1,
                                                          5.71628192246421288162E1,
                                                          4.40805073893200834700E1,
                                                          1.46849561928858024014E1,
                                                          2.18663306850790267539E0,
                                                          -1.40256079171354495875E-1,
                                                          -3.50424626827848203418E-2,
                                                          -8.57456785154685413611E-4,
                                                      };

        /* Approximation for interval z = sqrt(-2 log y ) between 8 and 64
         * i.e., y between Exp(-32) = 1.27e-14 and Exp(-2048) = 3.67e-890.
         */

        private static readonly double[] m_dblP2Arr = {
                                                          3.23774891776946035970E0,
                                                          6.91522889068984211695E0,
                                                          3.93881025292474443415E0,
                                                          1.33303460815807542389E0,
                                                          2.01485389549179081538E-1,
                                                          1.23716634817820021358E-2,
                                                          3.01581553508235416007E-4,
                                                          2.65806974686737550832E-6,
                                                          6.23974539184983293730E-9,
                                                      };

        private static readonly double[] m_dblQ0Arr = {
                                                          /* 1.00000000000000000000E0,*/
                                                          1.95448858338141759834E0,
                                                          4.67627912898881538453E0,
                                                          8.63602421390890590575E1,
                                                          -2.25462687854119370527E2,
                                                          2.00260212380060660359E2,
                                                          -8.20372256168333339912E1,
                                                          1.59056225126211695515E1,
                                                          -1.18331621121330003142E0,
                                                      };

        private static readonly double[] m_dblQ1Arr = {
                                                          /*  1.00000000000000000000E0,*/
                                                          1.57799883256466749731E1,
                                                          4.53907635128879210584E1,
                                                          4.13172038254672030440E1,
                                                          1.50425385692907503408E1,
                                                          2.50464946208309415979E0,
                                                          -1.42182922854787788574E-1,
                                                          -3.80806407691578277194E-2,
                                                          -9.33259480895457427372E-4,
                                                      };

        private static readonly double[] m_dblQ2Arr = {
                                                          /*  1.00000000000000000000E0,*/
                                                          6.02427039364742014255E0,
                                                          3.67983563856160859403E0,
                                                          1.37702099489081330271E0,
                                                          2.16236993594496635890E-1,
                                                          1.34204006088543189037E-2,
                                                          3.28014464682127739104E-4,
                                                          2.89247864745380683936E-6,
                                                          6.79019408009981274425E-9,
                                                      };

        #endregion

        #region Public

        /**
             * Pdf of standard normal
             * @return pdf
             * @param x  x
             */

        public override double Pdf(double dblX)
        {
            if (Double.IsInfinity(dblX))
            {
                return 0;
            }
            return Math.Exp(-dblX*dblX/2.0)/Math.Sqrt(2.0*Math.PI);
        }

        public override double Cdf(
            double dblX)
        {
            return CdfStatic(dblX);
        }

        public static double CdfStatic(
            double dblX)
        {
            if (dblX > 0)
            {
                return 0.5 + 0.5*ErrorFunct.ErrorFunction(dblX/
                                                          Math.Sqrt(2.0));
            }
            else
            {
                return 0.5 - 0.5*ErrorFunct.ErrorFunction(-dblX/
                                                          Math.Sqrt(2.0));
            }
        }

        /**
           *  Compute Cdf of standard normal
           *
           *@param  x  upper limit
           *@return    probability
           */

        public double Cdf2(
            double x)
        {
            // to do : compare performance of this method

            //return VisualNumerics.math.Statistics.normalCdf(x);
            if (Double.IsNaN(x))
            {
                return 0;
            }
            if (Double.IsInfinity(x))
            {
                return x > 0 ? 1 : 0;
            }

            double zabs = Math.Abs(x);
            if (zabs > 37)
            {
                return x > 0 ? 1 : 0;
            }
            double expntl = Math.Exp(-(zabs*zabs)/2);
            double p = 0;
            if (zabs < 7.071067811865475)
            {
                p = expntl*
                    ((((((zabs*.03526249659989109 + .7003830644436881)*zabs
                         + 6.37396220353165)*zabs + 33.912866078383)*zabs +
                       112.0792914978709)*zabs
                      + 221.2135961699311)*zabs + 220.2068679123761)/
                    (((((((zabs*.08838834764831844
                           + 1.755667163182642)*zabs + 16.06417757920695)*zabs +
                         86.78073220294608)*zabs
                        + 296.5642487796737)*zabs + 637.3336333788311)*zabs +
                      793.8265125199484)*zabs
                     + 440.4137358247522);
            }
            else
            {
                p = expntl/(zabs + 1/(zabs + 2/(zabs + 3/(zabs + 4/(zabs + .65)))))/
                    2.506628274631001;
            }
            return x > 0 ? 1 - p : p;
        }

        public override double CdfInv(double dblP)
        {
            return CdfInvStatic(dblP);
        }

        /**
         * Returns the value, <tt>x</tt>, for which the area under the
         * Normal (Gaussian) probability density function (integrated from
         * minus infinity to <tt>x</tt>) is equal to the argument <tt>y</tt> (assumes mean is zero, variance is one); formerly named <tt>ndtri</tt>.
         * <p>
         * For small arguments <tt>0 < y < Exp(-2)</tt>, the program computes
         * <tt>z = sqrt( -2.0 * Log(y) )</tt>;  then the approximation is
         * <tt>x = z - Log(z)/z  - (1/z) P(1/z) / Q(1/z)</tt>.
         * There are two rational functions P/Q, one for <tt>0 < y < Exp(-32)</tt>
         * and the other for <tt>y</tt> up to <tt>Exp(-2)</tt>.
         * For larger arguments,
         * <tt>w = y - 0.5</tt>, and  <tt>x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2))</tt>.
         *
         */

        public static double CdfInvStatic(double dblP)
        {
            double x, y, z, y2, x0, x1;
            int code;

            double s2pi = Math.Sqrt(2.0*Math.PI);

            if (dblP <= 0.0)
            {
                throw new ArgumentException();
            }
            if (dblP >= 1.0)
            {
                throw new ArgumentException();
            }
            code = 1;
            y = dblP;
            if (y > (1.0 - 0.13533528323661269189))
            {
                /* 0.135... = Exp(-2) */
                y = 1.0 - y;
                code = 0;
            }

            if (y > 0.13533528323661269189)
            {
                y = y - 0.5;
                y2 = y*y;
                x = y + y*(y2*Polynomial.polevl(y2, m_dblP0Arr, 4)/Polynomial.p1evl(y2, m_dblQ0Arr, 8));
                x = x*s2pi;
                return (x);
            }

            x = Math.Sqrt(-2.0*Math.Log(y));
            x0 = x - Math.Log(x)/x;

            z = 1.0/x;
            if (x < 8.0) /* y > Exp(-32) = 1.2664165549e-14 */
            {
                x1 = z*Polynomial.polevl(z, m_dblP1Arr, 8)/Polynomial.p1evl(z, m_dblQ1Arr, 8);
            }
            else
            {
                x1 = z*Polynomial.polevl(z, m_dblP2Arr, 8)/Polynomial.p1evl(z, m_dblQ2Arr, 8);
            }
            x = x0 - x1;
            if (code != 0)
            {
                x = -x;
            }
            return (x);
        }

        public override double NextDouble()
        {
            return NextDouble_static();
        }

        /**
        gaussian() uses the Box-Muller algorithm to transform raw()'s into
        gaussian deviates.

        @return a random real with a gaussian distribution,  standard deviation

        */

        public static double NextDouble_static()
        {
            System.Random rng = RandomFactory.Create();
            if (m_blnMoutputAvailable)
            {
                m_blnMoutputAvailable = false; // _WH_, March 12,1999
                return (m_blnMoutput);
            }

            double x, y, r, z;
            do
            {
                x = 2*rng.NextDouble() - 1; //x=uniform(-1,1); // _WH_, March 12,1999
                y = 2*rng.NextDouble() - 1; //y=uniform(-1,1);
                r = x*x + y*y;
            }
            while (r >= 1);

            z = Math.Sqrt(-2*Math.Log(r)/r);
            m_blnMoutput = x*z;
            m_blnMoutputAvailable = true;
            return y*z;
        }

        /**
         *  Insert the method's description here. Creation date: (3/6/00 11:42:39 AM)
         *
         *@param  n  number of variables to generate
         *@return    double[]
         */

        public double[] NextRandomOrdered(int n)
        {
            double[] x = NextDoubleArr(n);
            Array.Sort(x);
            return x;
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            return "NormalStd";
        }

        #endregion

        public static SortedDictionary<T, double[]> NormalizeVectors<T>(
            SortedDictionary<T, double[]> xMapAllTrain0,
            double[] stdDevArr,
            double[] meanArr,
            bool[] isNormallyDistributedOut)
        {
            try
            {
                var resultMap = new SortedDictionary<T, double[]>();

                List<double[]> featList = xMapAllTrain0.Values.ToList();

                int intVars = featList[0].Length;

                if (intVars != stdDevArr.Length ||
                    intVars != meanArr.Length)
                {
                    throw new HCException("Invalid dimensions");
                }

                for (int i = 0; i < intVars; i++)
                {
                    var blnIsNorm = isNormallyDistributedOut[i];
                    if (!blnIsNorm)
                    {
                        stdDevArr[i] = 1;
                        meanArr[i] = 0;
                    }
                }

                foreach (KeyValuePair<T, double[]> kvp in xMapAllTrain0)
                {
                    double[] feat = kvp.Value;
                    double[] newFeat = NormalizeFeature(stdDevArr, meanArr, feat);
                    resultMap[kvp.Key] = newFeat;
                }
                return resultMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static SortedDictionary<T, double[]> NormalizeVectors<T>(
            SortedDictionary<T, double[]> xMapAllTrain0,
            out double[] stdDevArr,
            out double[] meanArr,
            out bool[] isNormallyDistributedOut,
            bool[] isNormallyDistributedIn = null)
        {
            stdDevArr = new double[] { };
            meanArr = new double[] { };
            isNormallyDistributedOut = isNormallyDistributedIn;
            try
            {
                var resultMap = new SortedDictionary<T, double[]>();

                List<double[]> featList = xMapAllTrain0.Values.ToList();
                meanArr = Mean.GetMeanVector(featList);
                stdDevArr = StdDev.GetStdDevVector(featList, meanArr);

                int intVars = featList[0].Length;

                if (isNormallyDistributedIn == null)
                {
                    isNormallyDistributedOut = new bool[intVars];
                    for (int i = 0; i < intVars; i++)
                    {
                        List<double> list = TimeSeriesHelper.SelectVariable(
                            featList,
                            i);

                        bool blnIsNorm = 
                            NormalDistrHypTest.IsNormallyDistributed(
                                list);
                        isNormallyDistributedOut[i] = blnIsNorm;
                    }
                }
                else
                {
                    Console.WriteLine("Normaly distr check vector provided!");
                }

                for (int i = 0; i < intVars; i++)
                {
                    var blnIsNorm = isNormallyDistributedOut[i];
                    if (!blnIsNorm)
                    {
                        stdDevArr[i] = 1;
                        meanArr[i] = 0;
                    }
                }


                foreach (KeyValuePair<T, double[]> kvp in xMapAllTrain0)
                {
                    double[] feat = kvp.Value;
                    double[] newFeat = NormalizeFeature(stdDevArr, meanArr, feat);
                    resultMap[kvp.Key] = newFeat;
                }
                return resultMap;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double[]>();
        }

        public static double[] NormalizeFeature(
            double[] stdDevArr, 
            double[] meanArr, 
            double[] feat)
        {
            int intFeatureSize = feat.Length;
            var newFeat = new double[intFeatureSize];
            for (int i = 0; i < intFeatureSize; i++)
            {
                newFeat[i] = stdDevArr[i] == 0 ? 0 : (feat[i] - meanArr[i])/stdDevArr[i];
            }
            return newFeat;
        }

        public static SortedDictionary<T, double> NormalizeMap<T>(
            SortedDictionary<T, double> yMapTrainOri, 
            out double dblMean, 
            out double dblStdDev)
        {
            dblMean = 0;
            dblStdDev = 0;
            try
            {
                if(yMapTrainOri.Count == 0)
                {
                    return new SortedDictionary<T, double>();
                }

                var resultMap = new SortedDictionary<T, double>();
                var validVals = (from n in yMapTrainOri.Values
                                 where !double.IsNaN(n)
                                 select n).ToList();
                dblMean = validVals.Average();
                dblStdDev = StdDev.GetSampleStdDev(validVals);
                foreach (KeyValuePair<T, double> keyValuePair in yMapTrainOri)
                {
                    if (double.IsNaN(keyValuePair.Value))
                    {
                        resultMap[keyValuePair.Key] = double.NaN;
                        continue;
                    }
                    resultMap[keyValuePair.Key] =
                        dblStdDev == 0 ? 0 : (keyValuePair.Value - dblMean)/dblStdDev;
                }
                return resultMap;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<T, double>();
        }

        public static List<double> NormalizeVectors(
            List<double> xMapAllTrain0,
            out double dblStdDev,
            out double dblMean)
        {
            var resultMap = new List<double>();
            dblMean = xMapAllTrain0.Average();
            dblStdDev = StdDev.GetSampleStdDev(xMapAllTrain0);
            foreach (var keyValuePair in xMapAllTrain0)
            {
                resultMap.Add(
                    dblStdDev == 0 ? 0 : (keyValuePair - dblMean) / dblStdDev);
            }
            return resultMap;
        }
    }
}
