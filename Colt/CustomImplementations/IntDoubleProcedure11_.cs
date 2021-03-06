#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure11_ : IntDoubleProcedure
    {
        private readonly bool m_blnTransposeA;
        private readonly int m_intColumns;
        private readonly int m_intYi;
        private readonly int m_intYStride;
        private readonly int m_intZi;
        private readonly int m_intZStride;
        private readonly double[] m_yElements;
        private readonly double[] m_zElements;

        public IntDoubleProcedure11_(
            bool blnTransposeA,
            double[] yElements,
            double[] zElements,
            int intZi,
            int intZStride,
            int intYi,
            int intYStride,
            int intColumns)
        {
            m_intColumns = intColumns;
            m_blnTransposeA = blnTransposeA;
            m_yElements = yElements;
            m_zElements = zElements;
            m_intZi = intZi;
            m_intZStride = intZStride;
            m_intYi = intYi;
            m_intYStride = intYStride;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            int i = key/m_intColumns;
            int j = key%m_intColumns;
            if (m_blnTransposeA)
            {
                int tmp = i;
                i = j;
                j = tmp;
            }
            m_zElements[m_intZi + m_intZStride*i] += value*
                                                     m_yElements[
                                                         m_intYi + m_intYStride*j];
            //PrintToScreen.WriteLine("["+i+","+j+"]-->"+value);
            return true;
        }

        #endregion
    }
}
