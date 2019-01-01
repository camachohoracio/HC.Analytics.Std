#region

using System;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Analytics.Mathematics.Roots;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class GammaCfdDist : RealRootFunction
    {
        public double beta;
        public double cfd;
        public double gamma;
        public double mu;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = cfd - gammaCDF(mu, beta, gamma, x);

            return y;
        }

        #endregion

        // GAMMA DISTRIBUTION AND GAMMA FUNCTIONS

        // Gamma distribution - three parameter
        // Cumulative distribution function
        public static double gammaCDF(
            double mu,
            double beta,
            double gamma,
            double upperLimit)
        {
            if (upperLimit < mu)
            {
                throw new ArgumentException("The upper limit, " + upperLimit +
                                            "must be equal to or greater than the location parameter, " + mu);
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            double xx = (upperLimit - mu)/beta;
            return RegularizedIncompleteGammaFunct.regularisedGammaFunction(gamma, xx);
        }

        // Gamma distribution - standard
        // Cumulative distribution function
        public static double gammaCDF(double gamma, double upperLimit)
        {
            if (upperLimit < 0.0D)
            {
                throw new ArgumentException("The upper limit, " + upperLimit + "must be equal to or greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            return RegularizedIncompleteGammaFunct.regularisedGammaFunction(gamma, upperLimit);
        }

        // Gamma distribution - three parameter
        // probablity density function
        public static double gammaPDF(double mu, double beta, double gamma, double x)
        {
            if (x < mu)
            {
                throw new ArgumentException("The variable, x, " + x +
                                            "must be equal to or greater than the location parameter, " + mu);
            }
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            double xx = (x - mu)/beta;
            return Math.Pow(xx, gamma - 1)*Math.Exp(-xx)/(beta*
                                                          GammaFunct.Gamma(gamma));
        }

        // Gamma distribution - standard
        // probablity density function
        public static double gammaPDF(double gamma, double x)
        {
            if (x < 0.0D)
            {
                throw new ArgumentException("The variable, x, " + x + "must be equal to or greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            return Math.Pow(x, gamma - 1)*Math.Exp(-x)/
                   GammaFunct.Gamma(gamma);
        }

        // Gamma distribution - three parameter
        // mean
        public static double gammaMean(double mu, double beta, double gamma)
        {
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            return gamma*beta - mu;
        }

        // Gamma distribution - three parameter
        // mode
        public static double gammaMode(double mu, double beta, double gamma)
        {
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            double mode = double.NaN;
            if (gamma >= 1.0D)
            {
                mode = (gamma - 1.0D)*beta - mu;
            }
            return mode;
        }

        // Gamma distribution - three parameter
        // standard deviation
        public static double gammaStandardDeviation(double mu, double beta, double gamma)
        {
            return gammaStandDev(mu, beta, gamma);
        }


        // Gamma distribution - three parameter
        // standard deviation
        public static double gammaStandDev(double mu, double beta, double gamma)
        {
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            return Math.Sqrt(gamma)*beta;
        }


        // Returns an array of Gamma random deviates - clock seed
        public static double[] gammaRand(double mu, double beta, double gamma, int n)
        {
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            return gammaArray(mu, beta, gamma, n);
        }

        // Returns an array of Gamma random deviates - user supplied seed
        public static double[] gammaRand(double mu, double beta, double gamma, int n, long seed)
        {
            if (beta <= 0.0D)
            {
                throw new ArgumentException("The scale parameter, " + beta + "must be greater than zero");
            }
            if (gamma <= 0.0D)
            {
                throw new ArgumentException("The shape parameter, " + gamma + "must be greater than zero");
            }
            return gammaArray(mu, beta, gamma, n);
        }


        // method for generating an array of Gamma random deviates
        public static double[] gammaArray(double mu, double beta, double gamma, int n)
        {
            RngWrapper rng = new RngWrapper();
            double[] ran = new double[n];

            // Create instance of the class holding the gamma cfd function
            GammaCfdDist gart = new GammaCfdDist();

            // set function variables
            gart.mu = mu;
            gart.beta = beta;
            gart.gamma = gamma;

            // Set initial range for search
            double range = Math.Sqrt(gamma)*beta;

            // required tolerance
            double tolerance = 1e-10;

            // lower bound
            double lowerBound = mu;

            // upper bound
            double upperBound = mu + 5.0D*range;
            if (upperBound <= lowerBound)
            {
                upperBound += range;
            }

            for (int i = 0; i < n; i++)
            {
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

                // Loop to reject failed attempts at calculating the deviate
                bool test = true;
                int ii = 0;
                int imax = 10; // maximum number of successive attempts at calculating deviate allowed
                while (test)
                {
                    // set function cfd variable
                    gart.cfd = rng.NextDouble();

                    // call root searching method, bisectNewtonRaphson
                    double rangd = realR.bisect(gart, lowerBound, upperBound);

                    if (!Double.IsNaN(rangd))
                    {
                        test = false;
                        ran[i] = rangd;
                    }
                    else
                    {
                        if (ii > imax)
                        {
                            PrintToScreen.WriteLine("class: PsRandom,  method: gammaArray");
                            PrintToScreen.WriteLine(imax +
                                                    " successive attempts at calculating a random gamma deviate failed for values of mu = " +
                                                    mu + ", beta = " + beta + ", gamma = " + gamma);
                            PrintToScreen.WriteLine("NaN returned");
                            ran[i] = double.NaN;
                            test = false;
                        }
                        else
                        {
                            ii++;
                        }
                    }
                }
            }

            return ran;
        }
    }
}
