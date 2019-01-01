using HC.Core.Io.KnownObjects.KnownTypes;

namespace HC.Analytics.TimeSeries.Technicals
{
    [IsAKnownTypeAttr]
    public enum EnumTechIndType
    {
        None,
        ExMa,
        LwMa,
        SMa,
        RegressDiff,
        KalmanDiff,
        MaxVal,
        MinVal,
        SMaReturn,
        RegressDiffReturn,
        KalmanDiffReturn,
        MaxValReturn,
        MinValReturn,
        Pca1,
        Pca2,
        Pca3,
        CurrPrice,
        CurrVolume,
        Regress,
        RegressReturn,
        PriceAt,
        VolumeAt,
        SMaVol,
        RegressVol,
        RegressDiffVol,
        KalmanDiffVol,
        MaxValVol,
        MinValVol,
        Forecast,
        ForecastMeanStdDev
    }
}
