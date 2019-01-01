#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using HC.Analytics.Mathematics;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;
using HC.Core.Logging;
using HC.Core.Resources;

#endregion

namespace HC.Analytics.TimeSeries
{
    [Serializable]
    public class TsFunction : AFunction2D, ITsEvents
    {
        #region Members

        private List<ITsEvent> m_tsEvents;
        public List<TsRow2D> FunctionData { get; set; }
        protected readonly object m_functionDataLock = new object();
        private DateTime m_prevDate;
        private int m_intDateCounter;
        
        #endregion

        #region Properties

        public int SeedTimeSize
        {
            get;
            private set;
        }

        public DateTime MaxDate { get; set; }
        public DateTime MinDate { get; set; }

        public List<ITsEvent> TsEventsList
        {
            get
            {
                if (m_tsEvents == null)
                {
                    lock (m_functionDataLock)
                    {
                        m_tsEvents = new List<ITsEvent>(
                            from n in FunctionData select (ITsEvent) n);
                    }
                }
                return m_tsEvents;
            }
            set { m_tsEvents = value; }
        }

        public Object Owner { get; set; }

        public object DataRequestObj { get; set; }

        /// <summary>
        ///   Time series name
        /// </summary>
        [XmlIgnore]
        public IDataRequest DataRequest
        {
            get { return (IDataRequest) DataRequestObj; } // hack to add the request to the cache
            set { DataRequestObj = value; }
        }

        /// <summary>
        ///   Keeps track of how long this object has been living in the memory.
        ///   The ELT pool maintains the number of objects in the memory.
        ///   Older object are less likely to be included in the memory.
        /// </summary>
        public DateTime TimeUsed { get; set; }

        /// <summary>
        ///   True = The ELT has been chaged
        /// </summary>
        public bool HasChanged { get; set; }

        public int Count
        {
            get
            {
                lock (m_functionDataLock)
                {
                    return FunctionData.Count;
                }
            }
        }

        #endregion

        #region Constructors

        public TsFunction()
        {
            SetDefaultValues();
        }

        public TsFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> functionData) :
            this(strFunctionName,strXLabel,strYLabel,functionData,0)
        {
        }

        public TsFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel,
            List<TsRow2D> functionData,
            int intItemsLimitSize)
        {
            m_intItemsLimitSize = intItemsLimitSize;
            //
            // set default values
            //
            SetDefaultValues();

            FunctionName = strFunctionName;
            FunctionData = functionData;
            XLabel = strXLabel;
            YLabel = strYLabel;
            SetFunctionLimits();
        }

        private void SetDefaultValues()
        {
            XMin = 0;
            XMax = 0;
            YMin = double.MaxValue;
            YMax = -double.MaxValue;
            MinDate = DateTime.MaxValue;
            MaxDate = DateTime.MinValue;
        }

        #endregion

        #region ITsEvents Members

        public void Close()
        {
            Dispose();
        }

        ~TsFunction()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
        }

        #endregion

        public int GetIndexOfDate(
            DateTime date)
        {
            lock (m_functionDataLock)
            {
                return TimeSeriesUtils.GetIndexOfDate(
                    FunctionData,
                    date);
            }
        }

        public override double EvaluateFunction(double dblX)
        {
            HCException.Throw("Error. Function not enabled.");
            return double.NaN;
        }

        public double EvaluateFunction(
            DateTime x)
        {
            if (MinDate > x ||
                MaxDate < x)
            {
                if (MaxDate < x)
                {
                    lock (m_functionDataLock)
                    {
                        return FunctionData[
                            FunctionData.Count - 1].Fx;
                    }
                }
                if (MinDate > x)
                {
                    lock (m_functionDataLock)
                    {
                        return FunctionData[
                            0].Fx;
                    }
                }
                ////Debugger.Break();
                //throw new HCException("DateWrapper out of range.");
            }
            var intIndex = GetIndexOfDate(x);
            if (intIndex < 0)
            {
                //
                // index value was found
                //
                lock (m_functionDataLock)
                {
                    return FunctionData[intIndex].Fx;
                }
            }
            return EvaluateFunction(Math.Abs(intIndex));
        }

        public double EvaluateFunction(
            DateTime x,
            int intIndex)
        {
            if (MinDate > x ||
                MaxDate < x)
            {
                if (MaxDate < x)
                {
                    lock (m_functionDataLock)
                    {
                        return FunctionData[
                            FunctionData.Count - 1].Fx;
                    }
                }
                if (MinDate > x)
                {
                    lock (m_functionDataLock)
                    {
                        return FunctionData[
                            0].Fx;
                    }
                }
                ////Debugger.Break();
                //throw new HCException("DateWrapper out of range.");
            }

            //
            // do linear interpolation
            //
            DateTime lowDate;
            lock (m_functionDataLock)
            {
                lowDate =
                    FunctionData[intIndex - 1].Time;
            }
            DateTime highDate;
            lock (m_functionDataLock)
            {
                highDate =
                    FunctionData[intIndex].Time;
            }
            HCException.ThrowIfTrue(lowDate > highDate,
            "DateWrapper interpolation error.");
            
            HCException.ThrowIfTrue(x < lowDate,
                "DateWrapper interpolation error.");
            
            HCException.ThrowIfTrue(x > highDate,
                "DateWrapper interpolation error.");

            double dblTargetDateValue = x.Ticks;
            double dblPreviousDateValue = lowDate.Ticks;
            double dblNextDateValue = highDate.Ticks;

            double dblPreviousValue;
            double dblNextValue;
            lock (m_functionDataLock)
            {
                dblPreviousValue =
                    FunctionData[intIndex].Fx;
                dblNextValue =
                    FunctionData[intIndex].Fx;
            }


            return MathHelper.DoLinearInterpolation(
                dblTargetDateValue,
                dblPreviousDateValue,
                dblPreviousValue,
                dblNextDateValue,
                dblNextValue);
        }

        public double EvaluateFunctionInverse(double dblY)
        {
            HCException.Throw("Error. Function not enabled.");
            return double.NaN;
        }


        public void Add(TsRow2D tsRow2D)
        {
            try
            {
                DateTime currDate = tsRow2D.Time;
                if (currDate > m_prevDate)
                {
                    m_intDateCounter++;
                    m_prevDate = currDate;
                }
                if (currDate > MaxDate)
                {
                    MaxDate = currDate;
                }
                if (currDate < MinDate)
                {
                    MinDate = currDate;
                }
                XMin = 0;
                XMax = m_intDateCounter;
                double dblCurrY = tsRow2D.Fx;
                YMin = dblCurrY < YMin ? dblCurrY : YMin;
                YMax = dblCurrY > YMax ? dblCurrY : YMax;
                if (MaxDate.Year > 100)
                {
                    SetDefaultXMax(MaxDate.ToOADate());
                }
                lock (m_functionDataLock)
                {
                    if (FunctionData == null)
                    {
                        FunctionData = new List<TsRow2D>();
                    }

                    FunctionData.Add(tsRow2D);
                    if (m_intItemsLimitSize > 0 &&
                        FunctionData.Count > m_intItemsLimitSize)
                    {
                        FunctionData.RemoveAt(0);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override void SetFunctionLimits()
        {
            return;
            DoLoadFunctionLimits();
        }

        public void DoLoadFunctionLimits()
        {
            if (FunctionData != null)
            {
                if (FunctionData.Count == 0)
                {
                    return;
                }

                DateTime minDate = DateTime.MaxValue;
                DateTime maxDate = DateTime.MinValue;
                double dblMinY = double.MaxValue;
                double dblMaxY = -double.MaxValue;
                DateTime prevDate = DateTime.MinValue;
                int intDateCounter = 0;
                for (int i = 0; i < FunctionData.Count; i++)
                {
                    var tsRow = FunctionData[i];
                    DateTime currDate = tsRow.Time;
                    double dblCurrY = tsRow.Fx;
                    if (currDate > maxDate)
                    {
                        maxDate = currDate;
                    }
                    if (currDate < minDate)
                    {
                        minDate = currDate;
                    }
                    if (dblCurrY < dblMinY)
                    {
                        dblMinY = dblCurrY;
                    }
                    if (dblCurrY > dblMaxY)
                    {
                        dblMaxY = dblCurrY;
                    }
                    if (prevDate < currDate)
                    {
                        prevDate = currDate;
                        intDateCounter++;
                    }
                }

                //var qY = (from n in m_functionData
                //          select n.Fx);

                //var qX = (from n in m_functionData
                //          select n.Time);

                //
                // count the number of unique dates
                //
                //var dateValidator =
                //    new Dictionary<DateTime, object>();
                //foreach (DateTime dateTime in qX)
                //{
                //    if (!dateValidator.ContainsKey(dateTime))
                //    {
                //        dateValidator.Add(dateTime, null);
                //    }
                //}

                XMin = 0;
                XMax = intDateCounter;
                YMin = dblMinY;
                YMax = dblMaxY;
                MinDate = minDate;
                MaxDate = maxDate;
            }
        }

        public virtual List<TsRow2D> LoadTimeSeriesList()
        {
            lock (m_functionDataLock)
            {
                return FunctionData.ToList();
            }
        }

        public override string ToString()
        {
            return FunctionName;
        }

        public TsRow2D this[int intIndex]
        {
            get
            {
                lock (m_functionDataLock)
                {
                    return FunctionData[intIndex];
                }
            }
        }

        public List<TsRow2D> ToList()
        {
            lock (m_functionDataLock)
            {
                return FunctionData.ToList();
            }
        }

        public void Clear()
        {
            lock (m_functionDataLock)
            {
                FunctionData.Clear();
            }
        }

        public TsRow2D Last()
        {
            lock (m_functionDataLock)
            {
                return FunctionData.Last();
            }
        }

        public void AddRange(List<TsRow2D> currentTimeSeriesEvents)
        {
            lock (m_functionDataLock)
            {
                FunctionData.AddRange(currentTimeSeriesEvents);
            }
        }

        public virtual void Update(TsRow2D tsRow2D)
        {
            throw new NotImplementedException();
        }

        public TsFunction Clone()
        {
            try
            {
                var newInstance = (TsFunction) MemberwiseClone();
                if (m_tsEvents != null)
                {
                    newInstance.m_tsEvents = m_tsEvents.ToList();
                }
                if (FunctionData != null)
                {
                    newInstance.FunctionData = FunctionData.ToList();
                }
                return newInstance;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public void SetSeedTimeSize(int intSeedTimeSize)
        {
            SeedTimeSize = intSeedTimeSize;
        }
    }
}
