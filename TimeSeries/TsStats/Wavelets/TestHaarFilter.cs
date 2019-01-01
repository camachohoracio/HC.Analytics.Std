#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.Wavelets
{
    public static class TestHaarFilter
    {
        
        public static void TestFilter()
        {
            List<double> data0 = GetTestData();

            var rollingWindowTsFuntion = new HaarFilter();
            var lastDate = new DateTime(1900, 1, 1);
            using (var sw = new StreamWriter(@"c:\HC\Data\WaveletTest.csv"))
            {
                for (int i = 0; i < data0.Count; i++)
                {
                    double dblFilteredValue;
                    double dblNoise;
                    rollingWindowTsFuntion.Filter(
                        lastDate,
                        data0[i],
                        out dblFilteredValue,
                        out dblNoise);
                    lastDate = lastDate.AddMinutes(1);

                    if(rollingWindowTsFuntion.IsReady())
                    {
                        sw.WriteLine(data0[i] + "," + 
                            dblFilteredValue + "," + 
                            dblNoise);
                    }
                }
            }
            Debugger.Break();
        }

        public static List<double> GetTestData()
        {
            var data0 = new List<double>();
            using (var sr = new StreamReader(@"C:\HC\HC.Analytics\TimeSeries\TsStats\Wavelets\amat_close"))
            {
                string strLine;
                while ((strLine = sr.ReadLine()) != null)
                {
                    data0.Add(double.Parse(strLine));
                }
            }
            return data0;
        }
    }
}

