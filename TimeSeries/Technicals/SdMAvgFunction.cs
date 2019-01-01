#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.TimeSeries.Technicals
{
    /// <summary>
    ///   Simple moving average function
    /// </summary>
    [Serializable]
    public class SdMAvgFunction : ATechIndFunction
    {
        #region Properties

        public int TimeWindow
        {
            get { return m_intTimeWindow; }
        }

        public bool IsPositiveBand
        {
            get { return m_blnIsPositiveBand; }
            set { m_blnIsPositiveBand = value; }
        }

        #endregion

        #region Members

        private readonly List<TsRow2D> m_baseFunctionData;
        private readonly double m_dblStdDevDist;
        private readonly int m_intSdSample;
        private readonly int m_intTimeWindow;
        private bool m_blnIsPositiveBand;
        private List<TsRow2D> m_stdList;

        #endregion

        #region Constructors

        public SdMAvgFunction(
            string strFunctionName,
            List<TsRow2D> baseFunctionData,
            int intTimeWindow,
            int intSdSample,
            bool blnIsPositiveBand,
            double dblStdDevDist) :
                this(strFunctionName,
                     "Date",
                     "Price",
                     baseFunctionData,
                     intTimeWindow,
                     intSdSample,
                     blnIsPositiveBand,
                     dblStdDevDist)
        {
        }

        [FunctionConstructorAttr]
        public SdMAvgFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> baseFunctionData,
            int intTimeWindow,
            int intSdSample,
            bool blnIsPositiveBand,
            double dblStdDevDist) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel)
        {
            FunctionName = strFunctionName;
            m_intTimeWindow = intTimeWindow;
            m_intSdSample = intSdSample;
            m_blnIsPositiveBand = blnIsPositiveBand;
            m_baseFunctionData = baseFunctionData;
            m_dblStdDevDist = dblStdDevDist;
            LoadData();
        }

        #endregion

        #region Public

        public void LoadData()
        {
            //
            // get the first date from the moving average
            //
            var lowerBoundDate = m_baseFunctionData[0].Time;

            //
            // get index for lower bound date
            //
            var intIndexFromDate = TimeSeriesUtils.GetIndexOfDate(
                m_baseFunctionData,
                lowerBoundDate);

            var intSmaIndex = 0;
            if (m_intSdSample - 1 > intIndexFromDate)
            {
                intIndexFromDate = m_intSdSample - 1;
                intSmaIndex = TimeSeriesUtils.GetIndexOfDate(
                    m_baseFunctionData,
                    m_baseFunctionData[intIndexFromDate].Time);
            }

            LoadStdDevList(m_baseFunctionData, intIndexFromDate, intSmaIndex);

            var dblStdFactor = (m_blnIsPositiveBand
                                    ? m_dblStdDevDist
                                    : -m_dblStdDevDist);

            //
            // create a new function data list
            //
            lock (m_functionDataLock)
            {
                FunctionData = new List<TsRow2D>(m_baseFunctionData.Count + 1);
            }

            for (int i = intSmaIndex, j = 0;
                 i < m_baseFunctionData.Count;
                 i++, j++)
            {
                if (Math.Abs((m_baseFunctionData[i].Time -
                              m_stdList[j].Time).Days) > 0)
                {
                    //Debugger.Break();
                    throw new HCException("Dates do not match");
                }

                lock (m_functionDataLock)
                {
                    FunctionData.Add(
                        new TsRow2D(
                            m_baseFunctionData[i].Time,
                            dblStdFactor*m_stdList[j].Fx +
                            m_baseFunctionData[i].Fx));
                }
            }

            //
            // set the new limits for this function
            //
            SetFunctionLimits();
        }

        public override string ToString()
        {
            return " SDMA(" + m_intTimeWindow + "," +
                   (m_blnIsPositiveBand
                        ? "+"
                        : "-") + "," +
                   FunctionName + ") ";
        }

        public SdMAvgFunction Clone()
        {
            var sdMAvgFunction =
                new SdMAvgFunction(
                    FunctionName,
                    m_baseFunctionData,
                    m_intTimeWindow,
                    m_intSdSample,
                    m_blnIsPositiveBand,
                    m_dblStdDevDist);

            sdMAvgFunction.m_stdList =
                new List<TsRow2D>(m_stdList);

            lock (m_functionDataLock)
            {
                sdMAvgFunction.FunctionData =
                    TimeSeriesUtils.CloneTimeSeriesList(
                        FunctionData);
            }

            return sdMAvgFunction;
        }

        public override void Update(TsRow2D row2D)
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        private void LoadStdDevList(
            List<TsRow2D> functionData,
            int intIndexFromDate,
            int intSmaIndex)
        {
            if (m_stdList == null)
            {
                m_stdList = new List<TsRow2D>(functionData.Count + 1);
                for (var i = intIndexFromDate;
                     i < functionData.Count;
                     i++, intSmaIndex++)
                {
                    if (Math.Abs((m_baseFunctionData[intSmaIndex].Time -
                                  functionData[i].Time).Days) > 0)
                    {
                        throw new HCException("Dates do not match");
                    }
                    //
                    // get mean
                    //
                    double dblSum = 0;
                    for (var j = 0; j < m_intSdSample; j++)
                    {
                        dblSum += functionData[i - j].Fx;
                    }
                    var dblMean = dblSum/m_intSdSample;
                    //
                    // get std dev
                    //
                    double dblSumSq = 0;
                    for (var j = 0; j < m_intSdSample; j++)
                    {
                        dblSumSq += Math.Pow(functionData[i - j].Fx - dblMean, 2);
                    }
                    var dblStdDev = Math.Sqrt(dblSumSq/(m_intSdSample - 1));
                    m_stdList.Add(
                        new TsRow2D(
                            functionData[i].Time,
                            dblStdDev));
                }
            }
        }

        #endregion
    }
}
