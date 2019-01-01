using System;
using HC.Core;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowReturn : IDisposable
    {
        private readonly bool m_blnUseRate;
        private bool m_blnIsReady;

        public bool IsReady { 
            get { return m_blnIsReady && !double.IsNaN(Return); }
        }
        public DateTime LastDateTime { get; private set; }
        public double LastValue { get; private set; }
        public double Return { get; private set; }

        public RollingWindowReturn() // used for reflection
        {}

        public RollingWindowReturn(bool blnUseRate)
        {
            m_blnUseRate = blnUseRate;
            Return = double.NaN;
        }

        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            try
            {
                if (dateTime < LastDateTime)
                {
                    Verboser.WriteLine(typeof(RollingWindowReturn).Name + " invalid date");
                    return;
                }

                if (m_blnIsReady)
                {
                    Return = GetReturn(dblValue);
                }

                LastDateTime = dateTime;
                LastValue = dblValue;
                m_blnIsReady = true;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public double GetReturn(double dblValue)
        {
            double dblReturn;
            if (m_blnUseRate)
            {
                if (LastValue == 0)
                {
                    return 0;
                }

                dblReturn = (dblValue - LastValue)/LastValue;
            }
            else
            {
                dblReturn = dblValue - LastValue;
            }
            return dblReturn;
        }

        public void Dispose()
        {
        }
    }
}
