#region

using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class BivNormalDistFunction : AFunction3D
    {
        #region Memebers

        private readonly BivNormalDist m_bivNormalDist;

        #endregion

        public BivNormalDistFunction(
            double[] dblMeanArr,
            double[,] dblCovArray)
        {
            m_bivNormalDist = new BivNormalDist(
                dblMeanArr,
                dblCovArray,
                new RngWrapper());
            SetPlotterFunctionName("Bivariate Normal Distribution");
        }

        public override double EvaluateFunction(
            double dblX,
            double dblY)
        {
            return m_bivNormalDist.Pdf(dblX, dblY);
        }

        public override void SetFunctionLimits()
        {
            XMin = -3;
            XMax = 3;
            YMin = -3;
            YMax = 3;
            ZMin = 0;
            ZMax = 0.4;
        }
    }
}
