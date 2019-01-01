#region

using System;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.Functions.Gamma
{
    public class RegularizedIncompleteGammaFunct
    {
        //  Tolerance used in terminating series in Incomplete Gamma Function calculations
        public static double FPMIN = 1e-300;
        private static double igfeps = 1e-8;
        //  Maximum number of iterations allowed in Incomplete Gamma Function calculations
        private static int igfiter = 1000;
        // A small number close to the smallest representable floating point number

        // Regularised Incomplete Gamma Function P(a,x) = integral from zero to x of (Exp(-t)t^(a-1))dt
        // Series representation of the function - valid for x < a + 1
        public static double incompleteGammaSer(double a, double x)
        {
            if (a < 0.0D || x < 0.0D)
            {
                throw new ArgumentException("\nFunction defined only for a >= 0 and x>=0");
            }
            if (x >= a + 1)
            {
                throw new ArgumentException("\nx >= a+1   use Continued Fraction Representation");
            }

            double igf = 0.0D;

            if (x != 0.0D)
            {
                int i = 0;
                bool check = true;

                double acopy = a;
                double sum = 1.0/a;
                double incr = sum;
                double loggamma = LogGammaFunct.logGamma2(a);

                while (check)
                {
                    ++i;
                    ++a;
                    incr *= x/a;
                    sum += incr;
                    if (Math.Abs(incr) < Math.Abs(sum)*igfeps)
                    {
                        igf = sum*Math.Exp(-x + acopy*Math.Log(x) - loggamma);
                        check = false;
                    }
                    if (i >= igfiter)
                    {
                        check = false;
                        igf = sum*Math.Exp(-x + acopy*Math.Log(x) - loggamma);
                        PrintToScreen.WriteLine(
                            "\nMaximum number of iterations were exceeded in Stat.incompleteGammaSer().\nCurrent value returned.\nIncrement = " +
                            incr + ".\nSum = " + sum + ".\nTolerance =  " +
                            igfeps);
                    }
                }
            }

            return igf;
        }

        // Regularised Incomplete Gamma Function P(a,x) = integral from zero to x of (Exp(-t)t^(a-1))dt
        // Continued Fraction representation of the function - valid for x >= a + 1
        // This method follows the general procedure used in Numerical Recipes for C,
        // The Art of Scientific Computing
        // by W H Press, S A Teukolsky, W T Vetterling & B P Flannery
        // Cambridge University Press,   http://www.nr.com/
        public static double incompleteGammaFract(double a, double x)
        {
            if (a < 0.0D || x < 0.0D)
            {
                throw new ArgumentException("\nFunction defined only for a >= 0 and x>=0");
            }
            if (x < a + 1)
            {
                throw new ArgumentException("\nx < a+1   Use Series Representation");
            }

            double igf = 0.0D;

            if (x != 0.0D)
            {
                int i = 0;
                double ii = 0;
                bool check = true;

                double loggamma = LogGammaFunct.logGamma2(a);
                double numer = 0.0D;
                double incr = 0.0D;
                double denom = x - a + 1.0D;
                double first = 1.0D/denom;
                double term = 1.0D/FPMIN;
                double prod = first;

                while (check)
                {
                    ++i;
                    ii = i;
                    numer = -ii*(ii - a);
                    denom += 2.0D;
                    first = numer*first + denom;
                    if (Math.Abs(first) < FPMIN)
                    {
                        first = FPMIN;
                    }
                    term = denom + numer/term;
                    if (Math.Abs(term) < FPMIN)
                    {
                        term = FPMIN;
                    }
                    first = 1.0D/first;
                    incr = first*term;
                    prod *= incr;
                    if (Math.Abs(incr - 1.0D) < igfeps)
                    {
                        check = false;
                    }
                    if (i >= igfiter)
                    {
                        check = false;
                        PrintToScreen.WriteLine(
                            "\nMaximum number of iterations were exceeded in Stat.incompleteGammaFract().\nCurrent value returned.\nIncrement - 1 = " +
                            (incr - 1) + ".\nTolerance =  " +
                            igfeps);
                    }
                }
                igf = 1.0D - Math.Exp(-x + a*Math.Log(x) - loggamma)*prod;
            }

            return igf;
        }

        // Regularised Incomplete Gamma Function P(a,x) = integral from zero to x of (Exp(-t)t^(a-1))dt
        public static double regularisedGammaFunction(double a, double x)
        {
            if (a < 0.0D || x < 0.0D)
            {
                throw new ArgumentException("\nFunction defined only for a >= 0 and x>=0");
            }
            double igf = 0.0D;

            if (x != 0)
            {
                if (x < a + 1.0D)
                {
                    // Series representation
                    igf = incompleteGammaSer(a, x);
                }
                else
                {
                    // Continued fraction representation
                    igf = incompleteGammaFract(a, x);
                }
            }
            return igf;
        }
    }
}
