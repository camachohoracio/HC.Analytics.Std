namespace HC.Analytics.TimeSeries.MissingData.Delegates
{
    public delegate bool IsAValidEventDel<T>(
            T tsEvent,
            out T dailyTsEventOut);
}
