#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class IntProcedure_ : IntProcedure
    {
        private readonly int key_;
        private AbstractIntDoubleMap map_;

        public IntProcedure_(
            AbstractIntDoubleMap map,
            int key)
        {
            map_ = map;
            key_ = key;
        }

        #region IntProcedure Members

        public bool Apply(int iterKey)
        {
            return (key_ != iterKey);
        }

        #endregion
    }
}
