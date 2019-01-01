namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    public static class AllCointegrationTests
    {
        public static bool IsAnyCointegrated(double[] data,
            double dblConfidence,
            out double dblTStat,
            out double dblTestValue)
        {
            if(DickeyFullerTest.CheckIsStationary0(
                                                       data,
                                                       dblConfidence,
                                                       out dblTStat,
                                                       out dblTestValue))
            {
                return true;
            }
            if(CrdwTest.CheckIsStationary(
                                                       data,
                                                       0.05,
                                                       out dblTStat,
                                                       out dblTestValue))
            {
                return true;
            }
            if(DurbinWatsonTest.CheckIsStationary(
                                                       data,
                                                       0.05,
                                                       out dblTStat,
                                                       out dblTestValue))
            {
                return true;
            }
            return false;
        }

        public static bool IsCointegrated(double[] data)
        {
            double dblTStat;
            double dblTestValue;

            bool blnIsItStationaryDickey = DickeyFullerTest.CheckIsStationary(
                                                       data,
                                                       0.05,
                                                       out dblTStat,
                                                       out dblTestValue);
            bool blnIsItStationaryCrdw = CrdwTest.CheckIsStationary(
                                                       data,
                                                       0.05,
                                                       out dblTStat,
                                                       out dblTestValue);
            bool blnIsItStationaryDurbingWatson = DurbinWatsonTest.CheckIsStationary(
                                                       data,
                                                       0.05,
                                                       out dblTStat,
                                                       out dblTestValue);
            //
            // todo test variance ratio
            //
            return blnIsItStationaryDickey && blnIsItStationaryCrdw &&
                   blnIsItStationaryDurbingWatson;
        }
    }
}

