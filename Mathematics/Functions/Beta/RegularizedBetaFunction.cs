#region

using System;
using HC.Analytics.Mathematics.Functions.Gamma;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Mathematics.Functions.Beta
{
    public class RegularizedBetaFunction
    {
        // A small number close to the smallest representable floating point number
        public static double FPMIN = 1e-300;

        // Regularised Incomplete Beta function
        // Continued Fraction approximation (see Numerical recipies for details of method)
        public static double regularisedBetaFunction(double z, double w, double x)
        {
            if (x < 0.0D || x > 1.0D)
            {
                throw new ArgumentException("Argument x, " + x + ", must be lie between 0 and 1 (inclusive)");
            }
            double ibeta = 0.0D;
            if (x == 0.0D)
            {
                ibeta = 0.0D;
            }
            else
            {
                if (x == 1.0D)
                {
                    ibeta = 1.0D;
                }
                else
                {
                    // Term before continued fraction
                    ibeta = Math.Exp(
                        LogGammaFunct.logGamma2(z + w) - LogGammaFunct.logGamma2(z) - LogGammaFunct.logGamma2(w) +
                        z*Math.Log(x) + w*Math.Log(1.0D - x));
                    // Continued fraction
                    if (x < (z + 1.0D)/(z + w + 2.0D))
                    {
                        ibeta = ibeta*contFract(z, w, x)/z;
                    }
                    else
                    {
                        // Use symmetry relationship
                        ibeta = 1.0D - ibeta*contFract(w, z, 1.0D - x)/w;
                    }
                }
            }
            return ibeta;
        }


        // Regularised Incomplete Beta function
        // Continued Fraction approximation (see Numerical recipies for details of method)
        public static double regularizedBetaFunction(double z, double w, double x)
        {
            return regularisedBetaFunction(z, w, x);
        }

        // Regularised Incomplete Beta function
        // Continued Fraction approximation (see Numerical recipies for details of method)
        // retained for compatibility reasons
        public static double incompleteBeta(double z, double w, double x)
        {
            return regularisedBetaFunction(z, w, x);
        }

        // Incomplete fraction summation used in the method regularisedBetaFunction
        // modified Lentz's method
        public static double contFract(double a, double b, double x)
        {
            int maxit = 500;
            double eps = 3.0e-7;
            double aplusb = a + b;
            double aplus1 = a + 1.0D;
            double aminus1 = a - 1.0D;
            double c = 1.0D;
            double d = 1.0D - aplusb*x/aplus1;
            if (Math.Abs(d) < FPMIN)
            {
                d = FPMIN;
            }
            d = 1.0D/d;
            double h = d;
            double aa = 0.0D;
            double del = 0.0D;
            int i = 1, i2 = 0;
            bool test = true;
            while (test)
            {
                i2 = 2*i;
                aa = i*(b - i)*x/((aminus1 + i2)*(a + i2));
                d = 1.0D + aa*d;
                if (Math.Abs(d) < FPMIN)
                {
                    d = FPMIN;
                }
                c = 1.0D + aa/c;
                if (Math.Abs(c) < FPMIN)
                {
                    c = FPMIN;
                }
                d = 1.0D/d;
                h *= d*c;
                aa = -(a + i)*(aplusb + i)*x/((a + i2)*(aplus1 + i2));
                d = 1.0D + aa*d;
                if (Math.Abs(d) < FPMIN)
                {
                    d = FPMIN;
                }
                c = 1.0D + aa/c;
                if (Math.Abs(c) < FPMIN)
                {
                    c = FPMIN;
                }
                d = 1.0D/d;
                del = d*c;
                h *= del;
                i++;
                if (Math.Abs(del - 1.0D) < eps)
                {
                    test = false;
                }
                if (i > maxit)
                {
                    test = false;
                    PrintToScreen.WriteLine("Maximum number of iterations (" + maxit +
                                            ") exceeded in Stat.contFract in Stat.incomplete Beta");
                }
            }
            return h;
        }
    }
}
