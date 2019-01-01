using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class BasicRollingWindowInverted<T>
    {
        public List<T> Items { get; private set; }

        private readonly int m_intCapacity;

        public bool IsReady => Items.Count >= m_intCapacity;
        public int Count => Items.Count;

        public BasicRollingWindowInverted(int intCapacity)
        {
            Items =
            new List<T>(intCapacity + 2);
            m_intCapacity = intCapacity;
        }

        public void Update(T dblValue)
        {
            try
            {
                Items.Add(dblValue);
                if (Items.Count > m_intCapacity)
                {
                    Items.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public T this[int index]
        {
            get { return Items[
                Items.Count - index - 1]; }
            set { Items[Items.Count - index - 1] = value; }
        }

        public T Last()
        {
            if (Items.Count == 0)
            {
                return default(T);
            }
            return Items.Last();
        }
    }
}
