#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations.tmp
{
    [Serializable]
    public class DoubleFunctionRound : DoubleFunction
    {
        private readonly double m_dbPrecision;

        public DoubleFunctionRound(double dbPrecision)
        {
            m_dbPrecision = dbPrecision;
        }

        #region DoubleFunction Members

        public double Apply(double a)
        {
            return Math.Round(a/m_dbPrecision, 0)*m_dbPrecision;
        }

        #endregion
    }
}
