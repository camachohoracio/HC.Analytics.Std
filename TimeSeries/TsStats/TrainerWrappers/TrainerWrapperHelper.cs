using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics;
using HC.Core;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public static class TrainerWrapperHelper
    {
        public static void PrepareData(
            List<double[]> xVars0,
            List<double> yVars0,
            int intClases,
            bool blnVerbose,
            int intForecastDays,
            TrainerWrapperFactory trainerWrapperFactory,
            out ITrainerWrapper[] trainerWrappers,
            out double[] mrseArr,
            out double[] mean,
            out double[] stdDev,
            out List<List<double>> allSamplesY,
            out List<List<double[]>> allSamplesX,
            int intThreads)
        {
            trainerWrappers = new ITrainerWrapper[] { };
            mrseArr = new double[] { };
            allSamplesY = null;
            allSamplesX = null;
            mean = null;
            stdDev = null;

            try
            {

                List<List<int>> mappings;
                TimeSeriesHelper.LoadSamplesGivenForecastPeriods(
                    yVars0,
                    xVars0,
                    intForecastDays, 
                    out allSamplesX,
                    out allSamplesY,
                    out mappings);
                
                if (blnVerbose)
                {
                    Verboser.WriteLine(
                        trainerWrapperFactory.EnumTrainerWrapper.ToString() +
                        " training ensemble of [" +
                        intForecastDays + "] with total [" + xVars0.Count + "] samples [" +
                        allSamplesX[0].Count + "] ensembleSample and [" +
                        xVars0.First().Length + "] variables [" + intForecastDays + "] forecasts...");
                }

                trainerWrappers = new ITrainerWrapper[allSamplesX.Count];
                mrseArr = new double[allSamplesX.Count];
                mean = new double[allSamplesX.Count];
                stdDev = new double[allSamplesX.Count];
                var trainerWrappersTmp = trainerWrappers;
                var mrseArrTmp = mrseArr;
                var meanTmp = mean;
                var stdDevTmp = stdDev;
                var allSamplesXTmp = allSamplesX;
                var allSamplesYTmp = allSamplesY;

                //Parallel.For(0,
                //             allSamplesX.Count,
                //             new ParallelOptions { MaxDegreeOfParallelism = intThreads },
                //             delegate(int i) // TODO, use parallel.for
                             for (int i = 0; i < allSamplesX.Count; i++)
                             {
                                 try
                                 {
                                     ITrainerWrapper trainerWrapper = trainerWrapperFactory.Build(
                                         allSamplesXTmp[i],
                                         allSamplesYTmp[i],
                                         intClases,
                                         blnVerbose);

                                     List<double> errors;
                                     double dblMrseBase =
                                         ((ATrainerWrapper) trainerWrapper).GetMrse(
                                             out errors);
                                     trainerWrappersTmp[i] = trainerWrapper;
                                     mrseArrTmp[i] = dblMrseBase;
                                     var errSq = (from n in errors select n*n).ToList();
                                     meanTmp[i] = errSq.Average();
                                     stdDevTmp[i] = StdDev.GetSampleStdDev(errSq);
                                     Verboser.WriteLine("Finish training [" +
                                                        i + "]/[" +
                                                        allSamplesXTmp.Count + "][" +
                                                        trainerWrapperFactory.EnumTrainerWrapper + "][" +
                                                        trainerWrapper.GetMrse() + "] mrse");
                                 }
                                 catch(Exception ex)
                                 {
                                     Logger.Log(ex);
                                 }
                             }//);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
