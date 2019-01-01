#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Subtraction : AbstractGpOperator
    {
        public Subtraction()
        {
            NumbParameters = 2;
        }

        public override object Compute(object[] parameters)
        {
            return Convert.ToDouble(parameters[0]) - Convert.ToDouble(parameters[1]);
        }

        public override string ComputeToString(string[] parameters)
        {
            return parameters[0] + " - " + parameters[1];
        }

        public override string ToString()
        {
            return "-";
        }
    }
}
