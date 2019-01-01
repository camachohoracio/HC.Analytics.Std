#region

using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class IncrBasicRollingWindow
    {
        #region Properties

        public int WindowSize { get; private set; }
        public List<double> Data { get; private set; }
        public int NumOfValues { get; private set; }
        public double MeanValue { get; private set; }
        public double StdDevValue { get; private set; }
        public double VarValue { get; private set; }
        public double SumValue { get; private set; }
        public double SumAbsValues { get; private set; }
        public double SumValSqrd { get; private set; }
        public double MaxValue { get; private set; }
        public double MinValue { get; private set; }
        public bool IsFull { get; private set; }
        public double LastValue { get; private set; }

        #endregion

        #region Constructors

        public IncrBasicRollingWindow(int intWindowSize)
        {
            WindowSize = intWindowSize;
            Reset();
        }

        private void Reset()
        {
            Data =new List<double>();
            NumOfValues = 0;
            MeanValue = 0;
            StdDevValue = 0;
            VarValue = 0;
            SumValue = 0;
            SumAbsValues = 0;
            SumValSqrd = 0;
            MaxValue = -double.MaxValue;
            MinValue = double.MaxValue;
            IsFull = false;
        }

        #endregion


        #region Public

        public void Update(double dblValue)
        {
            if(double.IsNaN(dblValue) || double.IsInfinity(dblValue))
            {
                return;
            }
            LastValue = dblValue;
            NumOfValues++;

            Data.Add(dblValue);

            if(dblValue > MaxValue)
            {
                MaxValue = dblValue;
            }
            if (dblValue < MinValue)
            {
                MinValue = dblValue;
            }

            SumValue += dblValue;
            SumAbsValues += System.Math.Abs(dblValue);
            SumValSqrd += dblValue*dblValue;
            if(NumOfValues > WindowSize &&
                Data.Count > 0)
            {
                double dblOldValue = Data[0];
                Data.RemoveAt(0);
                NumOfValues--;
                SumValue -= dblOldValue;
                SumAbsValues -= System.Math.Abs(dblOldValue);
                SumValSqrd -= dblOldValue*dblOldValue;
                IsFull = true;
                if(dblOldValue == MaxValue)
                {
                    MaxValue = double.NaN;
                }
                if (dblOldValue == MinValue)
                {
                    MinValue = double.NaN;
                }
            }
            double dblMeanOfValSqrd = SumValSqrd/NumOfValues;
            MeanValue = SumValue/NumOfValues;
            VarValue = dblMeanOfValSqrd - (MeanValue*MeanValue);
            StdDevValue = System.Math.Sqrt(VarValue);
        }

        #endregion

        public double GetFirst()
        {
            if (Data.Count > 0)
            {
                return Data[0];
            }
            return double.NaN;
        }
    }
}

