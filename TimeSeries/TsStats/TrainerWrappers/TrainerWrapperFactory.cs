using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public class TrainerWrapperFactory
    {
        public EnumTrainerWrapper EnumTrainerWrapper { get; private set; }

        private readonly Dictionary<string, object> m_paramsMap;

        public TrainerWrapperFactory(
            EnumTrainerWrapper enumTrainerWrapper,
            Dictionary<string, object> paramsMap = null)
        {
            m_paramsMap = paramsMap;
            EnumTrainerWrapper = enumTrainerWrapper;
        }

        public ITrainerWrapper Build(
                List<double> xData,
                List<double> yData,
                int intClasses = 0,
                bool blnVerbose = true)
        {
            object objNumClasses;
            if (m_paramsMap != null &&
                m_paramsMap.TryGetValue(
                    "NumClasses",
                    out objNumClasses))
            {
                intClasses = (int)objNumClasses;
            }
            return Build(
                (from n in xData select new[] {n}).ToList(),
                yData,
                intClasses,
                blnVerbose);
        }

        public ITrainerWrapper Build(
                List<double[]> xData,
                List<double> yData,
                int intClasses = 0,
                bool blnVerbose = true)
        {
            try
            {
                object objNumClasses;
                if (m_paramsMap != null &&
                    m_paramsMap.TryGetValue(
                        "NumClasses",
                        out objNumClasses))
                {
                    intClasses = (int)objNumClasses;
                }

                if (EnumTrainerWrapper == EnumTrainerWrapper.Ensemble)
                {
                    object innerObj;
                    if (m_paramsMap != null &&
                        m_paramsMap.TryGetValue("InnerWrapperEnum", out innerObj) &&
                        innerObj != null)
                    {
                        var enumTrainerWrapper = (EnumTrainerWrapper) innerObj;
                        Dictionary<string, object> paramsMap = null;
                        if (m_paramsMap.TryGetValue("InnerParams", out innerObj))
                        {
                            paramsMap = (Dictionary<string, object>) innerObj;
                        }
                        var intForecastDays = (int) m_paramsMap["ForecastDays"];
                        var innerTrainerWrapperFactory = new TrainerWrapperFactory(
                            enumTrainerWrapper,
                            paramsMap);

                        return new GenericEnsembleTainerWrapper(
                            intForecastDays,
                            xData,
                            yData,
                            innerTrainerWrapperFactory,
                            intClasses,
                            blnVerbose);
                    }
                    throw new HCException("Ensemble params not found");
                }

                if (EnumTrainerWrapper == EnumTrainerWrapper.Nnet)
                {
                    double dblSampleSizeFactor = 1.0;
                    object obj;
                    if (m_paramsMap != null &&
                        m_paramsMap.TryGetValue("SampleSizeFactor", out obj) &&
                        obj is double)
                    {
                        dblSampleSizeFactor = (double)obj;
                    }

                    return new NnetTrainerWrapper(
                        xData,
                        yData,
                        intClasses,
                        true,
                        dblSampleSizeFactor: dblSampleSizeFactor);
                }
                if (EnumTrainerWrapper == EnumTrainerWrapper.Svm)
                {
                    return new SvmTrainerWrapper(
                        xData,
                        yData,
                        intClasses);
                }
                if (EnumTrainerWrapper == EnumTrainerWrapper.Rdf)
                {
                    return new RdfTrainerWrapper(
                        xData,
                        yData,
                        intClasses);
                }
                if (EnumTrainerWrapper == EnumTrainerWrapper.MnLogit)
                {
                    return new MnLogitTrainerWrapper(
                        xData,
                        yData,
                        intClasses);
                }
                if (EnumTrainerWrapper == EnumTrainerWrapper.LinearRegression)
                {
                    if (intClasses > 1)
                    {
                        throw new HCException("Multiple classes not supported for regression");
                    }
                    return new RegressionTrainerWrapper(
                        xData,
                        yData);
                }
                throw new HCException("not implemented");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }
    }
}
