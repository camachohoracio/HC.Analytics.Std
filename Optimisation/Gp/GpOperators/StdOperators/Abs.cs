#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Abs : AbstractGpOperator
    {
        public Abs()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            return Math.Abs(Convert.ToDouble(parameters[0]));
        }

        public override string ComputeToString(string[] parameters)
        {
            return "abs[" + parameters[0] + "]";
        }


        public override string ToString()
        {
            return "abs";
        }
    }
}
