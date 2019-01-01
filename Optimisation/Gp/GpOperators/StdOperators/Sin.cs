#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Sin : AbstractGpOperator
    {
        public Sin()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            return Math.Sin(Convert.ToDouble(parameters[0]));
        }

        public override string ComputeToString(string[] parameters)
        {
            return "sin[" + parameters[0] + "]";
        }

        public override string ToString()
        {
            return "sin";
        }
    }
}
