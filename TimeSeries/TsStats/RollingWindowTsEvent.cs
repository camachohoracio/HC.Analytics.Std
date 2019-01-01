#region

using System;
using System.Collections.Generic;
using HC.Core.DynamicCompilation;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowTsEvent<T> where T : ITsEvent
    {
        #region Properties

        public T LastValue { get; private set; }
        public DateTime LastUpdateTime { get; private set; }
        public LinkedList<T> TsList { get; private set; }

        public int Count
        {
            get
            {
                return TsList.Count;
            }
        }

        #endregion

        #region Members

        private double m_dblCurrCounter;
        public double m_dblSumVal;
        public int WindowSize { get; private set; }

        #endregion

        #region Constructors

        public RollingWindowTsEvent() : this(int.MaxValue){}

        public RollingWindowTsEvent(
            int windowSize)
        {
            if (windowSize < 2)
            {
                throw new Exception("Invalid window size: " +
                                    windowSize);
            }
            WindowSize = windowSize;
            TsList = new LinkedList<T>();
        }

        #endregion

        #region Public

        public void Update(
            T timeSeriesEvent)
        {
            try
            {
                DateTime dateTime = timeSeriesEvent.Time;
                if (LastUpdateTime >= dateTime &&
                    dateTime != DateTime.MinValue)
                {
                    Console.WriteLine("Invalid time last[" +
                                        LastUpdateTime + "]>=current[" +
                                        dateTime + "]");
                    return;
                }

                LastValue = timeSeriesEvent;
                LastUpdateTime = dateTime;
                TsList.AddLast(timeSeriesEvent);
                m_dblCurrCounter = TsList.Count;


                if (m_dblCurrCounter > WindowSize)
                {
                    //
                    // remove old values
                    //
                    TsList.RemoveFirst();
                    m_dblCurrCounter = WindowSize;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public bool IsReady
        {
            get { return m_dblCurrCounter >= WindowSize; }
        }

        #endregion

        public RollingWindowTsEvent<T> Clone()
        {
            return new RollingWindowTsEvent<T>(WindowSize)
                       {
                           LastUpdateTime = LastUpdateTime,
                           TsList = new LinkedList<T>(TsList),
                           LastValue = LastValue,
                           m_dblCurrCounter = m_dblCurrCounter,
                       };
        }
    }
}

