
using System;
using HC.Core.DynamicCompilation;

namespace HC.Analytics.TimeSeries
{
    /// <summary>
    /// Observer pattern. It forwards events to a set of delegates
    /// </summary>
    public class TsEventPublisher : IDisposable
    {
        public TsDataRequest TsDataRequest { get; set; }
        public ITsEvent TsEvent { get; set; }

        public void Dispose()
        {
            TsDataRequest = null;
            TsEvent = null;
        }
    }
}

