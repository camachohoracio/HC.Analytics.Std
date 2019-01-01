#region

using System.Collections.Generic;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.TimeSeries.TimeSeriesIndicators
{
    public class RsiTsFunction : TsFunction
    {
        #region Members

        private readonly List<TsRow2D> m_baseFunctionData;
        private readonly int m_intTimeWindow;

        #endregion

        #region Constructors

        public RsiTsFunction(
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
        public RsiTsFunction(
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

            double dblSumImprove = 0;
            double dblSumNonImprove = 0;
            var blnImproveArr = new bool[m_baseFunctionData.Count];
            var blnNonImproveArr = new bool[m_baseFunctionData.Count];

            for (var i = 1; i < m_baseFunctionData.Count; i++)
            {
                var intSeriesIndex = i - 1;

                var currentTsRow2D = m_baseFunctionData[i];
                var previousTsRow2D = m_baseFunctionData[i - 1];

                if (currentTsRow2D.Fx > previousTsRow2D.Fx)
                {
                    dblSumImprove++;
                    blnImproveArr[i] = true;
                }
                else if (currentTsRow2D.Fx < previousTsRow2D.Fx)
                {
                    dblSumNonImprove++;
                    blnNonImproveArr[i] = true;
                }

                if (intSeriesIndex >= m_intTimeWindow)
                {
                    //
                    // remove the first value
                    //
                    if (intSeriesIndex > m_intTimeWindow)
                    {
                        try
                        {
                            if (blnImproveArr[intSeriesIndex - m_intTimeWindow])
                            {
                                dblSumImprove--;
                                ;
                            }
                            if (blnNonImproveArr[intSeriesIndex - m_intTimeWindow])
                            {
                                dblSumNonImprove--;
                                ;
                            }
                        }
                        catch (HCException e2)
                        {
                            //Debugger.Break();
                            throw;
                        }
                    }

                    var dblRs = (dblSumImprove/m_intTimeWindow)/
                                (dblSumNonImprove/m_intTimeWindow);

                    var dblRsi = 100.0 - (100.0/(1.0 - dblRs));
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(
                            new TsRow2D(
                                m_baseFunctionData[i].Time,
                                dblRsi));
                    }
                }
            }
        }
    }
}
