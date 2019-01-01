using System;
using System.Collections.Generic;

namespace HC.Analytics.TimeSeries
{
    public class GenericTsEvent : ATsEvent, IDisposable
    {
        public Dictionary<string, object> Data { get; set; }
        
        ~GenericTsEvent()
        {
            Dispose();
        }

        public new void Dispose()
        {
            if(Data != null)
            {
                Data.Clear();
                Data = null;
            }
        }
    }
}

