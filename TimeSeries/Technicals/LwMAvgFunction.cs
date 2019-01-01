#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.Technicals
{
    [Serializable]
    public class LwMAvgFunction : ATechIndFunction
    {
        #region Properties

        public int TimeWindow
        {
            get { return m_intTimeWindow; }
        }

        #endregion

        #region Members

        private readonly int m_intTimeWindow;
        private double m_dblTotalWeight;
        private List<TsRow2D> m_innerFunctionData;
        private int m_intWindowCounter;
        private DateTime m_prevTime;

        #endregion

        #region Constructors

        public LwMAvgFunction(
            string strFunctionName,
            List<TsRow2D> functionData,
            int intTimeWindow) :
                this(strFunctionName,
                     "Date",
                     "Price",
                     functionData,
                     intTimeWindow)
        {
        }

        [FunctionConstructorAttr]
        public LwMAvgFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> functionData,
            int intTimeWindow) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel)
        {
            m_intTimeWindow = intTimeWindow;
            Reset();
            SetMovingAverage(functionData);
        }

        #endregion

        #region Private Methods

        private void SetMovingAverage(
            List<TsRow2D> functionData)
        {
            try
            {
                lock (m_functionDataLock)
                {
                    FunctionData = new List<TsRow2D>(functionData.Count + 1);
                }

                foreach (TsRow2D timeSeriesRow2D in functionData)
                {
                    Update(timeSeriesRow2D);
                }

                //
                // set the new limits for this function
                //
                SetFunctionLimits();
            }
            catch (HCException e)
            {
                //Debugger.Break();
                throw;
            }

        }

        #endregion

        #region Public

        public override void Reset()
        {
            m_innerFunctionData = new List<TsRow2D>();
            m_intWindowCounter = 0;
            m_dblTotalWeight = 0;
        }

        public override void Update(TsRow2D row2D)
        {

            try
            {
                if (m_prevTime >= row2D.Time)
                {
                    return;
                }
                m_prevTime = row2D.Time;

                m_innerFunctionData.Add(row2D);
                //
                // if window is zero, then add the whole time series
                //
                if (m_intTimeWindow == 0)
                {
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(row2D);
                    }
                    return;
                }

                if (m_intWindowCounter < m_intTimeWindow)
                {
                    m_dblTotalWeight += m_intWindowCounter + 1;
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(row2D);
                    }
                    m_intWindowCounter++;
                    return;
                }

                var i = m_intWindowCounter - 1;

                if (i >= m_intTimeWindow - 1)
                {
                    double dblCurrentWeight = 0;
                    for (var j = 0; j < m_intTimeWindow; j++)
                    {
                        dblCurrentWeight += (m_intTimeWindow - j)*
                                            m_innerFunctionData[i - j].Fx;
                    }
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(
                            new TsRow2D(
                                row2D.Time,
                                dblCurrentWeight/m_dblTotalWeight));
                    }
                }

                m_intWindowCounter++;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override string ToString()
        {
            return " LwMA(" + m_intTimeWindow + "," +
                   FunctionName + ") ";
        }

        #endregion
    }
}
