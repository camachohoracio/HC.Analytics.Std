#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Exp : AbstractGpOperator
    {
        public Exp()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            var dblValue = Convert.ToDouble(parameters[0]);
            if (dblValue >= 10)
            {
                return 9999999999.0;
            }
            return Math.Exp(dblValue);
        }

        public override string ComputeToString(string[] parameters)
        {
            return "exp[" + parameters[0] + "]";
        }


        public override string ToString()
        {
            return "exp";
        }
    }
}
