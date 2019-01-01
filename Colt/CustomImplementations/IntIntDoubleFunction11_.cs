#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction11_ : IntIntDoubleFunction
    {
        private readonly double m_dblAlpha;
        private readonly RCDoubleMatrix2D m_rCDoubleMatrix2D;


        public IntIntDoubleFunction11_(
            RCDoubleMatrix2D rCDoubleMatrix2D,
            double dblAlpha)
        {
            m_dblAlpha = dblAlpha;
            m_rCDoubleMatrix2D = rCDoubleMatrix2D;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_rCDoubleMatrix2D.setQuick(i, j,
                                        m_rCDoubleMatrix2D.getQuick(i, j) +
                                        m_dblAlpha*value);
            return value;
        }

        #endregion
    }
}
