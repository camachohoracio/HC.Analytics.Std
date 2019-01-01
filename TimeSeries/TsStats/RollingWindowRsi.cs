#region

using System;
using System.Collections.Generic;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    [Serializable]
    public class RollingWindowRsi : ITechnicalIndicator
    {
        private readonly object m_lockObject = new object();

        #region Properties

        public double LastValue { get; set; }
        public List<TsRow2D> Data0 { get; set; }
        public List<TsRow2D> DiffData { get; set; }
        public double SumOfPosValues { get; set; }
        public double SumOfNegValues { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public int WindowSize { get; set; }
        public double DiffDataCounter { get; set; }
        public int ChangeSize { get; set; }

        public double Indicator { get { return Rsi; }}

        public double Rsi
        {
            get
            {
                double dblRs = ((SumOfPosValues + 1) / WindowSize)/
                            ((SumOfNegValues + 1) / WindowSize);

                var dblRsi = 100.0 - (100.0 / (1.0 + dblRs));
                return dblRsi;
            }
        }

        public bool IsReady
        {
            get { return DiffData.Count >= WindowSize; }
        }

        #endregion

        #region Constructors
        
        public RollingWindowRsi() : this(int.MaxValue, 1) { }

        public RollingWindowRsi(int intWindowSize) : this(intWindowSize, 1) { }

        public RollingWindowRsi(
            int intWindowSize,
            int intChangeSize)
        {
            ChangeSize = intChangeSize;
            if (intWindowSize < 1)
            {
                throw new Exception("Invalid window size: " +
                                    intWindowSize);
            }
            WindowSize = intWindowSize;
            Data0 = new List<TsRow2D>();
            DiffData = new List<TsRow2D>();
        }

        #endregion

        #region Public


        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            lock (m_lockObject)
            {
                if (LastUpdateTime > dateTime &&
                    dateTime != DateTime.MinValue)
                {
                    throw new Exception("Invalid time");
                }

                if (double.IsNaN(dblValue) ||
                    double.IsInfinity(dblValue))
                {
                    throw new Exception("Invalid value");
                }

                LastValue = dblValue;

                Data0.Add(new TsRow2D(dateTime, dblValue));

                if(Data0.Count < ChangeSize  + 1)
                {
                    return;
                }

                double dblPrevValue = Data0[Data0.Count - ChangeSize - 1].Fx;
                double dblDiff = dblValue - dblPrevValue;
                if (dblDiff > 0)
                {
                    SumOfPosValues++;
                }
                else if (dblDiff < 0)
                {
                    SumOfNegValues++;
                }
                DiffData.Add(new TsRow2D(dateTime, dblDiff));
                DiffDataCounter = DiffData.Count;
                
                if (DiffDataCounter > WindowSize)
                {
                    //
                    // remove old values
                    //
                    double dblOldValue = DiffData[0].Fx;
                    if (dblOldValue > 0)
                    {
                        SumOfPosValues--;
                    }
                    else if(dblOldValue < 0)
                    {
                        SumOfNegValues--;
                    }
                    Data0.RemoveAt(0);
                    DiffData.RemoveAt(0);
                    DiffDataCounter = WindowSize;
                }

                LastUpdateTime = dateTime;
            }
        }

        #endregion

        public void Update(double dblCurrFitness)
        {
            Update(DateTime.MinValue, dblCurrFitness);
        }

        public RollingWindowRsi Clone()
        {
            var clone = (RollingWindowRsi)MemberwiseClone();
            clone.Data0 = new List<TsRow2D>(Data0);
            clone.DiffData = new List<TsRow2D>(DiffData);
            return clone;
        }

        public void Dispose()
        {
            try
            {
                if (Data0 != null)
                {
                    Data0.Clear();
                    Data0 = null;
                }
                if (DiffData != null)
                {
                    DiffData.Clear();
                    DiffData = null;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }


        public string Name()
        {
            return "rWindowsRsi[" +
            WindowSize + "].Rsi";
        }

        public void Update(
            DateTime dateTime, 
            double dblClose, 
            double dblLow, 
            double dblHigh, 
            double dblVolume)
        {
            Update(
                dateTime,
                dblClose);
        }
    }
}
