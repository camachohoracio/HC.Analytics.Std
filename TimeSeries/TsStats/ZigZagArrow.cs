using System;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class ZigZagArrow
    {
        public bool IsReady
        {
            get { return m_extZigzagBuffer.Count >= m_intWindowSizeParam; }
        }

        public double ExtZigzagBuffer
        {
            get { return m_extZigzagBuffer.Last(); }
        }

        public double ExtLowBuffer
        {
            get { return m_extLowBuffer.Last(); }
        }

        public double ExtHighBuffer
        {
            get { return m_extHighBuffer.Last(); }
        }

        public double UpZigzagBuffer
        {
            get { return m_upZigzagBuffer.Last(); }
        }

        public double DnZigzagBuffer
        {
            get { return m_dnZigzagBuffer.Last(); }
        }

        const int EXT_LEVEL = 3; // recounting's depth of extremums
        
        //---- indicator parameters, only three input paramaters!
        private readonly int m_intDeviationParam;  // Deviation
        private readonly int m_intBackstepParam;   // Backstep
        private readonly int m_intWindowSizeParam; // extra param


        private readonly BasicRollingWindow<double> m_extZigzagBuffer;
        private readonly BasicRollingWindow<double> m_extLowBuffer;
        private readonly BasicRollingWindow<double> m_extHighBuffer;
        private readonly BasicRollingWindow<double> m_upZigzagBuffer;
        private readonly BasicRollingWindow<double> m_dnZigzagBuffer;
        private readonly BasicRollingWindow<double> m_high;
        private readonly BasicRollingWindow<double> m_low;
        int m_intLastHighPos;
        int m_intLastLowPos;
        private double m_dblLastHigh;
        private double m_dblLastLow;
        private readonly SwingLowHigh m_swingLow;
        private readonly SwingLowHigh m_swingHigh;
        private readonly double m_dblAvgSpread;

        public ZigZagArrow(
            double dblAvgSpread,
            int intDepthParam = 12,     // Depth
            int intDeviationParam = 5,  // Deviation
            int intBackstepParam = 3,   // Backstep
            int intWindowSizeParam = 100) // extra param
        {
            m_dblAvgSpread = dblAvgSpread;
            m_intDeviationParam = intDeviationParam;
            m_intBackstepParam = intBackstepParam;
            m_intWindowSizeParam = intWindowSizeParam;

            m_swingLow =
                new SwingLowHigh(intDepthParam);
            m_swingHigh =
                new SwingLowHigh(intDepthParam);
            m_extZigzagBuffer = new BasicRollingWindow<double>(
                m_intWindowSizeParam);
            m_extLowBuffer = new BasicRollingWindow<double>(
                m_intWindowSizeParam);
            m_extHighBuffer = new BasicRollingWindow<double>(
                m_intWindowSizeParam);
            m_upZigzagBuffer = new BasicRollingWindow<double>(
                m_intWindowSizeParam);
            m_dnZigzagBuffer = new BasicRollingWindow<double>(
                m_intWindowSizeParam);
            m_high = new BasicRollingWindow<double>(
                m_intWindowSizeParam);
            m_low = new BasicRollingWindow<double>(
                m_intWindowSizeParam);

            if (m_intBackstepParam >= intDepthParam)
            {
                throw new Exception("invalid params");
            }
        }


        public void Update(
            DateTime dateTime,
            double dblCurHigh,
            double dblCurLow)
        {
            try
            {

                int intWhatLookFor = 0;
                int intLimit;
                m_extHighBuffer.Update(0);
                m_extLowBuffer.Update(0);
                m_dnZigzagBuffer.Update(0);
                m_extZigzagBuffer.Update(0);
                m_upZigzagBuffer.Update(0);
                m_swingLow.Update(
                    dateTime,
                    dblCurLow);
                m_swingHigh.Update(
                    dateTime,
                    dblCurHigh);
                m_high.Update(dblCurHigh);
                m_low.Update(dblCurLow);

                int intWindowSize = m_extZigzagBuffer.Count;

                //---
                //--- first calculations

                //--- find first extremum in the depth ExtLevel or window size
                int intCountFoundZigZag = 0;
                int intIndex = intWindowSize - 1;
                while (intCountFoundZigZag < EXT_LEVEL && intIndex > 0)
                {
                    if (m_extZigzagBuffer[intIndex] != 0.0)
                    {
                        intCountFoundZigZag++;
                    }
                    intIndex--;
                }
                //--- no extremum found - recounting all from begin
                if (intCountFoundZigZag == 0)
                {
                    intLimit = 0;
                }
                else
                {
                    //--- set start position to found extremum position
                    intLimit = intIndex;
                    //--- what kind of extremum?
                    if (m_extLowBuffer[intIndex] != 0.0)
                    {
                        //--- low extremum
                        dblCurLow = m_extLowBuffer[intIndex];
                        //--- will look for the next high extremum
                        intWhatLookFor = 1;
                    }
                    else
                    {
                        //--- high extremum
                        dblCurHigh = m_extHighBuffer[intIndex];
                        //--- will look for the next low extremum
                        intWhatLookFor = -1;
                    }
                    //--- clear the rest data
                    for (int i = intLimit; i < intWindowSize; i++)
                    {
                        m_extZigzagBuffer[i] = 0.0;
                        m_extLowBuffer[i] = 0.0;
                        m_extHighBuffer[i] = 0.0;
                    }
                }
                //--- main loop, from old to new      
                for (int i = intLimit; i < intWindowSize; i++) // loop from old to new
                {
                    //--- find lowest low in depth of bars
                    double dblExtremum = m_swingLow.Min;
                    //--- this lowest has been found previously
                    if (dblExtremum == m_dblLastLow)
                    {
                        dblExtremum = 0.0;
                    }
                    else
                    {
                        //--- new last low
                        m_dblLastLow = dblExtremum;
                        //--- discard extremum if current low is too high
                        if (m_low[i] - dblExtremum > m_intDeviationParam*m_dblAvgSpread)
                        {
                            dblExtremum = 0.0;
                        }
                        else
                        {
                            //--- clear previous extremums in backstep bars
                            for (int intBack = 1; intBack <= m_intBackstepParam; intBack++)
                            {
                                int intPos = i - intBack;

                                if (intPos>=0 && 
                                    m_extLowBuffer[intPos] != 0 &&
                                    m_extLowBuffer[intPos] > dblExtremum)
                                {
                                    m_extLowBuffer[intPos] = 0.0;
                                }
                            }
                        }
                    }

                    //--- found extremum is current low
                    if (m_low[i] == dblExtremum)
                    {
                        if (i > 0)
                        {
                            Console.WriteLine("test");
                        }
                        m_extLowBuffer[i] = dblExtremum;
                    }
                    else
                    {
                        m_extLowBuffer[i] = 0.0;
                    }
                    //--- find highest high in depth of bars
                    dblExtremum = m_swingHigh.Max;
                    //--- this highest has been found previously
                    if (dblExtremum == m_dblLastHigh)
                    {
                        dblExtremum = 0.0;
                    }
                    else
                    {
                        //--- new last high
                        m_dblLastHigh = dblExtremum;
                        //--- discard extremum if current high is too low
                        if (dblExtremum - m_high[i] > m_intDeviationParam*m_dblAvgSpread)
                        {
                            dblExtremum = 0.0;
                        }
                        else
                        {
                            //--- clear previous extremums in backstep bars
                            for (int intBack = 1; intBack <= m_intBackstepParam; intBack++)
                            {
                                int intPos = i - intBack;
                                if (intPos>=0&&
                                    m_extHighBuffer[intPos] != 0 && 
                                    m_extHighBuffer[intPos] < dblExtremum)
                                {
                                    m_extHighBuffer[intPos] = 0.0;
                                }
                            }
                        }
                    }
                    //--- found extremum is current high
                    if (m_high[i] == dblExtremum)
                    {
                        if (i > 0)
                        {
                            Console.WriteLine("test");
                        }
                        m_extHighBuffer[i] = dblExtremum;
                    }
                    else
                    {
                        m_extHighBuffer[i] = 0.0;
                    }
                }
                //--- final cutting 
                if (intWhatLookFor == 0)
                {
                    m_dblLastLow = 0.0;
                    m_dblLastHigh = 0.0;
                }
                else
                {
                    m_dblLastLow = dblCurLow;
                    m_dblLastHigh = dblCurHigh;
                }
                for (int i = intLimit; i < intWindowSize; i++) // loop from old to new
                {
                    switch (intWhatLookFor)
                    {
                        case 0: // look for peak or lawn 
                            if (m_dblLastLow == 0.0 && m_dblLastHigh == 0.0)
                            {
                                if (m_extHighBuffer[i] != 0.0)
                                {
                                    m_dblLastHigh = m_high[i];
                                    m_intLastHighPos = i;
                                    intWhatLookFor = -1;
                                    m_extZigzagBuffer[i] = m_dblLastHigh;
                                    m_dnZigzagBuffer[i] = m_dblLastHigh; // + (ad * Point);
                                }
                                if (m_extLowBuffer[i] != 0.0)
                                {
                                    m_dblLastLow = m_low[i];
                                    m_intLastLowPos = i;
                                    intWhatLookFor = 1;
                                    m_extZigzagBuffer[i] = m_dblLastLow;
                                    m_upZigzagBuffer[i] = m_dblLastLow; // - (ad * Point);
                                }
                            }
                            break;
                        case 1: // look for peak
                            if (m_extLowBuffer[i] != 0.0 && m_extLowBuffer[i] < m_dblLastLow &&
                                m_extHighBuffer[i] == 0.0)
                            {
                                m_extZigzagBuffer[m_intLastLowPos] = 0.0;
                                m_intLastLowPos = i;
                                m_dblLastLow = m_extLowBuffer[i];
                                m_extZigzagBuffer[i] = m_dblLastLow;
                                m_upZigzagBuffer[i] = m_dblLastLow; // - (ad * Point);
                            }
                            if (m_extHighBuffer[i] != 0.0 && m_extLowBuffer[i] == 0.0)
                            {
                                m_dblLastHigh = m_extHighBuffer[i];
                                m_intLastHighPos = i;
                                m_extZigzagBuffer[i] = m_dblLastHigh;
                                m_dnZigzagBuffer[i] = m_dblLastHigh; // + (ad * Point);
                                intWhatLookFor = -1;
                            }
                            break;
                        case -1: // look for lawn
                            if (m_extHighBuffer[i] != 0.0 && m_extHighBuffer[i] > m_dblLastHigh &&
                                m_extLowBuffer[i] == 0.0)
                            {
                                m_extZigzagBuffer[m_intLastHighPos] = 0.0;
                                m_intLastHighPos = i;
                                m_dblLastHigh = m_extHighBuffer[i];
                                m_extZigzagBuffer[i] = m_dblLastHigh;
                                m_dnZigzagBuffer[i] = m_dblLastHigh; // + (ad * Point);
                            }
                            if (m_extLowBuffer[i] != 0.0 && m_extHighBuffer[i] == 0.0)
                            {
                                m_dblLastLow = m_extLowBuffer[i];
                                m_intLastLowPos = i;
                                m_extZigzagBuffer[i] = m_dblLastLow;
                                m_upZigzagBuffer[i] = m_dblLastLow; // - (ad * Point);
                                intWhatLookFor = 1;
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
