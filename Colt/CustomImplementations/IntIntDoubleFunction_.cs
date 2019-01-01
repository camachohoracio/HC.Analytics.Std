#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction_ : IntIntDoubleFunction
    {
        private readonly RCDoubleMatrix2D m_rCDoubleMatrix2D;

        public IntIntDoubleFunction_(
            RCDoubleMatrix2D rCDoubleMatrix2D)
        {
            m_rCDoubleMatrix2D = rCDoubleMatrix2D;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_rCDoubleMatrix2D.setQuick(i, j, value);
            return value;
        }

        #endregion
    }
}
