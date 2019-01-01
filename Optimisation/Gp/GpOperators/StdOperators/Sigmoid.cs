#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Sigmoid : AbstractGpOperator
    {
        public Sigmoid()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            return 1.0/(1.0 + Math.Exp(-Convert.ToDouble(parameters[0])));
        }

        public override string ComputeToString(string[] parameters)
        {
            return "sigmoid[" + parameters[0] + "]";
        }

        public override string ToString()
        {
            return "sigmoid";
        }
    }
}
