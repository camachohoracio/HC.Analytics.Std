#region

using System;
using System.Collections.Generic;
using HC.Core;
using HC.Core.DynamicCompilation;
using HC.Core.Resources;

#endregion

namespace HC.Analytics.TimeSeries
{
    [Serializable]
    public class TsEvents : ITsEvents
    {
        #region Properties

        public List<ITsEvent> TsEventsList { get; set; }
        public IDataRequest DataRequest { get; set; }
        public DateTime TimeUsed { get; set; }
        public object Owner { get; set; }
        public bool HasChanged { get; set; }

        #endregion

        #region Constructor

        public TsEvents()
        {
            TsEventsList = new List<ITsEvent>();
        }

        #endregion

        #region Public

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (TsEventsList != null)
            {
                TsEventsList.Clear();
                TsEventsList = null;
            }
        }

        public void Close()
        {
            Dispose();
        }

        #endregion
    }
}
