#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntDoubleProcedure10_ : IntDoubleProcedure
    {
        private readonly AbstractIntDoubleMap m_elements;
        private readonly IntIntDoubleFunction m_function;
        private readonly int m_intColumns;

        public IntDoubleProcedure10_(
            AbstractIntDoubleMap elements,
            IntIntDoubleFunction function,
            int intColumns)
        {
            m_elements = elements;
            m_function = function;
            m_intColumns = intColumns;
        }

        #region IntDoubleProcedure Members

        public bool Apply(int key, double value)
        {
            int i = key/m_intColumns;
            int j = key%m_intColumns;
            double r = m_function.Apply(i, j, value);
            if (r != value)
            {
                m_elements.put(key, r);
            }
            return true;
        }

        #endregion
    }
}
