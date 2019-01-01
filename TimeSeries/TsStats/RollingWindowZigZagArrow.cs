using System;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowZigZagArrow
    {
        public bool IsReady
        {
            get { return m_extZigzagBuffer.Count >= m_intWindowSizeParam; }
        }

        public TsRow2D ExtZigzagBuffer
        {
            get { return m_extZigzagBuffer.Last(); }
        }

        public TsRow2D ExtLowBuffer
        {
            get { return m_extLowBuffer.Last(); }
        }

        public TsRow2D ExtHighBuffer
        {
            get { return m_extHighBuffer.Last(); }
        }

        public TsRow2D UpZigzagBuffer
        {
            get { return m_upZigzagBuffer.Last(); }
        }

        public TsRow2D DnZigzagBuffer
        {
            get { return m_dnZigzagBuffer.Last(); }
        }

        const int EXT_LEVEL = 3; // recounting's depth of extremums

        //---- indicator parameters, only three input paramaters!
        private readonly double m_dblDeviationParam;  // Deviation, as factor of spread
        private readonly int m_intBackstepParam;   // Backstep
        private readonly int m_intWindowSizeParam; // extra param


        private readonly BasicRollingWindowInverted<TsRow2D> m_extZigzagBuffer;
        private readonly BasicRollingWindowInverted<TsRow2D> m_extLowBuffer;
        private readonly BasicRollingWindowInverted<TsRow2D> m_extHighBuffer;
        private readonly BasicRollingWindowInverted<TsRow2D> m_upZigzagBuffer;
        private readonly BasicRollingWindowInverted<TsRow2D> m_dnZigzagBuffer;
        private readonly BasicRollingWindowInverted<TsRow2D> m_high;
        private readonly BasicRollingWindowInverted<TsRow2D> m_low;
        int m_intLastHighPos;
        int m_intLastLowPos;
        private TsRow2D m_lastHigh;
        private TsRow2D m_lastLow;
        private readonly SwingLowHigh m_swingLow;
        private readonly SwingLowHigh m_swingHigh;
        private readonly double m_dblAvgSpreadBps;

        public RollingWindowZigZagArrow(
            double dblAvgSpreadBps,
            int intDepthParam = 12,     // Depth
            int dblDeviationParam = 5,  // Deviation
            int intBackstepParam = 3,   // Backstep
            int intWindowSizeParam = 100) // extra param
        {
            m_dblAvgSpreadBps = dblAvgSpreadBps;
            m_dblDeviationParam = dblDeviationParam;
            m_intBackstepParam = intBackstepParam;
            m_intWindowSizeParam = intWindowSizeParam;

            m_swingLow =
                new SwingLowHigh(intDepthParam);
            m_swingHigh =
                new SwingLowHigh(intDepthParam);
            m_extZigzagBuffer = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);
            m_extLowBuffer = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);
            m_extHighBuffer = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);
            m_upZigzagBuffer = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);
            m_dnZigzagBuffer = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);
            m_high = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);
            m_low = new BasicRollingWindowInverted<TsRow2D>(
                m_intWindowSizeParam);

            if (m_intBackstepParam >= intDepthParam)
            {
                throw new Exception("invalid params");
            }
        }

        public bool IsGingDown
        {
            get
            {
                TsRow2D downItem = null;
                for (int i = m_dnZigzagBuffer.Count-1; i < 0; i--)
                {
                    TsRow2D item = m_dnZigzagBuffer[i];
                    if (item != null &&
                        item.Fx != 0)
                    {
                        downItem = item;
                        break;
                    }
                }
                TsRow2D upItem = null;
                for (int i = m_upZigzagBuffer.Count - 1; i < 0; i--)
                {
                    TsRow2D item = m_upZigzagBuffer[i];
                    if (item != null &&
                        item.Fx != 0)
                    {
                        upItem = item;
                        break;
                    }
                }
                if (downItem == null)
                {
                    return false;
                }
                if (upItem == null)
                {
                    return true;
                }
                return downItem.Time > upItem.Time;
            }
        }


        public void Update(
            DateTime dateTime,
            double dblCurHigh0,
            double dblCurLow0)
        {
            try
            {
                var currHigh =
                    new TsRow2D(dateTime, dblCurHigh0);
                var currLow =
                    new TsRow2D(dateTime, dblCurLow0);
                m_extHighBuffer.Update(null);
                m_extLowBuffer.Update(null);
                m_dnZigzagBuffer.Update(null);
                m_extZigzagBuffer.Update(null);
                m_upZigzagBuffer.Update(null);
                m_swingLow.Update(
                    currLow.Time,
                    currLow.Fx);
                m_swingHigh.Update(
                    currHigh.Time,
                    currHigh.Fx);
                m_high.Update(currHigh);
                m_low.Update(currLow);

                if (!IsReady)
                {
                    return;
                }

                //--- find first extremum in the depth EXT_LEVEL or 100 last bars
                int i;
                int counterZ = 0;
                int limit = 0;
                int whatlookfor = 0;

                for (i = 0; i < m_intWindowSizeParam; i++)
                {
                    if (counterZ > EXT_LEVEL)
                    {
                        break;
                    }
                    if (m_extZigzagBuffer[i] != null)
                        counterZ++;
                }

                i = Math.Min(i, m_intWindowSizeParam - 1);
                
                //--- no extremum found - recounting all from begin
                if (counterZ > 0)
                {
                    //--- set start position to found extremum position
                    limit = i - 1;
                    //--- what kind of extremum?
                    if (m_extLowBuffer[i] != null)
                    {
                        //--- low extremum
                        currLow = m_extLowBuffer[i];
                        //--- will look for the next high extremum
                        whatlookfor = 1;
                    }
                    else
                    {
                        //--- high extremum
                        currHigh = m_extHighBuffer[i];
                        //--- will look for the next low extremum
                        whatlookfor = -1;
                    }
                    //--- clear the rest data
                    for (i = limit - 1; i >= 0; i--)
                    {
                        m_extZigzagBuffer[i] = null;
                        m_extLowBuffer[i] = null;
                        m_extHighBuffer[i] = null;
                    }
                }

                double dblMid = 
                    ((currHigh == null ? 0 : currHigh.Fx) - 
                    (currLow == null ? 0 : currLow.Fx))/2.0;
                double dblAvgSpread = dblMid * m_dblAvgSpreadBps / 10000.0;

                //--- main loop      
                for (i = limit; i >= 0; i--)
                {
                    //--- find lowest low in depth of bars
                    var extremum = m_swingLow.MinItem.ConvertToTsRow();
                    //--- this lowest has been found previously
                    if (extremum.Fx == (m_lastLow == null ? 0 : m_lastLow.Fx))
                        extremum = null;
                    else
                    {
                        //--- new last low
                        m_lastLow = extremum;
                        //--- discard extremum if current low is too high
                        if (m_low[i].Fx - extremum.Fx > 
                            m_dblDeviationParam *dblAvgSpread)
                            extremum = null;
                        else
                        {
                            //--- clear previous extremums in backstep bars
                            for (int back = 1; back <= m_intBackstepParam; back++)
                            {
                                int pos = i + back;
                                pos = Math.Min(m_intWindowSizeParam - 1, pos);
                                if (m_extLowBuffer[pos] != null && 
                                    m_extLowBuffer[pos].Fx > extremum.Fx)
                                    m_extLowBuffer[pos] = null;
                            }
                        }
                    }
                    //--- found extremum is current low
                    if (m_low[i].Fx == (extremum == null ? 0 : extremum.Fx))
                        m_extLowBuffer[i] = extremum;
                    else
                        m_extLowBuffer[i] = null;
                    //--- find highest high in depth of bars
                    extremum = m_swingHigh.MaxItem.ConvertToTsRow();
                    //--- this highest has been found previously
                    if (extremum.Fx == (m_lastHigh == null ? 0  : m_lastHigh.Fx))
                        extremum = null;
                    else
                    {
                        //--- new last high
                        m_lastHigh = extremum;
                        //--- discard extremum if current high is too low
                        if (extremum.Fx - m_high[i].Fx > 
                            m_dblDeviationParam * dblAvgSpread)
                            extremum = null;
                        else
                        {
                            //--- clear previous extremums in backstep bars
                            for (int back = 1; back <= m_intBackstepParam; back++)
                            {
                                int pos = i + back;
                                pos = Math.Min(m_intWindowSizeParam - 1, pos);
                                if (m_extHighBuffer[pos] != null && -m_extHighBuffer[pos].Fx < extremum.Fx)
                                    m_extHighBuffer[pos] = null;
                            }
                        }
                    }
                    //--- found extremum is current high
                    if (extremum != null && m_high[i].Fx == extremum.Fx)
                        m_extHighBuffer[i] = extremum;
                    else
                        m_extHighBuffer[i] = null;
                }
                //--- final cutting 
                if (whatlookfor == 0)
                {
                    m_lastLow = null;
                    m_lastHigh = null;
                }
                else
                {
                    m_lastLow = currLow;
                    m_lastHigh = currHigh;
                }
                for (i = limit; i >= 0; i--)
                {
                    switch (whatlookfor)
                    {
                        case 0: // look for peak or lawn 
                            if (m_lastLow == null && m_lastHigh == null)
                            {
                                if (m_extHighBuffer[i] != null)
                                {
                                    m_lastHigh = m_high[i];
                                    m_intLastHighPos = i;
                                    whatlookfor = -1;
                                    m_extZigzagBuffer[i] = m_lastHigh;
                                    m_dnZigzagBuffer[i] = m_lastHigh;
                                }
                                if (m_extLowBuffer[i] != null)
                                {
                                    m_lastLow = m_low[i];
                                    m_intLastLowPos = i;
                                    whatlookfor = 1;
                                    m_extZigzagBuffer[i] = m_lastLow;
                                    m_upZigzagBuffer[i] = m_lastLow;
                                }
                            }
                            break;
                        case 1: // look for peak
                            if (m_extLowBuffer[i] != null && 
                                m_extLowBuffer[i].Fx < 
                                    (m_lastLow == null ? 0 : m_lastLow.Fx) && 
                                    m_extHighBuffer[i] == null)
                            {
                                m_extZigzagBuffer[m_intLastLowPos] = null;
                                m_intLastLowPos = i;
                                m_lastLow = m_extLowBuffer[i];
                                m_extZigzagBuffer[i] = m_lastLow;
                                m_upZigzagBuffer[i] = m_lastLow;
                            }
                            if (m_extHighBuffer[i] != null &&
                                m_extLowBuffer[i] == null)
                            {
                                m_lastHigh = m_extHighBuffer[i];
                                m_intLastHighPos = i;
                                m_extZigzagBuffer[i] = m_lastHigh;
                                m_dnZigzagBuffer[i] = m_lastHigh;
                                whatlookfor = -1;
                            }
                            break;
                        case -1: // look for lawn
                            if (m_extHighBuffer[i] != null &&
                                m_extHighBuffer[i].Fx > m_lastHigh.Fx &&
                                m_extLowBuffer[i] == null)
                            {
                                m_extZigzagBuffer[m_intLastHighPos] = null;
                                m_intLastHighPos = i;
                                m_lastHigh = m_extHighBuffer[i];
                                m_extZigzagBuffer[i] = m_lastHigh;
                                m_dnZigzagBuffer[i] = m_lastHigh;
                            }
                            if (m_extLowBuffer[i] != null &&
                                m_extHighBuffer[i] == null)
                            {
                                m_lastLow = m_extLowBuffer[i];
                                m_intLastLowPos = i;
                                m_extZigzagBuffer[i] = m_lastLow;
                                m_upZigzagBuffer[i] = m_lastLow;
                                whatlookfor = 1;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
