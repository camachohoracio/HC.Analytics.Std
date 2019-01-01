#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.TimeSeries.MissingData.Delegates;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.TimeSeries.MissingData
{
    public static class MissingDataCalc
    {
        public static void CheckMissingEvents<T>(
            SortedDictionary<string, List<T>> mapSymbolToDailyTsEvent,
            GenerateMissingEventDel<T> generateMissingEventDel,
            UpdateEventDel<T> updateEventDel,
            ResetDel resetDel,
            IsAValidEventDel<T> isAValidEventDel) where T : ITsEvent
        {
            if (mapSymbolToDailyTsEvent.Count == 0)
            {
                return;
            }
            //
            // check repeated events
            //
            foreach (KeyValuePair<string, List<T>> keyValuePair in mapSymbolToDailyTsEvent.ToArray())
            {
                mapSymbolToDailyTsEvent[keyValuePair.Key] = CheckRepeatedDateEvents(keyValuePair.Value);
            }

            //
            // remove lists with too few items
            //
            //RemoveLowFreqLists(mapSymbolToDailyTsEvent);

            //
            // first pass, get the first element in the list
            //
            List<T> baseList = mapSymbolToDailyTsEvent.First().Value;
            foreach (var kvp in mapSymbolToDailyTsEvent.ToList())
            {
                List<T> events = kvp.Value;
                List<DateTime> dateList = GetMergedDateList(
                    baseList,
                    events);
                baseList = CheckMisingData(
                    baseList, 
                    dateList,
                    generateMissingEventDel,
                    updateEventDel,
                    resetDel,
                    isAValidEventDel);

                events = CheckMisingData(
                    events, 
                    dateList,
                    generateMissingEventDel,
                    updateEventDel,
                    resetDel,
                    isAValidEventDel);
                mapSymbolToDailyTsEvent[kvp.Key] = events;
            }
            //
            // second pass, get the largest possible list of events
            //
            baseList = mapSymbolToDailyTsEvent.Last().Value;
            foreach (var kvp in mapSymbolToDailyTsEvent.ToList())
            {
                List<T> events = kvp.Value.ToList();
                var dateList = GetMergedDateList(
                    baseList,
                    events);
                baseList = CheckMisingData(
                    baseList,
                    dateList,
                    generateMissingEventDel,
                    updateEventDel,
                    resetDel,
                    isAValidEventDel);

                events = CheckMisingData(
                    events,
                    dateList,
                    generateMissingEventDel,
                    updateEventDel,
                    resetDel,
                    isAValidEventDel);
                if (baseList.Count != events.Count)
                {
                    throw new HCException("Invalid list size");
                }
                mapSymbolToDailyTsEvent[kvp.Key] = events;
            }
        }

        private static List<DateTime> GetMergedDateList<T>(
            List<T> list1,
            List<T> list2) where T :ITsEvent
        {
            var dateList = new List<DateTime>(from n in list1 select n.Time);
            dateList.AddRange(from n in list2 select n.Time);
            dateList = dateList.Distinct().ToList();
            dateList.Sort();
            return dateList;
        }

        public static List<T> CheckMisingData<T>(
            List<T> list,
            List<DateTime> dateList,
            GenerateMissingEventDel<T> generateMissingEventDel,
            UpdateEventDel<T> updateEventDel,
            ResetDel resetDel,
            IsAValidEventDel<T> isAValidEventDel) where T : ITsEvent
        {
            resetDel();
            var outList = new List<T>();
            int intJSize = list.Count;
            int j = 0;
            for (int i = 0; i < dateList.Count; i++)
            {
                T currTsEvent = list[j];
                DateTime iDate = dateList[i];
                DateTime jDate = currTsEvent.Time;
                if (iDate != jDate)
                {
                    //
                    // copy last valid event and insert
                    //
                    currTsEvent = generateMissingEventDel(iDate, j, list);
                }
                else
                {
                    isAValidEventDel(
                        currTsEvent,
                        out currTsEvent);
                    //
                    // same date, go to next index
                    //
                    if (j < intJSize - 1)
                    {
                        j++;
                    }
                }
                updateEventDel(currTsEvent);
                outList.Add(currTsEvent);
            }
            resetDel();

            //
            // check dates
            //
            if(outList.Count != dateList.Count)
            {
                throw new HCException("Invalid event count");
            }
            for (int i = 0; i < dateList.Count; i++)
            {
                if(outList[i].Time != dateList[i])
                {
                    throw new HCException("Invalid times");
                }
            }
            return outList;
        }

        private static void RemoveLowFreqLists<T>(
            SortedDictionary<string, List<T>> mapSymbolToDailyTsEvent)
        {
            int intMaxCounter = (from n in mapSymbolToDailyTsEvent.Values
                                 select n.Count).Max();
            var intCountLimit = (int)(intMaxCounter * 0.5);
            var itemsToDelete = (from n in mapSymbolToDailyTsEvent
                                 where n.Value.Count < intCountLimit
                                 select n.Key).ToList();
            foreach (string strSymbol in itemsToDelete)
            {
                mapSymbolToDailyTsEvent.Remove(strSymbol);
            }
        }

        private static List<T> CheckRepeatedDateEvents<T>(
            List<T> tsEvents) where T : ITsEvent
        {
            var dateValidator = new HashSet<DateTime>();
            var outList = new List<T>();
            foreach (T tsEvent in tsEvents)
            {
                if (!dateValidator.Contains(tsEvent.Time))
                {
                    dateValidator.Add(tsEvent.Time);
                    outList.Add(tsEvent);
                }
            }
            return outList;
        }
    }
}

