using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public abstract class ATrainerWrapper : ITrainerWrapper
    {
        protected const int ENSEMBLE_SIZE = 1;
        protected List<double[]> m_xData;
        protected List<double> m_yData;
        protected int m_intNumClasses;
        protected bool m_blnVerbose;
        protected int m_intThreads = 40;
        protected int m_intEnsembleSize = ENSEMBLE_SIZE;
        private List<double> m_erorrs;
        private readonly object m_errorLockObjects = new object();
        protected List<double> m_forecasts;
        private readonly object m_forecastLock = new object();

        public int Length
        {
            get
            {
                if (m_xData == null ||
                    m_xData.Count == 0)
                {
                    return 0;
                }
                return m_xData.Count;
            }
        }

        public virtual int Dimesions
        {
            get
            {
                if (m_xData == null ||
                    m_xData.Count == 0)
                {
                    return 0;
                }
                return m_xData[0].Length;
            }
        }

        public ATrainerWrapper() { } // used for serialization

        protected ATrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intNumClasses,
            bool blnVerbose = true,
            int intThreads = 40,
            int intEnsembleSize = ENSEMBLE_SIZE)
        {
            if (xData.Count != yData.Count)
            {
                throw new HCException("Invalid data size");
            }

            m_xData = xData;
            m_yData = yData;
            m_intNumClasses = Math.Max(1, intNumClasses);
            m_blnVerbose = blnVerbose;
            m_intThreads = intThreads;
            m_intEnsembleSize = intEnsembleSize;
        }

        protected ATrainerWrapper(
            double[] data,
            int intNumClasses)
        {
            GetXyData(data,
                out m_xData,
                out m_yData);
            m_intNumClasses = intNumClasses;
        }

        public List<double[]> GetxData()
        {
            return m_xData;
        }


        private static void GetXyData(
            double[] data,
            out List<double[]> xData,
            out List<double> yData)
        {
            xData = null;
            yData = null;
            try
            {
                xData = new List<double[]>();
                yData = new List<double>();
                for (int i = 0; i < data.Length; i++)
                {
                    double dblVal = data[i];
                    yData.Add(dblVal);
                    xData.Add(new double[] { i + 1 });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }



        public List<double> GetForecasts()
        {
            try
            {
                if (m_forecasts == null)
                {
                    lock (m_forecastLock)
                    {
                        if (m_forecasts == null)
                        {
                            m_forecasts = GenerateForecasts(m_xData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return m_forecasts;
        }

        protected List<double> GenerateForecasts(
            List<double[]> xData)
        {
            try
            {
                var forecasts = new List<double>(xData.Count + 2);
                foreach (var featureToForecast in xData)
                {
                    double dblPrediction = Forecast(featureToForecast);
                    if(double.IsNaN(dblPrediction))
                    {
                        throw new HCException("NaN prediction");
                    }
                    forecasts.Add(dblPrediction);
                }
                return forecasts;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        public double Forecast(double dblX)
        {
            return Forecast(new[] { dblX });
        }

        public abstract double Forecast(double[] xVar);

        public List<double> GetErrors()
        {
            try
            {
                if (m_erorrs == null)
                {
                    lock (m_errorLockObjects)
                    {
                        if (m_erorrs == null)
                        {
                            GetForecasts();
                            m_erorrs = GenerateErrors(
                                m_xData,
                                m_yData,
                                m_forecasts);
                        }
                    }
                }
                return m_erorrs;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return new List<double>();
        }

        protected List<double> GenerateErrors(
            List<double[]> xData,
            List<double> yData,
            List<double> forecasts)
        {
            try
            {
                var erorrs = new List<double>(xData.Count + 2);
                for (int i = 0; i < xData.Count; i++)
                {
                    double dblYForecast = forecasts[i];
                    double dblY = yData[i];
                    double dblError;
                    if (m_intNumClasses <= 1)
                    {
                        dblError = dblYForecast - dblY;
                    }
                    else
                    {
                        dblError = dblYForecast == dblY ? 0 : 1;
                    }
                    erorrs.Add(dblError);
                }
                return erorrs;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }


        public double GetMrse(
            List<double[]> x,
            List<double> y,
            out List<double> foreasts,
            out List<double> errors)
        {
            foreasts = new List<double>();
            errors = new List<double>();
            try
            {
                foreasts = GenerateForecasts(
                    x);
                errors = GenerateErrors(
                    x,
                    y,
                    foreasts);

                return TimeSeriesHelper.GetMrse(errors);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public double GetMrse()
        {
            List<double> errors;
            return GetMrse(
                out errors);
        }

        public double GetMrse(
            out List<double> errors)
        {
            errors = null;
            try
            {
                errors = GetErrors();
                return TimeSeriesHelper.GetMrse(errors);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public double GetCoeffDeterm()
        {
            try
            {
                return TimeSeriesHelper.GetCoeffDeterm(
                    m_yData,
                    GetErrors());
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public double GetPrcCorrectlyClassified()
        {
            return GetPrcCorrectlyClassified(
                m_xData,
                m_yData);
        }

        public double GetPrcCorrectlyClassified(
            List<double[]> xData,
            List<double> yData)
        {
            try
            {
                int intCorrect = 0;
                for (int i = 0; i < xData.Count; i++)
                {
                    double[] xRow = xData[i];
                    double dblYForecast = Forecast(xRow);
                    if (dblYForecast == yData[i])
                    {
                        intCorrect++;
                    }
                }
                return intCorrect * 1.0 / yData.Count;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public double GetAdjustedCoeffDeterm()
        {
            return TimeSeriesHelper.GetAdjustedCoeffDeterm(
                m_yData,
                GetErrors(),
                Dimesions);
        }

        public double GetRss()
        {
            double dblRss = (from n in GetErrors()
                             select Math.Pow(n, 2)).Sum();
            return dblRss;
        }

        public void Dispose()
        {
            m_xData = null;
            m_yData = null;
            m_erorrs = null;
            m_forecasts = null;
        }

    }
}