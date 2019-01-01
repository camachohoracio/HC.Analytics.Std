#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Cosine : AbstractGpOperator
    {
        public Cosine()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            return Math.Cos(Convert.ToDouble(parameters[0]));
        }

        public override string ComputeToString(string[] parameters)
        {
            return "cos[" + parameters[0] + "]";
        }

        public override string ToString()
        {
            return "cos";
        }
    }
}
