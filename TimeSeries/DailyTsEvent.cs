#region

using System;

#endregion

namespace HC.Analytics.TimeSeries
{
    [Serializable]
    public class DailyTsEvent : ATsEvent
    {
        #region Members

        private static string[] m_strFields;
        private double m_dblLowMid;
        private double m_dblHighMid;

        #endregion

        #region Properties

        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public double AdjClose { get; set; }
        public double Low{ get; set; }

        #endregion

        #region Pubic

        public double GetHighMid(
            double dblDefault = 0.005)
        {
            if (m_dblHighMid > 0)
            {
                return m_dblHighMid;
            }
            if (AdjClose == 0)
            {
                if (High > 0)
                {
                    if (Low > 0)
                    {
                        return (1.0 + dblDefault) * (High + Low) / 2.0;
                    }
                    m_dblHighMid = dblDefault + High;
                    return m_dblHighMid;
                }
                if (Low > 0)
                {
                    m_dblHighMid = (1.0 + dblDefault) * Low;
                    return m_dblHighMid;
                }
                return 0.0;
            }

            if (AdjClose == High)
            {
                m_dblHighMid = AdjClose * (1.0 + dblDefault);
                return m_dblHighMid;
            }
            if (High == 0)
            {
                High = AdjClose;
            }
            m_dblHighMid = (AdjClose + High) / 2.0;
            return m_dblHighMid;
        }

        public double GetHigh(
            double dblDefault = 0.005)
        {
            if (AdjClose == High)
            {
                return AdjClose * (1.0 + dblDefault);
            }
            return High;
        }

        public double GetLow(
            double dblDefault = 0.005)
        {
            if (AdjClose == Low)
            {
                return AdjClose / (1.0 + dblDefault);
            }
            return Low;
        }

        public double GetLowMid(
            double dblDefault = 0.005)
        {
            if (m_dblLowMid > 0)
            {
                return m_dblLowMid;
            }
            if (AdjClose == 0)
            {
                if (High > 0)
                {
                    if (Low > 0)
                    {
                        m_dblLowMid = (High + Low) / (2.0* (1.0 + dblDefault));
                        return m_dblLowMid;
                    }
                    m_dblLowMid = High / (1.0 + dblDefault);
                    return m_dblLowMid;
                }
                if (Low > 0)
                {
                    m_dblLowMid = Low / (1.0 + dblDefault);
                    return m_dblLowMid;
                }
                return 0.0;
            }

            if (AdjClose == Low)
            {
                m_dblLowMid = AdjClose / (1.0 + dblDefault);
                return m_dblLowMid;
            }
            if (Low == 0)
            {
                Low = AdjClose;
            }
            m_dblLowMid = (Low + AdjClose) / 2.0;
            return m_dblLowMid;
        }

        public static string[] GetFields()
        {
            if (m_strFields == null)
            {
                m_strFields =
                    TsEventHelper.GetFields(
                        typeof (DailyTsEvent));
            }
            return m_strFields;
        }

        #endregion

        public double GetMid()
        {
            return GetAvgPx();
        }

        public new object Clone()
        {
            return MemberwiseClone();
        }

        public double GetAvgPx()
        {
            if (AdjClose == 0)
            {
                if (High > 0)
                {
                    if (Low > 0)
                    {
                        return (High + Low)/2.0;
                    }
                    return High;
                }
                if (Low > 0)
                {
                    return Low;
                }
                return 0.0;
            }

            if (High == 0)
            {
                High = AdjClose;
            }
            if (Low == 0)
            {
                Low = AdjClose;
            }

            return (High + Low + AdjClose) / 3.0;
        }
    }
}
