#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowOutliers
    {
        #region Members

        private readonly RollingWindowRegression m_rollingWindowRegression;
        private TsRow2D m_lastRow;
        private TsRow2D m_lastDeriv;
        private bool m_blnOutlayerMode;
        private readonly double m_dblThreshold;

        #endregion

        #region Constructors

        public RollingWindowOutliers(
            double dblThreshold)
            : this(
                Outliers.DEFAULT_SAMPLE_SIZE,
                dblThreshold) { }

        public RollingWindowOutliers() : this(
            Outliers.DEFAULT_SAMPLE_SIZE,
            Outliers.DEFAULT_OUTLIER_THRESHOLD){}

        public RollingWindowOutliers(
            int intSampleSize,
            double dblThreshold)
        {
            try
            {
                m_dblThreshold = dblThreshold;
                if (intSampleSize < Outliers.DEFAULT_SAMPLE_SIZE)
                {
                    throw new HCException("Invalid sample size");
                }
                m_rollingWindowRegression = new RollingWindowRegression(intSampleSize);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Public

        public void Update(
            DateTime dateTime, 
            double dblValue)
        {
            try
            {
                if (m_lastRow != null)
                {
                    m_lastDeriv = new TsRow2D
                                      {
                                          Time = dateTime,
                                          Fx = dblValue - m_lastRow.Fx
                                      };
                }
                m_lastRow = new TsRow2D
                                {
                                    Time = dateTime,
                                    Fx = dblValue
                                };
                m_rollingWindowRegression.Update(dateTime, dblValue);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }

        }


        public TsRow2D GetLastCleanSample()
        {
            try
            {
                if (!m_rollingWindowRegression.IsReady())
                {
                    m_blnOutlayerMode = false;
                    return m_lastRow;
                }

                List<double> testData = m_rollingWindowRegression.YList;
                TsRow2D outlier = GetOutlier(m_lastRow, testData);
                if (outlier == null)
                {
                    m_blnOutlayerMode = false;
                    return m_lastRow;
                }
                List<double> testDataDeriv = TimeSeriesHelper.GetDerivative(1, testData);
                TsRow2D outlierDeriv = GetOutlier(m_lastDeriv, testDataDeriv);
                if (outlierDeriv != null ||
                    m_blnOutlayerMode)
                {
                    double dblPrediction = m_rollingWindowRegression.Predict(
                        m_rollingWindowRegression.XList.Last() + 1);
                    m_blnOutlayerMode = true;

                    return
                        new TsRow2D
                            {
                                Time = m_lastRow.Time,
                                Fx = dblPrediction
                            };
                }
                m_blnOutlayerMode = false;
                return m_lastRow;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }
        
        #endregion

        #region Private

        private TsRow2D GetOutlier(
            TsRow2D currRow,
            List<double> testData)
        {
            try
            {
                if (m_rollingWindowRegression.IsReady())
                {
                    double dblMedian = Median.GetMedian(testData.ToArray());
                    List<double> r = (from n in testData select Math.Abs(n - dblMedian)).ToList();
                    double dblMedianR = Median.GetMedian(r.ToArray());
                    double dblR = Math.Abs(currRow.Fx - dblMedian);
                    if (dblR >= m_dblThreshold*dblMedianR)
                    {
                        return currRow;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        #endregion

    }
}