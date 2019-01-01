#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    /**
    Base class for Histogram1D and Histogram2D.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public abstract class Histogram : IHistogram
    {
        private readonly string title_;

        public Histogram(string title)
        {
            title_ = title;
        }

        #region IHistogram Members

        public string title()
        {
            return title_;
        }


        /**
         * Number of all entries in all (both in-range and under/overflow) bins in the histogram.
         */
        public abstract int allEntries();
        /**
         * Returns 1 for one-dimensional histograms, 2 for two-dimensional histograms, and so on.
         */
        public abstract int dimensions();
        /**
         * Number of in-range entries in the histogram.
         */
        public abstract int entries();
        /** 
         * Number of equivalent entries.
         * @return <tt>SUM[ weight ] ^ 2 / SUM[ weight^2 ]</tt>.
         */
        public abstract double equivalentBinEntries();
        /**
         * Number of under and overflow entries in the histogram.
         */
        public abstract int extraEntries();
        /**
         * Reset contents; as if just constructed.
         */
        public abstract void reset();
        /**
         * Sum of all (both in-range and under/overflow) bin heights in the histogram.
         */
        public abstract double sumAllBinHeights();
        /**
         * Sum of in-range bin heights in the histogram.
         */
        public abstract double sumBinHeights();
        /**
         * Sum of under/overflow bin heights in the histogram.
         */
        public abstract double sumExtraBinHeights();

        #endregion
    }
}
