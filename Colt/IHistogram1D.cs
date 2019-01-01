namespace HC.Analytics.Colt
{
    ////package hep.aida;

    /**
    A Java interface corresponding to the AIDA 1D Histogram.
    <p> 
    <b>Note</b> All methods that accept a bin number as an argument will
    also accept the constants Constants.OVERFLOW or Constants.UNDERFLOW as the argument, and 
    as a result give the contents of the resulting Constants.OVERFLOW or Constants.UNDERFLOW
    bin.
    @see <a href="http://wwwinfo.cern.ch/asd/lhc++/AIDA/">AIDA</a>
    @author Pavel Binko, Dino Ferrero Merlino, Wolfgang Hoschek, Tony Johnson, Andreas Pfeiffer, and others.
    @version 1.0, 23/03/2000
    */

    public interface IHistogram1D : IHistogram
    {
        /**
         * Number of entries in the corresponding bin (ie the number of times fill was called for this bin).
         * @param index the bin number (0...N-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         */
        int binEntries(int index);
        /**
         * The error on this bin.
         * @param index the bin number (0...N-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         */
        double binError(int index);
        /**
         * Total height of the corresponding bin (ie the sum of the weights in this bin).
         * @param index the bin number (0...N-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         */
        double binHeight(int index);
        /**
         * Fill histogram with weight 1.
         */
        void fill(double x);
        /**
         * Fill histogram with specified weight.
         */
        void fill(double x, double weight);
        /**
         * Returns the mean of the whole histogram as calculated on filling-time.
         */
        double mean();
        /** 
         * Indexes of the in-range bins containing the smallest and largest binHeight(), respectively.
         * @return <tt>{minBin,maxBin}</tt>.
         */
        int[] minMaxBins();
        /**
         * Returns the rms of the whole histogram as calculated on filling-time.
         */
        double rms();

        /**
         * Returns the X Axis.
         */
        IAxis xAxis();
    }
}
