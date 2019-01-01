using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class SortedRollingWindow
    {
        public SortedSet<double> Items { get; private set; }

        private readonly int m_intCapacity;

        public bool IsReady => Items.Count >= m_intCapacity;

        public SortedRollingWindow(int intCapacity)
        {
            Items =
            new SortedSet<double>();
            m_intCapacity = intCapacity;
        }

        public void Update(double dblValue)
        {
            try
            {
                Items.Add(dblValue);
                if (Items.Count > m_intCapacity)
                {
                    Items.Remove(
                        Items.Last());
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
