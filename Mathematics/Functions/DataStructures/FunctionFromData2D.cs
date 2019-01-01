#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    public class FunctionFromData2D : AFunction2D
    {
        #region Members

        protected readonly object m_functionDataLock = new object();
        private readonly double m_dblDefaultYValueMax;
        private readonly double m_dblDefaultYValueMin;
        private readonly FunctionBinarySearchHelper m_functionBinarySearchHelperX;
        private readonly FunctionBinarySearchHelper m_functionBinarySearchHelperY;
        protected readonly List<FuncRow2D> m_functionData;
        private readonly int m_intMaxItemsSize;

        #endregion

        #region Constructors

        public FunctionFromData2D(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<FuncRow2D> functionData,
            double dblDefaultYValueMin,
            double dblDefaultYValueMax) : this(
            strFunctionName,
            strXLabel,
            strYLabel,
            functionData,
            dblDefaultYValueMin,
            dblDefaultYValueMax, 
            0)
        {
        }

        public FunctionFromData2D(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<FuncRow2D> functionData,
            double dblDefaultYValueMin,
            double dblDefaultYValueMax,
            int intMaxItemsSize)
        {
            m_intMaxItemsSize = intMaxItemsSize;
            FunctionName = strFunctionName;
            m_functionData = functionData;
            XLabel = strXLabel;
            YLabel = strYLabel;
            m_dblDefaultYValueMin = dblDefaultYValueMin;
            m_dblDefaultYValueMax = dblDefaultYValueMax;
            m_functionBinarySearchHelperX = new FunctionBinarySearchHelper();
            m_functionBinarySearchHelperX.OnEvauateFunctionGiven += EvaluateYValue;
            m_functionBinarySearchHelperX.OnEvauateFunctionSearch += EvaluateXValue;

            m_functionBinarySearchHelperY = new FunctionBinarySearchHelper();
            m_functionBinarySearchHelperY.OnEvauateFunctionGiven += EvaluateXValue;
            m_functionBinarySearchHelperY.OnEvauateFunctionSearch += EvaluateYValue;
        }

        #endregion

        private void InitializeValues()
        {
            XMin = double.MaxValue;
            XMax = -double.MaxValue;
            YMin = double.MaxValue;
            YMax = -double.MaxValue;
        }

        private double EvaluateXValue(int intIndex)
        {
            return m_functionData[intIndex].X;
        }

        private double EvaluateYValue(int intIndex)
        {
            return m_functionData[intIndex].Fx;
        }

        public override double EvaluateFunction(double dblX)
        {
            if (m_functionData.Count == 0)
            {
                return 0.0;
            }
            var qX = (from n in m_functionData
                      select n.X);

            //var qY = (from n in m_functionData
            //          select n.Fx);

            double dblXMin = qX.Min();
            double dblXMax = qX.Max();

            //
            // validate Min and Max values
            //
            if (dblX < dblXMin)
            {
                return m_dblDefaultYValueMin;
            }

            if (dblX > dblXMax)
            {
                return m_dblDefaultYValueMax;
            }

            if (dblX < 0)
            {
                //Debugger.Break();
            }
            return m_functionBinarySearchHelperX.DoBinarySearchWithInterpolaion(
                dblX,
                m_functionData);
        }

        public double EvaluateFunctionInverse(double dblY)
        {
            if (m_functionData.Count == 0)
            {
                return 0.0;
            }

            var qY = (from n in m_functionData
                      select n.Fx);

            double dblYMin = qY.Min();
            double dblYMax = qY.Max();


            //
            // validate Min and Max values
            //
            if (dblY < dblYMin)
            {
                throw new HCException("Error. Function value not defined");
            }

            if (dblY > dblYMax)
            {
                throw new HCException("Error. Function value not defined");
            }

            return m_functionBinarySearchHelperY.DoBinarySearchWithInterpolaion(
                dblY,
                m_functionData);
        }

        public void Add(FuncRow2D row)
        {
            //var qX = (from n in m_functionData
            //          select n.X);

            //var qY = (from n in m_functionData
            //          select n.Fx);
            double dblX = row.X;
            double dblY = row.Fx;
            XMin = Math.Min(dblX, XMin);
            XMax = Math.Max(dblX, XMax);
            YMin = Math.Min(dblY, YMin);
            YMax = Math.Max(dblY, YMax);

            lock(m_functionDataLock)
            {
                m_functionData.Add(row);
                if(m_intMaxItemsSize > 0 &&
                    m_functionData.Count > m_intMaxItemsSize)
                {
                    m_functionData.RemoveAt(0);
                }
            }
        }

        public override void SetFunctionLimits()
        {
            return;
            if(m_functionData.Count == 0)
            {
                return;
            }

            var qX = (from n in m_functionData
                      select n.X);

            var qY = (from n in m_functionData
                      select n.Fx);

            XMin = qX.Min();
            XMax = qX.Max();
            YMin = qY.Min();
            YMax = qY.Max();
        }

        public override List<FuncRow2D> GetFunctionData()
        {
            return m_functionData;
        }

        public override string ToString()
        {
            return FunctionName;
        }

        public FuncRow2D this[int intIndex]
        {
            get
            {
                lock (m_functionDataLock)
                {
                    return m_functionData[intIndex];
                }
            }
        }

        public List<FuncRow2D> ToList()
        {
            lock (m_functionDataLock)
            {
                return m_functionData.ToList();
            }
        }

        public void Clear()
        {
            lock (m_functionDataLock)
            {
                m_functionData.Clear();
            }
        }

        public FuncRow2D Last()
        {
            lock (m_functionDataLock)
            {
                return m_functionData.Last();
            }
        }

        public void AddRange(List<FuncRow2D> currentTimeSeriesEvents)
        {
            lock (m_functionDataLock)
            {
                m_functionData.AddRange(currentTimeSeriesEvents);
            }
        }
    }
}
