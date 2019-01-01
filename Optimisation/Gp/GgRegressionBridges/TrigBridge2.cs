#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public class TrigBridge2 : AbstractGpRegressionBridge
    {
        public TrigBridge2(
            GpOperatorsContainer gpOperatorsContainer,
            int testCases)
            : base(gpOperatorsContainer)
        {
            m_intNumbTestCases = testCases;
            DblMinX = -5;
            DblMaxX = 2;
            CreateTics();
        }

        public TrigBridge2(GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            DblMinX = -5;
            DblMaxX = 2;
            CreateTics();
        }

        protected override double EvaluateFunction(double x)
        {
            return Math.Sin(x*x + x) + Math.Cos(3*x);
        }
    }
}
