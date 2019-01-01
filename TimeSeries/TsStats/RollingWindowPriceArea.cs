#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;
using NUnit.Framework;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class RollingWindowPriceArea : IDisposable
    {
        private object m_lockObject = new object();
        private double m_dblFirstValue;
        private DateTime m_lastUpdateTimeVolume;
        private List<double> m_areas =
            new List<double>();
        private List<double> m_data =
            new List<double>();

        private readonly DateTime m_zeroDate = new DateTime();
        public double Area { get; set; }


        #region Properties

        public double LastValue { get; set; }

        //public List<TsRow2D> Data { get; set; }

        //public double SumOfValues { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public int WindowSize { get; set; }
        public double CurrCounter { get; set; }

        public int Count
        {
            get { return m_areas.Count; }
        }

        #endregion

        #region Constructors
        
        public RollingWindowPriceArea() : this(int.MaxValue) { }

        public RollingWindowPriceArea(
            int intWindowSize)
        {
            if (intWindowSize <= 2)
            {
                throw new Exception("Invalid window size: " +
                                    intWindowSize);
            }
            WindowSize = intWindowSize;
            //Data = new List<TsRow2D>();
        }

        #endregion

        #region Public

        public bool IsReady()
        {
            return m_areas.Count >= WindowSize;
        }

        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            try
            {
                lock (m_lockObject)
                {
                    if (LastUpdateTime > dateTime &&
                        dateTime != DateTime.MinValue)
                    {
                        Console.WriteLine("Invalid time");
                        return;
                    }

                    if (double.IsNaN(dblValue) ||
                        double.IsInfinity(dblValue))
                    {
                        throw new Exception("Invalid value");
                    }

                    if (dblValue == 0 ||
                        dateTime == m_zeroDate)
                    {
                        return;
                    }

                    if (m_dblFirstValue == 0 &&
                        m_lastUpdateTimeVolume == m_zeroDate)
                    {
                        m_dblFirstValue = dblValue;
                        m_lastUpdateTimeVolume = dateTime;
                    }
                    else
                    {
                        double dblTimeDelta = (dateTime - m_lastUpdateTimeVolume).TotalMilliseconds;
                        if (dblTimeDelta > 0)
                        {
                            double dblArea;
                            if (LastValue < m_dblFirstValue && dblValue >= m_dblFirstValue)
                            {
                                //
                                // a cross from bellow
                                //
                                double dblDeltaPxCrossing1 = m_dblFirstValue - LastValue;
                                double dblDeltaTimeCrossing1 = dblDeltaPxCrossing1*dblTimeDelta/
                                                               (dblValue - LastValue);

                                double dblArea1 = dblDeltaTimeCrossing1*dblDeltaPxCrossing1/2.0;

                                if (dblArea1 < 0)
                                {
                                    throw new HCException("Invalid area1");
                                }

                                double dblDeltaTimeCrossing2 = dblTimeDelta - dblDeltaTimeCrossing1;
                                double dblDeltaPxCrossing2 = dblValue - m_dblFirstValue;
                                double dblArea2 = dblDeltaTimeCrossing2*dblDeltaPxCrossing2/2.0;

                                if (dblArea2 < 0)
                                {
                                    throw new HCException("Invalid area2");
                                }
                                dblArea = -dblArea1 + dblArea2;
                            }
                            else if (LastValue > m_dblFirstValue && dblValue <= m_dblFirstValue)
                            {
                                //
                                // a cross from above
                                //
                                double dblDeltaPxCrossing1 = LastValue - m_dblFirstValue;
                                double dblDeltaTimeCrossing1 = dblDeltaPxCrossing1*dblTimeDelta/
                                                               (LastValue - dblValue);
                                double dblArea1 = dblDeltaTimeCrossing1*dblDeltaPxCrossing1/2.0;
                                if (dblArea1 < 0)
                                {
                                    throw new HCException("Invalid area1");
                                }

                                double dblDeltaTimeCrossing2 = dblTimeDelta - dblDeltaTimeCrossing1;
                                double dblDeltaPxCrossing2 = m_dblFirstValue - dblValue;
                                double dblArea2 = dblDeltaTimeCrossing2*dblDeltaPxCrossing2/2.0;
                                if (dblArea2 < 0)
                                {
                                    throw new HCException("Invalid area2");
                                }
                                dblArea = dblArea1 - dblArea2;
                            }
                            else if (m_dblFirstValue <= LastValue && m_dblFirstValue <= dblValue)
                            {
                                //
                                // above
                                //
                                double dblArea1 = dblTimeDelta*(LastValue - m_dblFirstValue);
                                if (dblArea1 < 0)
                                {
                                    throw new HCException("Invalid area1");
                                }
                                double dblArea2 = dblTimeDelta*(dblValue - LastValue)/2.0;
                                dblArea = dblArea1 + dblArea2;
                            }
                            else if (m_dblFirstValue >= LastValue && m_dblFirstValue >= dblValue)
                            {
                                //
                                // below
                                //
                                double dblArea1 = dblTimeDelta*(m_dblFirstValue - LastValue);
                                if (dblArea1 < 0)
                                {
                                    throw new HCException("Invalid area1");
                                }
                                double dblArea2 = dblTimeDelta*(LastValue - dblValue)/2.0;
                                dblArea = -dblArea1 - dblArea2;
                            }
                            else
                            {
                                throw new HCException("Case not found");
                            }

                            if (double.IsNaN(dblArea) ||
                                double.IsInfinity(dblArea))
                            {
                                throw new HCException("Invalid area");
                            }

                            m_areas.Add(dblArea);
                            m_data.Add(dblValue);
                            Area += dblArea;
                            m_lastUpdateTimeVolume = dateTime;
                        }
                    }

                    LastValue = dblValue;

                    //
                    // add to values
                    //
                    CurrCounter = m_areas.Count;


                    if (CurrCounter > WindowSize)
                    {
                        //
                        // remove old values
                        //
                        m_dblFirstValue = m_data[0];
                        double dblOldValue = m_areas[0];
                        Area -= dblOldValue;
                        m_areas.RemoveAt(0);
                        m_data.RemoveAt(0);
                        CurrCounter = WindowSize;
                    }

                    LastUpdateTime = dateTime;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        [Test]
        public static void DoTest()
        {

            const int intWindowSize = 10;
            var rw =
                new RollingWindowPriceArea(intWindowSize);
            var date = DateTime.Today;
            rw.Update(date, 4);
            Console.WriteLine(rw.Area);


            date = date.AddSeconds(1);
            rw.Update(date, 7);
            Console.WriteLine(rw.Area);

            date = date.AddSeconds(1);
            rw.Update(date, 2);
            Console.WriteLine(rw.Area);

            date = date.AddSeconds(1);
            rw.Update(date, 1);
            Console.WriteLine(rw.Area);

            date = date.AddSeconds(1);
            rw.Update(date, 5);
            Console.WriteLine(rw.Area);

            date = date.AddSeconds(1);
            rw.Update(date, 7);
            Console.WriteLine(rw.Area);

        }

        #endregion

        public void Update(double dblCurrFitness)
        {
            Update(DateTime.MinValue, dblCurrFitness);
        }

        public RollingWindowPriceArea Clone()
        {
            var clone = (RollingWindowPriceArea)MemberwiseClone();
            clone.m_areas = m_areas.ToList();
            clone.m_data = m_data.ToList();
            return clone;
        }

        public void Dispose()
        {
            if (m_areas != null)
            {
                m_areas.Clear();
                m_areas = null;
            }
            if (m_data != null)
            {
                m_data.Clear();
                m_data = null;
            }
            m_lockObject = null;
        }
    }
}
