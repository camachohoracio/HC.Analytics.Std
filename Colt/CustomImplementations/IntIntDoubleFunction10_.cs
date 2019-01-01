#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction10_ : IntIntDoubleFunction
    {
        private readonly bool m_blnTransposeA;
        private readonly DoubleMatrix1D[] m_Brows;
        private readonly DoubleMatrix1D[] m_Crows;
        private readonly double m_dblAlpha;
        private readonly PlusMult m_fun;


        public IntIntDoubleFunction10_(
            PlusMult fun,
            DoubleMatrix1D[] Brows,
            DoubleMatrix1D[] Crows,
            bool blnTransposeA,
            double dblAlpha)
        {
            m_fun = fun;
            m_Brows = Brows;
            m_Crows = Crows;
            m_blnTransposeA = blnTransposeA;
            m_dblAlpha = dblAlpha;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            m_fun.m_multiplicator = value*m_dblAlpha;
            if (!m_blnTransposeA)
            {
                m_Crows[i].assign(m_Brows[j], m_fun);
            }
            else
            {
                m_Crows[j].assign(m_Brows[i], m_fun);
            }
            return value;
        }

        #endregion
    }
}
