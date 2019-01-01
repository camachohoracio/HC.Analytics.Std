using System;
using HC.Core.Logging;

namespace HC.Analytics.Mathematics
{
    public class BinarySearchFromFunction
    {
        public delegate double FuncDeletegate(double dblVal);
        
        public static double DoBinarySearch(
            double dblTarget,
            double dblError,
            FuncDeletegate func,
            out double dblCurrValue,
            int intMaxIterations = 5000,
            double dblMin = 0.0,
            double dblMax = 1.0)
        {
            dblCurrValue = 0;
            try
            {
                int intIterations = 0;
                double dblMid = 0.0;
                while (dblMin <= dblMax)
                {
                    dblMid = (dblMin + dblMax)/2;
                    dblCurrValue = func(dblMid);
                    if (Math.Abs(dblTarget - dblCurrValue) <= dblError)
                    {
                        return dblMid;
                    }
                    if (dblTarget < dblCurrValue)
                    {
                        dblMax = dblMid;
                    }
                    else
                    {
                        dblMin = dblMid;
                    }
                    intIterations++;
                    if (intIterations >= intMaxIterations)
                    {
                        return dblMid;
                    }
                }
                return dblMid;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}
