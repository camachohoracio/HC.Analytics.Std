#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public class NormalDistributionBridge : AbstractGpRegressionBridge
    {
        #region Constructors

        public NormalDistributionBridge(
            GpOperatorsContainer gpOperatorsContainer,
            int testCases)
            : base(gpOperatorsContainer)
        {
            m_intNumbTestCases = testCases;
            DblMinX = -5;
            DblMaxX = 5;
            CreateTics();
        }

        public NormalDistributionBridge(
            GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            DblMinX = -5;
            DblMaxX = 5;
            CreateTics();
        }

        #endregion

        protected override double EvaluateFunction(double x)
        {
            // The approximation to the cumulative normal distribution
            // |epsilon| < 7.5e^-8

            var b1 = 0.319381530;
            var b2 = -0.356563782;
            var b3 = 1.781477937;
            var b4 = -1.821255978;
            var b5 = 1.330274429;
            var p = 0.2316419;
            var c = 0.39894228;

            if (x >= 0.0)
            {
                var t = 1.0/(1.0 + p*x);
                return (1.0 - c*Math.Exp(-x*x/2.0)*t*
                        (t*(t*(t*(t*b5 + b4) + b3) + b2) + b1));
            }
            else
            {
                var t = 1.0/(1.0 - p*x);
                return (c*Math.Exp(-x*x/2.0)*t*
                        (t*(t*(t*(t*b5 + b4) + b3) + b2) + b1));
            }


            // return term * Math.Exp(-(m_dblXArr * m_dblXArr / 2));
        }
    }
}
