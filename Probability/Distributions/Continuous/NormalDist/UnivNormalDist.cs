#region

using System;
using HC.Analytics.Probability.Random;
using HC.Analytics.Statistics;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class UnivNormalDist : UnivNormalDistStd
    {
        #region Constants

        private const int INT_RND_SEED = 25;

        #endregion

        #region Properties

        public double Var
        {
            get { return m_dblVar; }
        }

        #endregion

        #region Parameters

        public double Mean
        {
            get { return m_dblMean; }
            set
            {
                m_dblMean = value;
                SetState(
                    m_dblMean,
                    m_dblStdDev);
            }
        }

        public double StdDev
        {
            get { return m_dblStdDev; }
            set
            {
                m_dblStdDev = value;
                SetState(
                    m_dblMean,
                    m_dblStdDev);
            }
        }

        #endregion

        #region Memebers

        private double m_dblMean;
        private double m_dblSqrtInv;
        private double m_dblStdDev;
        private double m_dblVar;

        #endregion

        #region Constructors

        public UnivNormalDist(
            double dblMean,
            double dblStdDev,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblMean,
                dblStdDev);
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblMean,
            double dblStdDev)
        {
            m_dblMean = dblMean;
            m_dblStdDev = dblStdDev;
            m_dblVar = dblStdDev*dblStdDev;
            m_dblSqrtInv = 1.0/Math.Sqrt(2.0*Math.PI*Var);
        }

        #endregion

        #region Public

        // Returns two arrays, both of length n, of correlated Gaussian (normal) random deviates
        // of means, mean1 and mean2, and standard deviations, sd1 and sd2,
        // and a correlation coefficient, rho
        public double[,] correlatedGaussianArrays(double mean1, double mean2, double sd1, double sd2, double rho, int n)
        {
            if (Math.Abs(rho) > 1.0D)
            {
                throw new ArgumentException("The correlation coefficient, " + rho + ", must lie between -1 and 1");
            }
            double[,] ran = new double[2,n];
            double[] ran1 = base.NextDoubleArr(n);
            double[] ran2 = base.NextDoubleArr(n);

            //double ranh = 0.0D;
            double rhot = Math.Sqrt(1.0D - rho*rho);

            for (int i = 0; i < n; i++)
            {
                ran[0, i] = ran1[i]*sd1 + mean1;
                ran[1, i] = (rho*ran1[i] + rhot*ran2[i])*sd2 + mean2;
            }
            return ran;
        }

        public double[,] correlatedNormalArrays(double mean1, double mean2, double sd1, double sd2, double rho, int n)
        {
            return correlatedGaussianArrays(mean1, mean2, sd1, sd2, rho, n);
        }


        /**
             * Pdf of normal
             * @return pdf
             * @param x x
             * @param mu mean
             * @param sd standart deviation
             */

        public override double Pdf(
            double dblX)
        {
            double diff = dblX - Mean;
            return m_dblSqrtInv*Math.Exp(-(diff*diff)/(2.0*Var));
        }


        /**
         * Returns the area under the Normal (Gaussian) probability density
         * function, integrated from minus infinity to <tt>x</tt>.
         * <pre>
         *                            x
         *                             -
         *                   1        | |                 2
         *  normal(x)  = ---------    |    Exp( - (t-mean) / 2v ) dt
         *               Sqrt(2pi*v)| |
         *                           -
         *                          -inf.
         *
         * </pre>
         * where <tt>v = variance</tt>.
         * Computation is via the functions <tt>errorFunction</tt>.
         *
         * @param mean the mean of the normal distribution.
         * @param variance the variance of the normal distribution.
         * @param x the integration limit.
         */

        public override double Cdf(
            double dblX)
        {
            double dblZ = (dblX - Mean)/StdDev;
            return base.Cdf(dblZ);
        }

        /**
         *  Insert the method's description here. Creation date: (1/26/00 11:01:06 AM)
         *
         *@param  mu  double
         *@param  sd  double
         *@return     double
         *@author:    <Vadum Kutsyy, kutsyy@hotmail.com>
         */

        public override double NextDouble()
        {
            return (base.NextDouble() + Mean)*StdDev;
        }

        public static double[] NextDoubleArr(
            double dblMean,
            double dblStdDev,
            int n)
        {
            double[] x = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = (NextDouble_static() + dblMean)*dblStdDev;
            }
            return x;
        }

        public override double CdfInv(double dblProbability)
        {
            return (base.CdfInv(dblProbability) - Mean)/StdDev;
        }

        public static double CdfInvStatic(
            double dblMean,
            double dblStdDev,
            double dblProbability)
        {
            return (CdfInvStatic(dblProbability) - dblMean)/dblStdDev;
        }


        // Gaussian (normal) order statistic medians (n points)
        public static double[] gaussianOrderStatisticMedians(double mean, double sigma, int n)
        {
            double nn = n;
            double[] gosm = new double[n];

            double[] uosm = Statistics.Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                gosm[i] = CdfInvStatic(mean, sigma, uosm[i]);
            }
            gosm = Scale.scale(gosm, mean, sigma);
            return gosm;
        }

        public static double[] normalOrderStatisticMedians(double mean, double sigma, int n)
        {
            return gaussianOrderStatisticMedians(mean, sigma, n);
        }

        // Gaussian (normal) order statistic medians for a mean of zero and a standard deviation 0f unity (n points)
        public static double[] gaussianOrderStatisticMedians(int n)
        {
            return gaussianOrderStatisticMedians(0.0, 1.0, n);
        }

        public static double[] normalOrderStatisticMedians(int n)
        {
            return gaussianOrderStatisticMedians(0.0, 1.0, n);
        }


        // Returns an array of Gaussian (normal) random deviates - clock seed
        // mean  =  the mean, sd = standard deviation, Length of array
        public static double[] normalRand(double mean, double sd, int n)
        {
            double[] ran = new double[n];
            for (int i = 0; i < n; i++)
            {
                ran[i] = NextDouble_static();
            }

            ran = Standardize.standardize(ran);
            for (int i = 0; i < n; i++)
            {
                ran[i] = ran[i]*sd + mean;
            }
            return ran;
        }

        // Returns an array of Gaussian (normal) random deviates - clock seed
        // mean  =  the mean, sd = standard deviation, Length of array
        public static double[] gaussianRand(double mean, double sd, int n)
        {
            return normalRand(mean, sd, n);
        }

        // Returns an array of Gaussian (normal) random deviates - user provided seed
        // mean  =  the mean, sd = standard deviation, Length of array
        public static double[] normalRand(double mean, double sd, int n, long seed)
        {
            double[] ran = new double[n];
            for (int i = 0; i < n; i++)
            {
                ran[i] = NextDouble_static();
            }
            ran = Standardize.standardize(ran);
            for (int i = 0; i < n; i++)
            {
                ran[i] = ran[i]*sd + mean;
            }
            return ran;
        }

        // Returns an array of Gaussian (normal) random deviates - user provided seed
        // mean  =  the mean, sd = standard deviation, Length of array
        public static double[] gaussianRand(double mean, double sd, int n, long seed)
        {
            return normalRand(mean, sd, n, seed);
        }

        #endregion
    }
}
