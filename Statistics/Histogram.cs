#region

using System.IO;
using System.Linq;

#endregion

namespace HC.Analytics.Statistics
{
    public static class Histogram
    {
        public static double[,] GetHistogram(double[] dblDataArray, int intBinCount)
        {
            // get Min and Max values
            double dblMinValue = dblDataArray.Min();
            double dblMaxValue = dblDataArray.Max();
            return GetHistogram(
                dblDataArray,
                intBinCount,
                dblMinValue,
                dblMaxValue);
        }

        public static double[,] GetHistogram(
            double[] dblDataArray,
            int intBinCount,
            double dblMinValue,
            double dblMaxValue)
        {
            double[,] dblHistogramArray = new double[intBinCount,2];
            double dblDelta = (dblMaxValue - dblMinValue)/intBinCount;
            double dblCurrentBinValue = dblMinValue;
            // load bin values
            for (int i = 0; i < intBinCount; i++)
            {
                dblHistogramArray[i, 0] = dblCurrentBinValue;
                dblCurrentBinValue += dblDelta;
            }

            for (int i = 0; i < dblDataArray.Length; i++)
            {
                double dblDataValue = dblDataArray[i];
                double dblIndex = (dblDataValue - dblMinValue - 0.000000001)/dblDelta;
                int intBinIndex = -1;
                if (dblIndex > int.MaxValue)
                {
                    intBinIndex = intBinCount - 1;
                }
                else
                {
                    intBinIndex = (int) ((dblDataValue - dblMinValue - 0.000000001)/dblDelta);
                    if (intBinIndex < 0)
                    {
                        // by default set the initial bin as zero
                        intBinIndex = 0;
                    }
                }
                if (intBinIndex >= intBinCount)
                {
                    intBinIndex = intBinCount - 1;
                }
                dblHistogramArray[intBinIndex, 1]++;
            }
            return dblHistogramArray;
        }

        public static void SaveHistogramToCsvFile(
            double[,] dblHistogramArray,
            string strFileName)
        {
            StreamWriter sw = new StreamWriter(strFileName);
            for (int i = 0; i < dblHistogramArray.GetLength(0); i++)
            {
                sw.WriteLine(dblHistogramArray[i, 0] + "," +
                             dblHistogramArray[i, 1]);
            }
            sw.Close();
        }
    }
}
