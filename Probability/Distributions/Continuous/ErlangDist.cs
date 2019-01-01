#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class ErlangDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly ErlangDist m_ownInstance = new ErlangDist(
            1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblMean;
        private double m_dblStdDev;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 7;

        #endregion

        #region Properties

        public double Variance
        {
            get { return Math.Pow(StdDev, 2); }
        }

        #endregion

        #region Constructors

        public ErlangDist(
            double dblMean,
            double dblStdDev,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblMean,
                dblStdDev);
        }

        #endregion

        #region Parameters

        public double Mean
        {
            get { return m_dblMean; }
            set
            {
                m_dblMean = value;
                SetState(m_dblMean,
                         m_dblStdDev);
            }
        }

        public double StdDev
        {
            get { return m_dblStdDev; }
            set
            {
                m_dblStdDev = value;
                SetState(m_dblMean,
                         m_dblStdDev);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblMean,
            double dblStdDev)
        {
            m_dblMean = dblMean;
            m_dblStdDev = dblStdDev;
        }

        #endregion

        #region Public

        /**
        * Returns an erlang distributed random number with the given variance and mean.
        */

        public override double NextDouble()
        {
            int k = (int) ((Mean*Mean)/Variance + 0.5);
            k = (k > 0) ? k : 1;
            double a = k/Mean;

            double prod = 1.0;
            for (int i = 0; i < k; i++)
            {
                prod *= m_rng.NextDouble();
            }
            return -Math.Log(prod)/a;
        }

        public override double Cdf(double dblX)
        {
            throw new NotImplementedException();
        }

        public override double CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        public override double Pdf(double dblX)
        {
            throw new NotImplementedException();
        }


        // ERLANG DISTRIBUTION AND ERLANG EQUATIONS

        // Erlang distribution
        // Cumulative distribution function
        public static double erlangCDF(double lambda, int kay, double upperLimit)
        {
            return GammaCfdDist.gammaCDF(0.0D, 1.0D/lambda, kay, upperLimit);
        }

        public static double erlangCDF(double lambda, long kay, double upperLimit)
        {
            return GammaCfdDist.gammaCDF(0.0D, 1.0D/lambda, kay, upperLimit);
        }

        public static double erlangCDF(double lambda, double kay, double upperLimit)
        {
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }
            return GammaCfdDist.gammaCDF(0.0D, 1.0D/lambda, kay, upperLimit);
        }

        // Erlang distribution
        // probablity density function
        public static double erlangPDF(double lambda, int kay, double x)
        {
            return GammaCfdDist.gammaPDF(0.0D, 1.0D/lambda, kay, x);
        }

        public static double erlangPDF(double lambda, long kay, double x)
        {
            return GammaCfdDist.gammaPDF(0.0D, 1.0D/lambda, kay, x);
        }

        public static double erlangPDF(double lambda, double kay, double x)
        {
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }

            return GammaCfdDist.gammaPDF(0.0D, 1.0D/lambda, kay, x);
        }

        // Erlang distribution
        // mean
        public static double erlangMean(double lambda, int kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return kay/lambda;
        }

        public static double erlangMean(double lambda, long kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return kay/lambda;
        }

        public static double erlangMean(double lambda, double kay)
        {
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return kay/lambda;
        }

        // erlang distribution
        // mode
        public static double erlangMode(double lambda, int kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            double mode = double.NaN;
            if (kay >= 1)
            {
                mode = (kay - 1.0D)/lambda;
            }
            return mode;
        }

        public static double erlangMode(double lambda, long kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            double mode = double.NaN;
            if (kay >= 1)
            {
                mode = (kay - 1.0D)/lambda;
            }
            return mode;
        }

        public static double erlangMode(double lambda, double kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }
            double mode = double.NaN;
            if (kay >= 1)
            {
                mode = (kay - 1.0D)/lambda;
            }
            return mode;
        }


        // Erlang distribution
        // standard deviation
        public static double erlangStandardDeviation(double lambda, int kay)
        {
            return erlangStandDev(lambda, kay);
        }

        // standard deviation
        public static double erlangStandardDeviation(double lambda, long kay)
        {
            return erlangStandDev(lambda, kay);
        }

        // standard deviation
        public static double erlangStandardDeviation(double lambda, double kay)
        {
            return erlangStandDev(lambda, kay);
        }

        // standard deviation
        public static double erlangStandDev(double lambda, int kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return Math.Sqrt(kay)/lambda;
        }

        public static double erlangStandDev(double lambda, long kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return Math.Sqrt(kay)/lambda;
        }

        public static double erlangStandDev(double lambda, double kay)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }
            return Math.Sqrt(kay)/lambda;
        }

        // Returns an array of Erlang random deviates - clock seed
        public static double[] erlangRand(double lambda, int kay, int n)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }

            return GammaCfdDist.gammaRand(0.0D, 1.0D/lambda, kay, n);
        }

        public static double[] erlangRand(double lambda, long kay, int n)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return GammaCfdDist.gammaRand(0.0D, 1.0D/lambda, kay, n);
        }

        public static double[] erlangRand(double lambda, double kay, int n)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }
            return GammaCfdDist.gammaRand(0.0D, 1.0D/lambda, kay, n);
        }

        // Returns an array of Erlang random deviates - user supplied seed
        public static double[] erlangRand(double lambda, int kay, int n, long seed)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return GammaCfdDist.gammaRand(0.0D, 1.0D/lambda, kay, n, seed);
        }

        public static double[] erlangRand(double lambda, long kay, int n, long seed)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            return GammaCfdDist.gammaRand(0.0D, 1.0D/lambda, kay, n, seed);
        }

        public static double[] erlangRand(double lambda, double kay, int n, long seed)
        {
            if (kay < 1)
            {
                throw new ArgumentException("The rate parameter, " + kay + "must be equal to or greater than one");
            }
            if (kay - Math.Round(kay) != 0.0D)
            {
                throw new ArgumentException(
                    "kay must, mathematically, be an integer even though it may be entered as a double\nTry the Gamma distribution instead of the Erlang distribution");
            }
            return GammaCfdDist.gammaRand(0.0D, 1.0D/lambda, kay, n, seed);
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblAlpha,
            double dblBeta,
            double dblX)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblAlpha,
            double dblBeta,
            double dblX)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblAlpha,
            double dblBeta,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblAlpha,
            double dblBeta)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);
            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblAlpha,
            double dblBeta,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblAlpha,
            double dblBeta,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblAlpha,
                dblBeta);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
