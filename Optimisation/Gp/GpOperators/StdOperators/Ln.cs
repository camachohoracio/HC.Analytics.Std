#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Ln : AbstractGpOperator
    {
        public Ln()
        {
            NumbParameters = 1;
        }

        public override object Compute(object[] parameters)
        {
            var dblValue = Convert.ToDouble(parameters[0]);
            if (dblValue <= 0)
            {
                return 99999999999;
            }
            return Math.Log(dblValue, Math.E);
        }

        public override string ComputeToString(string[] parameters)
        {
            return "ln[" + parameters[0] + "]";
        }


        public override string ToString()
        {
            return "LN";
        }
    }
}
