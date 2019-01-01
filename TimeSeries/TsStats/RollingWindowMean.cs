#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class RollingWindowMean
    {
        private readonly object m_lockObject = new object();

        #region Properties

        public double Mean
        {
            get { return CurrCounter == 0 ?  SumOfValues : SumOfValues/CurrCounter; }
            set { }
        }

        public double LastValue { get; set; }

        public List<TsRow2D> Data { get; set; }

        public double SumOfValues { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public int WindowSize { get; set; }
        public double CurrCounter { get; set; }

        public double Max
        {
            get
            {
                if (Data == null || Data.Count == 0)
                {
                    return 0;
                }
                return (from n in Data select n.Fx).Max();
            }
            set { }
        }

        public double Min
        {
            get {
                if (Data == null || Data.Count == 0)
                {
                    return 0;
                }
                    return (from n in Data select n.Fx).Min(); }
            set { }
        }

        public int Count
        {
            get { return Data.Count; }
        }

        #endregion

        #region Constructors
        
        public RollingWindowMean() : this(int.MaxValue) { }

        public RollingWindowMean(
            int intWindowSize)
        {
            //if (intWindowSize <= 2)
            //{
            //    throw new Exception("Invalid window size: " +
            //                        intWindowSize);
            //}
            WindowSize = intWindowSize;
            Data = new List<TsRow2D>();
        }

        #endregion

        #region Public

        public bool IsReady()
        {
            return Data.Count >= WindowSize;
        }

        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            lock (m_lockObject)
            {
                if (LastUpdateTime > dateTime &&
                    dateTime != DateTime.MinValue)
                {
                    throw new Exception("Invalid time");
                }

                if (double.IsNaN(dblValue) ||
                    double.IsInfinity(dblValue))
                {
                    throw new Exception("Invalid value");
                }

                LastValue = dblValue;

                Data.Add(
                    new TsRow2D(dateTime, dblValue));

                //
                // add to values
                //
                SumOfValues += dblValue;
                CurrCounter = Data.Count;


                if (CurrCounter > WindowSize)
                {
                    //
                    // remove old values
                    //
                    double dblOldValue = Data[0].Fx;
                    SumOfValues -= dblOldValue;
                    Data.RemoveAt(0);
                    CurrCounter = WindowSize;
                }

                LastUpdateTime = dateTime;
            }
        }


        public static void DoTest()
        {
            var data = new[]
                           {
                               0.3136649,
                               0.340137912,
                               0.66962385,
                               0.693365458,
                               0.747020564,
                               0.075946156,
                               0.639564259,
                               0.086856861,
                               0.522924034,
                               0.574736478,
                               0.668914313,
                               0.841301719,
                               0.431452231,
                               0.823558576,
                               0.530728641,
                               0.625912979,
                               0.617261918,
                               0.247430901,
                               0.706435352,
                               0.444944501,
                               0.073941027,
                               0.462320561,
                               0.038049327,
                               0.544860359,
                               0.001164442,
                               0.040927214,
                               0.959685337,
                               0.506014396,
                               0.854543686,
                               0.724056741,
                               0.651440546,
                               0.019357499,
                               0.835840975,
                               0.366760571,
                               0.370995047,
                               0.279957782,
                               0.257752914,
                               0.010635459,
                               0.465879657,
                               0.487012252,
                               0.283204804,
                               0.483345893,
                               0.732101943,
                               0.024790537,
                               0.795215301,
                               0.924795424,
                               0.683081081,
                               0.316911604,
                               0.1243507,
                               0.983052084,
                               0.284739058,
                               0.968872378,
                               0.434701251,
                               0.272833844,
                               0.070588556,
                               0.748747599,
                               0.914695343,
                               0.661913272,
                               0.860957181,
                               0.667487045
                           };


            const int intWindowSize = 10;
            var incrBasicRollingWindow =
                new RollingWindowStdDev(intWindowSize);


            for (int i = 0; i < data.Length; i++)
            {
                incrBasicRollingWindow.Update(
                    DateTime.MinValue,
                    data[i]);


                if (i >= intWindowSize - 1)
                {
                    Console.WriteLine(
                        @"StDevValue = " +
                        incrBasicRollingWindow.StdDev);
                }
                else
                {
                    Console.WriteLine(
                        @"StDevValue = " +
                        incrBasicRollingWindow.StdDev);
                }
            }
        }

        #endregion

        public void Update(double dblCurrFitness)
        {
            Update(DateTime.MinValue, dblCurrFitness);
        }

        public RollingWindowMean Clone()
        {
            var clone = (RollingWindowMean)MemberwiseClone();
            clone.Data = new List<TsRow2D>(Data);
            clone.SumOfValues = SumOfValues;
            clone.CurrCounter = CurrCounter;
            return clone;
        }
    }
}
