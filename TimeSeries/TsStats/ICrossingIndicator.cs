using System;

namespace HC.Analytics.TimeSeries.TsStats
{
    public interface ICrossingIndicator
    {
        bool GetIsGoingUp(double dblStochasticSlow, double dblStochasticFast);
        DateTime TimeOfCrossing { get; set; }
        bool GetLastCrossIsGoingUp();
    }
}