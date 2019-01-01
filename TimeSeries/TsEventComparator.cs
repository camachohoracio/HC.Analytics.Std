#region

using System.Collections.Generic;

#endregion

namespace HC.Analytics.TimeSeries
{
    public class TsEventPublisherComparator : IComparer<TsEventPublisher>
    {
        #region Members

        private readonly TsEventComparator m_tsEventComparator;

        #endregion

        #region Constructors

        public TsEventPublisherComparator()
        {
            m_tsEventComparator = new TsEventComparator();
        }

        #endregion

        #region IComparer<ITsEvent> Members

        public int Compare(TsEventPublisher x, TsEventPublisher y)
        {
            return m_tsEventComparator.Compare(x.TsEvent, y.TsEvent);
        }

        #endregion
    }
}
