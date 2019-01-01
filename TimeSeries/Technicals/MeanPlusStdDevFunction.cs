using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Analytics.TimeSeries.TsStats;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.Technicals
{
    public class MeanPlusStdDevFunction : ATechIndFunction
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
        private RollingWindowStdDev m_rollingWindowStdDev;
        private readonly double m_dblStdDevFactor;
        private readonly bool m_blnIsShort;

        #endregion

        #region Constructors

        public MeanPlusStdDevFunction(
            string strFunctionName,
            List<TsRow2D> functionData,
            int intTimeWindow,
            double dblStdDevFactor,
            bool blnIsShort) :
                this(strFunctionName,
                     "Date",
                     "Price",
                     functionData,
                     intTimeWindow,
                     dblStdDevFactor,
            blnIsShort)
        {
        }

        [FunctionConstructorAttr]
        public MeanPlusStdDevFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> baseFunctionData,
            int intTimeWindow,
            double dblStdDevFactor,
            bool blnIsShort) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel)
        {
            m_intTimeWindow = intTimeWindow;
            m_baseFunctionData = baseFunctionData;
            m_rollingWindowStdDev = new RollingWindowStdDev(
                intTimeWindow);
            m_dblStdDevFactor = dblStdDevFactor;
            m_blnIsShort = blnIsShort;
            Reset();
            SetMovingAverage();
        }

        #endregion

        #region Private Methods

        private void SetMovingAverage()
        {
            lock (m_functionDataLock)
            {
                FunctionData = new List<TsRow2D>(m_baseFunctionData.Count + 1);
            }

            foreach (TsRow2D row2D in m_baseFunctionData)
            {
                if ((m_blnIsShort &&
                    row2D.Fx < 0) ||
                    (!m_blnIsShort &&
                    row2D.Fx > 0))
                {
                    Update(row2D);
                }
            }

            //
            // set the new limits for this function
            //
            SetFunctionLimits();
        }

        #endregion

        #region Public

        public override void Reset()
        {
            m_rollingWindowStdDev = 
                new RollingWindowStdDev(
                    m_intTimeWindow);
        }

        public override void Update(TsRow2D row)
        {
            try
            {
                m_rollingWindowStdDev.Update(
                    row.Time,
                    row.Fx);
                FunctionData.Add(
                    new TsRow2D(
                        row.Time,
                        m_rollingWindowStdDev.Mean + (m_blnIsShort ? -1.0 : 1.0) * m_dblStdDevFactor*m_rollingWindowStdDev.StdDev));
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override string ToString()
        {
            return " Mean_StdDev(" + m_intTimeWindow + "," +
                   FunctionName + ")";
        }

        public MeanPlusStdDevFunction Clone()
        {
            var function =
                new MeanPlusStdDevFunction(
                    FunctionName,
                    m_baseFunctionData,
                    m_intTimeWindow,
                    m_dblStdDevFactor,
                    m_blnIsShort);

            lock (m_functionDataLock)
            {
                function.FunctionData =
                    TimeSeriesUtils.CloneTimeSeriesList(
                        FunctionData);
            }

            return function;
        }

        #endregion
    }
}
