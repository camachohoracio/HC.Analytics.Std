#region

using System;
using HC.Analytics.Mathematics.Functions.Gamma;

#endregion

namespace HC.Analytics.Mathematics.Functions.Beta
{
    public class BetaFunct
    {
        // Beta function
        public static double betaFunction(double z, double w)
        {
            return Math.Exp(LogGammaFunct.logGamma2(z) +
                            LogGammaFunct.logGamma2(w) -
                            LogGammaFunct.logGamma2(z + w));
        }
    }
}
