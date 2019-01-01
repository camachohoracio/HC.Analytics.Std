#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure8_ : IntDoubleProcedure
    {
        private readonly AbstractIntDoubleMap m_elements;
        private readonly int m_intColumns;
        private readonly DoubleMatrix2D m_y;

        public IntDoubleProcedure8_(
            AbstractIntDoubleMap elements,
            DoubleMatrix2D y,
            int intColumns)
        {
            m_elements = elements;
            m_y = y;
            m_intColumns = intColumns;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            int i = key/m_intColumns;
            int j = key%m_intColumns;
            double r = value*m_y.getQuick(i, j);
            if (r != value)
            {
                m_elements.put(key, r);
            }
            return true;
        }

        #endregion
    }
}
