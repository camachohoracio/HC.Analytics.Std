#region

using System;
using System.Collections.Generic;
using HC.Core.DynamicCompilation;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries
{
    public class TsEventComparator : IComparer<ITsEvent>
    {
        #region IComparer<ITsEvent> Members

        public int Compare(ITsEvent x, ITsEvent y)
        {
            try
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                return x.Time.CompareTo(y.Time);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        #endregion
    }
}
