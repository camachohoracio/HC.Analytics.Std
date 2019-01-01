#region

using System;
using System.Collections.Generic;
using HC.Core.DynamicCompilation;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowTsEventByTypes<T> where T : ITsEvent
    {
        #region Properties

        public ITsEvent LastValue { get; private set; }
        public DateTime LastUpdateTime { get; private set; }
        public Dictionary<string, List<T>> MapTypeToTsList { get; private set; }

        #endregion

        #region Members

        private double m_dblCurrCounter;
        private readonly int m_intWindowSize;

        #endregion

        #region Constructors

        public RollingWindowTsEventByTypes(
            int intWindowSize)
        {
            if (intWindowSize <= 2)
            {
                throw new Exception("Invalid window size: " +
                                    intWindowSize);
            }
            m_intWindowSize = intWindowSize;
            MapTypeToTsList = new Dictionary<string, List<T>>();
        }

        #endregion

        #region Public

        public void Update(
            T timeSeriesEvent)
        {
            DateTime dateTime = timeSeriesEvent.Time;
            if (LastUpdateTime >= dateTime &&
                dateTime != DateTime.MinValue)
            {
                throw new Exception("Invalid time");
            }

            LastValue = timeSeriesEvent;
            LastUpdateTime = dateTime;
            string strTypeName = timeSeriesEvent.GetType().Name;
            List<T> currTsEventList;
            if(!MapTypeToTsList.TryGetValue(strTypeName, out currTsEventList))
            {
                currTsEventList = new List<T>();
                MapTypeToTsList[strTypeName] = currTsEventList;
            }
            currTsEventList.Add(timeSeriesEvent);
            m_dblCurrCounter = currTsEventList.Count;


            if (m_dblCurrCounter > m_intWindowSize)
            {
                //
                // remove old values
                //
                currTsEventList.RemoveAt(0);
                m_dblCurrCounter = m_intWindowSize;
            }
        }


        #endregion
    }
}

