using System;
using System.Collections.Generic;

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public interface ITrainerWrapper : IDisposable
    {
        int Length { get; }
        int Dimesions { get; }
        double Forecast(double[] xVar);
        List<double> GetErrors();
        List<double> GetForecasts();
        double GetMrse();
        double GetMrse(
            List<double[]> xData, 
            List<double> yData, 
            out List<double> foreasts, 
            out List<double> errors);
    }
}