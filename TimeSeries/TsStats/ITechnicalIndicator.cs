using System;

namespace HC.Analytics.TimeSeries.TsStats
{
    public interface ITechnicalIndicator : IDisposable
    {
        double Indicator { get; }
        bool IsReady { get; }

        string Name();

        void Update(
            DateTime dateTime,
            double dblClose,
            double dblLow,
            double dblHigh,
            double dblVolume);
    }
}