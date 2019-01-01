#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Power : AbstractGpOperator
    {
        public Power()
        {
            NumbParameters = 2;
        }

        public override object Compute(object[] parameters)
        {
            try
            {
                double dblVal = Math.Pow(
                    Math.Abs(
                        Convert.ToDouble(parameters[0])),
                    Convert.ToDouble(parameters[1]));
                if (double.IsInfinity(dblVal))
                {
                    return 1e20;
                }
                if (double.IsNaN(dblVal))
                {
                    return double.NaN;
                }
                return dblVal;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        public override string ToString()
        {
            return "^";
        }

        public override string ComputeToString(string[] parameters)
        {
            return parameters[0] + " ^ " + parameters[1];
        }
    }
}
