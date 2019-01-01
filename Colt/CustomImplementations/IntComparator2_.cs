#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntComparator2_ : IntComparator
    {
        private readonly ObjectArrayList names_;
        private Property property_;

        public IntComparator2_(
            ObjectArrayList names,
            Property property)
        {
            names_ = names;
            property_ = property;
        }

        #region IntComparator Members

        public int Compare(int a, int b)
        {
            return Property.get(names_, a).CompareTo(
                Property.get(names_, b));
        }

        #endregion
    }
}
