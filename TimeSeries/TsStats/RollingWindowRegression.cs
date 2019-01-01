#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.TsStats.TrainerWrappers;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class RollingWindowRegression : IDisposable
    {
        #region Properties

        public double LastX { get; private set; }
        public double LastY { get; private set; }
        public double Slope { get; private set; }
        public double Intercept { get; private set; }
        public List<double> XList { get; private set; }
        public List<double> YList { get; private set; }
        public List<TsRow2D> Predictions { get; private set; }
        public int WindowSize { get; private set; }
        public DateTime LastUpdateTime { get; private set; }

        #endregion

        #region Members

        private double m_dblCurrCounter;
        private double m_dblSumOfXValues;
        private double m_dblSumOfXxValues;
        private double m_dblSumOfXyValues;
        private double m_dblSumOfYValues;
        private List<double> m_xxList;
        private List<double> m_xyList;
        private int m_intCounter;

        #endregion

        #region Constructors

        public RollingWindowRegression(int intWindowSize)
        {
            Reset();
            WindowSize = intWindowSize;
        }

        #endregion

        #region Public

        public void Update(double dblX, double dblY)
        {
            Update(dblX, dblY, DateTime.MinValue);
        }

        public void Update(DateTime dateTime, double dblY)
        {
            m_intCounter++;
            Update(m_intCounter, dblY, dateTime);
        }

        public void Update(double dblX, double dblY, DateTime dateTime)
        {
            IsValid(dblX);
            IsValid(dblY);

            AddValues(dblX, dblY);
            m_dblCurrCounter = m_xyList.Count;
            RemoveValues();
            GetRegressionWeights();
            UpdatePredictions(dateTime, dblX);
        }

        private void IsValid(double dblX)
        {
            if (double.IsNaN(dblX) ||
                double.IsInfinity(dblX))
            {
                throw new Exception("Invalid value");
            }
        }

        public double Predict(double dblX)
        {
            double dblPrediction = Slope * dblX + Intercept;
            return dblPrediction;
        }

        public void DoTest()
        {
            string[] data = GetTestData();

            //var data = new[]
            //               {
            //                   "1,1.5",
            //                   "2,1.6",
            //                   "3,2.1",
            //                   "4,3.0",
            //               };

            //const int intWindowSize = 10;
            RollingWindowRegression rollingWindowRegression = this;


            for (int i = 0; i < data.Length; i++)
            {
                string[] toks = data[i].Split(',');

                rollingWindowRegression.Update(
                    double.Parse(toks[0]),
                    double.Parse(toks[1]));


                if (i >= WindowSize - 1)
                {
                    //
                    // compare with regression
                    //
                    var regression = new RegressionTrainerWrapper(
                        rollingWindowRegression.XList,
                        rollingWindowRegression.YList);
                    double[] weights = regression.Weights;
                    if (Math.Abs(rollingWindowRegression.Slope - weights[0]) > 1e10 - 6)
                    {
                        throw new Exception("Invalid regression result");
                    }
                    if (Math.Abs(rollingWindowRegression.Intercept - weights[1]) > 1e10 - 6)
                    {
                        throw new Exception("Invalid regression result");
                    }

                    Console.WriteLine(
                        @"Corr = " +
                        rollingWindowRegression.Slope);
                }
            }
        }

        public static string[] GetTestData()
        {
            var data = new[]
                           {
                               "18.2,20.6425",
                               "18.205,20.6275",
                               "18.1875,20.67",
                               "18.165,20.6675",
                               "18.1675,20.6625",
                               "18.175,20.6575",
                               "18.1525,20.66",
                               "18.1475,20.6225",
                               "18.135,20.6225",
                               "18.1425,20.6225",
                               "18.1375,20.62",
                               "18.1375,20.61",
                               "18.14,20.6225",
                               "18.1575,20.6325",
                               "18.165,20.6425",
                               "18.1525,20.6425",
                               "18.1525,20.65",
                               "18.145,20.66",
                               "18.145,20.66",
                               "18.15,20.6575",
                               "18.1325,20.635",
                               "18.1225,20.63",
                               "18.115,20.605",
                               "18.12,20.605",
                               "18.115,20.595",
                               "18.11,20.58",
                               "18.1225,20.5925",
                               "18.1275,20.6025",
                               "18.1375,20.61",
                               "18.1325,20.6075",
                               "18.1025,20.585",
                               "18.1025,20.5825",
                               "18.1075,20.5825",
                               "18.1225,20.585",
                               "18.0925,20.5725",
                               "18.1,20.5725",
                               "18.11,20.5825",
                               "18.125,20.6075",
                               "18.1525,20.6425",
                               "18.175,20.6575",
                               "18.1725,20.6575",
                               "18.1825,20.66",
                               "18.195,20.67",
                               "18.2,20.68",
                               "18.19,20.6725",
                               "18.195,20.6825",
                               "18.225,20.71",
                               "18.22,20.71",
                               "18.22,20.73",
                               "18.2325,20.725",
                               "18.235,20.725",
                               "18.22,20.7025",
                               "18.2,20.68",
                               "18.2075,20.69",
                               "18.205,20.6875",
                               "18.215,20.7025",
                               "18.21,20.7025",
                               "18.2175,20.7",
                               "18.225,20.7",
                               "18.2075,20.6975",
                               "18.2175,20.6925",
                               "18.1925,20.6775",
                               "18.1925,20.675",
                               "18.1975,20.675",
                               "18.205,20.6925",
                               "18.205,20.695",
                               "18.205,20.695",
                               "18.205,20.695",
                               "18.2025,20.695",
                               "18.205,20.6975",
                               "18.2225,20.71",
                               "18.235,20.7275",
                               "18.24,20.73",
                               "18.2475,20.7325",
                               "18.2325,20.7225",
                               "18.24,20.7225",
                               "18.22,20.71",
                               "18.2175,20.6925",
                               "18.205,20.69",
                               "18.205,20.69",
                               "18.215,20.7025",
                               "18.2025,20.69",
                               "18.205,20.695",
                               "18.2075,20.6825",
                               "18.21,20.695",
                               "18.2025,20.685",
                               "18.2025,20.6825",
                               "18.205,20.685",
                               "18.2175,20.6925",
                               "18.215,20.6925",
                               "18.2175,20.7",
                               "18.2175,20.7",
                               "18.215,20.6975",
                               "18.215,20.695",
                               "18.2125,20.67",
                               "18.2125,20.6675",
                               "18.2175,20.6675",
                               "18.2175,20.655",
                               "18.2125,20.6375",
                               "18.21,20.635",
                           };
            return data;
        }

        public bool IsReady()
        {
            return m_xyList.Count >= WindowSize;
        }

        public double PredictLastValue()
        {
            return Predict(XList.Last());
        }

        #endregion

        #region Private methods

        private void Reset()
        {
            m_xxList = new List<double>();
            m_xyList = new List<double>();
            XList = new List<double>();
            YList = new List<double>();
            Predictions = new List<TsRow2D>();
            m_dblCurrCounter = 0;
            m_dblSumOfXyValues = 0;
            m_dblSumOfXxValues = 0;
            m_dblSumOfXValues = 0;
            m_dblSumOfYValues = 0;
        }

        private void GetRegressionWeights()
        {
            if (m_dblCurrCounter > 1)
            {
                double dblNumerator =
                    (m_dblCurrCounter * m_dblSumOfXyValues) -
                    (m_dblSumOfXValues * m_dblSumOfYValues);
                double dblDenominator =
                    (m_dblCurrCounter * m_dblSumOfXxValues) -
                    Math.Pow(m_dblSumOfXValues, 2);
                Slope =
                    (Math.Abs(dblNumerator) < 1e-6 || Math.Abs(dblDenominator) < 1e-6) ? 0
                    : (dblNumerator / dblDenominator);
                Intercept = (m_dblSumOfYValues - (Slope * m_dblSumOfXValues)) / m_dblCurrCounter;

                //if ((Math.Abs(dblNumerator) < 1e-6 || Math.Abs(dblDenominator) < 1e-6))
                //{
                //    for (int i = 0; i < XList.Count; i++)
                //    {
                //        Console.WriteLine(XList[i] + "," + YList[i]);
                //    }
                //}

                if (double.IsNaN(Intercept) ||
                    double.IsInfinity(Intercept))
                {
                    throw new Exception("Invalid intercept");
                }
            }
            else
            {
                Slope = double.NaN;
                Intercept = double.NaN;
            }
        }

        private void UpdatePredictions(DateTime dateTime, double dblX)
        {
            LastUpdateTime = dateTime;
            double dblPrediciton = Predict(dblX);
            Predictions.Add(
                new TsRow2D(dateTime, dblPrediciton));
            if (Predictions.Count > WindowSize)
            {
                Predictions.RemoveAt(0);
            }
        }

        #region Remove methods

        private void RemoveValues()
        {
            if (m_dblCurrCounter > WindowSize)
            {
                //
                // remove old values
                //
                RemoveXy();
                RemoveXx();
                RemoveX();
                RemoveY();
                m_dblCurrCounter = WindowSize;
            }
        }

        private void RemoveXy()
        {
            double dblOldValue = m_xyList[0];
            m_dblSumOfXyValues -= dblOldValue;
            m_xyList.RemoveAt(0);
        }

        private void RemoveXx()
        {
            double dblOldValue = m_xxList[0];
            m_dblSumOfXxValues -= dblOldValue;
            m_xxList.RemoveAt(0);
        }

        private void RemoveX()
        {
            double dblOldValue = XList[0];
            m_dblSumOfXValues -= dblOldValue;
            XList.RemoveAt(0);
        }

        private void RemoveY()
        {
            double dblOldValue = YList[0];
            m_dblSumOfYValues -= dblOldValue;
            YList.RemoveAt(0);
        }

        #endregion

        #region Add methods

        private void AddValues(double dblX, double dblY)
        {
            LastX = dblX;
            LastY = dblY;
            AddXy(dblX, dblY);
            AddXx(dblX);
            AddX(dblX);
            AddY(dblY);
        }

        private void AddXy(double dblX, double dblY)
        {
            double dblXy = dblX * dblY;
            m_dblSumOfXyValues += dblXy;
            m_xyList.Add(dblXy);
        }

        private void AddXx(double dblX)
        {
            double dblXx = dblX * dblX;
            m_dblSumOfXxValues += dblXx;
            m_xxList.Add(dblXx);
        }

        private void AddX(double dblX)
        {
            m_dblSumOfXValues += dblX;
            XList.Add(dblX);
        }

        private void AddY(double dblY)
        {
            m_dblSumOfYValues += dblY;
            YList.Add(dblY);
        }

        #endregion

        #endregion

        ~RollingWindowRegression()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_xxList != null)
            {
                m_xxList.Clear();
            }
            if (m_xyList != null)
            {
                m_xyList.Clear();
            }
            if (XList != null)
            {
                XList.Clear();
            }
            if (YList != null)
            {
                YList.Clear();
            }
            if (Predictions != null)
            {
                Predictions.Clear();
            }
        }

    }
}
