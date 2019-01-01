#region

using System;
using System.Linq;
using System.Collections.Generic;
using HC.Core.DynamicCompilation;
using HC.Core.Logging;
using HC.Core.Reflection;

#endregion

namespace HC.Analytics.TimeSeries
{
    public static class TimeSeriesUtils
    {
        private static readonly TimeSeriesDateComparer m_dateComparer = new TimeSeriesDateComparer();

        private static readonly TsEventDateComparator m_dateComparator =
            new TsEventDateComparator();

        public static int FindIndexBeforeDate(
            List<ITsEvent> functionData,
            DateTime date)
        {
            return Math.Max(0, FindIndexAfterDate(
                functionData,
                date) - 1);
        }

        public static int FindIndexAfterDate(
            List<ITsEvent> functionData,
            DateTime date)
        {
            int intLength = functionData.Count;
            if (intLength == 0 || functionData[0].Time > date)
            {
                return 0;
            }

            if (functionData[intLength - 1].Time < date)
            {
                return intLength - 1;
            }
            for (int i = 0; i < intLength; i++)
            {
                if (functionData[i].Time > date)
                {
                    return i;
                }
            }
            return intLength - 1;
        }

        public static int FindIndexAfterDate(
            List<DateTime> functionData,
            DateTime date)
        {
            int intLength = functionData.Count;
            if (intLength == 0 || functionData[0] > date)
            {
                return 0;
            }

            if (functionData[intLength - 1] < date)
            {
                return intLength - 1;
            }
            for (int i = 0; i < intLength; i++)
            {
                if (functionData[i] > date)
                {
                    return i;
                }
            }
            return intLength - 1;
        }

        public static int FindIndexFloorDate(
            List<DateTime> functionData,
            DateTime date)
        {
            int intLength = functionData.Count;
            if (intLength == 0 || functionData[0] > date)
            {
                return 0;
            }

            if (functionData[intLength - 1] < date)
            {
                return intLength - 1;
            }
            for (int i = 0; i < intLength; i++)
            {
                if (functionData[i] == date)
                {
                    return i;
                }
                if (functionData[i] > date)
                {
                    return Math.Max(0, i - 1);
                }
            }
            return intLength - 1;
        }

        public static int FindIndex(
            List<ITsEvent> functionData,
            DateTime date)
        {
            int intLength = functionData.Count;
            if (intLength == 0 || functionData[0].Time > date)
            {
                return 0;
            }

            if (functionData[intLength - 1].Time < date)
            {
                return intLength - 1;
            }
            double dblDistance = Double.MaxValue;
            for (int i = 0; i < intLength; i++)
            {
                double dblCurrDist = Math.Abs((date - functionData[i].Time).TotalMilliseconds);
                if (dblCurrDist < dblDistance)
                {
                    dblDistance = dblCurrDist;
                }
                else
                {
                    //
                    // the distance is greater
                    //
                    return i - 1;
                }
            }
            return intLength - 1;
        }

        public static int FindIndex(
            List<TsRow2D> functionData,
            DateTime date)
        {
            var tsEvents = functionData.Cast<ITsEvent>();
            return FindIndex(tsEvents.ToList(), date);
        }

        public static int GetIndexOfDate<T>(
            List<T> functionData,
            DateTime date) where T : ATsEvent
        {
            try
            {
                T item = (T) ReflectorCache.GetReflector(
                    typeof (T)).CreateInstance();
                item.Time = date;
                int intIndex = Math.Abs(functionData.BinarySearch(
                    item,
                    m_dateComparator));
                if (intIndex >= functionData.Count)
                {
                    intIndex = functionData.Count - 1;
                }
                return intIndex;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static int GetIndexOfDate(
            List<TsRow2D> functionData,
            DateTime date)
        {
            try
            {
                int intIndex = Math.Abs(functionData.BinarySearch(
                    new TsRow2D(date, -1),
                    m_dateComparer));
                if (intIndex >= functionData.Count)
                {
                    intIndex = functionData.Count - 1;
                }
                return intIndex;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static int GetIndexOfDate(
            List<DateTime> functionData,
            DateTime date)
        {
            try
            {
                if (functionData == null || functionData.Count == 0)
                {
                    return -1;
                }

                return Math.Max(0, Math.Min(
                    functionData.Count - 1,
                    Math.Abs(functionData.BinarySearch(
                        date,
                        new SimpleDateComparer()))));
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return -1;
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
    }
}
