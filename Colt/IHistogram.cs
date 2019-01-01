namespace HC.Analytics.Colt
{
    ////package hep.aida;

    /**
    A common base interface for IHistogram1D, IHistogram2D and IHistogram3D.

    @author Pavel Binko, Dino Ferrero Merlino, Wolfgang Hoschek, Tony Johnson, Andreas Pfeiffer, and others.
    @version 1.0, 23/03/2000
    */

    public interface IHistogram
    {
        /**
         * Number of all entries in all (both in-range and under/overflow) bins in the histogram.
         */
        int allEntries();
        /**
         * Returns 1 for one-dimensional histograms, 2 for two-dimensional histograms, and so on.
         */
        int dimensions();
        /**
         * Number of in-range entries in the histogram.
         */
        int entries();
        /** 
         * Number of equivalent entries.
         * @return <tt>SUM[ weight ] ^ 2 / SUM[ weight^2 ]</tt>.
         */
        double equivalentBinEntries();
        /**
         * Number of under and overflow entries in the histogram.
         */
        int extraEntries();
        /**
         * Reset contents; as if just constructed.
         */
        void reset();
        /**
         * Sum of all (both in-range and under/overflow) bin heights in the histogram.
         */
        double sumAllBinHeights();
        /**
         * Sum of in-range bin heights in the histogram.
         */
        double sumBinHeights();
        /**
         * Sum of under/overflow bin heights in the histogram.
         */
        double sumExtraBinHeights();
        /**
         *  Title of the histogram (will be set only in the constructor).
         */
        string title();
    }
}
