#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper4_ : Swapper
    {
        private readonly ObjectArrayList m_names;
        private readonly ObjectArrayList m_values;

        public Swapper4_(ObjectArrayList names,
                         ObjectArrayList values)
        {
            m_names = names;
            m_values = values;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            Object tmp;
            tmp = m_names.get(a);
            m_names.set(a, m_names.get(b));
            m_names.set(b, tmp);
            tmp = m_values.get(a);
            m_values.set(a, m_values.get(b));
            m_values.set(b, tmp);
        }

        #endregion
    }
}
