#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.TsStats.Kalman;
using HC.Core.Logging;
using HC.Core.Time;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class TsMissingData
    {
        private const int MISSING_LIMIT = 3;

        public static SortedDictionary<DateTime, T> ProcessEvents<T>(
            SortedDictionary<DateTime, T> tsEvents)
        {
            if (tsEvents == null ||
                tsEvents.Count == 0)
            {
                return new SortedDictionary<DateTime, T>();
            }
            var dateSet = DateHelper.GetDailyWorkingDays(tsEvents.Keys.First(),
                                                         tsEvents.Keys.Last());
            
            var outEvents = new SortedDictionary<DateTime, T>();
            T lastValue = default(T);
            for (int i = 0; i < dateSet.Count; i++)
            {
                var currDate = dateSet[i];
                T currValue;
                if(tsEvents.TryGetValue(currDate, out currValue) &&
                    currValue != null)
                {
                    lastValue = currValue;
                }
                else
                {
                    currValue = lastValue;
                }
                outEvents[currDate] = currValue;
            }
            return outEvents;
        }

        public static SortedDictionary<DateTime, double> ProcessEvents(
            SortedDictionary<DateTime, double> tsEvents)
        {
            if(tsEvents == null ||
                tsEvents.Count == 0)
            {
                return new SortedDictionary<DateTime, double>();
            }

            var dateSet = DateHelper.GetDailyWorkingDays(tsEvents.Keys.First(),
                                                         tsEvents.Keys.Last());
            return ProcessEvents(
                dateSet,
                tsEvents);
        }

    public static List<TsRow2D> ProcessEvents(
            List<DateTime> dateSet,
            TsRow2D[] tsEvents)
    {
        try
        {
        	var tsEventsMap = new SortedDictionary<DateTime, Double>();
        	foreach(TsRow2D tsRow2D in tsEvents)
        	{
        		try
        		{
        			if(tsRow2D != null)
        			{
        				tsEventsMap[tsRow2D.Time] = tsRow2D.Fx;
        			}
	            }
	            catch(Exception ex)
	            {
	                Logger.Log(ex);
	            }
        	}
            SortedDictionary<DateTime, Double> tsEventsMap1 = ProcessEvents(dateSet, tsEventsMap);
        	var results = new List<TsRow2D>();
        	foreach(var kvp in tsEventsMap1)
        	{
        		results.Add(new TsRow2D(
        				kvp.Key, 
        				kvp.Value));
        	}
        	return results;
        }
        catch(Exception ex)
        {
            Logger.Log(ex);
        }
        return new List<TsRow2D>();
    }

        public static SortedDictionary<DateTime, double> ProcessEvents(
            List<DateTime> dateSet,
            SortedDictionary<DateTime, double> tsEvents)
        {
            try
            {
                if (tsEvents.Count == 0)
                {
                    return new SortedDictionary<DateTime, double>();
                }

                var filteredEvents = new SortedDictionary<DateTime, double>();
                var filterOpen = new KalmanFilter();
                int intMissingOpen = 0;

                for (int i = 0; i < dateSet.Count; i++)
                {
                    DateTime dateTime = dateSet[i];
                    double dblCleanValue;
                    double dblValue;
                    if (!tsEvents.TryGetValue(dateTime, out dblValue))
                    {
                        dblCleanValue = Math.Max(
                            Predict(ref filterOpen, ref intMissingOpen), 0);
                    }
                    else
                    {
                        //
                        // open price
                        //
                        if (!IsValid(dblValue))
                        {
                            dblCleanValue = Math.Max(
                                Predict(ref filterOpen, ref intMissingOpen),
                                0);
                        }
                        else
                        {
                            dblCleanValue = dblValue;
                            double dblFilteredValue;
                            double dblNoise;
                            filterOpen.Filter(dblCleanValue,
                                              out dblFilteredValue,
                                              out dblNoise);
                            intMissingOpen = 0;
                        }
                    }

                    dblCleanValue = Math.Max(0, dblCleanValue);
                    filteredEvents[dateTime] = dblCleanValue;
                }
                //
                // remove zeros
                //
                filteredEvents = new SortedDictionary<DateTime, double>((from n in filteredEvents
                                                                         where n.Value > 0
                                                                         select n).ToDictionary(t => t.Key, t => t.Value));

                return filteredEvents;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return tsEvents;
        }

        private static double Predict(
            ref KalmanFilter kalmanFilter,
            ref int intMissingCounter)
        {
            double dblOpen = 0;
            if (kalmanFilter.IsReady())
            {
                dblOpen = kalmanFilter.Predict();
                intMissingCounter++;
            }
            if (intMissingCounter >= MISSING_LIMIT)
            {
                kalmanFilter = new KalmanFilter();
                intMissingCounter = 0;
            }
            return dblOpen;
        }

        public static bool IsValid(double dblValue)
        {
            return !double.IsNaN(dblValue) &&
                     !double.IsInfinity(dblValue) &&
                     Math.Abs(dblValue) > 1e-6;
        }
    }
}
