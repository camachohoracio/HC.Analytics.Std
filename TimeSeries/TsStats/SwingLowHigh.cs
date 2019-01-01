using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class SwingLowHigh
    {
        public bool IsReady
        {
            get { return (m_sortedItems.Count >= m_intSize); }
        }
        public double Min
        {
            get { return m_sortedItems.Count == 0 ? 0 :
                    m_sortedItems.First().Value; }
        }
        public double Max
        {
            get { return m_sortedItems.Count == 0 ? 0 :
                    m_sortedItems.Last().Value; }
        }
        public SwingLowHighItem MinItem
        {
            get
            {
                return m_sortedItems.Count == 0 ? null :
                  m_sortedItems.First();
            }
        }
        public SwingLowHighItem MaxItem
        {
            get
            {
                return m_sortedItems.Count == 0 ? null :
                  m_sortedItems.Last();
            }
        }
        public int Count
        {
            get
            {
                return m_sortedItems.Count;
            }
        }

        private readonly int m_intSize;
        private DateTime m_prevDateTime;
        private readonly List<SwingLowHighItem> m_itemsCutLevel;
        private readonly List<SwingLowHighItem> m_nonSortedItems;
        private readonly SortedSet<SwingLowHighItem> m_sortedItems;
        private readonly int m_intCutIndex;

        public SwingLowHigh(int intSize)
        {
            m_intSize = intSize;
            m_itemsCutLevel = new List<SwingLowHighItem>();
            m_sortedItems = 
                new SortedSet<SwingLowHighItem>(
                new SwingLowHighItem());
            m_nonSortedItems = new List<SwingLowHighItem>();
            m_intCutIndex = (int)(intSize*1.0/3.0);
        }

        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            try
            {
                if (dateTime <= m_prevDateTime)
                {
                    return;
                }
                var currItem = new SwingLowHighItem
                {
                    Date = dateTime,
                    Value = dblValue
                };

                m_itemsCutLevel.Add(currItem);

                if (m_itemsCutLevel.Count > m_intCutIndex)
                {
                    SwingLowHighItem itemToAddIntoSortedQueue =
                        m_itemsCutLevel[0];
                    m_itemsCutLevel.RemoveAt(0);
                    m_sortedItems.Add(
                        itemToAddIntoSortedQueue);
                    m_nonSortedItems.Add(itemToAddIntoSortedQueue);

                    if (m_sortedItems.Count > m_intSize)
                    {
                        //
                        // remove the oldest item in all list
                        //
                        SwingLowHighItem firstItem =
                            m_nonSortedItems[0];
                        if (!m_sortedItems.Remove(firstItem))
                        {
                            throw new Exception("Item not found");
                        }
                        if (!m_nonSortedItems.Remove(firstItem))
                        {
                            throw new Exception("Item not found");
                        }
                    }
                }
                m_prevDateTime = dateTime;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
