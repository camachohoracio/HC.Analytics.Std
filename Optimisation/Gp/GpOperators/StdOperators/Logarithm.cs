#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Logarithm : AbstractGpOperator
    {
        public Logarithm()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            try
            {
                double dblValue = Convert.ToDouble(parameters[0]);
                if (dblValue <= 0)
                {
                    return 99999999999;
                }
                double dblVal = Math.Log(dblValue);

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

        public override string ComputeToString(string[] parameters)
        {
            return "log[" + parameters[0] + "]";
        }

        public override string ToString()
        {
            return "Log";
        }
    }
}
