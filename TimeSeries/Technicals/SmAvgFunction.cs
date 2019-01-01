#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.Technicals
{
    /// <summary>
    ///   Simple moving average function
    /// </summary>
    [Serializable]
    public class SmAvgFunction : ATechIndFunction
    {
        #region Properties

        public int TimeWindow
        {
            get { return m_intTimeWindow; }
        }

        #endregion

        #region Members

        private readonly List<TsRow2D> m_baseFunctionData;
        private readonly int m_intTimeWindow;
        private double m_dblSum;
        private int m_intWindowCounter;
        private List<double> m_rawData;
        private DateTime m_prevTime;

        #endregion

        #region Constructors

        public SmAvgFunction(
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
        public SmAvgFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> baseFunctionData,
            int intTimeWindow) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel)
        {
            m_rawData = new List<double>();
            m_intTimeWindow = intTimeWindow;
            m_baseFunctionData = baseFunctionData;
            Reset();
            SetMovingAverage();
        }

        #endregion

        #region Private Methods

        private void SetMovingAverage()
        {
            try
            {
                lock (m_functionDataLock)
                {
                    FunctionData = new List<TsRow2D>(m_baseFunctionData.Count + 1);
                }

                for (int i = 0; i < m_baseFunctionData.Count; i++)
                {
                    Update(m_baseFunctionData[i]);
                }

                //
                // set the new limits for this function
                //
                SetFunctionLimits();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Public

        public override void Reset()
        {
            m_intWindowCounter = 0;
            m_dblSum = 0;
            m_rawData = new List<double>();
        }

        public override void Update(TsRow2D row)
        {
            try
            {
                if (m_prevTime >= row.Time)
                {
                    if (m_prevTime == row.Time)
                    {
                        return;
                    }
                    throw new HCException("Date time cannot be >");
                }
                m_prevTime = row.Time;

                if (m_intTimeWindow == 0)
                {
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(row);
                    }
                    m_rawData.Add(row.Fx);
                    return;
                }

                if (m_intWindowCounter > 0)
                {
                    m_dblSum += row.Fx;
                }
                if (m_intWindowCounter <= m_intTimeWindow)
                {
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(row);
                    }
                    m_intWindowCounter++;
                    m_rawData.Add(row.Fx);
                    return;
                }

                int intSeriesIndex = m_intWindowCounter - 1;

                if (intSeriesIndex >= m_intTimeWindow - 1)
                {
                    //
                    // remove the first value
                    //
                    if (intSeriesIndex > m_intTimeWindow - 1)
                    {
                        m_dblSum -=
                            m_rawData[intSeriesIndex - m_intTimeWindow + 1];
                    }

                    double dblAverage = m_dblSum/m_intTimeWindow;
                    lock (m_functionDataLock)
                    {
                        FunctionData.Add(
                            new TsRow2D(
                                row.Time,
                                dblAverage));
                    }
                    m_rawData.Add(row.Fx);

                    HCException.ThrowIfTrue(double.IsNaN(dblAverage),
                                            "NaN value");


                    m_intWindowCounter++;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override string ToString()
        {
            return " SMA(" + m_intTimeWindow + "," +
                   FunctionName + ")";
        }

        public SmAvgFunction Clone()
        {
            var smAvgFunction =
                new SmAvgFunction(
                    FunctionName,
                    m_baseFunctionData,
                    m_intTimeWindow);

            lock (m_functionDataLock)
            {
                smAvgFunction.FunctionData =
                    TimeSeriesUtils.CloneTimeSeriesList(
                        FunctionData);
            }

            return smAvgFunction;
        }

        #endregion
    }
}
