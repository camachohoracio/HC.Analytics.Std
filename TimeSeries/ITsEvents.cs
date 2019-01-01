#region

using System.Collections.Generic;
using HC.Core.DynamicCompilation;
using HC.Core.Resources;

#endregion

namespace HC.Analytics.TimeSeries
{
    public interface ITsEvents : IResource
    {
        List<ITsEvent> TsEventsList { get; set; }
    }
}
