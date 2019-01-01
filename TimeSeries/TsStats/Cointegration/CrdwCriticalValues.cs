#region

using System;
using System.Collections.Generic;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Cointegration
{
    /// <summary>
    /// Table taken from:
    /// http://www.iei.liu.se/nek/ekonometrisk-teori-7-5-hp-730a07/labbar/1.233753/dfdistab7b.pdf
    /// </summary>
    public class CrdwCriticalValues
    {
        #region Properties

        public double Confidence { get; set; }
        public int SampleSize { get; set; }
        public double TestValue { get; set; }
        public static List<CrdwCriticalValues> CriticalValues { get; private set; }

        #endregion

        static CrdwCriticalValues()
        {
            CriticalValues = new List<CrdwCriticalValues>();
            IEnumerable<string> list = Config.GetCrdwList();
            foreach (string strLine in list)
            {
                string[] tokens = strLine.Split(',');
                CriticalValues.Add(
                    new CrdwCriticalValues
                        {
                            Confidence = double.Parse(tokens[0]),
                            SampleSize = (int) double.Parse(tokens[1]),
                            TestValue = double.Parse(tokens[2]),
                        });
            }
        }

        public static bool TestIsStationary(
            double dblTValue,
            double dblConfidence,
            int intSampleSize,
            out double dblTestValue)
        {
            dblTestValue = 0;
            try
            {
                //
                // get the closest sample item
                //
                CrdwCriticalValues criticalValue = FindParams(
                    intSampleSize,
                    dblConfidence);

                if (criticalValue == null)
                {
                    return false;
                }

                dblTestValue = criticalValue.TestValue;
                return dblTValue > dblTestValue;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }


        private static CrdwCriticalValues FindParams(
            int intSampleSize,
            double dblConfidence)
        {
            try
            {
                int intMinDiff = int.MaxValue;
                CrdwCriticalValues selectedTest = null;
                foreach (CrdwCriticalValues currTest in CriticalValues)
                {
                    int intDiff = Math.Abs(currTest.SampleSize - intSampleSize);
                    if (intDiff < intMinDiff &&
                        currTest.Confidence == dblConfidence)
                    {
                        intMinDiff = intDiff;
                        selectedTest = currTest;
                    }
                }
                return selectedTest;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }
    }
}
