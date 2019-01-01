#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Simple moving average function
    /// </summary>
    [Serializable]
    public class SMAvgFunction
    {
        #region Properties

        public int TimeWindow { get; private set; }
        public List<TsRow2D> FunctionData { get; set; }
        public string m_strFunctionName { get; private set; }

        #endregion

        #region Members

        private readonly IncrFinchWeightStats m_finchWeightedStatsNonExpiry;
        private bool blnDynamicLoad;
        private List<TsRow2D> m_baseFunctionData;
        private double m_dblSum;

        #endregion

        #region Constructors

        public SMAvgFunction()
        {
        }

        public SMAvgFunction(
            string strFunctionName,
            List<TsRow2D> baseFunctionData,
            int intTimeWindow)
        {
            m_strFunctionName = strFunctionName;
            FunctionData = new List<TsRow2D>();
            m_finchWeightedStatsNonExpiry = new IncrFinchWeightStats();
            TimeWindow = intTimeWindow;
            m_baseFunctionData = baseFunctionData;
            Reset();

            if (baseFunctionData != null)
            {
                SetMovingAverage();
            }
        }

        #endregion

        #region Public

        public void InsertRow(TsRow2D row)
        {
            if (FunctionData == null)
            {
                FunctionData = new List<TsRow2D>();
            }
            if (m_baseFunctionData == null)
            {
                m_baseFunctionData = new List<TsRow2D>();
                blnDynamicLoad = true;
            }
            else if (m_baseFunctionData.Count == 0)
            {
                blnDynamicLoad = true;
            }

            if (blnDynamicLoad)
            {
                m_baseFunctionData.Add(row);
            }

            m_dblSum += row.Fx;

            int intFunctCount = FunctionData.Count;
            if (intFunctCount < TimeWindow)
            {
                m_finchWeightedStatsNonExpiry.update(1, row.Fx);

                FunctionData.Add(new TsRow2D(row.Time,
                                                     m_finchWeightedStatsNonExpiry.getWeightedMean()));
                return;
            }

            if (double.IsNaN(m_dblSum))
            {
                throw new Exception("Invalid value");
            }


            //
            // remove the first value
            //
            m_dblSum -=
                m_baseFunctionData[intFunctCount - TimeWindow].Fx;
            double dblAverage = m_dblSum/TimeWindow;

            if (double.IsNaN(dblAverage))
            {
                throw new Exception("Invalid average value");
            }

            FunctionData.Add(
                new TsRow2D(
                    row.Time,
                    dblAverage));
        }

        public override string ToString()
        {
            return " SMA(" + TimeWindow + "," +
                   m_strFunctionName + ") ";
        }

        public SMAvgFunction Clone()
        {
            var smAvgFunction =
                new SMAvgFunction(
                    m_strFunctionName,
                    m_baseFunctionData,
                    TimeWindow);

            smAvgFunction.FunctionData =
                CloneTimeSeriesList(
                    FunctionData);

            return smAvgFunction;
        }

        public void Reset()
        {
            m_dblSum = 0;
        }

        public static List<TsRow2D> CloneTimeSeriesList(
            List<TsRow2D> functionData)
        {
            var newFunctionData =
                new List<TsRow2D>(functionData.Count + 1);

            foreach (TsRow2D row in functionData)
            {
                newFunctionData.Add(
                    row.Clone());
            }
            return newFunctionData;
        }

        #endregion

        #region Private

        private void SetMovingAverage()
        {
            FunctionData = new List<TsRow2D>(m_baseFunctionData.Count + 1);

            foreach (TsRow2D row2D in m_baseFunctionData)
            {
                Update(row2D);
            }
        }

        public void Update(TsRow2D row)
        {
            InsertRow(row);
        }

        public bool IsInitialized()
        {
            return m_baseFunctionData.Count >= TimeWindow;
        }

        #endregion
    }
}
