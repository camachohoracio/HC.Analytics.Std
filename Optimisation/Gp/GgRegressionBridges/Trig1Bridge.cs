#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public class Trig1Bridge : AbstractGpRegressionBridge
    {
        public Trig1Bridge(
            GpOperatorsContainer gpOperatorsContainer,
            int testCases)
            : base(gpOperatorsContainer)
        {
            m_intNumbTestCases = testCases;
            DblMinX = 0;
            DblMaxX = 6.5;
            CreateTics();
        }

        public Trig1Bridge(GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            DblMinX = 0;
            DblMaxX = 6.5;
            CreateTics();
        }

        protected override double EvaluateFunction(double x)
        {
            return 0.1*x*x + Math.Sin(x)*Math.Sin(100*x);
        }
    }
}
