
using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics;
using HC.Core.Exceptions;
using HC.Core.Logging;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowOutliersFast : IDisposable
    {
        public bool NoReturns { get; set; }

        private SortedDictionary<SortedItem, object> m_sortedData =
            new SortedDictionary<SortedItem, object>(
                new DuplicateKeyComparer<SortedItem>());
        private SortedDictionary<SortedItem, object> m_sortedDataReturns =
            new SortedDictionary<SortedItem, object>(
                new DuplicateKeyComparer<SortedItem>());

        private List<SortedItem> m_data =
            new List<SortedItem>();
        private List<SortedItem> m_dataListReturns =
            new List<SortedItem>();

        private readonly double m_dblThreshold;
        private readonly int m_intSampleSize;
        private int m_intCounter;
        private bool m_blnOutlayerMode;
        private double m_dblPrevValue;

        public RollingWindowOutliersFast(
            double dblThreshold)
            : this(
                Outliers.DEFAULT_SAMPLE_SIZE,
                dblThreshold) { }

        public RollingWindowOutliersFast() : this(
            Outliers.DEFAULT_SAMPLE_SIZE,
            Outliers.DEFAULT_OUTLIER_THRESHOLD){}


        public RollingWindowOutliersFast(
            int intSampleSize,
            double dblThreshold)
        {
            try
            {
                m_intSampleSize = intSampleSize;
                m_dblThreshold = dblThreshold;
                if (intSampleSize < Outliers.DEFAULT_SAMPLE_SIZE)
                {
                    throw new HCException("Invalid sample size");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void Update(
            double dblVal,
            out double dblCheckedValue,
            out bool blnIsOutlier)
        {
            dblCheckedValue = dblVal;
            blnIsOutlier = false;

            try
            {
                var sortedItem =
                    new SortedItem
                        {
                            Index = m_intCounter++,
                            Value = dblVal
                        };

                m_sortedData.Add(
                    sortedItem,
                    null);
                m_data.Add(sortedItem);
                double dblReturn = 0;

                if (m_data.Count > 1)
                {
                    if (NoReturns)
                    {
                        dblReturn = dblVal;
                    }
                    else
                    {
                        dblReturn = dblVal - m_dblPrevValue;
                    }
                    var sortedItemReturn =
                        new SortedItem
                            {
                                Index = m_intCounter,
                                Value = dblReturn
                            };

                    m_dataListReturns.Add(sortedItemReturn);
                    m_sortedDataReturns.Add(
                        sortedItemReturn,
                        null);
                }

                m_dblPrevValue = dblVal;
                if (m_data.Count > m_intSampleSize)
                {
                    SortedItem toRemove = m_data[0];
                    m_data.RemoveAt(0);
                    m_sortedData.Remove(toRemove);


                    SortedItem toRemoveRreturn = m_dataListReturns[0];
                    m_dataListReturns.RemoveAt(0);
                    m_sortedDataReturns.Remove(toRemoveRreturn);
                }
                else
                {
                    m_blnOutlayerMode = false;
                    return;
                }

                bool blnIsOutlier1 = CheckIsOutlier(
                    dblVal,
                    m_sortedData,
                    m_dblThreshold);
                if (blnIsOutlier1 ||
                    m_blnOutlayerMode)
                {
                    bool blnIsOutlier2 = CheckIsOutlier(
                        dblReturn,
                        m_sortedDataReturns,
                        m_dblThreshold);

                    if (blnIsOutlier2)
                    {
                        dblCheckedValue = (from n in m_data
                                           select n.Value).Average();
                        m_blnOutlayerMode = true;
                        blnIsOutlier = true;
                        return;
                    }
                }
                m_blnOutlayerMode = false;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }


        private static bool CheckIsOutlier(
            double dblVal,
            SortedDictionary<SortedItem, object> sortedData,
            double dblThreshold)
        {
            try
            {
                SortedItem item = Median.GetMedian(sortedData);
                double dblMedianVal = item.Value;
                List<double> r = (from n in sortedData
                                  select Math.Abs(n.Key.Value - dblMedianVal)).ToList();
                double dblMedianR = Median.GetMedian(r);
                double dblR = Math.Abs(dblVal - dblMedianVal);
                return dblR > dblThreshold*dblMedianR; // do not use greather or equal, this braks when MedianR is zero
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        public void Dispose()
        {
            try
            {
                if (m_sortedData != null)
                {
                    m_sortedData.Clear();
                    m_sortedData = null;
                }
                if (m_sortedDataReturns != null)
                {
                    m_sortedDataReturns.Clear();
                    m_sortedDataReturns = null;
                }
                if (m_data != null)
                {
                    m_data.Clear();
                    m_data = null;
                }
                if (m_dataListReturns != null)
                {
                    m_dataListReturns.Clear();
                    m_dataListReturns = null;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
