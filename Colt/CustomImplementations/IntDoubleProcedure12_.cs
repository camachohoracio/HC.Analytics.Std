#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure12_ : IntDoubleProcedure
    {
        private readonly bool m_blnTransposeA;
        private readonly DoubleMatrix1D[] m_Brows;
        private readonly DoubleMatrix1D[] m_Crows;
        private readonly double m_dblAlpha;
        private readonly PlusMult m_fun;
        private readonly int m_intColumns;

        public IntDoubleProcedure12_(
            PlusMult fun,
            DoubleMatrix1D[] Brows,
            DoubleMatrix1D[] Crows,
            bool blnTransposeA,
            double dblAlpha,
            int intColumns)
        {
            m_fun = fun;
            m_Brows = Brows;
            m_Crows = Crows;
            m_blnTransposeA = blnTransposeA;
            m_dblAlpha = dblAlpha;
            m_intColumns = intColumns;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            int i = key/m_intColumns;
            int j = key%m_intColumns;
            m_fun.m_multiplicator = value*m_dblAlpha;
            if (!m_blnTransposeA)
            {
                m_Crows[i].assign(m_Brows[j], m_fun);
            }
            else
            {
                m_Crows[j].assign(m_Brows[i], m_fun);
            }
            return true;
        }

        #endregion
    }
}
