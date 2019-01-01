#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public class SinRegressionBridge : AbstractGpRegressionBridge
    {
        public SinRegressionBridge(
            GpOperatorsContainer gpOperatorsContainer,
            int testCases)
            : base(gpOperatorsContainer)
        {
            m_intNumbTestCases = testCases;
            DblMinX = 0.0;
            DblMaxX = 6.0;
            CreateTics();
        }

        public SinRegressionBridge(GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            DblMinX = -(5*Math.PI);
            DblMaxX = 5*Math.PI;
            CreateTics();
        }

        protected override double EvaluateFunction(double x)
        {
            return Math.Sin(x);
        }
    }
}
