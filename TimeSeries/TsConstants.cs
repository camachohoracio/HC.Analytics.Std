#region


#endregion

using HC.Core.DynamicCompilation;

namespace HC.Analytics.TimeSeries
{
    public delegate void PublishTsPublisherDelegate(TsEventPublisher tsEvent);
    public delegate void PublishTsDelegate(ITsEvent tsEvent);
}
