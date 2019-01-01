#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowTsFunction : IDisposable
    {
        #region Properties

        public double LastValue { get; private set; }
        public DateTime LastUpdateTime { get; private set; }
        public List<TsRow2D> Data { get; private set; }

        #endregion

        #region Members

        private double m_dblCurrCounter;
        private readonly int m_intWindowSize;

        #endregion

        #region Constructors

        public RollingWindowTsFunction(
            int intWindowSize)
        {
            if (intWindowSize == 0)
            {
                throw new Exception("Invalid window size: " +
                                    intWindowSize);
            }
            m_intWindowSize = intWindowSize;
            Data = new List<TsRow2D>();
        }

        #endregion

        #region Public

        public bool IsReady()
        {
            return Data.Count >= m_intWindowSize;
        }

        public void Update(
            DateTime dateTime,
            double dblVal)
        {
            //if (LastUpdateTime >= dateTime &&
            //    dateTime != DateTime.MinValue)
            //{
            //    throw new Exception("Invalid time");
            //}

            LastValue = dblVal;
            LastUpdateTime = dateTime;
            Data.Add(new TsRow2D(dateTime,dblVal));
            m_dblCurrCounter = Data.Count;


            if (m_dblCurrCounter > m_intWindowSize)
            {
                //
                // remove old values
                //
                Data.RemoveAt(0);
                m_dblCurrCounter = m_intWindowSize;
            }
        }

        public RollingWindowTsFunction Clone()
        {
            var clone = (RollingWindowTsFunction)MemberwiseClone();
            clone.Data = new List<TsRow2D>(Data);
            return clone;
        }

        #endregion

        public void Dispose()
        {
            Data.Clear();
            Data = null;
        }
    }
}

