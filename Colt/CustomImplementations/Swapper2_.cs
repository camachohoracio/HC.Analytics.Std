#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Swapper2_ : Swapper
    {
        private readonly ObjectArrayList names_;
        private readonly ObjectArrayList values_;
        private Property property_;

        public Swapper2_(
            ObjectArrayList names,
            Property property,
            ObjectArrayList values)
        {
            names_ = names;
            property_ = property;
            values_ = values;
        }

        #region Swapper Members

        public void swap(int a, int b)
        {
            Object tmp;
            tmp = names_.get(a);
            names_.set(a, names_.get(b));
            names_.set(b, tmp);
            tmp = values_.get(a);
            values_.set(a, values_.get(b));
            values_.set(b, tmp);
        }

        #endregion
    }
}
