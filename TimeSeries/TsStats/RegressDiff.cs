#region

using System;
using HC.Core;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class RegressDiff : IDisposable
    {
        #region Delegates

        public delegate void CrossingDel(object context, RegressDiff smaDiff);

        #endregion

        public event CrossingDel OnCrossing;

        public void DoTest()
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


            //const int intWindowSize = 10;
            RegressDiff regressDiff = this;


            for (int i = 0; i < data.Length; i++)
            {
                string[] toks = data[i].Split(',');
                double dblCurrValue = Math.Log(double.Parse(toks[0])/
                                               double.Parse(toks[1]));
                regressDiff.Update(dblCurrValue);

                if (regressDiff.IsInitialized())
                {
                    double dblPrediciton1;
                    double dblPrediction2;
                    GetLastPrediction(
                        out dblPrediciton1,
                        out dblPrediction2);
                    Console.WriteLine(dblCurrValue + "," +
                                      dblPrediciton1 + "," +
                                      dblPrediction2);
                }
            }
        }

        #region Properties

        public double CurrDiff { get; set; }
        public RollingWindowRegression Regression1 { get; set; }
        public RollingWindowRegression Regression2 { get; set; }

        public bool IsGoingDown
        {
            get
            {
                RollingWindowRegression regressionSlow;
                RollingWindowRegression regressionFast;

                if (TimeWindow1 > TimeWindow2)
                {
                    regressionSlow = Regression1;
                    regressionFast = Regression2;
                }
                else
                {
                    regressionSlow = Regression2;
                    regressionFast = Regression1;
                }
                double dblPredictionSlow;
                double dblPredictionFast;
                GetLastPrediction(
                    out dblPredictionSlow,
                    out dblPredictionFast,
                    regressionSlow,
                    regressionFast);

                return dblPredictionSlow > dblPredictionFast;
            }
        }
        public DateTime LastUpdateTime { get; set; }
        public double LastUpdateValue { get; set; }
        public int Counter { get; private set; }
        public int TimeWindow1 { get; private set; }
        public int TimeWindow2 { get; private set; }

        #endregion

        #region Members

        [NonSerialized]
        private readonly object m_context;
        private double m_dblDiff;

        #endregion

        #region Constructors

        public RegressDiff(
            int timeWindow1,
            int timeWindow2,
            object context)
        {
            if (timeWindow1 > timeWindow2)
            {
                int intTmpValue = timeWindow2;
                timeWindow2 = timeWindow1;
                timeWindow1 = intTmpValue;
            }
            TimeWindow1 = timeWindow1;
            TimeWindow2 = timeWindow2;
            Regression1 = new RollingWindowRegression(timeWindow1);
            Regression2 = new RollingWindowRegression(timeWindow2);
            m_dblDiff = double.NaN;
            m_context = context;
        }

        #endregion

        #region Public

        public bool IsInitialized()
        {
            return Regression1.IsReady() && Regression2.IsReady();
        }

        public void Update(
            double dblValue)
        {
            Update(DateTime.MinValue, dblValue);
        }

        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            if (double.IsNaN(dblValue) ||
                double.IsInfinity(dblValue))
            {
                throw new Exception("Invalid value");
            }

            if(LastUpdateTime >= dateTime &&
                dateTime != DateTime.MinValue)
            {
                throw new Exception("Invalid time");
            }

        Counter++;
            LastUpdateValue = dblValue;
            LastUpdateTime = dateTime;
            Regression1.Update(
                Counter,
                dblValue,
                dateTime);
            Regression2.Update(
                Counter,
                dblValue,
                dateTime);

            if (!Regression1.IsReady() &&
                !Regression2.IsReady())
            {
                return;
            }

            //
            // check cross
            //
            double dblPrediciton1;
            double dblPrediction2;
            GetLastPrediction(
                out dblPrediciton1,
                out dblPrediction2);
            if (double.IsNaN(m_dblDiff))
            {
                m_dblDiff = dblPrediciton1 - dblPrediction2;
            }

            CurrDiff = dblPrediciton1 - dblPrediction2;

            if (double.IsNaN(m_dblDiff) ||
                double.IsNaN(CurrDiff))
            {
                throw new Exception("Invalid diff value");
            }

            if (Math.Sign(CurrDiff) != Math.Sign(m_dblDiff))
            {
                //
                // the two functions have crossed
                //
                m_dblDiff = CurrDiff;
                CrossingDel h = OnCrossing;
                if (h != null)
                {
                    h(m_context, this);
                }
            }
        }

        public void GetLastPrediction(
            out double dblPrediciton1,
            out double dblPrediction2)
        {
            GetLastPrediction(
                out dblPrediciton1,
                out dblPrediction2,
                Regression1,
                Regression2);
        }

        #endregion

        #region Private

        private void GetLastPrediction(
            out double dblPrediciton1,
            out double dblPrediction2,
            RollingWindowRegression regression1,
            RollingWindowRegression regression2)
        {
            dblPrediciton1 = regression1.Predict(Counter);
            dblPrediction2 = regression2.Predict(Counter);
        }

        #endregion

        ~RegressDiff()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (Regression1 != null)
            {
                Regression1.Dispose();
            }
            if (Regression2 != null)
            {
                Regression2.Dispose();
            }
        }
    }
}
