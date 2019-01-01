#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.TimeSeries.TimeSeriesIndicators
{
    /// <summary>
    ///   Calculates the difference between two time series functions
    /// </summary>
    public class DiffMovingAverageFunction : TsFunction
    {
        #region Members

        private readonly List<TsRow2D> m_functionData1;
        private readonly List<TsRow2D> m_functionData2;

        #endregion

        #region Constructors

        public DiffMovingAverageFunction(
            string strFunctionName,
            List<TsRow2D> functionData1,
            List<TsRow2D> functionData2) :
                this(strFunctionName,
                     "Date",
                     "Price",
                     functionData1,
                     functionData2)
        {
        }

        [FunctionConstructorAttr]
        public DiffMovingAverageFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> functionData1,
            List<TsRow2D> functionData2) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel,
                     null)
        {
            m_functionData1 = functionData1;
            m_functionData2 = functionData2;
            LoadData();
        }

        #endregion

        private void LoadData()
        {
            //
            // get lower bound date
            //
            var date1 = (from q in m_functionData1 select q.Time).Min();
            var date2 = (from q in m_functionData2 select q.Time).Min();
            DateTime lowerBoundDateTime;
            if (date1 > date2)
            {
                lowerBoundDateTime = date1;
            }
            else
            {
                lowerBoundDateTime = date2;
            }
            //
            // get lower bound index
            //
            var intIndex1 = TimeSeriesUtils.GetIndexOfDate(
                m_functionData1,
                lowerBoundDateTime);
            var intIndex2 = TimeSeriesUtils.GetIndexOfDate(
                m_functionData2,
                lowerBoundDateTime);

            //
            // load function data
            //
            lock (m_functionDataLock)
            {
                FunctionData = new List<TsRow2D>(Math.Max(m_functionData1.Count,
                                                            m_functionData2.Count) + 1);
            }
            for (int i = intIndex1, j = intIndex2;
                 i < Math.Max(m_functionData1.Count,
                              m_functionData2.Count);
                 i++, j++)
            {
                if (Math.Abs((m_functionData1[i].Time -
                              m_functionData2[j].Time).Days) > 0)
                {
                    //Debugger.Break();
                    throw new HCException("Dates do not match");
                }
                lock (m_functionDataLock)
                {
                    FunctionData.Add(
                        new TsRow2D(
                            m_functionData1[i].Time,
                            m_functionData1[i].Fx -
                            m_functionData2[j].Fx));
                }
            }
        }
    }
}
