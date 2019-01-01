using System;
using System.Collections.Generic;
using HC.Core.Logging;

namespace HC.Analytics.Statistics
{
    public class WintersForecastingOptimiseWorker : IDisposable
    {
        private readonly int m_intPeriodSizeL;
        private readonly bool m_blnTrainWithOutOfSample;
        private List<double> m_outOfSampleData;
        private List<double> m_validationData;
        private double[] m_dblAvgI;
        private readonly double m_dblS0;
        private readonly double m_dblB0;

        public WintersForecastingOptimiseWorker(
            int intPeriodSizeL,
            bool blnTrainWithOutOfSample,
            List<double> outOfSampleData,
            List<double> validationData,
            double[] dblAvgI,
            double dblS0,
            double dblB0)
        {
            m_intPeriodSizeL = intPeriodSizeL;
            m_blnTrainWithOutOfSample = blnTrainWithOutOfSample;
            m_outOfSampleData = outOfSampleData;
            m_validationData = validationData;
            m_dblAvgI = dblAvgI;
            m_dblS0 = dblS0;
            m_dblB0 = dblB0;
        }

        public double[] DoOptimise()
        {
            try
            {
                //
                // lower bound model parameters
                //
                alglib.minlmstate state;
                const double dblAlpha = 0.9999999;
                const double dblBeta = 0.0131445541273582;
                const double dblGamma = 0.1;
                var x = new[]
                            {
                                dblAlpha,
                                dblBeta,
                                dblGamma
                            };

                alglib.minlmcreatev(
                    1, // number o functions
                    x,  // initial solution
                    0.0005, // differentiation step
                    out state);
                alglib.minlmsetbc(state,
                                  new[] {0.00001, 0.00001, 0.00001},
                                  new[] {0.99999, 0.99999, 0.99999});
                const double dblEpsg = 1e-6; // stopping condition
                const double dblEpsf = 0; // stopping condition
                const double dblEpsx = 0; // stopping conditions
                const int intMaxIts = 100;
                List<double> outOfSampleDataDummy = m_outOfSampleData;
                alglib.minlmsetcond(state, dblEpsg, dblEpsf, dblEpsx, intMaxIts);
                alglib.minlmoptimize(state,
                                     (arg, fi, obj) => // arg are the variables, fi is the objective
                                         {
                                             if (m_blnTrainWithOutOfSample)
                                             {
                                                 FunctionOutOfSample(
                                                     arg,
                                                     outOfSampleDataDummy,
                                                     fi);
                                             }
                                             else
                                             {
                                                 Function(arg,
                                                          fi);
                                             }
                                         },
                                     null,
                                     null);
                alglib.minlmreport rep;
                alglib.minlmresults(state, out x, out rep);
                return x;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }

            return new double[] {};
        }

        public void FunctionOutOfSample(
            double[] x,
            List<double> outOfSampleData,
            double[] fi)
        {

            try
            {
                double dblAlpha = x[0];
                double dblBeta = x[1];
                double dblGamma = x[2];

                List<double> ltList;
                double dblBPrev;
                double dblSPrev;
                List<double> inSampleForecasts;


                WintersForecasting.GetEcm(
                    m_intPeriodSizeL,
                    dblAlpha,
                    m_dblB0,
                    m_dblS0,
                    dblGamma,
                    dblBeta,
                    m_dblAvgI,
                    m_validationData,
                    out ltList,
                    out dblBPrev,
                    out dblSPrev,
                    out inSampleForecasts);

                double dblEcm = 0;
                for (int j = 0; j < m_intPeriodSizeL; j++)
                {
                    double dblCurrForecast = ((dblSPrev + (dblBPrev * (j + 1)))) * ltList[j];
                    dblEcm += Math.Pow(outOfSampleData[j] - dblCurrForecast, 2);
                }
                fi[0] = dblEcm;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Function(
            double[] arg,
            double[] fi)
        {
            try
            {
                double dblAlpha = arg[0];
                double dblBeta = arg[1];
                double dblGamma = arg[2];
                List<double> ltList;
                double dblBPrev;
                double dblSPrev;
                List<double> inSampleForecasts;
                double dblEcm = WintersForecasting.GetEcm(
                    m_intPeriodSizeL,
                    dblAlpha,
                    m_dblB0,
                    m_dblS0,
                    dblGamma,
                    dblBeta,
                    m_dblAvgI,
                    m_validationData,
                    out ltList,
                    out dblBPrev,
                    out dblSPrev,
                    out inSampleForecasts);
                fi[0] = dblEcm;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Dispose()
        {
                m_outOfSampleData = null;
                m_validationData = null;
                m_dblAvgI = null;
        }
    }
}