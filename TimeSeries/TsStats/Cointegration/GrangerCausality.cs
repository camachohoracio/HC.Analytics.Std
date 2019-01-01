using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public static class GrangerCausality
    {
        private const int LAGS = 10;

        public static bool ExistsCausality(
            List<double> data1,
            List<double> data2,
            double dblConfidence)
        {
            try
            {
                if (data1.Count < LAGS*2)
                {
                    throw new HCException("Invalid number of samples");
                }

                bool blnExistsCausality = ExistsCausality0(data1, data2, dblConfidence);
                if (!blnExistsCausality)
                {
                    List<double> data1a = TimeSeriesHelper.GetDerivative(1, data1);
                    List<double> data2a = TimeSeriesHelper.GetDerivative(1, data2);
                    blnExistsCausality = ExistsCausality0(data1a, data2a, dblConfidence);
                    if (!blnExistsCausality)
                    {
                        blnExistsCausality = ExistsCausality0(data2, data1, dblConfidence);
                        if (!blnExistsCausality)
                        {
                            blnExistsCausality = ExistsCausality0(data2a, data1a, dblConfidence);
                            return blnExistsCausality;
                        }
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        private static bool ExistsCausality0(
            List<double> data1, 
            List<double> data2, 
            double dblConfidence)
        {
            List<double> yData1;
            List<double[]> xData1;
            TimeSeriesHelper.GetAutoRegressiveVariables(
                LAGS,
                data1,
                out yData1,
                out xData1);
            var regression1 = new RegressionTrainerWrapper(xData1, yData1);
            var erros1 = regression1.GetErrors();

            List<double> yData2;
            List<double[]> xData2;
            TimeSeriesHelper.GetAutoRegressiveVariables(
                LAGS,
                data2,
                out yData2,
                out xData2);

            List<double[]> data3 = TimeSeriesHelper.MergeTsLists(
                xData1,
                xData2);
            bool blnExistsCausality = ExistsCausality(
                regression1.Weights.Length - 1,
                regression1.GetRss(),
                data3,
                yData1,
                dblConfidence);
            return blnExistsCausality;
        }

        private static bool ExistsCausality(
            int intP1,
            double dblRss1,
            List<double[]> xData2,
            List<double> yData2,
            double dblConfidence)
        {
            int intP2 = xData2.First().Length;
            var regression2 = new RegressionTrainerWrapper(
                xData2,
                yData2);
            int intDegrFreedom1 = intP2 - intP1;
            int intDegrFreedom2 = yData2.Count - intP2;
            double dblRss2 = regression2.GetRss();
            double dblF = ((dblRss1 - dblRss2)/intDegrFreedom1) /
                (dblRss2 / intDegrFreedom2);
            double dblCriticalValue = alglib.fdistr.fdistribution(intDegrFreedom1, intDegrFreedom2, dblConfidence);
            // nullHyp model2 does not provide better fit than model1 
            bool blnRejectNullHyp = dblF > dblCriticalValue;
            return blnRejectNullHyp;
        }
    }
}

