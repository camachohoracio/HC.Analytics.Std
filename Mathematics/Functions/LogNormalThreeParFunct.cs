#region

using System;
using HC.Analytics.Mathematics.Roots;

#endregion

namespace HC.Analytics.Mathematics.Functions
{
    // Class to evaluate the three parameter log-normal distribution function
    public class LogNormalThreeParFunct : RealRootFunction
    {
        public double alpha;
        public double beta;
        public double cfd;
        public double gamma;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = cfd - logNormalThreeParCDF(alpha, beta, gamma, x);

            return y;
        }

        #endregion

        // THREE PARAMETER LOG-NORMAL DISTRIBUTION

        // Three parameter log-normal cumulative distribution function
        // probability that a variate will assume  a value less than the upperlimit
        public static double logNormalThreeParCDF(double alpha, double beta, double gamma, double upperLimit)
        {
            if (beta < 0)
            {
                throw new ArgumentException("The parameter beta, " + beta + ", must be greater than or equal to zero");
            }
            if (upperLimit <= alpha)
            {
                return 0.0D;
            }
            else
            {
                return 0.5D*(1.0D +
                             ErrorFunct.ErrorFunction(Math.Log((upperLimit - alpha)/gamma)/(beta*Math.Sqrt(2))));
            }
        }


        // Three parameter log-normal cumulative distribution function
        // probability that a variate will assume a value between the lower and  the upper limits
        public static double logNormalThreeParCDF(double alpha, double beta, double gamma, double lowerLimit,
                                                  double upperLimit)
        {
            if (beta < 0)
            {
                throw new ArgumentException("The parameter beta, " + beta + ", must be greater than or equal to zero");
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
                if (upperLimit > alpha)
                {
                    arg1 = 0.5D*
                           (1.0D + ErrorFunct.ErrorFunction(Math.Log((upperLimit - alpha)/gamma)/(beta*Math.Sqrt(2))));
                }
                if (lowerLimit > alpha)
                {
                    arg2 = 0.5D*
                           (1.0D + ErrorFunct.ErrorFunction(Math.Log((lowerLimit - alpha)/gamma)/(beta*Math.Sqrt(2))));
                }
                cdf = arg1 - arg2;
            }

            return cdf;
        }


        // Log-Normal Inverse Cumulative Distribution Function
        // Three parameter
        public static double logNormalInverseCDF(
            double alpha,
            double beta,
            double gamma,
            double prob)
        {
            if (prob < 0.0 || prob > 1.0)
            {
                throw new ArgumentException("Entered cdf value, " + prob + ", must lie between 0 and 1 inclusive");
            }

            double icdf = 0.0D;

            if (prob == 0.0)
            {
                icdf = alpha;
            }
            else
            {
                if (prob == 1.0)
                {
                    icdf = double.PositiveInfinity;
                }
                else
                {
                    // Create instance of the class holding the Log-Normal cfd function
                    LogNormalThreeParFunct lognorm = new LogNormalThreeParFunct();

                    // set function variables
                    lognorm.alpha = alpha;
                    lognorm.beta = beta;
                    lognorm.gamma = gamma;

                    // required tolerance
                    double tolerance = 1e-12;

                    // lower bound
                    double lowerBound = alpha;

                    // upper bound
                    double upperBound = logNormalThreeParMean(alpha, beta, gamma) +
                                        5.0*logNormalThreeParStandardDeviation(alpha, beta, gamma);

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

                    //  set function cfd  variable
                    lognorm.cfd = prob;

                    // call root searching method
                    icdf = realR.bisect(lognorm, lowerBound, upperBound);
                }
            }

            return icdf;
        }

        // Log-Normal Inverse Cumulative Distribution Function
        // Three parameter
        public static double logNormalThreeParInverseCDF(double alpha, double beta, double gamma, double prob)
        {
            return logNormalInverseCDF(alpha, beta, gamma, prob);
        }


        // Three parameter log-normal probability density function
        public static double logNormalThreeParPDF(double alpha, double beta, double gamma, double x)
        {
            if (beta < 0)
            {
                throw new ArgumentException("The parameter beta, " + beta + ", must be greater than or equal to zero");
            }
            if (x <= alpha)
            {
                return 0.0D;
            }
            else
            {
                return Math.Exp(-0.5D*Fmath.square(Math.Log((x - alpha)/gamma)/beta))/
                       ((x - gamma)*beta*Math.Sqrt(2.0D*Math.PI));
            }
        }


        // Three parameter log-normal mean
        public static double logNormalThreeParMean(double alpha, double beta, double gamma)
        {
            return gamma*Math.Exp(beta*beta/2.0D) + alpha;
        }

        // Three parameter log-normal standard deviation
        public static double logNormalThreeParStandardDeviation(double alpha, double beta, double gamma)
        {
            return logNormalThreeParStandDev(alpha, beta, gamma);
        }

        // Three parameter log-normal standard deviation
        public static double logNormalThreeParStandDev(double alpha, double beta, double gamma)
        {
            double beta2 = beta*beta;
            return Math.Sqrt((Math.Exp(beta2) - 1.0D)*Math.Exp(2.0D*Math.Log(gamma) + beta2));
        }

        // Three parameter log-normal mode
        public static double logNormalThreeParMode(double alpha, double beta, double gamma)
        {
            return gamma*Math.Exp(-beta*beta) + alpha;
        }

        // Three parameter log-normal median
        public static double logNormalThreeParMedian(double alpha, double gamma)
        {
            return gamma + alpha;
        }
    }
}
