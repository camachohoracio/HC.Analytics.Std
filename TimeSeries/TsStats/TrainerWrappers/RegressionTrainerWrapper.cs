#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics.Regression;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    [Serializable]
    public class RegressionTrainerWrapper : ATrainerWrapper
    {
        #region Properties

        public double[] Weights { get; set; } // set kept public for serializations

        #endregion

        #region Constructors

        public RegressionTrainerWrapper() { } // used for serialization

        public RegressionTrainerWrapper(
            List<double> xData,
            List<double> yData)
            : this(
                (from n in xData select new[] { n }).ToList(),
                yData)
        {
        }

        public RegressionTrainerWrapper(
            List<double[]> xData,
            List<double> yData)
        {
            m_xData = xData;
            m_yData = yData;
            m_intNumClasses = 1;
            DoRegression();
        }

        public RegressionTrainerWrapper(double[] data)
        {
            GetXyData(data);
            m_intNumClasses = 1;
            DoRegression();
        }

        #endregion

        private void GetXyData(double[] data)
        {
            m_xData = new List<double[]>();
            m_yData = new List<double>();
            for (int i = 0; i < data.Length; i++)
            {
                double dblVal = data[i];
                m_yData.Add(dblVal);
                m_xData.Add(new double[] { i + 1 });
            }
        }

        private void DoRegression()
        {
            try
            {
                if (m_xData[0].Length >= m_xData.Count - 1)
                {
                    Logger.Log(new HCException("Number of variables [" + m_xData[0].Length +
                                               "] are too large in comparison to num of samples [" + m_xData.Count + "]"));
                    return;
                }

                alglib.linreg.lrreport lrreport;
                Weights = RegressionHelperAlgLib.GetRegressionWeights(
                    m_xData,
                    m_yData,
                    out lrreport);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override double Forecast(double[] dblX)
        {
            try
            {
                if (Weights == null)
                {
                    return double.NaN;
                }

                double dblIntercept = 0;
                for (int i = 0; i < Weights.Length - 1; i++)
                {
                    dblIntercept += Weights[i] * dblX[i];
                }

                double dblYPrediction = dblIntercept + Weights[Weights.Length - 1];
                return dblYPrediction;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }
    }
}
