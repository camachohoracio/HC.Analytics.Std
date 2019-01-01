#region

using System;
using System.Collections.Generic;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowVolatility : ITechnicalIndicator
    {
        #region Properties

        public double Volatility
        {
            get
            {
                double dblVolatility = m_returns.Count == 0 ?  
                    0 :
                    Math.Sqrt(m_dblSumSqReturns/m_returns.Count);
                if (double.IsNaN(dblVolatility))
                {
                    return 0;
                }
                return dblVolatility;
            }
        }

        public int WindowSize { get; set; }

        #endregion

        #region Members

        private List<double> m_returns;
        private double m_dblPrevValue;
        private double m_dblSumSqReturns;

        #endregion

        #region Constructors

        public RollingWindowVolatility(int intWindowSize)
        {
            m_returns = new List<double>();
            WindowSize = intWindowSize;
            m_dblPrevValue = double.NaN;
        }

        #endregion

        #region Public

        public void Update(double dblValue)
        {
            try
            {
                if (double.IsNaN(dblValue))
                {
                    dblValue = 0;
                }

                if (double.IsNaN(m_dblPrevValue))
                {
                    m_dblPrevValue = dblValue;
                    m_dblPrevValue = dblValue;
                    return;
                }
                //
                // compute return
                //
                double dblReturn = Math.Log(dblValue) - Math.Log(m_dblPrevValue);
                if (double.IsNaN(dblReturn))
                {
                    dblReturn = 0;
                }
                m_returns.Add(dblReturn);
                m_dblSumSqReturns += Math.Pow(dblReturn, 2);

                //
                // remove window values
                //
                if (m_returns.Count > WindowSize)
                {
                    double dblOldReturn = m_returns[0];
                    m_returns.RemoveAt(0);
                    double dblOldSumSqReturn = Math.Pow(dblOldReturn, 2);
                    m_dblSumSqReturns -= dblOldSumSqReturn;

                    if (m_dblSumSqReturns < 0)
                    {
                        if (Math.Abs(m_dblSumSqReturns) > 1e-10)
                        {
                            throw new Exception("Invalid sumSq return");
                        }
                        m_dblSumSqReturns = 0;
                    }
                }

                m_dblPrevValue = dblValue;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        public void Dispose()
        {
            if (m_returns != null)
            {
                m_returns.Clear();
                m_returns = null;
            }
        }

        public double Indicator { get { return Volatility; }}

        public bool IsReady { get { return m_returns.Count >= WindowSize; }}

        public string Name()
        {
            return GetType().Name + "[" +
            WindowSize + "]";
        }

        public void Update(
            DateTime dateTime, 
            double dblClose, 
            double dblLow, 
            double dblHigh, 
            double dblVolume)
        {
            Update(dblClose);
        }
    }
}
