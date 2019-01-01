using System;

namespace HC.Analytics.TimeSeries.TsStats
{
    public class Uema : IDisposable
    {
        #region Properties

        public int Memory { get; private set; }
        public int Updates { get; private set; }
        public double Value { get; private set; }

        #endregion

        #region Constructors

        public Uema(int memory)
        {
            Value = double.NaN;
            Memory = memory;
        }

        #endregion

        #region Public

        public void Update(double dblValue)
        {
            Updates++;

            double dblUpdateParam = GetUpdateMultiplier();

            if(double.IsNaN(Value))
            {
                Value = dblValue;
            }
            else if(!double.IsNaN(dblValue))
            {
                Value =
                    dblValue*dblUpdateParam + (1 - dblUpdateParam)*Value;
            }
        }

        #endregion

        #region Private

        private double GetUpdateMultiplier()
        {
            int intMemory = (Updates + 1 < Memory)
                                ? Updates + 1
                                : Memory;
            return 2d/(1d + intMemory);
        }

        #endregion

        public void Dispose()
        {
        }
    }
}

