#region

using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public class PolynomialBridge : AbstractGpRegressionBridge
    {
        public PolynomialBridge(
            GpOperatorsContainer gpOperatorsContainer,
            int testCases)
            : base(gpOperatorsContainer)
        {
            m_intNumbTestCases = testCases;
            DblMinX = -5;
            DblMaxX = 5;
            CreateTics();
        }

        public PolynomialBridge(GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            DblMinX = -5;
            DblMaxX = 5;
            CreateTics();
        }

        protected override double EvaluateFunction(double x)
        {
            return x*x*x*x*x + x*x*x*x + x*x*x + x*x + x + 1;
        }
    }
}
