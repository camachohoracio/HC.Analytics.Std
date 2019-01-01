#region

using System;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Mathematics.Roots;
using HC.Analytics.Probability.Random;
using HC.Analytics.Statistics;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class LogNormalDist_3Params
    {
        // LogNormal order statistic medians (n points)
        // Three parametrs
        public static double[] logNormalThreeParOrderStatisticMedians(double alpha, double beta, double gamma, int n)
        {
            return logNormalOrderStatisticMedians(alpha, beta, gamma, n);
        }


        // LogNormal order statistic medians (n points)
        // Three parametrs
        public static double[] logNormalOrderStatisticMedians(
            double alpha,
            double beta, double gamma, int n)
        {
            double nn = n;
            double[] lnosm = new double[n];
            double[] uosm = Mean.uniformOrderStatisticMedians(n);
            for (int i = 0; i < n; i++)
            {
                lnosm[i] = LogNormalThreeParFunct.logNormalThreeParInverseCDF(alpha, beta, gamma, uosm[i]);
            }
            lnosm = Scale.scale(
                lnosm,
                LogNormalThreeParFunct.logNormalThreeParMean(alpha, beta, gamma),
                LogNormalThreeParFunct.logNormalThreeParStandardDeviation(alpha, beta, gamma));
            return lnosm;
        }


        // returns a three parameter log-normal random deviate
        public double nextLogNormalThreePar(double alpha, double beta, double gamma)
        {
            double ran = double.NaN;
            RngWrapper rng = new RngWrapper();
            // Create instance of the class holding the Three Parameter LogNormal cfd function
            LogNormalThreeParFunct logNorm3 = new LogNormalThreeParFunct();

            // set function variables
            logNorm3.alpha = alpha;
            logNorm3.beta = beta;
            logNorm3.gamma = gamma;

            // required tolerance
            double tolerance = 1e-10;

            // lower bound
            double lowerBound = alpha;
            double beta2 = beta*beta;
            double upperBound = 5.0D*Math.Sqrt((Math.Exp(beta2) - 1.0D)*Math.Exp(2.0D*Math.Log(gamma) + beta2));

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
                //  set function cfd  variable
                logNorm3.cfd = rng.NextDouble();

                // call root searching method
                ran = realR.falsePosition(logNorm3, lowerBound, upperBound);

                if (!Double.IsNaN(ran))
                {
                    test = false;
                }
                else
                {
                    if (ii > imax)
                    {
                        PrintToScreen.WriteLine("class: PsRandom,  method: nextLogNormalThreePar");
                        PrintToScreen.WriteLine(imax +
                                                " successive attempts at calculating a random log-normal deviate failed for values of alpha = " +
                                                alpha + ", beta = " + beta + ", gamma = " + gamma);
                        PrintToScreen.WriteLine("NaN returned");
                        ran = double.NaN;
                        test = false;
                    }
                    else
                    {
                        ii++;
                    }
                }
            }
            return ran;
        }

        // Returns an array of three parameter log-normal random deviates - user supplied seed
        public static double[] logNormalThreeParRand(double alpha, double beta, double gamma, int n, long seed)
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
            return logNormalThreeParArray(alpha, beta, gamma, n);
        }


        // returns an array of three parameter log-normal random deviates
        public static double[] logNormalThreeParArray(double alpha, double beta, double gamma, int n)
        {
            RngWrapper rng = new RngWrapper();

            double[] ran = new double[n];

            // Create instance of the class holding the Three Parameter log normal cfd function
            LogNormalThreeParFunct logNorm3 = new LogNormalThreeParFunct();

            // set function variables
            logNorm3.alpha = alpha;
            logNorm3.beta = beta;
            logNorm3.gamma = gamma;

            // required tolerance
            double tolerance = 1e-10;

            // lower bound
            double lowerBound = alpha;

            // upper bound
            double beta2 = beta*beta;
            double upperBound = 5.0D*Math.Sqrt((Math.Exp(beta2) - 1.0D)*Math.Exp(2.0D*Math.Log(gamma) + beta2));

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
                    //  set function cfd  variable
                    logNorm3.cfd = rng.NextDouble();

                    // call root searching method, bisectNewtonRaphson
                    double rangd = realR.falsePosition(logNorm3, lowerBound, upperBound);

                    if (!Double.IsNaN(rangd))
                    {
                        test = false;
                        ran[i] = rangd;
                    }
                    else
                    {
                        if (ii > imax)
                        {
                            PrintToScreen.WriteLine("class: PsRandom,  method: logNormalThreeParArray");
                            PrintToScreen.WriteLine(imax +
                                                    " successive attempts at calculating a log-normal gamma deviate failed for values of alpha = " +
                                                    alpha + ", beta = " + beta + ", gamma = " + gamma);
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
