using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public class GenericEnsembleTainerWrapper : ATrainerWrapper
    {
        private readonly ITrainerWrapper[] m_trainerWrappers;

        public GenericEnsembleTainerWrapper(
            int intForecastDays,
            List<double[]> xData,
            List<double> yData,
            TrainerWrapperFactory trainerWrapperFactory,
            int intNumClasses,
            bool blnVerbose = true,
            int intThreads = 40)
        {
            try
            {
                m_xData = xData;
                m_yData = yData;
                m_intNumClasses = intNumClasses;
                m_blnVerbose = blnVerbose;
                m_intThreads = intThreads;

                double[] mrseArr;
                List<List<double>> allSamplesY;
                List<List<double[]>> allSamplesX;
                double[] mean;
                double[] stdDev;
                TrainerWrapperHelper.PrepareData(
                    xData,
                    yData,
                    intNumClasses,
                    blnVerbose,
                    intForecastDays,
                    trainerWrapperFactory,
                    out m_trainerWrappers,
                    out mrseArr,
                    out mean,
                    out stdDev,
                    out allSamplesY,
                    out allSamplesX,
                    intThreads);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override double Forecast(double[] xVar)
        {
            try
            {
                var forecasts = new double[m_trainerWrappers.Length];
                for (int i = 0; i < m_trainerWrappers.Length; i++)
                {
                    forecasts[i] = m_trainerWrappers[i].Forecast(xVar);
                }
                if(m_intNumClasses> 1)
                {
                    return TimeSeriesHelper.GetMostVotedClass(
                        forecasts,
                        m_intNumClasses);
                }

                return forecasts.Average();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}
