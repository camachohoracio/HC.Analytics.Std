using System;
using System.Collections.Generic;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public class ClusterEnsembleTrainerWrapper : ATrainerWrapper
    {
        private List<ATrainerWrapper> m_trainerWrapper;

        private int m_intWrappers;

        public ClusterEnsembleTrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intNumClasses) :
            base(xData,
            yData,
            intNumClasses,
            true)
        {
            DoTraining();
        }

        private void DoTraining()
        {
            try
            {
                m_trainerWrapper = new List<ATrainerWrapper>
                                   {
                                       new MnLogitTrainerWrapper(
                                           m_xData,
                                           m_yData,
                                           m_intNumClasses),
                                       new NnetTrainerWrapper(
                                           m_xData,
                                           m_yData,
                                           intNumClasses: m_intNumClasses),
                                       new RdfTrainerWrapper(
                                           m_xData,
                                           m_yData,
                                           m_intNumClasses),
                                       //new SvmTrainerWrapper(
                                       //    m_xData,
                                       //    m_yData,
                                       //    m_intNumClasses,
                                       //    m_blnVerbose)
                                   };
                m_intWrappers = m_trainerWrapper.Count;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public SortedDictionary<string, int> ForecastByPredictor(double[] xVar)
        {
            try
            {
                var forecastByPredictor = new SortedDictionary<string, int>();
                for (int i = 0; i < m_intWrappers; i++)
                {
                    var intClass=(int) m_trainerWrapper[i].Forecast(
                        xVar);
                    if (intClass >= m_intNumClasses || 
                        intClass < 0)
                    {
                        throw new HCException("invalid class number");
                    }
                    forecastByPredictor[
                        m_trainerWrapper[i].GetType().Name] = 
                       intClass;
                }
                return forecastByPredictor;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new SortedDictionary<string, int>();
        }

        public override double Forecast(double[] xVar)
        {
            try
            {
                var votes = new int[m_intWrappers];
                for (int i = 0; i < m_intWrappers; i++)
                {
                    votes[i] = (int) m_trainerWrapper[i].Forecast(
                        xVar);
                }
                return TimeSeriesHelper.GetMostVotedClass(votes);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
        }
    }
}
