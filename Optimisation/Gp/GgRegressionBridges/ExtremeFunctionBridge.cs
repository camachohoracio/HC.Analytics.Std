#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public class ExtremeFunctionBridge : AbstractGpRegressionBridge
    {
        public ExtremeFunctionBridge(
            GpOperatorsContainer gpOperatorsContainer,
            int testCases)
            : base(
                gpOperatorsContainer)
        {
            m_intNumbTestCases = testCases;
            DblMinX = 0;
            DblMaxX = 25;
            CreateTics();
        }

        public ExtremeFunctionBridge(
            GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            DblMinX = 0;
            DblMaxX = 25;
            CreateTics();
        }

        protected override double EvaluateFunction(double x)
        {
            return x*Math.Sin(Math.PI*x);
        }
    }
}
