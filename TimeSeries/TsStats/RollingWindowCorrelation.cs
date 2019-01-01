#region

using System;
using System.Collections.Generic;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class RollingWindowCorrelation : IDisposable
    {
        #region Properties

        public List<double> XyData { get; set; }
        public RollingWindowStdDev RollingWindowStdDevX { get; set; }
        public RollingWindowStdDev RollingWindowStdDevY { get; set; }
        public int WindowSize { get; set; }
        public double CurrCounter { get; set; }
        public double SumOfXyValues { get; set; }

        public double Covariace
        {
            get
            {
                if(WindowSize <= 1)
                {
                    return 0;
                }
                double dblN = XyData.Count;
                return (SumOfXyValues -
                        ((RollingWindowStdDevX.SumOfValues*RollingWindowStdDevY.SumOfValues)/dblN))
                       /dblN;
            }
            set { }
        }

        public double Correlation
        {
            get
            {
                try
                {
                    double dblDenominator = RollingWindowStdDevX.StdDev*RollingWindowStdDevY.StdDev;
                    if (dblDenominator <= 1e-20 || WindowSize <= 1)
                    {
                        return 0;
                    }
                    double dblCorrelation = Covariace/dblDenominator;
                    dblCorrelation = dblCorrelation > 1.0 ? 1.0 : dblCorrelation;

                    if (double.IsNaN(dblCorrelation) ||
                        double.IsInfinity(dblCorrelation))
                    {
                        throw new HCException("Invalid correlation");
                    }

                    return dblCorrelation;
                }
                catch(Exception ex)
                {
                    Logger.Log(ex);
                }
                return 0;
            }
            set { }
        }

        #endregion

        #region Constructors

        public RollingWindowCorrelation() {}

        public RollingWindowCorrelation(int intWindowSize)
        {
            WindowSize = intWindowSize;
            XyData = new List<double>();
            RollingWindowStdDevX = new RollingWindowStdDev(intWindowSize);
            RollingWindowStdDevY = new RollingWindowStdDev(intWindowSize);
        }

        #endregion

        #region Public

        public void Update(
            DateTime dateTime,
            double dblXValue,
            double dblYValue)
        {
            RollingWindowStdDevX.Update(dateTime, dblXValue);
            RollingWindowStdDevY.Update(dateTime, dblYValue);
            double dblXy = dblXValue*dblYValue;
            XyData.Add(dblXy);

            //
            // add to values
            //
            SumOfXyValues += dblXy;
            CurrCounter = XyData.Count;


            if (CurrCounter > WindowSize)
            {
                //
                // remove old values
                //
                double dblOldValue = XyData[0];
                SumOfXyValues -= dblOldValue;
                XyData.RemoveAt(0);
                CurrCounter = WindowSize;
            }
        }


        public static void DoTest()
        {
            var data = new[]
                           {
                               "0.3136649,0.28324801",
                               "0.340137912,-0.145723862",
                               "0.66962385,0.659831924",
                               "0.693365458,-0.109347361",
                               "0.747020564,0.195801654",
                               "0.075946156,-0.025120657",
                               "0.639564259,0.104621617",
                               "0.086856861,-0.334022839",
                               "0.522924034,-0.064767627",
                               "0.574736478,0.079225877",
                               "0.668914313,0.332941748",
                               "0.841301719,0.831236719",
                               "0.431452231,0.25020422",
                               "0.823558576,0.745297408",
                               "0.530728641,0.528819975",
                               "0.625912979,0.394137191",
                               "0.617261918,0.569021397",
                               "0.247430901,0.060012718",
                               "0.706435352,0.662305869",
                               "0.444944501,0.423973279",
                               "0.073941027,-0.311833571",
                               "0.462320561,0.402158447",
                               "0.038049327,-0.653059598",
                               "0.544860359,0.410987613",
                               "0.001164442,-0.154476072",
                               "0.040927214,-0.092747685",
                               "0.959685337,0.80056919",
                               "0.506014396,0.18874122",
                               "0.854543686,0.560334214",
                               "0.724056741,0.067913693",
                               "0.651440546,0.094241312",
                               "0.019357499,-0.000498278",
                               "0.835840975,0.829028083",
                               "0.366760571,0.095300898",
                               "0.370995047,0.00235938",
                               "0.279957782,-0.010939002",
                               "0.257752914,0.208052294",
                               "0.010635459,-0.014210786",
                               "0.465879657,0.159604774",
                               "0.487012252,0.39626056",
                               "0.283204804,0.085076562",
                               "0.483345893,-0.335915524",
                               "0.732101943,0.671437279",
                               "0.024790537,0.024226406",
                               "0.795215301,-0.060369621",
                               "0.924795424,0.893012652",
                               "0.683081081,-0.100170312",
                               "0.316911604,0.031795052",
                               "0.1243507,0.106458163",
                               "0.983052084,0.918772877",
                               "0.284739058,0.260640274",
                               "0.968872378,0.967549819",
                               "0.434701251,0.091557048",
                               "0.272833844,-0.38344811",
                               "0.070588556,0.03486575",
                               "0.748747599,0.688376789",
                               "0.914695343,0.126961163",
                               "0.661913272,0.234552566",
                               "0.860957181,0.749456206",
                               "0.667487045,0.391442492",
                           };


            const int intWindowSize = 10;
            var rollingWindowCorrelation =
                new RollingWindowCorrelation(intWindowSize);


            for (int i = 0; i < data.Length; i++)
            {
                string[] toks = data[i].Split(',');

                rollingWindowCorrelation.Update(
                    DateTime.MinValue,
                    double.Parse(toks[0]),
                    double.Parse(toks[1]));


                if (i >= intWindowSize - 1)
                {
                    Console.WriteLine(
                        @"Corr = " +
                        rollingWindowCorrelation.Correlation);
                }
            }
        }

        #endregion

        public bool IsReady()
        {
            return RollingWindowStdDevX.IsReady() &&
                   RollingWindowStdDevY.IsReady();
        }

        public void Dispose()
        {
            try
            {
                if (XyData != null)
                {
                    XyData.Clear();
                    XyData = null;
                }

                if (RollingWindowStdDevX != null)
                {
                    RollingWindowStdDevX.Dispose();
                    RollingWindowStdDevX = null;
                }
                if (RollingWindowStdDevY != null)
                {
                    RollingWindowStdDevY.Dispose();
                    RollingWindowStdDevY = null;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
