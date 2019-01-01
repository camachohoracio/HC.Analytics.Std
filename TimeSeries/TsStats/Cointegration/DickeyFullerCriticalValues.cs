#region

using HC.Analytics.Probability.Distributions.Continuous;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public class DickeyFullerCriticalValues
    {
        #region Properties

        public double Confidence { get; set; }
        public int SampleSize { get; set; }
        public double TestValue { get; set; }

        #endregion

        public static bool TestIsStationary(
            double dblTValue,
            double dblConfidence,
            int intSampleSize,
            out double dblTestValue)
        {
            var tsStudentDist = new TStudentDist(intSampleSize -2, new RngWrapper());


            dblTestValue = 2 * tsStudentDist.CdfInv(dblConfidence);
            return dblTValue < dblTestValue;
        }
    }
}
