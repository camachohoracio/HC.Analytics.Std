using System;

namespace HC.Analytics.TimeSeries
{
    public class ScrappedTsEvent : ATsEvent
    {
        public DateTime CreatedTime { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        
        public ScrappedTsEvent()
        {
            CreatedTime = DateTime.Now;
            Time = DateTime.Today;
        }

        ~ScrappedTsEvent()
        {
            Dispose();
        }

        public new void Dispose()
        {
            Symbol = null;
            Description = null;
            Description = null;
        }
    }
}
