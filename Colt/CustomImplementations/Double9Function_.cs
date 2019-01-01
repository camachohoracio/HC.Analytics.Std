#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Double9Function_ : Double9Function
    {
        private readonly double m_dblAlpha;
        private readonly double m_dblBeta;

        public Double9Function_(
            double dblAlpha,
            double dblBeta)
        {
            m_dblAlpha = dblAlpha;
            m_dblBeta = dblBeta;
        }

        #region Double9Function Members

        public double Apply(double a00, double a01, double a02, double a10, double a11, double a12, double a20,
                            double a21, double a22)
        {
            return m_dblAlpha*a11 + m_dblBeta*(a01 + a10 + a12 + a21);
        }

        #endregion
    }
}
