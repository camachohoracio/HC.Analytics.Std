﻿#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HC.Analytics.Probability;
using HC.Analytics.TimeSeries.TsStats;
using HC.Analytics.TimeSeries.TsStats.Kalman;
using HC.Core.Exceptions;
using HC.Core.Logging;
using NUnit.Framework;

#endregion

namespace HC.Analytics.Statistics
{
    public static class WintersForecasting
    {
        private const int intMinPeriodSizeL = 5;
        private const int intMaxPeriodSizeL = 20;
        private const double minProportionOfData = 0.1;
        private const double deltaProportionOfData = 0.1;
        private const int INSTANCE_SAMPLE_SIZE = 60;
        private const double OUTLAYER_THRESHOLD = 15;

        [Test]
        public static void DoTest()
        {
            try
            {
                const string strFileName = @"C:\HC\Data\WintersTestData.txt";
                var values = new List<double>();
                using (var sr = new StreamReader(strFileName))
                {
                    string strLine;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        values.Add(
                            double.Parse(strLine));
                    }
                }
                List<double> outOfSampleData;
                RunWinters(
                    values, 
                    12, 
                    false,
                    out outOfSampleData);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        [Test]
        public static void DoTest2()
        {
            try
            {
                const string strFileName = @"C:\HC\Data\WintersRatioData.csv";
                var values = new List<double>();
                using (var sr = new StreamReader(strFileName))
                {
                    sr.ReadLine();
                    string strLine;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        if(string.IsNullOrEmpty(strLine))
                        {
                            continue;
                        }
                        var toks = strLine.Split(',');
                        if(toks.Length < 2)
                        {
                            continue;
                        }
                        values.Add(
                            double.Parse(toks[1]));
                    }
                }

                double[] bestLastBestParameters;
                int intBestPeriodSizeL;
                int bestSampleSize;
                List<double> outOfSampleForecasts;
                List<double> inSampleForecasts;
                RunWintersIterative(
                    values, 
                    out bestLastBestParameters, 
                    out intBestPeriodSizeL, 
                    out bestSampleSize, 
                    true, // save results
                    out inSampleForecasts,
                    out outOfSampleForecasts);

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static void RunWintersIterative(
            List<double> data,
            out double[] bestLastBestParameters,
            out int intBestPeriodSizeL,
            out int intBestSampleSize,
            bool blnSaveResults,
            out List<double> inSampleForecasts,
            out List<double> outOfSampleForecasts)
        {
            inSampleForecasts = null;
            bestLastBestParameters = null;
            intBestPeriodSizeL = 0;
            intBestSampleSize = 0;
            outOfSampleForecasts = null;
            try
            {
                double dblMin;
                double dblDelta;
                data = ProcessData(
                    data,
                    out dblMin,
                    out dblDelta);

                double dblMinError = double.MaxValue;
                double dblCurrProportionOfData = minProportionOfData;
                List<double> bestForecasts = null;
                double dblBestProportionOfData = 0;
                List<double> bestOutOfSampleList = null;
                List<double> bestErrorsList = null;

                while (dblCurrProportionOfData <= 1)
                {
                    Console.WriteLine("Running proportion " + dblCurrProportionOfData);
                    for (int i = intMinPeriodSizeL; i < intMaxPeriodSizeL; i++)
                    {
                        double dblCurrError;
                        List<double> outOfSampleList;
                        List<double> currErrors;
                        double[] lastBestParameters;
                        int intCurrProportionSize;
                        List<double> forecasts = RunWintersForProportion(
                                   data,
                                   i,
                                   false, // save results
                                   out currErrors,
                                   true, // train with out of sample
                                   dblCurrProportionOfData,
                                   out outOfSampleList,
                                   out lastBestParameters,
                                   out intCurrProportionSize);

                        if (forecasts != null && 
                            forecasts.Count > 0 &&
                            (dblCurrError = currErrors.Average()) < dblMinError)
                        {
                            bool blnIsBest;
                            if (bestErrorsList == null)
                            {
                                blnIsBest = true;
                            }
                            else
                            {
                                blnIsBest = !HypothesisTests.IsMeanDiffEqualTo(
                                    0.15,
                                    0,
                                    currErrors,
                                    bestErrorsList);
                            }

                            if (blnIsBest)
                            {
                                bestLastBestParameters = lastBestParameters;
                                bestErrorsList = currErrors;
                                bestOutOfSampleList = outOfSampleList;
                                dblBestProportionOfData = dblCurrProportionOfData;
                                bestForecasts = forecasts;
                                intBestPeriodSizeL = i;
                                dblMinError = dblCurrError;
                                intBestSampleSize = intCurrProportionSize;
                                Console.WriteLine("intBestPeriodSizeL " + intBestPeriodSizeL);
                                Console.WriteLine("MinError " + dblCurrError);
                            }
                            if (bestOutOfSampleList.Count != bestForecasts.Count)
                            {
                                throw new HCException("Invalid list size");
                            }
                        }
                    }

                    dblCurrProportionOfData += deltaProportionOfData;
                }

                if (bestForecasts != null && blnSaveResults)
                {
                    using (var streamWriter = new StreamWriter(@"c:\wintersResults.csv"))
                    {
                        streamWriter.WriteLine("Final MinError " + dblMinError);
                        streamWriter.WriteLine("Perid size " + intBestPeriodSizeL);
                        streamWriter.WriteLine("dblBestProportionOfData " + dblBestProportionOfData);
                        streamWriter.WriteLine("real,forecast");

                        for (int i = 0; i < bestOutOfSampleList.Count; i++)
                        {
                            streamWriter.WriteLine(bestOutOfSampleList[i] + "," + bestForecasts[i]);
                        }
                    }
                }

                inSampleForecasts = bestForecasts;

                //
                // now get out of sample results
                //
                List<double> trainingData;
                List<double> testData;
                TimeSeriesHelper.SplitListTail(
                    data,
                    intBestSampleSize,
                    out trainingData,
                    out testData);

                outOfSampleForecasts = GetForecasts(
                    testData,
                    intBestPeriodSizeL,
                    blnSaveResults,
                    bestLastBestParameters);
                
                //outOfSampleForecasts = RunWinters(testData,
                //           intBestPeriodSizeL,
                //           blnSaveResults,
                //           out dblError,
                //           out bestParams,
                //           false,
                //           out outOfSampleData);

                //
                // denormalize data
                //
                for (int i = 0; i < outOfSampleForecasts.Count; i++)
                {
                    outOfSampleForecasts[i] = dblMin + (outOfSampleForecasts[i]*dblDelta);
                }
                if (inSampleForecasts != null)
                {
                    for (int i = 0; i < inSampleForecasts.Count; i++)
                    {
                        inSampleForecasts[i] = dblMin + (inSampleForecasts[i]*dblDelta);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static List<double> ProcessData(
            List<double> data,
            out double dblMin,
            out double dblDelta)
        {
            dblMin = 0;
            dblDelta = 0;
            try
            {
                data = TimeSeriesHelper.NormalizeData(data, out dblMin, out dblDelta);
                data = Outliers.CorrectOutliers(data, OUTLAYER_THRESHOLD);
                var filter = new KalmanFilter();
                var oldData = data;
                var filteredData = new List<double>();
                foreach (double d in data)
                {
                    double dblNoise;
                    double dblFilteredValue;
                    if (!filter.Filter(d, out dblFilteredValue, out dblNoise))
                    {
                        dblFilteredValue = d;
                    }

                    filteredData.Add(dblFilteredValue);
                }

                //using (var sw = new StreamWriter(@"c:\filtered.csv"))
                //{
                //    for (int i = 0; i < oldData.Count; i++)
                //    {
                //        sw.WriteLine(oldData[i] + "," + filteredData[i]);
                //    }
                //}

                data = filteredData;
                return data;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public static List<double> RunWinters(
            List<double> data,
            int intPeriodSizeL,
            bool blnTrainWithOutOfSample,
            out List<double> outOfSampleData)
        {
            double dblCurrError;
            double[] bestParams;
            return RunWinters(data,
                       intPeriodSizeL,
                       true,
                       out dblCurrError,
                       out bestParams,
                       blnTrainWithOutOfSample,
                       out outOfSampleData);
        }

        public static List<double> RunWintersForProportion(
            List<double> data,
            int intPeriodSizeL,
            bool blnSaveResults,
            out List<double> errors,
            bool blnTrainWithOutOfSample,
            double dblSampleProportion,
            out List<double> finalOutOfSampleList,
            out double[] lastBestParameters,
            out int intProportionSize)
        {
            errors = null;
            finalOutOfSampleList = null;
            lastBestParameters = new double[0];
            intProportionSize = 0;
            try
            {
                int intMinSampleSize = intPeriodSizeL*4;
                intProportionSize =
                    Math.Max(
                        intMinSampleSize,
                        (int) (data.Count*dblSampleProportion));

                //
                // start with the last chunk of data
                //
                List<double> currData;
                List<double> dataDummy;
                TimeSeriesHelper.SplitListTail(
                    data,
                    intProportionSize,
                    out dataDummy,
                    out currData);
                TimeSeriesHelper.SplitListTail(
                    data,
                    intPeriodSizeL,
                    out data,
                    out dataDummy);


                errors = new List<double>();
                var allForecasts = new List<List<double>>();
                var allOutOfSampleData = new List<List<double>>();
                while (currData.Count >= intProportionSize)
                {

                    double dblCurrError;
                    double[] currParams;
                    List<double> outOfSampleData;
                    List<double> currForecasts = RunWinters(
                        currData,
                        intPeriodSizeL,
                        false,
                        out dblCurrError,
                        out currParams,
                        true,
                        out outOfSampleData);

                    if(lastBestParameters == null ||
                        lastBestParameters.Length == 0)
                    {
                        lastBestParameters = currParams;
                    }

                    allForecasts.Add(currForecasts);
                    allOutOfSampleData.Add(outOfSampleData);
                    errors.Add(dblCurrError);

                    if(data.Count < intProportionSize)
                    {
                        break;
                    }

                    TimeSeriesHelper.SplitListTail(
                        data,
                        intProportionSize,
                        out dataDummy,
                        out currData);
                    TimeSeriesHelper.SplitListTail(
                        data,
                        intPeriodSizeL,
                        out data,
                        out dataDummy);

                }

                if (errors.Count < INSTANCE_SAMPLE_SIZE)
                {
                    return new List<double>();
                }
                allForecasts.Reverse();
                allOutOfSampleData.Reverse();
                var finalForecastList = new List<double>();
                finalOutOfSampleList = new List<double>();
                for (int i = 0; i < allForecasts.Count; i++)
                {
                    List<double> forecast = allForecasts[i];
                    finalForecastList.AddRange(forecast);
                    finalOutOfSampleList.AddRange(allOutOfSampleData[i]);
                }
                return finalForecastList;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static List<double> RunWinters(
            List<double> data,
            int intPeriodSizeL,
            bool blnSaveResults,
            out double dblError,
            out double[] bestParams,
            bool blnTrainWithOutOfSample,
            out List<double> outOfSampleData)
        {
            dblError = double.MaxValue;
            bestParams = null;
            outOfSampleData = null;
            try
            {
                if (blnTrainWithOutOfSample)
                {
                    TimeSeriesHelper.SplitListTail(
                        data,
                        intPeriodSizeL,
                        out data,
                        out outOfSampleData);
                }

                List<double> validationData;
                double dblB0;
                double dblS0;
                double[] dblAvgI;
                GetImputParams(data, 
                    intPeriodSizeL, 
                    out validationData, 
                    out dblB0, 
                    out dblS0, 
                    out dblAvgI);
                //
                // create problem
                // m = number of functions = 1
                // x = initial values
                //
                double[] x;
                using(var optimiseWorker = new WintersForecastingOptimiseWorker(
                                                        intPeriodSizeL,
                                                        blnTrainWithOutOfSample,
                                                        outOfSampleData,
                                                        validationData,
                                                        dblAvgI,
                                                        dblS0,
                                                        dblB0))
                {
                    x = optimiseWorker.DoOptimise();
                }

                //List<double> ltList;
                //double dblBPrev;
                //double dblSPrev;
                //List<double> inSampleForecasts;
                //double dblEcmLowerBound = GetEcm(
                //    intPeriodSizeL,
                //    dblAlpha,
                //    dblB0,
                //    dblS0,
                //    dblGamma,
                //    dblBeta,
                //    dblAvgI,
                //    validationData,
                //    out ltList,
                //    out dblBPrev,
                //    out dblSPrev,
                //    out inSampleForecasts);

                bestParams = x;
                List<double> forcasts = GetForcasts(
                    intPeriodSizeL, 
                    blnSaveResults, 
                    out dblError, 
                    blnTrainWithOutOfSample, 
                    outOfSampleData, 
                    dblS0, 
                    x, 
                    dblB0, 
                    dblAvgI, 
                    validationData);


                return forcasts;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        private static List<double> GetForecasts(
            List<double> data,
            int intPeriodSizeL,
            bool blnSaveResults,
            double[] x)
        {
            try
            {
                List<double> validationData;
                double dblB0;
                double dblS0;
                double[] dblAvgI;

                GetImputParams(data,
                               intPeriodSizeL,
                               out validationData,
                               out dblB0,
                               out dblS0,
                               out dblAvgI);

                double dblError;

                return GetForcasts(
                    intPeriodSizeL,
                    blnSaveResults,
                    out dblError,
                    false,
                    data,
                    dblS0,
                    x,
                    dblB0,
                    dblAvgI,
                    validationData);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        private static List<double> GetForcasts(
            int intPeriodSizeL, 
            bool blnSaveResults, 
            out double dblError, 
            bool blnTrainWithOutOfSample, 
            List<double> outOfSampleData, 
            double dblS0, 
            double[] x,
            double dblB0, 
            double[] dblAvgI, 
            List<double> validationData)
        {
            dblError = 0;
            try
            {
                double dblAlphaOpt = x[0];
                double dblBetaOpt = x[1];
                double dblGammaOpt = x[2];
                List<double> ltList;
                double dblBPrev;
                double dblSPrev;
                List<double> inSampleForecasts;
                double dblEcmOptimised = GetEcm(
                    intPeriodSizeL,
                    dblAlphaOpt,
                    dblB0,
                    dblS0,
                    dblGammaOpt,
                    dblBetaOpt,
                    dblAvgI,
                    validationData,
                    out ltList,
                    out dblBPrev,
                    out dblSPrev,
                    out inSampleForecasts);

                dblError = dblEcmOptimised;

                var forcasts = new List<double>();

                StreamWriter streamWriter = null;

                if (blnSaveResults)
                {
                    streamWriter = new StreamWriter(@"c:\wintersInstanceResults.csv");
                    string strMessage = "dblEcmOptimised " + dblEcmOptimised;
                    streamWriter.WriteLine(strMessage);
                    streamWriter.WriteLine("real,forecast");

                    for (int i = 0; i < validationData.Count; i++)
                    {
                        streamWriter.WriteLine(validationData[i] + "," + inSampleForecasts[i]);
                    }
                }
                for (int j = 0; j < intPeriodSizeL; j++)
                {
                    double dblCurrForecast = ((dblSPrev + (dblBPrev*(j + 1))))*ltList[j];
                    forcasts.Add(dblCurrForecast);
                    if (streamWriter != null)
                    {
                        if (blnTrainWithOutOfSample)
                        {
                            streamWriter.Write(outOfSampleData[j]);
                        }
                        streamWriter.WriteLine("," + dblCurrForecast);
                    }
                }
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Dispose();
                }
                return forcasts;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        private static void GetImputParams(
            List<double> data,
            int intPeriodSizeL,
            out List<double> validationData,
            out double dblB0,
            out double dblS0,
            out double[] dblAvgI)
        {
            validationData = null;
            dblB0 = 0;
            dblS0 = 0;
            dblAvgI = new double[] { };
            try
            {
                int intNumOfFullSamplesR = data.Count/intPeriodSizeL;
                if (intNumOfFullSamplesR < 3)
                {
                    throw new HCException("Not enough samples");
                }

                int intNumOfFullSamples = intNumOfFullSamplesR*intPeriodSizeL;
                List<double> samples = TimeSeriesHelper.TakeLast(
                    data,
                    intNumOfFullSamples);
                TimeSeriesHelper.SplitListHead(
                    samples,
                    samples.Count - intPeriodSizeL,
                    out samples,
                    out validationData);

                intNumOfFullSamplesR--;
                double dblFirstAvg = samples.Take(intPeriodSizeL).Average();
                double dblLastAvg = TimeSeriesHelper.TakeLast(
                    samples,
                    intPeriodSizeL).Average();

                dblB0 = (dblLastAvg - dblFirstAvg)/((intNumOfFullSamplesR - 1.0)*intPeriodSizeL);
                dblS0 = dblFirstAvg - (dblB0*intPeriodSizeL/2.0);

                dblAvgI = GetStationaryData(
                    intPeriodSizeL,
                    samples,
                    intNumOfFullSamplesR,
                    dblB0);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private static double[] GetStationaryData(
            int intPeriodSizeL, 
            List<double> samples, 
            int intNumOfFullSamplesR, 
            double dblB0)
        {
            try
            {
                List<double> restOfData = samples.ToList();
                var dblAvgI = new double[intPeriodSizeL];
                for (int i = 0; i < intNumOfFullSamplesR; i++)
                {
                    List<double> currSamples;
                    TimeSeriesHelper.SplitListHead(
                        restOfData,
                        intPeriodSizeL,
                        out currSamples,
                        out restOfData);
                    if (currSamples.Count != intPeriodSizeL)
                    {
                        throw new HCException("Invalid sample size");
                    }
                    double dblCurrAvg = currSamples.Average();
                    for (int j = 0; j < intPeriodSizeL; j++)
                    {
                        double dblI = currSamples[j]/
                                      (dblCurrAvg - (((intPeriodSizeL + 1.0)/2.0) - (j + 1))*dblB0);
                        dblAvgI[j] += dblI;
                    }
                }
                for (int j = 0; j < intPeriodSizeL; j++)
                {
                    dblAvgI[j] /= intNumOfFullSamplesR;
                }

                //
                // normalize data
                //
                double dblSumI = dblAvgI.Sum();
                for (int j = 0; j < intPeriodSizeL; j++)
                {
                    dblAvgI[j] = (dblAvgI[j]*intPeriodSizeL)/dblSumI;
                }
                if ((dblAvgI.Sum() - intPeriodSizeL) > 1e-6)
                {
                    throw new HCException("Invalid sum of values");
                }

                return dblAvgI;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static double GetEcm(
            int intPeriodSizeL, 
            double dblAlpha, 
            double dblB0, 
            double dblS0, 
            double dblGamma,
            double dblBeta, 
            double[] dblAvgI, 
            List<double> validationData, 
            out List<double> ltList, 
            out double dblBPrev,
            out double dblSPrev,
            out List<double> inSampleForecasts)
        {
            //
            // evaluation of the forecast with ECM
            //
            ltList = new List<double>();
            dblBPrev = 0;
            dblSPrev = 0;
            inSampleForecasts = new List<double>();
            try
            {
                double dblSecondValue = (1 - dblAlpha)*(dblB0 + dblS0);
                dblSPrev = dblS0;
                dblBPrev = dblB0;
                ltList = new List<double>();
                for (int j = 0; j < intPeriodSizeL; j++)
                {
                    double dblCurrValue = validationData[j];
                    double dblSt = (dblAlpha*dblCurrValue/dblAvgI[j]) + dblSecondValue;
                    double dblBt = dblBeta*(dblSt - dblSPrev) + (1 - dblBeta)*dblBPrev;
                    double dblLt = (dblGamma*dblCurrValue/dblSt) + ((1 - dblGamma)*dblAvgI[j]);
                    ltList.Add(dblLt);
                    double dblPredictionInSample = (dblSPrev + dblBPrev)*dblAvgI[j];
                    dblSPrev = dblSt;
                    dblBPrev = dblBt;
                    inSampleForecasts.Add(dblPredictionInSample);
                }

                inSampleForecasts = Outliers.CorrectOutliers(inSampleForecasts, OUTLAYER_THRESHOLD);
                double dblEcm = 0;
                for (int j = 0; j < intPeriodSizeL; j++)
                {
                    double dblPredictionInSample = inSampleForecasts[j];
                    double dblCurrValue = validationData[j];
                    dblEcm += Math.Pow(dblPredictionInSample - dblCurrValue, 2);
                }

                dblEcm /= intPeriodSizeL;
                return dblEcm;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}