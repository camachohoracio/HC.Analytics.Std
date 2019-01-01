#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntIntDoubleFunction9_ : IntIntDoubleFunction
    {
        private readonly bool m_blnTransposeA;
        private readonly int m_intYi;
        private readonly int m_intYStride;
        private readonly int m_intZi;
        private readonly int m_intZStride;
        private readonly double[] m_yElements;
        private readonly double[] m_zElements;

        public IntIntDoubleFunction9_(
            bool blnTransposeA,
            double[] yElements,
            double[] zElements,
            int intZi,
            int intZStride,
            int intYi,
            int intYStride)
        {
            m_blnTransposeA = blnTransposeA;
            m_yElements = yElements;
            m_zElements = zElements;
            m_intZi = intZi;
            m_intZStride = intZStride;
            m_intYi = intYi;
            m_intYStride = intYStride;
        }

        #region IntIntDoubleFunction Members

        public double Apply(int i, int j, double value)
        {
            if (m_blnTransposeA)
            {
                int tmp = i;
                i = j;
                j = tmp;
            }
            m_zElements[m_intZi + m_intZStride*i] += value*m_yElements[m_intYi + m_intYStride*j];
            //z.setQuick(row,z.getQuick(row) + value * y.getQuick(column));
            //PrintToScreen.WriteLine("["+i+","+j+"]-->"+value);
            return value;
        }

        #endregion
    }
}
