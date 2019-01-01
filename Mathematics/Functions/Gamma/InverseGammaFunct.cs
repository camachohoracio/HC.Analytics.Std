#region

using System;
using HC.Analytics.Mathematics.Roots;

#endregion

namespace HC.Analytics.Mathematics.Functions.Gamma
{
    public class InverseGammaFunct : RealRootFunction
    {
        public double gamma;

        #region RealRootFunction Members

        public double function(double x)
        {
            double y = gamma - GammaFunct.Gamma(x);

            return y;
        }

        #endregion

        // Inverse Gamma Function
        public static double[] inverseGammaFunction(double gamma)
        {
            double gammaMinimum = 0.8856031944108839;
            double iGammaMinimum = 1.4616321399961483;
            if (gamma < gammaMinimum)
            {
                throw new ArgumentException("Entered argument (gamma) value, " + gamma +
                                            ", must be equal to or greater than 0.8856031944108839 - this method does not handle the negative domain");
            }

            double[] igamma = new double[2];

            // required tolerance
            double tolerance = 1e-12;


            // x value between 0 and 1.4616321399961483
            if (gamma == 1.0)
            {
                igamma[0] = 1.0;
            }
            else
            {
                if (gamma == gammaMinimum)
                {
                    igamma[0] = iGammaMinimum;
                }
                else
                {
                    // Create instance of the class holding the gamma inverse function
                    InverseGammaFunct gif1 = new InverseGammaFunct();

                    // Set inverse gamma function variable
                    gif1.gamma = gamma;

                    //  lower bounds
                    double lowerBound1 = 0.0;

                    // upper bound
                    double upperBound1 = iGammaMinimum;

                    // Create instance of RealRoot
                    RealRoot realR1 = new RealRoot();

                    // Set extension limits
                    realR1.noBoundsExtensions();

                    // Set tolerance
                    realR1.setTolerance(tolerance);

                    // Supress error messages and arrange for NaN to be returned as root if root not found
                    realR1.resetNaNexceptionToTrue();
                    realR1.supressLimitReachedMessage();
                    realR1.supressNaNmessage();

                    // call root searching method
                    igamma[0] = realR1.bisect(gif1, lowerBound1, upperBound1);
                }
            }

            // x value above 1.4616321399961483
            if (gamma == 1.0)
            {
                igamma[1] = 2.0;
            }
            else
            {
                if (gamma == gammaMinimum)
                {
                    igamma[1] = iGammaMinimum;
                }
                else
                {
                    // Create instance of the class holding the gamma inverse function
                    InverseGammaFunct gif2 = new InverseGammaFunct();

                    // Set inverse gamma function variable
                    gif2.gamma = gamma;

                    //  bounds
                    double lowerBound2 = iGammaMinimum;
                    double upperBound2 = 2.0;
                    double ii = 2.0;
                    double gii = GammaFunct.Gamma(ii);
                    if (gamma > gii)
                    {
                        bool test = true;
                        while (test)
                        {
                            ii += 1.0;
                            gii = GammaFunct.Gamma(ii);
                            if (gamma <= gii)
                            {
                                upperBound2 = ii;
                                lowerBound2 = ii - 1.0;
                                test = false;
                            }
                        }
                    }

                    // Create instance of RealRoot
                    RealRoot realR2 = new RealRoot();

                    // Set extension limits
                    realR2.noBoundsExtensions();

                    // Set tolerance
                    realR2.setTolerance(tolerance);

                    // Supress error messages and arrange for NaN to be returned as root if root not found
                    realR2.resetNaNexceptionToTrue();
                    realR2.supressLimitReachedMessage();
                    realR2.supressNaNmessage();

                    // call root searching method
                    igamma[1] = realR2.bisect(gif2, lowerBound2, upperBound2);
                }
            }

            return igamma;
        }
    }
}
