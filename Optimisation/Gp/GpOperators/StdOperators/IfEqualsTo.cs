#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class IfEqualsTo : AbstractGpOperator
    {
        public IfEqualsTo()
        {
            NumbParameters = 4;
        }

        public override object Compute(object[] parameters)
        {
            if (parameters[0].ToString().Equals(parameters[1].ToString()))
            {
                return parameters[2];
            }
            return parameters[3];
        }


        public override string ToString()
        {
            return "IfEqualsTo";
        }

        public override string ComputeToString(string[] parameters)
        {
            GpHelper.AddTabToParameter(parameters, 0);
            GpHelper.AddTabToParameter(parameters, 1);
            GpHelper.AddTabToParameter(parameters, 2);
            GpHelper.AddTabToParameter(parameters, 3);

            return Environment.NewLine + "if " +
                   Environment.NewLine + "(" +
                   Environment.NewLine + parameters[0] +
                   Environment.NewLine + " = " +
                   Environment.NewLine + parameters[1] +
                   Environment.NewLine + ")" +
                   Environment.NewLine + "then" +
                   Environment.NewLine + "{" +
                   Environment.NewLine + parameters[2] +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "else" +
                   Environment.NewLine + "{" +
                   Environment.NewLine + parameters[3] +
                   Environment.NewLine + "}" +
                   Environment.NewLine;
        }
    }
}
