using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public static class TrainerHelper
    {
        public static void BalancePostiveNegatives(
            List<double> yMapTrain,
            List<double[]> xMapTrain,
            List<double> classes,
            out List<double> yMapTrainResult,
            out List<double[]> xMapTrainResult)
        {
            yMapTrainResult = new List<double>();
            xMapTrainResult = new List<double[]>();

            try
            {
                if (yMapTrain.Count != xMapTrain.Count)
                {
                    throw new HCException("Invalid counter");
                }

                List<double> class1ITems = (from n in yMapTrain
                                            where n == classes[0]
                                            select n).ToList();
                List<double> class2ITems = (from n in yMapTrain
                                            where n == classes[1]
                                            select n).ToList();
                if (class1ITems.Count == 0 ||
                    class2ITems.Count == 0)
                {
                    throw new HCException("No items found for class");
                }

                int intToTake = Math.Min(
                    class1ITems.Count,
                    class2ITems.Count);
                double dblClassToCount;
                if (class1ITems.Count < class2ITems.Count)
                {
                    dblClassToCount = classes[1]; // count the other classs!
                }
                else
                {
                    dblClassToCount = classes[0];
                }
                int intCounter = 0;
                for (int i = 0; i < yMapTrain.Count; i++)
                {
                    if (dblClassToCount == yMapTrain[i])
                    {
                        intCounter++;
                    }
                    yMapTrainResult.Add(yMapTrain[i]);
                    xMapTrainResult.Add(xMapTrain[i]);
                    if (intCounter >= intToTake)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
