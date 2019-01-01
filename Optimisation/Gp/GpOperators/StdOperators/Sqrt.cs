#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Sqrt : AbstractGpOperator
    {
        public Sqrt()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            var dblValue = Convert.ToDouble(parameters[0]);
            if (dblValue < 0)
            {
                return 99999999999;
            }
            return Math.Sqrt(dblValue);
        }

        public override string ComputeToString(string[] parameters)
        {
            return "sqrt[" + parameters[0] + "]";
        }

        public override string ToString()
        {
            return "sqrt";
        }
    }
}
