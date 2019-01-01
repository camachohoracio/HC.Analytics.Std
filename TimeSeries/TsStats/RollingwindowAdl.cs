using System;
using HC.Core.Exceptions;

namespace HC.Analytics.TimeSeries.TsStats
{
    /// <summary>
    /// Accumulation distribution line
    /// </summary>
    public class RollingWindowAdl : ITechnicalIndicator
    {
        private object m_lockObject = new object();

        #region Properties

        public double Indicator
        {
            get { return Adl; }
            set { throw new HCException("Not implemented");}
        }
        

        public double Adl
        {
            get;
            set;
        }

        public double LastValue { get; set; }

        public DateTime LastUpdateTime { get; set; }


        #endregion

        #region Public

        public bool IsReady()
        {
            return !double.IsNaN(Adl);
        }

        bool ITechnicalIndicator.IsReady
        {
            get { return !double.IsNaN(Adl); }
        }

        public string Name()
        {
            return GetType().Name;
        }

        public void Update(
            DateTime dateTime,
            double dblClose,
            double dblLow,
            double dblHigh,
            double dblVolume)
        {
            lock (m_lockObject)
            {
                if (LastUpdateTime > dateTime &&
                    dateTime != DateTime.MinValue)
                {
                    return;
                }

                if (double.IsNaN(dblClose) ||
                    double.IsInfinity(dblClose))
                {
                    return;
                }

                LastValue = dblClose;

                LastUpdateTime = dateTime;

                //1. Money Flow Multiplier = [(Close  -  Low) - (High - Close)] /(High - Low) 
                //2. Money Flow Volume = Money Flow Multiplier x Volume for the Period
                //3. ADL = Previous ADL + Current Period's Money Flow Volume
                double dblDiff = dblHigh - dblLow;
                double dblMoneyFlowVolume;
                if (dblDiff <= 0)
                {
                    dblMoneyFlowVolume = 0;
                }
                else
                {
                    double dblMoneyFlowMultiplier = (dblClose + dblLow) - (dblHigh - dblClose)/dblDiff;
                    dblMoneyFlowVolume = dblVolume*dblMoneyFlowMultiplier;
                }

                double dblPrevAdl = Adl;
                double dblAdl = dblPrevAdl + dblMoneyFlowVolume;

                if(double.IsNaN(dblAdl))
                {
                    return;
                }

                Adl = dblAdl;
            }
        }


        #endregion

        public override string ToString()
        {
            return typeof(RollingWindowAdl).Name;
        }

        public RollingWindowAdl Clone()
        {
            var clone = (RollingWindowAdl)MemberwiseClone();
            return clone;
        }

        public void Dispose()
        {
            m_lockObject = null;
        }
    }
}
