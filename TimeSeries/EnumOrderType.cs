using HC.Core.Io.KnownObjects.KnownTypes;

namespace HC.Analytics.TimeSeries
{
    [IsAKnownTypeAttr]
    public enum EnumOrderType
    {
        None,
        MarketEnumOrder,
        LimitOrder,
        StopLoss
    }
}
