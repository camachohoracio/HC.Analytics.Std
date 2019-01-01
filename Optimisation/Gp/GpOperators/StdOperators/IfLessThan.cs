#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.StdOperators
{
    [Serializable]
    public class IfLessThan : AbstractGpOperator
    {
        public IfLessThan()
        {
            NumbParameters = 4;
        }

        public override object Compute(object[] parameters)
        {
            double dblValue0;
            if (parameters[0] is double)
            {
                dblValue0 = (double) parameters[0];
            }
            else
            {
                double.TryParse(parameters[0].ToString(), out dblValue0);
            }


            double dblValue1;
            if (parameters[1] is double)
            {
                dblValue1 = (double) parameters[1];
            }
            else
            {
                double.TryParse(parameters[1].ToString(), out dblValue1);
            }

            if (dblValue0 < dblValue1)
            {
                return parameters[2];
            }
            return parameters[3];
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
                   Environment.NewLine + " < " +
                   Environment.NewLine + parameters[1] +
                   Environment.NewLine + ")" +
                   Environment.NewLine + "then" +
                   Environment.NewLine + "{" +
                   Environment.NewLine + parameters[2] +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "else" +
                   Environment.NewLine + "{" +
                   Environment.NewLine + parameters[3] +
                   Environment.NewLine + "}" + Environment.NewLine;
        }

        public override string ToString()
        {
            return "IfLessThan";
        }
    }
}
