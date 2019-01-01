#region

using System;
using System.Collections;
using HC.Analytics.Colt.stat;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Comparer_ : IComparer
    {
        #region IComparer Members

        public int Compare(Object o1, Object o2)
        {
            int l1 = ((DoubleBufferStat) o1).level();
            int l2 = ((DoubleBufferStat) o2).level();
            return l1 < l2 ? -1 : l1 == l2 ? 0 : +1;
        }

        #endregion
    }
}
