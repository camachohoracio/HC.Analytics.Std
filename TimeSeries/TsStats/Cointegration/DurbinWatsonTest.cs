#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public static class DurbinWatsonTest
    {
        public static bool CheckIsStationary(
            List<double> xValues,
            List<double> yValues,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            return CheckIsStationary(
                (from n in xValues select new[] {n}).ToList(),
                yValues,
                dblConfidence,
                out dblTStat,
                out dblTestValue);
        }

        public static bool CheckIsStationary(
            List<double[]> xValues,
            List<double> yValues,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            var regression = new RegressionTrainerWrapper(xValues, yValues);
            return CheckIsStationary(regression, out dblTStat, dblConfidence, out dblTestValue);
        }

        public static bool CheckIsStationary(
            double[] xValues,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            var regression = new RegressionTrainerWrapper(xValues);
            return CheckIsStationary(regression, out dblTStat, dblConfidence, out dblTestValue);
        }

        private static bool CheckIsStationary(RegressionTrainerWrapper regression, out double dblTStat, double dblConfidence,
                                              out double dblTestValue)
        {
            List<double> errors = regression.GetErrors();
            errors = (from n in errors select n*10000).ToList();
            return CheckIsStationary(errors, out dblTStat, dblConfidence, out dblTestValue);
        }

        public static bool CheckIsStationary(
            List<double> data,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            double dblMean = data.Average();
            List<double> errors = (from n in data select n - dblMean).ToList();
            return CheckIsStationary(
                errors,
                out dblTStat,
                dblConfidence,
                out dblTestValue);
        }

        private static bool CheckIsStationary(
            List<double> errors,
            out double dblTStat,
            double dblConfidence,
            out double dblTestValue)
        {
            double dblErrorPrevSumSq = 0;
            for (int i = 1; i < errors.Count; i++)
            {
                dblErrorPrevSumSq +=
                    Math.Pow(errors[i] - errors[i - 1], 2);
            }
            double dblErrSumSq = (from n in errors select Math.Pow(n, 2)).Sum();
            dblTStat = dblErrorPrevSumSq/dblErrSumSq;
            return DurbingWatsonCriticalValues.TestIsStationary(dblTStat,
                                                                dblConfidence,
                                                                errors.Count,
                                                                out dblTestValue);
        }
    }
}
