#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator3_ : IntComparator
    {
        private readonly ObjectArrayList m_names;

        public IntComparator3_(ObjectArrayList names)
        {
            m_names = names;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            return Property.get(m_names, a).CompareTo(Property.get(m_names, b));
        }

        #endregion
    }
}
