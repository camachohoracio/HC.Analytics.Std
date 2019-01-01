#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics.Regression;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;
using HC.Core.Helpers;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public static class DickeyFullerTest
    {

        public static bool CheckIsStationaryDouble(
            double[] xValues,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            //
            // get delta values
            //
            var yDeltas = new List<double>();
            var xList = new List<double[]>();
            for (int i = 1; i < xValues.Length; i++)
            {
                double dblCurrDelta = xValues[i] - xValues[i - 1];
                yDeltas.Add(dblCurrDelta);
                xList.Add(new[] { xValues[i - 1] });
            }
            alglib.linreg.lrreport lrreport;
            double[] weights = RegressionHelperAlgLib.GetRegressionWeights(
                xList,
                yDeltas,
                out lrreport);


            int intCutIndex = xList.Count/2;
            return GetDblTStat(0,
                               intCutIndex,
                               yDeltas,
                               xList,
                               weights,
                               dblConfidence,
                               out dblTStat,
                               out dblTestValue) &&
                   GetDblTStat(intCutIndex,
                               xList.Count,
                               yDeltas,
                               xList,
                               weights,
                               dblConfidence,
                               out dblTStat,
                               out dblTestValue);
        }

        private static bool GetDblTStat(
            int intStartIndex, 
            int intCutIndex, 
            List<double> yDeltas, 
            List<double[]> xList, 
            double[] weights, 
            double dblConfidence, 
            out double dblTStat, 
            out double dblTestValue)
        {
            double dblAvgX = (from n in xList select n[0]).Average();
            double dblSumSqX = 0;
            double dblSumSqY = 0;
            for (int i = intStartIndex; i < intCutIndex; i++)
            {
                double dblX = xList[i][0];
                dblSumSqX += Math.Pow(dblX - dblAvgX, 2);
                double dblY = yDeltas[i];
                double dblYPrediction = weights[0] * dblX + weights[1];
                double dblError = Math.Pow(dblY - dblYPrediction, 2);
                dblSumSqY += dblError;
            }

            //
            // standard error for the coeff
            //
            int intLeght = intCutIndex - intStartIndex;
            double dblXStdErr = Math.Sqrt(dblSumSqY / (intLeght - 2)) /
                                Math.Sqrt(dblSumSqX);

            dblTStat = weights[0] / dblXStdErr;

            bool blnCheck = DickeyFullerCriticalValues.TestIsStationary(
                dblTStat,
                dblConfidence,
                intLeght,
                out dblTestValue);
            return blnCheck;
        }

        public static bool CheckIsStationary(
            double[] xValues,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            //
            // according to the definition, if a series is cointegrated, 
            // then the subset should also be cointegrated
            //
            dblTStat = 0;
            dblTestValue = 0;

            try
            {
                if (xValues == null || xValues.Length < 2)
                {
                    return false;
                }

                //bool blnIsNormallyDistr = NormalDistrHypTest.IsNormallyDistributed(xValues.ToList());

                //if(!blnIsNormallyDistr) // todo, make sure this is not too strict!!
                //{
                //    return false;
                //}

                bool blnStationary0 = CheckIsStationary0(
                    xValues,
                    dblConfidence,
                    out dblTStat,
                    out dblTestValue);
                
                if (blnStationary0)
                {
                    double[] arr1;
                    double[] arr2;
                    ArrayHelper.SplitArray(
                        xValues,
                        out arr1,
                        out arr2,
                        0.5);
                    
                    double dblTStatTmp;
                    double dblTestValueTmp;
                    bool blnStationary1 = CheckIsStationary0(
                        arr1,
                        dblConfidence,
                        out dblTStatTmp,
                        out dblTestValueTmp);
                    if(blnStationary1)
                    {
                        bool blnStationary2 = CheckIsStationary0(
                            arr2,
                            dblConfidence,
                            out dblTStatTmp,
                            out dblTestValueTmp);
                        return blnStationary2;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public static bool CheckIsStationary0(
            double[] xValues,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            dblTStat = 0;
            dblTestValue = 0;
            try
            {
                if (xValues == null || xValues.Length < 2)
                {
                    return false;
                }
                //
                // get delta values
                //
                var yDeltas = new List<double>();
                var xList = new List<double[]>();
                for (int i = 1; i < xValues.Length; i++)
                {
                    double dblCurrDelta = xValues[i] - xValues[i - 1];
                    yDeltas.Add(dblCurrDelta);
                    xList.Add(new[] {xValues[i - 1]});
                }
                bool blnCheck = RegressionTtest.TestVariable(
                    0,
                    0,
                    new RegressionTrainerWrapper(xList, yDeltas),
                    dblConfidence,
                    out dblTestValue,
                    out dblTStat);

                return blnCheck;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }
    }
}
