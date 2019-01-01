using System;
using System.Collections.Generic;

namespace HC.Analytics.TimeSeries.MissingData.Delegates
{
    public delegate T GenerateMissingEventDel<T>(
        DateTime missingDate,
        int intIndex,
        List<T> tsEvents);
}
