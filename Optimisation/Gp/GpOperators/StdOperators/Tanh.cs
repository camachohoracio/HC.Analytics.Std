#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class Tanh : AbstractGpOperator
    {
        #region Constructors

        public Tanh()
        {
            NumbParameters = 1;
        }

        #endregion

        public override object Compute(object[] parameters)
        {
            return Math.Tanh(Convert.ToDouble(parameters[0]));
        }

        public override string ComputeToString(string[] parameters)
        {
            return "Tanh[" + parameters[0] + "]";
        }

        public override string ToString()
        {
            return "Tanh";
        }
    }
}
