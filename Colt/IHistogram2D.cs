namespace HC.Analytics.Colt
{
    ////package hep.aida;

    /**
    A Java interface corresponding to the AIDA 2D Histogram.
    <p> 
    <b>Note</b> All methods that accept a bin number as an argument will
    also accept the constants Constants.OVERFLOW or Constants.UNDERFLOW as the argument, and 
    as a result give the contents of the resulting Constants.OVERFLOW or Constants.UNDERFLOW
    bin.
    @see <a href="http://wwwinfo.cern.ch/asd/lhc++/AIDA/">AIDA</a>
    @author Pavel Binko, Dino Ferrero Merlino, Wolfgang Hoschek, Tony Johnson, Andreas Pfeiffer, and others.
    @version 1.0, 23/03/2000
    */

    public interface IHistogram2D : IHistogram
    {
        /**
         * The number of entries (ie the number of times fill was called for this bin).
         * @param indexX the x bin number (0...Nx-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         * @param indexY the y bin number (0...Ny-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         */
        int binEntries(int indexX, int indexY);
        /**
         * Equivalent to <tt>projectionX().binEntries(indexX)</tt>.
         */
        int binEntriesX(int indexX);
        /**
         * Equivalent to <tt>projectionY().binEntries(indexY)</tt>.
         */
        int binEntriesY(int indexY);
        /**
         * The error on this bin.
         * @param indexX the x bin number (0...Nx-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         * @param indexY the y bin number (0...Ny-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         */
        double binError(int indexX, int indexY);
        /**
         * Total height of the corresponding bin (ie the sum of the weights in this bin).
         * @param indexX the x bin number (0...Nx-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         * @param indexY the y bin number (0...Ny-1) or Constants.OVERFLOW or Constants.UNDERFLOW.
         */
        double binHeight(int indexX, int indexY);
        /**
         * Equivalent to <tt>projectionX().binHeight(indexX)</tt>.
         */
        double binHeightX(int indexX);
        /**
         * Equivalent to <tt>projectionY().binHeight(indexY)</tt>.
         */
        double binHeightY(int indexY);
        /**
         * Fill the histogram with weight 1.
         */
        void fill(double x, double y);
        /**
         * Fill the histogram with specified weight.
         */
        void fill(double x, double y, double weight);
        /**
         *  Returns the mean of the histogram, as calculated on filling-time projected on the X axis.
         */
        double meanX();
        /**
         *  Returns the mean of the histogram, as calculated on filling-time projected on the Y axis.
         */
        double meanY();
        /** 
         * Indexes of the in-range bins containing the smallest and largest binHeight(), respectively.
         * @return <tt>{minBinX,minBinY, maxBinX,maxBinY}</tt>.
         */
        int[] minMaxBins();
        /**
         * Create a projection parallel to the X axis.
         * Equivalent to <tt>sliceX(Constants.UNDERFLOW,Constants.OVERFLOW)</tt>.
         */
        IHistogram1D projectionX();
        /**
         * Create a projection parallel to the Y axis.
         * Equivalent to <tt>sliceY(Constants.UNDERFLOW,Constants.OVERFLOW)</tt>.
         */
        IHistogram1D projectionY();
        /**
         * Returns the rms of the histogram as calculated on filling-time projected on the X axis.
         */
        double rmsX();
        /**
         * Returns the rms of the histogram as calculated on filling-time projected on the Y axis.
         */
        double rmsY();
        /**
         * Slice parallel to the Y axis at bin indexY and one bin wide.
         * Equivalent to <tt>sliceX(indexY,indexY)</tt>.
         */
        IHistogram1D sliceX(int indexY);
        /**
         * Create a slice parallel to the axis X axis, between "indexY1" and "indexY2" (inclusive).
         * The returned IHistogram1D represents an instantaneous snapshot of the
         * histogram at the time the slice was created.
         */
        IHistogram1D sliceX(int indexY1, int indexY2);
        /**
         * Slice parallel to the X axis at bin indexX and one bin wide.
         * Equivalent to <tt>sliceY(indexX,indexX)</tt>.
         */
        IHistogram1D sliceY(int indexX);
        /**
         * Create a slice parallel to the axis Y axis, between "indexX1" and "indexX2" (inclusive)
         * The returned IHistogram1D represents an instantaneous snapshot of the
         * histogram at the time the slice was created.
         */
        IHistogram1D sliceY(int indexX1, int indexX2);
        /**
         * Return the X axis.
         */
        IAxis xAxis();
        /**
         * Return the Y axis.
         */
        IAxis yAxis();
    }
}
