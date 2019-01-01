using System;
using System.Linq;
using HC.Analytics.Probability.Distributions.Continuous;
using HC.Analytics.Probability.Random;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;

namespace HC.Analytics.TimeSeries.TsStats
{
    public static class RegressionTtest
    {
        public static bool TestVariable(
            int intVariable,
            double dblVariableValueGuessNullHyp,
            RegressionTrainerWrapper regression, 
            double dblConfidence,
            out double dblTestValue,
            out double dblTStat)
        {
            var xData = regression.GetxData();
            double dblAvgX = (from n in xData select n[intVariable]).Average();
            double dblSumSqX = (from n in xData
                                select Math.Pow(n[intVariable] - dblAvgX, 2)).Sum();
            double dblSumSqY = (from n in regression.GetErrors()
                                select Math.Pow(n, 2)).Sum();

            //
            // standard error for the coeff
            //
            double dblXStdErr = Math.Sqrt(dblSumSqY / (xData.Count - 2)) /
                                Math.Sqrt(dblSumSqX);

            dblTStat = regression.Weights[intVariable] / dblXStdErr;
            var tsStudentDist = new TStudentDist(xData.Count - 2, new RngWrapper());

            dblTestValue = 2 * tsStudentDist.CdfInv(dblConfidence);
            return dblTStat < dblTestValue;
        }
    }
}

