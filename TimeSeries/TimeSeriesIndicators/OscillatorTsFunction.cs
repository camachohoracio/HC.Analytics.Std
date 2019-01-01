#region

using System.Collections.Generic;
using HC.Analytics.Mathematics.Functions.DataStructures;

#endregion

namespace HC.Analytics.TimeSeries.TimeSeriesIndicators
{
    public class OscillatorTsFunction : TsFunction
    {
        #region Members

        private readonly List<TsRow2D> m_baseFunctionData;
        private readonly int m_intTimeWindow;

        #endregion

        #region Constructors

        public OscillatorTsFunction(
            string strFunctionName,
            List<TsRow2D> baeFunctionData,
            int intTimeWindow) :
                this(strFunctionName,
                     "Date",
                     "Price",
                     baeFunctionData,
                     intTimeWindow)
        {
        }

        [FunctionConstructorAttr]
        public OscillatorTsFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> baseFunctionData,
            int intTimeWindow) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel,
                     null)
        {
            m_baseFunctionData = baseFunctionData;
            m_intTimeWindow = intTimeWindow;
            LoadData();
        }

        #endregion

        private void LoadData()
        {
            //
            // check that the 
            // provided time series is larger than the time window
            //
            if (m_intTimeWindow >= m_baseFunctionData.Count)
            {
                return;
            }
            lock (m_functionDataLock)
            {
                FunctionData = new List<TsRow2D>(m_baseFunctionData.Count + 1);
            }
            for (var i = 1; i < m_baseFunctionData.Count; i++)
            {
                var intSeriesIndex = i - 1;


                if (intSeriesIndex >= m_intTimeWindow)
                {
                    //
                    // go backwards and find the Min and Max differences
                    //
                    var dblMax = -double.MaxValue;
                    var dblMin = double.MaxValue;
                    for (var j = 0; j < m_intTimeWindow; j++)
                    {
                        if (m_baseFunctionData[i - j].Fx < dblMin)
                        {
                            dblMin = m_baseFunctionData[i - j].Fx;
                        }
                        if (m_baseFunctionData[i - j].Fx > dblMax)
                        {
                            dblMax = m_baseFunctionData[i - j].Fx;
                        }
                    }

                    var dblOscill = 100*(m_baseFunctionData[i].Fx - dblMin)/
                                    (dblMax - dblMin);

                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(
                            new TsRow2D(
                                m_baseFunctionData[i].Time,
                                dblOscill));
                    }
                }
            }
        }
    }
}
