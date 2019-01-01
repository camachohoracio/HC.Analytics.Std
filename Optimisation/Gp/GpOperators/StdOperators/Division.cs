#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Division : AbstractGpOperator
    {
        public Division()
        {
            NumbParameters = 2;
        }

        public override object Compute(object[] parameters)
        {
            try
            {
                var dblValue1 = Convert.ToDouble(parameters[1]);
                if (dblValue1 == 0)
                {
                    return 999999999.0;
                }

                double dblVal = Convert.ToDouble(parameters[0])/dblValue1;

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
            GpHelper.AddTabToParameter(parameters, 0);
            GpHelper.AddTabToParameter(parameters, 1);

            return
                Environment.NewLine + "(" + Environment.NewLine +
                parameters[0] + Environment.NewLine +
                ")" + Environment.NewLine +
                "/" + Environment.NewLine +
                "(" + Environment.NewLine +
                parameters[1] + Environment.NewLine +
                ")" + Environment.NewLine;
        }

        public override string ToString()
        {
            return "/";
        }
    }
}
