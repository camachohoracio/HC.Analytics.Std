#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace HC.Analytics.Statistics.Regression
{



    public class RegressionAlgLibWrapper
    {
        #region Properties

        public double[] Weights { get; private set; }

        #endregion

        #region Members

        private readonly object m_errorLockObjects = new object();
        private List<double> m_erorrs;
        private List<double[]> m_xData;
        private List<double> m_yData;

        #endregion

        #region Constructors

        public RegressionAlgLibWrapper(
            List<double> xData,
            List<double> yData)
            : this(
                (from n in xData select new[] { n }).ToList(),
                yData)
        {
        }

        public RegressionAlgLibWrapper(
            List<double[]> xData,
            List<double> yData)
        {
            m_xData = xData;
            m_yData = yData;
            DoRegression();
        }

        public RegressionAlgLibWrapper(double[] data)
        {
            GetXyData(data);
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
            alglib.linreg.lrreport lrreport;
            Weights = RegressionHelperAlgLib.GetRegressionWeights(
                m_xData,
                m_yData,
                //true,
                out lrreport);
        }

        public double Forecast(double[] dblX)
        {
            double dblIntercept = 0;
            for (int i = 0; i < Weights.Length - 1; i++)
            {
                dblIntercept += Weights[i] * dblX[i];
            }

            double dblYPrediction = dblIntercept + Weights[1];
            return dblYPrediction;
        }

        public List<double> GetErrors()
        {
            if (m_erorrs == null)
            {
                lock (m_errorLockObjects)
                {
                    if (m_erorrs == null)
                    {
                        m_erorrs = new List<double>();
                        for (int i = 0; i < m_xData.Count; i++)
                        {
                            double[] xRow = m_xData[i];
                            double dblYForecast = Forecast(xRow);
                            double dblY = m_yData[i];
                            double dblError = dblYForecast - dblY;
                            m_erorrs.Add(dblError);
                        }
                    }
                }
            }
            return m_erorrs;
        }
    }
}
