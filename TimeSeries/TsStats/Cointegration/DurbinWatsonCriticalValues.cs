#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    /// <summary>
    /// Values taken from;
    /// http://home.fuse.net/dgreath/SPSTechnical/tabledw05.htm
    /// Where n = number of observations and 1 independent variable
    /// </summary>
    public class DurbingWatsonCriticalValues
    {
        #region Properties

        public double Confidence { get; set; }
        public int SampleSize { get; set; }
        public double TestValueLower { get; set; }
        public double TestValueUpper { get; set; }
        public static List<DurbingWatsonCriticalValues> CriticalValues { get; private set; }

        #endregion

        static DurbingWatsonCriticalValues()
        {
            CriticalValues = new List<DurbingWatsonCriticalValues>();
            List<string> list = Config.GetDurbingWatsonList();
            foreach (string strLine in list)
            {
                string[] tokens = strLine.Split(',');
                CriticalValues.Add(
                    new DurbingWatsonCriticalValues
                        {
                            Confidence = 0.05,
                            SampleSize = (int) double.Parse(tokens[0]),
                            TestValueLower = double.Parse(tokens[1]),
                            TestValueUpper = double.Parse(tokens[2]),
                        });
            }
        }

        public static bool TestIsStationary(
            double dblTValue,
            double dblConfidence,
            int intSampleSize,
            out double dblTestValue)
        {
            //
            // get the closest sample item
            //
            DurbingWatsonCriticalValues durbingWatsonCriticalValue = FindParams(
                intSampleSize,
                dblConfidence);

            dblTestValue = durbingWatsonCriticalValue.TestValueUpper;
            return dblTValue > dblTestValue;
        }

        public static bool TestIsCorrelated(
            double dblTValue,
            double dblConfidence,
            int intSampleSize,
            out double dblTestValue)
        {
            //
            // get the closest sample item
            //
            DurbingWatsonCriticalValues selectedDickeyFuller = FindParams(
                intSampleSize,
                dblConfidence);

            dblTestValue = selectedDickeyFuller.TestValueLower;
            return dblTValue < dblTestValue;
        }

        private static DurbingWatsonCriticalValues FindParams(
            int intSampleSize,
            double dblConfidence)
        {
            int intMinDiff = int.MaxValue;
            DurbingWatsonCriticalValues selectedDickeyFuller = null;
            foreach (DurbingWatsonCriticalValues dickeyFullerCriticalValuese in CriticalValues)
            {
                int intDiff = Math.Abs(dickeyFullerCriticalValuese.SampleSize - intSampleSize);
                if (intDiff < intMinDiff &&
                    dickeyFullerCriticalValuese.Confidence == dblConfidence)
                {
                    intMinDiff = intDiff;
                    selectedDickeyFuller = dickeyFullerCriticalValuese;
                }
            }
            if (selectedDickeyFuller == null)
            {
                throw new Exception("Item not found");
            }
            return selectedDickeyFuller;
        }
    }
}
