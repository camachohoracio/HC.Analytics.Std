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
    ///   Exponential moving average function
    /// </summary>
    [Serializable]
    public class ExMAvgFunction : ATechIndFunction
    {
        #region Properties

        public double Alpha
        {
            get { return m_dblAlpha; }
            set { m_dblAlpha = value; }
        }

        #endregion

        #region Members

        private double m_dblAlpha;
        public double LastMovingAverage { get; private set; }
        public List<TsRow2D> InnerFunctionData { get; private set; }
        public List<TsRow2D> WindowData { get; private set; }
        private int m_intRowCounter;
        private DateTime m_prevTime;
        private readonly bool m_blnAddToList = true;
        public double WindowSize { get; private set; }

        public bool IsReady 
        {
            get
            {
                return WindowData.Count >= WindowSize;
            }
        }
        #endregion

        #region Constructors

        public ExMAvgFunction(
            double dblAlpha,
            bool blnAddtoList = true)
            : this(
                typeof(ExMAvgFunction).Name,
                new List<TsRow2D>(),
                dblAlpha)
        {
            m_blnAddToList = blnAddtoList;
        }

        public ExMAvgFunction(
            int intWindow,
            bool blnAddtoList = true) : this(
            typeof(ExMAvgFunction).Name,
            new List<TsRow2D>(),
            2.0 / (intWindow + 1.0))
        {
            m_blnAddToList = blnAddtoList;
        }

        public ExMAvgFunction(
            string strFunctionName,
            List<TsRow2D> functionData,
            double dblAlpha) :
                this(strFunctionName,
                     "Date",
                     "Price",
                     functionData,
                     dblAlpha)
        {
        }

        [FunctionConstructorAttr]
        public ExMAvgFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> functionData,
            double dblAlpha) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel)
        {
            WindowData = new List<TsRow2D>();
            m_dblAlpha = dblAlpha;
            WindowSize = (2.0 / dblAlpha) - 1.0;
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
                for (int i = 0; i < functionData.Count; i++)
                {
                    Update(functionData[i]);
                }
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
            m_intRowCounter = 0;
            LastMovingAverage = 0;
            InnerFunctionData = new List<TsRow2D>();
        }

        public void Update(
            DateTime dateTime,
            double dblVal)
        {
            Update(new TsRow2D(dateTime, dblVal));
        }

        public override void Update(TsRow2D row2D)
        {
            try
            {
                if (m_prevTime >= row2D.Time)
                {
                    if (m_prevTime == row2D.Time)
                    {
                        return;
                    }

                    throw new HCException("Date time cannot be >");
                }
                m_prevTime = row2D.Time;

                InnerFunctionData.Add(row2D);
                if (m_intRowCounter < 2)
                {
                    LastMovingAverage = row2D.Fx;
                    if (m_blnAddToList)
                    {
                        lock (m_functionDataLock)
                        {
                            FunctionData.Add(row2D);
                        }
                    }
                    m_intRowCounter++;
                    return;
                }


                //emav = (dampen * (Val - emav)) + emav;

                var dblCurrentMovingAverage =
                    m_dblAlpha* row2D.Fx +
                    (1.0 - m_dblAlpha)*LastMovingAverage;

                if (m_blnAddToList)
                {
                    lock (m_functionDataLock)
                    {
                        var tsRes = new TsRow2D(
                            row2D.Time,
                            dblCurrentMovingAverage);
                        FunctionData.Add(
                            tsRes);
                        WindowData.Add(tsRes);
                        if (WindowData.Count > WindowSize)
                        {
                            WindowData.RemoveAt(0);
                        }
                    }
                }
                LastMovingAverage = dblCurrentMovingAverage;
                m_intRowCounter++;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override string ToString()
        {
            return " ExMA(" + m_dblAlpha + "," +
                   FunctionName + ") ";
        }

        #endregion
    }
}
