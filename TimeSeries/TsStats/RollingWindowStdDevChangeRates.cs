#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class RollingWindowStdDevChangeRates
    {
        #region Properties

        public double SumOfValues { get; set; }
        public double SumSqOfValues { get; set; }
        public double Denominator { get; set; }
        public double DefaultDenominator { get; set; }
        public double Numerator { get; set; }
        public double CurrCounter { get; set; }

        public double Cv
        {
            get { return StdDev / Mean; }
            set { }
        }

        public double Variance
        {
            get
            {
                try
                {
                    if (Denominator == 0)
                    {
                        return 0.0;
                    }
                    double dblVar = Numerator / Denominator;
                    if (double.IsNaN(dblVar) || double.IsInfinity(dblVar))
                    {
                        throw new HCException("Invalid variance");
                    }
                    return dblVar;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                return 0;
            }
            set { }
        }

        public double StdDev
        {
            get
            {
                try
                {
                    double dblVar = Variance;
                    if (dblVar <= 0 ||
                        double.IsNaN(dblVar) ||
                        double.IsInfinity(dblVar))
                    {
                        return 0;
                    }
                    double dblStdDev = Math.Sqrt(dblVar);
                    return dblStdDev;

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                return 0;
            }
            set { }
        }

        public double Mean
        {
            get { return SumOfValues / CurrCounter; }
            set { }
        }

        public double Volatility
        {
            get
            {
                double dblVolatility = Math.Sqrt(SumSqOfValues/Returns.Count);
                if (double.IsNaN(dblVolatility))
                {
                    throw new Exception("Invalid volatility value");
                }
                return dblVolatility;
            }
        }

        public int WindowSize { get; set; }

        public double Max
        {
            get { return Returns.Count == 0 ? 0 : Returns.Max(); }
            set { } // empty for serialization
        }

        public double Min
        {
            get { return Returns.Count == 0 ? 0 : Returns.Min(); }
            set { }// empty for serialization
        }

        #endregion

        #region Members

        public List<double> Returns { get; private set; }
        private double m_dblPrevValue;
        private bool m_blnInitialized;
        private readonly RollingWindowOutliers m_rollingWindowOutliers;

        #endregion

        #region Constructors

        public RollingWindowStdDevChangeRates(int intWindowSize)
        {
            if (intWindowSize <= 4)
            {
                throw new Exception("Invalid window size: " +
                                    intWindowSize);
            }
            m_rollingWindowOutliers = new RollingWindowOutliers(3);
            Returns = new List<double>();
            WindowSize = intWindowSize;
            m_dblPrevValue = double.NaN;
            DefaultDenominator = WindowSize * (WindowSize - 1);
        }

        #endregion

        #region Public

        public bool IsReady()
        {
            return Returns.Count >= WindowSize;
        }

        public void Update(double dblValue0)
        {
            try
            {
                if (double.IsNaN(dblValue0))
                {
                    throw new Exception("Invalid volatility value");
                }

                if(!m_blnInitialized)
                {
                    m_blnInitialized = true;
                    m_dblPrevValue = dblValue0;
                    return;
                }

                //
                // compute return
                //
                double dblChange = Math.Abs((dblValue0 - m_dblPrevValue) / m_dblPrevValue);
                m_rollingWindowOutliers.Update(new DateTime(),  dblChange);
                dblChange = Math.Max(0, m_rollingWindowOutliers.GetLastCleanSample().Fx);

                if (double.IsNaN(dblChange) || double.IsInfinity(dblChange))
                {
                    throw new Exception("Invalid return");
                }
                Returns.Add(dblChange);

                //
                // add to values
                //
                SumOfValues += dblChange;
                SumSqOfValues += dblChange*dblChange;
                CurrCounter = Returns.Count;


                if (CurrCounter > WindowSize)
                {
                    //
                    // remove old values
                    //
                    double dblOldValue = Returns[0];
                    SumOfValues -= dblOldValue;
                    SumSqOfValues -= (dblOldValue*dblOldValue);
                    Returns.RemoveAt(0);
                    CurrCounter = WindowSize;
                }


                if (CurrCounter < WindowSize)
                {
                    Denominator = CurrCounter*(CurrCounter - 1);
                }
                else
                {
                    Denominator = DefaultDenominator;
                }


                Numerator = (SumSqOfValues*CurrCounter -
                             Math.Pow(SumOfValues, 2));
                if (Numerator < 0)
                {
                    if (Math.Abs(Numerator) < 1e-5)
                    {
                        Numerator = 0;
                    }
                }


                m_dblPrevValue = dblValue0;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

    }
}
